using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Json;

namespace QRMoveCar.Models
{
    public class VerifyCodeModel
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId VerifyCodeID { get; set; }

        public string PhoneNumber { get; set; }
        public string uniacid { get; set; }

        private string verifyCode = new Random().Next(100000, 999999) + "";

        [JsonConverter(typeof(Tools.Json.DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
        public string VerifyCode { get => verifyCode; set => verifyCode = value; }
    }
}
