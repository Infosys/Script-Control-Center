/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.WorkflowExecutionLibrary;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Infosys.WEM.Utilities.WorkflowExecuteUtil;
using System.Threading;
using System.Runtime.InteropServices;
using System.Configuration;
using Infosys.WEM.Node.Service.Contracts.Message;
using System.Runtime.Serialization.Json;
using Infosys.WEM.Infrastructure.Common;
using Infosys.IAP.CommonClientLibrary;
using Infosys.WEM.SecureHandler;
using System.Web.Script.Serialization;

namespace WorkflowExecuteUtil
{
    public class Program
    {
        static bool IsFileChanged = false;
        static string namedparamDelimiter = ":";
        static string paramDelimiter = ";";
        static bool exitsytem = false;
        static string mode = string.Empty;
        const int ONERROR_RETRY_FOR_GETNEXTREQUEST = 5;
        const int RETRY_TIMEGAP_FOR_GETNEXTREQUEST = 10000;
        static bool enableDetailedConsoleTrace = false;
        static bool HasAccess = false;

        //This delegate type used as handler routine to SetConsoleControlHandler.
        public delegate bool EventHandler(ControlTypes CtrlType);
        static EventHandler _handler;

        static string WorkflowId = string.Empty;
        static string CategoryId = string.Empty;
        static string WorkflowServiceURL = string.Empty;
        static string Version = string.Empty;
        static string ExternalAssemblyFolderPath = string.Empty;
        static string Parameters = string.Empty;
        static string WorkflowPath = string.Empty;
        static string IAPNodeServerNames = string.Empty;
        static string ExecutionMode = string.Empty;
        static string IapNodeTransportScheme = string.Empty;
        static string IapNodePortNumber = string.Empty;
        static string Domain = string.Empty;

        [STAThread]
        public static int Main(string[] args)
        {
            

            bool helpAsked = false;
            int status = (int)ExitCode.Success;
            try
            {
                //Event to handle exit on user clicking close button
                _handler = new EventHandler(Handler);
                SetConsoleCtrlHandler(_handler, true);

                //Parse all input argumemts
                if (!(args == null || args.Length == 1) && args.Length != 0)
                {
                    ParseArguments(args);
                }

                //Print Help
                if (args != null && args.Length == 1 && (args[0].Contains('?') || args[0].ToLower().Contains("help")))
                {
                    helpAsked = true;
                    PrintHelp();
                }

                //Enable detailed trace output to the console window
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableDetailedConsoleTrace"]))
                {
                    enableDetailedConsoleTrace = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDetailedConsoleTrace"]);
                }

                //Set execution mode
                if (!helpAsked)
                {
                    if (args == null || args.Length == 0)
                    {
                        mode = "Polling";
                    }
                    else
                    {
                        mode = "Normal";
                    }
                }

                switch (mode)
                {
                    case "Normal":
                        status = NormalModeExecution();
                        break;
                    case "Polling":
                        status = PollingModeExecution();
                        break;
                }
                return status;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nPlease check the usage/help for the correct ways to invoke the WorkflowExecuteUtil\n");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error occured...");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Console.ResetColor();
                Environment.ExitCode = (int)ExitCode.UnknownError;
                return (int)ExitCode.UnknownError;
            }
        }

        private static void ParseArguments(string[] args)
        {
            int i = 0;
            bool arePositionalArguments = true;

            foreach (string argument in args)
            {
                switch (argument.ToLower())
                {
                    case "-wid":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        WorkflowId = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-cid":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        CategoryId = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-surl":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        WorkflowServiceURL = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-ver":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        Version = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-eapath":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        ExternalAssemblyFolderPath = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-params":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        Parameters = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-wpath":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        WorkflowPath = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-em":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        ExecutionMode = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-servers":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        IAPNodeServerNames = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-sc":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        IapNodeTransportScheme = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-port":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        IapNodePortNumber = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-domain":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        Domain = args[i + 1];
                        arePositionalArguments = false;
                        break;
                }

                if (arePositionalArguments)
                    break;
                else
                    i += 1;
            }
            if (arePositionalArguments)
            {
                if (args.Count() >= 1)
                {
                    WorkflowId = args[0];
                }

                if (args.Count() >= 2)
                {
                    CategoryId = args[1];
                }

                if (args.Count() >= 3)
                {
                    WorkflowServiceURL = args[2];
                }

                if (args.Count() >= 4)
                {
                    Version = args[3];
                }

                if (args.Count() >= 5)
                {
                    ExternalAssemblyFolderPath = args[4];
                }

                if (args.Count() >= 6)
                {
                    Parameters = args[5];
                }

                if (args.Count() >= 7)
                {
                    WorkflowPath = args[6];
                }

                if (args.Count() >= 8)
                {
                    IAPNodeServerNames = args[7];
                }

                if (args.Count() >= 10)
                {
                    IapNodeTransportScheme = args[9];
                }

                if (args.Count() >= 11)
                {
                    IapNodePortNumber = args[10];
                }

                if (args.Count() >= 12)
                {
                    Domain = args[11];
                }
            }
        }

        private static int PollingModeExecution()
        {
            //read the retry app settings otherwise use default constants
            string retryCount = ConfigurationManager.AppSettings["OnErrorRetryCountForNextRequest"];
            int iretryCount = ONERROR_RETRY_FOR_GETNEXTREQUEST;
            int.TryParse(retryCount, out iretryCount);

            string retryTimegap = ConfigurationManager.AppSettings["RetryTimegapForNextRequest"];
            int iretryTimegap = RETRY_TIMEGAP_FOR_GETNEXTREQUEST;
            int.TryParse(retryTimegap, out iretryTimegap);

            try
            {
                Console.WriteLine("Workflow Console starting in Polling Mode...");
                string iAPnode = "UI_" + Environment.MachineName;

                //Start Polling
                AutomationWorkflow IAPnodeRegister = new AutomationWorkflow();
                IAPnodeRegister.RegisterNode();
                //Load dlls to App Domain
                DirectoryInfo dir;
                var path = ConfigurationManager.AppSettings["LoadAssembly"];
                if (!String.IsNullOrEmpty(path))
                {
                    if (Directory.Exists(path))
                    {
                        dir = new DirectoryInfo(path);
                        FileInfo[] fInfo = dir.GetFiles("*.dll");
                        foreach (FileInfo fis in fInfo)
                        {
                            var dllpath = fis.DirectoryName + "\\" + fis.Name;
                            Assembly assembly = Assembly.LoadFrom(dllpath);
                        }
                    }
                }
                FileSystemWatcher watcher = new FileSystemWatcher();
                if (!String.IsNullOrEmpty(path))
                {
                    watcher.Path = path;
                    /* Watch for changes in LastAccess and LastWrite times, and
                       the renaming of files or directories. */
                    watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                       | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    // Only watch text files.
                    watcher.Filter = "*.dll";

                    // Add event handlers.
                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    watcher.Created += new FileSystemEventHandler(OnChanged);
                    watcher.Deleted += new FileSystemEventHandler(OnChanged);
                    watcher.Renamed += new RenamedEventHandler(OnChanged);

                    // Begin watching.
                    watcher.EnableRaisingEvents = true;
                }
                int retry = iretryCount + 1;
                while (!exitsytem)
                {
                    retry--;
                    try
                    {
                        //Get workflow details from Service
                        Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest Req = new Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest();
                        GetNextRequestResMsg iwfRequest = new GetNextRequestResMsg();
                        //string serviceurl = ConfigurationManager.AppSettings["WEMService"];

                        Infosys.WEM.Client.ScheduledRequest Getclient = new Infosys.WEM.Client.ScheduledRequest();    //(serviceurl);
                        string domain = ConfigurationManager.AppSettings["Domain"];
                        iwfRequest = Getclient.ServiceChannel.GetNextRequest(domain, "UI_" + Environment.MachineName, "1");//you can call either workflow or script
                        //reset the 'on error retry count' for get next request for successfull getnexrequest call
                        retry = iretryCount + 1;
                        Req = iwfRequest.Request;
                        if (Req != null)
                        {
                            bool isProcessedWithAccess = false;
                            ConsoleTrace("New Workflow execution request received...");
                            WorkflowIndentifier wfIdentifier = new WorkflowIndentifier();
                            wfIdentifier.WorkflowId = Guid.Parse(Req.RequestId);
                            wfIdentifier.CategoryId = Req.CategoryId;
                            wfIdentifier.WEMWorkflowServiceUrl = "";
                            wfIdentifier.WorkflowVersion = Req.RequestVersion;
                            string userNameRunningInContextOf = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            UpdateRequestExecutionStatusReqMsg req = new UpdateRequestExecutionStatusReqMsg();
                            req.AssignedTo = Req.AssignedTo;

                            if (UserHasAccess(Req.Requestor, iwfRequest.Request.CategoryId))
                            {
                                string jSonParam = Req.InputParameters;

                                ConsoleTrace(string.Format("Scheduled request {0}, Workflow {1} to be executed...", Req.Id, Req.RequestId));
                                if (jSonParam != null)
                                {
                                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<Parameter>));
                                    MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(jSonParam));
                                    //List<Parameter> parameters = new List<Parameter>();
                                    wfIdentifier.Parameters = (List<Parameter>)js.ReadObject(ms);
                                    ms.Close();
                                }
                                ExecutionResult consolidatedResult = new WorkflowExecutionManager().Execute(wfIdentifier);
                                ConsoleTrace(string.Format("Scheduled request {0}, Workflow {1} execution completed...", Req.Id, Req.RequestId));

                                if (consolidatedResult.IsSuccess)
                                {
                                    req.Message = consolidatedResult.SuccessMessage;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Success... \n" + consolidatedResult.SuccessMessage);
                                    Console.ResetColor();
                                }
                                else
                                {
                                    req.Message = consolidatedResult.ErrorMessage;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Error... \n" + consolidatedResult.ErrorMessage);
                                    Console.ResetColor();
                                }
                                if (consolidatedResult.Output != null && consolidatedResult.Output.Count > 0)
                                {
                                    consolidatedResult.Output.ForEach(outPara =>
                                    {
                                        if (outPara.ParameterValue is object && outPara.IsSecret)
                                            outPara.ParameterValue = SecurePayload.Secure((new JavaScriptSerializer().Serialize(outPara.ParameterValue)), "IAP2GO_SEC!URE");
                                    });

                                    //serialize output parameters
                                    req.OutputParameters = JSONSerialize(consolidatedResult.Output);
                                    ConsoleTrace(string.Format("Scheduled request {0}, Workflow {1}, Output Result:", Req.Id, Req.RequestId));
                                    Console.WriteLine();
                                    ConsoleTrace(req.OutputParameters);
                                    Console.WriteLine();

                                }
                                isProcessedWithAccess = true;
                            }
                            ConsoleTrace(string.Format("Scheduled request {0}, Posting Workflow {1} execution update...", Req.Id, Req.RequestId));
                            if (isProcessedWithAccess)
                            {
                                req.ExecutionStatus = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.Completed;
                                req.IAPNode = "UI_" + Environment.MachineName;
                                req.Id = Req.Id;
                                req.ModifiedBy = userNameRunningInContextOf;
                            }
                            else
                            {
                                string unAuthorizedAccessMsg = string.Format("Requestor Does Not have Access to Run the Scheduled Transaction {0} with Workflow {1}", Req.Id, Req.RequestId);
                                req.ExecutionStatus = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.Failed;
                                req.IAPNode = "UI_" + Environment.MachineName;
                                req.Id = Req.Id;
                                req.ModifiedBy = userNameRunningInContextOf;
                                req.Message = unAuthorizedAccessMsg;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(unAuthorizedAccessMsg);
                                Console.ResetColor();
                            }
                            Infosys.WEM.Client.ScheduledRequest Postclient = new Infosys.WEM.Client.ScheduledRequest();
                            bool result = Postclient.ServiceChannel.UpdateRequestExecutionStatus(req);
                            if (result)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                ConsoleTrace(string.Format("Schedule Request {0} for Workflow {1} execution status update post succedded", Req.Id, Req.RequestId));
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                ConsoleTrace(string.Format("Schedule Request {0} for Workflow {1} execution status update post failed...", Req.Id, Req.RequestId));
                                Console.ResetColor();
                            };

                        }

                        if (IsFileChanged)
                        {
                            //MessageBox.Show("File Change Detected..Restarting");
                            Environment.ExitCode = (int)ExitCode.UnknownError;
                            Environment.Exit((int)ExitCode.UnknownError);
                        }
                        else
                        {
                            //wait for some time and again pool or any scheduled request to be executed
                            Thread.Sleep(iretryTimegap);
                        }
                    }
                    //catch (Exception ex)
                    catch (TimeoutException ex)
                    {
                        //for this type of exception, we need to re-try
                        //re-try will be taken care by the while loop
                        Console.ForegroundColor = ConsoleColor.Red;
                        ConsoleTrace("Time out has happened hence will retry the operation, Exception- " + ex.Message);
                        Console.WriteLine("");
                        ConsoleTrace("Will retry for another: " + retry.ToString() + " times");
                        Console.ResetColor();

                        if (retry == 0)
                        {
                            Environment.ExitCode = (int)ExitCode.UnknownError;
                            Environment.Exit((int)ExitCode.UnknownError);
                        }
                    }
                }
                Environment.ExitCode = (int)ExitCode.Success;
                return (int)ExitCode.Success;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error occured...");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Console.WriteLine("");
                Console.ResetColor();
                Environment.ExitCode = (int)ExitCode.UnknownError;
                Console.WriteLine(Environment.ExitCode);

                AutomationWorkflow IAPnodeRegister = new AutomationWorkflow();
                IAPnodeRegister.UnRegisterNode();
                //Console.ReadKey();
                LogHandler.LogError(ex.Message + "\n" + ex.StackTrace, LogHandler.Layer.Infrastructure);
                return (int)ExitCode.UnknownError;
            }
        }

        private static int NormalModeExecution()
        {
            int status = 0;
            bool RunLocal = false;
            bool IsNodeExecution = false;
            WorkflowIndentifier2 wfIdentifier2 = new WorkflowIndentifier2();
            WorkflowIndentifier wfIdentifier = new WorkflowIndentifier();
            List<ExecutionResult> consolidatedResult = null;
            ExecutionResult result = null;

            if (!string.IsNullOrEmpty(WorkflowPath))
            {
                RunLocal = true;
            }
            if (RunLocal)
            {
                HasAccess = true;
            }
            else
            {
                HasAccess = UserHasAccess(Convert.ToInt32(CategoryId));
            }

            if (HasAccess)
            {
                ConsoleTrace("Workflow Console starting in Normal Mode");

                ConsoleTrace(CategoryId);
                if (!string.IsNullOrEmpty(CategoryId))
                {
                    wfIdentifier.WorkflowId = Guid.Parse(WorkflowId);
                    wfIdentifier.CategoryId = int.Parse(CategoryId);
                    wfIdentifier.WEMWorkflowServiceUrl = WorkflowServiceURL;
                    wfIdentifier.WorkflowVersion = int.Parse(Version);
                }

                string configpath = ConfigurationManager.AppSettings["LoadAssembly"];
                if (!String.IsNullOrEmpty(configpath))
                {
                    LoadDllsFromFolder(configpath);
                }
                if (!string.IsNullOrEmpty(ExternalAssemblyFolderPath))
                {
                    string path = ExternalAssemblyFolderPath;
                    if (!(configpath == path))
                    {
                        if (!String.IsNullOrEmpty(path))
                        {
                            LoadDllsFromFolder(path);
                        }
                    }
                }
                //handle the parameters
                if (!string.IsNullOrEmpty(Parameters))
                {
                    ReadParams(wfIdentifier);
                }

                if (!string.IsNullOrEmpty(WorkflowPath))
                {
                    wfIdentifier.path = WorkflowPath;
                }

                //if it is an iap node execution call the execute method using wfidentifier2
                if (!string.IsNullOrEmpty(IAPNodeServerNames))
                {
                    wfIdentifier2 = new WorkflowIndentifier2();
                    wfIdentifier2.WorkflowId = wfIdentifier.WorkflowId;
                    wfIdentifier2.CategoryId = wfIdentifier.CategoryId;
                    wfIdentifier2.WEMWorkflowServiceUrl = wfIdentifier.WEMWorkflowServiceUrl;
                    wfIdentifier2.WorkflowVersion = wfIdentifier.WorkflowVersion;
                    wfIdentifier2.Parameters = wfIdentifier.Parameters;
                    IsNodeExecution = true;
                    if (!string.IsNullOrEmpty(Domain))
                    {
                        wfIdentifier2.Domain = Domain;
                    }
                    wfIdentifier2.IapNodeNetTcpPort = 9002;
                    wfIdentifier2.RemoteServerNames = IAPNodeServerNames;

                    if (!string.IsNullOrEmpty(ExecutionMode))
                    {
                        // Local Mode
                        if (ExecutionMode == "1")
                        {
                            wfIdentifier2.ExecutionMode = Infosys.WEM.WorkflowExecutionLibrary.Entity.ExecutionModeType.Local;
                            ConsoleTrace("Workflow execution mode has been configured for Local");
                        }
                        // IAP Nodes
                        else if (ExecutionMode == "2")
                        {
                            wfIdentifier2.ExecutionMode = Infosys.WEM.WorkflowExecutionLibrary.Entity.ExecutionModeType.RunOnIAPNode;
                            if (!string.IsNullOrEmpty(IapNodeTransportScheme) && IapNodeTransportScheme == "2")
                            {
                                wfIdentifier2.IapNodeTransport = Infosys.WEM.WorkflowExecutionLibrary.Entity.IapNodeTransportType.Nettcp;
                                if (!string.IsNullOrEmpty(IapNodePortNumber))
                                {
                                    wfIdentifier2.IapNodeNetTcpPort = Convert.ToInt32(IapNodePortNumber);
                                }
                            }
                            else if (!string.IsNullOrEmpty(IapNodeTransportScheme) && IapNodeTransportScheme == "1")
                            {
                                wfIdentifier2.IapNodeTransport = Infosys.WEM.WorkflowExecutionLibrary.Entity.IapNodeTransportType.Http;
                                if (!string.IsNullOrEmpty(IapNodePortNumber))
                                {
                                    wfIdentifier2.IapNodeHttpPort = Convert.ToInt32(IapNodePortNumber);
                                }
                            }
                            ConsoleTrace("Workflow execution mode has been configured for IAP Nodes");
                        }
                    }
                }

                if (string.IsNullOrEmpty(WorkflowId))
                {
                    Console.WriteLine(string.Format("Workflow executing..."));
                }
                else
                {
                    Console.WriteLine(string.Format("Workflow {0} started execution...", WorkflowId));
                }

                if (!IsNodeExecution)
                {
                    result = new WorkflowExecutionManager().Execute(wfIdentifier);
                }
                else
                {
                    consolidatedResult = new WorkflowExecutionManager().Execute(wfIdentifier2);
                }

                ConsoleTrace(string.Format("Workflow {0} executed...", wfIdentifier.WorkflowId.ToString()));
                // ExecutionResult result = consolidatedResult;
                if (consolidatedResult != null)
                {
                    foreach (var result1 in consolidatedResult)
                    {
                        ShowResult(result1, wfIdentifier.WorkflowId.ToString());
                    }
                }
                else
                {
                    ShowResult(result, wfIdentifier.WorkflowId.ToString());
                }

                ConsoleTrace("Workflow Execution Utility Exiting...");
                Environment.ExitCode = (int)ExitCode.Success;
                status = (int)ExitCode.Success;
            }
            else
                throw new Exception("User Does Not have Access to Run this Workflow");
            return status;
        }

        private static void ReadParams(WorkflowIndentifier wfIdentifier)
        {
            wfIdentifier.Parameters = new List<Parameter>();
            Parameter param = new Parameter();
            int i = 0;
            string argname = "";
            string val = "";
            bool argnameRead = false;
            bool firstQuote = false;
            bool secondQuote = false;

            if (Parameters.Contains(namedparamDelimiter)) //incase colon is part of value or used as seperator
            {   //for unnamed Parameters
                if (Parameters.Contains(paramDelimiter))
                {
                    string[] allparameters = Parameters.Split(paramDelimiter.ToCharArray());

                    foreach (string p in allparameters)
                    {
                        wfIdentifier.Parameters.Add(new Parameter() { ParameterName = "", ParameterValue = p.Replace("\"", string.Empty).Trim() });
                    }
                }
                //named Parameters
                else
                {
                    foreach (char c in Parameters)
                    {
                        if (c == ':')
                        {
                            if (!argnameRead)
                            {
                                argnameRead = true;
                            }
                            else
                            {
                                val = val + c;
                            }
                        }
                        else if (c == '\\')
                        {
                            val = val + c;
                        }
                        else if (c == '\"')
                        {
                            if (firstQuote == false)
                                firstQuote = true;
                            else
                            {
                                secondQuote = true;
                            }
                        }
                        else if (c == ' ')
                        {
                            if (secondQuote)
                            {
                                param = new Parameter();
                                param.ParameterName = argname;
                                param.ParameterValue = val;
                                wfIdentifier.Parameters.Add(param);
                                argname = "";
                                val = "";
                                secondQuote = false;
                                firstQuote = false;
                                argnameRead = false;
                            }
                            else if (!(firstQuote || secondQuote))
                            {
                                param = new Parameter();
                                param.ParameterName = argname;
                                param.ParameterValue = val;
                                wfIdentifier.Parameters.Add(param);
                                argname = "";
                                val = "";
                                secondQuote = false;
                                firstQuote = false;
                                argnameRead = false;
                            }
                            else
                            {
                                val = val + c;
                            }
                        }
                        else
                        {
                            if (!argnameRead)
                                argname = argname + c;
                            else val = val + c;
                        }
                        i = i + 1;
                    }
                    if (secondQuote || !(firstQuote && secondQuote))
                    {
                        param = new Parameter();
                        param.ParameterName = argname;
                        param.ParameterValue = val;
                        wfIdentifier.Parameters.Add(param);
                        argname = "";
                        val = "";
                        secondQuote = false;
                        firstQuote = false;
                        argnameRead = false;
                    }
                }
            }
            else if (Parameters.Contains(paramDelimiter))
            {
                string[] paramparts = Parameters.Split(paramDelimiter.ToCharArray());
                foreach (string p in paramparts)
                {
                    p.Replace(paramDelimiter, " ");
                    wfIdentifier.Parameters.Add(new Parameter() { ParameterName = "", ParameterValue = p.Replace("\"", string.Empty).Trim() });
                }
            }
            else if (!string.IsNullOrEmpty(Parameters.Trim()))
            {
                param.ParameterValue = Parameters.Replace("\"", string.Empty).Trim();
                wfIdentifier.Parameters.Add(param);
            }
            if (param != null && param.ParameterValue != null)
            {
                ConsoleTrace(param.ParameterValue.ToString());
            }
            wfIdentifier.Parameters.RemoveAll(x => (x.ParameterName == "" && x.ParameterValue == ""));
        }

        private static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            //Console.BackgroundColor = ConsoleColor.White;

            Console.WriteLine("TOPIC: WorkflowExecuteUtil Usage...");
            Console.WriteLine("");
            Console.WriteLine("DESCRIPTION");
            Console.WriteLine("Executes Workflow on local machine or on IAP node");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLES");
            Console.WriteLine("WorkFlowExecuteUtil.exe -wid 2e52519e-e5ef-4c39-8a9b-5b2be98b391e -cid 140  -ver 2 -params \"arg1:value1 arg2:value2\" -surl \"http://machinename/iapwemservices/WEMService.svc\"");
            Console.WriteLine("WorkFlowExecuteUtil.exe -em 1  -wpath  \"D:\\Automations\\TicketUpdate.xaml\"");
            Console.WriteLine("WorkFlowExecuteUtil.exe -wid 6691fbfa-43df-4996-a9fc-d4b358a23c8a -cid 140 -surl \"http://machinename/iapwemservices/WEMService.svc\" -ver 1 -servers \"node1,node2\" -em 2-sc 1 -port 80");
            Console.WriteLine("WorkFlowExecuteUtil.exe -wid 2e52519e-e5ef-4c39-8a9b-5b2be98b391e -cid 140 -surl \"http://machinename/iapwemservices/WEMService.svc\" -ver 2 -params \"value1;value2\" -eapath \"D:\\Custom\"");
            Console.WriteLine("");
            Console.WriteLine("NAMED PARAMETERS");
            Console.WriteLine("");
            Console.WriteLine("wid - Workflow ID");
            Console.WriteLine("cid - Category ID");
            Console.WriteLine("ver - Workflow Version to execute");
            Console.WriteLine("params - Workflow input parameters");
            Console.WriteLine("surl - Workflow Service URL");
            Console.WriteLine("em - Workflow Execution mode. 1 for Local, 2 for IAP node Server");
            Console.WriteLine("wpath - Full path of the Workflow on disk");
            Console.WriteLine("eapath - Full folder path. This is used to load all assemblies from given folder");
            Console.WriteLine("servers - IAP node Server names. Seperate multiple IAP by comma");
            Console.WriteLine("sc - Transport Scheme used to execute workflow on IAP nodes. Specify 1 for HTTP, \t 2 for NET.TCP");
            Console.WriteLine("port - Transport port listening on IAP node servers");
            Console.WriteLine("For unnamed parameter, the 'Parameters' should be passed seperated by semicolon as:");
            Console.WriteLine("\t\"param_value_1;param_value_2; ... ;param_value_N\"");
            Console.WriteLine("For named parameter, the 'Parameters' should be passed seperated by space as:");
            Console.WriteLine("\t\"param_name_1:param_value_1 param_name_2:\"param_value_2 with space\" ... param_name_N:param_value_N\"");
            Console.WriteLine("If values contain spaces, please enclose all values in quotes and then pass");
            Console.WriteLine("\t as arguments");
            Console.WriteLine("\t Example Named parameters: \":arg1:\"word1 word2\" arg2:\"word3 word4\"\"");
            Console.WriteLine("\t Example UnNamed parameters: \"\"word1 word2\";\"word3 word4\"\"");

            Console.WriteLine("");
            Console.WriteLine("UNNAMED PARAMETERS");
            Console.WriteLine("");
            Console.WriteLine(" WorkFlowExecuteUtil <Workflow Id> <Category Id> <Workflow Service Url> <Workflow Verion> <External Assembly Folder Path> <Parameters> <WorkflowPath> <IAPNodeServerName> <ExecutionMode> <IapNodeTransport> <IapNodePortNumber> <Domain> ");
            Console.ResetColor();
        }

        private static bool UserHasAccess(int categoryId)
        {
            int companyid = Convert.ToInt16(ConfigurationManager.AppSettings["Company"]);
            if (companyid == 0)
                throw new Exception("Invalid Configuration. Specify Company value in Application Configuration Settings");
            string[] roles = new string[3] { ApplicationConstants.ROLE_MANAGER, ApplicationConstants.ROLE_ANALYST, ApplicationConstants.ROLE_AGENT };
            return Security.CheckAccessInRole(companyid, categoryId, roles);

        }

        private static bool UserHasAccess(string alias, int categoryId)
        {
            int companyid = Convert.ToInt16(ConfigurationManager.AppSettings["Company"]);
            if (companyid == 0)
                throw new Exception("Invalid Configuration. Specify Company value in Application Configuration Settings");
            string[] roles = new string[3] { ApplicationConstants.ROLE_MANAGER, ApplicationConstants.ROLE_ANALYST, ApplicationConstants.ROLE_AGENT };

            return Security.CheckAccessInRole(alias, companyid, categoryId, roles);

        }

        public static void ShowResult(ExecutionResult result, string workflowId)
        {

            if (result == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Format("Error... \n" + "Unable to execute the workflow {0}...", workflowId.ToString()));
                Console.ResetColor();
                // Console.ReadKey();
            }
            else
            {
                if (result.IsSuccess)
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    if (!string.IsNullOrEmpty(WorkflowPath))
                        Console.WriteLine(string.Format("Workflow executed Successfully... \n" + result.SuccessMessage + "\n"));
                    else
                        Console.WriteLine(string.Format("Workflow {0} executed Successfully... \n" + result.SuccessMessage + "\n", workflowId.ToString()));

                    //also print the output parameter name value pair, if any
                    if (result.Output != null && result.Output.Count > 0)
                    {
                        ConsoleTrace("Output Parameter(s)\n");
                        result.Output.ForEach(p =>
                        {
                            ConsoleTrace(p.ParameterName + ":" + p.ParameterValue + "\n");
                        });

                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("Workflow {0} execution failed... \n" + result.ErrorMessage + "\n", workflowId.ToString()));
                    Console.ResetColor();
                    //Console.ReadKey();
                }
            }
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            IsFileChanged = true;
        }

        public static void LoadDllsFromFolder(string path)
        {
            DirectoryInfo dir;
            if (Directory.Exists(path))
            {

                dir = new DirectoryInfo(path);

                FileInfo[] fInfo = dir.GetFiles("*.dll");

                foreach (FileInfo fis in fInfo)
                {

                    path = fis.DirectoryName + "\\" + fis.Name;

                    Assembly assembly = Assembly.LoadFrom(path);

                }
            }
        }

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(EventHandler Handler, bool Add);

        private static bool Handler(ControlTypes ctrlType)
        {
            Environment.ExitCode = 0;
            if (mode == "Polling")
            {
                Console.WriteLine("Workflow Execution Node Unregistering..");
                AutomationWorkflow IAPnodeRegister = new AutomationWorkflow();
                IAPnodeRegister.UnRegisterNode();
                Console.WriteLine("Workflow Execution Node Unregistered.");

            }
            exitsytem = true;
            Environment.ExitCode = (int)ExitCode.Success;
            Console.WriteLine(Environment.ExitCode);
            Thread.Sleep(5000);
            Environment.Exit(0);

            Thread.Sleep(5000);
            return true;
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

        private static void ConsoleTrace(string message)
        {
            if (enableDetailedConsoleTrace)
            {
                if (!string.IsNullOrEmpty(message))
                    Console.WriteLine(message);
            }

        }
    }

    enum ExitCode : int
    {
        Success = 0,
        InvalidTermination = 1,
        UnknownError = 2
    }

    //Enumerated control types for handlers
    public enum ControlTypes
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }
}