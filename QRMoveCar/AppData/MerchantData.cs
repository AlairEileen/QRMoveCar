using ConfigData;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using QRMoveCar.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YTXDAL;

namespace QRMoveCar.AppData
{
    public class MerchantData : BaseData<CompanyModel>
    {
        internal void SetQiNiu(string uniacid, QiNiuModel qiNiuModel)
        {
            var companyCollection = mongo.GetMongoCollection<CompanyModel>();
            var company = companyCollection.Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            if (company == null)
            {
                companyCollection.InsertOne(new CompanyModel()
                {
                    uniacid = uniacid,
                    QiNiuModel = qiNiuModel
                });
            }
            else
            {
                companyCollection.UpdateOne(x => x.uniacid.Equals(uniacid), Builders<CompanyModel>.Update.Set(x => x.QiNiuModel, qiNiuModel));
            }
        }



        internal List<OrderViewModel> GetOrderViewList(string uniacid)
        {
            var list = mongo.GetMongoCollection<AccountModel>().Find(x => x.uniacid.Equals(uniacid)).ToList();
            List<OrderViewModel> tHList = new List<OrderViewModel>();
            list.ForEach(x =>
            {
                if (x.Orders != null)
                {
                    x.Orders.ForEach(y =>
                    {
                        if (y.IsPaid)
                        {
                            var thvm = new OrderViewModel()
                            {
                                AccountAvatar = x.AccountAvatar,
                                AccountName = x.AccountName,
                                CarNumber = x.CarNumber,
                                OrderInfo = y,
                                uniacid = x.uniacid,
                                AccountID = x.AccountID.ToString()
                            };
                            tHList.Add(thvm);
                        }
                    });
                }
            });
            tHList.Sort((x, y) => -x.OrderInfo.CreateTime.CompareTo(y.OrderInfo.CreateTime));
            return tHList;
        }

        internal List<AccountModel> GetAccountList(string uniacid)
        {
            var list = mongo.GetMongoCollection<AccountModel>().Find(x => x.uniacid.Equals(uniacid)).ToList();
            return list;
        }

        internal List<FeedbackModel> GetFeedbackList(string uniacid)
        {
            var list = mongo.GetMongoCollection<FeedbackModel>().Find(x => x.uniacid.Equals(uniacid)).ToList();
            return list;
        }

        internal CompanyModel GetCompanyModel(string uniacid)
        {
            var companyModel = mongo.GetMongoCollection<CompanyModel>().Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            return companyModel;
        }

        internal void SetQRSendFee(string uniacid, decimal qRSendFee)
        {
            var companyCollection = mongo.GetMongoCollection<CompanyModel>();
            var company = companyCollection.Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            if (company == null)
            {
                companyCollection.InsertOne(new CompanyModel()
                {
                    uniacid = uniacid,
                    QRSendFee = qRSendFee
                });
            }
            else
            {
                companyCollection.UpdateOne(x => x.uniacid.Equals(uniacid), Builders<CompanyModel>.Update.Set(x => x.QRSendFee, qRSendFee));
            }
        }

        internal void PushCert(string uniacid, IFormFile file)
        {
            long size = 0;
            var filename = ContentDispositionHeaderValue
                                  .Parse(file.ContentDisposition)
                                  .FileName
                                  .Trim('"');
            string dbSaveDir = $@"{MainConfig.CertsDir}{uniacid}/";
            string saveDir = $@"{MainConfig.BaseDir}{dbSaveDir}";
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            string[] files = Directory.GetFiles(saveDir);
            foreach (var item in files)
            {
                File.Delete(item);
            }

            string exString = filename.Substring(filename.LastIndexOf("."));
            string saveName = Guid.NewGuid().ToString("N");
            filename = $@"{saveDir}{saveName}{exString}";

            size += file.Length;
            using (FileStream fs = System.IO.File.Create(filename))
            {
                file.CopyTo(fs);
                fs.Flush();
                string[] fileUrls = new string[] { $@"{dbSaveDir}{saveName}{exString}" };
                //FileManager.Exerciser(uniacid, filename, null).SaveFile();

                var companyCollection = mongo.GetMongoCollection<CompanyModel>();
                var company = companyCollection.Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
                if (company == null)
                {
                    companyCollection.InsertOne(new CompanyModel()
                    {
                        uniacid = uniacid,
                        CertFileName = $"{saveName}{exString}"
                    });
                }
                else
                {
                    companyCollection.UpdateOne(x => x.uniacid.Equals(uniacid), Builders<CompanyModel>.Update.Set(x => x.CertFileName, $"{saveName}{exString}"));
                }
            }
        }

        internal void SendOrder(string uniacid, ObjectId accountID, Order order)
        {
          
            var accountCollection = mongo.GetMongoCollection<AccountModel>();
            var filter = Builders<AccountModel>.Filter;
            var filterSum = filter.Eq("Orders.OrderID", order.OrderID)&filter.Eq(x=>x.uniacid,uniacid);
            filterSum = filterSum & filter.Eq(x=>x.AccountID, accountID);
            var update = Builders<AccountModel>.Update.Set("Orders.$.Logistics", order.Logistics);
            accountCollection.UpdateOne(filterSum,update);
        }

        internal void SetYTX(string uniacid, YTXModel qiNiuModel)
        {
            var companyCollection = mongo.GetMongoCollection<CompanyModel>();
            var company = companyCollection.Find(x => x.uniacid.Equals(uniacid)).FirstOrDefault();
            if (company == null)
            {
                companyCollection.InsertOne(new CompanyModel()
                {
                    uniacid = uniacid,
                    YTX = qiNiuModel
                });
            }
            else
            {
                companyCollection.UpdateOne(x => x.uniacid.Equals(uniacid), Builders<CompanyModel>.Update.Set(x => x.YTX, qiNiuModel));
            }
        }
    }
}
