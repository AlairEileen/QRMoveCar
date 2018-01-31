using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace QRDAL
{
   public class QRCodeHelper
    {
        /// <summary>
        /// 将图片按一定的比例压缩
        /// </summary>
        /// <param name="b">资源图片</param>
        /// <param name="destHeight">高度</param>
        /// <param name="destWidth">宽度</param>
        /// <returns></returns>
        public static Bitmap GetThumbnail(Bitmap b, int destHeight, int destWidth)
        {
            System.Drawing.Image imgSource = b;
            System.Drawing.Imaging.ImageFormat thisFormat = imgSource.RawFormat;
            int sW = 0, sH = 0;
            // 按比例缩放    
            int sWidth = imgSource.Width;
            int sHeight = imgSource.Height;
            if (sHeight > destHeight || sWidth > destWidth)
            {
                if ((sWidth * destHeight) > (sHeight * destWidth))
                {
                    sW = destWidth;
                    sH = (destWidth * sHeight) / sWidth;
                }
                else
                {
                    sH = destHeight;
                    sW = (sWidth * destHeight) / sHeight;
                }
            }
            else
            {
                sW = sWidth;
                sH = sHeight;
            }
            Bitmap outBmp = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);
            // 设置画布的描绘质量    
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量    
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            imgSource.Dispose();
            return outBmp;
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content">需要生成二维码的内容</param>
        /// <param name="size">二维码图片长宽大小</param>
        /// <returns></returns>
        public static Bitmap Create(string content, int size)
        {
            try
            {

                IDictionary<EncodeHintType, object> dictionary = new Dictionary<EncodeHintType, object>();
                dictionary.Add(EncodeHintType.DISABLE_ECI, true);
                dictionary.Add(EncodeHintType.WIDTH, size);
                dictionary.Add(EncodeHintType.HEIGHT, size);
                dictionary.Add(EncodeHintType.MARGIN, 0);
                dictionary.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
                dictionary.Add(EncodeHintType.CHARACTER_SET, "UTF-8");

                BitMatrix byteMatrix = new MultiFormatWriter().encode(content, BarcodeFormat.QR_CODE, size, size, dictionary);



                Bitmap bitmap = ToBitmap(byteMatrix);
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static System.Drawing.Bitmap ToBitmap(BitMatrix matrix)
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

        #region 合并用户QR图片和用户头像

        /// <summary>   
        /// 合并用户QR图片和用户头像   
        /// </summary>   
        /// <param name="qrImg">QR图片（二维码图片）</param>   
        /// <param name="headerImg">用户头像</param>   
        /// <param name="n">缩放比例</param>   
        /// <returns></returns>   
        public static Bitmap MergeQrImg(Bitmap qrImg, Bitmap headerImg, double n = 0.23)
        {
            int margin = 10;
            float dpix = qrImg.HorizontalResolution;
            float dpiy = qrImg.VerticalResolution;
            var _newWidth = (10 * qrImg.Width - 46 * margin) * 1.0f / 46;
            var _headerImg = ZoomPic(headerImg, _newWidth / headerImg.Width);
            //处理头像   
            int newImgWidth = _headerImg.Width + margin;
            Bitmap headerBgImg = new Bitmap(newImgWidth, newImgWidth);
            headerBgImg.MakeTransparent();
            Graphics g = Graphics.FromImage(headerBgImg);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            Pen p = new Pen(new SolidBrush(Color.White));
            Rectangle rect = new Rectangle(0, 0, newImgWidth - 1, newImgWidth - 1);
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, 7))
            {
                g.DrawPath(p, path);
                g.FillPath(new SolidBrush(Color.White), path);
            }
            //画头像   
            Bitmap img1 = new Bitmap(_headerImg.Width, _headerImg.Width);
            Graphics g1 = Graphics.FromImage(img1);
            g1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g1.Clear(Color.Transparent);
            Pen p1 = new Pen(new SolidBrush(Color.Gray));
            Rectangle rect1 = new Rectangle(0, 0, _headerImg.Width - 1, _headerImg.Width - 1);
            using (GraphicsPath path1 = CreateRoundedRectanglePath(rect1, 7))
            {
                g1.DrawPath(p1, path1);
                TextureBrush brush = new TextureBrush(_headerImg);
                g1.FillPath(brush, path1);
            }
            g1.Dispose();
            PointF center = new PointF((newImgWidth - _headerImg.Width) / 2, (newImgWidth - _headerImg.Height) / 2);
            g.DrawImage(img1, center.X, center.Y, _headerImg.Width, _headerImg.Height);
            g.Dispose();
            Bitmap backgroudImg = new Bitmap(qrImg.Width, qrImg.Height);
            backgroudImg.MakeTransparent();
            backgroudImg.SetResolution(dpix, dpiy);
            headerBgImg.SetResolution(dpix, dpiy);
            Graphics g2 = Graphics.FromImage(backgroudImg);
            g2.Clear(Color.Transparent);
            g2.DrawImage(qrImg, 0, 0);
            PointF center2 = new PointF((qrImg.Width - headerBgImg.Width) / 2, (qrImg.Height - headerBgImg.Height) / 2);
            g2.DrawImage(headerBgImg, center2);
            g2.Dispose();
            return backgroudImg;
        }

        #endregion

        #region 图形处理

        /// <summary>   
        /// 创建圆角矩形   
        /// </summary>   
        /// <param name="rect">区域</param>   
        /// <param name="cornerRadius">圆角角度</param>   
        /// <returns></returns>   
        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            //下午重新整理下，圆角矩形   
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }

        /// <summary>   
        /// 图片按比例缩放   
        /// </summary>   
        private static Image ZoomPic(Image initImage, double n)
        {
            //缩略图宽、高计算   
            double newWidth = initImage.Width;
            double newHeight = initImage.Height;
            newWidth = n * initImage.Width;
            newHeight = n * initImage.Height;
            //生成新图   
            //新建一个bmp图片   
            System.Drawing.Image newImage = new System.Drawing.Bitmap((int)newWidth, (int)newHeight);
            //新建一个画板   
            System.Drawing.Graphics newG = System.Drawing.Graphics.FromImage(newImage);
            //设置质量   
            newG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            newG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //置背景色   
            newG.Clear(Color.Transparent);
            //画图   
            newG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, newImage.Width, newImage.Height), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
            newG.Dispose();
            return newImage;
        }

        #endregion
    }
}
