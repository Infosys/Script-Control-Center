/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Message;
using client = Infosys.WEM.Client;

namespace Infosys.WEM.AutomationActivity.Designers.Services
{

    internal class WFService   {

       

        WFService()
        {

        }

        internal static GetAllUsersResMsg GetUsers(string alias, string securityServiceUrl,string companyId)
        {
            GetAllUsersResMsg responseObj = null;
            client.SecurityAccess access = new client.SecurityAccess(securityServiceUrl);
            responseObj = access.ServiceChannel.GetUsers(alias,companyId);
            return responseObj;
        }

        internal static IsSuperAdminResMsg IsSuperAdmin(string alias, string companyId, string securityServiceUrl)
        {
            IsSuperAdminResMsg responseObj = null;
            client.SecurityAccess access = new client.SecurityAccess(securityServiceUrl);
            responseObj = access.ServiceChannel.IsSuperAdmin(alias, companyId);         
            return responseObj;
        }


    }
}
