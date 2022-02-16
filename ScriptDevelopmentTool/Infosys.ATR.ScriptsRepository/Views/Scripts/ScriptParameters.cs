using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net.NetworkInformation;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Infosys.ATR.ScriptRepository.Views;
using Infosys.ATR.CommonViews;
using Microsoft.Practices.CompositeUI.EventBroker;
using IMSWorkBench.Infrastructure.Interface;
using Infosys.WEM.ScriptExecutionLibrary;
using Infosys.WEM.SecureHandler;
using System.IO;

namespace Infosys.ATR.ScriptsRepository.Views.Scripts
{
    public partial class ScriptParameters : Form
    {
        // Delegate to handle event
        public delegate void ParametersUpdateHandler();
        public event ParametersUpdateHandler ParametersUpdated;

        private List<TextBox> txtBoxes;
        public int parameterCount = 0;
        List<string> myCollection = new List<string>();
        List<Infosys.ATR.ScriptRepository.Models.ScriptParameter> parameters = null;
        // for setting grpBoxRunScriptOptions height
        private const int intGrpBoxRunScriptHeight = 50;
        private const int inGgrpBoxRemoteParametersHeight = 110;
        private ScriptExecutionMode executionMode;
        Infosys.ATR.AutomationClient.ucIAPNodes nodesUc = new AutomationClient.ucIAPNodes(AutomationClient.ExecutionType.Script);





        public delegate void NodeExecutedEventHandler(List<ExecutionResultView> e);
        public event NodeExecutedEventHandler NodeExecuted;

        public delegate void IapNode_Executed(bool executed);
        public event IapNode_Executed IapNodeExecuted_EventHandler;

        Infosys.ATR.ScheduleRequestUI.ScheduleRequestUI ucScheduleRequest = new Infosys.ATR.ScheduleRequestUI.ScheduleRequestUI(2);

        private string remoteServerFilePath = "";
    

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScriptParameters()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with script parameters. This method calls GenerateParameterTextBoxes method and
        /// sets the position of various controls on the form.
        /// </summary>
        /// <param name="scriptParams">List of script parameters</param>
        public ScriptParameters(List<Infosys.ATR.ScriptRepository.Models.ScriptParameter> scriptParams)
        {

            InitializeComponent();
            parameters = new List<Infosys.ATR.ScriptRepository.Models.ScriptParameter>();
            parameters = scriptParams;
            GenerateParameterTextBoxes();

            if (MainRepositoryView.showSSHForm || MainRepositoryView.allowRemote==true)
            {
                remoteServerFilePath = GetAppPath() + "\\" + Infosys.ATR.ScriptRepository.Constants.Application.ServerFileName;
            }

            ResetControls();
            cmbShellServerList.Visible = false;
            if (MainRepositoryView.UsesUIAutomation)
                rdIAPNode.Enabled = false;
           else if (MainRepositoryView.showSSHForm)
            {
                EnableLinuxControls();
            }
           else if (MainRepositoryView.allowRemote==true)
            {
                GetServersList();
            }
            else
            {
                ResetControls();
                //DisableLinuxControls();
            }
        }

        /// <summary>
        ///  This method generates textboxes for each parameter and sets position of the same.
        /// </summary>
        private void GenerateParameterTextBoxes()
        {
            if (parameters != null && parameters.Count > 0)
            {
                txtBoxes = new List<TextBox>();
                parameterCount = parameters.Count();
                int intParamsScroll = 5;

                //Generate labels and text boxes
                for (int count = 0; count < parameterCount; count++)
                {
                    //Create a new label and text box
                    Label lblInput = new Label();
                    Label lblMandatory = new Label();
                    TextBox txtParameter = new TextBox();

                    if (parameters[count].Name.Length > 28)
                    {
                        lblInput.Text = parameters[count].Name.Substring(1, 25) + "...";
                        this.toolTip1.SetToolTip(lblInput, parameters[count].Name);
                    }
                    else
                        lblInput.Text = parameters[count].Name;

                    //lblInput.Width = lblServerName.Width;
                    if (parameterCount == 1)
                        lblInput.Location = new Point(0, 10);
                    else
                        lblInput.Location = new Point(0, 10 + (count * 30));
                    lblInput.AutoSize = true;

                    lblMandatory.Location = new Point(lblInput.Right + 5, lblInput.Location.Y + 5);
                    if (parameters[count].IsMandatory)
                    {
                        lblMandatory.Text = "*";
                        lblMandatory.Font = new Font(lblMandatory.Font, FontStyle.Bold);
                        lblMandatory.ForeColor = Color.Red;
                        lblMandatory.TextAlign = ContentAlignment.MiddleLeft;
                    }
                    lblMandatory.AutoSize = true;

                    //Initialize textBoxes Property
                    txtParameter.Location = new Point(lblMandatory.Width + 70, lblInput.Location.Y);
                    if (parameterCount > intParamsScroll)
                        txtParameter.Width = 375;
                    else
                        txtParameter.Width = 395;
                    txtParameter.Name = "TextBox" + count;
                    txtParameter.Text = parameters[count].DefaultValue;
                    if (parameters[count].IsSecret)
                    {
                        txtParameter.UseSystemPasswordChar = true;
                    }

                    txtBoxes.Add(txtParameter);

                    pnlTextBoxes.Controls.Add(lblInput);
                    pnlTextBoxes.Controls.Add(lblMandatory);
                    pnlTextBoxes.Controls.Add(txtParameter);

                    // Set panel scrollbar if number of parameters is greater than 5
                    if (parameterCount > intParamsScroll)
                    {
                        pnlTextBoxes.AutoScroll = true;
                        pnlTextBoxes.HorizontalScroll.Visible = false;
                        pnlTextBoxes.Height = intParamsScroll * 32;
                    }
                    else
                    {
                        pnlTextBoxes.Height = parameterCount * 32;
                    }

                    grpBoxScriptParams.Height = pnlTextBoxes.Height + 20;
                }
            }
            else
                grpBoxScriptParams.Visible = false;

        }

        //public void IapNode_ExecutedHandler(bool executed)
        //{
        //    IapNode_ExecutedHandler(executed);
        //}

        /// <summary>
        /// This method is used to verify if user has entered value for the mandatory parameters.
        /// </summary>
        /// <returns>true if validation is passed</returns>
        private Boolean ValidateParameters()
        {
            int count = 0;
            foreach (Control txtBox in this.pnlTextBoxes.Controls)
            {

                if (txtBox is TextBox)
                {
                    if (parameters[count].IsMandatory && (String.IsNullOrEmpty(((TextBox)txtBox).Text)))
                    {
                        MessageBox.Show("Please enter value for parameter " + parameters[count].Name);
                        txtBox.Focus();
                        return false;
                    }
                    count = count + 1;
                }
            }

            if (MainRepositoryView.allowRemote && rdbRemote.Checked)
            {
                // Perform validation for remote servers
                if (lstServerName.Items.Count == 0)
                {
                    MessageBox.Show("Please enter remote server name to list");
                    txtServerName.Focus();
                    return false;
                }

                // Check this validation for vb.js,py,bat file type
                if (MainRepositoryView.networkParamsValidation)
                {
                    if (string.IsNullOrEmpty(txtUserName.Text))
                    {
                        MessageBox.Show("Please enter User Alias");
                        txtUserName.Focus();
                        return false;
                    }

                    if (string.IsNullOrEmpty(txtPassword.Text))
                    {
                        MessageBox.Show("Please enter Password");
                        txtPassword.Focus();
                        return false;
                    }

                }


            }
            else if (MainRepositoryView.showSSHForm)
            {
                // Perform validation for Linux servers
                //if (string.IsNullOrEmpty(txtServerName.Text))
                //{
                //    MessageBox.Show("Please enter Linux server name");
                //    txtServerName.Focus();
                //    return false;
                //}
                if (lstServerName.Items.Count == 0)
                {
                    MessageBox.Show("Please enter remote server name to list");
                    txtServerName.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    MessageBox.Show("Please enter User Alias");
                    txtUserName.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please enter Password");
                    txtPassword.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method is used to add server name in the listbox.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">EventArgs object containing event data</param>
        private void btnAddToList_Click(object sender, EventArgs e)
        {
            string serverName = cmbShellServerList.Text;
            if (MainRepositoryView.showSSHForm)
            {            
                if (!string.IsNullOrEmpty(serverName))
                {
                    if (!ServerAlreadyExists(serverName))
                    {
                        lstServerName.Items.Add(serverName);
                        cmbShellServerList.ResetText();
                        cmbShellServerList.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Please enter Machine Name.", "Linux Machine Name", MessageBoxButtons.OK);
                }
            }
            else
            {
                lblProcessing.Visible = true;
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 3000;
                timer.Tick += (source, ev) => { lblProcessing.Visible = false; timer.Stop(); };
                timer.Start();

                if (!string.IsNullOrEmpty(serverName))
                {
                    if (PingServer(serverName))
                    {
                        if (!ServerAlreadyExists(serverName))
                        {
                            if (!CheckRemoteSettings(serverName))
                            {
                                string message = "Credentials needs to be delegated to " + serverName + " machine for script execution. Please click on Yes to allow IAP to enable WSManCredSSP on this machine. You must have admin privileges on " + txtServerName.Text + " machine to run this command and Shell EXE must be running in admin mode.";
                                DialogResult result = MessageBox.Show(message, "Enable WSManCredSSP on Remote Machine", MessageBoxButtons.YesNo);
                                if (result == DialogResult.Yes)
                                {
                                    // Delegate credentials to remote machine
                                    string output = EnableRemoteSettings(serverName);
                                    if (!string.IsNullOrEmpty(output))
                                    {
                                        MessageBox.Show(output, "Error");
                                        return;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Few of the scripts that require delegating of credentials to remote machine might not work properly.", "Remote Settings");
                                    // return;
                                }
                            }
                            lstServerName.Items.Add(serverName);
                            cmbShellServerList.ResetText();
                            cmbShellServerList.Focus();
                            btnRunScript.Enabled = true;
                        }
                    }
                    else
                    {
                        txtServerName.Focus();
                        MessageBox.Show(txtServerName.Text + " machine is not available.", "Ping Machine", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter Machine Name.", "Remote Machine Name", MessageBoxButtons.OK);
                }
                lblProcessing.Visible = false;
            }
        }

        /// <summary>
        /// This method is used to remove server name from the listbox.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">EventArgs object containing event data</param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstServerName.SelectedIndex != -1)
                lstServerName.Items.RemoveAt(lstServerName.SelectedIndex);
            else
                MessageBox.Show("Please select machine to remove", "Remove Machine", MessageBoxButtons.OK);
        }

        private bool ValidateLinuxParams()
        {
            if (MainRepositoryView.showSSHForm)
            {
                // Perform validation for Linux servers
                if (string.IsNullOrEmpty(txtServerName.Text))
                {
                    MessageBox.Show("Please enter Linux server name");
                    txtServerName.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    MessageBox.Show("Please enter User Alias");
                    txtUserName.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Please enter Password");
                    txtPassword.Focus();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method is used to verify if the system is able to ping the server.
        /// </summary>
        /// <param name="serverName">name of server</param>
        /// <returns>True if able to ping the server</returns>
        private bool PingServer(string serverName)
        {
            //return true;
            bool pingable = false;
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(serverName);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return pingable;
            }
            return pingable;
        }

        public void EnableLinuxControls()
        {
            txtServerName.Text = "";
            txtServerName.Visible = true;
            grpBoxRemoteParameters.Visible = true;
            rdbRemote.Enabled = false;
            rdIAPNode.Enabled = false;
            rdSchedule.Enabled = false;
            lblServerName.Text = "Linux Server Name";
            lblUserNameValidation.Visible = true;
            lblPasswordValidation.Visible = true;
            pnlUserCredentials.Visible = true;
            txtServerName.Visible = true;
            txtServerName.Text = "";
            txtServerName.Focus();
            lblDisplayServerName.Visible = false;
            lblUserName.Visible = true;
            lblPassword.Visible = true;
            txtUserName.Text = "";
            txtPassword.Text = "";
            // lblServerName.Text = "Remote Machine Name";
            grpIAPNode.Visible = false;
            grpBoxRemoteParameters.Visible = true;
            rbKeyAuth.Visible = true;
            rbPassAuth.Visible = true;
            toolTip1.SetToolTip(txtServerName, "If more than one server separate the server name by comma");
            pnlRemoteControls.Visible = true;
            grpBoxRunScriptOptions.Height = grpBoxRunScriptOptions.Height + pnlRemoteControls.Height;
            btnAddToList.Visible = true;
            cmbShellServerList.Visible = true;
            if (parameters != null && parameters.Count > 0)
            {

                pnlUserCredentials.Visible = true;
                txtUserName.Visible = true;
                txtPassword.Visible = true;
                grpBoxRemoteParameters.Visible = true;

                grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                grpBoxRemoteParameters.Location = new Point(grpBoxScriptParams.Location.X, grpBoxScriptParams.Bottom + 10);
                //pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + grpBoxRemoteParameters.Height + 10);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRemoteParameters.Bottom + 10);
                this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + grpBoxRemoteParameters.Height + pnlButtons.Height + 80;
            }
            else
            {
                grpBoxRemoteParameters.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                // pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRunScriptOptions.Bottom + grpBoxRemoteParameters.Height + 10);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRemoteParameters.Bottom + 10);
                this.Height = grpBoxRunScriptOptions.Height + grpBoxRemoteParameters.Height + pnlButtons.Height + 70;
            }
                GetServersList();
        }

        /// <summary>
        /// This method is used to verify if the sever has already been added to listbox.
        /// </summary>
        /// <param name="serverName">name of server</param>
        /// <returns>true if server name already exists in listbox</returns>
        private bool ServerAlreadyExists(string serverName)
        {
            bool serverExists = false;
            if (lstServerName.Items.Count > 0)
            {
                foreach (string item in lstServerName.Items)
                {
                    if (item.ToLower().Equals(serverName.ToLower()))
                    {
                        serverExists = true;
                        MessageBox.Show("Machine " + serverName + " already added.", "Duplicate Machine", MessageBoxButtons.OK);
                        break;
                    }
                }
            }

            return serverExists;
        }

        /// <summary>
        /// This method reads parameter values, assigns the same to the corresponding object and returns the control
        /// to the parent form after running the script.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">EventArgs object containing event data</param>
        private void btnRunScript_Click(object sender, EventArgs e)
        {
            if (ValidateParameters())
            {
                List<string> scriptParams = new List<string>();

                int count = 0;
                foreach (Control txtBox in this.pnlTextBoxes.Controls)
                {
                    if (txtBox is TextBox)
                    {
                        //if (parameters[count].IsSecret)
                        //    scriptParams.Add(parameters[count].DefaultValue);
                        //else
                        scriptParams.Add(((TextBox)txtBox).Text);

                        count = count + 1;
                    }
                }
                if (MainRepositoryView.allowRemote)
                {
                    // Generate comma separated list of servers
                    if (rdbRemote.Checked)
                    {
                        MainRepositoryView.allowRemote = true;
                        MainRepositoryView.remoteServerName = GetAllServerNamesSeparatedByComma();
                        MainRepositoryView.UserName = txtUserName.Text;
                        MainRepositoryView.Password = ConvertToSecureString(txtPassword.Text);
                        AddServersToFile();
                    }
                    // if run locally set allowRemote attribute back to false
                    else
                        MainRepositoryView.allowRemote = false;
                }
                if (MainRepositoryView.showSSHForm)
                {
                    //MainRepositoryView.LinuxServerName = txtServerName.Text;                  
                    MainRepositoryView.LinuxServerName = GetAllServerNamesSeparatedByComma();
                    MainRepositoryView.UserName = txtUserName.Text;
                    if (rbPassAuth.Checked == true)
                    {
                        MainRepositoryView.Password = ConvertToSecureString(txtPassword.Text);
                    }
                    if (rbKeyAuth.Checked == true)
                    {
                        MainRepositoryView.LinuxKeyPath = txtPassword.Text;
                    }
                    MainRepositoryView.parametersCollection = scriptParams;
                    AddServersToFile();

                }

                MainRepositoryView.parametersCollection = scriptParams;
                ScriptDetails.parametersCollection = scriptParams;
                if (executionMode == ScriptExecutionMode.Local || executionMode == ScriptExecutionMode.Remote)
                    ParametersUpdated();
                else if (executionMode == ScriptExecutionMode.Schedule)
                {
                    if (ucScheduleRequest.RunOnCluster)
                    {
                        MainRepositoryView.RunOnCluster = true;
                        MainRepositoryView.ClusterValue = ucScheduleRequest.ClusterValue;
                        MainRepositoryView.ClusterName = ucScheduleRequest.ClusterName;
                    }

                    else if (ucScheduleRequest.RunOnNode)
                    {
                        MainRepositoryView.RunOnNode = true;
                        MainRepositoryView.SelectedNodes = ucScheduleRequest.SelectedNodes;
                    }
                    if (ucScheduleRequest.ScheduledForNow)
                        MainRepositoryView.ScheduledForNow = true;
                    else
                    {
                        MainRepositoryView.StartDate = ucScheduleRequest.StartDate.ToUniversalTime();
                        if (ucScheduleRequest.NoEndDate)
                            MainRepositoryView.NoEndDate = true;
                        else if (ucScheduleRequest.EndBy)
                        {
                            MainRepositoryView.EndBy = true;
                            MainRepositoryView.EndDate = ucScheduleRequest.EndDate;//.ToUniversalTime();
                        }
                        else
                        {
                            MainRepositoryView.Iterations = ucScheduleRequest.Iterations;
                        }
                    }

                    MainRepositoryView.Priority = ucScheduleRequest.Priority;
                    ParametersUpdated();
                }
                else if (executionMode == ScriptExecutionMode.IAPNode)
                {
                    MainRepositoryView.SelectedNodes = nodesUc.SelectedNodes;

                    //NodeExecuted(DisplayOutput);
                    //Infosys.ATR.AutomationClient.ucIAPNodes node = new Infosys.ATR.AutomationClient.ucIAPNodes();
                    nodesUc.IAPNodeExecuted += new Infosys.ATR.AutomationClient.ucIAPNodes.IAPNodeExecutedEventHandler(DisplayOutput);

                    //assign the parameters
                    if (scriptParams != null && scriptParams.Count > 0)
                    {
                        List<Infosys.ATR.AutomationClient.Parameter> iapScrParams = new List<AutomationClient.Parameter>();
                        for (int i = 0; i < scriptParams.Count; i++)
                        {
                            Infosys.ATR.AutomationClient.Parameter scrParam = new AutomationClient.Parameter() { ParameterName = parameters[i].Name, ParameterValue = (parameters[i].IsSecret) ? SecurePayload.Secure(scriptParams[i], "IAP2GO_SEC!URE") : scriptParams[i], IsSecret = parameters[i].IsSecret };
                            iapScrParams.Add(scrParam);
                        }
                        nodesUc.Parameters = iapScrParams;
                    }
                    //assign the category id and script id
                    nodesUc.ScriptId = ScriptId;
                    nodesUc.CategoryId = CategoryId;
                    nodesUc.UsesUI = UsesUI;

                    //open it new thread so that actions can be performed on parent form 
                    new System.Threading.Thread(() => nodesUc.Execute()).Start();
                    //if (NodeExecuted != null)
                    //{
                    //    NodeExecuted(executionResultView);
                    //}
                }
                //     }


                // DisplayOutput(schResultView);
                // this.DialogResult = System.Windows.Forms.DialogResult.OK;


                //  }
                Application.DoEvents();
                this.Dispose();
            }
        }

        private string GetAllServerNamesSeparatedByComma()
        {
            string strServerList = "";
            for (int i = 0; i < lstServerName.Items.Count; i++)
            {
                strServerList = strServerList + lstServerName.Items[i] + ",";
            }
            if (!string.IsNullOrEmpty(strServerList))
            {
                // Remove last comma
                if (strServerList.Contains(","))
                {
                    strServerList = strServerList.Substring(0, strServerList.Length - 1);
                }               
            }
            return strServerList;
        }

        public void DisplayOutput(List<ExecutionResultView> executionResultView)
        {
            if (NodeExecuted != null)
            {

                NodeExecuted(executionResultView);
            }

        }
        /// <summary>
        /// This method is used to convert plain string to secure string.
        /// </summary>
        /// <param name="password">string to convert</param>
        /// <returns>secured string</returns>
        private static SecureString ConvertToSecureString(string password)
        {
            SecureString securePassword = new SecureString();
            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }
            return securePassword;
        }

        /// <summary>
        /// This method is used to reset the form to default position.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">EventArgs object containing event data</param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            foreach (Control txtBox in this.pnlTextBoxes.Controls)
            {
                if (txtBox is TextBox)
                {
                    txtBox.Text = "";
                }
            }
            lstServerName.Items.Clear();
            ResetControls();
        }

        /// <summary>
        /// This method is used to close the script parameters form.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">EventArgs object containing event data</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is ScriptParameters)
                {
                    form.Close();
                }
            }
        }

        /// <summary>
        /// This method is used to hide remote controls and display remaining controls accoringly.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">EventArgs object containing event data</param>
        private void rdbLocal_CheckedChanged(object sender, EventArgs e)
        {
            HideRemoteControls();
            cmbShellServerList.Visible = false;
            if (MainRepositoryView.showSSHForm)
            {
                grpBoxRunScriptOptions.Height = pnlLocalControls.Height + pnlUserCredentials.Height + intGrpBoxRunScriptHeight;
                // grpBoxRemoteParameters.Height = 100;
            }
            else
            {
                grpBoxRunScriptOptions.Height = pnlLocalControls.Height + intGrpBoxRunScriptHeight;
            }
            if (parameters != null && parameters.Count > 0)
            {
                if (MainRepositoryView.showSSHForm)
                {
                    EnableLinuxControls();
                }
                else
                {
                    grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                    pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + 10);
                    this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + 100;
                }
            }
            else if (MainRepositoryView.showSSHForm)
            {
                EnableLinuxControls();
            }
            else
            {

                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                this.Height = grpBoxRunScriptOptions.Height + 90;

            }
            this.CenterToParent();

            if (rdbLocal.Checked)
                executionMode = ScriptExecutionMode.Local;
            if (IapNodeExecuted_EventHandler != null)
                IapNodeExecuted_EventHandler(false);
        }

        /// <summary>
        /// This method is used to display remote controls and display remaining controls accoringly.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">EventArgs object containing event data</param>
        private void rdbRemote_CheckedChanged(object sender, EventArgs e)
        {
            DisplayRemoteControls();
            grpBoxRunScriptOptions.Height = pnlLocalControls.Height + pnlRemoteControls.Height + intGrpBoxRunScriptHeight;
            // grpBoxRemoteParameters.Height = 100;// pnlRemoteControls.Height + pnlUserCredentials.Height;

            pnlRemoteControls.Visible = true;
            //grpBoxRunScriptOptions.Height = grpBoxRunScriptOptions.Height + pnlRemoteControls.Height;
            btnAddToList.Visible = true;
            cmbShellServerList.Visible = true;
            txtServerName.Visible = false;
            if (parameters != null && parameters.Count > 0)
            {
                grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                grpBoxRemoteParameters.Location = new Point(grpBoxScriptParams.Location.X, grpBoxScriptParams.Bottom + 10);
                //pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + grpBoxRemoteParameters.Height + 10);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRemoteParameters.Bottom + 10);
                this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + grpBoxRemoteParameters.Height + pnlButtons.Height + 80;
            }
            else
            {
                grpBoxRemoteParameters.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                // pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRunScriptOptions.Bottom + grpBoxRemoteParameters.Height + 10);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRemoteParameters.Bottom + 10);
                this.Height = grpBoxRunScriptOptions.Height + grpBoxRemoteParameters.Height + pnlButtons.Height + 70;
            }
            this.CenterToScreen();

            if (rdbRemote.Checked)
                executionMode = ScriptExecutionMode.Remote;
            if (IapNodeExecuted_EventHandler != null)
                IapNodeExecuted_EventHandler(false);

            GetServersList();
        }

        /// <summary>
        /// This method hides controls.
        /// </summary>
        private void HideRemoteControls()
        {
            btnAddToList.Visible = false;
            pnlRemoteControls.Visible = false;
            txtServerName.Visible = false;
            lblDisplayServerName.Visible = true;
            lblServerName.Text = "Local Machine Name";
            lblDisplayServerName.Text = System.Environment.MachineName;
            lblProcessing.Visible = false;
            grpIAPNode.Visible = false;
            pnlUserCredentials.Visible = false;
        }

        /// <summary>
        /// This method display controls.
        /// </summary>
        private void DisplayRemoteControls()
        {
            btnAddToList.Visible = true;
            pnlRemoteControls.Visible = true;
            pnlUserCredentials.Visible = true;
            txtServerName.Visible = true;
            txtServerName.Text = "";
            txtServerName.Focus();
            lblDisplayServerName.Visible = false;
            lblServerName.Text = "Remote Machine Name";
            grpIAPNode.Visible = false;
            grpBoxRemoteParameters.Visible = true;
        }

        /// <summary>
        /// This method is used to set form controls to its default position.
        /// </summary>
        private void ResetControls()
        {
            btnRunScript.Enabled = true;
            lblProcessing.Visible = false;
            txtUserName.Text = "";
            txtPassword.Text = "";
            grpBoxRemoteParameters.Visible = false;
            grpIAPNode.Visible = false;
            rbPassAuth.Visible = false;
            rbKeyAuth.Visible = false;
            pnlScheduledRequest.Visible = false;
            toolTip1.Hide(txtServerName);
            if (MainRepositoryView.networkParamsValidation)
            {
                lblUserNameValidation.Visible = true;
                lblPasswordValidation.Visible = true;
            }
            else
            {
                lblUserNameValidation.Visible = false;
                lblPasswordValidation.Visible = false;
            }

            if (MainRepositoryView.allowRemote)
            {
                grpBoxRunScriptOptions.Visible = true;
                pnlRemoteControls.Visible = false;
                btnAddToList.Visible = false;
                txtServerName.Text = System.Environment.MachineName;
                rdbLocal.Checked = true;
                txtServerName.Visible = false;
                lblDisplayServerName.Text = System.Environment.MachineName;

                grpBoxRunScriptOptions.Height = pnlLocalControls.Height + intGrpBoxRunScriptHeight;
                if (parameters != null && parameters.Count > 0)
                {
                    grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                    pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + 10);
                    this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + 100;
                }
                else
                {
                    pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                    this.Height = grpBoxRunScriptOptions.Height + 90;
                }
            }
            else
            {
                grpBoxRunScriptOptions.Visible = false;
                grpBoxScriptParams.Location = new Point(10, 10);
                grpBoxScriptParams.Height = pnlTextBoxes.Height + 20;
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + 10);
                if (parameters != null && parameters.Count > 0)
                    this.Height = pnlLocalControls.Height + grpBoxScriptParams.Height + 60;
                else
                    this.Height = grpBoxRunScriptOptions.Height;
            }
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.CenterToParent();
        }

        /// <summary>
        /// This method is used to check if machine is configured to delegate credentials to remote machine.
        /// </summary>
        /// <param name="remoteMachineName">Name of remote machine</param>
        /// <returns>true if machine is configured to delegate credentials to remote machine</returns>
        private bool CheckRemoteSettings(string remoteMachineName)
        {
            Collection<PSObject> psOutput = null;
            StringBuilder result = new StringBuilder();
            bool enabled = false;
            using (Runspace runSpace = RunspaceFactory.CreateRunspace())
            {
                runSpace.Open();
                using (Pipeline pipeline = runSpace.CreatePipeline())
                {
                    PowerShell ps = PowerShell.Create();
                    ps.Runspace = runSpace;
                    ps.AddCommand("get-wsmancredssp");
                    psOutput = ps.Invoke();

                    if (ps.Streams.Error.Count == 0)
                    {
                        foreach (PSObject psObject in psOutput)
                        {
                            if (psObject != null)
                            {
                                result.AppendLine(psObject.BaseObject.ToString());
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed");
                        foreach (var errorRecord in ps.Streams.Error)
                        {
                            result.AppendLine(errorRecord.ToString());
                        }
                    }
                    if (result.ToString().Contains(remoteMachineName))
                        enabled = true;
                }
            }
            return enabled;
        }

        /// <summary>
        /// This method is used to delegate credentials to remote machine.
        /// </summary>
        /// <param name="remoteMachineName">Name of remote machine</param>
        /// <returns>Blank in case no error occurred</returns>
        private string EnableRemoteSettings(string remoteMachineName)
        {
            string enabled = "";
            Collection<PSObject> psOutput = null;
            StringBuilder result = new StringBuilder();
            using (Runspace runSpace = RunspaceFactory.CreateRunspace())
            {
                runSpace.Open();
                using (Pipeline pipeline = runSpace.CreatePipeline())
                {
                    PowerShell ps = PowerShell.Create();
                    ps.Runspace = runSpace;
                    ps.AddCommand("enable-wsmancredssp");
                    ps.AddParameter("role", "client");
                    if (LocalServer(remoteMachineName))
                        ps.AddParameter("DelegateComputer", "*");
                    else
                        ps.AddParameter("DelegateComputer", remoteMachineName);
                    ps.AddParameter("Force");
                    psOutput = ps.Invoke();

                    if (ps.Streams.Error.Count > 0)
                    {
                        foreach (var errorRecord in ps.Streams.Error)
                        {
                            result.AppendLine(errorRecord.ToString());
                        }

                        enabled = result.ToString();
                    }

                }
            }
            return enabled;
        }

        private static bool LocalServer(string serverName)
        {
            bool result = false;

            if (serverName.ToLower().Equals("localhost") || serverName.ToLower().Equals(Environment.MachineName.ToLower()) || serverName.Equals(".") || serverName.ToLower().Equals("127.0.0.1"))
                result = true;

            return result;
        }
        private void rdIAPNode_CheckedChanged(object sender, EventArgs e)
        {
            grpBoxRunScriptOptions.Height = 40;
            grpBoxRemoteParameters.Visible = false;
            grpIAPNode.Visible = true;

            //show the iap node user control
            //Infosys.ATR.AutomationClient.ucIAPNodes nodesUc = new AutomationClient.ucIAPNodes(AutomationClient.ExecutionType.Script);
            nodesUc.Dock = DockStyle.Bottom;
            nodesUc.HideExecute = true; //hiding execute button as, the execution will be triggered from the main window- run script button
            if (rdIAPNode.Checked && !grpIAPNode.Controls.Contains(nodesUc))
                grpIAPNode.Controls.Add(nodesUc);
            else if (!rdIAPNode.Checked && grpIAPNode.Controls.Contains(nodesUc))
                grpIAPNode.Controls.Remove(nodesUc);

            grpIAPNode.Height = nodesUc.Height + 20;

            if (parameters != null && parameters.Count > 0)
            {
                grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                grpIAPNode.Location = new Point(grpBoxScriptParams.Location.X, grpBoxScriptParams.Bottom + 10);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpIAPNode.Bottom + 10);
                this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + grpIAPNode.Height + pnlButtons.Height + 90;
            }
            else
            {
                grpIAPNode.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + 10);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpIAPNode.Bottom + 10);
                this.Height = grpBoxRunScriptOptions.Height + grpIAPNode.Height + pnlButtons.Height + 80;
            }

            if (rdIAPNode.Checked)
                executionMode = ScriptExecutionMode.IAPNode;

            this.CenterToScreen();
            if (IapNodeExecuted_EventHandler != null)
                IapNodeExecuted_EventHandler(rdIAPNode.Checked);
        }

        public int CategoryId { get; set; }

        public int ScriptId { get; set; }

        public bool UsesUI { get; set; }

        public string ScriptName { get; set; }

        private void rdSchedule_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSchedule.Checked == true)
            {
                executionMode = ScriptExecutionMode.Schedule;
                ucScheduleRequest.CategoryId = CategoryId;
                ucScheduleRequest.NodeType = 2;
                MainRepositoryView.ScheduledRequest = true;
                ResetScheduleControls();
            }
            else
            {
                btnRunScript.Text = "Run Script";
                pnlScheduledRequest.Visible = false;
                pnlLocalControls.Visible = true;
            }

            this.CenterToScreen();
            if (IapNodeExecuted_EventHandler != null)
                IapNodeExecuted_EventHandler(false);
        }

        private void ResetScheduleControls()
        {
            pnlLocalControls.Visible = false;
            pnlRemoteControls.Visible = false;
            grpIAPNode.Visible = false;
            grpBoxRemoteParameters.Visible = false;
            grpBoxRunScriptOptions.Height = 50;
            btnRunScript.Text = "Schedule Script";
            btnRunScript.Width = 90;
            pnlScheduledRequest.Visible = true;
            //pnlScheduledRequest.AutoScroll = true;
            //pnlScheduledRequest.HorizontalScroll.Visible = false;
            int height = 0;
            if (parameters != null && parameters.Count > 0)
            {
                grpBoxScriptParams.Location = new Point(grpBoxScriptParams.Location.X, grpBoxRunScriptOptions.Bottom);
                pnlScheduledRequest.Location = new Point(pnlScheduledRequest.Location.X, grpBoxScriptParams.Bottom);
                height = grpBoxScriptParams.Height;
            }
            else
            {
                pnlScheduledRequest.Location = new Point(pnlScheduledRequest.Location.X, grpBoxRunScriptOptions.Bottom);
            }

            if (pnlScheduledRequest.Controls.Contains(ucScheduleRequest))
            {
                pnlScheduledRequest.Controls.Remove(ucScheduleRequest);
            }
            pnlScheduledRequest.Controls.Add(ucScheduleRequest);
            pnlScheduledRequest.Height = ucScheduleRequest.Height;

            pnlButtons.Location = new Point(pnlButtons.Location.X, pnlScheduledRequest.Bottom + 10);
            if (parameters != null && parameters.Count > 0)
                this.Height = grpBoxRunScriptOptions.Height + height + pnlScheduledRequest.Height + pnlButtons.Height + 80;
            else
                this.Height = grpBoxRunScriptOptions.Height + pnlScheduledRequest.Height + pnlButtons.Height + 80;
            this.CenterToParent();
        }

        private void rbKeyAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (rbKeyAuth.Checked == true)
            {
                txtPassword.UseSystemPasswordChar = false;
                lblPassword.Text = "Enter Key Path";
                string LinuxKey = System.Configuration.ConfigurationManager.AppSettings["LinuxKeyPath"];
                txtPassword.Text = LinuxKey;

            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                lblPassword.Text = "Password";
                txtPassword.Text = "";
            }
        }

        /// <summary>
        /// Populates combobox with list of Linux servers
        /// </summary>
        private void GetServersList()
        {
            var list = new List<string>();
            cmbShellServerList.Items.Clear();
            if (File.Exists(remoteServerFilePath))
            {
                using (var fileStream = new FileStream(remoteServerFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        string serverName;
                        while ((serverName = streamReader.ReadLine()) != null)
                        {
                            cmbShellServerList.Items.Add(serverName);
                        }
                    }
                }
            }
            txtServerName.Visible = false;
        }

        /// <summary>
        /// Adds list of Linux servers to LinuxServer.txt file
        /// </summary>
        private void AddServersToFile()
        {
            foreach (string serverName in lstServerName.Items)
            {
                if (!cmbShellServerList.Items.Contains(serverName))
                    File.AppendAllText(remoteServerFilePath, serverName + Environment.NewLine);
            }
        }

        /// <summary>
        /// Gets the path of the application
        /// </summary>
        /// <returns></returns>
        private static string GetAppPath()
        {
            string path;
            path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            if (path.Contains(@"file:\\"))
            {
                path = path.Replace(@"file:\\", "");
            }

            else if (path.Contains(@"file:\"))
            {
                path = path.Replace(@"file:\", "");
            }

            return path;
        }

        private void bntDeleteServerList_Click(object sender, EventArgs e)
        {
            if (File.Exists(remoteServerFilePath))
            {
                File.Delete(remoteServerFilePath);
                cmbShellServerList.Items.Clear();
                lstServerName.Items.Clear();
                cmbShellServerList.ResetText();
                cmbShellServerList.Focus();
            }
        }
    }


    public enum ScriptExecutionMode
    {
        Local,
        Remote,
        IAPNode,
        Schedule
    }
}
