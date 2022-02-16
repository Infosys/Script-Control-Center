/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Observer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Client
{
    public class ScriptExecuteObserver
    {
        string _serviceUrl; // to be used to created end point programmatically

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; }
        }
        public bool isLocal;
        /// <summary>
        /// Constructor to establish communication channel to the observer service.
        /// </summary>
        /// <param name="serviceUrl">The base url of the observer service.
        /// if no url is passed then it would be constructed programmatically</param>
        public ScriptExecuteObserver(string serviceUrl = "")
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Channel to call the operations on observer service
        /// </summary>
        public  IScriptExecuteObserver ServiceChannel
        {
            get
            {
                ChannelFactory<IScriptExecuteObserver> securityAccessChannel;
                if (isLocal)
                {
                    var serviceBinding = WEMProxyLocal.GetBinding();
                    string _serviceAddress = null;

                    if (!string.IsNullOrEmpty(_serviceUrl))
                        _serviceAddress = _serviceUrl;
                    else
                        _serviceAddress = WEMProxyLocal.ServiceAddress(Services.WEMObserverService);

                    securityAccessChannel = new ChannelFactory<IScriptExecuteObserver>(serviceBinding, _serviceAddress);

                }
                else
                {
                    var serviceBinding = WEMProxy.GetBinding();
                    Uri _serviceAddress = null;

                    if (!string.IsNullOrEmpty(_serviceUrl))
                        _serviceAddress = new Uri(_serviceUrl);
                    else
                        _serviceAddress = WEMProxy.ServiceAddress(Services.WEMObserverService);

                    securityAccessChannel = new WebChannelFactory<IScriptExecuteObserver>(serviceBinding, _serviceAddress);

                }

                return securityAccessChannel.CreateChannel();
            }
        }
    }
}
