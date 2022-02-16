using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Constants;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.CompositeUI.WinForms;

namespace Infosys.ATR.Editor.Views
{
    public partial class PythonIDE : UserControl,IPythonIDE,IClose
    {
        int splitterDistance = 0;
        bool isSplitterCollapsed = false;
        string _filePath = "";
        internal string _ucName;
        BackgroundWorker bkg_runPython = new BackgroundWorker();
        public string ucName { get { return _ucName; } set { _ucName = value; } }


        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        //public string Richtext { get; set; }
        private string richText;
        private string textToSave;
        private string oldBatchContent;

        public string TextToSave
        {
            get { return textToSave; }
            set { scintilla1.AddText(value); }
        }

        public string RichText
        {
            get { return scintilla1.Text; }
            set
            {
                scintilla1.AddText(value);
            }
        }
        
        public PythonIDE(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                _filePath = args[0];
        }
        
        public PythonIDE()
        {
            InitializeComponent();
        }

        private void IDE_Load(object sender, EventArgs e)
        {
           // PopulateFunctions();
            splitterDistance = splitContainer1.SplitterDistance;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.Panel1Collapsed = true;
            //load file if provided
            if (!string.IsNullOrEmpty(_filePath))
            {
                if (System.IO.File.Exists(_filePath))
                {
                    scintilla1.Text = System.IO.File.ReadAllText(_filePath);
                }
            }

            //set the language
            scintilla1.ConfigurationManager.Language = "python";

            bkg_runPython.DoWork += new DoWorkEventHandler(bkg_runPython_DoWork);
            bkg_runPython.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkg_runPython_RunWorkerCompleted);
        }

        void lbl_MouseLeave(object sender, EventArgs e)
        {
            (sender as Label).ForeColor = Color.Black;
        }

        void lbl_MouseHover(object sender, EventArgs e)
        {
            (sender as Label).ForeColor = Color.Blue;
        }

        void lbl_Click(object sender, EventArgs e)
        {
            string text = (sender as Label).Text;
            scintilla1.AddText(text);
        }

        private void btnGetCtl_Click(object sender, EventArgs e)
        {

        }
        
        public void GetCtl_Click()
        {
            scintilla1.Name = "rtxtEditor";
            //ControlExplorer exp = new ControlExplorer();
            //exp.AutoStartControlExp += new ControlExplorer.AutoStartControlExpEventHandler(exp_AutoStartControlExp);
            //exp.ShowDialog();
            //rtxtEditor.SelectionLength = 0;
            //if (expRichtext != null)
            //    rtxtEditor.AddText(expRichtext, Color.Green);

        }

        private void splitContainer1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isSplitterCollapsed)
            {
                splitContainer1.SplitterDistance = 30;
                isSplitterCollapsed = true;
                lblFuns.Text = "F\nu\nn\nc\nt\ni\no\nn\ns";
                splitContainer1.Panel1.BackColor = Color.LightGray;
            }
            else
            {
                splitContainer1.SplitterDistance = 200;//splitterDistance;
                isSplitterCollapsed = false;
                lblFuns.Text = "Functions";
                splitContainer1.Panel1.BackColor = Color.White;
            }
        }

        private void splitContainer1_Panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isSplitterCollapsed)
            {
                splitContainer1.SplitterDistance = 30;
                isSplitterCollapsed = true;
                lblFuns.Text = "F\nu\nn\nc\nt\ni\no\nn\ns";
                splitContainer1.Panel1.BackColor = Color.LightGray;
            }
            else
            {
                splitContainer1.SplitterDistance = 200;//splitterDistance;
                isSplitterCollapsed = false;
                lblFuns.Text = "Functions";
                splitContainer1.Panel1.BackColor = Color.White;
            }
        }

        private void toolStripbtnGetCtl_Click(object sender, EventArgs e)
        {
            Infosys.ATR.DevelopmentStudio.ControlExplorer exp = new Infosys.ATR.DevelopmentStudio.ControlExplorer();
            exp.AutoStartControlExp += new Infosys.ATR.DevelopmentStudio.ControlExplorer.AutoStartControlExpEventHandler(exp_AutoStartControlExp);
            exp.ShowDialog();
            //rtxtEditor.SelectionLength = 0;
            if (exp.Result != null)
                scintilla1.AddText(exp.Result);
        }

        void exp_AutoStartControlExp()
        {
            System.Threading.Thread.Sleep(500);
            toolStripbtnGetCtl_Click(null, null);
        }

        private void toolStripbtnRun_Click(object sender, EventArgs e)
        {

        }

        private void toolStripbtnSave_Click(object sender, EventArgs e)
        {

        }

        private void PopulateFunctions()
        {
            string functionSetting = ConfigurationManager.AppSettings["Functions"];
            if (!string.IsNullOrEmpty(functionSetting))
            {
                int x = 36, y = 35, yOffset = 28;
                string[] functions = functionSetting.Split(';');
                for (int i = 0; i < functions.Length; i++)
                {
                    Label lblFunc = new Label();
                    lblFunc.Location = new Point(x, y + i * yOffset);
                    lblFunc.Click += new EventHandler(lbl_Click);
                    lblFunc.MouseHover += new EventHandler(lbl_MouseHover);
                    lblFunc.MouseLeave += new EventHandler(lbl_MouseLeave);
                    lblFunc.Text = functions[i] + "(...)";
                    lblFunc.Font = new Font(lblFunc.Font.Name, lblFunc.Font.Size, FontStyle.Bold);
                    lblFunc.AutoSize = true;
                    splitContainer1.Panel1.Controls.Add(lblFunc);
                }
            }
        }
        [EventSubscription(EventTopicNames.Save, ThreadOption.UserInterface)]
        public void Save(object sender, EventArgs e)
        {
            string text = scintilla1.Text;

        }

        /// <summary>
        /// Executes a shell command synchronously with the provided Python script.
        /// </summary>
        public void ExecutePythonScript()
        {
            try
            {
                string pythonIntLoc = ConfigurationManager.AppSettings["PythonInterpreterLoc"];
                if (!string.IsNullOrEmpty(pythonIntLoc) && !string.IsNullOrEmpty(_filePath))
                {
                    //check if the python interpreter is present at the configured location
                    if (!System.IO.File.Exists(pythonIntLoc))
                    {
                        MessageBox.Show("Python Interpreter is not found at the configured location- " + pythonIntLoc, "Error: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    //create python command
                    string command = pythonIntLoc + " \"" + _filePath +  "\"";

                    // Create the ProcessStartInfo using "cmd" as the program to be run,
                    // and "/c " as the parameters.
                    // "/c" tells cmd that you want it to execute the command that follows,
                    // then exit.
                    System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                    // The following commands are needed to redirect the standard output/ error.
                    // This means that it will be redirected to the Process.StandardOutput/RedirectStandardError StreamReader.
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.RedirectStandardError = true;
                    procStartInfo.UseShellExecute = false;

                    // Do not create the black window.
                    procStartInfo.CreateNoWindow = true;

                    // Now you create a process, assign its ProcessStartInfo, and start it.
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    // Get the output into a string.
                    string result = proc.StandardOutput.ReadToEnd();
                    string errorIfAny = proc.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(errorIfAny))
                        MessageBox.Show(errorIfAny, "Error: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        // Display the command output.
                        MessageBox.Show("Execution done with result: " + result, "Success: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception objException)
            {
                MessageBox.Show(objException.Message, "Exception: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executes a shell command synchronously with the provided Python script but thru the batch file.
        /// This is provided to support custom execution the python script has external dependencies like jython.
        /// </summary>
        public void ExecutePythonScriptThruBat()
        {
            try
            {
                if (!string.IsNullOrEmpty(_filePath) && UpdatePythonBatch())
                {                    
                    //create python command thru bat
                    string command = "RunPython.bat \"" + _filePath + "\"";

                    // Create the ProcessStartInfo using "cmd" as the program to be run,
                    // and "/c " as the parameters.
                    // "/c" tells cmd that you want it to execute the command that follows,
                    // then exit.
                    System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                    // The following commands are needed to redirect the standard output/ error.
                    // This means that it will be redirected to the Process.StandardOutput/RedirectStandardError StreamReader.
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.RedirectStandardError = true;
                    procStartInfo.UseShellExecute = false;

                    // Do not create the black window.
                    procStartInfo.CreateNoWindow = true;

                    // Now you create a process, assign its ProcessStartInfo, and start it.
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    // Get the output into a string.
                    string result = proc.StandardOutput.ReadToEnd();
                    string errorIfAny = proc.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(errorIfAny))
                        MessageBox.Show(errorIfAny, "Error: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        // Display the command output.
                        MessageBox.Show("Execution done with result: " + result, "Success: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ReveretBatchFile("RunPython.bat");
                }
            }
            catch (Exception objException)
            {
                MessageBox.Show(objException.Message, "Exception: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executes a shell command synchronously with the provided Sikuli script but thru the batch file.
        /// </summary>
        public void ExecuteSikuliScriptThruBat()
        {
            try
            {
                if (!string.IsNullOrEmpty(_filePath) && UpdateSikuliBatch())
                {
                    //create sikuli command thru bat
                    string command = "RunSikuli.bat";

                    // Create the ProcessStartInfo using "cmd" as the program to be run,
                    // and "/c " as the parameters.
                    // "/c" tells cmd that you want it to execute the command that follows,
                    // then exit.
                    System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                    // The following commands are needed to redirect the standard output/ error.
                    // This means that it will be redirected to the Process.StandardOutput/RedirectStandardError StreamReader.
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.RedirectStandardError = true;
                    procStartInfo.UseShellExecute = false;

                    // Do not create the black window.
                    procStartInfo.CreateNoWindow = true;

                    // Now you create a process, assign its ProcessStartInfo, and start it.
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    // Get the output into a string.
                    string result = proc.StandardOutput.ReadToEnd();
                    string errorIfAny = proc.StandardError.ReadToEnd();

                    if (!string.IsNullOrEmpty(errorIfAny))
                        MessageBox.Show(errorIfAny, "Error: Sikuli Runtime", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        // Display the command output.
                        MessageBox.Show("Execution done with result: " + result, "Success: Sikuli Runtime", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ReveretBatchFile("RunSikuli.bat");
                }
            }
            catch (Exception objException)
            {
                MessageBox.Show(objException.Message, "Exception: Sikuli Runtime", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool UpdatePythonBatch()
        {
            bool result = false;
            string pythonIntLoc = ConfigurationManager.AppSettings["PythonInterpreterLoc"];
            //check if the python interpreter is present at the configured location
            if (!string.IsNullOrEmpty(pythonIntLoc) && System.IO.File.Exists(pythonIntLoc))
            {
                string batchContent = System.IO.File.ReadAllText("RunPython.bat");
                oldBatchContent = batchContent;
                batchContent = batchContent.Replace("##PythonInterpreterLoc##", pythonIntLoc);
                System.IO.File.WriteAllText("RunPython.bat", batchContent);
                result = true;
            }
            else
                MessageBox.Show("Python Interpreter is not found at the configured location- " + pythonIntLoc, "Error: Python Interpreter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return result;
        }

        private bool UpdateSikuliBatch()
        {
            bool result = false;
            string sikuliHome = ConfigurationManager.AppSettings["Sikulihome"];
            string sikuliProjectHome = ConfigurationManager.AppSettings["ProjectHome"];
            //check if the python interpreter is present at the configured location
            if (!string.IsNullOrEmpty(sikuliHome) && !string.IsNullOrEmpty(sikuliProjectHome))
            {
                string batchContent = System.IO.File.ReadAllText("RunSikuli.bat");
                oldBatchContent = batchContent;
                batchContent = batchContent.Replace("##Sikulihome##", sikuliHome);
                batchContent = batchContent.Replace("##ProjectHome##", sikuliProjectHome);
                //assign just the file name without the .py ext
                string fileName = _filePath.Replace(".py", "");
                string[] fileNameParts = fileName.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (fileNameParts.Length > 0)
                {
                    fileName = fileNameParts[fileNameParts.Length - 1];
                    batchContent = batchContent.Replace("##FileName##", fileName);
                    System.IO.File.WriteAllText("RunSikuli.bat", batchContent);
                    result = true;
                }
            }
            else
                MessageBox.Show("Sikuli needed locations are not properly confiured. ", "Error: Sikuli Runtime", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return result;
        }

        private void ReveretBatchFile(string batchFile)
        {
            System.IO.File.WriteAllText(batchFile, oldBatchContent);
        }

        public void Close()
        {
            this._presenter.OnCloseView();
            var t = this._presenter.WorkItem.State["pyEditors"] as Dictionary<string, PythonIDE>;
            t.Remove(this.ucName);
        }

        public string CurrentFile { get; set; }

        internal void Run()
        {
            var ifeaPath = (String)  this._presenter.WorkItem.State["ifeaPath"] ;

            if (String.IsNullOrEmpty(ifeaPath))
                MessageBox.Show("Python executable path not set. Set the path in python settings", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (!string.IsNullOrEmpty(CurrentFile))
                    // Services.Utilities.RunPython(ifeaPath, CurrentFile);
                    bkg_runPython.RunWorkerAsync(ifeaPath);
            }
        }


        void bkg_runPython_DoWork(object sender, DoWorkEventArgs e)
        {
            Services.Utilities.RunPython(e.Argument.ToString(), CurrentFile);
        }

        void bkg_runPython_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                throw new Exception(e.Error.ToString());
        }


    }
}
