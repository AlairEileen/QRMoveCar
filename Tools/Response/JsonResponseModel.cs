using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Json;
using Tools.ResponseModels;

namespace Tools.Response
{
    public class JsonResponseModel
    {
        static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();


        static JsonResponseModel()
        {
            string[] param = new string[] { "StatusCode" };
            jsonSerializerSettings.ContractResolver = new LimitPropsContractResolver(param);
        }
        private static string successJson;
        public static string SuccessJson
        {
            get
            {
                if (string.IsNullOrEmpty(successJson))
                {
                    successJson = JsonConvert.SerializeObject(new BaseResponseModel<string>() { StatusCode = ActionParams.code_ok }, jsonSerializerSettings);
                }
                return successJson;
            }
        }
        private static string errorJson;
        public static string ErrorJson
        {
            get
            {
                if (string.IsNullOrEmpty(errorJson))
                {
                    errorJson = JsonConvert.SerializeObject(new BaseResponseModel<string>() { StatusCode = ActionParams.code_error }, jsonSerializerSettings);
                }
                return errorJson;
            }
        }
        public static string OtherJson(ActionParams actionParams)
        {
            return JsonConvert.SerializeObject(new BaseResponseModel<string>() { StatusCode = actionParams }, jsonSerializerSettings);
        }
        private static string nullJson;
        public static string NullJson
        {
            get
            {
                if (string.IsNullOrEmpty(nullJson))
                {
                    nullJson = JsonConvert.SerializeObject(new BaseResponseModel<string>() { StatusCode = ActionParams.code_null }, jsonSerializerSettings);
                }
                return nullJson;
            }
        }
        private static string errorNullJson;
        public static string ErrorNullJson
        {
            get
            {
                if (string.IsNullOrEmpty(errorNullJson))
                {
                    errorNullJson = JsonConvert.SerializeObject(new BaseResponseModel<string>() { StatusCode = ActionParams.code_error_null }, jsonSerializerSettings);
                }
                return errorNullJson;
            }
        }
    }
}
