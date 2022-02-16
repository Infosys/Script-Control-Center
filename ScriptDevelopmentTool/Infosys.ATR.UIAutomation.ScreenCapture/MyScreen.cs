/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Infosys.ATR.UIAutomation.UserActivityMonitor;
using Infosys.ATR.UIAutomation.ScreenCapture.Interface;

namespace Infosys.ATR.UIAutomation.ScreenCapture
{
    public class MyScreen:IMyScreen
    {

        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        //[DllImport("User32.dll")]
        //public static extern void ReleaseDC(IntPtr dc);

        [DllImport("User32.dll")]
        public extern static int ReleaseDC(System.IntPtr hWnd, System.IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        const int RDW_INVALIDATE = 0x0001;
        const int RDW_ALLCHILDREN = 0x0080;
        const int RDW_UPDATENOW = 0x0100;

        [DllImport("User32.dll")]
        static extern bool RedrawWindow(IntPtr hwnd, IntPtr rcUpdate, IntPtr regionUpdate, int flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InvalidateRect(IntPtr hWnd, ref Rectangle lpRect,bool bErase);


        //####################### Screen Capture Related declaration################################

         bool isStartClick = false;
         bool isEndclick = false;
         Point startPoint = new Point();
         Point endPoint = new Point();

         int rectangleHeight;
         int rectangleWidth;


         static int screenHeight = SystemInformation.VirtualScreen.Height;
         static int screenWidth = SystemInformation.VirtualScreen.Width;

         static Bitmap snap = new Bitmap(screenWidth, screenHeight);

         Graphics g = Graphics.FromImage(snap);
         Pen MyPen = new Pen(Color.Black, 1);      
         SolidBrush TransparentBrush = new SolidBrush(Color.White);
         Pen EraserPen = new Pen(Color.FromArgb(255, 255, 192), 1);
         SolidBrush eraserBrush = new SolidBrush(Color.FromArgb(255, 255, 192));
         Rectangle selRectangle = new Rectangle();

         bool rectangleDrawn = false;

         int tempStartPtX;
         int tempStartPtY;

         bool isAltPressed = false;
         bool canEndCaptureRaised = false;

        //####################### End #############################################################


         public event EventHandler <ScreenEventArgs> StartCapture;
         public event EventHandler<ScreenEventArgs> EndCapture;
         //public event EventHandler<ScreenEventArgs> CancelCapture;

         private string capturedImagePath = string.Empty;
         public string CapturedImagePath
         {
             get
             {
                 return capturedImagePath;
             }
             set
             {
                 capturedImagePath = value;
             }

         }

         private string imageName = string.Empty;
         public string ImageName
         {

             get
             {
                 return imageName;
             }
             set
             {
                 imageName = value;
             }
         
         }

        public void SubscribeScreenCapture(string folderPath )
        {
            //set folder path where captured images to be saved
            CapturedImagePath = folderPath;

            GlobalEventHandler.MouseDownEvents += HookManager_MouseDown;
            GlobalEventHandler.KeyDownEvents += HookManager_KeyDown;
            GlobalEventHandler.KeyUpEvents += HookManager_KeyUp;
        }
        public void UnSubscribeScreenCapture()
        {
            GlobalEventHandler.MouseDownEvents -= HookManager_MouseDown;
            GlobalEventHandler.KeyDownEvents -= HookManager_KeyDown;
            GlobalEventHandler.KeyUpEvents -= HookManager_KeyUp;
        }

        ///// <summary>
        ///// to do preprocessing before raise cancelCapture event
        ///// </summary>
        //private void CancelCaptureEvent()
        //{ 
        //    if(((!rectangleDrawn) && (isStartClick)) ||  ((rectangleDrawn) && (isEndclick)))
        //    {

        //        isStartClick = false;
        //        isEndclick = false;
        //        rectangleDrawn = false;
        //        CancelCapture(this, new ScreenEventArgs());
        //    }
        //}

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode.ToString() == "LMenu") || (e.KeyCode.ToString() == "RMenu"))
            {
                isAltPressed = true;
            }
            else
            {
                CancelScreenCaptureArea();
            }
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode.ToString() == "LMenu") || (e.KeyCode.ToString() == "RMenu"))
            {
                isAltPressed = false;

            }   
        }


        private void btnEraseRectangle_Click(object sender, EventArgs e)
        {
            RegisterDeRegiterMouseClickEvent(false);
            //if (Keyboard.IsKeyDown(Key.LeftAlt))
            //{
            //    MessageBox.Show("letf Alt key pressed");
            //}
            //else
            //{ 
            //}

            //if (Keyboard.GetKeyStates(Key.LeftAlt) == KeyStates.Down)
            //{
            //    MessageBox.Show("letf Alt key down");
            //}
            //else if (Keyboard.GetKeyStates(Key.LeftAlt) == KeyStates.None)
            //{
            //    MessageBox.Show("letf Alt key None");
            //}
            //else if (Keyboard.GetKeyStates(Key.LeftAlt) == KeyStates.Toggled)
            //{
            //    MessageBox.Show("letf Alt key toggled");
            //}

            //DrawEraseRectangle(true);
            
        }

        private void RegisterDeRegiterMouseClickEvent(bool isRegister)
        {
            if (isRegister)
            {
                GlobalEventHandler.MouseDownEvents += HookManager_MouseDown;
            }
            else
            {
                GlobalEventHandler.MouseDownEvents -= HookManager_MouseDown;
            }
        }

        private void HookManager_MouseDown(object sender, MouseEventArgs e)
        {

            ProcessMouseDownEvent(e);
        }

        private void CancelScreenCaptureArea()
        {
            if (rectangleDrawn) //it means  third click so rectangle defined and image already captured :  so cancel drawn rectangle and reset var to start again
            {
                DrawEraseRectangle(selRectangle, false);
                rectangleDrawn = false;
            }
            else // means second  click  : cancel rectangle to be redrawn again
            {
                isStartClick = false;
                isEndclick = false;
                rectangleDrawn = false;

                ScreenEventArgs eArg = new ScreenEventArgs();
                //raise event to indicate that capture module has finished capture
                if (canEndCaptureRaised)
                {
                    EndCapture(this, eArg);
                    canEndCaptureRaised = false;
                }
            }
            
        }


        /// <summary>
        /// Process mouse click event
        /// </summary>
        /// <param name="e"></param>
        private void ProcessMouseDownEvent(MouseEventArgs e)
        {
            //if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftAlt) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightAlt))
            if ((isAltPressed) && (e.Button == MouseButtons.Left))
            {
                DefineScreenCaptureArea(e);
            }
            else  //if any mouse button pressed clean it
            {
                CancelScreenCaptureArea();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void InvalidateDefinedRectangle()
        {
            rectangleDrawn = false;
        }

        /// <summary>
        /// capture mouse click and define rectangle screen capture area
        /// </summary>
        /// <param name="e"></param>
        void DefineScreenCaptureArea(MouseEventArgs e)
        {

            if (rectangleDrawn) //erase already drawn rectangle
            {
                //Erase the previous rectangle
                //g.DrawRectangle(EraserPen, selRectangle);
                DrawEraseRectangle(selRectangle, false);
            }

            if (isStartClick == false)
            {
                //raise event to indicate that capture module has invoked and started capture
                StartCapture(this, new ScreenEventArgs(e.X,e.Y));
                canEndCaptureRaised = true;

                tempStartPtX = e.X;
                tempStartPtY = e.Y;
                
                isStartClick = true;
                isEndclick = false;
                rectangleDrawn = false;

                DrawEraseRectangle(new Rectangle(e.X, e.Y, 1, 1), true);
            }
            else
            {
                if ((isStartClick == true) && (isEndclick == false))
                {
                    startPoint.X = tempStartPtX;
                    startPoint.Y = tempStartPtY;

                    endPoint.X = e.X;
                    endPoint.Y = e.Y;

                    if (IsValidRectangle())
                    {
                        CalculateRectangleStartEndPoints(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);

                        rectangleWidth = endPoint.X - startPoint.X;
                        rectangleHeight = endPoint.Y - startPoint.Y;

                        selRectangle = new Rectangle(startPoint.X, startPoint.Y, rectangleWidth, rectangleHeight);
                        //g.DrawRectangle(MyPen, selRectangle);
                        //selRectangle.X = startPoint.X;
                        //selRectangle.Y = startPoint.Y;
                        //selRectangle.Height = rectangleHeight;
                        //selRectangle.Width = rectangleWidth;

                        //call Draw Rectangle
                        DrawEraseRectangle(selRectangle, true);


                        isStartClick = false;
                        isEndclick = true;
                        rectangleDrawn = true;

                        Cursor.Current = Cursors.Hand;
                        //dummy for now
                        //imageName = i.ToString();
                        //i++;

                        TakeCustomSnapShot(startPoint.X, startPoint.Y, rectangleHeight, rectangleWidth, imageName, capturedImagePath, true);

                        ScreenEventArgs eArg = new ScreenEventArgs(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y);
                        //raise event to indicate that capture module has finished capture
                        EndCapture(this, eArg);

                    }
                    else
                    {
                        isStartClick = false;
                        isEndclick = false;
                        rectangleDrawn = false;
                    }

                }
            }

        }


        /// <summary>
        /// To draw rectangle on screen to show boundry
        /// </summary>
        /// <param name="sR"></param>
        /// <param name="draw"></param>
        private void DrawEraseRectangle(Rectangle sR, bool draw)
        {
            IntPtr desktopDC = GetDC(IntPtr.Zero);
            Graphics g = Graphics.FromHdc(desktopDC);
            try
            {
                if (draw)
                {
                    g.DrawRectangle(MyPen, sR);
                }
                else
                {
                    //g.DrawRectangle(EraserPen, sR);
                    //g.Flush();
                    //g.Restore(prevState);
                    // Redraw the desktop and its children
                    //RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
                    InvalidateRect(IntPtr.Zero, ref sR, true);
                }

            }
            finally
            {
                g.Dispose();
                ReleaseDC(GetDesktopWindow(),desktopDC);
            }

        }

        /// <summary>
        /// check if selected points draw a single line and not a rectangle
        /// </summary>
        /// <returns></returns>
        private  bool IsValidRectangle()
        {
            if ((startPoint.X == endPoint.X) || (startPoint.Y == endPoint.Y))
                return false;
            else
                return true;
        }

        /// <summary>
        /// depending upon rectangle start and end point, co-ordinate need to switch to calculate correct height and width
        /// </summary>
        private void CalculateRectangleStartEndPoints(int x1, int y1, int x2, int y2)
        {

            if (x1 < x2)
            {
                startPoint.X = x1;
                endPoint.X = x2;
            }
            else
            {
                startPoint.X = x2;
                endPoint.X = x1;
            }

            if (y1 < y2)
            {
                startPoint.Y = y1;
                endPoint.Y = y2;

            }
            else
            {
                startPoint.Y = y2;
                endPoint.Y = y1;
            }

        }
     

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Start Point X</param>
        /// <param name="y">Start Point Y</param>
        /// <param name="h"> height </param>
        /// <param name="w">Width </param>
        /// <param name="fileName"> Name of the file </param>
        /// <param name="Location"> target folder or location</param>
        /// <param name="capture"> true for capture, false for no Capture</param>
        public void TakeCustomSnapShot(int x, int y, int h, int w, string fileName, string location, bool capture)
        {
            //string location = "D:\\IMS\\TFS";
            //string location  = capturedImagePath;
            //string location = ConfigurationManager.AppSettings["TaskImageLocation"];

            if (!string.IsNullOrEmpty(location) && h > 0 && w > 0)
            {
                //check if the location exists, if not try creating
                if (!System.IO.Directory.Exists(location))
                    System.IO.Directory.CreateDirectory(location);
                if (capture)
                {
                    Bitmap snap = new Bitmap(w, h);
                    using (var g = Graphics.FromImage(snap))
                    {
                        g.CopyFromScreen(new Point(x, y), Point.Empty, new Size(w, h));
                    }
                    //i++;
                    snap.Save(location + @"\" + fileName + ".jpg", ImageFormat.Jpeg);
                }
            }
        }
    }

    public class ScreenEventArgs : EventArgs
    {
        public ScreenEventArgs()
        { 
        }

        public ScreenEventArgs(int sX, int sY)
        {
            StartPointX = sX;
            StartPointY = sY;
        }

        public ScreenEventArgs(int sX, int sY, int eX, int eY)
        {
            StartPointX = sX;
            StartPointY = sY;
            EndPointX = eX;
            EndPointY = eY;
        }


        public int StartPointX { get; internal set; }
        public int StartPointY { get; internal set; }
        public int EndPointX { get; internal set; }
        public int EndPointY { get; internal set; } 

        public string FileName {get;set;}

    }

}
