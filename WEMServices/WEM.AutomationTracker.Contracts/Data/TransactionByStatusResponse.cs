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
using System.Threading.Tasks;

namespace Infosys.WEM.AutomationTracker.Contracts.Data
{
    [DataContract]
    public class TransactionByStatusResponse
    {
        [DataMember]
        public string CompanyId { get; set; }
        [DataMember]
        public Guid TransactionId { get; set; }
        [DataMember]
        public string CurrentState { get; set; }
        [DataMember]
        public string ComputerName { get; set; }
        [DataMember]
        public string SourceTransactionId { get; set; }
        [DataMember]
        public string findbyIdUrl { get; set; }
        [DataMember]
        public string NiaServiceAccount { get; set; }
        [DataMember]
        public string NiaServiceAccPassword { get; set; }
        [DataMember]
        public string casServerUrl { get; set; }
        [DataMember]
        public string casServiceUrl { get; set; }
        [DataMember]
        public string niaEcrScriptExecuteUrl { get; set; }
        [DataMember]
        public string niaEcrFindByActivityIdUrl { get; set; }
        [DataMember]
        public string niaEcrFindAllNodesUrl { get; set; }
        [DataMember]
        public string serviceAreas { get; set; }

    }
}
