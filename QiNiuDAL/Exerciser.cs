using Qiniu.Common;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.RS;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QiNiuDAL
{
    public class Exerciser
    {
        public void UploadFile(string filePath, string accessKey, string secretKey, string bucket)
        {
            // 生成(上传)凭证时需要使用此Mac
            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            Mac mac = new Mac(accessKey, secretKey);

            string saveKey = filePath.Substring(filePath.LastIndexOf('/') + 1);

            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();

            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = bucket;

            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);

            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            //putPolicy.DeleteAfterDays = 1;

            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            

            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);

            UploadManager um = new UploadManager();

            HttpResult result = um.UploadFile(filePath, saveKey, token);
            Console.WriteLine(result);
        }

        public async Task<string> CreateDownloadUrl(string doMain, string fileName)
        {

            //return await Task.Run(() =>
            //{
                // 要保存的文件名，如果已存在则默认覆盖
                if (!System.IO.Directory.Exists(Settings.TempDir))
                {
                    System.IO.Directory.CreateDirectory(Settings.TempDir);
                }
                string saveFile = $@"{Settings.TempDir}{fileName}";

                fileName = $"{doMain}/{fileName}";
            // 可公开访问的url，直接下载
            HttpResult result = await DownloadManager.DownloadAsync(fileName, saveFile);
            return saveFile;
            //});

        }
        public void DeleteFile(string fileName, string accessKey, string secretKey, string bucket)
        {
            Mac mac = new Mac(accessKey, secretKey);
            BucketManager bm = new BucketManager(mac);
            bm.Delete(bucket, fileName);
        }



    }
}
