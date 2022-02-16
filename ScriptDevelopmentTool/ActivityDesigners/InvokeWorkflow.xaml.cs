using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using System.ComponentModel;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using Microsoft.VisualBasic.Activities;
using Infosys.WEM.Client;
using System.Collections.ObjectModel;
using System.Configuration;
using Infosys.ATR.Entities;
using Infosys.WEM.AutomationActivity.Designers.Services;
using System.Text.RegularExpressions;
using System.Activities.Presentation.View;
using Infosys.WEM.Service.Contracts.Message;
using Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.Service.Contracts;
using Data = Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.SecureHandler;

namespace Infosys.WEM.AutomationActivity.Designers
{
    // Interaction logic for InvokeScript.xaml
    public partial class InvokeWorkflow
    {
        public const string NODATA = "No Data";
        // Module ID is 3 for Workflow
        public const string ModuleID = "3";
        public const string MODULE_TYPE = "Workflow";
        Utilies util = new Utilies();
        public InvokeWorkflow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.InvokeWorkflowActivity_Loaded);
        }
        /// <summary> 
        /// Handles the Loaded event of the InvokeWorkflowActivity WFActivity. 
        /// </summary> 
        /// <param name="sender">The source of the event.</param> 
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param> 
        public void InvokeWorkflowActivity_Loaded(object sender, RoutedEventArgs e)
        {
            ModelItem myItem = this.ModelItem;
            do
            {
                myItem = myItem.Parent;

            }
            while (myItem.Parent != null && myItem.Parent.ItemType != typeof(Sequence));
            if (myItem.Parent == null)
            {
                MessageBox.Show(
                    ApplicationConstants.ACTIVITYPLACEMENTNOTINSEQUENCE,
                    ApplicationConstants.ACTIVITYPLACEMENTNOTINSEQUENCE_TITLE,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }

        }

        private string scriptCategory = "";
        private string scriptCategoryName = "";
        Dictionary<string, string> wfIdentifier = new Dictionary<string, string>();
        private Dictionary<string, List<WorkflowParam>> wfParameters = null;
        private string param = "";
        private Dictionary<string, Dictionary<int, string>> wfSubCat = new Dictionary<string, Dictionary<int, string>>();
        private WorkflowAutomation wfRepoSvc;
        private CommonRepository commonRepoSvc;

        TreeViewItem _root;
        public List<Category> categories { get; set; }
        List<string> catList = null;
        TreeViewItem trvSelectedItem;
        int categoryId = 0;
        private string userSecurity = "";
        private bool isSuperAdmin = false;

        private List<int> currentUserCategories;
        public string WorkflowCategory
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["WorkflowCategory"].ToString()))
                    scriptCategory = myItem.Properties["WorkflowCategory"].Value.GetCurrentValue().ToString();

                return scriptCategory;
            }
            set
            {
                scriptCategory = value;
                OnPropertyChanged("WorkflowCategory");
            }
        }

        public string WorkflowCategoryName
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["WorkflowCategoryName"].ToString()))
                    scriptCategoryName = myItem.Properties["WorkflowCategoryName"].Value.GetCurrentValue().ToString();

                return scriptCategoryName;
            }
            set
            {
                scriptCategoryName = value;
                OnPropertyChanged("WorkflowCategoryName");
            }
        }


        public List<string> WorkflowNames
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (myItem.Properties["WorkflowCategoryName"].Value != null)
                {
                    System.Activities.Presentation.Model.ModelPropertyCollection m = myItem.Properties["WorkflowRepositoryBaseURI"].Value.Properties; ;
                    txtCategory.Text = myItem.Properties["WorkflowCategoryName"].Value.GetCurrentValue().ToString();
                }

                if ((trvSelectedItem != null) && (wfIdentifier.Count > 0))
                    return wfIdentifier.Keys.ToList();
                else if (myItem.Properties["WorkflowName"].Value != null && wfIdentifier.Count == 0)
                {
                    // Logic to handle when user loads an already designed workflow
                    List<string> wfNames = new List<string>() { myItem.Properties["WorkflowName"].Value.GetCurrentValue().ToString() };
                    wfNames.Sort();
                    return wfNames;
                }
                else
                    return null;

            }
            set
            {

                if ((cmbWorkflowName.SelectedItem != null) && (cmbWorkflowName.SelectedItem.ToString() != NODATA))
                {
                    ModelItem.Properties["WorkflowId"].SetValue(wfIdentifier[cmbWorkflowName.SelectedValue.ToString()]);
                }
                OnPropertyChanged("WorkflowNames");
            }
        }

        public List<WorkflowParam> Parameters
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if ((cmbWorkflowName.SelectedItem != null) && (cmbWorkflowName.SelectedItem.ToString() != NODATA) && (wfParameters != null && wfParameters.Count > 0))
                {
                    if (wfParameters.ContainsKey((string)cmbWorkflowName.SelectedItem))
                    {
                        btnSave.IsEnabled = true;
                        ModelItem.Properties["Parameters"].SetValue(wfParameters[(string)cmbWorkflowName.SelectedItem]);
                        return wfParameters[(string)cmbWorkflowName.SelectedItem].ToList<WorkflowParam>();
                    }
                    else
                    {
                        btnSave.IsEnabled = false;
                        return null;
                    }

                }
                else if (myItem.Properties["Parameters"].Value != null && wfParameters == null) // Logic to handle when user loads an already designed workflow
                {
                    btnSave.IsEnabled = true;
                    return (List<WorkflowParam>)myItem.Properties["Parameters"].Value.GetCurrentValue();

                }
                else
                {
                    btnSave.IsEnabled = false;
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    wfParameters[param] = value;
                    OnPropertyChanged("Parameters");
                }

                if ((cmbWorkflowName.SelectedItem != null) && (cmbWorkflowName.SelectedItem.ToString() != NODATA))
                {
                    ModelItem.Properties["Parameters"].SetValue(wfParameters[(string)cmbWorkflowName.SelectedItem]);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        private void cmbWorkflowName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbWorkflowName.IsEnabled)
                RemoveWFVariables();

            //POPULATE scrParameters
            if (cmbWorkflowName.SelectedIndex == -1)
            {
                ObservableCollection<string> dummyList = new ObservableCollection<string>();
                dummyList.Clear();
                cmbWFParams.ItemsSource = dummyList;
                cmbWFParams.SelectedIndex = -1;
                btnSave.IsEnabled = false;
            }
            else
            {
                cmbWFParams.ItemsSource = Parameters;
                cmbWFParams.DisplayMemberPath = "ParameterName";
                cmbWFParams.SelectedValuePath = "ParameterName";
                cmbWFParams.SelectedIndex = 0;
                ModelItem myItem = this.ModelItem;
                if (wfIdentifier.Count > 0)
                    ModelItem.Properties["WorkflowId"].SetValue(wfIdentifier[cmbWorkflowName.SelectedValue.ToString()]);
                if ((Parameters == null) || (Parameters.Count == 0))
                {
                    btnSave.IsEnabled = false;
                }
                else
                    btnSave.IsEnabled = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if ((cmbWorkflowName == null) || (cmbWorkflowName.SelectedItem == null) || (cmbWorkflowName.SelectedItem.ToString() == NODATA))
            {
                MessageBox.Show("Workflow Name is required.");
                return;
            }

            if (string.IsNullOrEmpty(txtBoxParamVal.Text))
            {
                MessageBox.Show("Parameter value is required.");
                return;
            }

            // Format parameter name
            string paramsFormatted = cmbWFParams.SelectedValue.ToString().
                Replace(" (" + Data.ParamDirection.In + ")", "").
                Replace(" (" + Data.ParamDirection.Out + ")", "").
                Replace(" (" + Data.ParamDirection.InAndOut + ")", "");
            string scripNameFormatted = cmbWorkflowName.SelectedItem.ToString().Replace(" ", "");
            if (paramsFormatted.Contains(" "))
                paramsFormatted = paramsFormatted.Replace(" ", "___");
            string variableName = string.Format(ApplicationConstants.VARIABLENAMEWF_FORMAT, scripNameFormatted, paramsFormatted);
            Variable<string> mySimpleVar = new Variable<string>
            {
                Name = variableName
            };

            ModelItem myItem = this.ModelItem;
            do
            {
                myItem = myItem.Parent;
            }
            while (myItem.Parent != null && myItem.Parent.ItemType != typeof(Sequence));
            if (myItem.Parent == null)
            {
                MessageBox.Show(
                    ApplicationConstants.ACTIVITYPLACEMENTNOTINSEQUENCE + "\n" + ApplicationConstants.ACTIVITYVARIABLENOTCREATED,
                    ApplicationConstants.ACTIVITYPLACEMENTNOTINSEQUENCE_TITLE,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            ModelProperty mp = myItem.Parent.Properties["Variables"];

            IEnumerable<System.Activities.Variable> mis = ((IEnumerable<System.Activities.Variable>)mp.ComputedValue).Where(
            p => p.Name == variableName);

            string value = "";
            Regex rgx = new Regex(@"[^\s""]+|""[^""]*""", RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(txtBoxParamVal.Text);
            if (matches.Count > 1 && cmbWFParams.SelectedValue.ToString().ToLower().Equals("arguments (in)"))
            {
                foreach (Match match in matches)
                {
                    // Add escape character
                    value = value + "\"" + match.Value + "\"" + " ";
                }
                //Remove last blank space
                value = value.Substring(0, value.Length - 1);
                //Add commas
                value = "\"" + value + "\"";
            }
            else
                value = txtBoxParamVal.Text;

            if (mis.Count() > 0)
            {
                System.Activities.Variable varExists = mis.First();
                if (!string.IsNullOrEmpty(txtBoxParamVal.Text))
                {
                    //Remove variable and add again to update context with latest value of variable
                    myItem.Parent.Properties["Variables"].Collection.Remove(varExists);
                    ModelItem.Properties["Parameters"].Collection.Remove(varExists);

                    mySimpleVar.Default = new VisualBasicValue<string>
                    {
                        ExpressionText = (string)((Parameters.FindAll(x => x.ParameterName.Equals(cmbWFParams.SelectedValue.ToString())).FirstOrDefault().IsSecret) ? string.Format("\"{0}\"", SecurePayload.Secure(value.Replace("\"", ""), "IAP2GO_SEC!URE")) : value)
                    };
                    myItem.Parent.Properties["Variables"].Collection.Add(mySimpleVar);

                    //Logic to toggle pane window so that variable value can be refreshed
                    (DesignerView.ToggleVariableDesignerCommand as RoutedCommand).Execute(null, this.Designer.Context.Services.GetService<DesignerView>());
                    (DesignerView.ToggleVariableDesignerCommand as RoutedCommand).Execute(null, this.Designer.Context.Services.GetService<DesignerView>());

                }
            }
            else
            {
                mySimpleVar.Default = new VisualBasicValue<string>
                {
                    ExpressionText = (string)((Parameters.FindAll(x => x.ParameterName.Equals(cmbWFParams.SelectedValue.ToString())).FirstOrDefault().IsSecret) ? string.Format("\"{0}\"", SecurePayload.Secure(value.Replace("\"", ""), "IAP2GO_SEC!URE")) : value)
                };
                myItem.Parent.Properties["Variables"].Collection.Add(mySimpleVar);
            }
        }
        /// <summary>
        /// This is executed when parameter value is changed in the parameter dropdown.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">SelectionChangedEventArgs containing event data</param>
        private void cmbWFParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtBoxParamVal.Text = "";
            ModelItem myItem = this.ModelItem;
            do
            {
                myItem = myItem.Parent;
            }
            while (myItem.Parent != null && myItem.Parent.ItemType != typeof(Sequence));
            if (myItem.Parent != null)
            {
                ModelProperty mp = myItem.Parent.Properties["Variables"];
                IEnumerable<System.Activities.Variable> mis = ((IEnumerable<System.Activities.Variable>)mp.ComputedValue).Where(
                    p => p.Name == cmbWorkflowName.SelectedItem + "_" + cmbWFParams.SelectedItem);


                System.Activities.Variable varName = mis.FirstOrDefault<System.Activities.Variable>();
                if (varName != null)
                {

                    if (varName.Default.GetType().Name.Contains("VisualBasicValue"))
                    {

                        txtBoxParamVal.Text = ((VisualBasicValue<string>)varName.Default).ExpressionText;
                    }
                    else
                    {
                        txtBoxParamVal.Text = ((System.Activities.Expressions.Literal<string>)varName.Default).Value;
                    }
                }
                else
                {
                    // Logic to display default value of parameter (if specified)
                    WorkflowParam param = cmbWFParams.SelectedItem as WorkflowParam;
                    if (param != null && !string.IsNullOrEmpty(param.ParameterValue))
                    {
                        if (param.ParameterValue.Contains("\""))
                            txtBoxParamVal.Text = param.ParameterValue;
                        else
                            txtBoxParamVal.Text = "\"" + param.ParameterValue + "\"";
                    }
                    else
                        txtBoxParamVal.Text = "";
                }
            }
        }

        /// <summary>
        /// This method is used to fetch and load categories.
        /// </summary>
        /// <param name="sender">sender object </param>
        /// <param name="e">RoutedEventArgs comtaining event data</param>
        private void btnFetch_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(txtBoxSvcBaseUri.Text))
            {
                RemoveWFVariables();

                //Refresh all previous states
                wfIdentifier = new Dictionary<string, string>();
                cmbWorkflowName.ItemsSource = null;

                if (Tree1.Items != null)
                    Tree1.Items.Clear();

                cmbWFParams.ItemsSource = null;

                txtBoxParamVal.Text = "";
                string fetchCategoryUri = "";
                string serviceBaseUri = "";

                serviceBaseUri = txtBoxSvcBaseUri.Text;

                try
                {
                    CheckUserSecurity(serviceBaseUri + ApplicationConstants.SECURITY_SERVICEINTERFACE);

                    fetchCategoryUri = serviceBaseUri + ApplicationConstants.WORKFLOW_SERVICEINTERFACE;
                    wfRepoSvc = new WorkflowAutomation(fetchCategoryUri);
                    string companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];

                    if (categories != null)
                    {
                        categories.Clear();
                    }

                    commonRepoSvc = new CommonRepository(serviceBaseUri + ApplicationConstants.COMMON_SERVICEINTERFACE);

                    var response = commonRepoSvc.ServiceChannel.GetAllCategoriesByCompany(companyId, ModuleID);

                    categories = Translators.CategoryPE_SE.CategoryListSEtoPE(response.Categories.ToList());
                    if (!isSuperAdmin)
                    {
                        if (userSecurity.Equals("AllowAuthorised"))
                        {
                            var userCategory = currentUserCategories as List<int>;
                            if (userCategory != null)
                            {
                                categories = this.categories.Where(c => userCategory.Contains(Convert.ToInt32(c.CategoryId)) || c.CompanyId == 0).
                                Select(c => new Category
                                {
                                    CreatedBy = c.CreatedBy,
                                    Description = c.Description,
                                    CategoryId = c.CategoryId,
                                    ModifiedBy = c.ModifiedBy,
                                    Name = c.Name,
                                    ParentId = c.ParentId,
                                    CompanyId = c.CompanyId
                                }).ToList();

                            }
                        }
                    }

                    for (int i = 0; i < categories.Count; i++)
                    {
                        var c = categories[i];
                        if (c.CompanyId == 0 && c.ParentId == 0)
                        {

                            var subCat = categories.Where(sc => sc.CompanyId == 0 && sc.ParentId == Convert.ToInt32(c.CategoryId));
                            if (subCat == null || subCat.Count() == 0)
                            {
                                categories.Remove(c);
                            }
                        }
                    }

                    if (categories != null)
                    {
                        _root = new TreeViewItem();
                        _root.Header = "Categories";
                        Tree1.Items.Add(_root);
                        _root.IsExpanded = true;

                        categories.Where(c => c.ParentId == 0).ToList().
                            ForEach(sc =>
                            {
                                AddNode(sc, null);
                            });

                        txtCategory.Text = "Please select category";

                        cmbWorkflowName.IsEnabled = true;
                        txtCategory.IsEnabled = true;
                    }

                    ModelItem myItem = this.ModelItem;
                    string scriptCatExistsVal = "";
                    if (myItem.Properties["WorkflowCategory"].Value != null)
                    {
                        scriptCatExistsVal = myItem.Properties["WorkflowCategory"].Value.GetCurrentValue().ToString().Trim();
                    }

                    myItem.Properties["WorkflowRepositoryBaseURI"].SetValue(InArgument<string>.FromValue(serviceBaseUri));

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("There was no endpoint listening"))
                        MessageBox.Show("Unable to connect to " + txtBoxSvcBaseUri.Text + " .Please check for network connections");
                    else
                        MessageBox.Show(ex.Message);
                }
            }
            else
            {

                MessageBox.Show("Please specify the Workflow Metadata Repository service base URI.");
            }

        }

        /// <summary>
        /// This method is used to check security settings for the user.
        /// </summary>
        private void CheckUserSecurity(string securityServiceUrl)
        {

            var overrideSecurity = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideSecurity"]);

            if (!overrideSecurity)
            {
                userSecurity = "AllowAuthorised";
                if (IsActiveUser(securityServiceUrl))
                {
                   
                    IsSuperAdmin(util.GetAlias(), securityServiceUrl);
                }
                else
                {
                    throw new Exception("Oops ! You do not have access to this application. Contact Administrator");
                }

            }
            else
            {
                userSecurity = "AllowAll";
                isSuperAdmin = false;

            }

        }
        /// <summary>
        /// This method is used to check if user is super admin.
        /// </summary>
        /// <param name="p">user name</param>
        private void IsSuperAdmin(string p, string securityServiceUrl)
        {
            
            isSuperAdmin = WFService.IsSuperAdmin(util.GetAlias(), ConfigurationManager.AppSettings["Company"], securityServiceUrl).IsSuperAdmin;
            //isSuperAdmin = true;
        }

    
       

        /// <summary>
        /// This method checks if current user is active and sets up properties accordingly.
        /// </summary>
        /// <returns></returns>
        private bool IsActiveUser(string securityServiceUrl)
        {
            var name =util.GetAlias();
            var response = WFService.GetUsers(name, securityServiceUrl, ConfigurationManager.AppSettings["Company"]);
            if (response.Users != null && response.Users.Count > 0)
            {
                List<Users> users = new List<Users>();
                List<int> categories = new List<int>();
                response.Users.ForEach(u =>
                {
                    Users _user = new Users();
                    _user.Alias = u.Alias;
                    _user.Role = Enum.GetName(typeof(Roles), u.Role);
                    _user.DisplayName = u.DisplayName;
                    _user.CategoryId = u.CategoryId;
                    _user.GroupId = u.GroupId.GetValueOrDefault();
                    _user.Id = u.UserId;
                    users.Add(_user);
                    categories.Add(u.CategoryId);
                });

                currentUserCategories = categories;
                return true;
            }
            return false;


        }
        /// <summary>
        /// This method is used to remove already added variables (if any)
        /// </summary>
        private void RemoveWFVariables()
        {
            ModelItem myItem = this.ModelItem;
            do
            {
                myItem = myItem.Parent;
            }
            while (myItem.Parent != null && myItem.Parent.ItemType != typeof(Sequence));

            if (myItem.Parent != null)
            {
                ModelProperty mp = myItem.Parent.Properties["Variables"];
                //mp.ClearValue();
                //persist state of the response so user is not prompted repeatedly for other variables in the scope
                bool shouldRemoveVariables = false;
                //check with user has already responded to the query
                bool userPrompted = false;
                IEnumerable<System.Activities.Variable> mis = ((IEnumerable<System.Activities.Variable>)mp.ComputedValue).ToList();
                foreach (System.Activities.Variable varName in mis)
                {
                    if (varName.Name.Contains(ApplicationConstants.VARIABLENAMEWF_PREFIX))
                    {

                        if (!shouldRemoveVariables & !userPrompted)
                        {
                            MessageBoxResult result = MessageBox.Show(string.Format(ApplicationConstants.DELETEVARIABLES, MODULE_TYPE), ApplicationConstants.DELETEVARIABLES_WF_TITLE,
                                                 MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            userPrompted = true;
                            if (result == MessageBoxResult.Yes)
                                shouldRemoveVariables = true;

                        }

                        if (shouldRemoveVariables)
                        {
                            myItem.Parent.Properties["Variables"].Collection.Remove(varName);

                            if (ModelItem.Properties["Parameters"].Collection != null)
                            {
                                ModelItem.Properties["Parameters"].Collection.Remove(varName);
                            }
                        }
                    }

                }
            }
        }


        /// <summary>
        /// This method adds categories to treeview.
        /// </summary>
        /// <param name="c">category object</param>
        /// <param name="parent">parent category</param>
        private void AddNode(Category c, TreeViewItem parent)
        {
            if (parent == null)
            {
                parent = new TreeViewItem();
                parent.Header = c.Name;
                parent.Tag = c;
                _root.Items.Add(parent);
            }
            else
            {
                TreeViewItem child = new TreeViewItem();
                child.Header = c.Name;
                child.Tag = c;
                parent.Items.Add(child);
                parent = child;
            }

            categories.Where(sc => sc.ParentId == c.CategoryId).ToList().ForEach(child =>
            {
                AddNode(child, parent);
            });

        }
        private void Tree1_Initialized(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// This event is fired when mouse is put into textbox.
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">MouseButtonEventArgs containing event data</param>
        private void txtCategory_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PopupCat.Placement = System.Windows.Controls.Primitives.PlacementMode.RelativePoint;
            PopupCat.VerticalOffset = txtCategory.Height;
            PopupCat.StaysOpen = true;
            PopupCat.Height = Tree1.Height;
            PopupCat.Width = txtCategory.Width;
            PopupCat.IsOpen = true;
        }

        /// <summary>
        /// This event is fired when user puts mouse inside txtCategory textbox.
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">MouseEventArgs containing event data</param>
        private void txtCategory_MouseEnter(object sender, MouseEventArgs e)
        {
            PopupCat.StaysOpen = true;
        }

        /// <summary>
        /// This event is fired when mouse leaves txtCategory textbox.
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">MouseEventArgs containing event data</param>
        private void txtCategory_MouseLeave(object sender, MouseEventArgs e)
        {
            PopupCat.StaysOpen = false;
        }

        /// <summary>
        /// This method is fired when category is selected in the dropdown.
        /// </summary>
        /// <param name="sender">source of the event</param>
        /// <param name="e">RoutedPropertyChangedEventArgs containing event data</param>
        private void Tree1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string catFullName = "";
            trvSelectedItem = Tree1.SelectedItem as TreeViewItem;
            if (trvSelectedItem != null)
            {
                if (trvSelectedItem.Tag == null)
                {
                    txtCategory.Text = "";
                    PopupCat.IsOpen = false;
                    MessageBox.Show("Please select valid category");
                    txtCategory.Focus();
                    if (categories != null)
                        categories.Clear();
                    return;
                }
                // if (trvItem.Items.Count != 0) return;

                catList = new List<string>();
                Category c = trvSelectedItem.Tag as Category;

                if (CheckRootCategory(c.CategoryId.ToString()))
                {
                    txtCategory.Text = trvSelectedItem.Header.ToString();
                }
                else
                {
                    string catId = FindParentCategory(c.CategoryId.ToString());

                    catList.Reverse();
                    foreach (string category in catList)
                    {
                        if (string.IsNullOrEmpty(catFullName))
                            catFullName = category;
                        else
                            catFullName = catFullName + "." + category;
                    }

                    txtCategory.Text = catFullName; // value.Substring(0, value.Length - 1);
                    catList.Clear();
                    catFullName = "";
                }

                //header.Text = trvItem.Header.ToString();
                PopupCat.IsOpen = false;
                categoryId = c.CategoryId;

                FetchWorkflows(c.CategoryId, c.Name);
            }
        }

        /// <summary>
        /// This method is used to fecth and load workflows based on category passed.
        /// </summary>
        /// <param name="catId">category id</param>
        /// <param name="catName">category name</param>
        private void FetchWorkflows(int catId, string catName)
        {
            ModelItem myItem = this.ModelItem;
            myItem.Properties["WorkflowCategory"].SetValue(categoryId.ToString());
            myItem.Properties["WorkflowCategoryName"].SetValue(txtCategory.Text);

            //Fetch scripts associated to the category selected

            GetAllActiveWorkflowsByCategoryResMsg wfList = ((IWorkflowAutomation)wfRepoSvc.ServiceChannel).GetAllActiveWorkflowsByCategory(catId.ToString());

            //Message.GetAllScriptDetailsResMsg wfList = ((IScriptRepository)wfRepoSvc.ServiceChannel).GetAllScriptDetails(catId.ToString());
            WorkflowNames = null;

            if (wfIdentifier.Count > 0)
                wfIdentifier.Clear();
            if (wfList.CategoryWorkflowMapping != null && wfList.CategoryWorkflowMapping.Count > 0)
            {
                wfIdentifier = new Dictionary<string, string>();
                wfParameters = new Dictionary<string, List<WorkflowParam>>();
                WorkflowParam param = null;
                foreach (WorkflowMaster wfDetails in wfList.CategoryWorkflowMapping)
                {
                    List<WorkflowParam> paramterList = new List<WorkflowParam>();
                    foreach (Infosys.WEM.Service.Contracts.Data.WorkflowParam parameter in wfDetails.Parameters)
                    {
                        param = new WorkflowParam();
                        string direction = " (" + parameter.ParamType.ToString() + ")";
                        param.ParameterName = parameter.Name + direction;
                        param.ParameterValue = parameter.DefaultValue;
                        param.IsSecret = parameter.IsSecret;
                        paramterList.Add(param);
                    }
                    wfParameters.Add(wfDetails.Name, paramterList);
                    if (!wfIdentifier.ContainsKey(wfDetails.Name))
                        wfIdentifier.Add(wfDetails.Name, wfDetails.WorkflowID.ToString());
                    else
                        MessageBox.Show(String.Format(ApplicationConstants.ITEMDUPLICATE, MODULE_TYPE, wfDetails.Name),
                            String.Format(ApplicationConstants.ITEMDUPLICATE_TITLE, MODULE_TYPE),
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                }

                //populate all parameters list as Parameter name (In) or (Out)
                //Re-bind the control source with the updated scriptnames
                var sortedWF = WorkflowNames.OrderBy(s => s).ToList();
                cmbWorkflowName.ItemsSource = sortedWF;
                cmbWorkflowName.SelectedIndex = 0;
                ModelItem.Properties["WorkflowId"].SetValue(wfIdentifier[cmbWorkflowName.SelectedValue.ToString()]);
            }
            else
            {
                ObservableCollection<string> dummyList = new ObservableCollection<string>();
                dummyList.Clear();
                cmbWorkflowName.ItemsSource = dummyList;
                cmbWorkflowName.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// This method is used to check if the category is the root category.
        /// </summary>
        /// <param name="catID">category</param>
        /// <returns>true/false</returns>
        private Boolean CheckRootCategory(string catID)
        {
            var result = categories.Where(cat => cat.CategoryId.ToString().Equals(catID)).Select(e => e.ParentId).SingleOrDefault();
            //if ParentId is not null and greater than zero
            if (result == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method is used to find the parent category at the root level.
        /// </summary>
        /// <param name="catID">Category Id</param>
        /// <returns>Id of root category</returns>
        public string FindParentCategory(string catID)
        {
            string categoryId = catID;
            var result = categories.Where(cat => cat.CategoryId.ToString().Equals(catID)).Select(e => new { e.CategoryId, e.ParentId, e.Name }).Single();
            if (result != null && result.ParentId > 0)
            {
                catList.Add(result.Name);
                categoryId = FindParentCategory(result.ParentId.ToString());
            }
            else
            {
                catList.Add(result.Name);
                return result.CategoryId.ToString();
            }
            return categoryId;
        }
    }

    /// <summary>
    /// Categoty class to hold category details
    /// </summary>
    //public class Category
    //{
    //    public int CategoryId { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string ModifiedBy { get; set; }
    //    public int ParentId { get; set; }
    //    public int CompanyId { get; set; }
    //}

    /// <summary>
    /// Categoty class to hold category details
    /// </summary>
    public class WorkflowParam
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public bool IsSecret { get; set; }
    }

}

