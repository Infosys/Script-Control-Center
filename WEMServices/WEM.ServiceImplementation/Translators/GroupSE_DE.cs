/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.Infrastructure.Common;
using SE = Infosys.WEM.SecurityAccess.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public static class GroupSE_DE
    {
        public static SE.Group GroupDEToGroupSE(DE.Group source)
        {
            SE.Group destination = null;
            if (source != null)
            {
                destination = new SE.Group();
                destination.GroupId = source.Id;
                destination.Description = source.Description.Trim();             
                destination.CompanyID = source.CompanyId;
                destination.IsActive = source.IsActive.HasValue;
                destination.Name = source.Name.Trim();               
                destination.ParentId = source.ParentId.GetValueOrDefault();
            }
            return destination;
        }

        public static DE.Group GroupSEToGroupDE(SE.Group source)
        {
            DE.Group destination = null;
            if (source != null)
            {
                destination = new DE.Group();
                destination.Id = source.GroupId;
                destination.Description = source.Description.Trim();
                destination.CompanyId = source.CompanyID;
                destination.IsActive = source.IsActive;
                destination.Name = source.Name.Trim();
                destination.CreatedBy = Utility.GetLoggedInUser();// source.CreatedBy;
                destination.ParentId = source.ParentId;              
            }
            return destination;
        }

        public static List<SE.Group> GroupDEListtoSEList(List<DE.Group> groupDEList)
        {
            List<SE.Group> groupSEList = null;
            if (groupDEList != null)
            {
                groupSEList = new List<SE.Group>();
                groupDEList.ForEach(de =>
                {
                    groupSEList.Add(GroupDEToGroupSE(de));
                });
            }
            return groupSEList;
        }
    }
}
