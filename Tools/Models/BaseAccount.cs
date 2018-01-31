using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Models
{
    public class BaseAccount
    {
        [BsonId]
        [JsonConverter(typeof(Tools.Json.ObjectIdConverter))]
        public ObjectId AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountPhoneNumber { get; set; }
        public Tools.Models.Gender Gender { get; set; }
        public string AccountAvatar { get; set; }
        [JsonConverter(typeof(Tools.Json.DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind =DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
        [JsonConverter(typeof(Tools.Json.DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind =DateTimeKind.Local)]
        public DateTime LastChangeTime { get; set; }
    }
    public enum Gender
    {
        none = 0, male = 1, female = 2
    }
}
