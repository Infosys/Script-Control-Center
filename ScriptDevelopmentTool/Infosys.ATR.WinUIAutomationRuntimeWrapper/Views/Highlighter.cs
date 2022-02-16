using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Views
{
    public partial class Highlighter : Form
    {
        Rectangle _rectTobeDrawn;
        public Highlighter(Rect rectTobeDrawn)
        {
            InitializeComponent();
            _rectTobeDrawn = TranslateRectToRectangle(rectTobeDrawn);

            this.BackColor = Color.Lime;
            // Make the background color of form display transparently. 
            this.TransparencyKey = BackColor;
        }

        private Rectangle TranslateRectToRectangle(Rect rect)
        {
            Rectangle rectangle = Rectangle.Empty;
            if (rect != Rect.Empty)
            {
                rectangle.X = (int)rect.X;
                rectangle.Y = (int)rect.Y;
                rectangle.Width = (int)rect.Width;
                rectangle.Height = (int)rect.Height;
            }
            return rectangle;
        }

        private void Highlighter_Load_1(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.Location = new System.Drawing.Point(0, 0);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        }

        private void Highlighter_Paint_1(object sender, PaintEventArgs e)
        {
            //draw the rectangle
            if (_rectTobeDrawn != null)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    Pen pen = new Pen(Color.Red, 3);
                    g.DrawRectangle(pen, _rectTobeDrawn);
                }

                System.Threading.Thread.Sleep(2000);
                this.Close();
            }
        }

        private void Highlighter_MouseClick_1(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
