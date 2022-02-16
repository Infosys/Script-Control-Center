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

using Infosys.ATR.Admin.Constants;
using Infosys.ATR.Admin.Services;
using Infosys.ATR.Admin.Entities;

using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using Infosys.WEM.SecurityAccess.Contracts.Message;

using Infosys.WEM.Scripts.Service.Contracts;
using Infosys.WEM.Scripts.Service.Contracts.Message;

using Infosys.WEM.Service.Common.Contracts;
using Common = Infosys.WEM.Service.Common.Contracts.Data;
using Infosys.WEM.Service.Common.Contracts.Message;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.WEM.Service.Common.Contracts.Data;

namespace Infosys.ATR.Admin.Views
{
    public class GroupExplorerDetailsPresenter : Presenter<IGroupExplorerDetails>
    {
        [EventPublication(EventTopicNames.RefreshExplorerCategories, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs> RefreshGroupsCategories;

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        public override void OnCloseView()
        {
            base.CloseView();
        }

        [EventSubscription(EventTopicNames.ShowGroupExplorerDetails,ThreadOption.UserInterface)]
        public void ShowGroupDetails(object sender, EventArgs<Groups> g)
        {
            this.View.Groups = g.Data;
            this.View.Show();
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

        internal List<Entities.Groups> GetAllGroups()
        {
            var groups = WFService.GetallGroups(ConfigurationManager.AppSettings["Company"]);

            return groups.Groups.Select(g => new Groups
            {
                CompanyId = g.CompanyID,
                Description = g.Description,
                CategoryId = g.GroupId,
                Name = g.Name,
                ParentId = g.ParentId,
                Roles = g.Roles                
            }).ToList();
        }

        internal bool AddUser(Infosys.ATR.Entities.Users users)
        {           

            return WFService.AddUserToAGroup(new AddUserReqMsg
            {
                User = new User
                {
                    Alias = users.Alias,
                    DisplayName = users.DisplayName,
                    CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    Role = (int) Enum.Parse(typeof(Infosys.ATR.Entities.Roles),users.Role),
                    GroupId = users.GroupId,
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
            return WFService.AddGroups(new AddGroupReqMsg
            {
                Group = new Group {
                    CompanyID = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                  //  CreatedBy = "IAPAdmin",
                    Description = g.Description,
                    IsActive = true,
                    Name = g.Name,
                    ParentId = g.ParentId,                    
                }
            }).IsSuccess;
        }

        internal bool UpdateGroups(Groups g)
        {           

            return WFService.UpdateGroups(new UpdateGroupReqMsg
            {
                Group = new Group {
                    //CreatedBy = "IAPAdmin",
                    Description = g.Description,
                    ParentId = g.ParentId,
                    GroupId = g.CategoryId,
                    Name = g.Name,
                    CompanyID =  Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    IsActive =true
                }
            }).IsSuccess;
        }

        internal void RefreshGroupsCategory_Handler(string node)
        {
            RefreshGroupsCategories(this, new EventArgs<String>(node));
        }

        //internal bool UpdateGroups(Groups g)
        //{
        //    return WFService.UpdateGroups(new UpdateGroupReqMsg
        //    {
        //        Group = new Group
        //        {
        //            CompanyID = g.CompanyId,
        //            CreatedBy = "IAPAdmin",
        //            Description = g.Description,
        //            ParentId = g.ParentId,
        //            Name = g.Name,
        //            Roles = g.Roles,
        //            IsActive = true,
        //             GroupId = g.GroupId
        //        }
        //    }).IsSuccess;
        //}

     
    }
}
