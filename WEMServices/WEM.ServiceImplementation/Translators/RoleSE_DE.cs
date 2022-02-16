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
    public static class RoleSE_DE
    {
        internal static DE.Role RoleSEToRoleDE(SE.Role source)
        {
            DE.Role destination = null;
            if (source != null)
            {
                destination = new DE.Role();
                destination.Description = source.Description;
                destination.Id = source.RoleId;
                destination.Name = source.Name;
                destination.IsActive = source.IsActive;
                destination.CreatedBy = Utility.GetLoggedInUser();// source.CreatedBy;
                destination.CompanyId = source.CompanyId;
            }
            return destination;
        }

        internal static List<SE.Role> RoleDEListtoSEList(List<DE.Role> roleDEList)
        {
            List<SE.Role> roleSEList = null;
            if (roleDEList != null)
            {
                roleSEList = new List<SE.Role>();
                roleDEList.ForEach(de =>
                {
                    roleSEList.Add(RoleDEToRoleSE(de));
                });
            }
            return roleSEList;
        }

        public static SE.Role RoleDEToRoleSE(DE.Role source)
        {
            SE.Role destination = null;
            if (source != null)
            {
                destination = new SE.Role();
                destination.Description = source.Description;
                destination.RoleId = source.Id;
                destination.Name = source.Name;
                destination.IsActive = source.IsActive == null ? false : (bool)source.IsActive;
                destination.CompanyId = source.CompanyId;
            }
            return destination;
        }
    }
}
