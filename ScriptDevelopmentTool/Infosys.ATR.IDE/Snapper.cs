using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.UIAutomation.Recorder.ScreenCapture;
using System.Drawing.Imaging;
using System.Configuration;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.DevelopmentStudio
{
    public partial class Snapper : Form
    {
        int height = 0, width = 0;

        public List<string> FilePaths { get; set; }
        public bool SaveAndClose { get; set; }

        public Snapper()
        {
            InitializeComponent();
        }

        private void toolStripNew_Click(object sender, EventArgs e)
        {
            //take snap and increase the height of the tool
            Selector captureArea = new Selector();
            captureArea.ImageCaptured += new Selector.ImageCapturedEventHandler(captureArea_ImageCaptured);
            captureArea.Show();
            //this.BringToFront();
        }

        void captureArea_ImageCaptured(Selector.ImageCapturedArguements e)
        {
            if (e.Image != null)
            {
                pictureBox1.Image = e.Image;
                this.Height = height + pictureBox1.Image.Height;
                this.Width = width > pictureBox1.Image.Width + 20 ? width : pictureBox1.Image.Width + 20;
            }
        }

        private void toolStripbtnSave_Click(object sender, EventArgs e)
        {
            foreach (string filePath in FilePaths)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    pictureBox1.Image.Save(filePath, ImageFormat.Jpeg);
                }
            }
            if (FilePaths.Count > 0 && SaveAndClose)
            {
                this.Close();
                return;
            }

            saveFileDialog1.Filter = "Images|*.jpg;*.bmp;*.png;*.gif;*.tiff";
            //check for default path if configured
            string path = ConfigurationManager.AppSettings["ObjectModelPath"];
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                saveFileDialog1.InitialDirectory = path;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && pictureBox1.Image != null)
            {
                string ext = System.IO.Path.GetExtension(saveFileDialog1.FileName);
                ImageFormat format = ImageFormat.Jpeg;
                switch (ext)
                {
                    case ".png":
                        format = ImageFormat.Png;
                        break;
                    case ".jpg":
                        format = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        format = ImageFormat.Bmp;
                        break;
                    case ".gif":
                        format = ImageFormat.Gif;
                        break;
                    case ".tiff":
                        format = ImageFormat.Tiff;
                        break;
                }
                pictureBox1.Image.Save(saveFileDialog1.FileName, format);
            }
            if (SaveAndClose)
                this.Close();
        }

        private void Snapper_Load(object sender, EventArgs e)
        {
            height = this.Height;
            width = this.Width;
        }
    }
}
