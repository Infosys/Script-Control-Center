using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.ATR.DevelopmentStudio;
using System.Configuration;

namespace PythonEditor
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        PythonIDE ide;

        private void Form2_Load(object sender, EventArgs e)
        {
            ide = new PythonIDE();
            ide.Dock = DockStyle.Fill;
            panel1.Controls.Add(ide);
        }

        private void toolStripButton_Run_Click(object sender, EventArgs e)
        {
            //ExecutePythonCommand(@"C:\Usecase\IDEprojects\python.py");
            ide.FilePath = @"C:\Usecase\IDEprojects\python.py";
            ide.ExecutePythonScriptThruBat();
        }

        /// <summary>
        /// Executes a shell command synchronously with the provided Python script.
        /// </summary>
        /// <param name="pythonScriptFilePath"></param>
        public void ExecutePythonCommand(string pythonScriptFilePath)
        {
            try
            {
                string pythonIntLoc = ConfigurationManager.AppSettings["PythonInterpreterLoc"];
                if (!string.IsNullOrEmpty(pythonIntLoc))
                {
                    //create python command
                    string command = pythonIntLoc + " " + pythonScriptFilePath;
                    //string command = pythonIntLoc + " -c \"" + System.IO.File.ReadAllText(pythonScriptFilePath) + "\"";

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
    }
}
