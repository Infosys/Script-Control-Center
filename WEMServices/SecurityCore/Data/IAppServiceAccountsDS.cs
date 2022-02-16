/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using Infosys.WEM.Infrastructure.SecurityCore.Data.Entity;
namespace Infosys.WEM.Infrastructure.SecurityCore.Data
{
    public partial interface IAppServiceAccountsDS
    {
        AppServiceAccounts GetOne(System.String partitionKey, System.String rowKey);
        List<AppServiceAccounts> GetAll();
        AppServiceAccounts Insert(AppServiceAccounts appServiceAccounts);
        List<AppServiceAccounts> BatchInsert(List<AppServiceAccounts> list);
        AppServiceAccounts Update(AppServiceAccounts appServiceAccounts);
        List<AppServiceAccounts> BatchUpdate(List<AppServiceAccounts> list);
        bool Delete(System.String partitionKey, System.String rowKey);
        bool BatchDelete(List<AppServiceAccounts> list);
        List<AppServiceAccounts> GetMany(System.String partitionKey);
    }
}
