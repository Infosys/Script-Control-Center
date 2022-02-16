/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Web;
using Infosys.WEM.Infrastructure.Common;
using WEM.ScriptExecution.Message;
using System.Threading.Tasks;

namespace WEM.ScriptExecution.Contracts
{
    [ServiceContract]
    public interface IScriptExecute
    {
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        InitiateExecutionResMsg InitiateExecution(InitiateExecutionReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        Task<InitiateExecutionResMsg> AsyncInitiateExecution(InitiateExecutionReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        GetTransactionStatusResMsg GetTransactionStatus(GetTransactionStatusReqMsg value);

        //[WebInvoke(Method = "POST")]
        //[OperationContract]
        //[FaultContract(typeof(ServiceFaultError))]
        //UpdateTransactionStatusResMsg UpdateTransactionStatus(UpdateTransactionStatusReqMsg value);
    }
}
