using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Infosys.ATR.UIAutomation.Recorder.ScreenCapture
{

    public class ScreenShot
    {

        public static bool SaveToClipboard = false;
        //public static Image CapturedImage { get; set; }

        #region old code

        //public static void CaptureImage(bool showCursor, Size curSize, Point curPos, Point SourcePoint, Point DestinationPoint, Rectangle SelectionRectangle, string FilePath, string extension)
        //{

        //    using (Bitmap bitmap = new Bitmap(SelectionRectangle.Width, SelectionRectangle.Height))
        //    {

        //        using (Graphics g = Graphics.FromImage(bitmap))
        //        {

        //            g.CopyFromScreen(SourcePoint, DestinationPoint, SelectionRectangle.Size);

        //            if (showCursor)
        //            {

        //                Rectangle cursorBounds = new Rectangle(curPos, curSize);
        //                Cursors.Default.Draw(g, cursorBounds);

        //            }

        //        }

        //        if (saveToClipboard)
        //        {

        //            Image img = (Image)bitmap;
        //            Clipboard.SetImage(img);

        //        }
        //        else
        //        {

        //            switch (extension)
        //            {
        //                case ".bmp":
        //                    bitmap.Save(FilePath, ImageFormat.Bmp);
        //                    break;
        //                case ".jpg":
        //                    bitmap.Save(FilePath, ImageFormat.Jpeg);
        //                    break;
        //                case ".gif":
        //                    bitmap.Save(FilePath, ImageFormat.Gif);
        //                    break;
        //                case ".tiff":
        //                    bitmap.Save(FilePath, ImageFormat.Tiff);
        //                    break;
        //                case ".png":
        //                    bitmap.Save(FilePath, ImageFormat.Png);
        //                    break;
        //                default:
        //                    bitmap.Save(FilePath, ImageFormat.Jpeg);
        //                    break;
        //            }

        //        }

        //    }

        //}

        #endregion

        public static void CaptureImageToClipBorad(bool showCursor, Size curSize, Point curPos, Point SourcePoint, Point DestinationPoint, Rectangle SelectionRectangle)
        {

            if (SelectionRectangle.Width == 0 || SelectionRectangle.Height == 0)
                SelectionRectangle.Width = SelectionRectangle.Height =1;
            using (Bitmap bitmap = new Bitmap(SelectionRectangle.Width, SelectionRectangle.Height))
            {

                using (Graphics g = Graphics.FromImage(bitmap))
                {

                    g.CopyFromScreen(SourcePoint, DestinationPoint, SelectionRectangle.Size);

                    if (showCursor)
                    {

                        Rectangle cursorBounds = new Rectangle(curPos, curSize);
                        Cursors.Default.Draw(g, cursorBounds);

                    }

                }

                //CapturedImage = bitmap as Image;

                if (SaveToClipboard)
                {

                    Image img = (Image)bitmap;
                    Clipboard.SetImage(img);

                }
            }
            //return CapturedImage as Image;

        }
    }
}