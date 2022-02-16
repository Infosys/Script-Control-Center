/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ServiceAdapter
{
    //[ServiceContract]
    public interface IServiceAdapter
    {
        //[WebInvoke(Method = "POST")]
        //[OperationContract]
        List<ServiceModel.ServiceResponse> ExecuteService(ServiceModel.ServiceRequest request);

        //[WebInvoke(Method = "POST")]
       // [OperationContract]
        List<ServiceModel.GetTransactionStatusRes> GetTransactionStatus(ServiceModel.GetTransactionStatusReqMsg reqMsg);

    }
}
