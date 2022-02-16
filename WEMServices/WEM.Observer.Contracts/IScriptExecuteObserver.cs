/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using Infosys.WEM.Infrastructure.Common;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.Observer.Contracts.Message;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.Observer.Contracts
{
    [ServiceContract]
    [ValidationBehavior]
    public interface IScriptExecuteObserver
    {
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        IList<UpdateExecutionStatusResMsg> UpdateExecutionStatus(IList<UpdateExecutionStatusReqMsg> value);
    }
}
