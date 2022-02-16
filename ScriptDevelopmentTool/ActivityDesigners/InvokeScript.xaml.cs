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
using Infosys.WEM.Scripts.Service.Contracts;
using Message = Infosys.WEM.Scripts.Service.Contracts.Message;
using Data = Infosys.WEM.Scripts.Service.Contracts.Data;
using System.Collections.ObjectModel;
using System.Configuration;
using Infosys.ATR.Entities;
using Infosys.WEM.AutomationActivity.Designers.Services;
using System.Text.RegularExpressions;
using System.Activities.Presentation.View;
using Infosys.WEM.SecureHandler;
using Infosys.WEM.ScriptExecutionLibrary;


namespace Infosys.WEM.AutomationActivity.Designers
{
    // Interaction logic for InvokeScript.xaml
    public partial class InvokeScript
    {
        public const string NODATA = "No Data";
        public const string ModuleID = "2";
        public const string MODULE_TYPE = "Script";
        Utilies util = new Utilies();
        public InvokeScript()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(this.InvokeScriptActivity_Loaded);
        }
        /// <summary> 
        /// Handles the Loaded event of the InvokeScriptActivity WFActivity. 
        /// </summary> 
        /// <param name="sender">The source of the event.</param> 
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param> 
        public void InvokeScriptActivity_Loaded(object sender, RoutedEventArgs e)
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
        Dictionary<string, int> scriptIdentifier = new Dictionary<string, int>();
        private Dictionary<string, List<Parameter>> scrParameters = null; //new Dictionary<string, List<Parameter>>();
        private string param = "";
        private Dictionary<string, Dictionary<int, string>> scriptSubCat = new Dictionary<string, Dictionary<int, string>>();
        private ScriptRepository scriptRepoSvc;
        private CommonRepository commonRepoSvc;

        TreeViewItem _root;
        public List<Category> categories { get; set; }
        List<string> catList = null;
        TreeViewItem trvSelectedItem;
        int categoryId = 0;
        private string userSecurity = "";
        private bool isSuperAdmin = false;

        private List<int> currentUserCategories;



        public string ScriptCategory
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["ScriptCategory"].ToString()))
                    scriptCategory = myItem.Properties["ScriptCategory"].Value.GetCurrentValue().ToString();

                return scriptCategory;
            }
            set
            {
                scriptCategory = value;
                OnPropertyChanged("ScriptCategory");
            }
        }

        public string ScriptCategoryName
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["ScriptCategoryName"].ToString()))
                    scriptCategoryName = myItem.Properties["ScriptCategoryName"].Value.GetCurrentValue().ToString();

                return scriptCategoryName;
            }
            set
            {
                scriptCategoryName = value;
                OnPropertyChanged("ScriptCategoryName");
            }
        }


        public List<string> ScriptNames
        {
            get
            {
                ////todo 2: Fetch Script names for category selected and load dicScriptNames
                //if (cmbScriptSubCat == null || cmbScriptSubCat.SelectedItem == null)
                //{
                //    return dicScriptNames["NoData"];
                //}
                //else if (!dicScriptNames.ContainsKey((string)cmbScriptSubCat.SelectedItem))
                //{
                //    return dicScriptNames["NoData"];
                //}
                //else
                //{
                //    return dicScriptNames[(string)cmbScriptSubCat.SelectedItem];
                //}
                //if (scriptName.Count > 0)
                //    return scriptName;
                //else
                //    return null;
                ModelItem myItem = this.ModelItem;
                if (myItem.Properties["ScriptCategoryName"].Value != null)
                {
                    System.Activities.Presentation.Model.ModelPropertyCollection m = myItem.Properties["ScriptRepositoryBaseURI"].Value.Properties; ;
                    txtCategory.Text = myItem.Properties["ScriptCategoryName"].Value.GetCurrentValue().ToString();
                }

                if ((trvSelectedItem != null) && (scriptIdentifier.Count > 0))
                    return scriptIdentifier.Keys.ToList();
                else if (myItem.Properties["ScriptName"].Value != null && scriptIdentifier.Count == 0) // Logic to handle when user loads an already designed workflow
                    return new List<string>() { myItem.Properties["ScriptName"].Value.GetCurrentValue().ToString() };
                else
                    return null;

            }
            set
            {
                //dicScriptNames[(string)cmbScriptSubCat.SelectedItem] = value;
                //OnPropertyChanged("ScriptNames");
                //scriptIdentifier. = value;

                if ((cmbScriptName.SelectedItem != null) && (cmbScriptName.SelectedItem.ToString() != NODATA))
                {
                    ModelItem.Properties["ScriptId"].SetValue(scriptIdentifier[cmbScriptName.SelectedValue.ToString()]);
                }
                OnPropertyChanged("ScriptNames");
            }
        }

        public List<Parameter> Parameters
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if ((cmbScriptName.SelectedItem != null) && (cmbScriptName.SelectedItem.ToString() != NODATA) && (scrParameters != null && scrParameters.Count > 0))
                {
                    if (scrParameters.ContainsKey((string)cmbScriptName.SelectedItem))
                    {
                        btnSave.IsEnabled = true;
                        ModelItem.Properties["Parameters"].SetValue(scrParameters[(string)cmbScriptName.SelectedItem]);
                        return scrParameters[(string)cmbScriptName.SelectedItem].ToList<Parameter>();
                    }
                    else
                    {
                        btnSave.IsEnabled = false;
                        return null;
                    }

                }
                else if (myItem.Properties["Parameters"].Value != null && scrParameters == null) // Logic to handle when user loads an already designed workflow
                {
                    btnSave.IsEnabled = true;
                    return (List<Parameter>)myItem.Properties["Parameters"].Value.GetCurrentValue();

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
                    scrParameters[param] = value;
                    OnPropertyChanged("Parameters");
                }

                if ((cmbScriptName.SelectedItem != null) && (cmbScriptName.SelectedItem.ToString() != NODATA))
                {
                    ModelItem.Properties["Parameters"].SetValue(scrParameters[(string)cmbScriptName.SelectedItem]);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        private void cmbScriptName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbScriptName.IsEnabled)
                RemoveWFVariables();

            //POPULATE scrParameters
            if (cmbScriptName.SelectedIndex == -1)
            {
                ObservableCollection<string> dummyList = new ObservableCollection<string>();
                dummyList.Clear();
                cmbScriptParams.ItemsSource = dummyList;
                cmbScriptParams.SelectedIndex = -1;
                btnSave.IsEnabled = false;
            }
            else
            {
                cmbScriptParams.ItemsSource = Parameters;
                cmbScriptParams.DisplayMemberPath = "ParameterName";
                cmbScriptParams.SelectedValuePath = "ParameterName";
                cmbScriptParams.SelectedIndex = 0;
                ModelItem myItem = this.ModelItem;
                if (scriptIdentifier.Count > 0)
                    ModelItem.Properties["ScriptId"].SetValue(scriptIdentifier[cmbScriptName.SelectedValue.ToString()]);
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

            if ((cmbScriptName == null) || (cmbScriptName.SelectedItem == null) || (cmbScriptName.SelectedItem.ToString() == NODATA))
            {
                MessageBox.Show("Script Name is required.");
                return;
            }

            if (string.IsNullOrEmpty(txtBoxParamVal.Text))
            {
                MessageBox.Show("Parameter value is required.");
                return;
            }

            // Format parameter name
            string paramsFormatted = cmbScriptParams.SelectedValue.ToString().
                Replace(" (" + Data.ParamDirection.In + ")", "").
                Replace(" (" + Data.ParamDirection.Out + ")", "").
                Replace(" (" + Data.ParamDirection.InAndOut + ")", "");
            string scripNameFormatted = cmbScriptName.SelectedItem.ToString().Replace(" ", "");
            if (paramsFormatted.Contains(" "))
                paramsFormatted = paramsFormatted.Replace(" ", "___");
            string variableName = string.Format(ApplicationConstants.VARIABLENAMESCRIPT_FORMAT, scripNameFormatted, paramsFormatted);
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
            if (matches.Count > 1 && cmbScriptParams.SelectedValue.ToString().ToLower().Equals("arguments (in)"))
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
                        ExpressionText = (string)((Parameters.FindAll(x => x.ParameterName.Equals(cmbScriptParams.SelectedValue.ToString())).FirstOrDefault().IsSecret) ? string.Format("\"{0}\"", SecurePayload.Secure(value.Replace("\"", ""), "IAP2GO_SEC!URE")) : value)
                    };
                    myItem.Parent.Properties["Variables"].Collection.Add(mySimpleVar);

                    //if (this.Designer.WorkflowShellBarItemVisibility == ShellBarItemVisibility.All)
                    //{
                    //    this.Designer.WorkflowShellBarItemVisibility = ShellBarItemVisibility.All;
                    //    this.Designer.WorkflowShellBarItemVisibility = ShellBarItemVisibility.None; ;
                    //}
                    //else if (this.Designer.WorkflowShellBarItemVisibility == ShellBarItemVisibility.Variables)
                    //{
                    //    this.Designer.WorkflowShellBarItemVisibility = ShellBarItemVisibility.None;
                    //    this.Designer.WorkflowShellBarItemVisibility = ShellBarItemVisibility.All; ;
                    //}

                    //Logic to toggle pane window so that variable value can be refreshed
                    (DesignerView.ToggleVariableDesignerCommand as RoutedCommand).Execute(null, this.Designer.Context.Services.GetService<DesignerView>());
                    (DesignerView.ToggleVariableDesignerCommand as RoutedCommand).Execute(null, this.Designer.Context.Services.GetService<DesignerView>());

                    // ((Microsoft.VisualBasic.Activities.VisualBasicValue<string>)varExists.Defult).ExpressionText = value;
                }
            }
            else
            {
                mySimpleVar.Default = new VisualBasicValue<string>
                {
                    ExpressionText = (string)((Parameters.FindAll(x => x.ParameterName.Equals(cmbScriptParams.SelectedValue.ToString())).FirstOrDefault().IsSecret) ? string.Format("\"{0}\"", SecurePayload.Secure(value.Replace("\"", ""), "IAP2GO_SEC!URE")) : value)
                };
                myItem.Parent.Properties["Variables"].Collection.Add(mySimpleVar);
            }

        }
        /// <summary>
        /// This is executed when parameter value is changed in the parameter dropdown.
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">SelectionChangedEventArgs containing event data</param>
        private void cmbScriptParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                    p => p.Name == cmbScriptName.SelectedItem + "_" + cmbScriptParams.SelectedItem);


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
                    Parameter param = cmbScriptParams.SelectedItem as Parameter;
                    if (param != null && !string.IsNullOrEmpty(param.ParameterValue))
                    {
                        if (param.ParameterValue.Contains("\""))
                            txtBoxParamVal.Text = param.ParameterValue;
                        else
                            txtBoxParamVal.Text = "\"" + ((param.IsSecret) ? SecurePayload.UnSecure(param.ParameterValue, "IAP2GO_SEC!URE") : param.ParameterValue) + "\"";
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
                scriptIdentifier = new Dictionary<string, int>();
                cmbScriptName.ItemsSource = null;

                if (Tree1.Items != null)
                    Tree1.Items.Clear();

                cmbScriptParams.ItemsSource = null;

                txtBoxParamVal.Text = "";
                string fetchCategoryUri = "";
                string serviceBaseUri = "";

                serviceBaseUri = txtBoxSvcBaseUri.Text;

                try
                {
                    CheckUserSecurity(serviceBaseUri + ApplicationConstants.SECURITY_SERVICEINTERFACE);

                    fetchCategoryUri = serviceBaseUri + ApplicationConstants.SCRIPT_REPO_SERVICEINTERFACE;
                    scriptRepoSvc = new ScriptRepository(fetchCategoryUri);
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

                        cmbScriptName.IsEnabled = true;
                        txtCategory.IsEnabled = true;
                    }

                    ModelItem myItem = this.ModelItem;
                    string scriptCatExistsVal = "";
                    if (myItem.Properties["ScriptCategory"].Value != null)
                    {
                        scriptCatExistsVal = myItem.Properties["ScriptCategory"].Value.GetCurrentValue().ToString().Trim();
                    }

                    myItem.Properties["ScriptRepositoryBaseURI"].SetValue(InArgument<string>.FromValue(serviceBaseUri));

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {

                MessageBox.Show("Please specify the Script Metadata Repository service base URI.");
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
                    if (varName.Name.Contains(ApplicationConstants.VARIABLENAMESCRIPT_PREFIX))
                    {

                        if (!shouldRemoveVariables & !userPrompted)
                        {
                            MessageBoxResult result = MessageBox.Show(string.Format(ApplicationConstants.DELETEVARIABLES, MODULE_TYPE), ApplicationConstants.DELETEVARIABLES_SC_TITLE,
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

                FetchScripts(c.CategoryId, c.Name);
            }
        }

        /// <summary>
        /// This method is used to fecth and load scripts based on category passed.
        /// </summary>
        /// <param name="catId">category id</param>
        /// <param name="catName">category name</param>
        private void FetchScripts(int catId, string catName)
        {
            ModelItem myItem = this.ModelItem;
            myItem.Properties["ScriptCategory"].SetValue(categoryId.ToString());
            myItem.Properties["ScriptCategoryName"].SetValue(txtCategory.Text);

            //Fetch scripts associated to the category selected
            Message.GetAllScriptDetailsResMsg scriptList = ((IScriptRepository)scriptRepoSvc.ServiceChannel).GetAllScriptDetails(catId.ToString());
            ScriptNames = null;
            // ScriptNames = new List<string>();

            if (scriptIdentifier.Count > 0)
                scriptIdentifier.Clear();
            if (scriptList.Scripts != null && scriptList.Scripts.Count > 0)
            {
                scriptIdentifier = new Dictionary<string, int>();
                scrParameters = new Dictionary<string, List<Parameter>>();
                Parameter param = null;
                foreach (Data.Script script in scriptList.Scripts)
                {
                    List<Parameter> paramterList = new List<Parameter>();

                    // If argument value has been specified for command
                    if (script.TaskType.ToLower().Equals("command") && !string.IsNullOrEmpty(script.ArgString))
                    {
                        param = new Parameter();
                        string direction = " (" + "In" + ")";
                        param.ParameterName = "Arguments" + direction;
                        param.ParameterValue = script.ArgString;
                        paramterList.Add(param);
                    }
                    // If parameters provided
                    else
                    {
                        foreach (Data.ScriptParam parameter in script.Parameters)
                        {
                            param = new Parameter();
                            string direction = " (" + parameter.ParamType.ToString() + ")";
                            param.ParameterName = parameter.Name + direction;
                            param.ParameterValue = parameter.DefaultValue;
                            param.IsSecret = parameter.IsSecret;
                            param.IsPaired = !parameter.IsUnnamed;
                            paramterList.Add(param);
                        }
                    }
                    scrParameters.Add(script.Name, paramterList);
                    if (!scriptIdentifier.ContainsKey(script.Name))
                        scriptIdentifier.Add(script.Name, script.ScriptId);
                    else
                        MessageBox.Show(String.Format(ApplicationConstants.ITEMDUPLICATE, MODULE_TYPE, script.Name),
                            String.Format(ApplicationConstants.ITEMDUPLICATE_TITLE, MODULE_TYPE),
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                }

                //populate all parameters list as Parameter name (In) or (Out)
                //Re-bind the control source with the updated scriptnames
                var sortedList = ScriptNames.OrderBy(s => s).ToList();
                cmbScriptName.ItemsSource = sortedList;
                cmbScriptName.SelectedIndex = 0;
                ModelItem.Properties["ScriptId"].SetValue(scriptIdentifier[cmbScriptName.SelectedValue.ToString()]);
            }
            else
            {
                ObservableCollection<string> dummyList = new ObservableCollection<string>();
                dummyList.Clear();
                cmbScriptName.ItemsSource = dummyList;
                cmbScriptName.SelectedIndex = -1;
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
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int ParentId { get; set; }
        public int CompanyId { get; set; }
    }

    /////// <summary>
    /////// Class to hold script parameter details
    /////// </summary>
    ////public class Parameter
    ////{
    ////    public string ParameterName { get; set; }
    ////    public string ParameterValue { get; set; }
    ////    public bool IsSecret { get; set; }
    ////    public bool IsPaired { get; set; }
    ////}
}

