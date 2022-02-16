/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Text;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.Service.Contracts.Message;

using Infosys.WEM.Infrastructure.Common.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;


namespace Infosys.WEM.Service.Contracts
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    //[ExceptionShielding("WEM")]
    [ServiceContract]
    [ValidationBehavior]
    
    public interface IWorkflowAutomation
    {
        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        PublishResMsg Publish(PublishReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        PublishResMsg UpdateWorkflow(PublishReqMsg value);

        [WebGet(UriTemplate = "/GetWorkflowDetails?categoryId={categoryId}&workflowId={workflowId}&workflowVer={workflowVer}&requestId={requestId}&requestorSourceIp={requestorSourceIp}")]       
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]                
        GetWorkflowDetailsResMsg GetWorkflowDetails(int categoryId, Guid workflowId, int workflowVer,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string requestId,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string requestorSourceIp);

        [WebGet(UriTemplate = "/GetDocumentURI?categoryId={categoryId}&workflowId={workflowId}&workflowVer={workflowVer}&requestId={requestId}&requestorSourceIp={requestorSourceIp}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]   
        GetDocumentURIResMsg GetDocumentURI(int categoryId, string workflowId, int workflowVer,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string requestId,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string requestorSourceIp);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        InvokeWorkflowResMsg InvokeWorkflow(InvokeWorkflowReqMsg value);

        [WebGet(UriTemplate = "/GetAllActiveWorkflowsByCategory?categoryId={categoryId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]   
        GetAllActiveWorkflowsByCategoryResMsg GetAllActiveWorkflowsByCategory(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string categoryId);        

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddCategoryResMsg AddCategory(AddCategoryReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteWorkflowResMsg DeleteWorkflow(DeleteWorkflowReqMsg value);

    }

}
