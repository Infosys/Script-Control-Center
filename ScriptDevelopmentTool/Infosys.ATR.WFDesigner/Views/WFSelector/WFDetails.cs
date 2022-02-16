using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using Infosys.ATR.WFDesigner.Services;
using Infosys.ATR.WFDesigner.Entities;
using Infosys.WEM.Service.Contracts.Message;
using Infosys.ATR.WFDesigner.Services;
using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.ATR.WFDesigner.Constants;
using Infosys.ATR.CommonViews;
using System.Net;
using System.IO;
using System.Activities;
using System.Activities.XamlIntegration;
using Infosys.WEM.SecureHandler;
using Infosys.WEM.Infrastructure.Common;
using System.Text;
using Infosys.ATR.Entities;
using Infosys.IAP.CommonClientLibrary.Models;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.WFDesigner.Views
{
    public enum ExecutionMode
    {
        Online,
        Offline
    }
    public partial class WFDetails : UserControl
    {
        List<Category> CategoryDetails { get; set; }
        List<Category> Categories { get; set; }
        Guid workflowId;
        int workflowVersion;
        string subCategory;
        List<SubCategory> subCatDetails;
        string _scriptUrl;
        string _downloadWfname = "";
        WorkflowPE pe;
        bool _newparam;
        string selectedParamId;
        List<WorkflowParameterPE> originalParameters;


        int catgoryID;
        int subCatID;

        internal WFDesigner wfDe;
        ControlledWorkItem<WFWorkItem> _current = null;

        [EventPublication(EventTopicNames.DisplayWFDetails, PublicationScope.Global)]
        public event EventHandler<EventArgs<Category>> DisplayWFDetailsEvent;

        [EventPublication(Constants.EventTopicNames.ShowOutputView, PublicationScope.Global)]
        public event EventHandler<EventArgs<List<ExecutionResultView>>> ShowOutputView;

        [EventPublication(Constants.EventTopicNames.AppendOutputViewWF, PublicationScope.Global)]
        public event EventHandler<Infosys.ATR.WFDesigner.Views.ExecuteWf.AppendOutputViewArgsWF> AppendOutputViewWF;

        private ContextMenuStrip contextMenuPublishOptions;
        private bool createPackage = false;
        private bool enableSaveOption = false;
        private Users CurentUser = null;

        private bool publishLocally = false;
        private string executionMode = string.Empty;

        public WFDetails(bool isNew, bool isIapPackage = false)
        {
            InitializeComponent();
            executionMode = ConfigurationManager.AppSettings["Mode"];
            publishLocally = (executionMode.Equals(ExecutionMode.Offline.ToString(), StringComparison.InvariantCultureIgnoreCase)) ? true : isIapPackage;
            Initialize();
        }

        private void DisplayWFDetails(Category cat)
        {
            if (DisplayWFDetailsEvent != null)
                DisplayWFDetailsEvent(this, new EventArgs<Category>(cat));
        }

        private void Initialize()
        {
            int companyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);
            if (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase))
                GetCategories(companyId);

            this.cmbCategory.DataSource = Categories;
            this.cmbCategory.DisplayMember = "Name";
            this.cmbCategory.ValueMember = "CategoryId";

        }

        public void InitializeSaveOption()
        {
            if (!executionMode.Equals(ExecutionMode.Offline.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                CreateContextMenuStrip();
                enableSaveOption = true;
                this.btnSave.Image = ((System.Drawing.Image)((new System.ComponentModel.ComponentResourceManager(typeof(WFDetails))).GetObject("btnSave.Image")));
            }
            else
                this.btnSave.Text = "Export";
        }

        private void GetCategories(int companyId)
        {
            Categories = new List<Category>();
            var response = WFService.GetAllCategoriesByCompany(companyId);

            CategoryDetails = Translators.CategoryPE_SE.CategoryListSEtoPE(response.Categories.ToList());

            WFCache.CategoryDetails = CategoryDetails;

            var Users = Infosys.ATR.Entities.CommonObjects.Users;

            if (CategoryDetails != null)
            {
                var userCategory = Users.Select(u => u.CategoryId).Distinct().ToList();
                if (userCategory != null)
                {
                    this.Categories = response.Categories.Where(c => c.ParentId == 0)
                        .ToList().Where(c =>
                        {
                            if (Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                                return true;

                            return userCategory.Contains(Convert.ToInt32(c.CategoryId));
                        })
                        .Select(c => new Entities.Category
                        {
                            Description = c.Description,
                            CategoryId = c.CategoryId,
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
                    var subCat = this.CategoryDetails.Where(sc => sc.CompanyId == 0 && sc.ParentId == Convert.ToInt32(c.CategoryId));
                    if (subCat == null || subCat.Count() == 0)
                    {
                        this.Categories.Remove(c);
                    }
                }

                if (!Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                {
                    var catIds = Users.Where(u => u.Role == "Guest").Select(x => x.CategoryId).ToArray();
                    this.Categories.RemoveAll(uc => catIds.Contains(Convert.ToInt32(uc.CategoryId)));
                }
            }
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            subCategory = "";
            Category c = this.cmbCategory.SelectedItem as Category;
            if (c != null)
            {
                this.cmbSubCategory.DataSource = null;
                subCatDetails = new List<SubCategory>();
                FindAllSubCategories(c.CategoryId);
                if (subCatDetails.Count > 0)
                {
                    this.cmbSubCategory.DataSource = subCatDetails;
                    this.cmbSubCategory.DisplayMember = "SubCategoryName";
                    this.cmbSubCategory.ValueMember = "SubCategoryId";
                }
                //then select the intended subcategory, if any
                if (pe != null)
                    this.cmbSubCategory.SelectedValue = pe.CategoryID;
            }
        }


        System.Diagnostics.Stopwatch wfRun = System.Diagnostics.Stopwatch.StartNew();

        public void RunSelectedWF()
        {
            btnRunWF_Click(this, new EventArgs());
        }
        private void btnRunWF_Click(object sender, System.EventArgs e)
        {
            if (wfDe != null)
            {
                wfDe.Run();
            }
            else if (pe != null)
            {
                ExecuteWf run = null;
                var xaml = WFService.DownloadXAML(pe.WorkflowURI);

                //if (pe.Parameters == null)
                //    run = new ExecuteWf(null, xaml);
                //else
                //    run = new ExecuteWf(pe.Parameters, String.Empty);
                //run.WorkflowText = xaml;

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

                run.NodeExecuted += new ExecuteWf.NodeExecutedEventHandler(run_NodeExecuted);
                run.ExecutionResultView += run_ExecutionResultView;
                run.Show();
            }

            //else
            //{
            //    if (wfDe != null)
            //        wfDe.Run();
            //}
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
            else if (wfDe != null)
                wfDe.AppendOutputShell(e);
        }

        void run_NodeExecuted(List<CommonViews.ExecutionResultView> e)
        {
            var executionTime = wfRun.ElapsedMilliseconds;

            if (ShowOutputView != null)
                ShowOutputView(this, new EventArgs<List<ExecutionResultView>>(e));
            else if (wfDe != null)
                wfDe.DisplayOutput(e);
        }


        private void FindAllSubCategories(int catId)
        {
            //CategoryDetails.Where(c => c.ParentId == catId).ToList().ForEach(
            //   cat =>
            //   {
            //       Category c = cat;
            //       SubCategory item = new SubCategory();
            //       subCategory = subCategory + c.Name + ".";
            //       item.SubCategoryName = subCategory.Substring(0, subCategory.Length - 1);
            //       item.SubCategoryId = c.CategoryId;
            //       subCatDetails.Add(item);
            //       FindAllSubCategories(c.CategoryId);
            //       subCategory = "";
            //   }
            //   );
            var Users = Infosys.ATR.Entities.CommonObjects.Users;
            var userCategory = Users.Select(u => u.CategoryId).Distinct().ToList();
            string lastCat = subCategory;
            CategoryDetails.Where(cat => cat.ParentId == catId).ToList()
                .Where(cat =>
                {
                    if (Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                        return true;

                    return userCategory.Contains(Convert.ToInt32(cat.CategoryId));
                }).ToList()
                .ForEach(subCat =>
            {
                subCategory = lastCat;
                if (string.IsNullOrEmpty(subCategory))
                    subCategory = subCat.Name;
                else
                    subCategory += "." + subCat.Name;
                subCatDetails.Add(new SubCategory() { SubCategoryId = subCat.CategoryId, SubCategoryName = subCategory });
                FindAllSubCategories(subCat.CategoryId);
            });
        }

        [EventSubscription(Constants.EventTopicNames.ShowWFDetails, ThreadOption.UserInterface)]
        public void ShowWFDetailsEventHandler(object sender, EventArgs<WorkflowPE> e)
        {
            if (e.Data != null)
            {
                pe = e.Data;
                //hide/show the parameter panel
                if (pe.Parameters != null && pe.Parameters.Count > 0)
                {
                    pnlParameters.Visible = true;
                    pnlAddedParam.Visible = true;
                    //align the existing parameters, if any
                    dgParams.DataSource = GetWFParamSubset(pe.Parameters);
                    originalParameters = new List<WorkflowParameterPE>(pe.Parameters);
                }
                else
                {
                    pnlParameters.Visible = false;
                    pnlAddedParam.Visible = false;
                }

                txtName.Text = pe.Name;
                txtDescription.Text = pe.Description;
                _scriptUrl = pe.WorkflowURI;
                checkBox1.Checked = pe.UsesUIAutomation;
                chkIsPersistWF.Checked = pe.IslongRunningWorkflow;
                txtIdleTimeOut.Text = Convert.ToString(pe.IdleStateTimeout);

                txtTags.Text = pe.Tags;
                int extPos = (string.IsNullOrEmpty(_scriptUrl)) ? 0 : _scriptUrl.LastIndexOf('.');
                if (!string.IsNullOrEmpty(_scriptUrl) && extPos > 0 && !publishLocally && executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    _downloadWfname = pe.Name + _scriptUrl.Substring(extPos, _scriptUrl.Length - extPos);

                    if (CurentUser != null)
                    {
                        if (CurentUser.Role == Infosys.ATR.Entities.Roles.Guest.ToString() || CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                            btnDownload.Enabled = false;
                    }
                    else
                        btnDownload.Enabled = true;
                }
                //lblCreatedBy.Text = pe.CreatedBy;
                //lblCreatedOn.Text = pe.PublishedOn.ToString();
                catgoryID = pe.CategoryID;
                // subCatID = pe.SubCategoryID;
                workflowVersion = pe.WorkflowVersion;
                //if (!String.IsNullOrEmpty(pe.LastModifiedBy))
                //{
                //    lblModifiedBy.Text = pe.LastModifiedBy;
                //    lblLastModifiedOn.Text = pe.LastModifiedOn;
                //    lblLastModifiedByTitle.Visible = true;
                //    lblModifiedBy.Visible = true;
                //    lblLastModifiedOnTitle.Visible = true;
                //    lblLastModifiedOn.Visible = true;
                //}
                //else
                //{
                //    lblModifiedBy.Text = "";
                //    lblLastModifiedOn.Text = "";
                //    lblLastModifiedByTitle.Visible = false;
                //    lblModifiedBy.Visible = false;
                //    lblLastModifiedOnTitle.Visible = false;
                //    lblLastModifiedOn.Visible = false;
                //}

                if (!publishLocally)
                {
                    SetCategory(pe.CategoryID);

                    //then subcategory, if any
                    if (pe != null)
                        this.cmbSubCategory.SelectedValue = pe.CategoryID;

                    if (CheckRootCategory(pe.CategoryID))
                    {
                        cmbSubCategory.SelectedItem = null;
                    }
                    else
                    {
                        //if (pe.SubCategoryID > 0)
                        //    cmbSubCategory.SelectedValue = pe.SubCategoryID;
                        //else
                        //    cmbSubCategory.SelectedValue = pe.CategoryID;
                    }
                }
                workflowId = pe.WorkflowID;
            }
            else
            {
                txtDescription.Text = "";
                txtName.Text = "";
                pnlParameters.Visible = false;
                pnlAddedParam.Visible = false;
            }
        }

        private Boolean CheckRootCategory(int catID)
        {
            var result = CategoryDetails.Where(cat => cat.CategoryId == catID).Select(e => e.ParentId).SingleOrDefault();
            //if ParentId is not null and greater than zero
            if (result == 0)
                return true;
            else
                return false;
        }

        private void SetCategory(int catId)
        {
            subCategory = "";
            catId = FindParentCategory(catId);
            CategoryDetails.Where(c => c.ParentId == 0).ToList().ForEach(c =>
            {
                if (c.CategoryId == catId)
                {
                    this.cmbCategory.SelectedItem = c;
                    this.cmbCategory.SelectedValue = c.CategoryId;
                }
            });
        }
        /// <summary>
        /// This method is used to find the parent category at the root level.
        /// </summary>
        /// <param name="catID">Category Id</param>
        /// <returns>Id of root category</returns>
        public int FindParentCategory(int catID)
        {
            int categoryId = catID;
            var result = CategoryDetails.Where(cat => cat.CategoryId == catID).Select(e => new { e.CategoryId, e.ParentId, e.Name }).Single();
            if (result != null && result.ParentId > 0)
            {
                categoryId = FindParentCategory(result.ParentId);
            }
            else
            {
                return result.CategoryId;
            }
            return categoryId;
        }

        private void PublishWorkflow()
        {
            if (!publishLocally)
            {
                //if any is selected in the sub category dropt down, then use it
                if (cmbSubCategory.SelectedValue != null)
                    catgoryID = (int)cmbSubCategory.SelectedValue;
                else
                    catgoryID = (int)cmbCategory.SelectedValue;
            }

            if (!string.IsNullOrEmpty(txtName.Text))
            {
                if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(txtName.Text))
                {
                    MessageBox.Show("Please provide the Name without Special Characters", "Special Characters...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            //mark the parameter which were in the original received list (if any) but delete later
            if (originalParameters != null)
            {
                originalParameters.ForEach(oparam =>
                {
                    if (pe != null && pe.Parameters != null)
                    {
                        if (!pe.Parameters.Contains(oparam))
                        {
                            oparam.IsDeleted = true;
                            oparam.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            pe.Parameters.Add(oparam);
                        }
                    }
                });
            }

            //for (int i = 0; i < pe.Parameters.Count; i++)
            //{
            //    if (pe.Parameters[i].IsSecret)
            //        if (!string.IsNullOrEmpty(pe.Parameters[i].DefaultValue))
            //            pe.Parameters[i].DefaultValue = SecurePayload.Secure(pe.Parameters[i].DefaultValue, "IAP2GO_SEC!URE");
            //}

            if (Publish())
            {
                Category cat = new Category();

                if (subCatID > 0)
                {
                    cat.CategoryId = subCatID;
                    cat.ParentId = catgoryID;
                }
                else
                {
                    cat.CategoryId = catgoryID;
                    cat.ParentId = 0;
                }
                this.DisplayWFDetails(cat);
            }

        }

        //private void btnSave_Click(object sender, EventArgs e)
        //{
        //    //if any is selected in the sub category dropt down, then use it
        //    if (cmbSubCategory.SelectedValue != null)
        //        catgoryID = (int)cmbSubCategory.SelectedValue;
        //    else
        //        catgoryID = (int)cmbCategory.SelectedValue;

        //    if(!string.IsNullOrEmpty(txtName.Text))
        //    {
        //        if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(txtName.Text))
        //        {
        //            MessageBox.Show("Please provide the Name without Special Characters", "Special Characters...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            return;
        //        }
        //    }
        //    //mark the parameter which were in the original received list (if any) but delete later
        //    if (originalParameters != null)
        //    {
        //        originalParameters.ForEach(oparam =>
        //        {
        //            if (pe != null && pe.Parameters != null)
        //            {
        //                if (!pe.Parameters.Contains(oparam))
        //                {
        //                    oparam.IsDeleted = true;
        //                    oparam.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        //                    pe.Parameters.Add(oparam);
        //                }
        //            }
        //        });
        //    }

        //    //for (int i = 0; i < pe.Parameters.Count; i++)
        //    //{
        //    //    if (pe.Parameters[i].IsSecret)
        //    //        if (!string.IsNullOrEmpty(pe.Parameters[i].DefaultValue))
        //    //            pe.Parameters[i].DefaultValue = SecurePayload.Secure(pe.Parameters[i].DefaultValue, "IAP2GO_SEC!URE");
        //    //}

        //    if (Publish())
        //    {
        //        Category cat = new Category();

        //        if (subCatID > 0)
        //        {
        //            cat.CategoryId = subCatID;
        //            cat.ParentId = catgoryID;
        //        }
        //        else
        //        {
        //            cat.CategoryId = catgoryID;
        //            cat.ParentId = 0;
        //        }
        //        this.DisplayWFDetails(cat);
        //    }

        //}




        private bool ValidationPassed()
        {
            bool isPass = true;
            if (String.IsNullOrEmpty(txtName.Text))
            {
                isPass = false;
                return isPass;
            }

            return isPass;
        }

        private bool Publish()
        {
            WorkflowPE pe1 = null;
            int version = 0;
            if (!ValidationPassed())
            {
                MessageBox.Show("Please provide the details for all these Workflow fields- Name, Category.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            try
            {
                var iapworkflowstore = ConfigurationManager.AppSettings["IapWorkflowStore"];
                if (wfDe != null)
                {
                    //if (createPackage && !string.IsNullOrEmpty(wfDe.data.WorkflowURI))
                    //    if (Path.GetExtension(wfDe.data.WorkflowURI).ToLower().Equals(".xaml"))
                    //        wfDe.data = null;

                    if (wfDe.data == null)
                    {
                        Guid guid = Guid.NewGuid();
                        WorkflowPE data = new WorkflowPE();

                        data.CategoryID = catgoryID;

                        data.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        data.Description = txtDescription.Text;
                        data.Name = txtName.Text;
                        data.WorkflowID = guid;
                        version = 1;
                        data.FileName = "1.xaml";
                        data.CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]);
                        data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", guid, data.FileName);
                        data.IncrementVersion = true;
                        data.UsesUIAutomation = checkBox1.Checked;
                        data.IslongRunningWorkflow = chkIsPersistWF.Checked;
                        data.IdleStateTimeout = (string.IsNullOrEmpty(txtIdleTimeOut.Text)) ? 0 : int.Parse(txtIdleTimeOut.Text);

                        data.Tags = txtTags.Text;
                        if (pe != null)
                        {
                            data.Parameters = pe.Parameters;
                            if (data.Parameters != null && data.Parameters.Count > 0)
                            {
                                for (int i = 0; i < data.Parameters.Count; i++)
                                {
                                    data.Parameters[i].WorkflowId = data.WorkflowID.ToString();
                                }
                            }
                        }
                        //var repsonse = wfDe.Upload(data);
                        //data.WFContent = wfDe.GetWorkflowContent(data);
                        byte[] workflowContent = wfDe.GetWorkflowContent(data);
                        data.WFContent = (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions) ? SecurePayload.SecureBytes(workflowContent) : workflowContent;
                        data.Modified = true;

                        if (createPackage)
                        {
                            FolderBrowserDialog fldrDiag = new FolderBrowserDialog();
                            fldrDiag.Description = "Select the location for the IAPW package. The location should also have the depended files and folders referred by the xmal file in the iapd package to be created.";
                            if (fldrDiag.ShowDialog().ToString().ToLower() == "ok")
                            {
                                string path = fldrDiag.SelectedPath;
                                Stream xamlStream = new MemoryStream();
                                xamlStream.Write(data.WFContent, 0, data.WFContent.Length);
                                var result = Infosys.ATR.Packaging.Operations.Package("main", xamlStream, path);
                                if (result.IsSuccess)
                                {
                                    data.WFContent = ConvertStreamtoByteArray(result.PackageStream);
                                    data.FileName = "1.iapw";
                                    data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", guid, data.FileName);
                                    pe1 = Publish(data);
                                    MessageBox.Show("Workflow Published Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    wfDe.data.WorkflowVersion = version;
                                }
                                else
                                    MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                this.Cursor = Cursors.Default;
                                MessageBox.Show("Select the location for the IAPW package", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                        else
                        {
                            pe1 = Publish(data);
                            MessageBox.Show("Workflow Published Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            wfDe.data.WorkflowVersion = version;
                        }
                    }

                    // if editing and changes done in workflow
                    else if (wfDe.data != null && wfDe.isDirty)
                    {
                        int catid = catgoryID;
                        wfDe.data.CategoryID = catgoryID;
                        wfDe.data.Description = txtDescription.Text;
                        wfDe.data.Name = txtName.Text;
                        var fileName = Convert.ToInt16(wfDe.data.FileName.Split('.')[0]);
                        version = fileName + 1;
                        wfDe.data.FileName = (fileName + 1).ToString() + ".xaml";
                        wfDe.data.CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]);
                        wfDe.data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", wfDe.data.WorkflowID, wfDe.data.FileName);
                        wfDe.data.IncrementVersion = true;
                        wfDe.data.UsesUIAutomation = checkBox1.Checked;
                        wfDe.data.IslongRunningWorkflow = chkIsPersistWF.Checked;
                        wfDe.data.IdleStateTimeout = (string.IsNullOrEmpty(txtIdleTimeOut.Text)) ? 0 : int.Parse(txtIdleTimeOut.Text);

                        wfDe.data.Tags = txtTags.Text;
                        if (pe != null)
                        {
                            wfDe.data.Parameters = pe.Parameters;
                            if (wfDe.data.Parameters != null && wfDe.data.Parameters.Count > 0)
                            {
                                for (int i = 0; i < wfDe.data.Parameters.Count; i++)
                                {
                                    wfDe.data.Parameters[i].WorkflowId = wfDe.data.WorkflowID.ToString();
                                }
                            }
                        }
                        // var response = wfDe.Upload(wfDe.data);
                        //wfDe.data.WFContent = wfDe.GetWorkflowContent(wfDe.data);
                        byte[] workflowContent = wfDe.GetWorkflowContent(wfDe.data);
                        wfDe.data.WFContent = (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions) ? SecurePayload.SecureBytes(workflowContent) : workflowContent;
                        wfDe.data.Modified = true;

                        if (createPackage)
                        {
                            FolderBrowserDialog fldrDiag = new FolderBrowserDialog();
                            fldrDiag.Description = "Select the location for the IAPW package. The location should also have the depended files and folders referred by the xmal file in the iapw package to be created.";
                            var diagRes = fldrDiag.ShowDialog();
                            string path = "";
                            if (diagRes.ToString().ToLower() == "ok")
                            {
                                path = fldrDiag.SelectedPath;
                            }

                            Stream xamlStream = new MemoryStream();
                            xamlStream.Write(wfDe.data.WFContent, 0, wfDe.data.WFContent.Length);
                            var result = Infosys.ATR.Packaging.Operations.Package("main", xamlStream, path);
                            if (result.IsSuccess)
                            {
                                wfDe.data.WFContent = ConvertStreamtoByteArray(result.PackageStream);
                                wfDe.data.FileName = (fileName + 1).ToString() + ".iapw";
                                wfDe.data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", wfDe.data.WorkflowID, wfDe.data.FileName);
                            }
                            else
                                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            //else
                            //{
                            //    this.Cursor = Cursors.Default;
                            //    MessageBox.Show("Select the location for the IAPW package", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //}
                        }

                        if (wfDe.data.CategoryID == catid)
                        {
                            pe1 = Update(wfDe.data);
                        }
                        else
                        {
                            wfDe.data.CategoryID = catid;
                            pe1 = Publish(wfDe.data);
                        }

                        MessageBox.Show("Workflow Updated Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        wfDe.data.WorkflowVersion = version;
                    }

                    // if  no changes have been done in workflow
                    else if (wfDe.data != null && !wfDe.isDirty)
                    {
                        int catId;
                        wfDe.data.Description = txtDescription.Text;
                        wfDe.data.Name = txtName.Text;
                        wfDe.data.UsesUIAutomation = checkBox1.Checked;
                        wfDe.data.IslongRunningWorkflow = chkIsPersistWF.Checked;
                        wfDe.data.IdleStateTimeout = (string.IsNullOrEmpty(txtIdleTimeOut.Text)) ? 0 : int.Parse(txtIdleTimeOut.Text);

                        wfDe.data.Tags = txtTags.Text;
                        if (pe != null)
                        {
                            wfDe.data.Parameters = pe.Parameters;
                            if (wfDe.data.Parameters != null && wfDe.data.Parameters.Count > 0)
                            {
                                for (int i = 0; i < wfDe.data.Parameters.Count; i++)
                                {
                                    wfDe.data.Parameters[i].WorkflowId = wfDe.data.WorkflowID.ToString();
                                }
                            }
                        }
                        catId = catgoryID;


                        if (createPackage && Path.GetExtension(wfDe.data.WorkflowURI).ToLower().Equals(".xaml"))
                        {
                            //if (!wfDe.data.FileName.Contains(".iapw"))
                            //    wfDe.data.FileName = wfDe.data.FileName + "." + "iapw";

                            byte[] workflowContent = wfDe.GetWorkflowContent(wfDe.data);
                            wfDe.data.WFContent = (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions) ? SecurePayload.SecureBytes(workflowContent) : workflowContent;
                            wfDe.data.Modified = true;
                            wfDe.data.IncrementVersion = true;

                            FolderBrowserDialog fldrDiag = new FolderBrowserDialog();

                            fldrDiag.Description = "Select the location for the IAPW package. The location should also have the depended files and folders referred by the xmal file in the iapw package to be created.";
                            string path = "";
                            if (fldrDiag.ShowDialog().ToString().ToLower() == "ok")
                            {
                                path = fldrDiag.SelectedPath;
                            }

                            Stream xamlStream = new MemoryStream();
                            xamlStream.Write(wfDe.data.WFContent, 0, wfDe.data.WFContent.Length);
                            var result = Infosys.ATR.Packaging.Operations.Package("main", xamlStream, path);
                            if (result.IsSuccess)
                            {
                                wfDe.data.WFContent = ConvertStreamtoByteArray(result.PackageStream);
                                var fileName = Convert.ToInt16(wfDe.data.FileName.Split('.')[0]);
                                version = fileName + 1;
                                wfDe.data.FileName = (fileName + 1).ToString() + ".iapw";
                                wfDe.data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", wfDe.data.WorkflowID, wfDe.data.FileName);
                            }
                            else
                                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //}
                            //else
                            //{
                            //    this.Cursor = Cursors.Default;
                            //    MessageBox.Show("Select the location for the IAPW package", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            //    return false;
                            //}
                        }
                        else if (!createPackage && Path.GetExtension(wfDe.data.WorkflowURI).ToLower().Equals(".iapw"))
                        {
                            int catid = catgoryID;
                            wfDe.data.CategoryID = catgoryID;
                            var fileName = Convert.ToInt16(wfDe.data.FileName.Split('.')[0]);
                            version = fileName + 1;
                            wfDe.data.FileName = (fileName + 1).ToString() + ".xaml";
                            wfDe.data.CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]);
                            wfDe.data.WorkflowURI = String.Format(iapworkflowstore + "/{0}/{1}", wfDe.data.WorkflowID, wfDe.data.FileName);
                            wfDe.data.IncrementVersion = true;

                            byte[] workflowContent = wfDe.GetWorkflowContent(wfDe.data);
                            wfDe.data.WFContent = (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions) ? SecurePayload.SecureBytes(workflowContent) : workflowContent;
                            wfDe.data.Modified = true;

                        }
                        else
                        {
                            wfDe.data.Modified = false;

                            //if (!Path.GetExtension(wfDe.data.FileName).ToLower().Equals(".iapw"))
                            //    wfDe.data.FileName = wfDe.data.FileName + "." + "iapw";

                            //if (!Path.GetExtension(wfDe.data.FileName).ToLower().Equals(".xaml"))
                            //    wfDe.data.FileName = wfDe.data.FileName + "." + "xaml";

                            string pattern = "^[a-zA-Z]*://[a-zA-Z0-9.]*";
                            if (pe != null)
                                if (!string.IsNullOrEmpty(pe.WorkflowURI))
                                    wfDe.data.WorkflowURI = System.Text.RegularExpressions.Regex.Replace(wfDe.data.WorkflowURI, pattern, "");
                        }

                        // if only metadata changes e.g. name and/or desc has been updated
                        if (wfDe.data.CategoryID == catId)//&& wfDe.data.SubCategoryID == subCatId)
                        {
                            if (wfDe.data.Modified)
                                pe1 = Update(wfDe.data);
                            else
                                pe1 = Publish(wfDe.data);
                        }
                        else
                        {
                            //  var fileName = Convert.ToInt16(wfDe.data.FileName.Split('.')[0]);
                            //  wfDe.data.FileName = (fileName + 1).ToString() + ".xaml";
                            wfDe.data.CategoryID = catId;
                            wfDe.data.IncrementVersion = false;
                            if (wfDe.data.Modified)
                                pe1 = Publish(wfDe.data);
                            else
                                pe1 = Update(wfDe.data);
                        }

                        MessageBox.Show("Workflow Updated Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                }
                else
                {
                    PublishReqMsg _request = new PublishReqMsg();

                    _request.CategoryID = catgoryID;
                    //_request.SubCategoryID = subCatID;
                    _request.Description = txtDescription.Text;
                    _request.Name = txtName.Text;
                    _request.WorkflowID = workflowId;
                    _request.WorkflowVer = workflowVersion;
                    // _request.LastModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    //_request.LastModifiedOn = System.DateTime.Now;
                    _request.WorkflowURI = pe.WorkflowURI.Replace(ConfigurationManager.AppSettings["ServiceBaseUrl"], "");
                    _request.Parameters = Translators.WorkflowParameterPE_SE.WorkflowParameterListPEtoSE(pe.Parameters);
                    _request.UsesUIAutomation = checkBox1.Checked;
                    _request.IslongRunningWorkflow = chkIsPersistWF.Checked;
                    _request.IdleStateTimeout = (string.IsNullOrEmpty(txtIdleTimeOut.Text)) ? 0 : int.Parse(txtIdleTimeOut.Text);
                    _request.Tags = txtTags.Text;
                    _request.Modified = false;
                    WFService.Publish(_request);
                    MessageBox.Show("Workflow Updated Successfully", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }

                //once publish is done change the state of the workflow parameters to not-new.
                //this will avoid re-addition of these parameters is again save is called from the same instance of the wfdetails user control
                if (pe != null && pe.Parameters != null && pe.Parameters.Count > 0)
                {
                    for (int i = 0; i < pe.Parameters.Count; i++)
                    {
                        pe.Parameters[i].IsNew = false;
                    }
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
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (wfDe != null)
                    wfDe.isDirty = false;
            }

            return false;
        }

        internal WorkflowPE Publish(WorkflowPE request)
        {

            if (publishLocally)
            {
                if (request.WFContent == null)
                    request.WFContent = wfDe.GetWorkflowContent(wfDe.data);

                IAPPackage.Export(Translators.WorkflowPE_SE.WorkflowPEtoOE(request), request.WFContent);
                return request;
            }

            PublishReqMsg _request = new PublishReqMsg
            {
                CategoryID = request.CategoryID,
                //  SubCategoryID=request.SubCategoryID,
                //CreatedBy = request.CreatedBy,
                Description = request.Description,
                Name = request.Name,
                WorkflowID = request.WorkflowID,
                WorkflowURI = request.WorkflowURI,
                IncrementVersion = request.IncrementVersion,
                UsesUIAutomation = request.UsesUIAutomation,
                WorkflowVer = request.WorkflowVersion,
                WFContent = request.WFContent,
                CompanyId = request.CompanyId.ToString(),
                FileName = request.FileName,
                Modified = request.Modified,
                IslongRunningWorkflow = request.IslongRunningWorkflow,
                IdleStateTimeout = request.IdleStateTimeout,
                //StorageBaseURL = Infosys.WEM.Client.CommonServices.Instance.StorageBaseURL + request.WorkflowURI,                         
                Parameters = Translators.WorkflowParameterPE_SE.WorkflowParameterListPEtoSE(request.Parameters)

            };
            return WFService.Publish(_request);
        }

        internal WorkflowPE Update(WorkflowPE request)
        {
            if (publishLocally)
            {
                if (request.WFContent == null)
                    request.WFContent = wfDe.GetWorkflowContent(wfDe.data);

                IAPPackage.Export(Translators.WorkflowPE_SE.WorkflowPEtoOE(request), request.WFContent);
                return request;
            }

            PublishReqMsg _request = new PublishReqMsg
            {
                CategoryID = request.CategoryID,
                // SubCategoryID=request.SubCategoryID,
                //CreatedBy = request.CreatedBy,
                Description = request.Description,
                Name = request.Name,
                WorkflowID = request.WorkflowID,
                WorkflowURI = request.WorkflowURI,
                WorkflowVer = request.WorkflowVersion,
                IncrementVersion = request.IncrementVersion,
                UsesUIAutomation = request.UsesUIAutomation,
                WFContent = request.WFContent,
                CompanyId = request.CompanyId.ToString(),
                FileName = request.FileName,
                Modified = request.Modified,
                IslongRunningWorkflow = request.IslongRunningWorkflow,
                IdleStateTimeout = request.IdleStateTimeout,
                //StorageBaseURL = Infosys.WEM.Client.CommonServices.Instance.StorageBaseURL + request.WorkflowURI,                               
                Parameters = Translators.WorkflowParameterPE_SE.WorkflowParameterListPEtoSE(request.Parameters)
            };
            return WFService.Update(_request);
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
                //construct the workflow file name

                FolderBrowserDialog saveWf = new FolderBrowserDialog();
                saveWf.Description = "Select the work flow download folder:";
                if (saveWf.ShowDialog() == DialogResult.OK)
                {
                    string downloadLoc = saveWf.SelectedPath + "\\" + _downloadWfname;
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
                            System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(WorkflowPE));
                            xml.Serialize(sw, pe);
                        }

                        MessageBox.Show("Workflow downloaded.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        string err = "There is an error downloading the Workflow.";
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

        private void btnAddParam_Click(object sender, EventArgs e)
        {
            if (!pnlParameters.Visible)
                pnlParameters.Visible = true;
            pnlNewParam.Visible = true;
            pnlAddedParam.Dock = DockStyle.Top;
            _newparam = true;

            cmbIOTypes.SelectedIndex = 0;
            cmbBool.SelectedIndex = 1;
            cmbIsSecret.SelectedIndex = 0;
            cmbIsReference.SelectedIndex = 0;
            txtParamName.Text = "";
            txtDefaultValue.Text = "";
            txtAllowedValues.Text = "";
            txtParamName.Focus();
            btnAdd.Text = "Add";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlNewParam.Visible = false;
            pnlAddedParam.Dock = DockStyle.Fill;
            btnAdd.Text = "Add";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //add the new parmeter to the grid
            //if (cmbIOTypes.SelectedIndex < 0 || cmbBool.SelectedIndex < 0|| string.IsNullOrEmpty(txtParamName.Text.Trim()))
            if (string.IsNullOrEmpty(txtParamName.Text.Trim()))
            {
                MessageBox.Show("Please provide the Parameter Name.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(txtParamName.Text))
            {
                MessageBox.Show("Please provide the name without Special Characters", "Invalid Parameter name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (pe == null)
                pe = new WorkflowPE();

            if (pe.Parameters == null)
                pe.Parameters = new List<WorkflowParameterPE>();

            bool duplicate = CheckDuplicateParameter(pe.Parameters);

            if (_newparam)
            {
                if (duplicate)
                {
                    MessageBox.Show("Parameter with this name already added. Please specify another name.", "Duplicate Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtParamName.Focus();
                    return;
                }

                pe.Parameters.Add(new WorkflowParameterPE()
                {
                    AllowedValues = txtAllowedValues.Text.Trim(),
                    CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    DefaultValue = txtDefaultValue.Text.Trim(),
                    IsDeleted = false,
                    IsMandatory = cmbBool.SelectedItem != null ? bool.Parse(cmbBool.SelectedItem.ToString()) : false,
                    IsSecret = cmbIsSecret.SelectedItem != null ? bool.Parse(cmbIsSecret.SelectedItem.ToString()) : false,
                    IsReferenceKey = cmbIsReference.SelectedItem != null ? bool.Parse(cmbIsReference.SelectedItem.ToString()) : false,
                    ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                    Name = txtParamName.Text.Trim(),
                    ParamIOType = (Infosys.ATR.WFDesigner.Entities.ParameterIOTypes)Enum.Parse(typeof(Infosys.ATR.WFDesigner.Entities.ParameterIOTypes), cmbIOTypes.SelectedItem.ToString()),
                    WorkflowId = pe.WorkflowID.ToString(),
                    Id = Guid.NewGuid().ToString(), //to be used temporarily to distinguish, e.g. to be used while deleteing the just added parameter
                    IsNew = true
                });
            }
            else
            {
                //then update it in the pe.Parameters
                for (int i = 0; i < pe.Parameters.Count; i++)
                {
                    if (pe.Parameters[i].Id == selectedParamId)
                    {
                        if (!pe.Parameters[i].Name.ToLower().Equals(txtParamName.Text.ToLower()))
                            if (duplicate)
                            {
                                MessageBox.Show("Parameter with this name already added. Please specify another name.", "Duplicate Parameter Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txtParamName.Focus();
                                return;
                            }
                        pe.Parameters[i].AllowedValues = txtAllowedValues.Text.Trim();
                        pe.Parameters[i].CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        pe.Parameters[i].DefaultValue = txtDefaultValue.Text.Trim();
                        pe.Parameters[i].IsDeleted = false;
                        pe.Parameters[i].IsMandatory = cmbBool.SelectedItem != null ? bool.Parse(cmbBool.SelectedItem.ToString()) : false;
                        pe.Parameters[i].IsSecret = cmbIsSecret.SelectedItem != null ? bool.Parse(cmbIsSecret.SelectedItem.ToString()) : false;
                        pe.Parameters[i].ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        pe.Parameters[i].Name = txtParamName.Text.Trim();
                        pe.Parameters[i].ParamIOType = (Infosys.ATR.WFDesigner.Entities.ParameterIOTypes)Enum.Parse(typeof(Infosys.ATR.WFDesigner.Entities.ParameterIOTypes), cmbIOTypes.SelectedItem.ToString());
                        pe.Parameters[i].IsReferenceKey = cmbIsReference.SelectedItem != null ? bool.Parse(cmbIsReference.SelectedItem.ToString()) : false;
                        break;
                    }
                }
            }

            dgParams.DataSource = GetWFParamSubset(pe.Parameters);

            if (!pnlAddedParam.Visible)
                pnlAddedParam.Visible = true;
            //then
            pnlNewParam.Visible = false;
            pnlAddedParam.Dock = DockStyle.Fill;
        }

        private bool CheckDuplicateParameter(List<WorkflowParameterPE> parm)
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

        private List<WorkflowParameterSubSet> GetWFParamSubset(List<WorkflowParameterPE> parameters)
        {
            List<WorkflowParameterSubSet> objs = new List<WorkflowParameterSubSet>();
            parameters.ForEach(c =>
            {
                objs.Add(new WorkflowParameterSubSet() { Name = c.Name, IsMandatory = c.IsMandatory.ToString(), IOType = c.ParamIOType });
            });
            return objs;
        }

        private void dgParams_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                if (pe.Parameters.Count > 0)
                {
                    //i.e. Delete is called
                    if (MessageBox.Show("Are you sure you want to delete the parameter?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        //delete the row selected
                        pe.Parameters.RemoveAt(e.RowIndex);
                        //change the datasource of the grid to the new parameter list
                        dgParams.DataSource = GetWFParamSubset(pe.Parameters);
                        txtParamName.Text = "";
                        txtParamName.Focus();
                        _newparam = true;
                    }
                    else
                    {
                        pnlNewParam.Visible = true;
                        PopulateParameter(e.RowIndex);
                    }
                }
            }
            else if (e.RowIndex >= 0) //i.e. any other column of the row is selected, then show the details of the parameter selected
            {
                pnlNewParam.Visible = true;
                PopulateParameter(e.RowIndex);
            }
        }

        private void PopulateParameter(int row)
        {
            pnlAddedParam.Dock = DockStyle.Top;
            _newparam = false;
            WorkflowParameterPE parameter = null;
            if (pe != null &&
                (pe.Parameters != null && pe.Parameters.Count > 0))
            {
                parameter = pe.Parameters[row];

                txtParamName.Text = parameter.Name;
                txtAllowedValues.Text = parameter.AllowedValues;
                txtDefaultValue.Text = parameter.DefaultValue;

                if (parameter.IsReferenceKey)
                    cmbIsReference.SelectedIndex = 0;
                else
                    cmbIsReference.SelectedIndex = 1;

                if (parameter.IsMandatory)
                    cmbBool.SelectedIndex = 0;
                else
                    cmbBool.SelectedIndex = 1;

                if (parameter.IsSecret)
                {
                    cmbIsSecret.SelectedIndex = 1;
                    txtDefaultValue.UseSystemPasswordChar = true;
                }
                else
                    cmbIsSecret.SelectedIndex = 0;

                cmbIOTypes.SelectedIndex = (int)parameter.ParamIOType;
                selectedParamId = parameter.Id;
                btnAdd.Text = "Save";
            }
        }

        private void cmbSubCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SubCategory subCatTemop = this.cmbSubCategory.SelectedItem as SubCategory;
            //if (subCatTemop != null && pe != null)
            //{
            //    pe.CategoryID = (int)subCatTemop.SubCategoryId;
            //}
        }

        internal void SetData()
        {
            this.txtDescription.Text = wfDe.data.Description;
            this.txtName.Text = wfDe.data.Name;
            var catID = wfDe.data.CategoryID;
            this.checkBox1.Checked = wfDe.data.UsesUIAutomation;
            btnInfo.Visible = true;
            if (CheckRootCategory(catID))
            {
                cmbCategory.SelectedValue = catID;
                cmbSubCategory.SelectedItem = null;
            }
            else
            {
                SetCategory(catID);
                cmbSubCategory.SelectedValue = catID;
            }

        }

        private Boolean CheckRootCategory(string catID)
        {
            var result = CategoryDetails.Where(cat => cat.CategoryId.Equals(catID)).Select(e => e.ParentId).SingleOrDefault();
            //if ParentId is not null and greater than zero
            if (result == 0)
                return true;
            else
                return false;
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            if (pe == null)
                pe = new WorkflowPE();

            if (pe != null)
            {
                using (Info info = new Info())
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append(Environment.NewLine);
                    sb.Append(string.Format("Workflow Name :  {0}", pe.Name + Environment.NewLine));
                    sb.Append(string.Format("Category Id :  {0}", pe.CategoryID + Environment.NewLine));
                    sb.Append(string.Format("Version No :  {0}", pe.WorkflowVersion + Environment.NewLine));
                    sb.Append(string.Format("Workflow Id :  {0}", pe.WorkflowID + Environment.NewLine));
                    sb.Append(string.Format("Created By :  {0} on {1}", pe.CreatedBy, DateTime.SpecifyKind(pe.PublishedOn, DateTimeKind.Utc).ToLocalTime() + Environment.NewLine));
                    if (!string.IsNullOrEmpty(pe.LastModifiedOn))
                        sb.Append(string.Format("Last Modified By :  {0} on {1}", pe.LastModifiedBy, DateTime.SpecifyKind(Convert.ToDateTime(pe.LastModifiedOn), DateTimeKind.Utc).ToLocalTime() + Environment.NewLine));
                    info.Controls["txtOutput"].Text = sb.ToString();
                    info.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                    info.StartPosition = FormStartPosition.CenterParent;
                    info.MinimizeBox = false;
                    info.MaximizeBox = false;
                    info.Text = pe.Name;
                    info.ShowDialog();
                }
            }
        }


        private void DisableAllControls(Control ctrl, bool enable)
        {
            if (ctrl.Controls.Count > 0)
            {
                foreach (Control subControl in ctrl.Controls)
                    DisableAllControls(subControl, enable);
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

        internal void DisableControls(bool disable)
        {
            var Users = Infosys.ATR.Entities.CommonObjects.Users;
            if (Users != null)
            {
                if (Infosys.ATR.Entities.CommonObjects.IsSuperAdmin)
                    CurentUser = Users.FirstOrDefault();
                else
                    CurentUser = Users.Where(u => u.CategoryId == Convert.ToInt32(catgoryID)).FirstOrDefault();

                if (CurentUser != null)
                {

                    if (CurentUser.Role == Infosys.ATR.Entities.Roles.Guest.ToString() || CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                    {
                        for (int j = 0; j < this.Controls.Count; j++)
                            DisableAllControls(this.Controls[j], disable);

                        btnInfo.Enabled = true;

                        if (CurentUser.Role == Infosys.ATR.Entities.Roles.Agent.ToString())
                            btnRunWF.Enabled = !disable;

                        foreach (DataGridViewRow row in dgParams.Rows)
                        {
                            this.dgParams.Columns[0].Visible = false;
                            row.ReadOnly = true;
                        }
                    }
                    else
                        for (int i = 0; i < this.Controls.Count; i++)
                            this.Controls[i].Enabled = disable;
                }
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
        private void CreateContextMenuStrip()
        {
            contextMenuPublishOptions = new ContextMenuStrip();
            contextMenuPublishOptions.Items.Add(new ToolStripMenuItem() { Name = "Save as Workflow", Tag = "xaml", Text = "Save as Workflow" });
            contextMenuPublishOptions.Items.Add(new ToolStripMenuItem() { Name = "Save as Package", Tag = "iapw", Text = "Save as Package" });
            contextMenuPublishOptions.Items.Add(new ToolStripMenuItem() { Name = "Export", Tag = "export", Text = "Export" });

            foreach (ToolStripMenuItem mItem in contextMenuPublishOptions.Items)
                mItem.Click += new System.EventHandler(this.AutoFillToolStripMenuItem_Click);
        }
        private void AutoFillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            publishLocally = false;
            string m = ((ToolStripMenuItem)sender).Tag.ToString();
            if (m.Equals("xaml"))
                createPackage = false;
            else if (m.Equals("iapw"))
                createPackage = true;
            else if (m.Equals("export"))
                publishLocally = true;

            PublishWorkflow();
        }
        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (enableSaveOption)
            {
                btnSave.ContextMenuStrip = contextMenuPublishOptions;
                btnSave.ContextMenuStrip.Show(btnSave, new System.Drawing.Point(0, btnSave.Height));
            }
            else
                PublishWorkflow();
        }
        private static byte[] ConvertStreamtoByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream stream = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, read);
                }
                return stream.ToArray();
            }
        }

        private void txtIdleTimeOut_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(txtIdleTimeOut.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtIdleTimeOut.Text = "0";
            }
        }
    }

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
