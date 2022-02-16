
/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.OCRWrapper;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using tessnet2;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace Infosys.ATR.TessaractWrapper
{
    public class TessaractWrapper : ITextRecognition
    {
        /// <summary>
        /// Reads text from an image. Image is captured using the input parameters provided
        /// </summary>
        /// <param name="x">X coordinate on the screen</param>
        /// <param name="y">Y coordinate on the screen</param>
        /// <param name="height">height of the image to be captured</param>
        /// <param name="width">width of the image to be captured</param>
        /// <param name="filter">filter string consisting of characters that could possibly occur in the image. It can be alphanumeric or especial characters too</param>
        /// <param name="imageResizeCoeff">Coefficient used to resize the original image. A positive coeff will result increasing image size and negative coeff will result in decreasing the image size</param>
        /// <returns>Text in the image captured using input parameters</returns>
        public string ReadTextArea(double x, double y, double height, double width, string filter, float imageResizeCoeff)
        {


            //capture image using input parameters
            var image = GetGrayScreenShot(x, y, height, width);



            //Read text from the image
            string text = String.Empty;
            string directory = Directory.GetCurrentDirectory();
            string ocrdll = "";
            string[] filePaths = Directory.GetFiles(directory + @"\OCRWrapper\tessdata", "*.dll");
            if (filePaths.Length > 0)
            {
                ocrdll = filePaths[0];
            }

            if (File.Exists(ocrdll))
            {
                Assembly assembly = Assembly.LoadFile(ocrdll);
                Type classType = assembly.GetType("tessnet2.Tesseract");
                //Type classType = assembly.GetType("Tesseract.TessBaseAPI");
                //Type wordType = assembly.GetType("Word");
                text = String.Empty;
                object obj = Activator.CreateInstance(classType);
                MethodInfo m1 = classType.GetMethod("SetVariable");
                object[] parameters = new object[2];
                parameters[0] = "tessedit_char_whitelist";
                parameters[1] = filter;
                m1.Invoke(obj, parameters);

                MethodInfo m11 = classType.GetMethod("SetVariable");
                object[] parameters1 = new object[2];
                parameters1[0] = "preserve_interword_spaces";
                parameters1[1] = 1;
                m11.Invoke(obj, parameters);

                string language = "eng";

                string directory2 = Directory.GetCurrentDirectory();
                string path = directory2 + @"\OCRWrapper\tessdata";

                object[] parameters2 = new object[3];
                parameters2[0] = path;
                parameters2[1] = language;
                parameters2[2] = false;
                
                //MethodInfo m2 = classType.GetMethod("Init",new [] {typeof(string),typeof(string)});
                MethodInfo m2 = classType.GetMethod("Init");
                m2.Invoke(obj, parameters2);

                
                //Check to resize image +ve/-ve
                if (imageResizeCoeff  >1 || imageResizeCoeff<-1)
                {
                  int incrWidth;
                  int incrHeight;
                  if(imageResizeCoeff > 1)
                  { 
                    incrWidth = (int)(image.Width * imageResizeCoeff);
                    incrHeight = (int)(image.Height * imageResizeCoeff);
                   }
                  else
                  {
                      incrWidth = (int)(image.Width / imageResizeCoeff);
                      incrHeight = (int)(image.Height / imageResizeCoeff);
                  }

                  image = ResizeImage(image, incrWidth, incrHeight);
                  
                }

                object[] parameters3 = new object[2];
                parameters3[0] = image;
                parameters3[1] = Rectangle.Empty;
                MethodInfo m3 = classType.GetMethod("DoOCR");

                dynamic list = m3.Invoke(obj, parameters3);
                //foreach (dynamic o in list)
                for (int i = 0; i < list.Count; i++)
                {
                    text += list[i].Text + " ";
                    //text += o.Text;
                    //}
                  
                }
            }

            return text.Trim();
        }

        /// <summary>
    /// Resize the image to the specified width and height.
    /// </summary>
    /// <param name="image">The image to resize.</param>
    /// <param name="width">The width to resize to.</param>
    /// <param name="height">The height to resize to.</param>
    /// <returns>The resized image.</returns>
    public static Bitmap ResizeImage(Image image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);
        var destImage = new Bitmap(width, height);

        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

        using (var graphics = Graphics.FromImage(destImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using (var wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
            }
        }

        return destImage;
    }
        /// <summary>
        /// Captures Image using the input parameters provided
        /// </summary>
        /// <param name="x">X coordinate on the screen</param>
        /// <param name="y">Y coordinate on the screen</param>
        /// <param name="height">height of the image to be captured</param>
        /// <param name="width">width of the image to be captured</param>       
        /// <returns>Image in the bitmap format</returns>
        public Bitmap GetGrayScreenShot(double X, double Y, double height, double width)
        {
            Bitmap snap = new Bitmap((int)width, (int)height);

            using (var g = Graphics.FromImage(snap))
            {
                g.CopyFromScreen(new Point((int)X, (int)Y), Point.Empty, new Size((int)width, (int)height));
            }

            //Should the image be saved in some directory
            bool SaveImages = CheckSaveOCRImages();
            if (SaveImages)
            {
                string directory = Directory.GetCurrentDirectory();

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string path = directory + @"\TestImages\custom_" + X.ToString() + "_" + Y.ToString() + "_" + height.ToString() + ".jpg";
                //Save image in the path
                snap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            return snap;
        }

        private bool CheckSaveOCRImages()
        {
            bool save = false;
            //Load XMl settings            
            string directory = Directory.GetCurrentDirectory();
            string settingsFilePath = directory + @"\Infosys.ATR.WinUIAutomationRuntimeWrapper.xml";
            if (File.Exists(settingsFilePath))
            {
                XElement settings = XElement.Load(settingsFilePath);
                if (settings != null)
                {
                    IEnumerable<XElement> elements = settings.Elements().Where(e => e.Name.LocalName == "SaveOCRImages");
                    //XElement SaveOCRImages = settings.Elements().Where(e => e.Name.LocalName == "SaveOCRImages").Single();
                    if (elements.Count() != 0)
                    {
                        string val = settings.Elements().Where(e => e.Name.LocalName == "SaveOCRImages").Single().Value;
                        if (val.ToLower() == "true")
                            save = true;
                    }
                }
            }
            return save;
        }
    }
}
