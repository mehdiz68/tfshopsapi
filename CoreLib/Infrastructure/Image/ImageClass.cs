using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace CoreLib.Infrastructure.Image
{

    public class ImageClass
    {
        public ImageClass()
        {

        }

        public static Boolean IsFileAnImage(string strFileName)
        {
            Boolean result = true;
            try
            {
                //if (strFileName.IndexOf("http") < 0)
                //{
                //    System.Drawing.Image img = System.Drawing.Image.FromFile(strFileName);
                //    //System.Drawing.Image img = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(strFileName));
                //    img.Dispose();
                //}
                //else
                //    result = true;
                if (strFileName.ToLower().IndexOf(".png") > 0 || strFileName.ToLower().IndexOf(".jpg") > 0 || strFileName.ToLower().IndexOf(".jpeg") > 0 || strFileName.ToLower().IndexOf(".webp") > 0 || strFileName.ToLower().IndexOf(".bmp") > 0 || strFileName.ToLower().IndexOf(".gif") > 0)
                {
                    return true;
                }
                else
                    result = false;

            }
            catch (Exception x)
            {
                result = false;
            }
            return result;
        }

        public static string WaterMarkAndChangeSize(string path, string source_file, bool UseWatermark, bool ChangeSize, bool compression, Int16 compressionLevel, bool LargeWatermark, string WatermarkFileName = "", string WatermarkPosition = "")
        {
            // رفعِ باگِ «The process cannot access the file because it is being used by another process»
            // ---------------------------------------------------------------------------------------
            // نسخه‌ی قبلی تصویرِ مبدأ را با Image.FromFile باز می‌کرد و FromFile تا وقتی Bitmap زنده
            // است یک قفلِ باز روی همان مسیر نگه می‌دارد. بدتر اینکه mSourceImage داخلِ همان بلوکِ
            // using دو بارِ دیگر مقداردهی می‌شد (یک‌بار بعد از فشرده‌سازی روی فایلِ ‎_1‎ و یک‌بار برای
            // Multi Size روی خودِ فایلِ مبدأ) بدونِ اینکه Bitmapِ قبلی Dispose شود؛ یعنی using فقط
            // آخرین مقدار را آزاد می‌کرد و قفلِ دو تای قبلی باز می‌ماند. در انتهای متد
            // File.Delete/File.Move روی همان فایلِ هنوز قفل‌شده اجرا می‌شد و IOException می‌داد. آن
            // خطا به‌صورت رشته برمی‌گشت (نه استثنا) و بالادست به NullReference تبدیل می‌شد.
            // چون آزادسازیِ آن Bitmapهای رهاشده به زمان‌بندیِ GC وابسته بود، خطا متناوب بود - برای
            // همین آپلودِ تصویرِ نظرات گاهی کار می‌کرد و گاهی «خطایی رخ داد».
            //
            // حالا هیچ فایلی با FromFile باز نمی‌شود: بایت‌ها یک‌جا خوانده و از روی MemoryStream دیکد
            // می‌شوند، پس اصلاً قفلی روی دیسک نمی‌ماند. همه‌ی Bitmapها و استریم‌ها هم در finally
            // قطعی آزاد می‌شوند. نامِ فایل‌های خروجی، اندازه‌ها و فرمتشان دقیقاً مثلِ قبل است.
            List<IDisposable> disposables = new List<IDisposable>();
            Bitmap mSourceImage = null;
            string fileName = null;
            try
            {
                mSourceImage = LoadWithoutLockingFile(path + source_file, disposables);

                fileName = path + source_file;
                string extention = source_file.Substring(source_file.LastIndexOf(".")).ToLower();
                // save resized image to destination Folder
                if (UseWatermark)
                {
                    if (mSourceImage.Width >= 1200 && mSourceImage.Height >= 1200)
                    {
                        Graphics g = Graphics.FromImage(mSourceImage);
                        Bitmap WaterMarkPic = LoadWithoutLockingFile(@"C:\inetpub\wwwroot\api.tfshops.com\Content\UploadFiles\" + WatermarkFileName, disposables);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        #region Rezie Watermak if Needed
                        if (WaterMarkPic.Width >= mSourceImage.Width || WaterMarkPic.Height >= mSourceImage.Height)
                        {
                            //decrease To 0.3 of Source Image
                            WaterMarkPic = Track(resizeImage(WaterMarkPic, new Size(Convert.ToInt32(Math.Ceiling(mSourceImage.Width * 0.3)), Convert.ToInt32(Math.Ceiling(mSourceImage.Height * 0.3))), true, extention), disposables);
                        }
                        else
                        {
                            double ScaleX = mSourceImage.Width / WaterMarkPic.Width;
                            double ScaleY = mSourceImage.Height / WaterMarkPic.Width;
                            if (ScaleX < 3 || ScaleY < 3)
                            {
                                //decrease To 0.3 of Source Image
                                WaterMarkPic = Track(resizeImage(WaterMarkPic, new Size(Convert.ToInt32(Math.Ceiling(mSourceImage.Width * 0.3)), Convert.ToInt32(Math.Ceiling(mSourceImage.Height * 0.3))), true, extention), disposables);
                            }

                        }
                        #endregion
                        #region Get X,Y For Start Position
                        int startX = 0, startY = 0;
                        if (WatermarkPosition != "")
                            startX = CalculateWatermarkPosition(mSourceImage.Width, mSourceImage.Height, Convert.ToInt32(WatermarkPosition), WaterMarkPic.Width, WaterMarkPic.Height, out startY);
                        #endregion
                        #region Save Watermark Temporary
                        g.DrawImage(WaterMarkPic, new Point(startX, startY));
                        g.Save();
                        WaterMarkPic.Dispose();
                        g.Dispose();
                        string fileOut = Path.Combine(path, source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf(".")));
                        using (FileStream ms = new FileStream(fileOut, FileMode.Create, FileAccess.Write))
                        {
                            mSourceImage.Save(ms, mSourceImage.RawFormat);
                            ms.Flush();
                        }
                        #endregion
                    }
                    else
                        UseWatermark = false;

                }


                #region compression
                if (compression)
                {
                    if (extention == ".jpg" || extention == ".jpeg" || extention == ".png")
                    {
                        Compress(mSourceImage, path, source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf(".")), compressionLevel);
                        // Bitmapِ قبلی همین‌جا آزاد می‌شود؛ قبلاً فقط جایگزین می‌شد و قفلش می‌ماند
                        mSourceImage.Dispose();
                        mSourceImage = LoadWithoutLockingFile(path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf(".")), disposables);
                    }
                }
                #endregion
                #region Multi Size
                if (ChangeSize)
                {
                    if (LargeWatermark)
                    {
                        mSourceImage.Dispose();
                        mSourceImage = LoadWithoutLockingFile(path + source_file, disposables);
                    }
                    switch (extention)
                    {
                            case ".jpg":
                            case ".jpeg":
                                fileName = path + "MD_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 2, mSourceImage.Height / 2, extention, fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = path + "SM_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 4, mSourceImage.Height / 4, extention, fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = path + "XS_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 8, mSourceImage.Height / 8, extention, fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            case ".png":
                                fileName = path + "MD_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 2, mSourceImage.Height / 2, extention, fileName, System.Drawing.Imaging.ImageFormat.Png);
                                fileName = path + "SM_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 4, mSourceImage.Height / 4, extention, fileName, System.Drawing.Imaging.ImageFormat.Png);
                                fileName = path + "XS_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 8, mSourceImage.Height / 8, extention, fileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                            case ".gif":
                                fileName = path + "MD_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 2, mSourceImage.Height / 2, extention, fileName, System.Drawing.Imaging.ImageFormat.Gif);
                                fileName = path + "SM_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 4, mSourceImage.Height / 4, extention, fileName, System.Drawing.Imaging.ImageFormat.Gif);
                                fileName = path + "XS_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 8, mSourceImage.Height / 8, extention, fileName, System.Drawing.Imaging.ImageFormat.Gif);
                                break;
                            case ".bmp":
                                fileName = path + "MD_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 2, mSourceImage.Height / 2, extention, fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                fileName = path + "SM_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 4, mSourceImage.Height / 4, extention, fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                fileName = path + "XS_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 8, mSourceImage.Height / 8, extention, fileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                            case ".tif":
                                fileName = path + "MD_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 2, mSourceImage.Height / 2, extention, fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                                fileName = path + "SM_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 4, mSourceImage.Height / 4, extention, fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                                fileName = path + "XS_" + source_file.Substring(3);
                                SaveResized(mSourceImage, mSourceImage.Width / 8, mSourceImage.Height / 8, extention, fileName, System.Drawing.Imaging.ImageFormat.Tiff);
                                break;
                            default:
                                fileName = HttpContext.Current.Server.MapPath(path + "MD_" + source_file.Substring(3));
                                SaveResized(mSourceImage, mSourceImage.Width / 2, mSourceImage.Height / 2, extention, fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "SM_" + source_file.Substring(3));
                                SaveResized(mSourceImage, mSourceImage.Width / 4, mSourceImage.Height / 4, extention, fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                fileName = HttpContext.Current.Server.MapPath(path + "XS_" + source_file.Substring(3));
                                SaveResized(mSourceImage, mSourceImage.Width / 8, mSourceImage.Height / 8, extention, fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;

                    }

                }
                #endregion
                if (UseWatermark || compression)
                {
                    #region Save Final Image
                    mSourceImage.Dispose();
                    string tempFile = path + source_file.Substring(0, source_file.LastIndexOf(".")) + "_1" + source_file.Substring(source_file.LastIndexOf("."));
                    // اگر نه واترمارک خورده و نه فشرده‌سازی انجام شده (مثلاً gif/bmp با compression
                    // روشن، یا تصویرِ کوچک‌تر از ۱۲۰۰ که واترمارک نمی‌خورد) فایلِ ‎_1‎ اصلاً ساخته
                    // نشده؛ قبلاً همین‌جا File.Move روی یک فایلِ ناموجود می‌خورد و کلِ آپلود شکست
                    // می‌خورد. حالا در آن حالت فایلِ اصلی دست‌نخورده می‌ماند.
                    if (File.Exists(tempFile))
                    {
                        File.Delete(path + source_file);
                        File.Move(tempFile, path + source_file);
                    }

                    #endregion
                }
                else
                    mSourceImage.Dispose();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                // آزادسازیِ قطعی - برعکسِ ترتیبِ ساخت، تا هر Bitmap قبل از استریمِ خودش بسته شود
                for (int d = disposables.Count - 1; d >= 0; d--)
                {
                    try { disposables[d].Dispose(); }
                    catch { }
                }
            }
        }

        /// <summary>
        /// تصویر را بدونِ قفل‌کردنِ فایل روی دیسک باز می‌کند.
        /// Image.FromFile قفلِ فایل را تا زمانِ Dispose شدنِ Bitmap نگه می‌دارد؛ این‌جا بایت‌ها
        /// یک‌جا خوانده می‌شوند و تصویر از روی حافظه دیکد می‌شود، پس فایل بلافاصله آزاد است.
        /// Image.FromStream تنبل می‌خواند و استریم باید تا پایانِ عمرِ Bitmap باز بماند، برای همین
        /// استریم هم در همان لیستِ آزادسازی نگه داشته می‌شود.
        /// </summary>
        private static Bitmap LoadWithoutLockingFile(string fullPath, List<IDisposable> disposables)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(fullPath));
            disposables.Add(ms);
            return Track((Bitmap)System.Drawing.Image.FromStream(ms), disposables);
        }

        /// <summary>یک Bitmap را برای آزادسازیِ قطعی در finally ثبت می‌کند.</summary>
        private static Bitmap Track(Bitmap bmp, List<IDisposable> disposables)
        {
            disposables.Add(bmp);
            return bmp;
        }

        /// <summary>
        /// نسخه‌ی تغییرِ اندازه‌یافته را ذخیره و بلافاصله آزاد می‌کند.
        /// قبلاً خروجیِ resizeImage هیچ‌وقت Dispose نمی‌شد و به‌ازای هر تصویر سه هندلِ GDI نشت
        /// می‌کرد - زیرِ بار همین باعثِ خطاهای «Generic error in GDI+» می‌شود.
        /// </summary>
        private static void SaveResized(Bitmap source, int width, int height, string extention, string fileName, ImageFormat format)
        {
            using (Bitmap resized = resizeImage(source, new Size(width, height), false, extention))
            {
                resized.Save(fileName, format);
            }
        }


        public static Bitmap resizeImageAspectRatio(Bitmap imgToResize, int Width, int Height, string extention)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentH;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            return ImageClass.resizeImage(imgToResize, new Size() { Width = destWidth, Height = destHeight }, false, extention); ;

        }

        public static Bitmap resizeImage(Bitmap imgToResize, Size size, bool ForWatermakr, string extention)
        {
            // Get the image's original width and height
            int originalWidth = imgToResize.Width;
            int originalHeight = imgToResize.Height;

            // To preserve the aspect ratio
            float ratioX = (float)size.Width / (float)originalWidth;
            float ratioY = (float)size.Height / (float)originalHeight;
            //float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratioX);
            int newHeight = (int)(originalHeight * ratioY);

            // Convert other formats (including CMYK) to RGB
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            newImage.SetResolution(72, 72);
            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.DrawImage(imgToResize, 0, 0, newWidth, newHeight);
            }
            if (ForWatermakr)
                imgToResize.Dispose();

            // Get an ImageCodecInfo object that represents .
            //ImageCodecInfo imageCodecInfo;
            //switch (extention)
            //{
            //    case ".jpg":
            //    case ".jpeg":
            //         imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
            //        break;
            //    case ".png":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Png);
            //        break;
            //    case ".gif":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Gif);
            //        break;
            //    case ".bmp":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Bmp);
            //        break;
            //    case ".tif":
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Tiff);
            //        break;
            //    default:
            //        imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);
            //        break;
            //}

            // Create an Encoder object for the Quality parameter.
            //Encoder encoder = Encoder.Quality;

            //// Create an EncoderParameters object. 
            //EncoderParameters encoderParameters = new EncoderParameters(1);

            //// Save the image as a JPEG file with quality level.
            //EncoderParameter encoderParameter = new EncoderParameter(encoder, 1);
            //encoderParameters.Param[0] = encoderParameter;
            ////newImage.Save(filePath, imageCodecInfo, encoderParameters);
            return newImage;

            //int sourceWidth = imgToResize.Width;
            //int sourceHeight = imgToResize.Height;

            ////float nPercent = 0;
            //float nPercentW = 0;
            //float nPercentH = 0;

            //nPercentW = ((float)size.Width / (float)sourceWidth);
            //nPercentH = ((float)size.Height / (float)sourceHeight);

            ////if (nPercentH < nPercentW)
            ////    nPercent = nPercentH;
            ////else
            ////    nPercent = nPercentW;

            //int destWidth = (int)(sourceWidth * nPercentW);
            //int destHeight = (int)(sourceHeight * nPercentH);

            //Bitmap b = new Bitmap(destWidth, destHeight);
            //Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            //g.InterpolationMode = InterpolationMode.Default;

            //g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            //if(ForWatermakr)
            //    imgToResize.Dispose();
            //g.Dispose();

            //return b;
        }
        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        private static int CalculateWatermarkPosition(int imageWidth, int imageHeight, int WatermarkPosition, int watermarkImageWidth, int watermarkImageHeight, out int y)
        {
            try
            {
                switch (WatermarkPosition)
                {
                    // Y=ImageHeight-WaterMarkHeight- 10% Of ImageHeight , X= 2% of ImageWidth 
                    case 0: y = imageHeight - watermarkImageHeight - Convert.ToInt32(imageHeight * 0.02); return Convert.ToInt32(imageWidth * 0.02);
                    // Y=ImageHeight-WaterMarkHeight- 10% Of ImageHeight , X=(ImageWidth / 2) - (watermarkImageWidth / 2)
                    case 1: y = imageHeight - watermarkImageHeight - Convert.ToInt32(imageHeight * 0.02); return (imageWidth / 2) - (watermarkImageWidth / 2);
                    //Y=ImageHeight-WaterMarkHeight- 10% Of ImageHeight , X=(ImageWidth / 2) - (watermarkImageWidth / 2) - 2% of ImageWidth 
                    case 2: y = imageHeight - watermarkImageHeight - Convert.ToInt32(imageHeight * 0.02); return imageWidth - watermarkImageWidth - Convert.ToInt32(imageWidth * 0.02);
                    // Y=(imageHeight / 2) - (watermarkImageHeight / 2) , X= 2% of ImageWidth 
                    case 3: y = (imageHeight / 2) - (watermarkImageHeight / 2); return Convert.ToInt32(imageWidth * 0.02);
                    // Y=(ImageHeight / 2) - (watermarkImageHeight / 2) , X= (ImageWidth / 2) - (watermarkImageWidth / 2) 
                    case 4: y = (imageHeight / 2) - (watermarkImageHeight / 2); return (imageWidth / 2) - (watermarkImageWidth / 2);
                    //Y=Y=(ImageHeight / 2) - (watermarkImageHeight / 2) , X=(ImageWidth / 2) - (watermarkImageWidth / 2) - 2% of ImageWidth 
                    case 5: y = (imageHeight / 2) - (watermarkImageHeight / 2); return imageWidth - watermarkImageWidth - Convert.ToInt32(imageWidth * 0.02);
                    // Y=ImageHeight * 0.02 , X= ImageWidth * 0.02
                    case 6: y = Convert.ToInt32(imageHeight * 0.02); return Convert.ToInt32(imageWidth * 0.02);
                    // Y=ImageHeight * 0.02 ,  X=(ImageWidth / 2) - (watermarkImageWidth / 2)
                    case 7: y = Convert.ToInt32(imageHeight * 0.02); return (imageWidth / 2) - (watermarkImageWidth / 2);
                    // Y=ImageHeight * 0.02 ,  X=X=(ImageWidth / 2) - (watermarkImageWidth / 2) - 2% of ImageWidth
                    case 8: y = Convert.ToInt32(imageHeight * 0.02); return imageWidth - watermarkImageWidth - Convert.ToInt32(imageWidth * 0.02);
                    default: y = 1; return 1;
                }
            }
            catch (Exception)
            {
                y = 0;
                return 0;
            }
        }

        static void Compress(System.Drawing.Image original, string path, string filename, int compressionLevel)
        {
            ImageCodecInfo jpgEncoder = null;
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == ImageFormat.Jpeg.Guid)
                {
                    jpgEncoder = codec;
                    break;
                }
            }
            if (jpgEncoder != null)
            {
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters encoderParameters = new EncoderParameters(1);

                EncoderParameter encoderParameter;
                switch (compressionLevel)
                {
                    case 1: encoderParameter = new EncoderParameter(encoder, 5L); break;
                    case 2: encoderParameter = new EncoderParameter(encoder, 10L); break;
                    case 3: encoderParameter = new EncoderParameter(encoder, 15L); break;
                    case 4: encoderParameter = new EncoderParameter(encoder, 20L); break;
                    case 5: encoderParameter = new EncoderParameter(encoder, 25L); break;
                    case 6: encoderParameter = new EncoderParameter(encoder, 30L); break;
                    case 7: encoderParameter = new EncoderParameter(encoder, 35L); break;
                    case 8: encoderParameter = new EncoderParameter(encoder, 40L); break;
                    case 9: encoderParameter = new EncoderParameter(encoder, 100L); break;
                    default:
                        encoderParameter = new EncoderParameter(encoder, 40L);
                        break;
                }
                encoderParameters.Param[0] = encoderParameter;

                string fileOut = Path.Combine(path, filename);
                FileStream ms = new FileStream(fileOut, FileMode.Create, FileAccess.Write);
                original.Save(ms, jpgEncoder, encoderParameters);
                //Bitmap compressed = new Bitmap(ms);
                ms.Flush();
                ms.Close();
            }

        }
    }
}
