using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Infosys.ATR.Editor.Entities;

using Infosys.ATR.UIAutomation.Recorder.ScreenCapture;
using IMSWorkBench.Infrastructure.Library.Services;
using IMSWorkBench.Infrastructure.Interface.Services;

namespace Infosys.ATR.Editor.Views.ImageEditor
{
    public partial class ImageEditor : UserControl, IImageEditor, IClose
    {
        Area _area;
        BindingList<String> _states;
        Dictionary<String, Dictionary<Area, String>> _imagestates;
        string _name;
        internal string _ucName;
        string _appName;
        string _appPath;

        public string ucName { get { return _ucName; } set { _ucName = value; } }
        public string AppName { get { return _appName; } set { _appName = value; } }
        public string AppPath { get { return _appPath; } set { _appPath = value; } }

        internal ProjectMode Mode
        {
            set
            {
                if (value == ProjectMode.Win32)
                    pnlAppName.Hide();
            }
        }

        public ImageEditor()
        {
            InitializeComponent();
            _states = new BindingList<String>();
            _imagestates = new Dictionary<string, Dictionary<Area, string>>();
            this.cmbState.DataSource = _states;
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            CaptureThis(Area.Center);
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            CaptureThis(Area.Right);
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            CaptureThis(Area.Left);
        }

        private void btnTop_Click(object sender, EventArgs e)
        {
            CaptureThis(Area.Above);
        }

        private void btnBottom_Click(object sender, EventArgs e)
        {
            CaptureThis(Area.Below);
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            CaptureThis(Area.Validate);
        }

        private void CaptureThis(Area area)
        {
            _area = area;
            this.ParentForm.WindowState = FormWindowState.Minimized;
            this._presenter.CaptureThis(area, cmbState.Text);


        }

        private void txtDefault_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtDefault.Text))
            {
                _area = Area.Center;
                ShowImage(txtDefault.Text);
            }
        }

        private void txtRight_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRight.Text))
            {
                _area = Area.Right;
                ShowImage(txtRight.Text);
            }
        }

        private void txtLeft_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtLeft.Text))
            {
                _area = Area.Left;
                ShowImage(txtLeft.Text);
            }
        }

        private void txtTop_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTop.Text))
            {
                _area = Area.Above;
                ShowImage(txtTop.Text);
            }
        }

        private void txtValidate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtValidate.Text))
            {
                _area = Area.Validate;
                ShowImage(txtValidate.Text);
            }
        }


        private void UpdatePath()
        {
            switch (_area)
            {
                case Area.Center:
                    txtDefault.Text = ImagePath;
                    break;
                case Area.Below:
                    txtBottom.Text = ImagePath;
                    break;
                case Area.Above:
                    txtTop.Text = ImagePath;
                    break;
                case Area.Right:
                    txtRight.Text = ImagePath;
                    break;
                case Area.Left:
                    txtLeft.Text = ImagePath;
                    break;
                case Area.Validate:
                    txtValidate.Text = ImagePath;
                    break;
            }

            if (!_imagestates.ContainsKey(cmbState.Text))
            {
                Dictionary<Area, string> imagearea = new Dictionary<Area, string>();
                imagearea.Add(_area, ImagePath);
                _imagestates.Add(cmbState.Text, imagearea);
            }
            else
            {
                _imagestates[cmbState.Text][_area] = ImagePath;
            }
        }

        void ShowImage(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = Path.Combine(BaseDir, path);
                if (File.Exists(path))
                {
                    using (var t = new Bitmap(path))
                    {
                        pictureBox1.Image = new Bitmap(t);
                    }

                    this.label2.Text = _area.ToString();
                }
                else
                {
                    MessageBox.Show("Image not present in " + path +".Verify whether base directory is configured correctly.","IAP",MessageBoxButtons.OK,MessageBoxIcon.Warning );
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _states.Add(cmbState.Text);
            cmbState.Refresh();
            ClearText();
            cmbState.SelectedItem = _states[_states.Count - 1];
            // txtName.Text = _name;
        }

        private void txtDefault_TextChanged(object sender, EventArgs e)
        {
            ShowImage(txtDefault.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateImage(cmbState.Text);
        }

        public void UpdateImage(string state)
        {
            if (_imagestates.ContainsKey(state))
            {
                txtstateName.Text = _name;
                var imageState = _imagestates[state];
                txtBottom.Text = imageState.ContainsKey(Area.Below) ? imageState[Area.Below] : String.Empty;
                txtTop.Text = imageState.ContainsKey(Area.Above) ? imageState[Area.Above] : String.Empty;
                txtRight.Text = imageState.ContainsKey(Area.Right) ? imageState[Area.Right] : String.Empty;
                txtLeft.Text = imageState.ContainsKey(Area.Left) ? imageState[Area.Left] : String.Empty;
                txtValidate.Text = imageState.ContainsKey(Area.Validate) ? imageState[Area.Validate] : String.Empty;
                txtDefault.Text = imageState.ContainsKey(Area.Center) ? imageState[Area.Center] : String.Empty;
            }
        }

        private void ClearText()
        {
            // txtName.Text = String.Empty;
            txtBottom.Text = String.Empty;
            txtRight.Text = String.Empty;
            txtLeft.Text = String.Empty;
            txtTop.Text = String.Empty;
            txtDefault.Text = String.Empty;
            txtValidate.Text = String.Empty;
            pictureBox1.Image = null;
        }



        private void txtRight_TextChanged(object sender, EventArgs e)
        {
            ShowImage(txtRight.Text);
        }

        private void txtLeft_TextChanged(object sender, EventArgs e)
        {
            ShowImage(txtLeft.Text);
        }

        private void txtTop_TextChanged(object sender, EventArgs e)
        {
            ShowImage(txtTop.Text);
        }

        private void txtBottom_TextChanged(object sender, EventArgs e)
        {
            ShowImage(txtBottom.Text);
        }

        private void txtValidate_TextChanged(object sender, EventArgs e)
        {
            ShowImage(txtValidate.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _name = txtstateName.Text;
            this._presenter.OnUpdateName(_name);
        }


        internal void ShowApplicationPanel(string p)
        {
            if (p.Contains("Application"))
            {
                pnlApplication.Visible = true;
                //groupBox3.Controls.Remove(pnlAppName);  
                pnlAppName.Hide();
                pnlAppName.AutoSize = true;
                pnlState.AutoSize = true;
            }
            else
            {
                pnlApplication.Visible = false;
                pnlAppName.Show();
                pnlAppName.AutoSize = true;
                pnlState.AutoSize = true;
                //if (!groupBox3.Controls.Contains(pnlAppName))
                //    groupBox3.Controls.Add(pnlAppName);
            }
            groupBox3.AutoSize = true;          
        }

        public void Close()
        {
            this._presenter.OnCloseView();
        }

        private void btnAppPathBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog openApp = new OpenFileDialog();
            var result = openApp.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtAppPath.Text = _appPath = openApp.FileName;
            }
        }

        private void txtBottom_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBottom.Text))
            {
                _area = Area.Below;
                ShowImage(txtBottom.Text);
            }
        }

        private void btnAddApp_Click(object sender, EventArgs e)
        {
            if (cmbAppType.SelectedItem == null)
                throw new GenericException("Please select Application Type", true, false);
            if (cmbAppType.SelectedItem == "Web")
            {
                //app is web
                if (cmbBrowser.SelectedItem == null)
                    throw new GenericException("Please select Browser", true, false);

                this._presenter.UpdateAppProperties_Handler(new string[] { txtAppName.Text,cmbAppType.SelectedItem.ToString(),
                txtAppPath.Text, cmbBrowser.SelectedItem.ToString(), txtVersion.Text});
            }
            else if (cmbAppType.SelectedItem == "WinDesktop")
            {
                //App is a windesktop 
                this._presenter.UpdateAppProperties_Handler(new string[] { txtAppName.Text,cmbAppType.SelectedItem.ToString(),
                txtAppPath.Text, "", ""});
            }
        }

        private void cmbAppType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAppType.SelectedItem != "Web")
            {
                txtVersion.Enabled = false;
                cmbBrowser.Enabled = false;
                txtAppPath.Enabled = false;
                txtVersion.Text = "";
                //txtAppPath.Text = "";
                cmbBrowser.SelectedItem = null;
            }
            else
            {
                txtVersion.Enabled = true;
                cmbBrowser.Enabled = true;
                txtAppPath.Enabled = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                frmImage image = new frmImage();
                image.ShowImage(this.pictureBox1.Image);
            }
        }

    }
}
