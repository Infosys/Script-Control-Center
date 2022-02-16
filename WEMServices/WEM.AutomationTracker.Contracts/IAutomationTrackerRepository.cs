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
using System.ServiceModel.Web;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Infosys.WEM.AutomationTracker.Contracts.Message;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Infrastructure.Common.Validators;

namespace Infosys.WEM.AutomationTracker.Contracts
{
    [ServiceContract]
    [ValidationBehavior]
    public interface IAutomationTrackerRepository
    {      

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateTransactionStatusResMsg UpdateTransactionStatus(UpdateTransactionStatusReqMsg value);


        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateNotificationDetailsResMsg UpdateNotificationDetails(UpdateNotificationDetailsReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddRequestResMsg AddTransactionStatus(AddRequestReqMsg value);


        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        GetTransactionStatusResMsg GetTransactionStatus(GetTransactionStatusReqMsg request);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        GetTransactionsByStatusResMsg GetTransactionsByStatus(GetTransactionsByStatusReqMsg request);

    }
}
