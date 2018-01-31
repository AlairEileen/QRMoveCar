using ConfigData;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using We7Tools.Extend;

namespace We7Tools
{
    public class ProcessMiniTool
    {
        public static void GetProcessMiniZipPath(ISession session, out string zipFilePath)
        {
            var siteInfo = GetSiteInfo(session);

            var siteInfoJs = @"var siteinfo={
  'title': '" + siteInfo.title + @"',
  'uniacid': '" + siteInfo.uniacid + @"',
  'acid': '" + siteInfo.acid + @"',
  'multiid': '" + siteInfo.multiid + @"',
  'version': '" + siteInfo.version + @"',
  'siteroot': '" + siteInfo.siteroot + @"',
  'design_method': '" + siteInfo.design_method + @"',
  'redirect_module': '" + siteInfo.redirect_module + @"',
  'template': '" + siteInfo.template + @"'
};
//模块暴露
module.exports = siteinfo;";
            using (var sw = new StreamWriter(We7Config.ProcessMiniFolderPath + "/siteinfo.js", false, Encoding.UTF8))
            {
                sw.Write(siteInfoJs);
            }
            zipFilePath = (We7Config.ProcessMiniFolderPath.LastIndexOf("/") ==
                (We7Config.ProcessMiniFolderPath.Length - 1) ?
                We7Config.ProcessMiniFolderPath.Substring(0, We7Config.ProcessMiniFolderPath.LastIndexOf("/")) :
                We7Config.ProcessMiniFolderPath) + ".zip";
            System.IO.Compression.ZipFile.CreateFromDirectory(We7Config.ProcessMiniFolderPath, zipFilePath);
        }

        private static SiteInfo GetSiteInfo(ISession session)
        {
            JObject jObject = session.GetWe7Data();
            var siteInfo = new SiteInfo()
            {
                title = "",
                uniacid = (string)jObject["uniacid"],
                acid = (string)jObject["acid"],
                design_method = "",
                multiid = "",
                redirect_module = "",
                siteroot = We7Config.SiteRoot,
                template = (string)jObject["template"],
                version = We7Config.PMVersion
            };

            return siteInfo;
        }
    }

    public class SiteInfo
    {
        public string title { get; set; }
        public string uniacid { get; set; }
        public string acid { get; set; }
        public string multiid { get; set; }
        public string version { get; set; }
        public string siteroot { get; set; }
        public string design_method { get; set; }
        public string redirect_module { get; set; }
        public string template { get; set; }
    }
}
