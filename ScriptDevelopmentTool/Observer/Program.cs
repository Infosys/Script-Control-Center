/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Observer
{
    class Program
    {
        //Process wfProcess = null;
        static bool applicationClosed = false;
        static string scrFilepath = string.Empty;
        static string wfFilePath = string.Empty;
        static bool ExitScript = false;
        static bool ExitWf = false;
       
        static Process scptProcess;
        static Process wfProcess1;
        static void Main(string[] args)
        {


            try
            {
                //Console.ReadLine();
                int counter = 0;
                int exitCode;
                _handler = new EventHandler(Handler);
                SetConsoleCtrlHandler(_handler, true);
                string path = Directory.GetCurrentDirectory();
                Console.WriteLine("Enter the name of utility you want to execute(Script/WorkFlow/ScriptandWorkflow:)");
                string FileToExecute = Console.ReadLine();
                string exe = string.Empty;
                string arguments = string.Empty;
                string scriptConsoleLocationPath = ConfigurationManager.AppSettings["ScriptConsoleLocationPath"];
                string workflowConsoleLocationPath = ConfigurationManager.AppSettings["WorkflowConsoleLocationPath"];
                const string SCRIPT_CONSOLE_DIR = "ScriptExecuteUtil";
                const string WF_CONSOLE_DIR = "WorkflowExecuteUtil";
                const string SCRIPT_CONSOLE_EXE = "ScriptExecuteUtil.exe";
                const string WF_CONSOLE_EXE = "WorkflowExecuteUtil.exe";

                if (FileToExecute.ToLower().Equals("script"))
                {
                    scrFilepath = PrepareFilePath(scriptConsoleLocationPath, SCRIPT_CONSOLE_DIR, SCRIPT_CONSOLE_EXE);
                    if (string.IsNullOrEmpty(scrFilepath))
                        return;
                    Process wfProcess = Process.Start(scrFilepath, arguments);
                    wfProcess.Exited += Process_Exited;
                    WatchProcess(wfProcess);
                }

                else if (FileToExecute.ToLower().Equals("workflow"))
                {
                    wfFilePath = PrepareFilePath(scriptConsoleLocationPath, WF_CONSOLE_DIR, WF_CONSOLE_EXE);
                    if (string.IsNullOrEmpty(wfFilePath))
                        return;
                    Process wfProcess = Process.Start(wfFilePath, arguments);
                    wfProcess.Exited += Process_Exited;
                    WatchProcess(wfProcess);
                }
                else if (FileToExecute.ToLower().Equals("scriptandworkflow"))
                {
                    scrFilepath = PrepareFilePath(scriptConsoleLocationPath, SCRIPT_CONSOLE_DIR, SCRIPT_CONSOLE_EXE);
                    if (string.IsNullOrEmpty(scrFilepath))
                        return;
                    Thread thScript = new Thread(ThreadScript);
                    Thread.Sleep(500);
                    
                    wfFilePath = PrepareFilePath(scriptConsoleLocationPath, WF_CONSOLE_DIR, WF_CONSOLE_EXE);
                    if (string.IsNullOrEmpty(wfFilePath))
                        return;
                    Thread thWF = new Thread(ThreadWorkflow);
                    Thread.Sleep(500);
                    thScript.Start();
                    thWF.Start();
                    if(ExitScript)
                    {
                        thScript.Abort();
                    }
                }
                else
                {
                    Console.WriteLine("Please enter valid input");
                }
            }
            catch (Exception ex) { string error = ex.Message; }
        }

        private static string PrepareFilePath(string consoleLocationPath, string consoleDirName, string consoleExeName)
        {
            string exe = consoleExeName;
            string filePath = "";
            if (string.IsNullOrEmpty(consoleLocationPath))
            {
                filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), consoleDirName, exe);

                if (!File.Exists(filePath))
                {
                    filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), exe);
                }
            }
            else
            {
                filePath = Path.Combine(consoleLocationPath, exe);
            }

            if (Infosys.WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(Path.GetFileName(filePath)))
            {
                Console.WriteLine("The console location file path contains certain Special Characters which are not allowed. Please remove those Special Characters from the file path and try again.");
                return "";
            }
            return filePath;
        }

        public static void ThreadScript()
         {
             try
             {
                 Thread.Sleep(500);

                 scptProcess = Process.Start(scrFilepath);
                 scptProcess.WaitForExit();

                 while (scptProcess.ExitCode != 0 && scptProcess.ExitCode != -1073741510)
                 {
                     scptProcess = Process.Start(scrFilepath);
                     scptProcess.WaitForExit();
                 }
                 Thread.Sleep(500);
                 ExitScript = true;
                 Environment.Exit(0);
             }
            catch(Exception ex)
             {
                 Environment.ExitCode = 2;
                 Environment.Exit(2);
             }
         }
        public static void ThreadWorkflow()
        {
            try
            {
                Thread.Sleep(500);

                wfProcess1 = Process.Start(wfFilePath);
                wfProcess1.WaitForExit();
                while (wfProcess1.ExitCode != 0 && wfProcess1.ExitCode != -1073741510)
                {
                    wfProcess1 = Process.Start(wfFilePath);
                    wfProcess1.WaitForExit();
                }
                ExitWf = true;
                Environment.Exit(0);
            }
            catch(Exception ex)
            {
                Environment.ExitCode = 2;
                Environment.Exit(2);
            }
        }


        static void Process_Exited(object sender, EventArgs e)
        {
            applicationClosed = true;
            Console.WriteLine("Utility exited.");
        }
        public static int CheckforExit(Process wfProcess)
        {
            int exitCode = 0;
            do
            {
                Thread.Sleep(500);
                if (wfProcess.HasExited)
                {
                    Thread.Sleep(1000);
                    exitCode = wfProcess.ExitCode;

                }
            } while (!wfProcess.WaitForExit(1000));
            return exitCode;
        }

        static void WatchProcess(Process process)
        {
            while (!process.HasExited)
            {
                Thread.Sleep(500);
            }

            if (process.HasExited && process.ExitCode != 0 && process.ExitCode != -1073741510)
            {
                //i.e. closed due to error, hence restart and watch
                //N.B. the exit code is equal to -1073741510, when the utility is closed explicitly by cliking the close-'X' on the top right of the console window.
                Thread.Sleep(3000);
                Console.WriteLine("Re-starting the utility...");
                process.Start();
                WatchProcess(process);
            }
            else
                Environment.Exit(0);
        }
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(EventHandler Handler, bool Add);

        //This delegate type used as handler routine to SetConsoleControlHandler.
        public delegate bool EventHandler(ControlTypes CtrlType);
        static EventHandler _handler;
        //Enumerated control types for handlers
        public enum ControlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool Handler(ControlTypes ctrlType)
        {
            Environment.ExitCode = 0;
            if (scptProcess != null)
            {
                scptProcess.CloseMainWindow();
                scptProcess.Close();
            }
            if (wfProcess1 != null)
            {
                wfProcess1.CloseMainWindow();
                wfProcess1.Close();
            }
            Environment.Exit(0);
            

           // Thread.Sleep(5000);
            return true;
        }
    }

}

