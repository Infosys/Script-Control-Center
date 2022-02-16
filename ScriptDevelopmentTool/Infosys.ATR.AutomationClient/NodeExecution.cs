/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.WEM.Client;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.AutomationClient
{
    public class NodeExecution
    {
        public  Result ExecuteWf(int categoryId, string workFlowId, int workflowVer, List<Parameter> parameters, string serviceOnNodeUrl, bool usesUI, string node)
        {
             Result result = new  Result();
            try
            {
                if (usesUI)
                {
                    ScheduledRequest schReqClient = new ScheduledRequest();
                    AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
                    req.Request = new Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest();
                    req.Request.CategoryId = categoryId;
                    req.Request.Requestor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    req.Request.InputParameters = JSONSerialize(parameters);
                    req.Request.AssignedTo = node;
                    req.Request.Priority = 1000; 
                    req.Request.RequestId = workFlowId;
                    req.Request.RequestType = Infosys.WEM.Node.Service.Contracts.Data.RequestTypeEnum.Workflow;
                    req.Request.RequestVersion = workflowVer;
                    req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
                    req.Request.StopType = Infosys.WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                    req.Request.CompanyId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Company"]);
                    var scheduledReqs = schReqClient.ServiceChannel.AddScheduledRequest(req);
                }
                else
                {
                    //parameters- to be used once the execution of workflow with parameters is supported
                    NodeChannel channel = new NodeChannel(serviceOnNodeUrl);
                     ExecuteWfReq request = new  ExecuteWfReq();
                    request.CategoryId = categoryId;
                    request.WorkflowId = workFlowId;
                    request.WorkflowVer = workflowVer;
                    request.Parameters = Translator.WorkflowParameter_PE_SE.WorkflowParameterListPEtoSE(parameters);
                    result = channel.ServiceChannel.ExecuteWf(request);
                }
                //if (result != null)
                //    Alert(result);

            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                //Alert(result);
            }
            return result;
        }

        public  Result ExecuteScript(int categoryId, int scriptId, List<Parameter> parameters, string serviceOnNodeUrl, bool usesUI, string node)
        {
             Result result = new  Result();
            try
            {
                if (usesUI)
                {
                    ScheduledRequest schReqClient = new ScheduledRequest();
                    AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
                    req.Request = new Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest();
                    req.Request.CategoryId = categoryId;
                    req.Request.Requestor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    req.Request.InputParameters = JSONSerialize(parameters);
                    req.Request.AssignedTo = node;
                    req.Request.Priority = 1; //default 1, to be used in future to give priority to be executed on a particular iap node
                    req.Request.RequestId = scriptId.ToString();
                    req.Request.RequestType = Infosys.WEM.Node.Service.Contracts.Data.RequestTypeEnum.Script;
                    //req.Request.RequestVersion//currently not needed for script
                    req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
                    req.Request.CompanyId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Company"]);
                    var scheduledReqs = schReqClient.ServiceChannel.AddScheduledRequest(req);
                }
                else
                {
                    NodeChannel channel = new NodeChannel(serviceOnNodeUrl);
                     ExecuteScriptReq request = new  ExecuteScriptReq();
                    request.CategoryId = categoryId;
                    request.ScriptId = scriptId;
                    request.Parameters = Translator.ScriptParameter_PE_SE.ScriptParameterListPEtoSE(parameters);
                    result = channel.ServiceChannel.ExecuteScript(request);
                }
                //if (result != null)
                //    Alert(result);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                //Alert(result);
            }
            return result;
        }

        private void Alert( Result result)
        {
            if (result.IsSuccess)
            {
                MessageBox.Show(result.SuccessMessage, "Success...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show(result.ErrorMessage, "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public List<Entity.NodePE> GetAllNodesOnDomain(string domainName, string registeredNodesSrvUrl, int nodeType)
        {
            List<Entity.NodePE> nodes = null;
            RegisteredNodes nodesClient = new RegisteredNodes(registeredNodesSrvUrl);
            string strcompany = System.Configuration.ConfigurationManager.AppSettings["Company"];
            if(string.IsNullOrEmpty(strcompany))
                strcompany="0";
            GetRegisteredNodesResMsg result = nodesClient.ServiceChannel.GetRegisteredNodes(domainName, nodeType.ToString(), strcompany);
            if (result != null)
                nodes = Translator.Node_PE_SE.NodeListSEtoPE(result.Nodes);

            return nodes;
        }

        private string JSONSerialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(obj.GetType());
            jsonSer.WriteObject(stream, obj);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            return json;
        }
    }

    public class Parameter
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public bool IsSecret { get; set; }
    }
}
