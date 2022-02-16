/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WEM.ScriptExecution.Implementation;

namespace WEM.Host.ConsoleApp
{
    public static class Program
    {
        private static ServiceHost[] Hosts = null;
        
        public const string ServiceName = "WEMHostService";

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                Program.Start(args);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }

        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
                // running as service
                using (var service = new Service())
                    ServiceBase.Run(service);
            else
            {
                // running as console app
                Start(args);

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey(true);

                Stop();
            }

            //Program obj = new Program();
            //obj.GetAllClassesAndMethodsOfAssembly();
        }

        private static void Start(string[] args)
        {
            try
            {
                //string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
                //if (File.Exists(filepath))
                //{
                //    File.Delete(filepath);
                //}

                //Load WEM.ServiceImplementation assembly to get list of classes 
                List<Type> classTypes = GetAllClassesOfAssembly("WEM.ServiceImplementation.dll");
                Console.WriteLine("No.of classes in WEM.ServiceImplementation assembly" + classTypes.Count());
                WriteToFile("No.of classes in WEM.ServiceImplementation assembly" + classTypes.Count());

                //Load WEM.ScriptExecution assembly to get list of classes 
                List<Type> types = GetAllClassesOfAssembly("WEM.ScriptExecution.dll");
                classTypes.AddRange(types);
                Console.WriteLine("No.of services " + classTypes.Count());
                WriteToFile("No.of services " + classTypes.Count());

                int index = classTypes.Count();
                Hosts = new ServiceHost[index];
                int i = 0;

                //// Create a ServiceHost for the AutomationTrackerRepository,ScriptExecute etc... type and 
                //// provide the base address. 
                foreach (Type type in classTypes)
                {
                    Hosts[i] = new ServiceHost(type);
                    i++;
                    Console.WriteLine("Created a ServiceHost for the " + type.Name);
                    WriteToFile("Created a ServiceHost for the " + type.Name);
                }

                Console.WriteLine("Services are started at " + DateTime.Now);
                WriteToFile("Services are started at " + DateTime.Now);

                // Open the ServiceHostBase to create listeners and start listening for messages.
                foreach (ServiceHost host in Hosts)
                {
                    host.Open();
                    Console.WriteLine("Opened the ServiceHostBase for " + host.Description.Name);
                    WriteToFile("Opened the ServiceHostBase for " + host.Description.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured in Onstart --- Inner Exception : " + ex.InnerException + "Message: " + ex.Message);
                WriteToFile("Exception occured in Onstart --- Inner Exception : " + ex.InnerException + "Message: " + ex.Message);
            }
        }

        private static void Stop()
        {
            foreach (ServiceHost host in Hosts)
            {
                host.Close();
            }
            //Console.WriteLine("Service is stopped at " + DateTime.Now);
            Console.WriteLine("Service is stopped at " + DateTime.Now);
        }
        public static List<Type> GetAllClassesOfAssembly(string DllName)
        {
            try
            {
                string pathName = AppDomain.CurrentDomain.BaseDirectory;        //Get the Location of the application  
                Assembly assembly = Assembly.LoadFile(pathName + DllName);
                List<Type> types = assembly.GetTypes().Where(tc => tc.BaseType != null && tc.BaseType.Name.Contains("ServiceBase") && tc.IsPublic).OrderBy(b => b.FullName).ToList();
                return types;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured while reading classes of assembly.... Inner Exception " + ex.InnerException + " Message:" + ex.Message + " " + DateTime.Now);
                WriteToFile("Exception occured while reading classes of assembly.... Inner Exception " + ex.InnerException + " Message:" + ex.Message + " " + DateTime.Now);
                return null;
            }
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
