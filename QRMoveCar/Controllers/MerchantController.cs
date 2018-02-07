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
using Tools.Models;
using Tools.Response;
using Tools.Response.Json;
using We7Tools;
using We7Tools.Extend;
using YTXDAL;

namespace QRMoveCar.Controllers
{
    public class MerchantController : BaseController<MerchantData, CompanyModel>
    {
        private IHostingEnvironment hostingEnvironment;

        public MerchantController(IHostingEnvironment environment) : base(true)
        {
            hostingEnvironment = environment;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            List<OrderViewModel> thList = GetOrderViewList();
            return View(thList);
        }

        private List<OrderViewModel> GetOrderViewList()
        {
            return thisData.GetOrderViewList(HttpContext.Session.GetUniacID());
        }

        public IActionResult Settings()
        {
            var companyModel = thisData.GetCompanyModel(HttpContext.Session.GetUniacID());
            if (companyModel == null)
            {
                companyModel = new CompanyModel() { ProcessMiniInfo = new ProcessMiniInfo(), QiNiuModel = new QiNiuModel(), YTX = new YTXModel() };
            }
            return View(new ManageViewModel()
            {
                ProcessMiniInfo = companyModel.ProcessMiniInfo,
                YTX = (companyModel.YTX == null ? new YTXModel() : companyModel.YTX),
                QRSendFee = companyModel.QRSendFee,
                uniacid = companyModel.uniacid,
                HasWeChatQRverifyFileName = companyModel.HasWeChatQRverifyFileName
            });
        }

        public IActionResult Feedback()
        {
            List<FeedbackModel> fblist = thisData.GetFeedbackList(HttpContext.Session.GetUniacID());
            return View(fblist);
        }
        public IActionResult AccountInfo()
        {
            List<AccountModel> fblist = thisData.GetAccountList(HttpContext.Session.GetUniacID());
            return View(fblist);
        }
        public IActionResult ProcessMiniZipDownload()
        {
            if (!HttpContext.Session.HasWe7Data())
            {
                return RedirectToAction("Index", "Error", new { errorType = ErrorType.ErrorNoUserOrTimeOut });
            }
            try
            {
                string fileUrl;
                ProcessMiniTool.GetProcessMiniZipPath(HttpContext.Session, out fileUrl);
                byte[] fileByteArray = System.IO.File.ReadAllBytes(fileUrl);
                var fileName = Path.GetFileName(fileUrl);
                System.IO.File.Delete(fileUrl);
                return File(fileByteArray, "application/vnd.android.package-archive", fileName);
            }
            catch (Exception e)
            {
                e.Save();
                return RedirectToAction("Index", "Error");
                //throw;
            }
        }
        /// <summary>
        /// 设置服务费率
        /// </summary>
        /// <param name="qRSendFee"></param>
        /// <returns></returns>
        public IActionResult SetQRSendFee(decimal qRSendFee)
        {
            try
            {
                thisData.SetQRSendFee(HttpContext.Session.GetUniacID(), qRSendFee);
                return this.JsonSuccessStatus();
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
            }
        }

        public IActionResult SetYTX()
        {
            try
            {
                string json = new StreamReader(Request.Body).ReadToEnd();
                YTXModel qiNiuModel = JsonConvert.DeserializeObject<YTXModel>(json);
                qiNiuModel.AccountSID = qiNiuModel.AccountSID.Trim();
                qiNiuModel.AuthToken = qiNiuModel.AuthToken.Trim();
                qiNiuModel.AppID = qiNiuModel.AppID.Trim();
                thisData.SetYTX(HttpContext.Session.GetUniacID(), qiNiuModel);
                return this.JsonSuccessStatus();
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
               
            }
        }

        public IActionResult SendOrder(string accountID)
        {
            try
            {
                string json = new StreamReader(Request.Body).ReadToEnd();
                Order order = JsonConvert.DeserializeObject<Order>(json);
                thisData.SendOrder(HttpContext.Session.GetUniacID(), new ObjectId(accountID), order);
                return this.JsonSuccessStatus();
            }
            catch (Exception)
            {
                return this.JsonErrorStatus();
               
            }
        }

        public IActionResult PushWeChatQRRuleVerify()
        {
            try
            {
                var files = Request.Form.Files;
                thisData.PushWeChatQRRuleVerify(HttpContext.Session.GetUniacID(), files[0], hostingEnvironment.ContentRootPath);
                return this.JsonSuccessStatus();
            }
            catch (Exception e)
            {
                e.Save();
                return this.JsonErrorStatus();
               
            }
        }
    }
}
