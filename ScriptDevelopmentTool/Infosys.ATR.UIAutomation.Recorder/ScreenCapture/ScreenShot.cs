/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Infosys.ATR.UIAutomation.Recorder.ScreenCapture
{

    class ScreenShot
    {

        public static bool saveToClipboard = false;

        public static void CaptureImage(bool showCursor, Size curSize, Point curPos, Point SourcePoint, Point DestinationPoint, Rectangle SelectionRectangle, string FilePath, string extension)
        {
            if (SelectionRectangle.Width == 0 || SelectionRectangle.Height == 0)
                SelectionRectangle.Width = SelectionRectangle.Height = 1;
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

                if (saveToClipboard)
                {

                    Image img = (Image)bitmap;
                    Clipboard.SetImage(img);

                }
                else
                {

                    switch (extension)
                    {
                        case ".bmp":
                            bitmap.Save(FilePath, ImageFormat.Bmp);
                            break;
                        case ".jpg":
                            bitmap.Save(FilePath, ImageFormat.Jpeg);
                            break;
                        case ".gif":
                            bitmap.Save(FilePath, ImageFormat.Gif);
                            break;
                        case ".tiff":
                            bitmap.Save(FilePath, ImageFormat.Tiff);
                            break;
                        case ".png":
                            bitmap.Save(FilePath, ImageFormat.Png);
                            break;
                        default:
                            bitmap.Save(FilePath, ImageFormat.Jpeg);
                            break;
                    }

                }

            }

        }
    }
}