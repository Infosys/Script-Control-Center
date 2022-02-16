/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using IAP.Infrastructure.Services.Contracts;
using IAP.Infrastructure.Services;

namespace InfrastructureClientLibrary
{
    public class LoggerService
    {
        string _serviceUrl; // to be used to created end point programmatically

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; }
        }
        /// <summary>
        /// Constructor to establish communication channel to the RegisteredNodes service.
        /// </summary>
        /// <param name="serviceUrl">The base url of the RegisteredNodes service.
        /// if no url is passed then it would be constructed programmatically</param>
        public LoggerService(string serviceUrl = "")
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Channel to call the operations on RegisteredNodes service
        /// </summary>
        public ILogger ServiceChannel
        {
            get
            {
                Uri serviceAddress = new Uri(_serviceUrl);
                WebHttpBinding serviceBinding = new WebHttpBinding();
                if (serviceAddress.Scheme.Equals(Uri.UriSchemeHttps))
                    serviceBinding.Security.Mode = WebHttpSecurityMode.Transport;
                else
                    serviceBinding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                serviceBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                serviceBinding.SendTimeout = new TimeSpan(0, 3, 0);
                serviceBinding.ReceiveTimeout = new TimeSpan(0, 3, 0);
                WebChannelFactory<ILogger> loggerChannel = new WebChannelFactory<ILogger>(serviceBinding, serviceAddress);
                return loggerChannel.CreateChannel();
            }
        }
    }
}
