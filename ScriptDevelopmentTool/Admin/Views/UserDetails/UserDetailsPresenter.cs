/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

using Infosys.ATR.Admin.Constants;
using Infosys.ATR.Admin.Services;
using Infosys.ATR.Admin.Entities;

using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using Infosys.WEM.SecurityAccess.Contracts.Message;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Infosys.ATR.Admin.Services;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Infosys.ATR.Admin.Views
{
    public class UserDetailsPresenter : Presenter<IUserDetails>
    {
        [EventPublication(EventTopicNames.RefreshUsers,PublicationScope.WorkItem)]
        public event EventHandler<EventArgs> RefreshUsers;

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        internal void Close()
        {
            base.CloseView();
        }

        internal Users GetDetails(string alias)
        {
            return AD.GetUserDetails(alias);
        }

        internal void GetRoles()
        {
            string companyId = ConfigurationManager.AppSettings["Company"];
            var roles = WFService.GetAllRoles(companyId);

            this.View.Roles = roles.Roles.Select(r => new UserRoles
            {
                Description = r.Description,
                Id = r.RoleId,
                Name = r.Name,                
            }).ToList();
        }

        internal bool AddUser(Users users)
        {
            return WFService.AddUser(new AddUserReqMsg
            {
                User = new User { 
                    Alias = users.Alias,
                    DisplayName = users.DisplayName,
                    CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                    Role = users.RoleId,
                    CategoryId = users.CategoryId,
                 //   CreatedBy = "IAPAdmin",
                    IsDL = users.IsDL
                }
            }).IsSuccess;
        }

        public void RefreshUsers_Handler()
        {
            RefreshUsers(this, new EventArgs());
        }
    }
}
