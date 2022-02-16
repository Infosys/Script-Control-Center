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
using Infosys.WEM.Resource.DataAccess;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Business.Component.Translators;
using BE = Infosys.WEM.Business.Entity;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Business.Component
{
    public class ManageUsers
    {
        public void UpdateCategory(int categoryId, int groupId)
        {
            UserDS userDS = new UserDS();

            var users = userDS.GetAll().ToList();

            var userGroups = users.Where(u => u.GroupId == groupId && u.IsActiveGroup.GetValueOrDefault() == true);

            if (userGroups != null)
            {
                var users1 = userGroups.ToList();

                users1.ForEach(u =>
                {
                    if (u.CategoryId.GetValueOrDefault() == 0)
                    {
                        var existing = users.FirstOrDefault(us => us.CompanyId == u.CompanyId &&
                        us.Alias == u.Alias && us.CategoryId == categoryId && us.IsActive == true);

                        if (existing == null)
                        {
                            u.CategoryId = categoryId;
                            userDS.Update(u);
                        }
                    }
                    else
                    {
                        u.CategoryId = categoryId;
                        userDS.Insert(u);
                    }

                });
            }
        }       
    }
}
