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
    public class ScriptExecuteResponse
    {
        [DataMember]
        public Guid TransactionId { get; set; }
        [DataMember]
        public string CurrentState { get; set; }
        [DataMember]
        public string SuccessMessage { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string ComputerName { get; set; } //will be the machine on which command gets executed
        [DataMember]
        public string InputCommand { get; set; }
        [DataMember]
        public List<Parameter> OutParameters { get; set; }
        [DataMember]
        public string LogData { get; set; }
        [DataMember]
        public string SourceTransactionId  { get; set; }
        [DataMember]
        public bool IsNotified { get; set; }
        [DataMember]
        public string NotificationRemarks { get; set; }

    }
}
