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
using Infosys.WEM.AutomationTracker.Contracts;
using Infosys.WEM.Client.Constants;

namespace Infosys.WEM.Client
{
    public class AutomationTracker
    {
        string _serviceUrl; // to be used to created end point programmatically

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; }
        }
        public bool isLocal;
        /// <summary>
        /// Constructor to establish communication channel to the AutomationTracker repository service.
        /// </summary>
        /// <param name="serviceUrl">The base url of the AutomationTracker repository service.
        /// if no url is passed then it would be constructed programmatically</param>
        public AutomationTracker(string serviceUrl = "")
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Channel to call the operations on AutomationTracker repository service
        /// </summary>
        public IAutomationTrackerRepository ServiceChannel
        {
            get
            {
                ChannelFactory<IAutomationTrackerRepository> securityAccessChannel;
                if (isLocal)
                {
                    var serviceBinding = WEMProxyLocal.GetBinding();
                    string _serviceAddress = null;

                    if (!string.IsNullOrEmpty(_serviceUrl))
                        _serviceAddress = _serviceUrl;
                    else
                        _serviceAddress = WEMProxyLocal.ServiceAddress(Services.WEMAutomationTrackerService);

                     securityAccessChannel = new ChannelFactory<IAutomationTrackerRepository>(serviceBinding, _serviceAddress);
                    
                }
                else
                {
                    var serviceBinding = WEMProxy.GetBinding();
                    Uri _serviceAddress = null;

                    if (!string.IsNullOrEmpty(_serviceUrl))
                        _serviceAddress = new Uri(_serviceUrl);
                    else
                        _serviceAddress = WEMProxy.ServiceAddress(Services.WEMAutomationTrackerService);

                     securityAccessChannel = new WebChannelFactory<IAutomationTrackerRepository>(serviceBinding, _serviceAddress);
                    
                }

                return securityAccessChannel.CreateChannel();
            }
        }
    }
}
