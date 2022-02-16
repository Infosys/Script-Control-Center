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

using Infosys.WEM.Client.Constants;
using Infosys.WEM.Service.Common.Contracts;
using System.Configuration;

namespace Infosys.WEM.Client
{
    public class CommonRepository
    {
        string _serviceUrl; // to be used to created end point programmatically in the below code

        public string ServiceUrl
        {
            get { return _serviceUrl; }
            set { _serviceUrl = value; }
        }
        /// <summary>
        /// Constructor to establish communication channel to the Common repository service.
        /// </summary>
        /// <param name="serviceUrl">The base url of the Common repository service.
        /// if no url is passed then in the client config file, parameter with name 'ServiceBaseUrl' will be used to construct the service url</param>
        public CommonRepository(string serviceUrl = "")
        {
            _serviceUrl = serviceUrl;
        }

        /// <summary>
        /// Channel to call the operations on Common repository service
        /// </summary>
        public ICommonRepository ServiceChannel
        {
            get
            {               
                var serviceBinding = WEMProxy.GetBinding();
                Uri _serviceAddress = null;

                if (!string.IsNullOrEmpty(_serviceUrl))
                    _serviceAddress = new Uri(_serviceUrl);
                else
                    _serviceAddress = WEMProxy.ServiceAddress(Services.WEMCommonService);

                WebChannelFactory<ICommonRepository> commonChannel = new WebChannelFactory<ICommonRepository>(serviceBinding, _serviceAddress);
                return commonChannel.CreateChannel();
            }
        }
    }
}
