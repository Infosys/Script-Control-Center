/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Nia.Services;
using Infosys.WEM.ScriptExecutionLibrary.Helper.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infosys.WEM.ScriptExecutionLibrary.Helper
{
    public class NiaECRHelper
    {
        public RestClient Client { get; }
        public bool Status { get; }

        /// <summary>
        /// Executes an ECR script and returns the activity id
        /// </summary>
        
        /// <param name="requestBody">The request body that should be posted to the service</param>
        /// <returns></returns>
        public String ExecuteECRScript(String restUrl, String requestBody)
        {
            String activityId = Client.getResponseOfPOST(restUrl, requestBody);

            return activityId;
        }

        /// <summary>
        /// Creates the request body that needs to be sent to the Execute Script ECR service
        /// </summary>
        /// <param name="scriptDetails">Details of the script received</param>
        /// <param name="niaConfinguration">Configuration dictionary containing details picked up from Web.config</param>
        /// <returns>Request body in JSON format</returns>
        public string GetExecuteECRScriptRequestBody(ScriptIndentifier scriptDetails, Dictionary<String,String> niaConfinguration)
        {
            ScriptExecutionVO svo = new ScriptExecutionVO();
            svo.scriptId = scriptDetails.ScriptId;

            svo.scriptParams = new Dictionary<string, string>();
            scriptDetails.Parameters.ForEach(x => svo.scriptParams.Add(x.ParameterName, x.ParameterValue));

            var getAllNodesUrl = niaConfinguration["niaEcrFindAllNodesUrl"];
            var serviceAreas = niaConfinguration["serviceAreas"].Split(new char[] { ',' }).ToList<String>();
            svo.nodeList = PopulateNodeList(scriptDetails.RemoteServerNames, getAllNodesUrl, serviceAreas,scriptDetails.UserName);
            
            // Raise an exception if none of the node names are valid
            if (svo.nodeList.Count == 0)
            {
                throw new Exception(string.Format("None of specified node names are valid : %s", scriptDetails.RemoteServerNames));
            }
            
            svo.serviceAreas = serviceAreas;

            return JsonConvert.SerializeObject(svo);
        }

        /// <summary>
        /// the method gets the required json required for execution for GetActivityById
        /// </summary>
        /// <param name="niaConfinguration">Configuration dictionary containing details picked up from Web.config</param>
        /// <param name="ActivityId"></param>
        /// <returns>request body in json format</returns>
        public string GetActivityByIdScriptRequestBody(Dictionary<String, String> niaConfinguration, int ActivityId)
        {

            var serviceAreas = niaConfinguration["serviceAreas"].Split(new char[] { ',' }).ToList<String>();
            var serviceAreaJson = JsonConvert.SerializeObject(serviceAreas);
            string json = "{\"pathVariableMap\":{ \"activityId\":" + ActivityId + "},\"serviceAreas\":" + serviceAreaJson + "}";
            return json;

        }

        /// <summary>
        /// Generates the NodeVO objects with the details about the node from the remote server names list
        /// </summary>
        /// <param name="remoteServerNames">Comma separated list of remote servers</param>
        /// <param name="serviceAreas">List of service areas</param>
        /// <returns>List of NodeVO objects</returns>
        public List<NodeVO> PopulateNodeList(string remoteServerNames, String restUrl, List<String> serviceAreas, string userName)
        {
            var nodeList = new List<NodeVO>();

            var requestBody = JsonConvert.SerializeObject(serviceAreas);
            var nodeListResponse = Client.getResponseOfPOST(restUrl, requestBody);
            var nodesList = JsonConvert.DeserializeObject<List<NodeVO>>(nodeListResponse);

            var nodeNames = remoteServerNames.ToLowerInvariant().Split(new char[] { ',' }).ToList<String>();

            // filter based on server name and user id 
            nodeList = nodesList
                            .Where(x => nodeNames.Contains(x.name.ToLowerInvariant()) && userName.Equals(x.userId , StringComparison.InvariantCultureIgnoreCase))
                                .GroupBy(s => s.name)
                                   .Select(grp => grp.FirstOrDefault())
                                        .ToList();

            return nodeList;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userName">The username to login with</param>
        /// <param name="password">The password to use</param>
        /// <param name="casServerUrl">The URL of the CAS server</param>
        /// <param name="casServiceUrl">URL to the case service</param>
        public NiaECRHelper(string userName, string password, string casServerUrl, string casServiceUrl)
        {
            try
            {
                Client = RestClient.getInstance(userName, password, casServerUrl, casServiceUrl);

                if (string.IsNullOrEmpty(Client.TGTToken))
                    throw new Exception("Invalid credentials.");

                Status = Client.getServiceCallStatus(Client.TGTToken);
            }
            catch (Exception ex)
            {
                Status = false;
                throw new Exception(ex.Message);
            }
        }
    }
}
