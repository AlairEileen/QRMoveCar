using System;
using System.Collections.Generic;
using System.Text;

namespace WXSmallAppCommon.Models
{
    class WXResult
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public string errcode { get; set; }
        public string errmsg { get; set; }
    }
}
