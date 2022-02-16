using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Infosys.ATR.UIAutomation.Recorder.ScreenCapture;
using System.Runtime.InteropServices;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.DevelopmentStudio
{
    public delegate void CaptureImage(Image image, string boundry);
    public delegate void AddNode(string id,string name,string type,string ctrlPath);

    public partial class EditObjectModel : UserControl
    {
        public event CaptureImage OnCapture;
        public event AddNode OnAdd;
        string boundary;

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        public string ElementDetails
        {
            set
            {
                txtElement.Text = value;
                HighLightXMLTag(value);
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
                //release the image so that it can be edited
                pictureBox1.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                pictureBox1.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
            }
            get
            {
                return pictureBox1.Image;
            }
        }

        public string Id
        {
            get
            {
                return txtID.Text.Trim();
            }
        }
        public string ElementName
        {
            get
            {
                return txtName.Text.Trim();
            }
        }
        public string Type
        {
            get
            {
                return txtType.Text.Trim();
            }
        }
        public string AppTreePath
        {
            get
            {
                return txtAppTreePath.Text.Trim();
            }
        }

        public void AssignIdentifiers(string id, string name, string ctlType, string appTreePath)
        {
            txtID.Text = id;
            txtName.Text = name;
            txtType.Text = ctlType;
            txtAppTreePath.Text = appTreePath;
        }

        public void Set(string boundry, string path)
        {
            switch (boundry)
            {
                case "Center":
                    cmbCtr.Items.Add(path);
                    break;
                case "Right":
                    cmbRight.Items.Add(path);
                    break;
                case "Left":
                    cmbLeft.Items.Add(path);
                    break;
                case "Up":
                    cmbUp.Items.Add(path);
                    break;
                case "Down":
                    cmbDown.Items.Add(path);
                    break;
                case "Validate":                
                    cmbValidate.Items.Add(path);
                    break;

            }


        }

        //public string ElementImagePath
        //{
        //    set
        //    {

        //    }
        //}

        public EditObjectModel()
        {
            InitializeComponent();
        }

        public EditObjectModel(string id, string name, string ctlType, string appTreePath)
        {
            InitializeComponent();
            txtID.Text = id;
            txtName.Text = name;
            txtType.Text = ctlType;
            txtAppTreePath.Text = appTreePath;
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

        private void EditObjectModel_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(pictureBox1, "Double Click to view in right proportion");
        }

        //void txtElement_TextChanged(object sender, EventArgs e)
        //{
        //    //foreach (string line in txtElement.Lines)
        //    //{
        //    //    HighLightXMLTag(line);
        //    //} 
        //    HighLightXMLTag(txtElement.Text);
        //}

        private void HighLightXMLTag(string line)
        {
            MatchCollection matches = Regex.Matches(line, @"\<(.*?)\>");
            foreach (Match match in matches)
            {
                int pos = txtElement.Find(match.Value);
                txtElement.Select(pos, match.Value.Length);
                txtElement.SelectionColor = Color.Red;
                txtElement.SelectionFont = new Font("Microsoft YaHei UI", txtElement.Font.Size, FontStyle.Bold);
            }
        }

        private void cmbCtr_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImage(this.cmbCtr.Text);
        }

        private void ShowImage(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                this.pictureBox1.Visible = true;
                using (System.IO.FileStream bitmapFile =
                     new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                {
                    Image loaded = new Bitmap(bitmapFile);

                    this.pictureBox1.Image = loaded;

                }
            }

        }

        private void cmbRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImage(this.cmbRight.Text);
        }

        private void cmbLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImage(this.cmbLeft.Text);
        }

        private void cmbUp_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImage(this.cmbUp.Text);
        }

        private void cmbDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Check invalide characters
            if (!string.IsNullOrEmpty(this.cmbDown.Text))
            {
                if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(System.IO.Path.GetFileNameWithoutExtension(this.cmbDown.Text)))
                {
                    //throw new Exception("Please provide the name without Special Characters");
                    MessageBox.Show("Please provide the file name without Special Characters", "Invalid Image name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            ShowImage(this.cmbDown.Text);
        }

        private void btnCtr_Click(object sender, EventArgs e)
        {
            boundary = "Center";
            CaptureThis();
        }

        private void CaptureThis()
        {
            this.ParentForm.WindowState = FormWindowState.Minimized;
            Selector captureArea = new Selector();
            captureArea.ImageCaptured += new Selector.ImageCapturedEventHandler(captureArea_ImageCaptured);
            captureArea.Show();
            SetForegroundWindow(captureArea.Handle);
            SetFocus(captureArea.Handle);
        }

        void captureArea_ImageCaptured(Selector.ImageCapturedArguements e)
        {
            if (e.Image != null)
            {
                if (OnCapture != null)
                {
                    OnCapture(e.Image, boundary);
                }
            }
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            boundary = "Right";
            CaptureThis();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            boundary = "Left";
            CaptureThis();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            boundary = "Up";
            CaptureThis();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            boundary = "Down";
            CaptureThis();
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedTab.Text == "Images")
            {
                cmbCtr.SelectedIndex = 1;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
        }

        private void cmbValidate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImage(this.cmbValidate.Text);
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            boundary = "Validate";
            CaptureThis();
        }

        public void Add()
        {
            this.txtAppTreePath.Text = "";
            this.txtElement.Text = "";
            this.txtID.Text = "";
            this.txtName.Text = "";
            this.txtType.Text = "";
            this.btnAdd.Visible = true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (OnAdd != null)
                OnAdd(txtID.Text, txtName.Text, txtType.Text, txtAppTreePath.Text);
        }
    }
}
