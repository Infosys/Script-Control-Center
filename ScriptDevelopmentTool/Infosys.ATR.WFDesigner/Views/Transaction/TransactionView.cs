using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.ATR.WFDesigner.Entities;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;
using Infosys.WEM.WorkflowExecutionLibrary;
using Infosys.ATR.CommonViews;
using Infosys.WEM.SecureHandler;
using Infosys.ATR.AutomationClient;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.ATR.WFDesigner.Constants;
using IMSWorkBench.Infrastructure.Interface;
using System.IO;
using System.Activities.XamlIntegration;
using System.Activities;
using Infosys.WEM.Client;
using Infosys.WEM.Service.Contracts.Message;
using System.ServiceModel;
using Infosys.ATR.WFDesigner.Services;
using System.Reflection;

namespace Infosys.ATR.WFDesigner.Views
{

    public partial class TransactionView : UserControl, IClose, ITransaction
    {
        public List<TransactionPE> Transactions { get; set; }
        public List<String> Categories { get; set; }
        public List<String> Period { get; set; }
        public List<String> State { get; set; }
        public List<String> Modules { get; set; }
        public List<User> Users { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Artifact> Artifacts { get; set; }
        public List<WEM.Node.Service.Contracts.Data.Transaction> WEMTransactions { get; set; }
        public string TransactionId { get; set; }

        [EventPublication(Constants.EventTopicNames.ShowOutputView, PublicationScope.Global)]
        public event EventHandler<EventArgs<List<ExecutionResultView>>> ShowOutputView;

        [EventPublication(Constants.EventTopicNames.AppendOutputViewWF, PublicationScope.Global)]
        public event EventHandler<Infosys.ATR.WFDesigner.Views.ExecuteWf.AppendOutputViewArgsWF> AppendOutputViewWF;
        System.Diagnostics.Stopwatch wfRun = System.Diagnostics.Stopwatch.StartNew();
        public List<TransactionPE> TransSearchResult { get; set; }      

        private List<string> lstUserChecked = null;
        private List<string> lstNodeChecked = null;
        private List<string> lstWFScriptChecked = null;
        private bool ascending = false;
        private string orderByKey = string.Empty;
        WorkflowIndentifier wfToRun = null;
        private string sAction = string.Empty;

        public TransactionView()
        {
            InitializeComponent();
            lstNodeChecked = new List<string>();
            lstWFScriptChecked = new List<string>();
            lstUserChecked = new List<string>();
            this.splitContainer1.Panel2Collapsed = true;
        }        
        internal void PopulateView()
        {
            this._presenter.GetTransactions();

            if (Transactions != null)
            {
                TransSearchResult = Transactions;
                this.dgTransaction.DataSource = Transactions;
                dgTransaction.Columns["RunningDays"].DefaultCellStyle.Format = "ddd";
                dgTransaction.Columns["TransactionId"].Visible = false;
                this.dgTransaction.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
                List<string> _Categories = Transactions.Where(t => !string.IsNullOrEmpty(t.CategoryName)).ToList().Select(t => t.CategoryName).Distinct().ToList<string>();
                _Categories.Insert(0, "-- Select Category--");
                this.cmbCategory.DataSource = _Categories;
                this.cmbCategory.SelectedIndex = 0;
                List<string> _Modules = Transactions.Select(t => t.Module).Distinct().ToList<string>();
                _Modules.Insert(0, "-- Select Module--");
                this.cmbModule.DataSource = _Modules;
                this.cmbModule.SelectedIndex = 0;
                List<string> period = new List<string> { "-- Select Period--", "ALL", "Today", "Last 5 Days", "Last 30 Days", "Last 6 Month", "Last 1 Year" };                
                this.cmbPeriod.DataSource = period;
                this.cmbPeriod.SelectedIndex = 0;
                List<string> state = Enum.GetNames(typeof(StateType)).ToList<string>();
                state.Insert(0, "-- Select State--");
                this.cmbState.DataSource = state;
                this.cmbState.SelectedIndex = 0;

                this.lstUsers.DataSource = TransSearchResult.Select(t=>t.User).Distinct().ToList<string>();
                this.lstUsers.ItemCheck += lstUsers_ItemCheck;
                this.lstNodes.DataSource = TransSearchResult.Select(t => t.Node).Distinct().ToList<string>();
                this.lstNodes.ItemCheck += lstNodes_ItemCheck;
                this.lstWFScripts.DataSource = TransSearchResult.Select(t => t.Name).Distinct().ToList<string>();
                this.lstWFScripts.ItemCheck += lstWFScripts_ItemCheck;

                if (this.dgTransaction.Rows != null &&
                    this.dgTransaction.Rows.Count > 0)
                    this.dgTransaction.Rows[0].Selected = true;

                this._presenter.UpdateStatus(Transactions);
                this.loadTransactionSummary(Transactions);
                this.dgTransaction.Visible = false;
                this.flwlpSummary.Visible = true;
                this.splitContainer1.Panel2Collapsed = true;
            }
        }
        void lstWFScripts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                lstWFScriptChecked.Add(lstWFScripts.Items[e.Index].ToString());
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (lstWFScriptChecked.Exists(x => x.Equals(lstWFScripts.Items[e.Index].ToString())))
                    lstWFScriptChecked.Remove(lstWFScripts.Items[e.Index].ToString());
            }

            SearchContent(txtSearch.Text);
            dgTransaction.DataSource = TransSearchResult;
            loadTransactionSummary(TransSearchResult);
        }
        void lstNodes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                lstNodeChecked.Add(lstNodes.Items[e.Index].ToString());
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (lstNodeChecked.Exists(x => x.Equals(lstNodes.Items[e.Index].ToString())))
                    lstNodeChecked.Remove(lstNodes.Items[e.Index].ToString());
            }

            SearchContent(txtSearch.Text);
            dgTransaction.DataSource = TransSearchResult;
            loadTransactionSummary(TransSearchResult);
        }
        void lstUsers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                lstUserChecked.Add(lstUsers.Items[e.Index].ToString());
            else if (e.NewValue == CheckState.Unchecked)
            {
                if (lstUserChecked.Exists(x => x.Equals(lstUsers.Items[e.Index].ToString())))
                    lstUserChecked.Remove(lstUsers.Items[e.Index].ToString());
            }

            SearchContent(txtSearch.Text);
            dgTransaction.DataSource = TransSearchResult;
            loadTransactionSummary(TransSearchResult);
        }
        public bool Close()
        {
            this._presenter.OnCloseView();
            this._presenter.RemoveStatus();
            return true;
        }
        private void dgTransaction_SelectionChanged(object sender, EventArgs e)
        {
            if (dgTransaction.SelectedRows != null && dgTransaction.SelectedRows.Count > 0)
            {
                btnResume.Enabled = true;
                var selected = dgTransaction.SelectedRows[0];
                var selectedTransaction = selected.DataBoundItem as TransactionPE;
                TransactionId = selectedTransaction.TransactionId;
                btnResume.Visible = false;
                if (selectedTransaction.State.Equals(StateType.Paused.ToString()))
                {
                    btnResume.Text = "Resume";
                    btnResume.Visible = true;
                    sAction = "Resume";
                }
                else if (selectedTransaction.State.Equals(StateType.Aborted.ToString()) || selectedTransaction.State.Equals(StateType.Failed.ToString()))
                {
                    btnResume.Visible = true;
                    btnResume.Text = "Re-Start";
                    sAction = "ReStart";
                }
                else
                    sAction = string.Empty;


                if (propGrdTransaction.InvokeRequired)
                    propGrdTransaction.Invoke(new Action(() =>
                    {
                        propGrdTransaction.SelectedObjects = new object[] { selectedTransaction as TransactionPE };
                        this.propGrdTransaction.Dock = DockStyle.Fill;
                    }));
                else
                {
                    propGrdTransaction.SelectedObjects = new object[] { selectedTransaction as TransactionPE };
                    this.propGrdTransaction.Dock = DockStyle.Fill;
                }

                this.splitContainer1.Panel2Collapsed = (this.flwlpSummary.Visible) ? true : false;
            }
        }
        private void btnResume_Click(object sender, EventArgs e)
        {
            var selected = this.dgTransaction.SelectedRows[0];
            int index = selected.Index;
            var selectedTransaction = selected.DataBoundItem as TransactionPE;

            if (!string.IsNullOrEmpty(TransactionId))
            {
                var trans = WEMTransactions.Where(x => x.InstanceId.Equals(TransactionId)).FirstOrDefault();
                wfToRun = new WorkflowIndentifier();
                wfToRun.CategoryId = trans.CategoryId;
                wfToRun.WorkflowId = new Guid(trans.ModuleId);
                wfToRun.WorkflowVersion = int.Parse(trans.ModuleVersion);
                wfToRun.Identifier = Guid.NewGuid();
                wfToRun.TransactionInstanceId = trans.InstanceId;

                if (sAction.Equals("Resume",StringComparison.InvariantCultureIgnoreCase))
                {
                    wfToRun.LastWorkflowStateId = trans.WorkflowPersistedStateId;
                    wfToRun.BookMark = trans.WorkflowActivityBookmark;
                }

                wfToRun.UnloadWorkflowFromMemory = true;
                ResumeModule(wfToRun);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnFilter_Click(sender, e);
        }
        private void SearchContent(string searchValue)
        {
            #region code to search in Datagrid
            //List<DataGridViewRow> rowFound = (dgTransaction.Rows.Cast<DataGridViewRow>()
            //                                        .Where<DataGridViewRow>(row => row.Cells.Cast<DataGridViewCell>()
            //                                                .Any(cell =>

            //                                                    ((!string.IsNullOrEmpty(Convert.ToString(cell.Value))) ? Convert.ToString(cell.Value) : "").Contains(searchValue)

            //                                                ))).ToList <DataGridViewRow>();
            #endregion
            TransSearchResult = Transactions;

            // Filter for Dropdown
            if (cmbModule.SelectedIndex > 0)
                TransSearchResult = TransSearchResult.Where(ent => ent.Module.Equals(cmbModule.SelectedValue)).ToList<TransactionPE>();
            if (cmbCategory.SelectedIndex > 0)
                TransSearchResult = TransSearchResult.Where(ent => !string.IsNullOrEmpty(ent.CategoryName)).ToList<TransactionPE>().Where(ent=>ent.CategoryName.Equals(cmbCategory.SelectedValue)).ToList<TransactionPE>();
            if (cmbState.SelectedIndex > 0)
                TransSearchResult = TransSearchResult.Where(ent => ent.State.Equals(cmbState.SelectedValue)).ToList<TransactionPE>();
            if (cmbPeriod.SelectedIndex > 0)
            {
                int intPeriod = (int)(Period)Enum.Parse(typeof(Period), cmbPeriod.SelectedValue.ToString().Replace(" ", "_"));

                switch (intPeriod)
                {
                    case 2:
                        TransSearchResult = TransSearchResult.Where(ent => ent.RunningDays.TotalDays <= 1).ToList<TransactionPE>();
                        break;
                    case 3:
                        TransSearchResult = TransSearchResult.Where(ent => ent.RunningDays.TotalDays <= 5).ToList<TransactionPE>();
                        break;
                    case 4:
                        TransSearchResult = TransSearchResult.Where(ent => ent.RunningDays.TotalDays <= 30).ToList<TransactionPE>();
                        break;
                    case 5:
                        TransSearchResult = TransSearchResult.Where(ent => ent.RunningDays.TotalDays <= (DateTime.Today - DateTime.Now.AddMonths(-6)).TotalDays).ToList<TransactionPE>();
                        break;
                    case 6:
                        TransSearchResult = TransSearchResult.Where(ent => ent.RunningDays.TotalDays <= (DateTime.Today - DateTime.Now.AddYears(-1)).TotalDays).ToList<TransactionPE>();
                        break;
                    default:
                        break;
                }
            }
            // Filter for checked User, Node and Script/WF ID
            if (lstUserChecked.Count > 0)
                TransSearchResult = TransSearchResult.Where(ent => lstUserChecked.Contains(ent.User)).ToList<TransactionPE>();
            if (lstNodeChecked.Count > 0)
                TransSearchResult = TransSearchResult.Where(ent => lstNodeChecked.Contains(ent.Node)).ToList<TransactionPE>();
            if (lstWFScriptChecked.Count > 0)
                TransSearchResult = TransSearchResult.Where(ent => lstWFScriptChecked.Contains(ent.Name)).ToList<TransactionPE>();

            // Filter for entered text in search textbox
            if (!string.IsNullOrEmpty(txtSearch.Text))
                TransSearchResult = TransSearchResult.Where(ent => ent.GetType().GetProperties()
                                                                        .Any(prop => Convert.ToString(prop.GetValue(ent)).ToLower().Contains(searchValue.ToLower()))).ToList<TransactionPE>();

            if (!(TransSearchResult.Count > 0))
            {
                this.splitContainer1.Panel2Collapsed = true;
                this.splitContainer2.Dock = DockStyle.Fill;
            }
            this._presenter.UpdateStatus(TransSearchResult);
        }
        private void RefreshTransaction()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                this._presenter.GetTransactions();
                SearchContent(txtSearch.Text);
                #region Commented code
                //foreach (int i in this.lstUsers.CheckedIndices)
                //    this.lstUsers.SetItemCheckState(i, CheckState.Unchecked);
                //foreach (int i in this.lstWFScripts.CheckedIndices)
                //    this.lstWFScripts.SetItemCheckState(i, CheckState.Unchecked);
                //foreach (int i in this.lstNodes.CheckedIndices)
                //    this.lstNodes.SetItemCheckState(i, CheckState.Unchecked);
                #endregion
                if (Transactions != null)
                {
                    this.dgTransaction.DataSource = TransSearchResult;
                    this.dgTransaction.Refresh();
                    this.dgTransaction.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
                    loadTransactionSummary(TransSearchResult);

                    if (this.dgTransaction.Rows != null && this.dgTransaction.Rows.Count > 0)
                        this.dgTransaction.Rows[0].Selected = true;

                    ResetChkList();
                    this._presenter.UpdateStatus(TransSearchResult);
                }

            }
            finally
            { 
                this.Cursor = Cursors.Default; 
            }
        }
        private void btnFilter_Click(object sender, EventArgs e)
        {
            ReLoad();
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            RefreshTransaction();
        }
        public void doAction(StateType stateType)
        {
            var selected = this.dgTransaction.SelectedRows[0];
            int index = selected.Index;
            var selectedTransaction = selected.DataBoundItem as TransactionPE;

            if (!string.IsNullOrEmpty(TransactionId))
            {
                bool IsSuccess = this._presenter.UpdateTransactionStatus(TransactionId, stateType);
                if (IsSuccess)
                    this.dgTransaction.DataSource = Transactions;

                dgTransaction.Refresh();

                var row = this.dgTransaction.Rows
                  .Cast<DataGridViewRow>()
                  .FirstOrDefault(r => (TransactionPE)r.DataBoundItem == (Transactions.Where(x => x.TransactionId.Equals(TransactionId)).FirstOrDefault()));

                this.dgTransaction.Rows[row.Index].Selected = true;
            }
        }
        private void dgTransaction_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            SearchContent(txtSearch.Text);
            orderByKey = dgTransaction.Columns[e.ColumnIndex].Name;

            if (ascending)
            {
                dgTransaction.DataSource = TransSearchResult.OrderBy(x => x.GetType().GetProperty(orderByKey).GetValue(x, null)).ToList<TransactionPE>();
                ascending = false;
            }
            else
            {
                dgTransaction.DataSource = TransSearchResult.OrderByDescending(x => x.GetType().GetProperty(orderByKey).GetValue(x, null)).ToList<TransactionPE>();
                ascending = true;
            }

            dgTransaction.Refresh();
        }
        private void ResumeModule(WorkflowIndentifier workflow)
        {
            WorkflowPE pe = GetWorkflowDetails(workflow);
            if (pe != null)
            {
                //ExecuteWf.IsResumed = (!string.IsNullOrEmpty(workflow.LastWorkflowStateId)) ? true : false;
                ExecuteWf run = null;
                var xaml = WFService.DownloadXAML(pe.WorkflowURI);

                if (pe.Parameters != null)
                {
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        string missingParams = VerifyParameterArguments(xaml, pe.Parameters);
                        if (!string.IsNullOrEmpty(missingParams))
                        {
                            System.Windows.Forms.MessageBox.Show("Argument(s) " + missingParams + " have not been specified as per published parameters. Please correct the same to run the workflow.", "Missing Arguments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    run = new ExecuteWf(pe.Parameters, string.Empty);
                }
                else
                    run = new ExecuteWf(null, xaml);               

                if (!string.IsNullOrEmpty(xaml))
                {
                    run.WorkflowText = xaml;
                }

                if (pe != null)
                {
                    if (pe.CategoryID == 0 || pe.WorkflowVersion == 0 || pe.WorkflowID == Guid.Empty)
                    {
                        run.DisableIAPNodeExecution = true;
                        run.DisableIAPNodeSchedule = true;
                    }
                    else
                    {
                        run.WorkFlowId = pe.WorkflowID.ToString();
                        run.WorkFlowName = pe.Name;
                        run.WorkflowVersion = pe.WorkflowVersion;
                        run.CategoryId = pe.CategoryID;
                        run.UsesUI = pe.UsesUIAutomation;
                        if (pe.UsesUIAutomation)
                            run.DisableIAPNodeExecution = true;
                        else
                            run.DisableIAPNodeExecution = false;
                    }
                }
                else
                {
                    run.DisableIAPNodeExecution = true;
                    run.DisableIAPNodeSchedule = true;
                }
                run.TransactionInstanceId = workflow.TransactionInstanceId;
                run.WorkflowPersistedStateId = workflow.LastWorkflowStateId;
                run.WorkflowActivityBookmark = workflow.BookMark;
                run.NodeExecuted += new ExecuteWf.NodeExecutedEventHandler(run_NodeExecuted);
                run.ExecutionResultView += run_ExecutionResultView;
                run.Show();
            }
        }
        /// <summary>
        /// This method is used to check if number of arguments specified is equal to number of parameters published.
        /// </summary>
        /// <param name="wfText">Workflow body text</param>
        /// <param name="wfParameters">List of published parameters</param>
        /// <returns></returns>
        private string VerifyParameterArguments(string wfText, List<WorkflowParameterPE> wfParameters)
        {
            string paramName = "";
            TextReader txtReader = new StringReader(wfText);
            var dynamicActivity = ActivityXamlServices.Load(txtReader) as DynamicActivity;
            if (dynamicActivity == null)
                throw new InvalidDataException("Invalid DynamicActivity.");

            foreach (var parameter in wfParameters)
            {
                if (!dynamicActivity.Properties.Contains(parameter.Name))
                {
                    paramName = paramName + parameter.Name + ",";
                }
            }
            //int index = wfParameters.FindIndex(f => f.Name.ToLower().Equals(item.Name.ToLower()));

            if (!string.IsNullOrEmpty(paramName) && paramName.Contains(","))
            {
                paramName = paramName.Substring(0, paramName.Length - 1);
            }

            return paramName;
        }
        void run_ExecutionResultView(ExecuteWf.AppendOutputViewArgsWF e)
        {
            if (AppendOutputViewWF != null)
                AppendOutputViewWF(this, e);

            if (e.progress.Equals(100))
            {
                this._presenter.GetTransactions();

                if (Transactions != null)
                {
                    string transId = TransactionId;
                    if (this.dgTransaction.InvokeRequired)
                        this.dgTransaction.Invoke(new Action(() =>
                        {
                            ReLoad();
                            var row = this.dgTransaction.Rows
                                          .Cast<DataGridViewRow>()
                                          .FirstOrDefault(r => (TransactionPE)r.DataBoundItem == (Transactions.Where(x => x.TransactionId.Equals(transId)).FirstOrDefault()));
                            if (row != null)
                                this.dgTransaction.Rows[row.Index].Selected = true;

                        }));
                    else
                    {
                        ReLoad();
                        var row = this.dgTransaction.Rows
                                        .Cast<DataGridViewRow>()
                                        .FirstOrDefault(r => (TransactionPE)r.DataBoundItem == (Transactions.Where(x => x.TransactionId.Equals(transId)).FirstOrDefault()));

                        if (row != null)
                            this.dgTransaction.Rows[row.Index].Selected = true;
                    }
                }
            }
        }
        void run_NodeExecuted(List<CommonViews.ExecutionResultView> e)
        {
            var executionTime = wfRun.ElapsedMilliseconds;

            if (ShowOutputView != null)
                ShowOutputView(this, new EventArgs<List<ExecutionResultView>>(e));
        }
        private WorkflowPE GetWorkflowDetails(WorkflowIndentifier workflow)
        {
            GetWorkflowDetailsResMsg response = null;
            string storageBaseUrl = CommonServices.Instance.StorageBaseURL;
            WorkflowAutomation workflowClient = new WorkflowAutomation(workflow.WEMWorkflowServiceUrl);
            //this approach followed in handle the scenario where wcf service is called from wcf service
            //issue reference- http://blogs.msdn.com/b/pedram/archive/2008/07/19/webchannelfactory-inside-a-wcf-service.aspx
            var channel = workflowClient.ServiceChannel;
            using (new OperationContextScope((IContextChannel)channel))
            {
                response = channel.GetWorkflowDetails(workflow.CategoryId, workflow.WorkflowId, workflow.WorkflowVersion, "", "");
            }

            if (response != null)
            {
                var wf = response.WorkflowDetails;

                return new WorkflowPE
                {
                    Name = wf.Name,
                    Description = wf.Description,
                    CategoryID = wf.CategoryID,
                    WorkflowURI = storageBaseUrl + wf.WorkflowURI,
                    WorkflowID = wf.WorkflowID,
                    WorkflowVersion = wf.WorkflowVersion,
                    UsesUIAutomation = wf.UsesUIAutomation,
                    IslongRunningWorkflow = wf.IslongRunningWorkflow,
                    IdleStateTimeout = wf.IdleStateTimeout,
                    Parameters = Translators.WorkflowParameterPE_SE.WorkflowParameterListSEtoPE(wf.Parameters)
                };
            }
            else
                return new WorkflowPE();
        }
        private void loadTransactionSummary(List<TransactionPE> TransContent)
        {
            this.flwlpSummary.Controls.Clear();
            int? total = TransContent.Count;
            int? completed = TransContent.Count(t => t.State == "Completed");
            int? inprogress = TransContent.Count(t => t.State == "InProgress");
            int? paused = TransContent.Count(t => t.State == "Paused");
            int? failed = TransContent.Count(t => t.State == "Failed");
            int? aborted = TransContent.Count(t => t.State == "Aborted");

            var uclTotal = new Infosys.ATR.WFDesigner.Views.Transaction.uclTransStatus(Color.MidnightBlue, total.GetValueOrDefault().ToString(), Color.BlueViolet, "Total", Color.BlueViolet);
            this.flwlpSummary.Controls.Add(uclTotal);
            uclTotal.TransactionState += uclInProgress_TransactionState;

            foreach (string item in Enum.GetNames(typeof(StateType)).ToList<string>())
            {
                switch (item)
                {
                    case "InProgress":
                        var uclInProgress = new Infosys.ATR.WFDesigner.Views.Transaction.uclTransStatus(Color.DarkGoldenrod, inprogress.GetValueOrDefault().ToString(), Color.Goldenrod, item, Color.Goldenrod);
                        this.flwlpSummary.Controls.Add(uclInProgress);
                        uclInProgress.TransactionState += uclInProgress_TransactionState;
                        break;
                    case "Completed":
                        var uclCompleted = new Infosys.ATR.WFDesigner.Views.Transaction.uclTransStatus(Color.DarkOliveGreen, completed.GetValueOrDefault().ToString(), Color.OliveDrab, item, Color.OliveDrab);
                        this.flwlpSummary.Controls.Add(uclCompleted);
                        uclCompleted.TransactionState += uclInProgress_TransactionState;
                        break;
                    case "Paused":
                        var uclPaused = new Infosys.ATR.WFDesigner.Views.Transaction.uclTransStatus(Color.DarkOrange, paused.GetValueOrDefault().ToString(), Color.Orange, item, Color.Orange);
                        this.flwlpSummary.Controls.Add(uclPaused);
                        uclPaused.TransactionState += uclInProgress_TransactionState;
                        break;
                    case "Failed":
                        var uclFailed = new Infosys.ATR.WFDesigner.Views.Transaction.uclTransStatus(Color.DarkRed, failed.GetValueOrDefault().ToString(), Color.Red, item, Color.Red);
                        this.flwlpSummary.Controls.Add(uclFailed);
                        uclFailed.TransactionState += uclInProgress_TransactionState;
                        break;
                    case "Aborted":
                        var uclAborted = new Infosys.ATR.WFDesigner.Views.Transaction.uclTransStatus(Color.DarkRed, aborted.GetValueOrDefault().ToString(), Color.Red, item, Color.Red);
                        this.flwlpSummary.Controls.Add(uclAborted);
                        uclAborted.TransactionState += uclInProgress_TransactionState;
                        break;
                }
            }

            if (this.flwlpSummary.Visible.Equals(true))
                this.splitContainer1.Panel2Collapsed = true;

            this.flwlpSummary.Refresh();
        }
        void uclInProgress_TransactionState(string status)
        {
            flwlpSummary.Visible = false;
            dgTransaction.Visible = true;
            if (!status.ToLower().Equals("total"))
                cmbState.SelectedIndex = cmbState.Items.IndexOf(status);
            else
                cmbState.SelectedIndex = 0;
            SearchContent(txtSearch.Text);
            ResetChkList();
            dgTransaction.DataSource = TransSearchResult;
            loadTransactionSummary(TransSearchResult);
            this.btnHide.Text = "View Summary";
        }
        private void btnHide_Click(object sender, EventArgs e)
        {
            if (this.dgTransaction.Visible.Equals(false))
            {
                //loadTransactionSummary(SearchContent(txtSearch.Text));
                this.dgTransaction.Visible = true;
                this.flwlpSummary.Visible = false;
                if ((TransSearchResult.Count > 0))
                {
                    this.splitContainer1.Panel2Collapsed = false;
                    this.splitContainer1.Dock = DockStyle.Fill;
                }
                this.btnHide.Text = "View Summary";
                this.toolTip1.SetToolTip(this.btnHide, "Click here to Summary View");
            }
            else
            {
                this.dgTransaction.Visible = false;
                this.flwlpSummary.Visible = true;
                //loadTransactionSummary(SearchContent(txtSearch.Text));
                this.splitContainer1.Panel2Collapsed = true;
                this.splitContainer2.Dock = DockStyle.Fill;
                this.btnHide.Text = "View Detail";
                this.toolTip1.SetToolTip(this.btnHide, "Click here to Detail View");
            }
        }
        private void ResetChkList()
        {
            foreach (int i in this.lstUsers.CheckedIndices)
                this.lstUsers.SetItemCheckState(i, CheckState.Unchecked);
            foreach (int i in this.lstWFScripts.CheckedIndices)
                this.lstWFScripts.SetItemCheckState(i, CheckState.Unchecked);
            foreach (int i in this.lstNodes.CheckedIndices)
                this.lstNodes.SetItemCheckState(i, CheckState.Unchecked);

            this.lstUsers.DataSource = TransSearchResult.Select(t => t.User).Distinct().ToList<string>();
            this.lstNodes.DataSource = TransSearchResult.Select(t => t.Node).Distinct().ToList<string>();
            this.lstWFScripts.DataSource = TransSearchResult.Select(t => t.Name).Distinct().ToList<string>();
        }
        private void ReLoad()
        {
            SearchContent(txtSearch.Text);
            ResetChkList();
            dgTransaction.DataSource = TransSearchResult;
            this.dgTransaction.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            loadTransactionSummary(TransSearchResult);
        }
    }
}
