using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Infosys.ATR.Entities;
using Infosys.ATR.Admin.Entities;
using Infosys.WEM.SecureHandler;
using System.Collections.ObjectModel;

namespace Infosys.ATR.Admin.Views
{
    public partial class GroupDetails : UserControl, IGroupDetails, IClose
    {
        public Groups Groups { get; set; }
        public List<UserRoles> Roles { get; set; }
        private const int AllModuleID = 1;
        List<WEM.Service.Common.Contracts.Data.Category> categories = null;
        BindingList<SemanticGroup> clusters;
        public BindingList<SemanticGroup> Selected { get; set; }
        public GroupDetails()
        {
            InitializeComponent();
        }

        public new void Show()
        {
            clusters = new BindingList<SemanticGroup>();
            Selected = new BindingList<SemanticGroup>();

            if (Groups != null)
            {
                this.txtDescription.Text = Groups.Action != Infosys.ATR.Admin.Entities.Action.Add ? Groups.Description : "";
                this.txtName.Text = Groups.Action != Infosys.ATR.Admin.Entities.Action.Add ? Groups.Name : "";

                this._presenter.GetRoles();

                if(Groups.Action != Entities.Action.Add)
                    this.Selected = this._presenter.GetSemanticCategories(Groups.CategoryId);

                var grDetails = this._presenter.WorkItem.SmartParts.Get("AdminGroupDetials");

                if (grDetails == null)
                {
                    grDetails = this._presenter.WorkItem.SmartParts.AddNew<Admin.Views.GroupDetails>("AdminGroupDetials");
                }

                this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.RightWorkspace].Show(grDetails);

                if (Groups.Action == Entities.Action.View)
                {
                    this.btnAddGroup.Enabled = false;
                }
                else
                {
                    this.btnAddGroup.Enabled = true;
                }

                var isSuperAdmin = (bool)this._presenter.WorkItem.RootWorkItem.Items["IsSuperAdmin"];

                if (Groups.CompanyId == 0 && !isSuperAdmin)
                    btnUpdate.Enabled = false;
                else
                    btnUpdate.Enabled = true;
                btnUser.Enabled = true;

                DisplayModules();
                if (!string.IsNullOrEmpty(Groups.ModuleID))
                    cmbModule.SelectedValue = Convert.ToInt32(Groups.ModuleID);
                if (Groups.AddCategory)
                {
                    if (Groups.ParentId == 0 && Groups.CategoryId == 0)
                    {
                        cmbModule.SelectedValue = AllModuleID;
                        cmbModule.Enabled = true;
                    }
                    else
                    {
                        if (Groups.ModuleID.Equals(AllModuleID.ToString()))
                            cmbModule.Enabled = true;
                        else
                            cmbModule.Enabled = false;
                    }
                }
                else
                {
                    // if root category is selected
                    if (Groups.ParentId == 0 && Groups.CategoryId > 0)
                    {
                        cmbModule.Enabled = true;
                    }
                    else
                    {
                        // if child category is selected and if parent module is all, allow user to change the module for child category.
                        int parentModuleID = FindParentModule(Groups.ParentId.ToString());
                        if (parentModuleID == AllModuleID)
                            cmbModule.Enabled = true;
                        else
                            cmbModule.Enabled = false;
                    }
                }
            }
            else
            {
                txtDescription.Text = "";
                txtName.Text = "";
                btnAddGroup.Enabled = false;
                btnUpdate.Enabled = false;
                btnUser.Enabled = false;
                lblModule.Visible = false;
                cmbModule.Visible = false;
            }

            PopulateClusters();

            PopulateSelectedNodes();
            Filter();

        }

        private void Filter()
        {
            Selected.ToList().ForEach(s =>
            {

                var node1 = clusters.FirstOrDefault(n => n.Name == s.Name);
                if (node1 != null) clusters.Remove(node1);

            });
        }

        private void PopulateClusters()
        {
            // var c = this._presenter.WorkItem.Items.Get("SemanticCluster") as List<SemanticGroup>;

            var c = this._presenter.WorkItem.State["SemanticCluster"] as List<SemanticGroup>;
            if (c != null)
            {
                c.ForEach(n => clusters.Add(n));
            }
            PopulateActiveNodes();

        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            var userDetails = this._presenter.WorkItem.SmartParts.Get<UserDetails>("UserDetails");
            if (userDetails == null)
                userDetails = this._presenter.WorkItem.SmartParts.AddNew<UserDetails>("UserDetails");
            this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.RightWorkspace].Show(userDetails);
            userDetails.Initialize(Groups);
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            var companyId = ConfigurationManager.AppSettings["Company"];
            Groups g = new Entities.Groups();
            g.Name = txtName.Text;
            g.Description = txtDescription.Text;
            g.ParentId = Groups != null ? Groups.CategoryId : 0;
            g.CompanyId = Convert.ToInt32(companyId);
            g.ModuleID = cmbModule.SelectedValue.ToString();
            var isAdded = this._presenter.AddCategory(g);

            if (isAdded)
            {
                categories = this._presenter.GetAllCategories();
                var category = categories.FirstOrDefault(c => c.Name == g.Name);

                var users = (List<Infosys.ATR.Entities.Users>)this._presenter.WorkItem.RootWorkItem.Items["CurrentUser"];

                if (users != null)
                {
                    var user = users.FirstOrDefault();
                    Infosys.ATR.Entities.Users u = new ATR.Entities.Users();
                    u.Alias = user.Alias; 
                    u.GroupId = category.CategoryId;
                    u.DisplayName = user.DisplayName;                    
                    u.Role = Entities.Roles.Manager.ToString();
                    this._presenter.AddUser(u);
                }

                if (g.ParentId != 0)
                {
                    var result = Infosys.ATR.Admin.Services.WFService.GetAnyUsers(g.ParentId.ToString(), companyId);

                    if (result.Users != null)
                    {
                        result.Users.ForEach(user =>
                        {
                            Infosys.ATR.Entities.Users u = new ATR.Entities.Users();
                            u.Alias = user.Alias;
                            u.GroupId = category.CategoryId;
                            u.DisplayName = user.DisplayName;
                            u.Role = user.Role.ToString();
                            //u.Role = Entities.Roles.Manager.ToString();
                            this._presenter.AddUser(u);
                        });
                    }
                }
                var clusters = this._presenter.GetSemanticClusters(category.ParentId.GetValueOrDefault());

                clusters.ForEach(c => {
                    c.IsActive = true;
                    this._presenter.AddSemanticCategory(category.CategoryId, c);                
                });

                var rest = Selected.Where(s => !clusters.Contains(s));

                //if (Selected.Count > 0)
                if(rest.Count() > 0)
                {                
                    //this._presenter.AddSemanticCategory(category.CategoryId, Selected);
                    this._presenter.AddSemanticCategory(category.CategoryId, rest.ToList());
                }

                MessageBox.Show("Category created", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this._presenter.RefreshCategory_Handler(txtName.Text);
            }
            else
                MessageBox.Show("Category not created. Try giving an unique name", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Groups != null)
            {
                Groups g = Groups;
                g.NewName = txtName.Text;
                g.Description = txtDescription.Text;
                g.ModuleID = cmbModule.SelectedValue.ToString();
                var isUpdated = this._presenter.UpdateCategory(g);

                if (isUpdated)
                {
                    //if (Selected.Count > 0)
                    //    this._presenter.UpdateSemanticCategory(g.CategoryId, Selected);

                    g.Children.ForEach(g1 =>
                    {
                        this._presenter.UpdateSemanticCategory(g1, Selected);
                    });

                    g.Parents.ForEach(g2 =>
                    {
                        this._presenter.UpdateSemanticCategory(g2, Selected);
                    });

                    MessageBox.Show("Category updated", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this._presenter.RefreshCategory_Handler(txtName.Text);
                }
                else
                {
                    MessageBox.Show("Category could not be updated. Try giving different name.", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }



        public void Close()
        {
            this._presenter.OnCloseView();
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {

            this._presenter.ShowSelector();
        }

        private void GetModules()
        {
            if (cmbModule.Items.Count == 0)
            {
                categories = this._presenter.GetAllCategories();
                var modules = this._presenter.GetAllModules();
                this.cmbModule.DataSource = modules;
                this.cmbModule.DisplayMember = "ModuleName";
                this.cmbModule.ValueMember = "ModuleID";
                cmbModule.SelectedIndex = 0;
            }

        }
        private void DisplayModules()
        {
            GetModules();
            lblModule.Visible = true;
            cmbModule.Visible = true;
        }

        /// <summary>
        /// This method is used to find the parent category at the root level.
        /// </summary>
        /// <param name="parentID">Category Id</param>
        /// <returns>Id of root category</returns>
        public int FindParentModule(string parentID)
        {
            int moduleId = 0;
            var result = categories.FirstOrDefault(cat => cat.CategoryId.ToString().Contains(parentID));

            if (result != null && result.ModuleID > 0)
            {
                moduleId = result.ModuleID;
            }
            return moduleId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var n = this.listBox1.SelectedItem as SemanticGroup;
            if (n != null)
            {
                var one = Selected.FirstOrDefault(s => s.Name == n.Name && s.Id == n.Id);
                if (one == null)
                {
                    n.IsActive = true;
                    Selected.Add(n);
                }
                else
                {
                    one.IsActive = true;
                }

                clusters.Remove(n);
                PopulateActiveNodes();
                PopulateSelectedNodes();
            }
        }

        void PopulateActiveNodes()
        {
            this.listBox1.DataSource = clusters;
            this.listBox1.DisplayMember = "Name";
            this.listBox1.ValueMember = "Id";
            this.listBox1.Tag = typeof(SemanticGroup);
            this.listBox1.Refresh();
        }

        void PopulateSelectedNodes()
        {
            var ds = Selected.Where(n => n.IsActive == true).ToList();
            this.listBox2.DataSource = ds;
            this.listBox2.DisplayMember = "Name";
            this.listBox2.ValueMember = "Id";
            this.listBox2.Tag = typeof(SemanticGroup);
            this.listBox2.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var n = this.listBox2.SelectedItem as SemanticGroup;
            if (n != null)
            {
                var one = clusters.FirstOrDefault(s => s.Name == n.Name && s.Id == n.Id);
                if (one == null)
                {
                    clusters.Add(n);
                    PopulateActiveNodes();
                }

                var r = Selected.FirstOrDefault(n1 => n1.Name == n.Name && n1.Id == n.Id);
                r.IsActive = false;
                PopulateSelectedNodes();
            }
        }
    }
}
