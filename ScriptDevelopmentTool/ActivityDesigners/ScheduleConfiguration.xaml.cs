using Infosys.ATR.Entities;
using Infosys.WEM.AutomationActivity.Designers.Services;
using Infosys.WEM.Client;
using Infosys.WEM.Node.Service.Contracts.Data;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Service.Common.Contracts.Message;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WEMClient = Infosys.WEM.Client;
using System.Activities.Presentation;
using System.Windows.Input;
using System.Activities.Presentation.Model;
using Microsoft.VisualBasic.Activities;
using System.ComponentModel;
using System.Windows.Data;
using System.Activities.Expressions;
using Infosys.WEM.SecureHandler;

namespace Infosys.WEM.AutomationActivity.Designers
{
    // Interaction logic for Schedule.xaml
    public partial class ScheduleConfiguration
    {
        Utilies util = new Utilies();
        public ScheduleConfiguration()
        {
            InitializeComponent();
            //hide row containing date time options
            main.RowDefinitions[3].Height = new GridLength(0);
        }

        //workflow and script have different set of IAP nodes, hence workflow and Script will have their own lists of IAP nodes 
        List<string> registeredMachinesWF = new List<string>();
        List<string> selectedMachinesWF = new List<string>();
        List<string> registeredMachinesScript = new List<string>();
        List<string> selectedMachinesScript = new List<string>();

        List<string> registeredClusters = new List<string>();
        List<string> selectedClusters = new List<string>();

        List<SemanticNode> nodes = new List<SemanticNode>();


        private string userSecurity = "";
        private bool isSuperAdmin = false;
        private List<int> currentUserCategories;
        private WorkflowAutomation wfRepoSvc;
        private CommonRepository commonRepoSvc;
        TreeViewItem _root;
        public List<Category> categories { get; set; }
        List<string> catList = null;
        TreeViewItem trvSelectedItem;
        ModuleType moduleType = ModuleType.Workflow;
        int categoryId;
        string categoryName = null;
        SchedulePatterns scheduledPattern = SchedulePatterns.ScheduleNow;
        string clusterName = "";
        RemoteExecMode remoteExecMode = RemoteExecMode.ScheduleOnIAPnode;
        ScheduleStopCriteria scheduleStopCriteria = ScheduleStopCriteria.EndDate;
        DateTime startDate = DateTime.Now;
        DateTime? endDate = DateTime.Now.Add(new TimeSpan(1, 0, 0));
        int priority = 1000;
        int occur = 1;
        string metadataRepositoryURI = "";
        string domainName = "";
        string companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
        bool loading = true;

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public SchedulePatterns SchedulePattern
        {
            get
            {
                return scheduledPattern;
            }
            set
            {
                scheduledPattern = value;
                OnPropertyChanged("SchedulePattern");
            }
        }

        public ModuleType ModuleType
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["ModuleType"].ToString()))
                {
                    moduleType = (ModuleType)(myItem.Properties["ModuleType"].Value.GetCurrentValue());
                    if (moduleType == Designers.ModuleType.Workflow)
                        rdWF.IsChecked = true;
                    else rdScript.IsChecked = true;
                }

                return moduleType;
            }
            set
            {
                if (value == Designers.ModuleType.Script)
                    rdScript.IsChecked = true;
                else if (value == Designers.ModuleType.Workflow)
                    rdWF.IsChecked = true;

                moduleType = value;
                OnPropertyChanged("ModuleType");
            }
        }

        public int CategoryId
        {
            get
            {
                //ModelItem myItem = this.ModelItem;
                //if (!string.IsNullOrEmpty(myItem.Properties["CategoryId"].ToString()))
                //{
                //    categoryId = Convert.ToInt32(myItem.Properties["CategoryId"].Value.GetCurrentValue());
                //    txtCategory.Text = categoryId.ToString();
                //}

                return categoryId;
            }
            set
            {
                categoryId = value;
                OnPropertyChanged("CategoryId");
            }
        }

        public string CategoryName
        {
            get
            {
                //ModelItem myItem = this.ModelItem;
                //if (!string.IsNullOrEmpty(myItem.Properties["CategoryId"].ToString()))
                //{
                //    categoryId = Convert.ToInt32(myItem.Properties["CategoryId"].Value.GetCurrentValue());
                //    txtCategory.Text = categoryId.ToString();
                //}

                return categoryName;
            }
            set
            {
                categoryName = value;
                OnPropertyChanged("CategoryName");
            }
        }

        public DateTime ScheduleStartDateTime
        {
            get
            {
                //ModelItem myItem = this.ModelItem;
                //if (!string.IsNullOrEmpty(myItem.Properties["ScheduleStartDateTime"].ToString()))
                //{
                //    startDate = Convert.ToDateTime(myItem.Properties["ScheduleStartDateTime"].Value.GetCurrentValue());
                //    dpStartDate.Text = startDate.ToString();
                //}

                return startDate;
            }
            set
            {
                startDate = value;
                OnPropertyChanged("ScheduleStartDateTime");
            }
        }

        public ScheduleStopCriteria ScheduleStopCriteria
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["ScheduleStopCriteria"].ToString()))
                {
                    scheduleStopCriteria = (ScheduleStopCriteria)Convert.ToInt32(myItem.Properties["ScheduleStopCriteria"].Value.GetCurrentValue());
                    if (scheduleStopCriteria == Designers.ScheduleStopCriteria.OccurenceCount)
                    {
                        rdNoEndAfter.IsChecked = true;
                    }
                    else if (scheduleStopCriteria == Designers.ScheduleStopCriteria.EndDate)
                    {
                        rdBy.IsChecked = true;
                    }
                    else if (scheduleStopCriteria == Designers.ScheduleStopCriteria.NoEndDate)
                    {
                        rdNoEndDate.IsChecked = true;
                    }
                }

                return scheduleStopCriteria;
            }
            set
            {
                scheduleStopCriteria = value;
                OnPropertyChanged("ScheduleStopCriteria");
            }
        }

        public int ScheduleOcurrences
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["ScheduleOcurrences"].ToString()))
                {
                    occur = Convert.ToInt32(myItem.Properties["ScheduleOcurrences"].Value.GetCurrentValue());
                    // txtOccur.Text = occur.ToString();
                }

                return occur;
            }
            set
            {
                occur = value;
                OnPropertyChanged("ScheduleOcurrences");
            }
        }

        public DateTime? ScheduleEndDate
        {
            get
            {
                //ModelItem myItem = this.ModelItem;
                //if (!string.IsNullOrEmpty(myItem.Properties["ScheduleEndDate"].ToString()))
                //{
                //    endDate = Convert.ToDateTime(myItem.Properties["ScheduleEndDate"].Value.GetCurrentValue());
                //    dpEndDate.Text = endDate.ToString();
                //}

                return endDate;
            }
            set
            {
                endDate = value;
                OnPropertyChanged("ScheduleEndDate");
            }
        }

        public int SchedulePriority
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (myItem != null)
                    if (myItem.Properties["SchedulePriority"].Value != null)
                    {
                        priority = Convert.ToInt32(myItem.Properties["SchedulePriority"].Value.GetCurrentValue());
                        txtPriority.Text = priority.ToString();
                    }

                return priority;
            }
            set
            {
                priority = value;
                OnPropertyChanged("SchedulePriority");
            }
        }

        public string DomainName
        {
            get
            {
                //ModelItem myItem = this.ModelItem;
                //if (!string.IsNullOrEmpty(myItem.Properties["DomainName"].ToString()))
                //{
                //    //domainName = myItem.Properties["DomainName"].Value.GetCurrentValue().ToString();
                //    if (myItem.Properties["DomainName"].Value != null)
                //        domainName = ((Literal<string>)(((InArgument<string>)(myItem.Properties["DomainName"].Value.GetCurrentValue())).Expression)).Value;
                //}

                return domainName;
            }
            set
            {
                domainName = value;
                OnPropertyChanged("DomainName");
            }
        }

        public string MetadataRepositoryURI
        {
            get
            {
                //ModelItem myItem = this.ModelItem;
                //if (!string.IsNullOrEmpty(myItem.Properties["MetadataRepositoryURI"].ToString()))
                //{
                //    metadataRepositoryURI = myItem.Properties["MetadataRepositoryURI"].Value.GetCurrentValue().ToString();
                //}

                return metadataRepositoryURI;
            }
            set
            {
                metadataRepositoryURI = value;
                OnPropertyChanged("MetadataRepositoryURI");
            }
        }

        public string ScheduleClusterName
        {
            get
            {
                ModelItem myItem = this.ModelItem;
                if (!string.IsNullOrEmpty(myItem.Properties["ScheduleClusterName"].ToString()))
                {
                    clusterName = myItem.Properties["ScheduleClusterName"].Value.GetCurrentValue().ToString();
                    //cmbCluster.SelectedItem = clusterName;
                }

                return clusterName;
            }
            set
            {
                clusterName = value;
                OnPropertyChanged("ScheduleClusterName");
            }
        }

        public List<string> RemoteServerNames
        {
            get
            {
                if (rdWF.IsChecked != null)
                    if (Convert.ToBoolean(rdWF.IsChecked))
                    {
                        return selectedMachinesWF;
                    }
                if (rdScript.IsChecked != null)
                    if (Convert.ToBoolean(rdScript.IsChecked))
                    {
                        return selectedMachinesScript;
                    }
                return null;
            }
            set
            {
                if (rdWF.IsChecked != null)
                    if (Convert.ToBoolean(rdWF.IsChecked))
                    {
                        selectedMachinesWF = value;
                    }
                if (rdScript.IsChecked != null)
                    if (Convert.ToBoolean(rdScript.IsChecked))
                    {
                        selectedMachinesScript = value;
                    }
                OnPropertyChanged("RemoteServerNames");
            }
        }

        public List<string> RegisteredServerNames
        {
            get
            {
                if (rdWF.IsChecked != null)
                    if (Convert.ToBoolean(rdWF.IsChecked))
                    {
                        return registeredMachinesWF;
                    }
                    else if (rdScript.IsChecked != null)
                        if (Convert.ToBoolean(rdScript.IsChecked))
                        {
                            return registeredMachinesScript;
                        }
                return new List<string>();
            }
            set
            {
                if (rdWF.IsChecked != null)
                    if (Convert.ToBoolean(rdWF.IsChecked))
                    {
                        registeredMachinesWF = value;
                    }
                    else if (rdScript.IsChecked != null)
                        if (Convert.ToBoolean(rdScript.IsChecked))
                        {
                            registeredMachinesScript = value;
                        }

                OnPropertyChanged("RegisteredServerNames");
            }
        }

        public List<string> RegisteredClusters
        {
            get
            {
                //ModelItem myItem = this.ModelItem;
                //if (myItem.Properties["RegisteredClusters"].Value != null)
                //{
                //    if (rdWF.IsChecked != null)
                //        if (Convert.ToBoolean(rdWF.IsChecked))
                //        {
                //            registeredClustersWF = (List<string>)(myItem.Properties["RegisteredClusters"].Value.GetCurrentValue());
                //            lbxRegClusters.SelectedIndex = 0;
                //            return registeredClustersScript;
                //        }
                //        else if (rdScript.IsChecked != null)
                //            if (Convert.ToBoolean(rdScript.IsChecked))
                //            {
                //                registeredClustersScript = (List<string>)(myItem.Properties["RegisteredClusters"].Value.GetCurrentValue());
                //                lbxRegClusters.SelectedIndex = 0;
                //                return registeredClustersScript;
                //            }
                //}
                return registeredClusters;
            }
            set
            {
                registeredClusters = value;
                OnPropertyChanged("RegisteredClusters");
            }
        }

        public List<string> SelectedClusters
        {
            get
            {
                return selectedClusters;
            }
            set
            {
                selectedClusters = value;
                OnPropertyChanged("SelectedClusters");
            }
        }

        /// <summary>
        /// Fetches all IAP nodes for a domain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFetch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDomain.Text))
            {
                MessageBox.Show("Please enter Domain name");
            }
            else
            {
                ModelItem myItem = this.ModelItem;

                //Set previous property values to null

                myItem.Properties["RemoteServerNames"].SetValue(null);

                selectedMachinesScript = new List<string>();
                selectedMachinesWF = new List<string>();

                domainName = txtDomain.Text;
                myItem.Properties["DomainName"].SetValue(InArgument<string>.FromValue(domainName));
                //txtDomain lost its value after above statement, hence reassigning
                txtDomain.Text = domainName;
                selectedMachinesScript = null;
                selectedMachinesWF = null;
                if (rdScript.IsChecked != null)
                {
                    if (Convert.ToBoolean(rdScript.IsChecked))
                    {
                        FetchRegisteredMachinesForDomain(ModuleType.Script, false);
                    }
                }
                if (rdWF.IsChecked != null)
                {
                    if (Convert.ToBoolean(rdWF.IsChecked))
                    {
                        FetchRegisteredMachinesForDomain(ModuleType.Workflow, false);
                    }
                }
            }
        }

        /// <summary>
        /// Fetches all registered IAP nodes for a domain
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="populate">This specifies selected IAP nodes should be displayed or should not be displayed</param>
        private void FetchRegisteredMachinesForDomain(ModuleType moduleType, bool populate)
        {
            if (moduleType == Designers.ModuleType.Workflow)
            {
                //workflow and script have different set of IAP nodes
                registeredMachinesWF = new List<string>();
            }
            else if (moduleType == Designers.ModuleType.Script)
            {
                registeredMachinesScript = new List<string>();
            }

            //Control lbxRegNodes has been rendered
            if (lbxRegNodes != null)
            {
                lbxRegNodes.ItemsSource = null;
                lbxRegNodes.Items.Refresh();
                lbxRemoteNodes.ItemsSource = null;
                lbxRemoteNodes.Items.Refresh();

                if (!string.IsNullOrEmpty(txtDomain.Text))
                {
                    List<RegisteredNode> nodesP = GetAllNodesOnDomain(txtDomain.Text, "", moduleType); //blank service url so that the service end point is taken from the config file

                    if (nodesP != null && nodesP.Count > 0)
                    {
                        if (moduleType == Designers.ModuleType.Workflow)
                        //if (rdWF.IsChecked != null)
                        //    if (Convert.ToBoolean(rdWF.IsChecked))
                        {
                            foreach (RegisteredNode n in nodesP)
                                registeredMachinesWF.Add(n.HostMachineName);

                            //populate specifies selected IAP nodes should be displayed or should not be displayed
                            if (populate)
                            {
                                if (selectedMachinesWF != null)
                                {
                                    if (selectedMachinesWF.Count() > 0)
                                    {
                                        foreach (string sel in selectedMachinesWF)
                                        {
                                            //Remove selected IAP nodes from the available lists
                                            registeredMachinesWF.Remove(sel);
                                        }
                                    }
                                    lbxRemoteNodes.ItemsSource = selectedMachinesWF;
                                    lbxRemoteNodes.Items.Refresh();
                                    lbxRemoteNodes.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                //do not filter IAP nodes selected by user
                                selectedMachinesWF = new List<string>();
                                lbxRemoteNodes.ItemsSource = null;
                                lbxRemoteNodes.Items.Refresh();
                                lbxRemoteNodes.SelectedIndex = 0;
                            }

                            lbxRegNodes.ItemsSource = registeredMachinesWF;
                        }
                        if (moduleType == Designers.ModuleType.Script)
                        //if (rdScript.IsChecked != null)
                        //    if (Convert.ToBoolean(rdScript.IsChecked))
                        {
                            foreach (RegisteredNode n in nodesP)
                                registeredMachinesScript.Add(n.HostMachineName);

                            //populate specifies selected IAP nodes should be displayed or should not be displayed
                            if (populate)
                            {
                                if (selectedMachinesScript != null)
                                {
                                    if (selectedMachinesScript.Count() > 0)
                                    {
                                        foreach (string sel in selectedMachinesScript)
                                        {
                                            //Remove selected IAP nodes from the available lists
                                            registeredMachinesScript.Remove(sel);
                                        }
                                    }
                                    lbxRemoteNodes.ItemsSource = selectedMachinesScript;
                                    lbxRemoteNodes.Items.Refresh();
                                    lbxRemoteNodes.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                //do not filter IAP nodes selected by user
                                selectedMachinesScript = new List<string>();
                                lbxRemoteNodes.ItemsSource = null;
                                lbxRemoteNodes.Items.Refresh();
                                lbxRemoteNodes.SelectedIndex = 0;
                            }

                            lbxRegNodes.ItemsSource = registeredMachinesScript;
                        }

                        lbxRegNodes.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Service call to fetch all registered IAP nodes for a domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="registeredNodesSrvUrl"></param>
        /// <param name="nodeType">This is Workflow or Script</param>
        /// <returns></returns>
        private List<RegisteredNode> GetAllNodesOnDomain(string domainName, string registeredNodesSrvUrl, ModuleType nodeType)
        {
            List<RegisteredNode> nodes = null;
            RegisteredNodes nodesClient = new RegisteredNodes(registeredNodesSrvUrl);
            string strcompany = System.Configuration.ConfigurationManager.AppSettings["Company"];
            if (string.IsNullOrEmpty(strcompany))
                strcompany = "0";
            GetRegisteredNodesResMsg result = nodesClient.ServiceChannel.GetRegisteredNodes(domainName, (Convert.ToInt32(nodeType)).ToString(), strcompany);
            if (result != null)
                nodes = result.Nodes;

            return nodes;
        }

        /// <summary>
        /// Add the selected IAP node to the selected list, remove it from available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMachinesScript == null)
                selectedMachinesScript = new List<string>();
            if (selectedMachinesWF == null)
                selectedMachinesWF = new List<string>();
            if (rdWF.IsChecked != null)
                if (Convert.ToBoolean(rdWF.IsChecked))
                {
                    int c = registeredMachinesWF.Count();
                    if (lbxRegNodes.SelectedItem != null)
                    {
                        //remove IAP node from available list
                        registeredMachinesWF.Remove(lbxRegNodes.SelectedItem.ToString());
                        if (c > 0)
                        {
                            //Add the selected IAP node to the selected list
                            selectedMachinesWF.Add(lbxRegNodes.SelectedItem.ToString());
                        }
                    }

                    //Refresh list boxes
                    lbxRemoteNodes.ItemsSource = selectedMachinesWF;
                    lbxRemoteNodes.Items.Refresh();
                    lbxRemoteNodes.SelectedIndex = 0;
                    lbxRegNodes.ItemsSource = registeredMachinesWF;
                    lbxRegNodes.Items.Refresh();
                    lbxRegNodes.SelectedIndex = 0;
                    ModelItem myItem = this.ModelItem;
                    // myItem.Properties["RemoteServerNames"].SetValue(InArgument<List<string>>.FromValue(selectedMachinesWF));

                    myItem.Properties["RemoteServerNames"].SetValue(selectedMachinesWF);
                    OnPropertyChanged("RemoteServerNames");
                    myItem.Properties["RegisteredServerNames"].SetValue(registeredMachinesWF);
                    OnPropertyChanged("RegisteredServerNames");

                }
            if (rdScript.IsChecked != null)
                if (Convert.ToBoolean(rdScript.IsChecked))
                {
                    int c = registeredMachinesScript.Count();
                    if (lbxRegNodes.SelectedItem != null)
                    {
                        //remove IAP node from available list
                        registeredMachinesScript.Remove(lbxRegNodes.SelectedItem.ToString());
                        if (c > 0)
                        {
                            //Add the selected IAP node to the selected list
                            selectedMachinesScript.Add(lbxRegNodes.SelectedItem.ToString());
                        }
                    }

                    //Refresh list boxes
                    lbxRemoteNodes.ItemsSource = selectedMachinesScript;
                    lbxRemoteNodes.Items.Refresh();
                    lbxRemoteNodes.SelectedIndex = 0;
                    lbxRegNodes.ItemsSource = registeredMachinesScript;
                    lbxRegNodes.Items.Refresh();
                    lbxRegNodes.SelectedIndex = 0;
                    ModelItem myItem = this.ModelItem;
                    // myItem.Properties["RemoteExecutionMode"].SetValue(InArgument<List<string>>.FromValue(selectedMachinesScript));
                    myItem.Properties["RemoteServerNames"].SetValue(selectedMachinesScript);
                    OnPropertyChanged("RemoteServerNames");
                    myItem.Properties["RegisteredServerNames"].SetValue(registeredMachinesScript);
                    OnPropertyChanged("RegisteredServerNames");
                }
        }

        private List<string> GetSelectedClusterIds(List<string> names)
        {
            List<string> values = new List<string>();
            foreach (string name in names)
            {
                values.Add(nodes.Find(x => x.Name == name).Id);
            }
            return values;
        }

        /// <summary>
        /// Remove the selected IAP node from the selected list, add it to the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (rdWF.IsChecked != null)
                if (Convert.ToBoolean(rdWF.IsChecked))
                {
                    int c = selectedMachinesWF.Count();
                    if (lbxRemoteNodes.SelectedItem != null)
                    {
                        //remove IAP node from available list
                        selectedMachinesWF.Remove(lbxRemoteNodes.SelectedItem.ToString());
                        if (c > 0)
                        {
                            //Add the selected IAP node to the selected list
                            registeredMachinesWF.Add(lbxRemoteNodes.SelectedItem.ToString());
                        }
                    }

                    //Refresh list boxes
                    lbxRemoteNodes.ItemsSource = selectedMachinesWF;
                    lbxRemoteNodes.Items.Refresh();
                    lbxRemoteNodes.SelectedIndex = 0;
                    lbxRegNodes.ItemsSource = registeredMachinesWF;
                    lbxRegNodes.Items.Refresh();
                    lbxRegNodes.SelectedIndex = 0;
                    ModelItem myItem = this.ModelItem;
                    //myItem.Properties["RemoteExecutionMode"].SetValue(InArgument<List<string>>.FromValue(selectedMachinesWF));
                    //OnPropertyChanged("RemoteExecutionMode");
                    myItem.Properties["RemoteServerNames"].SetValue(selectedMachinesWF);
                    OnPropertyChanged("RemoteServerNames");
                    myItem.Properties["RegisteredServerNames"].SetValue(registeredMachinesWF);
                    OnPropertyChanged("RegisteredServerNames");
                }
            if (rdScript.IsChecked != null)
                if (Convert.ToBoolean(rdScript.IsChecked))
                {
                    int c = selectedMachinesScript.Count();
                    if (lbxRemoteNodes.SelectedItem != null)
                    {
                        //remove IAP node from available list
                        selectedMachinesScript.Remove(lbxRemoteNodes.SelectedItem.ToString());
                        if (c > 0)
                        {
                            //Add the selected IAP node to the selected list
                            registeredMachinesScript.Add(lbxRemoteNodes.SelectedItem.ToString());
                        }
                    }

                    //Refresh list boxes
                    lbxRemoteNodes.ItemsSource = selectedMachinesScript;
                    lbxRemoteNodes.Items.Refresh();
                    lbxRemoteNodes.SelectedIndex = 0;
                    lbxRegNodes.ItemsSource = registeredMachinesScript;
                    lbxRegNodes.Items.Refresh();
                    lbxRegNodes.SelectedIndex = 0;
                    ModelItem myItem = this.ModelItem;
                    //myItem.Properties["RemoteExecutionMode"].SetValue(InArgument<List<string>>.FromValue(selectedMachinesWF));
                    //OnPropertyChanged("RemoteExecutionMode");
                    myItem.Properties["RemoteServerNames"].SetValue(selectedMachinesScript);
                    OnPropertyChanged("RemoteServerNames");
                    myItem.Properties["RegisteredServerNames"].SetValue(registeredMachinesScript);
                    OnPropertyChanged("RegisteredServerNames");
                }
        }

        /// <summary>
        /// Updates model item with SchedulePattern as ScheduleNow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdNow_Checked(object sender, RoutedEventArgs e)
        {
            //hide row containing date time UI controls
            main.RowDefinitions[3].Height = new GridLength(0);

            int p = SchedulePriority;
            ModelItem myItem = this.ModelItem;
            if (myItem != null)
            {
                scheduledPattern = SchedulePatterns.ScheduleNow;
                myItem.Properties["SchedulePattern"].SetValue(scheduledPattern);
                myItem.Properties["ScheduleStartDateTime"].SetValue(null);
                myItem.Properties["ScheduleEndDate"].SetValue(null);

            }
        }

        /// <summary>
        /// Updates model item with SchedulePattern as Recurrences
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdDateTime_Checked(object sender, RoutedEventArgs e)
        {
            //display row containing date time UI controls
            main.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Auto);
            ModelItem myItem = this.ModelItem;
            if (myItem != null)
            {
                scheduledPattern = SchedulePatterns.ScheduleWithRecurrence;
                myItem.Properties["SchedulePattern"].SetValue(scheduledPattern);
            }
        }

        /// <summary>
        /// Updates model item with scheduleStopCriteria as NoEndDate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdNoEndDate_Checked(object sender, RoutedEventArgs e)
        {
            scheduleStopCriteria = Designers.ScheduleStopCriteria.NoEndDate;
            ModelItem myItem = this.ModelItem;
            if (myItem != null)
            {
                myItem.Properties["ScheduleStopCriteria"].SetValue(scheduleStopCriteria);
                myItem.Properties["ScheduleEndDate"].SetValue(null);
            }
        }

        /// <summary>
        /// Updates model item with scheduleStopCriteria as EndAfter occurences
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdNoEndAfter_Checked(object sender, RoutedEventArgs e)
        {
            scheduleStopCriteria = ScheduleStopCriteria.OccurenceCount;
            ModelItem myItem = this.ModelItem;
            if (myItem != null)
            {
                myItem.Properties["ScheduleStopCriteria"].SetValue(scheduleStopCriteria);
                //myItem.Properties["ScheduleStartDateTime"].SetValue(null);
                myItem.Properties["ScheduleEndDate"].SetValue(null);
            }
        }

        /// <summary>
        /// Updates model item with scheduleStopCriteria as EndDate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdBy_Checked(object sender, RoutedEventArgs e)
        {
            scheduleStopCriteria = ScheduleStopCriteria.EndDate;
            ModelItem myItem = this.ModelItem;
            if (myItem != null)
                myItem.Properties["ScheduleStopCriteria"].SetValue(scheduleStopCriteria);
        }

        /// <summary>
        /// Updates model item with ModuleType as Script
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdScript_Checked(object sender, RoutedEventArgs e)
        {
            ModelItem myItem = this.ModelItem;
            moduleType = Designers.ModuleType.Script;
            if (myItem != null)
            {
                myItem.Properties["ModuleType"].SetValue(moduleType);
            }
            //Fetch IAP nodes for script
            if (remoteExecMode == RemoteExecMode.ScheduleOnIAPnode)
                FetchRegisteredMachinesForDomain(ModuleType.Script, true);
        }

        /// <summary>
        /// Updates model item with ModuleType as Workflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdWF_Checked(object sender, RoutedEventArgs e)
        {
            moduleType = Designers.ModuleType.Workflow;
            ModelItem myItem = this.ModelItem;
            if (myItem != null)
            {
                myItem.Properties["ModuleType"].SetValue(moduleType);
            }
            //Fetch IAP nodes for script
            if (remoteExecMode == RemoteExecMode.ScheduleOnIAPnode)
                FetchRegisteredMachinesForDomain(ModuleType.Workflow, true);
            txtDomain.Text = domainName;
        }

        /// <summary>
        /// Updates model item with RemoteExecMode as RunOnIAPNode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdNode_Checked(object sender, RoutedEventArgs e)
        {
            remoteExecMode = RemoteExecMode.ScheduleOnIAPnode;
            main.RowDefinitions[7].Height = new GridLength(0);
            main.RowDefinitions[6].Height = new GridLength(1, GridUnitType.Auto);
            ModelItem myItem = this.ModelItem;
            //if (myItem != null)
            //    remoteExecMode = (RemoteExecMode)(((Literal<int>)(((InArgument<int>)(myItem.Properties["RemoteExecutionMode"].Value.GetCurrentValue())).Expression)).Value);

            if (myItem != null)
            {
                int y = (int)remoteExecMode;
                myItem.Properties["RemoteExecutionMode"].SetValue(y);
            }

            if (txtCategory != null)
            {
                //Reset property values
                //MetadataRepositoryURI = null;
                //CategoryName = null;
                //SelectedClusters = null;
                //myItem.Properties["MetadataRepositoryURI"].SetValue(InArgument<string>.FromValue(null));
                //myItem.Properties["CategoryName"].SetValue(InArgument<string>.FromValue(null));
                //myItem.Properties["SelectedClusters"].SetValue(null);
                myItem.Properties["MetadataRepositoryURI"].SetValue(null);
                myItem.Properties["CategoryName"].SetValue(null);
                myItem.Properties["SelectedClusters"].SetValue(null);
                myItem.Properties["RegisteredClusters"].SetValue(null);
                registeredClusters = new List<string>();
                //txtCategory.Text = "";
                //txtBoxSvcBaseUri.Text = "";
                lbxSelClusters.ItemsSource = null;
                lbxSelClusters.Items.Refresh();
                lbxRegClusters.ItemsSource = null;
                lbxRegClusters.Items.Refresh();
            }
            if (txtDomain != null)
            {
                //Reset control values
                if (myItem.Properties["DomainName"].Value != null)
                    txtDomain.Text = ((Literal<string>)(((InArgument<string>)(myItem.Properties["DomainName"].Value.GetCurrentValue())).Expression)).Value;

                if (rdWF.IsChecked != null)
                    if (Convert.ToBoolean(rdWF.IsChecked))
                    {
                        lbxRemoteNodes.ItemsSource = selectedMachinesWF;
                        lbxRegNodes.ItemsSource = registeredMachinesWF;
                    }
                if (rdScript.IsChecked != null)
                    if (Convert.ToBoolean(rdScript.IsChecked))
                    {
                        lbxRemoteNodes.ItemsSource = selectedMachinesScript;
                        lbxRegNodes.ItemsSource = registeredMachinesScript;
                    }
                lbxRemoteNodes.Items.Refresh();
                lbxRemoteNodes.SelectedIndex = 0;
                lbxRegNodes.Items.Refresh();
                lbxRegNodes.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Updates model item with RemoteExecMode as RunOnClusters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdClus_Checked(object sender, RoutedEventArgs e)
        {
            remoteExecMode = RemoteExecMode.ScheduleOnCluster;
            main.RowDefinitions[6].Height = new GridLength(0);
            main.RowDefinitions[7].Height = new GridLength(1, GridUnitType.Auto);
            ModelItem myItem = this.ModelItem;
            if (myItem != null)
            {
                int y = (int)remoteExecMode;
                myItem.Properties["RemoteExecutionMode"].SetValue(y);
            }

            if (txtDomain != null)
            {
                //Reset property values
                //DomainName = null;
                //RemoteServerNames = null;
                //myItem.Properties["DomainName"].SetValue(InArgument<string>.FromValue(null));
                //myItem.Properties["RemoteServerNames"].SetValue(null);
                myItem.Properties["DomainName"].SetValue(null);
                myItem.Properties["RemoteServerNames"].SetValue(null);
                myItem.Properties["RegisteredServerNames"].SetValue(null);
                registeredMachinesScript = new List<string>();
                registeredMachinesWF = new List<string>();
                selectedMachinesWF = new List<string>();
                selectedMachinesScript = new List<string>();

                //txtDomain.Text = "";
                lbxRegNodes.ItemsSource = null;
                lbxRegNodes.Items.Refresh();
                lbxRegNodes.SelectedIndex = -1;
                lbxRemoteNodes.ItemsSource = null;
                lbxRemoteNodes.Items.Refresh();
                lbxRemoteNodes.SelectedIndex = -1;
            }
            if (txtCategory != null)
            {
                if (myItem.Properties["CategoryName"].Value != null)
                    txtCategory.Text = ((Literal<string>)(((InArgument<string>)(myItem.Properties["CategoryName"].Value.GetCurrentValue())).Expression)).Value;

                if (myItem.Properties["MetadataRepositoryURI"].Value != null)
                    txtBoxSvcBaseUri.Text = ((Literal<string>)(((InArgument<string>)(myItem.Properties["MetadataRepositoryURI"].Value.GetCurrentValue())).Expression)).Value;
                //txtBoxSvcBaseUri.Text = myItem.Properties["MetadataRepositoryURI"].Value.GetCurrentValue().ToString();
                if (categoryId != 0)
                {
                    if (myItem.Properties["SelectedClusters"].Value != null)
                    {

                        GetClusters();
                        List<string> selectedClusterIds = (List<string>)(myItem.Properties["SelectedClusters"].Value.GetCurrentValue());
                        //selectedClusters = (List<string>)(myItem.Properties["SelectedClusters"].Value.GetCurrentValue());
                        //  selectedClusters = ((Literal<List<string>>)(((InArgument<List<string>>)(myItem.Properties["SelectedClusters"].Value.GetCurrentValue())).Expression)).Value;
                        selectedClusters = GetClusterNamesFromIds(selectedClusterIds);
                        lbxSelClusters.ItemsSource = selectedClusters;
                        lbxSelClusters.Items.Refresh();
                        lbxSelClusters.SelectedItem = -1;

                        foreach (string s in selectedClusters)
                        {
                            string r = registeredClusters.Find(x => x == s);
                            if (!string.IsNullOrEmpty(r))
                                registeredClusters.Remove(r);
                        }
                        lbxRegClusters.ItemsSource = registeredClusters;
                        lbxRegClusters.Items.Refresh();
                        lbxRegClusters.SelectedItem = -1;
                    }
                }
            }
        }

        private List<string> GetClusterNamesFromIds(List<string> selectedClusters)
        {
            if (selectedClusters != null)
            {
                List<string> names = new List<string>();
                if (nodes != null)
                {
                    if (nodes.Count > 0)
                    {
                        foreach (string node in selectedClusters)
                        {
                            SemanticNode result = nodes.Find(x => x.Id.ToLower() == node.ToLower());
                            if (result != null)
                            {
                                names.Add(result.Name);
                            }
                        }
                    }
                }
                return names;
            }
            else return null;
        }

        /// <summary>
        /// Entry point for code execution when a saved XAML workflow is opened from disk/repository
        /// Set all properties to value from the ModelItem
        /// As properties are two way binded with UI controls, this will initizlize values in UI control
        /// Priority always has a value. Either user defined or 1000 as default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPriority_TextChanged(object sender, TextChangedEventArgs e)
        {
            //loading as True signifies that this workflow is opened first time
            if (loading == true)
            {
                ModelItem myItem = this.ModelItem;
                //myItem is null when this activity is dropped from tollbox onto a designer
                //If workflow is opened from disk/repository this has a value
                if (myItem != null)
                {
                    //Set all properties to value from the model
                    if (Convert.ToInt32(myItem.Properties["SchedulePriority"].Value.GetCurrentValue()) == 0)
                    {
                        txtPriority.Text = "1000";
                        myItem.Properties["SchedulePriority"].SetValue(Convert.ToInt32(txtPriority.Text));
                    }
                    else
                    {
                        txtPriority.Text = myItem.Properties["SchedulePriority"].Value.GetCurrentValue().ToString();
                    }
                    if (myItem.Properties["SchedulePattern"].Value != null)
                    {
                        if (Convert.ToInt32(myItem.Properties["SchedulePattern"].Value.GetCurrentValue()) != 0)
                        {
                            scheduledPattern = (SchedulePatterns)myItem.Properties["SchedulePattern"].Value.GetCurrentValue();
                            if (scheduledPattern == SchedulePatterns.ScheduleNow)
                            {
                                rdNow.IsChecked = true;
                                main.RowDefinitions[3].Height = new GridLength(0);
                            }
                            else if (scheduledPattern == SchedulePatterns.ScheduleWithRecurrence)
                            {
                                //set the contrls n end date
                                rdDateTime.IsChecked = true;
                                main.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Auto);
                                scheduleStopCriteria = (ScheduleStopCriteria)myItem.Properties["ScheduleStopCriteria"].Value.GetCurrentValue();
                                if (scheduleStopCriteria == Designers.ScheduleStopCriteria.OccurenceCount)
                                {
                                    rdNoEndAfter.IsChecked = true;
                                }
                                else if (scheduleStopCriteria == Designers.ScheduleStopCriteria.EndDate)
                                {
                                    rdBy.IsChecked = true;
                                    if (myItem.Properties["ScheduleEndDate"].Value != null)
                                    {
                                        endDate = Convert.ToDateTime(myItem.Properties["ScheduleEndDate"].Value.GetCurrentValue());
                                        dpEndDate.SelectedDate = endDate;
                                    }
                                }
                                else if (scheduleStopCriteria == Designers.ScheduleStopCriteria.NoEndDate)
                                {
                                    rdNoEndDate.IsChecked = true;
                                }

                                //set the start date
                                if (myItem.Properties["ScheduleStartDateTime"].Value != null)
                                {
                                    startDate = Convert.ToDateTime(myItem.Properties["ScheduleStartDateTime"].Value.GetCurrentValue());
                                    dpStartDate.Text = startDate.ToString();

                                    //format and Initialize Hours and minutes for Scheudled Start date
                                    if (startDate.Hour == 0)
                                        txtHours.Text = "00";
                                    else if (startDate.Hour > 0 && startDate.Hour < 10)
                                        txtHours.Text = "0" + startDate.Hour.ToString();
                                    else
                                        txtHours.Text = startDate.Hour.ToString();

                                    if (startDate.Minute == 0)
                                        txtMinutes.Text = "00";
                                    else if (startDate.Minute > 0 && startDate.Minute < 10)
                                        txtMinutes.Text = "0" + startDate.Minute.ToString();
                                    else
                                        txtMinutes.Text = startDate.Minute.ToString();

                                    if (startDate.Date.Subtract(DateTime.Now.Date) < new TimeSpan(0, 0, 0, 0))
                                    {
                                        MessageBox.Show("Start date should not be a past date", "Error", MessageBoxButton.OK);
                                        dpStartDate.SelectedDate = DateTime.Now;
                                        myItem.Properties["ScheduleStartDateTime"].SetValue(DateTime.Now);
                                        OnPropertyChanged("ScheduleStartDateTime");
                                    }
                                }
                                else if (endDate != null && endDate != DateTime.MinValue)
                                {
                                    if ((Convert.ToDateTime(endDate)).Subtract(startDate).Days < 0)
                                    {
                                        MessageBox.Show("Start date should not be greater than the end date", "Error", MessageBoxButton.OK);
                                        dpEndDate.SelectedDate = null;
                                    }
                                }
                            }
                        }
                        else SchedulePattern = SchedulePatterns.ScheduleNow;
                    }
                    else SchedulePattern = SchedulePatterns.ScheduleNow;

                    if (myItem.Properties["RemoteExecutionMode"].Value != null)
                        remoteExecMode = (RemoteExecMode)myItem.Properties["RemoteExecutionMode"].Value.GetCurrentValue();

                    moduleType = (ModuleType)Convert.ToInt32(myItem.Properties["ModuleType"].Value.GetCurrentValue());
                    if (remoteExecMode == RemoteExecMode.ScheduleOnIAPnode)
                    {
                        //Populates IAP node specific properties
                        rdNode.IsChecked = true;
                        if (myItem.Properties["DomainName"].Value != null)
                        {
                            domainName = ((Literal<string>)(((InArgument<string>)(myItem.Properties["DomainName"].Value.GetCurrentValue())).Expression)).Value;
                            //domainName = myItem.Properties["DomainName"].Value.GetCurrentValue().ToString();
                            txtDomain.Text = domainName;

                            FetchRegisteredMachinesForDomain(moduleType, true);
                            if (myItem.Properties["RemoteServerNames"].Value != null)
                            {

                                //   List<string> mc = ((Literal<List<string>>)(((InArgument<List<string>>)(myItem.Properties["RemoteServerNames"].Value.GetCurrentValue())).Expression)).Value;
                                List<string> mc = (List<string>)myItem.Properties["RemoteServerNames"].Value.GetCurrentValue();
                                if (moduleType == Designers.ModuleType.Workflow)
                                {
                                    foreach (string m in mc)
                                    {
                                        selectedMachinesWF.Add(m);
                                        registeredMachinesWF.Remove(m);
                                    }
                                    myItem.Properties["RegisteredServerNames"].SetValue(registeredMachinesWF);
                                    lbxRegNodes.ItemsSource = registeredMachinesWF;
                                    lbxRemoteNodes.ItemsSource = selectedMachinesWF;
                                }
                                else if (moduleType == Designers.ModuleType.Script)
                                {
                                    foreach (string m in mc)
                                    {
                                        selectedMachinesScript.Add(m);
                                        registeredMachinesScript.Remove(m);
                                    }
                                    myItem.Properties["RegisteredServerNames"].SetValue(registeredMachinesScript);
                                    lbxRegNodes.ItemsSource = registeredMachinesScript;
                                    lbxRemoteNodes.ItemsSource = selectedMachinesScript;
                                }
                                lbxRegNodes.Items.Refresh();
                                lbxRegNodes.SelectedIndex = 0;
                                lbxRemoteNodes.Items.Refresh();
                                lbxRemoteNodes.SelectedIndex = 0;


                            }
                        }
                    }
                    else if (remoteExecMode == RemoteExecMode.ScheduleOnCluster)
                    {
                        //Populate cluster specific properties
                        rdClus.IsChecked = true;
                        if (myItem.Properties["MetadataRepositoryURI"].Value != null)
                            metadataRepositoryURI = ((Literal<string>)(((InArgument<string>)(myItem.Properties["MetadataRepositoryURI"].Value.GetCurrentValue())).Expression)).Value;
                        categoryId = Convert.ToInt32(myItem.Properties["CategoryId"].Value.GetCurrentValue());
                        bool iscategoryValid = IsCategoryValid(categoryId, metadataRepositoryURI);
                        if (iscategoryValid)
                        {
                            if (myItem.Properties["SelectedClusters"].Value != null)
                            {
                                List<string> clusterIds = (List<string>)(myItem.Properties["SelectedClusters"].Value.GetCurrentValue());
                                bool isClusterNameValid = IsClusterValid(categoryId, clusterIds);
                                GetClusters();
                                if (clusterIds.Count > 0)
                                {
                                    selectedClusters = GetClusterNamesFromIds(clusterIds);
                                }
                                if (isClusterNameValid)
                                {
                                    //selectedClusters = clusterIds;
                                    lbxSelClusters.ItemsSource = selectedClusters;
                                    lbxSelClusters.Items.Refresh();
                                    lbxSelClusters.SelectedItem = -1;
                                    //GetClusters();
                                    foreach (string s in selectedClusters)
                                    {
                                        string r = registeredClusters.Find(x => x == s);
                                        if (!string.IsNullOrEmpty(r))
                                            registeredClusters.Remove(r);
                                    }
                                    lbxRegClusters.ItemsSource = registeredClusters;
                                    lbxRegClusters.Items.Refresh();
                                    lbxRegClusters.SelectedItem = -1;
                                }
                                else
                                {
                                    MessageBox.Show("Category cluster association is invalid. Please select again.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Category is invalid. Please select again.");
                        }
                    }

                    moduleType = (ModuleType)Convert.ToInt32(myItem.Properties["ModuleType"].Value.GetCurrentValue());
                    if (moduleType == ModuleType.Workflow)
                        rdWF.IsChecked = true;
                    else if (moduleType == ModuleType.Script)
                        rdScript.IsChecked = true;

                }
                loading = false;
            }
            else
            {
                txtHours.Text = DateTime.Now.Hour.ToString();
                txtMinutes.Text = DateTime.Now.Minute.ToString();
            }
        }

        /// <summary>
        /// Validates whether category is associated with the logged in user
        /// </summary>
        /// <param name="catId"></param>
        /// <param name="serverName"></param>
        /// <returns></returns>
        private Boolean IsCategoryValid(int catId, string serverName)
        {
            Boolean isValid = false;
            var name = util.GetAlias();
            Infosys.WEM.SecurityAccess.Contracts.Message.GetAllUsersResMsg response = GetUsers(name, companyId);
            List<Infosys.WEM.SecurityAccess.Contracts.Data.User> users = response.Users.FindAll(x => x.CategoryId == categoryId);
            if (users == null)
                isValid = true;
            foreach (Infosys.WEM.SecurityAccess.Contracts.Data.User u in users)
            {
                if (u.CategoryId == catId)
                    isValid = true;
            }
            return isValid;
        }

        /// <summary>
        /// Validates whether cluster is associated with the category id selected by the user
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="clusterName"></param>
        /// <returns></returns>
        private Boolean IsClusterValid(int categoryId, List<string> clusters)
        {
            Boolean isValid = false;
            var response = GetAllClustersByCategory(categoryId.ToString());
            List<string> clusterNames = new List<string>();
            if (response.Nodes != null && response.Nodes.Count > 0)
            {
                List<SemanticNode> names = response.Nodes;
                foreach (SemanticNode n in names)
                {
                    List<string> s = clusters.FindAll(x => x.ToLower() == n.Id.ToLower());
                    if (s != null)
                        if (s.Count == 1)
                        {
                            isValid = true;
                        }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Fetches from all clusters associated with category id selected by user
        /// </summary>
        private void GetClusters()
        {
            var response = GetAllClustersByCategory(categoryId.ToString());
            nodes = response.Nodes;
            registeredClusters = new List<string>();
            if (response.Nodes != null && response.Nodes.Count > 0)
            {
                List<SemanticNode> names = response.Nodes.ToList();
                foreach (SemanticNode n in names)
                {
                    registeredClusters.Add(n.Name);
                }
            }
        }

        /// <summary>
        /// invokes a service call to fetch all clusetrs associated with category id selected by user
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private static Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg GetAllClustersByCategory(string categoryId)
        {
            Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg response = null;
            WEMClient.CommonRepository commonRepoClient = new WEMClient.CommonRepository();
            response = commonRepoClient.ServiceChannel.GetAllClustersByCategory(categoryId.ToString());
            return response;
        }

        /// <summary>
        /// Fetches category and displays them in a tree view control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFetchCategory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxSvcBaseUri.Text))
            {
                MessageBox.Show("Please enter server name");
            }
            else
            {
                ModelItem myItem = this.ModelItem;
                MetadataRepositoryURI = txtBoxSvcBaseUri.Text;

                //Set previous property values to null
                myItem.Properties["CategoryName"].SetValue(null);
                myItem.Properties["SelectedClusters"].SetValue(null);
                categoryId = 0;
                categoryName = null;
                selectedClusters = new List<string>();

                myItem.Properties["MetadataRepositoryURI"].SetValue(InArgument<string>.FromValue(MetadataRepositoryURI));
                //txtDomain lost its value after above statement, hence reassigning
                txtBoxSvcBaseUri.Text = MetadataRepositoryURI;
                string ModuleID = "3";
                if (!string.IsNullOrEmpty(txtBoxSvcBaseUri.Text))
                {
                    txtCategory.IsEnabled = true;
                    lbxRegClusters.ItemsSource = null;
                    lbxRegClusters.Items.Refresh();
                    lbxSelClusters.ItemsSource = null;
                    lbxSelClusters.Items.Refresh();

                    if (Tree1.Items != null)
                        Tree1.Items.Clear();

                    string fetchCategoryUri = "";
                    string serviceBaseUri = "";


                    serviceBaseUri = txtBoxSvcBaseUri.Text;

                    try
                    {
                        //CheckUserSecurity(serviceBaseUri + "/WEMSecurityAccessService.svc");
                        CheckUserSecurity(serviceBaseUri + ApplicationConstants.SECURITY_SERVICEINTERFACE);

                        //fetchCategoryUri = serviceBaseUri + "/WEMService.svc";
                          fetchCategoryUri = serviceBaseUri + ApplicationConstants.WORKFLOW_SERVICEINTERFACE;
                        wfRepoSvc = new WorkflowAutomation(fetchCategoryUri);


                        if (categories != null)
                        {
                            categories.Clear();
                        }

                        commonRepoSvc = new CommonRepository(serviceBaseUri + ApplicationConstants.COMMON_SERVICEINTERFACE);
                        //commonRepoSvc = new CommonRepository(serviceBaseUri + "/WEMCommonService.svc");

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

                            txtCategory.IsEnabled = true;
                        }

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
        }

        private List<Category> GetCategories(string ModuleID, string serviceBaseUri)
        {
            string fetchCategoryUri = "";


            serviceBaseUri = txtBoxSvcBaseUri.Text;

            CheckUserSecurity(serviceBaseUri + ApplicationConstants.SECURITY_SERVICEINTERFACE);

            fetchCategoryUri = serviceBaseUri + ApplicationConstants.WORKFLOW_SERVICEINTERFACE;
            wfRepoSvc = new WorkflowAutomation(fetchCategoryUri);


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
            return categories;
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
            var alias = util.GetAlias();
            isSuperAdmin = WFService.IsSuperAdmin(alias, ConfigurationManager.AppSettings["Company"], securityServiceUrl).IsSuperAdmin;
            //isSuperAdmin = true;
        }


        /// <summary>
        /// This method checks if current user is active and sets up properties accordingly.
        /// </summary>
        /// <returns></returns>
        private bool IsActiveUser(string securityServiceUrl)
        {
            var name = util.GetAlias();
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
                bool isCategoryAccessible = CheckUserRoleOnCategory(categoryId);
                if (isCategoryAccessible)
                {
                    ModelItem myItem = this.ModelItem;
                    myItem.Properties["CategoryId"].SetValue(categoryId);
                    myItem.Properties["CategoryName"].SetValue(InArgument<string>.FromValue(c.Name));
                    GetClusters();
                    lbxRegClusters.ItemsSource = registeredClusters;
                    lbxRegClusters.Items.Refresh();
                    lbxRegClusters.SelectedIndex = 0;
                    lbxSelClusters.ItemsSource = null;
                    lbxSelClusters.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("You do not have access to this category");
                }
            }
        }

        /// <summary>
        /// Validates whether user has a role other than Guest on the category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private bool CheckUserRoleOnCategory(int categoryId)
        {
            bool isCategoryAccessible = true;
            var name = util.GetAlias();
            Infosys.WEM.SecurityAccess.Contracts.Message.GetAllUsersResMsg response = GetUsers(name, companyId);
            List<Infosys.WEM.SecurityAccess.Contracts.Data.User> users = response.Users.FindAll(x => x.CategoryId == categoryId);
            if (users == null)
                isCategoryAccessible = true;
            return isCategoryAccessible;
        }

        /// <summary>
        /// Fetch category, roles for the logged in user
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        internal static Infosys.WEM.SecurityAccess.Contracts.Message.GetAllUsersResMsg GetUsers(string alias, string companyId)
        {
            Infosys.WEM.SecurityAccess.Contracts.Message.GetAllUsersResMsg responseObj = null;
            Infosys.WEM.Client.SecurityAccess access = new Infosys.WEM.Client.SecurityAccess();
            responseObj = access.ServiceChannel.GetUsers(alias, companyId);
            return responseObj;
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

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;
            if (date != null)
            {
                DateTime selectedDate = Convert.ToDateTime(date);


                if (string.IsNullOrEmpty(txtHours.Text))
                {
                    ModelItem myItem = this.ModelItem;
                    if (myItem.Properties["ScheduleStartDateTime"].Value != null)
                    {
                        DateTime t = Convert.ToDateTime(myItem.Properties["ScheduleStartDateTime"].Value.GetCurrentValue());
                        txtHours.Text = t.Hour.ToString();
                        if (string.IsNullOrEmpty(txtMinutes.Text))
                        {
                            txtMinutes.Text = t.Minute.ToString();
                            startDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, t.Hour, t.Minute, DateTime.Now.Second);
                        }
                        else
                        {
                            startDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, t.Hour, Convert.ToInt32(txtMinutes.Text), DateTime.Now.Second);
                        }
                    }
                    else
                    {
                        txtHours.Text = DateTime.Now.Hour.ToString();
                        txtMinutes.Text = DateTime.Now.Minute.ToString();
                        startDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    }
                }
                else if (string.IsNullOrEmpty(txtMinutes.Text))
                {
                    ModelItem myItem = this.ModelItem;
                    if (myItem.Properties["ScheduleStartDateTime"].Value != null)
                    {
                        DateTime t = Convert.ToDateTime(myItem.Properties["ScheduleStartDateTime"].Value.GetCurrentValue());
                        txtHours.Text = t.Hour.ToString();
                        txtMinutes.Text = t.Minute.ToString();
                        startDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, Convert.ToInt32(txtHours.Text), t.Minute, DateTime.Now.Second);
                    }
                    else
                    {
                        txtHours.Text = DateTime.Now.Hour.ToString();
                        txtMinutes.Text = DateTime.Now.Minute.ToString();
                        startDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    }
                }
                else
                {
                    startDate = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, Convert.ToInt32(txtHours.Text), Convert.ToInt32(txtMinutes.Text), DateTime.Now.Second);
                }

                if (startDate.Date.Subtract(DateTime.Now.Date) < new TimeSpan(0, 0, 0, 0))
                {
                    if (loading == false)
                    {
                        MessageBox.Show("Start date should not be a past date", "Error", MessageBoxButton.OK);
                        dpStartDate.SelectedDate = DateTime.Now;
                        //if (endDate != null)
                        //{
                        //    if ((Convert.ToDateTime(endDate)).Subtract(startDate).Days < 0)
                        //    {
                        //        MessageBox.Show("Start date should not be greater than the end date", "Error", MessageBoxButton.OK);
                        //    }
                        //}
                    }
                }
                else if (endDate != null && endDate != DateTime.MinValue)
                {
                    if (scheduleStopCriteria == Designers.ScheduleStopCriteria.EndDate)
                    {
                        if ((Convert.ToDateTime(endDate)).Subtract(startDate).Days < 0)
                        {
                            MessageBox.Show("Start date should not be greater than the end date", "Error", MessageBoxButton.OK);
                            dpEndDate.SelectedDate = null;
                        }
                    }
                }
                //else
                //{
                //    ModelItem myItem = this.ModelItem;
                //    myItem.Properties["ScheduleStartDateTime"].SetValue(startDate);
                //}
                ModelItem myItem1 = this.ModelItem;
                myItem1.Properties["ScheduleStartDateTime"].SetValue(startDate);
                OnPropertyChanged("ScheduleStartDateTime");
            }
        }

        private void dpEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            DateTime? date = picker.SelectedDate;
            endDate = Convert.ToDateTime(date);
            if (endDate != null && endDate != DateTime.MinValue)
            {
                if ((Convert.ToDateTime(endDate)).Subtract(startDate).Days < 0)
                {
                    MessageBox.Show("Start date should not be greater than the end date", "Error", MessageBoxButton.OK);
                    dpEndDate.SelectedDate = null;
                    ModelItem myItem = this.ModelItem;
                    myItem.Properties["ScheduleEndDate"].SetValue(null);
                }
                else
                {
                    ModelItem myItem = this.ModelItem;
                    myItem.Properties["ScheduleEndDate"].SetValue(endDate);
                }
            }
        }

        /// <summary>
        /// Validates value of minute
        /// update minutes in ScheduledStartDate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHours_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtHours.Text))
            {
                int hours;
                bool result = int.TryParse(txtHours.Text, out hours);

                if (result == false)
                {
                    MessageBox.Show("Invalid Hours");
                    txtHours.Text = "00";
                }
                else if (result == true)
                {
                    if (hours < 0 || hours > 23)
                    {
                        MessageBox.Show("Invalid Hours");
                        txtHours.Text = "00";
                    }
                    else
                    {
                        //update hours in ScheduledStartDate
                        if (string.IsNullOrEmpty(txtMinutes.Text))
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, Convert.ToInt32(txtHours.Text), 0, 0);
                        else if (!string.IsNullOrEmpty(txtHours.Text))
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, Convert.ToInt32(txtHours.Text), Convert.ToInt32(txtMinutes.Text), 0);
                        else
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, Convert.ToInt32(txtMinutes.Text), 0);

                        ModelItem myItem = this.ModelItem;
                        myItem.Properties["ScheduleStartDateTime"].SetValue(startDate);
                    }
                }
            }
        }

        /// <summary>
        /// Validates value of minute
        /// update minutes in ScheduledStartDate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMinutes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMinutes.Text))
            {
                int minutes;
                bool result = int.TryParse(txtMinutes.Text, out minutes);

                if (result == false)
                {
                    MessageBox.Show("Invalid Minutes");
                    txtMinutes.Text = "00";
                }
                else if (result == true)
                {
                    if (minutes < 0 || minutes > 59)
                    {
                        MessageBox.Show("Invalid Minutes");
                        txtMinutes.Text = "00";
                    }
                    else
                    {
                        //update minutes in ScheduledStartDate
                        if (string.IsNullOrEmpty(txtHours.Text))
                        {
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, Convert.ToInt32(txtMinutes.Text), 0);
                        }
                        else if (!string.IsNullOrEmpty(txtMinutes.Text))
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, Convert.ToInt32(txtHours.Text), Convert.ToInt32(txtMinutes.Text), 0);
                        else
                            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, Convert.ToInt32(txtHours.Text), 0, 0);

                        ModelItem myItem = this.ModelItem;
                        myItem.Properties["ScheduleStartDateTime"].SetValue(startDate);
                    }
                }
            }
        }

        /// <summary>
        /// Add selected cluster to the selected list, remove it from the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddCluster_Click(object sender, RoutedEventArgs e)
        {
            int c = registeredClusters.Count();
            if (lbxRegClusters.SelectedItem != null)
            {
                // Add selected cluster to the selected list, remove it from the available list
                registeredClusters.Remove(lbxRegClusters.SelectedItem.ToString());
                if (c > 0)
                {
                    selectedClusters.Add(lbxRegClusters.SelectedItem.ToString());
                }
            }

            //reinitialize list boxes data
            lbxSelClusters.ItemsSource = selectedClusters;
            lbxSelClusters.Items.Refresh();
            lbxSelClusters.SelectedIndex = 0;
            lbxRegClusters.ItemsSource = registeredClusters;
            lbxRegClusters.Items.Refresh();
            lbxRegClusters.SelectedIndex = 0;
            ModelItem myItem = this.ModelItem;
            // myItem.Properties["RemoteExecutionMode"].SetValue(InArgument<List<string>>.FromValue(selectedClusters));
            myItem.Properties["SelectedClusters"].SetValue(GetSelectedClusterIds(selectedClusters));
        }

        /// <summary>
        /// Remove selected cluster from the selected list, add it to the available list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveCluster_Click(object sender, RoutedEventArgs e)
        {
            int c = selectedClusters.Count();
            if (lbxSelClusters.SelectedItem != null)
            {
                // Remove selected cluster from the selected list, add it to the available list
                selectedClusters.Remove(lbxSelClusters.SelectedItem.ToString());
                if (c > 0)
                {
                    registeredClusters.Add(lbxSelClusters.SelectedItem.ToString());
                }
            }
            //reinitialize list boxes data
            lbxSelClusters.ItemsSource = selectedClusters;
            lbxSelClusters.Items.Refresh();
            lbxSelClusters.SelectedIndex = 0;
            lbxRegClusters.ItemsSource = registeredClusters;
            lbxRegClusters.Items.Refresh();
            lbxRegClusters.SelectedIndex = 0;
            ModelItem myItem = this.ModelItem;
            //  myItem.Properties["RemoteExecutionMode"].SetValue(InArgument<List<string>>.FromValue(selectedClusters));
            myItem.Properties["SelectedClusters"].SetValue(GetSelectedClusterIds(selectedClusters));
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            rdWF.IsChecked = true;
            rdNow.IsChecked = true;
            txtOccur.Text = "";
            rdNoEndAfter.IsChecked = true;
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            txtPriority.Text = "1000";
            rdNode.IsChecked = true;
            txtBoxSvcBaseUri.Text = "";
            txtCategory.Text = "";
            txtDomain.Text = "";
            lbxRegClusters.ItemsSource = null;
            lbxRegClusters.SelectedIndex = -1;
            lbxRegNodes.ItemsSource = null;
            lbxRegNodes.SelectedIndex = -1;
            lbxRemoteNodes.ItemsSource = null;
            lbxRemoteNodes.SelectedIndex = -1;
            lbxSelClusters.ItemsSource = null;
            lbxSelClusters.SelectedIndex = -1;

            selectedClusters = new List<string>();
            registeredClusters = new List<string>();
            selectedMachinesScript = new List<string>();
            registeredMachinesScript = new List<string>();
            selectedMachinesWF = new List<string>();
            registeredMachinesWF = new List<string>();

            remoteExecMode = RemoteExecMode.ScheduleOnIAPnode;
            moduleType = Designers.ModuleType.Workflow;
            startDate = DateTime.Now;
            endDate = null;
            scheduleStopCriteria = Designers.ScheduleStopCriteria.EndDate;
            occur = 0;
            priority = 1000;
            categoryId = 0;
            categoryName = null;
            metadataRepositoryURI = null;
            domainName = null;


            ModelItem myItem = this.ModelItem;
            myItem.Properties["ModuleType"].SetValue(moduleType);
            OnPropertyChanged("ModuleType");
            myItem.Properties["ScheduleStartDateTime"].SetValue(startDate);
            OnPropertyChanged("ScheduleStartDateTime");
            myItem.Properties["ScheduleStopCriteria"].SetValue(scheduleStopCriteria);
            OnPropertyChanged("ScheduleStopCriteria");
            myItem.Properties["ScheduleOcurrences"].SetValue(occur);
            OnPropertyChanged("ScheduleOcurrences");
            myItem.Properties["ScheduleEndDate"].SetValue(endDate);
            OnPropertyChanged("ScheduleEndDate");
            myItem.Properties["SchedulePriority"].SetValue(priority);
            OnPropertyChanged("SchedulePriority");
            myItem.Properties["RegisteredClusters"].SetValue(null);
            OnPropertyChanged("RegisteredClusters");
            myItem.Properties["RegisteredServerNames"].SetValue(null);
            OnPropertyChanged("RegisteredServerNames");
            myItem.Properties["RemoteExecutionMode"].SetValue((int)remoteExecMode);    
            OnPropertyChanged("RemoteExecutionMode");
            myItem.Properties["CategoryId"].SetValue(categoryId);
            OnPropertyChanged("CategoryId");
            myItem.Properties["MetadataRepositoryURI"].SetValue(InArgument<string>.FromValue(metadataRepositoryURI));
            OnPropertyChanged("MetadataRepositoryURI");
            myItem.Properties["CategoryName"].SetValue(InArgument<string>.FromValue(categoryName));
            OnPropertyChanged("CategoryName");
            myItem.Properties["SelectedClusters"].SetValue(null);
            OnPropertyChanged("SelectedClusters");
            myItem.Properties["DomainName"].SetValue(InArgument<string>.FromValue(domainName));
            OnPropertyChanged("DomainName");
            myItem.Properties["RemoteServerNames"].SetValue(null);
            OnPropertyChanged("RemoteServerNames");
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            bool success = false;
            ModelItem myItem = this.ModelItem;
            if (remoteExecMode == RemoteExecMode.ScheduleOnIAPnode)
            {
                if (myItem.Properties["RemoteServerNames"].Value == null)
                {
                    success = false;
                    MessageBox.Show("Please provide all mandatory fields");
                }
                else
                {
                    List<string> mc = (List<string>)myItem.Properties["RemoteServerNames"].Value.GetCurrentValue();
                    if (mc.Count > 0)
                        success = true;
                    else MessageBox.Show("Please provide all mandatory fields.");

                }
            }
            else if (remoteExecMode == RemoteExecMode.ScheduleOnCluster)
            {
                if (myItem.Properties["SelectedClusters"].Value == null)
                {
                    success = false;
                    MessageBox.Show("Please provide all mandatory fields");
                }
                else
                {
                    List<string> mc = (List<string>)myItem.Properties["SelectedClusters"].Value.GetCurrentValue();
                    if (mc.Count > 0)
                        success = true;
                    else MessageBox.Show("Please provide all mandatory fields");
                }
            }
            //if (rdby != null)
            {
                if (Convert.ToBoolean(rdBy.IsChecked))
                {
                    if (endDate != null && endDate != DateTime.MinValue)
                        if ((Convert.ToDateTime(endDate)).Subtract(startDate).Days < 0)
                        {
                            MessageBox.Show("Start date should not be greater than the end date", "Error", MessageBoxButton.OK);
                            dpEndDate.SelectedDate = null;
                            success = false;
                        }
                }
            }
            if (success)
            {
                MessageBox.Show("Verified: Success");
            }
        }
    }

    public class ScheduleConfigurationSpec
    {
        public ModuleType ModuleType { get; set; }
        public int RemoteExecutionMode { get; set; }
        public SchedulePatterns SchedulePattern { get; set; }
        public ScheduleStopCriteria ScheduleStopCriteria { get; set; }
        public DateTime ScheduleStartDateTime { get; set; }
        public int ScheduleOcurrences { get; set; }
        public DateTime? ScheduleEndDate { get; set; }
        public int SchedulePriority { get; set; }
        public int CategoryId { get; set; }
        public List<string> ScheduleClusterName { get; set; }
        public List<string> RemoteServerNames { get; set; }
    }

    public enum RemoteExecMode
    {
        ScheduleOnCluster = 1,
        ScheduleOnIAPnode = 2
    }

    public enum ScheduleStopCriteria
    {
        Now = 1,
        NoEndDate,
        OccurenceCount,
        EndDate
    }

    public enum SchedulePatterns
    {
        ScheduleNow = 1,
        ScheduleWithRecurrence = 2
    }

    public enum ModuleType
    {
        Workflow = 1,
        Script = 2
    }
}
