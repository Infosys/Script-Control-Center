/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.RemoteExecute
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
                if(string.IsNullOrEmpty(_serviceUrl))
                {
                    WebChannelFactory<INodeService> nodeChannel = new WebChannelFactory<INodeService>("NodesClient");
                    return nodeChannel.CreateChannel();
                }
                else
                {

                    if (_serviceUrl.ToLower().Contains("http"))
                    {
                        Uri serviceAddress = new Uri(_serviceUrl);
                        WebHttpBinding serviceBinding = new WebHttpBinding();
                        if (serviceAddress.Scheme.Equals(Uri.UriSchemeHttps))
                            serviceBinding.Security.Mode = WebHttpSecurityMode.Transport;
                        else
                            serviceBinding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                        serviceBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

                        serviceBinding.SendTimeout = new TimeSpan(0, 10, 0);
                        serviceBinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                        serviceBinding.MaxReceivedMessageSize = 4194304; //default 4 mb                                             
                        WebChannelFactory<INodeService> nodeChannel = new WebChannelFactory<INodeService>(serviceBinding, serviceAddress);
                        nodeChannel.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
                        return nodeChannel.CreateChannel();
                    }
                    else
                    {
                        EndpointAddress serviceAddress = new EndpointAddress(_serviceUrl);
                        NetTcpBinding serviceBinding = new NetTcpBinding();
                        serviceBinding.Security.Mode = SecurityMode.Transport;
                        serviceBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                        serviceBinding.SendTimeout = new TimeSpan(0, 10, 0);
                        serviceBinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                        serviceBinding.MaxReceivedMessageSize = 4194304; //default 4 mb
                        ChannelFactory<INodeService> nodeChannel = new ChannelFactory<INodeService>(serviceBinding, serviceAddress);
                        nodeChannel.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Delegation;
                        //nodeChannel.Credentials.Windows.AllowNtlm = false;
                        return nodeChannel.CreateChannel();
                    }
                }
            }
        }
    }
}
