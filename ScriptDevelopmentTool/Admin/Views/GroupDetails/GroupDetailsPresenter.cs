/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel;
using Infosys.ATR.Admin.Constants;
using Infosys.ATR.Admin.Services;
using Infosys.ATR.Admin.Entities;

using Infosys.WEM.SecureHandler;

using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using Infosys.WEM.SecurityAccess.Contracts.Message;

using Infosys.WEM.Scripts.Service.Contracts;
using Infosys.WEM.Scripts.Service.Contracts.Message;

using Infosys.WEM.Node.Service.Contracts;
using Infosys.WEM.Node.Service.Contracts.Message;

using Infosys.WEM.Service.Common.Contracts;
using Common = Infosys.WEM.Service.Common.Contracts.Data;
using Infosys.WEM.Service.Common.Contracts.Message;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.WinForms;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.WEM.Service.Common.Contracts.Data;

namespace Infosys.ATR.Admin.Views
{
    public class GroupDetailsPresenter : Presenter<IGroupDetails>
    {
        [EventPublication(EventTopicNames.RefreshCategories, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> RefreshCategories;

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        public override void OnCloseView()
        {
            base.CloseView();
        }

        [EventSubscription(EventTopicNames.ShowGroupDetails,ThreadOption.UserInterface)]
        public void ShowGroupDetails(object sender, EventArgs<Groups> g)
        {
            this.View.Groups = g.Data;
            this.View.Show();
        }

        [EventSubscription(EventTopicNames.CategoryDeleted, ThreadOption.UserInterface)]
        public void DeleteSemanticGroup(object sender, EventArgs<int> e)
        {
            foreach (SemanticGroup sg in this.View.Selected)
            {
                WFService.DeleteSemanticCategory(new DeleteSemanticCategoryReqMsg
                {
                    SemanticCategory = new WEM.Node.Service.Contracts.Data.SemanticCategory
                    {

                        CategoryId = e.Data,
                        ClusterId = sg.Id,
                        ClusterName = sg.Name,
                        IsActive = false

                    }

                });
            }
        }

        internal void GetRoles()
        {
            string companyId = ConfigurationManager.AppSettings["Company"];

            var roles = WFService.GetAllRoles(companyId);

            this.View.Roles = roles.Roles.Select(r => new UserRoles {
                Description =r.Description,
                Id = r.RoleId,
                Name = r.Name
            }).ToList();

        }

        internal List<Common.Category> GetAllCategories()
        {
            return WFCommonService.GetAllCategory(ConfigurationManager.AppSettings["Company"], Constants.Application.Module).Categories;
        }

        internal List<Common.Module> GetAllModules()
        {
            return WFCommonService.GetAllModules().Module;
        }

        internal bool AddUser(Infosys.ATR.Entities.Users users)
        {           

            return WFService.AddUser(new AddUserReqMsg
            {
                User = new User
                {
                    Alias = SecurePayload.Secure(users.Alias, "IAP2GO_SEC!URE"),
                    DisplayName = users.DisplayName,
                    CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    Role = (int) Enum.Parse(typeof(Infosys.ATR.Entities.Roles),users.Role),
                    CategoryId = users.GroupId,
                  //  CreatedBy = "IAPAdmin"
                }
            }).IsSuccess;
        }


        //internal bool AddGroups(Groups g)
        //{
        //    return WFService.AddGroups(new AddGroupReqMsg
        //    {
        //        Group = new Group { 
        //            CompanyID = g.CompanyId,
        //            CreatedBy = "IAPAdmin",
        //            Description = g.Description,
        //            ParentId = g.ParentId,
        //            Name = g.Name,
        //            Roles = g.Roles,
        //            IsActive = true
        //        }
        //    }).IsSuccess;
        //}

        internal bool AddCategory(Groups g)
        {
            Category c = new Category {                
                //CreatedBy = "IAPAdmin",
                Description = g.Description,
                ParentId = g.ParentId,
                Name = g.Name,
                CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                ModuleID = Convert.ToInt32 (g.ModuleID)
            };

            List<Category> categories = new List<Category>();
            categories.Add(c);

            return WFService.AddCategory(new Infosys.WEM.Service.Common.Contracts.Message.AddCategoryReqMsg
            {
                Categories = categories
            }).IsSuccess;
        }

        internal bool UpdateCategory(Groups g)
        {
            Category c = new Category
            {
                //CreatedBy = "IAPAdmin",
                Description = g.Description,
                ParentId = g.ParentId,
                CategoryId = g.CategoryId,
                Name = g.Name,
                NewName = g.NewName,
                CompanyId = g.CompanyId,
                ModuleID=Convert.ToInt32(g.ModuleID),
                IsDeleted = false
            };

            List<Category> categories = new List<Category>();
            categories.Add(c);

            return WFService.UpdateCategory(new Infosys.WEM.Service.Common.Contracts.Message.UpdateCategoryReqMsg
            {
                Categories = categories
            }).IsSuccess;
        }

        internal void RefreshCategory_Handler(string node)
        {
            RefreshCategories(this, new EventArgs<String>(node));
        }

        internal void ShowSelector()
        {
            var selector = this.WorkItem.SmartParts.Get("GroupSelector") as GroupSelector;
            if (selector == null)
            {
                selector = this.WorkItem.SmartParts.AddNew<GroupSelector>("GroupSelector") as GroupSelector;
            }

            selector.Initialize();

            WindowSmartPartInfo sp = new WindowSmartPartInfo();
            sp.MaximizeBox = false;
            sp.MinimizeBox = false;
            sp.Modal = true;
            sp.Height = 500;
            sp.Width = 500;
            sp.Title = "Group Selector";        
            
            this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.ModalWindows].Show(selector, sp);

        }

        internal void AddSemanticCategory(int catId, List<SemanticGroup> selected)
        {
            foreach (SemanticGroup sg in selected)
            {
                AddSemanticCategory(catId, sg);
            }
        }

        internal void AddSemanticCategory(int catId,System.ComponentModel.BindingList<SemanticGroup> selected)
        {
            foreach (SemanticGroup sg in selected)
            {
                AddSemanticCategory(catId, sg);
            }
        }

        internal void AddSemanticCategory(int catId,SemanticGroup sg)
        {
            WFService.AddSemanticCategory(new AddSemanticCategoryReqMsg
            {
                SemanticCategory = new WEM.Node.Service.Contracts.Data.SemanticCategory
                {
                    CategoryId = catId,
                    ClusterId = sg.Id,
                    ClusterName = sg.Name,
                    //CreatedBy = System.Threading.Thread.CurrentPrincipal.Identity.Name,
                    IsActive = sg.IsActive
                }
            });
        }


        internal void UpdateSemanticCategory(int catId, System.ComponentModel.BindingList<SemanticGroup> selected)
        {
            foreach (SemanticGroup sg in selected)
            {
                if (sg.IsActive == true)
                    AddSemanticCategory(catId, sg);
                else
                {
                    WFService.UpdateSemanticCategory(new UpdateSemanticCategoryReqMsg
                    {
                        SemanticCategory = new WEM.Node.Service.Contracts.Data.SemanticCategory { 
                            CategoryId = catId,
                            ClusterId = sg.Id,
                            ClusterName = sg.Name,
                            //CreatedBy = System.Threading.Thread.CurrentPrincipal.Identity.Name,
                            IsActive = sg.IsActive
                        }
                    });
                }                                 
            }
        }


        internal BindingList<SemanticGroup> GetSemanticCategories(int catId)
        {
            var response = WFService.GetAllClustersByCategory(catId);

            BindingList<SemanticGroup> clusters = new BindingList<SemanticGroup>();

            if (response.Nodes != null)
            {
                response.Nodes.ForEach(n =>
                {
                    SemanticGroup sg = new SemanticGroup();
                    sg.Name = n.Name;
                    sg.Id = n.Id;
                    sg.IsActive = true;
                    clusters.Add(sg);
                });
            }
            return clusters;                                  
        }

        internal List<SemanticGroup> GetSemanticClusters(int catId)
        {
            var response = WFService.GetAllClustersByCategory(catId);
            List<SemanticGroup> grps = new List<SemanticGroup>();
            if (response.Nodes != null)
            {
                response.Nodes.ForEach(n => grps.Add(new SemanticGroup
                {

                    Id = n.Id,
                    Name = n.Name
                    
                }));
            }

            return grps;
        }
    }
}
