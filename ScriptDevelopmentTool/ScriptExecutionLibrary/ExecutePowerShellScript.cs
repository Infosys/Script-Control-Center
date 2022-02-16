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
using System.Threading;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public class ExecutePowerShellScript : ExecuteBase
    {
        ExecutionResult output = null;
        public override List<ExecutionResult> Start()
        {
            CommonHelp.WriteLog("executing powershell script...");
            List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
            
            if (ProcessRunning != null && !string.IsNullOrEmpty(ScriptName))
            {
                if (ScriptIden.RemoteExecutionMode == ScriptIndentifier.RemoteExecutionHost.PS)
                {

                    //string[] p = ParameterString.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);
                    //List<string> arr = new List<string>();
                    //foreach (string s in p)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(s))
                    //        arr.Add(s);
                    //}
                    // if remote server, split server names by comma
                    if (!string.IsNullOrEmpty(ScriptIden.RemoteServerNames))
                    {
                        string[] arrServerName = ScriptIden.RemoteServerNames.Split(',');
                        // if (arrServerName.Length > 0)
                        {
                            // for (int i = 0; i < arrServerName.Length; i++)
                            {
                                output = new ExecutionResult();
                                // If local server
                                //if (LocalServer(arrServerName[i]))
                                //{
                                //    ProcessRunning.StandardInput.WriteLine("powershell -File \"" + ScriptName + "\" " + ParameterString);
                                //    ProcessRunning.StandardInput.Close();                                
                                //    var outTask = ProcessRunning.StandardOutput.ReadToEndAsync();
                                //    var errTask = ProcessRunning.StandardError.ReadToEndAsync();
                                //    ProcessRunning.WaitForExit();

                                //    output.SuccessMessage = outTask.Result;
                                //    output.ErrorMessage = errTask.Result;

                                //    if (string.IsNullOrEmpty(output.ErrorMessage))
                                //        output.IsSuccess = true;
                                //    else
                                //        output.IsSuccess = false;
                                //}
                                //  else
                                {
                                    ExecuteCommonScript psScript = new ExecuteCommonScript();
                                    consolidatedOutput = psScript.ExecuteRemoteScriptPS(ScriptName, RemoteScriptPath, PSRemoteParameters, arrServerName, FilePath, ScriptIden, FileContent);
                                    //PSRNP
                                    //consolidatedOutput = psScript.ExecuteRemoteScriptPS(RemoteScriptPath, PSRemoteParameters, arrServerName, FilePath, ScriptIden);
                                }

                                // consolidatedOutput.Add(output);
                            }
                        }
                        ProcessRunning.StandardInput.Close();                        
                        return consolidatedOutput;
                    }
                    else
                    {
                        throw new Exception("Invalid or no Remote server name(s) provided.");
                    }
                }
                else
                {
                    CommonHelp.WriteLog("executing powershell script in local");
                    output = new ExecutionResult();
                    string command;
                    //string command = "powershell -Command \"& '" + ScriptName + "'\" " + ParameterString;
                    //output.InputCommand = command;
                    //pass the Powershell file name to be executed

                    //ProcessRunning.StandardInput.WriteLine(command);
                    ////ProcessRunning.StandardInput.WriteLine("powershell -Command \"" + ScriptName + "\" " + p1 + " " + "$true" + " " + p3);
                    //ProcessRunning.StandardInput.Close();
                    ////to handle long outout/error stream
                    //var outTask = ProcessRunning.StandardOutput.ReadToEndAsync();
                    //var errTask = ProcessRunning.StandardError.ReadToEndAsync();
                    //ProcessRunning.WaitForExit();

                    //output.SuccessMessage = GetOnlySubString(outTask.Result, command);
                    //output.ErrorMessage = errTask.Result;

                    //if (string.IsNullOrEmpty(output.ErrorMessage))
                    //    output.IsSuccess = true;
                    //else
                    //    output.IsSuccess = false;

                    //close the earlier cmd process and run the powershell process 
                    //so that the entire script can be provided as command line argument
                    ProcessRunning.StandardInput.Close();

                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.WorkingDirectory = WorkingDir;
                    processStartInfo.UseShellExecute = false;
                    processStartInfo.RedirectStandardError = true;
                    processStartInfo.RedirectStandardOutput = true;
                    processStartInfo.FileName = "powershell.exe";

                    if (!string.IsNullOrEmpty(ParameterString))
                    {
                        if (ParameterString.EndsWith(",")) //i.e. un-named indirectly
                        {
                            //remove trailing ","
                            ParameterString = ParameterString.Substring(0, ParameterString.Length - 1);
                            command = "invoke-command -ScriptBlock { " + FileContent + "}" + " -ArgumentList " + ParameterString;                            
                        }
                        else
                            command = "invoke-command -ScriptBlock { &{ " + FileContent + "} " + ParameterString + "}";
                        
                    }
                    else
                    {
                        command = "invoke-command -ScriptBlock { " + FileContent + "}";
                    }
                    output.InputCommand = command;
                    //processStartInfo.Arguments = command;
                    //to escape any character which are treated as special by PowerShell command line parser
                    processStartInfo.Arguments = "-Command \"" + Regex.Replace(command, @"""|\\(?=\\*"")", @"\$&") + "\"";
                    Process powerShellProc = new Process();
                    powerShellProc.StartInfo = processStartInfo;
                    CommonHelp.WriteLog("starting powershell process");
                    powerShellProc.Start();
                    CommonHelp.WriteLog("started powershell process");

                    powerShellProc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(PowerShellProc_OutputDataReceived);
                    powerShellProc.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(PowerShellProc_ErrorDataReceived);
                    powerShellProc.BeginOutputReadLine();

                    //output.SuccessMessage = powerShellProc.StandardOutput.ReadToEndAsync().Result;
                    CommonHelp.WriteLog("collected success message, if any");
                    //output.ErrorMessage = powerShellProc.StandardError.ReadToEndAsync().Result;
                    CommonHelp.WriteLog("collected error message, if any");
                    CommonHelp.WriteLog("waiting for process to exit");
                    powerShellProc.WaitForExit();
                    CommonHelp.WriteLog("process exited");

                    //while (!powerShellProc.HasExited)
                    //{
                    //    string stdop, errop;
                    //    while (!string.IsNullOrEmpty(stdop = powerShellProc.StandardOutput.ReadLine()))
                    //        output.SuccessMessage += stdop;
                    //    while (!string.IsNullOrEmpty(errop = powerShellProc.StandardError.ReadLine()))
                    //        output.ErrorMessage += errop;
                    //    Thread.Sleep(200);
                    //}
                    
                    if (string.IsNullOrEmpty(output.ErrorMessage))
                    {
                        output.IsSuccess = true;
                        CommonHelp.WriteLog("success output- " + output.SuccessMessage);
                    }
                    else
                    {
                        output.IsSuccess = false;
                        CommonHelp.WriteLog("error output- " + output.ErrorMessage);
                    }
                }
            }
            consolidatedOutput.Add(output);
            return consolidatedOutput;
        }

        private void PowerShellProc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
             output.ErrorMessage += e.Data + Environment.NewLine;
            Console.WriteLine(e.Data);
        }

        private void PowerShellProc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            output.SuccessMessage += e.Data + Environment.NewLine;
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
