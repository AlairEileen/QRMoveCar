using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using QRMoveCar.AppData;
using QRMoveCar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Tools.Json;
using Tools.Models;
using Tools.Response;
using Tools.ResponseModels;
using WXSmallAppCommon.Models;

namespace QRMoveCar.Controllers
{
    public class AccountController : BaseController<AccountData, AccountModel>
    {
        private IHostingEnvironment hostingEnvironment;
        public AccountController(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        /// <summary>
        /// 请求登录
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="code"></param>
        /// <param name="iv"></param>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        [HttpGet]
        public string GetAccountID(string uniacid, string code, string iv, string encryptedData)
        {
            try
            {
                BaseResponseModel<AccountModel> responseModel = new BaseResponseModel<AccountModel>();

                //WXSmallAppCommon.Models.WXAccountInfo wXAccount = WXSmallAppCommon.WXInteractions.WXLoginAction.ProcessRequest(code, iv, encryptedData);
                ///微擎方式
                WXSmallAppCommon.Models.WXAccountInfo wXAccount = We7Tools.We7Tools.GetWeChatUserInfo(uniacid, code, iv, encryptedData);
                var accountCard = thisData.SaveOrdUpdateAccount(uniacid, wXAccount);
                ActionParams stautsCode = ActionParams.code_error;
                if (accountCard != null)
                {
                    responseModel.JsonData = accountCard;
                    stautsCode = ActionParams.code_ok;
                }
                responseModel.StatusCode = stautsCode;
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                string[] param = new string[] { "StatusCode", "JsonData", "AccountID", "HasCarNumber", "HasPhone" };
                jsonSerializerSettings.ContractResolver = new LimitPropsContractResolver(param);
                string jsonString = JsonConvert.SerializeObject(responseModel, jsonSerializerSettings);
                return jsonString;
            }
            catch (Exception e)
            {
                e.Save();
                return JsonResponseModel.ErrorJson;
                throw;
            }
        }

        /// <summary>
        /// 保存车辆信息
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <param name="carNumber">车牌号</param>
        /// <param name="accountPhone">手机号码</param>
        /// <param name="verifyCode">验证码</param>
        /// <returns></returns>
        public string SaveCarInfo(string uniacid, string accountID, string carNumber, string accountPhone, string verifyCode)
        {

            try
            {
                thisData.CheckVerify(uniacid, accountPhone, verifyCode);
                thisData.SaveCarInfo(uniacid, new ObjectId(accountID), carNumber, accountPhone);
                return JsonResponseModel.SuccessJson;
            }
            catch (ExceptionModel em)
            {
                return JsonResponseModel.OtherJson(em.ExceptionParam);
            }
            catch (Exception e)
            {
                e.Save();
                return JsonResponseModel.ErrorJson;
            }
        }

        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public string GetCarInfo(string uniacid, string accountID)
        {

            try
            {
                var account = thisData.GetAccountInfo(uniacid, new ObjectId(accountID));
                var responseModel = new BaseResponseModel<AccountModel>() { StatusCode = ActionParams.code_ok, JsonData = account };
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                string[] param = new string[] { "StatusCode", "JsonData", "AccountID", "CarNumber", "AccountPhoneNumber" };
                jsonSerializerSettings.ContractResolver = new LimitPropsContractResolver(param);
                string jsonString = JsonConvert.SerializeObject(responseModel, jsonSerializerSettings);
                return jsonString;
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
                throw;
            }
        }

        /// <summary>
        /// 申请邮寄
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public string CreateOrder(string uniacid, string accountID)
        {
            try
            {
                string json = new StreamReader(Request.Body).ReadToEnd();
                Order order = JsonConvert.DeserializeObject<Order>(json);
                order.OrderID = ObjectId.GenerateNewId();
                WXPayModel wpm = thisData.CreateOrder(uniacid, new ObjectId(accountID), order);
                return new BaseResponseModel2<string, WXPayModel>() { StatusCode = ActionParams.code_ok, JsonData = order.OrderID.ToString(), JsonData1 = wpm }.ToJson();
            }
            catch (Exception e)
            {
                e.Save();

                return JsonResponseModel.ErrorJson;
            }

        }

        /// <summary>
        /// 获取邮寄记录
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public string GetOrderList(string uniacid, string accountID)
        {
            try
            {
                List<Order> orders = thisData.GetOrderList(uniacid, new ObjectId(accountID));
                if (orders == null || orders.Count == 0)
                {
                    return JsonResponseModel.SuccessJson;
                }
                return new BaseResponseModel<List<Order>>() { StatusCode = ActionParams.code_ok, JsonData = orders }.ToJson();
            }
            catch (Exception e)
            {
                e.Save();
                return JsonResponseModel.ErrorJson;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public string GetAccountInfo(string uniacid, string accountID)
        {
            try
            {
                return new BaseResponseModel<AccountModel>() { JsonData = thisData.GetAccountInfo(uniacid, new ObjectId(accountID)), StatusCode = ActionParams.code_ok }.ToJson();
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
                throw;
            }
        }

        /// <summary>
        /// 呼叫账户
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="currentAccountID">呼叫者账户ID</param>
        /// <param name="anotherAccountID">被呼叫者账户ID</param>
        /// <returns></returns>
        public string CallAccount(string uniacid, string currentAccountID, string anotherAccountID)
        {
            try
            {
                thisData.CallAccount(uniacid, new ObjectId(currentAccountID), new ObjectId(anotherAccountID));
                return JsonResponseModel.SuccessJson;
            }
            catch (ExceptionModel em)
            {
                return JsonResponseModel.OtherJson(em.ExceptionParam);
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
            }
        }


        /// <summary>
        /// 设置用户手机号码
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <param name="phoneNumber">手机号码</param>
        /// <returns></returns>
        public string SetPhoneNumber(string uniacid, string accountID, string phoneNumber)
        {
            try
            {
                thisData.SetPhoneNumber(uniacid, new ObjectId(accountID), phoneNumber);
                return JsonResponseModel.SuccessJson;
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
            }
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public async Task<IActionResult> GetQRPic(string uniacid, string accountID)
        {
            byte[] data = await thisData.GetQRPic(uniacid, new ObjectId(accountID), hostingEnvironment.ContentRootPath);

            return File(data, "Image/jpg", $"{accountID}.jpg");
        }
        /// <summary>
        /// 获取邮寄费用
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <returns></returns>
        public string GetQRMoney(string uniacid)
        {
            try
            {
                var money = thisData.GetQRMoney(uniacid);
                return new BaseResponseModel<decimal>() { StatusCode = ActionParams.code_ok, JsonData = money }.ToJson();
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
                throw;
            }
        }
    }
}
