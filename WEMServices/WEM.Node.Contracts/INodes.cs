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
using System.ServiceModel.Activation;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Infrastructure.Common.Validators;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.Node.Service.Contracts
{
    [ServiceContract]
    [ValidationBehavior]    
    public interface INodes
    {
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        RegisterResMsg Register(RegisterReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UnRegisterResMsg UnRegister(UnRegisterReqMsg value);

        [WebGet(UriTemplate = "GetRegisteredNodes?v={domain}&t={nodeType}&c={companyId}")] //this kind of uri template to allow "." in the domain
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetRegisteredNodesResMsg GetRegisteredNodes(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string domain,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string nodeType,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

    }
}
