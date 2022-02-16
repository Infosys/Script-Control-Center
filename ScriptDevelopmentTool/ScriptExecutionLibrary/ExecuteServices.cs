/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ServiceAdapter;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    class ExecuteServices : ExecuteBase
    {
        private string _serviceURL;
        private string _observerURL;
        List<ExecutionResult> consolidatedResult = new List<ExecutionResult>();
        ExecutionResult output = null;

        public override List<ExecutionResult> Start()
        {
            using (LogHandler.TraceOperations("ExecuteServices:Start", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                string type = Path.GetExtension(ScriptName).Remove(0, 1);
                //Method to read serviceurl and observerUrl from DBs
                LoadServiceConfiguration(type);
                //method to call java service to execute the RPA
                List<ServiceModel.ServiceResponse> serviceResponse = ExecuteService(ScriptIden).Result;
                consolidatedResult = Translator.ExecutionResult_SE.ExecutionResultFromServiceResponse(serviceResponse);
                return consolidatedResult;
            }
        }

        private async Task<List<ServiceModel.ServiceResponse>> ExecuteService(ScriptIndentifier ScriptIden)
        {
            List<ServiceModel.ServiceResponse> result = new List<ServiceModel.ServiceResponse>();
            try
            {
                //HttpClient client = new HttpClient();

                using (LogHandler.TraceOperations("ExecuteServices:ExecuteService", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                {

                    //create data for Post request
                    ServiceModel.ServiceRequest objServiceRequestData = new ServiceModel.ServiceRequest();
                    objServiceRequestData.TransactionId = Convert.ToString(ScriptIden.TransactionId);
                    objServiceRequestData.ScriptId = ScriptIden.ScriptId;
                    objServiceRequestData.CategoryId = ScriptIden.SubCategoryId;
                    objServiceRequestData.ScriptName = ScriptIden.ScriptName;
                    if (ScriptIden.Parameters != null)
                    {
                        List<ServiceModel.Parameters> parameters = new List<ServiceModel.Parameters>();
                        foreach (Parameter objParam in ScriptIden.Parameters)
                        {
                            ServiceModel.Parameters objParams = new ServiceModel.Parameters();
                            objParams.DataType = objParam.DataType;
                            objParams.IsSecret = objParam.IsSecret;
                            objParams.ParameterName = objParam.ParameterName;
                            objParams.ParameterValue = objParam.ParameterValue;
                            parameters.Add(objParams);
                        }
                        objServiceRequestData.InParameters = parameters;
                    }

                    objServiceRequestData.UserName = ScriptIden.UserName;
                    objServiceRequestData.Password = new NetworkCredential("", base.ScriptIden.Password).Password;
                    objServiceRequestData.RemoteServerNames = ScriptIden.RemoteServerNames;
                    objServiceRequestData.Domain = ScriptIden.Domain;
                    objServiceRequestData.ReferenceKey = ScriptIden.ReferenceKey;
                    objServiceRequestData.Path = ScriptIden.Path;
                    objServiceRequestData.SEMObserverURL = _observerURL;

                    //Convert object to json
                    var jsonContent = JsonConvert.SerializeObject(objServiceRequestData);

                    //create content
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");


                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;

                    using (var client = new HttpClient())

                    {

                        // Make request here.
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //call the post service 
                        HttpResponseMessage response = await client.PostAsync(new Uri(_serviceURL), stringContent).ConfigureAwait(false);

                        if (response.IsSuccessStatusCode)
                        {
                            var resultResponse = response.Content.ReadAsStringAsync().Result;
                            string outputresult = JsonConvert.DeserializeObject(resultResponse.ToString()).ToString();
                            List<ServiceModel.ServiceResponse> apiOutput = JsonConvert.DeserializeObject<List<ServiceModel.ServiceResponse>>(outputresult);
                            result = apiOutput;
                        }
                        else
                        {
                            ServiceModel.ServiceResponse objresponse = new ServiceModel.ServiceResponse();
                            objresponse.IsSuccess = false;
                            // add remarks value returned. Limit it to 500 characters 
                            objresponse.ErrorMessage = response.ToString().Length <= 500 ? response.ToString() : response.ToString().Substring(0, 500);
                            result.Add(objresponse);

                        }

                    }
                }

            }
            catch (Exception ex)
            {
                //ServiceResponse objresponse = new ServiceResponse();
                //objresponse.TransactionId = Convert.ToString(ScriptIden.TransactionId);
                //objresponse.CurrentState = "FAILED";
                //objresponse.IsSuccess = false;
                //string err = ex.Message;
                //if (ex.InnerException != null)
                //    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                //objresponse.ErrorMessage = err;
                //result.Add(objresponse);
                throw new Exception(ex.Message+". \n Inner Exception - " + ex.InnerException.Message);
            }
            return result;
        }

        private void LoadServiceConfiguration(string scriptType)
        {
            using (LogHandler.TraceOperations("ExecuteServices:LoadServiceConfiguration", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                try
                {
                    //Get common service
                    CommonRepository commonrestClient = new CommonRepository();
                    var channel = commonrestClient.ServiceChannel;

                    Service.Common.Contracts.Message.GetReferenceDataReqMsg objRequest = new Service.Common.Contracts.Message.GetReferenceDataReqMsg();
                    //Get serviceurl value 
                    objRequest.ReferenceType = scriptType;
                    objRequest.ReferenceKey = "ServiceUrl";
                    objRequest.PartitionKey = ScriptIden.CompanyId.ToString("00000");
                    Service.Common.Contracts.Message.GetReferenceDataResMsg objResponse = channel.GetReferenceData(objRequest);
                    if (objResponse != null && objResponse.referenceData.Count > 0)
                    {
                        _serviceURL = objResponse.referenceData[0].ReferenceValue;
                    }
                    else
                        throw new Exception(objRequest.ReferenceType + " Referencevalue is not configure in DB for ComapanyId" + ScriptIden.CompanyId);
                    //Get observerurl value 
                    objRequest.ReferenceType = "GlobalConfiguration";
                    objRequest.ReferenceKey = "ObserverUrl";
                    objRequest.PartitionKey = ScriptIden.CompanyId.ToString("00000");
                    objResponse = channel.GetReferenceData(objRequest);
                    if (objResponse != null && objResponse.referenceData.Count > 0)
                    {
                        _observerURL = objResponse.referenceData[0].ReferenceValue;
                    }
                    else
                        throw new Exception(objRequest.ReferenceType + " Referencevalue is not configure in DB for ComapanyId" + ScriptIden.CompanyId);
                }
                catch (Exception)
                {
                    throw new Exception("Exception occurred while reading referencedata values for " + scriptType);
                }
            }
        }
    }
}
