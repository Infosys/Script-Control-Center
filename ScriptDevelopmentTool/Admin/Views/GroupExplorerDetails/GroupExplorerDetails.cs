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

namespace Infosys.ATR.Admin.Views
{
    public partial class GroupExplorerDetails : UserControl, IGroupExplorerDetails,IClose
    {
        public Groups Groups { get; set; }
        public List<UserRoles> Roles { get; set; }

        public GroupExplorerDetails()
        {
            InitializeComponent();
        }

        public new void Show()
        {
            if (Groups != null)
            {
                this.txtDescription.Text = Groups.Action != Infosys.ATR.Admin.Entities.Action.Add ? Groups.Description : "";
                this.txtName.Text = Groups.Action != Infosys.ATR.Admin.Entities.Action.Add ? Groups.Name : "";
                this._presenter.GetRoles();
                var grDetails = this._presenter.WorkItem.SmartParts.Get("GroupExplorerDetails");
                if (grDetails == null)
                {
                    grDetails = this._presenter.WorkItem.SmartParts.AddNew<GroupExplorerDetails>("GroupExplorerDetails");
                }
                this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.GrRightWorkspace].Show(grDetails);

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
            }
            else
            {
                txtDescription.Text = "";
                txtName.Text = "";
                btnAddGroup.Enabled = false;
                btnUpdate.Enabled = false;
                btnUser.Enabled = false;
            }

        }

        private void btnUser_Click(object sender, EventArgs e)
        {
            var userDetails = this._presenter.WorkItem.SmartParts.Get<GroupUserDetails>("GroupUserDetails");
            if(userDetails ==null)
                userDetails= this._presenter.WorkItem.SmartParts.AddNew<GroupUserDetails>("GroupUserDetails");
            this._presenter.WorkItem.Workspaces[Constants.WorkspaceNames.GrRightWorkspace].Show(userDetails);
            userDetails.Initialize(Groups);
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            Groups g = new Entities.Groups();
            g.Name = txtName.Text;
            g.Description = txtDescription.Text;
            g.ParentId = Groups != null ? Groups.CategoryId : 0;
            g.CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]);            
            var isAdded = this._presenter.AddCategory(g);

            if (isAdded)
            {
                var groups = this._presenter.GetAllGroups();
                var group = groups.FirstOrDefault(gr => gr.Name == g.Name);

                var users = (List<Infosys.ATR.Entities.Users>) this._presenter.WorkItem.RootWorkItem.Items["CurrentUser"];
                if (users != null)
                {
                    var user = users.FirstOrDefault();
                    Infosys.ATR.Entities.Users u = new ATR.Entities.Users();
                    u.Alias = user.Alias;
                    u.GroupId = group.CategoryId;
                    u.DisplayName = user.DisplayName;
                    u.Role = Entities.Roles.Manager.ToString();
                    this._presenter.AddUser(u);
                }
                MessageBox.Show("Group created", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this._presenter.RefreshGroupsCategory_Handler(txtName.Text);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Groups != null)
            {
                Groups g = Groups;
                g.Name = txtName.Text;
                g.Description = txtDescription.Text;                                     
                var isUpdated = this._presenter.UpdateGroups(g);

                if (isUpdated) MessageBox.Show("Group updated", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        public void Close()
        {
            this._presenter.OnCloseView();
        }
    }
}
