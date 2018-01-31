using System;
using System.Collections.Generic;
using System.Text;
using WXSmallAppCommon.WXTool;

namespace WXSmallAppCommon.WXInteractions
{
    public class Refund
    {
        /// <summary>
        /// 申请退款完整业务流程逻辑
        /// </summary>
        /// <param name="transaction_id">微信订单号</param>
        /// <param name="out_trade_no">商户订单号</param>
        /// <param name="total_fee">订单总金额</param>
        /// <param name="refund_fee">退款金额</param>
        /// <returns> 退款结果（xml格式）</returns>
        public static string Run(string transaction_id, string out_trade_no, int total_fee, int refund_fee,string appID=WxPayConfig.APPID, string mchID=WxPayConfig.MCHID,string key=WxPayConfig.KEY,string sSLCERT_PASSWORD = WxPayConfig.SSLCERT_PASSWORD,string sSLCERT_PATH=WxPayConfig.SSLCERT_PATH)
        {
            Log.Info("Refund", "Refund is processing...");
            WxPayData data = new WxPayData();
            if (!string.IsNullOrEmpty(transaction_id))//微信订单号存在的条件下，则已微信订单号为准
            {
                data.SetValue("transaction_id", transaction_id);
            }
            else//微信订单号不存在，才根据商户订单号去退款
            {
                data.SetValue("out_trade_no", out_trade_no);
            }
            data.SetValue("total_fee", total_fee);//订单总金额
            data.SetValue("refund_fee", refund_fee);//退款金额
            data.SetValue("out_refund_no", WxPayApi.GenerateOutTradeNo());//随机生成商户退款单号
            data.SetValue("op_user_id",mchID);//操作员，默认为商户号
            WxPayData result = WxPayApi.Refund(data,6,appID,mchID,key,sSLCERT_PASSWORD,sSLCERT_PATH);//提交退款申请给API，接收返回数据
            Log.Info("Refund", "Refund process complete, result : " + result.ToXml());
            return result.ToPrintStr();
        }
    }
}
