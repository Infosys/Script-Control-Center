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
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Client;
using Infosys.ATR.AutomationEngine.Contracts;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Infosys.ATR.RemoteExecute
{
    public class Hanlder
    {
        public static List<Result> DelegateExecution(ExecutingEntity entity)
        {
            List<Result> results = new List<Result>();
            try
            {
                switch (entity.ExecutionMode)
                {
                    case ExecutionModeType.RunOnIAPNode:
                        if (entity.UsesUIAutomation)
                        {
                            throw new RemoteExecutionException("Workflow or script with UI Automation can be executed on IAP Node (windows service based agent)");
                        }
                        else
                        {
                            results = DelegateOnIAPNodeAgent(entity);
                        }
                        break;
                    case ExecutionModeType.ScheduledOnIAPNode:
                        results = ScheduleOnIapNodes(entity);
                        break;
                    case ExecutionModeType.ScheduledOnIAPCluster:
                        results = ScheduleOnClusters(entity);
                        break;
                }
            }
            catch (RemoteExecutionException ex)
            {
                Result error = new Result();
                error.IsSuccess = false;
                error.ErrorMessage = ex.Message;
                results.Add(error);
            }

            return results;
        }

        private static List<Result> ScheduleOnIapNodes(ExecutingEntity entity)
        {
            List<Result> results = new List<Result>();
            entity.RemoteServerNames.AsParallel().ForAll(node =>
            {
                AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
                req.Request = new WEM.Node.Service.Contracts.Data.ScheduledRequest();
                req.Request.AssignedTo = node;
                req.Request.CategoryId = entity.CategoryId;
                req.Request.CompanyId = entity.ComapnyId;
                if (entity.ScheduledPattern == ScheduledPatternType.ScheduleWithRecurrence)
                    req.Request.ExecuteOn = entity.ScheduleStartDateTime;
                else if (entity.ScheduledPattern == ScheduledPatternType.ScheduleNow)
                    req.Request.ExecuteOn = DateTime.UtcNow;
                if (entity.Parameters != null)
                    req.Request.InputParameters = JSONSerialize(entity.Parameters);
                if (entity.ScheduleStopCriteria == ScheduleStopCriteriaType.NoEndDate)
                {
                    req.Request.StopType = WEM.Node.Service.Contracts.Data.StopTypes.Indefinite;
                }
                else if (entity.ScheduleStopCriteria == ScheduleStopCriteriaType.EndDate)
                {
                    req.Request.StopType = WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                    req.Request.EndDate = entity.ScheduleEndDateTime;
                }
                else if (entity.ScheduleStopCriteria == ScheduleStopCriteriaType.OccurenceCount)
                {
                    req.Request.StopType = WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                    req.Request.Iterations = entity.ScheduleOccurences;
                }
                req.Request.Priority = entity.SchedulePriority;
                req.Request.Requestor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
                switch (entity.EntityType)
                {
                    case ExecutingEntityType.Script:
                        req.Request.RequestId = entity.ScriptId.ToString();
                        req.Request.RequestType = WEM.Node.Service.Contracts.Data.RequestTypeEnum.Script;
                        break;
                    case ExecutingEntityType.Workflow:
                        req.Request.RequestId = entity.WorkflowId;
                        req.Request.RequestVersion = entity.WorkflowVersion;
                        req.Request.RequestType = WEM.Node.Service.Contracts.Data.RequestTypeEnum.Workflow;
                        break;
                }

                var response = new ScheduledRequest().ServiceChannel.AddScheduledRequest(req);
                lock (results)
                {
                    Result result = new Result();
                    if (!response.IsSuccess && response.ScheduledRequestIds != null && response.ScheduledRequestIds.Count > 0)
                    {
                        //then from the servive error message is put in the scheduled request id property
                        result.ErrorMessage = response.ScheduledRequestIds[0];
                    }
                    result.IsSuccess = response.IsSuccess;
                    result.ScheduledRequestIds = response.ScheduledRequestIds;
                    result.MachineName = node;
                    results.Add(result);
                }

            });
            return results;
        }

        private static List<Result> ScheduleOnClusters(ExecutingEntity entity)
        {
            List<Result> results = new List<Result>();
            entity.ScheduleOnClusters.AsParallel().ForAll(node =>
            {
                AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
                req.Request = new WEM.Node.Service.Contracts.Data.ScheduledRequest();
                req.Request.AssignedTo = node;
                req.Request.CategoryId = entity.CategoryId;
                req.Request.CompanyId = entity.ComapnyId;
                if (entity.ScheduledPattern == ScheduledPatternType.ScheduleWithRecurrence)
                    req.Request.ExecuteOn = entity.ScheduleStartDateTime;
                else if (entity.ScheduledPattern == ScheduledPatternType.ScheduleNow)
                    req.Request.ExecuteOn = DateTime.UtcNow;
                if (entity.Parameters != null)
                    req.Request.InputParameters = JSONSerialize(entity.Parameters);
                if (entity.ScheduleStopCriteria == ScheduleStopCriteriaType.NoEndDate)
                {
                    req.Request.StopType = WEM.Node.Service.Contracts.Data.StopTypes.Indefinite;
                }
                else if (entity.ScheduleStopCriteria == ScheduleStopCriteriaType.EndDate)
                {
                    req.Request.StopType = WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                    req.Request.EndDate = entity.ScheduleEndDateTime;
                }
                else if (entity.ScheduleStopCriteria == ScheduleStopCriteriaType.OccurenceCount)
                {
                    req.Request.StopType = WEM.Node.Service.Contracts.Data.StopTypes.Limited;
                    req.Request.Iterations = entity.ScheduleOccurences;
                }
                req.Request.Priority = entity.SchedulePriority;
                req.Request.Requestor = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
                switch (entity.EntityType)
                {
                    case ExecutingEntityType.Script:
                        req.Request.RequestId = entity.ScriptId.ToString();
                        req.Request.RequestType = WEM.Node.Service.Contracts.Data.RequestTypeEnum.Script;
                        break;
                    case ExecutingEntityType.Workflow:
                        req.Request.RequestId = entity.WorkflowId;
                        req.Request.RequestVersion = entity.WorkflowVersion;
                        req.Request.RequestType = WEM.Node.Service.Contracts.Data.RequestTypeEnum.Workflow;
                        break;
                }

                var response = new ScheduledRequest().ServiceChannel.AddScheduledRequest(req);
                //lock (results)
                //    results.Add(new Result() { IsSuccess = response.IsSuccess, ScheduledRequestIds = response.ScheduledRequestIds, MachineName = node });

                lock (results)
                {
                    Result result = new Result();
                    if (!response.IsSuccess && response.ScheduledRequestIds != null && response.ScheduledRequestIds.Count > 0)
                    {
                        //then from the servive error message is put in the scheduled request id property
                        result.ErrorMessage = response.ScheduledRequestIds[0];
                    }
                    result.IsSuccess = response.IsSuccess;
                    result.ScheduledRequestIds = response.ScheduledRequestIds;
                    result.MachineName = node;
                    results.Add(result);
                }

            });
            return results;
        }

        private static List<Result> DelegateOnIAPNodeAgent(ExecutingEntity entity)
        {
            List<Result> results = new List<Result>();
            entity.RemoteServerNames.AsParallel().ForAll(node =>
            {
                string serviceOnNodeUrl = "";
                if (entity.IapNodeTransport == IapNodeTransportType.Http)
                    serviceOnNodeUrl = "http://" + node + "." + entity.Domain + ":" + entity.IapNodeHttpPort + "/iap/rest";
                else if (entity.IapNodeTransport == IapNodeTransportType.Nettcp)
                    serviceOnNodeUrl = "net.tcp://" + node + "." + entity.Domain + ":" + entity.IapNodeNetTcpPort + "/iap";
                NodeChannel channel = new NodeChannel(serviceOnNodeUrl);
                switch (entity.EntityType)
                {
                    case ExecutingEntityType.Script:
                        ExecuteScriptReq requestsc = new ExecuteScriptReq();
                        requestsc.CategoryId = entity.CategoryId;
                        requestsc.ScriptId = entity.ScriptId; ;
                        requestsc.Parameters = Translator.ScriptParameter_PE_SE.ScriptParameterListPEtoSE(entity.Parameters);
                        lock (results)
                        {
                            var res = Translator.Result_SE_IE.ResultSEtoIE(channel.ServiceChannel.ExecuteScript(requestsc));
                            res.MachineName = node;
                            results.Add(res);
                        }
                        break;
                    case ExecutingEntityType.Workflow:
                        ExecuteWfReq requestwf = new ExecuteWfReq();
                        requestwf.CategoryId = entity.CategoryId; ;
                        requestwf.WorkflowId = entity.WorkflowId;
                        requestwf.WorkflowVer = entity.WorkflowVersion;
                        requestwf.Parameters = Translator.WorkflowParameter_PE_SE.WorkflowParameterListPEtoSE(entity.Parameters);
                        lock (results)
                        {
                            var res = Translator.Result_SE_IE.ResultSEtoIE(channel.ServiceChannel.ExecuteWf(requestwf));
                            res.MachineName = node;
                            results.Add(res);
                        }
                        break;
                }
            });
            return results;
        }

        private static string JSONSerialize(object obj)
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
}
