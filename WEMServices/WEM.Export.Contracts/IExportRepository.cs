/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.ServiceModel;
using System.ServiceModel.Web;
using Infosys.WEM.Infrastructure.Common;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using Infosys.WEM.Export.Service.Contracts.Message;
using System.Collections.Generic;
using Infosys.WEM.Infrastructure.Common.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Infosys.WEM.Export.Service.Contracts
{
    [ServiceContract]
    [ValidationBehavior]
    public interface IExportRepository
    {
        [WebGet(UriTemplate = "/GetExportTargetSystemDetails")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExportTargetSystemDetailsResMsg GetExportTargetSystemDetails();

        [WebGet(UriTemplate = "GetExportConfigurationMasterDetails?targetServerId={targetServerId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExportConfigurationMasterResMsg GetExportConfigurationMasterDetails(
            // [Infosys.WEM.    ("Special charaters not allowed")]
            string targetServerId);

        [WebGet(UriTemplate = "GetAllExportConfigurationMasterDetails")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllExportConfigurationMasterResMsg GetAllExportConfigurationMasterDetails();

        [WebGet(UriTemplate = "GetAllExportTransactionDetails?configId={configId}&status={status}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExportTransactionDetailsResMsg GetAllExportTransactionDetails(string configId, string status);

        //[WebGet(UriTemplate = "GetExportServerDetails/{targetServerId}")]
        [WebGet(UriTemplate = "GetExportServerDetails?targetServerId={targetServerId}&id={id}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExportServerDetailsResMsg GetExportServerDetails(
            //[SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string targetServerId,string id);

        [WebGet(UriTemplate = "/GetExportScriptConfigurationDetails/{exportConfigurationId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExportScriptConfigurationDetailsResMsg GetExportScriptConfigurationDetails(string exportConfigurationId);

        //[WebInvoke(Method = "POST")]
        //[OperationContract]
        //[FaultContract(typeof(ServiceFaultError))]
        //AddExportServerDetailsResMsg AddExportServerDetails(AddExportServerDetailsReqMsg value);

        // ExportConfigurationMaster Methods
        //[WebInvoke(Method = "POST")]
        //[OperationContract]
        //[FaultContract(typeof(ServiceFaultError))]
        //AddExportConfigurationMasterResMsg AddExportConfigurationMaster(AddExportConfigurationMasterReqMsg value);

        [WebInvoke(Method = "PUT")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateExportConfigurationMasterResMsg UpdateExportConfigurationMaster(UpdateExportConfigurationMasterReqMsg value);

        [WebInvoke(Method = "PUT")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateExportTransactionDetailsResMsg UpdateExportTransactionDetails(UpdateExportTransactionDetailsReqMsg value);


        //[WebInvoke(Method = "POST")]
        //[OperationContract]
        //[FaultContract(typeof(ServiceFaultError))]
        //AddExportScriptConfigurationDetailsResMsg AddExportScriptConfigurationDetail(AddExportScriptConfigurationDetailsReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddExportTransactionDetailsResMsg AddExportTransactionDetails(AddExportTransactionDetailsReqMsg value);

        // ExportJobProcessingHistory Methods
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddExportJobProcessingHistoryResMsg AddExportJobProcessingHistory(AddExportJobProcessingHistoryReqMsg value);

        [WebInvoke(Method = "PUT")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateExportJobProcessingHistoryResMsg UpdateExportJobProcessingHistory(UpdateExportJobProcessingHistoryReqMsg value);

        // ExportSourceTargetMapping Methods
        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddExportSourceTargetMappingResMsg AddExportSourceTargetMapping(AddExportSourceTargetMappingReqMsg value);

        [WebGet(UriTemplate = "GetExportSourceTargetMappingDetails?targetServerId={targetServerId}&targetCategoryId={targetCategoryId}&sourceCategoryId={sourceCategoryId}&sourceScriptId={sourceScriptId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExportSourceTargetMappingDetailsResMsg GetExportSourceTargetMappingDetails(string targetServerId,string targetCategoryId,string sourceCategoryId,string sourceScriptId = "");

        [WebGet(UriTemplate = "GetPastExportConfigurationMasterDetails?ExportDate={ExportDate}&exportStatus={exportStatus}&userName={userName}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetPastExportConfigurationMasterResMsg GetPastExportConfigurationMasterDetails(string ExportDate, string exportStatus, string userName);

        [WebGet(UriTemplate = "GetExportTransactionDetailsByUserName?exportScriptConfigurationId={exportScriptConfigurationId}&userName={userName}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetExportTransactionDetailsResMsg GetExportTransactionDetailsByUserName(string exportScriptConfigurationId,string userName);


        //[WebInvoke(Method = "POST")]
        //[OperationContract]
        //[FaultContract(typeof(ServiceFaultError))]
        //AddExportConfigurationMasterResMsg AddExportConfigurationDetails(AddExportConfigurationMasterdetailsReqMsg value);

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddExportConfigurationMasterResMsg AddExportConfigurationDetails(AddExportConfigurationMasterdetailsReqMsg data);
        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
        //   RequestFormat = WebMessageFormat.Json,
        //   BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        //[OperationContract]
        //[FaultContract(typeof(ServiceFaultError))]
        //AddExportConfigurationMasterResMsg AddExportConfigurationDetails(string data);

        [WebInvoke(Method = "PUT")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateExportSourceTargetMappingResMsg UpdateExportSourceTargetMapping(UpdateExportSourceTargetMappingReqMsg value);

        [WebInvoke(Method = "PUT")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdatePastExportConfigurationDetailsResMsg UpdatePastExportConfigurationDetails(UpdatePastExportConfigurationDetailsReqMsg value);

    }
}
