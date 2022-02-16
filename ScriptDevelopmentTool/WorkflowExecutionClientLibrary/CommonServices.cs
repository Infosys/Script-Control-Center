/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Service.Common.Contracts.Message;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Client
{
    public class CommonServices
    {
        public string DeploymentBaseURL { get; set; }
        public string StorageBaseURL { get; set; }
        public string RemoteShareUrl { get; set; }
        public bool EnableSecureTransactions { get; set; } 

        private static CommonServices _instance;
        public static CommonServices Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommonServices();
                }
                return _instance;
            }
        }

        private CommonServices()
        {
            var result = GetCompanyDetails(ConfigurationManager.AppSettings["Company"]);
            StorageBaseURL = result.Company.StorageBaseUrl;
            DeploymentBaseURL = result.Company.DeploymentBaseUrl;
            RemoteShareUrl = result.Company.RemoteShareUrl;
            EnableSecureTransactions = Convert.ToBoolean(result.Company.EnableSecureTransactions);
        }

        static GetCompanyResMsg GetCompanyDetails(string companyid)
        {
            GetCompanyResMsg responseObj = null;
            CommonRepository commonRepoClient = new CommonRepository();
            var commonchannel = commonRepoClient.ServiceChannel;
            using (new OperationContextScope((IContextChannel)commonchannel))
            {
                responseObj = commonchannel.GetCompanyDetails(companyid.ToString());
            }
            return responseObj;
        }
    }

    public enum Services 
    {
        WEMService,
        WEMNodeService,
        WEMSecurityAccessService,
        WEMScriptService,
        WEMScheduledRequest,
        WEMCommonService,
        WEMSemanticCluster,
        WEMTransaction,
        WEMECRService,
        WEMExportService,
        WEMAutomationTrackerService,
            WEMObserverService
    }
}
