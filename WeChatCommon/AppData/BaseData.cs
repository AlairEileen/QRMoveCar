using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.DB;

namespace WeChatCommon.AppData
{
    public abstract class BaseData<T>
    {
        protected MongoDBTool mongo;
        protected IMongoCollection<T> collection;
        protected BaseData()
        {
            mongo = new MongoDBTool();
            collection = mongo.GetMongoCollection<T>();
        }

        protected BaseData(string collectionName)
        {
            mongo = new MongoDBTool();
            collection = mongo.GetMongoCollection<T>(collectionName);
        }

        protected T GetModelByID(ObjectId objectId)
        {
            return collection.Find(Builders<T>.Filter.Eq("_id", objectId)).FirstOrDefault();
        }
        protected T GetModelByIDAndUniacID(ObjectId objectId, string uniacid)
        {
            return collection.Find(GetModelIDAndUniacIDFilter(objectId, uniacid)).FirstOrDefault();
        }

        protected List<T> GetAllModel()
        {
            return collection.Find(Builders<T>.Filter.Empty).ToList();
        }
        protected FilterDefinition<T> GetModelIDAndUniacIDFilter(ObjectId objectId, string uniacid)
        {
            return Builders<T>.Filter.Eq("_id", objectId) & Builders<T>.Filter.Eq("uniacid", uniacid);
        }
    }
}
