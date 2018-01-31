using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRDAL;
using QRMoveCar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tools.DB;
using Tools.Models;
using Tools.Response;
using YTXDAL;
using ZXing.Common;

namespace QRMoveCar.Controllers
{
    public class TestController : Controller
    {

        private IHostingEnvironment hostingEnvironment;
        public TestController(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
        }
        public IActionResult GetQRPic(string info)
        {

            //BitMatrix byteMatrix = new MultiFormatWriter().encode(info, BarcodeFormat.QR_CODE, 300, 300);
            //System.Drawing.Bitmap bitmap = toBitmap(byteMatrix);
            System.Drawing.Bitmap qr = QRCodeHelper.Create(info, 200);
            System.Drawing.Bitmap icon = new System.Drawing.Bitmap("../../../../../../../../../../../../../../home/temp/print.png");
            System.Drawing.Bitmap bitmap = QRCodeHelper.MergeQrImg(qr, icon);


            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] data = ms.GetBuffer();
            return File(data, "Image/png", Guid.NewGuid().ToString("N") + ".png");

        }

        public static System.Drawing.Bitmap toBitmap(BitMatrix matrix)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            var white = System.Drawing.ColorTranslator.FromHtml("0xFFFFFFFF");
            var black = System.Drawing.ColorTranslator.FromHtml("0xFF000000");
            System.Drawing.Bitmap bmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bmap.SetPixel(x, y, matrix[x, y] ? black : white);
                }
            }
            return bmap;
        }

        public string ConvertCarJson()
        {
            string oldJsonPath = $@"{hostingEnvironment.ContentRootPath}/wwwroot/account/car_number.json";
            string oldJson = System.IO.File.ReadAllText(oldJsonPath);
            List<CarNumberOld> cnoList = JsonConvert.DeserializeObject<List<CarNumberOld>>(oldJson);
            var list = new List<CarNumberModel>();
            foreach (var item in cnoList)
            {
                var name = item.code.Substring(0, 1);
                var code = item.code.Substring(1, item.code.Length - 1);
                var cnm = list.Find(x => x.Name.Equals(name));
                if (cnm == null)
                {
                    cnm = new CarNumberModel()
                    {
                        Name = name,
                        Province = item.province,
                        Codes = new List<CarNumberCodeModel>() {
                            new CarNumberCodeModel() {
                                Code = code, City = item.city
                            } }
                    };
                    list.Add(cnm);
                }
                else
                {
                    var cncm = cnm.Codes.Find(x => x.Code.Equals(code));
                    if (cncm == null)
                    {
                        cnm.Codes.Add(new CarNumberCodeModel()
                        {
                            Code = code,
                            City = item.city
                        });
                    }
                }
            }
            string newJsonPath = $@"{hostingEnvironment.ContentRootPath}/wwwroot/account/car_number_new.json";
            string json = JsonConvert.SerializeObject(list);
            System.IO.File.WriteAllText(newJsonPath, json);
            return json;
        }

        public string CallMany()
        {
            var mongo = new MongoDBTool();

            var company = mongo.GetMongoCollection<CompanyModel>().Find(x => x.uniacid.Equals("24")).FirstOrDefault();
            if (company == null || company.YTX == null)
            {
                var em = new ExceptionModel()
                {
                    Content = "商户未设置云通信信息"
                };
                em.Save();
                return JsonResponseModel.ErrorJson;
            }


            string jsonData = JsonConvert.SerializeObject(new MeetingPhoneModel()
            {
                appid = company.YTX.AppID,
                creator = "15501022450",
                meetingname = "ceshi",
                parties = new Party[] { new Party() { name = "22", phone = "13245437784" } }

            }
      );
            string url = "/call/TeleMeeting.wx";

            string result = new PhoneHelper(company.YTX).SendRequest(url, jsonData);
            var jObj = (JObject)JsonConvert.DeserializeObject(result);
            JToken statuscode;
            bool hasVal = jObj.TryGetValue("statusCode", out statuscode);
            if (hasVal && statuscode.ToString().Equals("0"))
            {
                return JsonResponseModel.SuccessJson;
            }
            return JsonResponseModel.ErrorJson;
        }


        public async Task<string> FS()
        {
            var mongo = new MongoDBTool().GetMongoDatabase();
            var bucket = new GridFSBucket(mongo);
            string newJsonPath = $@"{hostingEnvironment.ContentRootPath}/wwwroot/account/car_number_new.json";
            await bucket.UploadFromBytesAsync(Path.GetFileNameWithoutExtension(newJsonPath), System.IO.File.ReadAllBytes(newJsonPath));
            return JsonResponseModel.SuccessJson;
        }
        public async Task<IActionResult> FSDown()
        {
            var mongo = new MongoDBTool().GetMongoDatabase();
            var bucket = new GridFSBucket(mongo);
            string newJsonPath = $@"{hostingEnvironment.ContentRootPath}/wwwroot/account/car_number_new.json";
            byte[] data = await bucket.DownloadAsBytesByNameAsync("car_number_new");
            return File(data,"application/json", "car_number_new.json");
        }

    }

    public class CarNumberOld
    {

        public string code { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string Pcode { get; set; }
    }


}
