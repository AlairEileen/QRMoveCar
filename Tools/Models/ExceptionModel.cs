using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.DB;

namespace Tools.Models
{
    public class ExceptionModel:Exception
    {
        [BsonId]
        [JsonConverter(typeof(Tools.Json.ObjectIdConverter))]
        public ObjectId ExceptionID { get; set; }
        public string MethodFullName { get; set; }
        public ActionParams ExceptionParam { get; set; }
        public string Content { get; set; }
        [JsonConverter(typeof(Tools.Json.DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExceptionDate { get; set; }
        public string EXMessage { get; set; }
        public void Save()
        {
            new MongoDBTool().GetMongoCollection<ExceptionModel>().InsertOne(this);
        }
    }

    public static class ExceptionExtends
    {
        public static void Save(this Exception e)
        {
            var method = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            var em = new ExceptionModel()
            {
                MethodFullName =$"{method.ReflectedType.FullName}.{method.Name}",
                EXMessage = e.Message,
                ExceptionDate = DateTime.Now
            };
            new MongoDBTool().GetMongoCollection<ExceptionModel>().InsertOne(em);
        }
    }
}
