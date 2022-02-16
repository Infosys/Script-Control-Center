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
using Infosys.WEM.Client;
using Infosys.WEM.Service.Common.Contracts;
using Infosys.WEM.Service.Common.Contracts.Data;
using Infosys.WEM.Service.Common.Contracts.Message;

namespace Infosys.ATR.Admin.Services
{
    internal class WFCommonService
    {        

        WFCommonService()
        {

        }

        internal static GetAllCategoriesResMsg GetAllCategory(string companyId,string module)
        {
            CommonRepository common = new CommonRepository();
            return common.ServiceChannel.GetAllCategoriesByCompany(companyId,module);
        }

        internal static GetAllModulesResMsg GetAllModules()
        {
            CommonRepository common = new CommonRepository();
            return common.ServiceChannel.GetAllModules();
        }
    }
}
