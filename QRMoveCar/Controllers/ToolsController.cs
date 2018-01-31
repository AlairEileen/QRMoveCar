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
using Tools.Json;
using Tools.Models;
using Tools.Response;
using Tools.ResponseModels;
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
        public string GetSmsVerifyCode(string uniacid, string phoneNumber)
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
                return JsonResponseModel.ErrorJson;
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
                return JsonResponseModel.SuccessJson;
            }
            return JsonResponseModel.ErrorJson;
        }

        /// <summary>
        /// 反馈
        /// </summary>
        /// <param name="uniacid">商户ID</param>
        /// <returns></returns>
        public string PushFeedback()
        {
            try
            {
                string json = new StreamReader(Request.Body).ReadToEnd();
                FeedbackModel fm = JsonConvert.DeserializeObject<FeedbackModel>(json);
                fm.CreateTime = DateTime.Now;
                new MongoDBTool().GetMongoCollection<FeedbackModel>().InsertOne(fm);
                return JsonResponseModel.SuccessJson;
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
                throw;
            }

        }

        /// <summary>
        /// 获取车牌省简称
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetProvinceSimpleName()
        {
            try
            {
                await InitCarNumberModel();
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                string[] param = new string[] { "StatusCode", "JsonData", "Name" };
                jsonSerializerSettings.ContractResolver = new LimitPropsContractResolver(param);
                string jsonString = JsonConvert.SerializeObject(new BaseResponseModel<List<CarNumberModel>>() { StatusCode = Tools.ActionParams.code_ok, JsonData = carNumberList }, jsonSerializerSettings);
                return jsonString;
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
                throw;
            }
        }
        /// <summary>
        /// 获取车牌号字母
        /// </summary>
        /// <param name="provinceSimpleName">车牌省简称</param>
        /// <returns></returns>
        public async Task<string> GetCityCode(string provinceSimpleName)
        {
            try
            {
                await InitCarNumberModel();
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                string[] param = new string[] { "StatusCode", "JsonData", "Code" };
                jsonSerializerSettings.ContractResolver = new LimitPropsContractResolver(param);
                var codeList = carNumberList.Find(x=>x.Name.Equals(provinceSimpleName)).Codes;
                string jsonString = JsonConvert.SerializeObject(new BaseResponseModel<List<CarNumberCodeModel>>() { StatusCode = Tools.ActionParams.code_ok, JsonData = codeList }, jsonSerializerSettings);
                return jsonString;
            }
            catch (Exception)
            {
                return JsonResponseModel.ErrorJson;
                throw;
            }
        }

            private Task InitCarNumberModel()
        {
            return Task.Run(() =>
            {
                if (carNumberList==null||carNumberList.Count==0)
                {
                    string newJsonPath = $@"{hostingEnvironment.ContentRootPath}/wwwroot/account/car_number_new.json";
                    string oldJson = System.IO.File.ReadAllText(newJsonPath);
                    carNumberList = JsonConvert.DeserializeObject<List<CarNumberModel>>(oldJson);
                }
            });
        }
    }
}
