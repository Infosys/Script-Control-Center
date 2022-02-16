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

using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Web;

namespace Infosys.ATR.AutomationEngine.Contracts
{
    [ServiceContract]
    public interface INodeService
    {
        [OperationContract]
        //[WebGet(UriTemplate = "ExecuteWf/{categoryId}/{workFlowId}/{workflowVer}")]
        [WebInvoke(Method = "POST")]
        Result ExecuteWf(ExecuteWfReq request);

        [OperationContract]
        //[WebGet(UriTemplate = "ExecuteScript/{categoryId}/{scriptId}")]
        [WebInvoke(Method = "POST")]
        Result ExecuteScript(ExecuteScriptReq request);

        [WebGet(UriTemplate = "Ping")]
        [OperationContract]
        string Ping();
    }

    [DataContract]
    public class Result
    {
        [DataMember]
        public string SuccessMessage { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string InputCommand { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public List<Parameter> Output { get; set; }
    }

    [DataContract]
    public class Parameter
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public object ParameterValue { get; set; }
    }

    [DataContract]
    public class ScriptParameter
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public string ParameterValue { get; set; }
        //[DataMember]        
        //public bool IsSecret { get; set; }
        //[DataMember]
        //public bool IsPaired { get; set; }
    }

    [DataContract]
    public class WorkFlowParameter
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public string ParameterValue { get; set; }
    }

    [DataContract]
    public class ExecuteScriptReq
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public int ScriptId { get; set; }
        [DataMember]
        public List<ScriptParameter> Parameters { get; set; }
    }

    [DataContract]
    public class ExecuteWfReq
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public string WorkflowId { get; set; }
        [DataMember]
        public int WorkflowVer { get; set; }
        [DataMember]
        public List<WorkFlowParameter> Parameters { get; set; }
    }
}
