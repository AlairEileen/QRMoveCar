using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools.Models;
using Tools.Response.Json;
using YTXDAL;

namespace QRMoveCar.Models
{
    public class CompanyModel
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId CompanyID { get; set; }
        public string uniacid { get; set; }
        public ProcessMiniInfo ProcessMiniInfo { get; set; }
        public QiNiuModel QiNiuModel { get; set; }
        /// <summary>
        /// 服务费率
        /// </summary>
        public decimal QRSendFee { get; set; }
        public string CertFileName { get; set; }
        public string WeChatQRVerifyFileName { get; set; }
        [BsonIgnore]
        public bool HasWeChatQRverifyFileName { get { return !string.IsNullOrEmpty(WeChatQRVerifyFileName); } }
        public YTXModel YTX { get; set; }
    }



    public class QiNiuModel
    {
        public QiNiuDAL.Exerciser exerciser = new QiNiuDAL.Exerciser();
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Bucket { get; set; }
        public string DoMain { get; set; }
        public void UploadFile(string filePath)
        {
            exerciser.UploadFile(filePath, AccessKey, SecretKey, Bucket);
        }
        public async Task<string> GetFileUrl(string fileName)
        {
            return await exerciser.CreateDownloadUrl(DoMain, fileName);
        }
        public void DeleteFile(string fileName)
        {
            exerciser.DeleteFile(fileName, AccessKey, SecretKey, Bucket);
        }
    }
    public class ProcessMiniInfo
    {
        public string Detail { get; set; }
        public string Name { get; set; }
        public FileModel<string[]> Logo { get; set; }
    }
}
