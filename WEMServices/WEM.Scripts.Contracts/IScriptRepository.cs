/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Infrastructure.Common.Validators;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.Scripts.Service.Contracts
{
    [ServiceContract]
    [ValidationBehavior]    
    public interface IScriptRepository
    {
        [WebGet(UriTemplate = "GetScriptDetails?scriptId={scriptId}&categoryId={categoryId}&version={version}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetScriptDetailsResMsg GetScriptDetails(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string scriptId,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string categoryId,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string version = "0");

        [WebGet(UriTemplate = "GetAllScriptDetails/{categoryId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllScriptDetailsResMsg GetAllScriptDetails(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string categoryId);

        [WebGet(UriTemplate = "GetAllScriptDetailsWithSubcategories?categoryId={categoryId}&IncludeSubCategoryScripts={IncludeSubCategoryScripts}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllScriptDetailsResMsg GetAllScriptDetailsWithSubcategories(
            [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string categoryId,
              [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            Boolean IncludeSubCategoryScripts
            );

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddScriptResMsg AddScript(AddScriptReqMsg value);

        [WebInvoke(Method = "DELETE")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteScriptResMsg DeleteScript(DeleteScriptReqMsg value);

        [WebInvoke(Method = "POST")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateScriptResMsg UpdateScript(UpdateScriptReqMsg value);
    }
}
