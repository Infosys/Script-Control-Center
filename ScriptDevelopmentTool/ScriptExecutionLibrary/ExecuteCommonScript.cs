/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public class ExecuteCommonScript
    {
        //List<string> sb = new List<string>();
        //List<string> RemoteServerNames = new List<string>();
        Hashtable hsResult = new Hashtable();
        Hashtable hsError = new Hashtable();
        StringBuilder s = null;
        string serverName = "";
        /// <summary>
        /// This method is used to execute PowerShell script on remote machine.
        /// </summary>
        /// <param name="scriptToBeExecuted"></param>
        /// <param name="parameterString"></param>
        /// <param name="serverName"></param>
        /// <param name="filePath"></param>
        /// <param name="scriptIden"></param>
        /// <returns></returns>
        ///
        public List<ExecutionResult> ExecuteRemoteScriptPS(string scriptToBeExecuted, string remoteScript, object[] parameterString, string[] serverName, string filePath, ScriptIndentifier scriptIden, string fileContent)
        {
            List<ExecutionResult> consolidatedResult = new List<ExecutionResult>();
            ExecutionResult output = null;
            StringBuilder result = new StringBuilder();
            s = new StringBuilder();
            string scriptBlock = "";

            //hsResult = new Hashtable();
            //hsError = new Hashtable();
            ExecuteCommonScript script = new ExecuteCommonScript();
            try
            {
                using (Runspace runSpace = RunspaceFactory.CreateRunspace())
                {
                    runSpace.Open();
                    using (Pipeline pipeline = runSpace.CreatePipeline())
                    {
                        PowerShell ps = PowerShell.Create();
                        ps.Runspace = runSpace;

                        Command cmd = new Command("invoke-command");
                        cmd.Parameters.Add("ComputerName", serverName);

                        //add credential, if provided
                        if (!string.IsNullOrEmpty(scriptIden.UserName) && (scriptIden.Password != null))
                        {
                            PSCredential credential = new PSCredential(scriptIden.UserName, scriptIden.Password);
                            cmd.Parameters.Add("Credential", credential);
                        }

                        //check if nammed or unnamed
                        bool isParamNamed = false;
                        if (scriptIden.Parameters != null && scriptIden.Parameters.Count > 0)
                        {
                            //if any of the parameter is nammed then set isParamNamed = true
                            List<Parameter> pairedParam = scriptIden.Parameters.Where(p => p.IsPaired).ToList();
                            if (pairedParam != null && pairedParam.Count > 0)
                                isParamNamed = true;
                        }

                        if (!isParamNamed)
                        {
                            //if unnamed parameter
                            //cmd.Parameters.Add("FilePath", scriptToBeExecuted);
                            ScriptBlock scriptBlkObj = ScriptBlock.Create(fileContent);
                            cmd.Parameters.Add("ScriptBlock", scriptBlkObj);
                        }
                        else
                        {
                            //if nammed parameter
                            //add authentication
                            AuthenticationMechanism authentication = AuthenticationMechanism.Credssp;
                            cmd.Parameters.Add("Authentication", authentication);

                            //construct the input parameter definition in the script block search for "PSRNP"
                            //following check not needed as if isParamNamed then the following condition is true
                            //if (scriptIden.Parameters != null && scriptIden.Parameters.Count > 0)
                            //{
                            scriptIden.Parameters.ForEach(p =>
                            {
                                if (string.IsNullOrEmpty(scriptBlock))
                                    scriptBlock = " param(" + "$" + p.ParameterName;
                                else
                                    scriptBlock = scriptBlock + ",$" + p.ParameterName;
                            });
                            scriptBlock = scriptBlock + ")" + Environment.NewLine + "& \"" + remoteScript + "\"  @PSBoundParameters";
                            //}
                            //else
                            //    scriptBlock = remoteScript;
                            ScriptBlock scriptBlkObj = ScriptBlock.Create(scriptBlock);
                            cmd.Parameters.Add("ScriptBlock", scriptBlkObj);
                        }
                        cmd.Parameters.Add("ArgumentList", parameterString);

                        ps.Commands.AddCommand(cmd);


                        PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                        outputCollection.DataAdded += outputCollection_DataAdded;
                        ps.Streams.Error.DataAdded += Error_DataAdded;

                        IAsyncResult psOutput = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);

                        psOutput.AsyncWaitHandle.WaitOne();

                        string param = "";
                        if (parameterString != null && parameterString.Length > 0)
                        {
                            foreach (Object p in parameterString)
                            {
                                param = param + p.ToString() + " ";
                            }
                            param = param.Substring(0, param.Length - 1);
                            param = "\"" + param + "\"";
                        }
                        string data = "";
                        string server = string.Join(",", serverName);

                        if (isParamNamed)
                        {
                            if (parameterString != null && parameterString.Length > 0)
                                data = "invoke-command" + " -ComputerName " + server + " -ScriptBlock{ " + scriptBlock + "} -ArgumentList " + param;
                            else
                                data = "invoke-command" + " -ComputerName " + server + " -ScriptBlock{ " + scriptBlock + "}";
                        }
                        else
                        {
                            if (parameterString != null && parameterString.Length > 0)
                                data = "invoke-command" + " -ComputerName " + server + " -FilePath " + scriptToBeExecuted + " -ArgumentList " + param;
                            else
                                data = "invoke-command" + " -ComputerName " + server + " -FilePath " + scriptToBeExecuted;
                        }


                        //if (ps.Streams.Error.Count == 0)
                        //{                       
                        if (hsResult != null && hsResult.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsResult)
                            {
                                output = new ExecutionResult();

                                output.IsSuccess = true;
                                output.InputCommand = data.ToString();
                                output.ComputerName = entry.Key.ToString();
                                output.SuccessMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }

                        if (hsError != null && hsError.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsError)
                            {
                                output = new ExecutionResult();
                                output.IsSuccess = false;
                                output.InputCommand = data.ToString();
                                output.ComputerName = entry.Key.ToString();
                                output.ErrorMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }


                    }
                }
            }

            catch (Exception ex)
            {
                output.IsSuccess = false;
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                output.ErrorMessage = err;
                consolidatedResult.Add(output);
            }
            finally
            {
                hsResult = null;
            }

            return consolidatedResult;

        }

        private void outputCollection_DataAdded(object sender, DataAddedEventArgs e)
        {
            PSDataCollection<PSObject> psDataCollection = (PSDataCollection<PSObject>)sender;
            Collection<PSObject> results = psDataCollection.ReadAll();

            if (results != null && results.Count > 0)
            {
                PSObject result = results[0];
                if (result != null)
                {
                    string computerName = result.Properties["PSComputerName"].Value.ToString();
                    if (hsResult.ContainsKey(computerName))
                    {
                        hsResult[computerName] += result.ToString() + Environment.NewLine;
                    }
                    else
                        hsResult.Add(computerName, result.ToString() + Environment.NewLine);

                    Console.WriteLine(result.ToString());
                }
            }
        }

        private void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            // do something when an error is written to the error stream
            PSDataCollection<System.Management.Automation.ErrorRecord> psDataCollection = (PSDataCollection<System.Management.Automation.ErrorRecord>)sender;
            Collection<System.Management.Automation.ErrorRecord> results = psDataCollection.ReadAll();
            if (results != null && results.Count > 0)
            {
                System.Management.Automation.ErrorRecord result = results[0];
                if (result != null)
                {
                    //PSRNP
                    string computerName = "";
                    if (result.GetType() == typeof(System.Management.Automation.ErrorRecord))
                        computerName = result.ErrorDetails.Message.Substring(1, result.ErrorDetails.Message.IndexOf(']') - 1);
                    else
                        computerName = ((System.Management.Automation.Runspaces.RemotingErrorRecord)result).OriginInfo.PSComputerName;
                    //string computerName = result.TargetObject.ToString();
                    if (hsError.ContainsKey(computerName))
                    {
                        if (result.ErrorDetails != null)
                            hsError[computerName] += result.ErrorDetails.Message + Environment.NewLine;
                        else
                            hsError[computerName] += result.Exception.Message + Environment.NewLine;
                    }
                    else
                    {
                        if (result.ErrorDetails != null)
                            hsError.Add(computerName, result.ErrorDetails.Message + Environment.NewLine);
                        else
                            hsError.Add(computerName, result.Exception.Message + Environment.NewLine);
                    }
                    Console.WriteLine(result.ToString());
                }
            }
        }

        private void NonPSError_DataAdded(object sender, DataAddedEventArgs e)
        {
            // do something when an error is written to the error stream
            PSDataCollection<System.Management.Automation.ErrorRecord> psDataCollection = (PSDataCollection<System.Management.Automation.ErrorRecord>)sender;
            Collection<System.Management.Automation.ErrorRecord> results = psDataCollection.ReadAll();
            if (results != null && results.Count > 0)
            {
                System.Management.Automation.ErrorRecord result = results[0];
                //   (((System.Management.Automation.RemoteRunspace)(result.TargetObject)).ConnectionInfo).ComputerName
                if (result != null && result.ErrorDetails != null)
                {
                    string computerName = result.ErrorDetails.Message.Substring(1, result.ErrorDetails.Message.IndexOf(']') - 1);
                    if (hsError.ContainsKey(computerName))
                    {
                        hsError[computerName] += result.ErrorDetails.Message + Environment.NewLine;
                    }
                    else
                        hsError.Add(computerName, result.ErrorDetails.Message + Environment.NewLine);

                    Console.WriteLine(result.ToString());
                }
            }
        }

        /// <summary>
        /// This method is used to execute commands on remote machine.
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="parameterString"></param>
        /// <param name="remoteServerName"></param>
        /// <param name="scriptIden"></param>
        /// <returns></returns>
        public List<ExecutionResult> ExecuteRemoteCommand(string commandName, string parameterString, string[] remoteServerName, ScriptIndentifier scriptIden)
        {
            List<ExecutionResult> consolidatedResult = new List<ExecutionResult>();
            ExecutionResult output = new ExecutionResult();
            StringBuilder result = new StringBuilder();
            try
            {
                using (Runspace runSpace = RunspaceFactory.CreateRunspace())
                {
                    runSpace.Open();
                    using (Pipeline pipeline = runSpace.CreatePipeline())
                    {

                        RunspaceInvoke invoke = new RunspaceInvoke();
                        PowerShell ps = PowerShell.Create();
                        ps.Runspace = runSpace;
                        ps.AddCommand("invoke-command");
                        ps.AddParameter("Computer", remoteServerName);
                        if (!string.IsNullOrEmpty(scriptIden.UserName) && (scriptIden.Password != null))
                        {
                            PSCredential credential = new PSCredential(scriptIden.UserName, scriptIden.Password);
                            ps.AddParameter("Credential", credential);
                        }

                        ScriptBlock sb;
                        sb = ScriptBlock.Create(commandName);
                        //// if (string.IsNullOrEmpty(commandName))
                        //{
                        //    if (string.IsNullOrEmpty(parameterString))
                        //        sb = ScriptBlock.Create(commandName);
                        //    else
                        //        sb = ScriptBlock.Create(commandName + " " + parameterString);
                        //}

                        ps.AddParameter("ScriptBlock", sb);

                        if (!string.IsNullOrEmpty(parameterString))
                        {
                            ps.AddParameter("ArgumentList", parameterString);
                        }

                        PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();

                        outputCollection.DataAdded += outputCollection_DataAdded;

                        ps.Streams.Error.DataAdded += Error_DataAdded;

                        IAsyncResult psOutput = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
                        psOutput.AsyncWaitHandle.WaitOne();

                        string server = string.Join(",", remoteServerName);
                        string input = "invoke-command -Computer " + server + " -scriptblock { " + commandName + " }";
                        if (!string.IsNullOrEmpty(parameterString))
                            input = input + " -ArgumentList " + parameterString;                        

                        if (hsResult != null && hsResult.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsResult)
                            {
                                output = new ExecutionResult();

                                output.IsSuccess = true;
                                output.InputCommand = input;
                                output.ComputerName = entry.Key.ToString();
                                output.SuccessMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }

                        if (hsError != null && hsError.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsError)
                            {
                                output = new ExecutionResult();
                                output.IsSuccess = false;
                                output.InputCommand = input;
                                output.ComputerName = entry.Key.ToString();
                                output.ErrorMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                output.ErrorMessage = err;
                for (int i = 0; i < remoteServerName.Length; i++)
                {
                    output.ComputerName = remoteServerName[i];
                }

                consolidatedResult.Add(output);
            }

            return consolidatedResult;
        }

        /// <summary>
        /// This method is used to run Batch,JS,VBS scripts on remote machine.
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="parameterString"></param>
        /// <param name="remoteServerName"></param>
        /// <param name="remoteScriptPath"></param>
        /// <param name="scriptIden"></param>
        /// <returns></returns>
        public List<ExecutionResult> ExecuteRemoteScriptNonPS(string compiler, string parameterString, string[] remoteServerName, string remoteScriptPath, ScriptIndentifier scriptIden)
        {
            List<ExecutionResult> consolidatedResult = new List<ExecutionResult>();
            ExecutionResult output = new ExecutionResult();
            StringBuilder result = new StringBuilder();
            string scriptToBeExecuted = "";
            try
            {
                using (Runspace runSpace = RunspaceFactory.CreateRunspace())
                {
                    runSpace.Open();
                    using (Pipeline pipeline = runSpace.CreatePipeline())
                    {
                        if (string.IsNullOrEmpty(compiler))
                        {
                            if (string.IsNullOrEmpty(parameterString))
                                scriptToBeExecuted = remoteScriptPath;
                            else
                                scriptToBeExecuted = remoteScriptPath + " " + parameterString;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(parameterString))
                                scriptToBeExecuted = compiler + remoteScriptPath + "\" ";
                            else
                                scriptToBeExecuted = compiler + remoteScriptPath + "\" " + parameterString;
                        }

                        PSCredential credential = new PSCredential(scriptIden.UserName, scriptIden.Password);
                        PowerShell ps = PowerShell.Create();
                        ps.Runspace = runSpace;
                        string server = string.Join(",", remoteServerName);
                        if (remoteServerName.Length > 1)
                        {
                            server = "\"" + server.Replace(",", "\",\"") + "\"";
                        }
                        
                        ps.AddCommand("Set-Variable");
                        ps.AddParameter("Name", "cred");
                        ps.AddParameter("Value", credential);

                        ps.AddScript(@"$s = New-PSSession -ComputerName " + server + " -Credential $cred -Authentication Credssp");
                        ps.AddScript(@"$script = Invoke-Command -Session $s -ScriptBlock { " + scriptToBeExecuted + " }");
                        ps.AddScript(@"Remove-PSSession -Session $s");
                        ps.AddScript(@"echo $script");


                        result.AppendLine("invoke-command" + " -ComputerName " + server + " -ScriptBlock { " + scriptToBeExecuted + " }");


                        PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                        outputCollection.DataAdded += outputCollection_DataAdded;
                        ps.Streams.Error.DataAdded += NonPSError_DataAdded;
                        IAsyncResult psOutput = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
                        psOutput.AsyncWaitHandle.WaitOne();                        

                        if (hsResult != null && hsResult.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsResult)
                            {
                                output = new ExecutionResult();

                                output.IsSuccess = true;
                                output.InputCommand = result.ToString();
                                output.ComputerName = entry.Key.ToString();
                                serverName = entry.Key.ToString();
                                output.SuccessMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }

                        if (hsError != null && hsError.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsError)
                            {
                                output = new ExecutionResult();
                                output.IsSuccess = false;
                                output.InputCommand = result.ToString();
                                output.ComputerName = entry.Key.ToString();
                                output.ErrorMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                output.ErrorMessage = err;
                for (int i = 0; i < remoteServerName.Length; i++)
                {
                    output.ComputerName = remoteServerName[i];
                }
                consolidatedResult.Add(output);
            }
            return consolidatedResult;
        }

        /// <summary>
        /// This method is used to run Python scripts on remote machine.
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="parameterString"></param>
        /// <param name="remoteServerName"></param>
        /// <param name="remoteScriptPath"></param>
        /// <param name="scriptIden"></param>
        /// <returns></returns>
        public List<ExecutionResult> ExecuteRemoteScriptPython(string parameterString, string[] remoteServerName, string remoteScriptPath, ScriptIndentifier scriptIden, string PythonMethodName)
        {
            List<ExecutionResult> consolidatedResult = new List<ExecutionResult>();
            ExecutionResult output = new ExecutionResult();
            StringBuilder result = new StringBuilder();
            string scriptToBeExecuted = "";
            try
            {
                using (Runspace runSpace = RunspaceFactory.CreateRunspace())
                {
                    runSpace.Open();
                    using (Pipeline pipeline = runSpace.CreatePipeline())
                    {
                        string pythonIntLoc64 = System.Configuration.ConfigurationManager.AppSettings["PythonInterpreterRemoteLoc64"];
                        string pythonIntLoc32 = System.Configuration.ConfigurationManager.AppSettings["PythonInterpreterRemoteLoc32"];

                        string compiler = "$os=Get-WMIObject win32_operatingsystem " + Environment.NewLine;
                        compiler = compiler + "if ($os.OSArchitecture -eq \"64-bit\" ){" + Environment.NewLine;
                        compiler = compiler + pythonIntLoc64 + "#fileParam#" + Environment.NewLine;
                        compiler = compiler + "  }" + Environment.NewLine;
                        compiler = compiler + "  else" + Environment.NewLine;
                        compiler = compiler + "  {" + Environment.NewLine;
                        compiler = compiler + pythonIntLoc32 + "#fileParam#" + Environment.NewLine;
                        compiler = compiler + "  }" + Environment.NewLine;

                        if (string.IsNullOrEmpty(PythonMethodName))
                        {
                            if (string.IsNullOrEmpty(parameterString))
                                scriptToBeExecuted = compiler.Replace("#fileParam#", " \"" + remoteScriptPath + "\" ");
                            else
                                scriptToBeExecuted = compiler.Replace("#fileParam#", " \"" + remoteScriptPath + "\" " + parameterString);

                        }
                        else
                        {
                            parameterString = parameterString.TrimEnd(',');
                            string scriptName = Path.GetFileNameWithoutExtension(remoteScriptPath);
                            string filePath = Path.GetDirectoryName(remoteScriptPath);
                            remoteScriptPath = " -c " + " \"" + "import sys; sys.path.append(r'" + filePath + "');" + "import " + scriptName + ";" + scriptName + "." + PythonMethodName + "(" + parameterString + ")" + "\"";
                            scriptToBeExecuted = compiler.Replace("#fileParam#", remoteScriptPath);
                        }
                        

                        PSCredential credential = new PSCredential(scriptIden.UserName, scriptIden.Password);
                        PowerShell ps = PowerShell.Create();
                        ps.Runspace = runSpace;
                        string server = string.Join(",", remoteServerName);
                        if (remoteServerName.Length > 1)
                        {
                            server = "\"" + server.Replace(",", "\",\"") + "\"";
                        }
                        
                        ps.AddCommand("Set-Variable");
                        ps.AddParameter("Name", "cred");
                        ps.AddParameter("Value", credential);

                        ps.AddScript(@"$s = New-PSSession -ComputerName " + server + " -Credential $cred -Authentication Credssp");
                        ps.AddScript(@"$script = Invoke-Command -Session $s -ScriptBlock { " + scriptToBeExecuted + " }");
                        ps.AddScript(@"Remove-PSSession -Session $s");
                        ps.AddScript(@"echo $script");


                        result.AppendLine("invoke-command" + " -ComputerName " + server + " -ScriptBlock { " + scriptToBeExecuted + " }");


                        PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                        outputCollection.DataAdded += outputCollection_DataAdded;
                        ps.Streams.Error.DataAdded += NonPSError_DataAdded;
                        IAsyncResult psOutput = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
                        psOutput.AsyncWaitHandle.WaitOne();

                        
                        if (hsResult != null && hsResult.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsResult)
                            {
                                output = new ExecutionResult();

                                output.IsSuccess = true;
                                output.InputCommand = result.ToString();
                                output.ComputerName = entry.Key.ToString();
                                output.SuccessMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }

                        if (hsError != null && hsError.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsError)
                            {
                                output = new ExecutionResult();
                                output.IsSuccess = false;
                                output.InputCommand = result.ToString();
                                output.ComputerName = entry.Key.ToString();
                                output.ErrorMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                output.ErrorMessage = err;
                consolidatedResult.Add(output);
            }
            return consolidatedResult;
        }

        /// <summary>
        /// This method is used to run Iapd scripts on remote machine.
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="parameterString"></param>
        /// <param name="remoteServerName"></param>
        /// <param name="remoteScriptPath"></param>
        /// <param name="scriptIden"></param>
        /// <returns></returns>
        public List<ExecutionResult> ExecuteRemoteScriptIapd(string parameterString, string[] remoteServerName, string remoteScriptPath, ScriptIndentifier scriptIden, string PythonMethodName)
        {
            List<ExecutionResult> consolidatedResult = new List<ExecutionResult>();
            ExecutionResult output = new ExecutionResult();
            StringBuilder result = new StringBuilder();
            string scriptToBeExecuted = "";
            try
            {
                using (Runspace runSpace = RunspaceFactory.CreateRunspace())
                {
                    runSpace.Open();
                    using (Pipeline pipeline = runSpace.CreatePipeline())
                    {
                        string iapdInterpreter = System.Configuration.ConfigurationManager.AppSettings["IapdInterpreterRemoteLoc"];

                        string sharedFolder = Directory.GetParent(remoteScriptPath).FullName;
                        //string mappDriveCommand = "New-PSDrive –Name \"I\" –PSProvider FileSystem –Root \"" + sharedFolder + "\"";
                        //string removeMapp = "Remove-PSDrive I"; //not needed as once the script block is over, the drive is disconnected
                        string mappDriveCommand = "net use I: \"" + sharedFolder + "\" /persistent:yes";
                        string removeMapp = "net use I: /delete";
                        //once mapped, remote script path is with repect to the drive created
                        remoteScriptPath = "I:\\" + Path.GetFileName(remoteScriptPath);
                        
                        if (string.IsNullOrEmpty(PythonMethodName))
                        {
                            if (string.IsNullOrEmpty(parameterString))
                                scriptToBeExecuted = iapdInterpreter + " iapd:\"" + remoteScriptPath + "\"";
                            else
                                scriptToBeExecuted = iapdInterpreter + " iapd:\"" + remoteScriptPath + "\" parameters:\"" + parameterString + "\"";
                        }
                        else
                        {
                            scriptToBeExecuted = iapdInterpreter + " iapd:\"" + remoteScriptPath + "\" function:\"" + PythonMethodName + "(" + parameterString + ")" + "\"";
                        }

                        //combine with the map/unmap drive command
                        scriptToBeExecuted = mappDriveCommand + Environment.NewLine + scriptToBeExecuted + Environment.NewLine + removeMapp;

                        PSCredential credential = new PSCredential(scriptIden.UserName, scriptIden.Password);
                        PowerShell ps = PowerShell.Create();
                        ps.Runspace = runSpace;
                        string server = string.Join(",", remoteServerName);
                        if (remoteServerName.Length > 1)
                        {
                            server = "\"" + server.Replace(",", "\",\"") + "\"";
                        }
                        
                        ps.AddCommand("Set-Variable");
                        ps.AddParameter("Name", "cred");
                        ps.AddParameter("Value", credential);

                        ps.AddScript(@"$s = New-PSSession -ComputerName " + server + " -Credential $cred -Authentication Credssp");
                        ps.AddScript(@"$script = Invoke-Command -Session $s -ScriptBlock { " + scriptToBeExecuted + " }");
                        ps.AddScript(@"Remove-PSSession -Session $s");
                        ps.AddScript(@"echo $script");


                        result.AppendLine("invoke-command" + " -ComputerName " + server + " -ScriptBlock { " + scriptToBeExecuted + " }");


                        PSDataCollection<PSObject> outputCollection = new PSDataCollection<PSObject>();
                        outputCollection.DataAdded += outputCollection_DataAdded;
                        //outputCollection.Completed += outputCollection_Completed;
                        ps.Streams.Error.DataAdded += Error_DataAdded;
                        IAsyncResult psOutput = ps.BeginInvoke<PSObject, PSObject>(null, outputCollection);
                        

                        //psOutput.AsyncWaitHandle.WaitOne();
                        while (!psOutput.IsCompleted)
                        { }

                        Console.WriteLine("Script Execution Result:" + Environment.NewLine);

                        if (hsResult != null && hsResult.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsResult)
                            {
                                output = new ExecutionResult();

                                output.IsSuccess = true;
                                output.InputCommand = result.ToString();
                                output.ComputerName = entry.Key.ToString();
                                output.SuccessMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }

                        if (hsError != null && hsError.Count > 0)
                        {
                            foreach (DictionaryEntry entry in hsError)
                            {
                                output = new ExecutionResult();
                                output.IsSuccess = false;
                                output.InputCommand = result.ToString();
                                output.ComputerName = entry.Key.ToString();
                                output.ErrorMessage = entry.Value.ToString();                                
                                consolidatedResult.Add(output);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                output.IsSuccess = false;
                string err = ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                output.ErrorMessage = err;
                consolidatedResult.Add(output);
            }
            return consolidatedResult;
        }

        void outputCollection_Completed(object sender, EventArgs e)
        {
            PSDataCollection<PSObject> psDataCollection = (PSDataCollection<PSObject>)sender;
            Collection<PSObject> results = psDataCollection.ReadAll();

            if (results != null && results.Count > 0)
            {
                PSObject result = results[0];
                if (result != null)
                {
                    string computerName = result.Properties["PSComputerName"].Value.ToString();
                    if (hsResult.ContainsKey(computerName))
                    {
                        hsResult[computerName] += result.ToString() + Environment.NewLine;
                    }
                    else
                        hsResult.Add(computerName, result.ToString() + Environment.NewLine);
                }
            }
        }
    }
}
