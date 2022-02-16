/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Data;

namespace Infosys.WEM.Infrastructure.SecurityCore.Data.Entity
{
    public partial class AppServiceAccounts
    {

        public int CipherKeyIndex
        {
            get;
            set;
        }

        public System.Guid CompanyId
        {
            get;
            set;
        }

        public System.DateTime CreatedOn
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Domain
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public Nullable<System.DateTime> LastModifiedOn
        {
            get;
            set;
        }

        public string PartitionKey
        {
            get;
            set;
        }

        public string RowKey
        {
            get;
            set;
        }

        public string ServiceId
        {
            get;
            set;
        }

        public string ServicePassword
        {
            get;
            set;
        }
        private System.DateTime timestamp;
        public System.DateTime Timestamp
        {
            get
            {
                return timestamp;
            }

        }
    }
}
