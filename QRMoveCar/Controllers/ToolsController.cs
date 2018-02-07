using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRMoveCar.Models;
using Tools.DB;
using Tools.Models;
using Tools.Response;
using Tools.Response.Json;
using YTXDAL;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QRMoveCar.Controllers
{
    public class ToolsController : Controller
    {
        private IHostingEnvironment hostingEnvironment;
        public ToolsController(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        private static List<CarNumberModel> carNumberList;

        public const int VerifyTimeOut = 10;
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <param name="phoneNumber">手机号码</param>
        /// <returns></returns>
        public IActionResult GetSmsVerifyCode(string uniacid, string phoneNumber)
        {

            var filter = Builders<VerifyCodeModel>.Filter;
            var mongo = new MongoDBTool();

            var company = mongo.GetMongoCollection<CompanyModel>().Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            if (company == null || company.YTX == null)
            {
                var em = new ExceptionModel()
                {
                    Content = "商户未设置云通信信息"
                };
                em.Save();
                return this.JsonErrorStatus();
            }

            var collection = mongo.GetMongoCollection<VerifyCodeModel>();
            var date = DateTime.Now.AddMinutes(VerifyTimeOut);
            collection.DeleteMany(filter.Lt(x => x.CreateTime, date) & filter.Eq(x => x.uniacid, uniacid));
            var filterSum = filter.Eq(x => x.PhoneNumber, phoneNumber) & filter.Gt(x => x.CreateTime, date);
            VerifyCodeModel vcm = collection.FindOneAndDelete(filterSum);
            if (vcm == null)
            {
                vcm = new VerifyCodeModel()
                {
                    PhoneNumber = phoneNumber,
                    CreateTime = DateTime.Now,
                    uniacid = uniacid
                };
            }
            string jsonData = JsonConvert.SerializeObject(new SMSModel()
            {
                appid = company.YTX.AppID,
                mobile = vcm.PhoneNumber,
                datas = new string[] { vcm.VerifyCode, VerifyTimeOut + "" }
            });
            string url = "/sms/TemplateSMS.wx";

            string result = new PhoneHelper(company.YTX).SendRequest(url, jsonData);
            var jObj = (JObject)JsonConvert.DeserializeObject(result);
            JToken statuscode;
            bool hasVal = jObj.TryGetValue("statusCode", out statuscode);
            if (hasVal && statuscode.ToString().Equals("0"))
            {
                collection.InsertOne(vcm);
                return this.JsonErrorStatus();
            }
            return this.JsonErrorStatus();
        }

        /// <summary>
        /// 反馈
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <returns></returns>
        public IActionResult PushFeedback()
        {
            try
            {
                string json = new StreamReader(Request.Body).ReadToEnd();
                FeedbackModel fm = JsonConvert.DeserializeObject<FeedbackModel>(json);
                fm.CreateTime = DateTime.Now;
                new MongoDBTool().GetMongoCollection<FeedbackModel>().InsertOne(fm);
                return this.JsonErrorStatus();
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
                throw;
            }

        }

        /// <summary>
        /// 获取车牌省简称
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetProvinceSimpleName()
        {
            try
            {
                await InitCarNumberModel();
                string[] param = new string[] { "JsonData", "Name" };
                return this.JsonSuccessWithLimit(new JsonResponse1<List<CarNumberModel>>() { JsonData = carNumberList }, param);
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
                throw;
            }
        }
        /// <summary>
        /// 获取车牌号字母
        /// </summary>
        /// <param name="provinceSimpleName">车牌省简称</param>
        /// <returns></returns>
        public async Task<IActionResult> GetCityCode(string provinceSimpleName)
        {
            try
            {
                await InitCarNumberModel();
                string[] param = new string[] { "StatusCode", "JsonData", "Code" };
                var codeList = carNumberList.Find(x => x.Name.Equals(provinceSimpleName)).Codes;
             return   this.JsonSuccessWithLimit(new JsonResponse1<List<CarNumberCodeModel>>() { JsonData = codeList }, param);
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
                throw;
            }
        }

        private Task InitCarNumberModel()
        {
            return Task.Run(() =>
            {
                if (carNumberList == null || carNumberList.Count == 0)
                {
                    string newJsonPath = $@"{hostingEnvironment.ContentRootPath}/wwwroot/account/car_number_new.json";
                    string oldJson = System.IO.File.ReadAllText(newJsonPath);
                    carNumberList = JsonConvert.DeserializeObject<List<CarNumberModel>>(oldJson);
                }
            });
        }
    }
}
