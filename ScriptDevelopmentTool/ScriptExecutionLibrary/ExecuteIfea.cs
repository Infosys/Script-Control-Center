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
using System.Configuration;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public class ExecuteIfea : ExecuteBase
    {
        string outTask = string.Empty;
        string errTask = string.Empty;
        public override List<ExecutionResult> Start()
        {
            List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();
            ExecutionResult output = new ExecutionResult();

            string ifeaRuntimePath = ConfigurationManager.AppSettings["IfeaRuntimePath"];
            if (!string.IsNullOrEmpty(ifeaRuntimePath))
            {
                //change the working directory to the location of the py file
                ProcessRunning.StandardInput.WriteLine(GetWorkingDrive(ifeaRuntimePath)); //to handle the scenario if the default executio location is different than the ifean runtime folder
                ProcessRunning.StandardInput.WriteLine("cd " + Path.GetDirectoryName(ifeaRuntimePath));

                string paramIFEAScriptFileName = " -s \"" + IfeaScriptName + "\"";
                if (!string.IsNullOrEmpty(ParameterString))
                    ParameterString = "-r \"" + WorkingDir + "\\" + ScriptName + "\"" + paramIFEAScriptFileName + " --args " + ParameterString;
                else
                    ParameterString = "-r \"" + WorkingDir + "\\" + ScriptName + "\"" + paramIFEAScriptFileName;
                ProcessRunning.StandardInput.WriteLine(ifeaRuntimePath + " " + ParameterString);
                ProcessRunning.StandardInput.Close();               

                //output.SuccessMessage = ProcessRunning.StandardOutput.ReadToEnd();
                //output.ErrorMessage = ProcessRunning.StandardError.ReadToEnd();
                //to handle long outout/error stream
                //var outTask = ProcessRunning.StandardOutput.ReadToEndAsync();
                //var errTask = ProcessRunning.StandardError.ReadToEndAsync();
                
                ProcessRunning.WaitForExit();
                
                output.SuccessMessage = outTask;                
                output.ErrorMessage = errTask;               

                string command = ifeaRuntimePath + " " + ParameterString;
                output.InputCommand = command;
                output.SuccessMessage = GetOnlySubString(outTask, command);
                
                if (string.IsNullOrEmpty(output.ErrorMessage))
                    output.IsSuccess = true;
                else
                    output.IsSuccess = false;
                
            }
            else
            {
                output.IsSuccess = false;
                output.ErrorMessage = "Ifea runtime path (IfeaRuntimePath) is not configured";
            }
            consolidatedOutput.Add(output);
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
    }
}
