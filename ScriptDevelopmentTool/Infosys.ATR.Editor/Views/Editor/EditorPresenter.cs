/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Windows.Forms;

using Infosys.ATR.Editor.Constants;
using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.UIAutomation.Recorder.ScreenCapture;
using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.Editor.Services;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.Editor.Views
{
    public partial class EditorPresenter : Presenter<IEditor>
    {
        Entity.Root _root;       
        TreeNode _node;
        bool _imageCaptured = false;

        [EventPublication(EventTopicNames.SaveImage, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<string>> SaveImage;

        [EventPublication(EventTopicNames.Clear, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs> Clear;

        [EventPublication(EventTopicNames.UpdateImage, PublicationScope.WorkItem)]
        //public event EventHandler<EventArgs<List<Entity.State>>> UpdateImage;
        public event EventHandler<EventArgs<object[]>> UpdateImage;

        [EventPublication(EventTopicNames.UpdateApplication, PublicationScope.WorkItem)]
        //public event EventHandler<EventArgs<List<Entity.State>>> UpdateImage;
        public event EventHandler<EventArgs<Entities.ApplicationProperties>> UpdateApplication;

        [EventPublication(EventTopicNames.UpdateBaseDir, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<string>> UpdateBaseDir;


        [EventPublication(EventTopicNames.Maximize, PublicationScope.Global)]
        public event EventHandler<EventArgs> Maximize;

        [EventPublication(EventTopicNames.OMSave, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> OMSaved;


        /// <summary>
        /// This method is a placeholder that will be called by the view when it has been loaded.
        /// </summary>
        public override void OnViewReady()
        {
            base.OnViewReady();

        }

        /// <summary>
        /// Close the view
        /// </summary>
        public override void OnCloseView()
        {
            base.CloseView();
        }

        [EventSubscription(EventTopicNames.Capture, ThreadOption.UserInterface)]
        public void OnCapture(object sender, Selector.ImageCapturedArguements e)
        {
                this.View.Area = (Entity.Area)Enum.Parse(typeof(Entity.Area), e.Area);
                this.View.State = e.State;
                this.View.SaveImage(e.Image);
                Maximize(this, new EventArgs());
        }

        [EventSubscription(EventTopicNames.UpdateName, ThreadOption.UserInterface)]
        public void OnUpdateName(object sender, EventArgs<String> e)
        {
            this.View.UpdateName(e.Data);

        }

        [EventSubscription(EventTopicNames.UpdateAppProperties, ThreadOption.UserInterface)]
        public void OnUpdateAppProperties(object sender, EventArgs<String[]> e)
        {
            this.View.UpdateAppProperties(e.Data);

        }

        [EventSubscription(EventTopicNames.UpdateControlProperties, ThreadOption.UserInterface)]
        public void OnUpdateControlProperties(object sender, EventArgs<String> e)
        {
            this.View.UpdateControlProperties(e.Data);

        }

        public void Save(string imagePath)
        {
            if (SaveImage != null)
            {
                SaveImage(this, new EventArgs<string>(imagePath));
            }
        }


        public void ClearText()
        {
            if (Clear != null)
            {
                Clear(this, new EventArgs());
            }
        }


        internal void UpdateImageProperties(object[] param)
        {
            if (UpdateImage != null)
            {
                UpdateImage(this, new EventArgs<object[]>(param));
            }

        }


        internal void UpdateApplicationProperties(Entity.ApplicationProperties ctrl)
        {
            if (UpdateApplication != null)
            {
                UpdateApplication(this, new EventArgs<Entity.ApplicationProperties>(ctrl));
            }
        }

        internal void SetBaseDirectory()
        {
            if (UpdateBaseDir != null)
            {
                UpdateBaseDir(this, new EventArgs<String>(this.View.BaseDir));
            }
        }

        internal string Serialize(Entity.Root _root)
        {
            var autoConfig = Translate.ToAutomationConfig(_root);
            return Utilities.Serialize<AutomationConfig>(autoConfig);
        }

        internal void GenerateScript(Entity.Root _root)
        {
            var scripts = Translate.ToScript(_root);

            foreach (KeyValuePair<String, String> kp in scripts)
            {
                Write(Path.Combine(this.View.BaseDir, kp.Key), kp.Value);
            }

        }



        internal AutomationConfig Deserialize(string filepath)
        {
            using (StreamReader s = new StreamReader(filepath))
            {
                AutomationConfig autoConfig = Utilities.Deserialize<AutomationConfig>(s.ReadToEnd());
                return autoConfig;
            }
        }

        internal void Write(string path, string s)
        {
            Utilities.Write(path, s);
        }

        internal void BuildTree(AutomationConfig autoConfig)
        {
            var root = Translate.ToTreeView(autoConfig);
            this.View.BuildTree(root);
        }

        internal void GenerateObjectModel(TreeView tv, string ucName)
        {
            TreeNode n = tv.TopNode;
            try
            {
                if (n.Text.Contains("Desktop"))
                {
                    _root = new Entity.Root();
                    _root.BaseDirectory = this.View.BaseDir;
                    _root.ObjectModelName = this.View.ObjectModel;
                    _root.Application = new List<Entity.Application>();
                    _root.Mode = this.View.Mode;
                    GenerateObjectModel(n.Nodes);
                    var autoConfig = Serialize(_root);
                    this.View._root = _root;
                    this.View.Save(autoConfig, ucName);
                }
            }
            catch(Exception ex)
            {
                tv.SelectedNode = _node;
                MessageBox.Show(ex.Message, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }



        private void GenerateObjectModel(TreeNodeCollection nodes)
        {

            if (nodes != null && nodes.Count > 0)
            {
                foreach (TreeNode n in nodes)
                {
                    _node = n;
                    if (n.Text.Contains("Application"))
                    {
                        var ctrl = n.Tag as Entity.ApplicationProperties;

                        Validate(ctrl);

                        Entity.Application a = new Entity.Application();
                        a.Properties = ctrl;
                        if (a.Screens == null)
                        {
                            a.Screens = new List<Entity.Screen>();
                        }
                        _root.Application.Add(a);
                    }

                    else if (n.Text.Contains("Screen"))
                    {
                        var ctrl = n.Tag as Entity.BaseProperties;

                        Validate(ctrl);

                        Entity.Screen s = new Entity.Screen();
                        s.Properties = ctrl;
                        if (s.Controls == null) s.Controls = new List<Entity.Control>();
                        var a = _root.Application[_root.Application.Count - 1];
                        a.Screens.Add(s);
                    }

                    else if (n.Text.Contains("Control"))
                    {
                        //recursion inside recursion
                        //like dream inside a dream

                        var ctrl = n.Tag as Entity.ControlProperties;

                        Validate(ctrl);

                        Entity.Control c = new Entity.Control();
                        c.Properties = ctrl;
                        c.InnerControls = new List<Entity.Control>();
                        var a = _root.Application[_root.Application.Count - 1];
                        var s = a.Screens[a.Screens.Count - 1];

                        var t = n.Parent;
                        if (!t.Text.Contains("Screen"))
                        {
                            var t1 = t.Tag as Entity.ControlProperties;
                            Validate(t1);
                            //c.Properties.Parent = t1.ControlName;
                            c.Properties.Parent = t1.Name;
                            var parent = GetInnerControls(s.Controls, t1.Name);
                            parent.InnerControls.Add(c);
                        }
                        else
                        {
                            c.Properties.Parent = "Screen";
                            s.Controls.Add(c);
                        }
                    }
                    GenerateObjectModel(n.Nodes);
                }
            }
        }

        private void Validate(object ctrl)
        {
           // if(this.View.Mode == Entity.ProjectMode.Win32)
                Validator.Validate(ctrl);
        }

        private Entity.Control GetInnerControls(List<Entity.Control> children, string ctrlName)
        {

            foreach (Entity.Control c in children)
            {
                if (c.Properties.Name == ctrlName)
                {
                    return c;
                }
                else
                {
                    var ch = GetInnerControls(c.InnerControls, ctrlName);
                    if (ch != null)
                    {
                        return ch;
                    }
                }
            }

            return null;
        }


        internal void AddImagePanel(System.Windows.Forms.Control smartpart)
        {
            this.WorkItem.Workspaces[Constants.WorkspaceNames.UtilWorkSpace].Show(smartpart);

        }



        private void DeleteImage(TreeNode treeNode)
        {     
            var dir = this.View.BaseDir;
            var n = treeNode.Tag as Entities.BaseProperties;
            n.State.ForEach(s => {
                Delete(s.Area.Above.Path);
                Delete(s.Area.Below.Path);
                Delete(s.Area.Center.Path);
                Delete(s.Area.Left.Path);
                Delete(s.Area.Right.Path);
                Delete(s.Area.Validate.Path);                
            });
            
        }

        internal void Delete(TreeNode n)
        {
            DeleteImage(n);

            foreach (TreeNode n1 in n.Nodes)
            {
                Delete(n1);
            }
        }

        //private void DeleteImageRecursively(TreeNodeCollection nodes)
        //{
        //    foreach (TreeNode n in nodes)
        //    {
        //        DeleteImage(n);
        //        DeleteImageRecursively(n.Nodes);
        //    }
        //}

        private void Delete(string t)
        {
            if (!string.IsNullOrEmpty(t))
            {
                if (File.Exists(t))
                {
                    File.Delete(t);
                }
                else
                {
                    var t1 = Path.Combine(this.View.BaseDir, t);
                    if (File.Exists(t1))
                        File.Delete(t1);
                }
            }
        }

        internal void OMSaveHandler(string objectModel)
        {
           
            OMSaved(this, new EventArgs<string>(objectModel));
        
        }
    }
}
