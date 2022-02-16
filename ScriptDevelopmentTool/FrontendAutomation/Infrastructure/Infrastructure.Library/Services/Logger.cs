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
using System.Net;
using Infosys.WEM.Infrastructure.Common;

namespace IMSWorkBench.Infrastructure.Library.Services
{
    public class Logger
    {
        private static Boolean IsOnline = GetMode();

        private static bool GetMode()
        {
            Boolean result = false;
            string mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
            if (string.IsNullOrEmpty(mode))
                result = false;
            else if (mode.ToLower() == "offline")
                result = false;
            else if (mode.ToLower() == "online")
                result = true;
            else result = false;

            return result;
        }

        public static string Log(string modulename, string fnname, string data, string transactId = "")
        {
            if (IsOnline)
                modulename += "(online)";
            else
                modulename += "(offline)";

            if(string.IsNullOrEmpty(transactId))
            {
            transactId = Guid.NewGuid().ToString();
            }
            LogHandler.TrackUsageAsync(modulename, fnname, GetAlias(), GetMachineName(), GetIP(),
                GetVersion(), transactId, data);
            return transactId;
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
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public static void LogWFExecutionTime(string executionTime, string wfName, Dictionary<string, string> param, string result)
        {
            LogHandler.WorkflowTracking(GetAlias(), executionTime, wfName, param, result);
        }

        public static void InitiateOfflineLog()
        {
            LogHandler.SynchronizeOfflineTrackUsageLogsWithDB();
        }
    }

    public class LogObject
    {
        public string ModuleName { get; set; }
        public string FnName { get; set; }
        public string Alias { get; set; }
        public string MachineName { get; set; }
        public string Version { get; set; }
        public string TransId { get; set; }
        public string Data { get; set; }
        public string IP { get; set; }
    }
}
