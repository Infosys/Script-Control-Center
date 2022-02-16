/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.Serialization;
using BPAdapter.SoapService;
using Newtonsoft.Json;
using ServiceAdapter;
 
namespace BPAdapter.Controllers
{
    [RoutePrefix("api/BPAdapter")]
    public class BPAdapterController : ApiController, IServiceAdapter
    {
        //[Route("ExecuteService")]
        [HttpPost]
        [Route("ExecuteService")]        
        public List<ServiceModel.ServiceResponse> ExecuteService(ServiceModel.ServiceRequest request)
        {     
            List<ServiceModel.ServiceResponse> serviceResponses = new List<ServiceModel.ServiceResponse>();
            try
            {
                if (request != null)
                {
                    XElement parameters = null;
                    if (request.InParameters != null)
                    {
                        List<ServiceModel.input> inputs = new List<ServiceModel.input>();
                        foreach (ServiceModel.Parameters param in request.InParameters)
                        {
                            inputs.Add(new ServiceModel.input()
                            {
                                name = param.ParameterName,
                                value = param.ParameterValue,
                                type = "text"
                            });
                        }
                        parameters = new XElement("inputs",
                                            inputs.Select(i => new XElement("input",
                                                new XAttribute("name", i.name),
                                                new XAttribute("type", i.type != null ? i.type : "text"),
                                                new XAttribute("value", i.value)
                                            )));
                    }

                    string bpInstance = ConfigurationManager.AppSettings["bpInstance"];
                    
                    string ProcessName = request.ScriptName; 
                    string ProcessParameters =parameters!=null?parameters.ToString(SaveOptions.DisableFormatting).Replace("\"", "'").Replace("<","&lt;") :null;
                    // "<inputs><input name='FolderName' type='text' value='BluePrism' /></inputs>";
                    string CallbackInfo = request.SEMObserverURL;
                    string transactionID = request.TransactionId;

                    if (ConfigurationManager.AppSettings["UseSSO"] !=null && !string.IsNullOrEmpty(bpInstance) && !string.IsNullOrEmpty(ProcessName) && 
                        !string.IsNullOrEmpty(CallbackInfo) && !string.IsNullOrEmpty(transactionID) 
                        && ConfigurationManager.AppSettings["UserName"] != null
                        && ConfigurationManager.AppSettings["Password"] != null)
                    {
                        bool UseSSO = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSSO"]);
                        string userName = ConfigurationManager.AppSettings["UserName"];
                        string password = ConfigurationManager.AppSettings["Password"];

                        UtilityProcessDispatcherv221PortTypeClient client =new  UtilityProcessDispatcherv221PortTypeClient();                      
                       // ProcessDispatcher01PortTypeClient client = new ProcessDispatcher01PortTypeClient();
                        client.ClientCredentials.UserName.UserName = userName;
                        client.ClientCredentials.UserName.Password = password;                       
                       

                        RunProcess runProcess = new RunProcess()
                        {
                            bpInstance=bpInstance,
                            TransactionID=transactionID,
                            ProcessName=ProcessName,
                            ProcessParameters=ProcessParameters,
                            CallbackInfo=CallbackInfo,
                            UseSSO=UseSSO                            
                        };
                        RunProcessResponse response = client.RunProcess(runProcess);

                        ServiceModel.ServiceResponse serviceResponse = new ServiceModel.ServiceResponse();
                        serviceResponse = JsonConvert.DeserializeObject<ServiceModel.ServiceResponse>(response.Response);
                        serviceResponses.Add(serviceResponse);
                    }
                    else
                    {                        
                        ServiceModel.ServiceResponse response = new ServiceModel.ServiceResponse()
                        {
                            ComputerName = request.RemoteServerNames,
                            CurrentState = "FAILED",
                            TransactionId = request.TransactionId,
                            IsSuccess = false,
                            SuccessMessage = null,
                            ErrorMessage = "Invalid input data"
                        };
                        serviceResponses.Add(response);
                    }
                }
                
            }
            catch (TimeoutException e)
            {
                ServiceModel.ServiceResponse response = new ServiceModel.ServiceResponse()
                {
                    ComputerName = request.RemoteServerNames,
                    CurrentState = "FAILED",
                    TransactionId = request.TransactionId,
                    IsSuccess = false,
                    SuccessMessage = null,
                    ErrorMessage = "A Timeout exception occured"+ e.Message
                };
                serviceResponses.Add(response);
            }            
            catch (FaultException faultEx)
            {                
                string error = "An unknown exception was received. "
                  + faultEx.Message
                  + faultEx.InnerException.Message
                ;
                ServiceModel.ServiceResponse response = new ServiceModel.ServiceResponse()
                {
                    ComputerName = request.RemoteServerNames,
                    CurrentState = "FAILED",
                    TransactionId = request.TransactionId,
                    IsSuccess = false,
                    SuccessMessage = null,
                    ErrorMessage = error
                };
                serviceResponses.Add(response);

            }
            // Standard communication fault handler.
            catch (CommunicationException commProblem)
            {               
               string error="There was a communication problem. " + commProblem.Message + commProblem.InnerException.Message;
                ServiceModel.ServiceResponse response = new ServiceModel.ServiceResponse()
                {
                    ComputerName = request.RemoteServerNames,
                    CurrentState = "FAILED",
                    TransactionId = request.TransactionId,
                    IsSuccess = false,
                    SuccessMessage = null,
                    ErrorMessage = error
                };
                serviceResponses.Add(response);
            }           
            return serviceResponses;
        }


        [HttpPost]
        [Route("GetTransactionStatus")]
        public List<ServiceModel.GetTransactionStatusRes> GetTransactionStatus(ServiceModel.GetTransactionStatusReqMsg reqMsg)
        {
            List<ServiceModel.GetTransactionStatusRes> serviceResponses = new List<ServiceModel.GetTransactionStatusRes>();
            try
            {
                if (reqMsg != null)
                {
                    string bpInstance = ConfigurationManager.AppSettings["bpInstance"];
                    string TransactionId = reqMsg.TransactionId;

                    if (!string.IsNullOrEmpty(bpInstance) && !string.IsNullOrEmpty(TransactionId) 
                        && ConfigurationManager.AppSettings["UseSSO"]!=null
                        && ConfigurationManager.AppSettings["UserName"] != null
                        && ConfigurationManager.AppSettings["Password"] != null)
                    {
                        bool UseSSO = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSSO"]);

                        UtilityProcessDispatcherv221PortTypeClient client = new UtilityProcessDispatcherv221PortTypeClient();
                        client.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["UserName"];
                        client.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["Password"];

                        GetProcessStatus processStatus = new GetProcessStatus()
                        {
                            bpInstance = bpInstance,
                            TransactionID = TransactionId,
                            UseSSO = UseSSO
                        };
                        GetProcessStatusResponse soapResponse = client.GetProcessStatus(processStatus);

                        ServiceModel.GetTransactionStatusRes response = new ServiceModel.GetTransactionStatusRes();
                        response = JsonConvert.DeserializeObject<ServiceModel.GetTransactionStatusRes>(soapResponse.Response);
                        serviceResponses.Add(response);
                    }
                    else
                    {
                        ServiceModel.GetTransactionStatusRes response = new ServiceModel.GetTransactionStatusRes()
                        {
                            TransactionId = reqMsg.TransactionId,
                            ErrorMessage = "Invalid input data"
                        };
                        serviceResponses.Add(response);
                    }
                }

            }
            catch (TimeoutException e)
            {
                ServiceModel.GetTransactionStatusRes response = new ServiceModel.GetTransactionStatusRes()
                {
                    TransactionId = reqMsg.TransactionId,
                    ErrorMessage = "A Timeout exception occured" + e.Message
                };
                serviceResponses.Add(response);
            }
            catch (FaultException faultEx)
            {
                string error = "An unknown exception was received. "
                  + faultEx.Message
                  + faultEx.InnerException.Message
                ;
                ServiceModel.GetTransactionStatusRes response = new ServiceModel.GetTransactionStatusRes()
                {                    
                    TransactionId = reqMsg.TransactionId,
                    ErrorMessage = error
                };
                serviceResponses.Add(response);

            }
            // Standard communication fault handler.
            catch (CommunicationException commProblem)
            {
                string error = "There was a communication problem. " + commProblem.Message + commProblem.InnerException.Message;
                ServiceModel.GetTransactionStatusRes response = new ServiceModel.GetTransactionStatusRes()
                {
                    TransactionId = reqMsg.TransactionId,
                    ErrorMessage = error
                };
                serviceResponses.Add(response);
            }
            return serviceResponses;
        }
    }
}
