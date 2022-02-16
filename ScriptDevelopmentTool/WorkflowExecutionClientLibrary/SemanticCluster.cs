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
using Infosys.WEM.Node.Service.Contracts;
using Infosys.WEM.Client.Constants;

namespace Infosys.WEM.Client
{
    public class SemanticCluster
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
        public SemanticCluster(string serviceUrl = "")
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Channel to call the operations on RegisteredNodes service
        /// </summary>
        public ISemanticCluster ServiceChannel
        {
            get
            {

                var serviceBinding = WEMProxy.GetBinding();
                Uri _serviceAddress = null;

                if (!string.IsNullOrEmpty(_serviceUrl))
                    _serviceAddress = new Uri(_serviceUrl);
                else
                    _serviceAddress = WEMProxy.ServiceAddress(Services.WEMSemanticCluster);

                WebChannelFactory<ISemanticCluster> securityAccessChannel = new WebChannelFactory<ISemanticCluster>(serviceBinding, _serviceAddress);
                return securityAccessChannel.CreateChannel();
            }
        }
    }
}
