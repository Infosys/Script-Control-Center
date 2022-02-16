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
using System.Configuration;

using Infosys.ATR.Admin.Constants;
using Infosys.ATR.Admin.Services;
using Infosys.ATR.Admin.Entities;

using Infosys.WEM.Scripts.Service.Contracts;
//using Infosys.WEM.Scripts.Service.Contracts.Message;

using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using Infosys.WEM.SecurityAccess.Contracts.Message;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.WEM.Service.Common.Contracts.Data;
using Infosys.WEM.Service.Common.Contracts.Message;

namespace Infosys.ATR.Admin.Views
{
    public class GroupsExplorerPresenter : Presenter<IGroupsExplorer>
    {
        [EventPublication(EventTopicNames.ShowGroupExplorerDetails, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<Groups>> ShowGroupDetails;


        string _companyId;

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        public override void OnCloseView()
        {
            base.OnCloseView();
        }       

        internal void GetCategories()
        {
            string companyId = ConfigurationManager.AppSettings["Company"];
           // var groups = WFScriptService.GetAllCategories(companyId);

            var groups = WFCommonService.GetAllCategory(companyId, Constants.Application.Module);

            this.View.Groups = groups.Categories.Select(g => new Groups
            {
                Description = g.Description,
                CategoryId = g.CategoryId,
                Name = g.Name,
                ParentId = g.ParentId.GetValueOrDefault(),
                CompanyId = g.CompanyId,
                ModuleID = g.ModuleID.ToString()
            }).ToList();


            for (int i = 0; i < this.View.Groups.Count; i++)
            {
                var c = this.View.Groups[i];
                if (c.CompanyId == 0 && c.ParentId == 0)
                {
                    var subCat = this.View.Groups.Where(sc => sc.CompanyId == 0 && sc.ParentId == c.CategoryId);
                    if (subCat == null || subCat.Count() == 0)
                    {
                        this.View.Groups.Remove(c);
                    }
                }
            }

            //this.View.Groups.ForEach(c =>
            //{
            //    if (c.CompanyId == 0 && c.ParentId == 0)
            //    {
            //        var subCat = this.View.Groups.Where(sc => sc.CompanyId == 0 && sc.ParentId == c.GroupId);
            //        if (subCat == null || subCat.Count() == 0)
            //        {
            //            this.View.Groups.Remove(c);
            //        }
            //    }
            //});

            var security = this.WorkItem.RootWorkItem.State["Security"];

            if (security == "AllowAuthorised")
            {
                var isSuperAdmin = (bool)this.WorkItem.RootWorkItem.Items["IsSuperAdmin"];
                if (!isSuperAdmin)
                {
                    CheckAccess();
                }
            }
        }

        private void CheckAccess()
        {
            var currentUser = this.WorkItem.RootWorkItem.Items["CurrentUser"] as List<Infosys.ATR.Entities.Users>;

            string companyID = ConfigurationManager.AppSettings["Company"];
            var allUsers = WFService.GetAllUsers(companyID);

            currentUser = allUsers.Users.Where(u => u.Alias == currentUser[0].Alias).Select(n => new Infosys.ATR.Entities.Users { 
                Alias = n.Alias,DisplayName = n.DisplayName,CategoryId = n.CategoryId,Id = n.UserId,Role = Enum.GetName(typeof(Infosys.ATR.Entities.Roles),n.Role)
            }).ToList();

            var userCategory = currentUser.Where(u => u.Role == "Manager").Select(c => c.GroupId).ToList();

            if (currentUser != null && currentUser.Count > 0)
            {
                var categories = this.View.Groups;
                this.View.Groups = categories.Where(c => userCategory.Contains(c.CategoryId) || c.CompanyId == 0).ToList();

                if (this.View.Groups == null || this.View.Groups.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Oops ! You are not part of any group as a Manager", "IAP", System.Windows.Forms.MessageBoxButtons.OK,
                   System.Windows.Forms.MessageBoxIcon.Information);                    
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Oops ! You are not part of any group as a Manager", "IAP", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
        }

        internal void GetAllGroups()
        {
            _companyId = ConfigurationManager.AppSettings["Company"];
            var groups = WFService.GetallGroups(_companyId);

            this.View.Groups = groups.Groups.Select(g => new Groups
            {
                CompanyId = g.CompanyID,
                Description = g.Description,
                CategoryId = g.GroupId,
                Name = g.Name,
                ParentId = g.ParentId,
                Roles = g.Roles
            }).ToList();
        }

        internal List<UserRoles> GetAllRoles()
        {
            _companyId = ConfigurationManager.AppSettings["Company"];
            var roles = WFService.GetAllRoles(_companyId);
            roles.Roles.RemoveAll(r => r.Name == "Administrator");
            return roles.Roles.Select(r => new UserRoles
            {
                Description = r.Description,
                Id = r.RoleId,
                Name = r.Name
            }).ToList();
        }

        internal void GetAllUsersInaGroup(Groups g)
        {
            _companyId = ConfigurationManager.AppSettings["Company"];
            var roles = GetAllRoles();
            var users = WFService.GetAllUsersInaGroup(g.CategoryId.ToString(), _companyId);
            UserComparer distinct = new UserComparer();

            this.View.Users = new System.ComponentModel.BindingList<Users>(
                users.Users.Where(u => u.GroupId == g.CategoryId).Select(u => new Users
                {
                    Delete = Image.FromFile(@"Images\remove.png"),
                    DL = u.IsDL == true ? Image.FromFile(@"Images\dl.png") : Image.FromFile(@"Images\user.jpg"),
                    Alias = u.Alias,
                    Id = u.UserId,
                    RoleId = u.Role,
                    CategoryId = u.CategoryId
                }).Distinct(distinct).ToList());
        }

        internal void ShowGroupDetails_Handler(Groups group)
        {
            ShowGroupDetails(this, new EventArgs<Groups>(group));
        }

        internal bool UpdateUser(Users user)
        {
            return WFService.UpdateUser(new UpdateUserReqMsg
            {
                User = new User
                {
                    Alias = user.Alias,
                    UserId = user.Id,
                    CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    Role = user.RoleId,
                    IsActive = true,
                    IsActiveGroup = true
                }
            }).IsSuccess;
        }

        internal bool DeleteUser(Users user)
        {
            return WFService.DeleteUserInAGroup(new DeleteUserReqMsg
            {
                User = new User
                {

                    Alias = user.Alias,
                    CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    UserId = user.Id

                }
            }).IsSuccess;
        }


        internal bool DeleteGroup(Groups g)
        {            
            return WFService.DeleteGroup(new DeleteGroupReqMsg
            {
                CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                GroupId = g.CategoryId
            
            }).IsSuccess;
        }

        internal void GetAllUsers()
        {
            string companyId = ConfigurationManager.AppSettings["Company"];
            WFService.GetAllUsers(companyId);
        }
        [EventSubscription(Constants.EventTopicNames.RefreshGroupUsers, ThreadOption.UserInterface)]
        public void RefreshCategories(object sender, EventArgs e)
        {
            this.View.RefreshGroupUsers();
        }

        [EventSubscription(Constants.EventTopicNames.RefreshExplorerCategories,ThreadOption.UserInterface)]
        public void RefreshCategories(object sender, EventArgs<String> e)
        {
            this.View.RefershCategory(e.Data);
        }
    }

    public class UserComparer : IEqualityComparer<Users>
    {

        public bool Equals(Users x, Users y)
        {
            if (x.Alias == y.Alias)
                return true;
            return false;
        }

        public int GetHashCode(Users obj)
        {
            return default(int);
        }
    }
}
