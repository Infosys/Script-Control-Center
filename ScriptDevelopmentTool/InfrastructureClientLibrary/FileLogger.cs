/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace InfrastructureClientLibrary
{
    public class FileLogger
    {

        public void WriteToFile(string message,  bool isError, bool isAudit)
        {
            LogEntry logEntry = new LogEntry();
            logEntry.Priority = 3;
            logEntry.Severity = System.Diagnostics.TraceEventType.Information;
            logEntry.Message = message;
            if (isError)
                logEntry.Categories.Add("WriteErrorsToFile");
            else if (isAudit)
                logEntry.Categories.Add("WriteAuditsToFile");
            else
                logEntry.Categories.Add("GeneralFileLogging");

            Logger.Write(logEntry);
        }

        public void AddTraceListener(string logFilePathErrors, string logFilePathAudits, string logFilePathMessages, bool isError, bool isAudit)
        {
            string date = DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" + DateTime.Today.Year.ToString();

            string pathErrors = System.IO.Path.Combine(logFilePathErrors, date);
            if (!Directory.Exists(pathErrors))
            {
                Directory.CreateDirectory(pathErrors);
            }

            string pathAudits = System.IO.Path.Combine(logFilePathAudits, date);
            if (!Directory.Exists(pathAudits))
            {
                Directory.CreateDirectory(pathAudits);
            }

            string pathMessages = System.IO.Path.Combine(logFilePathMessages, date);
            if (!Directory.Exists(pathMessages))
            {
                Directory.CreateDirectory(pathMessages);
            }

            string path1 = pathErrors + @"\iap_errors.log";
            string path2 = pathAudits + @"\iap_audits.log";
            string path3 = pathMessages + @"\iap_general.log";

            if (!File.Exists(path1))
            {
                {
                    using (StreamWriter file = new StreamWriter(path1, true))
                    {
                        file.WriteLine("TicketNumber;TimeStamp;ScriptId, Message;ErrorDetails;ErrorTypeID;CreatedBy;CreatedFromMachineName;CreatedFromMachineIP;");
                    }
                }
            }
            if (!File.Exists(path2))
            {
                {
                    using (StreamWriter file = new StreamWriter(path2, true))
                    {
                        file.WriteLine("TicketNumber;StateId;ScriptId;Message;LogData;CreatedBy;TimeStamp;CreatedFromMachineName;CreatedFromMachineIP");
                    }
                }
            }
            if (!File.Exists(path3))
            {
                using (StreamWriter file = new StreamWriter(path3, true))
                {
                    file.WriteLine("Message;TimeStamp");
                }
            }


            SetTraceListenerForFile(path1, path2, path3);

            var oldLogger = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
            if (oldLogger != null)
                oldLogger.Dispose();

        }

        private void SetTraceListenerForFile(string path1, string path2, string path3)
        {
            var builder = new ConfigurationSourceBuilder();

            builder.ConfigureLogging()
                        .WithOptions
                            .DoNotRevertImpersonation()
                        .LogToCategoryNamed("WriteErrorsToFile")
                            .SendTo.RollingFile("Errors Log File")
                            .FormatWith(new FormatterBuilder()
                              .TextFormatterNamed("CustomFormatter")
                              .UsingTemplate("{message}"))
                              .RollAfterSize(5000)
                              .RollEvery(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollInterval.Day)
                              .WhenRollFileExists(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollFileExistsBehavior.Increment)
                              .WithHeader("")
                              .WithFooter("")
                            .ToFile(path1)
                        .LogToCategoryNamed("WriteAuditsToFile")
                            .SendTo.RollingFile("Audits Log File")
                            .FormatWith(new FormatterBuilder()
                              .TextFormatterNamed("CustomFormatter2")
                              .UsingTemplate("{message}"))
                              .RollAfterSize(5000)
                              .RollEvery(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollInterval.Day)
                              .WhenRollFileExists(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollFileExistsBehavior.Increment)
                              .WithHeader("")
                              .WithFooter("")
                            .ToFile(path2)
                        .LogToCategoryNamed("GeneralFileLogging")
                            .SendTo.RollingFile("General Log File")
                            .FormatWith(new FormatterBuilder()
                              .TextFormatterNamed("CustomFormatter3")
                              .UsingTemplate("{message},{timestamp}"))
                              .RollAfterSize(5000)
                              .RollEvery(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollInterval.Day)
                              .WhenRollFileExists(Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.RollFileExistsBehavior.Increment)
                              .WithHeader("")
                              .WithFooter("")
                            .ToFile(path3);

            var configSource = new DictionaryConfigurationSource();
            builder.UpdateConfigurationWithReplace(configSource);
            EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);
        }


    }

}

