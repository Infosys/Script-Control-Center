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
using BE = Infosys.WEM.Business.Entity;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Business.Component.Translators
{
    internal class UserBE_DE
    {
        internal static BE.User UserDEToBE(DE.User source)
        {
            BE.User destination = new BE.User();
            destination.Alias = source.Alias;
            destination.CategoryId = source.CategoryId.GetValueOrDefault();
            destination.CompanyId = source.CompanyId;
            destination.CreatedBy = source.CreatedBy;
            destination.DisplayName = source.DisplayName;
            destination.GroupId = source.GroupId;
            destination.IsActive = source.IsActive.GetValueOrDefault();
            destination.IsDL = source.IsDL.GetValueOrDefault();
            destination.LastModifiedBy= source.LastModifiedBy;
            destination.Role = source.Role;
            destination.UserId = source.Id;
            return destination;
        }

        internal static DE.User UserBEToDE(BE.User source)
        {
            DE.User destination = new DE.User();
            destination.Alias = source.Alias;
            destination.CategoryId = source.CategoryId;
            destination.CompanyId = source.CompanyId;
            destination.CreatedBy = source.CreatedBy;
            destination.DisplayName = source.DisplayName;
            destination.GroupId = source.GroupId;
            destination.IsActive = source.IsActive;
            destination.IsDL = source.IsDL;
            destination.LastModifiedBy = source.LastModifiedBy;
            destination.Role = source.Role;
            destination.Id = source.UserId;
            return destination;
        }
    }
}
