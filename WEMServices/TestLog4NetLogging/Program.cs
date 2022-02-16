/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Infrastructure.Common;
using log4net;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestLog4NetLogging
{
    class Program
    {
        static void Main(string[] args)
        {
            //LogHandler.TrackUsage("moduleloader", "run", GetAlias(), GetMachineName(), GetIP(),
            //    GetVersion(), "", "shell launched");
      
         //   string id =   LogHandler.TrackUsage("aa", "bb", "test_user", "hyd", "IP", "1.9", "mmnnlloo");

           // string id = LogHandler.TrackUsage("aa", "bb", "test_user", "hyd", "IP", "1.9", "", "launched");
        
            // Console.Read();
        }

        private static string GetAlias()
        {
            return System.Threading.Thread.CurrentPrincipal.Identity.Name;
        }

        private static string GetMachineName()
        {
            return Environment.MachineName;
        }

        private static string GetIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;

        }

        private static string GetVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion;
        }

        private static void TestLog4Net()
        {
            Infosys.WEM.Infrastructure.Common.LogHandler.Layer applicationLayer = LogHandler.Layer.Business;
            //LogHandler.WriteLogUsingLog4Net(null, LogPriority.P2, LogCategory.General, System.Diagnostics.TraceEventType.Error, (int)applicationLayer, null);
            //LogHandler.WriteLogUsingLog4Net("Statistics data count 2", LogPriority.P2, LogCategory.Statistics, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, null);
            //LogHandler.WriteLogUsingLog4Net("MessageArchive data count 2", LogPriority.P2, LogCategory.MessageArchive, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, null);
            //LogHandler.WriteLogUsingLog4Net("Performance data count 2", LogPriority.P2, LogCategory.Performance, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, null);
            //LogHandler.WriteLogUsingLog4Net("ScriptTracking data count 2", LogPriority.P2, LogCategory.ScriptTracking, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, null);
            //LogHandler.WriteLogUsingLog4Net("WFTracking data count 2", LogPriority.P2, LogCategory.WFTracking, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, null);
            ////LogHandler.WriteLogUsingLog4Net("Trace data count 2", LogPriority.P2, LogCategory.Trace, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, null);
            Console.ReadKey();
        }

        public static Tracer TraceOperations(string message, Infosys.WEM.Infrastructure.Common.LogHandler.Layer applicationLayer, Guid activityId, params object[] messageArguments)
        {
            string enableLogsConfig = "test";
            bool enableLogs = false;
            if (!string.IsNullOrEmpty(enableLogsConfig))
                enableLogs = true;
            if (enableLogs)
            {

                try
                {

                    TraceManager traceMgr = new TraceManager(EnterpriseLibraryContainer.Current.GetInstance<LogWriter>());
                    Tracer tracer;
                    if (activityId != null)
                    {

                        tracer = traceMgr.StartTrace("Performance", activityId);
                    }
                    else
                    {
                        tracer = traceMgr.StartTrace("Performance");
                    }


                    LogEntry logEntry = new LogEntry();
                    logEntry.EventId = (int)applicationLayer;
                    logEntry.Priority = 11;
                    logEntry.Severity = System.Diagnostics.TraceEventType.Verbose;
                    if (null != messageArguments)
                    {
                        logEntry.Message = string.Format(message, messageArguments);
                    }
                    else
                    {
                        logEntry.Message = message;
                    }
                    logEntry.Categories.Add("General");
                    //logEntry.Categories.Add("Performance");
                    if (Microsoft.Practices.EnterpriseLibrary.Logging.Logger.ShouldLog(logEntry))
                    {
                        Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
                    }
                    return tracer;

                }
                catch (Exception ex)
                {
                }
            }

            return null;
        }
    }
}
