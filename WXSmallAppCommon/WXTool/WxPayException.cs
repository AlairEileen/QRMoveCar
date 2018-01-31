using System;
using System.Collections.Generic;
using System.Text;

namespace WXSmallAppCommon.WXTool
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg) : base(msg)
        {

        }
    }
}
