﻿/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Configuration;
using Infosys.WEM.Client.Constants;

namespace Infosys.WEM.Client
{
    public class WEMProxyLocal
    {
        private static readonly NetNamedPipeBinding _serviceBinding =
            new NetNamedPipeBinding();

        static string _deploymentUrl = "";

        static WEMProxyLocal()
        {
            _deploymentUrl = Convert.ToString(@"net.pipe://localhost/");//todo

            _serviceBinding.Security.Mode = NetNamedPipeSecurityMode.None;
            _serviceBinding.MaxReceivedMessageSize = 20000000;
            _serviceBinding.MaxBufferSize = 20000000;
            _serviceBinding.MaxBufferPoolSize = 20000000;
        }

        public static NetNamedPipeBinding GetBinding()
        {
            return _serviceBinding;
        }

        public static string ServiceAddress(Services service)
        {
            string _serviceUrl
                = string.Format(ServiceConst.sUrl, _deploymentUrl, service);

            return _serviceUrl;
        }


    }
}
