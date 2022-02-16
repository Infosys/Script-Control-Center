/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public class ExecuteJScript : ExecuteBase
    {
        string outTask = string.Empty;
        string errTask = string.Empty;
        public override List<ExecutionResult> Start()
        {
            List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
            ExecutionResult output = null;
            if (ProcessRunning != null && !string.IsNullOrEmpty(ScriptName))
            {
                if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS)
                {
                    if (!string.IsNullOrEmpty(ScriptIden.RemoteServerNames))
                    {
                        string[] arrServerName = ScriptIden.RemoteServerNames.Split(',');
                        ExecuteCommonScript objExecuteJS = new ExecuteCommonScript();
                        consolidatedOutput = objExecuteJS.ExecuteRemoteScriptNonPS("cscript \"", ParameterString, arrServerName, RemoteScriptPath, ScriptIden);
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

                    // return consolidatedOutput;
                }
                else
                {
                    output = new ExecutionResult();
                    string command = "cscript \"" + ScriptName + "\" " + ParameterString;
                    output.InputCommand = command;
                    //pass the js file name to be executed
                    ProcessRunning.StandardInput.WriteLine(command);
                    ProcessRunning.StandardInput.Close();
                    
                    //output.SuccessMessage = ProcessRunning.StandardOutput.ReadToEnd();
                    //output.ErrorMessage = ProcessRunning.StandardError.ReadToEnd();
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
    }
}

