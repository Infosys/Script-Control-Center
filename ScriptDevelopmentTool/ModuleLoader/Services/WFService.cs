/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;
using System.Configuration;
using Infosys.WEM.Client;
using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using Infosys.WEM.SecurityAccess.Contracts.Message;
using Infosys.WEM.SecureHandler;
namespace Infosys.ATR.ModuleLoader.Services
{


    internal class WFService
    {      

        WFService()
        {

        }


        internal static GetAllUsersResMsg GetUsers(string alias, string companyId)
        {
            GetAllUsersResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            alias = SecurePayload.Secure(alias, "IAP2GO_SEC!URE");            
            responseObj = access.ServiceChannel.GetUsers(alias, companyId);
            return responseObj;
        }

        internal static IsSuperAdminResMsg IsSuperAdmin(string alias,string companyId)
        {
            IsSuperAdminResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.IsSuperAdmin(alias, companyId);
            return responseObj;
        }

       
    }
}
