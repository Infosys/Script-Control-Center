using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Infosys.IAP.CommonClientLibrary
{
    public partial class uclProgressStatus : UserControl
    {
        public uclProgressStatus()
        {
            InitializeComponent();
            label1.ForeColor = Color.Black;
            this.ForeColor = SystemColors.Highlight;
        }

        protected float percent = 0.0f; 

        public float Value
        {
            get { return percent; }
            set 
            {
                if (value < 0) value = 0;
                else if (value > 100) value = 100;
                percent = value;
                label1.Text = value.ToString();

                this.Invalidate();
            }        
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Brush b = new SolidBrush(this.ForeColor);
            LinearGradientBrush lb = new LinearGradientBrush(new Rectangle(0, 0, this.Width, this.Height), Color.FromArgb(255, Color.White), Color.FromArgb(50, Color.White), LinearGradientMode.ForwardDiagonal);
            int width = (int)((percent / 100) * this.Width);
            e.Graphics.FillRectangle(b, 0, 0, width, this.Height);
            e.Graphics.FillRectangle(lb, 0, 0, width, this.Height);
            b.Dispose(); lb.Dispose();

        }

        private void uclProgressStatus_SizeChanged(object sender, EventArgs e)
        {
            label1.Location = new Point(this.Width / 2 - 21 / 2 - 4, this.Height / 2 - 15 / 2);
        }
    }
}
