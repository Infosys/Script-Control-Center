using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Toolbox;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Presentation.Factories;
//using AutomationActivitiesLibrary;
using System.Activities.XamlIntegration;
//using Microsoft.Samples.DynamicArguments;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Input;
using System.Net;

using Infosys.ATR.WFDesigner.Entities;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.WEM.Resource.Entity.Document;
using Infosys.WEM.Resource.DataAccess.Document;

using System.Configuration;
using System.Reflection;
using Infosys.ATR.CommonViews;
using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.VisualBasic.Activities;
using System.Xaml;
using Infosys.WEM.Client;
using System.Xml.Linq;
using System.Activities.Presentation.Debug;
using System.Activities.Debugger;
using System.Windows.Threading;

namespace Infosys.ATR.WFDesigner.Views
{
    public partial class WFDesigner : UserControl, IWFDesigner, IClose
    {
        private IDictionary<ToolboxCategory, IList<string>> loadedToolboxActivities;
        private IDictionary<string, ToolboxCategory> toolboxCategoryMap;
        private System.Activities.Presentation.Toolbox.ToolboxControl txc;
        private WorkflowDesigner wd;
        public bool isDirty, promptToSave;
        public WorkflowPE data = null;

        private delegate void TimerDelegate();
        string storageBaseUrl;
        int companyId;
        List<string> namespaces = null;
        Assembly assembly = null;
        Mode _mode;
        internal string Title;

        public IDesignerDebugView DebuggerService { get; set; }
        public string WorkFlowPath { get; set; }

        public WFDesigner()
        {
            InitializeComponent();
        }

        [EventPublication(Constants.EventTopicNames.ShowOutputView, PublicationScope.Global)]
        public event EventHandler<EventArgs<List<ExecutionResultView>>> ShowOutputView;

        [EventPublication(Constants.EventTopicNames.AppendOutputViewWF, PublicationScope.Global)]
        public event EventHandler<Infosys.ATR.WFDesigner.Views.ExecuteWf.AppendOutputViewArgsWF> AppendOutputViewWF;

        private void Form1_Load(object sender, EventArgs e)
        {
            _mode = (Mode)Enum.Parse(typeof(Mode), this._presenter.WorkItem.RootWorkItem.State["Mode"].ToString());

            (new DesignerMetadata()).Register();

            this.txc = new ToolboxControl();
            InitialiseToolbox();
            elementHost3.Child = txc;
            companyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);

            if (_mode == Mode.Online)
                //storageBaseUrl = GetStorageBaseUrl();
                storageBaseUrl = CommonServices.Instance.StorageBaseURL;
        }

        //private string GetStorageBaseUrl()
        //{
        //    return this._presenter.GetCompany(companyId).Company.StorageBaseUrl;
        //}

        /// <summary>
        /// This method load WEM.AutomationActivity.Libraries assembly
        /// </summary>
        private void LoadAutomationActivityAssembly()
        {
            string automationActivityAssemblyName = System.Configuration.ConfigurationManager.AppSettings["AutomationActivityAssemblyName"];
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                if (dll.Contains(automationActivityAssemblyName))
                {
                    assembly = Assembly.LoadFile(dll);
                    break;
                }
        }
        /// <summary>
        /// This method loads System.Activities assembly that contains NativeActvity class
        /// </summary>
        /// <returns>NativeActivity Type</returns>
        private Type GetNativeActivityType()
        {
            string nativeActivityAssemblyPath = System.Configuration.ConfigurationManager.AppSettings["NativeActivityAssemblyPath"];
            Assembly nativeAssembly = Assembly.LoadFrom(nativeActivityAssemblyPath);
            Type type = nativeAssembly.GetTypes().Where(t => t.Name.Equals(Constants.Application.NativeActivity)).SingleOrDefault();
            return type;
        }

        /// <summary>
        /// This method gets all the classes that inherit from NativeActivity
        /// </summary>
        /// <param name="assembly">Assembly containing NativeActivity class</param>
        /// <param name="baseType">NativeActivity Type</param>
        /// <returns>Lisy of Types</returns>
        private List<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t)).ToList();
        }

        /// <summary>
        /// This method is used to get all the namespaces containing classes that derive from NativeActivity class
        /// </summary>
        private void GetAllNamespaces()
        {
            namespaces = new List<string>();
            LoadAutomationActivityAssembly();
            Type nativeActivityType = GetNativeActivityType();
            List<Type> listOfTypes = FindDerivedTypes(assembly, nativeActivityType);
            string topItem = "";

            foreach (Type type in listOfTypes)
            {
                if (type.Namespace.Equals(Constants.Application.AutomationActivityAssemblyName))
                    topItem = type.Namespace;
                else
                    namespaces.Add(type.Namespace);
            }

            // Add IFEA at top
            if (!String.IsNullOrEmpty(topItem))
                namespaces.Insert(0, topItem);
        }

        /// <summary>
        /// This method is used to get activity type and accordingly concatenates the type with IFEA
        /// </summary>
        /// <param name="value">Activity Name</param>
        /// <returns>Name concatenated with IFEA</returns>
        private string GetActivityType(string value)
        {
            string activityType = "";
            string activityAutomationCategory = System.Configuration.ConfigurationManager.AppSettings["AutomationActivityCategory"];
            if (value.Equals(Constants.Application.AutomationActivityAssemblyName))
                activityType = activityAutomationCategory;
            else
            {
                int index = value.LastIndexOf(".");
                if (index > 0)
                    activityType = activityAutomationCategory + "-" + value.Substring(index + 1);
            }
            return activityType;
        }

        private void InitialiseToolbox()
        {
            this.loadedToolboxActivities = new Dictionary<ToolboxCategory, IList<string>>();
            this.toolboxCategoryMap = new Dictionary<string, ToolboxCategory>();

            // GetAllNamespaces();

            var wfServices = WFServices.Instance;

            var activities = wfServices.activities;

            foreach (Tuple<String, List<Type>> t in activities)
            {
                //item1 - activityType
                //item2 - classList
                this.AddCategoryToToolbox(t.Item1, t.Item2);
            }


            /////********* IFEA WF activity ************///
            //foreach (string value in namespaces)
            //{
            //    string activityType = GetActivityType(value);
            //    {
            //        var classList = assembly.GetTypes().ToList().Where(t => t.Namespace == value).OrderBy(column => column.Name).ToList();
            //        if (classList != null)
            //        {
            //            this.AddCategoryToToolbox(activityType, classList);
            //        }
            //    }
            //}

            this.AddCategoryToToolbox(
                "Control Flow",
                new List<Type>
                {
                    //typeof(ForEach<>),        
                    // Fix for ForEech as per http://blogs.msdn.com/b/tilovell/archive/2009/12/29/the-trouble-with-system-activities-foreach-and-parallelforeach.aspx
                    typeof(System.Activities.Core.Presentation.Factories.ForEachWithBodyFactory<>),
                    typeof(If),
                    typeof(Parallel),
                    typeof(ParallelForEach<>),
                    typeof(DoWhile),
                    typeof(Pick),
                    typeof(PickBranch),
                    typeof(Sequence),
                    typeof(Switch<>),
                    typeof(While),
                });

            this.AddCategoryToToolbox(
                "Flowchart",
                new List<Type>
                {
                    typeof(Flowchart),
                    typeof(FlowDecision),
                    typeof(FlowSwitch<>),
                });

            this.AddCategoryToToolbox(
                "Messaging",
                new List<Type>
                {
                    typeof(CorrelationScope),
                    typeof(InitializeCorrelation),
                    typeof(Receive),
                    typeof(ReceiveAndSendReplyFactory),
                    typeof(Send),
                    typeof(SendAndReceiveReplyFactory),
                    typeof(TransactedReceiveScope)
                });

            this.AddCategoryToToolbox(
                "Runtime",
                new List<Type>
                {
                    typeof(Persist),
                    typeof(TerminateWorkflow),
                });

            this.AddCategoryToToolbox(
                "Primitives",
                new List<Type>
                {
                    typeof(Assign),
                    typeof(Delay),
                    typeof(InvokeMethod),
                    typeof(WriteLine),
                });

            this.AddCategoryToToolbox(
                "Transaction",
                new List<Type>
                {
                    typeof(CancellationScope),
                    typeof(CompensableActivity),
                    typeof(Compensate),
                    typeof(Confirm),
                    typeof(TransactionScope),
                });

            this.AddCategoryToToolbox(
                "Collection",
                new List<Type>
                {
                    typeof(AddToCollection<>),
                    typeof(ClearCollection<>),
                    typeof(ExistsInCollection<>),
                    typeof(RemoveFromCollection<>),
                });

            this.AddCategoryToToolbox(
                "Error Handling",
                new List<Type>
                {
                    typeof(Rethrow),
                    typeof(Throw),
                    typeof(TryCatch),
                });

            //this.AddCategoryToToolbox(
            //    "ForEachActivity",
            //      new List<Type>
            //      {
            //    typeof(ForEachFactory<>)
            //});
        }

        private void AddCategoryToToolbox(string categoryName, List<Type> activities)
        {
            foreach (Type activityType in activities)
            {
                if (this.IsValidToolboxActivity(activityType))
                {
                    ToolboxCategory category = this.GetToolboxCategory(categoryName);

                    if (!this.loadedToolboxActivities[category].Contains(activityType.FullName))
                    {
                        string displayName;
                        string[] splitName = activityType.Name.Split('`');
                        if (splitName.Length == 1)
                        {
                            displayName = activityType.Name;
                        }
                        else
                        {
                            displayName = string.Format("{0}<>", splitName[0]);
                        }

                        this.loadedToolboxActivities[category].Add(activityType.FullName);
                        category.Add(new ToolboxItemWrapper(activityType.FullName, activityType.Assembly.FullName, null, displayName));
                    }
                }
            }
        }

        private bool IsValidToolboxActivity(Type activityType)
        {
            return activityType.IsPublic && !activityType.IsNested && !activityType.IsAbstract
                        && (typeof(System.Activities.Activity).IsAssignableFrom(activityType) || typeof(IActivityTemplateFactory).IsAssignableFrom(activityType) ||
                        typeof(System.Activities.NativeActivity).IsAssignableFrom(activityType) ||
                        typeof(System.Activities.Statements.FlowNode).IsAssignableFrom(activityType) ||
                        typeof(System.Activities.Statements.PickBranch).IsAssignableFrom(activityType));
        }

        private ToolboxCategory GetToolboxCategory(string name)
        {
            if (this.toolboxCategoryMap.ContainsKey(name))
            {
                return this.toolboxCategoryMap[name];
            }
            else
            {
                ToolboxCategory category = new ToolboxCategory(name);
                this.toolboxCategoryMap[name] = category;
                this.loadedToolboxActivities.Add(category, new List<string>());
                this.txc.Categories.Add(category);
                return category;
            }
        }

        internal void NewWorkFlow()
        {
            wd = new WorkflowDesigner();

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            System.Activities.ActivityBuilder act = new ActivityBuilder();
            ////add default argument for passing some framework level information to workflow
            //act.Properties.Add(new DynamicActivityProperty
            //{
            //    Name = "iap_trackingref",
            //    Type = typeof(string),
            //    Value = ""
            //});
            act.Name = "IAPWorkflowDesign" + DateTime.Now.ToString("yyyyMMdd");

            wd.Load(act);

            st.Stop();
            var elapsed = st.Elapsed;
            elementHost1.Child = wd.View;
            wd.ModelChanged += new EventHandler(wd_ModelChanged);
            wd.TextChanged += new System.Windows.Controls.TextChangedEventHandler(wd_TextChanged);

            AddPropertyInspectorTab();
            this._presenter.ActivateMenuHandler();
            //elementHost2.Child = wd.PropertyInspectorView;


        }

        internal void Save()
        {
            saveFileDialog1.Filter = "XAML (*.xaml) |*.xaml|JPG Files (*.JPG)| *.jpg";
            saveFileDialog1.FileName = this.Title;
            saveFileDialog1.ShowDialog();

        }
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string name = saveFileDialog1.FileName;
            var filter = saveFileDialog1.Filter;
            var safeFileName = System.IO.Path.GetFileNameWithoutExtension(name);
            if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(safeFileName))
            {
                e.Cancel = true;

                return;

            }
            if (!string.IsNullOrEmpty(name))
            {
                if (saveFileDialog1.FilterIndex == 2)
                {
                    this.SaveImageToFile(saveFileDialog1.FileName, this.CreateWorkflowDefinitionImage());
                    System.Windows.Forms.MessageBox.Show("Image saved successfully to " + saveFileDialog1.FileName);
                }
                else
                {
                    wd.Save(name);
                    System.Windows.Forms.MessageBox.Show("Workflow saved successfully");
                    this._presenter.WFSaveHandler(name);
                    var wfWorkItems = this._presenter.WorkItem.Items.Get("Workflows") as Dictionary<WFDesigner, bool>;
                    wfWorkItems[this] = true;
                    promptToSave = false;
                    // CloseView(wfWorkItems);
                }

                this.Title = Path.GetFileNameWithoutExtension(name);

            }

        }

        internal void OpenWorkFlow(String fileName)
        {
            OpenWFDesigner(fileName);
            var wfWorkItems = this._presenter.WorkItem.Items.Get("Workflows") as Dictionary<WFDesigner, bool>;
            wfWorkItems[this] = true;
        }       

        # region Workflow Highlighting - Helper Methods
        private void RegisterMetadata()
        {
            (new DesignerMetadata()).Register();
        }
        object GetRootInstance()
        {
            ModelService modelService = wd.Context.Services.GetService<ModelService>();
            if (modelService != null)
            {
                return modelService.Root.GetCurrentValue();
            }
            else
            {
                return null;
            }
        }        
        Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService()
        {
            object rootInstance = GetRootInstance();
            Dictionary<object, SourceLocation> sourceLocationMapping = new Dictionary<object, SourceLocation>();
            Dictionary<object, SourceLocation> designerSourceLocationMapping = new Dictionary<object, SourceLocation>();

            if (rootInstance != null)
            {
                Activity documentRootElement = GetRootWorkflowElement(rootInstance);
                SourceLocationProvider.CollectMapping(GetRootRuntimeWorkflowElement(), documentRootElement, sourceLocationMapping,
                    wd.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);
                SourceLocationProvider.CollectMapping(documentRootElement, documentRootElement, designerSourceLocationMapping,
                   wd.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

            }

            // Notify the DebuggerService of the new sourceLocationMapping.
            // When rootInstance == null, it'll just reset the mapping.
            //DebuggerService debuggerService = debuggerService as DebuggerService;
            if (this.DebuggerService != null)
            {
                ((DebuggerService)this.DebuggerService).UpdateSourceLocations(designerSourceLocationMapping);
            }

            return sourceLocationMapping;
        }
        // Get root WorkflowElement.  Currently only handle when the object is ActivitySchemaType or WorkflowElement.
        // May return null if it does not know how to get the root activity.
        Activity GetRootWorkflowElement(object rootModelObject)
        {
            System.Diagnostics.Debug.Assert(rootModelObject != null, "Cannot pass null as rootModelObject");

            Activity rootWorkflowElement;
            IDebuggableWorkflowTree debuggableWorkflowTree = rootModelObject as IDebuggableWorkflowTree;
            if (debuggableWorkflowTree != null)
            {
                rootWorkflowElement = debuggableWorkflowTree.GetWorkflowRoot();
            }
            else // Loose xaml case.
            {
                rootWorkflowElement = rootModelObject as Activity;
            }
            return rootWorkflowElement;
        }
        Activity GetRuntimeExecutionRoot()
        {
            Activity root = ActivityXamlServices.Load(WorkFlowPath);
            WorkflowInspectionServices.CacheMetadata(root);
            return root;
        }
        Activity GetRootRuntimeWorkflowElement()
        {
            Activity root = null;
            if(!string.IsNullOrEmpty(WorkFlowPath))
                root = ActivityXamlServices.Load(WorkFlowPath);
            else
                root = ActivityXamlServices.Load(new System.IO.StringReader(wd.Text));

            WorkflowInspectionServices.CacheMetadata(root);

            IEnumerator<Activity> enumerator1 = WorkflowInspectionServices.GetActivities(root).GetEnumerator();
            //Get the first child of the x:class
            enumerator1.MoveNext();
            root = enumerator1.Current;

            //rootActivity = root;
            return root;
        }
        void ShowDebug(SourceLocation srcLoc)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Render
                , (Action)(() =>
                {
                    wd.DebugManagerView.CurrentLocation = srcLoc;

                }));

        }
        void RemoveDebug() 
        {
            //This is to remove the final debug adornment
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Render
                , (Action)(() =>
                {
                    wd.DebugManagerView.CurrentLocation = new SourceLocation(WorkFlowPath, 1, 1, 1, 10);
                }));
        }
        #endregion

        internal void OpenWFDesigner(string file)
        {
            WorkFlowPath = file;
            RegisterMetadata();

            wd = new WorkflowDesigner();
            this.DebuggerService = wd.DebugManagerView;
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            wd.Load(file);
            
            st.Stop();
            var elapsed = st.Elapsed;

            wd.ModelChanged += new EventHandler(wd_ModelChanged);
            wd.TextChanged += new System.Windows.Controls.TextChangedEventHandler(wd_TextChanged);
            elementHost1.Child = wd.View;
            // elementHost2.Child = wd.PropertyInspectorView;
            AddPropertyInspectorTab();
            this._presenter.ActivateMenuHandler();
            this._presenter.WFSaveHandler(file);
        }

        internal void OpenWorkFlow()
        {

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                try
                {
                    OpenWFDesigner(file);
                    var wfWorkItems = this._presenter.WorkItem.Items.Get("Workflows") as Dictionary<WFDesigner, bool>;
                    wfWorkItems[this] = true;

                }
                catch (System.IO.IOException)
                {
                }

            }
            else
            {
                this._presenter.OnCloseView();
            }

        }

        internal void OpenWorkFlowFromRepository(WorkflowPE data)
        {
            string content = String.Empty;
            try
            {
                this.data = data;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(data.WorkflowURI);
                CredentialCache cc = new CredentialCache();
                cc.Add(
                    new Uri(storageBaseUrl),
                    "NTLM",
                    CredentialCache.DefaultNetworkCredentials);
                req.Credentials = cc;
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();

                if (data.WorkflowURI.Contains(".iapw"))
                {
                    FolderBrowserDialog saveWorkflow = new FolderBrowserDialog();
                    saveWorkflow.Description = "Select download folder for the IAPW Package :";
                    if (saveWorkflow.ShowDialog() == DialogResult.OK)
                    {
                        string downloadLoc = saveWorkflow.SelectedPath + "\\" + data.Name + ".iapw";
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
                            string xamlFileName = "main";
                            string xamlFilePath = Path.Combine(extractionLoc, data.Name.ToString(), xamlFileName + ".xaml");
                            if (!File.Exists(xamlFilePath))
                                throw new Exception("Expected " + xamlFileName + ".xaml file not found in the package provided.");
                            else
                            {
                                byte[] byteFile = File.ReadAllBytes(xamlFilePath);
                                if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                                {
                                    if (Encoding.Unicode.GetString(byteFile).Contains("aWFw"))
                                    {
                                        byte[] byteContent = Infosys.WEM.SecureHandler.SecurePayload.UnSecureBytes(byteFile);
                                        Stream ScriptContent = new System.IO.MemoryStream(byteContent);
                                        using (StreamWriter sw = new StreamWriter("temp.xaml"))
                                            sw.Write((new StreamReader(ScriptContent)).ReadToEnd());
                                    }
                                    else
                                    {
                                        using (StreamWriter sw = new StreamWriter("temp.xaml"))
                                            sw.Write((new StreamReader(new System.IO.MemoryStream(byteFile)).ReadToEnd()));
                                    }
                                }
                                else
                                {
                                    using (StreamWriter sw = new StreamWriter("temp.xaml"))
                                        sw.Write((new StreamReader(new System.IO.MemoryStream(byteFile)).ReadToEnd()));
                                }
                            }
                        }
                        else
                            throw new Exception(result.Message);
                    }
                }
                else
                {
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

                            if (Encoding.Unicode.GetString(bytes).Contains("aWFw"))
                            {
                                byte[] byteContent = Infosys.WEM.SecureHandler.SecurePayload.UnSecureBytes(bytes);
                                Stream ScriptContent = new System.IO.MemoryStream(byteContent);
                                using (StreamWriter sw = new StreamWriter("temp.xaml"))
                                    sw.Write((new StreamReader(ScriptContent)).ReadToEnd());
                            }
                            else
                            {
                                using (StreamWriter sw = new StreamWriter("temp.xaml"))
                                    sw.Write((new StreamReader(new System.IO.MemoryStream(bytes)).ReadToEnd()));
                            }
                        }
                        else
                        {
                            using (StreamWriter sw = new StreamWriter("temp.xaml"))
                                sw.Write(sr.ReadToEnd());
                        }
                    }
                }


                WorkFlowPath = "temp.xaml";
                RegisterMetadata();
                wd = new WorkflowDesigner();
                this.DebuggerService = wd.DebugManagerView;
                wd.Load("temp.xaml");
                elementHost1.Child = wd.View;

                var wfWorkItems = this._presenter.WorkItem.Items.Get("Workflows") as Dictionary<WFDesigner, bool>;
                wfWorkItems[this] = true;

                wd.ModelChanged += new EventHandler(wd_ModelChanged);
                wd.TextChanged += new System.Windows.Controls.TextChangedEventHandler(wd_TextChanged);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //TabPage propertyinspector = new TabPage("Property Inspector");
            //System.Windows.Forms.Integration.ElementHost host = new System.Windows.Forms.Integration.ElementHost();
            //host.Child = wd.PropertyInspectorView;
            //host.Dock = DockStyle.Fill;
            //propertyinspector.Controls.Add(host);
            //tabWFProperties.TabPages.Add(propertyinspector);

            AddPropertyInspectorTab();

            // AddPropertyTab(data);
            AddPropertyTab(data);

            this._presenter.WFSaveHandler(data.Name);
            this._presenter.DisbalePublish_Handler();
        }
        private static byte[] ReadFully(Stream input)
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



        //internal void OpenWorkFlowFromRepository(WorkflowPE data)
        //{
        //    try
        //    {
        //        this.data = data;
        //        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(data.WorkflowURI);
        //        CredentialCache cc = new CredentialCache();
        //        cc.Add(
        //            new Uri(storageBaseUrl),
        //            "NTLM",
        //            CredentialCache.DefaultNetworkCredentials);
        //        req.Credentials = cc;
        //        HttpWebResponse response = (HttpWebResponse)req.GetResponse();

        //        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
        //        {
        //            if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
        //            {
        //                var bytes = default(byte[]);
        //                using (var memstream = new MemoryStream())
        //                {
        //                    var buffer = new byte[1024];
        //                    var bytesRead = default(int);
        //                    while ((bytesRead = sr.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
        //                        memstream.Write(buffer, 0, bytesRead);
        //                    bytes = memstream.ToArray();
        //                }

        //                if (Encoding.Unicode.GetString(bytes).Contains(Infosys.WEM.SecureHandler.SecurePayload.keyText))
        //                {
        //                    byte[] byteContent = Infosys.WEM.SecureHandler.SecurePayload.UnSecureBytes(bytes);
        //                    Stream ScriptContent = new System.IO.MemoryStream(byteContent);
        //                    using (StreamWriter sw = new StreamWriter("temp.xaml"))
        //                        sw.Write((new StreamReader(ScriptContent)).ReadToEnd());
        //                }
        //                else
        //                {
        //                    using (StreamWriter sw = new StreamWriter("temp.xaml"))
        //                        sw.Write((new StreamReader(new System.IO.MemoryStream(bytes)).ReadToEnd()));
        //                }
        //            }
        //            else
        //            {
        //                using (StreamWriter sw = new StreamWriter("temp.xaml"))
        //                    sw.Write(sr.ReadToEnd());
        //            }
        //        }
        //        wd = new WorkflowDesigner();
        //        wd.Load("temp.xaml");
        //        elementHost1.Child = wd.View;

        //        var wfWorkItems = this._presenter.WorkItem.Items.Get("Workflows") as Dictionary<WFDesigner, bool>;
        //        wfWorkItems[this] = true;

        //        wd.ModelChanged += new EventHandler(wd_ModelChanged);
        //        wd.TextChanged += new System.Windows.Controls.TextChangedEventHandler(wd_TextChanged);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    //TabPage propertyinspector = new TabPage("Property Inspector");
        //    //System.Windows.Forms.Integration.ElementHost host = new System.Windows.Forms.Integration.ElementHost();
        //    //host.Child = wd.PropertyInspectorView;
        //    //host.Dock = DockStyle.Fill;
        //    //propertyinspector.Controls.Add(host);
        //    //tabWFProperties.TabPages.Add(propertyinspector);

        //    AddPropertyInspectorTab();

        //    // AddPropertyTab(data);
        //    AddPropertyTab(data);

        //    this._presenter.WFSaveHandler(data.Name);
        //    this._presenter.DisbalePublish_Handler();
        //}

        void wd_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //isDirty = true;
        }

        void wd_ModelChanged(object sender, EventArgs e)
        {
            isDirty = true;
            promptToSave = true;

        }

        //public void AddPropertyTab(WorkflowPE data)
        public void AddPropertyTab(WorkflowPE data)
        {

            if (tabWFProperties.TabPages.Count == 1)
            {
                TabPage wfProperties = new TabPage("Workflow Details");
                wfProperties.AutoScroll = true;

                //add the workflow details view
                WFDetails wfDetails = new WFDetails(false, IsIapPackage);                
                wfDetails.InitializeSaveOption();

                wfDetails.Dock = DockStyle.Fill;
                wfProperties.Controls.Add(wfDetails);
                wfDetails.wfDe = this;
                if (data != null)
                {
                    if (!IsIapPackage)
                        wfDetails.SetData();

                    wfDetails.ShowWFDetailsEventHandler(this, new IMSWorkBench.Infrastructure.Interface.EventArgs<WorkflowPE>(data));

                    if (IsIapPackage)
                    {
                        Control btnInfo = wfDetails.Controls.Find("btnInfo", true)[0];
                        btnInfo.Visible = false;
                    }
                }
                else
                {
                    Control btnInfo = wfDetails.Controls.Find("btnInfo", true)[0];
                    btnInfo.Visible = false;
                }
                tabWFProperties.TabPages.Add(wfProperties);
                tabWFProperties.SelectedTab = tabWFProperties.TabPages[1];

            }
        }

        public void AddPropertyInspectorTab()
        {
            TabPage propertyinspector = new TabPage("Property Inspector");
            System.Windows.Forms.Integration.ElementHost host = new System.Windows.Forms.Integration.ElementHost();
            host.Child = wd.PropertyInspectorView;
            host.Dock = DockStyle.Fill;
            propertyinspector.Controls.Add(host);
            tabWFProperties.TabPages.Add(propertyinspector);
        }

        public void Run()
        {

            ExecuteWf executionNode = null;
            string wfText = "";
            wd.Flush(); // Flush has to be invoked before calling the text method to read the Activity XAML
            if (!string.IsNullOrEmpty(wd.Text))
            {
                wfText = wd.Text;
            }
            if (data != null && data.Parameters != null)
            {
                if (!string.IsNullOrEmpty(wd.Text))
                {
                    string missingParams = VerifyParameterArguments(wd.Text, data.Parameters);
                    if (!string.IsNullOrEmpty(missingParams))
                    {
                        System.Windows.Forms.MessageBox.Show("Argument(s) " + missingParams + " have not been specified as per published parameters. Please correct the same to run the workflow.", "Missing Arguments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                executionNode = new ExecuteWf(data.Parameters, string.Empty);
            }
            else
                executionNode = new ExecuteWf(null, wfText);

            if (!string.IsNullOrEmpty(wd.Text))
            {
                executionNode.WorkflowText = wd.Text;
            }
            var mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];


            if (mode == "Offline")
            {
                executionNode.DisableIAPNodeExecution = true;
                executionNode.DisableIAPNodeSchedule = true;
                //executionNode.Run();
                //if (executionNode.DialogResult == DialogResult.OK)
                //{
                //    System.Windows.Forms.MessageBox.Show("Workflow successfully executed/invoked.", "Workflow Invocation", MessageBoxButtons.OK);
                //}
            }

            else
            {
                if (data != null)
                {
                    if (data.CategoryID == 0 || data.WorkflowVersion == 0 || data.WorkflowID == Guid.Empty)
                    {
                        executionNode.DisableIAPNodeExecution = true;
                        executionNode.DisableIAPNodeSchedule = true;
                    }
                    else
                    {
                        executionNode.WorkFlowId = data.WorkflowID.ToString();
                        executionNode.WorkFlowName = data.Name;
                        executionNode.WorkflowVersion = data.WorkflowVersion;
                        executionNode.CategoryId = data.CategoryID;
                        executionNode.UsesUI = data.UsesUIAutomation;
                        if (data.UsesUIAutomation)
                            executionNode.DisableIAPNodeExecution = true;
                        else
                            executionNode.DisableIAPNodeExecution = false;

                    }
                }
                else
                {
                    executionNode.DisableIAPNodeExecution = true;
                    executionNode.DisableIAPNodeSchedule = true;
                }
            }

            executionNode.NodeExecuted += new ExecuteWf.NodeExecutedEventHandler(DisplayOutput);
            if (!String.IsNullOrEmpty(WorkFlowPath))
            {
                executionNode.SourceLocationMapping += new ExecuteWf.WFSourceLocationMappingEventHandler(UpdateSourceLocationMappingInDebuggerService);
                executionNode.ShowDebugEvent += new ExecuteWf.ShowDebugEventHandler(ShowDebug);
                executionNode.RemoveDebugEvent += new ExecuteWf.RemoveDebugEventHandler(RemoveDebug);
            }

            executionNode.ExecutionResultView -= new ExecuteWf.WFExecutionProgressEventHandler(AppendOutputShell);
            executionNode.ExecutionResultView += new ExecuteWf.WFExecutionProgressEventHandler(AppendOutputShell);
            executionNode.IsChanged = isDirty;

            executionNode.Show();
            //new System.Threading.Thread( () => executionNode.ShowDialog()).Start();
            // }
            //if (executionNode.ShowDialog() == DialogResult.None)
            //{ 
            //wd.Flush(); // Flush has to be invoked before calling the text method to read the Activity XAML
            //DynamicActivity wf = ActivityXamlServices.Load(new System.IO.StringReader(wd.Text)) as DynamicActivity;

            //WorkflowInvoker.Invoke(wf);
            //}

            //System.Windows.Forms.MessageBox.Show("Workflow successfully run.", "Workflow Invocation", MessageBoxButtons.OK);


        }
        public void DisplayOutput(List<ExecutionResultView> executionResultView)
        {
            ShowOutputView(this, new EventArgs<List<ExecutionResultView>>(executionResultView));
        }

        public void AppendOutputShell(Infosys.ATR.WFDesigner.Views.ExecuteWf.AppendOutputViewArgsWF appendOutputView)
        {
            AppendOutputViewWF(this, appendOutputView);
        }

        /// <summary>
        /// This method calls the SaveImageToFile method and passes the name of the image.
        /// </summary>
        internal void SaveWorkFlowAsImage()
        {
            SaveFileDialog saveImage = new SaveFileDialog();
            saveImage.Filter = "JPG Files (*.JPG)| *.jpg";

            if (saveImage.ShowDialog() == DialogResult.OK)
            {
                this.SaveImageToFile(saveImage.FileName, this.CreateWorkflowDefinitionImage());
                System.Windows.Forms.MessageBox.Show("Image saved successfully to " + saveImage.FileName);
                saveImage.Dispose();
            }
        }

        /// <summary>
        /// This method saves workflow as image.
        /// </summary>
        /// <param name="fileName">Name with path of the workflow image</param>
        /// <param name="image">Workflow image</param>
        private void SaveImageToFile(string fileName, BitmapFrame image)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// This method creates the screen space to be used for creating the image.
        /// </summary>
        /// <returns>Bitmap image</returns>
        private BitmapFrame CreateWorkflowDefinitionImage()
        {
            const double DPI = 96.0;
            Visual areaToSave = ((DesignerView)VisualTreeHelper.GetChild(this.wd.View, 0)).RootDesigner;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(areaToSave);
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)bounds.Width,
                (int)bounds.Height, DPI, DPI, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(areaToSave);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            bitmap.Render(dv);
            return BitmapFrame.Create(bitmap);
        }


        public bool Close()
        {
            var wfWorkItems = this._presenter.WorkItem.Items.Get("Workflows") as Dictionary<WFDesigner, bool>;
            var saved = wfWorkItems[this];


            if (promptToSave || !saved)
            {
                var result = System.Windows.MessageBox.Show("Do you want to save changes to Workflow - " + Title + " before exiting?", "IAP", MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
                if (result == MessageBoxResult.No)
                {
                    CloseView(wfWorkItems);
                    return true;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    Save();
                    return true;
                }
            }
            else
            {
                CloseView(wfWorkItems);
                return true;
            }
            return false;

        }

        private void CloseView(Dictionary<WFDesigner, bool> wfWorkItems)
        {
            wfWorkItems.Remove(this);
            this._presenter.OnCloseView();
        }

        internal byte[] GetWorkflowContent(WorkflowPE data)
        {

            if (string.IsNullOrEmpty(wd.Text))
            {
                wd.Save("temp.xaml");
            }
            else
            {
                ProcessWFXaml(data);
            }

            this.data = data;


            byte[] wf = File.ReadAllBytes("temp.xaml");            
            File.Delete("temp.xaml");
            return wf;
        }

        internal Workflow Upload(WorkflowPE data)
        {
            if (string.IsNullOrEmpty(wd.Text))
            {
                wd.Save("temp.xaml");
            }
            else
            {
                ProcessWFXaml(data);
            }

            this.data = data;
            WorkflowDS wf = new WorkflowDS();
            Workflow entity = new Workflow();
            entity.CompanyId = data.CompanyId;
            entity.File = new MemoryStream(File.ReadAllBytes("temp.xaml"));
            entity.FileName = data.FileName;
            entity.StorageBaseURL = storageBaseUrl + data.WorkflowURI;
            entity.UploadedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            entity.WorkflowId = data.WorkflowID.ToString();
            
            // entity.WorkflowVer = 1;
            Workflow response = wf.Upload(entity);

            if (response.StatusCode != 0)
            {
                File.Delete("temp.xaml");
                throw new Exception("File not uploaded");
            }

            return response;
            //  UploadAsImage(entity.WorkflowId);
        }

        /// <summary>
        /// This method is used to process workflow XAML for handling arguments.
        /// </summary>
        /// <param name="data">WorkflowPE object containing workflow metadata</param>
        private void ProcessWFXaml(WorkflowPE data)
        {
            // wd.Save("temp.xaml");
            wd.Flush();
            TextReader txtReader = new StringReader(wd.Text);
            var dynamicActivity = ActivityXamlServices.Load(txtReader) as DynamicActivity;
            if (dynamicActivity == null)
                throw new InvalidDataException("Invalid DynamicActivity.");

            dynamicActivity.Name = data.Name;
            var actBuilder = new ActivityBuilder();
            actBuilder.Implementation = dynamicActivity.Implementation != null ? dynamicActivity.Implementation() : null;
            actBuilder.Name = dynamicActivity.Name;

            foreach (var attribute in dynamicActivity.Attributes)
                actBuilder.Attributes.Add(attribute);


            foreach (var constraint in dynamicActivity.Constraints)
                actBuilder.Constraints.Add(constraint);

            foreach (var item in dynamicActivity.Properties)
            {
                var property = new DynamicActivityProperty
                {
                    Name = item.Name,
                    Type = item.Type,
                    Value = item.Value
                };

                foreach (var attribute in item.Attributes)
                    property.Attributes.Add(attribute);
                actBuilder.Properties.Add(property);
            }

            VisualBasic.SetSettings(actBuilder, VisualBasic.GetSettings(dynamicActivity));
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            XamlWriter xamlWriter = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(writer, new XamlSchemaContext()));
            XamlServices.Save(xamlWriter, actBuilder);

            // Add mc ignorable attribue and save the updated xaml in temp file
            string serializedXAML = AddMcIgnorableAttribute(builder.ToString());
            System.IO.File.WriteAllText(@"temp.xaml", serializedXAML);
            writer.Close();
        }

        private void UploadAsImage(string guid)
        {

            ((RoutedCommand)DesignerView.CopyAsImageCommand).Execute(null, wd.Context.Services.GetService<DesignerView>());
            if (System.Windows.Clipboard.ContainsImage())
            {
                System.Windows.Interop.InteropBitmap m = System.Windows.Clipboard.GetImage() as System.Windows.Interop.InteropBitmap;
                // Save
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(m));
                string fileName = @"temp.jpg";
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    encoder.Save(fs);
                }

                WorkflowDS wf = new WorkflowDS();
                Workflow entity = new Workflow();

                entity.File = new MemoryStream(File.ReadAllBytes("temp.jpg"));
                entity.FileName = data.FileName;
                entity.StorageBaseURL = "http://localhost/iapworkflowstore";
                entity.WorkflowId = guid;

                Workflow response = wf.Upload(entity);

                if (response.StatusCode != 0)
                    throw new Exception("Workflow Image not uploaded");
            }
            else
            {
                throw new Exception("Unable to upload Image");
            }

        }

        /// <summary>
        /// This method is used to add mc ignorable attribute to activity. The workflow execution was getting failed 
        /// when running from console. This attribute has been added to address that issue. Refer to below for details.
        /// http://blogs.msdn.com/b/tilovell/archive/2012/02/15/wf4-xaml-mc-ignoreable-viewstates-hintsizes-visualbasic-settings-and-manipulating-xaml-programmatically.aspx
        /// </summary>
        /// <param name="wfXaml">activity XAML</param>
        /// <returns>Updated XAML containing mc ingnorable attribute</returns>
        private string AddMcIgnorableAttribute(string wfXaml)
        {
            string encoding = "UTF-16";
            Stream xamlStream = new MemoryStream(Encoding.GetEncoding(encoding).GetBytes(wfXaml));
            XName activityName = XName.Get("Activity", "http://schemas.microsoft.com/netfx/2009/xaml/activities");
            XName mcIgnorableName = XName.Get("Ignorable", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            XElement xElement = XElement.Load(xamlStream, LoadOptions.PreserveWhitespace);
            string prefix = xElement.GetPrefixOfNamespace(XNamespace.Get("http://schemas.microsoft.com/netfx/2009/xaml/activities/presentation"));
            xElement.Add(new XAttribute(XNamespace.Xmlns + "mc", mcIgnorableName.NamespaceName));
            List<XAttribute> EventAttributes = xElement.Attributes().ToList();
            EventAttributes.Insert(0, new XAttribute(mcIgnorableName, prefix));
            xElement.ReplaceAttributes(EventAttributes.ToArray());
            MemoryStream updatedXamlStream = new MemoryStream();
            xElement.Save(updatedXamlStream, SaveOptions.DisableFormatting);
            updatedXamlStream.Position = 0;
            return new StreamReader(updatedXamlStream).ReadToEnd();
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

        public Mode OpMode { get; set; }
        public bool IsIapPackage { get; set; }
    }
}
