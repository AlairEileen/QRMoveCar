using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Json;
using Tools.Models;

namespace QRMoveCar.Models
{
    public class AccountModel : BaseAccount
    {
        public string OpenID { get; set; }
        /// <summary>
        /// 微擎专用
        /// </summary>
        public string uniacid { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNumber { get; set; }
        [BsonIgnore]
        public bool HasCarNumber { get { return !string.IsNullOrEmpty(CarNumber); } }

        [BsonIgnore]
        public bool HasPhone { get { return !string.IsNullOrEmpty(AccountPhoneNumber); } }

        public List<Order> Orders { get; set; }
        [JsonConverter(typeof(ObjectIdConverter))]
        [JsonIgnore]
        public ObjectId QRFileID { get; set; }
    }

    public class Order
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId OrderID { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string WeChatOrderID { get; set; }
        public bool IsPaid { get; set; }
        public decimal Total { get; set; }
        /// <summary>
        /// 物流信息
        /// </summary>
        public Logistics Logistics { get; set; }
        [JsonConverter(typeof(Tools.Json.DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
    }

    public class Logistics
    {
        public string Company { get; set; }
        public string Number { get; set; }
    }
}
