/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Infosys.WEM.Client;
using SE = Infosys.WEM.Scripts.Service.Contracts;
using Infosys.WEM.Infrastructure.Common;
using System.Xml.Linq;
using System.Configuration;
using System.Net;
using Infosys.WEM.Service.Common.Contracts.Message;
using System.Runtime.Serialization.Json;
using Infosys.WEM.SecureHandler;
using Infosys.WEM.Scripts.Service.Contracts.Data;
using Infosys.IAP.CommonClientLibrary;
using System.Management;
using System.ServiceModel;
using System.Security.Principal;
using TD=Infosys.WEM.AutomationTracker.Contracts.Data;
using TM = Infosys.WEM.AutomationTracker.Contracts.Message;
using System.Threading.Tasks;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public enum ExecutionMode
    {
        Online,
        Offline,
        Delegate
    }

    public class ScriptExecutionManager
    {
        private static string executionMode = string.Empty;
        //events for client developers
        #region Event- RetrievedScriptMetadata
        public class RetrievedScriptMetadataArgs : EventArgs
        {
            public Script ScriptMetadata { get; set; }
        }
        public delegate void RetrievedScriptMetadataEventHandler(RetrievedScriptMetadataArgs e);
        public static event RetrievedScriptMetadataEventHandler RetrievedScriptMetadata;
        #endregion

        #region Event- SendExecutionStatus
        public class SendExecutionStatusArgs : EventArgs
        {
            public Guid Identifier { get; set; }
            public string ScriptID { get; set; }
            public string StatusMessage { get; set; }
            public bool IsSuccess { get; set; }
            public string ServerName { get; set; }
            public int PercentComplete { get; set; }
            public string data { get; set; }

        }
        public delegate void SendExecutionStatusEventHandler(SendExecutionStatusArgs e);
        public static event SendExecutionStatusEventHandler SendExecutionStatus;
        public static string FileContent { get; set; }
        public static Stream FileContentStream { get; set; }
        #endregion

        #region Event- ProcessingScript
        public delegate void ProcessingScriptEventHandler();
        public static event ProcessingScriptEventHandler ProcessingScript;
        #endregion

        #region Event- ScriptExecutionStarting
        public delegate void ScriptExecutionStartingEventHandler();
        public static event ScriptExecutionStartingEventHandler ScriptExecutionStarting;
        #endregion

        #region Event- ScriptExecutionCompleted
        public class ScriptExecutionCompletedArgs : EventArgs
        {
            public string ScriptName { get; set; }
            public bool IsExecutionSuccessful { get; set; }
            public string ErrorMessage { get; set; }
            public string SuccessMessage { get; set; }
        }
        public delegate void ScriptExecutionCompletedEventHandler(ScriptExecutionCompletedArgs e);
        public static event ScriptExecutionCompletedEventHandler ScriptExecutionCompleted;
        #endregion

        #region Event- ScriptProcessed
        public class ScriptProcessedArgs : EventArgs
        {
            public string ScriptName { get; set; }
            public string OutPut { get; set; }
            public string OtherInformation
            {
                get
                {
                    return "For more information on the status of the script/command execution, subscribe to the event- ScriptExecutionCompleted";
                }
            }
            //add properties for output parameters
        }
        public delegate void ScriptProcessedEventHandler(ScriptProcessedArgs e);
        public static event ScriptProcessedEventHandler ScriptProcessed;
        #endregion

        #region Event- Script execution completed, to be used in asynchronous mode
        static Guid _scriptInstanceId = Guid.Empty;
        public class AsyncScriptExecutionCompletedArgs : EventArgs
        {
            public List<ExecutionResult> ExecutionOutputs { get; set; }
            public string ScriptId { get; set; }
            public Guid ScriptInstanceId { get; set; }
        }
        public delegate void AsyncScriptExecutionCompletedEventHandler(AsyncScriptExecutionCompletedArgs e);
        public event AsyncScriptExecutionCompletedEventHandler AsyncScriptExecutionCompleted;

        //the below delegate to convert the synchronous capability to asynchronous
        private delegate List<ExecutionResult> AsyncExecuteCaller(ScriptIndentifier script);

        AsyncExecuteCaller caller = null;
        #endregion

        /// <summary>
        /// For published script: To be called to start the execution of the script in the asynchronous mode.
        /// once the execiton is complete, an event- AsyncScriptExecutionCompleted is raised having the result of execution.
        /// </summary>
        /// <param name="script">The identifying details of the script to be executed</param>
        //public void ExecuteAsync(ScriptIndentifier script)
        //{
        //    caller = new AsyncExecuteCaller(Execute);
        //    IAsyncResult result = caller.BeginInvoke(script, CallBackForScriptCompleted, script.ScriptId);
        //}

        public List<ExecutionResult> ExecuteAsync(ScriptIndentifier script)
        {

            #region new implementation

            caller = new AsyncExecuteCaller(Execute);
            IAsyncResult result = caller.BeginInvoke(script, null, null);
            result.AsyncWaitHandle.WaitOne();
            var res = caller.EndInvoke(result);
            result.AsyncWaitHandle.Close();
            return res;

            #endregion

            //return Task.Run(() =>
            //{
            //    return Execute(script);
            //}).Result;
        }

        private void CallBackForScriptCompleted(IAsyncResult result)
        {
            List<ExecutionResult> executionResult = caller.EndInvoke(result);
            if (AsyncScriptExecutionCompleted != null)
            {
                string scriptId = result.AsyncState.ToString();
                AsyncScriptExecutionCompleted(new AsyncScriptExecutionCompletedArgs() { ScriptId = scriptId, ExecutionOutputs = executionResult, ScriptInstanceId = _scriptInstanceId });
            }
        }

        /// <summary>
        /// The interface to start the execution of the intended script or command.
        /// </summary>
        /// <param name="scripttobeExecuted">The details of the script file or cmmand to be executed</param>
        /// <param name="asAdmin">Flag to dictate if the process script/command to be executed in admin mode, by default it is false</param>
        /// <returns>The result of execution of the script or command</returns>
        public static List<ExecutionResult> Executes(Script scripttobeExecuted, ScriptIndentifier scriptIdentifier, string filePath, bool asAdmin = false)
        {       
                DateTime processStartedTime = DateTime.Now;
                //ShowOutputView(null,null);
                List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
                ExecutionResult output = new ExecutionResult();
                string executionTime = "";
                System.Collections.Generic.Dictionary<string, string> parameters = null;
                string scriptType = "";
                string scriptName = "";
            try
            {
                using (LogHandler.TraceOperations("ScriptExecutionManager:Executes", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                {

                    ExecuteBase client = null;


                    if (scripttobeExecuted.ScriptName.Contains('.'))
                    {
                        if (scripttobeExecuted.TaskType.ToLower().Contains("file"))
                            scriptType = Path.GetExtension(scripttobeExecuted.ScriptName).Remove(0, 1);
                        else
                            scriptType = scripttobeExecuted.ScriptName.Substring(0, scripttobeExecuted.ScriptName.LastIndexOf('.'));
                        scriptName = Path.GetFileNameWithoutExtension(scripttobeExecuted.ScriptName);
                    }
                    else
                        scriptName = scripttobeExecuted.ScriptName;

                    // Get script type to be executed
                    if (scripttobeExecuted.TaskType.ToLower().Contains("file"))
                    {
                        scriptType = GetScriptType(scriptType);
                    }
                    else if (scripttobeExecuted.TaskType.ToLower().Contains("sh command"))
                    {
                        scriptType = GetScriptType(scripttobeExecuted.TaskType);
                    }
                    else
                        scriptType = "";
                    if (!String.IsNullOrEmpty(scriptType))
                    {

                        scriptType = "Infosys.WEM.ScriptExecutionLibrary" + "." + scriptType;
                        client = (ExecuteBase)Activator.CreateInstance(Type.GetType(scriptType));

                    }
                    else
                        client = new ExecuteDefault();

                    if (client != null)
                    {
                        // Block33
                        //DateTime processStartedTime1 = DateTime.Now;
                        client.FileContent = FileContent;
                        ClientInitializationResult clientini = client.InitializeClient(scripttobeExecuted, scriptIdentifier, filePath, asAdmin);
                        //LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 33(cmplete IC (Calling): {0}", DateTime.Now.Subtract(processStartedTime1).TotalSeconds), LogHandler.Layer.Business, null);
                        if (!clientini.IsSuccess)
                        {
                            throw new Exception(clientini.ErrorMessage);
                        }
                        DateTime startTime = System.DateTime.Now;
                        //if (Path.GetExtension(scripttobeExecuted.ScriptName).ToLower().Equals(".iapd"))
                        //    client.FileContentStream = FileContentStream;
                        //else
                        // Block34
                        //DateTime processStartedTime2 = DateTime.Now;
                        consolidatedOutput = client.Start();
                        //LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 34(client start) : {0}", DateTime.Now.Subtract(processStartedTime2).TotalSeconds), LogHandler.Layer.Business, null);
                        DateTime endTime = System.DateTime.Now;
                        TimeSpan span = endTime - startTime;
                        executionTime = ((int)span.TotalMilliseconds).ToString() + " " + "msec";
                    }
                }
                //LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block_1 : {0}", DateTime.Now.Subtract(processStartedTime_1).TotalSeconds), LogHandler.Layer.Business, null);

            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                if (!string.IsNullOrEmpty(scriptIdentifier.RemoteServerNames) && scriptIdentifier.RemoteServerNames.Length != 0)
                {
                    string[] servernames = scriptIdentifier.RemoteServerNames.Split(',');
                    for (int i = 0; i < servernames.Length; i++)
                    {
                        ExecutionResult output1 = new ExecutionResult();
                        output1.ComputerName = servernames[i];

                        output1.ErrorMessage = err;

                        consolidatedOutput.Add(output1);
                    }
                }
                else
                {
                    output.ErrorMessage = err;
                    consolidatedOutput.Add(output);
                }

            }
                // Block35
                DateTime processStartedTime3 = DateTime.Now;
                if (!String.IsNullOrEmpty(scriptType.ToLower()))
                {
                    if (scripttobeExecuted.Parameters != null && scripttobeExecuted.Parameters.Count > 0)
                    {
                        parameters = new Dictionary<string, string>();
                        scripttobeExecuted.Parameters.ForEach(p =>
                        {
                            if (p.IsSecret)
                            {
                            //parameters.Add(p.ParameterName, "********");

                            foreach (ExecutionResult result in consolidatedOutput)
                                {

                                    if (!string.IsNullOrEmpty(result.SuccessMessage))
                                        result.SuccessMessage = result.SuccessMessage.Replace(result.InputCommand, "*******");
                                    if (!string.IsNullOrEmpty(result.InputCommand))
                                        result.InputCommand = result.InputCommand.Replace(p.ParameterValue, "*******");
                                //if (!string.IsNullOrEmpty(result.ErrorMessage))
                                //  result.ErrorMessage = result.ErrorMessage.Replace(p.ParameterValue, "*******");
                            }
                            }

                            else if (!string.IsNullOrEmpty(p.ParameterName))
                                parameters.Add(p.ParameterName, p.ParameterValue);
                        });
                    }
                }

                foreach (ExecutionResult result in consolidatedOutput)
                {
                    if (result.IsSuccess)
                    {
                        LogHandler.ScriptTracking(System.Security.Principal.WindowsIdentity.GetCurrent().Name, executionTime, scriptName, parameters, result.SuccessMessage);
                    }
                    else
                        LogHandler.ScriptTracking(System.Security.Principal.WindowsIdentity.GetCurrent().Name, executionTime, scriptName, parameters, result.ErrorMessage);
                }
            LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 35 : {0}", DateTime.Now.Subtract(processStartedTime3).TotalSeconds), LogHandler.Layer.Business, null);
            LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager: Execute2(Overall) : {0}", DateTime.Now.Subtract(processStartedTime).TotalSeconds), LogHandler.Layer.Business, null);
            return consolidatedOutput;
            }

        /// <summary>
        /// Function to check a given parameter against a list of allowed values.
        /// </summary>
        /// <param name="param">The parameter to be checked for validity against allowed values</param>
        /// <param name="allowedValues">List of allowed values which is valid. Comma separated list of allowed values for multiple values.</param> 
        /// <returns>Boolean - True if the parameter is allowed, False if parameter is not allowed</returns>     

        public static bool CheckAllowedParamValues(string param, string allowedValues)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:CheckAllowedParamValues", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                if (String.IsNullOrEmpty(allowedValues)) return true;
                string[] allowedParamValues = allowedValues.Split(',');
                return (allowedParamValues.Contains(param));
            }
        }

        /// <summary>
        /// The interface to start the execution of the intended script or command. If there is a script file, e.g. .bat, then
        /// make sure that in the http store the right mime type is added to download the script file, otherwise 404 error will be returned from the http store.
        /// </summary>
        /// <param name="script">The identifying details of the script file or cmmand to be executed</param>        
        /// <returns>The result of execution of the script or command</returns>
        public static List<ExecutionResult> Execute(ScriptIndentifier script)//, bool asAdmin = false)
        {
            //Block21
            DateTime processStartedTime20 = DateTime.Now;
            using (LogHandler.TraceOperations("ScriptExecutionManager:Execute", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                //Block1
                DateTime processStartedTime = DateTime.Now;
                string test = "";
                //assign the script instance id- this is needed for the workbench or client to co-relate the different events raised
                _scriptInstanceId = script.TransactionId;

                // If user has specified any execution mode in their request, consider that
                //if(!string.IsNullOrEmpty(script.ExecutionMode.ToString()))
                //{
                //    executionMode = script.ExecutionMode.ToString();
                //}
                //else 
                //{

                string scriptConfiguredExecutionMode = string.Empty;
                // check if the reference has been set to execution by a downstream platform eg:nia, iap

               
                if (!string.IsNullOrEmpty(script.ReferenceKey))
                {
                    // Read the configured execution mode against the name of the downstream platform eg:nia, iap provided in reference key
                    scriptConfiguredExecutionMode = GetConfiguredExecutionMode(script.ReferenceKey);

                    if (!string.IsNullOrEmpty(scriptConfiguredExecutionMode))
                    {
                        executionMode = scriptConfiguredExecutionMode;
                        if (executionMode.ToLower() == "delegate")
                        {
                            script.ExecutionMode = ExecutionModeType.Delegate;
                        }
                    }
                    // If not specified even in scripttype.xml, consider the execution mode specified in web.config
                    else
                    {
                        executionMode = ConfigurationManager.AppSettings["Mode"];
                    }
                }
                else
                {
                    executionMode = ConfigurationManager.AppSettings["Mode"];
                }
                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 1 : Get ExecutionMode : {0}", DateTime.Now.Subtract(processStartedTime).TotalSeconds), LogHandler.Layer.Business, null);


                //prepare for the transaction start entry
                TransactionRepository transacRepoClient = new TransactionRepository();
                //Block2 
                DateTime processStartedTime1 = DateTime.Now;
                var transacChannel = (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase)) ? transacRepoClient.ServiceChannel : null;
                Node.Service.Contracts.Message.LogTransactionResMsg transacLogRes = null;
                Infosys.ATR.RepositoryAccess.Entity.ScriptDoc scriptDoc = null;

                CommonHelp.WriteLog("script execution manager called by- " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
                ExecutionResult output = new ExecutionResult();

                string scriptName = "";
                string filePath = "";
                string executionTime = "";
                Guid Identifier = Guid.Empty;


                if (script.TransactionId == Guid.Empty)
                {
                    Identifier = Guid.NewGuid();
                    script.TransactionId = Identifier;
                }
                else
                {
                    Identifier = script.TransactionId;
                }

                System.Collections.Generic.Dictionary<string, string> parameters = null;
                SE.Data.Script scriptdata = null;
                Script scriptToBeExecuted = null;
                string taskType = string.Empty;
                Infosys.IAP.CommonClientLibrary.Models.ContentMeta mContent = null;
                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 2 : Prepare for the transaction start entry : {0}", DateTime.Now.Subtract(processStartedTime1).TotalSeconds), LogHandler.Layer.Business, null);


                #region OLD Commented Code
                //if (!string.IsNullOrEmpty(script.Path))
                //{
                //    Stream fs = null;
                //    string extractionLoc= string.Empty;
                //    Infosys.IAP.CommonClientLibrary.Models.ContentMeta contentMeta = new IAP.CommonClientLibrary.Models.ContentMeta();
                //    IAPPackage.Import(script.Path, out contentMeta, out fs, out extractionLoc);

                //    contentMeta.Parameters.ForEach(param =>
                //    {
                //        if (param.IsSecret && param.IOType.Equals(ParamDirection.In))
                //        {
                //            script.Parameters.ForEach(x =>
                //            {
                //                if (x.ParameterName.Equals(param.Name))
                //                    x.ParameterValue = SecurePayload.UnSecure(Convert.ToString(x.ParameterValue), "IAP2GO_SEC!URE");
                //            });
                //        }
                //    });                

                //    scriptToBeExecuted = Translator.Script_SE.ScriptFromOE(contentMeta);
                //    if (contentMeta.TaskType.ToLower().Contains("file"))
                //    {
                //        if (contentMeta.Parameters.Count > 0)
                //        {
                //            scriptToBeExecuted.Parameters = MapToNewParameter(script.Parameters, scriptToBeExecuted.Parameters);
                //            script.Parameters = scriptToBeExecuted.Parameters;
                //        }
                //    }
                //    FileContent = StreamToString(fs);

                //    if (String.IsNullOrEmpty(contentMeta.WorkingDir))
                //    {
                //        scriptToBeExecuted.ExecutionDir = GetAppPath();
                //    }


                //    consolidatedOutput = Execute(scriptToBeExecuted, script, filePath, scriptToBeExecuted.RunAsAdmin);
                //    return consolidatedOutput;
                //}
                #endregion
                //Block3

                DateTime processStartedTime2 = DateTime.Now;
                if (string.IsNullOrEmpty(script.Path))
                    if (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase))
                        if (!UserHasAccess(script.SubCategoryId))
                            throw new Exception("User Does Not have Access to Run this Script");
                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 3 : Check User access : {0}", DateTime.Now.Subtract(processStartedTime2).TotalSeconds), LogHandler.Layer.Business, null);

                //Block3_1
                //DateTime processStartedTime3_1 = DateTime.Now;
                try
                {

                if (SendExecutionStatus != null && string.IsNullOrEmpty(script.RemoteServerNames))
                { sendExecutionStatus(true, "", DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Script Execution Initiated... " + Environment.NewLine, script.RemoteServerNames, script.TransactionId, 5); }


                    //by default the script id is assigned, but later as the script details are fetched from service, name is assigned.
                    //this default assignment to id will be be helpful if name is not found for any reason.
                    scriptName = script.ScriptId.ToString();
                    SE.Message.GetScriptDetailsResMsg response = null;
                   
                    if (string.IsNullOrEmpty(script.Path) && (executionMode.Equals(ExecutionMode.Online.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    {
                       //LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 3_1 : {0}", DateTime.Now.Subtract(processStartedTime3_1).TotalSeconds), LogHandler.Layer.Business, null);

                        //Block4
                        DateTime processStartedTime3 = DateTime.Now;
                        string storageBaseUrl = GetStorageBaseUrl(script.WEMScriptServiceUrl);
                        CommonHelp.WriteLog("received storage base url- " + storageBaseUrl);
                        ScriptRepository scriptRepoClient = new ScriptRepository(script.WEMScriptServiceUrl);

                        //this approach followed to handle the scenario where wcf service is called from wcf service
                        //issue reference- http://blogs.msdn.com/b/pedram/archive/2008/07/19/webchannelfactory-inside-a-wcf-service.aspx
                        //Block14
                        //DateTime processStartedTime13 = DateTime.Now;

                        var channel = scriptRepoClient.ServiceChannel;
                        LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 4 (GetStorageBaseUrl) : {0}", DateTime.Now.Subtract(processStartedTime3).TotalSeconds), LogHandler.Layer.Business, null);

                        //LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 14 : {0}", DateTime.Now.Subtract(processStartedTime13).TotalSeconds), LogHandler.Layer.Business, null);
                        //SE.Message.GetScriptDetailsResMsg response = GetResponse<SE.Message.GetScriptDetailsResMsg>(scriptRepoClient.ServiceUrl + "/GetScriptDetails?scriptId=" + script.ScriptId.ToString() + "&categoryId= " + script.SubCategoryId.ToString());

                        //Block5 
                        DateTime processStartedTime4 = DateTime.Now;
                        using (LogHandler.TraceOperations("ScriptExecutionManager:GetScriptDetails", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                        {
                            using (new OperationContextScope((IContextChannel)channel))
                            {
                                response = channel.GetScriptDetails(Convert.ToString(script.ScriptId), Convert.ToString(script.SubCategoryId));
                            }
                        }
                        LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 5 (GetScriptDetails) : {0}", DateTime.Now.Subtract(processStartedTime4).TotalSeconds), LogHandler.Layer.Business, null);


                        if (response != null)
                        {
                            //Block6 
                            DateTime processStartedTime5 = DateTime.Now;

                            using (LogHandler.TraceOperations("ScriptExecutionManager:LogTransaction", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                            {                            
                                //log the transaction
                                if (response.ScriptDetails != null)

                                    if (response.ScriptDetails.ScriptFileVersion > 0)
                                    {

                                        transacLogRes = transacChannel.LogTransaction(GetTransactionDetails(script.SubCategoryId, response.ScriptDetails.ScriptType, script.ScriptId.ToString(), script.ReferenceKey, response.ScriptDetails.ScriptFileVersion.ToString(), Node.Service.Contracts.Data.StateType.InProgress));
                                    }
                                CommonHelp.WriteLog("received script details");
                            }
                            LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 6 (LogTransaction) : {0}", DateTime.Now.Subtract(processStartedTime5).TotalSeconds), LogHandler.Layer.Business, null);

                            
                            using (LogHandler.TraceOperations("ScriptExecutionManager:Unsecure Params", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                            {
                                //Block7
                                DateTime processStartedTime6 = DateTime.Now;
                                //Unique ScriptName
                                response.ScriptDetails.Name = response.ScriptDetails.Name + "_" + DateTime.Now.Ticks;
                                //then set the script url to the TLS to be used in case of iapw kind of workflow
                                System.Threading.Thread.FreeNamedDataSlot("iappackageurl");
                                LocalDataStoreSlot localData = System.Threading.Thread.AllocateNamedDataSlot("iappackageurl");
                                System.Threading.Thread.SetData(localData, storageBaseUrl + response.ScriptDetails.ScriptURL);

                                response.ScriptDetails.Parameters.ForEach(param =>
                                {
                                    if (param.IsSecret && param.ParamType.Equals(ParamDirection.In))
                                    {
                                        script.Parameters.ForEach(x =>
                                        {
                                            if (x.ParameterName.Equals(param.Name))
                                                x.ParameterValue = SecurePayload.UnSecure(Convert.ToString(x.ParameterValue), "IAP2GO_SEC!URE");
                                        });
                                    }
                                });
                                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 7 (Unsecure Params) : {0}", DateTime.Now.Subtract(processStartedTime6).TotalSeconds), LogHandler.Layer.Business, null);

                            }
                            //script.Parameters.ForEach(param =>
                            //{
                            //    if (param.IsSecret)
                            //        if (!string.IsNullOrEmpty(param.ParameterValue))
                            //            param.ParameterValue = SecurePayload.UnSecure(param.ParameterValue, "IAP2GO_SEC!URE");
                            //}); 


                            //Block8
                            DateTime processStartedTime7 = DateTime.Now;
                            //raise event- ProcessingScript
                            if (ProcessingScript != null)
                                ProcessingScript();


                            response.ScriptDetails.StorageBaseUrl = storageBaseUrl;
                            scriptdata = response.ScriptDetails;
                            //Script scriptToBeExecuted = Translator.Script_SE.ScriptFromSE(scriptdata);
                            scriptToBeExecuted = Translator.Script_SE.ScriptFromSE(scriptdata);

                            LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 8 (Translator.Script_SE.ScriptFromSE) : {0}", DateTime.Now.Subtract(processStartedTime7).TotalSeconds), LogHandler.Layer.Business, null);

                            //Block9
                            DateTime processStartedTime8 = DateTime.Now;
                            //raise event RetrievedScriptMetadata
                            if (RetrievedScriptMetadata != null)
                            {
                                RetrievedScriptMetadataArgs e = new RetrievedScriptMetadataArgs() { ScriptMetadata = HideSecretParameter(scriptToBeExecuted.Clone() as Script) };
                                RetrievedScriptMetadata(e);
                            }

                            LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 9 : (raise event RetrievedScriptMetadata ) : {0}", DateTime.Now.Subtract(processStartedTime8).TotalSeconds), LogHandler.Layer.Business, null);
                            //Block 9_1
                            //DateTime processStartedTime9_1 = DateTime.Now;

                            //if the task type is file, then download the file and copy it to the working folder.
                            if (!String.IsNullOrEmpty(script.WorkingDir))
                                scriptdata.WorkingDir = script.WorkingDir;
                            if (scriptdata.TaskType.ToLower().Contains("file"))
                            {
                                //LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 9_1 : {0}", DateTime.Now.Subtract(processStartedTime9_1).TotalSeconds), LogHandler.Layer.Business, null);

                                //Block10
                                DateTime processStartedTime9 = DateTime.Now;
                                using (LogHandler.TraceOperations("ScriptExecutionManager:Create Directory to download script", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                                {
                                    if (!String.IsNullOrEmpty(scriptdata.WorkingDir))
                                    {
                                        if (!scriptdata.ScriptType.Equals("sh"))
                                        {
                                            if (!Directory.Exists(scriptdata.WorkingDir))
                                                Directory.CreateDirectory(scriptdata.WorkingDir);
                                            scriptToBeExecuted.ExecutionDir = scriptdata.WorkingDir;
                                        }
                                    }
                                    else
                                    {
                                        scriptToBeExecuted.ExecutionDir = scriptdata.WorkingDir = GetAppPath();
                                        string newPath = @scriptToBeExecuted.ExecutionDir + "\\" + System.Configuration.ConfigurationManager.AppSettings["DefaultDirectory"];
                                        if (!Directory.Exists(newPath))
                                            Directory.CreateDirectory(newPath);
                                        scriptToBeExecuted.ExecutionDir = scriptdata.WorkingDir = newPath;
                                    }
                                }



                                //check for value invalidcharacters
                                var safeFileName = scriptdata.Name;
                                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 10 (Create Directory to download script) : {0}", DateTime.Now.Subtract(processStartedTime9).TotalSeconds), LogHandler.Layer.Business, null);

                                using (LogHandler.TraceOperations("ScriptExecutionManager:Download script", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                                {

                                    //Block11
                                    DateTime processStartedTime10 = DateTime.Now;
                                    if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(safeFileName))
                                    {
                                        output.ErrorMessage = "\nScript Name value of " + safeFileName + " contains illegitimate characters which are not to be used in the name.Please provide the script name without Special Characters" + Environment.NewLine;
                                        throw new Exception(output.ErrorMessage);
                                    }

                                    scriptName = scriptdata.Name + "." + scriptdata.ScriptType;
                                    //raise event DownloadingScriptFile
                                    if (SendExecutionStatus != null && string.IsNullOrEmpty(script.RemoteServerNames))
                                    {
                                        sendExecutionStatus(true, Convert.ToString(scriptdata.ScriptId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Download of the ScriptFile Started... " + scriptName, script.RemoteServerNames, Identifier, 20);
                                    }
                                    scriptDoc = Translator.Script_DocumentEntity.ScriptToDocument(scriptdata, script);
                                    Infosys.ATR.RepositoryAccess.FileRepository.ScriptRepositoryDS scriptDocDs = new Infosys.ATR.RepositoryAccess.FileRepository.ScriptRepositoryDS();
                                    CommonHelp.WriteLog("downloading script file");
                                    scriptDoc = scriptDocDs.Download(scriptDoc);
                                    LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 11 (Download script) : {0}", DateTime.Now.Subtract(processStartedTime10).TotalSeconds), LogHandler.Layer.Business, null);

                                    //Block12
                                    DateTime processStartedTime11 = DateTime.Now;
                                    //raise event DownloadedScriptFile
                                    if (SendExecutionStatus != null && string.IsNullOrEmpty(script.RemoteServerNames))
                                    {
                                        SendExecutionStatusArgs e = new SendExecutionStatusArgs();

                                        e.IsSuccess = scriptDoc.IsDownloadSuccessful;
                                        if (!scriptDoc.IsDownloadSuccessful)
                                        {
                                            e.StatusMessage = scriptDoc.AnyError;
                                            CommonHelp.WriteLog("script could not be downloaded- " + scriptDoc.AnyError);
                                        }
                                        else
                                        {
                                            e.StatusMessage = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Script Downloaded...";
                                            e.StatusMessage += "\nScript Details: " + scriptdata.Name + "." + scriptdata.ScriptType;

                                        }
                                        sendExecutionStatus(e.IsSuccess, Convert.ToString(scriptdata.ScriptId), e.StatusMessage, script.RemoteServerNames, Identifier, 55);
                                    }
                                    LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 12 : {0}", DateTime.Now.Subtract(processStartedTime11).TotalSeconds), LogHandler.Layer.Business, null);
                                }
                                //Block13
                                DateTime processStartedTime12 = DateTime.Now;
                                if (scriptDoc.File != null)
                                {

                                    using (LogHandler.TraceOperations("ScriptExecutionManager:Write the file to the working dir", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                                    {

                                        //write the file to the working dir
                                        filePath = scriptdata.WorkingDir + "\\" + scriptdata.Name + "." + scriptdata.ScriptType;

                                        CommonHelp.WriteLog("File path " + filePath);
                                        //write to local if not ps1, py
                                        switch (scriptdata.ScriptType.ToLower())
                                        {
                                            case "ps1":
                                            case "sh":
                                            case "py":
                                                //assign the file content
                                                scriptDoc.File.Seek(0, SeekOrigin.Begin);
                                                FileContent = StreamToString(scriptDoc.File);
                                                CommonHelp.WriteLog("script content received- " + FileContent);
                                                break;

                                            //case "iapd":
                                            //    FileContentStream=scriptDoc.File;
                                            //    break;
                                            default:
                                                using (FileStream destination = File.Create(filePath))
                                                {
                                                    scriptDoc.File.Seek(0, SeekOrigin.Begin);
                                                    scriptDoc.File.CopyTo(destination);
                                                }
                                                break;
                                        }

                                        if (scriptdata.ScriptType.ToLower().Equals("ps1"))
                                            scriptToBeExecuted.ScriptName = filePath;
                                    }
                                }
                                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 13 (write the file to the working dir) : {0}", DateTime.Now.Subtract(processStartedTime12).TotalSeconds), LogHandler.Layer.Business, null);
                            }
                            else
                                scriptName = scriptdata.Name;

                        }
                    }
                    else if (executionMode.Equals(ExecutionMode.Delegate.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        // TODO
                        scriptToBeExecuted = new Script();
                        script.ExecutionMode = ExecutionModeType.Delegate;
                    }
                    else
                    {
                        Stream fs = null;
                        string extractionLoc = string.Empty;
                        if (Path.GetExtension(script.Path).Equals(".iapl", StringComparison.InvariantCultureIgnoreCase))
                        {
                            mContent = new IAP.CommonClientLibrary.Models.ContentMeta();
                            IAPPackage.Import(script.Path, out mContent, out fs, out extractionLoc);

                            mContent.Parameters.ForEach(param =>
                            {
                                if (param.IsSecret && param.IOType.Equals(ParamDirection.In))
                                {
                                    script.Parameters.ForEach(x =>
                                    {
                                        if (x.ParameterName.Equals(param.Name))
                                            x.ParameterValue = SecurePayload.UnSecure(Convert.ToString(x.ParameterValue), "IAP2GO_SEC!URE");
                                    });
                                }
                            });

                            scriptToBeExecuted = Translator.Script_SE.ScriptFromOE(mContent);
                            taskType = mContent.TaskType;
                            scriptName = mContent.Name;
                            scriptToBeExecuted.ExecutionDir = (String.IsNullOrEmpty(mContent.WorkingDir)) ? GetAppPath() : mContent.WorkingDir;
                            filePath = Path.Combine(scriptToBeExecuted.ExecutionDir, mContent.Name + "." + mContent.ContentType);

                            //write to local if not ps1, py
                            switch (mContent.ContentType.ToLower())
                            {
                                case "ps1":
                                case "sh":
                                case "py":
                                    //assign the file content
                                    fs.Seek(0, SeekOrigin.Begin);
                                    FileContent = StreamToString(fs);
                                    CommonHelp.WriteLog("script content received- " + FileContent);
                                    break;
                                default:
                                    using (FileStream destination = File.Create(filePath))
                                    {
                                        fs.Seek(0, SeekOrigin.Begin);
                                        fs.CopyTo(destination);
                                    }
                                    break;
                            }
                            if (mContent.ContentType.ToLower().Equals("ps1"))
                                scriptToBeExecuted.ScriptName = filePath;
                        }
                        else
                        {
                            FileContent = File.ReadAllText(script.Path);

                        }
                    }

                    //then try to execute the script/command                    
                    //if parameter is provided then use it while executing the script/command
                    using (LogHandler.TraceOperations("ScriptExecutionManager:Use Parameter while executing Script", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                    {
                        //Block15
                        DateTime processStartedTime14 = DateTime.Now;
                        if (scriptdata != null)
                            taskType = scriptdata.TaskType;

                        if (taskType.ToLower().Contains("file"))
                        {

                            scriptToBeExecuted.Parameters = MapToNewParameter(script.Parameters, scriptToBeExecuted.Parameters);
                            script.Parameters = scriptToBeExecuted.Parameters;
                            string allowedValue = string.Empty;
                            if (script.Parameters != null && script.Parameters.Count > 0)
                                script.Parameters.ForEach(p =>
                                {
                                    bool parameterValid = CheckAllowedParamValues(p.ParameterValue, p.allowedValues);

                                    if (!parameterValid)
                                    {
                                        if (mContent != null)
                                            mContent.Parameters.ForEach(param =>
                                            {
                                                if (p.ParameterName.Equals(param.Name) && param.IsMandatory)
                                                    allowedValue += "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues + Environment.NewLine;
                                                else if (p.ParameterName.Equals(param.Name) && !param.IsMandatory && !string.IsNullOrEmpty(param.AllowedValues) && !string.IsNullOrEmpty(p.ParameterValue))
                                                    allowedValue += "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues + Environment.NewLine;
                                            });
                                        else
                                            response.ScriptDetails.Parameters.ForEach(param =>
                                            {
                                                if (p.ParameterName.Equals(param.Name) && param.IsMandatory)
                                                    allowedValue += "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues + Environment.NewLine;
                                                else if (p.ParameterName.Equals(param.Name) && !param.IsMandatory && !string.IsNullOrEmpty(param.AllowedValues) && !string.IsNullOrEmpty(p.ParameterValue))
                                                    allowedValue += "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues + Environment.NewLine;
                                            });

                                        //TODO: Send Fail status to display
                                        //sendExecutionStatus(true, Convert.ToString(scriptdata.ScriptId), allowedValue, "");
                                        // output.ErrorMessage += "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues;
                                        //consolidatedOutput.Add(output);

                                    }
                                });

                            if (!string.IsNullOrEmpty(allowedValue))
                            {
                                output.ErrorMessage = allowedValue;
                                throw new Exception(output.ErrorMessage);
                            }

                            //check for parameter name value for invalidcharacters
                            string parameterNameValue = string.Empty;
                            if (script.Parameters != null && script.Parameters.Count > 0)
                                script.Parameters.ForEach(p =>
                                {
                                    bool parameterInvalidValid = WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(p.ParameterName);
                                    if (parameterInvalidValid)
                                        parameterNameValue += "\nParameter value of " + p.ParameterName + " contains illegitimate characters which are not to be used in the name" + Environment.NewLine;
                                });

                            if (!string.IsNullOrEmpty(parameterNameValue))
                            {
                                output.ErrorMessage = parameterNameValue;
                                throw new Exception(output.ErrorMessage);
                            }

                            //scriptToBeExecuted.Parameters = MapToNewParameter(script.Parameters, scriptToBeExecuted.Parameters);

                        }
                        else
                        {
                            if (script.Parameters != null && script.Parameters.Count > 0)
                            {
                                scriptToBeExecuted.Parameters = new List<Parameter>();
                                foreach (Parameter p in script.Parameters)
                                {
                                    scriptToBeExecuted.Parameters.Add(new Parameter() { ParameterValue = p.ParameterValue });
                                }
                                scriptToBeExecuted.Parameters.ForEach(p =>
                                {
                                    bool parameterValid = CheckAllowedParamValues(p.ParameterValue, p.allowedValues);
                                    if (!parameterValid)
                                    {
                                        if (mContent != null)
                                            mContent.Parameters.ForEach(param =>
                                            {
                                                if (p.ParameterName.Equals(param.Name) && param.IsMandatory)
                                                    output.ErrorMessage = "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues;
                                                else if (p.ParameterName.Equals(param.Name) && !param.IsMandatory && param.AllowedValues != null && !string.IsNullOrEmpty(p.ParameterValue))
                                                    output.ErrorMessage = "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues;
                                            });
                                        else
                                            response.ScriptDetails.Parameters.ForEach(param =>
                                            {
                                                if (p.ParameterName.Equals(param.Name) && param.IsMandatory)
                                                    output.ErrorMessage = "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues;
                                                else if (p.ParameterName.Equals(param.Name) && !param.IsMandatory && param.AllowedValues != null && !string.IsNullOrEmpty(p.ParameterValue))
                                                    output.ErrorMessage = "\nParameter value of " + p.ParameterName + " does not match allowed values:" + p.allowedValues;
                                            });


                                    //consolidatedOutput.Add(output);
                                    throw new Exception(output.ErrorMessage);
                                    }
                                });

                            }
                        }
                    LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 15 (Use Parameter while executing Script) : {0}", DateTime.Now.Subtract(processStartedTime14).TotalSeconds), LogHandler.Layer.Business, null);

                    }
                    //If AllowRemote is true, add ServerName parameter
                    //if (script.AllowRemote)
                    //{
                    //    bool paramExists = false;
                    //    Parameter paramsServerName = new Parameter();
                    //    paramsServerName.ParameterName = script.Parameters[script.Parameters.Count - 1].ParameterName;
                    //    paramsServerName.ParameterValue = script.Parameters[script.Parameters.Count - 1].ParameterValue;
                    //    paramsServerName.IsPaired = true;

                    //    foreach (Parameter p in scriptToBeExecuted.Parameters)
                    //    {
                    //        if (p.ParameterName !=null && p.ParameterName.Equals("AllowRemoteServerName"))
                    //        {
                    //            paramExists = true;
                    //            break;
                    //        }
                    //    }

                    //    if (!paramExists)
                    //        scriptToBeExecuted.Parameters.Add(paramsServerName);
                    //}
                    //Block16
                    DateTime processStartedTime15 = DateTime.Now;
                    //rasie event ScriptExecutionStarting
                    using (LogHandler.TraceOperations("ScriptExecutionManager:Raise event ScriptExecutionStarting", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                    {
                        if (SendExecutionStatus != null && string.IsNullOrEmpty(script.RemoteServerNames))
                        {
                            sendExecutionStatus(true, Convert.ToString(scriptdata.ScriptId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Script Execution Started...", script.RemoteServerNames, Identifier, 80);
                        }
                        DateTime startTime = System.DateTime.Now;

                        switch (script.ExecutionMode)
                        {
                            case ExecutionModeType.RunOnIAPNode:
                            case ExecutionModeType.ScheduledOnIAPCluster:
                            case ExecutionModeType.ScheduledOnIAPNode:
                                Infosys.ATR.RemoteExecute.ExecutingEntity entity = new ATR.RemoteExecute.ExecutingEntity();
                                entity.CategoryId = script.SubCategoryId;
                                entity.ComapnyId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["Company"]);
                                entity.Domain = script.Domain;
                                entity.EntityType = ATR.RemoteExecute.ExecutingEntityType.Script;
                                entity.ExecutionMode = (Infosys.ATR.RemoteExecute.ExecutionModeType)((int)script.ExecutionMode);
                                if (entity.ExecutionMode == ATR.RemoteExecute.ExecutionModeType.RunOnIAPNode || entity.ExecutionMode == ATR.RemoteExecute.ExecutionModeType.ScheduledOnIAPNode)
                                    entity.RemoteServerNames = script.RemoteServerNames.Split(',').ToList();
                                else if (entity.ExecutionMode == ATR.RemoteExecute.ExecutionModeType.ScheduledOnIAPCluster)
                                    entity.ScheduleOnClusters = script.ScheduleOnClusters;
                                if (script.IapNodeHttpPort != 0)
                                {
                                    entity.IapNodeHttpPort = script.IapNodeHttpPort;
                                }
                                else
                                {
                                    entity.IapNodeHttpPort = 9001;
                                }
                                if (script.IapNodeNetTcpPort != 0)
                                {
                                    entity.IapNodeNetTcpPort = script.IapNodeNetTcpPort;
                                }
                                else
                                {
                                    entity.IapNodeNetTcpPort = 9002;
                                }
                                entity.IapNodeTransport = (Infosys.ATR.RemoteExecute.IapNodeTransportType)((int)script.IapNodeTransport);
                                if (scriptToBeExecuted.Parameters != null && scriptToBeExecuted.Parameters.Count > 0)
                                {
                                    if (entity.Parameters == null)
                                        entity.Parameters = new List<ATR.RemoteExecute.Parameter>();
                                    scriptToBeExecuted.Parameters.ForEach(p =>
                                    {
                                        entity.Parameters.Add(new ATR.RemoteExecute.Parameter()
                                        {
                                            ParameterName = p.ParameterName,
                                            ParameterValue = ((p.IsSecret) && p.ParameterValue.GetType().Equals(typeof(string))) ? SecurePayload.Secure(p.ParameterValue, "IAP2GO_SEC!URE") : p.ParameterValue,
                                            IsSecret = p.IsSecret,
                                            IsPaired = p.IsPaired,
                                            DataType = p.DataType,
                                            allowedValues = p.allowedValues
                                        });
                                    });
                                }
                                entity.ScheduledPattern = (ATR.RemoteExecute.ScheduledPatternType)((int)script.ScheduledPattern);
                                entity.ScheduleEndDateTime = script.ScheduleEndDateTime;
                                entity.ScheduleOccurences = script.ScheduleOccurences;
                                entity.SchedulePriority = script.SchedulePriority;
                                entity.ScheduleStartDateTime = script.ScheduleStartDateTime;
                                entity.ScheduleStopCriteria = (ATR.RemoteExecute.ScheduleStopCriteriaType)((int)script.ScheduleStopCriteria);
                                entity.ScriptId = script.ScriptId;
                                entity.UsesUIAutomation = script.UsesUIAutomation;

                                var remoteResult = Infosys.ATR.RemoteExecute.Hanlder.DelegateExecution(entity);
                                if (remoteResult != null && remoteResult.Count > 0)
                                {
                                    if (consolidatedOutput == null)
                                        consolidatedOutput = new List<ExecutionResult>();
                                    remoteResult.ForEach(r =>
                                    {
                                        ExecutionResult tempresult = new ExecutionResult();
                                        if (r.Output != null && r.Output.Count > 0)
                                        {
                                            if (tempresult.Output == null)
                                                tempresult.Output = new List<OutParameter>();
                                            r.Output.ForEach(o =>
                                            {
                                                tempresult.Output.Add(new OutParameter() { ParameterName = o.ParameterName, ParameterValue = o.ParameterValue });
                                            });
                                        }
                                        //tempresult.ComputerName =""; is needed for remote using powershell engine.
                                        tempresult.ErrorMessage = r.ErrorMessage;
                                        tempresult.InputCommand = r.InputCommand;
                                        tempresult.IsSuccess = r.IsSuccess;
                                        tempresult.SuccessMessage = r.SuccessMessage;
                                        tempresult.ScheduledRequestIds = r.ScheduledRequestIds;

                                        tempresult.ComputerName = r.MachineName;
                                        consolidatedOutput.Add(tempresult);
                                    });
                                }
                                break;

                            //Added the case handler for Delegate
                            case ExecutionModeType.Delegate:
                                //String[] data = filePath.Split('\\');
                                //String sciptNm = data[data.Length - 1];
                                if (!string.IsNullOrEmpty(script.ReferenceKey))
                                {
                                    if (!string.IsNullOrEmpty(script.ScriptName))
                                    {
                                        scriptToBeExecuted.ScriptName = script.ScriptName + "." + script.ReferenceKey;
                                    }
                                    else
                                        scriptToBeExecuted.ScriptName = "AutomationScript." + script.ReferenceKey;
                                }
                                scriptToBeExecuted.TaskType = "file";
                                InsertScriptExecuteRequest(script);
                                consolidatedOutput = Executes(scriptToBeExecuted, script, filePath, scriptToBeExecuted.RunAsAdmin);
                                //if(consolidatedOutput.Count>0 && consolidatedOutput[0].IsSuccess)
                                //{
                                TM.UpdateTransactionStatusResMsg responseMsg = UpdateResponse(consolidatedOutput);
                                //}

                                //if (!responseMsg.isSuccess)
                                //{

                                //}
                                break;

                            default:
                                //Block22
                                DateTime processStartedTime21 = DateTime.Now;
                                InsertScriptExecuteRequest(script);
                                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 22 (InsertScriptExecuteRequest) : {0}", DateTime.Now.Subtract(processStartedTime21).TotalSeconds), LogHandler.Layer.Business, null);

                                //Block23
                                DateTime processStartedTime22 = DateTime.Now;
                                consolidatedOutput = Executes(scriptToBeExecuted, script, filePath, scriptToBeExecuted.RunAsAdmin);
                                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 23 (Executes) : {0}", DateTime.Now.Subtract(processStartedTime22).TotalSeconds), LogHandler.Layer.Business, null);
                                //if (consolidatedOutput.Count > 0 && consolidatedOutput[0].IsSuccess)
                                //{
                                //Block24
                                DateTime processStartedTime23 = DateTime.Now;
                                UpdateResponse(consolidatedOutput);
                                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 24 (UpdateResponse) : {0}", DateTime.Now.Subtract(processStartedTime23).TotalSeconds), LogHandler.Layer.Business, null);
                                // }                        
                                CommonHelp.WriteLog("execution completed");
                                break;
                        }
                        LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 16 :3 method calls {0}", DateTime.Now.Subtract(processStartedTime15).TotalSeconds), LogHandler.Layer.Business, null);
                        //consolidatedOutput = Execute(scriptToBeExecuted, script, filePath, scriptToBeExecuted.RunAsAdmin);


                        //Block17
                        DateTime processStartedTime16 = DateTime.Now;
                        foreach (var result in consolidatedOutput)
                        {

                            var stream1 = new MemoryStream();
                            var ser = new DataContractJsonSerializer(typeof(ExecutionResult));
                            ser.WriteObject(stream1, result);
                            stream1.Position = 0;
                            var sr = new StreamReader(stream1);
                            string resultJson = sr.ReadToEnd();
                            sr = null;
                            stream1 = null;

                            result.TransactionId = script.TransactionId;
                            //log transaction for script execution complete
                            Node.Service.Contracts.Data.StateType curState;
                            string descriptionOutput = "";
                            if (string.IsNullOrEmpty(result.ErrorMessage))
                            {
                                curState = Node.Service.Contracts.Data.StateType.Completed;
                                descriptionOutput = result.SuccessMessage;
                            }
                            else
                            {
                                curState = Node.Service.Contracts.Data.StateType.Failed;
                                descriptionOutput = result.ErrorMessage;
                            }
                            test = "stream length" + Convert.ToString(12);
                            if (transacLogRes != null)
                            {
                                transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg()
                                {
                                    Request = new Node.Service.Contracts.Data.Transaction()
                                    {
                                        CategoryId = script.SubCategoryId,
                                        ModuleId = script.ScriptId.ToString(),
                                        InstanceId = transacLogRes.InstanceId,
                                        CurrentState = curState,
                                        Module = Node.Service.Contracts.Data.ModuleType.Script,
                                        CompanyId = script.CompanyId,
                                        Description = descriptionOutput,
                                        TransactionMetadata = resultJson,
                                        MachineName = result.ComputerName
                                    }
                                });

                            }

                        }
                        LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 17 (log transaction for script execution complete) : {0}", DateTime.Now.Subtract(processStartedTime16).TotalSeconds), LogHandler.Layer.Business, null);

                        //Block18
                        DateTime processStartedTime17 = DateTime.Now;
                        if (SendExecutionStatus != null && string.IsNullOrEmpty(script.RemoteServerNames))
                        {
                            sendExecutionStatus(true, Convert.ToString(scriptdata.ScriptId), DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.fffffff") + " Script Execution Completed...", script.RemoteServerNames, Identifier, 99);
                        }
                        DateTime endTime = System.DateTime.Now;
                        TimeSpan span = endTime - startTime;
                        executionTime = ((int)span.TotalMilliseconds).ToString() + " " + "msec";

                        string scriptNameWithoutExt = scriptName;
                        if (scriptNameWithoutExt.Contains('.'))
                            scriptNameWithoutExt = scriptNameWithoutExt.Substring(0, scriptNameWithoutExt.IndexOf('.'));

                        if (taskType.ToLower().Contains("file"))
                        {
                            if (scriptToBeExecuted.Parameters != null && scriptToBeExecuted.Parameters.Count > 0)
                            {
                                parameters = new Dictionary<string, string>();
                                scriptToBeExecuted.Parameters.ForEach(p =>
                                {
                                    parameters.Add(p.ParameterName, p.ParameterValue);
                                });
                            }
                        }
                        LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 18 (SendExecutionStatus) : {0}", DateTime.Now.Subtract(processStartedTime17).TotalSeconds), LogHandler.Layer.Business, null);
                        //if (output.IsSuccess)
                        //    LogHandler.ScriptTracking(System.Security.Principal.WindowsIdentity.GetCurrent().Name, executionTime, scriptNameWithoutExt, parameters, output.SuccessMessage);
                        //else
                        //    LogHandler.ScriptTracking(System.Security.Principal.WindowsIdentity.GetCurrent().Name, executionTime, scriptNameWithoutExt, parameters, output.ErrorMessage);

                        //Block19
                        DateTime processStartedTime18 = DateTime.Now;
                        if (consolidatedOutput.Count == 0 || consolidatedOutput == null)
                        {
                            if (!string.IsNullOrEmpty(script.RemoteServerNames))
                            {
                                List<string> remoteServers = script.RemoteServerNames.Split(new char[] { ',' }).ToList();

                                foreach (string s in remoteServers)
                                {
                                    ExecutionResult outputnew = new ExecutionResult();
                                    outputnew.ErrorMessage = "Unknown Status";
                                    outputnew.ComputerName = s;
                                    consolidatedOutput.Add(outputnew);

                                    var stream1 = new MemoryStream();
                                    var ser = new DataContractJsonSerializer(typeof(ExecutionResult));
                                    ser.WriteObject(stream1, outputnew);
                                    stream1.Position = 0;
                                    var sr = new StreamReader(stream1);
                                    string resultJson = sr.ReadToEnd();
                                    sr = null;
                                    stream1 = null;
                                    //log transaction for script execution complete
                                    if (transacLogRes != null)
                                    {
                                        transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg()
                                        {
                                            Request = new Node.Service.Contracts.Data.Transaction()
                                            {
                                                CategoryId = script.SubCategoryId,
                                                ModuleId = script.ScriptId.ToString(),
                                                InstanceId = transacLogRes.InstanceId,
                                                CurrentState = Node.Service.Contracts.Data.StateType.Failed,
                                                Module = Node.Service.Contracts.Data.ModuleType.Script,
                                                CompanyId = script.CompanyId,
                                                Description = outputnew.ErrorMessage,
                                                TransactionMetadata = resultJson,
                                                MachineName = outputnew.ComputerName
                                            }
                                        });

                                    }
                                }
                            }
                        }
                        CommonHelp.WriteLog("returning from script execution library");
                        LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 19 ((log transaction for script execution complete)): {0}", DateTime.Now.Subtract(processStartedTime18).TotalSeconds), LogHandler.Layer.Business, null);
                        LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Execute method : {0}", DateTime.Now.Subtract(processStartedTime).TotalMilliseconds), LogHandler.Layer.Business, null);

                        return consolidatedOutput;

                        //}
                    }
                }
                catch (Exception ex)
                {

                    if (consolidatedOutput == null)
                        consolidatedOutput = new List<ExecutionResult>();
                    output.IsSuccess = false;
                    string err = test + "Exception on Script Execution: " + ex.Message + " Stack Trace: " + ex.StackTrace;
                    if (ex.InnerException != null)
                        err = err + ". \nInner Exception- " + ex.InnerException.Message;
                    //log transaction for failure
                    if (transacLogRes != null)
                    {
                        transacChannel.LogTransaction(new Node.Service.Contracts.Message.LogTransactionReqMsg()
                        {
                            Request = new Node.Service.Contracts.Data.Transaction()
                            {
                                CategoryId = script.SubCategoryId,
                                ModuleId = script.ScriptId.ToString(),
                                InstanceId = transacLogRes.InstanceId,
                                CurrentState = Node.Service.Contracts.Data.StateType.Failed,
                                Description = err,
                                Module = Node.Service.Contracts.Data.ModuleType.Script
                            }
                        });
                    }
                    if (!string.IsNullOrEmpty(script.RemoteServerNames))
                    {
                        List<string> remoteServers = script.RemoteServerNames.Split(new char[] { ',' }).ToList();

                        foreach (string s in remoteServers)
                        {
                            ExecutionResult outputnew = new ExecutionResult();
                            outputnew.ErrorMessage = err;
                            if (string.IsNullOrEmpty(err))
                            {
                                outputnew.ErrorMessage = "Unknown Status";
                            }
                            outputnew.ComputerName = s;
                            consolidatedOutput.Add(outputnew);
                        }


                    }
                    else if (script.ScheduleOnClusters != null)
                    {
                        if (script.ScheduleOnClusters.Count > 0)
                        {
                            foreach (string s in script.ScheduleOnClusters)
                            {
                                ExecutionResult outputnew = new ExecutionResult();
                                outputnew.ErrorMessage = err;
                                outputnew.ComputerName = s;
                                consolidatedOutput.Add(outputnew);
                            }
                        }
                        //else
                        //{
                        //    output.ErrorMessage = err;
                        //    output.ComputerName = script.RemoteServerNames;
                        //    consolidatedOutput.Add(output);
                        //}
                    }
                    else
                    {
                        output.ComputerName = Environment.MachineName;
                        output.ErrorMessage = err;
                        consolidatedOutput.Add(output);
                    }
                }
                
                finally
                {
                    //Block20
                    //DateTime processStartedTime19 = DateTime.Now;
                    // Delete processed file
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                   // LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 20 : {0}", DateTime.Now.Subtract(processStartedTime20).TotalSeconds), LogHandler.Layer.Business, null);
                }
                CommonHelp.WriteLog("retruning from script execution library");

                LogHandler.LogError(string.Format("Time taken by ScriptExecutionManager:Block 21(Overall Execute) : {0}", DateTime.Now.Subtract(processStartedTime20).TotalSeconds), LogHandler.Layer.Business, null);
                return consolidatedOutput;
                
            }
            
        }
        private static bool InsertScriptExecuteRequest(ScriptIndentifier script)
        {
            string isBypasscertificate = Convert.ToString(ConfigurationManager.AppSettings["ByPassCertificate"]);
            bool isSuccess = false;
            try
            {
                //Block25
                //DateTime processStartedTime24 = DateTime.Now;
                using (LogHandler.TraceOperations("ScriptExecutionManager:InsertScriptExecuteRequest",LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
                {
                    if (!string.IsNullOrEmpty(isBypasscertificate) && isBypasscertificate.ToUpper() == "YES")
                    {
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                    }
                    Infosys.WEM.Client.AutomationTracker automationTrackerRepoClient = new Infosys.WEM.Client.AutomationTracker();
                    var channel = automationTrackerRepoClient.ServiceChannel;
                    TM.AddRequestReqMsg addReqMsg = new TM.AddRequestReqMsg();
                    List<TD.Parameter> inParams = new List<TD.Parameter>();
                    if (script != null && script.Parameters != null)
                    {
                        foreach (Parameter parameter in script.Parameters)
                        {
                            TD.Parameter param = new TD.Parameter();
                            param.ParameterName = parameter.ParameterName;
                            param.ParameterValue = parameter.ParameterValue;
                            inParams.Add(param);
                        }
                    }
                    //LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 25 InsertScriptExecuteRequest(Channel) : {0}", DateTime.Now.Subtract(processStartedTime24).TotalSeconds), LogHandler.Layer.Business, null);

                    //Block26
                    //DateTime processStartedTime25 = DateTime.Now;
                    TD.ScriptExecuteRequest executeRequest = new TD.ScriptExecuteRequest()
                    {
                        TransactionId = script.TransactionId,
                        CompanyId = script.CompanyId,
                        ScriptId = script.ScriptId,
                        CategoryId = script.SubCategoryId,
                        ScriptName = script.ScriptName,
                        Path = script.Path,
                        InParameters = inParams,
                        RemoteServerNames = script.RemoteServerNames,
                        UserName = script.UserName,
                        Password = Convert.ToString(script.Password),
                        ReferenceKey = script.ReferenceKey,
                        ExecutionMode = Enum.IsDefined(typeof(ExecutionModeType), script.ExecutionMode) ? Convert.ToInt32(script.ExecutionMode) : 0,
                        Domain = script.Domain,
                        IapNodeTransport = Convert.ToString(script.IapNodeTransport),
                        ResponseNotificationCallbackURL = script.ResponseNotificationCallbackURL
                    };
                    //LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 26 InsertScriptExecuteRequest(Assignment) : {0}", DateTime.Now.Subtract(processStartedTime25).TotalSeconds), LogHandler.Layer.Business, null);

                    addReqMsg.requestData = executeRequest;
                    //Block27
                    //DateTime processStartedTime26 = DateTime.Now;
                    TM.AddRequestResMsg addResMsg = channel.AddTransactionStatus(addReqMsg);
                    //LogHandler.LogDebug(string.Format("Time taken by ScriptExecutionManager:Block 27 InsertScriptExecuteRequest(AddTransactionstatusCall) : {0}", DateTime.Now.Subtract(processStartedTime26).TotalSeconds), LogHandler.Layer.Business, null);
                    isSuccess = true;
                }
                
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return isSuccess;
        }

        private static TM.UpdateTransactionStatusResMsg UpdateResponse(List<ExecutionResult> consolidatedOutput)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:UpdateResponse", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                TM.UpdateTransactionStatusResMsg updateResmsg = new TM.UpdateTransactionStatusResMsg();
                string isBypasscertificate = Convert.ToString(ConfigurationManager.AppSettings["ByPassCertificate"]);
                if (isBypasscertificate.ToUpper() == "YES")
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                }
                try
                {
                    foreach (ExecutionResult entity in consolidatedOutput)
                    {
                        Infosys.WEM.Client.AutomationTracker automationTrackerclient = new Infosys.WEM.Client.AutomationTracker();
                        var channel = automationTrackerclient.ServiceChannel;
                        TM.UpdateTransactionStatusReqMsg updateTransaction = new TM.UpdateTransactionStatusReqMsg();

                        List<Infosys.WEM.AutomationTracker.Contracts.Data.Parameter> outParams = new List<Infosys.WEM.AutomationTracker.Contracts.Data.Parameter>();
                        if (entity != null && entity.Output != null)
                        {
                            foreach (OutParameter parameter in entity.Output)
                            {
                                TD.Parameter param = new TD.Parameter();
                                param.ParameterName = parameter.ParameterName;
                                param.ParameterValue = Convert.ToString(parameter.ParameterValue);
                                outParams.Add(param);
                            }
                        }

                        TD.ScriptExecuteResponse executeResponse = new TD.ScriptExecuteResponse()
                        {
                            TransactionId = entity.TransactionId,
                            CurrentState = string.IsNullOrEmpty(entity.Status) ? "" : entity.Status,
                            SuccessMessage = entity.SuccessMessage,
                            ErrorMessage = entity.ErrorMessage,
                            IsSuccess = entity.IsSuccess,
                            OutParameters = outParams,
                            LogData = entity.LogData,
                            SourceTransactionId = entity.SourceTransactionId,
                            ComputerName = string.IsNullOrEmpty(entity.ComputerName) ? "" : entity.ComputerName,
                            InputCommand = entity.InputCommand
                        };
                        updateTransaction.scriptExecuteResponse = executeResponse;
                        updateResmsg = channel.UpdateTransactionStatus(updateTransaction);
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                return updateResmsg;
            }
        }


        private static string GetConfiguredExecutionMode(string referenceKey)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetConfiguredExecutionMode", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null)) 
                {
                string executionMode = string.Empty;
                XElement root = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"XML\ScriptType.xml");
                //var objType = from scripttype in root.Elements("Type")
                //              where scripttype.Attribute("key").Value.ToLower().Equals(referenceKey.ToLower())
                //              select scripttype.Value;

                //if (objType.FirstOrDefault() != null)
                //executionMode = objType.FirstOrDefault().ToString();

                var element = root.Elements("Type").Where(x => x.Attribute("key").Value.ToLower().Equals(referenceKey.ToLower(), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (element != null)
                {
                    executionMode = element.Attribute("executionMode").Value.ToLower();
                }

                return executionMode;
            }
        }

        private static void sendExecutionStatus(bool isSuccess, string scriptID, string statusMessage, string serverName, Guid Identifier, int Progress)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:sendExecutionStatus", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {

                if (string.IsNullOrEmpty(serverName)) serverName = Environment.MachineName;
                SendExecutionStatusArgs e = new SendExecutionStatusArgs();
                e.IsSuccess = isSuccess;
                e.ScriptID = scriptID;
                e.StatusMessage = statusMessage;
                e.ServerName = serverName;
                e.Identifier = Identifier;
                e.PercentComplete = Progress;
                SendExecutionStatus((SendExecutionStatusArgs)e);
            }
        }

        private static string GetAppPath()
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetAppPath", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                string path;
                path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

                if (path.Contains(@"file:\\"))
                {
                    path = path.Replace(@"file:\\", "");
                }

                else if (path.Contains(@"file:\"))
                {
                    path = path.Replace(@"file:\", "");
                }

                return path;
            }
        }

        private static Script HideSecretParameter(Script retrievedScript)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:HideSecretParameter", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                if (retrievedScript != null && retrievedScript.Parameters != null && retrievedScript.Parameters.Count > 0)
                {
                    foreach (Parameter parameter in retrievedScript.Parameters)
                    {
                        if (parameter.IsSecret)
                            parameter.ParameterValue = null;
                    }
                }
                return retrievedScript;
            }
        }

        private static List<Parameter> MapToNewParameter(List<Parameter> newParam, List<Parameter> existingParam)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:MapToNewParameter", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                {

                    if (existingParam == null || existingParam.Count == 0)
                        existingParam = newParam;
                    else if (string.IsNullOrEmpty(newParam[0].ParameterName))
                    {
                        // i.e. the parameters are unnamed then assumtion is, the order and count is as expected.
                        for (int i = 0; i < existingParam.Count; i++)
                        {
                            existingParam[i].ParameterValue = newParam[i].ParameterValue;
                        }
                    }
                    else
                    {
                        for (int e = 0; e < existingParam.Count; e++)
                        {
                            for (int n = 0; n < newParam.Count; n++)
                            {
                                if (newParam[n].ParameterName == existingParam[e].ParameterName)
                                {
                                    existingParam[e].ParameterValue = newParam[n].ParameterValue;
                                    // existingParam[e].DataType = newParam[n].DataType;
                                    break;
                                }
                            }
                        }
                    }
                }
                return existingParam;
            }
        }

        /// <summary>  
        /// This method is used to find the type of the script to be loaded for executing the script.
        /// </summary>
        /// <param name="scriptType">script type e.g. bat, .ps1 etc.</param>
        /// <returns>Procssing class name for the script</returns>
        private static string GetScriptType(string scriptType)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetScriptType", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                string type = "";
                XElement root = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"XML\ScriptType.xml");
                var objType = from scripttype in root.Elements("Type")
                              where scripttype.Attribute("key").Value.ToLower().Equals(scriptType.ToLower())
                              select scripttype.Value;
                if (objType.FirstOrDefault() != null)
                    type = objType.FirstOrDefault().ToString();

                return type;
            }
        }
        static string companyId = "";
        /// <summary>
        /// This method is used to get StorageBaseURL value from company table
        /// </summary>
        /// <returns>Value of StorageBaseURL column</returns>
        private static string GetStorageBaseUrl(string companyServiceUrl)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetStorageBaseUrl", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                string storageBaseUrl = "";
                companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
                if (string.IsNullOrEmpty(companyServiceUrl))
                {
                    storageBaseUrl = CommonServices.Instance.StorageBaseURL;

                    //Infosys.WEM.Client.CommonRepository commonserviceClient = new Infosys.WEM.Client.CommonRepository();
                    ////this approach followed in handle the scenario where wcf service is called from wcf service
                    ////issue reference- http://blogs.msdn.com/b/pedram/archive/2008/07/19/webchannelfactory-inside-a-wcf-service.aspx
                    //var channel = commonserviceClient.ServiceChannel;
                    //GetCompanyResMsg company = GetResponse<GetCompanyResMsg>(commonserviceClient.ServiceUrl + "/GetCompanyDetails/" + companyId);
                    //storageBaseUrl = company.Company.StorageBaseUrl;
                }
                else
                {
                    Uri uri = new Uri(companyServiceUrl);
                    string serverUrl = uri.GetLeftPart(UriPartial.Authority);
                    //Infosys.WEM.Client.CommonRepository commonserviceClient = new Infosys.WEM.Client.CommonRepository(serverUrl + "/WEMCommonService.svc");
                    Infosys.WEM.Client.CommonRepository commonserviceClient = new Infosys.WEM.Client.CommonRepository(serverUrl + WEM.Infrastructure.Common.ApplicationConstants.COMMON_SERVICEINTERFACE);

                    GetCompanyResMsg company = commonserviceClient.ServiceChannel.GetCompanyDetails(companyId);
                    storageBaseUrl = company.Company.StorageBaseUrl;
                }

                return storageBaseUrl;
            }
        }

        private static T GetResponse<T>(string url)
        {
            WebClient srvProxy = new WebClient();
            srvProxy.Credentials = CredentialCache.DefaultCredentials;
            byte[] data = srvProxy.DownloadData(url);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(T));
            return (T)obj.ReadObject(stream);
        }

        private static string StreamToString(Stream fileContent)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:StreamToString", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                StreamReader reader = new StreamReader(fileContent);
                string fileString = reader.ReadToEnd();
                return fileString;
            }
        }

        private static bool UserHasAccess(int categoryId)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:UserHasAccess", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                int companyid = Convert.ToInt16(ConfigurationManager.AppSettings["Company"]);
                if (companyid == 0)
                    throw new Exception("Invalid Configuration. Specify Company value in Application Configuration Settings");
                string[] roles = new string[3] { ApplicationConstants.ROLE_MANAGER, ApplicationConstants.ROLE_ANALYST, ApplicationConstants.ROLE_AGENT };
                return Security.CheckAccessInRole(companyid, categoryId, roles);
            }
        }

        private static Infosys.WEM.Node.Service.Contracts.Message.LogTransactionReqMsg GetTransactionDetails(int category, string filetype, string scriptId, string referenceKey, string scriptVersion, Node.Service.Contracts.Data.StateType state)
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetTransactionDetails", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                Infosys.WEM.Node.Service.Contracts.Message.LogTransactionReqMsg transac = new Node.Service.Contracts.Message.LogTransactionReqMsg();
                transac.Request = new Node.Service.Contracts.Data.Transaction();
                transac.Request.CategoryId = category;
                transac.Request.Executor = "SC_" + GetMachineName();
                transac.Request.FileType = filetype;
                transac.Request.IPAddress = GetMachineIPAddress();
                transac.Request.MachineName = GetMachineName();
                transac.Request.Module = Node.Service.Contracts.Data.ModuleType.Script;
                transac.Request.ModuleId = scriptId;
                transac.Request.OSDetails = GetOSName();
                transac.Request.ReferenceKey = referenceKey;
                transac.Request.ModuleVersion = scriptVersion;
                transac.Request.CurrentState = state;

                return transac;
            }
        }

        private static string GetMachineName()
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetMachineName", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                string machineName = Environment.MachineName;
                return machineName;
            }
        }

        private static string GetMachineIPAddress()
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetMachineIPAddress", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                string machineName = Environment.MachineName;
                string ipAdd = Dns.GetHostEntry(machineName).AddressList[0].ToString();
                return ipAdd;
            }
        }

        private static string GetOSName()
        {
            using (LogHandler.TraceOperations("ScriptExecutionManager:GetOSName", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
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
        }

       
    }
}
