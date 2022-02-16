using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infosys.ATR.AutomationClient;
using Infosys.ATR.CommonViews;
using System.IO;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;
using Infosys.WEM.WorkflowExecutionLibrary;
using Infosys.ATR.WFDesigner.Services;
using Infosys.WEM.Service.Common.Contracts.Message;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Client;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Globalization;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Runtime.Serialization.Json;
using Infosys.WEM.SecureHandler;

namespace Infosys.ATR.WFDesigner.Views
{
    public partial class ExecuteWf : Form
    {
        ucIAPNodes iapPane = new ucIAPNodes(ExecutionType.Workflow);
        //   ucIAPNodes iapPaneSchedule = new ucIAPNodes(ExecutionType.Workflow, "Schedule");
        WorkflowExecutionMode executionMode;
        List<Entities.WorkflowParameterPE> _parameters;
        public delegate void NodeExecutedEventHandler(List<ExecutionResultView> e);
        public event NodeExecutedEventHandler NodeExecuted;

        public delegate void WFExecutionProgressEventHandler(Infosys.ATR.WFDesigner.Views.ExecuteWf.AppendOutputViewArgsWF e);
        public event WFExecutionProgressEventHandler ExecutionResultView;

        public delegate Dictionary<object, System.Activities.Debugger.SourceLocation> WFSourceLocationMappingEventHandler();
        public event WFSourceLocationMappingEventHandler SourceLocationMapping;

        public delegate void ShowDebugEventHandler(System.Activities.Debugger.SourceLocation srcLoc);
        public event ShowDebugEventHandler ShowDebugEvent;

        public delegate void RemoveDebugEventHandler();
        public event RemoveDebugEventHandler RemoveDebugEvent;        

        private static Dictionary<string, string> outputwfMap = new Dictionary<string, string>();

        private List<TextBox> txtBoxes;
        private int parameterCount = 0;
        private int intParamsScroll = 5;
        // for storing count of IN parameters
        int inParamCount = 0;
        private List<string> lstToolTip = null;
        //bool paramExists = false;
        bool recurrancePanel = false;
        bool nodePanel = false;
        bool runOnCluster = false;
        private const int margin = 10;
        GetAllClustersByCategoryResMsg response = null;
        Infosys.ATR.ScheduleRequestUI.ScheduleRequestUI ucScheduleRequest = new Infosys.ATR.ScheduleRequestUI.ScheduleRequestUI(1);
        static bool _IsResumed = false;
        public static bool IsResumed
        {
            get
            {
                return _IsResumed;
            }
            set
            {
                _IsResumed = value;

            }
        }


        //[EventPublication(Constants.EventTopicNames.AppendOutputViewWF, PublicationScope.Global)]
        //public event EventHandler<AppendOutputViewArgsWF> wfAppendOutputView;
        public class AppendOutputViewArgsWF : EventArgs
        {
            public string scriptID;
            public Guid Identifier;
            public int progress;
            public ExecutionResultView executionResultView;
            public bool Script;
        }

        public bool IsChanged { get; set; }

        public ExecuteWf(List<Entities.WorkflowParameterPE> parameters, string wfText)
        {
            InitializeComponent();
            chkTraceWFDetails.Checked = false;
            // pnlScheduledRequest.Width = grpBoxRunScriptOptions.Width;
            // defaultHeight = this.Height = 150;         
            rdLocally.Checked = true;
            if (!_IsResumed)
            {
                if (parameters != null && parameters.Count > 0)
                {
                    if (parameters.Exists(ent => ent.ParamIOType.Equals(Entities.ParameterIOTypes.In) || ent.ParamIOType.Equals(Entities.ParameterIOTypes.InAndOut)))
                    {
                        // paramExists = true;
                        grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + margin);
                        pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + margin);
                        this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + 100;
                    }
                    _parameters = parameters.Where(p => p.ParamIOType == Entities.ParameterIOTypes.In || p.ParamIOType == Entities.ParameterIOTypes.InAndOut || p.ParamIOType == Entities.ParameterIOTypes.Out).ToList();
                }
                if (string.IsNullOrEmpty(wfText))
                {
                    GenerateParameterTextBoxes();
                }
                else
                    DisplayUnPublishedArguments(wfText);
                // DrawPropertyPanel();
            }
            else
                grpBoxScriptParams.Visible = false;
        }

        /// <summary>
        /// This method is used to idetify in,in/out parameters for offline and unpublished mode
        /// </summary>
        /// <param name="wfText"></param>
        private void DisplayUnPublishedArguments(string wfText)
        {
            TextReader txtReader = new StringReader(wfText);
            var dynamicActivity = ActivityXamlServices.Load(txtReader) as DynamicActivity;
            if (dynamicActivity == null)
                throw new InvalidDataException("Invalid DynamicActivity.");

            Entities.WorkflowParameterPE param = null;
            _parameters = new List<Entities.WorkflowParameterPE>();
            lstToolTip = new List<string>();
            foreach (var item in dynamicActivity.Properties)
            {
                if (item.Type.Name.Contains("InArgument") || item.Type.Name.Contains("InOutArgument"))
                {
                    param = new Entities.WorkflowParameterPE();
                    // Get default value
                    if (item.Value != null)
                    {
                        var valueProp = item.Value.GetType().GetProperty("Expression", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
                        var expression = valueProp.GetValue(item.Value, null);
                        var expressionValueProp = expression.GetType().GetProperty("Value");
                        if (expressionValueProp != null)
                            param.DefaultValue = expressionValueProp.GetValue(expression, null).ToString();
                    }
                    param.Name = item.Name;
                    param.ParamIOType = Entities.ParameterIOTypes.In;
                    _parameters.Add(param);

                    if (item.Type.GenericTypeArguments[0].Name.Equals("String"))
                        lstToolTip.Add("");
                    else
                        lstToolTip.Add("Provide data for " + item.Type.GenericTypeArguments[0].Name + " in json format e.g.{\"Property1\":\"Value1\",\"Property2\":\"Value2\"}");
                }
            }

            GenerateParameterTextBoxes();

        }
        /// <summary>
        ///  This method generates textboxes for each parameter and sets position of the same.
        /// </summary>
        private void GenerateParameterTextBoxes()
        {
            btnReset.Visible = false;
            if (_parameters != null && _parameters.Count > 0 && _parameters.Exists(ent => ent.ParamIOType.Equals(Entities.ParameterIOTypes.In) || ent.ParamIOType.Equals(Entities.ParameterIOTypes.InAndOut)))
            {
                txtBoxes = new List<TextBox>();
                parameterCount = _parameters.Count();

                //Generate labels and text boxes
                for (int count = 0; count < parameterCount; count++)
                {
                    if (_parameters[count].ParamIOType == Entities.ParameterIOTypes.In || _parameters[count].ParamIOType == Entities.ParameterIOTypes.InAndOut)
                    {
                        inParamCount = inParamCount + 1;
                        //Create a new label and text box
                        Label lblInput = new Label();
                        Label lblMandatory = new Label();
                        TextBox txtParameter = new TextBox();

                        lblInput.Text = _parameters[count].Name;
                        if (parameterCount == 1)
                            lblInput.Location = new Point(0, 10);
                        else
                            lblInput.Location = new Point(0, 10 + (count * 30));
                        lblInput.AutoSize = true;
                        if (lstToolTip != null && !string.IsNullOrEmpty(lstToolTip[count].ToString()))
                        {
                            ToolTip lblTip = new ToolTip();
                            lblTip.SetToolTip(lblInput, lstToolTip[count].ToString());
                        }

                        lblMandatory.Location = new Point(lblInput.Right + 5, lblInput.Location.Y + 5);
                        if (_parameters[count].IsMandatory)
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
                            txtParameter.Width = 340;
                        else
                            txtParameter.Width = 365;
                        txtParameter.Name = "TextBox" + count;
                        if (_parameters[count].IsSecret)
                        {
                            txtParameter.Text = _parameters[count].DefaultValue;//"*******";
                            txtParameter.UseSystemPasswordChar = true;
                        }
                        else
                        {
                            txtParameter.Text = _parameters[count].DefaultValue;
                        }
                        txtBoxes.Add(txtParameter);

                        pnlTextBoxes.Controls.Add(lblInput);
                        pnlTextBoxes.Controls.Add(lblMandatory);
                        pnlTextBoxes.Controls.Add(txtParameter);
                    }
                }
               
                // Set panel scrollbar if number of parameters is greater than 5
                if (inParamCount > intParamsScroll)
                {
                    pnlTextBoxes.AutoScroll = true;
                    pnlTextBoxes.HorizontalScroll.Visible = false;
                    pnlTextBoxes.Height = intParamsScroll * 30;
                    btnReset.Visible = true;
                    btnReset.Location = new Point(btnReset.Location.X - margin, pnlTextBoxes.Height + 30);
                    grpBoxScriptParams.Height = pnlTextBoxes.Height + btnReset.Height + 40;
                    pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + margin);
                    this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + 100;
                    this.CenterToParent();
                }
                else if (inParamCount > 0)
                {
                    pnlTextBoxes.Height = (inParamCount * 32) + btnReset.Height + margin;
                    btnReset.Visible = true;
                    btnReset.Location = new Point(btnReset.Location.X + margin, pnlTextBoxes.Height);
                    grpBoxScriptParams.Height = pnlTextBoxes.Height + btnReset.Height + 15;

                    grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + margin);
                    pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + margin);
                    this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + 100;
                    this.CenterToParent();
                }
            }
            else
            {
                grpBoxScriptParams.Visible = false;
            }
        }
        /// <summary>
        /// This method is used to set form controls to its default position.
        /// </summary>
        private void ResetControls()
        {
            if (rdSchedule.Checked == true)
            {
                ResetScheduleControls();
            }
            else if (_parameters != null && _parameters.Count > 0)
            {
                if (inParamCount > intParamsScroll)
                    pnlTextBoxes.Height = intParamsScroll * 30;
                else
                    pnlTextBoxes.Height = inParamCount * 30;

                btnReset.Location = new Point(btnReset.Location.X, pnlTextBoxes.Height + 30);
                grpBoxScriptParams.Height = pnlTextBoxes.Height + btnReset.Height + 40;

                grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + margin);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxScriptParams.Bottom + margin);
                this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + 100;
            }
            else
            {
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpBoxRunScriptOptions.Bottom + margin);
                this.Height = grpBoxRunScriptOptions.Height + 90;
            }
        }        

        private void ResetScheduleControls()
        {
            executionMode = rdSchedule.Checked ? WorkflowExecutionMode.Schedule : WorkflowExecutionMode.Locally;
            pnlScheduledRequest.Visible = true;

            //rbNow.Checked = true;
            //rbRunOnNode.Checked = true;

            //cmbSemantic.Visible = true;
            //lblSelectCluster.Visible = true;
            //grpIAPNode.Visible = false;
            //grpSchduleIAPNode.Visible = false;
            recurrancePanel = false;
            runOnCluster = false;
            btnRun.Text = "Schedule Workflow";
            int incrementHeight = 0;
            //pnlSchedule.AutoScroll = false;

            if (pnlScheduledRequest.Controls.Contains(ucScheduleRequest))
            {
                pnlScheduledRequest.Controls.Remove(ucScheduleRequest);
            }
            pnlScheduledRequest.Controls.Add(ucScheduleRequest);
            pnlScheduledRequest.Height = ucScheduleRequest.Height + margin;
            if (inParamCount > 3)
            {
                pnlTextBoxes.AutoScroll = true;
                pnlTextBoxes.HorizontalScroll.Visible = false;
                pnlTextBoxes.Height = 3 * 30;
            }
            else
                pnlTextBoxes.Height = inParamCount * 30;

            btnReset.Location = new Point(btnReset.Location.X, pnlTextBoxes.Height + 30);
            grpBoxScriptParams.Height = pnlTextBoxes.Height + btnReset.Height + 40;

            if (_parameters != null && _parameters.Count > 0)
            {
                incrementHeight = grpBoxScriptParams.Height;
                pnlScheduledRequest.Location = new Point(pnlScheduledRequest.Location.X, grpBoxScriptParams.Bottom + margin);
            }
            else
                pnlScheduledRequest.Location = new Point(pnlScheduledRequest.Location.X, grpBoxRunScriptOptions.Bottom + margin);

            // pnlSchedule.Height = panel1.Height + pnlScheduleProperties.Height + margin;
            // pnlScheduledRequest.Height = pnlSchedule.Height + 20;
            this.Height = grpBoxRunScriptOptions.Height + incrementHeight + pnlScheduledRequest.Height + pnlButtons.Height + 80;

            //pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, panel1.Height + margin);
            pnlButtons.Location = new Point(pnlButtons.Location.X, pnlScheduledRequest.Bottom + margin);
        }
        private void rdLocally_CheckedChanged(object sender, EventArgs e)
        {
            executionMode = rdLocally.Checked ? WorkflowExecutionMode.Locally : WorkflowExecutionMode.IAPNodes;
            grpIAPNode.Visible = false;
            pnlScheduledRequest.Visible = false;
            ResetControls();
            this.CenterToParent();
        }

        private void rdIapNodes_CheckedChanged(object sender, EventArgs e)
        {
            executionMode = rdIapNodes.Checked ? WorkflowExecutionMode.IAPNodes : WorkflowExecutionMode.Locally;
            grpIAPNode.Visible = true;
            iapPane.Dock = DockStyle.Bottom;
            iapPane.HideExecute = true;
            if (rdIapNodes.Checked && !grpIAPNode.Controls.Contains(iapPane))
                grpIAPNode.Controls.Add(iapPane);
            grpIAPNode.Height = iapPane.Height + 20;

            if (_parameters != null && _parameters.Count > 0)
            {
                if (inParamCount > intParamsScroll)
                    pnlTextBoxes.Height = intParamsScroll * 30;
                else
                    pnlTextBoxes.Height = inParamCount * 30;

                btnReset.Location = new Point(btnReset.Location.X, pnlTextBoxes.Height + 30);
                grpBoxScriptParams.Height = pnlTextBoxes.Height + btnReset.Height + 40;

                grpBoxScriptParams.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + margin);
                grpIAPNode.Location = new Point(grpBoxScriptParams.Location.X, grpBoxScriptParams.Bottom + margin);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpIAPNode.Bottom + margin);
                this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + grpIAPNode.Height + pnlButtons.Height + 90;
            }
            else
            {
                grpIAPNode.Location = new Point(grpBoxRunScriptOptions.Location.X, grpBoxRunScriptOptions.Bottom + margin);
                pnlButtons.Location = new Point(pnlButtons.Location.X, grpIAPNode.Bottom + margin);
                this.Height = grpBoxRunScriptOptions.Height + grpIAPNode.Height + pnlButtons.Height + 80;
            }

            this.CenterToScreen();
        }

        public void DisplayOutput(List<ExecutionResultView> executionResultView)
        {

            executionResultView.ForEach(e =>
            {
                if (outputwfMap.ContainsKey(e.ServerName))
                {
                    e.Identifier = new Guid(outputwfMap[e.ServerName]);
                    e.Progress = 100;
                }
                else if (e.Identifier == Guid.Empty)
                    e.Identifier = Guid.NewGuid();
            });



            if (NodeExecuted != null)
            {
                foreach (string s in iapPane.SelectedNodes)
                {
                    if (outputwfMap.ContainsKey(s))
                    {
                        Guid Identifier = new Guid(outputwfMap[s]);
                        SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Workflow Execution Completed....." + Environment.NewLine, s, 99);
                    }
                }
                NodeExecuted(executionResultView);
            }

        }

        // <summary>
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
                    if (_parameters[count].IsMandatory && (String.IsNullOrEmpty(((TextBox)txtBox).Text)))
                    {
                        MessageBox.Show("Please enter value for parameter " + _parameters[count].Name);
                        txtBox.Focus();
                        return false;
                    }
                    count = count + 1;
                }
            }

            if (rdSchedule.Checked && ucScheduleRequest.RunOnCluster)
            {
                if (string.IsNullOrEmpty(ucScheduleRequest.ClusterValue))
                {
                    MessageBox.Show("please select cluster");
                    return false;
                }
            }

            return true;
        }
        public string WorkflowText { get; set; }


        public int CategoryId { get; set; }

        public string WorkFlowId { get; set; }

        public string WorkFlowName { get; set; }

        public int WorkflowVersion { get; set; }

        public bool UsesUI { get; set; }

        public bool DisableIAPNodeExecution
        {
            set
            {
                rdIapNodes.Enabled = !value;
            }
        }

        public bool DisableIAPNodeSchedule
        {
            set
            {
                rdSchedule.Enabled = !value;
            }
        }

        public string TransactionInstanceId { get; set; }
        public string WorkflowPersistedStateId { get; set; }
        public string WorkflowActivityBookmark { get; set; }    

        private void btnRun_Click_1(object sender, EventArgs e)
        {
            WorkflowExecutionManager wfMgr = new WorkflowExecutionManager();
            bool offline;

            if (ValidateParameters())
            {
                this.WindowState = FormWindowState.Minimized;
                List<Infosys.ATR.AutomationClient.Parameter> iapScrParams = new List<AutomationClient.Parameter>();
                //IDictionary<string, object> wfInParams = new Dictionary<string, object>();
                List<Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter> paramsInputValues = new List<Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter>();
                int count = 0;

                switch (executionMode)
                {
                    case WorkflowExecutionMode.IAPNodes:

                        IDictionary<string, object> wfInParams = new Dictionary<string, object>();
                        outputwfMap.Clear();
                        foreach (string s in iapPane.SelectedNodes)
                        {
                            Guid Identifier = Guid.NewGuid();

                            outputwfMap.Add(s, Identifier.ToString());
                            SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Workflow Execution Initiated....." + Environment.NewLine, s, 20);
                        }
                        //gather the parameters               
                        foreach (Control param in pnlTextBoxes.Controls)
                        {
                            if (param is TextBox)
                            {
                                iapScrParams.Add(new Infosys.ATR.AutomationClient.Parameter() { ParameterName = _parameters[count].Name, ParameterValue = (_parameters[count].IsSecret) ? SecurePayload.Secure(param.Text, "IAP2GO_SEC!URE") : param.Text, IsSecret = _parameters[count].IsSecret });
                                wfInParams.Add(_parameters[count].Name, param.Text);
                                count = count + 1;
                            }
                        }

                        if (iapPane != null)
                        {
                            iapPane.IAPNodeExecuted += new ucIAPNodes.IAPNodeExecutedEventHandler(DisplayOutput);
                            iapPane.Parameters = iapScrParams;
                            iapPane.WorkFlowId = WorkFlowId;
                            iapPane.WorkflowVersion = WorkflowVersion;
                            iapPane.CategoryId = CategoryId;
                            iapPane.UsesUI = UsesUI;

                            //open it new thread so that actions can be performed on parent form 
                            new System.Threading.Thread(() => iapPane.Execute()).Start();
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        }

                        break;
                    case WorkflowExecutionMode.Locally:

                        //gather the parameters               
                        foreach (Control param in pnlTextBoxes.Controls)
                        {
                            if (param is TextBox)
                            {
                                paramsInputValues.Add(new Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter() { ParameterName = _parameters[count].Name, ParameterValue = (_parameters[count].IsSecret) ? SecurePayload.Secure(param.Text, "IAP2GO_SEC!URE") : param.Text });
                                count = count + 1;
                            }
                        }

                        WorkflowIndentifier wfToRun = new WorkflowIndentifier();
                        TransactionMetadata transMeta = new TransactionMetadata();
                        transMeta.executionMode = executionMode.ToString();
                        transMeta.Parameters = paramsInputValues;

                        if (!string.IsNullOrEmpty(TransactionInstanceId))
                            wfToRun.TransactionInstanceId = TransactionInstanceId;
                        if (!string.IsNullOrEmpty(WorkflowPersistedStateId))
                            wfToRun.LastWorkflowStateId = WorkflowPersistedStateId;
                         if (!string.IsNullOrEmpty(WorkflowActivityBookmark))
                             wfToRun.BookMark = WorkflowActivityBookmark;

                        ExecutionResult result;
                        wfToRun.Identifier = Guid.NewGuid();

                        // Unpublished/Offline mode/changed
                        if (string.IsNullOrEmpty(WorkFlowId) || IsChanged)
                        {
                            offline = true;
                            //result = wfMgr.Execute(WorkflowText, paramsInputValues);
                            wfMgr.SendExecutionStatus += wfExecutionManager_SendExecutionStatus;
                            wfMgr.WorkflowExecutionCompleted += wfMgr_WorkflowExecutionCompleted;
                            wfMgr.UpdateSourceLocationMapping += wfMgr_UpdateSourceLocationMapping;
                            wfMgr.ShowDebug += wfMgr_ShowDebug;
                            wfMgr.RemoveDebugAdornment += wfMgr_RemoveDebugAdornment;
                            wfMgr.EnableWfTraceDetails = chkTraceWFDetails.Checked;
                            wfMgr.ExecuteAsync(WorkflowText, paramsInputValues);
                        }
                        // Online mode
                        else
                        {
                            wfToRun.WorkflowId = new Guid(WorkFlowId);
                            wfToRun.CategoryId = CategoryId;
                            wfToRun.Parameters = paramsInputValues;
                            wfToRun.WorkflowVersion = WorkflowVersion;
                            wfToRun.Identifier = Guid.NewGuid();
                            wfToRun.TransactionMetadata = wfMgr.JsonSerialize(transMeta);
                            wfMgr.EnableWfTraceDetails = chkTraceWFDetails.Checked;
                            wfMgr.SendExecutionStatus += wfExecutionManager_SendExecutionStatus;
                            wfMgr.WorkflowExecutionCompleted += wfMgr_WorkflowExecutionCompleted;
                            // WorkflowExecutionManager.SendExecutionStatus -= new WorkflowExecutionManager.SendExecutionStatusEventHandler(wfExecutionManager_SendExecutionStatus);
                            // WorkflowExecutionManager.SendExecutionStatus += new WorkflowExecutionManager.SendExecutionStatusEventHandler(wfExecutionManager_SendExecutionStatus);

                            wfMgr.UpdateSourceLocationMapping += wfMgr_UpdateSourceLocationMapping;
                            wfMgr.ShowDebug += wfMgr_ShowDebug;
                            wfMgr.RemoveDebugAdornment += wfMgr_RemoveDebugAdornment;
                            offline = true;
                            //result = wfMgr.Execute(wfToRun);
                            wfMgr.ExecuteAsync(wfToRun);
                        }

                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        break;
                    case WorkflowExecutionMode.Schedule:
                        AddScheduledRequestResMsg scheduledReqIds = null;
                        List<ExecutionResultView> schResultView = new List<ExecutionResultView>();
                        ExecutionResultView schView = null;
                        int nodeCount = 0;
                        string serverName = "";
                        List<string> selectedNodes = new List<string>();
                        List<string> selectedNodeNames = new List<string>();

                        //gather the parameters
                        Dictionary<string, string> inParams = new Dictionary<string, string>();
                        List<Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter> workflowInParams = new List<Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter>();

                        foreach (Control param in pnlTextBoxes.Controls)
                        {
                            if (param is TextBox)
                            {
                                Infosys.WEM.WorkflowExecutionLibrary.Entity.Parameter paraminput = new WEM.WorkflowExecutionLibrary.Entity.Parameter();
                                paraminput.ParameterName = _parameters[count].Name;
                                paraminput.ParameterValue = (_parameters[count].IsSecret) ? SecurePayload.Secure(param.Text, "IAP2GO_SEC!URE") : param.Text;

                                workflowInParams.Add(paraminput);

                                inParams.Add(_parameters[count].Name, param.Text);
                                count = count + 1;
                            }
                        }
                        outputwfMap.Clear();


                        if (ucScheduleRequest.RunOnCluster)
                        {
                            selectedNodes.Add(ucScheduleRequest.ClusterValue);
                            selectedNodeNames.Add(ucScheduleRequest.ClusterName);
                            // selectedNodes.Add(cmbSemantic.SelectedValue.ToString());
                        }
                        else if (ucScheduleRequest.RunOnNode)
                        {
                            selectedNodes = ucScheduleRequest.SelectedNodes;
                            selectedNodeNames = ucScheduleRequest.SelectedNodes;
                        }
                        foreach (string s in selectedNodeNames)
                        {
                            Guid Identifier = Guid.NewGuid();

                            outputwfMap.Add(s, Identifier.ToString());
                            SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Scheduling Initiated....." + Environment.NewLine, s, 20);
                        }

                        foreach (string node in selectedNodes)
                        {
                            AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
                            req.Request = new Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest();
                            req.Request.CategoryId = CategoryId;
                            req.Request.Requestor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            if (ucScheduleRequest.ScheduledForNow)
                                req.Request.ExecuteOn = System.DateTime.UtcNow;
                            else
                            {
                                req.Request.ExecuteOn = ucScheduleRequest.StartDate.ToUniversalTime();
                                if (ucScheduleRequest.NoEndDate)
                                    req.Request.Iterations = -1;
                                else if (ucScheduleRequest.EndBy)
                                {
                                    req.Request.EndDate = ucScheduleRequest.EndDate;//.ToUniversalTime();
                                }
                                else
                                {
                                    int iterations = 0;
                                    Int32.TryParse(ucScheduleRequest.Iterations, out iterations);
                                    if (iterations > 0)
                                        req.Request.Iterations = iterations;
                                    else if (iterations == 0)
                                        req.Request.Iterations = 1;
                                    // if less than zero, set infinite iterations
                                    else if (iterations < 0)
                                        req.Request.Iterations = -1;
                                }
                            }

                            if (inParams.Count > 0)
                            {
                                //string json = new JavaScriptSerializer().Serialize(inParams);
                                //foreach (var wfParam in workflowInParams)
                                //{
                                //    wfParam.ParameterValue = (wfParam.IsSecret) ? SecurePayload.UnSecure(Convert.ToString(wfParam.ParameterValue), "IAP2GO_SEC!URE") : wfParam.ParameterValue;
                                //}

                                string json = JSONSerialize(workflowInParams);
                                req.Request.InputParameters = json;
                            }
                            req.Request.AssignedTo = node;

                            req.Request.Priority = Convert.ToInt32(ucScheduleRequest.Priority);
                            req.Request.RequestId = WorkFlowId;
                            req.Request.RequestType = Infosys.WEM.Node.Service.Contracts.Data.RequestTypeEnum.Workflow;
                            req.Request.RequestVersion = WorkflowVersion;
                            req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
                            req.Request.StopType = Infosys.WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                            req.Request.CompanyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);

                            ScheduledRequest client = new ScheduledRequest("");
                            scheduledReqIds = client.ServiceChannel.AddScheduledRequest(req);

                            if (ucScheduleRequest.RunOnCluster)
                                serverName = ucScheduleRequest.ClusterName;
                            else
                                serverName = selectedNodes[nodeCount];
                            nodeCount = nodeCount + 1;
                            if (outputwfMap.ContainsKey(serverName))
                            {
                                Guid Identifier = new Guid(outputwfMap[serverName]);
                                SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Scheduling Completed....." + Environment.NewLine, serverName, 99);
                            }

                            if (scheduledReqIds.IsSuccess)
                            {
                                foreach (string id in scheduledReqIds.ScheduledRequestIds)
                                {
                                    schView = new ExecutionResultView();
                                    schView.IsSuccess = true;
                                    schView.ServerName = serverName;
                                    if (outputwfMap.ContainsKey(serverName))
                                    {
                                        schView.Identifier = new Guid(outputwfMap[serverName]);
                                    }
                                    schView.SuccessMessage = "The workflow " + WorkFlowName + " (id:" + WorkFlowId + ") has been scheduled. The corresponding scheduled request id is " + id + ".";
                                    schResultView.Add(schView);
                                }
                            }
                            else
                            {
                                foreach (string id in scheduledReqIds.ScheduledRequestIds)
                                {
                                    schView = new ExecutionResultView();
                                    schView.IsSuccess = false;
                                    schView.ServerName = serverName;
                                    if (outputwfMap.ContainsKey(serverName))
                                    {
                                        schView.Identifier = new Guid(outputwfMap[serverName]);
                                    }
                                    schView.ErrorMessage = id;
                                    schResultView.Add(schView);
                                }
                            }
                        }

                        DisplayOutput(schResultView);
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;

                        break;
                }
                this.Close();
            }
        }

        void wfMgr_RemoveDebugAdornment()
        {
            if (RemoveDebugEvent != null)
                RemoveDebugEvent();
        }

        void wfMgr_ShowDebug(System.Activities.Debugger.SourceLocation srcLoc)
        {
            if (ShowDebugEvent != null)
                ShowDebugEvent(srcLoc);
        }

        Dictionary<object, System.Activities.Debugger.SourceLocation> wfMgr_UpdateSourceLocationMapping()
        {
            if (SourceLocationMapping != null)
                return SourceLocationMapping();
            else
                return null;            
        }

        void wfMgr_WorkflowExecutionCompleted(WorkflowExecutionManager.WorkflowExecutionCompletedArgs e)
        {
            List<ExecutionResultView> scheduleResultView = new List<ExecutionResultView>();
            ExecutionResultView view = new ExecutionResultView();

            ExecutionResult result = e.ExecutionOutput;

            if (string.IsNullOrEmpty(result.ErrorMessage))
            {
                view.IsScript = false;
                view.IsSuccess = true;
                view.SuccessMessage = result.SuccessMessage;
                //Translate output data to OutParameter object
                if (result.Output != null && result.Output.Count > 0)
                {
                    List<OutParameter> outParams = new List<OutParameter>();
                    OutParameter outParam = null;
                    foreach (WEM.WorkflowExecutionLibrary.Entity.Parameter p in result.Output)
                    {
                        outParam = new OutParameter();
                        outParam.ParameterName = p.ParameterName;
                        if (p.ParameterValue is string)
                            outParam.ParameterValue = p.ParameterValue.ToString();
                        else
                        {
                            outParam.ParameterValue = new JavaScriptSerializer().Serialize(p.ParameterValue);  //wfMgr.JsonSerialize(p.ParameterValue);

                            _parameters.ForEach(param =>
                            {
                                if (param.ParamIOType == Entities.ParameterIOTypes.Out && param.IsSecret)
                                    if (param.Name.Equals(outParam.ParameterName))
                                        outParam.ParameterValue = SecurePayload.Secure(outParam.ParameterValue, "IAP2GO_SEC!URE");
                            });
                        }
                        outParams.Add(outParam);
                    }
                    view.Output = outParams;
                }
            }
            else
            {
                view.IsScript = false;
                view.IsSuccess = false;
                view.ErrorMessage = result.ErrorMessage;
            }

            view.ServerName = Environment.MachineName;
            scheduleResultView.Add(view);

            AppendOutputViewArgsWF argWF = new AppendOutputViewArgsWF();
            argWF.Identifier = e.WorkflowInstanceId;
            argWF.progress = 100;
            argWF.executionResultView = view;
            AppendOutput(argWF);
        }
        private void SendStatusUpdate(Guid Identifier, string StatusMessge, string ServerName, int PercentComplete)
        {
            ExecutionResultView view = null;
            view = new ExecutionResultView();

            view.IsSuccess = true; ;

            // if (e.IsSuccess)
            view.SuccessMessage = StatusMessge;
            //else
            view.ErrorMessage = StatusMessge;
            //view.ServerName = Environment.MachineName;

            view.ServerName = string.IsNullOrEmpty(ServerName) ? Environment.MachineName : ServerName;

            view.IsScript = true;
            // string scriptID = e.ScriptID;


            AppendOutputViewArgsWF appendArgs = new AppendOutputViewArgsWF();

            appendArgs.Identifier = Identifier;
            appendArgs.progress = PercentComplete;
            appendArgs.executionResultView = view;
            List<ExecutionResultView> executionViewList = new List<ExecutionResultView>();
            executionViewList.Add(view);
            //DisplayOutput(executionViewList);
            AppendOutput(appendArgs);
        }

        private void wfExecutionManager_SendExecutionStatus(WorkflowExecutionManager.SendExecutionStatusArgs e)
        {
            ExecutionResultView view = null;
            view = new ExecutionResultView();

            view.IsSuccess = e.IsSuccess;

            if (e.IsSuccess)
                view.SuccessMessage = e.StatusMessage;
            else
                view.ErrorMessage = e.StatusMessage;
            //view.ServerName = Environment.MachineName;
            view.ServerName = e.ServerName;

            string workflowId = e.WorkflowId;

            AppendOutputViewArgsWF appendArgs = new AppendOutputViewArgsWF();
            appendArgs.scriptID = workflowId;
            appendArgs.Identifier = e.Identifier;
            appendArgs.progress = e.PercentComplete;
            appendArgs.executionResultView = view;
            List<ExecutionResultView> executionViewList = new List<ExecutionResultView>();
            executionViewList.Add(view);
            //DisplayOutput(executionViewList);
            AppendOutput(appendArgs);
        }

        private void AppendOutput(AppendOutputViewArgsWF appendArgs)
        {
            if (ExecutionResultView != null)
                ExecutionResultView(appendArgs);
        }


        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            foreach (Control param in pnlTextBoxes.Controls)
            {
                if (param is TextBox)
                    param.Text = "";
            }
        }

        private void GetClusters()
        {
            response = WFService.GetAllClustersByCategory(CategoryId.ToString());


        }

        //private void FillClusters()
        //{
        //    response = WFService.GetAllClustersByCategory(CategoryId.ToString());
        //    if ( response.Nodes!=null && response.Nodes.Count>0)
        //    {
        //    this.cmbSemantic.DataSource = response.Nodes.ToList();
        //    this.cmbSemantic.DisplayMember = "Name";
        //    this.cmbSemantic.ValueMember = "Id";
        //    //cmbSemantic.Items.Add("Select Cluster");
        //    cmbSemantic.SelectedIndex = 0;
        //    }
        //}

        private void rdSchedule_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSchedule.Checked == true)
            {
                ucScheduleRequest.CategoryId = CategoryId;
                // NodeType 1 is for Workflow
                ucScheduleRequest.NodeType = 1;
                ResetScheduleControls();

                // FillClusters();

                //DisplayRunOnNodeUI();
            }
            else
            {
                btnRun.Text = "Run Workflow";
                pnlScheduledRequest.Visible = false;
            }
            this.CenterToScreen();
        }


        //private void txtPriority_TextChanged(object sender, EventArgs e)
        //{
        //    int num;
        //    bool isNum = int.TryParse(txtPriority.Text.Trim(), out num);

        //    if (!isNum)
        //    {
        //        MessageBox.Show("Please enter only numbers > 0");
        //        txtPriority.Text = "1000";
        //    }
        //}



        //private void rbSetExecute_CheckedChanged(object sender, EventArgs e)
        //{

        //    if (rbSetExecute.Checked == true)
        //    {
        //        grpSetExecute.Visible = true;
        //        recurrancePanel = true;
        //        int height = grpSetExecute.Height;
        //        pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, grpSetExecute.Bottom);
        //        if (nodePanel)
        //            grpSchduleIAPNode.Location = new Point(grpSchduleIAPNode.Location.X, pnlScheduleProperties.Bottom - 40);
        //        pnlButtons.Location = new Point(pnlButtons.Location.X, pnlButtons.Location.Y + height);
        //        pnlSchedule.Height = pnlSchedule.Height + height;
        //        pnlScheduledRequest.Height = pnlScheduledRequest.Height + height;
        //        this.Height = this.Height + height;
        //        dtPickerEndBy.Value = DateTime.Today.AddMonths(1);
        //        rbNoEndDate.Checked = true;
        //    }
        //    else
        //    {
        //        grpSetExecute.Visible = false;
        //        pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, panel1.Height + margin);
        //        pnlScheduledRequest.Height = panel1.Height + pnlScheduleProperties.Height + 20;
        //        this.Height = this.Height - grpSetExecute.Height;
        //    }
        //    this.CenterToParent();
        //}


        //private void txtRatio_TextChanged_1(object sender, EventArgs e)
        //{
        //    int num;
        //    bool isNum = int.TryParse(txtRatio.Text.Trim(), out num);

        //    if (!isNum)
        //    {
        //        MessageBox.Show("Please enter only numbers > 0");
        //        //txtPriority.Text = "1000";
        //    }
        //}

        //private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        //{
        //    //DateTime startdate = Convert.ToDateTime(dtPickerStartDate.Text);
        //    //DateTime enddate = Convert.ToDateTime(dtPickerEndBy.Text);
        //    //if (startdate > enddate)
        //    //{
        //    //    MessageBox.Show("Start Date Must be Less Than End Date");
        //    //    //TimeSpan daycount = enddate.Subtract(startdate);
        //    //    //int dacount1 = Convert.ToInt32(daycount.Days) + 1;

        //    //}
        //    //rbEndBy.Checked = true;

        //}

        //private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        //{
        //    DateTime startdate = Convert.ToDateTime(dtPickerStartDate.Text);
        //    if (startdate != null)
        //    {
        //        if (startdate < DateTime.Now)
        //        {
        //            MessageBox.Show("Start Date Must be Greater Than or Equal To Current Date");
        //        }
        //    }
        //}


        //private void rbNow_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rbNow.Checked)
        //    {
        //        if (recurrancePanel == true && nodePanel == true)
        //        {
        //            recurrancePanel = false;
        //            grpSetExecute.Visible = false;
        //            pnlScheduleProperties.Location = new Point(pnlScheduleProperties.Location.X, panel1.Bottom);
        //            DisplayIAPNode();
        //        }
        //        else if (recurrancePanel)
        //        {
        //            recurrancePanel = false;
        //            int height = grpSetExecute.Height;
        //            grpSetExecute.Visible = false;
        //            pnlSchedule.Height = pnlSchedule.Height - height;
        //            pnlScheduledRequest.Height = pnlSchedule.Height + margin + 20;
        //            //this.Height = this.Height - height + margin;
        //            pnlButtons.Location = new Point(pnlButtons.Location.X, pnlScheduledRequest.Bottom + margin);

        //            //ResetScheduleControls();
        //        }
        //        else if (nodePanel)
        //        {
        //            nodePanel = false;
        //            DisplayIAPNode();
        //        }

        //    }
        //}

        //private void rbRunOnNode_CheckedChanged_1(object sender, EventArgs e)
        //{
        //    if (rbRunOnNode.Checked == true)
        //    {
        //        DisplayRunOnNodeUI();
        //    }
        //    else
        //    {
        //        if (grpSchduleIAPNode.Controls.Contains(iapPaneSchedule))
        //            grpSchduleIAPNode.Controls.Remove(iapPaneSchedule);
        //        grpSchduleIAPNode.Height = 0;
        //        nodePanel = false;
        //        //rbSelCluster.Checked = true;
        //    }
        //}

        //private void DisplayRunOnNodeUI()
        //{
        //    iapPaneSchedule.NodeType = 1;
        //    nodePanel = true;
        //    pnlSchedule.AutoScroll = true;
        //    pnlSchedule.HorizontalScroll.Visible = false;
        //    cmbSemantic.Visible = false;
        //    lblSelectCluster.Visible = false;
        //    grpSchduleIAPNode.Visible = true;
        //    iapPaneSchedule.mode = "Schedule";

        //    iapPaneSchedule.Dock = DockStyle.Top;
        //    iapPaneSchedule.HideExecute = true;

        //    if (rbRunOnNode.Checked && !grpSchduleIAPNode.Controls.Contains(iapPaneSchedule))
        //        grpSchduleIAPNode.Controls.Add(iapPaneSchedule);
        //    DisplayIAPNode();
        //}

        //private void DisplayIAPNode()
        //{

        //    grpSchduleIAPNode.Width = iapPaneSchedule.Width;
        //    grpSchduleIAPNode.Height = iapPaneSchedule.Height + margin;
        //    pnlScheduledRequest.Width = grpBoxRunScriptOptions.Width + margin;

        //    if (nodePanel)
        //    {
        //        if (rbNow.Checked)
        //            pnlSchedule.Height = panel1.Height + pnlScheduleProperties.Height + 150;//grpSchduleIAPNode.Height;
        //        else
        //            pnlSchedule.Height = panel1.Height + grpSetExecute.Height + pnlScheduleProperties.Height + 150;// grpSchduleIAPNode.Height;
        //    }
        //    else if (runOnCluster)
        //    {
        //        if (rbNow.Checked)
        //            pnlSchedule.Height = panel1.Height + pnlScheduleProperties.Height;
        //        else
        //            pnlSchedule.Height = panel1.Height + grpSetExecute.Height + pnlScheduleProperties.Height;
        //    }

        //    pnlScheduledRequest.Height = pnlSchedule.Height + margin;

        //    grpSchduleIAPNode.Location = new Point(pnlScheduleProperties.Location.X, pnlScheduleProperties.Bottom-40);
        //    pnlButtons.Location = new Point(pnlButtons.Location.X, pnlScheduledRequest.Bottom + margin);

        //    if (_parameters != null && _parameters.Count > 0)
        //        this.Height = grpBoxRunScriptOptions.Height + grpBoxScriptParams.Height + pnlButtons.Height + pnlScheduledRequest.Height + 80;
        //    else
        //        this.Height = grpBoxRunScriptOptions.Height + pnlButtons.Height + pnlScheduledRequest.Height + 80;
        //    this.CenterToParent();
        //}

        //private void rbSelCluster_CheckedChanged_1(object sender, EventArgs e)
        //{
        //    if (rbSelCluster.Checked == true)
        //    {
        //        runOnCluster = true;
        //        lblSelectCluster.Visible = true;
        //        cmbSemantic.Visible = true;

        //        int height = 150;// iapPaneSchedule.Height + margin;
        //        grpSchduleIAPNode.Visible = false;
        //        grpSchduleIAPNode.Height = 0;
        //        pnlSchedule.AutoScroll = false;
        //        pnlSchedule.Height = pnlSchedule.Height - height;
        //        pnlScheduledRequest.Height = pnlScheduledRequest.Height - height + margin;
        //        this.Height = this.Height - height + margin;
        //        pnlButtons.Location = new Point(pnlButtons.Location.X, pnlScheduledRequest.Bottom + margin);
        //    }
        //    else
        //    {
        //        cmbSemantic.Hide();
        //        lblSelectCluster.Visible = false;
        //        runOnCluster = false;
        //    }

        //    this.CenterToScreen();

        //}

        //private void txtRatio_Leave(object sender, EventArgs e)
        //{
        //    rbEndAfter.Checked = true;
        //    if (!IsValidInteger(txtRatio.Text.Trim()))
        //    {
        //        MessageBox.Show("Please enter positive or negative integer value only", "Validate Input");
        //    }
        //}

        //private void txtPriority_Enter(object sender, EventArgs e)
        //{
        //    TextBox txtBox = (TextBox)sender;
        //    int VisibleTime = 1000;  //in milliseconds

        //    ToolTip tt = new ToolTip();
        //    tt.Show("lower the value higher the priority", txtBox, 0, 0, VisibleTime);

        //}

        //private void txtPriority_Leave(object sender, EventArgs e)
        //{
        //    if (!IsValidInteger(txtPriority.Text.Trim()))
        //    {
        //        MessageBox.Show("Please enter positive or negative integer value only.", "Validate Input");
        //    }
        //}

        /// <summary>
        /// This method is used to validate if entered value is positive or negative number.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsValidInteger(string input)
        {
            string expression = @"^-?[0-9]\d*(\d+)?$";
            if (Regex.IsMatch(input, expression))
                return true;
            else
                return false;
        }

        //private void dtPickerEndBy_ValueChanged(object sender, EventArgs e)
        //{
        //    DateTime startdate = Convert.ToDateTime(dtPickerStartDate.Text);
        //    DateTime enddate = Convert.ToDateTime(dtPickerEndBy.Text);
        //    if (startdate > enddate)
        //    {
        //        MessageBox.Show("End date must be greater than start date");
        //        return;
        //    }
        //    rbEndBy.Checked = true;

        //} 

        private string JSONSerialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(obj.GetType());
            jsonSer.WriteObject(stream, obj);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            return json;
        }

        private void chkTraceWFDetails_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
    public enum WorkflowExecutionMode
    {
        Locally,
        IAPNodes,
        Schedule
    }
}
