using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.ATR.CommonViews;
using Infosys.WEM.SecureHandler;
using Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.AutomationClient
{
    public partial class ucIAPNodes : UserControl
    {
        ExecutionType _execute;
        string _mode;
        NodeExecution nodeClient = null;
        List<ListItem> registeredMachines;
        List<ListItem> selectedMachines = new List<ListItem>();
        const string DEFAULT_DOMAIN = "DomainName";
        List<string> domains = new List<string>();

        public delegate void IAPNodeExecutedEventHandler(List<ExecutionResultView> e);
        public event IAPNodeExecutedEventHandler IAPNodeExecuted;

        public ucIAPNodes(ExecutionType excute)
        {
            InitializeComponent();
            _execute = excute;
            _mode = mode;
            //if (mode == "Schedule")
            //{
            //    lblDomain.Text = "Cluster";
            //}
            
            //get the domain from config
            string domainsString = System.Configuration.ConfigurationManager.AppSettings["Domains"];
            if (!string.IsNullOrEmpty(domainsString))
            {
                domains = domainsString.Split(',').ToList();
            }
            else
                domains.Add(DEFAULT_DOMAIN);

            cmbDomains.Items.AddRange(domains.ToArray());

            cmbDomains.SelectedIndex = 0;
                
        }
        public ucIAPNodes(ExecutionType excute, string modeOfexecution)
        {
            InitializeComponent();
            //if(modeOfexecution=="Schedule")
            //{
            //    lblDomain.Text = "Cluster";
            //}
            _execute = excute;
            //get the domain from config
            string domainsString = System.Configuration.ConfigurationManager.AppSettings["Domains"];
            if (!string.IsNullOrEmpty(domainsString))
            {
                domains = domainsString.Split(',').ToList();
            }
            else
                domains.Add(DEFAULT_DOMAIN);

            cmbDomains.Items.AddRange(domains.ToArray());
            cmbDomains.SelectedIndex = 0;
        }


        public ucIAPNodes()
        {
            InitializeComponent();
            cmbDomains.SelectedIndex = 0;
        }

        private void btnFetchDomain_Click(object sender, EventArgs e)
        {
            lbxRegisteredNodes.DataSource = null;

            if (!string.IsNullOrEmpty(cmbDomains.SelectedItem.ToString()))
            {
                nodeClient = new NodeExecution();
                List<Entity.NodePE> nodesP = nodeClient.GetAllNodesOnDomain(cmbDomains.SelectedItem.ToString(), "", NodeType); //blank service url so that the service end point is taken from the config file
                if (nodesP != null && nodesP.Count > 0)
                {
                    registeredMachines = new List<ListItem>();
                    nodesP.ForEach(pe =>
                    {
                        if (pe != null)
                        {
                            ListItem lbl = new ListItem() { DiaplayMember = pe.HostMachineName };
                            lbl.Tag = pe;
                            registeredMachines.Add(lbl);
                        }
                    });
                    lbxRegisteredNodes.DataSource = registeredMachines;
                    lbxRegisteredNodes.DisplayMember = "DiaplayMember";
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            foreach (ListItem lbl in lbxRegisteredNodes.SelectedItems)
            {
                selectedMachines.Add(lbl);
                registeredMachines.Remove(lbl);
                //SelectedNodes.Add(lbl.DiaplayMember);
            }
            lbxRegisteredNodes.DataSource = null;
            lbxSelectedNodes.DataSource = null;

            lbxSelectedNodes.DataSource = selectedMachines;
            lbxSelectedNodes.DisplayMember = "DiaplayMember";

            lbxRegisteredNodes.DataSource = registeredMachines;
            lbxRegisteredNodes.DisplayMember = "DiaplayMember";
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            Execute();
        }

        public void Execute()
        {
            if (nodeClient != null && lbxSelectedNodes.SelectedItems != null)
            {
                List<IapNodeResultMapping> results = new List<IapNodeResultMapping>();

                //if (Parameters != null && Parameters.Count > 0)
                //{
                //    Parameters.ForEach(x =>
                //    {
                //        x.ParameterValue = (x.IsSecret) ? SecurePayload.UnSecure(x.ParameterValue, "IAP2GO_SEC!URE") : x.ParameterValue;
                //    });
                //}

                if (lbxSelectedNodes.Items == null || lbxSelectedNodes.Items.Count == 0)
                {
                    MessageBox.Show("No Iap Node selected for delegating the execution.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //frist get the inuemerable list
                List<ListItem> lbls = new List<ListItem>();
                foreach (ListItem lbl in lbxSelectedNodes.Items)
                    lbls.Add(lbl);

                lbls.AsParallel().ForAll(lbl => {
                    Entity.NodePE node = lbl.Tag as Entity.NodePE;
                    string nodeUrl = !string.IsNullOrEmpty(node.NettcpUrl) ? node.NettcpUrl : node.HttpUrl;
                    //string nodeUrl = node.HttpUrl;
                    if (!string.IsNullOrEmpty(nodeUrl))
                    {
                        //check if the execution is for script for workflow
                        switch (_execute)
                        {
                            case ExecutionType.Script:
                                if (CategoryId == 0 || ScriptId == 0)
                                    MessageBox.Show("Execution of only published Script is allowed on IAP node(s). Either Category Id or Script Id is invalid. For running on IAP node, these are mandatorily needed.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    lock (results) //to avoid the race condition so that more than item not tried to be added to the same index
                                    results.Add(new IapNodeResultMapping() { ExecResult = nodeClient.ExecuteScript(CategoryId, ScriptId, Parameters, nodeUrl, UsesUI, node.HostMachineName), NodeName = node.HostMachineName });
                                break;
                            case ExecutionType.Workflow:
                                if (CategoryId == 0 || WorkFlowId == Guid.Empty.ToString() || WorkflowVersion == 0)
                                    MessageBox.Show("Execution of only published Workflow is allowed on IAP node(s).", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                else
                                    lock (results)
                                    results.Add(new IapNodeResultMapping() { ExecResult = nodeClient.ExecuteWf(CategoryId, WorkFlowId, WorkflowVersion, Parameters, nodeUrl, UsesUI, node.HostMachineName), NodeName = node.HostMachineName });
                                break;
                            case ExecutionType.None:
                            default:
                                MessageBox.Show("No or unsupported execution type provided.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                        }
                    }
                });

                //foreach (ListItem lbl in lbxSelectedNodes.Items)
                ////foreach (ListItem lbl in lbxSelectedNodes.SelectedItems)
                //{
                //    Entity.NodePE node = lbl.Tag as Entity.NodePE;
                //    string nodeUrl = !string.IsNullOrEmpty(node.NettcpUrl) ? node.NettcpUrl : node.HttpUrl;
                //    //string nodeUrl = node.HttpUrl;
                //    if (!string.IsNullOrEmpty(nodeUrl))
                //    {
                //        //check if the execution is for script for workflow
                //        switch (_execute)
                //        {
                //            case ExecutionType.Script:
                //                if (CategoryId == 0 || ScriptId == 0)
                //                    MessageBox.Show("Execution of only published Script is allowed on IAP node(s). Either Category Id or Script Id is invalid. For running on IAP node, these are mandatorily needed.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //                else
                //                    results.Add(new IapNodeResultMapping() { ExecResult = nodeClient.ExecuteScript(CategoryId, ScriptId, Parameters, nodeUrl, UsesUI, node.HostMachineName), NodeName = node.HostMachineName });
                //                break;
                //            case ExecutionType.Workflow:
                //                if (CategoryId == 0 || WorkFlowId == Guid.Empty.ToString() || WorkflowVersion == 0)
                //                    MessageBox.Show("Execution of only published Workflow is allowed on IAP node(s).", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //                else
                //                    results.Add(new IapNodeResultMapping() { ExecResult = nodeClient.ExecuteWf(CategoryId, WorkFlowId, WorkflowVersion, Parameters, nodeUrl, UsesUI, node.HostMachineName), NodeName = node.HostMachineName });
                //                break;
                //            case ExecutionType.None:
                //            default:
                //                MessageBox.Show("No or unsupported execution type provided.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //                break;
                //        }
                //    }
                //}
                //if(lbxSelectedNodes.SelectedItems.Count>0)
                //    MessageBox.Show(string.Format("Execution of {0} triggered on the intended IAP nodes", _execute.ToString()), "Triggered...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (results.Count > 0)
                {
                    //IAPNodeResults resultsOP = new IAPNodeResults(results);
                    //resultsOP.ShowDialog();

                    List<ExecutionResultView> executionResultView = new List<ExecutionResultView>();
                    ExecutionResultView view = null;

                    foreach (IapNodeResultMapping result in results)
                    {
                        int rowIndex = 0;
                        string[] arrServerList = result.NodeName.Split(',');
                        Result r = result.ExecResult as Result;

                        view = new ExecutionResultView();
                        view.SuccessMessage = r.SuccessMessage;
                        view.ErrorMessage = r.ErrorMessage;
                        view.IsSuccess = r.IsSuccess;
                        view.ServerName = arrServerList[rowIndex];
                        if (r.Output != null)
                        {
                            List<OutParameter> outParams = new List<OutParameter>();
                            r.Output.ForEach(op =>
                            {
                                OutParameter o = new OutParameter();
                                o.ParameterName = op.ParameterName;
                                o.ParameterValue = op.ParameterValue.ToString();
                                outParams.Add(o);
                            });
                            view.Output = outParams;
                        }                        
                        executionResultView.Add(view);
                        view.Identifier = Guid.NewGuid();
                        rowIndex = rowIndex + 1;
                    }
                    if (IAPNodeExecuted != null)
                    {
                        IAPNodeExecuted(executionResultView);
                    }
                }
            }
        }

        public string mode { get; set; }

        public ExecutionType ToBeExecuted { set { _execute = value; } }

        public int CategoryId { get; set; }

        public int ScriptId { get; set; }

        public string WorkFlowId { get; set; }

        public int WorkflowVersion { get; set; }

        public List<Parameter> Parameters { get; set; }

        public bool UsesUI { get; set; }

        /// <summary>
        /// currently valid node types are- 0-None (i,e, for windows service based IAP node) 1- Workflow, 2 – Script
        /// </summary>
        public int NodeType { get; set; }

        public List<string> SelectedNodes
        {
            get
            {
                var list = new List<string>();
                foreach (ListItem item in lbxSelectedNodes.Items)
                {
                    list.Add(item.DiaplayMember);
                }
                return list;

            }          
        }

        public bool HideExecute
        {
            set
            {
                if (value)
                {
                    btnExecute.Visible = false;
                    lblExe.Visible = false;
                    this.Height = 310;
                }
                else
                {
                    btnExecute.Visible = true;
                    lblExe.Visible = true;
                    this.Height = 350;
                }
            }
        }

        private void btnUnSelect_Click(object sender, EventArgs e)
        {
            foreach (ListItem lbl in lbxSelectedNodes.SelectedItems)
            {
                registeredMachines.Add(lbl);
                selectedMachines.Remove(lbl);
                //SelectedNodes.Remove(lbl.DiaplayMember);
            }
            lbxRegisteredNodes.DataSource = null;
            lbxSelectedNodes.DataSource = null;

            lbxSelectedNodes.DataSource = selectedMachines;
            lbxSelectedNodes.DisplayMember = "DiaplayMember";

            lbxRegisteredNodes.DataSource = registeredMachines;
            lbxRegisteredNodes.DisplayMember = "DiaplayMember";
        }
    }

    public enum ExecutionType
    {
        None, // always keep this member as the first one so that it will be default value
        Script,
        Workflow
    }

    public class ListItem
    {
        public string DiaplayMember { get; set; }
        public object Tag { get; set; }
    }

    public class IapNodeResultMapping
    {
        public string NodeName { get; set; }
        public Result ExecResult { get; set; }
    }
}
