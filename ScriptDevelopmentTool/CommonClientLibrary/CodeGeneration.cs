/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using CodeGenerationEngine.Model;
using Infosys.IAP.CommonClientLibrary.Models;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.IAP.CommonClientLibrary
{
    public class CodeGeneration
    {
        static ProgressStatus progress = new ProgressStatus();
        //public static void CreatePlaybackScript(string UseCasesFilePath, out ContentMeta metaData, out Stream fileContent, out string scriptPath)
        public static PackageMeta CreatePlaybackScript(string UseCasesFilePath)
        {
            progress = new ProgressStatus();

            ContentMeta metaData = null;
            Stream fileContent = null;
            string scriptPath = null;

            if (!string.IsNullOrEmpty(UseCasesFilePath))
            {
                if (Path.GetExtension(UseCasesFilePath).EndsWith("atrwb",StringComparison.InvariantCultureIgnoreCase))
                {
                    progress.Show();
                    try
                    {
                        CodeGenerator.atrwbPath = UseCasesFilePath;
                        CodeGenerator.SendExecutionStatus += CodeGenerator_SendExecutionStatus;
                        CodeGenerator.GenerateCode();

                        if (!string.IsNullOrEmpty(CodeGenerator.outputPath))
                        {
                            scriptPath = Path.Combine(CodeGenerator.outputPath, "iap_controller.py");
                            fileContent = new MemoryStream(File.ReadAllBytes(scriptPath));

                            metaData = new ContentMeta()
                            {
                                Name = CodeGenerator.UseCaseName,
                                ContentType = "iapd",
                                TaskType = "File",
                                ModuleType = ModuleType.Script,
                                ModuleLocation = CodeGenerator.outputPath,
                                Parameters = ReadParameterValues(scriptPath).Parameters,
                                isGeneratedScript = true
                            };
                        }

                        progress.Value = 100;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error While Generating Code. Please try again later..");
                    }
                    finally
                    {
                        progress.Hide();
                        progress.Dispose();
                    }
                }
                else 
                {
                    metaData = ReadParameterValues(UseCasesFilePath);
                    scriptPath = Path.Combine(UseCasesFilePath);
                    fileContent = new MemoryStream(File.ReadAllBytes(UseCasesFilePath));
                }                
            }

            return new PackageMeta(){Content = metaData,FileStream = fileContent,PackagePath = scriptPath};
        }

        private static void CodeGenerator_SendExecutionStatus(CodeGenerator.SendExecutionStatusArgs e)
        {
            progress.Value = e.PercentComplete;
        }

        public static ContentMeta ReadParameterValues(string filepath)
        {
            var folder = Path.GetDirectoryName(filepath);
            var targerFile = Path.Combine(folder, "iap_parameters.py");
            string IronPythonLib = Path.Combine(ConfigurationManager.AppSettings["IronPython"],"Lib");
            ContentMeta metaData = new ContentMeta();
            List<ContentParameter> contentParameter = new List<ContentParameter>();

            var pySrc = File.ReadAllText(targerFile);
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();

            ICollection<string> paths = engine.GetSearchPaths();
            paths.Add(IronPythonLib);
            paths.Add(folder);
            engine.SetSearchPaths(paths);
            engine.Execute(pySrc, scope);

            // get function and dynamically invoke
            var GetArguments = scope.GetVariable("GetArguments");
            IronPython.Runtime.PythonDictionary d = GetArguments();
            List<object> lst = d.Keys.ToList();

            if (lst.Count() > 0)
            {
                lst.ForEach(param =>
                           {
                               contentParameter.Add(new ContentParameter() { Name = Convert.ToString(param), IOType = ParameterIOTypes.In, DefaultValue = d.get(param).ToString()});
                           });
            }
            metaData = new ContentMeta()
            {
                Name = Path.GetFileNameWithoutExtension(filepath),
                ContentType = "iapd",
                TaskType = "File",
                ModuleType = ModuleType.Script,
                ModuleLocation = folder,
                Parameters = contentParameter,
                isGeneratedScript = true
            };
            return metaData;
        }

        public static PlaybackResult ReplayScript(string UseCasesFilePath)
        {
            string extractionLoc = Path.GetFileNameWithoutExtension(UseCasesFilePath);

            PackageMeta metaData = CreatePlaybackScript(UseCasesFilePath);
            PlaybackResult exeResult = null;
            if (!string.IsNullOrEmpty(metaData.PackagePath))
            {
                string sourFolder = Path.GetDirectoryName(metaData.PackagePath);

                if (!Directory.Exists(extractionLoc))
                    Directory.CreateDirectory(extractionLoc);
               
                foreach (string newPath in Directory.GetFiles(sourFolder, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourFolder, extractionLoc), true);

                string command = "\"iap_controller.py\"";

                //get the python interpreter location from appsetting
                string pythonInter = "";
                if (Is64BitOS())
                    pythonInter = ConfigurationManager.AppSettings["PythonInterpreterRemoteLoc64"];
                else
                    pythonInter = ConfigurationManager.AppSettings["PythonInterpreterRemoteLoc32"];

                if (!string.IsNullOrEmpty(command))
                {
                    exeResult = Execute(pythonInter, command, Path.GetFullPath(extractionLoc));

                    //then write the success or error message to the respective standard streams i.e. output or error
                    if (exeResult.IsSuccess)
                        Console.WriteLine(exeResult.SuccessMessage);
                    else
                        Console.Error.WriteLine(exeResult.ErrorMessage);
                }
            }

            System.Threading.Thread.Sleep(2000);
            //then delete the umpackaged folder            
           return exeResult;
        }
        private static bool Is64BitOS()
        {
            bool is64Bit = Environment.Is64BitOperatingSystem;
            if (is64Bit)
                Console.WriteLine("64 Bit OS");
            else
                Console.WriteLine("32 Bit OS");
            return is64Bit;
        }
        private static PlaybackResult Execute(string pythonInter, string commandArgString, string pyFileLocation)
        {
            PlaybackResult result = new PlaybackResult();
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe");
         
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardInput = true;

            using (Process processRunning = Process.Start(processInfo))
            {                
                processRunning.StandardInput.WriteLine(pyFileLocation.Substring(0, 2));
                processRunning.StandardInput.WriteLine("cd " + pyFileLocation);

                processRunning.StandardInput.WriteLine("\"" + pythonInter + "\"" + " " + commandArgString);
                processRunning.StandardInput.Close();

                //to handle long outout/error stream
                var outTask = processRunning.StandardOutput.ReadToEnd();
                var errTask = processRunning.StandardError.ReadToEnd();
                processRunning.WaitForExit();

                result.SuccessMessage = outTask;
                result.SuccessMessage = GetOnlySubString(result.SuccessMessage, pythonInter + " " + commandArgString);
                result.SuccessMessage = result.SuccessMessage.Replace(pyFileLocation + ">", "");
                result.ErrorMessage = errTask;
            }

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                result.IsSuccess = false;
            else
                result.IsSuccess = true;

            return result;
        }

        private static string GetOnlySubString(string complete, string afterThis)
        {
            if (!string.IsNullOrEmpty(complete) && !string.IsNullOrEmpty(afterThis))
            {
                complete = complete.Substring(complete.IndexOf(afterThis) + afterThis.Length);
            }
            return complete;
        }
    }
    public class PlaybackResult
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}
