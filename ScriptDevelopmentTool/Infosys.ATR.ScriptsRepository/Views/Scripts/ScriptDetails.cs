using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Infosys.WEM.SecureHandler;
using WEMClient = Infosys.WEM.Client;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Scripts.Service.Contracts.Data;
using System.Net;
using System.IO;
using Infosys.ATR.ScriptsRepository.Views.Scripts;
using Infosys.WEM.ScriptExecutionLibrary;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Infosys.ATR.ScriptRepository.Models;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.ATR.CommonViews;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.WEM.Client;
using Infosys.WEM.Node.Service.Contracts.Message;
using System.Web.Script.Serialization;
using System.Collections;
using System.Runtime.Serialization.Json;
using Infosys.WEM.Infrastructure.Common;
using System.Text;
using System.Configuration;
using Infosys.IAP.CommonClientLibrary.Models;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.ScriptRepository.Views
{
    public enum ExecutionMode
    {
        Online,
        Offline
    }
    public partial class ScriptDetails : UserControl
    {
        internal Models.Script _script;
        bool iapnodeexecuted = false;
        bool clearedOnLoad = false;
        string _categoryId, _paramId, _scriptUrl, _downloadScriptname;
        bool _newItem, _newParam;
        List<Models.ScriptParameter> _parameters = new List<Models.ScriptParameter>();
        WEMClient.ScriptRepository client = new WEMClient.ScriptRepository();
        WEMClient.CommonRepository commonRepositoryClient = new WEMClient.CommonRepository();
        private const string SCRIPTRESULT = "Script Execution Results";
        List<string> selectedNodeNames = new List<string>();

        internal static bool showSSHForm = false;
        private ContextMenuStrip contextMenuPublishOptions;
        private bool publishLocally = false;
        private bool enableSaveOption = false;
        private string executionMode = string.Empty;
        private string localScriptPath = string.Empty;
        private string moduleLocation = string.Empty;
        bool isGenratedScript;
        List<Infosys.ATR.ScriptRepository.Models.Category> CategoryDetails { get; set; }
        List<Infosys.ATR.ScriptRepository.Models.Category> Categories { get; set; }
        string subCategory;
        List<SubCategory> subCatDetails;
        string stoarageBaseUrl;
        public static Dictionary<string, string> outputScriptMap = new Dictionary<string, string>();

        public class ScriptProcessedArgs : EventArgs
        {
            public string ScriptId { get; set; }
            public string CategoryId { get; set; }
            public string SubCategoryId { get; set; }
            public string Name { get; set; }
        }
        public delegate void ScriptProcessedEventHandler(ScriptProcessedArgs e);
        public event ScriptProcessedEventHandler ScriptProcessed;
        public class ScriptStatusUpdateArgs : EventArgs
        {

            public Guid Identifier { get; set; }
            public string StatusMessge { get; set; }
            public string ServerName { get; set; }
            public int PercentComplete { get; set; }
            public Dictionary<string, string> outputScriptMap { get; set; }
        }
        public delegate void ScriptStatusUpdateArgsEventHandler(ScriptStatusUpdateArgs e);
        public event ScriptStatusUpdateArgsEventHandler ScriptStatus;


        public delegate void EditScriptEventHanlder(object[] e);
        public event EditScriptEventHanlder EditScript;

        public delegate void ScriptExecutedEventHandler(List<ExecutionResultView> e);
        public event ScriptExecutedEventHandler ScriptExecuted;

        public delegate void IAPNodeExecutedEventHandler(List<ExecutionResultView> e);
        public event IAPNodeExecutedEventHandler IAPNodeExecuted;

        public delegate byte[] ReadScript();
        public event ReadScript ReadScript_Handler;

        public static List<string> parametersCollection = null;

        public string scriptContent;
        public Hashtable hsDataTypes;

        internal bool enableControls;

        public ScriptDetails(Models.Script script, string categoryId = "", bool newItem = false, string fileXtn = "", string filePath = "", bool isIapPackage = false, bool GenratedScript = false)
        {
            InitializeComponent();
            executionMode = ConfigurationManager.AppSettings["Mode"];
            publishLocally = (executionMode.Equals(ExecutionMode.Offline.ToString(), StringComparison.InvariantCultureIgnoreCase)) ? true : isIapPackage;
            localScriptPath = filePath;

            if (GenratedScript)
            {
                isGenratedScript = GenratedScript;
                pnlSciptLocation.Visible = true;
                moduleLocation = Path.GetDirectoryName(filePath);
            }

            if (executionMode.Equals(ExecutionMode.Offline.ToString(), StringComparison.InvariantCultureIgnoreCase))
                this.cmbScriptXtn.Items.AddRange(new object[] {
                "bat",
                "vbs",
                "iap",
                "py",
                "js",
                "ps1",
                "iapd",
                "sh"});
            else if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                this.cmbScriptXtn.Items.AddRange(new object[] {
                "sh",               
                "ps1"});
            else
                this.cmbScriptXtn.Items.AddRange(new object[] {
                "bat",
                "vbs",
                "iap",
                "py",
                "js",
                "ps1",
                "iapd",
                "sh"});

            showSSHForm = false;
            pnlArgOption.Visible = false;
            rdbArgString.Checked = true;
            _newItem = newItem;
            _categoryId = categoryId;
            toolTip1.SetToolTip(txtScriptName, "Script name should be same as the file name (without ext) in case of script file of type .iap (ifea)");
            int companyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);
            if (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                stoarageBaseUrl = CommonServices.Instance.StorageBaseURL; //commonRepositoryClient.ServiceChannel.GetCompanyDetails(companyId.ToString()).Company.StorageBaseUrl;
                GetCategories(companyId);
            }
            lblDataType.Visible = false;
            cmbDataType.Visible = false;
            if (script != null)
            {
                _script = script;

                //if (_script.TaskType == "Command")
                //    linkLabel1.Enabled = false;               

                if (!(isIapPackage || isGenratedScript))
                {
                    if (CheckRootCategory(script.CategoryId))
                    {
                        cmbCategory.SelectedValue = script.CategoryId;
                        cmbSubCategory.SelectedItem = null;
                    }
                    else
                    {
                        SetCategory(script.CategoryId);
                        cmbSubCategory.SelectedValue = script.CategoryId;
                    }
                }

                txtScriptName.Text = script.Name;
                txtDesc.Text = script.Description;
                checkBox1.Checked = script.UsesUIAutomation;
                txtTags.Text = script.Tags;
                //txtScriptType.Text = script.ScriptType;
                //txtLoc.Text = script.ScriptLocation;
                if (!string.IsNullOrEmpty(script.ScriptLocation))
                {
                    btnDownload.Enabled = true;
                    _downloadScriptname = script.Name + "." + script.ScriptType;
                }
                else
                    btnDownload.Enabled = false;

                _scriptUrl = stoarageBaseUrl + script.ScriptLocation;
                txtCommand.Text = script.TaskCommand;
                txtArgs.Text = script.ArgumentString;
                txtWorkingDir.Text = script.WorkingDir;
                chkRunasAdmin.Checked = script.RunAsAdmin;
                lblTaskType.Text = script.TaskType;
                cmbTaskType.SelectedItem = script.TaskType;
                if (script != null && !String.IsNullOrEmpty(script.ScriptType))
                {
                    cmbScriptXtn.SelectedItem = script.ScriptType;

                    if (!string.IsNullOrEmpty(script.ScriptType))
                        if (script.ScriptType.Equals("iap"))
                            txtIfeaScriptName.Text = script.IfeaScriptName;
                        else if (script.ScriptType.Equals("py") || script.ScriptType.Equals("iapd"))
                            txtIfeaScriptName.Text = script.CallMethod;
                }

                UpdateTaskType(script.TaskType);
                //  _subCategory = script.SubCategory;

                if (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    btnRunScript.Enabled = true;
                else
                    btnRunScript.Enabled = false;

                if (publishLocally && isIapPackage)
                    btnRunScript.Enabled = true;

                if (script.Parameters != null && script.Parameters.Count > 0)
                {
                    _parameters = script.Parameters;
                    dgParams.DataSource = GetScriptSubset(script.Parameters);
                    ClearSelection();
                    btnAddParam.Enabled = true;
                    pnlParameters.Visible = true;
                    rdbParameters.Checked = true;
                }
                else
                {
                    pnlParameters.Visible = false;
                }
            }
            // new script
            else
            {
                // Get selected category id and select the same in the category dropdown
                string selectedCatID = MainRepositoryView.selectedCatID;
                if (!string.IsNullOrEmpty(selectedCatID))
                {
                    if (CheckRootCategory(selectedCatID))
                    {
                        cmbCategory.SelectedValue = selectedCatID;
                        cmbSubCategory.SelectedItem = null;
                    }
                    else
                    {
                        SetCategory(selectedCatID);
                        cmbSubCategory.SelectedValue = selectedCatID;
                    }
                }
                else
                {
                    cmbCategory.SelectedValue = -1;
                    cmbSubCategory.SelectedValue = -1;
                }
                pnlParameters.Visible = false;
                btnRunScript.Enabled = false;
                if (!String.IsNullOrEmpty(fileXtn))
                {
                    cmbScriptXtn.SelectedItem = fileXtn;
                    cmbTaskType.SelectedItem = "File";

                    if (!String.IsNullOrEmpty(filePath))
                    {
                        txtScriptName.Text = Path.GetFileNameWithoutExtension(filePath);
                        if (fileXtn.ToLower().Equals("iap"))
                            txtScriptFile.Text = filePath;
                    }
                }
            }

            if (newItem)
            {
                btnSave.Text = (executionMode.Equals(ExecutionMode.Offline.ToString(), StringComparison.InvariantCultureIgnoreCase)) ? "Export" : "Add";
                label2.Text = "Script Details";
                btnInfo.Visible = false;
                label18.Visible = false;
                txtArgs.Visible = false;
                if (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    EnableSaveOption();
            }
            else
            {
                btnInfo.Visible = true;
                var users = MainRepositoryView.Users;

                if (users != null)
                {
                    var user = users.Where(u => u.CategoryId == Convert.ToInt32(script.CategoryId)).FirstOrDefault();

                    if (user != null)
                    {
                        if (user.Role == Infosys.ATR.Entities.Roles.Guest.ToString() || user.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                        {
                            for (int j = 0; j < this.Controls.Count; j++)
                                DisableControls(this.Controls[j], false);

                            btnInfo.Enabled = true;

                            if (user.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                                btnRunScript.Enabled = true;

                            foreach (DataGridViewRow row in dgParams.Rows)
                            {
                                this.dgParams.Columns[0].Visible = false;
                                row.ReadOnly = true;
                            }

                        }
                        else if (user.Role == Infosys.ATR.Entities.Roles.Analyst.ToString())
                        {
                            btnDownload.Enabled = true;
                        }
                        else if (user.Role == Infosys.ATR.Entities.Roles.Manager.ToString())
                        {
                            btnDownload.Enabled = true;
                        }
                    }
                }
            }
        }
        public void EnableSaveOption()
        {
            CreateContextMenuStrip();
            enableSaveOption = true;
            this.btnSave.Image = ((System.Drawing.Image)((new System.ComponentModel.ComponentResourceManager(typeof(ScriptDetails))).GetObject("btnSave.Image")));
        }
        private void CreateContextMenuStrip()
        {
            contextMenuPublishOptions = new ContextMenuStrip();
            contextMenuPublishOptions.Items.Add(new ToolStripMenuItem() { Name = "Publish", Tag = "Publish", Text = "Publish" });
            contextMenuPublishOptions.Items.Add(new ToolStripMenuItem() { Name = "Export", Tag = "Export", Text = "Export" });

            foreach (ToolStripMenuItem mItem in contextMenuPublishOptions.Items)
                mItem.Click += new System.EventHandler(this.AutoFillToolStripMenuItem_Click);
        }
        private void AutoFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string m = ((ToolStripMenuItem)sender).Tag.ToString();
            if (m.Equals("Export"))
                publishLocally = true;

            SaveScript();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (enableSaveOption)
            {
                btnSave.ContextMenuStrip = contextMenuPublishOptions;
                btnSave.ContextMenuStrip.Show(btnSave, new System.Drawing.Point(0, btnSave.Height));
            }
            else
                SaveScript();
        }
        private void btnInfo_Click(object sender, System.EventArgs e)
        {
            if (_script != null)
            {
                using (Infosys.ATR.ScriptsRepository.Views.Info info = new Infosys.ATR.ScriptsRepository.Views.Info())
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(Environment.NewLine);
                    sb.Append(string.Format("Script Name :  {0}", _script.Name + Environment.NewLine));
                    sb.Append(string.Format("Category Id :  {0}", _script.CategoryId + Environment.NewLine));
                    sb.Append(string.Format("Version No :  {0}", _script.Version + Environment.NewLine));
                    sb.Append(string.Format("Script Id :  {0}", _script.Id + Environment.NewLine));
                    sb.Append(string.Format("Created By :  {0} on {1}", _script.CreatedBy, DateTime.SpecifyKind(_script.CreatedOn, DateTimeKind.Utc).ToLocalTime() + Environment.NewLine));
                    sb.Append(string.Format("Last Modified By :  {0} on {1}", _script.ModifiedBy, DateTime.SpecifyKind(_script.ModifiedOn, DateTimeKind.Utc).ToLocalTime() + Environment.NewLine));
                    info.Controls["txtOutput"].Text = sb.ToString();
                    info.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    info.StartPosition = FormStartPosition.CenterParent;
                    info.MinimizeBox = false;
                    info.MaximizeBox = false;
                    info.Text = _script.Name;
                    info.ShowDialog();
                }
            }
        }

        /// <summary>
        /// This method is used to fecth categories from DB and binds root categories to dropdown.
        /// </summary>
        /// <param name="companyId"></param>
        private void GetCategories(int companyId)
        {
            Categories = new List<Infosys.ATR.ScriptRepository.Models.Category>();
            Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg catResponse = commonRepositoryClient.ServiceChannel.GetAllCategoriesByCompany(companyId.ToString(), Constants.Application.ModuleID);

            Categories = CategoryDetails = Translators.CategoryPE_SE.CategoryListSEtoPE(catResponse.Categories.ToList());

            Users = Infosys.ATR.Entities.CommonObjects.Users;

            if (CategoryDetails != null && Users != null)
            {
                var userCategory = Users.Select(u => u.CategoryId).Distinct().ToList();
                if (userCategory != null)
                {
                    this.Categories = catResponse.Categories.Where(c => c.ParentId == 0).ToList().
                        Where(c =>
                        {
                            if (Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                                return true;

                            return userCategory.Contains(Convert.ToInt32(c.CategoryId));
                        }).
                    Select(c => new Models.Category
                    {
                        Description = c.Description,
                        Id = Convert.ToString(c.CategoryId),
                        Name = c.Name,
                        ParentId = c.ParentId.GetValueOrDefault(),
                        CompanyId = c.CompanyId
                    }).ToList();
                }
            }

            for (int i = 0; i < this.Categories.Count; i++)
            {
                var c = this.Categories[i];
                //do not show common category if no child categories are present in common
                if (c.CompanyId == 0 && c.ParentId == 0)
                {
                    var subCat = this.CategoryDetails.Where(sc => sc.CompanyId == 0 && sc.ParentId == Convert.ToInt32(c.Id));
                    if (subCat == null || subCat.Count() == 0)
                    {
                        this.Categories.Remove(c);
                    }
                }

                if (!Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                {
                    var catIds = Users.Where(u => u.Role == "Guest").Select(x => x.CategoryId).ToArray();
                    this.Categories.RemoveAll(uc => catIds.Contains(Convert.ToInt32(uc.Id)));
                }
            }

            //if (!Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
            //{
            //    var catIds = Users.Where(u => u.Role == "Guest").Select(x => x.CategoryId).ToArray();
            //    this.Categories.RemoveAll(c => catIds.Contains(Convert.ToInt32(c.Id)));
            //}

            this.cmbCategory.DataSource = this.Categories;
            this.cmbCategory.DisplayMember = "Name";
            this.cmbCategory.ValueMember = "Id";
            //}
        }

        /// <summary>
        /// This method is used to populate child categories based on selected root category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            Infosys.ATR.ScriptRepository.Models.Category c = this.cmbCategory.SelectedItem as Infosys.ATR.ScriptRepository.Models.Category;
            if (c != null)
            {
                this.cmbSubCategory.DataSource = null;
                subCatDetails = new List<SubCategory>();

                CombineChildCategories(String.Empty, c, CategoryDetails, subCatDetails);

                if (subCatDetails.Count > 0)
                {
                    this.cmbSubCategory.DataSource = subCatDetails;
                    this.cmbSubCategory.DisplayMember = "SubCategoryName";
                    this.cmbSubCategory.ValueMember = "SubCategoryId";
                }
            }
        }

        /// <summary>
        /// This method is used to combine child categories separated by dot.
        /// </summary>
        /// <param name="prefix">prefix to append to child category name</param>
        /// <param name="c">category object</param>
        /// <param name="categories">List of all categories</param>
        /// <param name="childCategories">List of child categories</param>
        private void CombineChildCategories(string prefix, Models.Category c,
                           IEnumerable<Models.Category> categories, List<SubCategory> childCategories)
        {
            //if (c.ParentId == 0)
            //{
            //    catStrings.Add(c.Name);
            //    prefix = c.Name;
            //}

           
            var children = categories.Where(cats => cats.ParentId == Convert.ToInt32(c.Id));

            if (!Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
            {
                var userCategory = Users.Select(u => u.CategoryId).Distinct().ToList();               

                children = children.Where(cats =>
                    {
                        //if (Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                        //    return true;

                        return userCategory.Contains(Convert.ToInt32(cats.Id));

                    }).ToList();
            }            

            if (children.Count() == 0)
            {
                return;
            }

            foreach (Models.Category child in children)
            {
                SubCategory childCat = new SubCategory();
                if (string.IsNullOrEmpty(prefix))
                {
                    childCat.SubCategoryId = child.Id;
                    childCat.SubCategoryName = child.Name;
                    childCategories.Add(childCat);

                    CombineChildCategories(child.Name,
                                       child, categories, childCategories);
                }
                else
                {
                    childCat.SubCategoryId = child.Id;
                    childCat.SubCategoryName = prefix + "." + child.Name;
                    childCategories.Add(childCat);

                    CombineChildCategories(prefix + "." + child.Name,
                                       child, categories, childCategories);
                }
            }
        }

        /// <summary>
        /// This method is used to verify if the passed category is root category.
        /// </summary>
        /// <param name="catID">category id</param>
        /// <returns>true if category is root category</returns>
        private Boolean CheckRootCategory(string catID)
        {
            var result = CategoryDetails.Where(cat => cat.Id.Equals(catID)).Select(e => e.ParentId).SingleOrDefault();
            //if ParentId is not null and greater than zero
            if (result == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method is used to select the category in the dropdown based on category id passed.
        /// </summary>
        /// <param name="catId">category id to be selected in the dropdown</param>
        private void SetCategory(string catId)
        {
            subCategory = "";
            catId = FindParentCategory(catId);
            CategoryDetails.Where(c => c.ParentId == 0).ToList().ForEach(c =>
            {
                if (c.Id.Equals(catId))
                {
                    this.cmbCategory.SelectedItem = c;
                    this.cmbCategory.SelectedValue = c.Id;
                }
            });
        }
        /// <summary>
        /// This method is used to find the parent category at the root level.
        /// </summary>
        /// <param name="catID">Category Id</param>
        /// <returns>Id of root category</returns>
        public string FindParentCategory(string catID)
        {
            string categoryId = catID;

            //var result = CategoryDetails.Where(cat => cat.Id.Equals(catID)).Select(e => new { e.Id, e.ParentId, e.Name }).Single();

            var result = CategoryDetails.FirstOrDefault(cat => cat.Id.Equals(catID));

            if (result != null && result.ParentId > 0)
            {
                categoryId = FindParentCategory(result.ParentId.ToString());
            }
            else if (result != null)
            {
                return result.Id;
            }
            return categoryId;
        }

        internal void DisableControls(bool disable)
        {
            //for (int i = 0; i < this.Controls.Count; i++)
            //    this.Controls[i].Enabled = disable;
        }

        //private void FetchCategories(string categoryId)
        //{
        //    WEMClient.CommonRepository commonRepositoryClient = new WEMClient.CommonRepository();
        //    //fetch all the categories
        //    string companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
        //    Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg catResponse = commonRepositoryClient.ServiceChannel.GetAllCategoriesByCompany(companyId);
        //    if (catResponse != null && catResponse.Categories != null && catResponse.Categories.Count > 0)
        //    {
        //        cmbCategory.DisplayMember = "Name";
        //        cmbCategory.ValueMember = "CategoryId";
        //        cmbCategory.DataSource = catResponse.Categories;

        //        if (!string.IsNullOrEmpty(categoryId))
        //        {
        //            catResponse.Categories.ForEach(c =>
        //            {
        //                if (c.CategoryId.ToString() == categoryId)
        //                    cmbCategory.SelectedValue = int.Parse(categoryId);
        //            });
        //        }
        //    }
        //}

        private void CheckScriptParamters(int scriptIndex)
        {
            MainRepositoryView.networkParamsValidation = false;
            List<Models.ScriptParameter> pm = new List<Models.ScriptParameter>();

            if (!string.IsNullOrEmpty(_script.ScriptType))
            {
                // Check if AllowRemote option has been enabled
                if (!publishLocally)
                    MainRepositoryView.allowRemote = CheckAllowRemoteOption(_script.ScriptType);

                if (_script.ScriptType.ToLower().Equals("vbs") || _script.ScriptType.ToLower().Equals("js") || _script.ScriptType.ToLower().Equals("bat") || _script.ScriptType.ToLower().Equals("py"))
                    MainRepositoryView.networkParamsValidation = true;
                MainRepositoryView.showSSHForm = false;
                if (_script.ScriptType.ToLower().Equals("sh"))
                    MainRepositoryView.showSSHForm = true;
            }
            else
            {
                if (_script.TaskType.ToLower().Equals("sh command"))
                {
                    MainRepositoryView.showSSHForm = true;
                }
                MainRepositoryView.allowRemote = CheckAllowRemoteOption(_script.TaskType);
            }

            MainRepositoryView.UsesUIAutomation = _script.UsesUIAutomation;

            if (_script.Parameters != null && _script.Parameters.Count > 0)
            {
                var inputParameters = from p in _script.Parameters
                                      where p.IOType.ToString().ToLower().Equals("in")
                                      select p;

                if (inputParameters.FirstOrDefault() != null)
                {
                    pm = inputParameters.ToList<Models.ScriptParameter>();
                    ScriptParameters frm = new ScriptParameters(pm);
                    frm.ScriptId = int.Parse(_script.Id);

                    if (!publishLocally)
                        frm.CategoryId = int.Parse(_script.CategoryId);

                    frm.UsesUI = _script.UsesUIAutomation;
                    frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);
                    frm.NodeExecuted += new ScriptParameters.NodeExecutedEventHandler(DisplayOutput);
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
                            if (ScriptStatus != null)
                            {
                                ScriptStatusUpdateArgs e = new ScriptStatusUpdateArgs();
                                e.PercentComplete = 25;
                                e.Identifier = Identifier;
                                e.ServerName = s;
                                e.outputScriptMap = outputScriptMap;
                                e.StatusMessge = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Node Execution Initiated....." + Environment.NewLine;
                                ScriptStatus(e);
                            }

                        }
                    }
                }
                else
                {
                    RunScriptLocally(scriptIndex);
                }
            }
            else if (string.IsNullOrEmpty(_script.ScriptType) && !string.IsNullOrEmpty(_script.ArgumentString))
            {
                ScriptParameter p = new ScriptParameter();
                p.Name = "Command Arguments";
                p.DefaultValue = _script.ArgumentString;
                p.IOType = Infosys.ATR.ScriptRepository.Models.ParameterIOTypes.In;
                p.IsSecret = false;
                p.IsUnnamed = true;
                //p.IsMandatory = true;
                pm.Add(p);
                //selectedScriptIndex = scriptIndex;
                ScriptParameters frm = new ScriptParameters(pm);
                frm.ScriptId = int.Parse(_script.Id);
                frm.CategoryId = int.Parse(_script.CategoryId);
                frm.UsesUI = _script.UsesUIAutomation;
                frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);
                frm.NodeExecuted += new ScriptParameters.NodeExecutedEventHandler(DisplayOutput);
                frm.ShowDialog(this);
            }

            else
            {
                if (MainRepositoryView.allowRemote)
                {
                    ScriptParameters frm = new ScriptParameters(null);
                    frm.ScriptId = int.Parse(_script.Id);
                    frm.CategoryId = int.Parse(_script.CategoryId);
                    frm.UsesUI = _script.UsesUIAutomation;
                    frm.ParametersUpdated += new ScriptParameters.ParametersUpdateHandler(ScriptParameters_RunScript_ButtonClicked);
                    frm.NodeExecuted += new ScriptParameters.NodeExecutedEventHandler(DisplayOutput);
                    frm.ShowDialog(this);
                }
                else
                {
                    RunScriptLocally(scriptIndex);
                }
            }
        }
        private void frm_IapNodeExecutedEventHandler(bool iapnodeexecuted)
        {
            this.iapnodeexecuted = iapnodeexecuted;
        }

        public void DisplayOutput(List<ExecutionResultView> executionResultView)
        {
            if (IAPNodeExecuted != null)
            {
                foreach (var result in executionResultView)
                {
                    if (outputScriptMap.ContainsKey(result.ServerName))
                    {
                        Guid Identifier = new Guid(outputScriptMap[result.ServerName]);
                        result.Identifier = Identifier;
                        result.IsScript = true;
                        result.data = result.SuccessMessage;
                    }
                }
                IAPNodeExecuted(executionResultView);
            }
            else if (ScriptDesigner != null)
                ScriptDesigner.DisplayOutput(executionResultView);
        }

        // Run the script
        private void ScriptParameters_RunScript_ButtonClicked()
        {
            int scriptId = 0;
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
            if (MainRepositoryView.ScheduledRequest)
                ScheduleScript();
            else if (Int32.TryParse(_script.Id, out scriptId))
            {
                RunScriptLocally(scriptId);
            }
        }

        private void ScheduleScript()
        {
            AddScheduledRequestResMsg scheduledReqIds = null;
            List<ExecutionResultView> schResultView = new List<ExecutionResultView>();
            ExecutionResultView schView = null;
            int nodeCount = 0;
            int count = 0;
            string serverName = "";
            List<string> selectedNodes = new List<string>();

            //gather the parameters
            Dictionary<string, string> inParams = new Dictionary<string, string>();
            List<Parameter> scriptInParams = new List<Parameter>();

            List<Parameter> param = new List<Parameter>();

            if (_script.Parameters != null && _script.Parameters.Count > 0)
            {
                List<Models.ScriptParameter> pm = new List<Models.ScriptParameter>();
                var inputParameters = from p in _script.Parameters
                                      where p.IOType.ToString().ToLower().Equals("in")
                                      select p;

                if (inputParameters.FirstOrDefault() != null)
                {
                    foreach (Models.ScriptParameter p in _script.Parameters)
                    {
                        if (p.IOType.ToString().ToLower().Equals("in"))
                        {
                            Parameter paramInputValue = new Parameter();
                            paramInputValue.ParameterName = _script.Parameters[count].Name;
                            paramInputValue.ParameterValue = (_script.Parameters[count].IsSecret) ? SecurePayload.Secure(parametersCollection[count], "IAP2GO_SEC!URE") : parametersCollection[count];
                            paramInputValue.allowedValues = _script.Parameters[count].AllowedValues;
                            paramInputValue.DataType = _script.Parameters[count].DataType;
                            paramInputValue.IsPaired = !_script.Parameters[count].IsUnnamed;
                            paramInputValue.IsSecret = _script.Parameters[count].IsSecret;

                            inParams.Add(_script.Parameters[count].Name, parametersCollection[count]);
                            scriptInParams.Add(paramInputValue);
                            count = count + 1;
                        }
                    }

                }
            }


            if (MainRepositoryView.RunOnCluster)
            {
                selectedNodes = new List<string>();
                selectedNodes.Add(MainRepositoryView.ClusterValue);
                selectedNodeNames = new List<string>();
                selectedNodeNames.Add(MainRepositoryView.ClusterName);
            }
            else if (MainRepositoryView.RunOnNode)
            {
                selectedNodes = new List<string>();
                selectedNodes = MainRepositoryView.SelectedNodes;
                selectedNodeNames = new List<string>();
                selectedNodeNames = MainRepositoryView.SelectedNodes;
            }
            if (selectedNodeNames != null)
            {
                outputScriptMap.Clear();
                foreach (string node in selectedNodeNames)
                {
                    Guid Identifier = Guid.NewGuid();

                    outputScriptMap.Add(node, Identifier.ToString());
                    if (ScriptStatus != null)
                    {
                        ScriptStatusUpdateArgs e = new ScriptStatusUpdateArgs();
                        e.PercentComplete = 20;
                        e.Identifier = Identifier;
                        e.ServerName = node;
                        e.outputScriptMap = outputScriptMap;
                        e.StatusMessge = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Scheduling Request....." + Environment.NewLine;
                        ScriptStatus(e);
                    }

                }
            }

            foreach (string node in selectedNodes)
            {
                AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
                req.Request = new Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest();
                req.Request.CategoryId = Convert.ToInt32(_script.CategoryId);
                req.Request.Requestor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                if (MainRepositoryView.ScheduledForNow)
                    req.Request.ExecuteOn = System.DateTime.UtcNow;
                else
                {
                    req.Request.ExecuteOn = MainRepositoryView.StartDate.ToUniversalTime();
                    if (MainRepositoryView.NoEndDate)
                        req.Request.Iterations = -1;
                    else if (MainRepositoryView.EndBy)
                    {
                        req.Request.EndDate = MainRepositoryView.EndDate;
                    }
                    else
                    {
                        int iterations = 0;
                        Int32.TryParse(MainRepositoryView.Iterations, out iterations);
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

                req.Request.Priority = Convert.ToInt32(MainRepositoryView.Priority);
                req.Request.RequestId = _script.Id;
                req.Request.RequestType = Infosys.WEM.Node.Service.Contracts.Data.RequestTypeEnum.Script;
                // req.Request.RequestVersion = 1;//No need to assign for script
                req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
                req.Request.StopType = Infosys.WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                req.Request.CompanyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);

                ScheduledRequest client = new ScheduledRequest("");
                scheduledReqIds = client.ServiceChannel.AddScheduledRequest(req);

                if (MainRepositoryView.RunOnCluster)
                    serverName = MainRepositoryView.ClusterName;
                else
                    serverName = selectedNodes[nodeCount];
                nodeCount = nodeCount + 1;
                if (!string.IsNullOrEmpty(serverName))
                {
                    if (outputScriptMap.ContainsKey(serverName))
                    {
                        Guid Identifier = new Guid(outputScriptMap[serverName]);
                        if (ScriptStatus != null)
                        {
                            ScriptStatusUpdateArgs e = new ScriptStatusUpdateArgs();
                            e.PercentComplete = 99;
                            e.Identifier = Identifier;
                            e.ServerName = serverName;
                            e.outputScriptMap = outputScriptMap;
                            e.StatusMessge = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Scheduling Completed....." + Environment.NewLine;
                            ScriptStatus(e);
                        }
                    }

                }
                if (scheduledReqIds.IsSuccess)
                {
                    foreach (string id in scheduledReqIds.ScheduledRequestIds)
                    {
                        schView = new ExecutionResultView();
                        schView.IsSuccess = true;
                        if (outputScriptMap != null && outputScriptMap.Count != 0)
                        {
                            Guid Identifier = new Guid(outputScriptMap[serverName]);
                            schView.Identifier = Identifier;
                        }
                        //else
                        //{
                        //    schView.Identifier = scheduledReqIds.;
                        //}
                        schView.IsScript = true;
                        schView.Progress = 100;
                        schView.ServerName = serverName;
                        schView.SuccessMessage = "The script " + _script.Name + " (id:" + _script.Id + ") has been scheduled. The corresponding scheduled request id is " + id + ".";
                        schView.data = schView.SuccessMessage;
                        schResultView.Add(schView);

                    }
                }
                else
                {
                    foreach (string id in scheduledReqIds.ScheduledRequestIds)
                    {
                        schView = new ExecutionResultView();
                        schView.IsSuccess = false;
                        if (outputScriptMap != null && outputScriptMap.Count != 0)
                        {
                            Guid Identifier = new Guid(outputScriptMap[serverName]);
                            schView.Identifier = Identifier;
                        }
                        //else
                        //{
                        //    schView.Identifier = scheduledReqIds.;
                        //}
                        schView.IsScript = true;
                        schView.Progress = 100;
                        schView.ServerName = serverName;
                        schView.ErrorMessage = id;
                        schResultView.Add(schView);


                    }
                }
            }
            DisplayOutput(schResultView);
            MainRepositoryView.RunOnCluster = false;
            MainRepositoryView.RunOnNode = false;
            MainRepositoryView.ScheduledForNow = false;
            MainRepositoryView.ScheduledForLater = false;
            MainRepositoryView.NoEndDate = false;
            MainRepositoryView.EndBy = false;
            MainRepositoryView.UsesUIAutomation = false;
            MainRepositoryView.ScheduledRequest = false;
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

        internal async void RunScriptLocally(int selectedRowIndex)
        {
            Infosys.WEM.ScriptExecutionLibrary.ScriptIndentifier scriptIden = new Infosys.WEM.ScriptExecutionLibrary.ScriptIndentifier();
            int count = 0;

            if (!string.IsNullOrEmpty(localScriptPath))
                scriptIden.Path = localScriptPath;

            // scriptIden.Identifier = new Guid();
            scriptIden.ScriptId = Convert.ToInt32(_script.Id);
            scriptIden.SubCategoryId = Convert.ToInt32(_script.CategoryId);
            List<Parameter> param = new List<Parameter>();

            if (_script.Parameters != null && _script.Parameters.Count > 0)
            {
                List<Models.ScriptParameter> pm = new List<Models.ScriptParameter>();
                var inputParameters = from p in _script.Parameters
                                      where p.IOType.ToString().ToLower().Equals("in")
                                      select p;

                if (inputParameters.FirstOrDefault() != null)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form is ScriptParameters)
                        {
                            form.Close();
                        }

                        this.Enabled = true;
                    }

                    foreach (Models.ScriptParameter p in _script.Parameters)
                    {
                        if (p.IOType.ToString().ToLower().Equals("in"))
                        {
                            Parameter paramsInputValue = new Parameter();
                            paramsInputValue.ParameterName = _script.Parameters[count].Name;
                            paramsInputValue.ParameterValue = (_script.Parameters[count].IsSecret) ? SecurePayload.Secure(parametersCollection[count], "IAP2GO_SEC!URE") : parametersCollection[count];
                            paramsInputValue.IsSecret = _script.Parameters[count].IsSecret;
                            // cmbDataType.SelectedValue = _script.Parameters[count].DataType;
                            //if (cmbDataType.SelectedIndex != -1)
                            //    paramsInputValue.DataType = cmbDataType.Text;
                            param.Add(paramsInputValue);
                            count = count + 1;
                        }
                    }
                    scriptIden.Parameters = param;
                }
            }

            else if (string.IsNullOrEmpty(_script.ScriptType) && !string.IsNullOrEmpty(_script.ArgumentString))
            {
                // Parameter paramsInputValue = new Parameter();
                // paramsInputValue.ParameterName = "Command";

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
            if (MainRepositoryView.showSSHForm)
            {
                //Add Remote parameters
                scriptIden.RemoteExecutionMode = ScriptIndentifier.RemoteExecutionHost.Linux;
                scriptIden.RemoteServerNames = MainRepositoryView.LinuxServerName;
                scriptIden.UserName = MainRepositoryView.UserName;
                scriptIden.Password = MainRepositoryView.Password;

                scriptIden.LinuxKeyPath = MainRepositoryView.LinuxKeyPath;


                outputScriptMap.Clear();
                string[] servernames = scriptIden.RemoteServerNames.Split(',');
                foreach (string server in servernames)
                {
                    Guid Identifier = Guid.NewGuid();

                    outputScriptMap.Add(server, Identifier.ToString());
                    if (ScriptStatus != null)
                    {
                        ScriptStatusUpdateArgs e = new ScriptStatusUpdateArgs();
                        e.PercentComplete = 20;
                        e.Identifier = Identifier;
                        e.ServerName = server;
                        e.outputScriptMap = outputScriptMap;
                        e.StatusMessge = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Execution Initiated....." + Environment.NewLine;
                        ScriptStatus(e);
                    }
                }


            }

            if (MainRepositoryView.allowRemote)
            {
                // Add remote parameters
                scriptIden.RemoteExecutionMode = ScriptIndentifier.RemoteExecutionHost.PS;
                scriptIden.RemoteServerNames = MainRepositoryView.remoteServerName;
                scriptIden.UserName = MainRepositoryView.UserName;
                scriptIden.Password = MainRepositoryView.Password;
                // if (scriptIden.RemoteServerNames.Contains(','))
                {
                    outputScriptMap.Clear();
                    string[] servernames = scriptIden.RemoteServerNames.Split(',');
                    foreach (string server in servernames)
                    {
                        Guid Identifier = Guid.NewGuid();

                        outputScriptMap.Add(server, Identifier.ToString());
                        if (ScriptStatus != null)
                        {
                            ScriptStatusUpdateArgs e = new ScriptStatusUpdateArgs();
                            e.PercentComplete = 20;
                            e.Identifier = Identifier;
                            e.ServerName = server;
                            e.outputScriptMap = outputScriptMap;
                            e.StatusMessge = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Execution Initiated....." + Environment.NewLine;
                            ScriptStatus(e);
                        }
                    }
                }


            }

            bool _executeScript = false;
            if ((scriptIden.ScriptId > 0 && scriptIden.SubCategoryId > 0 && !String.IsNullOrEmpty(_scriptUrl)))
                _executeScript = true;
            else if (!string.IsNullOrEmpty(scriptIden.Path))
                _executeScript = true;

            if (_executeScript)
            {
                List<ExecutionResult> result = await Task<List<ExecutionResult>>.Run(() =>
                {
                    List<ExecutionResult> scriptExecResult = ScriptExecutionManager.Execute(scriptIden);
                    return scriptExecResult;
                });
                if (MainRepositoryView.allowRemote || MainRepositoryView.showSSHForm || !string.IsNullOrEmpty(MainRepositoryView.LinuxServerName))
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
                                    if (ScriptStatus != null)
                                    {
                                        ScriptStatusUpdateArgs e = new ScriptStatusUpdateArgs();
                                        e.Identifier = new Guid(outputScriptMap[s]);
                                        scriptIden.TransactionId = e.Identifier;
                                        e.PercentComplete = 95;
                                        e.ServerName = s;
                                        e.StatusMessge = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Response recieved....." + Environment.NewLine;
                                        ScriptStatus(e);
                                        ScriptStatusUpdateArgs e1 = new ScriptStatusUpdateArgs();
                                        e1.Identifier = new Guid(outputScriptMap[s]);
                                        e1.PercentComplete = 99;
                                        e1.ServerName = s;
                                        e1.StatusMessge = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + "Script Execution Completed....." + Environment.NewLine;
                                        ScriptStatus(e1);
                                    }
                                }
                            }
                        }

                    }
                }
                MessageBox.Show("Script has been executed. Please Click OK button to see the output.", SCRIPTRESULT);
                List<ExecutionResultView> executionResultView = new List<ExecutionResultView>();
                ExecutionResultView view = null;
                if (MainRepositoryView.allowRemote)
                {
                    foreach (ExecutionResult r in result)
                    {
                        view = new ExecutionResultView();
                        view.SuccessMessage = r.InputCommand + Environment.NewLine + Environment.NewLine + r.SuccessMessage;
                        view.ErrorMessage = r.InputCommand + Environment.NewLine + Environment.NewLine + r.ErrorMessage;
                        view.IsSuccess = r.IsSuccess;
                        view.data = r.IsSuccess ? r.SuccessMessage + Environment.NewLine : "";
                        if (!string.IsNullOrEmpty(r.ComputerName))
                        {
                            view.ServerName = r.ComputerName;
                        }
                        else
                        {
                            view.ServerName = Environment.MachineName;
                        }
                        if (outputScriptMap != null && outputScriptMap.Count != 0 && !string.IsNullOrEmpty(r.ComputerName) && outputScriptMap.ContainsKey(r.ComputerName))
                        {
                            Guid Identifier = new Guid(outputScriptMap[r.ComputerName]);
                            view.Identifier = Identifier;
                        }
                        else
                        {
                            view.Identifier = r.TransactionId;
                        }
                        view.IsScript = true;
                        view.Progress = 100;
                        // view.Identifier = r.Identifier;
                        executionResultView.Add(view);
                    }
                }
                else
                {
                    foreach (ExecutionResult r in result)
                    {
                        view = new ExecutionResultView();
                        view.SuccessMessage = r.InputCommand + Environment.NewLine + Environment.NewLine + r.SuccessMessage;
                        view.ErrorMessage = r.InputCommand + Environment.NewLine + Environment.NewLine + r.ErrorMessage;
                        view.IsSuccess = r.IsSuccess;
                        view.data = r.IsSuccess ? r.SuccessMessage + Environment.NewLine : "";
                        if (!string.IsNullOrEmpty(r.ComputerName))
                        {
                            view.ServerName = r.ComputerName;
                        }
                        else
                        {
                            view.ServerName = Environment.MachineName;
                        }

                        if (outputScriptMap != null && outputScriptMap.Count != 0 && !string.IsNullOrEmpty(r.ComputerName) && outputScriptMap.ContainsKey(r.ComputerName))
                        {
                            Guid Identifier = new Guid(outputScriptMap[r.ComputerName]);
                            view.Identifier = Identifier;
                        }
                        else
                        {
                            view.Identifier = r.TransactionId;
                        }
                        view.IsScript = true;

                        //view.ServerName = Environment.MachineName;
                        view.Progress = 100;
                        executionResultView.Add(view);

                    }
                }
                MainRepositoryView.LinuxServerName = String.Empty;
                if (ScriptExecuted != null)
                {
                    ScriptExecuted(executionResultView);
                }
                else if (ScriptDesigner != null)
                    ScriptDesigner.DisplayOutput(executionResultView);

                //MainRepositoryView view = new MainRepositoryView();
                //   view.DisplayOutput(executionResultView);
                // ShowOutputView(this, new EventArgs<List<ExecutionResultView>>(executionResultView));
                //MessageBox.Show("Script has been executed. Please Click OK button to see the output.", SCRIPTRESULT);
                //if (MainRepositoryView.allowRemote)
                //    ScriptOutputDisplay.serverName = MainRepositoryView.remoteServerName;
                //else
                //    ScriptOutputDisplay.serverName = Environment.MachineName;

                //ScriptOutputDisplay displayOutput = new ScriptOutputDisplay(result);
                //displayOutput.Text = SCRIPTRESULT;
                //displayOutput.ShowDialog(this);

                //if (result.IsSuccess)
                //{
                //    MessageBox.Show("Script executed successfully. Please Click OK button to see the output", SCRIPTRESULT);
                //    ScriptOutputDisplay displayOutput = new ScriptOutputDisplay(result.SuccessMessage);
                //    displayOutput.Text = SCRIPTRESULT;
                //    displayOutput.ShowDialog(this);
                //}
                //else
                //{
                //    MessageBox.Show("Error in script execution. Please Click OK button to see the error details", SCRIPTRESULT);
                //    ScriptOutputDisplay displayOutput = new ScriptOutputDisplay(result.ErrorMessage);
                //    displayOutput.Text = SCRIPTRESULT;
                //    displayOutput.ShowDialog(this);
                //}
            }
            else
            {
                MessageBox.Show("Invalid Script", SCRIPTRESULT);
            }
        }

        private void btnRunScript_Click(object sender, EventArgs e)
        {
            int scriptId = 0;
            if (Int32.TryParse(_script.Id, out scriptId))
            {
                CheckScriptParamters(scriptId);
            }
        }

        //private void btnSave_Click(object sender, EventArgs e)
        private void SaveScript()
        {
            if (!ValidationPassed())
            {
                MessageBox.Show("Please provide  details for all these - Category, Name, Script Type and Script Extension Or Command.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!string.IsNullOrEmpty(txtScriptName.Text))
            {
                if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(txtScriptName.Text))
                {
                    //throw new Exception("Please provide the script name without Special Characters");
                    MessageBox.Show("Please provide the script name without Special Characters", "Invalid Script name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    if (_newItem)
                    {
                        AddScriptReqMsg req = new AddScriptReqMsg();
                        req.Script = new Infosys.WEM.Scripts.Service.Contracts.Data.Script();
                        //if (txtArgs.Enabled)
                        //    req.Script.ArgString = txtArgs.Text.Trim();
                        req.Script.BelongsToOrg = System.Configuration.ConfigurationManager.AppSettings["Company"];
                        //  req.Script.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        req.Script.Description = txtDesc.Text.Trim();
                        req.Script.Name = txtScriptName.Text.Trim();
                        req.Script.UsesUIAutomation = checkBox1.Checked;
                        req.Script.Tags = txtTags.Text;
                        //req.Script.LicenseType = "TechNet terms of use";
                        //req.Script.SourceUrl = "https://gallery.technet.microsoft.com/Add-AD-UserGroup-to-Local-fe5e9239";

                        //if(!string.IsNullOrEmpty(txtIfeaScriptName.Text.Trim())


                        //if (txtScriptFile.Enabled && !string.IsNullOrEmpty(txtScriptFile.Text.Trim()))
                        //    req.Script.ScriptType = txtScriptFile.Text.Trim().Substring(txtScriptFile.Text.Trim().IndexOf('.') + 1);

                        // if (txtScriptFile.Enabled)
                        // if (String.IsNullOrEmpty(cmbScriptXtn.SelectedText))

                        byte[] filecontent = null;

                        if (cmbTaskType.SelectedItem.ToString() == "File")
                        {
                            req.Script.ScriptType = cmbScriptXtn.SelectedItem.ToString();
                            if (cmbScriptXtn.SelectedIndex != -1)
                                if (cmbScriptXtn.SelectedItem.ToString().Equals("iap"))
                                    req.Script.IfeaScriptName = txtIfeaScriptName.Text.Trim();
                                else if (cmbScriptXtn.SelectedItem.ToString().Equals("py"))
                                    req.Script.CallMethod = txtIfeaScriptName.Text.Trim();
                                else if (cmbScriptXtn.SelectedItem.ToString().Equals("iapd"))
                                {
                                    req.Script.CallMethod = txtIfeaScriptName.Text.Trim();
                                    //package to create .iapd by asking the asking the path. the path may have the depended files to be included in the package
                                    //then assign the path of the so created .iapd to txtScriptFile
                                    //then hide btnBrowse so that the path of the .iapd cant be changed


                                    if (!isGenratedScript)
                                        if (MessageBox.Show("Please confirm if the Python file opened in the editor is the starting script file to be used in the IAPD package.", "Please confirm...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question).ToString().ToLower() != "ok")
                                        {
                                            this.Cursor = Cursors.Default;
                                            return;
                                        }

                                    FolderBrowserDialog fldrDiag = new FolderBrowserDialog();
                                    fldrDiag.Description = "Select the location for the IAPD package. The location should also have the depended files and folders referred by the .py file in the iapd package to be created.";

                                    var diagres = new DialogResult();
                                    if (!isGenratedScript) // check whether current script is playback script or not
                                        diagres = fldrDiag.ShowDialog();
                                    else
                                    {
                                        diagres = DialogResult.OK;
                                        fldrDiag.SelectedPath = moduleLocation;
                                    }

                                    if (diagres.ToString().ToLower() == "ok")
                                    {
                                        string path = fldrDiag.SelectedPath;
                                        filecontent = ReadScript_Handler();
                                        path = Path.Combine(path, req.Script.Name + ".py");
                                        File.WriteAllBytes(path, filecontent);
                                        var result = Infosys.ATR.Packaging.Operations.Package(path);
                                        if (result.IsSuccess)
                                        {
                                            txtScriptFile.Visible = true;
                                            txtScriptFile.Text = result.PackagePath;
                                            btnBrowse.Visible = false;
                                        }
                                        else
                                            MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else if (diagres.ToString().ToLower() == "cancel")
                                    {
                                        //get the working directory
                                        string path = GetAppPath();
                                        filecontent = ReadScript_Handler();
                                        path = Path.Combine(path, req.Script.Name + ".py");
                                        File.WriteAllBytes(path, filecontent);

                                        var result = Infosys.ATR.Packaging.Operations.PackageJustFile(path, "iapd");
                                        if (result.IsSuccess)
                                        {
                                            txtScriptFile.Visible = true;
                                            txtScriptFile.Text = result.PackagePath;
                                            btnBrowse.Visible = false;
                                        }
                                        else
                                            MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        this.Cursor = Cursors.Default;
                                        return;
                                    }
                                }
                        }

                        if (cmbSubCategory.SelectedIndex != -1)
                            req.Script.CategoryId = Convert.ToInt32(cmbSubCategory.SelectedValue);
                        else
                            req.Script.CategoryId = Convert.ToInt32(cmbCategory.SelectedValue);

                        if (txtCommand.Enabled)
                            req.Script.TaskCmd = txtCommand.Text.Trim();
                        req.Script.TaskType = lblTaskType.Text;

                        req.Script.WorkingDir = txtWorkingDir.Text.Trim();
                        req.Script.RunAsAdmin = chkRunasAdmin.Checked;
                        //req.Script.StorageBaseUrl = stoarageBaseUrl;
                        if (cmbTaskType.SelectedItem.ToString() == "File")
                        {
                            if (filecontent == null)
                                filecontent = ReadScript_Handler();

                            if ((filecontent == null || filecontent.Length == 0) && !req.Script.ScriptType.Equals("iap"))
                                throw new Exception("No data to publish");

                            foreach (var param in _parameters)
                            {
                                if (param.IsSecret)
                                    param.DefaultValue = SecurePayload.Secure(param.DefaultValue, "IAP2GO_SEC!URE");
                            }

                            req.Script.Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_parameters);
                            //cant use stream with webhttpbinding, refer- http://stackoverflow.com/questions/24527029/unable-to-send-image-file-as-part-datacontract
                            //causes error- Type 'System.IO.FileStream' with data contract name 'FileStream:http://schemas.datacontract.org/2004/07/System.IO' is not expected. Consider using a DataContractResolver or add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to DataContractSerializer.
                            //need to instead use byte[]
                            //byte[] filecontent = null;
                            if (cmbTaskType.SelectedItem != null)
                            {
                                switch (cmbTaskType.SelectedItem.ToString())
                                {
                                    //the below case is commented because as per the new implementation of the ifea runtime
                                    //the ifea scripts are already packaged as single .iap file and ifea runtime will be also expecting the script package as single .iap file
                                    //but keeping the switch so that in future for any other types of scripts, if the approach happens to be different then 
                                    //we will be able to handle it appropriately

                                    //case "ifea":
                                    //    filecontent = PackageIfeaFolder(txtScriptFile.Text.Trim());
                                    //    req.Script.ScriptContent = filecontent;
                                    //    break;
                                    default:
                                        if (System.IO.File.Exists(txtScriptFile.Text.Trim()))
                                        {
                                            if (req.Script.ScriptType.Equals("iap") || req.Script.ScriptType.Equals("iapd"))
                                            {
                                                if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(Path.GetFileNameWithoutExtension(txtScriptFile.Text)))
                                                {
                                                    //throw new Exception("Please provide the file name without Special Characters");
                                                    MessageBox.Show("Please provide the file name without Special Characters", "Invalid file name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    return;
                                                }

                                                req.Script.IfeaScriptName = txtIfeaScriptName.Text.Trim();
                                                filecontent = System.IO.File.ReadAllBytes(txtScriptFile.Text.Trim()); //to read the entire package
                                            }
                                        }

                                        if (filecontent != null && !publishLocally && (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                                        {
                                            if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                                                filecontent = SecurePayload.SecureBytes(filecontent);

                                            req.Script.ScriptContent = filecontent;
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (rdbArgString.Checked)
                                req.Script.ArgString = txtArgs.Text.Trim();
                            else if (rdbParameters.Checked)
                                req.Script.Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_parameters);
                        }


                        if (!publishLocally)
                        {
                            var response = client.ServiceChannel.AddScript(req);
                            if (response.ServiceFaults != null)
                            {
                                var faults = response.ServiceFaults;
                                WEM.Infrastructure.Common.WEMException ex = new WEM.Infrastructure.Common.WEMException();
                                ex.Data.Add("ServiceFaults", faults);
                                throw ex;
                            }

                            //if (client.ServiceChannel.AddScript(req).IsSuccess)
                            if (response.IsSuccess)
                            {
                                if (ScriptProcessed != null)
                                {
                                    ScriptProcessedArgs scriptArgs = new ScriptProcessedArgs();
                                    scriptArgs.CategoryId = cmbCategory.SelectedValue.ToString();
                                    if (cmbSubCategory.SelectedIndex != -1)
                                        scriptArgs.SubCategoryId = cmbSubCategory.SelectedValue.ToString();
                                    ScriptProcessed(scriptArgs);
                                }
                                MessageBox.Show(string.Format("The new script- {0} is added.", txtScriptName.Text.Trim()), "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                                MessageBox.Show("There is an error adding the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            ContentMeta contentMeta = new ContentMeta()
                            {
                                Name = req.Script.Name,
                                Description = req.Script.Description,
                                UsesUIAutomation = req.Script.UsesUIAutomation,
                                ArgumentString = req.Script.ArgString,
                                CallMethod = req.Script.CallMethod,
                                ContentType = req.Script.ScriptType,
                                IfeaScriptName = req.Script.IfeaScriptName,
                                ModuleType = ModuleType.Script,
                                RunAsAdmin = req.Script.RunAsAdmin,
                                Tags = req.Script.Tags,
                                TaskCommand = req.Script.TaskCmd,
                                TaskType = req.Script.TaskType,
                                WorkingDir = req.Script.WorkingDir,
                                Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoOE(_parameters)
                            };

                            IAPPackage.Export(contentMeta, filecontent);
                            this.Cursor = Cursors.Default;
                            return;
                        }
                    }
                    else
                    {
                        UploadScript(null, null);
                    }
                }
                catch (WEMException ex)
                {
                    StringBuilder exMsg = new StringBuilder();
                    List<ServiceFaultError> serviceFault = (List<ServiceFaultError>)ex.Data["ServiceFaults"];
                    serviceFault.ForEach(x =>
                    {
                        exMsg.Append(string.Format("{0}\n", x.Message));
                    });
                    MessageBox.Show(exMsg.ToString(), "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    string err = "There is an error adding/updating the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.";
                    err = err + "\nMore Infomation- " + ex.Message;
                    if (ex.InnerException != null)
                        err = err + ". \nInner Exception- " + ex.InnerException.Message;
                    MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Cursor = Cursors.Default;
            }
        }


        private string GetAppPath()
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


        internal void Publish(byte[] p, string xtn)
        {
            PublishScript(p, xtn);
        }

        private void PublishScript(byte[] p, string xtn)
        {
            UpdateScriptReqMsg req = new UpdateScriptReqMsg();
            req.Script = new Infosys.WEM.Scripts.Service.Contracts.Data.Script();
            req.Script.BelongsToOrg = System.Configuration.ConfigurationManager.AppSettings["Company"];
            req.Script.ScriptId = int.Parse(_script.Id);
            // req.Script.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            req.Script.Description = _script.Description;
            req.Script.Name = _script.Name;
            req.Script.UsesUIAutomation = _script.UsesUIAutomation;
            if (!lblTaskType.Text.ToLower().Equals("command") && !lblTaskType.Text.ToLower().Equals("sh command"))
                req.Script.ScriptURL = GetRelativeURL(_scriptUrl);
            req.Script.Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_parameters);
            if (req.Script.Parameters != null)
                foreach (ScriptParam parameter in req.Script.Parameters)
                {
                    if (parameter.IsSecret)
                        parameter.DefaultValue = SecurePayload.Secure(parameter.DefaultValue, "IAP2GO_SEC!URE");

                    parameter.ScriptId = req.Script.ScriptId;
                    parameter.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }

            req.Script.ScriptType = xtn;
            req.Script.CategoryId = Convert.ToInt32(_script.CategoryId);
            req.Script.TaskCmd = _script.TaskCommand;
            req.Script.TaskType = _script.TaskType;
            req.Script.WorkingDir = _script.WorkingDir;
            req.Script.RunAsAdmin = _script.RunAsAdmin;
            //req.Script.StorageBaseUrl = stoarageBaseUrl;
            req.Script.ScriptContent = p;

            if (req.Script.ScriptType.Equals("iap"))
                req.Script.IfeaScriptName = _script.IfeaScriptName;
            else if (req.Script.ScriptType.Equals("py"))
                req.Script.IfeaScriptName = _script.CallMethod;

            if (client.ServiceChannel.UpdateScript(req).IsSuccess)
            {
                if (ScriptProcessed != null)
                {
                    ScriptProcessedArgs scriptArgs = new ScriptProcessedArgs();
                    if (cmbSubCategory.SelectedValue != null)
                    {
                        scriptArgs.SubCategoryId = cmbSubCategory.SelectedValue.ToString();
                    }
                    scriptArgs.CategoryId = cmbCategory.SelectedValue.ToString();
                    scriptArgs.ScriptId = _script.Id;
                    ScriptProcessed(scriptArgs);
                }
                MessageBox.Show(string.Format("The script- {0} is updated.", _script.Name.Trim()), "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("There is an error updating the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UploadScript(byte[] p, string xtn)
        {
            UpdateScriptReqMsg req = new UpdateScriptReqMsg();
            req.Script = new Infosys.WEM.Scripts.Service.Contracts.Data.Script();
            //if (txtArgs.Enabled)
            //    req.Script.ArgString = txtArgs.Text.Trim();
            req.Script.BelongsToOrg = System.Configuration.ConfigurationManager.AppSettings["Company"];
            req.Script.ScriptId = int.Parse(_script.Id);
            // req.Script.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            req.Script.Description = txtDesc.Text.Trim();
            req.Script.Name = txtScriptName.Text.Trim();
            req.Script.UsesUIAutomation = checkBox1.Checked;
            req.Script.Tags = txtTags.Text;
            foreach (var param in _parameters)
            {
                if (param.IsSecret)
                    param.DefaultValue = SecurePayload.Secure(param.DefaultValue, "IAP2GO_SEC!URE");
            }

            byte[] filecontent = null;
            if (!lblTaskType.Text.ToLower().Equals("command") && !lblTaskType.Text.ToLower().Equals("sh command"))
            {
                req.Script.ScriptURL = GetRelativeURL(_scriptUrl);
                req.Script.Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_parameters);
                if (cmbScriptXtn.SelectedItem.ToString().Equals("py"))
                    req.Script.CallMethod = txtIfeaScriptName.Text.Trim();

                //to handle iapd package
                else if (cmbScriptXtn.SelectedItem.ToString().Equals("iapd"))
                {
                    req.Script.CallMethod = txtIfeaScriptName.Text.Trim();
                    //package to create .iapd by asking the asking the path. the path may have the depended files to be included in the package
                    //then assign the path of the so created .iapd to txtScriptFile
                    //then hide btnBrowse so that the path of the .iapd cant be changed

                    if (MessageBox.Show("Please confirm if the Python file opened in the editor is the starting script file to be used in the IAPD package.", "Please confirm...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question).ToString().ToLower() != "ok")
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }

                    FolderBrowserDialog fldrDiag = new FolderBrowserDialog();
                    fldrDiag.Description = "Select the location for the IAPD package. The location should also have the depended files and folders referred by the .py file in the iapd package to be created.";
                    if (fldrDiag.ShowDialog().ToString().ToLower() == "ok")
                    {
                        string path = fldrDiag.SelectedPath;
                        path = Path.Combine(path, req.Script.Name + ".py");
                        if (ReadScript_Handler != null)
                        {
                            filecontent = ReadScript_Handler();
                            File.WriteAllBytes(path, filecontent);
                        }

                        var result = Infosys.ATR.Packaging.Operations.Package(path);
                        if (result.IsSuccess)
                        {
                            txtScriptFile.Visible = true;
                            txtScriptFile.Text = result.PackagePath;
                            btnBrowse.Visible = false;
                        }
                        else
                            MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
            }
            else
            {
                if (rdbArgString.Checked)
                {
                    req.Script.ArgString = txtArgs.Text.Trim();
                    req.Script.Parameters = null;
                }
                else if (rdbParameters.Checked)
                {
                    req.Script.Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_parameters);
                    req.Script.ArgString = "";
                }
            }
            if (req.Script.Parameters != null)
                foreach (ScriptParam parameter in req.Script.Parameters)
                {
                    parameter.ScriptId = req.Script.ScriptId;
                    parameter.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                }
            //if (string.IsNullOrEmpty(xtn))
            //{
            //    if (txtScriptFile.Enabled && !string.IsNullOrEmpty(txtScriptFile.Text.Trim()))
            //        req.Script.ScriptType = txtScriptFile.Text.Trim().Substring(txtScriptFile.Text.Trim().IndexOf('.') + 1);
            //}
            //else
            // {

            if (cmbTaskType.SelectedItem.ToString() == "File")
            {
                req.Script.ScriptType = cmbScriptXtn.SelectedItem.ToString();
                if (req.Script.ScriptType.Equals("iap"))
                    req.Script.IfeaScriptName = txtIfeaScriptName.Text.Trim();
            }

            // }
            if (cmbSubCategory.SelectedIndex != -1)
                req.Script.CategoryId = Convert.ToInt32(cmbSubCategory.SelectedValue);
            else
                req.Script.CategoryId = Convert.ToInt32(cmbCategory.SelectedValue);
            if (txtCommand.Enabled)
                req.Script.TaskCmd = txtCommand.Text.Trim();
            req.Script.TaskType = lblTaskType.Text;
            req.Script.WorkingDir = txtWorkingDir.Text.Trim();
            req.Script.RunAsAdmin = chkRunasAdmin.Checked;
            //req.Script.StorageBaseUrl = stoarageBaseUrl;

            //byte[] filecontent = null;
            if (ReadScript_Handler != null && filecontent == null)
                filecontent = ReadScript_Handler();

            if (p == null)
            {
                //if (txtScriptFile.Enabled && txtScriptFile.Text.Trim() != "")
                {
                    /// if (System.IO.File.Exists(txtScriptFile.Text.Trim()))
                    {
                        switch (cmbTaskType.SelectedItem.ToString())
                        {
                            //the below case is commented because as per the new implementation of the ifea runtime
                            //the ifea scripts are already packaged as single .iap file and ifea runtime will be also expecting the script package as single .iap file
                            //but keeping the switch so that in future for any other types of scripts, if the approach happens to be different then 
                            //we will be able to handle it appropriately

                            //case "ifea":
                            //    filecontent = PackageIfeaFolder(txtScriptFile.Text.Trim());
                            //    req.Script.ScriptContent = filecontent;
                            //    break;
                            default:
                                if (System.IO.File.Exists(txtScriptFile.Text.Trim()))
                                {
                                    if (req.Script.ScriptType.Equals("iap") || req.Script.ScriptType.Equals("iapd"))
                                    {
                                        if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(Path.GetFileNameWithoutExtension(txtScriptFile.Text)))
                                        {
                                            //throw new Exception("Please provide the file name without Special Characters");
                                            MessageBox.Show("Please provide the file name without Special Characters", "Invalid file name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                        req.Script.IfeaScriptName = txtIfeaScriptName.Text.Trim(); //to read the content of the package
                                        filecontent = System.IO.File.ReadAllBytes(txtScriptFile.Text.Trim());
                                    }
                                }

                                if (filecontent != null)
                                {
                                    if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                                        filecontent = SecurePayload.SecureBytes(filecontent);

                                    req.Script.ScriptContent = filecontent;
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                req.Script.ScriptContent = p;
            }

            var response = client.ServiceChannel.UpdateScript(req);
            if (response.ServiceFaults != null)
            {
                var faults = response.ServiceFaults;
                WEM.Infrastructure.Common.WEMException ex = new WEM.Infrastructure.Common.WEMException();
                ex.Data.Add("ServiceFaults", faults);
                throw ex;
            }

            //if (client.ServiceChannel.UpdateScript(req).IsSuccess)
            if (response.IsSuccess)
            {
                if (ScriptProcessed != null)
                {
                    ScriptProcessedArgs scriptArgs = new ScriptProcessedArgs();
                    scriptArgs.CategoryId = cmbCategory.SelectedValue.ToString();
                    if (cmbSubCategory.SelectedIndex != -1)
                        scriptArgs.SubCategoryId = cmbSubCategory.SelectedValue.ToString();
                    scriptArgs.ScriptId = _script.Id;
                    ScriptProcessed(scriptArgs);

                }
                MessageBox.Show(string.Format("The script- {0} is updated.", txtScriptName.Text.Trim()), "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("There is an error updating the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private int GetParentCatId(string parentCat)
        {
            int catId = 0;
            foreach (SubCategory c in cmbSubCategory.Items)
            {
                if (c.SubCategoryName.Equals(parentCat))
                {
                    catId = Convert.ToInt32(c.SubCategoryId.ToString());
                    break;
                }
            }

            return catId;
        }
        private string GetRelativeURL(string url)
        {
            string relativeURL = url;
            int pos = url.IndexOf("iapscriptstore");
            if (pos > 0)
            {
                relativeURL = url.Substring(pos - 1);
            }

            return relativeURL;
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtScriptName.ReadOnly = false;
            if (cmbTaskType.SelectedItem != null)
            {
                switch (cmbTaskType.SelectedItem.ToString().ToLower())
                {
                    case "file":
                        OpenFileDialog browseScript = new OpenFileDialog();
                        //string fileTypeFilters = System.Configuration.ConfigurationManager.AppSettings["ScriptTypeFilters"]; 

                        string fileTypeFilters = "*.iap";

                        browseScript.Filter = "Script Files(" + fileTypeFilters + ")|" + fileTypeFilters;
                        if (browseScript.ShowDialog() == DialogResult.OK)
                        {
                            txtScriptFile.Text = browseScript.FileName;
                            if (System.IO.Path.GetExtension(txtScriptFile.Text) == ".iap")
                            {
                                //txtScriptName.Text = browseScript.SafeFileName.Substring(0, browseScript.SafeFileName.IndexOf('.'));
                                //txtScriptName.ReadOnly = true;
                            }
                        }
                        break;
                    //the below case is commented because as per the new implementation of the ifea runtime
                    //the ifea scripts are already packaged as single .iap file and ifea runtime will be also expecting the script package as single .iap file
                    //but keeping the switch so that in future for any other types of scripts, if the approach happens to be different then 
                    //we will be able to handle it appropriately
                    //case "ifea":
                    //    FolderBrowserDialog browseIfeaFolder = new FolderBrowserDialog();
                    //    browseIfeaFolder.Description = "Select the ifea folder, e.g. *.ifea :";
                    //    browseIfeaFolder.RootFolder = Environment.SpecialFolder.MyComputer;
                    //    if (browseIfeaFolder.ShowDialog() == DialogResult.OK)
                    //    {
                    //        //verify if the right ifea folder is selected
                    //        if (browseIfeaFolder.SelectedPath.EndsWith(".ifea", true, null))
                    //            txtScriptFile.Text = browseIfeaFolder.SelectedPath;
                    //        else
                    //            MessageBox.Show("Select the right ifea folder, e.g. *.ifea.", "Wrong ifea folder", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    //    }
                    //    break;
                }
            }
        }
        private void btnAddParam_Click(object sender, EventArgs e)
        {
            if (!pnlParameters.Visible)
                pnlParameters.Visible = true;
            pnlParam.Visible = true;
            _newParam = true;
            cmbBool.SelectedIndex = 1;
            cmbIsSecret.SelectedIndex = 0;
            cmbIsReference.SelectedIndex = 0;
            cmbIOTypes.SelectedIndex = 0;
            cmbIsPaired.SelectedIndex = 1;
            txtParamName.Text = "";
            txtDefaultValue.Text = "";
            txtAllowedValues.Text = "";
            txtParamName.Focus();
            btnAdd.Text = "Add";
        }
        private List<Models.ScriptParameterSubSet> GetScriptSubset(List<Models.ScriptParameter> parameters)
        {
            List<Models.ScriptParameterSubSet> objs = new List<Models.ScriptParameterSubSet>();
            parameters.ForEach(c =>
            {
                objs.Add(new Models.ScriptParameterSubSet() { Name = c.Name, IsMandatory = c.IsMandatory.ToString(), IOType = c.IOType });
            });
            return objs;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlParam.Visible = false;
            btnAdd.Text = "Add";
        }
        private bool CheckDuplicateParameter(List<ScriptParameter> parm)
        {
            bool duplicate = false;
            foreach (var p in parm)
            {
                if (p.Name.ToLower().Equals(txtParamName.Text.Trim().ToLower()))
                {
                    duplicate = true;
                    break;
                }
            }
            return duplicate;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbIOTypes.SelectedIndex < 0 || cmbBool.SelectedIndex < 0)// || string.IsNullOrEmpty(txtParamName.Text.Trim()))
            {
                MessageBox.Show("Please provide the details for all these Parameter fields- Name, Direction, Is Mandatory.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(txtParamName.Text))
            {
                //throw new Exception("Please provide the name of parameter without Special Characters");
                MessageBox.Show("Please provide the parameter name without Special Characters", "Invalid Parameter name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //foreach (var p in _parameters)
            //{
            //    if (p.Name.ToLower().Equals(txtParamName.Text.Trim().ToLower()))
            //    {
            //        duplicate = true;                    
            //        break;
            //    }
            //}

            bool duplicate = CheckDuplicateParameter(_parameters);


            //once the parameter is added to the grid, close it
            Models.ScriptParameter param1 = new Models.ScriptParameter();
            param1.AllowedValues = txtAllowedValues.Text.Trim();
            //if (_newParam)
            param1.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //else
            param1.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            param1.DefaultValue = txtDefaultValue.Text.Trim();
            if (cmbBool.SelectedItem != null)
                param1.IsMandatory = bool.Parse(cmbBool.SelectedItem.ToString());
            if (cmbIsSecret.SelectedItem != null)
                param1.IsSecret = bool.Parse(cmbIsSecret.SelectedItem.ToString());
            if (cmbIsPaired.SelectedItem != null)
                param1.IsUnnamed = bool.Parse(cmbIsPaired.SelectedItem.ToString());
            if (cmbIsReference.SelectedItem != null)
                param1.IsReferenceKey = bool.Parse(cmbIsReference.SelectedItem.ToString());

            param1.Name = txtParamName.Text.Trim();
            param1.IOType = (Models.ParameterIOTypes)Enum.Parse(typeof(Models.ParameterIOTypes), cmbIOTypes.SelectedItem.ToString());

            if (cmbDataType.Visible)
                param1.DataType = cmbDataType.SelectedValue.ToString();

            if (_newParam)
            {
                if (duplicate)
                {
                    MessageBox.Show("Parameter with this name already added. Please specify another name.", "Duplicate Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParamName.Focus();
                    return;
                }

                param1.ParamId = Guid.NewGuid().ToString();
                _parameters.Add(param1);
            }
            else
            {
                param1.ParamId = _paramId;


                for (int i = 0; i < _parameters.Count; i++)
                {
                    var p = _parameters[i];
                    if (p.ParamId == _paramId)
                    {
                        if (!_parameters[i].Name.ToLower().Equals(txtParamName.Text.ToLower()))
                            if (duplicate)
                            {
                                MessageBox.Show("Parameter with this name already added. Please specify another name.", "Duplicate Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtParamName.Focus();
                                return;
                            }

                        p.AllowedValues = param1.AllowedValues;
                        p.CreatedBy = param1.CreatedBy;
                        p.DataType = param1.DataType;
                        p.DefaultValue = param1.DefaultValue;
                        p.IOType = param1.IOType;
                        p.IsMandatory = param1.IsMandatory;
                        p.IsSecret = param1.IsSecret;
                        p.IsUnnamed = param1.IsUnnamed;
                        p.ModifiedBy = param1.ModifiedBy;
                        p.Name = param1.Name;
                        p.ParamId = param1.ParamId;
                        p.ScriptId = param1.ScriptId;

                        // _parameters.Remove(p);
                    }
                }



                //_parameters.Add(param1);
            }
            //chane the datasource of the grid to the new parameter list
            dgParams.DataSource = GetScriptSubset(_parameters);
            pnlParam.Visible = false;
            txtParamName.Text = "";
            txtAllowedValues.Text = "";
            txtDefaultValue.Text = "";
        }
        private void dgParams_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                if (_parameters.Count > 0)
                {
                    //i.e. Delete is called
                    if (MessageBox.Show("Are you sure you want to delete the parameter?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //delete the row selected
                        _parameters.RemoveAt(e.RowIndex);
                        //chane the datasource of the grid to the new parameter list
                        dgParams.DataSource = GetScriptSubset(_parameters);
                        txtParamName.Text = "";
                        txtParamName.Focus();
                    }
                }
            }
            else if (e.RowIndex >= 0)
            {
                pnlParam.Visible = true;
                PopulateParameter(e.RowIndex);
            }
        }
        private void PopulateParameter(int row)
        {
            _newParam = false;
            Models.ScriptParameter parameter = null;
            if (_script != null &&
                (_script.Parameters != null && _script.Parameters.Count > 0))
            {
                parameter = _script.Parameters[row];
            }
            else
            {
                parameter = _parameters[row];
            }
            txtParamName.Text = parameter.Name;
            txtAllowedValues.Text = parameter.AllowedValues;
            txtDefaultValue.Text = parameter.DefaultValue;
            if (parameter.IsMandatory)
                cmbBool.SelectedIndex = 0;
            else
                cmbBool.SelectedIndex = 1;

            if (parameter.IsSecret)
            {
                if (!string.IsNullOrEmpty(parameter.DefaultValue))
                    if (parameter.DefaultValue.EndsWith("aWFwMTYy"))
                        txtDefaultValue.Text = SecurePayload.UnSecure(parameter.DefaultValue, "IAP2GO_SEC!URE");

                cmbIsSecret.SelectedIndex = 1;
            }
            else
                cmbIsSecret.SelectedIndex = 0;

            if (parameter.IsUnnamed)
                cmbIsPaired.SelectedIndex = 0;
            else
                cmbIsPaired.SelectedIndex = 1;

            if (parameter.IsReferenceKey)
                cmbIsReference.SelectedIndex = 1;
            else
                cmbIsReference.SelectedIndex = 0;

            //cmbIOTypes.SelectedText = parameter.IOType.ToString();
            cmbIOTypes.SelectedIndex = (int)parameter.IOType;
            _paramId = parameter.ParamId;
            if (!string.IsNullOrEmpty(parameter.DataType))
                cmbDataType.SelectedValue = parameter.DataType;
            btnAdd.Text = "Save";
        }
        private void ClearSelection()
        {
            if (dgParams.Rows.Count > 0)
                dgParams.Rows[0].Selected = false;
        }
        private void dgParams_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (!clearedOnLoad)
            {
                ClearSelection();
                clearedOnLoad = true;
            }
        }
        private void UpdateTaskType(string taskType)
        {
            switch (taskType.ToLower())
            {
                case "file":
                    EnableDisableTasksDetails(false);
                    break;
                case "command":
                    EnableDisableTasksDetails(true);
                    break;
                case "sh command":
                    EnableDisableTasksDetails(true);
                    break;
            }
        }
        private void EnableDisableTasksDetails(bool isCommand)
        {
            //btnBrowse.Enabled = !isCommand;
            //txtScriptFile.Enabled = !isCommand;
            btnAddParam.Enabled = !isCommand;
            txtCommand.Enabled = isCommand;
            txtArgs.Enabled = isCommand;
            //txtWorkingDir.Enabled = isCommand;
        }
        //private void lblUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    if (lblUrl.Text != "---")
        //    {
        //        ProcessStartInfo sInfo = new ProcessStartInfo(lblUrl.Text);
        //        Process.Start(sInfo);
        //    }
        //}

        //  private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        //   {
        //fetch the subcategories and bind the intended one
        //GetAllSubCategoriesResMsg response = client.ServiceChannel.GetAllSubCategories(cmbCategory.SelectedValue.ToString());
        //if (response.SubCategories != null && response.SubCategories.Count > 0)
        //{
        //    cmbSubCategory.DataSource = response.SubCategories;
        //    cmbSubCategory.DisplayMember = "Name";
        //    cmbSubCategory.ValueMember = "CategoryId";
        //    if (!string.IsNullOrEmpty(_subCategory))
        //    {
        //        response.SubCategories.ForEach(sc =>
        //        {
        //            if (sc.CategoryId.ToString() == _subCategory)
        //            {
        //                cmbSubCategory.SelectedValue = sc.CategoryId;
        //            }
        //        });
        //    }
        //}
        //else
        //    cmbSubCategory.DataSource = null;
        //  }
        private void ToggleVisibility(bool value)
        {
            //label16.Visible = value;
            //label16.Visible = value;
            label18.Visible = value;
            txtCommand.Visible = value;
            txtArgs.Visible = value;
            label16.Visible = value;
            pnlArgOption.Visible = value;
            label20.Visible = !value;
            txtWorkingDir.Visible = !value;
        }
        private void cmbTaskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var scriptType = cmbTaskType.SelectedItem.ToString();
            lblTaskType.Text = scriptType;

            if (scriptType == "File")
            {
                ToggleVisibility(false);
            }
            else
            {
                ToggleVisibility(true);
            }

            UpdateTaskType(lblTaskType.Text);
        }
        private bool ValidationPassed()
        {
            bool isPass = true;
            if ((cmbTaskType.SelectedIndex < 0 || cmbCategory.SelectedIndex < 0) && !publishLocally)
            {
                isPass = false;
                return isPass;
            }

            if (string.IsNullOrEmpty(txtScriptName.Text.Trim()))
            {
                isPass = false;
                return isPass;
            }

            if (Convert.ToString(cmbTaskType.SelectedItem) == "File")
            {
                if (cmbScriptXtn.SelectedItem == null
                    || String.IsNullOrEmpty(cmbScriptXtn.SelectedItem.ToString()))
                {
                    return false;
                }
            }

            //if (lblTaskType.Text.ToLower().Equals("file"))
            //{
            //    if (_script == null && string.IsNullOrEmpty(txtScriptFile.Text))
            //    {
            //        //MessageBox.Show("Please enter command.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        isPass = false;
            //        return isPass;
            //    }
            //}
            //else
            //{
            if (lblTaskType.Text.ToLower().Equals("command"))
            {
                if (string.IsNullOrEmpty(txtCommand.Text))
                {
                    // MessageBox.Show("Please select Script File.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isPass = false;
                    return isPass;
                }
            }
            //}

            return isPass;
        }
        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadScript();
        }
        private void DownloadScript()
        {
            this.Cursor = Cursors.WaitCursor;
            if (!string.IsNullOrEmpty(_scriptUrl))
            {
                FolderBrowserDialog saveScript = new FolderBrowserDialog();
                saveScript.Description = "Select the script download folder:";
                if (saveScript.ShowDialog() == DialogResult.OK)
                {
                    string downloadLoc = saveScript.SelectedPath + "\\" + _downloadScriptname;
                    WebRequest request = WebRequest.Create(_scriptUrl);
                    CredentialCache credential = new CredentialCache();
                    credential.Add(new Uri(_scriptUrl), "NTLM", CredentialCache.DefaultNetworkCredentials);
                    request.Credentials = credential;
                    try
                    {
                        using (WebResponse response = request.GetResponse())
                        {
                            using (FileStream stream = new FileStream(downloadLoc, FileMode.Create, FileAccess.Write))
                            {
                                byte[] bytes = ReadFully(response.GetResponseStream());
                                stream.Write(bytes, 0, bytes.Length);
                            }
                        }
                        var meta = downloadLoc + ".meta";
                        using (StreamWriter sw = new StreamWriter(meta))
                        {
                            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(Models.Script));
                            xml.Serialize(sw, _script);
                        }

                        MessageBox.Show("Script file downloaded.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        string err = "There is an error downloading the Script.";
                        err = err + "\nMore Infomation- " + ex.Message;
                        if (ex.InnerException != null)
                            err = err + ". \nInner Exception- " + ex.InnerException.Message;
                        MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            this.Cursor = Cursors.Default;
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        //private byte[] PackageIfeaFolder(string ifeaFolder)
        //{
        //    byte[] package = null;
        //    if (System.IO.Directory.Exists(ifeaFolder))
        //    {
        //        //zip the content in the provided folder
        //        //and provide the compressed file name same as folder name with ext- .ifeapkg
        //        //and then return the byte array of the so-formed compressed file
        //        Infosys.Compress_DeCompress.Compression compClient = new Compress_DeCompress.Compression();
        //        //get the ifea folder name
        //        string[] folderparts = ifeaFolder.Split(new char[] { '\\' });
        //        string packagename = folderparts[folderparts.Length - 1];
        //        compClient.AddFolder(packagename, ifeaFolder);
        //        package = System.IO.File.ReadAllBytes(packagename);
        //    }
        //    return package;
        //}
        public static List<Infosys.ATR.Entities.Users> Users { get; set; }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            EditScript_Handler();
        }
        internal void EditScript_Handler()
        {
            string content = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(_scriptUrl))
                {
                    WebRequest request = WebRequest.Create(_scriptUrl);
                    CredentialCache credential = new CredentialCache();
                    credential.Add(new Uri(_scriptUrl), "NTLM", CredentialCache.DefaultNetworkCredentials);
                    request.Credentials = credential;
                    using (WebResponse response = request.GetResponse())
                    {
                        switch (_script.ScriptType.ToLower())
                        {
                            case "iapd":
                                FolderBrowserDialog saveScript = new FolderBrowserDialog();
                                saveScript.Description = "Select download folder for the IAPD Package :";
                                if (saveScript.ShowDialog() == DialogResult.OK)
                                {
                                    string downloadLoc = saveScript.SelectedPath + "\\" + _downloadScriptname;
                                    using (FileStream stream = new FileStream(downloadLoc, FileMode.Create, FileAccess.Write))
                                    {
                                        byte[] bytes = ReadFully(response.GetResponseStream());
                                        stream.Write(bytes, 0, bytes.Length);
                                    }

                                    //now unpackage and read the content of the default py file
                                    string extractionLoc = Directory.GetParent(downloadLoc).FullName;
                                    var result = Infosys.ATR.Packaging.Operations.Unpackage(downloadLoc, extractionLoc);
                                    if (result.IsSuccess)
                                    {
                                        string pyFileName = Path.GetFileNameWithoutExtension(downloadLoc);
                                        string pyFilePath = Path.Combine(extractionLoc, pyFileName, pyFileName + ".py");
                                        if (!File.Exists(pyFilePath))
                                            throw new Exception("Expected " + pyFileName + ".py file not found in the package provided.");
                                        else
                                        {
                                            byte[] byteFile = File.ReadAllBytes(pyFilePath);
                                            if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                                            {
                                                if (Encoding.Unicode.GetString(byteFile).Contains(SecurePayload.keyText))
                                                {
                                                    byte[] byteContent = SecurePayload.UnSecureBytes(byteFile);
                                                    Stream ScriptContent = new System.IO.MemoryStream(byteContent);
                                                    content = (new StreamReader(ScriptContent)).ReadToEnd();
                                                }
                                                else
                                                    content = File.ReadAllText(pyFilePath);
                                            }
                                            else
                                                content = File.ReadAllText(pyFilePath);
                                        }
                                    }
                                    else
                                        throw new Exception(result.Message);
                                }
                                else
                                    return;
                                break;
                            default:
                                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                                {
                                    if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                                    {
                                        var bytes = default(byte[]);

                                        using (var memstream = new MemoryStream())
                                        {
                                            var buffer = new byte[1024];
                                            var bytesRead = default(int);
                                            while ((bytesRead = sr.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                                memstream.Write(buffer, 0, bytesRead);
                                            bytes = memstream.ToArray();
                                        }

                                        if (Encoding.Unicode.GetString(bytes).Contains(SecurePayload.keyText))
                                        {
                                            byte[] byteContent = SecurePayload.UnSecureBytes(bytes);
                                            Stream ScriptContent = new System.IO.MemoryStream(byteContent);
                                            content = (new StreamReader(ScriptContent)).ReadToEnd();
                                        }
                                        else
                                            content = (new StreamReader(new System.IO.MemoryStream(bytes)).ReadToEnd());
                                    }
                                    else
                                        content = sr.ReadToEnd();
                                }
                                break;
                        }
                    }
                    EditScript(new object[] { content, _script });
                }
                else
                    throw new Exception("Invalid or missing script/package url");
            }
            catch (Exception ex)
            {
                string err = "There is an error downloading the Script.";
                err = err + "\nMore Infomation- " + ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cmbIsSecret_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbIsSecret.Text))
            {
                if (cmbIsSecret.Text.ToLower().Equals("true"))
                {
                    txtDefaultValue.UseSystemPasswordChar = true;
                }
                else
                {
                    txtDefaultValue.UseSystemPasswordChar = false;
                }
            }
        }
        /// <summary>
        /// This method is used to display textbox for entering command arguments.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event arguments</param>
        private void rdbArgString_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbArgString.Checked)
            {
                label18.Visible = true;
                txtArgs.Visible = true;
                btnAddParam.Enabled = false;
                pnlParameters.Visible = false;
            }
        }
        /// <summary>
        /// This method is used to display parameter panelbox to allow user to enter parameters.
        /// </summary>
        /// <param name="sender">source of event</param>
        /// <param name="e">event arguments</param>
        private void rdbParameters_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbParameters.Checked)
            {
                label18.Visible = false;
                txtArgs.Visible = false;
                btnAddParam.Enabled = true;

                if (_parameters != null && _parameters.Count > 0)
                {
                    btnAddParam_Click(sender, e);
                }
            }
        }
        internal void ShowScriptExtension()
        {
            this.lblScriptExtn.Visible = true;
            this.cmbScriptXtn.Visible = true;
        }
        internal void DisableRun()
        {
            //this.btnRunScript.Enabled = false;
        }
        private void cmbScriptXtn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbScriptXtn.Text.Equals("ps1"))
            {
                lblDataType.Visible = true;
                cmbDataType.Visible = true;
                //if (cmbScriptXtn.Text.Equals("ps1"))
                //    FillPowerShellDataTypes("ps1");
                FillNativeDataTypes("ps1");
            }
            else if (cmbScriptXtn.Text.Equals("py") || cmbScriptXtn.Text.Equals("iapd"))
            {
                lblDataType.Visible = true;
                cmbDataType.Visible = true;
                lblIFEA.Visible = true;
                txtIfeaScriptName.Visible = true;
                lblIFEA.Text = "Function Name";
                FillNativeDataTypes("py"); //same for iapd
                toolTip1.SetToolTip(txtIfeaScriptName, "Enter name of function to be invoked by Script execution engine");

                label4.Visible = false;
                txtScriptFile.Visible = false;
                btnBrowse.Visible = false;
            }
            //else
            //{
            //    lblDataType.Visible = false;
            //    cmbDataType.Visible = false;
            //}

            else if (cmbScriptXtn.Text.Equals("iap"))
            {
                lblIFEA.Visible = true;
                lblIFEA.Text = "IFEA Script";
                txtIfeaScriptName.Visible = true;
                label4.Visible = true;
                txtScriptFile.Visible = true;
                btnBrowse.Visible = true;
            }
            else
            {
                lblIFEA.Visible = false;
                txtIfeaScriptName.Visible = false;
                label4.Visible = false;
                txtScriptFile.Visible = false;
                btnBrowse.Visible = false;

                lblDataType.Visible = false;
                cmbDataType.Visible = false;
            }
        }
        //private void RetrieveDataTypes(string scriptType)
        //{
        //    hsDataTypes = new Hashtable();
        //    switch (scriptType)
        //    {
        //        case "ps1":
        //            hsDataTypes.Add(scriptType + "01", "string");
        //            hsDataTypes.Add(scriptType + "02", "char");
        //            hsDataTypes.Add(scriptType + "03", "byte");
        //            hsDataTypes.Add(scriptType + "04", "int");
        //            hsDataTypes.Add(scriptType + "05", "long");
        //            hsDataTypes.Add(scriptType + "06", "bool");
        //            hsDataTypes.Add(scriptType + "07", "decimal");
        //            hsDataTypes.Add(scriptType + "08", "single");
        //            hsDataTypes.Add(scriptType + "09", "short");
        //            hsDataTypes.Add(scriptType + "10", "double");
        //            hsDataTypes.Add(scriptType + "11", "datetime");
        //            break;
        //        case "py":
        //            break;
        //        default:
        //            break;
        //    }

        //    //return hsDataTypes;
        //}
        private void FillNativeDataTypes(string scriptType)
        {

            BindingSource bs = new BindingSource();
            Hashtable hsDataTypes = NativeDataTypes.RetrieveDataTypes(scriptType);
            bs.DataSource = hsDataTypes;
            cmbDataType.DataSource = bs;
            cmbDataType.DisplayMember = "Value";
            cmbDataType.ValueMember = "Key";
            cmbDataType.SelectedValue = scriptType + "01";

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
        private void DisableControls(Control ctrl, bool enable)
        {
            if (ctrl.Controls.Count > 0)
            {
                foreach (Control subControl in ctrl.Controls)
                    DisableControls(subControl, enable);
            }
            else
            {
                if (ctrl.GetType().Equals(typeof(TextBox)))
                    ((TextBox)ctrl).ReadOnly = !enable;
                else if (ctrl.GetType().Equals(typeof(Button)))
                    ctrl.Enabled = enable;
                else if (ctrl.GetType().Equals(typeof(RadioButton)))
                    ctrl.Enabled = false;
                else if (ctrl.GetType().Equals(typeof(CheckBox)))
                    ctrl.Enabled = enable;
                else if (ctrl.GetType().Equals(typeof(ComboBox)))
                    ctrl.Enabled = enable;
            }
        }
        //private void FillPowerShellDataTypes(string scriptType)
        //{

        //    BindingSource bs = new BindingSource();
        //    RetrieveDataTypes(scriptType);
        //    //Hashtable hsDataTypes = RetrieveDataTypes(scriptType);
        //    bs.DataSource = hsDataTypes;
        //    cmbDataType.DataSource = bs;
        //    cmbDataType.DisplayMember = "Value";
        //    cmbDataType.ValueMember = "Key";
        //    cmbDataType.SelectedValue = scriptType + "01";

        //}
        public bool OpenedFromRepository { get; set; }
        public MainRepositoryView MainRepoView { get; set; }
        public ScriptDesigner ScriptDesigner { get; set; }

        private void btnScriptLocation_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(moduleLocation))
            {
                if (Directory.Exists(moduleLocation))
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        Arguments = moduleLocation,
                        FileName = "explorer.exe"
                    };
                    System.Diagnostics.Process.Start(startInfo);
                }
            }
        }
    }
    /// <summary>
    /// Class containing categoryid and categoryname
    /// </summary>
    public class SubCategory
    {
        public string SubCategoryName { get; set; }
        public object SubCategoryId { get; set; }

        public override string ToString()
        {
            return SubCategoryName;
        }
    }

}
