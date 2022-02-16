﻿/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Infosys.WEM.Infrastructure.Common;
using System.ServiceModel.Web;


namespace Infosys.WEM.Infrastructure.Common
{
    public class ExceptionHandler
    {
        /// <summary>
        /// Operation to handle exceptions across the applications
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="exceptionHandlingPolicy"></param>
        /// <param name="exceptionToThrow"></param>
        /// <returns></returns>
        public static bool HandleException(Exception exception, string exceptionHandlingPolicy, out Exception exceptionToThrow)
        {
            return ExceptionPolicy.HandleException(exception, exceptionHandlingPolicy, out exceptionToThrow);
        }

        public static List<ServiceFaultError> ExtractServiceFaults(Exception ex)
        {
            return new List<ServiceFaultError> ((List<ServiceFaultError>)(ex as WebFaultException<List<ServiceFaultError>>).Detail); 
        }
    }
}
