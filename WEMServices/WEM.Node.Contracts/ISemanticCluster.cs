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
    public interface ISemanticCluster
    {
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        string AddSemanticCluster(AddSemanticClusterReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateSemanticClusterResMsg UpdateSemanticCluster(UpdateSemanticClusterReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddSemanticNodeClusterResMsg AddSemanticNodeCluster(AddSemanticNodeClusterReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateSemanticNodeClusterResMsg UpdateSemanticNodeCluster(UpdateSemanticNodeClusterReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddSemanticCategoryResMsg AddSemanticCategory(AddSemanticCategoryReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateSemanticCategoryResMsg UpdateSemanticCategory(UpdateSemanticCategoryReqMsg value);

        [WebGet(UriTemplate = "/GetAllNodesByCluster?clusterId={clusterId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllNodesByClusterResMsg GetAllNodesByCluster(
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string clusterId);

        [WebGet(UriTemplate = "/GetAllClusters?companyId={companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllClustersResMsg GetAllClusters(
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteSemanticCategoryResMsg DeleteSemanticCategory(DeleteSemanticCategoryReqMsg value);


    }
}
