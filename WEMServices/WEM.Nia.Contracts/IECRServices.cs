/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Nia.Service.Contracts.Message;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Nia.Service.Contracts
{
    [ServiceContract]
    [ValidationBehavior]
    public interface IECRServices
    {
        [WebInvoke(Method = "POST",  UriTemplate = "/category/browse")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        BrowseScriptCategoryResMsg BrowseScriptCategory(
            BrowseScriptCategoryReqMsg value);

        [WebInvoke(Method = "POST", UriTemplate = "/category/add")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        AddECRCategoryResMsg AddScriptCategory(
           AddECRCategoryReqMsg value);

        [WebInvoke(Method = "POST", UriTemplate = "/script/fetch/ScriptsBycategory")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetScriptByCategoryResMsg GetAllScriptsByCategoryId(
         GetScriptByCategoryReqMsg value);
    }
}
