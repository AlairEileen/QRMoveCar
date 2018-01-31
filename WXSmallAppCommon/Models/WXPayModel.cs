using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WXSmallAppCommon.Models
{
    public class WXPayModel
    {
        [JsonProperty("appId")]
        public string AppId { get; set; }
        [JsonProperty("nonceStr")]
        public string NonceStr { get; set; }
        [JsonProperty("package")]
        public string Package { get; set; }
        [JsonProperty("paySign")]
        public string PaySign { get; set; }
        [JsonProperty("signType")]
        public string SignType { get; set; }
        [JsonProperty("timeStamp")]
        public string TimeStamp { get; set; }
    }
}
