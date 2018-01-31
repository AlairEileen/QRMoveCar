using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using WXSmallAppCommon.Models;
using WXSmallAppCommon.WXTool;

namespace WXSmallAppCommon.WXInteractions
{
    public class WXLoginAction
    {
        /// <summary>
        /// 获取用户信息——包括微擎方式
        /// </summary>
        /// <param name="code"></param>
        /// <param name="iv"></param>
        /// <param name="encryptedData"></param>
        /// <param name="appID"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static WXAccountInfo ProcessRequest(string code, string iv, string encryptedData,string appID= WxPayConfig.APPID,string appSecret=WxPayConfig.APPSECRET)
        {

            Console.WriteLine("进入登陆：&&&");
            string grant_type = "authorization_code";
            WXAccountInfo userInfo = new WXAccountInfo();
            //向微信服务端 使用登录凭证 code 获取 session_key 和 openid 
            string url = $"https://api.weixin.qq.com/sns/jscode2session?appid={appID}&secret={appSecret}&js_code={code}&grant_type={grant_type}";
            string type = "utf-8";

            WXAccountInfoGetter GetUsersHelper = new WXAccountInfoGetter();
            string j = WXAccountInfoGetter.GetUrltoHtml(url, type);//获取微信服务器返回字符串
            Console.WriteLine("登陆返回参数：" + j);
            //将字符串转换为json格式
            JObject jo = (JObject)JsonConvert.DeserializeObject(j);

            WXResult res = new WXResult();
            try
            {
                //微信服务器验证成功
                res.openid = jo["openid"].ToString();
                res.session_key = jo["session_key"].ToString();
            }
            catch (Exception)
            {
                //微信服务器验证失败
                res.errcode = jo["errcode"].ToString();
                res.errmsg = jo["errmsg"].ToString();
            }
            if (!string.IsNullOrEmpty(res.openid))
            {
                //用户数据解密
                WXAccountInfoGetter.AesIV = iv;
                WXAccountInfoGetter.AesKey = res.session_key;
                Console.WriteLine("iv:{0},aeskey:{1}", iv, res.session_key);
                string result = GetUsersHelper.AESDecrypt(encryptedData);

                Console.WriteLine("result:" + result);
                //存储用户数据
                JObject _usrInfo = (JObject)JsonConvert.DeserializeObject(result);


                userInfo.OpenId = _usrInfo["openId"].ToString();
                Console.WriteLine("openId:" + userInfo.OpenId);

                try //部分验证返回值中没有unionId
                {
                    userInfo.UnionId = _usrInfo["unionId"].ToString();
                }
                catch (Exception)
                {
                    userInfo.UnionId = "unionId";
                }
                Console.WriteLine("unionId:" + userInfo.UnionId);

                userInfo.NickName = _usrInfo["nickName"].ToString();
                userInfo.Gender = Convert.ToInt16(_usrInfo["gender"].ToString());
                userInfo.City = _usrInfo["city"].ToString();
                userInfo.Province = _usrInfo["province"].ToString();
                userInfo.Country = _usrInfo["country"].ToString();
                userInfo.AvatarUrl = _usrInfo["avatarUrl"].ToString();

                object watermark = _usrInfo["watermark"].ToString();
                object appid = _usrInfo["watermark"]["appid"].ToString();
                object timestamp = _usrInfo["watermark"]["timestamp"].ToString();
            }
            return userInfo;
        }
    }
}
