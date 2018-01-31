using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using We7Tools.Extend;
using WeChatCommon.AppData;

namespace WeChatCommon.Controllers
{
    public class BaseController<T, P> : Controller where T : BaseData<P>
    {
        protected T thisData;
        protected bool hasIdentity;
        protected BaseController(bool hasIdentity = false)
        {
            thisData = Activator.CreateInstance<T>();
            this.hasIdentity = hasIdentity;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (hasIdentity)
            {
                if (!context.HttpContext.Session.HasWe7Data())
                {
                    context.Result = new RedirectToActionResult("Index", "Error", new { errorType = ErrorType.ErrorNoUserOrTimeOut });
                    return;
                }
            }
            base.OnActionExecuting(context);
        }

    }
}
