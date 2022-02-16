/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region using
using System;
using System.Collections.Generic;
using System.Configuration;
using Infosys.WEM.ScriptExecutionLibrary.Helper;
using System.Net;
using System.Web.Configuration;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using Infosys.WEM.ScriptExecutionLibrary.Helper.Model;
using Infosys.WEM.Client;
#endregion

namespace Infosys.WEM.ScriptExecutionLibrary
{
    #region ExecuteNia
    class ExecuteNia : ExecuteBase
    {
        #region Variables
        Dictionary<String, String> commonConfig = new Dictionary<string, string>();
        #endregion

        #region Start
        public override List<ExecutionResult> Start()
        {
            LoadCommonConfiguration();

            List<ExecutionResult> consolidatedOutput = null;
            consolidatedOutput = ExecuteECRScript();
            return consolidatedOutput;
        }
        #endregion

        #region LoadCommonConfiguration
        private void LoadCommonConfiguration()
        {
            try
            {

                string isBypasscertificate = Convert.ToString(ConfigurationManager.AppSettings["ByPassCertificate"]);
                if (isBypasscertificate.ToUpper() == "YES")
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                }
                //Get common service
                CommonRepository commonrestClient = new CommonRepository();
                var channel = commonrestClient.ServiceChannel;

                Service.Common.Contracts.Message.GetReferenceDataReqMsg objRequest = new Service.Common.Contracts.Message.GetReferenceDataReqMsg();
                objRequest.PartitionKey = ScriptIden.CompanyId.ToString("00000");
                objRequest.ReferenceType = ScriptIden.ReferenceKey;
                Service.Common.Contracts.Message.GetReferenceDataResMsg objResponse = channel.GetReferenceData(objRequest);
                if(objResponse!=null && objResponse.referenceData.Count > 0)
                {
                    foreach(Service.Common.Contracts.Data.ReferenceData data in objResponse.referenceData)
                    {
                        commonConfig.Add(data.ReferenceKey, data.ReferenceValue);
                    }
                }
                else
                {
                    throw new Exception("Nia Properties are not found in ReferenceData table");
                }
            }
            catch (Exception)
            {
                throw new Exception("Exception occurred while reading nia properties in DB");
            }
            

            //var config = ConfigurationManager.GetSection("niaAdapterProperties") as NameValueCollection;

            //if (config != null)
            //{
            //    try
            //    {
            //        // Coding decision made
            //        // As of today, we are needing to have casServerUrl, casServiceUrl, niaEcrScriptExecuteUrl, serviceAreas in configurtaion
            //        foreach (var key in config.Keys)
            //        {
            //            commonConfig.Add(key.ToString(), config[key.ToString()].ToString());
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        throw new Exception("Exception occurred while reading from niaAdapterProperties section in web.config");
            //    }
            //}
            //else
            //{
            //    throw new Exception("niaAdapterProperties section not found in web.config");
            //}
        }
        #endregion

        #region ExecuteECRScript
        private List<ExecutionResult> ExecuteECRScript()
        {
            List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();

            String password = new NetworkCredential("", base.ScriptIden.Password).Password;
           
            var niaHelper = new NiaECRHelper(base.ScriptIden.UserName, password, commonConfig["casServerUrl"], commonConfig["casServiceUrl"]);
            var reqBody = niaHelper.GetExecuteECRScriptRequestBody(base.ScriptIden, commonConfig);
            var activityId = niaHelper.ExecuteECRScript(commonConfig["niaEcrScriptExecuteUrl"], reqBody);

            // if Activity id is available  
            if (activityId != null)
            {
                // create request body as required by nia for executing FindByActivityId
                var reqActivityByIdBody = niaHelper.GetActivityByIdScriptRequestBody(commonConfig, Convert.ToInt32(activityId));

                // based on the activity id get script execution id 
                // this script execution id will be stored as a source transaction id for further query to nia using windows service
                var findByActivityIdOutput = niaHelper.ExecuteECRScript(commonConfig["niaEcrFindByActivityIdUrl"], reqActivityByIdBody);

                // convert the json to object 
                List<JToken> activityIdObjList = null;
                JArray findByActivityIdObjArr = Newtonsoft.Json.Linq.JArray.Parse(findByActivityIdOutput);
                activityIdObjList = findByActivityIdObjArr.Children<JObject>().ToList<JToken>();

                // convert req body to object to fetch computer name and ip mapping 

                ScriptExecutionVO svo = JsonConvert.DeserializeObject<ScriptExecutionVO>(reqBody);
                List<NodeVO> nodeList = svo.nodeList;

                if (!string.IsNullOrEmpty(ScriptIden.RemoteServerNames) && ScriptIden.RemoteServerNames.Length != 0)
                {
                    string[] computerNames = ScriptIden.RemoteServerNames.Split(',');
                    foreach (string ComputerName in computerNames)
                    {
                        string scriptExecutionId = null;

                        // for the computer name fetch the node id from svo object 
                        var nodeInfo = nodeList.Where(x => ComputerName.Equals(x.name, StringComparison.InvariantCultureIgnoreCase)).ToList();
                        if (nodeInfo.Count != 0)
                        {
                            // get the node id 
                            string nodeId = nodeInfo.FirstOrDefault().id.ToString();

                            // for the node id fetch the script execution id from activityIdObjList object 
                            JToken node = activityIdObjList.Where<JToken>(x => JObject.Parse(x.ToString()).GetValue("nodeId").ToString().Equals(nodeId)).FirstOrDefault();
                            if (node != null)
                                // get the value of script execution id 
                                scriptExecutionId = node.Value<string>("id");
                        }

                        ExecutionResult result = new ExecutionResult()
                        {
                            TransactionId = ScriptIden.TransactionId,
                            //IsSuccess = true,
                            //Output = new List<OutParameter>()
                            //{
                            //    new OutParameter() { ParameterName = "activityId", ParameterValue = activityId }
                            //},
                            // use this script execution id in sourceTransactionId
                            SourceTransactionId = scriptExecutionId,
                            //SuccessMessage = "QUEUED",
                            Status = "QUEUED",
                            ComputerName = ComputerName
                        };
                        consolidatedOutput.Add(result);
                           
                    }
                }
            }     
            return consolidatedOutput;
        } 
        #endregion
    }
    #endregion
}
