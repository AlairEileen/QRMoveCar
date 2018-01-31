using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.ResponseModels
{
    public class BaseResponseModel<T>
    {
        public ActionParams StatusCode { get; set; }
        public T JsonData { get; set; }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class BaseResponseModel2<T, P> : BaseResponseModel<T>
    {
        public P JsonData1 { get; set; }
    }
    public class BaseResponseModel3<T, P, H> : BaseResponseModel<T>
    {
        public P JsonData1 { get; set; }
        public H JsonData2 { get; set; }
    }
}
