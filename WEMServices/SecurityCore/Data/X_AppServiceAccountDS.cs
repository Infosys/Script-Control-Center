/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.WEM.Infrastructure.SecurityCore.Data.Entity;

namespace Infosys.WEM.Infrastructure.SecurityCore.Data
{
    partial class AppServiceAccountsDS
    {
    }
    
    /// <summary>
    /// Implements extension methods for AppServiceAccounts data entity
    /// </summary>
    public static class AppServiceAccountsExtensions
    {
        /// <summary>
        /// Operation to generate partition key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static AppServiceAccounts GeneratePartitionKey(this AppServiceAccounts entity)
        {
            
            entity.PartitionKey = entity.CompanyId.ToString();
            return entity;
        }

        /// <summary>
        /// Operation to generate rowkey
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static AppServiceAccounts GenerateRowKey(this AppServiceAccounts entity)
        {

            entity.RowKey = entity.Domain + @"\" + entity.ServiceId;
            return entity;

        }

        /// <summary>
        /// Operation to generate partition key
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static AppServiceAccounts GeneratePartitionKeyAndRowKey(this AppServiceAccounts entity)
        {
            return entity.GeneratePartitionKey().GenerateRowKey();
        }

    }
}
