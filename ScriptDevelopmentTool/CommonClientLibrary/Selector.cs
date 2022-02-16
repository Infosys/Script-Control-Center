using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infosys.IAP.CommonClientLibrary
{
    public partial class Selector : Form
    {
        Rectangle roi; //depicts the region of interest
        bool _saveToClipboard;
        bool _enableSave; //to enable the save capability form inside this view
        bool _autosave;
        string _capturedImagePath;
        Form _instanceRef = null;
        public Form InstanceRef
        {
            get { return _instanceRef; }
            set { _instanceRef = value; }
        }

        public string CapturedImagePath
        {
            get { return _capturedImagePath; }
            set { _capturedImagePath = value; }
        }

        //to raise event depicting the image is captured
        public class ImageCapturedArguements : EventArgs
        {
            public Image Image { get; set; }
            public string Area { get; set; }
            public string State { get; set; }
        }
        public delegate void ImageCapturedEventHandler(ImageCapturedArguements e);
        public event ImageCapturedEventHandler ImageCaptured;

        public Selector(bool saveToClipBoard = true, bool enableSave = false,bool autosave = false)
        {
            _saveToClipboard = saveToClipBoard;
            _enableSave = enableSave;
            _autosave = autosave;
            InitializeComponent();
        }

        private void Selector_MouseDown(object sender, MouseEventArgs e)
        {
            //set the start point of roi
            roi = new Rectangle(e.X, e.Y, 0, 0);
            this.Invalidate();
        }

        private void Selector_MouseMove(object sender, MouseEventArgs e)
        {
            //identify the roi as mouse moves
            if (e.Button == MouseButtons.Left)
            {
                roi = new Rectangle(roi.Left, roi.Top, e.X - roi.Left, e.Y - roi.Top);
                this.Invalidate();
            }
        }

        private void Selector_Paint(object sender, PaintEventArgs e)
        {
            //draw the roi
            using (Pen pen = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawRectangle(pen, roi);
            }
        }

        private void Selector_MouseUp(object sender, MouseEventArgs e)
        {
            if (roi.Width > 0 && roi.Height > 0)
            {
                this.Opacity = 0; //so that no shade gets added in the snapshot taken

                //take snapshot of the roi and close the form
                Bitmap snapshot = new Bitmap(roi.Width, roi.Height);
                Graphics graphics = Graphics.FromImage(snapshot);
                graphics.CopyFromScreen(roi.Left, roi.Top, 0, 0, snapshot.Size);

                if (_autosave)
                {
                    if (!string.IsNullOrEmpty(_capturedImagePath))
                        snapshot.Save(_capturedImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                //save the snapshot taken as jpeg file
                if (_enableSave)
                {
                    saveFileDialog1.Filter = "jpg files (*.jpg)|*.jpg";
                    if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        snapshot.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        _capturedImagePath = saveFileDialog1.FileName;
                    }
                }
                if (_saveToClipboard)
                {
                    Clipboard.SetImage(snapshot);
                }

                if (ImageCaptured != null)
                {
                    ImageCaptured(new ImageCapturedArguements() { Image = snapshot });
                }
                this.Close();
            }
        }

        private void Selector_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
