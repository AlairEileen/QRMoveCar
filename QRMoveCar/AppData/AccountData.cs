using ConfigData;
using ImageDAL;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRDAL;
using QRMoveCar.Controllers;
using QRMoveCar.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Tools.Models;
using We7Tools;
using WXSmallAppCommon.Models;
using WXSmallAppCommon.WXInteractions;
using YTXDAL;

namespace QRMoveCar.AppData
{
    public class AccountData : BaseData<AccountModel>
    {

        /// <summary>
        /// 调取微信用户，更新或者保存本地用户
        /// </summary>
        /// <param name="wXAccount">微信用户</param>
        /// <returns></returns>
        internal AccountModel SaveOrdUpdateAccount(string uniacid, WXAccountInfo wXAccount)
        {
            Console.WriteLine("在SaveOrdUpdateAccount");
            AccountModel accountCard = null;
            if (wXAccount.OpenId != null)
            {
                var filter = Builders<AccountModel>.Filter.Eq(x => x.OpenID, wXAccount.OpenId) &
                   Builders<AccountModel>.Filter.Eq(x => x.uniacid, uniacid);
                var update = Builders<AccountModel>.Update.Set(x => x.LastChangeTime, DateTime.Now);
                accountCard = collection.FindOneAndUpdate<AccountModel>(filter, update);
                Console.WriteLine($"在SaveOrdUpdateAccount{accountCard == null}");

                if (accountCard == null)
                {
                    //string avatarUrl = DownloadAvatar(wXAccount.AvatarUrl, wXAccount.OpenId);
                    string avatarUrl = wXAccount.AvatarUrl;
                    accountCard = new AccountModel() { uniacid = uniacid, OpenID = wXAccount.OpenId, AccountName = wXAccount.NickName, Gender = wXAccount.GetGender, AccountAvatar = avatarUrl, CreateTime = DateTime.Now, LastChangeTime = DateTime.Now };
                    collection.InsertOne(accountCard);
                }
            }
            return accountCard;
        }

        internal void SaveCarInfo(string uniacid, ObjectId accountID, string carNumber, string accountPhone)
        {
            if (string.IsNullOrEmpty(carNumber) || string.IsNullOrEmpty(accountPhone))
            {
                var em = new ExceptionModel()
                {
                    Content = "参数为空"
                };
                em.Save();
                throw em;
            }
            var filter = Builders<AccountModel>.Filter;
            var filterSum = filter.Eq(x => x.uniacid, uniacid) & filter.Eq(x => x.AccountID, accountID);
            var update = Builders<AccountModel>.Update.Set(x => x.CarNumber, carNumber).Set(x => x.AccountPhoneNumber, accountPhone);
            collection.UpdateOne(filterSum, update);
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="uniacid"></param>
        /// <param name="accountID"></param>
        /// <returns></returns>
        internal AccountModel GetAccountInfo(string uniacid, ObjectId accountID)
        {
            return GetModelByIDAndUniacID(accountID, uniacid);
        }



        internal void CheckVerify(string uniacid, string accountPhone, string verifyCode)
        {
            var filter = Builders<VerifyCodeModel>.Filter;
            var vcCollection = mongo.GetMongoCollection<VerifyCodeModel>();
            var date = DateTime.Now.AddMinutes(-ToolsController.VerifyTimeOut);
            vcCollection.DeleteMany(filter.Lt(x => x.CreateTime, date) & filter.Eq(x => x.uniacid, uniacid));
            var vc = vcCollection.FindOneAndDelete(filter.Eq(x => x.uniacid, uniacid) & filter.Eq(x => x.PhoneNumber, accountPhone) & filter.Eq(x => x.VerifyCode, verifyCode));
            if (vc == null)
            {
                var em = new ExceptionModel()
                {
                    Content = "验证码错误",
                    ExceptionParam =Tools.Response.ResponseStatus.验证失败
                };
                em.Save();
                throw em;
            }
        }

        internal WXPayModel CreateOrder(string uniacid, ObjectId accountID, Order order)
        {
            order.CreateTime = DateTime.Now;
            order.Total = GetQRMoney(uniacid) * order.GoodsNumber;
            return GetCreatePacketsPayParams(uniacid, GetModelByIDAndUniacID(accountID, uniacid), order);
        }

        internal decimal GetQRMoney(string uniacid)
        {
            var comapny = mongo.GetMongoCollection<CompanyModel>().Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            return comapny.QRSendFee;
        }

        private WXPayModel GetCreatePacketsPayParams(string uniacid, AccountModel account, Order order)
        {

            if (account.Orders == null)
            {
                collection.UpdateOne(x => x.AccountID.Equals(account.AccountID),
                    Builders<AccountModel>.Update.Set(x => x.Orders, new List<Order>()));
            }
            collection.UpdateOne(x => x.AccountID.Equals(account.AccountID),
                    Builders<AccountModel>.Update.Push(x => x.Orders, order));
            ///微擎相关
            JsApiPay jsApiPay = new JsApiPay();
            jsApiPay.openid = account.OpenID;
            jsApiPay.total_fee = order.Total.ConvertToMoneyCent();
            var body = "二维码邮寄";
            var attach = account.AccountID + "," + order.OrderID.ToString();
            var goods_tag = "二维码邮寄";
            jsApiPay.CreateWeChatOrder(uniacid, body, attach, goods_tag);
            var param = jsApiPay.GetJsApiParameters(We7Tools.Models.We7ProcessMiniConfig.GetAllConfig(uniacid).KEY);
            var wxpm = JsonConvert.DeserializeObject<WXPayModel>(param);
            return wxpm;
        }



        internal List<Order> GetOrderList(string uniacid, ObjectId accountID)
        {
            return GetModelByIDAndUniacID(accountID, uniacid).Orders.FindAll(x => x.IsPaid);
        }

        internal void CallAccount(string uniacid, ObjectId currentAccountID, ObjectId anotherAccountID)
        {
            var currentAccount = GetModelByIDAndUniacID(currentAccountID, uniacid);
            var anotherAccount = GetModelByIDAndUniacID(anotherAccountID, uniacid);
            CheckCallNull(currentAccount, anotherAccount);
            var company = mongo.GetMongoCollection<CompanyModel>().Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            if (company == null || company.YTX == null)
            {
                var em = new ExceptionModel()
                {
                    Content = "商户未设置云通信信息",
                    ExceptionParam = Tools.Response.ResponseStatus.请求失败
                };
                em.Save();
                throw em;
            }
            string jsonData = JsonConvert.SerializeObject(new TwoPhoneModel()
            {
                appid = company.YTX.AppID,
                src = currentAccount.AccountPhoneNumber,
                dst = anotherAccount.AccountPhoneNumber
            });
            string url = "/call/DailbackCall.wx";

            string result = new PhoneHelper(company.YTX).SendRequest(url, jsonData);
            var jObj = (JObject)JsonConvert.DeserializeObject(result);
            JToken statuscode;
            bool hasVal = jObj.TryGetValue("statusCode", out statuscode);
            if (!hasVal || !statuscode.ToString().Equals("0"))
            {
                var em = new ExceptionModel()
                {
                    Content = "呼叫不成功！",
                    ExceptionParam = Tools.Response.ResponseStatus.请求失败
                };
                em.Save();
                throw em;
            }
        }

        private void CheckCallNull(AccountModel currentAccount, AccountModel anotherAccount)
        {
            if (string.IsNullOrEmpty(currentAccount.AccountPhoneNumber))
            {
                var em = new ExceptionModel()
                {
                    Content = "呼叫者电话为空",
                    ExceptionParam = Tools.Response.ResponseStatus.呼叫者手机号为空
                };
                em.Save();
                throw em;
            }
            if (string.IsNullOrEmpty(anotherAccount.AccountPhoneNumber))
            {
                var em = new ExceptionModel()
                {
                    Content = "被呼叫者电话为空",
                    ExceptionParam = Tools.Response.ResponseStatus.被呼叫者手机号为空
                };
                em.Save();
                throw em;
            }
        }

        internal void SetPhoneNumber(string uniacid, ObjectId accountID, string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                var em = new ExceptionModel() { ExceptionParam = Tools.Response.ResponseStatus.请求参数不正确 };
                em.Save();
                throw em;
            }
            collection.UpdateOne(x => x.uniacid.Equals(uniacid) && x.AccountID.Equals(accountID), Builders<AccountModel>.Update.Set(x => x.AccountPhoneNumber, phoneNumber));
        }

        internal async Task<byte[]> GetQRPic(string uniacid, ObjectId accountID, string contentRootPath)
        {
            var account = GetModelByIDAndUniacID(accountID, uniacid);
            if (string.IsNullOrEmpty(account.AccountPhoneNumber) || string.IsNullOrEmpty(account.CarNumber))
            {
                string no_qrPath = $@"{contentRootPath}/wwwroot/images/no_qr.jpg";
                return File.ReadAllBytes(no_qrPath);
            }
            byte[] qrData;
            var bucket = new GridFSBucket(mongoDB);
            if (account.QRFileID != ObjectId.Empty)
            {
                return await bucket.DownloadAsBytesAsync(account.QRFileID);
            }
            string qrInfo = $@"{We7Config.SiteRoot}account/GetAccountInfo?uniacid={uniacid}&AccountID={account.AccountID}";
            Bitmap qr = QRCodeHelper.Create(qrInfo, 300);
            Image qrRaw = ImageTools.ResizeImage(qr, 286, 286, 0);
            string bgPath = $@"{contentRootPath}/wwwroot/images/qr_bg.jpg";
            Bitmap qrBit = ImageTools.CombinImage(new Bitmap(bgPath), qrRaw, 171, 128);
            MemoryStream ms = new MemoryStream();
            qrBit.Save(ms, ImageFormat.Jpeg);
            qrData = ms.GetBuffer();
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument {
                    {"content-type","Image/jpg" }
                }
            };
            var id = await bucket.UploadFromBytesAsync($"qr_{accountID.ToString()}.jpg", qrData, options);
            collection.UpdateOne(x => x.AccountID.Equals(accountID) && x.uniacid.Equals(uniacid), Builders<AccountModel>.Update.Set(x => x.QRFileID, id));
            return qrData;
        }


    }
}
