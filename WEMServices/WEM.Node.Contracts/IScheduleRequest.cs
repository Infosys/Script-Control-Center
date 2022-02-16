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
    public interface IScheduleRequest
    {
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddScheduledRequestResMsg AddScheduledRequest(AddScheduledRequestReqMsg value);

        [WebGet(UriTemplate = "GetExecutionStatus/{iapNodeName}/{requestId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExecutionStatusResMsg GetExecutionStatus(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string requestId,
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string iapNodeName); // requestId is the one received from AddScheduledRequest

        //[WebGet(UriTemplate = "GetNextRequest/{iapNodeName}/{requestType}")]
        [WebGet(UriTemplate = "GetNextRequest?v={domain}&n={iapNodeName}&r={requestType}")] //this kind of uri template to allow "." in the domain
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetNextRequestResMsg GetNextRequest(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string domain,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string iapNodeName,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string requestType); //requestType is 1- Workflow, 2 – Script

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        bool UpdateRequestExecutionStatus(UpdateRequestExecutionStatusReqMsg value);

        [WebGet(UriTemplate = "GetScheduledRequestActivities/{scheduledRequestId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetScheduledRequestActivitiesResMsg GetScheduledRequestActivities(
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string scheduledRequestId);

        [WebGet(UriTemplate = "GetTodaysScheduledRequestActivities/{companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetNScheduledRequestActivitiesResMsg GetTodaysScheduledRequestActivities(
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebGet(UriTemplate = "GetScheduledRequestActivitiesFor/{companyId}/{lastNdays}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetNScheduledRequestActivitiesResMsg GetScheduledRequestActivitiesFor(
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId,
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string lastNdays);

        [WebGet(UriTemplate = "GetScheduledRequestActivitiesBetween/{companyId}/{fromDateTicks}/{toDateTicks}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetNScheduledRequestActivitiesResMsg GetScheduledRequestActivitiesBetween(
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId,
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string fromDateTicks,
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string toDateTicks);

        [WebGet(UriTemplate = "GetLongInitiatedRequests?com={companyId}&cat={categoryId}&us={requestor}")] //this kind of uri template to allow "/" in the requestor alias
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetLongInitiatedRequestsResMsg GetLongInitiatedRequests(
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId,
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string categoryId,
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string requestor);
    }
}
