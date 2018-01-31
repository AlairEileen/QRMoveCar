using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Json;

namespace We7Tools.Models
{
   public class We7Temp
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId We7TempID { get; set; }
        public string Data { get; set; }
    }
}
