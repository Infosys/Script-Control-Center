/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security;
using System.IO;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Service.Common.Contracts.Message;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Collections;
using IMSWorkBench.Infrastructure.Library.Services;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public abstract class ExecuteBase
    {
        ProcessStartInfo processInfo;
        public Process ProcessRunning;
        public string ParameterString = "";
        public string ScriptName;
        public string FileContent { get; set; }
        public string WorkingDir = "";
        public string IfeaScriptName = "";
        public string PythonMethodName = "";
        public string FilePath = "";
        public ScriptIndentifier ScriptIden = null;
        public string RemoteScriptPath = "";
        public string NetworkPath = "";
        private Hashtable hsDataTypes = null;
        public object[] PSRemoteParameters;
        public bool IsCommand = false;
        public Stream FileContentStream = null;
        string outTask = string.Empty;
        string errTask = string.Empty;
        
        /// <summary>
        /// The interface to start the execution of the intended script.
        /// The script is executed in a command prompt after navigating to the intended directory.
        /// </summary>
        /// <returns>the result of execution of the script</returns>
        public virtual List<ExecutionResult> Start()
        {
            List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
            ExecutionResult output = null;
            if (ProcessRunning != null && !string.IsNullOrEmpty(ScriptName))
            {
                if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS)
                {
                    if (ScriptName.IndexOf(" ") > 0)
                        ScriptName = "\"" + ScriptName + "\"";
                    // if remote server, split server names by comma
                    if (!string.IsNullOrEmpty(ScriptIden.RemoteServerNames))
                    {
                        string[] arrServerName = ScriptIden.RemoteServerNames.Split(',');
                        if (arrServerName.Length > 0)
                        {
                            output = new ExecutionResult();
                            ExecuteCommonScript objExecuteBatch = new ExecuteCommonScript();
                            if (System.IO.Path.GetExtension(FilePath).ToLower().Equals(".bat"))
                            {
                                consolidatedOutput = objExecuteBatch.ExecuteRemoteScriptNonPS("", ParameterString, arrServerName, RemoteScriptPath, ScriptIden);
                            }
                            else if (string.IsNullOrEmpty(FilePath))
                            {
                                consolidatedOutput = objExecuteBatch.ExecuteRemoteCommand(ScriptName, ParameterString, arrServerName, ScriptIden);
                            }

                        }
                        ProcessRunning.StandardInput.Close();
                        if (System.IO.Directory.Exists(NetworkPath))
                        {
                            System.IO.Directory.Delete(NetworkPath, true);
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid or no Remote server name(s) provided.");
                    }
                }
                else
                {
                    output = new ExecutionResult();
                    string command = "";
                    if (ScriptName.IndexOf(" ") > 0)
                    {
                        string updatedScriptName = "\"" + ScriptName + "\"";
                        command = updatedScriptName + " " + ParameterString;
                        ProcessRunning.StandardInput.WriteLine(command);
                    }
                    else
                    {
                        command = ScriptName + " " + ParameterString;
                        ProcessRunning.StandardInput.WriteLine(command);
                    }

                    ProcessRunning.StandardInput.Close();
                    outTask = string.Empty;
                    errTask = string.Empty;
                    //to handle long outout/error stream
                   // var outTask = ProcessRunning.StandardOutput.ReadToEndAsync();
                    //var errTask = ProcessRunning.StandardError.ReadToEndAsync();
                    ProcessRunning.WaitForExit();                  

                    output.SuccessMessage = GetOnlySubString(outTask, command);
                    output.ErrorMessage = errTask;
                    output.InputCommand = command;

                    if (string.IsNullOrEmpty(output.ErrorMessage))
                        output.IsSuccess = true;
                    else
                        output.IsSuccess = false;

                    consolidatedOutput.Add(output);
                }
            }
            return consolidatedOutput;
        }

        /// <summary>
        /// The interface to stop the started process
        /// </summary>
        public void Stop()
        {
            if (ProcessRunning != null && !ProcessRunning.HasExited)
            {
                ProcessRunning.Kill();
                ProcessRunning.WaitForExit();
                ProcessRunning = null;
            }
        }

        /// <summary>
        /// The interface to initiate the client and be ready to execute the script/command
        /// </summary>
        /// <param name="scripttobeExecuted">the details of the script file or cmmand to be executed</param>
        /// <param name="asAdmin">flag to dictate if the process script/command to be executed in admin mode</param>
        /// <returns>The result of initialization</returns>
        public virtual ClientInitializationResult InitializeClient(Script scripttobeExecuted, ScriptIndentifier scriptIdentifier, string filePath, bool asAdmin = false)
        {
            using (LogHandler.TraceOperations("ExecuteBase:InitializeClient", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {

                ClientInitializationResult result = new ClientInitializationResult() { IsSuccess = true };
                string fileExt = "";
                FilePath = filePath;
                this.ScriptIden = scriptIdentifier;
                fileExt = GetFileExtension(scripttobeExecuted.ScriptName);
                if (fileExt.ToLower().Equals("sh")  || (scripttobeExecuted.TaskType.ToLower().Equals("sh command")))
                {
                    //    ClientInitializationResult result = new ClientInitializationResult() { IsSuccess = true };
                    //    FilePath = filePath;

                    //    fileContent = FileContent;
                    this.ScriptIden = scriptIdentifier;
                    if (scripttobeExecuted.Parameters != null && scripttobeExecuted.Parameters.Count > 0)
                    {

                        scripttobeExecuted.Parameters.ForEach(p =>
                        {
                            if (!string.IsNullOrEmpty(p.ParameterValue))
                            {
                                if (p.IsPaired)
                                    ParameterString += " " + p.ParameterName + "=" + p.ParameterValue;
                                else
                                    ParameterString += " " + p.ParameterValue;

                            }
                        });

                    }
                    ScriptName = scripttobeExecuted.ScriptName;
                    if (scripttobeExecuted.TaskType.ToLower().Equals("sh command"))
                    {
                        ScriptName = scripttobeExecuted.TaskCmd;
                        IsCommand = true;
                    }
                    else
                    {
                        ScriptName = scripttobeExecuted.ScriptName;
                        this.ScriptIden.WorkingDir = scripttobeExecuted.ExecutionDir;
                    }
                }
                else
                {

                    processInfo = new ProcessStartInfo("cmd.exe");
                    processInfo.RedirectStandardInput = true;
                    processInfo.RedirectStandardOutput = true;
                    processInfo.UseShellExecute = false;
                    processInfo.RedirectStandardError = true;
                    processInfo.RedirectStandardOutput = true;
                    processInfo.RedirectStandardInput = true;


                    if (asAdmin)
                    {
                        //check of the parent process is running asadmin,
                        //if so then go ahead and try executing the script
                        //else return a message that the parent process invoking the script execution library needs to be run asadmnin
                        WindowsIdentity identity = WindowsIdentity.GetCurrent();
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                        {
                            result.IsSuccess = false;
                            result.ErrorMessage = "Please run the process which is invoking script execution, as Administrator";
                            return result;
                        }
                    }

                    ProcessRunning = Process.Start(processInfo);
                    ProcessRunning.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRunning_OutputDataReceived);
                    ProcessRunning.ErrorDataReceived += new  System.Diagnostics.DataReceivedEventHandler(ProcessRunning_ErrorDataReceived);
                    ProcessRunning.BeginOutputReadLine();
                    if (ProcessRunning != null)
                    {
                        //navigate to the working dir
                        if (!string.IsNullOrEmpty(scripttobeExecuted.ExecutionDir))
                        {
                            WorkingDir = scripttobeExecuted.ExecutionDir;
                            //navigate the working drive
                            ProcessRunning.StandardInput.WriteLine(GetWorkingDrive(scripttobeExecuted.ExecutionDir));
                            ProcessRunning.StandardInput.WriteLine("cd " + scripttobeExecuted.ExecutionDir);
                        }


                        //construct the parameters/arguements
                        //if the parameters are of kind- name-value pair
                        //then construct the parameter string as- /name:"value"
                        if (scripttobeExecuted.Parameters != null && scripttobeExecuted.Parameters.Count > 0)
                        {
                            PSRemoteParameters = new object[scripttobeExecuted.Parameters.Count];
                            int count = 0;
                            if (fileExt.ToLower().Equals("ps1") || fileExt.ToLower().Equals("py") || fileExt.ToLower().Equals("iapd"))
                                hsDataTypes = NativeDataTypes.RetrieveDataTypes(fileExt.ToLower());


                            if (!string.IsNullOrEmpty(scripttobeExecuted.MethodName))
                                PythonMethodName = scripttobeExecuted.MethodName;

                            scripttobeExecuted.Parameters.ForEach(p =>
                            {
                                // If named parameters provided
                                if (!string.IsNullOrEmpty(p.ParameterName) && p.IsPaired)
                                {
                                    string paramName = p.ParameterName;
                                    string paramValue = p.ParameterValue;
                                    if (paramName.IndexOf(" ") > 0 || paramName.IndexOf(@"\") > 0)
                                        paramName = "\"" + paramName + "\"";

                                    if (paramValue.IndexOf(" ") > 0 || paramValue.IndexOf(@"\") > 0)
                                        paramValue = "\"" + paramValue + "\"";

                                    if (fileExt.ToLower().Equals("ps1"))
                                    {
                                        // RetrieveDataTypes("ps1");
                                        if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS)
                                        {
                                            object value = GetPowerShellRemoteDataType(p);
                                            PSRemoteParameters[count] = value;
                                        }
                                        else
                                        {
                                            string value = GetPowerShellDataType(p);
                                            ParameterString += " -" + paramName + value;
                                        }
                                    }
                                    else if (fileExt.ToLower().Equals("iapd"))
                                        ParameterString += " " +paramName + ":" + paramValue;
                                    else //(fileExt.ToLower().Equals("vbs"))
                                        ParameterString += " /" + paramName + ":" + paramValue;
                                    // ParameterString += " /" + paramName + ":\"" + paramValue + "\"";
                                }
                                else
                                {
                                    if (scripttobeExecuted.TaskType.ToLower().Equals("command"))
                                    {
                                        //if (p.ParameterValue.IndexOf(" ") > 0 || p.ParameterValue.IndexOf(@"\") > 0)
                                        //    ParameterString += " \"" + p.ParameterValue + "\"";
                                        //else
                                        //    ParameterString += " " + p.ParameterValue;

                                        //in case of type command, the parameter (if any) property has the  argument to be passed to the command
                                        //and hence assign it as it without any extra double quotes.
                                        //as per standard for any command, if any argument has space then that argument has to be passed in double quotes from the client itself 

                                        ParameterString += " " + p.ParameterValue;
                                    }
                                    else
                                    {
                                        if (fileExt.ToLower().Equals("ps1"))
                                        {
                                            //RetrieveDataTypes("ps1");
                                            if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS)
                                            {
                                                object value = GetPowerShellRemoteDataType(p);
                                                PSRemoteParameters[count] = value;
                                            }
                                            else
                                            {
                                                string value = GetPowerShellDataType(p);
                                                ParameterString += value + ",";
                                            }
                                        }
                                        else if (fileExt.ToLower().Equals("py"))
                                        {
                                            // Run with default implementation
                                            if (string.IsNullOrEmpty(PythonMethodName))
                                            {
                                                ParameterString += " \"" + p.ParameterValue + "\"";
                                            }
                                            // Run with data types
                                            else
                                            {
                                                string value = GetPythonDataType(p);
                                                ParameterString += value + ",";
                                            }
                                        }
                                        else if (fileExt.ToLower().Equals("iapd"))
                                        {
                                            if (string.IsNullOrEmpty(PythonMethodName))
                                            {
                                                if (string.IsNullOrEmpty(ParameterString))
                                                    ParameterString += "'" + p.ParameterValue + "'";
                                                else
                                                    ParameterString += ",'" + p.ParameterValue + "'";
                                            }
                                            // Run with data types
                                            else
                                            {
                                                string value = GetPythonDataType(p);
                                                if (string.IsNullOrEmpty(ParameterString))
                                                    ParameterString += value;
                                                else
                                                    ParameterString += "," + value;
                                            }
                                        }
                                        else
                                            ParameterString += " \"" + p.ParameterValue + "\"";
                                    }
                                }
                                count = count + 1;
                            });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(scripttobeExecuted.MethodName))
                                PythonMethodName = scripttobeExecuted.MethodName;
                        }

                        if (scripttobeExecuted.TaskType.ToLower().Equals("command"))
                            ScriptName = scripttobeExecuted.TaskCmd;
                        else
                            ScriptName = scripttobeExecuted.ScriptName;

                        if (!string.IsNullOrEmpty(scripttobeExecuted.IfeaScriptName))
                            IfeaScriptName = scripttobeExecuted.IfeaScriptName;


                        // Copy script file to remote machine
                        if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS && !string.IsNullOrEmpty(filePath))
                        {
                            //Removed PS Condition
                            if (fileExt.ToLower().Equals("vbs") || fileExt.ToLower().Equals("js") || fileExt.ToLower().Equals("bat") || fileExt.ToLower().Equals("py") || fileExt.ToLower().Equals("iapd") ) //PSRNP
                            {
                                RemoteScriptPath = GetScriptPathOnRemote(GetRemoteShareUrl(scriptIdentifier.WEMScriptServiceUrl), filePath, FileContent);
                            }
                        }
                    }
                }
                return result;
            }
        }

        public virtual void ProcessRunning_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            errTask += e.Data + Environment.NewLine;
            Console.WriteLine(e.Data);
        }

        public virtual void ProcessRunning_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            outTask += e.Data + Environment.NewLine;
            Console.WriteLine(e.Data);
        }
        

        //private void RetrieveDataTypes(string scriptType)
        //{
        //    hsDataTypes = new Hashtable();
        //    switch (scriptType)
        //    {
        //        case "ps1":
        //            hsDataTypes.Add(scriptType + "01", "string");
        //            hsDataTypes.Add(scriptType + "02", "char");
        //            hsDataTypes.Add(scriptType + "03", "byte");
        //            hsDataTypes.Add(scriptType + "04", "int");
        //            hsDataTypes.Add(scriptType + "05", "long");
        //            hsDataTypes.Add(scriptType + "06", "bool");
        //            hsDataTypes.Add(scriptType + "07", "decimal");
        //            hsDataTypes.Add(scriptType + "08", "single");
        //            hsDataTypes.Add(scriptType + "09", "short");
        //            hsDataTypes.Add(scriptType + "10", "double");
        //            hsDataTypes.Add(scriptType + "11", "datetime");
        //            break;
        //        case "py":
        //            break;
        //        default:
        //            break;
        //    }

        //    //return hsDataTypes;
        //}
        private object GetPowerShellRemoteDataType(Parameter p)
        {
            string dataType = "";
            if (!string.IsNullOrEmpty(p.DataType))
                dataType = hsDataTypes[p.DataType].ToString();
            else
                dataType = hsDataTypes["ps101"].ToString();

            switch (dataType)
            {
                case "char":
                    return Convert.ToChar(p.ParameterValue);
                case "byte":
                    return Convert.ToByte(p.ParameterValue);
                case "int":
                    return Convert.ToInt32(p.ParameterValue);
                case "long":
                    return Convert.ToInt64(p.ParameterValue);
                case "bool":
                    if (p.ParameterValue.ToLower().Equals("true"))
                        return true;
                    else
                        return false;
                case "decimal":
                    return Convert.ToDecimal(p.ParameterValue);
                case "single":
                    return Convert.ToSingle(p.ParameterValue);
                case "short":
                    return Convert.ToInt16(p.ParameterValue);
                case "double":
                    return Convert.ToDouble(p.ParameterValue);
                case "datetime":
                    return Convert.ToDateTime(p.ParameterValue);
                default:
                    return p.ParameterValue;
                //case "bool":
                //    return true;
                //case "datetime":
                //    return Convert.ToDateTime( p.ParameterValue);
                //case "int":
                //    return Convert.ToInt32(p.ParameterValue);
                //default:
                //    return p.ParameterValue;
            }
        }

        private string GetPythonDataType(Parameter p)
        {
            string value = "";
            string dataType = "";
            if (hsDataTypes[p.DataType] != null)
                dataType = hsDataTypes[p.DataType].ToString();
            else
                dataType = hsDataTypes["py01"].ToString();

            switch (dataType)
            {
                case "bool":
                    if (p.ParameterValue.ToLower().Equals("true"))
                        value = "True";
                    else
                        value = "False";
                    break;
                case "int":
                    value = p.ParameterValue;
                    break;
                case "long":
                    value = p.ParameterValue + "L";
                    break;
                case "float":
                    value = p.ParameterValue;
                    break;
                //case "complex":
                //    value = p.ParameterValue + "j";
                //    break;
                //case "unicode":
                //    value = " \'" + p.ParameterValue + "\'";
                //    break;
                case "list":
                    //value = "[" + p.ParameterValue + "]";
                    value = p.ParameterValue;
                    break;
                case "tuple":
                    //value = "(" + p.ParameterValue + ")";
                    value = p.ParameterValue;
                    break;
                //case "bytearray":
                //    value = "\'" + p.ParameterValue + "\'";
                //    break;
                //case "buffer":
                //    value = "\'" + p.ParameterValue + "\'";
                //    break;
                //case "xrange":
                //    value = "\'" + p.ParameterValue + "\'";
                //    break;
                case "set":
                    value = p.ParameterValue;
                    break;
                //case "frozenset":
                //    value = "\'" + p.ParameterValue + "\'";
                //    break;
                case "dict":
                    value = p.ParameterValue;
                    break;
                //case "file":
                //    value = "\'" + p.ParameterValue + "\'";
                //    break;
                //case "memoryview":
                //    value = "\'" + p.ParameterValue + "\'";
                //    break;
                default:
                    value = "\'" + p.ParameterValue + "\'";
                    break;
            }
            return value;

        }


        private string GetPowerShellDataType(Parameter p)
        {
            string value = "";
            string dataType = "";
            if (!string.IsNullOrEmpty(p.DataType))
                dataType = hsDataTypes[p.DataType].ToString();
            else
                dataType = hsDataTypes["ps101"].ToString();

            switch (dataType)
            {
                case "char":
                    value = " 0x" + p.ParameterValue;
                    break;
                case "byte":
                    value = " \'" + p.ParameterValue + "\'";
                    break;
                case "int":
                    value = " " + p.ParameterValue;
                    break;
                case "long":
                    value = " " + p.ParameterValue;
                    break;
                case "bool":
                    if (p.ParameterValue.ToLower().Equals("true"))
                        value = " $" + "True";
                    else
                        value = " $" + "False";
                    break;
                case "decimal":
                    value = " " + p.ParameterValue + "d";
                    break;
                case "single":
                    value = " " + p.ParameterValue;
                    break;
                case "short":
                    value = " " + p.ParameterValue;
                    break;
                case "double":
                    value = " \'" + p.ParameterValue + "\'";
                    break;
                case "datetime":
                    value = " \'" + p.ParameterValue + "\'";
                    break;
                default:
                    value = " \'" + p.ParameterValue + "\'";
                    break;
            }
            return value;
        }

        public string GetWorkingDrive(string workingDir)
        {
            string drive = "";
            string[] folderpaths = workingDir.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (folderpaths.Length > 0)
                drive = folderpaths[0] + ":";

            return drive;
        }

        private static string GetFileExtension(string fileName)
        {
            return System.IO.Path.GetExtension(fileName).Replace(".", "");
        }

        /// <summary>
        /// This method is used to copy script file on remote machine by creating temp folder and
        /// returns of the complete path of the script on the remote machine.
        /// </summary>
        /// <param name="remoteShareUrl">Network Path where temp folder will be created</param>
        /// <param name="localFilePath">Local path of script file</param>
        /// <param name="fileContent">Content of script file</param>
        /// <returns>Remote path of the script file</returns>
        private string GetScriptPathOnRemote(string remoteShareUrl, string localFilePath, string fileContent="")
        {
            string result = "";
            if (!string.IsNullOrEmpty(remoteShareUrl))
                NetworkPath = remoteShareUrl + @"\" + Guid.NewGuid().ToString();
            if (!System.IO.Directory.Exists(NetworkPath))
            {
                System.IO.Directory.CreateDirectory(NetworkPath);
            }
            var remote = Path.Combine(NetworkPath, Path.GetFileName(localFilePath));
            if (File.Exists(localFilePath))
                File.Copy(localFilePath, remote, true);
            else if(!string.IsNullOrEmpty(fileContent))
                File.WriteAllText(remote, fileContent);
            result = remote;
            return result;
        }

        /// <summary>
        /// This method is used to check if current server is local server.
        /// </summary>
        /// <param name="serverName">name of server</param>
        /// <returns>true if server is local</returns>
        private static bool LocalServer(string serverName)
        {
            bool result = false;

            if (serverName.ToLower().Equals("localhost") || serverName.ToLower().Equals(Environment.MachineName.ToLower()) || serverName.Equals("."))
                result = true;

            return result;
        }

        /// <summary>
        /// This method is used to get RemoteShareUrl value from company table and then
        /// constructs path of network share folder based on iapremoteshare in app config.
        /// </summary>
        /// <returns>Value of StorageBaseURL column</returns>
        private static string GetRemoteShareUrl(string companyServiceUrl)
        {
            string remoteShareUrl = "";
            string companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
            string iapremoteshare = System.Configuration.ConfigurationManager.AppSettings["iapremoteshare"];
            remoteShareUrl = Infosys.WEM.Client.CommonServices.Instance.RemoteShareUrl + @"\" + iapremoteshare;
            return remoteShareUrl;
        }

        private static T GetResponse<T>(string url)
        {
            WebClient srvProxy = new WebClient();
            byte[] data = srvProxy.DownloadData(url);
            Stream stream = new MemoryStream(data);
            DataContractJsonSerializer obj = new DataContractJsonSerializer(typeof(T));
            return (T)obj.ReadObject(stream);
        }

        public static string GetOnlySubString(string complete, string afterThis)
        {
            if (!string.IsNullOrEmpty(complete) && !string.IsNullOrEmpty(afterThis))
            {
                complete = complete.Substring(complete.IndexOf(afterThis) + afterThis.Length);
            }
            return complete;
        }

        
    }
}
