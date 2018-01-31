using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Tools.Models
{
    public class FileModel<T>
    {
        [BsonId]
        [JsonConverter(typeof(Tools.Json.ObjectIdConverter))]
        public ObjectId FileID { get; set; }
        /// <summary>
        /// 0：源文件，1：中压缩文件，2：最小文件
        /// </summary>
        public T FileUrlData { get; set; }
        public int FileNum { get; set; }
    }
}
