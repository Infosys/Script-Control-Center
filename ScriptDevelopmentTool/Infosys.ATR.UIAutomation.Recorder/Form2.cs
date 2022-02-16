using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.UIAutomation.SEE;
using System.Diagnostics;
using Infosys.ATR.UIAutomation.Recorder.ScreenCapture;
using Infosys.ATR.UIAutomation.ScreenCapture;
using System.IO;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.UIAutomation.Recorder
{
    public partial class Form2 : Form
    {
        System.Windows.Forms.ToolTip ToolTipForCapture = new System.Windows.Forms.ToolTip();
        System.Windows.Forms.ToolTip ToolTipForExpandCollapse = new System.Windows.Forms.ToolTip();
        string path = Directory.GetCurrentDirectory();
        Infosys.ATR.UIAutomation.ScreenCapture.MyScreen screen = new Infosys.ATR.UIAutomation.ScreenCapture.MyScreen();
        //private variables
        string imageLocation = "";
        bool showEventImage = true;
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
        int height, width, formHeight;
        IntPtr current = IntPtr.Zero;
        //public variables
        public string UseCaseId = "";

        public IntPtr Current { get { return current; } }
        /// <summary>
        /// this event will notify the parent form about child form closed
        /// </summary>
        // public event EventHandler formClosed;
        int index = 0;
        TaskViewItem t = null;

        public delegate void TaskViewClosingEventHandler(TaskViewClosingEventArgs e);
        public event TaskViewClosingEventHandler TaskViewClosing;



        /// <summary>
        /// The event to be raise when the intended size of the event image is changed
        /// </summary>
        public event ImageSizeChangedEventHandler ImageSizeChanged;
        public class ImageSizeChangedArgs : EventArgs
        {
            public int Height { get; set; }
            public int Width { get; set; }
        }
        public delegate void ImageSizeChangedEventHandler(ImageSizeChangedArgs e);

        /// <summary>
        /// The event to be raised when any task is asked to be deleted
        /// </summary>
        public event TaskDeletedEventHandler TaskDeleted;
        public class TaskDeletedArgs : EventArgs
        {
            public string TaskId { get; set; }
        }
        public delegate void TaskDeletedEventHandler(TaskDeletedArgs e);

        /// <summary>
        /// The event to be raised when a task is selected
        /// </summary>
        public class TaskSelectedArgs : EventArgs
        {
            public string TaskId { get; set; }
        }
        public delegate void TaskSelectedEventHandler(TaskSelectedArgs e);
        public event TaskSelectedEventHandler TaskSelected;

        public class WaitEventArgs : EventArgs
        {
            public string TaskId { get; set; }
            public string Interval { get; set; }
        }

        public delegate void UpdateWaitEventHandler(WaitEventArgs e);
        public event UpdateWaitEventHandler UpdateEvent;

        public delegate void AddWaitEventHandler(IntPtr handle);
        public event AddWaitEventHandler AddWaitEvent;
        public Form1 parent;
        public BindingList<EventView> events;
        public Form2(Form1 parent)
        {
            InitializeComponent();


            this.parent = parent;
            //register to update event in parent form
            parent.updateDesc += new EventHandler(UpdateData);

            ToolTipForExpandCollapse.SetToolTip(this.pictureBox1, "Collapse/Hide");
            ToolTipForCapture.SetToolTip(this.pictureBox3, "Add Task:Press ALT+LEFT for screen capture");
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(this.pictureBox2, "Add Wait");
            this.events = new BindingList<EventView>();
            if (parent.EventsList != null && parent.EventsList.Count > 0)
            {

                foreach (EventView ev in parent.EventsList)
                {
                    events.Add(ev);
                }


            }
            this.dataGridView1.DataSource = events;
            this.dataGridView1.AllowUserToAddRows = false;
            
        }


        /// <summary>
        /// update text in text box when even t is raised in parent form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateData(object sender, EventArgs e)
        {
            if (dataGridView1.IsHandleCreated)
            {
                if (dataGridView1.InvokeRequired)
                {
                    dataGridView1.BeginInvoke(new Action<RecordActions.AnyUserEventArgs>(UpdateData1), new object[] { e });
                    return;
                }
                else
                {
                    UpdateData1(e as RecordActions.AnyUserEventArgs);
                }
            }
            else
            {
                throw new Exception("Datagridview handle not created");
            }
            
        }

        public void UpdateGrid(RecordActions.AnyUserEventArgs e)
        {
            var temp = e;
            if (dataGridView1.Rows.Count != 0)
                dataGridView1.NotifyCurrentCellDirty(true);
            events.Add(new EventView
            {
                Delete = new Bitmap(@"Images\del.jpg"),
                Description = e.EventDesc,
              Snip = new Bitmap(@"Images\camera.jpg"),
            
                Id = e.EventId
            });
            dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
            var eventView = dataGridView1.Rows[dataGridView1.Rows.Count - 1].DataBoundItem as EventView;
            ShowImage(eventView.Description, eventView.Id);

        }

        private void UpdateData1(RecordActions.AnyUserEventArgs e)
        {
            UpdateGrid(e);

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            PositionMe();
            formHeight = Size.Height;
        }

        private void PositionMe()
        {
            //fix the form location to right side bottom
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - (Size.Height + 90));
            //in the task panel, scroll to the top
            if (pnlTasks.Controls.Count > 0)
                pnlTasks.Controls[pnlTasks.Controls.Count - 1].Select();
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (showEventImage)
            {
                showEventImage = false;
                this.Height = 295;
                PositionMe();
                label1.Visible = false;
                pictureBox1.ImageLocation = path + @"\Images\expand-or-show.png";
                ToolTipForExpandCollapse.SetToolTip(this.pictureBox1, "Expand/Show");
                //pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("up")));
            }
            else
            {
                showEventImage = true;
                this.Height = formHeight;
                PositionMe();
                label1.Visible = true;
                pictureBox1.ImageLocation = path + @"\Images\collapse-or-hide.png";
                ToolTipForExpandCollapse.SetToolTip(this.pictureBox1, "Collapse/Hide");
                //pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("down")));
            }
        }

        private void hsbSize_Scroll(object sender, ScrollEventArgs e)
        {
            lblPercent.Text = "%" + e.NewValue.ToString();
            if (!int.TryParse(ConfigurationManager.AppSettings["TaskImageHeight"], out height))
                height = 0;
            if (!int.TryParse(ConfigurationManager.AppSettings["TaskImageWidth"], out width))
                width = 0;
            if (e.NewValue > 0 && height > 0 && width > 0)
            {
                height = (height + (height * e.NewValue) / 100);// *3;
                width = (width + (width * e.NewValue) / 100);// * 3;
            }

            if (ImageSizeChanged != null)
            {
                ImageSizeChangedArgs args = new ImageSizeChangedArgs() { Height = height, Width = width };
                ImageSizeChanged(args);
            }
        }

        private void pbEventImage_DoubleClick(object sender, EventArgs e)
        {
            //show the clicked image in a bigger window, this is needed when the captured image is 
            //too big to be visible in the available space in the main display window
            Form imageViewer = new Form() { Text = "Image Viewer in near real size" };
            imageViewer.MaximizeBox = imageViewer.MinimizeBox = false;
            imageViewer.StartPosition = FormStartPosition.CenterScreen;
            imageViewer.Size = new System.Drawing.Size(700, 500);
            imageViewer.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            PictureBox imageBox = new PictureBox() { Image = pbEventImage.Image };
            imageBox.Dock = DockStyle.Fill;
            imageViewer.Controls.Add(imageBox);
            imageViewer.ShowDialog();
        }

        private void Clean()
        {
            label1.Text = "Image";
            panel1.Visible = false;
            pbEventImage.Visible = true;
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clean();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                var eventViewer = ((EventView)this.dataGridView1.SelectedRows[0].DataBoundItem);
                WaitEventArgs waitargs = new WaitEventArgs { Interval = textBox1.Text, TaskId = eventViewer.Id };
                UpdateEvent(waitargs);
                dataGridView1.NotifyCurrentCellDirty(true);
                events.First(ev => ev.Id == eventViewer.Id).Description = "Wait(" + textBox1.Text + ")";
                dataGridView1.Refresh();
            }
            Clean();
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {
            AddWaitEvent(Process.GetCurrentProcess().MainWindowHandle);

        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            dataGridView1.Columns[0].Width = dataGridView1.Columns[2].Width = 15;
            dataGridView1.Columns[3].Visible = false;


        }

        private void ShowImage(string eventText, string taskId)
        {
            Clean();
            if (imageLocation == "")
                imageLocation = ConfigurationManager.AppSettings["TaskImageLocation"];
            if (imageLocation != null)
            {
                string imageFile = imageLocation + UseCaseId + @"\" + taskId + ".jpg";
                if (System.IO.File.Exists(imageFile))
                {
                    using (Bitmap img = new Bitmap(imageFile))
                    {
                        pbEventImage.Image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), img.PixelFormat);
                        pbEventImage.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        pbEventImage.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                    }
                }
            }
            if (eventText.Contains("Wait"))
            {
                label1.Text = "Edit wait interval";
                panel1.Visible = true;
                pbEventImage.Visible = false;

            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var eventView = dataGridView1.Rows[e.RowIndex].DataBoundItem as EventView;
            var eventText = eventView.Description;
            var taskId = eventView.Id;

            if (e.ColumnIndex == 0)
            {
                this.dataGridView1.NotifyCurrentCellDirty(true);
                events.Remove(eventView);
                TaskDeletedArgs args = new TaskDeletedArgs() { TaskId = taskId };
                TaskDeleted(args);
            }
            else if (e.ColumnIndex == 1)
            {
                ShowImage(eventText, taskId);
            }
            else if (e.ColumnIndex == 2)
            {
                Form controlPanelFrm = new ControlPanel();

                Selector captureArea = new Selector(false,false,true);
                captureArea.InstanceRef = controlPanelFrm;
                string imageLocation = "";
                string imageFilePath = "";

                imageLocation = System.Configuration.ConfigurationManager.AppSettings["TaskImageLocation"];
                if (!string.IsNullOrEmpty(imageLocation))
                {
                    if (UseCaseId != null)
                        imageFilePath = imageLocation + UseCaseId + @"\" + taskId + ".jpg";
                    else
                        imageFilePath = imageLocation + @"\" + taskId + ".jpg";

                }
                captureArea.CapturedImagePath = imageFilePath;
                captureArea.Show();
            }

        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {


        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TaskViewClosing != null)
            {
                TaskViewClosing(new TaskViewClosingEventArgs { EventList = events });
            }
            parent.updateDesc -= new EventHandler(UpdateData);
           
        }
        int counter = 1;
        private void pictureBox3_Click(object sender, EventArgs e)
        {
           
           if (counter == 1)
            {
                //Form controlPanelFrm = new ControlPanel();
                //CaptureArea captureArea = new CaptureArea();
                //captureArea.InstanceRef = controlPanelFrm;
                //string imageLocation = "";
                //string imageFilePath = "";
                //Guid guid = Guid.NewGuid();
                imageLocation = System.Configuration.ConfigurationManager.AppSettings["TaskImageLocation"];
               
                /*if active draw ,inactive close button*/

                pictureBox3.ImageLocation = path + @"\Images\information.png";
                ToolTipForCapture.SetToolTip(this.pictureBox3, "Click again to stop screen capture");
               
                //if (!string.IsNullOrEmpty(imageLocation))
                //{
                //    if (UseCaseId != null)
                //        imageFilePath = imageLocation + UseCaseId + @"\"+guid+".jpg";
                //}
                //captureArea.CapturedImagePath = imageFilePath;
                //captureArea.Show();

                screen.SubscribeScreenCapture(imageLocation);

                screen.StartCapture += new EventHandler<ScreenEventArgs>(screen_StartCapture);
                screen.EndCapture += new EventHandler<ScreenEventArgs>(screen_EndCapture);
                counter++;
               // screen.UnSubscribeScreenCapture();
            }
          else
            {
               
            screen.UnSubscribeScreenCapture();
            pictureBox3.ImageLocation = path + @"\Images\add.png";
            ToolTipForCapture.SetToolTip(this.pictureBox3, "Add Task:Press ALT+LEFT for screen capture");
                counter = 1;
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                screen.UnSubscribeScreenCapture();
                e.SuppressKeyPress = true;
                return;
            }
            base.OnKeyDown(e);
        }
        void screen_EndCapture(object sender, ScreenEventArgs e)
        {

            Debug.Write("Capture Ended : File Name " + screen.ImageName + Environment.NewLine);

        }

        void screen_StartCapture(object sender, ScreenEventArgs e)
        {

            screen.ImageName = GetGuid();
            Debug.Write("Capture Started : File Name " + screen.ImageName + Environment.NewLine);
        }
        public string GetGuid()
        {
            Guid guid = Guid.NewGuid();
            string f = guid.ToString();
            Debug.Write(" file name guid:" + f + Environment.NewLine);
            return f;
        }
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null)
                throw e.Exception;


        }
    }

    public class TaskViewClosingEventArgs : EventArgs
    {
        public BindingList<EventView> EventList { get; set; }
    }

}
