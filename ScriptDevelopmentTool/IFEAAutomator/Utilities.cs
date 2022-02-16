/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace ImageBasedAutomator
{
    public static class Utilities
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        private const int KEYEVENTF_KEYUP = 0x0002;
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        //private const int KEYEVENT_SHIFT = 0x10;
        //private const int KEYEVENT_CONTROL = 0x11;
        //private const int KEYEVENT_META = 0x9d;
        //private const int KEYEVENT_ALT = 0x12;
        //private const int KEYEVENT_CAPITAL = 0x14;
        private const Keys KEYEVENT_SHIFT = Keys.LShiftKey;
        private const Keys KEYEVENT_CONTROL = Keys.LControlKey;
        private const Keys KEYEVENT_WINDOWS = Keys.LWin;
        private const Keys KEYEVENT_ALT = Keys.LMenu;
        private const Keys KEYEVENT_CAPITAL = Keys.Capital;
        private const Keys KEYEVENT_ENTER = Keys.Enter;
        private const Keys KEYEVENT_BACKSPACE = Keys.Back;
        private const Keys KEYEVENT_TAB = Keys.Tab;
        private const Keys KEYEVENT_DEL = Keys.Delete;
        private const Keys KEYEVENT_SPACE = Keys.Space;

        private const int KEYPRESS_DELAY = 100; //in ms

        private static System.Collections.Hashtable KeyModifiers;// = new System.Collections.Hashtable();  
        static Utilities()
        {
            KeyModifiers = new System.Collections.Hashtable();
            KeyModifiers.Add(0, KEYEVENT_SHIFT);
            KeyModifiers.Add(1, KEYEVENT_CONTROL);
            KeyModifiers.Add(2, KEYEVENT_WINDOWS);
            KeyModifiers.Add(3, KEYEVENT_ALT);
            KeyModifiers.Add(4, KEYEVENT_CAPITAL);
            KeyModifiers.Add(5, KEYEVENT_ENTER);
            KeyModifiers.Add(6, KEYEVENT_TAB);
            KeyModifiers.Add(7, KEYEVENT_BACKSPACE);
            KeyModifiers.Add(8, KEYEVENT_DEL);
            KeyModifiers.Add(9, KEYEVENT_SPACE);
        }

        public static Image<Bgr, byte> GetScreenShot()
        {
            Image<Bgr, byte> screen = null;
            //get the screen height and width
            int height = SystemInformation.PrimaryMonitorSize.Height;
            int width = SystemInformation.PrimaryMonitorSize.Width;
            Bitmap snap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(snap))
                g.CopyFromScreen(new Point(0, 0), Point.Empty, SystemInformation.PrimaryMonitorSize);
            screen = new Image<Bgr, byte>(snap);
            return screen;
        }

        public static Image<Gray, byte> GetGrayScreenShot()
        {
            Image<Gray, byte> screen = null;
            //get the screen height and width
            int height = SystemInformation.PrimaryMonitorSize.Height;
            int width = SystemInformation.PrimaryMonitorSize.Width;
            Bitmap snap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(snap))
                g.CopyFromScreen(new Point(0, 0), Point.Empty, SystemInformation.PrimaryMonitorSize);
            screen = new Image<Gray, byte>(snap);
            return screen;
        }

        public static void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        public static void DoMouseDown()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
        }

        public static void DoMouseUp()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        public static void DoMouseRightClick()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }

        public static void PlaceMouseCursor(double x, double y)
        {
            System.Windows.Point clickablePoint = new System.Windows.Point();
            clickablePoint.X = x;
            clickablePoint.Y = y;

            System.Windows.Forms.Cursor.Position =
             new System.Drawing.Point((int)clickablePoint.X, (int)clickablePoint.Y);
        }

        public static void KeyPress(string text, params int[] modifiers)
        {
            Keys key;
            foreach (int modifier in modifiers)
            {
                if (KeyModifiers.ContainsKey(modifier))
                {
                    //if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
                    //    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
                    //passing 0 instead of KEYEVENTF_EXTENDEDKEY as KEYEVENTF_EXTENDEDKEY was having issue with CTRL
                    //issue was- ctrl was getting pressed but not released
                    if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
                        keybd_event((byte)key, 0, 0, 0);
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                foreach (char c in text.ToUpper())
                {
                    //special char is kep differently then alpha-nuemeric so that modifiers will affect the alpha-nuemeric key press
                    if (char.IsLetter(c))
                    {
                        if (Enum.TryParse(c.ToString(), out key))
                        {
                            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
                            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
                            System.Threading.Thread.Sleep(KEYPRESS_DELAY);
                        }
                    }
                    else if(char.IsDigit(c))
                    {
                        if (Enum.TryParse("D"+c.ToString(), out key))
                        {
                            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
                            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
                            System.Threading.Thread.Sleep(KEYPRESS_DELAY);
                        }
                    }
                    else
                    {
                        SendKeys.SendWait(c.ToString()); 
                    }
                }
            }
            for (int i = modifiers.Length - 1; i >= 0; i--)
            {
                if (KeyModifiers.ContainsKey(modifiers[i]))
                {
                    if (Enum.TryParse(KeyModifiers[modifiers[i]].ToString(), out key))
                        keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
                }
            } 
            //foreach (int modifier in modifiers)
            //{
            //    if (KeyModifiers.ContainsKey(modifier))
            //    {
            //        if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
            //            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
            //    }
            //}
        }
    }

    public class KeyModifier
    {
        public const int SHIFT = 0;
        public const int CTRL = 1;
        public const int WINDOWS = 2;
        public const int ALT = 3;
        public const int CAPITAL = 4;
        public const int ENTER = 5;
        public const int TAB = 6;
        public const int BACKSPACE = 7;
        public const int DEL = 8;
        public const int SPACE = 9;
    }
}
