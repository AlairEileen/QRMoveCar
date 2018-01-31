using System;
using System.Collections.Generic;
using System.Text;
using WXSmallAppCommon.WXTool;

namespace WXSmallAppCommon.WXInteractions
{
   public class Transfer
    {

        /// <summary>
        /// 商户向用户付款
        /// </summary>
        /// <param name="amount">支付金额</param>
        /// <param name="desc">支付描述</param>
        /// <param name="openid">用户openid</param>
        /// <param name="partner_trade_no">商户订单号</param>
        /// <param name="appID"></param>
        /// <param name="mchID"></param>
        /// <param name="key"></param>
        /// <param name="sSLCERT_PASSWORD"></param>
        /// <param name="sSLCERT_PATH"></param>
        /// <returns>WxPayData</returns>
        public static WxPayData Run(int amount, string desc, string openid, string partner_trade_no, string appID = WxPayConfig.APPID, string mchID = WxPayConfig.MCHID, string key = WxPayConfig.KEY, string sSLCERT_PASSWORD = WxPayConfig.SSLCERT_PASSWORD, string sSLCERT_PATH = WxPayConfig.SSLCERT_PATH)
        {
            Log.Info("Refund", "Refund is processing...");
            WxPayData data = new WxPayData();
            data.SetValue("amount", amount);//支付金额
            data.SetValue("desc", desc);//支付描述
            data.SetValue("openid", openid);//用户openid
            data.SetValue("partner_trade_no", partner_trade_no);//商户订单号
            WxPayData result = WxPayApi.Transfer(data, 6, appID, mchID, key, sSLCERT_PASSWORD, sSLCERT_PATH);//提交退款申请给API，接收返回数据
            Log.Info("Refund", "Refund process complete, result : " + result.ToXml());
          
            return result;
        }
    }
}
