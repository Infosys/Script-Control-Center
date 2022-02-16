using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infosys.WEM.ScriptExecutionLibrary;
using Infosys.WEM.WorkflowExecutionLibrary;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;

namespace ExecuteScript
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Script script = new Script();
            //script.ExecutionDir = @"D:\IMS\TFS\script";
            //script.ScriptName = "greet.bat";
            ////script.ScriptName = "dir";
            ////ExecuteBase client = new ExecuteBat();
            ////ExecuteBase client = new ExecuteDefault();
            ////client.InitializeClient(script);
            ////ExecutionResult resutl = client.Start();
            //ScriptExecutionManager manager = new ScriptExecutionManager();
            //ExecutionResult resutl =  manager.Execute(script);

            ScriptIndentifier scriptIden = new ScriptIndentifier();
            scriptIden.ScriptId = 3;
            //scriptIden.CompanyId = 1;
            scriptIden.SubCategoryId = 4;
            scriptIden.WEMScriptServiceUrl = "http://localhost/iapwemservices/WEMScriptService.svc";// "http://localhost:61335/WEMScriptService.svc";

           
            ScriptExecutionManager.RetrievedScriptMetadata += new ScriptExecutionManager.RetrievedScriptMetadataEventHandler(ScriptExecutionManager_RetrievedScriptMetadata);
            ScriptExecutionManager.DownloadingScriptFile += new ScriptExecutionManager.DownloadingScriptFileEventHandler(ScriptExecutionManager_DownloadingScriptFile);
            ScriptExecutionManager.DownloadedScriptFile += new ScriptExecutionManager.DownloadedScriptFileEventHandler(ScriptExecutionManager_DownloadedScriptFile);
            ScriptExecutionManager.ProcessingScript += new ScriptExecutionManager.ProcessingScriptEventHandler(ScriptExecutionManager_ProcessingScript);
            ScriptExecutionManager.ScriptExecutionStarting += new ScriptExecutionManager.ScriptExecutionStartingEventHandler(ScriptExecutionManager_ScriptExecutionStarting);
            ScriptExecutionManager.ScriptExecutionCompleted += new ScriptExecutionManager.ScriptExecutionCompletedEventHandler(ScriptExecutionManager_ScriptExecutionCompleted);
            ScriptExecutionManager.ScriptProcessed += new ScriptExecutionManager.ScriptProcessedEventHandler(ScriptExecutionManager_ScriptProcessed);
            Infosys.WEM.ScriptExecutionLibrary.ExecutionResult result = ScriptExecutionManager.Execute(scriptIden);

            if (result.IsSuccess)
                textBox1.Text = result.SuccessMessage;
            else
                textBox1.Text = result.ErrorMessage;
        }

        void ScriptExecutionManager_ScriptProcessed(ScriptExecutionManager.ScriptProcessedArgs e)
        {
            
        }

        void ScriptExecutionManager_ScriptExecutionCompleted(ScriptExecutionManager.ScriptExecutionCompletedArgs e)
        {
            
        }

        void ScriptExecutionManager_ScriptExecutionStarting()
        {
            
        }

        void ScriptExecutionManager_ProcessingScript()
        {
            
        }

        void ScriptExecutionManager_DownloadedScriptFile(ScriptExecutionManager.DownloadedScriptFileArgs e)
        {
            
        }

        void ScriptExecutionManager_DownloadingScriptFile()
        {
            
        }

        void ScriptExecutionManager_RetrievedScriptMetadata(ScriptExecutionManager.RetrievedScriptMetadataArgs e)
        {
            
        }

        private void btnExeWf_Click(object sender, EventArgs e)
        {
            WorkflowIndentifier wfid = new WorkflowIndentifier();
            wfid.WEMWorkflowServiceUrl = "http://localhost/iapwemservices/WEMService.svc";
            wfid.CategoryId = 57;
            wfid.WorkflowId = Guid.Parse("5150E9A7-430A-41CF-831E-4FC42EF59876");
            wfid.WorkflowVersion = 1;

            Infosys.WEM.WorkflowExecutionLibrary.Entity.ExecutionResult result = new WorkflowExecutionManager().Execute(wfid);
            if (result.IsSuccess)
                textBox1.Text = result.SuccessMessage;
            else
                textBox1.Text = result.ErrorMessage;
        }
    }
}
