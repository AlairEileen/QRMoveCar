using System;
using System.Collections.Generic;
using System.Text;
using WXSmallAppCommon.WXTool;

namespace WXSmallAppCommon.WXInteractions
{
    public class WxRedPack
    {
        /// <summary>
        /// 用于企业向微信用户个人发现金红包。需要商户证书
        /// 目前支持向指定微信用户的openid发放指定金额红包。
        /// </summary>
        /// <returns></returns>
        public string SendRedPack(SendRedPackModel sendRedPack)
        {

            //加入常规的参数
            WxPayData data = new WxPayData();
            data.SetValue("wxappid", WxPayConfig.APPID);//公众账号appid
            data.SetValue("mch_id", WxPayConfig.MCHID);//商户号
            data.SetValue("nonce_str", WxPayApi.GenerateNonceStr());//随机字符串
            data.SetValue("send_name", sendRedPack.SenderName);//    红包发送者名称

            //商户订单号（每个订单号必须唯一） 组成：mch_id+yyyymmdd+10位一天内不能重复的数字。
            //接口根据商户订单号支持重入，如出现超时可再调用。
            data.SetValue("mch_billno", WxPayApi.GenerateOutTradeNo());

            data.SetValue("re_openid", sendRedPack.re_openid);
            data.SetValue("total_amount", sendRedPack.total_amount);
            data.SetValue("total_num", sendRedPack.total_num);
            data.SetValue("wishing", sendRedPack.wishing);
            data.SetValue("client_ip", sendRedPack.client_ip==null ? WxPayConfig.IP:sendRedPack.client_ip);
            data.SetValue("act_name", sendRedPack.act_name);
            data.SetValue("remark", sendRedPack.remark);

            data.SetValue("sign", data.MakeSign());//最后生成签名

            var url = "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack";
            return HttpService.Post(data.ToXml(), url);
        }
    }
    /// <summary>
    /// 现金红包和裂变红包的基础信息
    /// </summary>
    public class BaseRedPackModel
    {
        /// <summary>
        /// 接受红包的用户
        /// 用户openid    
        /// </summary>
        public string re_openid { get; set; }

        /// <summary>
        /// 付款金额，单位分
        /// </summary>
        public int total_amount { get; set; }

        /// <summary>
        /// 红包发放总人数
        /// </summary>
        public int total_num { get; set; }

        /// <summary>
        /// 红包祝福语
        /// </summary>
        public string wishing { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string act_name { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string remark { get; set; }
        public string SenderName { get; set; }
    }

    /// <summary>
    /// 发送红包的数据信息
    /// </summary>
    public class SendRedPackModel : BaseRedPackModel
    {
        /// <summary>
        /// 调用接口的机器Ip地址
        /// </summary>
        public string client_ip { get; set; }

        public SendRedPackModel()
        {
            this.total_num = 1;//红包发放总人数
        }
    }

}
