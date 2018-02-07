using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.DB;
using Tools.Response.Json;

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

        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
        public string VerifyCode { get => verifyCode; set => verifyCode = value; }

        public static IMongoCollection<VerifyCodeModel> Collection {
            get {
                var collection = new MongoDBTool().GetMongoCollection<VerifyCodeModel>();
                try
                {
                    var options = new CreateIndexOptions<VerifyCodeModel>();
                    options.Name = "timer_delete_20";
                    options.ExpireAfter = new TimeSpan(0, 0, 20);
                    options.Version = 1;
                    var idk = Builders<VerifyCodeModel>.IndexKeys;
                    collection.Indexes.CreateOne(idk.Ascending(x => x.CreateTime), options);
                }
                catch (Exception)
                {

                }
                return collection;
            }
        }

    }
}
