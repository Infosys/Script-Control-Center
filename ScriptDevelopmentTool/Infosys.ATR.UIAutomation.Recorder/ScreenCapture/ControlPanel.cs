using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Infosys.IAP.CommonClientLibrary;


namespace Infosys.ATR.UIAutomation.Recorder.ScreenCapture
{

    public partial class ControlPanel : Form
    {

        string ScreenPath;
        private static bool TipsShowing;

        private Form m_InstanceRef = null;
        public Form InstanceRef
        {

            get
            {

                return m_InstanceRef;

            }
            set
            {

                m_InstanceRef = value;

            }

        }

        public ControlPanel()
        {

            InitializeComponent();

        }

        public ControlPanel(bool Save)
        {

            InitializeComponent();
            this.FormClosed += new FormClosedEventHandler(Form_Close);

        }

        public void key_press(object sender, KeyEventArgs e)
        {

            keyTest(e);

        }


        private void keyTest(KeyEventArgs e)
        {

            if (e.KeyCode.ToString() == "S")
            {

                screenCapture(true);

            }

        }


        private void Form_Close(object sender, FormClosedEventArgs e)
        {

            Application.Exit();

        }

        public void bttCaptureArea_Click(object sender, EventArgs e)
        {

            this.Hide();
            Selector form1 = new Selector();
            form1.InstanceRef = this;
            form1.Show();

        }

        public void screenCapture(bool showCursor)
        {

            Point curPos = new Point(Cursor.Position.X, Cursor.Position.Y);
            Size curSize = new Size();
            curSize.Height = Cursor.Current.Size.Height;
            curSize.Width = Cursor.Current.Size.Width;

            ScreenPath = "";

            if (!ScreenShot.saveToClipboard)
            {

                saveFileDialog1.DefaultExt = "bmp";
                saveFileDialog1.Filter = "bmp files (*.bmp)|*.bmp|jpg files (*.jpg)|*.jpg|gif files (*.gif)|*.gif|tiff files (*.tiff)|*.tiff|png files (*.png)|*.png";
                saveFileDialog1.Title = "Save screenshot to...";
                saveFileDialog1.ShowDialog();
                ScreenPath = saveFileDialog1.FileName;

            }

            

            if (ScreenPath!=""||ScreenShot.saveToClipboard)
            {

                //Conceal this form while the screen capture takes place
                this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
                this.TopMost = false;

                //Allow 250 milliseconds for the screen to repaint itself (we don't want to include this form in the capture)
                System.Threading.Thread.Sleep(250);

                Rectangle bounds = Screen.GetBounds(Screen.GetBounds(Point.Empty));
                string fi = "";

                if (ScreenPath != "")
                {

                    fi = new FileInfo(ScreenPath).Extension;

                }

                ScreenShot.CaptureImage(showCursor, curSize, curPos, Point.Empty, Point.Empty, bounds, ScreenPath, fi);
                          
                //The screen has been captured and saved to a file so bring this form back into the foreground
                this.WindowState = System.Windows.Forms.FormWindowState.Normal;
                this.TopMost = true;

                if (ScreenShot.saveToClipboard)
                {

                    MessageBox.Show("Screen saved to clipboard", "Screen Capture", MessageBoxButtons.OK);

                }
                else
                {

                    MessageBox.Show("Screen saved to file", "Screen Capture", MessageBoxButtons.OK);

                }


            }


        }

        private void bttCaptureScreen_Click(object sender, EventArgs e)
        {

            screenCapture(false);

        }

        private void bttTips_Click(object sender, EventArgs e)
        {
            if (TipsShowing)
            {

                TipsShowing = false;
                txtTips.Visible = false;
                ControlPanel.ActiveForm.Width = 140;

            }
            else
            {

                TipsShowing = true;
                txtTips.Visible = true;
                ControlPanel.ActiveForm.Width = 385;

            }

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ControlPanel_Load(object sender, EventArgs e)
        {

            this.KeyUp += new KeyEventHandler(key_press);

            System.Text.Encoding Encoder = System.Text.ASCIIEncoding.Default;
            Byte[] buffer = new byte[] { (byte)149 };
            string bullet = System.Text.Encoding.GetEncoding(1252).GetString(buffer);

            TipsShowing = false;
            txtTips.Visible = false;

            txtTips.Text =
            bullet + "To capture an area:" + Environment.NewLine + 
            "  Click and hold down the left mouse button." + Environment.NewLine +
            "  Draw the selection area required." + Environment.NewLine + 
            "  Once the selection area is drawn you can" + Environment.NewLine +
            "  drag it to move it." + Environment.NewLine + Environment.NewLine +
            "  The selection area can also be resized at its" + Environment.NewLine +
            "  four sides and four corners." + Environment.NewLine + Environment.NewLine +
            "  Once you are satisfied with the selection area" + Environment.NewLine +
            "  simply double click anywhere within or outside" + Environment.NewLine +
            "  the selection area to save the selection to an" + Environment.NewLine +
            "  image file." + Environment.NewLine + Environment.NewLine +
            bullet + " With this form active press the 'S' key to" + Environment.NewLine +
            "  capture the screen with the cursor included." + Environment.NewLine +
            "  Alternatively press the 'S' key when you" + Environment.NewLine +
            "  have selected an area in order to include the" + Environment.NewLine +
            "  cursor in the area captured.";

            this.Width = 140;

        }

        private void saveToClipboard_CheckedChanged(object sender, EventArgs e)
        {

            ScreenShot.saveToClipboard = saveToClipboard.Checked;
            
        }

        private void saveToClipboard_KeyUp(object sender, KeyEventArgs e)
        {

            keyTest(e);

        }

        private void bttCaptureArea_KeyUp(object sender, KeyEventArgs e)
        {

            keyTest(e);

        }

        private void bttTips_KeyUp(object sender, KeyEventArgs e)
        {

            keyTest(e);

        }

        private void bttCaptureScreen_KeyUp(object sender, KeyEventArgs e)
        {

            keyTest(e);

        }

        private void txtTips_KeyUp(object sender, KeyEventArgs e)
        {

            keyTest(e);

        }

    }
}