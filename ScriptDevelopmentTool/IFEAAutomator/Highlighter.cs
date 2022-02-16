using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageBasedAutomator
{
    public partial class Highlighter : Form
    {
        List<Rectangle> _rectTobeDrawn;
        public Highlighter(List<Rectangle> rectTobeDrawn)
        {
            InitializeComponent();
            _rectTobeDrawn = rectTobeDrawn;

            this.BackColor = Color.Lime;
            // Make the background color of form display transparently. 
            this.TransparencyKey = BackColor;
        }

        private void Highlighter_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            this.Location = new Point(0, 0);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = Screen.PrimaryScreen.Bounds.Width;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
        }

        private void Highlighter_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void Highlighter_Paint(object sender, PaintEventArgs e)
        {
            //draw the rectangle
            if (_rectTobeDrawn != null && _rectTobeDrawn.Count > 0)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    Pen pen = new Pen(Color.Red, 3);
                    g.DrawRectangles(pen, _rectTobeDrawn.ToArray());
                }

                System.Threading.Thread.Sleep(2000);
                this.Close();
            }
        }
    }
}
