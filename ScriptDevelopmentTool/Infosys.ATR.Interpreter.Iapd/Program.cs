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

using Infosys.ATR.Packaging;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using Infosys.WEM.SecureHandler;

namespace Infosys.ATR.Interpreter.Iapd
{
    class Program
    {
        static string[] keys = new string[] { "iapd", "function", "parameters"};
        //e.g.
        //iapd:"D:\temp\sample.iapd"
        //function:"PrintFunction('rb')"
        //parameters:"'rb','1','True'"

        static Dictionary<string, string> pairs = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            bool helpAsked = false;      
            if (args != null && args.Length >= 1)
            {
                //IapdInterpreter.exe
                if (args[0].ToLower().Contains("help"))
                {
                    helpAsked = true;
                    Console.WriteLine("");
                    Console.WriteLine("Iapd Interpreter usage...");
                    Console.WriteLine("");
                    Console.WriteLine("Iapd.exe iapd:\"D:\\temp\\sample.iapd\"");
                    Console.WriteLine("Or");
                    Console.WriteLine("Iapd.exe iapd:\"<iapd_package.iapd>\" parmeters:\"'rb','1','True'\"");
                    Console.WriteLine("Or");
                    Console.WriteLine("Iapd.exe iapd:\"<iapd_package.iapd>\" function:\"PrintFunction('rb',1,True)\"");
                    Console.WriteLine("");
                    Console.WriteLine("Here, ");
                    Console.WriteLine("1. 'iapd_package.iapd' is the package having the .py as well as the dependent files and libraries.");
                    Console.WriteLine("2. optional argument- 'parmeters' is the list of the parameters (comma separated) e.g. \"'1','rb','True'\". Note the double quotes has to be provided for any data type or combination of data types, e.g. \"'rb'\" for string, \"'rb','1'\" for string and int.");
                    Console.WriteLine("3. optional argument- 'function' is the function/method to be called from the .py file in the iapd package with name same as the iapd package name. It expects the parameters (comma separated) in their respective data types e.g. 1,'rb',True for int, string, bool.");
                    Console.WriteLine("");                    
                }

                if (helpAsked)
                    return;

                //parse the arguments for the key-value pairs
                ParseArgumentPairs(args);

                string copyrightLabel = Environment.NewLine + "Infosys Automation Platform, Iapd Interpreter" + Environment.NewLine + "(c) Infosys Limited. All rights reserved." + Environment.NewLine;
                Console.WriteLine(copyrightLabel);

                try
                {
                    //unpackage the iapd file - e.g. rb12.iapd
                    string iapdFile = pairs["iapd"].Replace("\"", "");
                    if (File.Exists(iapdFile))
                    {
                        //old- unpackage at the current location of execution
                        //string extractionLoc = GetAppPath();

                        //unpackage at the same location as the iapd file
                        string extractionLoc = Directory.GetParent(iapdFile).FullName;// GetAppPath();

                        Console.WriteLine("IAPD package extraction location: " + extractionLoc );
                        string pyFileName = "";

                        var filecontent = System.IO.File.ReadAllBytes(Path.Combine(extractionLoc , iapdFile)); //to read the entire package
                        //if (Encoding.Unicode.GetString(filecontent.ToArray()).Contains(SecurePayload.keyText))
                        //{
                        //    File.WriteAllBytes(Path.Combine(extractionLoc, iapdFile), SecurePayload.UnSecureBytes(filecontent.ToArray()));                        
                        //}

                        var result = Packaging.Operations.Unpackage(iapdFile, extractionLoc);
                        //at the location of extraction, a folder is created by the same name as the iapd package (rb12.iapd i.e. rb12)
                        //inside this folder there will be one py file by the same name as the iapd file (rb12.iapd i.e. rb12.py)
                        // this py file woudl be run using the python or iron-python interpreter
                        if (!result.IsSuccess)
                            throw new Exception("Unpackaging of the iapd package failed. More Information- " + result.Message);

                        //get the python interpreter location from appsetting
                        string pythonInter = "";
                        if (Is64BitOS())
                            pythonInter = ConfigurationManager.AppSettings["PythonInterpreter64Bit"];
                        else
                            pythonInter = ConfigurationManager.AppSettings["PythonInterpreter32Bit"];
                        if (!string.IsNullOrEmpty(pythonInter) && File.Exists(pythonInter))
                        {
                            Console.WriteLine("Python Interpreter to be used: " + pythonInter);

                            //construct the .py file path
                            pyFileName = Path.GetFileNameWithoutExtension(iapdFile);
                            string pyFilePath = Path.Combine(extractionLoc, pyFileName, pyFileName + ".py");
                            if (!File.Exists(pyFilePath))
                                throw new Exception("Expected " + pyFileName + ".py file not found in the package provided.");

                            //start the python interpreter passing the py file and the entry function, if provided.
                            string command = "\"" + pyFileName + ".py\"";

                            if (pairs.ContainsKey("parameters"))
                                command = command + " " + pairs["parameters"].Replace("','", "' '"); //replace ',' by space as parameters are expected as comma separated. ',' is used instead of just ,(commma) to void considering ,(comma) in parameter value itself

                            if (pairs.ContainsKey("function"))
                                command = " -c " + "\"" + "import " + pyFileName + ";" + pyFileName + "." + pairs["function"] + "\"";

                            if (!string.IsNullOrEmpty(command))
                            {
                                ExecutionResult exeResult = Execute(pythonInter, command, Path.Combine(extractionLoc, pyFileName));
                                //then write the success or error message to the respective standard streams i.e. output or error

                                if (exeResult.IsSuccess)
                                    Console.WriteLine(exeResult.SuccessMessage);
                                else
                                    Console.Error.WriteLine(exeResult.ErrorMessage);
                            }
                        }
                        else
                            throw new Exception("Python or Iron-Python interpreter is NOT configured properly");

                        //then delete the umpackaged folder
                        string deleteFolder = Path.Combine(extractionLoc, pyFileName);
                        if (Directory.Exists(deleteFolder))
                            Directory.Delete(deleteFolder, true);
                    }
                    else
                    {
                        Console.WriteLine("Error");
                        throw new Exception(string.Format("IAPD package- {0} provided doesn't exist.", iapdFile));
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("\n Error: " + ex.Message);
                    if (ex.InnerException != null)
                        Console.Error.WriteLine("\n" + ex.InnerException.Message);
                }
            }
        }

        private static ExecutionResult Execute(string pythonInter, string commandArgString, string pyFileLocation)
        {
            ExecutionResult result = new ExecutionResult();
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe");
            processInfo.RedirectStandardInput = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardInput = true;

            Process processRunning = Process.Start(processInfo);
            processRunning.StandardInput.WriteLine(GetWorkingDrive(pyFileLocation));
            processRunning.StandardInput.WriteLine("cd " + pyFileLocation);

            processRunning.StandardInput.WriteLine("\""+pythonInter+"\"" + " " + commandArgString);
            processRunning.StandardInput.Close();

            //to handle long outout/error stream
            var outTask = processRunning.StandardOutput.ReadToEndAsync();
            var errTask = processRunning.StandardError.ReadToEndAsync();
            processRunning.WaitForExit();

            result.SuccessMessage = outTask.Result;
            result.SuccessMessage = GetOnlySubString(result.SuccessMessage, pythonInter + " " + commandArgString);
            result.SuccessMessage = result.SuccessMessage.Replace(pyFileLocation + ">", "");
            result.ErrorMessage = errTask.Result;            
            
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                result.IsSuccess = false;
            else
                result.IsSuccess = true;

            return result;
        }

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

        private static string GetWorkingDrive(string workingDir)
        {
            string drive = "";
            string[] folderpaths = workingDir.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (folderpaths.Length > 0)
                drive = folderpaths[0] + ":";

            return drive;
        }

        private static string GetOnlySubString(string complete, string afterThis)
        {
            if (!string.IsNullOrEmpty(complete) && !string.IsNullOrEmpty(afterThis))
            {
                complete = complete.Substring(complete.IndexOf(afterThis) + afterThis.Length);
            }
            return complete;
        }

        private static void ParseArgumentPairs(string[] arguments)
        {
            foreach (string arg in arguments)
            {
                foreach (string key in keys)
                {
                    if (arg.ToLower().Contains(key + ":"))
                    {
                        pairs.Add(key, arg.Substring(arg.IndexOf(key + ":") + (key + ":").Length));
                        break;
                    }
                }
            }
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
    }

    public class ExecutionResult
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }
}
