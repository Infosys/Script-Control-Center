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
using System.ServiceModel.Web;
using Infosys.WEM.Client;
using Infosys.WEM.Scripts.Service.Contracts;
using Infosys.WEM.Scripts.Service.Contracts.Data;
using Infosys.ATR.Admin.Entities;
using Infosys.WEM.Service.Common.Contracts;
using Infosys.WEM.Service.Common.Contracts.Message;

namespace Infosys.ATR.Admin.Services
{
    internal class WFScriptService
    {
        WFScriptService()
        {

        }

        internal static GetAllCategoriesResMsg GetAllCategories(string companyId)
        {
            GetAllCategoriesResMsg responseObj = null;

            CommonRepository common = new CommonRepository();
            responseObj = common.ServiceChannel.GetAllCategoriesByCompany(companyId,"Script");

            return responseObj;
        }

        internal static DeleteCategoryResMsg DeleteCategories(DeleteCategoryReqMsg value)
        {
            DeleteCategoryResMsg responseObj = null;         

            CommonRepository common = new CommonRepository();
            responseObj = common.ServiceChannel.DeleteCategory(value);

            return responseObj;
        }



    }
}
