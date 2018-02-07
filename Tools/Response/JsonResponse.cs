using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Response.Json;

namespace Tools.Response
{
    public class JsonResponse
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public ResponseStatus StatusCode { get; set; }
      
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Convert To Json Result
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <returns>JsonResult</returns>
        public Microsoft.AspNetCore.Mvc.JsonResult ToJsonResult(Microsoft.AspNetCore.Mvc.Controller controller)
        {
            return controller.JsonSuccess(this);
        }
    }

    /// <summary>
    /// 一个数据模型的 json result
    /// </summary>
    /// <typeparam name="T">数据模型</typeparam>
    public class JsonResponse1<T>:JsonResponse
    {
        /// <summary>
        /// 数据模型1
        /// </summary>
        public T JsonData { get; set; }
      
    }

    /// <summary>
    /// 一个数据模型的 json result
    /// </summary>
    /// <typeparam name="T">数据模型1</typeparam>
    /// <typeparam name="P">数据模型2</typeparam>
    public class JsonResponse2<T, P> : JsonResponse1<T>
    {
        /// <summary>
        /// 数据模型2
        /// </summary>
        public P JsonData1 { get; set; }
    }

    /// <summary>
    /// 一个数据模型的 json result
    /// </summary>
    /// <typeparam name="T">数据模型1</typeparam>
    /// <typeparam name="P">数据模型2</typeparam>
    /// <typeparam name="H">数据模型3</typeparam>
    public class JsonResponse3<T, P, H> : JsonResponse2<T,P>
    {
        /// <summary>
        /// 数据模型3
        /// </summary>
        public H JsonData2 { get; set; }
    }

    /// <summary>
    /// 一个数据模型的 json result
    /// </summary>
    /// <typeparam name="T">数据模型1</typeparam>
    /// <typeparam name="P">数据模型2</typeparam>
    /// <typeparam name="H">数据模型3</typeparam>
    /// <typeparam name="I">数据模型4</typeparam>
    public class JsonResponse4<T, P, H,I> : JsonResponse3<T,P,H>
    {
        /// <summary>
        /// 数据模型4
        /// </summary>
        public H JsonData3 { get; set; }
    }

    /// <summary>
    /// 一个数据模型的 json result
    /// </summary>
    /// <typeparam name="T">数据模型1</typeparam>
    /// <typeparam name="P">数据模型2</typeparam>
    /// <typeparam name="H">数据模型3</typeparam>
    /// <typeparam name="I">数据模型4</typeparam>
    /// <typeparam name="J">数据模型5</typeparam>
    public class JsonResponse5<T, P, H,I,J> : JsonResponse4<T,P,H,I>
    {
        /// <summary>
        /// 数据模型5
        /// </summary>
        public H JsonData4 { get; set; }
    }
}
