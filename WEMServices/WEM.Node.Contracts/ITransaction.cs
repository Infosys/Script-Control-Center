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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Node.Service.Contracts.Message;

namespace Infosys.WEM.Node.Service.Contracts
{
    [ServiceContract]
    public interface ITransaction
    {
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        LogTransactionResMsg LogTransaction(LogTransactionReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        GetTransactionsResMsg GetTransactions(GetTransactionsReqMsg value);
    }
}
