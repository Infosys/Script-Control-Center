using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.UIAutomation.SEE;
using System.Configuration;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Infosys.ATR.DataAnalysis;
using System.Reflection;

namespace Infosys.ATR.UIAutomation.Recorder
{

    public partial class Form1 : Form
    {
        public BindingList<EventView> _events;

        public BindingList<EventView> EventsList
        {
            get { return _events; }
            set { _events = value; }
        }

        public class RecordingStoppedArgs : EventArgs
        {
            public string ScriptPath { get; set; }
        }

        public Form2 EventViewer
        {
            get { return child; }
        }

        //public BindingList<EventView> tasks = null;

        public delegate void RecordingStoppedEventHandler(RecordingStoppedArgs e);
        public event RecordingStoppedEventHandler RecordingStopped;

        public delegate void PreviewEventHandler(RecordingStoppedArgs e);
        public event PreviewEventHandler Preview;

        public event EventHandler updateDesc;
        Form3 browserForm = new Form3();

        //public static bool isDescriptionShown = false;
        bool isRecording = false;
        public Form2 child;
        public static bool isBrowserWindowShown = false;

        int height, width;
        bool pause = false;
        string previewFilePath = "";
        string previewImageDir = "";
        UseCase original = null;
        ToolTip showDesc = null;
        string location = "";

        protected virtual void OnUpdateDesc(EventArgs e)
        {
            //var args = e as RecordActions.AnyUserEventArgs;

            //if (child != null)
            //{
            //    if (updateDesc != null)
            //    {
            //        updateDesc(this, e);
            //    }

            //}
            //else
            //{
            //    if (_events == null)
            //        _events = new BindingList<EventView>();
            //    _events.Add(new EventView
            //    {
            //        Id = args.EventId,
            //        Description = args.EventDesc,
            //        Delete = new Bitmap(@"Images\del.jpg"),
            //        Snip = new Bitmap(@"Images\camera.jpg")
            //    });
            //}

        }
        RecordActions record = new RecordActions();
        public Form1()
        {

            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //fix the form location to right bottom
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(workingArea.Right - Size.Width,
                                      workingArea.Bottom - Size.Height);

            showDesc = new ToolTip();
            showDesc.SetToolTip(btn_ShowRec, "View Event Details");
            showDesc.SetToolTip(btnRecord, "Start/Stop Recording");
            showDesc.SetToolTip(btnPause, "Pause and Preview");



        }

        /// <summary>
        /// start and stop recording on check changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRecord_Click(object sender, EventArgs e)
        {
            try
            {

                //location = String.Empty;
                if (child != null)
                    child.UseCaseId = "";
                if (isRecording == false)
                {
                    this.Text = "Recording";
                    //try cleaning the old images (if any) at the configured folder location
                    //TryCleaningImages();
                    record = new RecordActions();
                    record.HTMLContent += new RecordActions.HTMLContentEventHandler(record_HTMLContent);
                    record.AnyUserEvent += new RecordActions.AnyUserEventHandler(record_AnyUserEvent);
                    record.Start(true, height, width);
                    location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    location = Path.GetDirectoryName(location);
                    btnRecord.Image = new System.Drawing.Bitmap(Path.Combine(location + @"\Images\stop.png"));

                    //btnRecord.Image = new System.Drawing.Bitmap(Path.Combine(location + @"\Images\stop.png"));
                    isRecording = true;
                    // LaunchEvents();
                    btnPause.Enabled = true;
                }
                else if (pause)
                {
                    Tracking.RecordKeyboardKeydown(true);
                    Tracking.RecordMouseClick(true, 0, 0);
                    Tracking.RecordMouseDoubleClick(true, 0, 0);
                    this.Text = "Paused";
                    showDesc.SetToolTip(btnPause, "Pause and Preview");
                    btnRecord.Image = new System.Drawing.Bitmap(Path.Combine(location + @"\Images\stop.png"));
                    pause = false;
                    this.btnRecord.Enabled = true;
                    if (!String.IsNullOrEmpty(previewFilePath))
                        File.Delete(previewFilePath);
                }
                else
                {
                    this.Text = "Stopped";
                    string output = record.Start(false);
                    // chk_Record.BackgroundImage = new System.Drawing.Bitmap("../../Images/back - Copy.png");
                    location = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    location = Path.GetDirectoryName(location);
                    btnRecord.Image = new System.Drawing.Bitmap(Path.Combine(location + @"\Images\back - Copy.png"));
                    isRecording = false;
                    CloseBrowserForm(browserForm);
                    SaveFile(output);
                    this.btnPause.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "----" + ex.InnerException.Message, "Recorder");
            }
        }

        private void CloseBrowserForm(Form3 form)
        {
            form.Close();
            record.Browser = null;
        }
        /// <summary>
        /// event raised for wondows applications
        /// </summary>
        /// <param name="e"></param>
        void record_AnyUserEvent(RecordActions.AnyUserEventArgs e)
        {
            // OnUpdateDesc(e);
            var args = e as RecordActions.AnyUserEventArgs;

            if (child != null)
            {
                if (updateDesc != null)
                {
                    updateDesc(this, e);
                }

            }
            else
            {
                if (_events == null)
                    _events = new BindingList<EventView>();
                _events.Add(new EventView
                {
                    Id = args.EventId,
                    Description = args.EventDesc,
                    Delete = new Bitmap(@"Images\del.jpg"),
                    Snip = new Bitmap(@"Images\camera.jpg")

                });
            }
        }

        /// <summary>
        /// event raised for browser applications
        /// </summary>

        void record_HTMLContent()
        {
            if (!isBrowserWindowShown)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action<Form1>(OpenBrowserForm), new object[] { this });
                }
                else
                {
                    OpenBrowserForm(this);
                }
            }
        }
        private void OpenBrowserForm(Form1 parent)
        {
            isBrowserWindowShown = true;
            browserForm = new Form3();
            browserForm.formClosed += new EventHandler(BrowserWindow_formClosed);
            browserForm.passBrowserEvent += new PassBrowser(PassBrowserReference);

            browserForm.Show();
        }


        /// <summary>
        /// eventhandler for webBrowser control reference passed from form3 to form1
        /// </summary>
        /// <param name="browser"></param>
        private void PassBrowserReference(WebBrowser browser)
        {
            record.Browser = browser;
        }

        /// <summary>
        /// set flag when browser form window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserWindow_formClosed(object sender, EventArgs e)
        {
            isBrowserWindowShown = false;
        }

        private void LaunchEvents()
        {
            // if (!isDescriptionShown)
            {
                child = new Form2(this);
                child.ImageSizeChanged += new Form2.ImageSizeChangedEventHandler(child_ImageSizeChanged);
                child.TaskDeleted += new Form2.TaskDeletedEventHandler(child_TaskDeleted);
                child.UpdateEvent += new Form2.UpdateWaitEventHandler(child_UpdateEvent);
                child.AddWaitEvent += new Form2.AddWaitEventHandler(child_AddWaitEvent);             //  child.Show();

                child.TaskViewClosing += new Form2.TaskViewClosingEventHandler(child_TaskViewClosing);
                //  isDescriptionShown = true;
            }
        }

        void child_TaskViewClosing(TaskViewClosingEventArgs e)
        {
            // isDescriptionShown = false;          

            //this._events = this.Clone<BindingList<EventView>>(e.EventList);
            if (_events == null)
                _events = new BindingList<EventView>();
            if (e.EventList != null)
            {
                foreach (EventView ev in e.EventList)
                {
                    this._events.Add(ev);
                }
            }

            child = null;
            btn_ShowRec.Enabled = true;
        }


        /// <summary>
        /// show description window 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="e"></param>
        private void btn_ShowRec_Click(object sender, EventArgs e)
        {
            if (child == null)
                LaunchEvents();
            child.Show();
            btn_ShowRec.Enabled = false;
        }

        void child_AddWaitEvent(IntPtr handle)
        {
            Tracking.currentHandle = handle;
            record.Wait();
        }

        void child_UpdateEvent(Form2.WaitEventArgs e)
        {
            record.UpdateWait(e.Interval, e.TaskId);
        }

        void child_TaskDeleted(Form2.TaskDeletedArgs e)
        {
            record.DeleteTask(e.TaskId);
            //then also delet the image(if any) associate with the task
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TaskImageLocation"]);
            if (!string.IsNullOrEmpty(imagePath))
            {
                imagePath += @"\" + e.TaskId + ".jpg";
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }
        }

        void child_ImageSizeChanged(Form2.ImageSizeChangedArgs e)
        {
            height = e.Height;
            width = e.Width;
            if (record != null)
            {
                record.Height = height;
                record.Width = width;
            }
        }

        /// <summary>
        /// set flag when description window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void ChildFormClosed(object sender, TaskViewClosingEventArgs e)
        //{
        //    isDescriptionShown = false;
        //    child = null;

        //    this._events = this.Clone<BindingList<EventView>>(e.EventList);

        //   // LaunchEvents();
        //}

        /// <summary>
        /// save the recording in a folder 
        /// the file extension would be .imswb
        /// </summary>
        /// <param name="text"></param>
        private void SaveFile(string text)
        {
            try
            {
                //move the captured images from default locations to the new folder corresponding to the use case id
                UseCase obj = new UseCase();
                obj = SerializeAndDeserialize.Deserialize(text, typeof(UseCase)) as UseCase;


                var genLog = ConfigurationManager.AppSettings["GenerateLog"];

                if (Convert.ToBoolean(!String.IsNullOrEmpty(genLog)))
                {

                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("Name,Time,Type,Event,Keys,ControlPath,Screen,Application");              


                    obj.Activities.ForEach(a =>
                    {
                        var application = a.TargetApplication.ApplicationExe;

                        DateTime? currTime = null;

                        a.Tasks.ForEach(t =>
                        {
                            if ((t.CapturedTime == DateTime.MinValue && currTime != null))
                                t.CapturedTime = currTime.GetValueOrDefault();

                            var ctrlName = t.ControlName.Replace(',','-');                            
                            var type = t.ControlType;
                            var operation = t.Event;

                            string keys = "";

                            if (t.Event == EventTypes.KeyboardKeyPress)
                                keys = t.TargetControlAttributes.FirstOrDefault(k => k.Name == "KeyData").Value;

                            var screen = t.WindowTitle;

                            var treepath = t.ApplictionTreePath ?? String.Empty;

                            sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7}", 
                                ctrlName,currTime, type, operation, keys, t.ApplictionTreePath, screen, application));
                            currTime = t.CapturedTime;
                        });
                    });


                    string loc = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["UsecaseLocation"]);
                    if (!Directory.Exists(loc))
                    {
                        Directory.CreateDirectory(loc);
                    }
                    File.WriteAllText(Path.Combine(loc, "log.csv"), sb.ToString());
                }

                string location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TaskImageLocation"]);

                if (!string.IsNullOrEmpty(location))
                {
                    //check if the location exists, then only proceed
                    if (System.IO.Directory.Exists(location))
                    {

                        System.IO.DirectoryInfo dirSource = new DirectoryInfo(location);
                        //set the new location
                        location += @"\" + obj.Id;
                        System.IO.DirectoryInfo dirTarget = new DirectoryInfo(location);
                        if (!System.IO.Directory.Exists(location))
                            System.IO.Directory.CreateDirectory(location);

                        foreach (var file in dirSource.GetFiles())
                        {
                            file.MoveTo(dirTarget + @"\" + file.Name);
                        }
                        if (child != null)
                            child.UseCaseId = @"\" + obj.Id;
                    }
                }

                string finalPath = "";
                //FileDialog dialog = new SaveFileDialog();
                //dialog.InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
                //dialog.Filter = "Usecase XML (*.atrwb)|*.atrwb";
                //DialogResult result = dialog.ShowDialog();
                //if (result == System.Windows.Forms.DialogResult.OK)
                //{
                //    obj.Name = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                //    string editedUC = SerializeAndDeserialize.Serialize(obj);
                //    //string fileExt = dialog.FileName.Substring(dialog.FileName.IndexOf('.'));
                //    //string fileNameWOExt = dialog.FileName.Substring(0, dialog.FileName.IndexOf('.'));
                //    //once the recording is stopped and atrwb filename is give, to the file name appending _1 depicting the recording done and the base xml is generated.
                //    //finalPath = fileNameWOExt + "_1" + fileExt;
                //    finalPath = dialog.FileName;
                //    File.WriteAllText(finalPath, editedUC);
                //}
                //   string usecaseName = ShowDialog("Provide Use Case name", "Name");

                string usecaseName = Microsoft.VisualBasic.Interaction.InputBox("Provide Usecase name", "Frontend Automation");

                if (!string.IsNullOrEmpty(usecaseName))
                {
                    string folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["UsecaseLocation"]);
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }
                    string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".atrwb";
                    obj.Name = usecaseName;
                    string editedUC = SerializeAndDeserialize.Serialize(obj);
                    string pathSring = Path.Combine(folderName, filename);
                    finalPath = pathSring;
                    using (FileStream fs = File.Create(pathSring))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(editedUC);
                        fs.Write(info, 0, info.Length);
                    }

                    var extractDataLog = ConfigurationManager.AppSettings["ExtractDataLog"];
                    if (Convert.ToBoolean(string.IsNullOrEmpty(extractDataLog) ? "false" : "true"))
                    {
                        try
                        {
                            ExtractData(new List<string>() { filename });
                        }
                        catch { }
                    }
                }
                else
                {
                    string deldir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TaskImageLocation"]);
                    deldir = Path.Combine(deldir + "\\" + obj.Id);
                    Directory.Delete(deldir, true);
                }
                //raise the event  RecordingStopped
                if (RecordingStopped != null)
                {
                    RecordingStoppedArgs args = new RecordingStoppedArgs() { ScriptPath = finalPath };
                    RecordingStopped(args);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// promt user to save the recording
        /// and if user chooses to save, then ask for usecase name
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 400;
            prompt.Height = 120;
            prompt.Text = caption;
            prompt.StartPosition = FormStartPosition.CenterParent;
            Label textLabel = new Label() { Width = 150, Text = text };
            TextBox textBox = new TextBox() { Left = 20, Top = 30, Width = 350 };
            Button confirmation = new Button() { Text = "Ok", Left = 320, Width = 50, Top = 55 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();
            return textBox.Text;
        }

        private void TryCleaningImages()
        {
            try
            {
                string location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TaskImageLocation"]);
                if (!string.IsNullOrEmpty(location))
                {
                    //check if the location exists, then only proceed
                    if (System.IO.Directory.Exists(location))
                    {
                        System.IO.DirectoryInfo dir = new DirectoryInfo(location);
                        dir.GetFiles().ToList().ForEach(f =>
                        {
                            f.Delete();
                        });
                    }
                }
            }
            catch
            {
                //probably any image is being used by some other process and hence ignore
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnPause = new System.Windows.Forms.Button();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btn_ShowRec = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.SystemColors.ControlText;
            this.btnPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPause.Enabled = false;
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.Image = global::Infosys.ATR.UIAutomation.Recorder.Properties.Resources.pause;
            this.btnPause.Location = new System.Drawing.Point(81, 12);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(26, 29);
            this.btnPause.TabIndex = 4;
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnRecord
            // 
            this.btnRecord.BackColor = System.Drawing.SystemColors.ControlText;
            this.btnRecord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRecord.Image = global::Infosys.ATR.UIAutomation.Recorder.Properties.Resources.back___Copy;
            this.btnRecord.Location = new System.Drawing.Point(33, 12);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(26, 29);
            this.btnRecord.TabIndex = 3;
            this.btnRecord.UseVisualStyleBackColor = false;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btn_ShowRec
            // 
            this.btn_ShowRec.BackColor = System.Drawing.SystemColors.ControlText;
            this.btn_ShowRec.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_ShowRec.BackgroundImage")));
            this.btn_ShowRec.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_ShowRec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_ShowRec.Location = new System.Drawing.Point(129, 12);
            this.btn_ShowRec.Name = "btn_ShowRec";
            this.btn_ShowRec.Size = new System.Drawing.Size(26, 29);
            this.btn_ShowRec.TabIndex = 1;
            this.btn_ShowRec.UseVisualStyleBackColor = false;
            this.btn_ShowRec.Click += new System.EventHandler(this.btn_ShowRec_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(190, 55);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.btn_ShowRec);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Recorder";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed_1);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        private void btnSleep_Click(object sender, EventArgs e)
        {
            record.Wait();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (child != null)
                child.Close();
        }

        public T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            string finalPath = "";
            if (!pause)
            {

                //   this.btnRecord.Enabled = false;
                showDesc.SetToolTip(btnPause, "Click to continue recording");
                Tracking.RecordKeyboardKeydown(false);
                Tracking.RecordMouseClick(false, 0, 0);
                Tracking.RecordMouseDoubleClick(false, 0, 0);
                // btnPause.Image = new System.Drawing.Bitmap(Path.Combine(location + @"\Images\back - Copy.png"));
                btnRecord.Image = new System.Drawing.Bitmap(location + @"\Images\back - Copy.png");
                pause = true;

                var result = MessageBox.Show("Do you want to preview", "ATR Workbench", MessageBoxButtons.YesNo);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {

                    UseCase temp = Clone<UseCase>(record.useCase);

                    record.GroupKeyPresses(temp);
                    //roder the tasks in an activities
                    for (int i = 0; i < temp.Activities.Count; i++)
                    {
                        temp.Activities[i].Tasks = temp.Activities[i].Tasks.OrderBy(t => t.Order).ToList();
                    }
                    string usecase = SerializeAndDeserialize.Serialize(temp);

                    #region --save file and run--

                    UseCase obj = new UseCase();
                    obj = SerializeAndDeserialize.Deserialize(usecase, typeof(UseCase)) as UseCase;
                    string imglocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["TaskImageLocation"]);
                    if (!string.IsNullOrEmpty(imglocation))
                    {
                        //check if the location exists, then only proceed
                        if (System.IO.Directory.Exists(imglocation))
                        {

                            System.IO.DirectoryInfo dirSource = new DirectoryInfo(imglocation);
                            //set the new location
                            imglocation += @"\" + obj.Id;
                            System.IO.DirectoryInfo dirTarget = new DirectoryInfo(imglocation);
                            if (!System.IO.Directory.Exists(imglocation))
                                System.IO.Directory.CreateDirectory(imglocation);
                            previewImageDir = imglocation;
                            foreach (var file in dirSource.GetFiles())
                            {
                                file.MoveTo(dirTarget + @"\" + file.Name);
                            }
                            if (child != null)
                                child.UseCaseId = @"\" + obj.Id;
                        }
                    }


                    string usecaseName = "Preview";

                    if (!string.IsNullOrEmpty(usecaseName))
                    {
                        string folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["UsecaseLocation"]);
                        if (!Directory.Exists(folderName))
                        {
                            Directory.CreateDirectory(folderName);
                        }
                        string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".atrwb";
                        //string filename = "Preview.atrwb";
                        obj.Name = usecaseName;
                        string editedUC = SerializeAndDeserialize.Serialize(obj);
                        string pathSring = Path.Combine(folderName, filename);
                        previewFilePath = finalPath = pathSring;
                        using (FileStream fs = File.Create(pathSring))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes(editedUC);
                            fs.Write(info, 0, info.Length);
                        }
                    }
                    //raise the event  RecordingStopped
                    if (RecordingStopped != null)
                    {
                        RecordingStoppedArgs args = new RecordingStoppedArgs() { ScriptPath = finalPath };
                        Preview(args);
                    }

                    #endregion
                }

            }
            else
            {
                //Tracking.RecordKeyboardKeydown(true);
                //Tracking.RecordMouseClick(true, 0, 0);
                //showDesc.SetToolTip(btnPause, "Pause and Preview");
                //btnPause.Image = new System.Drawing.Bitmap(Path.Combine(location + @"\Images\pause.png"));

                //toggle = false;
                //this.chk_Record.Enabled = true;
                //File.Delete(previewFilePath);

            }
        }

        private void Form1_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            if (!String.IsNullOrEmpty(previewFilePath))
                File.Delete(previewFilePath);
            // isDescriptionShown = false;
        }

        private void ExtractData(List<string> lstUseCase)
        {
            string extractionPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataExtraction", DateTime.Now.ToString("yyyy-MM-dd-hhmmss"));

            if (!Directory.Exists(extractionPath))
                Directory.CreateDirectory(extractionPath);

            GeneratedData(DataExtractionType.All, lstUseCase, Path.Combine(extractionPath, "DataExtraction_All.csv"));
            // DataExtraction for type ControlPath
            GeneratedData(DataExtractionType.ControlPath, lstUseCase, Path.Combine(extractionPath, "DataExtraction_ControlPath.csv"));
            // DataExtraction for type ApplicationPath
            GeneratedData(DataExtractionType.ApplicationPath, lstUseCase, Path.Combine(extractionPath, "DataExtraction_ApplicationPath.csv"));
            // DataExtraction for type ApplicationsUsage
            GeneratedData(DataExtractionType.ApplicationsUsage, lstUseCase, Path.Combine(extractionPath, "DataExtraction_ApplicationsUsage.csv"));
            // DataExtraction for type ScreenPath
            GeneratedData(DataExtractionType.ScreenPath, lstUseCase, Path.Combine(extractionPath, "DataExtraction_ScreenPath.csv"));
        }


        private void GeneratedData(DataExtractionType dataExtractionType, List<string> lstUseCase, string outputPath)
        {
            List<ExtractedData> data = DataExtraction.Extract(dataExtractionType, lstUseCase);
            StringBuilder csvL1 = new StringBuilder();
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(ExtractedData).GetProperties();
            var count = propertyInfos.Count();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name.Equals("OtherInfo"))
                {
                    if (data.Exists(x => x.OtherInfo != null))
                    {
                        propertyInfos = typeof(AllExtractData).GetProperties();
                        foreach (PropertyInfo propertyInfo1 in propertyInfos)
                        {
                            csvL1.Append(string.Format("{0},", propertyInfo1.Name));
                        }
                    }
                }
                else
                    csvL1.Append(string.Format("{0},", propertyInfo.Name));
            }
            csvL1.Append(Environment.NewLine);

            foreach (var extractedData in data)
            {
                if (data.Exists(x => x.OtherInfo != null))
                {
                    foreach (var allExtractData in extractedData.OtherInfo)
                    {
                        StringBuilder csvL2 = new StringBuilder();
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.Identifier) ? "" : extractedData.Identifier));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.IdentifierIncidentCount.ToString()) ? "" : extractedData.IdentifierIncidentCount.ToString()));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.UseCaseId) ? "" : extractedData.UseCaseId));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathId) ? "" : extractedData.ScreenPathId));
                        csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathSequence.ToString()) ? "" : extractedData.ScreenPathSequence.ToString()));
                        propertyInfos = typeof(AllExtractData).GetProperties();
                        foreach (PropertyInfo propertyInfo1 in propertyInfos)
                        {
                            csvL2.Append(string.Format("{0},", allExtractData.GetType().GetProperty(propertyInfo1.Name).GetValue(allExtractData, null)));
                        }
                        csvL2.Append(Environment.NewLine);
                        csvL1.Append(csvL2.ToString());
                    }
                }
                else
                {
                    StringBuilder csvL2 = new StringBuilder();
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.Identifier) ? "" : extractedData.Identifier));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.IdentifierIncidentCount.ToString()) ? "" : extractedData.IdentifierIncidentCount.ToString()));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.UseCaseId) ? "" : extractedData.UseCaseId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathId) ? "" : extractedData.ScreenPathId));
                    csvL2.Append(string.Format("{0},", string.IsNullOrEmpty(extractedData.ScreenPathSequence.ToString()) ? "" : extractedData.ScreenPathSequence.ToString()));
                    csvL2.Append(Environment.NewLine);
                    csvL1.Append(csvL2.ToString());
                }
            }
            File.WriteAllText(outputPath, csvL1.ToString());

            if (dataExtractionType.Equals(DataExtractionType.All))
                GenerateCustomeData(data,Path.GetDirectoryName(outputPath));
        }

        private void GenerateCustomeData(List<ExtractedData> data, string extractionPath)
        {
            var allExtractData = data.Select(x => x.OtherInfo).ToList();
            List<CustomeExtract> customExtracteSummary = new List<CustomeExtract>();

            allExtractData.ForEach(edata => 
            {
                if (edata != null)
                {
                    edata.ForEach(info =>
                    {
                        var customeExtract = new CustomeExtract()
                     {
                         TaskOrder = info.TaskOrder,
                         ApplicationName = info.ApplicationName,
                         TaskControlName = info.TaskControlName,
                         TaskControlType = info.TaskControlType,
                         TaskWindowTitle = info.TaskWindowTitle,
                         TaskKeyCodeAttribute = info.TaskKeyCodeAttribute,
                         TaskEvent = info.TaskEvent,
                         TaskCreatedOn = info.TaskCreatedOn,
                         UseCaseCreatedBy = info.UseCaseCreatedBy,
                         ScreenId = info.ScreenId,
                         TaskId = info.TaskId,
                         Summary = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", info.ApplicationName, info.TaskEvent, info.TaskControlName, info.TaskControlType, info.TaskKeyCodeAttribute, info.TaskWindowTitle),

                     };
                        if (customeExtract != null)
                            customExtracteSummary.Add(customeExtract);
                    });
                }
            });

            StringBuilder csvL1 = new StringBuilder();
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(CustomeExtract).GetProperties();            
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                csvL1.Append(string.Format("{0},", propertyInfo.Name));
            }
            csvL1.Append(Environment.NewLine);

            foreach (var extractedData in customExtracteSummary)
            {
                StringBuilder csvL2 = new StringBuilder();
                foreach (PropertyInfo propInfo in propertyInfos)  
                {
                    csvL2.Append(string.Format("{0},", extractedData.GetType().GetProperty(propInfo.Name).GetValue(extractedData, null)));
                }
                csvL2.Append(Environment.NewLine);
                csvL1.Append(csvL2.ToString());
            }
            File.WriteAllText(Path.Combine(extractionPath, "DataExtraction_Summary.csv"), csvL1.ToString());
        }
    }
    public class EventView
    {
        public System.Drawing.Image Delete { get; set; }
        public string Description { get; set; }
        public System.Drawing.Image Snip { get; set; }
        public string Id { get; set; }
    }



    public class CustomeExtract
    {        
        public int TaskOrder { get; set; }
        public string UseCaseCreatedBy { get; set; }
        public string TaskCreatedOn { get; set; }
        public string ApplicationName { get; set; }
        public string ScreenId { get; set; }
        public string TaskId { get; set; }
        public string TaskEvent { get; set; }
        public string TaskControlName { get; set; }
        public string TaskControlType { get; set; }
        public string TaskKeyCodeAttribute { get; set; }
        public string TaskWindowTitle { get; set; }        
        public string Summary { get; set; }
    }
}
