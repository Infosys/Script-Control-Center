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
using System.Reflection;
using Infosys.ATR.UIAutomation.Entities;

namespace Infosys.ATR.DevelopmentStudio
{
    public partial class uc_ControlEditor : UserControl
    {
       
    
        Desktop _desktop;
        string controlId = ""; 
        string processId;
        string folderHierarchicalImagePath = "";
        string currentObjectImagePath;
        string processName = "";
        TreeNode lastSelectedNode = null;
        Type selectedOM;
        bool isRootNode;
        string javaProcessId;
        bool javaAppTreeCreated;
        bool createFileForManualSelection;
        bool htmlLoaded;
        bool SHIFTisBeingPressed = false;
        bool capturingOnMove = false;
        string imagePath;
        string objectModelPath;

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Structures.RECT lpRect);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern IntPtr SetForegroundWindowNative(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        
         public uc_ControlEditor()
        {
            InitializeComponent();
             splitContainer1.Panel2Collapsed = true;
             //PopulateTreeWithObjectModel();
        }

        public Desktop Desktop
         {
            set
             {
                 _desktop = value;
             }
         }

        public string ObjectModel
        {
            set
            {
                string xml = System.IO.File.ReadAllText(value);                
                _desktop = SerializeAndDeserialize.Deserialize(xml, typeof(Desktop)) as Desktop;
                imagePath = Path.GetDirectoryName(_desktop.Applications[0].ElementImageFile);
                objectModelPath = Path.GetDirectoryName(value);
            }
        }

       

        public void Render()
        {
            PopulateTreeWithObjectModel();
            this.Show();
        }

        public void Reload()
        {
            PopulateTreeWithObjectModel();
        }

        private void PopulateTreeWithObjectModel()
        {
            var images = ControlExplorer.elementImageMappings;
            if (_desktop != null)
            {
                //refresh the tree
                trAppControl.Nodes.Clear();
                splitContainer1.Panel2Collapsed = true;
                trAppControl.CheckBoxes = false;

                TreeNode rootNode = new TreeNode("desktop");
                rootNode.Tag = _desktop;
                if (images.ContainsKey("desktop"))
                {
                    rootNode.ImageKey = "desktop";
                    rootNode.SelectedImageKey = "desktop";
                }
                foreach (UIElement application in _desktop.Applications)
                {
                    TreeNode childNode = new TreeNode(application.DisplayText);
                    childNode.Tag = application;
                    childNode.Nodes.Add("Please wait...");
                    //add node image
                    string imageKey = application.DisplayText.ToLower().Split('-')[0].Trim();
                    if (images.ContainsKey(imageKey))
                    {
                        childNode.ImageKey = imageKey;
                        childNode.SelectedImageKey = imageKey;
                    }
                    rootNode.Nodes.Add(childNode);
                }
                trAppControl.Nodes.Add(rootNode);
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
                    splitContainer1.Panel2Collapsed = false;
                    PopulateControlDetails(e.Node);
                }
                else if (e.Node.Tag.GetType() == typeof(EX_JABHelper.AccessibleTreeItem))
                {
                    controlId = e.Node.Name.Split('#')[0];
                    //processName = "java";
                    splitContainer1.Panel2Collapsed = false;
                    PopulateJavaControlDetails(e.Node);
                }
                else if (e.Node.Tag.GetType() == typeof(UIElement))
                {
                    splitContainer1.Panel2Collapsed = false;
                    PopulateObjectModel(e.Node);
                }
                else
                {
                    splitContainer1.Panel2Collapsed = true;
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


        private void PopulateControlDetails(TreeNode node)
        {
            ShowObjectModelView(ObjectModelViewMode.ViewMode);

            if (splitContainer1.Panel2.Controls.Count > 0)
            {
                if (splitContainer1.Panel2.Controls[0].GetType() == typeof(ViewObjectModel))
                {
                    ViewObjectModel view = splitContainer1.Panel2.Controls[0] as ViewObjectModel;
                    AutomationElement element = node.Tag as AutomationElement;
                    view.ElementDetails = "Name- " + element.Current.Name + Environment.NewLine;
                    view.ElementDetails += "Automation Id- " + element.Current.AutomationId + Environment.NewLine;
                    view.ElementDetails += "Control Type- " + element.Current.ControlType.ProgrammaticName + Environment.NewLine;
                    view.ElementDetails += "Localized Control Type- " + element.Current.LocalizedControlType + Environment.NewLine;
                    view.ElementDetails += "Access Key- " + element.Current.AccessKey + Environment.NewLine;
                    view.ElementDetails += "UI Framework- " + element.Current.FrameworkId + Environment.NewLine;
                    view.ElementDetails += "Bounding Rectangle- " + element.Current.BoundingRectangle.ToString() + Environment.NewLine;
                    view.ElementDetails += "Window Handle- " + (IntPtr)element.Current.NativeWindowHandle + Environment.NewLine;
                    view.ElementDetails += "Class Name- " + element.Current.ClassName + Environment.NewLine;
                    view.ElementDetails += "Process Id- " + element.Current.ProcessId + Environment.NewLine;
                    view.ElementDetails += "Control Pattern(s)- ";
                    foreach (AutomationPattern pattern in element.GetSupportedPatterns())
                    {
                        view.ElementDetails += pattern.ProgrammaticName + ",";
                    }
                    string[] nodeIdParts = node.Name.Split('#');
                    if (nodeIdParts.Length == 2 && ControlExplorer.treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
                    {
                        view.ElementDetails += Environment.NewLine + "Pointer in Application Tree Path (Level[index])-" + ControlExplorer.treeNodeAndTreePathMappings[nodeIdParts[1]];
                    }
                }
            }
        }

        private void PopulateJavaControlDetails(TreeNode node)
        {
            ShowObjectModelView(ObjectModelViewMode.ViewMode);

            if (splitContainer1.Panel2.Controls.Count > 0)
            {
                if (splitContainer1.Panel2.Controls[0].GetType() == typeof(ViewObjectModel))
                {
                    ViewObjectModel view = splitContainer1.Panel2.Controls[0] as ViewObjectModel;
                    EX_JABHelper.AccessibleTreeItem element = node.Tag as EX_JABHelper.AccessibleTreeItem;
                    view.ElementDetails = "Name- " + element.name + Environment.NewLine;
                    view.ElementDetails += "Role- " + element.role + Environment.NewLine;
                    view.ElementDetails += "State(s)- " + element.states + Environment.NewLine;
                    view.ElementDetails += "Framework- " + processName + Environment.NewLine;
                    view.ElementDetails += "Accessible Action(s)- " + element.accessibleActions + Environment.NewLine;
                    view.ElementDetails += "Bounding Rectangle- " + element.Bounds.ToString() + Environment.NewLine;
                    view.ElementDetails += "Description- " + element.description + Environment.NewLine;
                    view.ElementDetails += "Element Pointer- " + element.ElementPointer.ToString() + Environment.NewLine;
                    view.ElementDetails += "Text Value (if any)- " + element.textValue;
                    string[] nodeIdParts = node.Name.Split('#');
                    if (nodeIdParts.Length == 2 && ControlExplorer.treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
                    {
                        view.ElementDetails += Environment.NewLine + "Pointer in Application Tree Path (Level[index])-" + ControlExplorer.treeNodeAndTreePathMappings[nodeIdParts[1]];
                    }
                }
            }
        }

        private void PopulateObjectModel(TreeNode node)
        {
            ShowObjectModelView(ObjectModelViewMode.EditMode);

            if (splitContainer1.Panel2.Controls.Count > 0)
            {
                if (splitContainer1.Panel2.Controls[0].GetType() == typeof(EditObjectModel))
                {
                    EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel;
                    view.OnCapture += new CaptureImage(view_OnCapture);
                    UIElement ctl = node.Tag as UIElement;
                    currentObjectImagePath = ctl.ElementImageFile;
                    folderHierarchicalImagePath = ctl.HierarchicalImageLoc;
                    selectedOM = ctl.ElementAttributes.GetType();
                    view.ElementDetails = SerializeAndDeserialize.Serialize(ctl.ElementAttributes);
                    if (ctl.ElementAttributes.GetType() == typeof(WindowControl))
                    {
                        WindowControl item = ctl.ElementAttributes as WindowControl;
                        view.AssignIdentifiers(item.AutomationId, item.Name, item.LocalizedControlType, item.ApplicationTreePath);
                    }
                    else if (ctl.ElementAttributes.GetType() == typeof(JavaControl))
                    {
                        JavaControl item = ctl.ElementAttributes as JavaControl;
                        view.AssignIdentifiers(item.Id, item.Name, item.Role, item.ApplicationTreePath);
                    }

                    if (!string.IsNullOrEmpty(ctl.ElementImageFile))
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromFile(ctl.ElementImageFile);
                        view.ElementImage = img;
                    }

                    view.Set("Center", ctl.Images.Center);
                    view.Set("Right", ctl.Images.Right);
                    view.Set("Left", ctl.Images.Left);
                    view.Set("Up", ctl.Images.Up);
                    view.Set("Down", ctl.Images.Down);
                }

            }
        }

        void view_OnCapture(System.Drawing.Image image,string boundary)
        {
            TreeNode selected = trAppControl.SelectedNode;
            UIElement element = selected.Tag as UIElement;
            var temp = _desktop.Applications[0].Children[0].Children.First(c => c.DisplayText == element.DisplayText);
            var filename = Path.GetFileNameWithoutExtension(temp.Images.Center);
            var directory = Path.GetDirectoryName(temp.Images.Center);
            string t;
            switch(boundary)
            {
                    
                case "Center":
                    File.Delete(temp.Images.Center);
                    image.Save(temp.Images.Center);
                    break;
                case "Left":
                    if (String.IsNullOrEmpty(temp.Images.Left))
                    {
                        t = Path.Combine(directory, filename + "_left.jpg");
                        image.Save(t);
                        temp.Images.Left = t;
                    }
                    else
                    {
                        File.Delete(temp.Images.Left);
                        image.Save(temp.Images.Left);
                    }
                    break;
                case "Right":
                    if (String.IsNullOrEmpty(temp.Images.Right))
                    {
                        t = Path.Combine(directory, filename + "_right.jpg");
                        image.Save(t);
                        temp.Images.Right = t;
                    }
                    else
                    {
                        File.Delete(temp.Images.Right);
                        image.Save(temp.Images.Right);
                    }
                    break;
                case "Up":
                    if (String.IsNullOrEmpty(temp.Images.Up))
                    {
                        t = Path.Combine(directory, filename + "_up.jpg");
                        image.Save(t);
                        temp.Images.Up = t;
                    }
                    else
                    {
                        File.Delete(temp.Images.Up);
                        image.Save(temp.Images.Up);
                    }
                    break;
                case "Down":
                    if (String.IsNullOrEmpty(temp.Images.Down))
                    {
                        t = Path.Combine(directory, filename + "_down.jpg");
                        image.Save(t);
                        temp.Images.Down = t;
                    }
                    else
                    {
                        File.Delete(temp.Images.Down);
                        image.Save(temp.Images.Down);
                    }
                    break;
            }
            Save();
            
        }

        private void ShowObjectModelView(ObjectModelViewMode mode)
        {
            splitContainer1.Panel2.Controls.Clear();
            switch (mode)
            {
                case ObjectModelViewMode.ViewMode:
                    ViewObjectModel view1 = new ViewObjectModel();
                    view1.Dock = DockStyle.Fill;
                    splitContainer1.Panel2.Controls.Add(view1);
                    break;
                case ObjectModelViewMode.EditMode:
                    EditObjectModel view2 = new EditObjectModel();
                    view2.Dock = DockStyle.Fill;
                    splitContainer1.Panel2.Controls.Add(view2);
                    break;
            }
        }

        private void trAppControl_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked && e.Node.Parent != null)
                e.Node.Parent.Checked = true;
          
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

        private void trAppControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (createFileForManualSelection && e.Button == MouseButtons.Left)
            {
                Point p = new Point(e.X + 5, e.Y + 5);
                TreeNode node = trAppControl.GetNodeAt(p);
                if (node != null && ! ControlExplorer.manuallySelectedNodes.Contains(node.Tag))
                {
                    ControlExplorer.manuallySelectedNodes.Add(node.Tag);
                }
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
                    var children = node.FindAll(TreeScope.Children, Condition.TrueCondition);
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

        private string GetProcessName(AutomationElement element)
        {
            return Process.GetProcessById(element.Current.ProcessId).ProcessName;
        }


        private void AddJavaNodeToTreeView(TreeNode currNode, EX_JABHelper.AccessibleTreeItem child, int peerIndex)
        {
            string nodeId = Guid.NewGuid().ToString();
            if (isRootNode)
            {
                ControlExplorer.treeNodeAndTreePathMappings.Add(nodeId, "/0[0]");
            }
            else
            {
                string[] nodeIdParts = currNode.Name.Split('#');
                if (nodeIdParts.Length == 2 && ControlExplorer.treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
                {
                    int parentLevel = GetControlLevelInAppTree(ControlExplorer.treeNodeAndTreePathMappings[nodeIdParts[1]]);
                    int childLevel = parentLevel + 1;
                    ControlExplorer.treeNodeAndTreePathMappings.Add(nodeId, ControlExplorer.treeNodeAndTreePathMappings[nodeIdParts[1]] + "/" + childLevel + "[" + peerIndex + "]");
                }
            }

            string id = child.name;
            TreeNode node = new TreeNode(child.role + "- " + id);
            node.Name = id + "#" + nodeId;
            node.Tag = child;
            //rootNode.ImageIndex = 0;
            node.Nodes.Add("Please wait...");
            //add node image
            if (ControlExplorer.elementImageMappings.ContainsKey(child.role.ToLower()))
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
                    ControlExplorer.processMappings.Add(new ProcessMapping() { ParentProcessId = child.Current.ProcessId, ChildProcessIds = childProcesses });
                }
                ControlExplorer.treeNodeAndTreePathMappings.Add(nodeId, "/0[0]");
            }
            else
            {
                string[] nodeIdParts = currNode.Name.Split('#');
                if (nodeIdParts.Length == 2 && ControlExplorer.treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
                {
                    int parentLevel = GetControlLevelInAppTree(ControlExplorer.treeNodeAndTreePathMappings[nodeIdParts[1]]);
                    int childLevel = parentLevel + 1;
                    ControlExplorer.treeNodeAndTreePathMappings.Add(nodeId, ControlExplorer.treeNodeAndTreePathMappings[nodeIdParts[1]] + "/" + childLevel + "[" + peerIndex + "]");
                }
            }
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
            if (ControlExplorer.elementImageMappings.ContainsKey(child.Current.LocalizedControlType.ToLower()))
            {
                node.ImageKey = child.Current.LocalizedControlType;
                node.SelectedImageKey = child.Current.LocalizedControlType;
            }

            currNode.Nodes.Add(node);
        }

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

        private void AddObjectModelNodeToTree(TreeNode currNode, UIElement child)
        {
            TreeNode childNode = new TreeNode(child.DisplayText);
            childNode.Tag = child;
            childNode.Nodes.Add("Please wait...");
            //add node image
            string imageKey = child.DisplayText.ToLower().Split('-')[0].Trim();
            if (ControlExplorer.elementImageMappings.ContainsKey(imageKey))
            {
                childNode.ImageKey = imageKey;
                childNode.SelectedImageKey = imageKey;
            }
            currNode.Nodes.Add(childNode);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (!capturingOnMove)
            {
                GlobalEventHandler.KeyDownEvents += new KeyEventHandler(HookManager_KeyDown);
                GlobalEventHandler.KeyUpEvents += new KeyEventHandler(HookManager_KeyUp);
               // HookManager.MouseMove += new MouseEventHandler(HookManager_MouseMove);
                GlobalEventHandler.MouseDownEvents +=new MouseEventHandler(HookManager_MouseDown);
                capturingOnMove = true;
                //toolStripbtnCtlCaptureOnMove.Text = "Stop capturing the control details from application";
                //toolStripbtnCtlCaptureOnMove.Image = imageList2.Images[3];
            }
            else
            {
                GlobalEventHandler.KeyDownEvents -= new KeyEventHandler(HookManager_KeyDown);
                GlobalEventHandler.KeyUpEvents -= new KeyEventHandler(HookManager_KeyUp);
                //HookManager.MouseMove -= new MouseEventHandler(HookManager_MouseMove);
                GlobalEventHandler.MouseDownEvents -= new MouseEventHandler(HookManager_MouseDown);
                capturingOnMove = false;
                //toolStripbtnCtlCaptureOnMove.Text = "Start capturing the control details from application. Press 'SHIFT + move Mouse' on the control";
                //toolStripbtnCtlCaptureOnMove.Image = imageList2.Images[2];
            }
        }

        void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (SHIFTisBeingPressed && e.Button == MouseButtons.Left)
                {
                    AutomationElement element = ElementFromCursor(e.X, e.Y);
                    GlobalEventHandler.KeyDownEvents -= new KeyEventHandler(HookManager_KeyDown);
                    GlobalEventHandler.KeyUpEvents -= new KeyEventHandler(HookManager_KeyUp);
                    GlobalEventHandler.MouseDownEvents -= new MouseEventHandler(HookManager_MouseDown);
                    capturingOnMove = false;
                    ChangeCursor(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\cursors\aero_arrow.cur");

                    //take the snap of the application
                    System.Windows.Rect rect = element.Current.BoundingRectangle;                 
                    string finalFilename = TakeSnap((int)rect.X, (int)rect.Y, (int)rect.Height, (int)rect.Width,
                        element.Current.ProcessId, true, imagePath + "\\" + element.Current.LocalizedControlType + "-" + element.Current.AutomationId);


                    UIElement app = new UIElement();
                    app.ElementAttributes = GetWindowControl(element);
                    app.DisplayText = element.Current.LocalizedControlType + "-" + element.Current.AutomationId;
                    app.ElementImageFile = imagePath + "\\" + element.Current.LocalizedControlType + "-" + element.Current.AutomationId + ".jpg";
                    app.Images = new CtrlImage();
                    app.Images.Center = app.ElementImageFile;
                    //app.ElementImageFile = finalFilename + ".jpg";
                    app.Children = new List<UIElement>();                    
                    _desktop.Applications[0].Children[0].Children.Add(app);
                    ControlExplorer.ReflectClassesInObjectModel(app, app.DisplayText, imagePath);
                    Reload();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured. Reason- " + ex.Message, "OOPS... Caught by an error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    SetForegroundWindowNative(handle);
                }
                Bitmap snap = new Bitmap(width, height);
                using (var g = Graphics.FromImage(snap))
                    g.CopyFromScreen(new Point(x, y), Point.Empty, new Size(width, height));
                if (snap != null)
                {
                    if (splitContainer1.Panel2.Controls.Count > 0)
                    {
                        if (splitContainer1.Panel2.Controls[0].GetType() == typeof(ViewObjectModel))
                        {
                            ViewObjectModel view = splitContainer1.Panel2.Controls[0] as ViewObjectModel;
                            view.ElementImage = snap;
                        }
                        else if (splitContainer1.Panel2.Controls[0].GetType() == typeof(EditObjectModel))
                        {
                            EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel;
                            view.ElementImage = snap;
                        }
                    }
                }

                //set the control lookup back to the top
                proc = Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
                handle = proc.MainWindowHandle;
                SetForegroundWindowNative(handle);

                //save the snap
                if (saveSnap && !string.IsNullOrEmpty(snapLocation))
                {
                    if (!System.IO.File.Exists(snapLocation + ".jpg"))
                        snap.Save(snapLocation + ".jpg", ImageFormat.Jpeg);
                    else
                    {
                        snapLocation += Guid.NewGuid();
                        snap.Save(snapLocation + ".jpg", ImageFormat.Jpeg);
                    }
                }
            }
            return snapLocation;
        }

        private WindowControl GetWindowControl(AutomationElement element)
        {          
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
            
            //TreeNode node = this.trAppControl.SelectedNode;
            //string[] nodeIdParts = node.Name.Split('#');
            //if (nodeIdParts.Length == 2 && ControlExplorer.treeNodeAndTreePathMappings.ContainsKey(nodeIdParts[1]))
            //{
            //    ctl.ApplicationTreePath = ControlExplorer.treeNodeAndTreePathMappings[nodeIdParts[1]];
            //}
            ctl.ApplicationTreePath = "/0[0]/1[0]/2[2]";
            return ctl;
        }

        string currentCursor = "";

        void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(string.Format("Key Up - {0}\n", e.KeyCode));
            if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
            {
                SHIFTisBeingPressed = false;
                Cursor.Current = Cursors.Default;
                ChangeCursor(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\cursors\aero_arrow.cur");
                if (!string.IsNullOrEmpty(currentCursor))
                    ChangeCursor(currentCursor, true);
                else
                    ChangeCursor(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\cursors\aero_arrow.cur");
            }
        }

        void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show(string.Format("Key Down - {0}\n", e.KeyCode));
            if (e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
            {
                SHIFTisBeingPressed = true;
                Cursor.Current = Cursors.Cross;// new Cursor(@"images\lookup.png");
                ChangeCursor(@"images\Gun_Sight.ani");
            }
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);
        const int SPI_SETCURSORS = 0x0057;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDCHANGE = 0x02;

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
                {
                    //  PointElementInTree(element);
                }
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
                   // PointElementInTree(pID, item);
                }
            });
            //changing the apartment state to STA as for certain control like webbrowser (activex) getting error
            //that activex control can be used in a STA only
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return element;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void Save()
        {
            string xml = SerializeAndDeserialize.Serialize(_desktop);
            System.IO.File.WriteAllText(@objectModelPath + "\\ObjectModel.xml", xml);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var text = this.trAppControl.SelectedNode.Text;
            EditObjectModel view = splitContainer1.Panel2.Controls[0] as EditObjectModel;

            if (view == null)
            {
                view = new EditObjectModel();
                view.OnAdd += new AddNode(view_OnAdd);
            }
            view.Add();
        }

        void view_OnAdd(string id, string name, string type, string ctrlPath)
        {

            var parentNode = FindParent(this.trAppControl.SelectedNode, null);

            var parent = _desktop.Applications.FirstOrDefault(p => p.DisplayText == parentNode.Text);

            UIElement parentElement = FindParent(parent);

            UIElement child = new UIElement();
            child.DisplayText = type + "- " + id;
            WindowControl ctrl = new WindowControl();
            ctrl.Name = name;
            ctrl.AutomationId = id;
            ctrl.ControlType = type;
            ctrl.ApplicationTreePath = ctrlPath;
            child.ElementAttributes = ctrl;
            parentElement.Children.Add(child);
            Reload();
        }

        private TreeNode FindParent(TreeNode node, TreeNode child)
        {
            if (node.Parent != null)
                return FindParent(node.Parent, node);
            return child;
        }

        private UIElement FindParent(UIElement parent)
        {
            foreach (UIElement e in parent.Children)
            {
                if (e.DisplayText == this.trAppControl.SelectedNode.Text)
                    return e;
                return FindParent(e);
            }
            return null;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var node = this.trAppControl.SelectedNode.Text;
            
            //check whether it is application

            var app = _desktop.Applications.FirstOrDefault(a => a.DisplayText == node);

            //if not get the selected child control
            if (app == null)
            {
                var parentNode = FindParent(this.trAppControl.SelectedNode, null);

                var parent = _desktop.Applications.FirstOrDefault(p => p.DisplayText == parentNode.Text);

                Remove(parent);
            }
            else
            {
                _desktop.Applications.Remove(app);
            }
        }

        private void Remove(UIElement parent)
        {
            foreach (UIElement child in parent.Children)
            {
                if (child.DisplayText == this.trAppControl.SelectedNode.Text)
                {
                    parent.Children.Remove(child);
                    break;
                }
                Remove(child);
            }       
        }

        private void trAppControl_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left &&
               e.Node.IsSelected == true)
            {
                contextMenuStrip1.Show();
            }
        }

    }

}

