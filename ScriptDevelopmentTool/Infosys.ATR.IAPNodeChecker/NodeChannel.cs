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

using Infosys.ATR.AutomationEngine.Contracts;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Infosys.ATR.IAPNodeChecker
{
    public class NodeChannel
    {
        string _serviceUrl; // to be used to created end point programmatically
        /// <summary>
        /// Constructor to establish communication channel to the Nodes .
        /// </summary>
        /// <param name="serviceUrl">The base url of the Nodes service.
        /// if no url is passed then in the client config file, a service end point with name 'NodesClient' would be expected</param>
        public NodeChannel(string serviceUrl = "")
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Channel to call the operations on RegisteredNodes service
        /// </summary>
        public INodeService ServiceChannel
        {
            get
            {
                Uri serviceAddress = new Uri(_serviceUrl);
                WebHttpBinding serviceBinding = new WebHttpBinding();
                serviceBinding.SendTimeout = new TimeSpan(0, 10, 0);
                serviceBinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                WebChannelFactory<INodeService> nodeChannel = new WebChannelFactory<INodeService>(serviceBinding, serviceAddress);
                return nodeChannel.CreateChannel();
            }
        }
    }
}
