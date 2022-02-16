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

namespace Infosys.ATR.ExportUtility.Constants
{
    public class Application
    {
        public const string ModuleID = "2";
        public const string ServerFileName = "ServerName.txt";
        public const string ServiceArea = "General";
        public const string CasServerUriFormat = "https://{0}/cas/v1/tickets";
        public const string ECRServerUriFormat = "http://{0}/iap-controller/iap-client";
        public const string BrosweCategoryUriFormat = "http://{0}/iap-controller/spring/category/browse";
        public const string AddECRScriptCategory = "http://{0}/iap-controller/spring/category/add";
        public const string GetScriptByCategoryUrl = "http://{0}/iap-controller/spring/script/fetch/ScriptsBycategory";
        public const string AddScriptUrl = "http://{0}/iap-controller/spring/script/add";
        public const string UpdateScriptUrl = "http://{0}/iap-controller/spring/script/update";
        public const string FindScriptUrl = "http://{0}/iap-controller/spring/script/findById";
    }
}
