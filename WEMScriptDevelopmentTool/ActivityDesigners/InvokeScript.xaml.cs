using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using Microsoft.VisualBasic.Activities;
using Infosys.WEM.Client;
using Infosys.WEM.Scripts.Service.Contracts;
using Message = Infosys.WEM.Scripts.Service.Contracts.Message;
using Data = Infosys.WEM.Scripts.Service.Contracts.Data;


namespace Infosys.WEM.AutomationActivity.Designers
{
    // Interaction logic for InvokeScript.xaml
    public partial class InvokeScript
    {
        public const string NODATA = "No Data";

        public InvokeScript()
        {
            //noData.Add(NODATA);

           // scriptCategory.Add(NODATA);

            InitializeComponent(); 



        }

        //private List<string> noData = new List<string>();
        private List<string> scriptCategory = new List<string>();
        private List<string> scriptSubCategory = new List<string>();
        //private List<string> scriptName = new List<string>();
        Dictionary<int, string> scriptIdentifier = new Dictionary<int, string>();
        //private Dictionary<string, List<string>> dicScriptNames = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> scrParameters = new Dictionary<string, List<string>>();
        private string param = "";
        private Dictionary<string, Dictionary<int, string>> scriptSubCat = new Dictionary<string, Dictionary<int, string>>();
        private ScriptRepository scriptRepoSvc;
        private CommonRepository commonRepoSvc;


        public List<string> ScriptCategory
        {
            get
            {
                 ModelItem myItem = this.ModelItem;
                 if (myItem.Properties["ScriptCategory"].Value != null && scriptCategory.Count==0 )
                     scriptCategory.Add(myItem.Properties["ScriptCategory"].Value.GetCurrentValue().ToString());

                return scriptCategory;
            }
            set
            {
                scriptCategory = value;
                OnPropertyChanged("ScriptCategory");


            }

        }


        public List<string> ScriptSubCategory
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if ((cmbScriptCat.SelectedItem != null) && (cmbScriptCat.SelectedItem.ToString() != NODATA) && (scriptSubCat.Count > 0))
                    return scriptSubCat[(string)cmbScriptCat.SelectedItem].Values.ToList();
                else if (myItem.Properties["ScriptSubCategory"].Value != null && scriptSubCat.Count == 0) // Logic to handle when user loads an already designed workflow
                    return new List<string>(){myItem.Properties["ScriptSubCategory"].Value.GetCurrentValue().ToString()};
                else
                    return null;
            }
            set
            {
                scriptSubCategory = value;
                OnPropertyChanged("ScriptSubCategory");


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
                if ((cmbScriptSubCat.SelectedItem != null) && (cmbScriptSubCat.SelectedItem.ToString() != NODATA) && (scriptIdentifier.Count>0))
                    return scriptIdentifier.Values.ToList();
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
                    ModelItem.Properties["ScriptId"].SetValue(scriptIdentifier.ElementAt(cmbScriptName.SelectedIndex).Key);
                }
                OnPropertyChanged("ScriptNames");
            }
        }

        public List<string> Parameters
        {
            get
            {
                //todo 3: Fetch Script params for script selected and load scrParameters
                //if (cmbScriptName == null || cmbScriptName.SelectedItem == null)
                //{
                //    btnSave.IsEnabled = false;
                //    //return null;
                //}
                //else if (!scrParameters.ContainsKey((string)cmbScriptName.SelectedItem))
                //{
                //    btnSave.IsEnabled = false;
                //    //return null;
                //}
                //else
                //{
                //    List<string> scrParams = new List<string>();
                //    scrParams = scrParameters[(string)cmbScriptName.SelectedItem];
                //    if (scrParams.Count == 0)
                //    {
                //        btnSave.IsEnabled = false;
                //    }
                //    else
                //    {
                //        btnSave.IsEnabled = true;
                //    }
                //    return scrParams;
                //}
                ModelItem myItem = this.ModelItem;
                if ((cmbScriptName.SelectedItem != null) && (cmbScriptName.SelectedItem.ToString() != NODATA) && (scrParameters.Count > 0))
                {
                    if (scrParameters.ContainsKey((string)cmbScriptName.SelectedItem))
                    {
                        btnSave.IsEnabled = true;
                        ModelItem.Properties["Parameters"].SetValue(scrParameters[(string)cmbScriptName.SelectedItem]);
                        return scrParameters[(string)cmbScriptName.SelectedItem].ToList<string>();
                    }
                    else
                    {
                        btnSave.IsEnabled = false;
                        return null;
                    }

                }
                else if (myItem.Properties["Parameters"].Value != null && scrParameters.Count == 0) // Logic to handle when user loads an already designed workflow
                {
                    btnSave.IsEnabled = true;
                    return (List<string>) myItem.Properties["Parameters"].Value.GetCurrentValue();
                }
                else
                {
                    btnSave.IsEnabled = false;
                    return null;
                }
            }
            set
            {
                scrParameters[param] = value;
                OnPropertyChanged("Parameters");

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


        private void cmbScriptCat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScriptSubCategory != null)
            {
                cmbScriptSubCat.ItemsSource = ScriptSubCategory;
                cmbScriptSubCat.SelectedIndex = 0;
            }
            //comboBox2.SelectedIndex = 0;
        }

        private void cmbScriptSubCat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbScriptSubCat.SelectedItem == null || cmbScriptSubCat.SelectedItem.ToString() == NODATA)
            {
                return;
            }
            ModelItem myItem = this.ModelItem;
            if (scriptSubCat.Count > 0)
            {
                Dictionary<int, string> subCatid = scriptSubCat[(string)cmbScriptCat.SelectedItem];
                int id = subCatid.ElementAt(cmbScriptSubCat.SelectedIndex).Key;
                myItem.Properties["ScriptSubCategoryId"].SetValue(id);

                //Fetch scripts associated to the sub-category selected
                Message.GetAllScriptDetailsResMsg scriptList = ((IScriptRepository)scriptRepoSvc.ServiceChannel).GetAllScriptDetails(id.ToString());
                ScriptNames = null;

                scriptIdentifier = new Dictionary<int, string>();
                scrParameters = new Dictionary<string, List<string>>();

                foreach (Data.Script script in scriptList.Scripts)
                {
                    List<string> paramterName = new List<string>();
                    foreach (Data.ScriptParam parameter in script.Parameters)
                    {
                        string direction = " (" + parameter.ParamType.ToString() + ")";

                        paramterName.Add(parameter.Name + direction);
                    }
                    scrParameters.Add(script.Name, paramterName);
                    scriptIdentifier.Add(script.ScriptId, script.Name);

                }

                //populate all parameters list as Parameter name (In) or (Out)
                //Re-bind the control source with the updated scriptnames
                cmbScriptName.ItemsSource = ScriptNames;
                cmbScriptName.SelectedIndex = 0;
                ModelItem.Properties["ScriptId"].SetValue(scriptIdentifier.ElementAt(cmbScriptName.SelectedIndex).Key);
            }
            else
            {
                //Logic to handle the scenario where the user loads an existing workflow design, the designer displays the values previously selected 
                // and available by the activity library
                string scriptNameExistsVal = "";
                if (myItem.Properties["ScriptName"].Value != null)
                {
                    scriptNameExistsVal = myItem.Properties["ScriptName"].Value.GetCurrentValue().ToString().Trim();
                }
                cmbScriptName.ItemsSource = ScriptNames;

                if (scriptNameExistsVal != "")
                    cmbScriptName.SelectedItem = scriptNameExistsVal;
                else
                    cmbScriptName.SelectedIndex = 0;
            }


        }

        private void cmbScriptName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //POPULATE scrParameters

            cmbScriptParams.ItemsSource = Parameters;
            cmbScriptParams.SelectedIndex = 0;
            ModelItem myItem = this.ModelItem;
            if(scriptIdentifier.Count>0)
                ModelItem.Properties["ScriptId"].SetValue(scriptIdentifier.ElementAt(cmbScriptName.SelectedIndex).Key);
            if ((Parameters == null) || (Parameters.Count == 0))
            {
                btnSave.IsEnabled = false;
            }

        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if ((cmbScriptName == null) || (cmbScriptName.SelectedItem == null) || (cmbScriptName.SelectedItem.ToString() == NODATA))
            {
                MessageBox.Show("Script Name is required.");
                return;
            }

            string paramsFormatted = cmbScriptParams.SelectedItem.ToString().
                Replace(" (" + Data.ParamDirection.In + ")", "").
                Replace(" (" + Data.ParamDirection.Out + ")", "").
                Replace(" (" + Data.ParamDirection.InAndOut + ")", "");
            string scripNameFormatted = cmbScriptName.SelectedItem.ToString().Replace(" ", "");
            string variableName = string.Format(ApplicationConstants.VARIABLENAME_FORMAT, scripNameFormatted, paramsFormatted);
            Variable<string> mySimpleVar = new Variable<string>
            {
                Name = variableName
            };

            ModelItem myItem = this.ModelItem;
            do
            {
                myItem = myItem.Parent;
            }
            while (myItem.Parent.ItemType != typeof(Sequence));
            ModelProperty mp = myItem.Parent.Properties["Variables"];


            IEnumerable<System.Activities.Variable> mis = ((IEnumerable<System.Activities.Variable>)mp.ComputedValue).Where(
            p => p.Name == variableName);
            if (mis.Count() > 0)
            {
                //using (ModelEditingScope scope = myItem.BeginEdit())
                //{

                System.Activities.Variable varExists = mis.First();

                ((Microsoft.VisualBasic.Activities.VisualBasicValue<string>)varExists.Default).ExpressionText = txtBoxParamVal.Text;
                //    scope.Complete();
                //}

            }
            else
            {
                mySimpleVar.Default = new VisualBasicValue<string>
                {
                    //ExpressionText = (string)textBox1.Expression.GetCurrentValue()
                    ExpressionText = (string)txtBoxParamVal.Text
                };
                myItem.Parent.Properties["Variables"].Collection.Add(mySimpleVar);
            }

        }

        private void cmbScriptParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelItem myItem = this.ModelItem;
            do
            {
                myItem = myItem.Parent;
            }
            while (myItem.Parent.ItemType != typeof(Sequence));

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
                txtBoxParamVal.Text = "";
            }

        }

        private void btnFetch_Click(object sender, RoutedEventArgs e)
        {


            if (!string.IsNullOrEmpty(txtBoxSvcBaseUri.Text))
            {
                //Refresh all previous states
                scriptIdentifier = new Dictionary<int, string>();
                cmbScriptCat.ItemsSource = null;
                cmbScriptSubCat.ItemsSource = null;
                cmbScriptName.ItemsSource = null;
                cmbScriptParams.ItemsSource = null;                
                txtBoxParamVal.Text = "";
                string fetchCategoryUri="";
                string serviceBaseUri = "";
                if (!txtBoxSvcBaseUri.Text.ToLower().Contains("http"))
                    serviceBaseUri = "http://" + txtBoxSvcBaseUri.Text;
                else
                    serviceBaseUri = txtBoxSvcBaseUri.Text;

                fetchCategoryUri = serviceBaseUri + ApplicationConstants.SCRIPT_REPO_SERVICEINTERFACE;
                try
                {
                    scriptRepoSvc = new ScriptRepository(fetchCategoryUri);
                    commonRepoSvc = new CommonRepository(serviceBaseUri + ApplicationConstants.COMMON_SERVICEINTERFACE);

                    Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoryTreeResMsg categoryMsg = ((Infosys.WEM.Service.Common.Contracts.ICommonRepository)commonRepoSvc.ServiceChannel).GetAllCategory();
                   // Message.GetAllCategoryAndSubcategoryResMsg categoryMsg = ((IScriptRepository)scriptRepoSvc.ServiceChannel).GetAllCategoryAndSubcategory();

                    //Populate script category
                    scriptCategory = categoryMsg.Categories.Select(s => s.Name).ToList();
                    //Fill scriptCategory
                    //Logic to handle the scenario where the user loads an existing workflow design, the designer displays the values previously selected 
                    // and available by the activity library
                    ModelItem myItem = this.ModelItem;
                    string scriptCatExistsVal = "";
                    if (myItem.Properties["ScriptCategory"].Value != null)
                    {
                       scriptCatExistsVal = myItem.Properties["ScriptCategory"].Value.GetCurrentValue().ToString().Trim();
                    }
                    cmbScriptCat.ItemsSource = ScriptCategory;

                    if (scriptCatExistsVal != "")
                        cmbScriptCat.SelectedItem = scriptCatExistsVal;
                    else
                        cmbScriptCat.SelectedIndex = 0;

                    
                    //scriptSubCategory = null;
                    scriptSubCat = new Dictionary<string, Dictionary<int, string>>();
                    //Fill scriptSubCategory
                    foreach (Infosys.WEM.Service.Common.Contracts.Data.CategoryTree category in categoryMsg.Categories)
                    {
                        Dictionary<int, string> subCategory = new Dictionary<int, string>();
                        foreach (Infosys.WEM.Service.Common.Contracts.Data.Category subCategoryItem in category.SubCategories)
                        {
                            subCategory.Add(subCategoryItem.CategoryId, subCategoryItem.Name);

                        }
                        scriptSubCat.Add(category.Name, subCategory);
                    }
                    //Logic to handle the scenario where the user loads an existing workflow design, the designer displays the values previously selected 
                    // and available by the activity library
                    string scriptSubCatExistsVal = "";
                    if (myItem.Properties["ScriptSubCategory"].Value != null)
                    {
                        scriptSubCatExistsVal = myItem.Properties["ScriptSubCategory"].Value.GetCurrentValue().ToString().Trim();
                    }
                    cmbScriptSubCat.ItemsSource = ScriptSubCategory;

                    if (scriptCatExistsVal != "")
                        cmbScriptSubCat.SelectedItem = scriptSubCatExistsVal;
                    else
                        cmbScriptSubCat.SelectedIndex = 0;
                    //Update property in the activity library

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
    }
}
