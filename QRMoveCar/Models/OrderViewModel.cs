using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QRMoveCar.Models
{
    public class OrderViewModel
    {
        public Order OrderInfo { get; set; }
        public string AccountName { get; set; }
        public string AccountAvatar { get; set; }
        public string CarNumber { get; set; }
        public string AccountID { get; set; }
        public string uniacid { get; set; }
    }
}
