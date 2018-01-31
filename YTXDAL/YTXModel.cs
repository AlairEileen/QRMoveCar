using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace YTXDAL
{
    public class YTXModel
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        [BsonIgnore]
        public string RestIP
        {
            get
            {
                return IsTestApp ? "http://sandbox.ytx.net" : "http://api.ytx.net";
            }
        }
        private bool isTestApp = true;

        public string AppID { get; set; }
        public bool IsTestApp { get => isTestApp; set => isTestApp = value; }
    }

}
