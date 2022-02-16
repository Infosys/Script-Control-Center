/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Infosys.WEM.Service.Common.Contracts.Data
{
    public class Company
    {
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string StorageBaseUrl { get; set; }
        [DataMember]
        public string DeploymentBaseUrl { get; set; }
        [DataMember]
        public string RemoteShareUrl { get; set; }
        [DataMember]
        public bool? EnableSecureTransactions { get; set; }
    }
}