using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Response.Json
{
    /// <summary>
    /// json result extensions methods 
    /// </summary>
    public static class JsonExtensions
    {
        private static JsonSerializerSettings defaultJSS = new JsonSerializerSettings();

        /// <summary>
        /// success 
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="jsonObject">要序列化的JsonResponse</param>
        /// <param name="serializerSettings">json设置</param>
        /// <returns>jsonResult</returns>
        public static Microsoft.AspNetCore.Mvc.JsonResult JsonSuccess(this Microsoft.AspNetCore.Mvc.Controller controller, JsonResponse jsonObject, JsonSerializerSettings serializerSettings = null)
        {
            jsonObject.StatusCode = ResponseStatus.请求成功;
            jsonObject.Message = ResponseStatus.请求成功.ToString();
            return controller.Json(jsonObject, serializerSettings ?? defaultJSS);
        }

        /// <summary>
        /// success
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="jsonObject">要序列化的JsonResponse</param>
        /// <param name="limitParams">要显示的参数数组</param>
        /// <returns>jsonResult</returns>
        public static Microsoft.AspNetCore.Mvc.JsonResult JsonSuccessWithLimit(this Microsoft.AspNetCore.Mvc.Controller controller, JsonResponse jsonObject, string[] limitParams = null)
        {
            JsonSerializerSettings settings;
            if (limitParams == null)
            {
                settings = defaultJSS;
            }
            else
            {
                string[] parmas = new string[limitParams.Length + 2];
                parmas[0] = "StatusCode";
                parmas[1] = "Message";
                limitParams.CopyTo(parmas, 2);
                settings = new JsonSerializerSettings() { ContractResolver = new LimitPropsContractResolver(parmas) };
            }
            jsonObject.StatusCode = ResponseStatus.请求成功;
            jsonObject.Message = ResponseStatus.请求成功.ToString();
            return controller.Json(jsonObject, settings);
        }

        /// <summary>
        /// error status
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <returns>jsonResult</returns>
        public static Microsoft.AspNetCore.Mvc.JsonResult JsonErrorStatus(this Microsoft.AspNetCore.Mvc.Controller controller)
        {
            return controller.Json(new JsonResponse() { StatusCode = ResponseStatus.请求失败, Message = ResponseStatus.请求失败.ToString() }, defaultJSS);
        }

        /// <summary>
        /// success status
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <returns>jsonResult</returns>
        public static Microsoft.AspNetCore.Mvc.JsonResult JsonSuccessStatus(this Microsoft.AspNetCore.Mvc.Controller controller)
        {
            return controller.Json(new JsonResponse() { StatusCode = ResponseStatus.请求成功, Message = ResponseStatus.请求成功.ToString() }, defaultJSS);
        }

        /// <summary>
        /// other stauts
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="actionParams">状态参数</param>
        /// <returns>jsonResult</returns>
        public static Microsoft.AspNetCore.Mvc.JsonResult JsonOtherStatus(this Microsoft.AspNetCore.Mvc.Controller controller, ResponseStatus actionParams)
        {
            return controller.Json(new JsonResponse() { StatusCode = actionParams, Message = actionParams.ToString() }, defaultJSS);
        }
    }
}
