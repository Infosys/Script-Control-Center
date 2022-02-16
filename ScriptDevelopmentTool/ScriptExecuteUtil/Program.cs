/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Infosys.WEM.ScriptExecutionLibrary;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Reflection;
using Infosys.WEM.Utilities.ScriptExecuteUtil;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.IAP.CommonClientLibrary;
using System.Runtime.Serialization.Json;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using Infosys.WEM.SecureHandler;
using Infosys.WEM.Infrastructure.Common;

namespace ScriptExecuteUtil
{
    public class Program
    {
        static bool IsFileChanged = false;
        static string namedparamDelimiter = ":";
        static string paramDelimiter = ";";
        static bool exitsytem = false;
        static string mode = string.Empty;
        static bool enableDetailedConsoleTrace = false;

        const int ONERROR_RETRY_FOR_GETNEXTREQUEST = 5;
        const int RETRY_TIMEGAP_FOR_GETNEXTREQUEST = 10000;

        //This delegate type used as handler routine to SetConsoleControlHandler.
        public delegate bool EventHandler(ControlTypes CtrlType);
        static EventHandler _handler;

        static string ScriptId = string.Empty;
        static string CategoryId = string.Empty;
        static string ScriptServiceURL = string.Empty;
        static string WorkingDir = string.Empty;
        static string Parameters = string.Empty;
        static string UserName = string.Empty;
        static SecureString Password;
        static string LinuxKeyPath = string.Empty;
        static string ScriptPath = string.Empty; 
        static string IAPNodeServerNames = string.Empty;
        static string ExecutionMode = string.Empty;
        static string IapNodeTransportScheme = string.Empty;
        static string IapNodePortNumber = string.Empty;
        static string Domain = string.Empty;
        static bool HasAccess = false;

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
                Console.WriteLine("\nPlease check the usage/help for the correct ways to invoke the ScriptExecuteUtil\n");
                Environment.ExitCode = (int)ExitCode.UnknownError;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error occured...");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                Console.ResetColor();
                Environment.ExitCode = (int)ExitCode.UnknownError;
                Console.WriteLine("Exit Code=" + Environment.ExitCode);
                return (int)ExitCode.UnknownError;
            }
        }

        private static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("TOPIC: ScriptExecuteUtil Usage...");
            Console.WriteLine("");
            Console.WriteLine("DESCRIPTION");
            Console.WriteLine("Executes Script on local machine or on IAP node");
            Console.WriteLine("");
            Console.WriteLine("EXAMPLES");
            Console.WriteLine("ScriptExecuteUtil.exe -cid 140 -sid 294 -surl \"http://servername/iapwemservices/WEMScriptService.svc\" ");
            Console.WriteLine("ScriptExecuteUtil.exe -cid 140 -sid 300 -surl \"http://servername/iapwemservices/WEMScriptService.svc\" -params \"arg1:val1 arg2:val2\" ");
            Console.WriteLine("ScriptExecuteUtil.exe -cid 140 -sid 300 -surl \"http://servername/iapwemservices/WEMScriptService.svc\" -params \"val1;val2\" ");
            Console.WriteLine("ScriptExecuteUtil.exe exe  -cid 140 -sid 300 -surl \"http://servername/iapwemservices/WEMScriptService.svc\" -params \"arg1:val1 arg2:val2\" -servers \"node1,node2\" -em 2 -sc 1 -port 80 -uname suchi_paharia -pwd phjghjg ");
            Console.WriteLine("");
            Console.WriteLine("NAMED PARAMETERS");
            Console.WriteLine("");
            Console.WriteLine("sid - Script ID");
            Console.WriteLine("cid - Category ID");
            Console.WriteLine("params - Script input parameters");
            Console.WriteLine("surl - Script Service URL");
            Console.WriteLine("wdir - Script working directory");
            Console.WriteLine("em - Script Execution mode. 1 for Local, 2 for IAP node Server, 5 for PowerShell scripts, 6 for Linux ");
            Console.WriteLine("spath - Full path of the script on disk");
            Console.WriteLine("uname - User Name");
            Console.WriteLine("pwd - Secured Password");
            Console.WriteLine("lkp - Linux key path");
            Console.WriteLine("servers - IAP node Server names. Seperate multiple IAP by comma");
            Console.WriteLine("sc - Transport Scheme used to execute workflow on IAP nodes. Specify 1 for HTTP, \t 2 for NET.TCP");
            Console.WriteLine("port - Transport port listening on IAP node servers");
            Console.WriteLine("domain - Domain of IAP node server");
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
            Console.WriteLine("ScriptExecuteUtil <Categroy Id> <Script Id> <Script Service Url> <Parameters> <Working Dir> <Remote Server Names> <UserName> <Password> <LinuxKeyPath>  <Remote Execution Mode> <IapNodeTransport> <IapNodePortNumber> <Domain> ");

            Console.ResetColor();
        }

        private static void ParseArguments(string[] args)
        {
            int i = 0;
            bool arePositionalArguments = true;

            foreach (string argument in args)
            {
                switch (argument.ToLower())
                {
                    case "-sid":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        ScriptId = args[i + 1];
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
                        ScriptServiceURL = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-wdir":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        WorkingDir = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-params":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        Parameters = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-uname":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        UserName = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-pwd":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        string pwd = args[i + 1];
                        arePositionalArguments = false;
                        UnSecurePassword(pwd);
                        break;
                    case "-lkp":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        LinuxKeyPath = args[i + 1];
                        arePositionalArguments = false;
                        break;
                    case "-spath":
                        if (args[i + 1] == null)
                            throw new System.Exception("Please provide all input parameters");
                        ScriptPath = args[i + 1];
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
                if (string.IsNullOrEmpty(args[1]))
                {
                    throw new System.Exception("Check Help for correct usage of ScriptExecutionUtil");
                }
                else ScriptId = args[1];

                if (args[0] == null)
                {
                    throw new System.Exception("Check Help for correct usage of ScriptExecutionUtil");
                }
                else CategoryId = args[0];

                if (args.Count() >= 3)
                {
                    ScriptServiceURL = args[2];                   
                }
                else throw new System.Exception("Check Help for correct usage of ScriptExecutionUtil");

                if (args.Count() >= 4)
                {
                    Parameters = args[3];
                }

                if (args.Count() >= 5)
                {
                    WorkingDir = args[4];
                }

                if (args.Count() >= 6)
                {
                    IAPNodeServerNames = args[5];
                }

                if (args.Count() >= 7)
                {
                    UserName = args[6];
                }

                if (args.Count() >= 8)
                {
                    UnSecurePassword(args[7]);
                }

                if (args.Count() >= 9)
                {
                    LinuxKeyPath = args[8];
                }

                if (args.Count() >= 10)
                {
                    ExecutionMode = args[9];
                }

                if (args.Count() >= 11)
                {
                    IapNodeTransportScheme = args[10];
                }

                if (args.Count() >= 12)
                {
                    IapNodePortNumber = args[11];
                }

                if (args.Count() >= 13)
                {
                    Domain = args[12];
                }

                if (args.Count() >= 14)
                {
                    ScriptPath = args[13];
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
                ConsoleTrace("Script Console starting in Polling Mode...");
                string iAPnode = "SC_" + Environment.MachineName;

                //Start Polling
                RegisterScript IAPnodeRegister = new RegisterScript();
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
                            path = fis.DirectoryName + "\\" + fis.Name;
                            Assembly assembly = Assembly.LoadFrom(path);
                        }
                    }

                    FileSystemWatcher watcher = new FileSystemWatcher();
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
                        iwfRequest = Getclient.ServiceChannel.GetNextRequest(domain, "SC_" + Environment.MachineName, "2");//you can call either workflow or script

                        //reset the 'on error retry count' for get next request for successfull getnexrequest call
                        retry = iretryCount + 1;

                        Req = iwfRequest.Request;
                        if (Req != null)
                        {
                            bool isProcessedWithAccess = false;

                            ConsoleTrace("New Script execution request received...");
                            ScriptIndentifier scrIdentifier = new ScriptIndentifier();
                            scrIdentifier.ScriptId = int.Parse(Req.RequestId);
                            scrIdentifier.SubCategoryId = Req.CategoryId;
                            scrIdentifier.WEMScriptServiceUrl = "";
                            string userNameRunningInContextOf = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            UpdateRequestExecutionStatusReqMsg req = new UpdateRequestExecutionStatusReqMsg();
                            req.AssignedTo = Req.AssignedTo;
                            if (UserHasAccess(Req.Requestor, iwfRequest.Request.CategoryId))
                            {
                                //  scrIdentifier. = Req.RequestVersion;

                                string jSonParam = Req.InputParameters;
                                ConsoleTrace(string.Format("Scheduled request {0}, Script {1} to be executed...", Req.Id, scrIdentifier.ScriptId.ToString()));
                                if (jSonParam != null)
                                {
                                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<Parameter>));
                                    MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(jSonParam));
                                    // List<Parameter> parameters = new List<Parameter>();
                                    scrIdentifier.Parameters = (List<Parameter>)js.ReadObject(ms);
                                    ms.Close();

                                }
                                List<ExecutionResult> consolidatedResult = ScriptExecutionManager.Execute(scrIdentifier);
                                ConsoleTrace(string.Format("Scheduled request {0}, Script {1} execution completed...", Req.Id, scrIdentifier.ScriptId.ToString()));
                                ExecutionResult result = consolidatedResult[0];
                                if (result.Output != null && result.Output.Count > 0)
                                {
                                    //TBD Script Output parameters securehandling
                                    //serialize output parameters
                                    req.OutputParameters = JSONSerialize(result.Output);
                                    ConsoleTrace(string.Format("Scheduled request {0}, Script {1}, Output Result:", Req.Id, scrIdentifier.ScriptId.ToString()));
                                    Console.WriteLine();
                                    ConsoleTrace(req.OutputParameters);
                                    Console.WriteLine();

                                }

                                if (result.IsSuccess)
                                {
                                    req.Message = result.SuccessMessage;
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(string.Format("Script {0} executed Successfully... \n" + result.SuccessMessage, scrIdentifier.ScriptId.ToString()));
                                    Console.ResetColor();
                                }
                                else
                                {
                                    req.Message = result.ErrorMessage;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(string.Format("Script {0} execution Failed... \n" + result.ErrorMessage, scrIdentifier.ScriptId.ToString()));
                                    Console.ResetColor();

                                }

                                isProcessedWithAccess = true;
                            }
                            if (isProcessedWithAccess)
                            {
                                req.ExecutionStatus = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.Completed;
                                req.IAPNode = "SC_" + Environment.MachineName;
                                req.Id = Req.Id;
                                req.ModifiedBy = userNameRunningInContextOf;
                            }
                            else
                            {
                                string unAuthorizedAccessMsg = string.Format("Requestor Does Not have Access to Run the Scheduled Transaction {0} with Script {1}", Req.Id, scrIdentifier.ScriptId.ToString());
                                req.ExecutionStatus = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.Failed;
                                req.IAPNode = "SC_" + Environment.MachineName;
                                req.Id = Req.Id;
                                req.ModifiedBy = userNameRunningInContextOf;
                                req.Message = unAuthorizedAccessMsg;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(unAuthorizedAccessMsg);
                                Console.ResetColor();
                            }
                            Infosys.WEM.Client.ScheduledRequest Postclient = new Infosys.WEM.Client.ScheduledRequest();
                            ConsoleTrace(string.Format("Posting Script {0} execution update...", scrIdentifier.ScriptId.ToString()));
                            bool result1 = Postclient.ServiceChannel.UpdateRequestExecutionStatus(req);
                            if (result1)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                ConsoleTrace(string.Format("Schedule Request {0} for Script {1} execution status update post succedded", req.Id, scrIdentifier.ScriptId.ToString()));
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                ConsoleTrace(string.Format("Schedule Request {0} for Script {1} execution status update post failed...", req.Id, scrIdentifier.ScriptId.ToString()));
                                Console.ResetColor();
                            }


                        }

                        if (IsFileChanged)
                        {
                            Environment.ExitCode = (int)ExitCode.UnknownError;
                            Environment.Exit((int)ExitCode.UnknownError);
                        }
                        else
                        {
                            //wait for some time and again pool or any scheduled request to be executed
                            Thread.Sleep(iretryTimegap);
                        }
                    }
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
                Console.WriteLine("Script Execution Utility Exiting..");
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
                RegisterScript IAPnodeRegister = new RegisterScript();
                IAPnodeRegister.UnRegisterNode();
                Console.WriteLine(Environment.ExitCode);
                LogHandler.LogError(ex.Message + "\n" + ex.StackTrace, LogHandler.Layer.Infrastructure);
                // Console.ReadKey();
                return (int)ExitCode.UnknownError;
            } throw new NotImplementedException();
        }

        private static int NormalModeExecution()
        {

            bool RunLocal = false;

             if (!string.IsNullOrEmpty(ScriptPath))
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
                ConsoleTrace("Script Console starting in Normal Mode");
                ScriptIndentifier scriptIden = new ScriptIndentifier();

                if (!RunLocal)
                {
                    scriptIden.ScriptId = int.Parse(ScriptId);
                    scriptIden.SubCategoryId = int.Parse(CategoryId);
                    scriptIden.WEMScriptServiceUrl = ScriptServiceURL;
                }

                if (!string.IsNullOrEmpty(IAPNodeServerNames))
                {
                    scriptIden.RemoteServerNames = IAPNodeServerNames;
                    ConsoleTrace("Script has remote server call");
                }

                scriptIden.Password = Password;

                if (ExecutionMode == "6")
                {
                    if (!string.IsNullOrEmpty(LinuxKeyPath))
                    {
                        if (Password != null)
                        {
                            ConsoleTrace("Please specify only one method of Authentication.Either Use Key or Password based Auth");
                            return 0;
                        }
                        scriptIden.LinuxKeyPath = LinuxKeyPath;
                    }
                    else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["LinuxKeyPath"]) && Password == null)
                    {
                        scriptIden.LinuxKeyPath = System.Configuration.ConfigurationManager.AppSettings["LinuxKeyPath"];
                    }
                    else if (!string.IsNullOrEmpty(UserName) && Password == null)
                    {
                        ConsoleTrace("Please provide password for the User Name");
                        return 0;
                    }
                    else if (string.IsNullOrEmpty(UserName) && Password != null)
                    {
                        ConsoleTrace("Please provide User Name and Password");
                        return 0;
                    }
                    else if (string.IsNullOrEmpty(LinuxKeyPath) && string.IsNullOrEmpty(UserName) && Password == null)
                    {
                        ConsoleTrace("Please specify method of Authentication.Either Linux Key or Password based Auth");
                        return 0;
                    }
                }

                if (!string.IsNullOrEmpty(ExecutionMode))
                {
                    // Local mode
                    if (ExecutionMode == "1")
                    {
                        scriptIden.ExecutionMode = Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType.Local;
                        ConsoleTrace("Script execution mode has been configured for Local");
                    }
                    // for IAP Nodes
                    if (ExecutionMode == "2")
                    {
                        scriptIden.ExecutionMode = Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType.RunOnIAPNode;
                        if (IapNodeTransportScheme == "2")
                        {
                            scriptIden.IapNodeTransport = Infosys.WEM.ScriptExecutionLibrary.IapNodeTransportType.Nettcp;
                            if (!string.IsNullOrEmpty(IapNodePortNumber))
                            {
                                scriptIden.IapNodeNetTcpPort = Convert.ToInt32(IapNodePortNumber);
                            }
                        }
                        else if (!string.IsNullOrEmpty(IapNodeTransportScheme))
                        {
                            scriptIden.IapNodeTransport = Infosys.WEM.ScriptExecutionLibrary.IapNodeTransportType.Http;
                            if (!string.IsNullOrEmpty(IapNodePortNumber))
                            {
                                scriptIden.IapNodeHttpPort = Convert.ToInt32(IapNodePortNumber);
                            }
                        }
                        if (!string.IsNullOrEmpty(Domain))
                        {
                            scriptIden.Domain = Domain;
                        }
                        ConsoleTrace("Script execution mode has been configured for IAP Nodes");
                    }
                    // Remote mode
                    if (ExecutionMode == "5")
                    {
                        scriptIden.ExecutionMode = Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType.PowerShell;
                        scriptIden.UserName = UserName;
                        ConsoleTrace("Script execution mode has been configured for Remote Machine");
                    }
                    //Linux Mode
                    if (ExecutionMode == "6")
                    {
                        scriptIden.ExecutionMode = Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType.Linux;
                        scriptIden.UserName = UserName;
                        ConsoleTrace("Script execution mode has been configured for Linux Machine");
                    }

                    //Delegate Mode
                    if (ExecutionMode == "7")
                    {
                        List<Parameter> parameters = new List<Parameter>();
                        scriptIden.Path = ScriptPath;
                        scriptIden.ScriptId = Convert.ToInt32(ScriptId);
                        String[] parameter = Parameters.Split(',');
                        foreach (String data in parameter) {
                            Parameter param = new Parameter();
                            param.ParameterValue = data;
                            parameters.Add(param);
                        }
                      
                        scriptIden.Parameters = parameters;
                        scriptIden.ExecutionMode = Infosys.WEM.ScriptExecutionLibrary.ExecutionModeType.Delegate;
                        
                        ConsoleTrace("Script execution mode has been configured for Delegate Mode");
                    }
                }

                if (!string.IsNullOrEmpty(WorkingDir))
                {
                    scriptIden.WorkingDir = WorkingDir;
                }

                //handle the parameters
                if (!string.IsNullOrEmpty(Parameters))
                {
                    ReadParams(scriptIden);
                }

                if (!string.IsNullOrEmpty(ScriptPath))
                {
                    scriptIden.Path = ScriptPath;
                }

                ConsoleTrace(string.Format("Script {0} to be executed...", scriptIden.ScriptId.ToString()));

                List<ExecutionResult> consolidatedResult = ScriptExecutionManager.Execute(scriptIden);
                ConsoleTrace(string.Format("Script {0} executed...", scriptIden.ScriptId.ToString()));
                foreach (var result in consolidatedResult)
                {
                    if (result.IsSuccess)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(string.Format("Script {0} executed Successfully... \n" + result.SuccessMessage, scriptIden.ScriptId.ToString()));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(string.Format("Script {0} execution Failed... \n" + result.ErrorMessage, scriptIden.ScriptId.ToString()));
                        Console.ResetColor();
                    }

                }
                Console.WriteLine("Script Execution Utility Exiting...");
                Environment.ExitCode = (int)ExitCode.Success;
                return (int)ExitCode.Success;
            }
            else
                throw new Exception("User Does Not have Access to run this Script");
        }

        private static void UnSecurePassword(string pwd)
        {
            try
            {
                if (!string.IsNullOrEmpty(pwd))
                {

                    string Unsecurepwd = SecurePayload.UnSecure(pwd, "IAP2GO_SEC!URE");
                    Password = new SecureString();
                    foreach (char c in Unsecurepwd.ToCharArray())
                        Password.AppendChar(c);
                }
            }
            catch (Exception ex)
            {
                throw new System.Exception("Please provide valid secure string for the password");
            }
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

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            IsFileChanged = true;
        }

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(EventHandler Handler, bool Add);

        private static bool Handler(ControlTypes ctrlType)
        {
            Environment.ExitCode = 0;
            if (mode == "Polling")
            {
                Console.WriteLine("Script Execution Node Unregistering..");
                RegisterScript IAPnodeRegister = new RegisterScript();
                IAPnodeRegister.UnRegisterNode();
                Console.WriteLine("Script Execution Node Unregistered.");
            }

            exitsytem = true;
            Environment.ExitCode = (int)ExitCode.Success;
            Console.WriteLine(Environment.ExitCode);
            Thread.Sleep(5000);
            Environment.Exit(0);

            Thread.Sleep(5000);
            return true;
        }

        private static string[] SplitNamedParameter(string arg)
        {
            string[] parts = Regex.Split(arg, @":(?!/)", RegexOptions.None); //i.e. delimiter is ':' but it is not followed by '/'
            // to skip those with '/' before ':' use regex- (?<!/)
            return parts;
        }

        private static bool ContainsNamedParamDelimiter(string arg)
        {
            bool contains = Regex.IsMatch(arg, @":(?!/)"); //i.e. delimiter is ':' but it is not followed by '/'
            // to skip those with '/' before ':' use regex- (?<!/)
            return contains;
        }

        private static void ConsoleTrace(string message)
        {
            if (enableDetailedConsoleTrace)
            {
                if (!string.IsNullOrEmpty(message))
                    Console.WriteLine(message);
            }

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

        private static void ReadParams(ScriptIndentifier scriptIden)
        {
            scriptIden.Parameters = new List<Parameter>();
            Parameter param = new Parameter();
            int i = 0;
            string argname = "";
            string val = "";
            bool argnameRead = false;
            bool firstQuote = false;
            bool secondQuote = false;

            if (Parameters.Contains(namedparamDelimiter))
            {   //for unnamed Parameters
                if (Parameters.Contains(paramDelimiter))
                {
                    string[] allparameters = Parameters.Split(paramDelimiter.ToCharArray());

                    foreach (string p in allparameters)
                    {
                        string[] paramvalues = p.Split(namedparamDelimiter.ToCharArray());
                        scriptIden.Parameters.Add(new Parameter() { ParameterName = paramvalues[0], ParameterValue = paramvalues[1].Replace("\"", string.Empty).Trim() });
                    }
                }
                //named Parameters
                else
                {
                    foreach (char c in Parameters)
                    {
                        if (c == ':' && !argnameRead)
                        {
                            argnameRead = true;
                        }
                        //else if (c == '\\')
                        //{
                        //}
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
                                scriptIden.Parameters.Add(param);
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
                                scriptIden.Parameters.Add(param);
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
                        scriptIden.Parameters.Add(param);
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
                    scriptIden.Parameters.Add(new Parameter() { ParameterName = "", ParameterValue = p.Replace("\"", string.Empty).Trim() });
                }
            }
            else if (!string.IsNullOrEmpty(Parameters.Trim()))
            {
                param.ParameterValue = Parameters.Replace("\"", string.Empty).Trim();
                scriptIden.Parameters.Add(param);
            }
            if (param != null && param.ParameterValue != null)
            {
                ConsoleTrace(param.ParameterValue.ToString());
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
