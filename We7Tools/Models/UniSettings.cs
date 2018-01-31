using ConfigData;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Tools.DB;
using We7Tools.MysqlTool;
using WXSmallAppCommon.WXTool;

namespace We7Tools.Models
{
    internal class We7Settings
    {
        internal We7Settings(string uniacid)
        {
            string jsonString = "";
            using (WebClient wc = new WebClient())
            {
                jsonString = wc.DownloadString($"{We7Config.We7DataGetUrl}{uniacid}");
            }
            jsonObj = (JObject)JsonConvert.DeserializeObject(jsonString);
        }
        internal void WriteConfig(ProcessMiniConfig processMiniConfig)
        {
            var payment = jsonObj["payment"];
            if (payment != null)
            {
                var wechat = payment["wechat"];
                if (wechat != null)
                {
                    processMiniConfig.MCHID = wechat["mchid"] == null ? null : wechat["mchid"].ToString();
                    processMiniConfig.KEY = wechat["signkey"] == null ? null : wechat["signkey"].ToString();

                }
            }
            processMiniConfig.APPID = jsonObj["key"] == null ? null : jsonObj["key"].ToString();
            processMiniConfig.APPSECRET = jsonObj["secret"] == null ? null : jsonObj["secret"].ToString();

        }
        private JObject jsonObj;

    }
    public class ProcessMiniConfig
    {
        public string MCHID { get; set; }
        public string KEY { get; set; }
        private string sSLCERT_PASSWORD;
        public string CertKey { get; set; }
        public string cert { get; set; }
        public string APPID { get; set; }
        public string APPSECRET { get; set; }
        public string SSLCERT_PASSWORD { get { return sSLCERT_PASSWORD == null ? MCHID : sSLCERT_PASSWORD; } set { sSLCERT_PASSWORD = value; } }

        public string SSLCERT_PATH { get; internal set; }
    }

    public class We7ProcessMiniConfig
    {
        public static ProcessMiniConfig GetAllConfig(string uniacid)
        {
            ProcessMiniConfig pmc = new ProcessMiniConfig();
            new We7Settings(uniacid).WriteConfig(pmc);
            WriteCertFileConfig(uniacid, pmc);
            return pmc;
        }

        private static void WriteCertFileConfig(string uniacid, ProcessMiniConfig pmc)
        {
            BsonDocument document = new MongoDBTool().GetMongoCollection<BsonDocument>("CompanyModel").Find(Builders<BsonDocument>.Filter.Eq("uniacid", uniacid)).FirstOrDefault();
            var cfn = document.GetValue("CertFileName").ToString();
            pmc.SSLCERT_PATH = $"{MainConfig.BaseDir}{MainConfig.CertsDir}/{uniacid}/{cfn}";
        }
    }


}
