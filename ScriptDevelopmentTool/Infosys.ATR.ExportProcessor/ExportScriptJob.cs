using Infosys.ATR.ExportUtility.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.ATR.ExportProcessor
{
    partial class ExportScriptJob : ServiceBase
    {
        System.Timers.Timer timer = null;
        string logFilePath = "";
        public ExportScriptJob()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                logFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();
                System.IO.File.AppendAllText(logFilePath, "service started..");
                timer = new System.Timers.Timer();
                timer.Interval =180000; // 2 Mins  
                timer.Elapsed += Timer_Elapsed;
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logFilePath,ex.Message + ex.StackTrace + ex.InnerException.Message);
            }
        }        

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                System.IO.File.AppendAllText(logFilePath, "Starting Export Job");
                ExportUtility.RunExportJob();
                System.IO.File.AppendAllText(logFilePath, "Completed Export Job");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logFilePath, ex.Message + ex.StackTrace + ex.InnerException.Message);
            }
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            System.IO.File.AppendAllText(logFilePath, "service stopped..");
        }
    }
}
