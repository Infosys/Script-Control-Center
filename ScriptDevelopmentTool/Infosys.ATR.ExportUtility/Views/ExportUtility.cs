using Infosys.ATR.ExportUtility.Models;
using Infosys.WEM.Export.Service.Contracts.Data;
using Infosys.WEM.Export.Service.Contracts.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEMClient = Infosys.WEM.Client;
using Infosys.ATR.ExportUtility.Views;
using System.Security;
using Infosys.WEM.SecureHandler;
using System.Configuration;
using Infosys.ATR.Entities;
using System.Net;
using Infosys.ATR.ExportUtility.Service;
using Infosys.ATR.ScriptRepository.Models;
using Newtonsoft.Json;
using Infosys.Nia.Services;

namespace Infosys.ATR.ExportUtility
{
    public partial class ExportUtility : Form
    {
        //WEMClient.ECRServices ecrServicesClient = new WEMClient.ECRServices();
        private List<Models.NIACategory> ecrCategories = new List<Models.NIACategory>();
        private List<Models.NIACategory> ecrCategoriesWithParentId = new List<Models.NIACategory>();
        List<Script> scripts = new List<Script>();
        TreeNode ecr_root;
        private const string ImageKey = "CatImage";
        private const string ImageKeyScript = "ScriptImage";
        private const string ScriptKey = "ScriptImage";

        internal List<Infosys.ATR.ScriptRepository.Models.Category> categories = null;
        WEMClient.CommonRepository commonRepositoryClient = new WEMClient.CommonRepository();
        WEMClient.ScriptRepository scriptClient = new WEMClient.ScriptRepository();
        WEMClient.ExportRepository exportClient = new WEMClient.ExportRepository();
        WEMClient.SecurityAccess securityClient = new WEMClient.SecurityAccess();
        string companyId;
        //bool isSuperAdmin;
        TreeNode _root;
        internal static List<int> userCatList = null;

        Models.NIACategory destCat = null;
        Infosys.ATR.ScriptRepository.Models.Category sourceCat = null;
        Script sourceScript = null;

        //private ExportScriptPresenter _presenter = null;
        ImageList treeList = new ImageList();
        string sourceParentCatPath = "$";
        string targetParentCatPath = "$";
        int targetCatId = 0;
        int maxECRCatId = 0;
        Boolean loadECRScriptsfromDB = true;

        List<Models.ExportServerDetails> serverDetails = null;
        // AddExportScriptConfigurationDetailsReqMsg scriptConfigurationDetails = new AddExportScriptConfigurationDetailsReqMsg();

        List<ScriptData> scriptData = null;
        List<ScriptDetails> scriptsTobeAdded = null;
        List<Models.ECR.NIAScript> niaScripts = null;
        ECRService ecrService = null;

        Boolean serverTypeFirst = false;
        Boolean serverInstanceFirst = false;
        int defaultType;
        string selectedSourceCategoryName = "";
        string targetParentAllCatPath = "$";
        List<string> targetCatPathList = null;
        //string str = "$";
        // Past Exports
        Boolean isSuperAdmin = false;
        //int srno;
        List<PastExportConfigurationDetails> pastExportScriptDetails = null;
        BackgroundWorker bgw = new BackgroundWorker();
        public ExportUtility()
        {
            InitializeComponent();
        }

        private void ExportUtility_Load(object sender, EventArgs e)
        {
            InitilizeData();
            //PastExports();
        }
        private void InitilizeData()
        {
            companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
            scriptData = new List<ScriptData>();
            scriptsTobeAdded = new List<ScriptDetails>();
            niaScripts = new List<Models.ECR.NIAScript>();
            destCat = new Models.NIACategory();
            sourceCat = new Infosys.ATR.ScriptRepository.Models.Category();
            sourceScript = new Script();
            AddTreeViewImage();
            LoadCategory();
            serverTypeFirst = true;
            serverInstanceFirst = true;
            GetTargetSystemDetails();
            serverTypeFirst = false;
            serverInstanceFirst = false;
            //cmbServerType.Focus();
            //this.cmbServerType.SelectedValue = defaultType;
            cmbServerType.SelectedIndex = defaultType;
            GetServerDetails(cmbServerType.SelectedValue.ToString());
            cmbServerInstance.SelectedIndex = -1;
            // GetLoginDetails(cmbServerType.SelectedValue.ToString());
            SetToolTip();
            tvSource.ImageList = treeList;
            tvDestination.ImageList = treeList;
            DisableControls();
            btnExport.Enabled = false;
            this.CenterToScreen();
        }
        private void AddTreeViewImage()
        {
            treeList = new ImageList();
            Image img1 = Image.FromFile(@"Images\Folder.png");
            treeList.Images.Add(ImageKey, img1);
            Image img2 = Image.FromFile(@"Images\script.png");
            treeList.Images.Add(ImageKeyScript, img2);

            //if (treeList.Tag != null)
            //{
            //    //if (treeList.Tag.ToString().Equals("Infosys.ATR.ScriptRepository.Models.Category"))
            //        treeList.Images.Add(ImageKey, img1);
            //   // else
            //        //treeList.Images.Add(ImageKey, img2);
            //    tvSource.ImageList = treeList;
            //    tvDestination.ImageList = treeList;
            //}
        }
        private void GetTargetSystemDetails()
        {
            List<Infosys.ATR.ExportUtility.Models.TargetSystemDetails> targetSystems = new List<Infosys.ATR.ExportUtility.Models.TargetSystemDetails>();
            Infosys.WEM.Export.Service.Contracts.Message.GetExportTargetSystemDetailsResMsg serverResponse = exportClient.ServiceChannel.GetExportTargetSystemDetails();
            targetSystems = Translators.TargetSystemDetailsSE_PE.TargetSystemDEtoSE(serverResponse.ExportTargetSystemDetails);

            this.cmbServerType.DataSource = targetSystems;
            this.cmbServerType.DisplayMember = "Name";
            this.cmbServerType.ValueMember = "Id";
            //this.cmbServerType.SelectedIndexChanged += new EventHandler(cmbServerType_SelectedIndexChanged);
            //defaultType = targetSystems.Where(a => a.DefaultType == true)
            //           .Select(a => a.id)
            //           .First().ToString();
            defaultType = targetSystems.FindIndex(a => a.DefaultType == true);


        }
        private void btnGo_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Value = 10;
            if (ValidateLoginValues())
            {             
                if (scriptData != null && scriptData.Count > 0)
                {
                    scriptData.Clear();
                    scriptData = new List<ScriptData>();
                }
                try
                {
                    progressBar1.Value = 20;
                    string securePassword = SecurePayload.Secure(Convert.ToString(txtPassword.Text), "IAP2GO_SEC!URE");
                    ecrService = ECRService.InstanceCreation();
                    progressBar1.Value = 40;
                    var client = ecrService.getInstance(txtUserId.Text, securePassword, txtCasServer.Text, txtTargetServer.Text);
                    progressBar1.Value = 50;
                    var response = ecrService.BrowseScriptCategory(txtTargetServer.Text);
                    progressBar1.Value = 75;
                    this.ecrCategories = Translators.ECRCategorySE_PE.CategoryListSEtoPE(response.rootCategories);
                    this.ecrCategoriesWithParentId = Translators.ECRCategorySE_PE.CategoryListSEtoPEWithParentId(response.rootCategories);

                    PopulateECRCategories();
                    loadECRScriptsfromDB = false;
                    btnExport.Enabled = true;
                    progressBar1.Value = 100;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString() + ex.Message);
                }
            }
        }


        private void SetToolTip()
        {
            toolTip1.SetToolTip(cmbServerType, "Select server type");
            toolTip1.SetToolTip(txtCasServer, "Enter Cas Server name/ip along with port e.g server:port");
            toolTip1.SetToolTip(txtTargetServer, "Enter Target Server name/ip along with port e.g server:port");
            toolTip1.SetToolTip(txtUserId, "Enter Nia user id");
            toolTip1.SetToolTip(txtPassword, "Enter Nia password");
            toolTip1.SetToolTip(btnGo, "Click this button to fecth scripts from Nia");
            toolTip1.SetToolTip(tvSource, "Click to select source category/script");
            toolTip1.SetToolTip(tvDestination, "Click to select destination category/script");
            toolTip1.SetToolTip(btnAdd, "Click this button to Add script(s) from source tree to destination tree view");
            //toolTip1.SetToolTip(btnRemove, "Click this button to Remove selected category/script(s) from destination tree view");
            toolTip1.SetToolTip(btnExport, "Click this button to submit scripts for Export process");

        }
        private void PopulateECRCategories()
        {
            tvDestination.Nodes.Clear();
            ecr_root = new TreeNode("Categories");
            this.tvDestination.Nodes.Add(ecr_root);
            ecr_root.Expand();
            this.ecrCategories.ForEach(sg =>
                {
                    AddECRNode(sg, null);
                });

            this.tvDestination.SelectedNode = ecr_root;
            this.tvDestination.ExpandAll();

        }

        private void AddECRNode(Infosys.ATR.ExportUtility.Models.NIACategory c, TreeNode parent)
        {
            if (parent == null)
            {
                parent = new TreeNode();
                parent.Text = c.Name;
                parent.Tag = c;
                //parent.ImageKey = ImageKey;
                ecr_root.Nodes.Add(parent);
            }
            else
            {
                TreeNode child = new TreeNode();
                child.Text = c.Name;
                child.Tag = c;
                //child.ImageKey = ImageKey;
                parent.Nodes.Add(child);
                parent = child;
            }
            if (Convert.ToInt32(c.Id) > maxECRCatId)
                maxECRCatId = Convert.ToInt32(c.Id);
            if (c.Children != null && c.Children.Count > 0)
            {
                foreach (var cat in c.Children)
                {
                    AddECRNode(cat, parent);
                    //parent.Tag = cat;
                    //parent.Nodes.Add(cat.Name);
                }
            }
            parent.ImageIndex = 0;
            parent.SelectedImageIndex = 0;

            if (chkRetrieveScripts.Checked && loadECRScriptsfromDB == true)
                GetECRScriptsFromDB(c, parent);
            //if (chkRetrieveScripts.Checked && loadECRScriptsfromDB==false)
            //    LoadSECRAllScripts(c, parent);
        }

        private void GetECRScriptsFromDB(Infosys.ATR.ExportUtility.Models.NIACategory c, TreeNode parent)
        {
            //GetScriptByCategoryReqMsg scriptRequest = new GetScriptByCategoryReqMsg();
            ////scriptRequest.ParentCategoryId = 2;
            //scriptRequest.Login = new WEM.Nia.Service.Contracts.Data.LoginDetails();
            //scriptRequest.Login.CasServerAddr = txtCasServer.Text;
            //scriptRequest.Login.ECRServerAddr = txtTargetServer.Text;
            //scriptRequest.Login.UserName = txtUserId.Text;
            //scriptRequest.Login.Password = txtPassword.Text;


            Dictionary<string, string> requestMap = new Dictionary<string, string>();
            Dictionary<string, string> pathVariableMap = new Dictionary<string, string>();
            PathVariablesVO pathVariablesVO = new PathVariablesVO();
            List<string> seriveAreas = new List<string>();

            requestMap.Add("Content-Type", "application/json");
            pathVariableMap.Add("categoryId", c.Id);
            pathVariablesVO.setPathVariableMap(pathVariableMap);
            //Console.WriteLine("SERVICEAREA:" + Constants.Application.serviceArea);
            seriveAreas.Add(Constants.Application.ServiceArea);
            pathVariablesVO.setServiceAreas(seriveAreas);
            //scriptRequest.RequestHeaderMap = requestMap;
            string data = JsonConvert.SerializeObject(pathVariablesVO);
            //scriptRequest.Data = data;
            // TODO - remove hardcoding of category id 
            List<Models.ECR.NIAScript> resp = null;
            if (int.Parse(c.Id) < 3)
                resp = ecrService.GetAllScriptsByCategoryId(txtTargetServer.Text, data, requestMap); ;

            if (resp != null && resp.Count > 0)
            {

                foreach (Models.ECR.NIAScript script in resp)
                {
                    TreeNode child = new TreeNode();
                    child.Text = script.scriptName;
                    child.Tag = script;
                    //child.ImageKey = ImageKeyScript;
                    parent.Nodes.Add(child);

                    child.ImageIndex = 1;
                    child.SelectedImageIndex = 1;

                    niaScripts.Add(script);

                    ScriptData item = new ScriptData();
                    item.SourceCategoryId = Convert.ToInt32(c.Id);
                    //item.SourceScriptPath = "";// FindSourceParentCategoryPath(sourceCat);
                    item.TargetCategoryId = Convert.ToInt32(c.Id);// Convert.ToInt32(destCat.Id);
                                                                  //item.NiaTargetCategoryId = Convert.ToInt32(c.Id);
                                                                  //item.TargetScriptPath = targetParentCatPath;// + "\\" + destCat.Name;
                    item.SourceScriptId = script.scriptId;
                    item.ScriptName = script.scriptName;
                    item.Added = false;
                    scriptData.Add(item);

                }
            }

        }

        public void LoadCategory(List<Models.Category> categories = null)
        {
            try
            {
                var alias = System.Threading.Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
                alias = SecurePayload.Secure(alias, "IAP2GO_SEC!URE");
                bool overrideSecurity = false;

                overrideSecurity = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideSecurity"]);
                var security = "";

                if (!overrideSecurity)
                {
                    security = "AllowAuthorised";
                }
                else
                {
                    security = "AllowAll";
                }

                //this.categories.Clear();
                if (this.categories == null)
                {
                    Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response =
                        commonRepositoryClient.ServiceChannel.GetAllCategoriesByCompany(companyId, Infosys.ATR.ExportUtility.Constants.Application.ModuleID);

                    this.categories = Infosys.ATR.ScriptRepository.Translators.CategoryTreePE_SE.CategoryListSEtoPE(response.Categories);

                    if (this.categories != null)
                    {

                        Infosys.WEM.SecurityAccess.Contracts.Message.IsSuperAdminResMsg result = securityClient.ServiceChannel.IsSuperAdmin(alias, companyId);
                        isSuperAdmin = result.IsSuperAdmin;
                        if (!result.IsSuperAdmin)
                        {
                            if (security.Equals("AllowAuthorised"))
                            {
                                var resp = securityClient.ServiceChannel.GetUsers(alias, companyId);

                                List<Users> users = new List<Users>();
                                List<int> userCategory = new List<int>();
                                resp.Users.ForEach(u =>
                                {
                                    Users _user = new Users();
                                    _user.Alias = u.Alias;
                                    _user.Role = Enum.GetName(typeof(Roles), u.Role);
                                    _user.DisplayName = u.DisplayName;
                                    _user.CategoryId = u.CategoryId;
                                    _user.GroupId = u.GroupId.GetValueOrDefault();
                                    _user.Id = u.UserId;
                                    users.Add(_user);
                                    userCategory.Add(u.CategoryId);
                                });

                                userCatList = userCategory;
                                if (userCategory != null)
                                {
                                    this.categories = response.Categories.Where(c => userCategory.Contains(Convert.ToInt32(c.CategoryId))).
                                 Select(c => new Infosys.ATR.ScriptRepository.Models.Category
                                 {
                                     Description = c.Description,
                                     Id = c.CategoryId.ToString(),
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

        private void PopulateCategories()
        {
            _root = new TreeNode("Categories");
            this.tvSource.Nodes.Add(_root);
            _root.Expand();
            categories.Where(g => g.ParentId == 0).ToList().
                ForEach(sg =>
                {
                    AddNode(sg, null);
                });

            this.tvSource.SelectedNode = _root;
        }

        private void AddNode(Infosys.ATR.ScriptRepository.Models.Category c, TreeNode parent)
        {
            if (parent == null)
            {
                parent = new TreeNode();
                parent.Text = c.Name;
                parent.Tag = c;
                //parent.ImageKey = ImageKey;
                _root.Nodes.Add(parent);
            }
            else
            {
                TreeNode child = new TreeNode();
                child.Text = c.Name;
                child.Tag = c;
                //child.ImageKey = ImageKey;
                parent.Nodes.Add(child);
                parent = child;
            }
            parent.ImageIndex = 0;
            parent.SelectedImageIndex = 0;

            //Image img1 = Image.FromFile(@"Images\Folder.png");
            //    treeList.Images.Add(ImageKey, img1);

            categories.Where(sc => sc.ParentId == Convert.ToInt32(c.Id)).ToList().ForEach(child =>
        {
            AddNode(child, parent);
        });

            LoadScripts(c, parent);
        }
        private void LoadScripts(Infosys.ATR.ScriptRepository.Models.Category c, TreeNode parent)
        {
            var response = scriptClient.ServiceChannel.GetAllScriptDetails(c.Id);
            if (response.Scripts != null)
            {
                List<Infosys.ATR.ScriptRepository.Models.Script> catScripts = Infosys.ATR.ScriptRepository.Translators.ScriptPE_SE.ScriptListSEtoPE(response.Scripts.ToList());
                if (catScripts != null && catScripts.Count > 0)
                {
                    foreach (Infosys.ATR.ScriptRepository.Models.Script script in catScripts)
                    {
                        TreeNode child = new TreeNode();
                        child.Text = script.Name;
                        child.Tag = script;
                        //child.ImageKey = ImageKeyScript;
                        parent.Nodes.Add(child);

                        child.ImageIndex = 1;
                        child.SelectedImageIndex = 1;

                        scripts.Add(script);
                    }
                }
            }
        }
        private void tvSource_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvSource.SelectedNode.Tag != null)
            {
                if (tvSource.SelectedNode.Tag.ToString().Equals("Infosys.ATR.ScriptRepository.Models.Category"))
                {
                    Infosys.ATR.ScriptRepository.Models.Category c = this.tvSource.SelectedNode.Tag as Infosys.ATR.ScriptRepository.Models.Category;
                    if (c != null)
                    {
                        sourceCat = c;
                    }
                }
                else if (tvSource.SelectedNode.Tag.ToString().Equals("Infosys.ATR.ScriptRepository.Models.Script"))
                {
                    Infosys.ATR.ScriptRepository.Models.Script c = this.tvSource.SelectedNode.Tag as Infosys.ATR.ScriptRepository.Models.Script;
                    if (c != null)
                    {
                        sourceScript = c;
                    }
                }
            }
        }

        private void tvDestination_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvDestination.SelectedNode.Tag != null)
            {
                if (tvDestination.SelectedNode.Tag.ToString().Equals("Infosys.ATR.ExportUtility.Models.NIACategory"))
                {
                    Infosys.ATR.ExportUtility.Models.NIACategory c = this.tvDestination.SelectedNode.Tag as Infosys.ATR.ExportUtility.Models.NIACategory;
                    if (c != null)
                    {
                        destCat = c;
                    }
                }
                else if (tvDestination.SelectedNode.Tag.ToString().Equals("Infosys.ATR.ExportUtility.Models.NIAScript"))
                {
                    destCat = null;
                    MessageBox.Show("Please sleect parent category under which source script needs to be added.", "Warning.");
                }
            }
        }

        private void DeleteNode(IList<NIACategory> nodes, NIACategory cat)
        {
            NIACategory nodeToDelete = null;
            foreach (var node in nodes)
            {
                if (node.Equals(cat))
                {
                    nodeToDelete = node;
                    break;
                }
                if (node.Children != null && node.Children.Count > 0)
                    DeleteNode(node.Children, cat);
            }
            if (nodeToDelete != null)
            {
                nodes.Remove(nodeToDelete);
            }
        }

        private string FindSourceParentCategoryPath(Infosys.ATR.ScriptRepository.Models.Category cat)
        {
            if (cat.ParentId > 0)
            {
                var findParentCategory = this.categories.Where(c => c.Id == cat.ParentId.ToString()).FirstOrDefault();
                if (findParentCategory != null)
                {
                    FindSourceParentCategoryPath(findParentCategory);
                    sourceParentCatPath = sourceParentCatPath + "\\" + findParentCategory.Name;
                }
            }
            return sourceParentCatPath;
        }

        private void FindTargetPath(NIACategory cat)
        {
            NIACategory findCat = this.ecrCategoriesWithParentId.Where(c => c.Id == cat.ParentId.ToString()).FirstOrDefault();
            if (findCat != null)
            {
                targetCatPathList.Add(findCat.Name);
                FindTargetPath(findCat);
            }
        }

        private void PopulateECRScripts()
        {
            foreach (var script in scriptData)
            {
                //if (script.Added)
                {
                    TreeNode selectedNode = null;
                    foreach (TreeNode node in tvDestination.Nodes)
                    {
                        selectedNode = FindTargetCatById(Convert.ToString(script.TargetCategoryId), node);
                        if (selectedNode != null) break;
                    }

                    TreeNode child = new TreeNode();
                    child.Text = script.ScriptName;
                    child.Tag = script;
                    //child.ImageKey = ImageKeyScript;
                    selectedNode.Nodes.Add(child);

                    child.ImageIndex = 1;
                    child.SelectedImageIndex = 1;
                    //script.Added = false;
                }
            }
        }

        private string FindTargetParentCategoryPath(NIACategory cat)
        {
            foreach (NIACategory ecrCategory in this.ecrCategories)
            {
                if (ecrCategory.Equals(cat))
                {
                    return targetParentCatPath;
                }
                else
                {
                    if (ecrCategory.Children != null && ecrCategory.Children.Count > 0)
                    {
                        if (ecrCategory.Children.Contains(cat))
                        {
                            FindTargetParentCategoryPath(ecrCategory);
                            targetParentCatPath = targetParentCatPath + "\\" + ecrCategory.Name;
                        }
                        //else
                        //{
                        //    foreach (NIACategory c in ecrCategory.Children.ToList())
                        //    {
                        //        FindTargetParentCategoryPath(c);
                        //        targetParentCatPath = targetParentCatPath + "\\" + ecrCategory.Name;
                        //    }
                        //}
                    }
                }
            }
            return targetParentCatPath;
        }

        private string FindTargetParentCategoryPath(IList<NIACategory> nodes, NIACategory cat)
        {
            foreach (NIACategory ecrCategory in nodes)
            {
                if (ecrCategory.Equals(cat))
                {
                    //return targetParentCatPath;
                }
                else
                {
                    if (ecrCategory.Children != null && ecrCategory.Children.Count > 0)
                    {
                        if (ecrCategory.Children.Contains(cat))
                            //FindTargetParentCategoryPath(ecrCategory);
                            targetParentCatPath = targetParentCatPath + "\\" + ecrCategory.Name;
                        else
                            FindTargetParentCategoryPath(ecrCategory.Children, cat);
                    }
                }
            }


            return targetParentCatPath;
        }

        private string getparent(NIACategory cat, List<NIACategory> listtocheck)
        {
            foreach (var item in listtocheck)
            {
                if (item.Children == null) continue;
                if (item.Children.Count > 0)
                {
                    if (item.Children.Contains(cat))
                    {
                        targetParentAllCatPath = targetParentAllCatPath + "\\" + item.Name + "\\";
                        //else
                        getparent(item, item.Children);
                    }
                }
            }
            return targetParentAllCatPath;
        }

        private TreeNode FindTargetCatById(string catId, TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                if (node.Tag.ToString().Equals("Infosys.ATR.ExportUtility.Models.NIACategory"))
                {
                    if ((node.Tag as NIACategory).Id.Equals(catId)) return node;
                    TreeNode next = FindTargetCatById(catId, node);
                    if (next != null) return next;
                }
            }
            return null;
        }
        public static string GetCatPath(string path, string cat)
        {
            string destPath = "";
            if (path.Contains(cat))
            {
                int index = path.IndexOf(cat);
                destPath = path.Substring(index);
            }

            return destPath;
        }
        private void AddAllScripts(ScriptRepository.Models.Category sourceCat, int index)
        {
            var catScripts = this.scripts.Where(s => s.CategoryId.Equals(sourceCat.Id)).ToList();
            if (catScripts != null && catScripts.Count > 0)
            {
                string scriptPath = FindSourceParentCategoryPath(sourceCat) + "\\" + sourceCat.Name;
                string selectedPath = GetCatPath(scriptPath, selectedSourceCategoryName);
                string destPath = "$";
                if (destCat == null || string.IsNullOrEmpty(destCat.Name))
                    destPath = "$" + "\\" + selectedPath;
                else
                    destPath = FindTargetParentCategoryPath(destCat) + "\\" + destCat.Name + "\\" + selectedPath;
                foreach (Script catScript in catScripts)
                {
                    ScriptData item = new ScriptData();
                    item.SourceCategoryId = Convert.ToInt32(sourceCat.Id);
                    item.SourceScriptPath = scriptPath; // FindSourceParentCategoryPath(sourceCat);
                    item.TargetCategoryId = index;// Convert.ToInt32(destCat.Id);
                    item.TargetScriptPath = destPath;// + "\\" + destCat.Name;
                    //item.SourceScriptId = Convert.ToInt32(catScript.Id);
                    item.ScriptName = catScript.Name;
                    //item.NiaTargetCategoryId = NiaTargetCategoryId;
                    //item.ActualSourceCategoryId = actualSourceCategoryId;
                    //item.SourceParentCategory = sourceParentCategory;
                    item.Added = true;
                    scriptData.Add(item);
                    //sourceParentCategory = false;
                }
                sourceParentCatPath = "$";
                targetParentCatPath = "$";
                //maxECRCatId = maxECRCatId + 1;
            }
        }

        //private void AddAllScripts(ScriptRepository.Models.Category sourceCat, int targetId)
        //{
        //    var catScripts = this.scripts.Where(s => s.CategoryId.Equals(sourceCat.Id)).ToList();
        //    var firstItem = ecrCategories.ElementAt(targetId);
        //    if (catScripts != null && catScripts.Count > 0)
        //    {
        //        string scriptPath= FindSourceParentCategoryPath(sourceCat) + "\\" + sourceCat.Name;
        //        string destPath = FindTargetParentCategoryPath(destCat) + "\\" + destCat.Name;
        //        foreach (Script catScript in catScripts)
        //        {
        //            ScriptData item = new ScriptData();
        //            item.SourceCategoryId = Convert.ToInt32(sourceCat.Id);
        //            item.SourceScriptPath = scriptPath; // FindSourceParentCategoryPath(sourceCat);
        //            item.TargetCategoryId = Convert.ToInt32(firstItem.Id); //targetCatId;// Convert.ToInt32(destCat.Id);
        //            item.TargetScriptPath = destPath;// + "\\" + destCat.Name;
        //            item.SourceScriptId = Convert.ToInt32(catScript.Id);
        //            item.ScriptName = catScript.Name;
        //            item.Added = true;
        //            scriptData.Add(item);
        //        }
        //        sourceParentCatPath = "$";
        //        targetParentCatPath = "$";
        //        //maxECRCatId = maxECRCatId + 1;
        //    }
        //}

        //private void FindAllChildCategories(Infosys.ATR.ScriptRepository.Models.Category sourceCat)
        //{
        //    AddAllScripts(sourceCat);
        //    var findChildCategory = this.categories.Where(c => c.ParentId.ToString().Equals(sourceCat.Id)).ToList();
        //    if (findChildCategory != null && findChildCategory.Count > 0)
        //    {
        //        foreach (var cat in findChildCategory)
        //        {
        //            //maxECRCatId = maxECRCatId + 1;
        //            FindAllChildCategories(cat);
        //        }

        //    }
        //    //else
        //    //{
        //    //    AddAllScripts(sourceCat);
        //    //}
        //}

        private void btnAdd_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            sourceParentCatPath = "$";
            targetParentCatPath = "$";
            targetCatId = 0;
            selectedSourceCategoryName = "";
            targetParentAllCatPath = "$";
            string targetCatCompletePath = "$";
            targetCatPathList = new List<string>();

            // If target root node selected then user can add only categories
            if (tvDestination.SelectedNode.Text.Equals("Categories"))
            {
                // Not allow to add script under root category. It must be added to category/subcateogry.
                if (sourceScript != null && Convert.ToInt32(sourceScript.Id) > 0)
                {
                    MessageBox.Show("Please select target category under which script needs to be exported.", "Validation");
                    //sourceScript = null;
                }
                else if (sourceCat == null)
                {
                    MessageBox.Show("Please select source category which needs to be exported.", "Validation");
                    sourceCat = null;
                }
                // If source category selected
                else if (sourceCat != null)
                {
                    selectedSourceCategoryName = sourceCat.Name;

                    ScriptDetails details = new ScriptDetails();
                    details.SourceCategoryId = int.Parse(sourceCat.Id);
                    details.SourceScriptPath = FindSourceParentCategoryPath(sourceCat) + "\\" + sourceCat.Name;
                    //details.TargetCategoryId = int.Parse(destCat.Id);
                    details.TargetScriptPath = targetParentCatPath; //FindTargetParentCategoryPath(destCat) + "\\" + destCat.Name;
                    //details.SourceScriptId = int.Parse(sourceScript.Id);
                    //details.ScriptName = sourceScript.Name;
                    scriptsTobeAdded.Add(details);

                    sourceParentCatPath = "$";
                    targetParentCatPath = "$";

                    int index = ecrCategories.Count;
                    //index = index + 1;
                    maxECRCatId = maxECRCatId + 1;
                    //AddAllScripts(sourceCat,0);
                    Infosys.ATR.ExportUtility.Models.NIACategory c = new Infosys.ATR.ExportUtility.Models.NIACategory();
                    c.Id = Convert.ToString(maxECRCatId);
                    c.Name = sourceCat.Name;
                    c.Children = AddChildCategories(sourceCat, ecrCategories.Count);
                    //if (destCat != null)
                    //{
                    //    c.TargetCategoryId = destCat.Id;
                    //}
                    //else
                    //c.TargetCategoryId = "0";

                    this.ecrCategories.Insert(index, c);
                    AddAllScripts(sourceCat, maxECRCatId);


                    PopulateECRCategories();
                    PopulateECRScripts();

                }

                sourceScript = null;
                sourceCat = null;
                destCat = null;
            }
            // If target category selected
            else if (destCat != null)
            {
                NIACategory res = ecrCategories.Where(n => n.Name.Equals(destCat.Name)).FirstOrDefault();
                if (res == null)
                {
                    var parentIdCat = this.ecrCategoriesWithParentId.Where(i => i.Name.Equals(destCat.Name)).FirstOrDefault();
                    FindTargetPath(parentIdCat);
                    targetCatPathList.Reverse();
                    foreach (var str in targetCatPathList)
                    {
                        targetCatCompletePath = targetCatCompletePath + "\\" + str;
                    }

                    targetCatCompletePath = targetCatCompletePath + "\\" + destCat.Name;
                }
                else
                {
                    targetCatCompletePath = targetCatCompletePath + "\\" + destCat.Name;
                }

                // If single script is selected then include then add that script to target category
                if (sourceScript != null && Convert.ToInt32(sourceScript.Id) > 0)
                {
                    Infosys.ATR.ScriptRepository.Models.Category cat = this.categories.Where(a => a.Id.Equals(sourceScript.CategoryId)).FirstOrDefault();
                    sourceCat = cat;

                    int index = ecrCategories.Count;


                    ScriptDetails details = new ScriptDetails();
                    details.SourceCategoryId = int.Parse(sourceCat.Id);
                    details.SourceScriptPath = FindSourceParentCategoryPath(sourceCat) + "\\" + sourceCat.Name;
                    details.TargetCategoryId = int.Parse(destCat.Id);
                    //details.TargetScriptPath = FindTargetParentCategoryPath(destCat) + "\\" + destCat.Name;
                    details.TargetScriptPath = targetCatCompletePath;
                    details.SourceScriptId = int.Parse(sourceScript.Id);
                    //details.ScriptName = sourceScript.Name;
                    scriptsTobeAdded.Add(details);

                    sourceParentCatPath = "$";
                    targetParentCatPath = "$";

                    int idx = index + 1;
                    ScriptData item = new ScriptData();
                    item.SourceCategoryId = int.Parse(sourceCat.Id);
                    item.SourceScriptPath = FindSourceParentCategoryPath(sourceCat) + "\\" + sourceCat.Name;
                    item.TargetCategoryId = int.Parse(destCat.Id);

                    item.TargetScriptPath = targetCatCompletePath;//  FindTargetParentCategoryPath(destCat) + "\\" + destCat.Name;
                    item.SourceScriptId = int.Parse(sourceScript.Id);
                    item.ScriptName = sourceScript.Name;
                    //item.NiaTargetCategoryId = int.Parse(destCat.Id);
                    //item.SourceParentCategory = false;
                    //item.ActualSourceCategoryId = int.Parse(sourceCat.Id);
                    item.Added = true;
                    scriptData.Add(item);

                    // Add script to target tree view
                    TreeNode selectedNode = null;
                    foreach (TreeNode node in tvDestination.Nodes)
                    {
                        selectedNode = FindTargetCatById(destCat.Id, node);
                        if (selectedNode != null) break;
                    }

                    if (selectedNode != null)
                    {
                        TreeNode childNode = new TreeNode();
                        childNode.Tag = sourceScript;
                        childNode.Text = sourceScript.Name;
                        childNode.ImageIndex = 1;
                        childNode.SelectedImageIndex = 1;
                        selectedNode.Nodes.Add(childNode);
                    }
                    sourceScript = null;
                }
                // If category is selected then include all the scripts under that category
                else if (sourceCat != null)
                {
                    ScriptDetails details = new ScriptDetails();
                    details.SourceCategoryId = int.Parse(sourceCat.Id);
                    details.SourceScriptPath = FindSourceParentCategoryPath(sourceCat) + "\\" + sourceCat.Name;
                    details.TargetCategoryId = int.Parse(destCat.Id);
                    details.TargetScriptPath = targetCatCompletePath;
                    //details.SourceScriptId = int.Parse(sourceScript.Id);
                    //details.ScriptName = sourceScript.Name;
                    scriptsTobeAdded.Add(details);

                    sourceParentCatPath = "$";
                    targetParentCatPath = "$";

                    // int index = ecrCategories.FindIndex(a => a.Id == destCat.Id);
                    //var item = ecrCategories.Max(x => int.Parse( x.Id));
                    selectedSourceCategoryName = sourceCat.Name;
                    Infosys.ATR.ExportUtility.Models.NIACategory c = new Infosys.ATR.ExportUtility.Models.NIACategory();
                    c.Children = new List<NIACategory>();
                    maxECRCatId = maxECRCatId + 1;
                    c.Id = Convert.ToString(maxECRCatId); //sourceCat.Id;
                    //c.TargetCategoryId = destCat.Id;
                    targetCatId = int.Parse(destCat.Id);
                    //if (destCat != null)
                    //{
                    //    c.TargetCategoryId = destCat.Id;
                    //}
                    //else
                    //    c.TargetCategoryId = "0";

                    c.Name = sourceCat.Name;
                    c.Children = AddChildCategories(sourceCat, ecrCategories.Count);
                    if (destCat.Children == null)
                        destCat.Children = new List<NIACategory>();
                    destCat.Children.Add(c);

                    AddAllScripts(sourceCat, maxECRCatId);

                    PopulateECRCategories();
                    PopulateECRScripts();

                    //AddAllECRScripts(sourceCat, c);
                    //TreeNode selectedNode = null;
                    //foreach (TreeNode node in tvDestination.Nodes)
                    //{
                    //    selectedNode = FindTargetCatById(destCat.Id, node);
                    //    if (selectedNode != null) break;
                    //}        
                    destCat = null;
                }
                sourceCat = null;

            }
            else
            {
                MessageBox.Show("Please select target category.");
            }

        }
        private List<NIACategory> AddChildCategories(Infosys.ATR.ScriptRepository.Models.Category addedCat, int index)
        {
            //AddAllScripts(addedCat, maxECRCatId);
            List<NIACategory> lstECRChildren = new List<NIACategory>();
            NIACategory ecrChildCat = null;
            var resp = this.categories.Where(g => g.ParentId.Equals(Convert.ToInt32(addedCat.Id))).ToList();
            if (resp != null && resp.Count > 0)
            {
                resp.ForEach(sg =>
                {
                    ecrChildCat = new NIACategory();
                    ecrChildCat.Children = new List<NIACategory>();
                    index = index + 1;
                    maxECRCatId = maxECRCatId + 1;
                    ecrChildCat.Id = Convert.ToString(maxECRCatId);
                    ecrChildCat.Name = sg.Name;
                    // ecrChildCat.TargetCategoryId=sg.
                    ecrChildCat.Children = AddChildCategories(sg, index);
                    lstECRChildren.Add(ecrChildCat);

                    AddAllScripts(sg, maxECRCatId);
                    // this.ecrCategories.Insert(index + 1, child);
                });
            }

            return lstECRChildren;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to export the script(s)?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string securePassword = SecurePayload.Secure(Convert.ToString(txtPassword.Text), "IAP2GO_SEC!URE");

                    AddExportConfigurationMasterdetailsReqMsg req = new AddExportConfigurationMasterdetailsReqMsg();
                    req.ExportConfigurationMasterDetails = new WEM.Export.Service.Contracts.Data.ExportConfigurationMasterDetails();
                    req.ExportConfigurationMasterDetails.AutomationServerTypeId = Convert.ToInt32(cmbServerType.SelectedValue);
                    req.ExportConfigurationMasterDetails.AutomationServerIPAddress = txtTargetServer.Text;
                    req.ExportConfigurationMasterDetails.CasServerIPAddress = txtCasServer.Text;
                    req.ExportConfigurationMasterDetails.TargetSystemUserName = txtUserId.Text;
                    req.ExportConfigurationMasterDetails.TargetSystemPassword = securePassword;

                    req.ExportConfigurationMasterDetails.ExportConfigurationDetails = new List<ExportConfigurationScriptDetails>();
                    foreach (ScriptDetails details in scriptsTobeAdded)
                    {
                        ExportConfigurationScriptDetails record = new ExportConfigurationScriptDetails();
                        record.SourceCategoryId = details.SourceCategoryId;
                        record.SourceScriptPath = details.SourceScriptPath;
                        record.TargetScriptPath = details.TargetScriptPath;
                        record.SourceScriptId = details.SourceScriptId;
                        record.TargetCategoryId = details.TargetCategoryId;
                        req.ExportConfigurationMasterDetails.ExportConfigurationDetails.Add(record);
                    }
                    //var data = JsonConvert.SerializeObject(req);
                    //var resp = exportClient.ServiceChannel.AddExportConfigurationDetails(req);
                    var resp = exportClient.ServiceChannel.AddExportConfigurationDetails(req);

                    if (resp.IsSuccess)
                        MessageBox.Show("Scripts would be exported in the next bacth cycle.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        throw new Exception("An error has ocurred. Pls contact the admin.");


                    sourceParentCatPath = "$";
                    targetParentCatPath = "$";
                    scriptData.Clear();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //MessageBox.Show(resp.Id.ToString());
            }
            //InitilizeData();
            //PopulateECRCategories();
            this.Close();
        }

        private void GetServerDetails(string targetServerId)
        {
            this.cmbServerInstance.DataSource = null;

            var exportServerDetails = exportClient.ServiceChannel.GetExportServerDetails(targetServerId,"");
            serverDetails = Translators.ExportServerDetailsSE_PE.ExportServerDetailsSEtoPEList(exportServerDetails.ExportServerDetails.ToList());

            if (serverDetails != null && serverDetails.Count > 0)
            {
                Models.ExportServerDetails det = new Models.ExportServerDetails();
                det.DNSServer = "Specify new server instance";
                det.TargetSystemId = 0;
                serverDetails.Insert(0, det);
                this.cmbServerInstance.DataSource = serverDetails;
                this.cmbServerInstance.DisplayMember = "DNSServer";
                this.cmbServerInstance.ValueMember = "TargetSystemId";
            }
            else
            {
                Models.ExportServerDetails det = new Models.ExportServerDetails();
                det.DNSServer = "Specify new server instance";
                det.TargetSystemId = 0;
                serverDetails = new List<Models.ExportServerDetails>();
                serverDetails.Insert(0, det);
                this.cmbServerInstance.DataSource = serverDetails;
                this.cmbServerInstance.DisplayMember = "DNSServer";
                this.cmbServerInstance.ValueMember = "TargetSystemId";
            }
            this.cmbServerInstance.SelectedIndex = -1;
            serverTypeFirst = false;
        }
        private Boolean ValidateLoginValues()
        {
            if (string.IsNullOrEmpty(txtTargetServer.Text.Trim()))
            {
                MessageBox.Show("Please enter DNS Server along with port.", "Export Utility Login Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTargetServer.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtCasServer.Text.Trim()))
            {
                MessageBox.Show("Please enter Cas Server along with port.", "Export Utility Login Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCasServer.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtUserId.Text.Trim()))
            {
                MessageBox.Show("Please enter Nia user id.", "Export Utility Login Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserId.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                MessageBox.Show("Please enter Nia Password.", "Export Utility Login Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }
            return true;
        }

        private void cmbServerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            tvDestination.Nodes.Clear();
            EnableControls();

            if (cmbServerType.SelectedIndex > -1 && serverTypeFirst == false)
                GetServerDetails(cmbServerType.SelectedValue.ToString());

        }



        private void chkRetrieveScripts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRetrieveScripts.Checked)
                loadECRScriptsfromDB = true;
            else
                loadECRScriptsfromDB = false;
        }

        private void cmbServerInstance_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DisableControls();
            if (cmbServerInstance.SelectedIndex > 0 && serverInstanceFirst == false)
            {
                int selectedTargetId = int.Parse(cmbServerInstance.SelectedValue.ToString());
                var details = serverDetails.Where(s => s.TargetSystemId.Equals(selectedTargetId) && s.DNSServer.Equals(cmbServerInstance.Text)).FirstOrDefault();

                if (details != null)
                {
                    txtCasServer.Text = details.CasServer;
                    txtTargetServer.Text = details.DNSServer;
                }
                serverInstanceFirst = false;
            }
            else if (cmbServerInstance.SelectedIndex == 0)
            {
                EnableControls();
                txtTargetServer.Focus();
            }
        }

        private void EnableControls()
        {
            txtTargetServer.Text = "";
            txtCasServer.Text = "";
            txtUserId.Text = "";
            txtPassword.Text = "";
            txtTargetServer.Enabled = true;
            txtCasServer.Enabled = true;
            txtUserId.Enabled = true;
            txtPassword.Enabled = true;
        }

        private void DisableControls()
        {
            txtTargetServer.Enabled = false;
            txtCasServer.Enabled = false;
            txtUserId.Enabled = false;
            txtPassword.Enabled = false;
        }

        // Past Exports
        #region "Past Exports"

        private void AddFilterCriteria()
        {
            cmbFilterCriteria.Items.Add("Last 1 Week");
            cmbFilterCriteria.Items.Add("Today");
            cmbFilterCriteria.Items.Add("Last 1 Month");
            cmbFilterCriteria.Items.Add("Last 3 Months");
            cmbFilterCriteria.Items.Add("Last 6 Months");
            cmbFilterCriteria.Items.Add("Last 1 Year");
            cmbFilterCriteria.Items.Add("All");
            cmbFilterCriteria.SelectedIndex = 0;

        }

        private void ClearGrid()
        {
            while (grdPastExport.Rows.Count > 0)
            {
                grdPastExport.Rows.RemoveAt(0);
            }
        }
        private void cmbFilterCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbUserList.Items.Clear();
            cmbExportList.Items.Clear();
            cmbPastExportStatus.SelectedIndex = cmbPastExportStatus.Items.Count - 1;
            //srno = 0;
            grdPastExport.DataSource = null;
            grdPastExport.Rows.Clear();


            DateTime selectedDate = GetDate(cmbFilterCriteria.SelectedItem.ToString());

            GetPastExports(selectedDate.ToShortDateString());
        }

        private DateTime GetDate(string CreatedDate)
        {
            DateTime selectedDate = DateTime.Now;
            switch (CreatedDate)
            {
                case "Last 1 Week":
                    selectedDate = selectedDate.AddDays(-7);
                    break;
                case "Today":
                    selectedDate = DateTime.Now;
                    break;
                case "Last 1 Month":
                    selectedDate = selectedDate.AddMonths(-1);
                    break;
                case "Last 3 Months":
                    selectedDate = selectedDate.AddMonths(-3);
                    break;
                case "Last 6 Months":
                    selectedDate = selectedDate.AddMonths(-6);
                    break;
                case "Last 1 Year":
                    selectedDate = selectedDate.AddYears(-1);
                    break;
                case "All":
                    selectedDate = DateTime.Now.AddYears(-100);
                    break;
                default:
                    selectedDate = selectedDate.AddDays(-7);
                    break;
            }

            return selectedDate;
        }

        private void BindDataGrid(List<PastExportConfigurationDetails> pastExports)
        {
            //grdPastExport.DataSource = null;
            // grdPastExport.Rows.Clear();
            //grdPastExport.Refresh();

            //cmbUserList.Items.Clear();
            //srno = 0;

            //List<Models.ECR.PastExport> pastExportList = GetPastExports(CompletedDate);
            if (pastExports != null && pastExports.Count > 0)
            {
                grdPastExport.DataSource = pastExports;
                grdPastExport.AutoGenerateColumns = false;

                //grdPastExport.Columns[0].HeaderText = "#";
                grdPastExport.Columns[1].HeaderText = "Source Path";
                grdPastExport.Columns[2].HeaderText = "Target Path";
                grdPastExport.Columns[3].HeaderText = "Script Name";
                grdPastExport.Columns[4].HeaderText = "Satus";
                grdPastExport.Columns[5].HeaderText = "Remark";
                grdPastExport.Columns[6].HeaderText = "Select";

                grdPastExport.Columns[0].Visible = false;
                for (int i = 6; i <= 17; i++)
                {
                    // Display export date in Grid
                    //if (i == 13)
                    //    grdPastExport.Columns[i].Visible = true;
                    //else
                    grdPastExport.Columns[i].Visible = false;
                }
                grdPastExport.Columns[0].Width = 25;
                grdPastExport.Columns[1].Width = 165;
                grdPastExport.Columns[2].Width = 165;
                grdPastExport.Columns[3].Width = 80;
                grdPastExport.Columns[4].Width = 55;
                grdPastExport.Columns[5].Width = 120;

                if (!grdPastExport.Columns.Contains("ActionDropdown"))
                {
                    DataGridViewComboBoxColumn actions = new DataGridViewComboBoxColumn();
                    actions.Name = "ActionDropdown";
                    actions.HeaderText = "Select Action";
                    actions.DataSource = GetActions();
                    actions.DisplayMember = "Action";
                    actions.ValueMember = "Action";
                    actions.DataPropertyName = "Id";
                    //actions.DisplayIndex = 6;
                    grdPastExport.Columns.Add(actions);
                }

                for (int i = 0; i < grdPastExport.Rows.Count; i++)
                {
                    if (grdPastExport.Rows[i].Cells[5].Value != null)
                    {
                        if (grdPastExport.Columns.Contains("ActionDropdown"))
                        {
                            //string remark = grdPastExport.Rows[i].Cells[5].Value.ToString();
                            string status = grdPastExport.Rows[i].Cells[4].Value.ToString();
                            if (status == "Exists")
                                grdPastExport.Rows[i].Cells[18].ReadOnly = false;
                            else
                                grdPastExport.Rows[i].Cells[18].ReadOnly = true;
                        }
                    }
                }
            }
        }

        private void GetPastExports(string CreatedOn)
        {
            string userName = "";

            if (!isSuperAdmin)
                userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            // Get All successful past exports from ExportConfigurationMaster
            GetPastExportConfigurationMasterResMsg pastExports = this.exportClient.ServiceChannel.GetPastExportConfigurationMasterDetails(CreatedOn, "6", userName);
            if (pastExports.PastExportConfigurationMasterDetails != null && pastExports.PastExportConfigurationMasterDetails.Count > 0)
            {
                pastExportScriptDetails = Translators.PastExportConfigurationDetailsSE_PEList.PastExportConfigurationDetailsSEtoPEList(pastExports.PastExportConfigurationMasterDetails);
                var users = pastExportScriptDetails.Select(e => e.CreatedBy).Distinct();

                foreach (string user in users)
                {
                    cmbUserList.Items.Add(user);
                }
                if (!isSuperAdmin)
                    cmbUserList.Enabled = false;
                cmbUserList.SelectedIndex = 0;

                var exportDate = pastExportScriptDetails.Select(e => e.CreatedOn).Distinct().ToList();
                foreach (var date in exportDate)
                {
                    cmbExportList.Items.Add(date.ToString());
                }
                cmbExportList.SelectedIndex = -1;
                // BindDataGrid(pastExportScriptDetails);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPastExports"])
            {
                AddFilterCriteria();
                FillExportstatus();
            }

        }

        private void grdPastExport_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //if (grdPastExport.Rows.Count > 0)
            //{
            //    //srno = srno + 1;
            //    // grdPastExport.Rows[e.RowIndex].Cells[0].Value = srno;
            //    if (grdPastExport.Rows[e.RowIndex].Cells[5].Value != null)
            //    {
            //        if (grdPastExport.Columns.Contains("ActionDropdown"))
            //        {
            //            string remark = grdPastExport.Rows[e.RowIndex].Cells[5].Value.ToString();
            //            if (remark == "NA")
            //                grdPastExport.Rows[e.RowIndex].Cells[18].ReadOnly = true;
            //            else
            //                grdPastExport.Rows[e.RowIndex].Cells[18].ReadOnly = false;
            //        }
            //    }
            //}
        }

        private void cmbExportList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbExportList.SelectedItem != null)
            {
                DateTime dt = DateTime.Parse(cmbExportList.SelectedItem.ToString());

                var details = pastExportScriptDetails.Where(s => s.CreatedOn.Year == dt.Year &&
             s.CreatedOn.Month == dt.Month && s.CreatedOn.Day == dt.Day && s.CreatedOn.Hour == dt.Hour && s.CreatedOn.Minute == dt.Minute
             && s.CreatedOn.Second == dt.Second).ToList();

                //var details = pastExportScriptDetails.Where(s => s.CompletedOn.GetValueOrDefault().Year == dt.Year &&
                //s.CompletedOn.GetValueOrDefault().Month == dt.Month && s.CompletedOn.GetValueOrDefault().Day == dt.Day && s.CompletedOn.GetValueOrDefault().Hour == dt.Hour && s.CompletedOn.GetValueOrDefault().Minute == dt.Minute
                //&& s.CompletedOn.GetValueOrDefault().Second == dt.Second).ToList();
                BindDataGrid(details);
            }
        }

        private void FillExportstatus()
        {
            cmbPastExportStatus.Items.Clear();
            var values = Enum.GetValues(typeof(ExportStatus)).Cast<ExportStatus>();
            foreach (ExportStatus status in Enum.GetValues(typeof(ExportStatus)))
            {
                cmbPastExportStatus.Items.Add(status);
            }

            cmbPastExportStatus.Items.Add("All");
            cmbPastExportStatus.SelectedIndex = cmbPastExportStatus.Items.Count - 1;
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("Are you sure you want to perform this operation?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {

                    UpdatePastExportConfigurationDetailsReqMsg req = new UpdatePastExportConfigurationDetailsReqMsg();
                    req.PastExportConfigurationMasterDetails = new List<PastExportConfigurationMasterDetails>();

                    for (int i = 0; i < grdPastExport.Rows.Count; i++)
                    {
                        if (grdPastExport.Rows[i].Cells["ActionDropdown"].Value != null)
                        {
                            PastExportConfigurationMasterDetails record = new PastExportConfigurationMasterDetails();
                            record.masterExportId = int.Parse(grdPastExport.Rows[i].Cells[11].Value.ToString());
                            record.ScriptTransactionId = int.Parse(grdPastExport.Rows[i].Cells[0].Value.ToString());
                            record.ExportStatus = (int)ExportStatus.Resubmit;
                            record.Status = (short)TransactionStatus.Exist;
                            record.ModifiedOn = DateTime.UtcNow;
                            record.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                            string action = grdPastExport.Rows[i].Cells["ActionDropdown"].Value.ToString();
                            if (action.Equals(Models.Action.CreateNew.ToString()))
                            {
                                record.Action = (int)Models.Action.CreateNew;
                                record.ExistReasonCode = short.Parse(grdPastExport.Rows[i].Cells[8].Value.ToString());
                                req.PastExportConfigurationMasterDetails.Add(record);
                            }
                            else if (action.Equals(Models.Action.Overwrite.ToString()))
                            {
                                record.Action = (int)Models.Action.Overwrite;
                                record.ExistReasonCode = short.Parse(grdPastExport.Rows[i].Cells[8].Value.ToString());
                                req.PastExportConfigurationMasterDetails.Add(record);
                            }
                            else if (action.Equals(Models.Action.Ignore.ToString()))
                            {
                                record.Action = (int)Models.Action.Ignore;
                                record.ExistReasonCode = short.Parse(grdPastExport.Rows[i].Cells[8].Value.ToString());
                                req.PastExportConfigurationMasterDetails.Add(record);
                            }
                        }
                    }

                    if (req.PastExportConfigurationMasterDetails != null & req.PastExportConfigurationMasterDetails.Count > 0)
                    {

                        var resp = exportClient.ServiceChannel.UpdatePastExportConfigurationDetails(req);

                        if (resp.IsSuccess)
                            MessageBox.Show("Scripts would be procssed in the next bacth cycle.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            throw new Exception("An error has ocurred. Pls contact the admin.");

                        this.Close();
                    }
                    else
                        MessageBox.Show("Please select atleast one script for processing.", "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void grdPastExport_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if (e.ColumnIndex == 18)
            //{
            //    e.Value = "None";
            //}

        }

        private List<PastExportAction> GetActions()
        {
            List<PastExportAction> actionList = new List<PastExportAction>();
            PastExportAction action = new PastExportAction();
            action.Id = 1;
            action.Action = "None";
            actionList.Add(action);

            action = new PastExportAction();
            action.Id = 2;
            action.Action = "Overwrite";
            actionList.Add(action);

            action = new PastExportAction();
            action.Id = 3;
            action.Action = "CreateNew";
            actionList.Add(action);

            action = new PastExportAction();
            action.Id = 4;
            action.Action = "Ignore";
            actionList.Add(action);

            return actionList;
        }
        #endregion

        private void grdPastExport_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //var cmbRole = e.Control as ComboBox;
            //if (cmbRole != null)
            //{

            //}
        }

        private void grdPastExport_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void cmbUserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUserList.SelectedItem != null)
            {
                var user = cmbUserList.SelectedItem.ToString();
                var details = pastExportScriptDetails.Where(s => s.CreatedBy.Equals(user)).ToList();
                BindDataGrid(details);
            }
        }

        private void cmbPastExportStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPastExportStatus.SelectedItem != null)
            {
                grdPastExport.DataSource = null;
                int exportStatus;
                switch (cmbPastExportStatus.SelectedItem.ToString())
                {
                    case "Submitted":
                        exportStatus = 0;
                        break;
                    case "InProgress":
                        exportStatus = 1;
                        break;
                    case "Success":
                        exportStatus = 2;
                        break;
                    case "Failed":
                        exportStatus = 3;
                        break;
                    case "Resubmit":
                        exportStatus = 4;
                        break;
                    case "Pending_Conflicts":
                        exportStatus = 5;
                        break;
                    case "All":
                        exportStatus = 6;
                        break;
                    default:
                        exportStatus = 6;
                        break;
                }

                //var exportStatus = int.Parse( cmbPastExportStatus.SelectedItem.ToString());
                if (exportStatus < 6)
                {
                    if (cmbExportList.SelectedItem == null)
                    {
                        var details = pastExportScriptDetails.Where(s => s.ExportStatus == exportStatus && s.CreatedBy.Equals(cmbUserList.SelectedItem.ToString()) && s.CreatedOn >= GetDate(cmbFilterCriteria.SelectedItem.ToString())).ToList();
                        BindDataGrid(details);
                    }
                    else
                    {
                        var details = pastExportScriptDetails.Where(s => s.ExportStatus == exportStatus && s.CreatedBy.Equals(cmbUserList.SelectedItem.ToString()) && s.CreatedOn >= GetDate(cmbFilterCriteria.SelectedItem.ToString())).ToList();
                        BindDataGrid(details);
                    }
                }
                else
                {
                    //var details = pastExportScriptDetails.Where(s => s.ExportStatus == exportStatus).ToList();
                    BindDataGrid(pastExportScriptDetails);
                }
            }
        }

        private void grdPastExport_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush b = new SolidBrush(((DataGridView)sender).RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }

        }

        private void cmbServerInstance_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtUserId.Text = "";
            txtPassword.Text = "";
            txtUserId.Enabled = true;
            txtPassword.Enabled = true;

            //txtUserId.Text = "test_user5";
            //txtPassword.Text = "$5Iip12$";
        }

        private void btnCancelExport_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            //Infosys.ATR.Service.ExportUtility.RunExportJob();
            //MessageBox.Show("Done");
        }
    }

}
