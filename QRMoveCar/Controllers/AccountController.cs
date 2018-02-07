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
using Tools.Models;
using Tools.Response;
using Tools.Response.Json;
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
        public IActionResult GetAccountID(string uniacid, string code, string iv, string encryptedData)
        {
            try
            {
                JsonResponse1<AccountModel> responseModel = new JsonResponse1<AccountModel>();

                //WXSmallAppCommon.Models.WXAccountInfo wXAccount = WXSmallAppCommon.WXInteractions.WXLoginAction.ProcessRequest(code, iv, encryptedData);
                ///微擎方式
                WXSmallAppCommon.Models.WXAccountInfo wXAccount = We7Tools.We7Tools.GetWeChatUserInfo(uniacid, code, iv, encryptedData);
                var accountCard = thisData.SaveOrdUpdateAccount(uniacid, wXAccount);
                ResponseStatus stautsCode = ResponseStatus.请求失败;
                if (accountCard != null)
                {
                    responseModel.JsonData = accountCard;
                    stautsCode = ResponseStatus.请求成功;
                }
                responseModel.StatusCode = stautsCode;
                string[] param = new string[] { "JsonData", "AccountID", "HasCarNumber", "HasPhone" };
                return this.JsonSuccessWithLimit(responseModel,param);
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
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
        public IActionResult SaveCarInfo(string uniacid, string accountID, string carNumber, string accountPhone, string verifyCode)
        {

            try
            {
                thisData.CheckVerify(uniacid, accountPhone, verifyCode);
                thisData.SaveCarInfo(uniacid, new ObjectId(accountID), carNumber, accountPhone);
                return this.JsonSuccessStatus();
            }
            catch (ExceptionModel em)
            {
                return this.JsonOtherStatus(em.ExceptionParam);
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }

        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public IActionResult GetCarInfo(string uniacid, string accountID)
        {

            try
            {
                var account = thisData.GetAccountInfo(uniacid, new ObjectId(accountID));
                var responseModel = new JsonResponse1<AccountModel>() { JsonData = account };
                string[] param = new string[] { "JsonData", "AccountID", "CarNumber", "AccountPhoneNumber" };
                return this.JsonSuccessWithLimit(responseModel,param);
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
              
            }
        }

        /// <summary>
        /// 申请邮寄
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public IActionResult CreateOrder(string uniacid, string accountID)
        {
            try
            {
                string json = new StreamReader(Request.Body).ReadToEnd();
                Order order = JsonConvert.DeserializeObject<Order>(json);
                order.OrderID = ObjectId.GenerateNewId();
                WXPayModel wpm = thisData.CreateOrder(uniacid, new ObjectId(accountID), order);
                return new JsonResponse2<string, WXPayModel>() { JsonData = order.OrderID.ToString(), JsonData1 = wpm }.ToJsonResult(this);
            }
            catch (Exception e)
            {
                e.Save();

                return this.JsonErrorStatus();
            }

        }

        /// <summary>
        /// 获取邮寄记录
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public IActionResult GetOrderList(string uniacid, string accountID)
        {
            try
            {
                List<Order> orders = thisData.GetOrderList(uniacid, new ObjectId(accountID));
                if (orders == null || orders.Count == 0)
                {
                    return this.JsonSuccessStatus();
                }
                return new JsonResponse1<List<Order>>() { JsonData = orders }.ToJsonResult(this);
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <returns></returns>
        public IActionResult GetAccountInfo(string uniacid, string accountID)
        {
            try
            {
                return new JsonResponse1<AccountModel>() { JsonData = thisData.GetAccountInfo(uniacid, new ObjectId(accountID))}.ToJsonResult(this);
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
              
            }
        }

        /// <summary>
        /// 呼叫账户
        /// </summary>
        /// <param name="uniacid">商户识别ID</param>
        /// <param name="currentAccountID">呼叫者账户ID</param>
        /// <param name="anotherAccountID">被呼叫者账户ID</param>
        /// <returns></returns>
        public IActionResult CallAccount(string uniacid, string currentAccountID, string anotherAccountID)
        {
            try
            {
                thisData.CallAccount(uniacid, new ObjectId(currentAccountID), new ObjectId(anotherAccountID));
                return this.JsonSuccessStatus();
            }
            catch (ExceptionModel em)
            {
                return this.JsonOtherStatus(em.ExceptionParam);
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
            }
        }


        /// <summary>
        /// 设置用户手机号码
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="accountID">用户ID</param>
        /// <param name="phoneNumber">手机号码</param>
        /// <returns></returns>
        public IActionResult SetPhoneNumber(string uniacid, string accountID, string phoneNumber)
        {
            try
            {
                thisData.SetPhoneNumber(uniacid, new ObjectId(accountID), phoneNumber);
                return this.JsonSuccessStatus();
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
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
        public IActionResult GetQRMoney(string uniacid)
        {
            try
            {
                var money = thisData.GetQRMoney(uniacid);
                return new JsonResponse1<decimal>() { JsonData = money }.ToJsonResult(this);
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
              
            }
        }
    }
}
