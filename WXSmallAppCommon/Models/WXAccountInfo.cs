using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Models;

namespace WXSmallAppCommon.Models
{
    public class WXAccountInfo
    {
        [JsonProperty("openId")]
        public string OpenId { get; set; }
        [JsonProperty("nickName")]
        public string NickName { get; set; }
        [JsonProperty("gender")]
        public Int16 Gender { get; set; }
        public Gender GetGender
        {
            get
            {
                switch (Gender)
                {
                    case 1:
                        return Tools.Models.Gender.male;
                    case 2: return Tools.Models.Gender.female;
                    default:
                        return Tools.Models.Gender.none;
                }
            }
        }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("avatarUrl")]
        public string AvatarUrl { get; set; }
        [JsonProperty("unionId")]
        public string UnionId { get; set; }
        [JsonProperty("watermark")]
        public WXWatermark Watermark { get; set; }
    }
   
}
