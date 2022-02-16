using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.UIAutomation.Entities;
using System.Drawing.Imaging;

namespace EditUseCase
{
    public partial class ActivityView : UserControl
    {
        //public variable
        public class ActivityChangedArgs : EventArgs
        {
            public Activity ChangedActivity { get; set; }
        }
        public delegate void ActivityChangedEventHandler(ActivityChangedArgs e);
        public event ActivityChangedEventHandler ActivityChanged;

        //private variables
        Activity _activity = new Activity();
        class SubTask
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Control { get; set; }
        }
        int taskIndex = -1;
        bool crop = false;
        int clickCounter;
        int leftx, lefty, width, height;
        bool imageChanged = false;
        string imagePath;
        Bitmap bmp;

        public ActivityView()
        {
            InitializeComponent();
        }

        public ActivityView(Activity activity)
        {
            InitializeComponent();
            _activity = activity;
            txtActId.Text = activity.Id;
            txtAppExe.Text = activity.TargetApplication.ApplicationExe;
            txtAppName.Text = activity.TargetApplication.TargetApplicationAttributes[0].Value;
            txtDesc.Text = activity.Description ?? "";
            txtParentId.Text = activity.ParentId ?? "";
        }

        private void ActivityView_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(ActivityView_Paint);
            //populate tasks
            dgTasks.DataSource = GetSubTasks();
            if (dgTasks.RowCount > 0)
            {
                dgTasks.Rows[0].Selected = true;
                dgTasks_CellMouseClick(new object(), null);
            }
        }

        void ActivityView_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new System.Drawing.Pen(Color.Gray);
            //for activity
            //vertical line
            int x = txtActId.Location.X + txtActId.Width + 25;
            int y = txtActId.Location.Y;
            e.Graphics.DrawLine(pen, x, y, x, y + 110);
            //horizontal line
            x = lblTasks.Location.X;
            y = lblTasks.Location.Y - 10;
            e.Graphics.DrawLine(pen, x, y, x + 640, y);

            //for task
            //vertical line
            x = txtControlId.Location.X + txtActId.Width + 25;
            y = txtControlId.Location.Y;
            e.Graphics.DrawLine(pen, x, y, x, y + 238);
            //horizontal line
            x = lblSelectedTask.Location.X;
            y = lblSelectedTask.Location.Y - 10;
            e.Graphics.DrawLine(pen, x, y, x + 640, y);

            //last horizontal line
            x = lblEnd.Location.X;
            y = lblEnd.Location.Y - 10;
            e.Graphics.DrawLine(pen, x, y, x + 640, y);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ActivityChanged != null)
                ActivityChanged(new ActivityChangedArgs() { ChangedActivity = _activity });
            if (imageChanged)
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    if (!System.IO.Directory.Exists(imagePath))
                        System.IO.Directory.CreateDirectory(imagePath);
                    string imageFile = imagePath + @"\" + _activity.Tasks[taskIndex].Id + ".jpg";
                    if (System.IO.File.Exists(imageFile))
                        System.IO.File.Delete(imageFile);
                    pic.Image.Save(imagePath + @"\" + _activity.Tasks[taskIndex].Id + ".jpg", ImageFormat.Jpeg);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            StringBuilder sbSteps = new StringBuilder();
            sbSteps.Append("Steps to crop image:" + Environment.NewLine);
            sbSteps.Append("1. Click on the 'Crop' button, the button text will change to 'Stop'" + Environment.NewLine);
            sbSteps.Append("2. In the Picture boc, click at the TOP-LEFT corner of the intended image" + Environment.NewLine);
            sbSteps.Append("3. Then click at the BOTTOM-RIGHT corner of the intended image" + Environment.NewLine);
            sbSteps.Append("4. Once the boundary is specified by the above steps 2 and 3, the final image will be shown" + Environment.NewLine);
            MessageBox.Show(sbSteps.ToString(), "Steps to Crop Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog browseImage = new OpenFileDialog();
            browseImage.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (browseImage.ShowDialog() == DialogResult.OK)
            {
                pic.Image = new Bitmap(browseImage.FileName);
            }
        }

        private void dgTasks_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            taskIndex = dgTasks.SelectedRows[0].Index;
            txtControlId.Text = _activity.Tasks[taskIndex].ControlId;
            txtTaskName.Text = _activity.Tasks[taskIndex].Name;
            txtTaskDesc.Text = _activity.Tasks[taskIndex].Description;
            txtWinTitle.Text = _activity.Tasks[taskIndex].WindowTitle;
            txtTaskGroupId.Text = _activity.Tasks[taskIndex].GroupScriptId;
            txtXPath.Text = _activity.Tasks[taskIndex].XPath ?? "";
            if (_activity.Tasks[taskIndex].Name == TaskNames.GroupedKeys.ToString())
                txtTaskText.Text = _activity.Tasks[taskIndex].TargetControlAttributes[1].Value;
            else
                txtTaskText.Text = "";
            if (imagePath == null)
                imagePath = System.Configuration.ConfigurationManager.AppSettings["TaskImageLocation"];
            if (!string.IsNullOrEmpty(imagePath))
            {
                string imagefile = imagePath + @"\" + _activity.Tasks[taskIndex].Id + ".jpg";
                if (System.IO.File.Exists(imagefile))
                {
                    bmp = new Bitmap(imagefile);
                    pic.Image = bmp;
                    //using (Bitmap bmp = new Bitmap(imagefile))
                    //    pic.Image = bmp; 
                }
                else
                {
                    pic.Image = pic.InitialImage;
                }
            }
            else
            {
                pic.Image = pic.InitialImage;
            }
        }

        private List<SubTask> GetSubTasks()
        {
            List<SubTask> subTasks = new List<SubTask>();
            _activity.Tasks.ForEach(t =>
            {
                subTasks.Add(new SubTask() { Description = t.Description, Id = t.Id, Name = t.Name, Control = t.ControlId });
            });
            return subTasks;
        }

        private void txtParentId_TextChanged(object sender, EventArgs e)
        {
            _activity.ParentId = txtParentId.Text;
        }

        private void txtDesc_TextChanged(object sender, EventArgs e)
        {
            _activity.Description = txtDesc.Text;
        }

        private void txtAppExe_TextChanged(object sender, EventArgs e)
        {
            _activity.TargetApplication.ApplicationExe = txtAppExe.Text;
        }

        private void txtTaskDesc_TextChanged(object sender, EventArgs e)
        {
            _activity.Tasks[taskIndex].Description = txtTaskDesc.Text;
        }

        private void txtWinTitle_TextChanged(object sender, EventArgs e)
        {
            _activity.Tasks[taskIndex].WindowTitle = txtWinTitle.Text;
        }

        private void txtTaskGroupId_TextChanged(object sender, EventArgs e)
        {
            _activity.Tasks[taskIndex].GroupScriptId = txtTaskGroupId.Text;
        }

        private void txtXPath_TextChanged(object sender, EventArgs e)
        {
            _activity.Tasks[taskIndex].XPath = txtXPath.Text;
        }

        private void txtTaskText_TextChanged(object sender, EventArgs e)
        {
            if (_activity.Tasks[taskIndex].Name == TaskNames.GroupedKeys.ToString())
            {
                _activity.Tasks[taskIndex].TargetControlAttributes[1].Value = txtTaskText.Text.ToUpper();
            }
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            if (!crop)
            {
                crop = true;
                pic.MouseClick += new MouseEventHandler(pic_MouseClick);
                btnCrop.Text = "Stop";
            }
            else
            {
                crop = false;
                pic.MouseClick -= new MouseEventHandler(pic_MouseClick);
                btnCrop.Text = "Crop";
            }
        }

        void pic_MouseClick(object sender, MouseEventArgs e)
        {
            clickCounter++;
            if (clickCounter == 1)
            {
                leftx = e.X;
                lefty = e.Y;
            }
            if (clickCounter == 2)
            {
                clickCounter = 0;
                crop = false;
                pic.MouseClick -= new MouseEventHandler(pic_MouseClick);
                btnCrop.Text = "Crop";

                //crop the image and put the copy
                width = e.X - leftx;
                height = e.Y - lefty;

                if (width < 0 || height < 0)
                {
                    MessageBox.Show("The mouse clicks are not proper, please try again. Second click should be on the 'right of' and/or 'lower than' the first click.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Rectangle cropImage = new Rectangle(leftx, lefty, width, height);
                    Rectangle cropImageLocation = new Rectangle(leftx + pic.Location.X, lefty + pic.Location.Y, width, height);
                    //check if the rectangle is withing the bounds of the main picture box
                    if (pic.Bounds.Contains(cropImageLocation))
                    {
                        //pic.Image = CropImage(pic.Image, cropImage);
                        Image newImage = CropImage(pic.Image, cropImage);
                        pic.Image = newImage;
                        bmp.Dispose();
                    }
                }
            }
        }

        private Image CropImage(Image img, Rectangle rect)
        {
            imageChanged = true;
            if (rect.X + rect.Width > img.Width)
                rect.Width = img.Width - rect.X;
            if (rect.Y + rect.Height > img.Height)
                rect.Height = img.Height - rect.Y;
            return ((Bitmap)img).Clone(rect, img.PixelFormat);
        }
    }
}
