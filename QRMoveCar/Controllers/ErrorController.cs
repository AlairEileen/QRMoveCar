using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRMoveCar.Controllers
{
    public class ErrorController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.ErrorNoUserOrTimeOut:
                    ViewData["ErrorInfo"] = "用户登录超时或者用户不正确";
                    break;
                default:
                    ViewData["ErrorInfo"] = "未知错误";
                    break;
            }
            return View();
        }

    }

    public enum ErrorType
    {
        ErrorNoUserOrTimeOut = 0
    }
}
