using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Linq.Expressions;

using WEMClient = Infosys.WEM.Client;
using Infosys.ATR.Entities;
using Infosys.WEM.ScriptExecutionLibrary;
using Infosys.ATR.ScriptsRepository.Views.Scripts;
using Infosys.WEM.Service.Common.Contracts.Data;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.ATR.ScriptRepository.Models;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;

using ScriptParams = Infosys.WEM.Scripts.Service.Contracts.Data;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using Infosys.ATR.CommonViews;

using Infosys.WEM.Node.Service.Contracts.Message;
using System.Web.Script.Serialization;
using Infosys.WEM.Client;
using System.Runtime.Serialization.Json;
using Infosys.WEM.SecureHandler;
using System.Collections;

namespace Infosys.ATR.ScriptRepository.Views
{
    public partial class MainRepositoryView : UserControl, IScripts
    {
        [EventPublication("EditScript", PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<Object[]>> EditScript;

        [EventPublication(Constants.EventTopicNames.ShowOutputView, PublicationScope.Global)]
        public event EventHandler<EventArgs<List<ExecutionResultView>>> ShowOutputView;

        [EventPublication(Constants.EventTopicNames.AppendOutputView, PublicationScope.Global)]
        public event EventHandler<AppendOutputViewArgs> AppendOutputView;
        public bool iAPNode = false;
        public class AppendOutputViewArgs : EventArgs
        {
            public ExecutionResultView executionResultView;
            public string scriptID;
            public Guid Identifier;
            public int progress;
        }

        [EventPublication(Constants.EventTopicNames.ShowRun, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ShowRun;

        TreeNodeContext currentNode;
        bool asc = false;
        WEMClient.ScriptRepository scriptClient = new WEMClient.ScriptRepository();
        WEMClient.CommonRepository commonRepositoryClient = new WEMClient.CommonRepository();
        string _categorySelected = "";
        private const string SCRIPTRESULT = "Script Execution Results";
        private const string ImageKey = "CatImage";
        internal int selectedScriptIndex = 0;
        public static List<string> parametersCollection = null;
        ImageList treeList = null;
        string companyId;

        //List<Models.Category> categories = null;
        TreeNode _root;
        internal static List<Users> Users = null;
        internal static string securityType = "";
        internal static List<int> userCatList = null;
        internal List<Models.Category> categories = null;
        internal static string selectedCatID = "";

        Views.ScriptDetails editScriptDetails = null;

        internal static bool allowRemote = false;
        internal static string NetworkPath = "";
        internal static string UserName = "";
        internal static SecureString Password = null;
        internal static bool networkParamsValidation = false;
        /// <summary>
        /// Decide whether to show the SSHForm for accepting hostname, username and password for executing scripts via SSH.
        /// </summary>
        internal static bool showSSHForm = false;
        internal static string remoteServerName = "";
        internal static string LinuxServerName = "";
        internal static string LinuxKeyPath = "";

        internal static bool ScheduledRequest = false;

        Models.Script selectedScript = null;

        internal static bool RunOnCluster = false;
        internal static bool RunOnNode = false;
        internal static string Priority = "";
        internal static DateTime StartDate;
        internal static DateTime EndDate;
        internal static bool ScheduledForNow = false;
        internal static bool ScheduledForLater = false;
        internal static bool NoEndDate = false;
        internal static bool EndBy = false;
        internal static string ClusterName = "";
        internal static string ClusterValue = "";
        internal static string Iterations = "";
        internal static List<string> SelectedNodes;
        internal static bool UsesUIAutomation = false;
        internal bool enableControls = true;
        bool isSuperAdmin;
        public static Dictionary<string, string> outputScriptMap = new Dictionary<string, string>();
        bool iapnodeexecuted = false;
        string LinuxKey = string.Empty;
        internal static Users CurentUser = null; 

        public MainRepositoryView()
        {
            InitializeComponent();
            splitContainerDetails.Panel2.VerticalScroll.Visible = true;
            //showSSHForm = false;
            parametersCollection = new List<string>();
            treeList = new ImageList();
            Image img = Image.FromFile(@"Images\Folder.png");
            treeList.Images.Add(ImageKey, img);
            tvCatSubcat.ImageList = treeList;
            showSSHForm = false;
            companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
            LinuxKey = System.Configuration.ConfigurationManager.AppSettings["LinuxKeyPath"];
            
        }

        public void LoadCategory(List<Models.Category> categories = null)
        {
            try
            {
                var security = this._presenter.WorkItem.RootWorkItem.State["Security"];
                securityType = security.ToString();
                //this.Cursor = Cursors.WaitCursor;
                //this.categories.Clear();

                if (this.categories == null)
                {

                    Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response =
                        commonRepositoryClient.ServiceChannel.GetAllCategoriesByCompany(companyId, Constants.Application.ModuleID);

                    //Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response =
                    //  commonRepositoryClient.ServiceChannel.GetAllCategoriesWithData(companyId, Constants.Application.ModuleID);

                    this.categories = Translators.CategoryTreePE_SE.CategoryListSEtoPE(response.Categories);    

                    if (this.categories != null)
                    {
                        isSuperAdmin = (bool)this._presenter.WorkItem.RootWorkItem.Items["IsSuperAdmin"];
                        if (!isSuperAdmin)
                        {

                            if (security.Equals("AllowAuthorised"))
                            {
                                var userCategory = this._presenter.WorkItem.RootWorkItem.Items.Get("CurrentUserCategories") as List<int>;
                                userCatList = userCategory;
                                if (userCategory != null)
                                {
                                    Users = this._presenter.WorkItem.RootWorkItem.Items.Get("CurrentUser") as List<Users>;
                                    this.categories = response.Categories.Where(c => userCategory.Contains(Convert.ToInt32(c.CategoryId))).
                                    Select(c => new Models.Category
                                    {
                                       // CreatedBy = c.CreatedBy,
                                        Description = c.Description,
                                        Id = c.CategoryId.ToString(),
                                       // ModifiedBy = c.ModifiedBy,
                                        Name = c.Name,
                                        ParentId = c.ParentId.GetValueOrDefault(),
                                        CompanyId = c.CompanyId
                                    }).ToList();

                                }
                            }
                        }


                        for (int i = 0; i < this.categories.Count; i++)
                        {
                            var c = this.categories[i];
                            if (c.CompanyId == 0 && c.ParentId == 0)
                            {

                                var subCat = this.categories.Where(sc => sc.CompanyId == 0 && sc.ParentId == Convert.ToInt32(c.Id));
                                if (subCat == null || subCat.Count() == 0)
                                {
                                    this.categories.Remove(c);
                                }
                            }
                        }

                       // RemoveEmptyCategories();

                        PopulateCategories();
                    }
                    else
                    {
                        MessageBox.Show("No Category exists", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }

        private void RemoveEmptyCategories()
        {
            for (int i = 0; i < this.categories.Count; i++)
            {
                var c = categories[i];

                if (!HasScripts(c))
                    categories.Remove(c);

            }

        }

        private bool HasScripts(Models.Category c)
        {
            var childCategories = categories.Where(sc => sc.ParentId == Convert.ToInt32(c.Id)).ToList();

            if (childCategories == null || childCategories.Count == 0)
            {
                var scripts = scriptClient.ServiceChannel.GetAllScriptDetails(c.Id);

                if (scripts.Scripts == null || scripts.Scripts.Count == 0)
                    return false;
            }

            return true;
        }

        private void AddNode(Models.Category c, TreeNode parent)
        {
            if (parent == null)
            {
                parent = new TreeNode();
                parent.Text = c.Name;
                parent.Tag = c;
                parent.ImageKey = ImageKey;
                _root.Nodes.Add(parent);
            }
            else
            {
                TreeNode child = new TreeNode();
                child.Text = c.Name;
                child.Tag = c;
                child.ImageKey = ImageKey;
                parent.Nodes.Add(child);
                parent = child;
            }

            categories.Where(sc => sc.ParentId == Convert.ToInt32(c.Id)).ToList().ForEach(child =>
            {
                AddNode(child, parent);
            });

        }

        private void PopulateCategories()
        {
            _root = new TreeNode("Categories");
            this.tvCatSubcat.Nodes.Add(_root);
            _root.Expand();
            categories.Where(g => g.ParentId == 0).ToList().
                ForEach(sg =>
                {
                    AddNode(sg, null);
                });

            this.tvCatSubcat.SelectedNode = _root;

        }


        private void tvCatSubcat_MouseClick(object sender, MouseEventArgs e)
        {
            this._presenter.ActivateMenuHandler(false);

        }

        private void tvCatSubcat_AfterExpand(object sender, TreeViewEventArgs e)
        {
            this._presenter.ActivateMenuHandler(false);
        }

        private void dgList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgList.DataSource == null)
                return;
            if (e.RowIndex < 0)
                return;

            this.Cursor = Cursors.WaitCursor;
            selectedScriptIndex = e.RowIndex;
            // if (IsScript(e.RowIndex))
            {
                if (e.ColumnIndex == 0 && IsScript(e.RowIndex))
                {
                    //i.e. Delete is called
                    if (MessageBox.Show("Are you sure you want to delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //delete the row selected
                        try
                        {
                            DeleteScript(e.RowIndex);
                            BindCategory();
                            MessageBox.Show("Record deleted successfully", "Status", MessageBoxButtons.OK);
                        }
                        catch (Exception ex)
                        {
                            string err = ex.Message;
                            if (ex.InnerException != null)
                                err = err + ". \nInner Exception- " + ex.InnerException.Message;
                            MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                // Logic to run the script
                else if (e.ColumnIndex == 1 && IsScript(e.RowIndex))
                {
                    CheckScriptParamters(e.RowIndex);
                }
                else if (e.ColumnIndex == 2 && IsScript(e.RowIndex))
                {
                    //if (((DataGridViewImageColumn ) this.dgList.Columns[e.ColumnIndex].DefaultCellStyle.NullValue==null))                
                    //   return;
                    // if (this.dgList.Rows[e.RowIndex].Cells[2].Value == null)

                    Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>> tupleScriptDetails =
                  (Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>)dgList.Rows[e.RowIndex].Cells[7].Value;
                    // If task type is null
                    if (tupleScriptDetails != null)
                    {
                        if ((!string.IsNullOrEmpty(tupleScriptDetails.Item7)) && tupleScriptDetails.Item7.ToLower().Equals("command") || tupleScriptDetails.Item7.ToLower().Equals("sh command"))
                            return;
                    }

                    Models.Script selectedscript = this.dgList.Rows[e.RowIndex].Tag as Models.Script;
                    ShowCategoryScriptView(e.RowIndex);
                    editScriptDetails.EditScript_Handler();
                }
                //}
                // Display details in the right panel
                else
                {
                    splitContainerDetails.Panel2.Controls.Clear();

                    var category = this.dgList.Rows[e.RowIndex].DataBoundItem as
                         Tuple<Image, Image, Image, Image, string, string, int, Tuple<List<ScriptParams.ScriptParam>,
                string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>;

                    Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>> tupleScriptDetails =
                        (Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>)
                        this.dgList.Rows[e.RowIndex].Cells[7].Value;

                    // If task type is null
                    //if (string.IsNullOrEmpty(tupleScriptDetails.Item6))
                    if (string.IsNullOrEmpty(tupleScriptDetails.Item7))
                    {
                        currentNode = TreeNodeContext.Category;
                    }
                    else
                    {
                        currentNode = TreeNodeContext.Script;
                    }

                    switch (currentNode)
                    {
                        case TreeNodeContext.Root:
                            //i.e. category is listed in the grid view
                            ShowCategoryDetails(e.RowIndex);
                            //ShowCategoryDetails();
                            break;
                        case TreeNodeContext.Category:
                            //    //i.e. sub category is listed in the grid view
                            ShowCategoryDetails(e.RowIndex);
                            break;
                        case TreeNodeContext.Script:
                            ShowCategoryScriptView(e.RowIndex);
                            break;
                    }                    
                }
            }
            this.Cursor = Cursors.Default;
        }

        // Run the script
        private void ScriptParameters_RunScript_ButtonClicked()
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form is ScriptParameters)
                {
                    form.Hide();
                    form.Close();
                    form.Dispose();
                    break;
                }
            }
            Models.Script s = null;
            if (this.selectedScript == null)
            {
                if (this.dgList.CurrentRow != null)
                    s = this.dgList.CurrentRow.Tag as Models.Script;
                else if (editScriptDetails != null)
                    s = editScriptDetails._script;

            }
            else
                s = this.selectedScript;
            if (ScheduledRequest)
                ScheduleScript(s);
            else
                RunScriptLocally(s);
            //RunScriptLocally(selectedScriptIndex);
        }

        private void ScheduleScript(Models.Script selectedscript)
        {
            AddScheduledRequestResMsg scheduledReqIds = null;
            List<ExecutionResultView> schResultView = new List<ExecutionResultView>();
            ExecutionResultView schView = null;
            int nodeCount = 0;
            int count = 0;
            string serverName = "";
            List<string> selectedNodes = new List<string>();
            List<string> selectedNodeNames = new List<string>();

            //gather the parameters
            Dictionary<string, string> inParams = new Dictionary<string, string>();
            List<Parameter> scriptInParams = new List<Parameter>();

            if (selectedscript.Parameters != null && selectedscript.Parameters.Count > 0)
            {
                var inputParameters = from p in selectedscript.Parameters
                                      where p.IOType.ToString().ToLower().Equals("in")
                                      select p;
                if (inputParameters.FirstOrDefault() != null)
                {
                    if (scriptInParams != null && scriptInParams.Count > 0)
                        scriptInParams.Clear();
                    scriptInParams = new List<Parameter>();
                    foreach (Models.ScriptParameter p in selectedscript.Parameters)
                    {
                        if (p.IOType.ToString().ToLower().Equals("in"))
                        {
                            Parameter paramInputValue = new Parameter();
                            paramInputValue.ParameterName = selectedscript.Parameters[count].Name;
                            paramInputValue.ParameterValue = (selectedscript.Parameters[count].IsSecret) ? SecurePayload.Secure(parametersCollection[count], "IAP2GO_SEC!URE") : parametersCollection[count]; 
                            paramInputValue.allowedValues = selectedscript.Parameters[count].AllowedValues;
                            paramInputValue.DataType = selectedscript.Parameters[count].DataType;
                            paramInputValue.IsPaired = !selectedscript.Parameters[count].IsUnnamed;
                            paramInputValue.IsSecret = selectedscript.Parameters[count].IsSecret;

                            inParams.Add(selectedscript.Parameters[count].Name, parametersCollection[count]);
                            scriptInParams.Add(paramInputValue);

                            count = count + 1;
                        }
                    }
                }
            }


            if (RunOnCluster)
            {
                selectedNodes.Add(ClusterValue);
                selectedNodeNames.Add(ClusterName);


            }
            else if (RunOnNode)
            {
                selectedNodes = SelectedNodes;
                selectedNodeNames = SelectedNodes;


            }
            if (selectedNodeNames != null)
            {
                outputScriptMap.Clear();
                foreach (string node in selectedNodeNames)
                {
                    Guid Identifier = Guid.NewGuid();

                    outputScriptMap.Add(node, Identifier.ToString());
                    SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Scheduling Request....." + Environment.NewLine, node, 20);
                }
            }

            foreach (string node in selectedNodes)
            {
                AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
                req.Request = new Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest();
                req.Request.CategoryId = Convert.ToInt32(selectedscript.CategoryId);
                req.Request.Requestor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                if (ScheduledForNow)
                    req.Request.ExecuteOn = System.DateTime.UtcNow;
                else
                {
                    req.Request.ExecuteOn = StartDate;
                    if (NoEndDate)
                        req.Request.Iterations = -1;
                    else if (EndBy)
                    {
                        req.Request.EndDate = EndDate;
                    }
                    else
                    {
                        int iterations = 0;
                        Int32.TryParse(Iterations, out iterations);
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
                    string json = JSONSerialize(scriptInParams);
                    req.Request.InputParameters = json;
                }
                req.Request.AssignedTo = node;

                req.Request.Priority = Convert.ToInt32(Priority);
                req.Request.RequestId = selectedscript.Id;
                req.Request.RequestType = Infosys.WEM.Node.Service.Contracts.Data.RequestTypeEnum.Script;
                // req.Request.RequestVersion = 1;//No need to assign for script
                req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
                req.Request.StopType = Infosys.WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                req.Request.CompanyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);

                ScheduledRequest client = new ScheduledRequest("");
                scheduledReqIds = client.ServiceChannel.AddScheduledRequest(req);

                if (RunOnCluster)
                    serverName = ClusterName;
                else
                    serverName = selectedNodes[nodeCount];
                nodeCount = nodeCount + 1;
                if (!string.IsNullOrEmpty(serverName))
                {
                    if (outputScriptMap.ContainsKey(serverName))
                    {
                        Guid Identifier = new Guid(outputScriptMap[serverName]);
                        SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Scheduling Completed....." + Environment.NewLine, serverName, 99);
                    }

                }

                if (scheduledReqIds.IsSuccess)
                {
                    foreach (string id in scheduledReqIds.ScheduledRequestIds)
                    {
                        schView = new ExecutionResultView();
                        schView.IsSuccess = true;
                        schView.Progress = 100;
                        if (outputScriptMap.ContainsKey(serverName))
                        {
                            schView.Identifier = new Guid(outputScriptMap[serverName]);
                        }
                        schView.ServerName = serverName;
                        schView.SuccessMessage = "The script " + selectedscript.Name + " (id:" + selectedscript.Id + ") has been scheduled. The corresponding scheduled request id is " + id + ".";
                        schView.data = schView.SuccessMessage;
                        schResultView.Add(schView);
                    }
                }
                else
                {
                    foreach (string id in scheduledReqIds.ScheduledRequestIds)
                    {
                        schView = new ExecutionResultView();
                        schView.Progress = 100;
                        if (outputScriptMap.ContainsKey(serverName))
                        {
                            schView.Identifier = new Guid(outputScriptMap[serverName]);
                        }
                        schView.IsSuccess = false;
                        schView.ServerName = serverName;
                        schView.ErrorMessage = id;
                        schResultView.Add(schView);
                    }
                }
            }

            DisplayOutput(schResultView);
            RunOnCluster = false;
            RunOnNode = false;
            ScheduledForNow = false;
            ScheduledForLater = false;
            NoEndDate = false;
            EndBy = false;
            UsesUIAutomation = false;

        }

        internal void CheckScriptParamters(int scriptIndex)
        {
            ShowCategoryScriptView(scriptIndex);
            Models.Script s = null;
            ScheduledRequest = false;

            if (this.dgList.Rows.Count > scriptIndex)
            {
                s = this.dgList.Rows[scriptIndex].Tag as Models.Script;

                if (s != null)
                    RunScript(s, scriptIndex);
                else if (editScriptDetails != null && editScriptDetails.OpenedFromRepository == true)
                    RunScript(editScriptDetails._script, 0);
            }
        }

        internal void RunScript_1(Models.Script selectedscript, int scriptIndex)
        {
            networkParamsValidation = false;

            this.selectedScript = selectedscript;

            if (!string.IsNullOrEmpty(selectedscript.ScriptType))
            {
                // Check if AllowRemote option has been enabled
                allowRemote = CheckAllowRemoteOption(selectedscript.ScriptType);

                if (selectedscript.ScriptType.ToLower().Equals("ps1"))
                    networkParamsValidation = true;
            }
            else
            {
                allowRemote = CheckAllowRemoteOption(selectedscript.TaskType);
                networkParamsValidation = true;
            }

            if (selectedscript.Parameters != null && selectedscript.Parameters.Count > 0)
            {
                List<Models.ScriptParameter> pm = new List<Models.ScriptParameter>();
                var inputParameters = from p in selectedscript.Parameters
                                      where p.IOType.ToString().ToLower().Equals("in")
                                      select p;

                if (inputParameters.FirstOrDefault() != null)
                {
                    pm = inputParameters.ToList<Models.ScriptParameter>();
                    selectedScriptIndex = scriptIndex;
                    ScriptParameters frm = new ScriptParameters(pm);
                    frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);

                    frm.ShowDialog(this);
                }
                else
                {
                    RunScriptLocally(selectedscript);
                }
            }
            else
            {
                // If AllowRemote is enabled
                if (allowRemote)
                {
                    ScriptParameters frm = new ScriptParameters(null);
                    frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);
                    frm.ShowDialog(this);
                }
                else
                {
                    RunScriptLocally(selectedscript);
                }
            }

        }
        internal void RunScript(Models.Script selectedscript, int scriptIndex)
        {
            networkParamsValidation = false;

            List<Models.ScriptParameter> pm = new List<Models.ScriptParameter>();

            if (!string.IsNullOrEmpty(selectedscript.ScriptType))
            {
                // Check if AllowRemote option has been enabled
                allowRemote = CheckAllowRemoteOption(selectedscript.ScriptType);

                if (selectedscript.ScriptType.ToLower().Equals("vbs") || selectedscript.ScriptType.ToLower().Equals("js") || selectedscript.ScriptType.ToLower().Equals("bat") || selectedscript.ScriptType.ToLower().Equals("py"))
                    networkParamsValidation = true;
                showSSHForm = false;
                if (selectedscript.ScriptType.ToLower().Equals("sh"))
                    showSSHForm = true;
                
            }
            else if (!string.IsNullOrEmpty(selectedscript.TaskType))
            {
                if (selectedscript.TaskType.ToLower().Equals("sh command"))
                {
                    showSSHForm = true;
                    allowRemote = CheckAllowRemoteOption(selectedscript.TaskType);
                }
                else
                {
                    allowRemote = CheckAllowRemoteOption(selectedscript.TaskType);
                }
            }


            UsesUIAutomation = selectedscript.UsesUIAutomation;

            if (selectedscript.Parameters != null && selectedscript.Parameters.Count > 0)
            {

                var inputParameters = from p in selectedscript.Parameters
                                      where p.IOType.ToString().ToLower().Equals("in")
                                      select p;

                if (inputParameters.FirstOrDefault() != null)
                {
                    pm = inputParameters.ToList<Models.ScriptParameter>();
                    selectedScriptIndex = scriptIndex;
                    ScriptParameters frm = new ScriptParameters(pm);
                    frm.ScriptId = int.Parse(selectedscript.Id);
                    frm.ScriptName = selectedscript.Name;
                    frm.CategoryId = int.Parse(selectedscript.CategoryId);
                    frm.UsesUI = selectedscript.UsesUIAutomation;

                    frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);

                    frm.NodeExecuted += new ScriptParameters.NodeExecutedEventHandler(IapDisplayOutput);

                    frm.IapNodeExecuted_EventHandler += frm_IapNodeExecutedEventHandler;
                    frm.ShowDialog(this);

                    List<string> SelectedNodes = MainRepositoryView.SelectedNodes;

                    if (iapnodeexecuted && SelectedNodes != null)
                    {
                        outputScriptMap.Clear();
                        foreach (string s in SelectedNodes)
                        {
                            Guid Identifier = Guid.NewGuid();

                            outputScriptMap.Add(s, Identifier.ToString());
                            SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Node Execution Initiated....." + Environment.NewLine, s, 25);
                        }
                    }



                }
                else
                {
                    RunScriptLocally(selectedscript);
                }
            }
            // Check command line arguments
            else if (string.IsNullOrEmpty(selectedscript.ScriptType) && !string.IsNullOrEmpty(selectedscript.ArgumentString))
            {
                ScriptParameter p = new ScriptParameter();
                p.Name = "Command Arguments";
                p.DefaultValue = selectedscript.ArgumentString;
                p.IOType = ParameterIOTypes.In;
                p.IsSecret = false;
                p.IsUnnamed = true;
                //p.IsMandatory = true;
                pm.Add(p);
                selectedScriptIndex = scriptIndex;
                ScriptParameters frm = new ScriptParameters(pm);
                frm.ScriptId = int.Parse(selectedscript.Id);
                frm.ScriptName = selectedscript.Name;
                frm.CategoryId = int.Parse(selectedscript.CategoryId);
                frm.UsesUI = selectedscript.UsesUIAutomation;
                frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);
                frm.NodeExecuted += new ScriptParameters.NodeExecutedEventHandler(DisplayOutput);
                frm.ShowDialog(this);
            }

            else
            {
                //if (ScheduledRequest)
                //{
                //    ScriptParameters frm = new ScriptParameters(null);
                //    frm.ScriptId = int.Parse(selectedscript.Id);
                //    frm.ScriptName = selectedscript.Name;
                //    frm.CategoryId = int.Parse(selectedscript.CategoryId);
                //    frm.UsesUI = selectedscript.UsesUIAutomation;
                //    frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);
                //    frm.NodeExecuted += new ScriptParameters.NodeExecutedEventHandler(DisplayOutput);
                //    frm.ShowDialog(this);
                //}
                // If AllowRemote is enabled
                if (allowRemote)
                {
                    ScriptParameters frm = new ScriptParameters(null);
                    frm.ScriptId = int.Parse(selectedscript.Id);
                    frm.ScriptName = selectedscript.Name;
                    frm.CategoryId = int.Parse(selectedscript.CategoryId);
                    frm.UsesUI = selectedscript.UsesUIAutomation;
                    frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);
                    frm.NodeExecuted += new ScriptParameters.NodeExecutedEventHandler(DisplayOutput);
                    frm.ShowDialog(this);
                }
                else
                {
                    RunScriptLocally(selectedscript);
                }
            }
            iapnodeexecuted = false;
        }

        private void frm_IapNodeExecutedEventHandler(bool iapnodeexecuted)
        {
            this.iapnodeexecuted = iapnodeexecuted;
        }

        /// <summary>
        /// This method is used to check if the allowRemote attribute is set to true for a particular script type.
        /// </summary>
        /// <param name="scriptType">Type of script e.g. vbs, ps1 etc.</param>
        /// <returns>ture or false</returns>
        private bool CheckAllowRemoteOption(string scriptType)
        {
            bool allowRemote = false;
            XElement root = XElement.Load(@"XML\ScriptType.xml");
            var objType = from scripttype in root.Elements("Type")
                          where scripttype.Attribute("key").Value.ToLower().Equals(scriptType.ToLower())
                          select scripttype.Attribute("allowremote").Value;
            if (objType.FirstOrDefault() != null)
                allowRemote = Convert.ToBoolean(objType.FirstOrDefault().ToString());

            return allowRemote;
        }
        /// <summary>
        /// This method gets file extension without dot.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>file extension without dot</returns>
        private string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName).Replace(".", "");
        }

        /// <summary>
        /// This method is used to run script.
        /// </summary>
        /// <param name="selectedscript">script to run</param>
        internal async void RunScriptLocally(Models.Script selectedscript)
        {
            ScriptIndentifier scriptIden = new ScriptIndentifier();
            List<Parameter> param = new List<Parameter>();

            scriptIden.ScriptId = Convert.ToInt32(selectedscript.Id);
            scriptIden.SubCategoryId = Convert.ToInt32(selectedscript.CategoryId);

            if (ScheduledRequest)
            {
                ScheduleScript(selectedscript);
            }
            else
            {
                int count = 0;
                if (selectedscript.Parameters != null && selectedscript.Parameters.Count > 0)
                {
                    var inputParameters = from p in selectedscript.Parameters
                                          where p.IOType.ToString().ToLower().Equals("in")
                                          select p;
                    if (inputParameters.FirstOrDefault() != null)
                    {
                        foreach (Models.ScriptParameter p in selectedscript.Parameters)
                        {
                            if (p.IOType.ToString().ToLower().Equals("in"))
                            {
                                Parameter paramsInputValue = new Parameter();
                                paramsInputValue.ParameterName = selectedscript.Parameters[count].Name;
                                paramsInputValue.ParameterValue = (selectedscript.Parameters[count].IsSecret) ? SecurePayload.Secure(parametersCollection[count], "IAP2GO_SEC!URE") : parametersCollection[count]; 
                                paramsInputValue.IsPaired = selectedscript.Parameters[count].IsUnnamed;
                                paramsInputValue.allowedValues = selectedscript.Parameters[count].AllowedValues;
                                param.Add(paramsInputValue);
                                count = count + 1;
                            }
                        }
                        scriptIden.Parameters = param;
                    }
                }
                else if (string.IsNullOrEmpty(selectedscript.ScriptType) && !string.IsNullOrEmpty(selectedscript.ArgumentString))
                {
                    Regex rgx = new Regex(@"[^\s""]+|""[^""]*""", RegexOptions.IgnoreCase);
                    MatchCollection matches = rgx.Matches(parametersCollection[0]);
                    if (matches.Count > 0)
                    {
                        Parameter paramsInputValue = null;
                        //foreach (string arg in args)
                        foreach (Match match in matches)
                        {
                            paramsInputValue = new Parameter();
                            string value = match.Value;
                            if (value.Contains("\""))
                                value = value.Replace("\"", "");

                            paramsInputValue.ParameterValue = value;
                            // paramsInputValue.ParameterName = "Command";

                            param.Add(paramsInputValue);

                            scriptIden.Parameters = param;
                            //scriptIden.Parameters.Add(new Parameter() { ParameterValue = value ,IsPaired=false,IsSecret=false,ParameterName="Command"});
                        }
                    }

                }

                if (allowRemote)
                {
                    //Add Remote parameters
                    scriptIden.RemoteExecutionMode = ScriptIndentifier.RemoteExecutionHost.PS;
                    scriptIden.RemoteServerNames = remoteServerName;
                    scriptIden.UserName = MainRepositoryView.UserName;
                    scriptIden.Password = MainRepositoryView.Password;
                }
                if (showSSHForm)
                {
                    //Add Remote parameters
                    scriptIden.RemoteExecutionMode = ScriptIndentifier.RemoteExecutionHost.Linux;
                    scriptIden.RemoteServerNames = LinuxServerName;
                    scriptIden.UserName = MainRepositoryView.UserName;
                    scriptIden.Password = MainRepositoryView.Password;
                    
                    scriptIden.LinuxKeyPath = MainRepositoryView.LinuxKeyPath;
                    
                    if (scriptIden.RemoteServerNames.Contains(','))
                    {
                        outputScriptMap.Clear();
                        string[] servernames = scriptIden.RemoteServerNames.Split(',');
                        foreach (string server in servernames)
                        {
                            Guid Identifier = Guid.NewGuid();

                            outputScriptMap.Add(server, Identifier.ToString());
                            SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Execution Initiated....." + Environment.NewLine, server, 20);
                        }
                    }
                    else
                    {
                        scriptIden.TransactionId = Guid.NewGuid();
                        SendStatusUpdate(scriptIden.TransactionId, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Execution Initiated....." + Environment.NewLine, scriptIden.RemoteServerNames, 20);
                    }

                }

                if (scriptIden.ScriptId > 0 && scriptIden.SubCategoryId > 0)
                {
                    if (!allowRemote)
                    {
                        if (scriptIden.TransactionId == Guid.Empty)
                        {
                            scriptIden.TransactionId = Guid.NewGuid();
                        }
                        ScriptExecutionManager.SendExecutionStatus -= null;
                        ScriptExecutionManager.SendExecutionStatus -= new ScriptExecutionManager.SendExecutionStatusEventHandler(ScriptExecutionManager_SendExecutionStatus);
                        ScriptExecutionManager.SendExecutionStatus += null;
                        ScriptExecutionManager.SendExecutionStatus += new ScriptExecutionManager.SendExecutionStatusEventHandler(ScriptExecutionManager_SendExecutionStatus);
                    }
                    else
                    {
                        if (scriptIden.RemoteServerNames.Length != 0)
                        {
                            outputScriptMap.Clear();
                            string[] RemoteServerNames = scriptIden.RemoteServerNames.Split(',');

                            foreach (string s in RemoteServerNames)
                            {
                                scriptIden.TransactionId = Guid.NewGuid();

                                outputScriptMap.Add(s, scriptIden.TransactionId.ToString());
                                SendStatusUpdate(scriptIden.TransactionId, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Execution Initiated....." + Environment.NewLine, s, 20);
                            }
                        }

                    }

                    List<ExecutionResult> result = await Task<List<ExecutionResult>>.Run(() =>
                    {

                        List<ExecutionResult> scriptExecResult = ScriptExecutionManager.Execute(scriptIden);
                        return scriptExecResult;

                    });


                    List<ExecutionResultView> executionResultView = new List<ExecutionResultView>();
                    ExecutionResultView view = null;
                    ScriptExecutionManager.SendExecutionStatusArgs args = new ScriptExecutionManager.SendExecutionStatusArgs();
                    if ((allowRemote) || (!string.IsNullOrEmpty(LinuxServerName)) ||showSSHForm)
                    {
                        if (scriptIden.RemoteServerNames.Length > 0)
                        {
                            string[] RemoteServerNames = scriptIden.RemoteServerNames.Split(',');
                            foreach (string s in RemoteServerNames)
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    if (outputScriptMap.ContainsKey(s))
                                    {
                                        args.Identifier = new Guid(outputScriptMap[s]);
                                        scriptIden.TransactionId = args.Identifier;
                                    }

                                }
                                SendStatusUpdate(scriptIden.TransactionId, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Response Received....." + Environment.NewLine, s, 95);
                                SendStatusUpdate(scriptIden.TransactionId, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Execution Completed....." + Environment.NewLine, s, 99);
                            }
                        }

                        foreach (ExecutionResult r in result)
                        {
                            /*
                            view = new ExecutionResultView();
                            view.SuccessMessage = r.InputCommand + Environment.NewLine +  Environment.NewLine + r.SuccessMessage;
                            view.ErrorMessage = r.InputCommand + Environment.NewLine + Environment.NewLine + r.ErrorMessage;
                            view.IsSuccess = r.IsSuccess;
                            view.ServerName = r.ComputerName;
                           */
                            if (!string.IsNullOrEmpty(r.ComputerName))
                            {
                                if (outputScriptMap.ContainsKey(r.ComputerName))
                                {
                                    args.Identifier = new Guid(outputScriptMap[r.ComputerName]);
                                    scriptIden.TransactionId = args.Identifier;
                                }

                            }
                            else
                            {
                                args.Identifier = scriptIden.TransactionId;
                            }
                            args.ServerName = r.ComputerName;
                            args.IsSuccess = r.IsSuccess;
                            if (scriptIden.TransactionId == Guid.Empty)
                            { args.Identifier = r.TransactionId; }
                            else { args.Identifier = scriptIden.TransactionId; }
                            args.StatusMessage = r.IsSuccess ? r.InputCommand + Environment.NewLine + Environment.NewLine + r.SuccessMessage :
                                r.InputCommand + Environment.NewLine + Environment.NewLine + r.ErrorMessage;
                            args.data=r.IsSuccess ? r.SuccessMessage + Environment.NewLine:"";
                            args.ScriptID = selectedscript.Id;
                            args.PercentComplete = 100;

                            ScriptExecutionManager_SendExecutionStatus(args);
                            //ScriptExecutionManager_SendExecutionStatus(args);
                            //executionResultView.Add(view);
                        }
                        //DisplayOutput(executionResultView);
                    }
                    else
                    {
                        foreach (ExecutionResult r in result)
                        {
                            /*
                            view = new ExecutionResultView();
                            view.SuccessMessage = r.InputCommand + Environment.NewLine + Environment.NewLine + r.SuccessMessage;
                            view.ErrorMessage = r.InputCommand + Environment.NewLine + Environment.NewLine + r.ErrorMessage;
                            view.IsSuccess = r.IsSuccess;
                            view.ServerName = r.ComputerName;
                            //executionResultView.Add(view);
                            */
                            args.ServerName = r.ComputerName;
                            args.IsSuccess = r.IsSuccess;
                            args.ScriptID = selectedscript.Id;
                            if (scriptIden.TransactionId == Guid.Empty)
                            { args.Identifier = r.TransactionId; }
                            else { args.Identifier = scriptIden.TransactionId; }
                            args.PercentComplete = 100;
                            args.data = r.IsSuccess ? r.SuccessMessage + Environment.NewLine : "";
                            args.StatusMessage = r.IsSuccess ? r.SuccessMessage : r.InputCommand + Environment.NewLine + Environment.NewLine + r.ErrorMessage;

                        }
                        ScriptExecutionManager_SendExecutionStatus(args);
                    }


                    MessageBox.Show("Script has been executed. Please Click OK button to see the output.", SCRIPTRESULT);
                    // DisplayOutput(executionResultView);

                }
                else
                {
                    MessageBox.Show("Invalid Script", SCRIPTRESULT);
                }
            }
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
            view.Identifier = Identifier;
            view.IsScript = true;
            // string scriptID = e.ScriptID;

            AppendOutputViewArgs appendArgs = new AppendOutputViewArgs();
            // appendArgs.scriptID = scriptID;
            appendArgs.Identifier = Identifier;
            appendArgs.progress = PercentComplete;
            appendArgs.executionResultView = view;

            AppendOutput(appendArgs);
        }
        private void ScriptExecutionManager_SendExecutionStatus(ScriptExecutionManager.SendExecutionStatusArgs e)
        {
            ExecutionResultView view = null;
            view = new ExecutionResultView();

            view.IsSuccess = e.IsSuccess;

            if (e.IsSuccess)
                view.SuccessMessage = e.StatusMessage;
            else
                view.ErrorMessage = e.StatusMessage;
            //view.ServerName = Environment.MachineName;

            view.ServerName = string.IsNullOrEmpty(e.ServerName) ? Environment.MachineName : e.ServerName;

            view.IsScript = true;
            view.data = e.data;
            string scriptID = e.ScriptID;

            AppendOutputViewArgs appendArgs = new AppendOutputViewArgs();
            appendArgs.scriptID = scriptID;
            appendArgs.Identifier = e.Identifier;
            appendArgs.progress = e.PercentComplete;
            appendArgs.executionResultView = view;

            AppendOutput(appendArgs);
        }

        public void AppendOutput(AppendOutputViewArgs appendArgs)
        {
            AppendOutputView(this, appendArgs);
        }

        public void DisplayOutput(List<ExecutionResultView> executionResultView)
        {
            //if (AppendOutputView == null)

            foreach (var result in executionResultView)
            {
                if (outputScriptMap != null && outputScriptMap.Count!=0)
                {
                    if (outputScriptMap.ContainsKey(result.ServerName))
                    {
                        Guid Identifier = new Guid(outputScriptMap[result.ServerName]);
                        result.Identifier = Identifier;
                        result.IsScript = true;
                        if (iapnodeexecuted)
                        {
                            SendStatusUpdate(Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Node Execution Completed...." + Environment.NewLine, result.ServerName, 99);
                            iAPNode = false;
                        }
                        result.Progress = 100;

                    }
                }
            }
            ShowOutputView(this, new EventArgs<List<ExecutionResultView>>(executionResultView));
        }

        private void DisplayScriptExecutionNotification(string scriptName)
        {
            notifyIcon1.Icon = new System.Drawing.Icon(System.IO.Path.GetFullPath(@"D:\POC\POC\POC\Images\Notification.ico"));
            notifyIcon1.Text = "Script Execution";
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipTitle = "Script Execution";
            notifyIcon1.BalloonTipText = "Script " + scriptName + " is currently executing. Please wait...Click here close notification";
            notifyIcon1.ShowBalloonTip(1000);
        }

        private void tvCatSubcat_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (splitContainerDetails.Panel2.Controls.Count > 0)
            {
                for (int i = 0; i < splitContainerDetails.Panel2.Controls.Count; i++)
                {
                    splitContainerDetails.Panel2.Controls.RemoveAt(i);
                }
            }

            // The category id will be used to display correct category in the dropdown while adding new script
            selectedCatID = "";
            Models.Category c = this.tvCatSubcat.SelectedNode.Tag as Models.Category;
            if (c != null)
            {
                selectedCatID = c.Id;
                this.enableControls = true;
            }
            else
                this.enableControls = false;
            dgList.Enabled = true;

            if (c != null && !String.IsNullOrEmpty(c.Id))
            {
                if (!isSuperAdmin)
                {
                    var user = Users.FirstOrDefault(u => u.CategoryId == Convert.ToInt32(c.Id));
                    if (user != null)
                    {
                        CurentUser = user;
                        if (user.Role == Roles.Guest.ToString() || user.Role == Roles.Agent.ToString())
                        {
                            //dgList.Enabled = false;
                            //CurentUser = user; 
                            this.enableControls = false;
                        }
                    }
                }
            }

            BindCategory();

            if (editScriptDetails != null && editScriptDetails.OpenedFromRepository != true)
            {
                ShowRun(this, new EventArgs<bool>(this.enableControls));
            }
            else if (CurentUser != null)
            {
                if (CurentUser.Role == Roles.Guest.ToString())
                {
                    ShowRun(this, new EventArgs<bool>(this.enableControls));
                }
            }
        }

        private void BindCategory()
        {

            List<Tuple<Image, Image, Image, Image, string, string, int, Tuple<List<ScriptParams.ScriptParam>,
                string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>> _categories = null;


            if (tvCatSubcat.SelectedNode.Text == "Categories")
            {
                _categories = this.categories.Where(c => c.ParentId == 0).
                      Select(c1 => new Tuple<Image, Image, Image, Image, string, string, int, Tuple<List<ScriptParams.ScriptParam>,
                        string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>
                        (null, null, null, Image.FromFile(@"Images\folder.png"), c1.Name, c1.Description, c1.ParentId,
                    new Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>(null,
                        string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>(string.Empty, c1.Id, false, null, null, null, null)))).ToList();

            }
            else
            {
                Models.Category c = this.tvCatSubcat.SelectedNode.Tag as Models.Category;

                _categories = this.categories.Where(c1 => c1.ParentId == Convert.ToInt32(c.Id)).
                 Select(c1 => new Tuple<Image, Image, Image, Image, string, string, int, Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>(
                    null, null, null, Image.FromFile(@"Images\folder.png"), c1.Name, c1.Description, c1.ParentId,
                    new Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>
                        (null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>(string.Empty, c1.Id, false, null, null, null, null)))).ToList();

                var scripts = scriptClient.ServiceChannel.GetAllScriptDetails(c.Id);

                if (scripts.Scripts != null && scripts.Scripts.Count > 0)
                {

                    var t = scripts.Scripts.
                     Select(s => new Tuple<Image, Image, Image, Image, string, string, int, Tuple<List<ScriptParams.ScriptParam>,
                               string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>(
                         Image.FromFile(@"Images\remove.png"), Image.FromFile(@"Images\play.png"), null, Image.FromFile(@"Images\script.png"),
                         s.Name, s.Description, Convert.ToInt32(c.Id),
                        new Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>
                            (s.Parameters, s.ScriptURL, s.TaskCmd, s.ArgString, s.WorkingDir, s.RunAsAdmin.ToString(), s.TaskType, 
                            new Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>
                                (s.ScriptType, s.ScriptId.ToString(), s.UsesUIAutomation, s.IfeaScriptName, 
                                //new string[] { s.ScriptFileVersion.ToString(), "", "", s.CallMethod }, null, null)))).ToList();
                                new string[] { s.ScriptFileVersion.ToString(), s.CreatedBy, s.ModifiedBy, s.CallMethod,s.Tags,s.LicenseType,s.SourceUrl}, s.CreatedOn, s.ModifiedOn)))).ToList();

                    
                    _categories.AddRange(t);
                }

            }

            dgList.DataSource = _categories;
            //dgList.Columns[0].Width = 25;
            //dgList.Columns[1].Width = 25;
            //dgList.Columns[2].Width = 25;
            dgList.Columns[3].Width = 30;
            dgList.Columns[4].Width = 250;
            dgList.Columns[5].Width = 374;
            //dgList.Columns[0].HeaderText = "";
            //dgList.Columns[1].HeaderText = "";
            //dgList.Columns[2].HeaderText = "";
            dgList.Columns[3].HeaderText = "";
            dgList.Columns[4].HeaderText = "Name";
            dgList.Columns[5].HeaderText = "Description";
            dgList.Columns[6].Visible = dgList.Columns[7].Visible = false;
            dgList.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //this.dgList.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //this.dgList.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.dgList.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //this.dgList.Columns[0].DefaultCellStyle.NullValue = null;
            //this.dgList.Columns[1].DefaultCellStyle.NullValue = null;
            //this.dgList.Columns[2].DefaultCellStyle.NullValue = null;

            dgList.Columns[0].Visible = false;
            dgList.Columns[1].Visible = false;
            dgList.Columns[2].Visible = false;
          

            ShowCategoryScriptView(0);
           // DisplayImages();
        }

        /// <summary>
        /// This method is used to display delete and run buttons in the grid.
        /// </summary>
        private void DisplayImages()
        {
            foreach (DataGridViewRow row in this.dgList.Rows)
            {
                Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>> tupleScriptDetails =
                    (Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>)row.Cells[7].Value;
                // If task type is null
                if (!string.IsNullOrEmpty(tupleScriptDetails.Item7))
                {

                    this.dgList.Columns[0].Visible = true;
                    this.dgList.Columns[1].Visible = true;
                    this.dgList.Columns[2].Visible = true;

                }
                else
                {
                    this.dgList.Columns[0].Visible = false;
                    this.dgList.Columns[1].Visible = false;
                    this.dgList.Columns[2].Visible = false;
                }

                if (CurentUser != null)
                {
                    if (CurentUser.Role == Roles.Guest.ToString() || CurentUser.Role == Roles.Agent.ToString()) 
                    {
                        this.dgList.Columns[0].Visible = false;
                        this.dgList.Columns[1].Visible = false;
                        this.dgList.Columns[2].Visible = false;
                    }
                }
            }
        }


        private List<Models.CategorySubCategorySubset> GetCategorySubset(List<Models.Category> categories)
        {
            List<Models.CategorySubCategorySubset> objs = new List<Models.CategorySubCategorySubset>();
            categories.ForEach(c =>
            {
                objs.Add(new Models.CategorySubCategorySubset() { Name = c.Name, Description = c.Description });
            });
            return objs;
        }

        private List<Models.CategorySubCategorySubset> GetSubCategorySubset(List<Models.SubCategory> subCategories)
        {
            List<Models.CategorySubCategorySubset> objs = new List<Models.CategorySubCategorySubset>();
            subCategories.ForEach(c =>
            {
                objs.Add(new Models.CategorySubCategorySubset() { Name = c.Name, Description = c.Description });
            });
            return objs;
        }

        private List<Models.ScriptSubSet> GetScriptSubset(List<Models.Script> scripts)
        {
            List<Models.ScriptSubSet> objs = new List<Models.ScriptSubSet>();
            scripts.ForEach(c =>
            {
                objs.Add(new Models.ScriptSubSet() { Name = c.Name, Description = c.Description, ScriptType = c.ScriptType });
            });
            return objs;
        }


        private void ShowCategoryDetails(int index)
        {
            List<Models.Category> categories = tvCatSubcat.SelectedNode.Tag as List<Models.Category>;
            if (index != -1)
            {
                // Models.Category catSelected = categories[index];
                var category = this.dgList.Rows[index].DataBoundItem as
                    Tuple<Image, Image, Image, Image, string, string, int,
                    Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>;
                Views.CategoryDetails catdetails = new CategoryDetails(new Models.Category { Name = category.Item5, Description = category.Item6 });
                catdetails.CategoryProcessed += new CategoryDetails.CategoryProcessedEventHandler(cat_CategoryProcessed);
                catdetails.Dock = DockStyle.Top;
                splitContainerDetails.Panel2.Controls.Add(catdetails);
                dgList.Columns[3].Visible = true;
            }
        }

        private void dgList_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //return if the top let header is clicked
            if (e.ColumnIndex == 0 && e.RowIndex == -1)
                return;
            DataGridViewColumn columnClicked = dgList.Columns[e.ColumnIndex];
            if (dgList.Rows.Count > 0 && dgList.DataSource != null && e.ColumnIndex ==4)
            {
                var dataSource = dgList.DataSource as List<Tuple<Image, Image, Image, Image, string, string, int, Tuple<List<ScriptParams.ScriptParam>,
                     string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>>;

                if (!asc)
                {
                    asc = true;
                    var ascendingOrder = dataSource.OrderBy(t => t.Item5).ToList();
                    dgList.DataSource = ascendingOrder;
                }
                else
                {
                    asc = false;
                    var descendingOrder = dataSource.OrderByDescending(t => t.Item5).ToList();
                    dgList.DataSource = descendingOrder;
                }

                ShowCategoryScriptView(0);
            }
        }

        void cat_CategoryProcessed(CategoryDetails.CategoryProcessedArgs e)
        {
            LoadCategory();
        }

        void subcat_SubCategoryProcessed(SubCategoryDetails.SubCategoryProcessedArgs e)
        {
            LoadCategory();
            //then expand the parent category
            ExpandCategory(e.ParentCategoryId);
        }

        void script_ScriptProcessed(ScriptDetails.ScriptProcessedArgs e)
        {
            LoadCategory();
            BindCategory();
            ExpandSubCategory(e.SubCategoryId, e.CategoryId, e.ScriptId);
        }
        void script_ScriptUpdate(ScriptDetails.ScriptStatusUpdateArgs e)
        {
            outputScriptMap = e.outputScriptMap;
            SendStatusUpdate(e.Identifier, e.StatusMessge, e.ServerName, e.PercentComplete);
        }


        private void DeleteScript(int index)
        {
            var script = this.dgList.Rows[index].DataBoundItem as Tuple<Image, Image, Image, Image, string, string, int,
                Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>;
            Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>> tupleScriptDetails =
                (Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>)this.dgList.Rows[index].Cells[7].Value;
            Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>> tupleScriptId = tupleScriptDetails.Rest;
            DeleteScriptReqMsg req = new DeleteScriptReqMsg() { ScriptId = Convert.ToInt32(tupleScriptId.Item2), CategoryId = script.Item7, ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name };
            scriptClient.ServiceChannel.DeleteScript(req);
        }

        private void ExpandCategory(string id)
        {
            tvCatSubcat.TopNode.Expand();
            foreach (TreeNode node in tvCatSubcat.TopNode.Nodes)
            {
                if ((node.Tag as Models.Category).Id == id)
                {
                    node.Expand();
                    break;
                }
            }
            dgList.Columns[0].Visible = false;
            dgList.Columns[1].Visible = false;
            dgList.Columns[2].Visible = false;
        }

        private void ExpandSubCategory(string subcategoryId, string categoryId, string scriptId = "")
        {

            TreeNode cat = null, subcat = null;
            foreach (TreeNode node in tvCatSubcat.TopNode.Nodes)
            {
                if ((node.Tag as Models.Category).Id == categoryId)
                {
                    node.Expand();
                    cat = node;
                    break;
                }
            }
            if (cat != null)
            {
                foreach (TreeNode node in cat.Nodes)
                {
                    //if ((node.Tag as Models.SubCategory).Id == subcategoryId)
                    //{
                    //    node.Nodes.Clear();
                    //    node.Expand();
                    //    subcat = node;
                    //    subCategory = node.Tag as Models.SubCategory; ;
                    //    break;
                    //}
                }
            }
            if (subcat != null)
            {
                //currentNode = TreeNodeContext.Script;
                FetchScripts(subcat);
                tvCatSubcat.SelectedNode = subcat;
            }
            //else
            //{
            //    FetchScripts(cat);
            //    tvCatSubcat.SelectedNode = cat;
            //}
        }

        private void FetchScripts(TreeNode subcategory)
        {
            Models.SubCategory subCat = null;
            subCat = subcategory.Tag as Models.SubCategory;
            GetAllScriptDetailsResMsg response = scriptClient.ServiceChannel.GetAllScriptDetails(subCat.Id);
            if (response.Scripts != null)
            {
                List<Models.Script> scripts = Translators.ScriptPE_SE.ScriptListSEtoPE(response.Scripts.ToList());
                //assign the scripts fetched to the node tag having the sub-category details
                if (scripts != null && scripts.Count > 0)
                {
                    (subcategory.Tag as Models.SubCategory).Scripts = scripts;

                    //then populate grid
                    dgList.DataSource = GetScriptSubset(scripts);
                    //select the first row if any
                    if (dgList.Rows.Count > 0)
                    {
                        dgList.Rows[0].Selected = true;
                        dgList_CellMouseClick(new object(), new DataGridViewCellMouseEventArgs(2, 0, 0, 0, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0)));
                        // TO DO                    
                        dgList.Columns[1].Visible = true;
                    }
                    else
                    {
                        dgList.DataSource = null;
                        dgList.Columns[0].Visible = false;
                        dgList.Columns[1].Visible = false;
                        dgList.Columns[2].Visible = false;
                    }
                }
            }
            else
            {
                subcategory.Nodes.Clear();
                dgList.DataSource = null;
                dgList.RowHeadersVisible = false;
            }
        }

        private void newCategoryMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewCategory();
        }

        internal void CreateNewCategory()
        {
            this.Cursor = Cursors.WaitCursor;
            splitContainerDetails.Panel2.Controls.Clear();
            Views.CategoryDetails cat = new CategoryDetails(null, true);
            cat.CategoryProcessed += new CategoryDetails.CategoryProcessedEventHandler(cat_CategoryProcessed);
            cat.Dock = DockStyle.Top;
            splitContainerDetails.Panel2.Controls.Add(cat);
            this.Cursor = Cursors.Default;
        }

        private void newSubCategoryMenuItem_Click(object sender, EventArgs e)
        {
            //CreateNewSubCategory();
        }

        private void newScriptMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewScript();
        }

        internal void CreateNewScript()
        {

            this.Cursor = Cursors.WaitCursor;
            splitContainerDetails.Panel2.Controls.Clear();
            Views.ScriptDetails script = new ScriptDetails(null, _categorySelected, true);
            script.ScriptProcessed += new ScriptDetails.ScriptProcessedEventHandler(script_ScriptProcessed);
            script.EditScript += script_EditScript;
            script.Dock = DockStyle.Top;
            splitContainerDetails.Panel2.Controls.Add(script);
            this.Cursor = Cursors.Default;
            if (this.dgList.SelectedRows.Count > 0)
                this._presenter.ActivateMenuHandler(IsScript(this.dgList.SelectedRows[0].Index));

        }

        void script_EditScript(object[] content)
        {
            EditScript(new object(), new EventArgs<Object[]>(new object[] { content[0], content[1] }));
        }

        internal void runScriptLocallyMenuItem_Click(object sender, EventArgs e)
        {
            CheckScriptParamters(selectedScriptIndex);
        }

        private void toolbarNewScript_Click(object sender, EventArgs e)
        {
            CreateNewScript();
        }

        internal void toolbarRunScript_Click(object sender, EventArgs e)
        {
            CheckScriptParamters(selectedScriptIndex);
        }

        private void toolbarNewCategory_Click(object sender, EventArgs e)
        {
            CreateNewCategory();
        }

        private void toolbarNewSubCategory_Click(object sender, EventArgs e)
        {
            //CreateNewSubCategory();
        }

        private void dgList__CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (IsScript(e.RowIndex))
            {
                if (e.ColumnIndex == 0)
                {
                    e.ToolTipText = "Delete";
                }
                if (e.ColumnIndex == 1)
                {
                    e.ToolTipText = "Run Script";
                }
                if (e.ColumnIndex == 2)
                {
                    e.ToolTipText = "Edit Script";
                }
            }
        }

        private void toolbarNewCategory_Click_1(object sender, EventArgs e)
        {
            //CreateNewCategory();
        }

        private void toolbarNewSubCategory_Click_1(object sender, EventArgs e)
        {
            //CreateNewSubCategory();
        }

        private void dgList_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (IsScript(e.RowIndex))
            {
                if (e.ColumnIndex == 1)
                {
                    dgList.Cursor = Cursors.Hand;
                }
            }
            //else if (e.ColumnIndex == 1)
            //{
            //    dgList.Cursor = Cursors.Hand;
            //}

        }

        private void dgList_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgList.Cursor = Cursors.Arrow;

        }
        private void dgList_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            ShowCategoryScriptView(0);
        }

        private bool IsScript(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                var dataBound = this.dgList.Rows[rowIndex].DataBoundItem as
                 Tuple<Image, Image, Image, Image, string, string, int, Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>;

                if (dataBound.Rest.Item1 != null)
                    return true;

            }
            return false;
        }

        private void RemoveCtrls()
        {
            var container = splitContainerDetails.Panel2.Controls;

            for (int i = 0; i < container.Count; i++)
            {
                container.RemoveAt(i);
            }
        }

        private void ShowCategoryScriptView(int rowIndex)
        {
            if (this.dgList.CurrentRow != null)
            {
                var category = this.dgList.Rows[rowIndex].DataBoundItem as Tuple<Image, Image, Image, Image, string, string, int,
                    Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>;

                Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>> tupleScriptDetails =
                    (Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>)this.dgList.Rows[rowIndex].Cells[7].Value;

                //Tuple<string> tupleCatName = tupleScriptDetails.Rest;

                Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>> tupleCatName = tupleScriptDetails.Rest;

                RemoveCtrls();

                // If task type is null
                if (string.IsNullOrEmpty(tupleScriptDetails.Item7))
                {
                    Views.CategoryDetails catdetails = new CategoryDetails(new Models.Category { Name = category.Item5, Description = category.Item6 });
                    catdetails.Dock = DockStyle.Top;
                    splitContainerDetails.Panel2.Controls.Add(catdetails);
                    this._presenter.ActivateMenuHandler(false);
                    currentNode = TreeNodeContext.Category;
                }
                else
                {
                    DataGridViewRow selectedRow = this.dgList.Rows[rowIndex];
                    //Tuple<string, string, string, string, string, string, string, List<ScriptParams.ScriptParam>> tupleScriptDetails = (Tuple<string, string, string, string, string, string, string, List<ScriptParams.ScriptParam>>)selectedRow.Cells[7].Value;

                    //   if (tupleScriptDetails != null)
                    {
                        Models.Script s = new Models.Script
                        {
                            Name = category.Item5,
                            Description = category.Item6,
                            Id = tupleCatName.Item2,
                            CategoryId = category.Item7.ToString(),
                            ScriptLocation = tupleScriptDetails.Item2,
                            ScriptType = tupleScriptDetails.Rest.Item1,
                            TaskCommand = tupleScriptDetails.Item3,
                            TaskType = tupleScriptDetails.Item7,
                            ArgumentString = tupleScriptDetails.Item4,
                            WorkingDir = tupleScriptDetails.Item5,
                            RunAsAdmin = Convert.ToBoolean(tupleScriptDetails.Item6),
                            UsesUIAutomation = tupleScriptDetails.Rest.Item3,
                            IfeaScriptName = tupleScriptDetails.Rest.Item4,
                            Version = tupleScriptDetails.Rest.Item5[0],
                            CreatedBy = tupleScriptDetails.Rest.Item5[1],
                            ModifiedBy = tupleScriptDetails.Rest.Item5[2],
                            CallMethod = tupleScriptDetails.Rest.Item5[3],
                            Tags = tupleScriptDetails.Rest.Item5[4],
                            LicenseType = tupleScriptDetails.Rest.Item5[5],
                            SourceUrl = tupleScriptDetails.Rest.Item5[6],                           
                            CreatedOn = Convert.ToDateTime(tupleScriptDetails.Rest.Item6),
                            ModifiedOn = Convert.ToDateTime(tupleScriptDetails.Rest.Item7),
                            OpenedFromRepository = true,
                            Parameters = GetParameters(category)
                        
                            //Parameters = category.Rest.Item1.Select(m => new Models.ScriptParameter
                            //{
                            //    AllowedValues = m.AllowedValues,
                            //    CreatedBy = m.CreatedBy,
                            //    DefaultValue = (m.IsSecret) ? SecurePayload.UnSecure(m.DefaultValue,"IAP2GO_SEC!URE"): m.DefaultValue,
                            //    IOType = (Models.ParameterIOTypes)Enum.Parse(typeof(Models.ParameterIOTypes), m.ParamType.ToString()),
                            //    IsMandatory = m.IsMandatory,
                            //    IsSecret = m.IsSecret,
                            //    IsUnnamed = m.IsUnnamed,
                            //    ModifiedBy = m.ModifiedBy,
                            //    Name = m.Name,
                            //    ParamId = m.ParamId.ToString(),
                            //    ScriptId = m.ScriptId.ToString(),
                            //    DataType = m.DataType, 
                            //    IsReferenceKey=m.IsReferenceKey
                                 
                            //}).ToList(),

                        };
                        this.dgList.CurrentRow.Tag = s;
                        editScriptDetails = new ScriptDetails(s, tupleCatName.Item2, false);
                        //editScriptDetails.ShowControls(this.enableControls);
                        editScriptDetails.OpenedFromRepository = true;
                        editScriptDetails.EditScript += script_EditScript;
             
                        editScriptDetails.ScriptProcessed += new ScriptDetails.ScriptProcessedEventHandler(script_ScriptProcessed);
                        editScriptDetails.ScriptStatus += new ScriptDetails.ScriptStatusUpdateArgsEventHandler(script_ScriptUpdate);
                        editScriptDetails.ScriptExecuted += new ScriptDetails.ScriptExecutedEventHandler(DisplayOutput);
                        editScriptDetails.IAPNodeExecuted += new ScriptDetails.IAPNodeExecutedEventHandler(IapDisplayOutput);
                        editScriptDetails.Dock = DockStyle.Top;
                        editScriptDetails.MainRepoView = this;
                        splitContainerDetails.Panel2.Controls.Add(editScriptDetails);
                        this._presenter.ActivateMenuHandler(true);

                        if (CurentUser != null)
                        {
                            if (CurentUser.Role == Roles.Guest.ToString())
                                ShowRun(this, new EventArgs<bool>(this.enableControls));
                        }
                    }
                }
            }
        }

        private List<ScriptParameter> GetParameters(Tuple<Image, Image, Image, Image, string, string, int,
                    Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, 
            Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>>
            category)
        {
            if (category.Rest.Item1 != null && category.Rest.Item1.Count > 0)
            {
                return category.Rest.Item1.Select(m => new Models.ScriptParameter
                {
                    AllowedValues = m.AllowedValues,
                    CreatedBy = m.CreatedBy,
                    DefaultValue = (m.IsSecret) ? SecurePayload.UnSecure(m.DefaultValue, "IAP2GO_SEC!URE") : m.DefaultValue,
                    IOType = (Models.ParameterIOTypes)Enum.Parse(typeof(Models.ParameterIOTypes), m.ParamType.ToString()),
                    IsMandatory = m.IsMandatory,
                    IsSecret = m.IsSecret,
                    IsUnnamed = m.IsUnnamed,
                    ModifiedBy = m.ModifiedBy,
                    Name = m.Name,
                    ParamId = m.ParamId.ToString(),
                    ScriptId = m.ScriptId.ToString(),
                    DataType = m.DataType,
                    IsReferenceKey = m.IsReferenceKey

                }).ToList();
            }
            else
                return new List<ScriptParameter>();
        }

        private void IapDisplayOutput(List<ExecutionResultView> executionResultView)
        {
            foreach (var result in executionResultView)
            {
                if (outputScriptMap.ContainsKey(result.ServerName))
                {
                    Guid Identifier = new Guid(outputScriptMap[result.ServerName]);
                    result.Identifier = Identifier;
                    result.IsScript = true;
                }
                    //if (iAPNode)
                    //{
                SendStatusUpdate(result.Identifier, DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Node Execution Completed...." + Environment.NewLine, result.ServerName, 99);
                    //    iAPNode = false;
                    //}
                    result.Progress = 100;
                    result.data = result.SuccessMessage;

                
            }
            ShowOutputView(this, new EventArgs<List<ExecutionResultView>>(executionResultView));
        }

        private void Test()
        {
        }
        private void dgList_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            dgList.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            if (e.RowIndex != -1)
            {
                this.dgList.Columns[3].DefaultCellStyle.Padding = new Padding(5, 0, -5, 0);
                this.dgList.Columns[4].DefaultCellStyle.Padding = new Padding(-10, 0, 0, 0);

                if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        dgList.GridColor, 0, ButtonBorderStyle.None,
                        dgList.GridColor, 1, ButtonBorderStyle.None,
                        dgList.GridColor, 1, ButtonBorderStyle.Solid,
                        dgList.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                }
                if (e.ColumnIndex == 3)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        dgList.GridColor, 1, ButtonBorderStyle.None,
                        dgList.GridColor, 1, ButtonBorderStyle.None,
                        dgList.GridColor, 0, ButtonBorderStyle.None,
                        dgList.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                }
                if (e.ColumnIndex == 4 || e.ColumnIndex == 5)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                    ControlPaint.DrawBorder(e.Graphics, e.CellBounds,
                        dgList.GridColor, 0, ButtonBorderStyle.None,
                        dgList.GridColor, 1, ButtonBorderStyle.None,
                        dgList.GridColor, 1, ButtonBorderStyle.Solid,
                        dgList.GridColor, 1, ButtonBorderStyle.Solid);
                    e.Handled = true;
                }
            }


        }



        internal void Publish(byte[] p, Models.Script s)
        {
            if (editScriptDetails == null)
            {
                Views.ScriptDetails script = new ScriptDetails(s);
                script.Publish(p, s.ScriptType);
            }
            else
            {
                editScriptDetails._script = s;
                editScriptDetails.Publish(p, s.ScriptType);
            }
        }

        private void dgList_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>> tupleScriptDetails =
                    (Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>)dgList.Rows[e.RowIndex].Cells[7].Value;
                // If task type is null
                if (tupleScriptDetails != null)
                {
                    if ((!string.IsNullOrEmpty(tupleScriptDetails.Item7)) && !tupleScriptDetails.Item7.ToLower().Equals("command") && !tupleScriptDetails.Item7.ToLower().Equals("sh command"))
                    {
                        e.Value = Image.FromFile(@"Images\edit.png");
                    }
                }
                // if (dgList.Rows[e.RowIndex].Cells[2].Value.ToString().Equals("Command"))

            }
        }

        private void Refresh()
        {

            if (this.categories != null)
            {
                this.categories.Clear();
                this.categories = null;
                this.tvCatSubcat.Nodes.Clear();
                LoadCategory(null);
            }
        }

        [EventSubscription(Constants.EventTopicNames.RefreshCategories, ThreadOption.UserInterface)]
        public void RefreshCategories(object sender, EventArgs<String> e)
        {

            Refresh();
        }

        [EventSubscription(Constants.EventTopicNames.CategoryDeleted, ThreadOption.UserInterface)]
        public void CategoryDeleted(object sender, EventArgs e)
        {
            Refresh();
        }

        [EventSubscription(Constants.EventTopicNames.CurrentTab, ThreadOption.UserInterface)]
        public void TabChanged(object sender, EventArgs e)
        {
           // Refresh();
            //if (this.tvCatSubcat != null &&
            //    this.tvCatSubcat.SelectedNode != null)
            //    this.tvCatSubcat.SelectedNode.Expand();
        }

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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void tvCatSubcat_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }



        internal void Delete(int scriptIndex)
        {
            if (MessageBox.Show("Are you sure you want to delete?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //delete the row selected
                try
                {
                    DeleteScript(scriptIndex);
                    BindCategory();
                    MessageBox.Show("Record deleted successfully", "Status", MessageBoxButtons.OK);
                }
                catch (Exception ex)
                {
                    string err = ex.Message;
                    if (ex.InnerException != null)
                        err = err + ". \nInner Exception- " + ex.InnerException.Message;
                    MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
                Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>> tupleScriptDetails =
             (Tuple<List<ScriptParams.ScriptParam>, string, string, string, string, string, string, Tuple<string, string, bool, string, string[], Nullable<System.DateTime>, Nullable<System.DateTime>>>)dgList.Rows[e.RowIndex].Cells[7].Value;
                // If task type is null
                if (tupleScriptDetails != null)
                {
                    if ((!string.IsNullOrEmpty(tupleScriptDetails.Item7)) && tupleScriptDetails.Item7.ToLower().Equals("command") || tupleScriptDetails.Item7.ToLower().Equals("sh command"))
                        return;
                }

                Models.Script selectedscript = this.dgList.Rows[e.RowIndex].Tag as Models.Script;
                ShowCategoryScriptView(e.RowIndex);
            if (isSuperAdmin)
                editScriptDetails.EditScript_Handler();
            else if (CurentUser.Role != Roles.Agent.ToString())
                    editScriptDetails.EditScript_Handler();
            

        }

        private void dgList_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == ConsoleKey.DownArrow)
            //{
            //    dgList_CellMouseClick(this,
            //}
        }
    }

    public enum TreeNodeContext
    {
        Category, Script, Root
    }

    
}
