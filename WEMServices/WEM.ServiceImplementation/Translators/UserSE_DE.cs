/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using Infosys.WEM.Infrastructure.Common;
using SE = Infosys.WEM.SecurityAccess.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators
{
    public class UserSE_DE
    {
        public static Resource.Entity.User UserSEToUserDE(SE.User source)
        {
            DE.User destination = null;
            if (source != null)
            {
                destination = new DE.User();
                destination.Id = source.UserId;
                //destination.Timestamp = source.Timestamp;
                destination.Role = source.Role;
                destination.IsActive = source.IsActive;
                destination.LastModifiedBy = Utility.GetLoggedInUser();// source.LastModifiedBy;
                destination.CategoryId = source.CategoryId;
                destination.DisplayName = source.DisplayName;
                destination.CreatedBy = Utility.GetLoggedInUser();// source.CreatedBy;
                destination.CompanyId = source.CompanyId;
                destination.Alias = source.Alias;
                destination.IsDL = source.IsDL;
                destination.GroupId = source.GroupId;
                destination.IsActiveGroup = source.IsActiveGroup;
            }
            return destination;
        }

        public static SE.User UserDEToUserSE(DE.User source)
        {
            SE.User destination = null;
            if (source != null)
            {
                destination = new SE.User();
                destination.UserId = source.Id;
                //destination.Timestamp = source.Timestamp;
                destination.Role = source.Role;
                destination.IsActive = source.IsActive == null ? false : true;
               // destination.LastModifiedBy = Utility.GetLoggedInUser();// source.LastModifiedBy;
                destination.CategoryId = source.CategoryId == null ? 0 : (int)source.CategoryId;
                destination.DisplayName = source.DisplayName;
               // destination.CreatedBy = Utility.GetLoggedInUser(); // source.CreatedBy;
                destination.CompanyId = source.CompanyId;
                destination.Alias = source.Alias;
                destination.IsDL = source.IsDL.GetValueOrDefault();
                destination.GroupId = source.GroupId.GetValueOrDefault();
                destination.IsActiveGroup = source.IsActiveGroup.GetValueOrDefault();
            }
            return destination;
        }

        public static List<SE.User> UserDEListtoSEList(List<DE.User> userDEList)
        {
            List<SE.User> userSEList = null;
            if (userDEList != null)
            {
                userSEList = new List<SE.User>();
                userDEList.ForEach(de =>
                {
                    userSEList.Add(UserDEToUserSE(de));
                });
            }
            return userSEList;
        }
    }
}
