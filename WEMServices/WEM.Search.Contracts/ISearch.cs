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

using Infosys.WEM.Scripts.Service.Contracts.Data;
using Infosys.WEM.Scripts.Service.Contracts.Message;

using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Infrastructure.Common.Validators;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.Search.Contracts
{
    public interface ISearch
    {
        [WebGet(UriTemplate = "SearchMeta?data={data}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        GetScriptDetailsResMsg SearchMeta(string data);
    }
}
