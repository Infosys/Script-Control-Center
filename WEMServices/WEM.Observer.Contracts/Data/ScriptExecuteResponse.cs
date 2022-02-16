/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Observer.Contracts.Data
{
    [DataContract]
    public class ScriptExecuteResponse
    {
        [DataMember]
        public string TransactionId { get; set; }
        [DataMember]
        public string CurrentState { get; set; }
        [DataMember]
        public string SuccessMessage { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string ComputerName { get; set; }
        [DataMember]
        public string InputCommand { get; set; }
        [DataMember]
        public List<Parameters> OutParameters { get; set; }
        [DataMember]
        public string LogData { get; set; }
        [DataMember]
        public string SourceTransactionId { get; set; }
        [DataMember]
        public bool IsNotified { get; set; }
        [DataMember]
        public string NotificationRemarks { get; set; }



    }

    [DataContract]
    public class Parameters
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public string ParameterValue { get; set; }
    }

    public enum Status
    {
        queued,
        success,
        failed
    };




}
