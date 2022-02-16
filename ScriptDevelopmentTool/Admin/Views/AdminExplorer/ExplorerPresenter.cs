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
using Infosys.WEM.SecureHandler;

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
    public class ExplorerPresenter : Presenter<IExplorer>
    {
        [EventPublication(EventTopicNames.ShowGroupDetails, PublicationScope.Global)]
        public event EventHandler<EventArgs<Groups>> ShowGroupDetails;

        [EventPublication(EventTopicNames.ShowSemanticCluster, PublicationScope.Global)]
        public event EventHandler<EventArgs<SemanticGroup>> ShowSemanticCluster;


        [EventPublication(EventTopicNames.CategoryDeleted, PublicationScope.Global)]
        public event EventHandler<EventArgs<int>> CategoryDeleted;


        string _companyId;
        internal bool ShowExplorer {get;set;}

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        protected override void CloseView()
        {
            base.CloseView();
        }

        internal void GetCategories()
        {
            this.ShowExplorer = true;
            string companyId = ConfigurationManager.AppSettings["Company"];
            // var groups = WFScriptService.GetAllCategories(companyId);

            var groups = WFCommonService.GetAllCategory(companyId,Constants.Application.Module);

            this.View.Groups = groups.Categories.Select(g => new Groups
            {
                Description = g.Description,
                CategoryId = g.CategoryId,
                Name = g.Name,
                ParentId = g.ParentId.GetValueOrDefault(),
                CompanyId = g.CompanyId,
                ModuleID=g.ModuleID.ToString()
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

        internal void GetClusters()
        {
            string companyId = ConfigurationManager.AppSettings["Company"];
            var clusters = WFService.GetAllClusters(companyId);

            if (clusters.Nodes != null)
            {
                this.View.Clusters = clusters.Nodes.Select(n => new SemanticGroup
                {

                    Description = n.Description,
                    Id = n.Id,
                    Name = n.Name,
                    Priority = n.Priority,
                    ActiveNodes = GetNodes(n.Id)

                }).ToList();
            }
        }

        private List<Nodes> GetNodes(string clusterId)
        {
            var response = WFService.GetAllNodesByCluster(clusterId);
            List<Nodes> nodes = new List<Nodes>();
            response.Nodes.ForEach(n => nodes.Add(new Nodes { 
                CompanyId =n.CompanyId.ToString(),
                Domain = n.HostMachineDomain,
                DotNetVersion = n.DotNetVersion,
                ExecutionEngineSupported = n.ExecutionEngineSupported,
                HttpPort = n.HttpPort,
                Is64Bit = n.Is64Bit,
                Name = n.HostMachineName,
                OSVersion = n.OSVersion,
                State = n.State.ToString(),
                TcpPort = n.TcpPort,
                WorkflowServiceVersion = n.WorkflowServiceVersion
            }));
            
            return nodes;
        }

        private void CheckAccess()
        {
            var currentUser = this.WorkItem.RootWorkItem.Items["CurrentUser"] as List<Infosys.ATR.Entities.Users>;

            string companyID = ConfigurationManager.AppSettings["Company"];
            var allUsers = WFService.GetAllUsers(companyID);

            currentUser = allUsers.Users.Where(u => u.Alias.Equals(currentUser[0].Alias,StringComparison.InvariantCultureIgnoreCase)).Select(n => new Infosys.ATR.Entities.Users
            {
                Alias = n.Alias,
                DisplayName = n.DisplayName,
                CategoryId = n.CategoryId,
                Id = n.UserId,
                Role = Enum.GetName(typeof(Infosys.ATR.Entities.Roles), n.Role)
            }).ToList();

            var userCategory = currentUser.Where(u => u.Role == "Manager").Select(c => c.CategoryId).ToList();

            if (currentUser != null && currentUser.Count > 0)
            {
                var categories = this.View.Groups;
                this.View.Groups = categories.Where(c => userCategory.Contains(c.CategoryId)).ToList();

                if (this.View.Groups == null || this.View.Groups.Count == 0)
                {
                    this.ShowExplorer = false;
                    System.Windows.Forms.MessageBox.Show("Oops ! You are not part of any group as a Manager", "IAP", System.Windows.Forms.MessageBoxButtons.OK,
                   System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            else
            {
                this.ShowExplorer = false;
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

        internal void GetAnyUsers(Groups g)
        {
            _companyId = ConfigurationManager.AppSettings["Company"];
            var roles = GetAllRoles();
            var users = WFService.GetAnyUsers(g.CategoryId.ToString(), _companyId);

            this.View.Users = new System.ComponentModel.BindingList<Users>(
                users.Users.Where(u => u.CategoryId == g.CategoryId).Select(u => new Users
                {
                    Delete = Image.FromFile(@"Images\remove.png"),
                    DL = u.IsDL == true ? Image.FromFile(@"Images\dl.png") : Image.FromFile(@"Images\user.jpg"),
                    Alias = u.Alias,
                    Id = u.UserId,
                    RoleId = u.Role,
                    CategoryId = u.CategoryId,
                    CompanyId = u.CompanyId
                }).ToList());
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
                    Alias = Infosys.WEM.SecureHandler.SecurePayload.Secure(user.Alias, "IAP2GO_SEC!URE"),
                    UserId = user.Id,
                    CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    Role = user.RoleId,
                    IsActive = true
                }
            }).IsSuccess;
        }

        internal bool DeleteUser(Users user)
        {
            return WFService.DeleteUser(new DeleteUserReqMsg
            {
                User = new User
                {

                    Alias = SecurePayload.Secure(user.Alias, "IAP2GO_SEC!URE"),
                    CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    UserId = user.Id

                }
            }).IsSuccess;
        }

        //internal bool DeleteGroup(Groups g)
        //{
        //    return WFService.DeleteGroup(new DeleteGroupReqMsg
        //    {
        //        CompanyId = g.CompanyId,
        //        GroupId = g.GroupId
        //    }).IsSuccess;
        //}

        internal bool DeleteGroup(Groups g)
        {
            List<Category> categories = new List<Category>();

            Category c = new Category();
            c.CategoryId = g.CategoryId;
            c.Description = g.Description;
            c.Name = g.Name;
            c.ParentId = g.ParentId;
            categories.Add(c);
            return WFScriptService.DeleteCategories(new DeleteCategoryReqMsg
            {
                Categories = categories
            }).IsSuccess;
        }

        internal void GetAllUsers()
        {
            string companyId = ConfigurationManager.AppSettings["Company"];
            WFService.GetAllUsers(companyId);
        }

        [EventSubscription(Constants.EventTopicNames.RefreshCategories, ThreadOption.UserInterface)]
        public void RefreshCategories(object sender, EventArgs<String> e)
        {
            this.View.RefreshCategory(e.Data);
        }

        [EventSubscription(Constants.EventTopicNames.RefreshUsers, ThreadOption.UserInterface)]
        public void RefreshCategories(object sender, EventArgs e)
        {
            this.View.RefreshUsers();
        }


        [EventSubscription(Constants.EventTopicNames.UpdateSematicTree, ThreadOption.UserInterface)]
        public void RefreshCategories(object sender, EventArgs<SemanticGroup> e)
        {
            this.View.RefreshSemanticTree();
        }

        [EventSubscription(Constants.EventTopicNames.AddGroupUsers, ThreadOption.UserInterface)]
        public void AddGroupUsers(object sender, EventArgs<List<int>> e)
        {
            if (e.Data != null && e.Data.Count > 0)
            {
                var g = this.View.SelectedNode.Tag as Groups;
                //var result = WFService.UpdateUserCategory(new UpdateUserCategoryReqMsg { CategoryId = g.CategoryId, Groups = e.Data });

               // this.View.GetAllNodes(g);

                UpdateUserResMsg result;

                g.Children.ForEach(c =>
                {
                    result = WFService.UpdateUserCategory(new UpdateUserCategoryReqMsg { CategoryId = c, Groups = e.Data });
                });

                g.Parents.ForEach(c =>
                {
                    result = WFService.UpdateUserCategory(new UpdateUserCategoryReqMsg { CategoryId = g.CategoryId, Groups = e.Data });
                });



                this.View.DisplayMessage("Group added successfully");
            }
            else
            {
                this.View.DisplayMessage("No Groups selected or no users present in the selected group");
            }
        }


        internal void CategoryDeleted_Handler(int catid)
        {
            CategoryDeleted(this, new EventArgs<int>(catid));
        }

        internal void ShowSemanticCluster_Handler(SemanticGroup sc)
        {           

            ShowSemanticCluster(this, new EventArgs<SemanticGroup>(sc));
        }

       
    }
}
