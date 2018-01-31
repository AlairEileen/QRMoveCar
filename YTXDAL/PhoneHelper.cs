using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace YTXDAL
{
    public class PhoneHelper
    {
        private YTXModel yTXModel;

        public PhoneHelper(YTXModel yTXModel)
        {
            this.yTXModel = yTXModel;
        }
        //云通信平台接口版本号
        //public string version = "201512";
        //以上参数，仅作事例，请登录云通信平台及提供的相关开发说明文档获取各参数具体值

        #region 1.0、发送http请求访问接口+ public static string SendRequest(string url, string jsonData)
        public string SendRequest(string url, string jsonData)
        {
            string Authorization;
            string Sign;
            CreatAuthorizationAndSign(out Authorization, out Sign);
            string version = "201512";
            if (url == "/call/TeleMeeting.wx" || url == "/account/getBlance.wx" || url == "/call/MeetingQuery.wx" || url == "/traffic/Traffic.wx")
            {
                version = "201612";
            }
            string urlSign = yTXModel.RestIP + "/" + version + "/sid/" + yTXModel.AccountSID + url + "?Sign=" + Sign;
            return SendRequest(urlSign, jsonData, Authorization);
        }
        #endregion

        #region 1.0.1、发送http请求访问接口+ public static string SendRequest(string url, string jsonData)
        public string SendRequest(string accountsid, string authtoken, string url, string jsonData)
        {
            string Authorization;
            string Sign;
            string version = "201512";
            if (url == "/call/TeleMeeting.wx" || url == "/account/getBlance.wx" || url == "/call/MeetingQuery.wx" || url == "/traffic/Traffic.wx")
            {
                version = "201612";
            }
            CreatAuthorizationAndSign(accountsid, authtoken, out Authorization, out Sign);
            string urlSign = yTXModel.RestIP + "/" + version + "/sid/" + accountsid + url + "?Sign=" + Sign;
            return SendRequest(urlSign, jsonData, Authorization);
        }
        #endregion

        #region 1.1、发送http请求访问接口+ public static string SendRequest(string url, string jsonData, string authorization)
        /// <summary>
        /// 发送http请求访问接口
        /// </summary>
        /// <param name="url">api地址</param>
        /// <param name="jsonData">api所需请求参数json字符串</param>
        /// <returns>消息状态码</returns>
        public string SendRequest(string url, string jsonData, string authorization)
        {
            //、将参数转化为assic码
            byte[] postBytes = Encoding.UTF8.GetBytes(jsonData);
            HttpWebRequest caaSRequest = null;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            //初始化新的webRequst
            caaSRequest = (HttpWebRequest)WebRequest.Create(url);
            caaSRequest.Method = "POST";
            caaSRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            string AuthorizationCode = authorization;
            caaSRequest.Headers.Set("Authorization", AuthorizationCode);
            caaSRequest.ContentLength = postBytes.Length;
            using (Stream reqStream = caaSRequest.GetRequestStream())
            {
                reqStream.Write(postBytes, 0, postBytes.Length);
                reqStream.Close();
            }
            string content = "";
            Stream resStream = null;
            try
            {
                HttpWebResponse caaSResponse = (HttpWebResponse)caaSRequest.GetResponse();
                resStream = caaSResponse.GetResponseStream();
                using (StreamReader sr = new StreamReader(resStream))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse wr = (WebResponse)ex.Response;
                resStream = wr.GetResponseStream();
                using (StreamReader sr = new StreamReader(resStream))
                {
                    content = sr.ReadToEnd();
                }
            }
            return content;
        }
        #endregion


        #region 2、生成鉴权字符串+public static void CreatAuthorizationAndSign(out string Authorization, out string Sign)
        public void CreatAuthorizationAndSign(out string Authorization, out string Sign)
        {
            string dtF = DateTime.Now.ToString("yyyyMMddHHmmss");
            //auth:账户Id + | + 时间戳
            string authFormal = string.Format("{0}|{1}", yTXModel.AccountSID, dtF);
            Authorization = Base64EnCode(authFormal);
            //------------------------------------
            //sig:MD5加密（主帐号Id + 主帐号授权令牌 +时间戳）
            string SIGFormal = string.Format("{0}{1}{2}", yTXModel.AccountSID, yTXModel.AuthToken, dtF);
            Sign= UserMd5(SIGFormal);
            //Sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(SIGFormal, "md5");

        }
        #endregion

        public string UserMd5(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("x2");

            }
            return pwd;
        }

        #region 2.0、生成鉴权字符串+public static void CreatAuthorizationAndSign(string accountsid,string authtoken,out string Authorization, out string Sign)
        public void CreatAuthorizationAndSign(string accountsid, string authtoken, out string Authorization, out string Sign)
        {
            string dtF = DateTime.Now.ToString("yyyyMMddHHmmss");
            //auth:账户Id + | + 时间戳
            string authFormal = string.Format("{0}|{1}", accountsid, dtF);
            Authorization = Base64EnCode(authFormal);
            //------------------------------------
            //sig:MD5加密（主帐号Id + 主帐号授权令牌 +时间戳）
            string SIGFormal = string.Format("{0}{1}{2}", accountsid, authtoken, dtF);
            Sign = UserMd5(SIGFormal);

            //Sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(SIGFormal, "md5");
        }
        #endregion

        #region 3、回调验证证书问题+private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受     
        }
        #endregion

        #region 4、Base64加密+public static string Base64EnCode(string Message)
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public string Base64EnCode(string Message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Message);
            return Convert.ToBase64String(bytes);
        }
        #endregion

        #region 5、Base64解密+public static string Base64Decode(string Message)
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public string Base64Decode(string Message)
        {
            byte[] bytes = Convert.FromBase64String(Message);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion
    }
}
