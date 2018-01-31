using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WXSmallAppCommon.WXTool;

namespace WXSmallAppCommon
{
    public class Test
    {
        public string CertFileExists()
        {
            string path = WxPayConfig.SSLCERT_PATH;
            var file = File.Open(path, FileMode.Open);
            return File.Exists(path)+"";

            //X509Certificate2 cert = new X509Certificate2(path + WxPayConfig.SSLCERT_PATH, WxPayConfig.SSLCERT_PASSWORD);
            //request.ClientCertificates.Add(cert);
        }
    }
}
