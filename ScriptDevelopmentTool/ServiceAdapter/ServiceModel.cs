/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapter
{
    public class ServiceModel
    {      
        public class ServiceRequest
        {
            public string TransactionId { get; set; }
            public int ScriptId { get; set; }
            public int CategoryId { get; set; }
            public string ScriptName { get; set; }
            public List<Parameters> InParameters { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string RemoteServerNames { get; set; }
            public string Domain { get; set; }
            public string ReferenceKey { get; set; }
            public string Path { get; set; }
            public string SEMObserverURL { get; set; }

        }
        public class Parameters
        {
            public string DataType { get; set; }
            public bool IsSecret { get; set; }
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
        }

        public class ServiceResponse
        {
            public string TransactionId { get; set; }
            public string CurrentState { get; set; }
            public string SuccessMessage { get; set; }
            public string ErrorMessage { get; set; }
            public bool IsSuccess { get; set; }
            public string ComputerName { get; set; }
            public string InputCommand { get; set; }
            public List<Parameters> OutParameters { get; set; }
            public string Executedby { get; set; }
            public DateTime ExecutedOn { get; set; }
            public string LogData { get; set; }
        }

        public class inputs
        {
            public input input { get; set; }
        }
        public class input
        {
            public string name { get; set; }
            public string type { get; set; }
            public string value { get; set; }
        }

        public class GetTransactionStatusReqMsg
        {            
            public string TransactionId { get; set; }
        }        

        public class GetTransactionStatusRes
        {
            
            public string TransactionId { get; set; }
            public string Status { get; set; }
            public string SuccessMessage { get; set; }
            public string ErrorMessage { get; set; }
            public bool IsSuccess { get; set; }
            public string ComputerName { get; set; } 
            public string InputCommand { get; set; }
            public List<OutParameter> OutParameters { get; set; }
            public string LogData { get; set; }
            public string SourceTransactionId { get; set; }
        }
        public class OutParameter
        {
            public string ParameterName { get; set; }
            public string ParameterValue { get; set; }
        }
    }
}
