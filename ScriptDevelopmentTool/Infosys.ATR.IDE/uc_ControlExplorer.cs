using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Windows.Automation;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Infosys.ATR.UIAutomation.UserActivityMonitor;
using System.Threading;
using System.Management;
using Infosys.JavaAccessBridge;
using System.Configuration;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.IO;
using Infosys.ATR.CodeGeneration;
using Infosys.ATR.UIAutomation.Entities;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Infosys.ATR.DevelopmentStudio
{
    public partial class uc_ControlExplorer : UserControl
    {
        Shell shell;
        //public variables/interfaces
        public delegate void AutoStartControlExpEventHandler();
        public event AutoStartControlExpEventHandler AutoStartControlExp;

        public delegate void SaveImage(Bitmap image);
        public event SaveImage SaveImageHandler;

        public string Result { get; private set; }

        //private variables/interfaces
        string controlId = "", processId = "", processName = "", javaProcessId = "", selectedOMPath, folderHierarchicalImagePath = "";
        string currentCursor = "", objectModelPath = "", currentObjectImagePath;
        //WebBrowser browser = null;
        bool htmlLoaded = false;
        bool capturingOnClick = false;
        bool capturingOnMOve = false;
        bool SHIFTisBeingPressed = false;
        bool isRootNode = false;
        bool javaAppTreeCreated = false;
        bool menuStripDisabledForEdit = false;
        bool copyImageToRespectiveFolders = false;
        TreeNode lastSelectedNode = null;
        bool createFileForManualSelection = false;
        public static List<ProcessMapping> processMappings = new List<ProcessMapping>();
        public static Dictionary<string, string> elementImageMappings = new Dictionary<string, string>();
        //a mpping to hold the node Id and the corresponding control tree path i.e. pair: NodeId,ControlTreePath
        public static Dictionary<string, string> treeNodeAndTreePathMappings = new Dictionary<string, string>();
        //List<UIElement> Applications = new List<UIElement>();
        public static List<object> manuallySelectedNodes = new List<object>();
        static Desktop desktopElement = null;
        ContextMenuStrip menu = new ContextMenuStrip();
        Type selectedOM;
        CSharpCodeGenerator codeGenCSharp = new CSharpCodeGenerator();

        const int SPI_SETCURSORS = 0x0057;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;
        const int SW_RESTORE = 9;
        const int SW_SHOW = 5;
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Structures.RECT lpRect);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern IntPtr SetForegroundWindowNative(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        public delegate void Select(string[] automationElements);
        public event Select OnSelect;

        public uc_ControlExplorer()
        {
            InitializeComponent();
        }


        private void ControlExplorer_Load(object sender, EventArgs e)
        {
            try
            {
                //populate the element- icon mapping
                PopulateNodeImageList();

                RefreshControlLookUp();

                //check if the control images are to be copied to respective folders also
                string copy = ConfigurationManager.AppSettings["CopyImageToRespectiveFolders"];
                bool.TryParse(copy, out copyImageToRespectiveFolders);

                //check if class files are to be generated only for the manually selected nodes
                string generate = ConfigurationManager.AppSettings["GenerateClassFilesForManualSelection"];
                bool.TryParse(generate, out createFileForManualSelection);
            }
            catch (Exception ex)
            {
                var exConfig = ConfigurationManager.GetSection("exceptionHandling");
                if (exConfig != null)
                    ExceptionPolicy.HandleException(ex, "Default Policy");

                var message = ex.InnerException != null ? (!String.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message : ex.Message) : ex.Message;

                MessageBox.Show(message, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            // SubscribeEvents();
        }

        void SubscribeEvents()
        {
            //todo
            //find a better way to do this
            shell = (Shell)this.Parent.Parent.Parent.Parent;

            shell.OnRefresh += new DevelopmentStudio.Refresh(shell_OnRefresh);
            shell.OnCapture += new DevelopmentStudio.Capture(shell_OnCapture);
            shell.OnSave += new Save(shell_OnSave);
            shell.OnEdit += new Edit(shell_OnEdit);
            shell.OnTakeSnap += new DevelopmentStudio.TakeSnap(shell_OnTakeSnap);
            shell.OnFormClose += new FormClose(shell_OnFormClose);

        }

        void shell_OnEdit()
        {
            if (!menuStripDisabledForEdit)
            {

                openFileDialog1.Filter = "Object Model|*.xml";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ////this.Cursor = Cursors.WaitCursor;
                    //// menuStripDisabledForEdit = true;
                    ////   toolStripButtonEditOM.Text = "Go back to the Control Lookup Mode";
                    ////disable all the tool stip item except this
                    ////foreach (ToolStripButton item in toolStrip1.Items)
                    ////{
                    ////    if (item == toolStripButtonEditOM || item == toolStripButtonTakeSnap || item == toolStripButton_SaveOM)
                    ////        item.Enabled = true;
                    ////    else
                    ////        item.Enabled = false;
                    ////}
                    ////stop tracking if already on
                    //if (capturingOnMOve)
                    //{
                    //    toolStripbtnCtlCaptureOnMove_Click(null, null);
                    //}
                    //if (capturingOnClick)
                    //{
                    //    toolStripbtnCtrlCapture_Click(null, null);
                    //}

                    shell.ShowControlEditor(openFileDialog1.FileName);
                    //ControlEditor edit = new ControlEditor();
                    //edit.ObjectModel = openFileDialog1.FileName;
                    //edit.Render();

                    //open the object model

                    //selectedOMPath = openFileDialog1.FileName;
                    //string xml = System.IO.File.ReadAllText(selectedOMPath);
                    //desktopElement = SerializeAndDeserialize.Deserialize(xml, typeof(Desktop)) as Desktop;
                    //PopulateTreeWithObjectModel();
                    //this.Cursor = Cursors.Default;
                }
            }
            else
            {
                menuStripDisabledForEdit = false;
                RefreshControlLookUp();
                // toolStripButtonEditOM.Text = "Edit an Object Model";
            }
        }

        void shell_OnFormClose()
        {
            GlobalEventHandler.KeyDownEvents -= new KeyEventHandler(HookManager_KeyDown);
            GlobalEventHandler.KeyUpEvents -= new KeyEventHandler(HookManager_KeyUp);
            GlobalEventHandler.MouseDownEvents -= new MouseEventHandler(HookManager_MouseDown);
        }

        void shell_OnTakeSnap()
        {
            Snapper snap = new Snapper();
            snap.FilePaths = new List<string> { currentObjectImagePath, folderHierarchicalImagePath };
            snap.SaveAndClose = true;
            snap.Show();
        }

        //void shell_OnEdit()
        //{
        //    if (!menuStripDisabledForEdit)
        //    {

        //        openFileDialog1.Filter = "Object Model|*.xml";
        //        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        //        {
        //            //this.Cursor = Cursors.WaitCursor;
        //            // menuStripDisabledForEdit = true;
        //            //   toolStripButtonEditOM.Text = "Go back to the Control Lookup Mode";
        //            //disable all the tool stip item except this
        //            //foreach (ToolStripButton item in toolStrip1.Items)
        //            //{
        //            //    if (item == toolStripButtonEditOM || item == toolStripButtonTakeSnap || item == toolStripButton_SaveOM)
        //            //        item.Enabled = true;
        //            //    else
        //            //        item.Enabled = false;
        //            //}
        //            //stop tracking if already on
        //            //if (capturingOnMOve)
        //            //{
        //            //    shell_OnCapture(null, null);
        //            //}
        //            //if (capturingOnClick)
        //            //{
        //            //    toolStripbtnCtrlCapture_Click(null, null);
        //            //}

        //            ControlEditor edit = new ControlEditor();
        //            edit.ObjectModel = openFileDialog1.FileName;
        //            edit.Render();

        //            //open the object model

        //            //selectedOMPath = openFileDialog1.FileName;
        //            //string xml = System.IO.File.ReadAllText(selectedOMPath);
        //            //desktopElement = SerializeAndDeserialize.Deserialize(xml, typeof(Desktop)) as Desktop;
        //            //PopulateTreeWithObjectModel();
        //            //this.Cursor = Cursors.Default;
        //        }
        //    }
        //    else
        //    {
        //        menuStripDisabledForEdit = false;
        //        RefreshControlLookUp();
        //       // toolStripButtonEditOM.Text = "Edit an Object Model";
        //    }
        //}

        void shell_OnSave()
        {
            if (!menuStripDisabledForEdit)
            {

                if (desktopElement == null)
                {
                    desktopElement = new Desktop();
                    desktopElement.Applications = new List<UIElement>();
                }

                //desktopElement = new Desktop();
                //desktopElement.Applications = new List<UIElement>();
                folderBrowserDialog1.Description = "Select folder to save the object model(s)";
                if (string.IsNullOrEmpty(objectModelPath))
                    if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        objectModelPath = folderBrowserDialog1.SelectedPath;// +"\\" + Guid.NewGuid();

                this.Cursor = Cursors.WaitCursor;
                if (!string.IsNullOrEmpty(objectModelPath))
                {
                    string currentLoc = objectModelPath + "\\" + Guid.NewGuid();
                    foreach (TreeNode appNode in trAppControl.Nodes[0].Nodes)
                    {
                        if (appNode.Checked)
                            SaveAppObjectModel(appNode, currentLoc);
                    }
                    // GenerateCode(currentLoc);
                    //serialize the object model

                    string appXml = SerializeAndDeserialize.Serialize(desktopElement);

                    if (shell.Mode != "Web")
                    {
                        desktopElement = null;
                        //if (System.IO.Directory.Exists(currentLoc))
                        //    System.IO.File.WriteAllText(currentLoc + "\\objectModel.xml", appXml);
                        using (StreamWriter sw = new StreamWriter(currentLoc + "\\objectModel.xml"))
                        {
                            sw.Write(appXml);
                        }

                    }
                    else
                    {
                        if (File.Exists(currentLoc + "\\objectModel.xml"))
                            File.Delete(currentLoc + "\\objectModel.xml");

                        using (StreamWriter sw = new StreamWriter(currentLoc + "\\objectModel.xml"))
                        {
                            sw.Write(appXml);
                        }
                    }

                    //string appXml = SerializeAndDeserialize.Serialize(desktopElement);
                    //if (System.IO.Directory.Exists(currentLoc))
                    //    System.IO.File.WriteAllText(currentLoc + "\\objectModel.xml", appXml);
                }
                this.Cursor = Cursors.Default;
                shell.ShowControlEditor(desktopElement);
            }
            //   else
            // {
            this.Cursor = Cursors.WaitCursor;
            //i.e. in the edit mode
            // if (splitContainer1.Panel2.Controls.Count > 0)
            // {
            //if (splitContainer1.Panel2.Controls[0].GetType() == typeof(EditObjectModel))
            // {
            //using (EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel)
            //{
            // EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel;
            // BaseControl ctl = SerializeAndDeserialize.Deserialize(view.ElementDetails.Trim(), selectedOM) as BaseControl;
            // if (ctl.GetType() == typeof(WindowControl))
            //{
            //    WindowControl item = ctl as WindowControl;
            //    item.AutomationId = view.Id;
            //    item.Name = view.ElementName;
            //    item.LocalizedControlType = view.Type;
            //    item.ApplicationTreePath = view.AppTreePath;
            //    //first over write the object in the main desktop element to be persisted later
            //    ReplaceUIElementInApplication((trAppControl.SelectedNode.Tag as UIElement).ElementAttributes, item);
            //    (trAppControl.SelectedNode.Tag as UIElement).ElementAttributes = item;
            //}
            //else if (ctl.GetType() == typeof(JavaControl))
            //{
            //    JavaControl item = ctl as JavaControl;
            //    item.Id = view.Id;
            //    item.Name = view.ElementName;
            //    item.Role = view.Type;
            //    item.ApplicationTreePath = view.AppTreePath;
            //    //first over write the object in the main desktop element to be persisted later
            //    ReplaceUIElementInApplication((trAppControl.SelectedNode.Tag as UIElement).ElementAttributes, item);
            //    (trAppControl.SelectedNode.Tag as UIElement).ElementAttributes = item;
            //}

            //persist the changes
            // string desktopXml = SerializeAndDeserialize.Serialize(desktopElement);
            //System.IO.File.WriteAllText(selectedOMPath, desktopXml);
            //}
            //}
            // }



            //   this.Cursor = Cursors.Default;
            // }
        }

        void shell_OnCapture()
        {
            if (!capturingOnMOve)
            {
                GlobalEventHandler.KeyDownEvents += new KeyEventHandler(HookManager_KeyDown);
                GlobalEventHandler.KeyUpEvents += new KeyEventHandler(HookManager_KeyUp);
                GlobalEventHandler.MouseMovedEvent += new MouseEventHandler(HookManager_MouseMove);
                capturingOnMOve = true;
                //toolStripbtnCtlCaptureOnMove.Text = "Stop capturing the control details from application";
                //toolStripbtnCtlCaptureOnMove.Image = imageList2.Images[3];
            }
            else
            {
                GlobalEventHandler.KeyDownEvents -= new KeyEventHandler(HookManager_KeyDown);
                GlobalEventHandler.KeyUpEvents -= new KeyEventHandler(HookManager_KeyUp);
                GlobalEventHandler.MouseMovedEvent -= new MouseEventHandler(HookManager_MouseMove);
                capturingOnMOve = false;
                //toolStripbtnCtlCaptureOnMove.Text = "Start capturing the control details from application. Press 'SHIFT + move Mouse' on the control";
                //toolStripbtnCtlCaptureOnMove.Image = imageList2.Images[2];
            }
        }

        void shell_OnRefresh()
        {

            //splitContainer1.Panel2Collapsed = true;
            //this.Close();
            //if (AutoStartControlExp != null)
            //{
            //    AutoStartControlExp();
            //}
        }

        public void Refresh()
        {
            processMappings = new List<ProcessMapping>();
            treeNodeAndTreePathMappings = new Dictionary<string, string>();
            RefreshControlLookUp();
            //if (trAppControl.Nodes[0].IsExpanded)
            //{
            //    trAppControl.Nodes[0].Collapse();
            //    //System.Threading.Thread.Sleep(1000);
            //    //trAppControl.Nodes[0].Expand();
            //}
        }

        void snapItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = trAppControl.SelectedNode;
            if (selectedNode.Tag.GetType() == typeof(AutomationElement))
            {
                //TakeSnap((selectedNode.Tag as AutomationElement).Current.BoundingRectangle);
                System.Windows.Rect rect = (selectedNode.Tag as AutomationElement).Current.BoundingRectangle;
                TakeSnap((int)rect.X, (int)rect.Y, (int)rect.Height, (int)rect.Width, GetProcessIdForNode(selectedNode));
            }
            else if (selectedNode.Tag.GetType() == typeof(EX_JABHelper.AccessibleTreeItem))
            {
                Rectangle rect = (selectedNode.Tag as EX_JABHelper.AccessibleTreeItem).Bounds;
                TakeSnap(rect.X, rect.Y, rect.Height, rect.Width, GetProcessIdForNode(selectedNode));
            }
        }

        private void trAppControl_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currentObjectImagePath = "";
            folderHierarchicalImagePath = "";
            try
            {
                if (e.Node.Tag.GetType() == typeof(AutomationElement))
                {
                    controlId = e.Node.Name.Split('#')[0];
                    processId = (e.Node.Tag as AutomationElement).Current.ProcessId.ToString();
                    //processName = GetProcessName(int.Parse(processId));
                    // splitContainer1.Panel2Collapsed = false;
                    PopulateControlDetails(e.Node);
                    // AutomationElement element = e.Node.Tag as AutomationElement;


                }
                else if (e.Node.Tag.GetType() == typeof(EX_JABHelper.AccessibleTreeItem))
                {
                    controlId = e.Node.Name.Split('#')[0];
                    //processName = "java";
                    // splitContainer1.Panel2Collapsed = false;
                    PopulateJavaControlDetails(e.Node);
                }
                else if (e.Node.Tag.GetType() == typeof(UIElement))
                {
                    // splitContainer1.Panel2Collapsed = false;
                    PopulateObjectModel(e.Node);
                }
                else
                {
                    // splitContainer1.Panel2Collapsed = true;
                }

                if (lastSelectedNode != null)
                {
                    lastSelectedNode.BackColor = Color.White;
                    lastSelectedNode.ForeColor = Color.Black;
                }
                lastSelectedNode = e.Node;
                lastSelectedNode.BackColor = Color.Blue;
                lastSelectedNode.ForeColor = Color.White;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
        }

        private void trAppControl_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                ExpandNode(e.Node);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExpandNode(TreeNode currentNode)
        {
            //determine if the currently node being expanded is a root node
            if (currentNode.Parent == null)
                isRootNode = true;
            else
                isRootNode = false;

            if (currentNode.Tag.GetType() == typeof(AutomationElement))
            {
                AutomationElement node = currentNode.Tag as AutomationElement;
                processName = GetProcessName(node).ToLower();
                if (processName.Contains("java"))
                {
                    javaProcessId = node.Current.ProcessId.ToString();
                    Int32 vmid = 0;
                    EX_JABHelper.AccessibleTreeItem javaNode = EX_JABHelper.GetComponentTree((IntPtr)node.Current.NativeWindowHandle, out vmid);
                    javaAppTreeCreated = true;
                    var children = javaNode.children;
                    currentNode.Nodes.Clear();
                    for (int i = 0; i < children.Count; i++)
                    {
                        AddJavaNodeToTreeView(currentNode, children[i], i);
                    }
                }
                else
                {
                    //var children = node.FindAll(TreeScope.Children, Condition.TrueCondition);
                    List<AutomationElement> children = new List<AutomationElement>();
                    TreeWalker rawtw = TreeWalker.RawViewWalker;
                    AutomationElement child = rawtw.GetFirstChild(node);
                    while (child != null)
                    {
                        children.Add(child);
                        child = rawtw.GetNextSibling(child);
                    }

                    currentNode.Nodes.Clear();
                    for (int i = 0; i < children.Count; i++)
                    {
                        AddToTreeView(currentNode, children[i], i);
                    }
                }
            }
            else if (currentNode.Tag.GetType() == typeof(WebBrowser) && htmlLoaded)
            {
                WebBrowser browser = currentNode.Tag as WebBrowser;
                HtmlDocument doc = browser.Document;

            }
            else if (currentNode.Tag.GetType() == typeof(HtmlElement))
            {

            }
            else if (currentNode.Tag.GetType() == typeof(EX_JABHelper.AccessibleTreeItem)) //to handle java nodes
            {
                EX_JABHelper.AccessibleTreeItem javaNode = currentNode.Tag as EX_JABHelper.AccessibleTreeItem;
                var children = javaNode.children;
                currentNode.Nodes.Clear();
                for (int i = 0; i < children.Count; i++)
                {
                    AddJavaNodeToTreeView(currentNode, children[i], i);
                }
            }
            else if (currentNode.Tag.GetType() == typeof(UIElement))
            {
                UIElement OMNode = currentNode.Tag as UIElement;
                currentNode.Nodes.Clear();
                for (int i = 0; i < OMNode.Children.Count; i++)
                {
                    AddObjectModelNodeToTree(currentNode, OMNode.Children[i]);
                }
            }
        }

        private void AddObjectModelNodeToTree(TreeNode currNode, UIElement child)
        {
            TreeNode childNode = new TreeNode(child.DisplayText);
            childNode.Tag = child;
            childNode.Nodes.Add("Please wait...");
            //add node image
            string imageKey = child.DisplayText.ToLower().Split('-')[0].Trim();
            if (elementImageMappings.ContainsKey(imageKey))
            {
                childNode.ImageKey = imageKey;
                childNode.SelectedImageKey = imageKey;
            }
            currNode.Nodes.Add(childNode);
        }

        private void AddJavaNodeToTreeView(TreeNode currNode, EX_JABHelper.AccessibleTreeItem child, int peerIndex)
        {
            string nodeId = Guid.NewGuid().ToString();
            if (isRootNode)
            {
                treeNodeAndTreePathMappings.Add(nodeId, "/0[0]");
            }
            else
            {
                string[] nodeIdParts = currNode.Name.Split('#');
                if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
                {
                    int parentLevel = GetControlLevelInAppTree(treeNodeAndTreePathMappings[nodeIdParts[1]]);
                    int childLevel = parentLevel + 1;
                    treeNodeAndTreePathMappings.Add(nodeId, treeNodeAndTreePathMappings[nodeIdParts[1]] + "/" + childLevel + "[" + peerIndex + "]");
                }
            }

            string id = child.name;
            TreeNode node = new TreeNode(child.role + "- " + id);
            node.Name = id + "#" + nodeId;
            node.Tag = child;
            //rootNode.ImageIndex = 0;
            node.Nodes.Add("Please wait...");
            //add node image
            if (elementImageMappings.ContainsKey(child.role.ToLower()))
            {
                node.ImageKey = child.role;
                node.SelectedImageKey = child.role;
            }
            currNode.Nodes.Add(node);
        }

        private void AddToTreeView(TreeNode currNode, AutomationElement child, int peerIndex)
        {
            string nodeId = Guid.NewGuid().ToString();
            if (isRootNode) //i.e. the child is corresponding to the application node
            {
                List<int> childProcesses = GetAllChildProcesses(child.Current.ProcessId);
                if (childProcesses.Count > 0)
                {
                    processMappings.Add(new ProcessMapping() { ParentProcessId = child.Current.ProcessId, ChildProcessIds = childProcesses });
                }
                treeNodeAndTreePathMappings.Add(nodeId, "/0[0]");
            }
            else
            {
                string[] nodeIdParts = currNode.Name.Split('#');
                if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
                {
                    int parentLevel = GetControlLevelInAppTree(treeNodeAndTreePathMappings[nodeIdParts[1]]);
                    int childLevel = parentLevel + 1;
                    treeNodeAndTreePathMappings.Add(nodeId, treeNodeAndTreePathMappings[nodeIdParts[1]] + "/" + childLevel + "[" + peerIndex + "]");
                }
            }

            //to handle html content in the Internet Explorer
            //htmlLoaded = false;
            //if (child.Current.ClassName == "Internet Explorer_Server")
            //{
            //    AddHTMLDocumentToTreeView(currNode, child);
            //    return;
            //}
            string id = "";
            if (!string.IsNullOrEmpty(child.Current.AutomationId))
                id = child.Current.AutomationId;
            else if (!string.IsNullOrEmpty(child.Current.Name))
                id = child.Current.Name;
            else if (!string.IsNullOrEmpty(child.Current.ClassName))
                id = child.Current.ClassName;
            TreeNode node = new TreeNode(child.Current.LocalizedControlType + "- " + id);
            node.Name = id + "#" + nodeId;
            node.Tag = child;
            //rootNode.ImageIndex = 0;
            node.Nodes.Add("Please wait...");
            //add node image
            if (elementImageMappings.ContainsKey(child.Current.LocalizedControlType.ToLower()))
            {
                node.ImageKey = child.Current.LocalizedControlType;
                node.SelectedImageKey = child.Current.LocalizedControlType;
            }

            currNode.Nodes.Add(node);
        }

        private void AddHTMLDocumentToTreeView(TreeNode currNode, AutomationElement child)
        {
            string id = "";
            if (!string.IsNullOrEmpty(child.Current.AutomationId))
                id = child.Current.AutomationId;
            else if (!string.IsNullOrEmpty(child.Current.Name))
                id = child.Current.Name;
            TreeNode node = new TreeNode(child.Current.LocalizedControlType + "- " + id);
            node.Name = id;

            WebBrowser browser = new WebBrowser();
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            object objPattern;
            if (child.TryGetCurrentPattern(ValuePattern.Pattern, out objPattern))
            {
                ValuePattern valuePattern = objPattern as ValuePattern;
                string url = valuePattern.Current.Value;
                browser.Url = new Uri(url);
                node.Tag = browser;
            }
            node.Nodes.Add("Please wait...");
            currNode.Nodes.Add(node);
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            htmlLoaded = true;
        }

        private void AddHTMLElementToTreeView(TreeNode currNode, HtmlElement child)
        {

        }

        private void PopulateControlDetails(TreeNode node)
        {
            // ShowObjectModelView(ObjectModelViewMode.ViewMode);

            //if (splitContainer1.Panel2.Controls.Count > 0)
            //{
            //    if (splitContainer1.Panel2.Controls[0].GetType() == typeof(ViewObjectModel))
            //    {
            //    ViewObjectModel view = splitContainer1.Panel2.Controls[0] as ViewObjectModel;

            //view.ElementDetails = "Name- " + element.Current.Name + Environment.NewLine;
            //view.ElementDetails += "Automation Id- " + element.Current.AutomationId + Environment.NewLine;
            //view.ElementDetails += "Control Type- " + element.Current.ControlType.ProgrammaticName + Environment.NewLine;
            //view.ElementDetails += "Localized Control Type- " + element.Current.LocalizedControlType + Environment.NewLine;
            //view.ElementDetails += "Access Key- " + element.Current.AccessKey + Environment.NewLine;
            //view.ElementDetails += "UI Framework- " + element.Current.FrameworkId + Environment.NewLine;
            //view.ElementDetails += "Bounding Rectangle- " + element.Current.BoundingRectangle.ToString() + Environment.NewLine;
            //view.ElementDetails += "Window Handle- " + (IntPtr)element.Current.NativeWindowHandle + Environment.NewLine;
            //view.ElementDetails += "Class Name- " + element.Current.ClassName + Environment.NewLine;
            //view.ElementDetails += "Process Id- " + element.Current.ProcessId + Environment.NewLine;
            //view.ElementDetails += "Control Pattern(s)- ";
            //foreach (AutomationPattern pattern in element.GetSupportedPatterns())
            //{
            //    view.ElementDetails += pattern.ProgrammaticName + ",";
            //}
            //string[] nodeIdParts = node.Name.Split('#');
            //if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
            //{
            //    view.ElementDetails += Environment.NewLine + "Pointer in Application Tree Path (Level[index])-" + treeNodeAndTreePathMappings[nodeIdParts[1]];
            //}

            AutomationElement element = node.Tag as AutomationElement;
            StringBuilder child = new StringBuilder();
            StringBuilder childData = new StringBuilder();
            child.Append("Name- " + element.Current.Name + Environment.NewLine);
            childData.Append(element.Current.Name + Environment.NewLine);
            child.Append("Automation Id- " + element.Current.AutomationId + Environment.NewLine);
            childData.Append(element.Current.AutomationId + Environment.NewLine);
            child.Append("Control Type- " + element.Current.ControlType.ProgrammaticName + Environment.NewLine);
            childData.Append(element.Current.ControlType.ProgrammaticName + Environment.NewLine);
            child.Append("Localized Control Type- " + element.Current.LocalizedControlType + Environment.NewLine);
            child.Append("Access Key- " + element.Current.AccessKey + Environment.NewLine);
            child.Append("UI Framework- " + element.Current.FrameworkId + Environment.NewLine);
            child.Append("Bounding Rectangle- " + element.Current.BoundingRectangle.ToString() + Environment.NewLine);
            child.Append("Window Handle- " + (IntPtr)element.Current.NativeWindowHandle + Environment.NewLine);
            child.Append("Class Name- " + element.Current.ClassName + Environment.NewLine);
            child.Append("Process Id- " + element.Current.ProcessId + Environment.NewLine);
            child.Append("Control Pattern(s)- ");
            foreach (AutomationPattern pattern in element.GetSupportedPatterns())
            {
                child.Append(pattern.ProgrammaticName + ",");
            }
            string[] nodeIdParts = node.Name.Split('#');
            if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
            {
                child.Append(Environment.NewLine + "Pointer in Application Tree Path (Level[index])-" + treeNodeAndTreePathMappings[nodeIdParts[1]]);
                childData.Append(Environment.NewLine + treeNodeAndTreePathMappings[nodeIdParts[1]]);
            }

            StringBuilder parent = new StringBuilder();

            if (node.Parent != null)
            {
                element = node.Parent.Tag as AutomationElement;
                parent.Append("Name- " + element.Current.Name + Environment.NewLine);
                parent.Append("Automation Id- " + element.Current.AutomationId + Environment.NewLine);
                parent.Append("Control Type- " + element.Current.ControlType.ProgrammaticName + Environment.NewLine);
                parent.Append("Localized Control Type- " + element.Current.LocalizedControlType + Environment.NewLine);
                parent.Append("Access Key- " + element.Current.AccessKey + Environment.NewLine);
                parent.Append("UI Framework- " + element.Current.FrameworkId + Environment.NewLine);
                parent.Append("Bounding Rectangle- " + element.Current.BoundingRectangle.ToString() + Environment.NewLine);
                parent.Append("Window Handle- " + (IntPtr)element.Current.NativeWindowHandle + Environment.NewLine);
                parent.Append("Class Name- " + element.Current.ClassName + Environment.NewLine);
                parent.Append("Process Id- " + element.Current.ProcessId + Environment.NewLine);
                parent.Append("Control Pattern(s)- ");

                foreach (AutomationPattern pattern in element.GetSupportedPatterns())
                {
                    parent.Append(pattern.ProgrammaticName + ",");
                }
                string[] nodeIdParts1 = node.Parent.Name.Split('#');
                if (nodeIdParts1.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts1[1]))
                {
                    parent.Append(Environment.NewLine + "Pointer in Application Tree Path (Level[index])-" + treeNodeAndTreePathMappings[nodeIdParts1[1]]);
                }
            }
            if (OnSelect != null)
                OnSelect(new string[] { child.ToString(), childData.ToString(), parent.ToString() });

            // }
            //}
        }

        private void PopulateJavaControlDetails(TreeNode node)
        {
            // ShowObjectModelView(ObjectModelViewMode.ViewMode);

            //if (splitContainer1.Panel2.Controls.Count > 0)
            //{
            //    if (splitContainer1.Panel2.Controls[0].GetType() == typeof(ViewObjectModel))
            //    {
            //        ViewObjectModel view = splitContainer1.Panel2.Controls[0] as ViewObjectModel;
            //        EX_JABHelper.AccessibleTreeItem element = node.Tag as EX_JABHelper.AccessibleTreeItem;
            //        view.ElementDetails = "Name- " + element.name + Environment.NewLine;
            //        view.ElementDetails += "Role- " + element.role + Environment.NewLine;
            //        view.ElementDetails += "State(s)- " + element.states + Environment.NewLine;
            //        view.ElementDetails += "Framework- " + processName + Environment.NewLine;
            //        view.ElementDetails += "Accessible Action(s)- " + element.accessibleActions + Environment.NewLine;
            //        view.ElementDetails += "Bounding Rectangle- " + element.Bounds.ToString() + Environment.NewLine;
            //        view.ElementDetails += "Description- " + element.description + Environment.NewLine;
            //        view.ElementDetails += "Element Pointer- " + element.ElementPointer.ToString() + Environment.NewLine;
            //        view.ElementDetails += "Text Value (if any)- " + element.textValue;
            //        string[] nodeIdParts = node.Name.Split('#');
            //        if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
            //        {
            //            view.ElementDetails += Environment.NewLine + "Pointer in Application Tree Path (Level[index])-" + treeNodeAndTreePathMappings[nodeIdParts[1]];
            //        }
            //    }
            //}

            EX_JABHelper.AccessibleTreeItem element = node.Tag as EX_JABHelper.AccessibleTreeItem;
            StringBuilder sb = new StringBuilder();
            sb.Append("Name- " + element.name + Environment.NewLine);
            sb.Append("Role- " + element.role + Environment.NewLine);
            sb.Append("State(s)- " + element.states + Environment.NewLine);
            sb.Append("Framework- " + processName + Environment.NewLine);
            sb.Append("Accessible Action(s)- " + element.accessibleActions + Environment.NewLine);
            sb.Append("Bounding Rectangle- " + element.Bounds.ToString() + Environment.NewLine);
            sb.Append("Description- " + element.description + Environment.NewLine);
            sb.Append("Element Pointer- " + element.ElementPointer.ToString() + Environment.NewLine);
            sb.Append("Text Value (if any)- " + element.textValue);
            string[] nodeIdParts = node.Name.Split('#');
            if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
            {
                sb.Append(Environment.NewLine + "Pointer in Application Tree Path (Level[index])-" + treeNodeAndTreePathMappings[nodeIdParts[1]]);
            }

            if (OnSelect != null)
                OnSelect(new string[] { sb.ToString() });
        }

        private void PopulateObjectModel(TreeNode node)
        {
            // ShowObjectModelView(ObjectModelViewMode.EditMode);

            //if (splitContainer1.Panel2.Controls.Count > 0)
            //{
            //    if (splitContainer1.Panel2.Controls[0].GetType() == typeof(EditObjectModel))
            //    {
            //        EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel;
            //        UIElement ctl = node.Tag as UIElement;
            //        currentObjectImagePath = ctl.ElementImageFile;
            //        folderHierarchicalImagePath = ctl.HierarchicalImageLoc;
            //        selectedOM = ctl.ElementAttributes.GetType();
            //        view.ElementDetails = SerializeAndDeserialize.Serialize(ctl.ElementAttributes);
            //        if (ctl.ElementAttributes.GetType() == typeof(WindowControl))
            //        {
            //            WindowControl item = ctl.ElementAttributes as WindowControl;
            //            view.AssignIdentifiers(item.AutomationId, item.Name, item.LocalizedControlType, item.ApplicationTreePath);
            //        }
            //        else if (ctl.ElementAttributes.GetType() == typeof(JavaControl))
            //        {
            //            JavaControl item = ctl.ElementAttributes as JavaControl;
            //            view.AssignIdentifiers(item.Id, item.Name, item.Role, item.ApplicationTreePath);
            //        }

            //        if (!string.IsNullOrEmpty(ctl.ElementImageFile))
            //        {
            //            System.Drawing.Image img = System.Drawing.Image.FromFile(ctl.ElementImageFile);
            //            view.ElementImage = img;
            //        }
            //    }
            //}
        }

        void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (SHIFTisBeingPressed && e.Button == MouseButtons.Left)
                {
                    AutomationElement element = ElementFromCursor(e.X, e.Y);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //bool working = false;
        //void HookManager_MouseMove(object sender, MouseEventArgs e)
        //{
        //    try
        //    {
        //        if (SHIFTisBeingPressed && !working)
        //        {
        //            working = true;
        //            //Thread.Sleep(2000);
        //            AutomationElement element = ElementFromCursor(e.X, e.Y);
        //            //Thread.Sleep(2000);
        //            working = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        working = false;
        //    }
        //}
        DateTime starttime = DateTime.MinValue;
        void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (SHIFTisBeingPressed)
                {
                    if (starttime == DateTime.MinValue)
                        starttime = DateTime.Now;

                    if (DateTime.Now.Millisecond - 500 > starttime.Second)
                    {
                        AutomationElement element = ElementFromCursor(e.X, e.Y);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentCursor))
            {
                //MessageBox.Show(string.Format("Key Up - {0}\n", e.KeyCode));
                if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                {
                    SHIFTisBeingPressed = false;
                    starttime = DateTime.MinValue;
                    //Cursor.Current = Cursors.Default;
                    //ChangeCursor(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\cursors\aero_arrow.cur");
                    //if (!string.IsNullOrEmpty(currentCursor))
                    ChangeCursor(currentCursor, true);
                    //else
                    //ChangeCursor(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\cursors\aero_arrow.cur");
                    // ChangeCursor(@"images\aero_arrow.cur");
                }
            }
        }

        void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            //if currentcursor value is available then only change, otherwise wont be able to return to default.
            if (!string.IsNullOrEmpty(currentCursor))
            {
                //MessageBox.Show(string.Format("Key Down - {0}\n", e.KeyCode));
                if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.ControlKey)
                {
                    SHIFTisBeingPressed = true;
                    //Cursor.Current = Cursors.Cross;// new Cursor(@"images\lookup.png");
                    ChangeCursor(@"images\Gun_Sight.ani");
                }
            }
        }

        private AutomationElement ElementFromCursor(int X, int Y)
        {
            // Convert mouse position to System.Windows.Point.
            AutomationElement element = null;
            //putting this code under a different thread because of the exception:
            //An outgoing call cannot be made since the application is dispatching an input-synchronous call. (Exception from HRESULT: 0x8001010D (RPC_E_CANTCALLOUT_ININPUTSYNCCALL)).
            //ref.- http://stackoverflow.com/questions/18670437/com-exception-was-unhandled-in-c-sharp, http://go4answers.webhost4life.com/Example/exception-observed-while-getting-58920.aspx
            Thread thread = new Thread(() =>
            {
                System.Windows.Point point = new System.Windows.Point(X, Y);
                element = AutomationElement.FromPoint(point);
                int pID = 0;

                if (element != null)
                    pID = element.Current.ProcessId;

                if (element != null && !GetProcessName(element).ToLower().Contains("java"))
                    PointElementInTree(element);
                else
                {
                    if (!javaAppTreeCreated)
                    {
                        //as per JAB, the component tree needs to be created before calls like GetItemAt, etc
                        Int32 vmid = 0;
                        EX_JABHelper.GetComponentTree((IntPtr)element.Current.NativeWindowHandle, out vmid);
                        javaAppTreeCreated = true;
                    }
                    EX_JABHelper.AccessibleTreeItem item = EX_JABHelper.GetItemAt(new Point(X, Y));
                    PointElementInTree(pID, item);
                }
            });
            //changing the apartment state to STA as for certain control like webbrowser (activex) getting error
            //that activex control can be used in a STA only
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return element;
        }

        /// <summary>
        /// Point the element in the application tree for non Java application
        /// </summary>
        /// <param name="element"></param>
        private void PointElementInTree(AutomationElement element)
        {
            //return if the click has happened on this tool itself
            if (ClickedOnMe(element.Current.ProcessId))
                return;
            if (trAppControl.InvokeRequired)
            {
                trAppControl.BeginInvoke(new Action<AutomationElement>(PointElementInTree), new object[] { element });
                return;
            }
            TreeNode appNode = ExpandTheApplicationNode(element.Current.ProcessId);
            if (appNode != null)
            {
                foreach (TreeNode node in appNode.Nodes)
                {
                    if (HighLightNode(element, node) != null)
                        break;
                }
            }
        }


        /// <summary>
        /// Point element in the Java Application tree
        /// </summary>
        /// <param name="element"></param>
        private void PointElementInTree(int procId, EX_JABHelper.AccessibleTreeItem element)
        {
            //return if the click has happened on this tool itself
            if (ClickedOnMe(procId))
                return;
            if (trAppControl.InvokeRequired)
            {
                trAppControl.BeginInvoke(new Action<int, EX_JABHelper.AccessibleTreeItem>(PointElementInTree), new object[] { procId, element });
                return;
            }
            TreeNode appNode = ExpandTheApplicationNode(procId);
            //appNode.ExpandAll();
            //Thread.Sleep(1000);
            if (appNode != null)
            {
                foreach (TreeNode node in appNode.Nodes)
                {
                    if (HighLightNode(element, node) != null)
                        break;
                }
            }
        }

        private TreeNode ExpandTheApplicationNode(int processid)
        {
            bool appNodeFound = false;
            TreeNode appNode = null;
            try
            {
                if (!trAppControl.Nodes[0].IsExpanded)
                    trAppControl.Nodes[0].Expand();
                //first look for the parent process id then child processes
                for (int i = 0; i < trAppControl.Nodes[0].Nodes.Count; i++)
                {
                    try
                    {
                        AutomationElement appElement = trAppControl.Nodes[0].Nodes[i].Tag as AutomationElement;
                        if (appElement != null && appElement.Current.ProcessId != null && appElement.Current.ProcessId == processid)
                        {
                            trAppControl.Nodes[0].Nodes[i].Expand();
                            appNode = trAppControl.Nodes[0].Nodes[i];
                            appNodeFound = true;
                            break;
                        }
                    }
                    catch (System.Windows.Automation.ElementNotAvailableException ex)
                    {
                        //just ignore as the UI might not be in scope now
                    }

                    //cant check for child process like below because for the bottom task bar
                    //it will find one icon for the application and will look for the control in the task bar itself.
                    //refer to the if (!appNodeFound) section below

                    //else //check if the process id from control is one of the child processes, if any
                    //{
                    //ProcessMapping map=  processMappings.Where(p => p.ParentProcessId == appElement.Current.ProcessId).FirstOrDefault();
                    //if (map != null)
                    //{
                    //    if (map.ChildProcessIds.Contains(processid))
                    //    {
                    //        trAppControl.Nodes[0].Nodes[i].Expand();
                    //        appNode = trAppControl.Nodes[0].Nodes[i];
                    //        break;
                    //    }
                    //}
                    //}
                }
                if (!appNodeFound)
                {
                    for (int i = 0; i < trAppControl.Nodes[0].Nodes.Count; i++)
                    {
                        try
                        {
                            AutomationElement appElement = trAppControl.Nodes[0].Nodes[i].Tag as AutomationElement;
                            ProcessMapping map = processMappings.Where(p => p.ParentProcessId == appElement.Current.ProcessId).FirstOrDefault();
                            if (map != null)
                            {
                                if (map.ChildProcessIds.Contains(processid))
                                {
                                    trAppControl.Nodes[0].Nodes[i].Expand();
                                    appNode = trAppControl.Nodes[0].Nodes[i];
                                    break;
                                }
                            }
                        }
                        catch (System.Windows.Automation.ElementNotAvailableException ex)
                        {
                            //just ignore as the UI might not be in scope now
                        }
                    }
                }
            }
            catch (System.Windows.Automation.ElementNotAvailableException ex)
            {
                //then release the the shift key
                SHIFTisBeingPressed = false;
                starttime = DateTime.MinValue;
                ChangeCursor(currentCursor, true);

                //MessageBox.Show("Caught by an error while expanding application, please refresh the control explorer?", "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //if (MessageBox.Show("Caught by an error, please refresh the control explorer?", "OOPS... Caught by an error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                //{
                //    // this.Close();
                //    //if (AutoStartControlExp != null)
                //    //{
                //    //    AutoStartControlExp();
                //    //}
                //}                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return appNode;
        }

        /// <summary>
        /// Highlight the node for Java Application tree
        /// </summary>
        /// <param name="element"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private TreeNode HighLightNode(EX_JABHelper.AccessibleTreeItem element, TreeNode node)
        {
            TreeNode highlightedNode = null;
            EX_JABHelper.AccessibleTreeItem tempItem = node.Tag as EX_JABHelper.AccessibleTreeItem;
            if (tempItem == element)
            {
                trAppControl.SelectedNode = node;
                highlightedNode = node;
            }
            else
            {
                if (!node.IsExpanded)
                    node.Expand();
                foreach (TreeNode childNolde in node.Nodes)
                {
                    highlightedNode = HighLightNode(element, childNolde);
                    if (highlightedNode != null)
                        break;
                }
            }
            return highlightedNode;
        }

        /// <summary>
        ///  Highlight the node for Non-Java Application tree
        /// </summary>
        /// <param name="element"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private TreeNode HighLightNode(AutomationElement element, TreeNode node)
        {
            TreeNode highlightedNode = null;
            if (node.Tag as AutomationElement == element)
            {
                trAppControl.SelectedNode = node;
                highlightedNode = node;
            }
            else
            {
                //AutomationElementCollection children = (node.Tag as AutomationElement).FindAll(TreeScope.Children, Condition.TrueCondition);
                node.Expand();
                foreach (TreeNode childNolde in node.Nodes)
                {
                    highlightedNode = HighLightNode(element, childNolde);
                    if (highlightedNode != null)
                        break;
                }
            }
            return highlightedNode;
        }

        private bool ClickedOnMe(int processId)
        {
            //get current process id
            if (processId == System.Diagnostics.Process.GetCurrentProcess().Id)
                return true;
            else
                return false;
        }

        //private void toolStripbtnOK_Click(object sender, EventArgs e)
        //{
        //    this.Result = "GetAppWindowFromProcessName(\"" + processName + "\").FindControl(\"" + controlId + "\")";
        //    Clipboard.SetText(this.Result);
        //    HookManager.KeyDown -= new KeyEventHandler(HookManager_KeyDown);
        //    HookManager.KeyUp -= new KeyEventHandler(HookManager_KeyUp);
        //    HookManager.MouseDown -= new MouseEventHandler(HookManager_MouseDown);
        //   // this.Close();
        //}



        //private void ControlExplorer_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    HookManager.KeyDown -= new KeyEventHandler(HookManager_KeyDown);
        //    HookManager.KeyUp -= new KeyEventHandler(HookManager_KeyUp);
        //    HookManager.MouseDown -= new MouseEventHandler(HookManager_MouseDown);
        //}

        public void Capture()
        {
            if (capturingOnClick)
            {
                // toolStripbtnCtrlCapture_Click(null, null);
            }
            if (!capturingOnMOve)
            {
                GlobalEventHandler.KeyDownEvents += new KeyEventHandler(HookManager_KeyDown);
                GlobalEventHandler.KeyUpEvents += new KeyEventHandler(HookManager_KeyUp);
                GlobalEventHandler.MouseMovedEvent += new MouseEventHandler(HookManager_MouseMove);
                capturingOnMOve = true;
                // toolStripbtnCtlCaptureOnMove.Text = "Stop capturing the control details from application";
                //toolStripbtnCtlCaptureOnMove.Image = imageList2.Images[3];
            }
            else
            {
                GlobalEventHandler.KeyDownEvents -= new KeyEventHandler(HookManager_KeyDown);
                GlobalEventHandler.KeyUpEvents -= new KeyEventHandler(HookManager_KeyUp);
                GlobalEventHandler.MouseMovedEvent -= new MouseEventHandler(HookManager_MouseMove);
                capturingOnMOve = false;
                // toolStripbtnCtlCaptureOnMove.Text = "Start capturing the control details from application. Press 'SHIFT + move Mouse' on the control";
                // toolStripbtnCtlCaptureOnMove.Image = imageList2.Images[2];
            }
        }

        private void toolStripbtnCtlCaptureOnMove_Click(object sender, EventArgs e)
        {
            Capture();
        }

        //private void toolStripButton_SaveOM_Click(object sender, EventArgs e)
        //{
        //    if (!menuStripDisabledForEdit)
        //    {
        //        desktopElement = new Desktop();
        //        desktopElement.Applications = new List<UIElement>();
        //        folderBrowserDialog1.Description = "Select folder to save the object model(s)";
        //        if (string.IsNullOrEmpty(objectModelPath))
        //            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //                objectModelPath = folderBrowserDialog1.SelectedPath;// +"\\" + Guid.NewGuid();

        //        this.Cursor = Cursors.WaitCursor;
        //        if (!string.IsNullOrEmpty(objectModelPath))
        //        {
        //            string currentLoc = objectModelPath + "\\" + Guid.NewGuid();
        //            foreach (TreeNode appNode in trAppControl.Nodes[0].Nodes)
        //            {
        //                if (appNode.Checked)
        //                    SaveAppObjectModel(appNode, currentLoc);
        //            }
        //            GenerateCode(currentLoc);
        //            //serialize the object model
        //            string appXml = SerializeAndDeserialize.Serialize(desktopElement);
        //            if(System.IO.Directory.Exists(currentLoc))
        //                System.IO.File.WriteAllText(currentLoc + "\\objectModel.xml", appXml);
        //        }
        //        this.Cursor = Cursors.Default;
        //    }
        //    else
        //    {
        //        this.Cursor = Cursors.WaitCursor;
        //        //i.e. in the edit mode
        //        if (splitContainer1.Panel2.Controls.Count > 0)
        //        {
        //            if (splitContainer1.Panel2.Controls[0].GetType() == typeof(EditObjectModel))
        //            {
        //                //using (EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel)
        //                //{
        //                EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel;
        //                BaseControl ctl = SerializeAndDeserialize.Deserialize(view.ElementDetails.Trim(), selectedOM) as BaseControl;
        //                if (ctl.GetType() == typeof(WindowControl))
        //                {
        //                    WindowControl item = ctl as WindowControl;
        //                    item.AutomationId = view.Id;
        //                    item.Name = view.ElementName;
        //                    item.LocalizedControlType = view.Type;
        //                    item.ApplicationTreePath = view.AppTreePath;
        //                    //first over write the object in the main desktop element to be persisted later
        //                    ReplaceUIElementInApplication((trAppControl.SelectedNode.Tag as UIElement).ElementAttributes, item);
        //                    (trAppControl.SelectedNode.Tag as UIElement).ElementAttributes = item;
        //                }
        //                else if (ctl.GetType() == typeof(JavaControl))
        //                {
        //                    JavaControl item = ctl as JavaControl;
        //                    item.Id = view.Id;
        //                    item.Name = view.ElementName;
        //                    item.Role = view.Type;
        //                    item.ApplicationTreePath = view.AppTreePath;
        //                    //first over write the object in the main desktop element to be persisted later
        //                    ReplaceUIElementInApplication((trAppControl.SelectedNode.Tag as UIElement).ElementAttributes, item);
        //                    (trAppControl.SelectedNode.Tag as UIElement).ElementAttributes = item;
        //                }

        //                //persist the changes
        //                string desktopXml = SerializeAndDeserialize.Serialize(desktopElement);
        //                System.IO.File.WriteAllText(selectedOMPath, desktopXml);
        //                //}
        //            }
        //        }



        //        this.Cursor = Cursors.Default;
        //    }
        //}

        private void GenerateCode(string path)
        {
            //todo
            //re-factor
            List<Infosys.ATR.UIAutomation.Entities.ControlElement> ctrlElements =
                new List<UIAutomation.Entities.ControlElement>();

            Dictionary<string, string> configElements = new Dictionary<string, string>();

            desktopElement.Applications.ForEach(a => a.Children.ForEach(c => c.Children.ForEach(cl =>
            {
                Infosys.ATR.UIAutomation.Entities.ControlElement ctrlElement = new UIAutomation.Entities.ControlElement();
                WindowControl ctrlAttr = cl.ElementAttributes as WindowControl;
                var ctrlName = ctrlAttr.LocalizedControlType + "_" + ctrlAttr.AutomationId;
                ctrlElement.ControlName = ctrlName;
                ctrlElement.ControlType = ctrlAttr.ControlType.Split('.')[1];
                ctrlElement.Name = Path.GetFileName(cl.HierarchicalImageLoc);
                configElements.Add(ctrlName, Path.GetDirectoryName(cl.HierarchicalImageLoc));
                ctrlElements.Add(ctrlElement);
            })));

            string className = desktopElement.Applications[0].DisplayText.Split('-')[1];
            if (className.Contains("http"))
            {
                var t = className.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(t[1]))
                {
                    if (t[1].Contains("/"))
                    {
                        var t1 = t[1].Split('/');
                        className = t1[1];
                    }
                }
            }
            //string code = CodeGenerator.Generate(className, ctrlElements);
            //System.IO.File.WriteAllText(path + @"\\" + className + ".py", code);
            //string config = CodeGenerator.GenerateConfig(configElements);
            string config = CodeGenerator.GenerateConfig(desktopElement);
            System.IO.File.WriteAllText(path + @"\\" + className + ".config", config);
            //  config = CodeGenerator.GenerateJsonConfig(desktopElement);
            System.IO.File.WriteAllText(path + @"\\" + className + ".json", config);
        }

        //private void toolStripButtonEditOM_Click(object sender, EventArgs e)
        //{
        //    if (!menuStripDisabledForEdit)
        //    {

        //        openFileDialog1.Filter = "Object Model|*.xml";
        //        if (openFileDialog1.ShowDialog() == DialogResult.OK)
        //        {
        //            //this.Cursor = Cursors.WaitCursor;
        //           // menuStripDisabledForEdit = true;
        //         //   toolStripButtonEditOM.Text = "Go back to the Control Lookup Mode";
        //            //disable all the tool stip item except this
        //            //foreach (ToolStripButton item in toolStrip1.Items)
        //            //{
        //            //    if (item == toolStripButtonEditOM || item == toolStripButtonTakeSnap || item == toolStripButton_SaveOM)
        //            //        item.Enabled = true;
        //            //    else
        //            //        item.Enabled = false;
        //            //}
        //            //stop tracking if already on
        //            if (capturingOnMOve)
        //            {
        //                toolStripbtnCtlCaptureOnMove_Click(null, null);
        //            }
        //            if (capturingOnClick)
        //            {
        //                toolStripbtnCtrlCapture_Click(null, null);
        //            }

        //            ControlEditor edit = new ControlEditor();
        //            edit.ObjectModel = openFileDialog1.FileName;
        //            edit.Render();

        //            //open the object model

        //            //selectedOMPath = openFileDialog1.FileName;
        //            //string xml = System.IO.File.ReadAllText(selectedOMPath);
        //            //desktopElement = SerializeAndDeserialize.Deserialize(xml, typeof(Desktop)) as Desktop;
        //            //PopulateTreeWithObjectModel();
        //            //this.Cursor = Cursors.Default;
        //        }
        //    }
        //    else
        //    {
        //        menuStripDisabledForEdit = false;
        //        RefreshControlLookUp();
        //        toolStripButtonEditOM.Text = "Edit an Object Model";
        //    }
        //}

        private List<int> GetAllChildProcesses(int parentProcessId)
        {
            List<int> childProcessIds = new List<int>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", parentProcessId));
            foreach (ManagementObject mo in mos.Get())
            {
                childProcessIds.Add(Convert.ToInt32(mo["ProcessID"]));
            }
            return childProcessIds;
        }

        private int GetControlLevelInAppTree(string appTreePath)
        {
            //e.g. of appTreePath - /0[0]/1[3], control level is 1
            string[] appTreePathParts = appTreePath.Split('/');
            string controlAppTreePathPart = appTreePathParts[appTreePathParts.Length - 1]; //e.g. 1[3]
            int controlLevel = int.Parse(controlAppTreePathPart.Split('[')[0]); //e.g. 1
            return controlLevel;
        }

        private void WatchElementUnderMouse()
        {
            while (SHIFTisBeingPressed)
            {
                System.Drawing.Point mouse = System.Windows.Forms.Cursor.Position;
                AutomationElement element = ElementFromCursor(mouse.X, mouse.Y);
                Thread.Sleep(1000);
            }
        }

        private string GetProcessName(int processId)
        {
            return Process.GetProcessById(processId).ProcessName;
        }

        private string GetProcessName(AutomationElement element)
        {
            return Process.GetProcessById(element.Current.ProcessId).ProcessName;
        }

        private void PopulateNodeImageList()
        {
            string elementTypes = ConfigurationManager.AppSettings["UIElementTypes"];
            string elementIcons = ConfigurationManager.AppSettings["UIElementIcons"];
            if (!string.IsNullOrEmpty(elementTypes) && !string.IsNullOrEmpty(elementIcons))
            {
                string[] elementTypeParts = elementTypes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                string[] elementIconParts = elementIcons.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (elementTypeParts.Length != elementIconParts.Length)
                {
                    MessageBox.Show("There is mismatch in the count of Element Types and the corresponding Icons. Please refer to the application settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                for (int i = 0; i < elementTypeParts.Length; i++)
                {
                    if (!elementImageMappings.ContainsKey(elementTypeParts[i]))
                    {
                        string iconPath = @"images\" + elementIconParts[i];
                        if (System.IO.File.Exists(iconPath))
                        {
                            //update mappings
                            elementImageMappings.Add(elementTypeParts[i], elementIconParts[i]);
                            //then populate image list
                            System.Drawing.Image icon = System.Drawing.Image.FromFile(@"images\" + elementIconParts[i]);
                            imageList1.Images.Add(elementTypeParts[i], icon);
                        }
                    }
                }
            }
        }

        private void ChangeCursor(string curFile, bool forceCursorFile = false)
        {
            try
            {
                if (forceCursorFile || System.IO.File.Exists(curFile))
                {
                    Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Cursors\", "Arrow", curFile);
                    SystemParametersInfo(SPI_SETCURSORS, 0, 0, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Don’t show message box because it may be irritating in the scenario when user is not an admin and can’t change the registry
            }
        }

        private void trAppControl_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked && e.Node.Parent != null)
                e.Node.Parent.Checked = true;
        }

        private void trAppControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X + 5, e.Y + 5);
                TreeNode node = trAppControl.GetNodeAt(p);
                if (node != null)
                {
                    trAppControl.SelectedNode = node;
                    menu.Show(trAppControl, p);
                }
            }
        }

        public string TakeSnap(int x, int y, int height, int width, int appProcessId, bool saveSnap = false, string snapLocation = "")
        {
            if (appProcessId > 0)
            {
                //get the concerned application at the top
                Process proc = Process.GetProcessById(appProcessId);
                IntPtr handle = proc.MainWindowHandle;
                Structures.RECT rect = new Structures.RECT();
                bool result = GetWindowRect(handle, ref rect);
                if (result)
                {
                    //first show the window in case minimized
                    //the below method shows the minimized(to task bar) app but restores it to the default state,
                    //not to the state before minimize
                    //ShowWindowAsync(handle, SW_RESTORE);


                    //hack needs fix
                    var parent = this.Parent.Parent.Parent.Parent.Parent.Parent.Parent;
                    ShowWindow(parent.Handle, 2);
                    //  ShowWindow(handle, 3);
                    SetForegroundWindowNative(handle);
                    Thread.Sleep(100);
                }
                Bitmap snap = new Bitmap(width, height);
                using (var g = Graphics.FromImage(snap))
                    g.CopyFromScreen(new Point(x, y), Point.Empty, new Size(width, height));
                //if (snap != null)
                //{
                //    if (splitContainer1.Panel2.Controls.Count > 0)
                //    {
                //        if (splitContainer1.Panel2.Controls[0].GetType() == typeof(ViewObjectModel))
                //        {
                //            ViewObjectModel view = splitContainer1.Panel2.Controls[0] as ViewObjectModel;
                //            view.ElementImage = snap;
                //        }
                //        else if (splitContainer1.Panel2.Controls[0].GetType() == typeof(EditObjectModel))
                //        {
                //            EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel;
                //            view.ElementImage = snap;
                //        }
                //    }
                //}

                //set the control lookup back to the top
                //proc = Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
                //handle = proc.MainWindowHandle;
                //SetForegroundWindowNative(handle);

                if (SaveImageHandler != null)
                {
                    SaveImageHandler(snap);
                }

                //save the snap
                //if (saveSnap && !string.IsNullOrEmpty(snapLocation))
                //{
                //    if (!System.IO.File.Exists(snapLocation + ".jpg"))
                //        snap.Save(snapLocation + ".jpg", ImageFormat.Jpeg);
                //    else
                //    {
                //        snapLocation += Guid.NewGuid();
                //        snap.Save(snapLocation + ".jpg", ImageFormat.Jpeg);
                //    }
                //}
            }
            return snapLocation;
        }

        private int GetProcessIdForNode(TreeNode node)
        {
            int processId = 0;
            while (node.Tag.GetType() != typeof(AutomationElement))
            {
                node = node.Parent;
            }
            processId = (node.Tag as AutomationElement).Current.ProcessId;
            return processId;
        }

        private void SaveAppObjectModel(TreeNode appNode, string destination)
        {
            destination = destination + "\\" + CreateFileFolderName(appNode.Text, 30);
            if (!System.IO.Directory.Exists(destination))
                System.IO.Directory.CreateDirectory(destination);
            //take the snap of the application
            System.Windows.Rect rect = (appNode.Tag as AutomationElement).Current.BoundingRectangle;
            string filename = CreateFileFolderName(appNode.Text);
            string finalFilename = TakeSnap((int)rect.X, (int)rect.Y, (int)rect.Height, (int)rect.Width, GetProcessIdForNode(appNode), true, destination + "\\" + filename);

            UIElement app = new UIElement() { DisplayText = appNode.Text };
            app.ElementAttributes = GetWindowControl(appNode);
            app.ElementImageFile = finalFilename + ".jpg";
            app.Children = new List<UIElement>();
            app.Images = new CtrlImage();
            app.Images.Center = app.ElementImageFile;

            string path = "";
            if (copyImageToRespectiveFolders)
            {
                string[] fileNameParts = finalFilename.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (fileNameParts.Length > 0)
                {
                    path = destination + "\\" + CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    System.IO.File.Copy(app.ElementImageFile, path + "\\" + CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10) + ".jpg");
                    app.HierarchicalImageLoc = path + "\\" + CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10) + ".jpg";
                    //write the corresponding class file
                    //the lower part of the code is commented because, even if in case of manuall selection,
                    //the class file for the application needs to be created.
                    //if (createFileForManualSelection)
                    //{
                    //    if (manuallySelectedNodes.Contains(appNode.Tag))
                    //        ReflectClassesInObjectModel(app, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), path);
                    //}
                    //else
                    ReflectClassesInObjectModel(app, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), path);
                }
            }
            else
            {
                //write the corresponding class file
                string[] fileNameParts = finalFilename.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                //the lower part of the code is commented because, even if in case of manuall selection,
                //the class file for the application needs to be created.
                //if (createFileForManualSelection)
                //{
                //    if (manuallySelectedNodes.Contains(appNode.Tag))
                ReflectClassesInObjectModel(app, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), destination + "\\classes");
                //}
                //else
                ReflectClassesInObjectModel(app, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), destination + "\\classes");
                //ReflectClassesInObjectModel(app, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), destination + "\\classes");
            }

            foreach (TreeNode ctlNode in appNode.Nodes)
            {
                if (ctlNode.Checked)
                    SaveCtlObjectModel(ctlNode, destination, app, path);
            }

            if (shell.Mode != "Web" ||
           desktopElement.Applications.Count == 0)
            {
                desktopElement.Applications.Add(app);
            }
            else
            {

                desktopElement.Applications[0].Children.AddRange(app.Children);
            }

            //desktopElement.Applications.Add(app);
        }

        private void SaveCtlObjectModel(TreeNode ctlNode, string destination, UIElement parent, string parentFolder = "")
        {
            string filename = CreateFileFolderName(ctlNode.Text);
            UIElement childUIElement = new UIElement() { DisplayText = ctlNode.Text };
            childUIElement.Children = new List<UIElement>();
            string finalFilename = "";
            if (ctlNode.Tag.GetType() == typeof(AutomationElement))
            {
                //TakeSnap((selectedNode.Tag as AutomationElement).Current.BoundingRectangle);
                System.Windows.Rect rect = (ctlNode.Tag as AutomationElement).Current.BoundingRectangle;
                finalFilename = TakeSnap((int)rect.X, (int)rect.Y, (int)rect.Height, (int)rect.Width, GetProcessIdForNode(ctlNode), true, destination + "\\" + filename);
                childUIElement.ElementAttributes = GetWindowControl(ctlNode);
            }
            else if (ctlNode.Tag.GetType() == typeof(EX_JABHelper.AccessibleTreeItem))
            {
                Rectangle rect = (ctlNode.Tag as EX_JABHelper.AccessibleTreeItem).Bounds;
                finalFilename = TakeSnap(rect.X, rect.Y, rect.Height, rect.Width, GetProcessIdForNode(ctlNode), true, destination + "\\" + filename);
                childUIElement.ElementAttributes = GetJavaControl(ctlNode);
            }
            childUIElement.ElementImageFile = finalFilename + ".jpg";
            childUIElement.Images = new CtrlImage();
            childUIElement.Images.Center = finalFilename + ".jpg";

            string path = "";
            if (!string.IsNullOrEmpty(parentFolder))
            {
                string[] fileNameParts = finalFilename.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (fileNameParts.Length > 0)
                {
                    path = parentFolder + "\\" + CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10);
                    if (!System.IO.Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    System.IO.File.Copy(childUIElement.ElementImageFile, path + "\\" + CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 15) + ".jpg");
                    childUIElement.HierarchicalImageLoc = path + "\\" + CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10) + ".jpg";
                    //write the corresponding class file
                    if (createFileForManualSelection)
                    {
                        if (manuallySelectedNodes.Contains(ctlNode.Tag))
                            ReflectClassesInObjectModel(childUIElement, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), path);
                    }
                    else
                        ReflectClassesInObjectModel(childUIElement, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), path);
                    //ReflectClassesInObjectModel(childUIElement, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), path);
                }
            }
            else
            {
                //write the corresponding class file
                string[] fileNameParts = finalFilename.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (createFileForManualSelection)
                {
                    if (manuallySelectedNodes.Contains(ctlNode.Tag))
                        ReflectClassesInObjectModel(childUIElement, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), destination + "\\classes");
                }
                else
                    ReflectClassesInObjectModel(childUIElement, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), destination + "\\classes");
                //ReflectClassesInObjectModel(childUIElement, CreateFileFolderName(fileNameParts[fileNameParts.Length - 1], 10), destination + "\\classes");
            }

            foreach (TreeNode child in ctlNode.Nodes)
            {
                if (child.Checked)
                    SaveCtlObjectModel(child, destination, childUIElement, path);
            }
            parent.Children.Add(childUIElement);
        }

        public static string CreateFileFolderName(string nodeText, int length = 20)
        {
            nodeText = Regex.Replace(nodeText, "[^A-Za-z0-9 _ -]", "").Replace(" ", "");
            if (nodeText.Length > length)
                nodeText = nodeText.Substring(0, length);
            return nodeText;
        }

        private WindowControl GetWindowControl(TreeNode node)
        {
            AutomationElement element = node.Tag as AutomationElement;
            WindowControl ctl = new WindowControl()
            {
                AccessKey = element.Current.AccessKey,
                Name = element.Current.Name,
                AutomationId = element.Current.AutomationId,
                ControlType = element.Current.ControlType.ProgrammaticName,
                LocalizedControlType = element.Current.LocalizedControlType,
                UIFramework = element.Current.FrameworkId,
                BoundingRectangle = element.Current.BoundingRectangle.ToString(),
                WindowHandle = element.Current.NativeWindowHandle.ToString(),
                ClassName = element.Current.ClassName,
                ProcessId = element.Current.ProcessId.ToString()
            };
            foreach (AutomationPattern pattern in element.GetSupportedPatterns())
            {
                ctl.ControlPatterns += pattern.ProgrammaticName + ",";
            }
            string[] nodeIdParts = node.Name.Split('#');
            if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
            {
                ctl.ApplicationTreePath = treeNodeAndTreePathMappings[nodeIdParts[1]];
            }
            return ctl;
        }

        private JavaControl GetJavaControl(TreeNode node)
        {
            EX_JABHelper.AccessibleTreeItem element = node.Tag as EX_JABHelper.AccessibleTreeItem;
            JavaControl ctl = new JavaControl()
            {
                Name = element.name,
                Role = element.role,
                States = element.states,
                Framework = processName,
                AccessibleActions = element.accessibleActions,
                BoundingRectangle = element.Bounds.ToString(),
                Description = element.description,
                ElementPointer = element.ElementPointer.ToString(),
                TextValue = element.textValue
            };
            string[] nodeIdParts = node.Name.Split('#');
            if (nodeIdParts.Length == 2 && treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
            {
                ctl.ApplicationTreePath = treeNodeAndTreePathMappings[nodeIdParts[1]];
            }
            return ctl;
        }

        private void RefreshControlLookUp()
        {
            //toolTip1.SetToolTip(btnOk, "Use the control highlighted");
            //toolTip1.SetToolTip(btnCtrlCapture, "Start capturing the control details from application. Press 'CTRL + Mouse left key' on the control");
            //toolTip1.SetToolTip(btnRefresh, "Refresh the control explorer");
            //toolTip1.SetToolTip(pictureBox1, "Double Click to view in right proportion");

            //enable all the toll strip items
            //foreach (ToolStripButton item in toolStrip1.Items)
            //    item.Enabled = true;

            //refresh the tree
            trAppControl.Nodes.Clear();
            // splitContainer1.Panel2Collapsed = true;
            trAppControl.CheckBoxes = true;

            //currently hide the interface to capture control details on shift+ mouse click
            //   toolStripbtnCtrlCapture.Visible = false;

            //just show the root desktop
            AutomationElement desktop = AutomationElement.RootElement;
            TreeNode rootNode = new TreeNode("desktop- " + Environment.MachineName);
            //TreeNode rootNode = new TreeNode(desktop.Current.LocalizedControlType + "- Desktop : " + Environment.MachineName);
            rootNode.Name = Environment.MachineName;
            rootNode.Tag = desktop;
            if (elementImageMappings.ContainsKey("desktop"))
            {
                rootNode.ImageKey = "desktop";
                rootNode.SelectedImageKey = "desktop";
            }
            else
                rootNode.ImageIndex = 0;
            rootNode.Nodes.Add("Please wait...");
            trAppControl.Nodes.Add(rootNode);
            trAppControl.SelectedNode = null;

            //start Java Access Bridge
            JABHelper.Windows_run();

            //get the current cursor file
            currentCursor = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Cursors\", "Arrow", "") as string;

            //add context menu to the tree node    
            menu.Items.Clear();
            trAppControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trAppControl_MouseUp);
            ToolStripMenuItem snapItem = new ToolStripMenuItem("Take Control Snap");
            snapItem.Image = imageList2.Images["camera.png"];
            snapItem.Click += new EventHandler(snapItem_Click);
            snapItem.ToolTipText = "The concerned Application could be at the back but must NOT be minimized to task bar.";
            menu.Items.Add(snapItem);
            menu.ImageScalingSize = new System.Drawing.Size(24, 24);

            //assign the objectModel path
            objectModelPath = ConfigurationManager.AppSettings["ObjectModelPath"];

            //also expand the root
            trAppControl.Nodes[0].Expand();
        }

        private void PopulateTreeWithObjectModel()
        {
            if (desktopElement != null)
            {
                //refresh the tree
                trAppControl.Nodes.Clear();
                // splitContainer1.Panel2Collapsed = true;
                trAppControl.CheckBoxes = false;
                //dont show the context menu which is added on mouse up event
                trAppControl.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.trAppControl_MouseUp);

                TreeNode rootNode = new TreeNode("desktop");
                rootNode.Tag = desktopElement;
                if (elementImageMappings.ContainsKey("desktop"))
                {
                    rootNode.ImageKey = "desktop";
                    rootNode.SelectedImageKey = "desktop";
                }
                foreach (UIElement application in desktopElement.Applications)
                {
                    TreeNode childNode = new TreeNode(application.DisplayText);
                    childNode.Tag = application;
                    childNode.Nodes.Add("Please wait...");
                    //add node image
                    string imageKey = application.DisplayText.ToLower().Split('-')[0].Trim();
                    if (elementImageMappings.ContainsKey(imageKey))
                    {
                        childNode.ImageKey = imageKey;
                        childNode.SelectedImageKey = imageKey;
                    }
                    rootNode.Nodes.Add(childNode);
                }
                trAppControl.Nodes.Add(rootNode);
            }
        }

        private void toolStripButtonTakeSnap_Click(object sender, EventArgs e)
        {
            Snapper snap = new Snapper();
            snap.FilePaths = new List<string> { currentObjectImagePath, folderHierarchicalImagePath };
            snap.SaveAndClose = true;
            snap.Show();
        }

        private void ReplaceUIElementInApplication(BaseControl oldChildElement, BaseControl newChildElement)
        {
            for (int i = 0; i < desktopElement.Applications.Count; i++)
            {
                if (desktopElement.Applications[i].ElementAttributes == oldChildElement)
                {
                    desktopElement.Applications[i].ElementAttributes = newChildElement;
                    break;
                }
                else
                {
                    bool found = ReplaceUIElement(oldChildElement, newChildElement, desktopElement.Applications[i]);
                    if (found)
                        break;
                }
            }
        }

        private bool ReplaceUIElement(BaseControl oldChildElement, BaseControl newChildElement, UIElement parentElement)
        {
            bool found = false;
            for (int i = 0; i < parentElement.Children.Count; i++)
            {
                if (parentElement.Children[i].ElementAttributes == oldChildElement)
                {
                    parentElement.Children[i].ElementAttributes = newChildElement;
                    found = true;
                    break;
                }
                else if (parentElement.Children[i].Children.Count > 0)
                {
                    found = ReplaceUIElement(oldChildElement, newChildElement, parentElement.Children[i]);
                    if (found)
                        break;
                }

            }
            return found;
        }

        private void ShowObjectModelView(ObjectModelViewMode mode)
        {
            //splitContainer1.Panel2.Controls.Clear();
            //switch (mode)
            //{
            //    case ObjectModelViewMode.ViewMode:
            //ViewObjectModel view1 = new ViewObjectModel();
            //view1.Dock = DockStyle.Fill;
            //splitContainer1.Panel2.Controls.Add(view1);
            //        break;
            //    case ObjectModelViewMode.EditMode:
            //        EditObjectModel view2 = new EditObjectModel();
            //        view2.Dock = DockStyle.Fill;
            //        splitContainer1.Panel2.Controls.Add(view2);
            //        break;
            //}
        }

        public static void ReflectClassesInObjectModel(object obj, string className, string classFileLoc)
        {
            string codeForLang = ConfigurationManager.AppSettings["CodeForLanguage"];
            if (!string.IsNullOrEmpty(codeForLang))
            {
                if (obj != null)
                {
                    List<PropertyDef> propertyDefs = new List<PropertyDef>();
                    propertyDefs.AddRange(GetAllPremitiveProperties(obj));

                    string strClass = "";
                    switch (codeForLang.ToLower())
                    {
                        case "csharp":
                            strClass = CSharpCodeGenerator.Generate("Infosys.ATR", className.Replace('-', '_'), propertyDefs);
                            break;
                    }
                    if (!string.IsNullOrEmpty(strClass))
                    {
                        if (!System.IO.Directory.Exists(classFileLoc))
                            System.IO.Directory.CreateDirectory(classFileLoc);
                        System.IO.File.WriteAllText(classFileLoc + "\\" + className + ".cs", strClass);
                    }
                }
            }
        }

        private static List<PropertyDef> GetAllPremitiveProperties(object obj)
        {
            List<PropertyDef> propertyDefs = new List<PropertyDef>();
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                //check if the property type is a system defined type 
                if (!prop.PropertyType.Namespace.ToLower().Contains("system"))
                    propertyDefs.AddRange(GetAllPremitiveProperties(prop.GetValue(obj, null)));
                else
                {
                    PropertyDef propertyDef = new PropertyDef();
                    propertyDef.PropertyName = prop.Name;
                    propertyDef.PropertyType = prop.PropertyType;
                    propertyDef.Comments = "Holds the information regarding- " + prop.Name;
                    if (prop.PropertyType.IsArray)
                    {
                        propertyDef.IsCollection = true;
                        propertyDef.PropertyType = typeof(object);
                    }
                    else if (prop.PropertyType.IsGenericType && (prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
                    {
                        propertyDef.PropertyType = typeof(List<object>);
                    }
                    propertyDefs.Add(propertyDef);
                }
            }

            return propertyDefs;
        }

        private void trAppControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (createFileForManualSelection && e.Button == MouseButtons.Left)
            {
                Point p = new Point(e.X + 5, e.Y + 5);
                TreeNode node = trAppControl.GetNodeAt(p);
                if (node != null && !manuallySelectedNodes.Contains(node.Tag))
                {
                    manuallySelectedNodes.Add(node.Tag);
                }
            }
        }
    }

    public enum ObjectModelViewMode
    {
        EditMode,
        ViewMode
    }

    public class ProcessMapping
    {
        public int ParentProcessId { get; set; }
        public List<int> ChildProcessIds { get; set; }
    }

    //[Serializable]
    //public class Desktop
    //{
    //    [XmlElement("Application")]
    //    public List<UIElement> Applications = new List<UIElement>();
    //    //[XmlAttribute]
    //    //public string DisplayText { get;set;}
    //    //string _DisplayText;
    //    [XmlAttribute]
    //    public string DisplayText = "desktop";
    //    //{
    //    //    get { return _DisplayText; }
    //    //    set { _DisplayText = "desktop"; }
    //    //}
    //}

    //[Serializable]
    //public class UIElement
    //{
    //    [XmlElement]
    //    public BaseControl ElementAttributes { get; set; }
    //    [XmlAttribute]
    //    public string ElementImageFile { get; set; }
    //    [XmlAttribute]
    //    public string HierarchicalImageLoc { get; set; }
    //    [XmlAttribute]
    //    public string DisplayText { get; set; }
    //    [XmlElement("ChildElement")]
    //    public List<UIElement> Children { get; set; }
    //    [XmlElement]
    //    public CtrlImage Images { get; set; }
    //}

    //[Serializable]
    //public class CtrlImage
    //{
    //    private string _center = "";
    //    private string _right = "";
    //    private string _left = "";
    //    private string _up = "";
    //    private string _down = "";

    //    [XmlElement]
    //    public string Center 
    //    {
    //        get 
    //        {
    //            return _center;
    //        } 
    //        set{
    //            _center = value;
    //        }
    //    }
    //    [XmlElement]
    //    public string Left { get { return _left; } set { _left = value; } }
    //    [XmlElement]
    //    public string Right { get { return _right; } set { _right = value; } }
    //    [XmlElement]
    //    public string Up { get { return _up; } set { _up = value; } }
    //    [XmlElement]
    //    public string Down { get { return _down; } set { _down = value; } }
    //}

    [Serializable]
    public class JavaControl : BaseControl
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Role { get; set; }
        public string States { get; set; }
        public string Framework { get; set; }
        public string AccessibleActions { get; set; }
        public string BoundingRectangle { get; set; }
        public string Description { get; set; }
        public string ElementPointer { get; set; }
        public string TextValue { get; set; }
        public string ApplicationTreePath { get; set; }
    }


    //public class BaseControl
    //{ }

    public static class SerializeAndDeserialize
    {
        public static string Serialize(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), new Type[] { typeof(WindowControl), typeof(JavaControl), typeof(UIElement) });
            Utf8StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }

        public static object Deserialize(string xmlObj, Type type)
        {
            StringReader stringReader = new StringReader(xmlObj);
            XmlSerializer serializer = new XmlSerializer(type, new Type[] { typeof(WindowControl), typeof(JavaControl), typeof(UIElement) });
            return serializer.Deserialize(stringReader);
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
    }

    public static class Structures
    {
        [StructLayout(LayoutKind.Sequential)]

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public override bool Equals(object obj)
            {
                return (
                    obj is RECT
                    && ((RECT)obj).left == left
                    && ((RECT)obj).top == top
                    && ((RECT)obj).right == right
                    && ((RECT)obj).bottom == bottom
                    );
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                const string seperator = ",";
                return left + seperator + top + seperator + right + seperator + bottom;
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        public class EventMsg
        {
            public int message;
            public int paramL;
            public int paramH;
            public int time;
            public int hwnd;
        }

    }
}


