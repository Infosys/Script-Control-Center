/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Text.RegularExpressions;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public class ExecutePython : ExecuteBase
    {
        ScriptEngine ipyEngine = null;
        ScriptScope ipyScope = null;
        ScriptRuntime ipyRuntime = null;
        public override List<ExecutionResult> Start()
        {
            List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
            List<ExecutionResult> consolidatedOutput32 = new List<ExecutionResult>();
            List<ExecutionResult> consolidatedOutput64 = new List<ExecutionResult>();
            //List<string> lstServerName32 = new List<string>();
            //List<string> lstServerName64 = new List<string>();

            ExecutionResult output = null;
            string pythonIntLoc = "";
            if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS)
            {
                if (!string.IsNullOrEmpty(ScriptIden.RemoteServerNames))
                {
                    string[] arrServerName = ScriptIden.RemoteServerNames.Split(',');

                    #region old code to handle 32 and 64 bits python interpretor
                    //if (arrServerName.Length > 0)
                    //{
                    //    for (int i = 0; i < arrServerName.Length; i++)
                    //    {
                    //        try
                    //        {
                    //            if (CheckIfOSIs64Bit(arrServerName[i], ScriptIden))
                    //                lstServerName64.Add(arrServerName[i]);
                    //            else
                    //                lstServerName32.Add(arrServerName[i]);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            output = new ExecutionResult();
                    //            output.IsSuccess = false;
                    //            string err = ex.Message;
                    //            if (ex.InnerException != null)
                    //                err = err + ". \nInner Exception- " + ex.InnerException.Message;
                    //            output.ErrorMessage = err;
                    //            output.ComputerName = arrServerName[i];
                    //            consolidatedOutput.Add(output);
                    //        }
                    //    }
                    //}
                    //// if (CheckIfOSIs64Bit(arrServerName[0], ScriptIden))
                    //string pythonIntLoc64 = ConfigurationManager.AppSettings["PythonInterpreterRemoteLoc64"];
                    //string pythonIntLoc32 = ConfigurationManager.AppSettings["PythonInterpreterRemoteLoc32"];


                    //if (lstServerName32 != null && lstServerName32.Count > 0)
                    //{
                    //    ExecuteCommonScript objExecutePy32 = new ExecuteCommonScript();
                    //    consolidatedOutput32 = objExecutePy32.ExecuteRemoteScriptNonPS(pythonIntLoc32 + " \"", ParameterString, lstServerName32.ToArray(), RemoteScriptPath, ScriptIden);
                    //}
                    //if (lstServerName64 != null && lstServerName64.Count > 0)
                    //{
                    //    ExecuteCommonScript objExecutePy64 = new ExecuteCommonScript();
                    //    consolidatedOutput64 = objExecutePy64.ExecuteRemoteScriptNonPS(pythonIntLoc64 + " \"", ParameterString, lstServerName64.ToArray(), RemoteScriptPath, ScriptIden);
                    //}

                    #endregion

                    ExecuteCommonScript objExecutePy = new ExecuteCommonScript();
                    consolidatedOutput64 = objExecutePy.ExecuteRemoteScriptPython(ParameterString, arrServerName, RemoteScriptPath, ScriptIden,PythonMethodName);

                    foreach (ExecutionResult result in consolidatedOutput32)
                    {
                        consolidatedOutput.Add(result);
                    }

                    foreach (ExecutionResult result in consolidatedOutput64)
                    {
                        consolidatedOutput.Add(result);
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
                //pythonIntLoc = ConfigurationManager.AppSettings["PythonInterpreterLoc"];
                output = new ExecutionResult();

                //if (string.IsNullOrEmpty(pythonIntLoc))
                //{
                    //output.IsSuccess = false;
                    //output.ErrorMessage = "Python Interpreter Location (PythonInterpreterLoc) is not configured";
                //}
                //else
                //{

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

                        var outputWr = new EventRaisingStreamWriter(streamOut);
                        var outputErrWr = new EventRaisingStreamWriter(streamErr);
                        outputWr.StringWritten += new EventHandler<CustomEvtArgs<string>>(output_StringWritten);
                        outputErrWr.StringWritten += new EventHandler<CustomEvtArgs<string>>(output_StringWritten);
                        
                        ipyEngine.Runtime.IO.SetOutput(streamOut, outputWr);
                        ipyEngine.Runtime.IO.SetErrorOutput(streamErr, outputErrWr);

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
                        var outputWr = new EventRaisingStreamWriter(streamOut);
                        var outputErrWr = new EventRaisingStreamWriter(streamErr);
                        outputWr.StringWritten += new EventHandler<CustomEvtArgs<string>>(output_StringWritten);
                        outputErrWr.StringWritten += new EventHandler<CustomEvtArgs<string>>(output_StringWritten);

                        ipyEngine.Runtime.IO.SetOutput(streamOut, outputWr);
                        ipyEngine.Runtime.IO.SetErrorOutput(streamErr, outputErrWr);

                        ScriptSource source = ipyEngine.CreateScriptSourceFromString(FileContent, SourceCodeKind.Statements);
                        source.Execute(ipyScope);

                        output.SuccessMessage = Encoding.Default.GetString(streamOut.ToArray());
                        output.ErrorMessage = Encoding.Default.GetString(streamErr.ToArray());

                        if (string.IsNullOrEmpty(output.ErrorMessage))
                            output.IsSuccess = true;
                        else
                            output.IsSuccess = false;

                        consolidatedOutput.Add(output);
                    }
                //}
            }
            return consolidatedOutput;
        }

        private void output_StringWritten(object sender, CustomEvtArgs<string> e)
        {
           Console.WriteLine(e.Value); 
        }       

        /// <summary>
        /// This function is used to check if the current/remote machine is 32 bit or 64 bit. 
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        //private bool CheckIfOSIs64Bit(string serverName, ScriptIndentifier scriptIden)
        //{
        //    bool result = false;

        //    using (Runspace runSpace = RunspaceFactory.CreateRunspace())
        //    {
        //        runSpace.Open();
        //        PowerShell ps = PowerShell.Create();
        //        ps.Runspace = runSpace;
        //        ps.AddCommand("Get-WmiObject");
        //        ps.AddParameter("class", "Win32_OperatingSystem");
        //        ps.AddParameter("ComputerName", serverName);

        //        if (!string.IsNullOrEmpty(scriptIden.UserName) && (scriptIden.Password != null))
        //        {
        //            PSCredential credential = new PSCredential(scriptIden.UserName, scriptIden.Password);
        //            ps.AddParameter("Credential", credential);
        //        }

        //        Collection<PSObject> psOutput = ps.Invoke();
        //        if (ps.Streams.Error.Count == 0)
        //        {
        //            if (psOutput != null && psOutput.Count == 1)
        //            {
        //                string strOSArchValue = psOutput[0].Members["OSArchitecture"].Value.ToString();
        //                if (!string.IsNullOrEmpty(strOSArchValue) && strOSArchValue.Contains("64"))
        //                {
        //                    result = true;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        /// <summary>
        /// This method is used to check if current server is local server.
        /// </summary>
        /// <param name="serverName">name of server</param>
        /// <returns>true if server is local</returns>
        //private static bool LocalServer(string serverName)
        //{
        //    bool result = false;

        //    if (serverName.ToLower().Equals("localhost") || serverName.ToLower().Equals(Environment.MachineName.ToLower()) || serverName.Equals("."))
        //        result = true;

        //    return result;
        //}
    }

    public class CustomEvtArgs<T> : EventArgs 
    {
        public T Value
        {
            get;
            private set;
        }
        public CustomEvtArgs(T value)
        {
            this.Value = value;
        }
    }

    public class EventRaisingStreamWriter : StreamWriter
    {
        #region Event
        public event EventHandler<CustomEvtArgs<string>> StringWritten;
        #endregion

        #region Constructor
        public EventRaisingStreamWriter(Stream s)
            : base(s)
        { }
        #endregion

        #region Private Methods
        private void LaunchEvent(string txtWritten)
        {
            if (StringWritten != null)
            {
                StringWritten(this, new CustomEvtArgs<string>(txtWritten));
            }
        }
        #endregion

        #region Overrides

        public override void Write(string value)
        {
            base.Write(value);
            LaunchEvent(value);
        }
        public override void Write(bool value)
        {
            base.Write(value);
            LaunchEvent(value.ToString());
        }        

        #endregion
    }
}
