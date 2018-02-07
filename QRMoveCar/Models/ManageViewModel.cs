using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YTXDAL;

namespace QRMoveCar.Models
{
    public class ManageViewModel
    {
        public ProcessMiniInfo ProcessMiniInfo { get; set; }

        /// <summary>
        /// 二维码寄送费用
        /// </summary>
        public decimal QRSendFee { get; set; }

        public YTXModel YTX { get; set; }
        public string uniacid { get; set; }
        public bool HasWeChatQRverifyFileName { get; set; }

    }
}
