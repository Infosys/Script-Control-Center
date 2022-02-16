using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using Infosys.WEM.Service.Implementation;
using WEM.ScriptExecution.Implementation;
using System.Reflection;

namespace Infosys.WEM.Host.WindowsService
{
    public partial class WEMService : ServiceBase
    {
        public ServiceHost serviceHost = null;
        Timer timer = new Timer(); // name space(using System.Timers;) 
        private static ServiceHost[] Hosts = null;

        public WEMService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {                
                if (serviceHost != null)
                {
                    serviceHost.Close();
                }
                //Load WEM.ServiceImplementation assembly to get list of classes 
                List<Type> classTypes = GetAllClassesOfAssembly("WEM.ServiceImplementation.dll");
                WriteToFile("No.of classes in WEM.ServiceImplementation assembly" + classTypes.Count());

                //Load WEM.ScriptExecution assembly to get list of classes 
                List<Type> types = GetAllClassesOfAssembly("WEM.ScriptExecution.dll");
                classTypes.AddRange(types);
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
                    WriteToFile("Created a ServiceHost for the " + type.Name);
                }                 
               
                WriteToFile("Services are started at " + DateTime.Now);

                // Open the ServiceHostBase to create listeners and start listening for messages.
                foreach (ServiceHost host in Hosts)
                {
                    host.Open();
                    WriteToFile("Opened the ServiceHostBase for " + host.Description.Name);                    
                }                
            }
            catch (Exception ex)
            {
                WriteToFile("Exception occured in Onstart --- Inner Exception : " + ex.InnerException+"Message: "+ ex.Message);
            }
           
        }

        private List<Type> GetAllClassesOfAssembly(string DllName)
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
                WriteToFile("Exception occured while reading classes of assembly.... Inner Exception " +ex.InnerException+" Message:"+ex.Message+" "+ DateTime.Now);
                return null;
            }
        }

        public static void Main()
        {
            ServiceBase.Run(new WEMService());            
        }

        protected override void OnStop()
        {
            foreach (ServiceHost host in Hosts)
            {
                host.Close();
            }
            //Console.WriteLine("Service is stopped at " + DateTime.Now);
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
           // Console.WriteLine("Service is recall at " + DateTime.Now);
            WriteToFile("Service is recall at " + DateTime.Now);
        }
        public void WriteToFile(string Message)
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
