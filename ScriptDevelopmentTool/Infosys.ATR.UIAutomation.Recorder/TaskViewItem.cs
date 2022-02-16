using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infosys.ATR.UIAutomation.Recorder.ScreenCapture;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.UIAutomation.Recorder
{
    public partial class TaskViewItem : UserControl
    {
        //public variables
        //public variables
        public string UseCaseId { get; set; }

        public string Text { get { return this.lblDesc.Text; } set { lblDesc.Text = value; } }

        public class TaskDeletedArgs : EventArgs
        {
            public string TaskId { get; set; }
        }
        public delegate void TaskDeletedEventHandler(TaskDeletedArgs e);
        public event TaskDeletedEventHandler TaskDeleted;
        
        public class TaskSelectedArgs : EventArgs
        {
            public string TaskId { get; set; }
        }
        public delegate void TaskSelectedEventHandler(TaskSelectedArgs e);
        public event TaskSelectedEventHandler TaskSelected;

        public string Id { get; set; }

        //private variables
        
        
        public TaskViewItem(string desc, string id)
        {
            InitializeComponent();
            lblDesc.Text = desc;
            Id = id;
        }

        public void Highlight(bool yesOrNo)
        {
            if(yesOrNo)
                lblDesc.Font = new Font(lblDesc.Font.Name, lblDesc.Font.Size, FontStyle.Bold);
            else
                lblDesc.Font = new Font(lblDesc.Font.Name, lblDesc.Font.Size, FontStyle.Regular);
        }

        private void pbDelete_Click(object sender, EventArgs e)
        {
            if (TaskDeleted != null)
            {
                TaskDeletedArgs args = new TaskDeletedArgs() { TaskId = Id };
                TaskDeleted(args);
            }
        }

        private void lblDesc_Click(object sender, EventArgs e)
        {
            if (TaskSelected != null)
            {
                TaskSelectedArgs args = new TaskSelectedArgs() { TaskId = Id };
                TaskSelected(args);
            }
        }

        private void pbCapture_Click(object sender, EventArgs e)
        {
            Form controlPanelFrm = new ControlPanel();
           
            //controlPanelFrm.Show();
            //this.Hide();
            Selector captureArea = new Selector();
            captureArea.InstanceRef = controlPanelFrm;
            string imageLocation = "";
            string imageFilePath = "";
             
            imageLocation = System.Configuration.ConfigurationManager.AppSettings["TaskImageLocation"];
            if (!string.IsNullOrEmpty(imageLocation))
            {
                if (UseCaseId!= null)
                    imageFilePath = imageLocation + UseCaseId +  @"\" + Id + ".jpg";
                else
                    imageFilePath = imageLocation +  @"\" + Id + ".jpg";

            }
            captureArea.CapturedImagePath = imageFilePath;
            captureArea.Show();

        }
    }
}
