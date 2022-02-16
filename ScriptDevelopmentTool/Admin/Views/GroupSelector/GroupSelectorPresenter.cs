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
//using Infosys.WEM.Scripts.Service.Contracts.Data;
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
    public class GroupSelectorPresenter : Presenter<IGroupSelector>
    {
        [EventPublication(EventTopicNames.AddGroupUsers,PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<List<int>>> AddGroupsUsers;
        
        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        public override void OnCloseView()
        {
            base.CloseView();
        }

        internal void GetAllGroups()
        {
            var _companyId = ConfigurationManager.AppSettings["Company"];
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

        internal void AddGroupUsers_Handler(List<int> groups)
        {
            AddGroupsUsers(this,new EventArgs<List<int>>(groups));
        }
    }
}
