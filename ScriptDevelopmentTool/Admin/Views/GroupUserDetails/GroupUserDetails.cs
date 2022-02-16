using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infosys.ATR.Admin.Entities;
using IMSWorkBench.Infrastructure.Interface.Services;

namespace Infosys.ATR.Admin.Views
{
    public partial class GroupUserDetails : UserControl,IGroupUserDetails,IClose
    {
        public List<UserRoles> Roles { get; set; }
        Groups g = null;       

        public GroupUserDetails()
        {
            InitializeComponent();
        }

        public void Initialize(Groups g)
        {
            this.g = g;
            this._presenter.GetRoles();
            this.cmbRoles.DataSource = Roles;
            this.cmbRoles.DisplayMember = "Name";
            this.cmbRoles.ValueMember = "Id";
            this.txtAlias.Text = String.Empty;
        }  

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Users users = null;
            if (!String.IsNullOrEmpty(txtAlias.Text))
            {
                users = this._presenter.GetDetails(txtAlias.Text);                
                bool userAdded = false;
                if (users != null)
                {
                    //add all children
                    g.Children.ForEach(gr =>
                    {
                        users.Alias = txtAlias.Text;
                        users.RoleId = (int)cmbRoles.SelectedValue;
                        users.CategoryId = gr;
                        userAdded = this._presenter.AddUserToAGroup(users);
                        //if (!userAdded) throw new GenericException(String.Format("User {0} already present in this category",users.Alias));
                    });

                    //add all parents
                    //g.Parents.ForEach(gr =>
                    //{
                    //    users.Alias = txtAlias.Text;
                    //    users.RoleId = (int)cmbRoles.SelectedValue;
                    //    users.CategoryId = gr;
                    //    userAdded = this._presenter.AddUser(users);
                    //});   
                    

                    if (userAdded) MessageBox.Show("User Added", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this._presenter.RefreshGroupUsers_Handler();
                }
                else
                {
                    MessageBox.Show("User not found", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }                

            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this._presenter.Close();  
        }



        public void Close()
        {
            this._presenter.Close();
        }
    }
}
