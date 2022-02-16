/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using IMSWorkBench.Infrastructure.Interface;
using Infosys.ATR.Admin.Entities;
using Infosys.ATR.Admin.Constants;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.WinForms;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Infosys.ATR.Admin.Views
{
    public class ModuleController : WorkItemController
    {
        Explorer _explorer = null;

        [EventPublication(Constants.EventTopicNames.LoadGeneric, PublicationScope.Global)]
        public event EventHandler LoadAdmin;

        [EventPublication(Constants.EventTopicNames.Exit, PublicationScope.Global)]
        public event EventHandler Exit;

        [EventPublication(EventTopicNames.Showtoolbar, PublicationScope.Global)]
        public event EventHandler Showtoolbar;


        public override void Run()
        {
            //AddView();
            //LoadAdmin(this, new EventArgs());
            //ExtendMenu();
            //ExtendToolStrip();            
        }

        [EventSubscription(Constants.EventTopicNames.AdminView,ThreadOption.UserInterface)]
        public void AdminView(object sender, EventArgs e)
        {
            AddView();    
        }

        [EventSubscription(Constants.EventTopicNames.CloseAdminView, ThreadOption.UserInterface)]
        public void CloseAdminView(object sender, EventArgs e)
        {
            var obj = this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart;
            if (obj.GetType().Name == "Explorer")
            {
                this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Close(obj);
                var o = this.WorkItem.SmartParts.Get("Explorer");
                if(o != null)
                    this.WorkItem.SmartParts.Remove(o);
            }
        }

        void ExtendMenu()
        {
            ToolStripMenuItem newCat = new ToolStripMenuItem("New Category");
            newCat.Click += newCat_Click;
            newCat.ShortcutKeys = Keys.Control | Keys.C;
            newCat.ToolTipText = "Create new category";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(newCat);

            ToolStripMenuItem delCat = new ToolStripMenuItem("Delete Category");
            delCat.Click += delCat_Click;
            delCat.ShortcutKeys = Keys.Control | Keys.D;
            delCat.ToolTipText = "Delete category";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(delCat);

            ToolStripMenuItem refresh = new ToolStripMenuItem("Refresh");
            refresh.Click += refresh_Click;
            refresh.ShortcutKeys = Keys.Control | Keys.R;
            refresh.ToolTipText = "Reload categories";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(refresh);

            ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
            exit.Click += new EventHandler(exit_Click);
            exit.ShortcutKeys = Keys.Control | Keys.Q;
            exit.ToolTipText = "Exit Application";
            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripMenuItem>(exit);


            ToolStripMenuItem groups = new ToolStripMenuItem("Groups");
            this.WorkItem.RootWorkItem.UIExtensionSites["MainMenu"].Add<ToolStripMenuItem>(groups);

            ToolStripMenuItem explorer = new ToolStripMenuItem("Explorer");
            explorer.Click += explorer_Click;
            explorer.ShortcutKeys = Keys.Control | Keys.G;
            explorer.ToolTipText = "Manage Groups";
            groups.DropDownItems.Add(explorer);

        }

        private void ExtendToolStrip()
        {
            ToolStripButton newCat = new ToolStripButton();
            newCat.Image = new System.Drawing.Bitmap(@"Images\add-script.png");
            newCat.ToolTipText = "Create new Category";
            newCat.Click += newCat_Click;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(newCat);

            ToolStripButton delCat = new ToolStripButton();
            delCat.Image = new System.Drawing.Bitmap(@"Images\remove.png");
            delCat.ToolTipText = "Delete Category";
            delCat.Click +=delCat_Click;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(delCat);

            ToolStripButton refresh = new ToolStripButton();
            refresh.Image = new System.Drawing.Bitmap(@"Images\Refresh-or-Reset.png");
            refresh.ToolTipText = "Reload categories";
            refresh.Click +=refresh_Click;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(refresh);

            this.WorkItem.RootWorkItem.UIExtensionSites["FileMenu"].Add<ToolStripSeparator>(new ToolStripSeparator());

            ToolStripButton groups = new ToolStripButton();
            groups.Image = new System.Drawing.Bitmap(@"Images\groups.jpg");
            groups.ToolTipText = "Manage Groups";
            groups.Click += explorer_Click;
            this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].Add<ToolStripButton>(groups);                      
       
            Showtoolbar(this, new EventArgs());
        }

        void newCat_Click(object sender, EventArgs e)
        {
            _explorer.NewCategory();
        }

        void refresh_Click(object sender, EventArgs e)
        {
            _explorer.Refresh();
        }

        [EventSubscription(Constants.EventTopicNames.DeleteAdminNode,ThreadOption.UserInterface)]
        public void delCat_Click(object sender, EventArgs e)
        {
            _explorer.DeleteCategory();
        }

       
        [EventSubscription(Constants.EventTopicNames.GroupExplorer,ThreadOption.UserInterface)]
        public void explorer_Click(object sender, EventArgs e)
        {
            var ge = AddViewIfNotExists<GroupsExplorer>("GroupExplorer");
            ge.Dock = DockStyle.Fill;
            WindowSmartPartInfo sp = new WindowSmartPartInfo();
            sp.MaximizeBox = false;
            sp.MinimizeBox = false;            
            sp.Modal = true;
            sp.Title = "Group Explorer";
            ge.Initialize();
            sp.Height = 450;
            sp.Width = _explorer.Width-350;            

            sp.Location = new Point(sp.Location.X + (sp.Width - _explorer.Width) / 2, sp.Location.Y + (sp.Height - _explorer.Height) / 2);
            this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.ModalWindows].Show(ge, sp);
        }

        void AddView()
        {
            var o = this.WorkItem.SmartParts.Get("Explorer");
            if (o == null)
            {
                _explorer = this.WorkItem.SmartParts.AddNew<Explorer>("Explorer");
                _explorer.Dock = DockStyle.Fill;
                _explorer.Initialize();

                if (_explorer.ShowExplorer)
                {
                    WindowSmartPartInfo info = new WindowSmartPartInfo();
                    info.Title = "Admin Explorer";
                    this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(_explorer, info);

                    GroupDetails grDetails = AddViewIfNotExists<GroupDetails>("AdminGroupDetials");
                    grDetails.Dock = DockStyle.Fill;

                    SemanticCluster semanticCluster = AddViewIfNotExists<SemanticCluster>("SemanticCluster");
                    semanticCluster.Dock = DockStyle.Fill;

                    UserDetails userDetails = AddViewIfNotExists<UserDetails>("UserDetails");
                    userDetails.Dock = DockStyle.Fill;

                    GroupUserDetails gruserDetails = AddViewIfNotExists<GroupUserDetails>("GroupUserDetails");
                    gruserDetails.Dock = DockStyle.Fill;

                    GroupExplorerDetails grexplorerDetails = AddViewIfNotExists<GroupExplorerDetails>("GroupExplorerDetails");
                    grexplorerDetails.Dock = DockStyle.Fill;

                    GroupSelector grSelector = AddViewIfNotExists<GroupSelector>("GroupSelector");
                    grSelector.Dock = DockStyle.Fill;
                }
                else
                    this.WorkItem.SmartParts.Remove(_explorer);
            }
        }

        void exit_Click(object sender, EventArgs e)
        {
            if (Exit != null)
                Exit(this, e);
        }

        internal T AddViewIfNotExists<T>(string name) where T : IClose
        {
            T t = this.WorkItem.SmartParts.Get<T>(name);

            if (t != null)
            {
                t.Close();
                this.WorkItem.SmartParts.Remove(t);
            }

            t = this.WorkItem.SmartParts.AddNew<T>(name);            

            return t;

        }


    }
}
