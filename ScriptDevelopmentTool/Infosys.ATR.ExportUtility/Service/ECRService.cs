/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.ExportUtility.Models.ECR;
using Infosys.Nia.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.ATR.ExportUtility.Service
{
    public class ECRService
    {
        public ECRService()
        {

        }
        private RestClient client;
        private volatile static ECRService singleTonObject;
        private string token;
        private Boolean status;

        public static ECRService InstanceCreation()
        {
            object lockingObject = new object();

            if (singleTonObject == null)
            {
                lock (lockingObject)
                {
                    if (singleTonObject == null)
                    {
                        singleTonObject = new ECRService();
                    }
                }
            }

            return singleTonObject;
        }

        public RestClient getInstance(string userName, string password, string casServer, string casService)
        {
            try
            {
                casServer = string.Format(Constants.Application.CasServerUriFormat, casServer);
                casService = string.Format(Constants.Application.ECRServerUriFormat, casService);
                client = RestClient.getInstance(userName, password, casServer, casService);

                //client.getTicket();
                if (string.IsNullOrEmpty(client.TGTToken))
                    throw new Exception("Invalid credentials.");
                token = client.TGTToken;
                status = client.getServiceCallStatus(token);
                return client;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
       
        public CategoryTree BrowseScriptCategory(string restUrl)
        {
            CategoryTree response=null;
            if (status)
            {
                restUrl = string.Format(Constants.Application.BrosweCategoryUriFormat, restUrl);
                var resp = client.getResponseOfGET(restUrl);
                if (!string.IsNullOrEmpty(resp))
                {
                    string rawdata = resp.Replace("\\", "");
                    var cats = JsonConvert.DeserializeObject<Models.ECR.CategoryTree>(rawdata);
                    response = new Models.ECR.CategoryTree();
                    response = cats;
                }
            }
            return response;
        }

        public string AddScriptCategory(string restUrl, ECRCategory cat)
        {
            string result = "";
            if (status)
            {
                string postBody = JsonConvert.SerializeObject(cat);
                result = client.getResponseOfPOST(string.Format(Constants.Application.AddECRScriptCategory, restUrl), postBody);
            }
            return result;
        }

        public string AddScript(string restUrl, string postBody)
        {
            string response = "";
            if (status)
            {
                //string postBody = JsonConvert.SerializeObject(fileParamName);
                response = client.AddNiaScript(string.Format(Constants.Application.AddScriptUrl, restUrl), postBody);
            }
            return response;
        }

        public string UpdateScript(string restUrl, string postBody)
        {
            string response = "";
            if (status)
            {
                //string postBody = JsonConvert.SerializeObject(fileParamName);
                response = client.AddNiaScript(string.Format(Constants.Application.UpdateScriptUrl, restUrl), postBody);
            }
            return response;
        }

        public string FindScriptById(string restUrl, string postBody, Dictionary<string, string> requestHeaderMap)
        {
            string response = "";
            if (status)
            {
                //string postBody = JsonConvert.SerializeObject(fileParamName);
                response = client.getResponseOfPOST(string.Format(Constants.Application.FindScriptUrl, restUrl), postBody, requestHeaderMap);
            }
            return response;
        }

        public List<NIAScript> GetAllScriptsByCategoryId(string restUrl, string requestBody, Dictionary<string, string> requestHeaderMap)
        {
            //List<NIAScript> response = new List<NIAScript>();
            List<NIAScript> niaScriptsList = new List<NIAScript>();
            var resultsGet = client.getResponseOfPOST(string.Format(Constants.Application.GetScriptByCategoryUrl, restUrl), requestBody, requestHeaderMap);

            dynamic results = JsonConvert.DeserializeObject(resultsGet);

            foreach (var result in results)
            {
                NIAScript niaScript = new NIAScript();
                niaScript.scriptId = result.id;
                niaScript.scriptName = result.name;
                niaScript.description = result.description;
                niaScript.scriptType = result.scriptType;

                // Setting parameter values for the scripts
                List<NIAScriptParamVOList> niaScriptParamsList = new List<NIAScriptParamVOList>();
                var parameters = result.scriptParamVOList;
                foreach (var parameter in parameters)
                {
                    NIAScriptParamVOList scriptParams = new NIAScriptParamVOList();
                    scriptParams.id = parameter.id;
                    scriptParams.paramName = parameter.paramName;
                    scriptParams.defaultValue = parameter.defParamValue;
                    scriptParams.id = niaScript.scriptId;
                    scriptParams.isMandatory = parameter.isMandatory;
                    niaScriptParamsList.Add(scriptParams);
                }
                niaScript.niaScriptParamList = niaScriptParamsList;
                niaScriptsList.Add(niaScript);
            }
            //response.NIAScripts = niaScriptsList;

            return niaScriptsList;
        }
    }
}

