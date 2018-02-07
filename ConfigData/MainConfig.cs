using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigData
{
    public class MainConfig
    {
        public const bool IsDev = false;

        public const string BaseDir = "/home/project_data/"+(IsDev? "qr_move_car/" : "QRMoveCar/");
        public const string AvatarDir = "avatar/";
        public const string TempDir = "temp/";
        public const string AlbumDir = "album/";
        public const string GoodsImagesDir = "goods_images/";
        public const string LogoImagesDir = "logos/";
        public const string CertsDir="certs/";


        public const string MongoDBLineConn = "mongodb://47.94.208.29:27027";
        public const string MongoDBLocalConn = "mongodb://localhost:27027";
        public const string MongoDBName= IsDev? "qr_move_car_wxxcx" : "QRMoveCarWeChatMP";

          }
}
