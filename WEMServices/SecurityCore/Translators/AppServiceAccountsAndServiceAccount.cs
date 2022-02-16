/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Data;

namespace Infosys.WEM.Infrastructure.SecurityCore.Translators
{

    public sealed partial class AppServiceAccountsAndServiceAccount
    {
        private AppServiceAccountsAndServiceAccount()
        {
        }

        public static Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount TranslateAppServiceAccountsDEToServiceAccountBE(Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts source)
        {
            Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount destination = null;
            if (source != null)
            {
                destination = new Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount();

                destination.CipherKeyIndex = source.CipherKeyIndex;
                destination.CompanyId = source.CompanyId;
                destination.CreatedOn = source.CreatedOn;
                destination.Description = source.Description;
                destination.Domain = source.Domain;
                destination.IsActive = source.IsActive;
                destination.LastModifiedOn = source.LastModifiedOn;
                destination.PartitionKey = source.PartitionKey;
                destination.RowKey = source.RowKey;
                destination.ServiceId = source.ServiceId;
                destination.ServicePassword = source.ServicePassword;
                destination.Timestamp = source.Timestamp;

            }
            return destination;

        }
        public static Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts TranslateServiceAccountBEToAppServiceAccountsDE(Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount source)
        {
            Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts destination = null;
            if (source != null)
            {
                destination = new Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts();

                destination.CipherKeyIndex = source.CipherKeyIndex;
                destination.CompanyId = source.CompanyId;
                destination.CreatedOn = source.CreatedOn;
                destination.Description = source.Description;
                destination.Domain = source.Domain;
                destination.IsActive = source.IsActive;
                destination.LastModifiedOn = source.LastModifiedOn;
                destination.PartitionKey = source.PartitionKey;
                destination.RowKey = source.RowKey;
                destination.ServiceId = source.ServiceId;
                destination.ServicePassword = source.ServicePassword;


            }
            return destination;

        }
        public static System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount> TranslateAppServiceAccountsListDEToServiceAccountListBE(System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts> source)
        {
            System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount> destination = new System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount>();
            foreach (Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts item in source)
            {
                destination.Add(TranslateAppServiceAccountsDEToServiceAccountBE(item));
            }
            return destination;

        }
        public static System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts> TranslateServiceAccountListBEToAppServiceAccountsListDE(System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount> source)
        {
            System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts> destination = new System.Collections.Generic.List<Infosys.WEM.Infrastructure.SecurityCore.Data.Entity.AppServiceAccounts>();
            foreach (Infosys.WEM.Infrastructure.SecurityCore.Business.Entity.ServiceAccount item in source)
            {
                destination.Add(TranslateServiceAccountBEToAppServiceAccountsDE(item));
            }
            return destination;

        }

    }

}
