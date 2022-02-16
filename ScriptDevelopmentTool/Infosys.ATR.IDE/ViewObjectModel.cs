using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infosys.ATR.DevelopmentStudio
{
    public partial class ViewObjectModel : UserControl
    {
        public string ElementDetails
        {
            set
            {
                txtElement.Text = value;
            }
            get
            {
                return txtElement.Text;
            }
        }
        public Image ElementImage
        {
            set
            {
                pictureBox1.Image = value;
                if (value.Height > pictureBox1.Height || value.Width > pictureBox1.Width)
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                else
                    pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            }
        }

        public ViewObjectModel()
        {
            InitializeComponent();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //show the clicked image in a bigger window, this is needed when the captured image is 
            //too big to be visible in the available space in the main display window
            Form imageViewer = new Form() { Text = "Image Viewer in right proportion" };
            imageViewer.MaximizeBox = imageViewer.MinimizeBox = false;
            imageViewer.StartPosition = FormStartPosition.CenterScreen;
            imageViewer.Size = new System.Drawing.Size(700, 500);
            imageViewer.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            PictureBox imageBox = new PictureBox() { Image = pictureBox1.Image };
            if (pictureBox1.Image.Height > 400 || pictureBox1.Image.Width > 600)
                imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            else
                imageBox.SizeMode = PictureBoxSizeMode.Normal;
            imageBox.Dock = DockStyle.Fill;
            imageViewer.Controls.Add(imageBox);
            imageViewer.ShowDialog();
        }

        private void ViewObjectModel_Load(object sender, EventArgs e)
        {
            txtElement.ResetText();
            toolTip1.SetToolTip(pictureBox1, "Double Click to view in right proportion");
        }
    }
}
