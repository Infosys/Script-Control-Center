using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.UIAutomation.SEE;
using Infosys.ATR.UIAutomation.Entities;
using System.Text.RegularExpressions;

namespace TrackActivities
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UseCase usecase1 = new UseCase();
            usecase1.Id = "UC1";
            usecase1.Name = "Usecase 1";
            usecase1.Description = "details of usecase 1";
            usecase1.CreatedBy = "admin";
            usecase1.CreatedOn = DateTime.Now;
            usecase1.Domain = "DomainName";
            usecase1.MachineIP = "127.0.0.0";
            usecase1.MachineName = "127.0.0.1";
            usecase1.MachineType = "32 bit";
            usecase1.OS = "win 7";
            usecase1.Activities = new List<Activity>();


            Activity act = new Activity();
            act.Description = "description for activity 1";
            act.Id = "AC1";
            act.Name = "activity1";
            act.TargetApplication = new ApplicationDetails();
            act.TargetApplication.ApplicationType = ApplicationTypes.Win32.ToString();
            act.TargetApplication.TargetApplicationAttributes = new List<NameValueAtribute>();
            act.TargetApplication.TargetApplicationAttributes.Add(new NameValueAtribute("Name", "calc"));
            act.TargetApplication.TargetApplicationAttributes.Add(new NameValueAtribute("ModuleName", "calc.exe"));
            act.TargetApplication.TargetApplicationAttributes.Add(new NameValueAtribute("FilePath", @"C:\Windows\System32\calc.exe"));

            act.Tasks = new List<Task>();
            Task task = new Task();
            task.Description = "description for task 1";
            task.Id = "TSK1";
            task.Name = "task1";
            task.Order = 1;
            task.ControlId = "ctl1";
            task.ControlName = "button1";
            task.ControlType = "Button";
            task.Event = EventTypes.MouseLeftClick;
            
            task.TargetControlAttributes = new List<NameValueAtribute>();
            task.TargetControlAttributes.Add(new NameValueAtribute("Name", "Backspace"));
            task.TargetControlAttributes.Add(new NameValueAtribute("AutomationId", "83"));
            task.TargetControlAttributes.Add(new NameValueAtribute("ControlType", "Button"));
            task.TargetControlAttributes.Add(new NameValueAtribute("AccessKey", "ControlType.Button"));
            task.TargetControlAttributes.Add(new NameValueAtribute("ClassName", "Button"));
            task.TargetControlAttributes.Add(new NameValueAtribute("Rectangle", "229,317,34,27"));
            task.TargetControlAttributes.Add(new NameValueAtribute("ControlPatterns", "InvokePatternIdentifiers.Pattern"));

            act.Tasks.Add(task);
            task.Order = 2;
            act.Tasks.Add(task);
            usecase1.Activities.Add(act);
            act.ParentId = "AC1";
            usecase1.Activities.Add(act);

            string strUC = SerializeAndDeserialize.Serialize(usecase1);
            textBox1.Text = strUC;
            UseCase uc2 =  SerializeAndDeserialize.Deserialize(strUC, typeof(UseCase)) as UseCase;
        }

        private void btnkeyboard_Click(object sender, EventArgs e)
        {
            Tracking.KeyBoardKeyPresed += new Tracking.KeyBoardKeyPresedEventHandler(Tracking_KeyBoardKeyPresed);
            Tracking.KeyBoardFlushCatured += new Tracking.KeyBoardFlushCaturedEventHandler(Tracking_KeyBoardFlushCatured);
            Tracking.RecordKeyboardKeydown(true);            
        }

        void Tracking_KeyBoardFlushCatured(Tracking.KeyBoardFlushCaturedEventArgs e)
        {
            //textBox1.Text = e.KeyCodes;
        }

        void Tracking_KeyBoardKeyPresed(Tracking.KeyBoardKeyPresedEventArgs e)
        {
            //textBox1.Text = e.Task.TargetControlAttributes[0].Value;
            textBox1.Text = e.Task.Description;
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            Tracking.RecordMouseClick(true);
            Tracking.MouseClicked += new Tracking.MouseClickedEventHandler(Tracking_MouseClicked);
        }

        void Tracking_MouseClicked(Tracking.MouseClickedEventArgs e)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.BeginInvoke(new Action<Tracking.MouseClickedEventArgs>(UpdateText), new object[] { e });
                return;
            }
            UpdateText(e);
        }

        void UpdateText(Tracking.MouseClickedEventArgs e)
        {
            textBox1.Text = e.Task.Description + Environment.NewLine;
            textBox1.Text += e.Task.ControlName + Environment.NewLine;
            e.Task.TargetControlAttributes.ForEach(c =>
            {
                textBox1.Text += c.Name + ": " + c.Value + Environment.NewLine;
            });
        }

        RecordActions record = new RecordActions();
        private void btnRecord_Click(object sender, EventArgs e)
        {
            record = new RecordActions();
            record.HTMLContent += new RecordActions.HTMLContentEventHandler(record_HTMLContent);
            record.AnyUserEvent += new RecordActions.AnyUserEventHandler(record_AnyUserEvent);

            record.Start();
        }

        void record_AnyUserEvent(RecordActions.AnyUserEventArgs e)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.BeginInvoke(new Action<RecordActions.AnyUserEventArgs>(record_AnyUserEvent), new object[] { e });
                return;
            }
            textBox1.Text += e.EventDesc + Environment.NewLine;
        }

        void record_HTMLContent()
        {
            MessageBox.Show("html content...");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            textBox1.Text = record.Start(false);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PlayActions play = new PlayActions();
            play.GetApplications(txtUc.Text);
            play.ExecuteActionsOn(txtApp.Text, new IntPtr(int.Parse(txtHandle.Text)), int.Parse(txtProcId.Text));
        }

        private void btnMachineDetails_Click(object sender, EventArgs e)
        {
            String stringToEdit = "string to replace characters in!!!! - _";
            stringToEdit = Regex.Replace(stringToEdit, "[^A-Za-z0-9 _ -]", "").Replace(" ","");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string str= "/0[a]/1[b]/2[cd]/3[e]/4[f]";
            string[] strs = str.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);
            List<string> levels = new List<string>();
            List<string> newlevels = new List<string>();
            List<string> peerIndexes = new List<string>();
            foreach (string st in strs)
            {
                string[] sts = st.Substring(0, st.Length-1).Split('[');
                levels.Add(sts[0]);
                peerIndexes.Add(sts[1]);
            }
            peerIndexes.Reverse();
            str="";
            for (int i = 0; i < levels.Count; i++)
            {
                str += "/" + levels[i] + "[" + peerIndexes[i] + "]";
            }
        }
    }
}
