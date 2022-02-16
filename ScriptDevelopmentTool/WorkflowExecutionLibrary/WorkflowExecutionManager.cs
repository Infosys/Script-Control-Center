/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Activities;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;
using Infosys.WEM.Service.Common.Contracts.Message;
using Infosys.WEM.Client;
using Infosys.WEM.Service.Contracts.Message;
using System.Activities.XamlIntegration;
using Infosys.WEM.Infrastructure.Common;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Infosys.WEM.Service.Contracts.Data;
using System.Linq;
using System.Diagnostics;
using Infosys.WEM.SecureHandler;
using System.Text;
using System.Configuration;
using Infosys.IAP.CommonClientLibrary;
using IAP.Infrastructure.Services.Contracts;
using System.Management;
using System.ServiceModel;
using System.Activities.Tracking;
using System.Activities.Debugger;
using System.Globalization;

namespace Infosys.WEM.WorkflowExecutionLibrary
{
    public class WorkflowExecutionManager
    {
        static string companyId = "";
        static string pauseStatesType = "MissingInformation,WaitingForApproval,3rdPartyPending,RequestInformation";
        Guid _workflowInstanceId = Guid.Empty;
        bool workflowPaused = false;
        #region Event- SendExecutionStatus
        public class SendExecutionStatusArgs : EventArgs
        {
            public Guid Identifier { get; set; }
            public string WorkflowId { get; set; }
            public string StatusMessage { get; set; }
            public bool IsSuccess { get; set; }
            public string ServerName { get; set; }
            public int PercentComplete { get; set; }

        }
        public delegate void SendExecutionStatusEventHandler(SendExecutionStatusArgs e);
        public event SendExecutionStatusEventHandler SendExecutionStatus;

        public delegate Dictionary<object, SourceLocation> UpdateSourceLocationMappingEventHandler();
        public event UpdateSourceLocationMappingEventHandler UpdateSourceLocationMapping;

        public delegate void ShowDebugEventHandler(SourceLocation srcLoc);
        public event ShowDebugEventHandler ShowDebug;

        public delegate void RemoveDebugAdornmentEventHandler();
        public event RemoveDebugAdornmentEventHandler RemoveDebugAdornment;

        #endregion

        public bool EnableWfTraceDetails { get; set; }

        #region Event- Workflow execution completed, to be used in asynchronous mode
        public class WorkflowExecutionCompletedArgs : EventArgs
        {
            public ExecutionResult ExecutionOutput { get; set; }
            public string WorkflowId { get; set; }
            public Guid WorkflowInstanceId { get; set; }
        }
        public delegate void WorkflowExecutionCompletedEventHandler(WorkflowExecutionCompletedArgs e);
        public event WorkflowExecutionCompletedEventHandler WorkflowExecutionCompleted;

        //the below delegate to convert the synchronous capability to asynchronous
        private delegate ExecutionResult AsyncExecuteCaller1(WorkflowIndentifier workflow);
        private delegate ExecutionResult AsyncExecuteCaller2(string wfText, List<Parameter> wfParams);

        AsyncExecuteCaller1 caller1 = null;
        AsyncExecuteCaller2 caller2 = null;
        #endregion

        /// <summary>
        /// For published workflow: To be called to start the execution of the workflow in the asynchronous mode.
        /// once the execiton is complete, an event- WorkflowExecutionCompleted is raised having the result of execution.
        /// </summary>
        /// <param name="workflow">The identifying details of the workflow to be executed</param>
        public void ExecuteAsync(WorkflowIndentifier workflow)
        {
            caller1 = new AsyncExecuteCaller1(Execute);
            IAsyncResult result = caller1.BeginInvoke(workflow, CallBackForWorkflowCompleted1, workflow.WorkflowId);
        }

        /// <summary>
        /// For local/unpublished workflow: To be called to start the execution of the workflow in the asynchronous mode.
        /// once the execiton is complete, an event- WorkflowExecutionCompleted is raised having the result of execution.
        /// </summary>
        /// <param name="wfText">the work xaml</param>
        /// <param name="wfParams">input parameters needed (if any) for the workflow to execute</param>
        public void ExecuteAsync(string wfText, List<Parameter> wfParams)
        {
            caller2 = new AsyncExecuteCaller2(Execute);
            IAsyncResult result = caller2.BeginInvoke(wfText, wfParams, CallBackForWorkflowCompleted2, null);
        }

        private void CallBackForWorkflowCompleted1(IAsyncResult result)
        {
            ExecutionResult executionResult = caller1.EndInvoke(result);
            if (WorkflowExecutionCompleted != null)
            {
                string workflowId = result.AsyncState.ToString();
                WorkflowExecutionCompleted(new WorkflowExecutionCompletedArgs() { WorkflowId = workflowId, ExecutionOutput = executionResult, WorkflowInstanceId = _workflowInstanceId });
            }
        }

        private void CallBackForWorkflowCompleted2(IAsyncResult result)
        {
            ExecutionResult executionResult = caller2.EndInvoke(result);
            if (WorkflowExecutionCompleted != null)
            {
                string workflowId = "";// as this is for unpublished or dirty/edited workflow... result.AsyncState as string;
                WorkflowExecutionCompleted(new WorkflowExecutionCompletedArgs() { WorkflowId = workflowId, ExecutionOutput = executionResult, WorkflowInstanceId = _workflowInstanceId });
            }
        }

        /// <summary>
        /// The interface to start the execution of the intended workflow.
        /// Make sure that in the http store the right mime type for xaml is added to download the workflow file, otherwise 404 error will be returned from the http store.
        /// </summary>
        /// <param name="workflow">The identifying details of the workflow to be executed</param>
        /// <returns>The result of execution of the workflow</returns>
        public ExecutionResult Execute(WorkflowIndentifier workflow)
        {
            Infosys.ATR.RepositoryAccess.Entity.WorkflowDoc workflowDoc = null;
            ExecutionResult executionOutput = new ExecutionResult();

            //assign the workflow instance id- this is needed for the workbench or client to co-relate the different events raised
            _workflowInstanceId = workflow.Identifier;

            //prepare for the transaction start entry
            TransactionRepository transacRepoClient = new TransactionRepository();
            var transacChannel = transacRepoClient.ServiceChannel;
            Node.Service.Contracts.Message.LogTransactionResMsg transacLogRes = null;

            if (string.IsNullOrEmpty(workflow.path))
                if (!UserHasAccess(workflow.CategoryId))
                    throw new Exception("User Does Not have Access to Run this Workflow");

            IDictionary<string, object> wfOutParams = new Dictionary<string, object>();
            var executionTime = System.Diagnostics.Stopwatch.StartNew();
            GetWorkflowDetailsResMsg response = null;
            Guid Identifier = Guid.Empty;
            bool IslongRunningWorkflow = false;
            try
            {
                if (SendExecutionStatus != null)
                {
                    sendExecutionStatus(true, "", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.ffffff") + " Workflow Execution Initiated... " + Environment.NewLine, "", workflow.Identifier, 5);
                }
                if (workflow.Identifier != null)
                {
                    Identifier = workflow.Identifier;
                }
                if (string.IsNullOrEmpty(workflow.path))
                {
                    WorkflowAutomation workflowClient = new WorkflowAutomation(workflow.WEMWorkflowServiceUrl);
                    //this approach followed in handle the scenario where wcf service is called from wcf service
                    //issue reference- http://blogs.msdn.com/b/pedram/archive/2008/07/19/webchannelfactory-inside-a-wcf-service.aspx
                    var channel = workflowClient.ServiceChannel;
                    //response = GetResponse<GetWorkflowDetailsResMsg>(workflowClient.ServiceUrl + "/GetWorkflowDetails?categoryId=" + workflow.CategoryId + "&workflowId=" + workflow.WorkflowId + "&workflowVer=" + workflow.WorkflowVersion + "&requestId=" + "" + "&requestorSourceIp=" + "");
                    using (new OperationContextScope((IContextChannel)channel))
                    {
                        response = channel.GetWorkflowDetails(workflow.CategoryId, workflow.WorkflowId, workflow.WorkflowVersion, "", "");
                    }


                    if (response != null)
                    {
                        IslongRunningWorkflow = response.WorkflowDetails.IslongRunningWorkflow;
                        string workflowurl = response.WorkflowDetails.WorkflowURI;

                        //if (string.IsNullOrEmpty(workflow.LastWorkflowStateId))
                        //{
                        response.WorkflowDetails.Parameters.ForEach(param =>
                        {
                            if (param.IsSecret && param.ParamType.Equals(ParamDirection.In))
                            {
                                workflow.Parameters.ForEach(x =>
                                {
                                    if (x.ParameterName.Equals(param.Name))
                                        x.ParameterValue = SecurePayload.UnSecure(Convert.ToString(x.ParameterValue), "IAP2GO_SEC!URE");
                                });
                            }

                            if (param.IsReferenceKey)
                            {
                                if (param.ParamType.Equals(ParamDirection.In))
                                {
                                    if (string.IsNullOrEmpty(workflow.ReferenceKey))
                                        workflow.ReferenceKey = string.Format("{0}={1}", param.Name, workflow.Parameters.Where(x => x.ParameterName.Equals(param.Name)).FirstOrDefault().ParameterValue);
                                    else
                                        workflow.ReferenceKey += string.Format(";{0}={1}", param.Name, workflow.Parameters.Where(x => x.ParameterName.Equals(param.Name)).FirstOrDefault().ParameterValue);
                                }
                            }
                        });
                        //}
                        //else
                        //    workflow.Parameters = null;

                        //log the transaction
                        if (workflow.WorkflowVersion>0)
                            transacLogRes = transacChannel.LogTransaction(GetTransactionDetails(workflow.CategoryId, response.WorkflowDetails.WorkflowURI.Substring(response.WorkflowDetails.WorkflowURI.LastIndexOf('.') + 1), workflow.WorkflowId.ToString(), workflow.ReferenceKey, workflow.WorkflowVersion.ToString(), Node.Service.Contracts.Data.StateType.InProgress, workflow.TransactionInstanceId, workflow.TransDescription, workflow.TransactionMetadata, workflow.BookMark));

                        //
                        if (string.IsNullOrEmpty(workflow.LastWorkflowStateId))
                        {
                            if (workflow.Parameters != null)
                            {
                                List<WorkflowParam> inParams = response.WorkflowDetails.Parameters.FindAll(u => u.ParamType == ParamDirection.In);
                                if (inParams.Count() != workflow.Parameters.Count())
                                {
                                    //log error
                                    executionOutput.IsSuccess = false;
                                    executionOutput.ErrorMessage = "Please supply all input parameters, check expected parameters in the published workflow";
                                    LogHandler.LogError(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
                                    return executionOutput;
                                }
                                else
                                {
                                    var wfiParam = workflow.Parameters.Where(p => string.IsNullOrEmpty(p.ParameterName)).ToList();

                                    if (wfiParam.Count() > 0)
                                    {
                                        if (inParams.Count() != wfiParam.Count())
                                        {
                                            //log error
                                            executionOutput.IsSuccess = false;
                                            executionOutput.ErrorMessage = "Please supply all input parameters, check expected parameters in the published workflow";
                                            LogHandler.LogError(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
                                            return executionOutput;
                                        }
                                        else
                                        {
                                            int i = 0;
                                            foreach (WorkflowParam p in response.WorkflowDetails.Parameters)
                                            {
                                                workflow.Parameters[i].ParameterName = p.Name;
                                                if (p.IsSecret)
                                                    workflow.Parameters[i].ParameterValue = SecurePayload.UnSecure(Convert.ToString(workflow.Parameters[i].ParameterValue), "IAP2GO_SEC!URE");
                                                i = i + 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        inParams.ForEach(param =>
                                        {
                                            workflow.Parameters.ForEach(p =>
                                            {
                                                if (p.ParameterName.Equals(param.Name, StringComparison.InvariantCultureIgnoreCase))
                                                    p.ParameterName = param.Name;
                                            });
                                        });
                                    }
                                }
                            }
                        }

                        //download the work flow xaml and then invoke it
                        string storageBaseUrl = GetStorageBaseUrl(workflow.WEMWorkflowServiceUrl); //CommonServices.Instance.StorageBaseURL;
                        workflowurl = storageBaseUrl + workflowurl;
                        workflow.path = workflowurl; //to be used later to under what is the type i.e. .xaml or .iapw

                        //then set the workflow url to the TLS to be used in case of iapw kind of workflow
                        System.Threading.Thread.FreeNamedDataSlot("iappackageurl");
                        LocalDataStoreSlot localData = System.Threading.Thread.AllocateNamedDataSlot("iappackageurl");
                        System.Threading.Thread.SetData(localData, workflowurl);

                        companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
                        workflowDoc = Translator.Workflow_DocumentEntry.WorkflowToDocument(response.WorkflowDetails, storageBaseUrl, companyId);
                        Infosys.ATR.RepositoryAccess.FileRepository.WorkflowRepositoryDS workflowDocDS = new Infosys.ATR.RepositoryAccess.FileRepository.WorkflowRepositoryDS();
                        if (SendExecutionStatus != null)
                        {
                            sendExecutionStatus(true, Convert.ToString(workflow.WorkflowId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Download of the WorkflowFile Started... ", "", Identifier, 20);
                        }
                        workflowDoc = workflowDocDS.Download(workflowDoc);

                        //raise event DownloadedScriptFile
                        if (SendExecutionStatus != null)
                        {
                            SendExecutionStatusArgs e = new SendExecutionStatusArgs();

                            e.IsSuccess = true;


                            e.StatusMessage = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Workflow Downloaded...";
                            e.StatusMessage += "\nWorkflow Details: " + workflowDoc.FileName;


                            sendExecutionStatus(e.IsSuccess, Convert.ToString(workflow.WorkflowId), e.StatusMessage, "", Identifier, 55);
                        }
                        //rasie event ScriptExecutionStarting
                        if (SendExecutionStatus != null)
                        {
                            sendExecutionStatus(true, Convert.ToString(workflow.WorkflowId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Workflow Execution Started...", "", Identifier, 80);
                        }
                    }
                    else
                    {
                        //log error
                        executionOutput.IsSuccess = false;
                        executionOutput.ErrorMessage = string.Format("Unable to fetch the work flow with details with category id- {0}, workflow id- {1} and version- {2}", workflow.CategoryId, workflow.WorkflowId, workflow.WorkflowVersion);
                        LogHandler.LogError(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
                        return executionOutput;
                    }

                }
                else
                {
                    if (!WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(Path.GetFileNameWithoutExtension(workflow.path)))
                    {
                        Stream fs = File.OpenRead(workflow.path);
                        workflowDoc = new Infosys.ATR.RepositoryAccess.Entity.WorkflowDoc();
                        workflowDoc.File = fs;
                    }
                    else
                    {
                        throw new Exception("Invalid Characters in File Name");
                    }
                }

                //invoke the workflow
                string wfContent = "";

                if (workflow.path.Contains(".xaml"))
                {
                    if (workflowDoc.File != null)
                    {
                        using (var sr = new StreamReader(workflowDoc.File))
                        {
                            workflowDoc.File.Position = 0;
                            wfContent = sr.ReadToEnd();
                        }
                    }
                }
                else if (workflow.path.EndsWith(".iapw", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (workflowDoc.File != null)
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        using (Stream input = workflowDoc.File)
                        {
                            input.CopyTo(memoryStream);

                            memoryStream.Position = 0;
                            if (Encoding.Unicode.GetString(memoryStream.ToArray()).Contains(SecurePayload.keyText))
                            {
                                byte[] outStreamArray = SecurePayload.UnSecureBytes(memoryStream.ToArray());
                                workflowDoc.File = new System.IO.MemoryStream(outStreamArray);
                            }
                            //then first extract the xaml from the iapw stream
                            var sr = new StreamReader(Infosys.ATR.Packaging.Operations.ExtractFile(workflowDoc.File, @"\main.xaml"));
                            wfContent = sr.ReadToEnd();
                        }
                    }
                }
                else if (workflow.path.EndsWith(".iapl", StringComparison.InvariantCultureIgnoreCase))
                {
                    Infosys.IAP.CommonClientLibrary.Models.ContentMeta mContent = new IAP.CommonClientLibrary.Models.ContentMeta();
                    Stream cStream = null; string extractionLoc = string.Empty;
                    IAPPackage.Import(workflow.path, out mContent, out cStream, out extractionLoc);

                    if (mContent != null && cStream != null)
                    {
                        mContent.Parameters.ForEach(param =>
                        {
                            if (param.IsSecret && param.IOType.Equals(ParamDirection.In))
                            {
                                workflow.Parameters.ForEach(x =>
                                {
                                    if (x.ParameterName.Equals(param.Name))
                                        x.ParameterValue = SecurePayload.UnSecure(Convert.ToString(x.ParameterValue), "IAP2GO_SEC!URE");
                                });
                            }
                        });

                        if (workflow.Parameters != null)
                        {
                            if (mContent.Parameters.Count() != workflow.Parameters.Count())
                            {
                                //log error
                                executionOutput.IsSuccess = false;
                                executionOutput.ErrorMessage = "Please supply all input parameters, check expected parameters in the published workflow";
                                LogHandler.LogError(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
                                return executionOutput;
                            }
                            else
                            {
                                int i = 0;
                                foreach (var p in mContent.Parameters)
                                {
                                    workflow.Parameters[i].ParameterName = p.Name;
                                    i = i + 1;
                                }
                            }
                        }

                        if (mContent.ContentType.Equals("xaml", StringComparison.InvariantCultureIgnoreCase))
                            wfContent = new StreamReader(cStream).ReadToEnd();
                        else if (mContent.ContentType.Equals("iapw", StringComparison.InvariantCultureIgnoreCase))
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            using (Stream input = cStream)
                            {
                                input.CopyTo(memoryStream);

                                memoryStream.Position = 0;
                                if (Encoding.Unicode.GetString(memoryStream.ToArray()).Contains(SecurePayload.keyText))
                                {
                                    byte[] outStreamArray = SecurePayload.UnSecureBytes(memoryStream.ToArray());
                                    cStream = new System.IO.MemoryStream(outStreamArray);
                                }
                                //then first extract the xaml from the iapw stream
                                var iapw = new StreamReader(Infosys.ATR.Packaging.Operations.ExtractFile(cStream, @"\main.xaml"));
                                wfContent = iapw.ReadToEnd();
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(wfContent))
                    throw new Exception("Invalid XAML");

                DynamicActivity wf = ActivityXamlServices.Load(new System.IO.StringReader(wfContent)) as DynamicActivity;

                WFActivityTrackingParticipant wfTrackingParticipant = null;
               
                DebugWorkflow(ref wfTrackingParticipant);

                IDictionary<string, object> wfInParams = null;
                if (workflow.Parameters != null)
                    wfInParams = ProcessInParameters(wf, workflow.Parameters);

                //rasie event WorkflowExecutionStarting
                //instead now use the property from workflow metadata
                //if (!response.WorkflowDetails.IslongRunningWorkflow)
                if (!IslongRunningWorkflow)
                {
                    #region Commented code - Old Approach to Invoke Workflow
                    //if (wfInParams != null && wfInParams.Count > 0)
                    //    wfOutParams = WorkflowInvoker.Invoke(wf, wfInParams);
                    //else
                    //    wfOutParams = WorkflowInvoker.Invoke(wf);
                    #endregion

                    WorkflowInvoker invoker = new WorkflowInvoker(wf);
                    invoker.Extensions.Add(wfTrackingParticipant);

                    if (wfInParams != null && wfInParams.Count > 0)
                        wfOutParams = invoker.Invoke(wfInParams, new TimeSpan(1, 0, 0));
                    else
                        wfOutParams = invoker.Invoke(new TimeSpan(1, 0, 0));

                    //log transaction for script execution complete
                    if (transacLogRes != null)
                    {
                        transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg() { Request = new Node.Service.Contracts.Data.Transaction() { CategoryId = workflow.CategoryId, ModuleId = workflow.WorkflowId.ToString(), InstanceId = transacLogRes.InstanceId, CurrentState = Node.Service.Contracts.Data.StateType.Completed, Module = Node.Service.Contracts.Data.ModuleType.Workflow } });
                    }
                }
                else
                {
                    Infosys.ATR.WorkflowStateManagement.Operations wfStateOp = new ATR.WorkflowStateManagement.Operations();
                    Guid persistedState;
                    if (Guid.TryParse(workflow.LastWorkflowStateId, out persistedState)) //i.e. sebsequent call for stateful workflow execution
                    {
                        ATR.WorkflowStateManagement.Entity.Result res = null;
                        if (workflow.UnloadWorkflowFromMemory)
                            res = wfStateOp.Resume(persistedState, wf, wfInParams, ATR.WorkflowStateManagement.PeristenceType.PersistAndUnload, workflow.BookMark, wfTrackingParticipant);
                        else
                            res = wfStateOp.Resume(persistedState, wf, wfInParams, ATR.WorkflowStateManagement.PeristenceType.Persist, workflow.BookMark, wfTrackingParticipant);
                        //var res = wfStateOp.Resume(persistedState, wf, wfInParams, workflow.UnloadWorkflowFromMemory, workflow.RePersistWorkflowState);
                        if (res != null && res.Output != null)
                            wfOutParams = res.Output;

                        //log transaction for script execution complete/pause
                        if (transacLogRes != null)
                        {
                            if (wfStateOp.CurrentState == ATR.WorkflowStateManagement.State.Completed)
                                transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg() { Request = new Node.Service.Contracts.Data.Transaction() { CategoryId = workflow.CategoryId, ModuleId = workflow.WorkflowId.ToString(), InstanceId = transacLogRes.InstanceId, CurrentState = Node.Service.Contracts.Data.StateType.Completed, Module = Node.Service.Contracts.Data.ModuleType.Workflow } });
                            else
                            {
                                workflowPaused = true;
                                transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg() { Request = new Node.Service.Contracts.Data.Transaction() { CategoryId = workflow.CategoryId, ModuleId = workflow.WorkflowId.ToString(), InstanceId = transacLogRes.InstanceId, CurrentState = Node.Service.Contracts.Data.StateType.Paused, Module = Node.Service.Contracts.Data.ModuleType.Workflow, WorkflowPersistedStateId = res.PersistedWorkflowInstanceId.ToString(), WorkflowActivityBookmark = res.BookMarkOfPausingActivity } });
                            }
                        }
                    }
                    else //i.e. first time stateful workflow execution
                    {
                        var res = wfStateOp.Invoke(wf, wfInParams, workflow.UnloadWorkflowFromMemory, wfTrackingParticipant);
                        if (res != null && res.Output != null)
                            wfOutParams = res.Output;

                        //log transaction for script execution complete/pause
                        if (transacLogRes != null)
                        {
                            if (wfStateOp.CurrentState == ATR.WorkflowStateManagement.State.Completed)
                                transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg() { Request = new Node.Service.Contracts.Data.Transaction() { CategoryId = workflow.CategoryId, ModuleId = workflow.WorkflowId.ToString(), InstanceId = transacLogRes.InstanceId, CurrentState = Node.Service.Contracts.Data.StateType.Completed, Module = Node.Service.Contracts.Data.ModuleType.Workflow } });
                            else
                            {
                                workflowPaused = true;
                                transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg() { Request = new Node.Service.Contracts.Data.Transaction() { CategoryId = workflow.CategoryId, ModuleId = workflow.WorkflowId.ToString(), InstanceId = transacLogRes.InstanceId, CurrentState = Node.Service.Contracts.Data.StateType.Paused, Module = Node.Service.Contracts.Data.ModuleType.Workflow, WorkflowPersistedStateId = res.PersistedWorkflowInstanceId.ToString(), WorkflowActivityBookmark = res.BookMarkOfPausingActivity } });
                            }
                        }
                    }
                }

                if (wfOutParams != null && wfOutParams.Count > 0)
                {
                    if (string.IsNullOrEmpty(workflow.path))
                    {
                        if (response != null)
                        {
                            response.WorkflowDetails.Parameters.ForEach(param =>
                            {
                                if (param.IsSecret && param.ParamType.Equals(ParamDirection.Out))
                                {
                                    if (wfOutParams.ContainsKey(param.Name))
                                    {
                                        if (wfOutParams[param.Name].GetType() == typeof(string))
                                        {
                                            wfOutParams[param.Name] = SecurePayload.Secure(Convert.ToString(wfOutParams[param.Name]), "IAP2GO_SEC!URE");
                                        }
                                    }
                                }
                            });
                        }
                    }

                    List<Parameter> outParam = new List<Parameter>();
                    foreach (KeyValuePair<string, object> dic in wfOutParams)
                    {
                        outParam.Add(new Parameter { ParameterName = dic.Key, ParameterValue = dic.Value });
                    }

                    if (string.IsNullOrEmpty(workflow.path))
                    {
                        if (response != null)
                        {
                            response.WorkflowDetails.Parameters.ForEach(param =>
                            {
                                if (param.IsSecret && param.ParamType.Equals(ParamDirection.Out))
                                {
                                    outParam.ForEach(x =>
                                    {
                                        x.IsSecret = (x.ParameterName.Equals(param.Name)) ? param.IsSecret : false;
                                    });
                                }
                            });
                        }
                    }

                    executionOutput.Output = outParam;
                }

                executionOutput.IsSuccess = true;
                executionOutput.SuccessMessage = string.Format("Successfully invoked work flow with details category id- {0}, workflow id- {1} and version- {2}", workflow.CategoryId, workflow.WorkflowId, workflow.WorkflowVersion);

                if (!string.IsNullOrEmpty(workflow.path))
                    if (!workflow.path.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                        executionOutput.SuccessMessage = string.Format("Successfully invoked work flow with details workflow path- {0}", workflow.path);

                //check if the workflow status is mentioned in the TLS, if so then add the same to the success message
                try
                {
                    LocalDataStoreSlot stateData = System.Threading.Thread.GetNamedDataSlot("iapworkflowstate");
                    if (stateData != null)
                    {
                        string data = System.Threading.Thread.GetData(stateData).ToString();
                        if (!string.IsNullOrEmpty(data))
                        {
                            TicketState stateId = (TicketState)Convert.ToInt32(data);
                            // check if the value is same as one of the expected allowed pause states
                            if (stateId == TicketState.ClientPending_RequestInformation || stateId == TicketState.ClientPending_WaitingForApproval || stateId == TicketState.ThirdPartyPending)
                            {
                                executionOutput.SuccessMessage += ". Current State of the workflow is- Workflow Paused";
                            }
                        }
                    }
                }
                catch
                {
                    //i.e. there isnt any TLS data slot by that name
                }

                //log debug
                if (SendExecutionStatus != null)
                {
                    if (workflowPaused)
                        sendExecutionStatus(true, Convert.ToString(workflow.WorkflowId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Workflow Execution Paused..." + Environment.NewLine, "", Identifier, 99);
                    else
                        sendExecutionStatus(true, Convert.ToString(workflow.WorkflowId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Workflow Execution Completed..." + Environment.NewLine, "", Identifier, 99);
                }
                LogHandler.LogInfo(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);

            }
            catch (Exception ex)
            {
                //log transaction for script execution complete
                if (transacLogRes != null)
                {
                    transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg() { Request = new Node.Service.Contracts.Data.Transaction() { CategoryId = workflow.CategoryId, ModuleId = workflow.WorkflowId.ToString(), InstanceId = transacLogRes.InstanceId, CurrentState = Node.Service.Contracts.Data.StateType.Failed, Module = Node.Service.Contracts.Data.ModuleType.Workflow, Description = ex.Message } });
                }

                string err = ex.Message + "\n" + ex.StackTrace;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                executionOutput.IsSuccess = false;
                executionOutput.ErrorMessage = err;
                sendExecutionStatus(false, Convert.ToString(workflow.WorkflowId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Workflow Execution Completed..." + Environment.NewLine + err, "", Identifier, 99);
                LogHandler.LogError(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
            }

            TrackWF(workflow.Parameters, executionOutput, executionTime, workflow.WorkflowId.ToString());

            if (RemoveDebugAdornment != null)
                RemoveDebugAdornment();

            return executionOutput;
        }

        public List<ExecutionResult> Execute(WorkflowIndentifier2 workflow)
        {
            List<ExecutionResult> consolidatedResult = new List<ExecutionResult>();
            ExecutionResult executionOutput = new ExecutionResult();

            //prepare for the transaction start entry
            TransactionRepository transacRepoClient = new TransactionRepository();
            var transacChannel = transacRepoClient.ServiceChannel;
            Node.Service.Contracts.Message.LogTransactionResMsg transacLogRes = null;

            if (!UserHasAccess(workflow.CategoryId))
                throw new Exception("User Does Not have Access to Run this Workflow");

            IDictionary<string, object> wfOutParams = new Dictionary<string, object>();
            var executionTime = System.Diagnostics.Stopwatch.StartNew();
            Guid Identifier = Guid.Empty;
            try
            {
                if (SendExecutionStatus != null)
                {
                    sendExecutionStatus(true, "", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.ffffff") + " Workflow Execution Initiated... " + Environment.NewLine, "", workflow.Identifier, 5);
                }
                if (workflow.Identifier != null)
                {
                    Identifier = workflow.Identifier;
                }



                WorkflowAutomation workflowClient = new WorkflowAutomation(workflow.WEMWorkflowServiceUrl);
                //this approach followed in handle the scenario where wcf service is called from wcf service
                //issue reference- http://blogs.msdn.com/b/pedram/archive/2008/07/19/webchannelfactory-inside-a-wcf-service.aspx
                var channel = workflowClient.ServiceChannel;
                //GetWorkflowDetailsResMsg response = GetResponse<GetWorkflowDetailsResMsg>(workflowClient.ServiceUrl + "/GetWorkflowDetails?categoryId=" + workflow.CategoryId + "&workflowId=" + workflow.WorkflowId + "&workflowVer=" + workflow.WorkflowVersion + "&requestId=" + "" + "&requestorSourceIp=" + "");
                GetWorkflowDetailsResMsg response = null;
                using (new OperationContextScope((IContextChannel)channel))
                {
                    response = channel.GetWorkflowDetails(workflow.CategoryId, workflow.WorkflowId, workflow.WorkflowVersion, "", "");
                }

                if (response != null)
                {
                    response.WorkflowDetails.Parameters.ForEach(param =>
                    {
                        if (param.IsSecret && param.ParamType.Equals(ParamDirection.In))
                        {
                            workflow.Parameters.ForEach(x =>
                            {
                                if (!(workflow.ExecutionMode == ExecutionModeType.ScheduledOnIAPCluster || workflow.ExecutionMode == ExecutionModeType.ScheduledOnIAPNode))
                                    if (x.ParameterName.Equals(param.Name))
                                        x.ParameterValue = SecurePayload.UnSecure(Convert.ToString(x.ParameterValue), "IAP2GO_SEC!URE");
                            });
                        }
                    });
                }
                //check the execution mode
                if (workflow.ExecutionMode == ExecutionModeType.RunOnIAPNode || workflow.ExecutionMode == ExecutionModeType.ScheduledOnIAPCluster || workflow.ExecutionMode == ExecutionModeType.ScheduledOnIAPNode)
                {
                    Infosys.ATR.RemoteExecute.ExecutingEntity entity = new ATR.RemoteExecute.ExecutingEntity();
                    entity.CategoryId = workflow.CategoryId;
                    entity.ComapnyId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Company"]);
                    entity.Domain = workflow.Domain;
                    entity.EntityType = ATR.RemoteExecute.ExecutingEntityType.Workflow;
                    entity.ExecutionMode = (Infosys.ATR.RemoteExecute.ExecutionModeType)((int)workflow.ExecutionMode);
                    if (entity.ExecutionMode == ATR.RemoteExecute.ExecutionModeType.RunOnIAPNode || entity.ExecutionMode == ATR.RemoteExecute.ExecutionModeType.ScheduledOnIAPNode)
                        entity.RemoteServerNames = workflow.RemoteServerNames.Split(',').ToList();
                    else if (entity.ExecutionMode == ATR.RemoteExecute.ExecutionModeType.ScheduledOnIAPCluster)
                        entity.ScheduleOnClusters = workflow.ScheduleOnClusters;
                    if (workflow.IapNodeHttpPort != 0)
                    {
                        entity.IapNodeHttpPort = workflow.IapNodeHttpPort;
                    }
                    else
                    {
                        entity.IapNodeHttpPort = 9001;
                    }
                    if (workflow.IapNodeNetTcpPort != 0)
                    {
                        entity.IapNodeNetTcpPort = workflow.IapNodeNetTcpPort;
                    }
                    else
                    {
                        entity.IapNodeNetTcpPort = 9002;
                    }
                    entity.IapNodeTransport = (Infosys.ATR.RemoteExecute.IapNodeTransportType)((int)workflow.IapNodeTransport);
                    if (workflow.Parameters != null && workflow.Parameters.Count > 0)
                    {
                        if (entity.Parameters == null)
                            entity.Parameters = new List<ATR.RemoteExecute.Parameter>();

                        workflow.Parameters.ForEach(p =>
                        {
                            entity.Parameters.Add(new ATR.RemoteExecute.Parameter()
                            {
                                ParameterName = p.ParameterName,
                                ParameterValue = (p.IsSecret && p.ParameterValue.GetType().Equals(typeof(string))) ? SecurePayload.Secure(p.ParameterValue.ToString(), "IAP2GO_SEC!URE") : p.ParameterValue.ToString(),
                                IsSecret = p.IsSecret
                            });
                        });
                    }
                    entity.ScheduledPattern = (ATR.RemoteExecute.ScheduledPatternType)((int)workflow.ScheduledPattern);
                    entity.ScheduleEndDateTime = workflow.ScheduleEndDateTime;
                    entity.ScheduleOccurences = workflow.ScheduleOccurences;
                    entity.SchedulePriority = workflow.SchedulePriority;
                    entity.ScheduleStartDateTime = workflow.ScheduleStartDateTime;
                    entity.ScheduleStopCriteria = (ATR.RemoteExecute.ScheduleStopCriteriaType)((int)workflow.ScheduleStopCriteria);
                    entity.WorkflowId = workflow.WorkflowId.ToString();
                    entity.WorkflowVersion = workflow.WorkflowVersion;
                    entity.UsesUIAutomation = workflow.UsesUIAutomation;

                    var remoteResult = Infosys.ATR.RemoteExecute.Hanlder.DelegateExecution(entity);
                    if (remoteResult != null && remoteResult.Count > 0)
                    {
                        if (consolidatedResult == null)
                            consolidatedResult = new List<ExecutionResult>();
                        remoteResult.ForEach(r =>
                        {
                            ExecutionResult tempresult = new ExecutionResult();
                            if (r.Output != null && r.Output.Count > 0)
                            {
                                if (tempresult.Output == null)
                                    tempresult.Output = new List<Parameter>();
                                r.Output.ForEach(o =>
                                {
                                    tempresult.Output.Add(new Parameter() { ParameterName = o.ParameterName, ParameterValue = o.ParameterValue });
                                });
                            }
                            //tempresult.ComputerName =""; is needed for remote using powershell engine.
                            tempresult.ErrorMessage = r.ErrorMessage;
                            tempresult.IsSuccess = r.IsSuccess;
                            tempresult.SuccessMessage = r.SuccessMessage;
                            tempresult.ScheduledRequestIds = r.ScheduledRequestIds;
                            tempresult.MachineName = r.MachineName;
                            consolidatedResult.Add(tempresult);
                        });
                    }
                }
                else
                    throw new Exception("Incorrect interface called for the provided Execution Mode");

            }
            catch (Exception ex)
            {
                string err = ex.Message + "\n" + ex.StackTrace;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                executionOutput.IsSuccess = false;
                executionOutput.ErrorMessage = err;
                consolidatedResult.Add(executionOutput);
                sendExecutionStatus(false, Convert.ToString(workflow.WorkflowId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Workflow Execution Completed..." + Environment.NewLine + err, "", Identifier, 99);
                LogHandler.LogError(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
            }

            TrackWF(workflow.Parameters, executionOutput, executionTime, workflow.WorkflowId.ToString());
            return consolidatedResult;
        }

        private void sendExecutionStatus(bool isSuccess, string wfID, string statusMessage, string serverName, Guid Identifier, int Progress)
        {

            if (string.IsNullOrEmpty(serverName)) serverName = Environment.MachineName;
            SendExecutionStatusArgs e = new SendExecutionStatusArgs();
            e.IsSuccess = isSuccess;
            e.WorkflowId = wfID;
            e.StatusMessage = statusMessage;
            e.ServerName = serverName;
            e.Identifier = Identifier;
            e.PercentComplete = Progress;

            if (SendExecutionStatus != null)
                SendExecutionStatus((SendExecutionStatusArgs)e);
        }

        public ExecutionResult Execute(string wfText, List<Parameter> wfParams)
        {
            ExecutionResult executionOutput = new ExecutionResult();
            IDictionary<string, object> wfOutParams = new Dictionary<string, object>();
            var executionTime = System.Diagnostics.Stopwatch.StartNew();

            _workflowInstanceId = Guid.NewGuid();

            try
            {
                if (SendExecutionStatus != null)
                {
                    sendExecutionStatus(true, "", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.ffffff") + " Workflow Execution Initiated... " + Environment.NewLine, "", _workflowInstanceId, 5);
                }

                DynamicActivity wf = ActivityXamlServices.Load(new System.IO.StringReader(wfText)) as DynamicActivity;
                WFActivityTrackingParticipant wfTrackingParticipant = null;

                DebugWorkflow(ref wfTrackingParticipant);

                IDictionary<string, object> wfInParams = ProcessInParameters(wf, wfParams);

                WorkflowInvoker invoker = new WorkflowInvoker(wf);
                invoker.Extensions.Add(wfTrackingParticipant);

                if (wfInParams != null && wfInParams.Count > 0)
                    wfOutParams = invoker.Invoke(wfInParams, new TimeSpan(1, 0, 0));
                else
                    wfOutParams = invoker.Invoke(new TimeSpan(1, 0, 0));


                //if (wfInParams != null && wfInParams.Count > 0)
                //    wfOutParams = WorkflowInvoker.Invoke(wf, wfInParams);
                //else
                //    wfOutParams = WorkflowInvoker.Invoke(wf);

                if (wfOutParams != null && wfOutParams.Count > 0)
                {
                    List<Parameter> outParam = new List<Parameter>();
                    foreach (KeyValuePair<string, object> dic in wfOutParams)
                    {
                        outParam.Add(new Parameter { ParameterName = dic.Key, ParameterValue = dic.Value });
                    }
                    executionOutput.Output = outParam;
                }
                executionOutput.IsSuccess = true;
                executionOutput.SuccessMessage = string.Format("Successfully invoked workflow");

                if (SendExecutionStatus != null)
                    sendExecutionStatus(true, "", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Workflow Execution Completed..." + Environment.NewLine, "", _workflowInstanceId, 99);

                //log debug
                LogHandler.LogInfo(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
            }

            catch (Exception ex)
            {
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;

                //log err
                executionOutput.IsSuccess = false;
                executionOutput.ErrorMessage = err;
                LogHandler.LogError(executionOutput.ErrorMessage, LogHandler.Layer.Infrastructure, null);
            }

            TrackWF(wfParams, executionOutput, executionTime, "");
            if (RemoveDebugAdornment != null)
                RemoveDebugAdornment();

            return executionOutput;
        }

        private void TrackWF(List<Parameter> Parameters, ExecutionResult executionOutput, Stopwatch executionTime, string wfId)
        {
            var parameters = new Dictionary<string, string>();

            if (Parameters != null)
            {
                Parameters.ForEach(p =>
                {
                    parameters.Add(p.ParameterName, p.ParameterValue.ToString());
                });
            }

            if (executionOutput.Output != null)
            {
                executionOutput.Output.ForEach(o =>
                {
                    if (o.ParameterValue == null)
                        o.ParameterValue = String.Empty;

                    parameters.Add(o.ParameterName, o.ParameterValue.ToString());
                });
            }

            if (executionOutput.IsSuccess)
                IMSWorkBench.Infrastructure.Library.Services.Logger.LogWFExecutionTime(executionTime.ElapsedMilliseconds.ToString()
                , wfId, parameters, executionOutput.SuccessMessage);
            else
                IMSWorkBench.Infrastructure.Library.Services.Logger.LogWFExecutionTime(executionTime.ElapsedMilliseconds.ToString()
               , wfId, parameters, executionOutput.ErrorMessage);
        }
        /// <summary>
        /// This method is used to get StorageBaseURL value from company table
        /// </summary>
        /// <returns>Value of StorageBaseURL column</returns>
        private string GetStorageBaseUrl(string companyServiceUrl)
        {
            string storageBaseUrl = "";
            companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
            if (string.IsNullOrEmpty(companyServiceUrl))
            {
                storageBaseUrl = CommonServices.Instance.StorageBaseURL;
            }
            else
            {
                Uri uri = new Uri(companyServiceUrl);
                string serverUrl = uri.GetLeftPart(UriPartial.Authority);
                Infosys.WEM.Client.CommonRepository commonserviceClient = new Infosys.WEM.Client.CommonRepository(serverUrl + WEM.Infrastructure.Common.ApplicationConstants.COMMON_SERVICEINTERFACE);
                //Infosys.WEM.Client.CommonRepository commonserviceClient = new Infosys.WEM.Client.CommonRepository(serverUrl + "/WEMCommonService.svc");
                GetCompanyResMsg company = commonserviceClient.ServiceChannel.GetCompanyDetails(companyId);
                storageBaseUrl = company.Company.StorageBaseUrl;

            }

            return storageBaseUrl;
        }

        /// <summary>
        /// This method is used to desearlize parameter values containing complex objects and returns list of parameters.
        /// </summary>
        /// <param name="wf">Dynamic actvitiy object</param>
        /// <param name="wfParams">List of In parameters</param>
        /// <returns>Dictinory object containing parameter names and corresponding value as object</returns>
        private IDictionary<string, object> ProcessInParameters(DynamicActivity wf, List<Parameter> wfParams)
        {
            int count = 0;
            IDictionary<string, object> wfInParams = new Dictionary<string, object>();
            object paramValue = null;
            if (wfParams == null)
                throw new System.Exception("Please provide all input parameters, check published workflow for expected parameters");
            foreach (var prop in wf.Properties)
            {
                if (prop.Type.Name.Contains("InArgument") || prop.Type.Name.Contains("InOutArgument"))
                {
                    if (prop.Type.GenericTypeArguments[0].Name.Equals("String"))
                        paramValue = wfParams[count].ParameterValue;
                    else
                    {
                        if (!(wfParams[count].ParameterValue is string))
                            paramValue = wfParams[count].ParameterValue;
                        else
                            paramValue = JsonDeserialize(wfParams[count].ParameterValue.ToString(), prop.Type.GenericTypeArguments[0]);
                    }
                    wfInParams.Add(wfParams[count].ParameterName, paramValue);
                    count = count + 1;
                }
            }
            return wfInParams;
        }

        private T GetResponse<T>(string url)
        {
            WebClient srvProxy = new WebClient();
            srvProxy.Credentials = CredentialCache.DefaultCredentials;
            byte[] data = srvProxy.DownloadData(url);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(T));
            return (T)obj.ReadObject(stream);
        }

        /// <summary>
        /// This method is used to deserlize json string to the concerned type.
        /// </summary>
        /// <param name="jsonData">Input string in json format</param>
        /// <param name="type">Type of object</param>
        /// <returns>Concerned object</returns>
        private object JsonDeserialize(string jsonData, Type type)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            string rawInput = jsonData.Replace(@"\", "");
            object obje = jsSerializer.Deserialize(rawInput, type);
            return obje;
        }

        /// <summary>
        /// This method is used to serialize object to json string.
        /// </summary>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>json string</returns>
        public string JsonSerialize(object obj)
        {
            string json = new JavaScriptSerializer().Serialize(obj);
            return json;
        }

        private bool UserHasAccess(int categoryId)
        {
            int companyid = Convert.ToInt16(ConfigurationManager.AppSettings["Company"]);
            if (companyid == 0)
                throw new Exception("Invalid Configuration. Specify Company value in Application Configuration Settings");
            string[] roles = new string[3] { ApplicationConstants.ROLE_MANAGER, ApplicationConstants.ROLE_ANALYST, ApplicationConstants.ROLE_AGENT };
            return Security.CheckAccessInRole(companyid, categoryId, roles);

        }

        private static Infosys.WEM.Node.Service.Contracts.Message.LogTransactionReqMsg GetTransactionDetails(int category, string filetype, string workflowId, string referenceKey, string workflowVersion, Node.Service.Contracts.Data.StateType state, string transactionInstanceId = "", string description = "", string transactionMetadata = "", string WorkflowActivityBookmark = "")
        {
            Infosys.WEM.Node.Service.Contracts.Message.LogTransactionReqMsg transac = new Node.Service.Contracts.Message.LogTransactionReqMsg();
            transac.Request = new Node.Service.Contracts.Data.Transaction();
            transac.Request.CategoryId = category;
            transac.Request.Executor = "UI_" + GetMachineName();
            transac.Request.FileType = filetype;
            transac.Request.IPAddress = GetMachineIPAddress();
            transac.Request.MachineName = GetMachineName();
            transac.Request.Module = Node.Service.Contracts.Data.ModuleType.Workflow;
            transac.Request.ModuleId = workflowId;
            transac.Request.OSDetails = GetOSName();
            transac.Request.ReferenceKey = referenceKey;
            transac.Request.ModuleVersion = workflowVersion;

            if (!string.IsNullOrEmpty(transactionInstanceId))
            {
                transac.Request.InstanceId = transactionInstanceId;
                transac.Request.CurrentState = Node.Service.Contracts.Data.StateType.Resumed;
            }
            else
                transac.Request.CurrentState = state;

            transac.Request.Description = description;
            transac.Request.TransactionMetadata = transactionMetadata;
            if (!string.IsNullOrEmpty(WorkflowActivityBookmark))
                transac.Request.WorkflowActivityBookmark = WorkflowActivityBookmark;

            return transac;
        }

        private static string GetMachineName()
        {
            string machineName = Environment.MachineName;
            return machineName;
        }

        private static string GetMachineIPAddress()
        {
            string machineName = Environment.MachineName;
            string ipAdd = Dns.GetHostEntry(machineName).AddressList[0].ToString();
            return ipAdd;
        }

        private static string GetOSName()
        {
            string osName = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                osName = os["Caption"].ToString();
                break;
            }

            //few more info
            osName += " (" + Environment.OSVersion.VersionString + ")";
            return osName;
        }
        private Dictionary<string, Activity> BuildActivityIdToWfElementMap(Dictionary<object, SourceLocation> wfElementToSourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in wfElementToSourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;
        }

        private void DebugWorkflow(ref WFActivityTrackingParticipant wfTrackingParticipant)
        {
            Dictionary<object, SourceLocation> wfElementToSourceLocationMap = null;
            Dictionary<string, Activity> activityIdToWfElementMap = null;

            //Mapping between the Object and Line No.
            if (UpdateSourceLocationMapping != null)
                wfElementToSourceLocationMap = UpdateSourceLocationMapping();

            //Mapping between the Object and the Instance Id
            if (wfElementToSourceLocationMap != null)
                activityIdToWfElementMap = BuildActivityIdToWfElementMap(wfElementToSourceLocationMap);

            const String all = "*";
            wfTrackingParticipant = new WFActivityTrackingParticipant()
            {
                TrackingProfile = new TrackingProfile()
                {
                    Name = "CustomTrackingProfile",
                    Queries = 
                        {
                            new CustomTrackingQuery() {Name = all, ActivityName = all },
                            // Limit workflow instance tracking records for started and completed workflow states
                            new WorkflowInstanceQuery(){States = { all }},
                            new ActivityStateQuery()
                            {
                                // Subscribe for track records from all activities for all states
                                ActivityName = all,
                                States = { all },
                                // Extract workflow variables and arguments as a part of the activity tracking record
                                // VariableName = "*" allows for extraction of all variables in the scope
                                // of the activity
                                Variables ={{ all }},
                                Arguments ={{ all }}
                            }   
                        }
                }
            };

            wfTrackingParticipant.ActivityIdToWorkflowElementMap = activityIdToWfElementMap;

            if (EnableWfTraceDetails)
            {
                //As the tracking events are received
                wfTrackingParticipant.TrackingRecordReceived += (trackingParticpant, trackingEventArgs) =>
                {
                    if (trackingEventArgs.Activity != null)
                        if (wfElementToSourceLocationMap != null)
                            ShowDebug(wfElementToSourceLocationMap[trackingEventArgs.Activity]);

                    if (trackingEventArgs.ActivityState != null)
                    {
                        IDictionary<String, object> variables = trackingEventArgs.ActivityState.Variables;
                        StringBuilder vars = new StringBuilder();

                        if (variables.Count > 0)
                        {
                            vars.AppendLine("\n\tVariables:");
                            foreach (KeyValuePair<string, object> variable in variables)
                            {
                                vars.AppendLine(String.Format("\t\tName: {0} Value: {1}", variable.Key, variable.Value));
                            }
                        }
                        Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "\n{0} Activity {1} {2} {3}",
                            DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.ffffff"), trackingEventArgs.ActivityState.Activity.Name, trackingEventArgs.ActivityState.State,
                            ((variables.Count > 0) ? vars.ToString() : String.Empty)));

                        if (trackingEventArgs.ActivityState.State.Equals("Closed", StringComparison.InvariantCultureIgnoreCase))
                            Console.WriteLine("===============================================================");


                        //Add a sleep so that the debug adornments are visible to the user
                        System.Threading.Thread.Sleep(300);
                    }
                };
            }
        }
    }
}
