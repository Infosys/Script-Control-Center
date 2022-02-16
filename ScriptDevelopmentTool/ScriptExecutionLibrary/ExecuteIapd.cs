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

using System.Configuration;
using System.IO;
using Infosys.ATR.Packaging;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Infosys.WEM.ScriptExecutionLibrary
{

    public class ExecuteIapd : ExecuteBase
    {
        //ScriptEngine ipyEngine = null;
        //ScriptScope ipyScope = null;
        //ScriptRuntime ipyRuntime = null;
        string outTask = string.Empty;
        string errTask = string.Empty;
        public override List<ExecutionResult> Start()
        {
            List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
            ExecutionResult output = null;
            //string pythonIntLoc = "";
            if (ProcessRunning != null && !string.IsNullOrEmpty(ScriptName))
            {
                if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS)
                {
                    if (!string.IsNullOrEmpty(ScriptIden.RemoteServerNames))
                    {
                        string[] arrServerName = ScriptIden.RemoteServerNames.Split(',');
                        ExecuteCommonScript objExecuteIapd = new ExecuteCommonScript();
                        consolidatedOutput = objExecuteIapd.ExecuteRemoteScriptIapd(ParameterString, arrServerName, RemoteScriptPath, ScriptIden, PythonMethodName);

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
                    #region Code with New Approach
                    /*output = new ExecutionResult();
                    string pyFileName = Path.GetFileNameWithoutExtension(ScriptName).ToLower();
                    List<Stream> lstStream = Operations.ExtractResourceFile(FileContentStream, ".py");
                    //Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                    //extracting dll files from stream and load assemblies in Appdomain
                    Operations.ExtractResourceFile(FileContentStream, ".dll").ForEach(resource => 
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                resource.CopyTo(ms);
                                AppDomain.CurrentDomain.Load(ms.ToArray());
                            }
                        });

                    //Assembly[] assemblies1 = AppDomain.CurrentDomain.GetAssemblies(); 

                    Stream fileStream = Operations.ExtractFile(FileContentStream, string.Format(@"\{0}.py", pyFileName));

                    if (fileStream == null)
                        throw new Exception("Expected " + pyFileName + ".py file not found in the package provided.");

                    FileContent = (new StreamReader(fileStream)).ReadToEnd().ToString();
                    string command = "";
                    if (string.IsNullOrEmpty(PythonMethodName))
                        command = pythonIntLoc + " \"" + ScriptName + "\" " + ParameterString;
                    else
                    {
                        ParameterString = ParameterString.TrimEnd(',');
                        ScriptName = ScriptName.Substring(0, ScriptName.Length - 3);
                        command = "\"" + pythonIntLoc + "\"" + " -c " + "\"" + "import " + ScriptName + ";" + ScriptName + "." + PythonMethodName + "(" + ParameterString + ")" + "\"";
                    }

                    output.InputCommand = command;

                    ProcessRunning.StandardInput.Close(); //close the default cmd console started in the base as it is not needed for python

                    if (!string.IsNullOrEmpty(PythonMethodName))
                    {
                        ipyEngine = Python.CreateEngine();
                        ipyRuntime = ipyEngine.Runtime;
                        ipyScope = ipyEngine.CreateScope();

                        //first compile the complete script
                        //CompileCodeAndExecute(FileContent);
                        ScriptSource source = ipyEngine.CreateScriptSourceFromString(FileContent, SourceCodeKind.Statements);
                        source.Execute(ipyScope);

                        //call the method name provided
                        var streamOut = new MemoryStream();
                        var streamErr = new MemoryStream();
                        ipyEngine.Runtime.IO.SetOutput(streamOut, Encoding.Default);
                        ipyEngine.Runtime.IO.SetErrorOutput(streamErr, Encoding.Default);
                        //ipyEngine.Operations.Invoke(method(ParameterString));
                        source = ipyEngine.CreateScriptSourceFromString(PythonMethodName + "(" + ParameterString + ")", SourceCodeKind.Statements);
                        source.Execute(ipyScope);
                        output.SuccessMessage = Encoding.Default.GetString(streamOut.ToArray());
                        output.ErrorMessage = Encoding.Default.GetString(streamErr.ToArray());

                        if (string.IsNullOrEmpty(output.ErrorMessage))
                            output.IsSuccess = true;
                        else
                            output.IsSuccess = false;

                        consolidatedOutput.Add(output);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(ParameterString))
                        {
                            IDictionary<string, object> argoptions = new Dictionary<string, object>();
                            //the parameter string in thic case wud be separated by space. and each value is ion double quotes whihc may have space inside
                            var regex = new Regex(@"(""((\\"")|([^""]))*"")|('((\\')|([^']))*')|(\S+)");
                            var matchFound = regex.Matches(ParameterString);
                            List<string> parameters = new List<string>();
                            foreach (Match match in matchFound)
                            {
                                parameters.Add(match.Value);
                            }
                            argoptions["Arguments"] = parameters.ToArray();
                            ipyEngine = Python.CreateEngine(argoptions);
                        }
                        else
                            ipyEngine = Python.CreateEngine();

                        ipyRuntime = ipyEngine.Runtime;
                        ipyScope = ipyEngine.CreateScope();

                        var streamOut = new MemoryStream();
                        var streamErr = new MemoryStream();
                        ipyEngine.Runtime.IO.SetOutput(streamOut, Encoding.Default);
                        ipyEngine.Runtime.IO.SetErrorOutput(streamErr, Encoding.Default);

                        ScriptSource source = ipyEngine.CreateScriptSourceFromString(FileContent, SourceCodeKind.Statements);
                        source.Execute(ipyScope);

                        output.SuccessMessage = Encoding.Default.GetString(streamOut.ToArray());
                        output.ErrorMessage = Encoding.Default.GetString(streamErr.ToArray());

                        if (string.IsNullOrEmpty(output.ErrorMessage))
                            output.IsSuccess = true;
                        else
                            output.IsSuccess = false;

                        consolidatedOutput.Add(output);
                    }*/
                    #endregion
                                        
                    string iapdInterpreter = ConfigurationManager.AppSettings["IapdInterpreter"];
                    if (string.IsNullOrEmpty(iapdInterpreter))
                        iapdInterpreter = Path.Combine(GetAppPath(), "iapd.exe");
                    output = new ExecutionResult();
                    //if (!string.IsNullOrEmpty(iapdInterpreter))
                    if (File.Exists(iapdInterpreter))
                    {
                        string command = "";
                        if (string.IsNullOrEmpty(PythonMethodName))
                        {
                            if (string.IsNullOrEmpty(ParameterString))
                                command = iapdInterpreter + " iapd:\"" + ScriptName + "\"";
                            else
                                command = iapdInterpreter + " iapd:\"" + ScriptName + "\" parameters:\"" + ParameterString + "\"";
                    }
                        else
                        {
                            command = iapdInterpreter + " iapd:\"" + ScriptName + "\" function:\"" + PythonMethodName + "(" + ParameterString + ")" + "\"";
                        }
                        output.InputCommand = command;
                        ProcessRunning.StandardInput.WriteLine(command);
                        ProcessRunning.StandardInput.Close();
                       
                        //to handle long outout/error stream
                        //var outTask = ProcessRunning.StandardOutput.ReadToEndAsync();
                        //var errTask = ProcessRunning.StandardError.ReadToEndAsync();

                        ProcessRunning.WaitForExit();

                        output.SuccessMessage = GetOnlySubString(outTask, command);
                        output.ErrorMessage = errTask;
                        if (string.IsNullOrEmpty(output.ErrorMessage))
                            output.IsSuccess = true;
                        else
                            output.IsSuccess = false;
                    }
                    else
                    {
                        output.IsSuccess = false;
                        output.ErrorMessage = "IAPD Interpreter Location (IapdInterpreter) is not configured or not present.";
                    }
                    consolidatedOutput.Add(output);                   
                }
            }
            return consolidatedOutput;
        }

        public override void ProcessRunning_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            errTask += e.Data + Environment.NewLine;
            Console.WriteLine(e.Data);
        }

        public override void ProcessRunning_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            outTask += e.Data + Environment.NewLine;
            Console.WriteLine(e.Data);
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
        /// Gets the curent executable's execution location
        /// </summary>
        /// <returns>execution path</returns>
        private static string GetAppPath()
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
}
