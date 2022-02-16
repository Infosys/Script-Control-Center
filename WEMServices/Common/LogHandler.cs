/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Infosys.WEM.Infrastructure.Common
{

    public class LogHandler
    {
        public enum BusinessFunctionConstants
        {
            PUBLISH = 1000,
            INVOKEWF = 1001,
            SCRIPTEXECUTE = 1002,
            WFEXECUTE = 1003,

        }

        public enum Layer
        {
            WebServiceHost = 500,
            Business = 501,
            Resource = 502,
            Infrastructure = 503,
            Activity = 504,
            ScriptEngine = 505,
            WFEngine = 506
        }

        public enum LogProvider
        {
            EnterpriseLibrary = 1,
            Log4Net
        }

        static LogProvider logProvider = ReadProviderFromConfig();
        static ILog trackUsageOnlineLogger = null;
        static ILog trackUsageOfflineLogger = null;
        static ILog generalLogger = null;
        static ILog statisticsLogger = null;
        static ILog messageLogger = null;
        static ILog scriptLogger = null;
        static ILog wftrackingLogger = null;

        public static Boolean SetUpDBLogger()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();

                Boolean serverReachable = CheckServerReachability();
                if (serverReachable)
                {
                    if (trackUsageOnlineLogger == null)
                        trackUsageOnlineLogger = LogManager.GetLogger("TrackUsageOnline");

                }
                else
                {
                    if (trackUsageOfflineLogger == null)
                    {
                        //Set up Logger
                        SetUpOfflineLogger();
                        trackUsageOfflineLogger = LogManager.GetLogger("TrackUsageOffline");
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Check id database server is reachable
        /// </summary>
        /// <returns></returns>
        private static bool CheckServerReachability()
        {
            bool status = false;
            string connString = System.Configuration.ConfigurationManager.AppSettings["TrackUsageConnString"];
            if (!string.IsNullOrEmpty(connString))
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connString;
                try
                {
                    conn.Open();
                    status = true;
                }
                catch (Exception ex)
                {
                    status = false;
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            return status;
        }

        //static Boolean isLoggerSetUp = SetUpDBLogger();

        private static string enableLogsConfig = System.Configuration.ConfigurationManager.AppSettings["EnableAllLogs"];
        private static bool enableLogs = false;
        LogWriter writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();

        private static LogProvider ReadProviderFromConfig()
        {
            LogProvider provider = LogProvider.EnterpriseLibrary;
            if (System.Configuration.ConfigurationManager.AppSettings["LogProvider"] != null)
                provider = LogProvider.Log4Net;
            return provider;
        }

        /// <summary>
        /// Log initiation of any business function in the views or processes
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="businessFunction">Business function in which the method is invoked</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        /// <returns>Tracer object if trace has been successfully initialized, null if tracer or logger component is not configured properly .</returns>
        public static Tracer LogBusinessFunction(string message, BusinessFunctionConstants businessFunction, params object[] messageArguments)
        {

            if (!string.IsNullOrEmpty(enableLogsConfig))
                enableLogs = true;
            if (enableLogs)
            {
                try
                {
                    TraceManager traceMgr = new TraceManager(EnterpriseLibraryContainer.Current.GetInstance<LogWriter>());
                    //Guid activityId = Guid.NewGuid();
                    Tracer tracer = traceMgr.StartTrace("Performance");
                    string strActivityId = "";
                    //if (tracer.IsTracingEnabled())
                    //{
                    //    strActivityId = "[Activity:" + activityId.ToString() + "]";
                    //}
                    LogEntry logEntry = new LogEntry();

                    logEntry.EventId = (int)businessFunction;
                    logEntry.Priority = 6;
                    logEntry.Severity = System.Diagnostics.TraceEventType.Information;
                    if (null != messageArguments)
                    {
                        logEntry.Message = string.Format(strActivityId + message, messageArguments);
                    }
                    else
                    {
                        logEntry.Message = strActivityId + message;
                    }
                    //logEntry.Categories.Add("Performance");

                    if (Microsoft.Practices.EnterpriseLibrary.Logging.Logger.ShouldLog(logEntry))
                    {
                        Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
                    }

                    return tracer;
                }
                catch (Exception) { }
            }
            return null;
        }

        /// <summary>
        /// Log to debug code statements in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogDebug(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 10, "General", System.Diagnostics.TraceEventType.Verbose, (int)applicationLayer, messageArguments);
            }
            else
            {
                WriteLogUsingLog4NetAsync(message, LogPriority.P10, LogCategory.General, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, messageArguments);
            }
        }

        /// <summary>
        /// Log Information statements in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogInfo(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 10, "General", System.Diagnostics.TraceEventType.Information, (int)applicationLayer, messageArguments);
            }
            else
            {
                WriteLogUsingLog4NetAsync(message, LogPriority.P10, LogCategory.General, System.Diagnostics.TraceEventType.Information, (int)applicationLayer, messageArguments);
            }
        }

        /// <summary>
        /// Log warnings in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogWarning(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 3, "General", System.Diagnostics.TraceEventType.Warning, (int)applicationLayer, messageArguments);
            }
            else
            {
                WriteLogUsingLog4NetAsync(message, LogPriority.P3, LogCategory.General, System.Diagnostics.TraceEventType.Warning, (int)applicationLayer, messageArguments);
            }
        }

        /// <summary>
        /// Log errors in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogError(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 2, "General", System.Diagnostics.TraceEventType.Error, (int)applicationLayer, messageArguments);
            }
            else
            {
                WriteLogUsingLog4NetAsync(message, LogPriority.P2, LogCategory.General, System.Diagnostics.TraceEventType.Error, (int)applicationLayer, messageArguments);
            }
        }

        public static void WriteActivitiesError(string message)
        {
            ILog errorLogger = LogManager.GetLogger(Log4NetConstants.ErrorsLogger);
            errorLogger.Error(message);
        }

        public static void WriteActivitiesAudit(string message)
        {
            ILog auditLogger = LogManager.GetLogger(Log4NetConstants.AuditsLogger);
            auditLogger.Info(message);
        }

        public static void WriteActivitiesInfo(string message)
        {
            ILog messageLogger = LogManager.GetLogger(Log4NetConstants.MessagesLogger);
            messageLogger.Info(message);
        }

        private static void WriteLogUsingEnterpriseLibrary(string message, int priority, string category, TraceEventType traceEvent, int applicationLayer, object[] messageArguments)
        {
            if (message != null)
            {
                if (!string.IsNullOrEmpty(enableLogsConfig))
                    enableLogs = true;
                if (enableLogs)
                {
                    try
                    {
                        LogEntry logEntry = new LogEntry();
                        logEntry.EventId = (int)applicationLayer;
                        logEntry.Priority = priority;
                        logEntry.Severity = traceEvent;
                        if (null != messageArguments)
                        {
                            logEntry.Message = string.Format(message, messageArguments);
                        }
                        else
                        {
                            logEntry.Message = message;
                        }
                        logEntry.Categories.Add(category);

                        if (Microsoft.Practices.EnterpriseLibrary.Logging.Logger.ShouldLog(logEntry))
                        {
                            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        private static void WriteLogUsingLog4NetAsync(string message, LogPriority priority, LogCategory category, TraceEventType traceEvent, int applicationLayer, object[] messageArguments)
        {
            var t = new Thread(() => WriteLogUsingLog4Net(message, priority, category, traceEvent, applicationLayer, messageArguments));
            t.Start();
        }

        private static void WriteLogUsingLog4Net(string message, LogPriority priority, LogCategory category, TraceEventType traceEvent, int applicationLayer, object[] messageArguments)
        {
            string logMessage = string.Empty;

            if (message != null && message != string.Empty)
            {
                if (!string.IsNullOrEmpty(enableLogsConfig))
                    enableLogs = true;
                if (enableLogs)
                {
                    if (null != messageArguments)
                    {
                        logMessage = string.Format(message, messageArguments);
                    }
                    else
                    {
                        logMessage = message;
                    }

                    log4net.Config.XmlConfigurator.Configure();


                    log4net.ThreadContext.Properties["pid"] = Process.GetCurrentProcess().Id;
                    log4net.ThreadContext.Properties["pname"] = Process.GetCurrentProcess().ProcessName;
                    log4net.ThreadContext.Properties["eventId"] = (int)applicationLayer;
                    log4net.ThreadContext.Properties["severity"] = traceEvent.ToString();
                    log4net.ThreadContext.Properties["threadName"] = Thread.CurrentThread.Name;
                    log4net.ThreadContext.Properties["title"] = "";
                    log4net.ThreadContext.Properties["formattedMessage"] = null;
                    log4net.ThreadContext.Properties["Priority"] = (int)priority;

                    log4net.ThreadContext.Properties["MachineName"] = Environment.MachineName;
                    log4net.ThreadContext.Properties["AppDomainName"] = AppDomain.CurrentDomain.FriendlyName;
                    log4net.ThreadContext.Properties["Win32ThreadId"] = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    log4net.ThreadContext.Properties["Message"] = logMessage;
                    log4net.ThreadContext.Properties["LogId"] = "";

                    switch (category)
                    {
                        case LogCategory.General:
                            if (generalLogger == null)
                            {
                                generalLogger = LogManager.GetLogger(Log4NetConstants.GeneralLogger);
                            }
                            generalLogger.Info(logMessage);
                            break;
                        case LogCategory.Statistics:
                            if (statisticsLogger == null)
                            {
                                statisticsLogger = LogManager.GetLogger(Log4NetConstants.StatisticsLogger);
                            }
                            statisticsLogger.Info(logMessage);
                            break;
                        case LogCategory.MessageArchive:
                            if (messageLogger == null)
                            {
                                messageLogger = LogManager.GetLogger(Log4NetConstants.MessageLogger);
                            }
                            messageLogger.Info(logMessage);
                            break;
                        case LogCategory.ScriptTracking:
                            if (scriptLogger == null)
                            {
                                scriptLogger = LogManager.GetLogger(Log4NetConstants.ScriptTrackingLogger);
                            }
                            scriptLogger.Info(logMessage);
                            break;
                        case LogCategory.WFTracking:
                            if (wftrackingLogger == null)
                            {
                                wftrackingLogger = LogManager.GetLogger(Log4NetConstants.WFTracking);
                            }
                            wftrackingLogger.Info(logMessage);
                            break;
                    }
                }
            }
        }

        public static void SetupLog4Net(string errorsLogFilePath, string auditsLogFilePath, string messagesLogFilePath)
        {
            string date = DateTime.Today.Day.ToString() + "-" + DateTime.Today.Month.ToString() + "-" + DateTime.Today.Year.ToString();

            string pathErrors = System.IO.Path.Combine(errorsLogFilePath, date);
            if (!Directory.Exists(pathErrors))
            {
                Directory.CreateDirectory(pathErrors);
            }

            string pathAudits = System.IO.Path.Combine(auditsLogFilePath, date);
            if (!Directory.Exists(pathAudits))
            {
                Directory.CreateDirectory(pathAudits);
            }

            string pathMessages = System.IO.Path.Combine(messagesLogFilePath, date);
            if (!Directory.Exists(pathMessages))
            {
                Directory.CreateDirectory(pathMessages);
            }

            //string path1 = pathErrors + @"\iap_errors.log";
            string path1 = pathErrors + @"\iap_errors-" + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + ".log";
            string path2 = pathAudits + @"\iap_audits-" + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + ".log";
            string path3 = pathMessages + @"\iap_general-" + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Year + DateTime.Now.Hour + ".log";

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            RemoveAppender(Log4NetConstants.ErrorsRollingFile);
            RemoveAppender(Log4NetConstants.AuditsRollingFile);
            RemoveAppender(Log4NetConstants.MessagesRollingFile);

            RollingFileAppender roller = CreateRollingFileAppender(Log4NetConstants.ErrorsRollingFile, path1, Log4NetConstants.ErrorsLogger, Log4NetConstants.ErrorsHeader);
            hierarchy.Root.AddAppender(roller);

            RollingFileAppender roller2 = CreateRollingFileAppender(Log4NetConstants.AuditsRollingFile, path2, Log4NetConstants.AuditsLogger, Log4NetConstants.AuditsHeader);
            hierarchy.Root.AddAppender(roller2);

            RollingFileAppender roller3 = CreateRollingFileAppender(Log4NetConstants.MessagesRollingFile, path3, Log4NetConstants.MessagesLogger, Log4NetConstants.MessagesHeader);
            hierarchy.Root.AddAppender(roller3);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;

            log4net.Config.XmlConfigurator.Configure();
        }

        public static void DisposeLog4Net()
        {
            ILog errorLogger = LogManager.GetLogger(Log4NetConstants.ErrorsLogger);
            if (errorLogger != null)
                errorLogger.Logger.Repository.Shutdown();

            ILog auditLogger = LogManager.GetLogger(Log4NetConstants.AuditsLogger);
            if (auditLogger != null)
                auditLogger.Logger.Repository.Shutdown();

            ILog messageLogger = LogManager.GetLogger(Log4NetConstants.MessagesLogger);
            if (messageLogger != null)
                messageLogger.Logger.Repository.Shutdown();
        }

        private static RollingFileAppender CreateRollingFileAppender(string name, string logFilePath, string loggerMatch, string header, string conversionPattern = "")
        {
            PatternLayout patternLayout = new PatternLayout();
            if (string.IsNullOrEmpty(conversionPattern))
            {
                patternLayout.ConversionPattern = "%newline%message";
            }
            else
            {
                patternLayout.ConversionPattern = conversionPattern;
            }
            patternLayout.Header = header;
            patternLayout.ActivateOptions();

            HeaderOnceAppender roller = new HeaderOnceAppender();
            roller.Name = name;
            log4net.Filter.LoggerMatchFilter filter = new log4net.Filter.LoggerMatchFilter();
            filter.LoggerToMatch = loggerMatch;
            roller.AddFilter(filter);
            roller.AddFilter(new log4net.Filter.DenyAllFilter());
            roller.AppendToFile = true;

            //roller.LockingModel = new FileAppender.MinimalLock();
            // roller.ImmediateFlush

            roller.File = logFilePath;
            roller.Layout = patternLayout;
            roller.CountDirection = 1;
            roller.PreserveLogFileNameExtension = true;
            roller.MaxSizeRollBackups = 100;
            roller.DatePattern = "MMddyyyyHH";
            roller.MaximumFileSize = "1000KB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Date;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            roller.ImmediateFlush = true;
            return roller;
        }

        private static void RemoveAppender(string name)
        {
            log4net.Repository.ILoggerRepository repository = log4net.LogManager.GetRepository();
            foreach (log4net.Appender.IAppender appender in repository.GetAppenders())
            {
                if (appender.Name != null)
                {
                    if (appender.Name.CompareTo(name) == 0 && appender is log4net.Appender.RollingFileAppender)
                    {
                        log4net.Appender.RollingFileAppender fileAppender = (log4net.Appender.RollingFileAppender)appender;
                        Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
                        hierarchy.Root.RemoveAppender(fileAppender);
                    }
                }
            }
        }

        /// <summary>
        /// Trace operation blocks in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        /// <param name="activityid">Optional. Overrides system generated activity id </param>
        /// <returns>Tracer object if trace has been successfully initialized, null if tracer or logger component is not configured properly .</returns>
        public static Tracer TraceOperations(string message, Layer applicationLayer, Guid activityId, params object[] messageArguments)
        {
            if (!string.IsNullOrEmpty(enableLogsConfig))
                enableLogs = true;
            if (enableLogs)
            {
                try
                {
                    TraceManager traceMgr = new TraceManager(EnterpriseLibraryContainer.Current.GetInstance<LogWriter>());
                    Tracer tracer;
                    if (activityId != null || activityId != System.Guid.Empty)
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

        /// <summary>
        /// Log to track application usage
        /// </summary>
        /// <param name="message">Message to be logged for tracking application Usage</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogUsage(string message, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 15, "Statistics", System.Diagnostics.TraceEventType.Information, 5000, messageArguments);
            }
            else
            {
                WriteLogUsingLog4NetAsync(message, LogPriority.P15, LogCategory.Statistics, System.Diagnostics.TraceEventType.Information, 5000, messageArguments);
            }
        }

        /// <summary>
        /// Log to track application usage
        /// </summary>
        /// <param name="message">Message to be logged for tracking application Usage</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void ArchiveMessages(string message, string location)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message + "$" + location, 20, "MessageArchive", System.Diagnostics.TraceEventType.Information, 6000, null);
            }
            else
            {
                WriteLogUsingLog4NetAsync(message + "$" + location, LogPriority.P20, LogCategory.MessageArchive, System.Diagnostics.TraceEventType.Information, 6000, null);
            }
        }

        public static void ScriptTracking(string userId, string executionTime, string scriptName, Dictionary<string, string> parameters, string output)
        {
            try
            {
                string parameterStr = string.Empty;
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (System.Collections.Generic.KeyValuePair<string, string> parameter in parameters)
                    {
                        parameterStr = parameterStr + parameter.Key + "=" + parameter.Value + "&&";
                    }
                    parameterStr = parameterStr.Substring(0, parameterStr.LastIndexOf("&&"));
                }
                string messageFormatBrief = string.Format("UserName:{0}||ExecutionTime:{1}||Name:{2}", userId, executionTime, scriptName);
                string messageFormatDetail = string.Format("UserName:{0}||ExecutionTime:{1}||Name:{2}||Parameters:{3}||Output:{4}", userId, executionTime, scriptName, parameterStr, output);


                if (logProvider == LogProvider.EnterpriseLibrary)
                {
                    WriteLogUsingEnterpriseLibrary(messageFormatBrief, 5, "ScriptTracking", System.Diagnostics.TraceEventType.Information, (int)BusinessFunctionConstants.SCRIPTEXECUTE, null);
                    LogDebug(messageFormatDetail, Layer.ScriptEngine);
                }
                else
                {
                    WriteLogUsingLog4NetAsync(messageFormatBrief, LogPriority.P5, LogCategory.ScriptTracking, System.Diagnostics.TraceEventType.Information, (int)BusinessFunctionConstants.SCRIPTEXECUTE, null);
                    LogDebug(messageFormatDetail, Layer.ScriptEngine);
                }
            }
            catch (Exception) { }
        }

        public static void WorkflowTracking(string userId, string executionTime, string workflowName, Dictionary<string, string> parameters, string output)
        {
            try
            {
                string parameterStr = "";
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (System.Collections.Generic.KeyValuePair<string, string> parameter in parameters)
                    {
                        parameterStr = parameterStr + parameter.Key + "=" + parameter.Value + "&&";
                    }
                    parameterStr = parameterStr.Substring(0, parameterStr.LastIndexOf("&&"));
                }
                string messageFormatBrief = string.Format("UserName:{0}||ExecutionTime:{1}||Name:{2}", userId, executionTime, workflowName);
                string messageFormatDetail = string.Format("UserName:{0}||ExecutionTime:{1}||Name:{2}||Parameters:{3}||Output:{4}", userId, executionTime, workflowName, parameterStr, output);

                if (logProvider == LogProvider.EnterpriseLibrary)
                {
                    WriteLogUsingEnterpriseLibrary(messageFormatBrief, 6, "WFTracking", System.Diagnostics.TraceEventType.Information, (int)BusinessFunctionConstants.WFEXECUTE, null);
                    LogDebug(messageFormatDetail, Layer.WFEngine);
                }
                else
                {
                    WriteLogUsingLog4NetAsync(messageFormatBrief, LogPriority.P6, LogCategory.WFTracking, System.Diagnostics.TraceEventType.Information, (int)BusinessFunctionConstants.WFEXECUTE, null);
                    LogDebug(messageFormatDetail, Layer.WFEngine);
                }
            }
            catch 
            {
                //just kill the exception and keep trying    
            }
        }
     
        public static void TrackUsageAsync(string moduleName = "", string functionName = "", string userAlias = "", string machineName = "", string machineIP = "",
            string appVersion = "", string transactionID = "", string data = "")
        {
            Boolean serverReachable = CheckServerReachability();
            ILog logger = null;

            if (serverReachable)
            {
                if (trackUsageOnlineLogger == null)
                    logger = LogManager.GetLogger("TrackUsageOnline");
                else logger = trackUsageOnlineLogger;

            }
            else
            {
                if (trackUsageOfflineLogger == null)
                {
                    //Set up Logger
                    SetUpOfflineLogger();
                    logger = LogManager.GetLogger("TrackUsageOffline");
                }
                else logger = trackUsageOfflineLogger;

            }
            var t = new Thread(() => TrackUsage(logger, moduleName, functionName, userAlias, machineName, machineIP, appVersion, transactionID, data));
            t.Start();
        }

        private static void TrackUsage(ILog logger, string moduleName = "", string functionName = "", string userAlias = "", string machineName = "", string machineIP = "",
            string appVersion = "", string transactionID = "", string data = "", string hostName = "", string timestamp = "", string is64Bit = "")
        {
            log4net.ThreadContext.Properties["TransactionID"] = transactionID;
            if (string.IsNullOrEmpty(timestamp))
            {
                log4net.ThreadContext.Properties["Timestamp"] = DateTime.UtcNow.ToString();
            }
            else log4net.ThreadContext.Properties["Timestamp"] = timestamp;

            if (string.IsNullOrEmpty(hostName))
            {
                log4net.ThreadContext.Properties["HostName"] = AppDomain.CurrentDomain.FriendlyName;
            }
            else log4net.ThreadContext.Properties["HostName"] = hostName;

            if (string.IsNullOrEmpty(is64Bit))
            {
                log4net.ThreadContext.Properties["Is64Bit"] = Environment.Is64BitOperatingSystem; ;
            }
            else log4net.ThreadContext.Properties["Is64Bit"] = is64Bit;


            if (string.IsNullOrEmpty(userAlias))
                log4net.ThreadContext.Properties["UserAlias"] = Environment.UserName;
            else
                log4net.ThreadContext.Properties["UserAlias"] = userAlias;
            if (string.IsNullOrEmpty(machineName))
                log4net.ThreadContext.Properties["MachineName"] = Environment.MachineName;
            else
                log4net.ThreadContext.Properties["MachineName"] = machineName;
            if (string.IsNullOrEmpty(machineIP))
                log4net.ThreadContext.Properties["MachineIP"] = GetIP();
            else
                log4net.ThreadContext.Properties["MachineIP"] = machineIP;
            if (string.IsNullOrEmpty(appVersion))
                log4net.ThreadContext.Properties["ApplicationVersion"] = System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion;
            else
                log4net.ThreadContext.Properties["ApplicationVersion"] = appVersion;
            if (!string.IsNullOrEmpty(moduleName))
                log4net.ThreadContext.Properties["Module"] = moduleName;
            if (!string.IsNullOrEmpty(functionName))
                log4net.ThreadContext.Properties["FunctionName"] = functionName;
            if (!string.IsNullOrEmpty(data))
                log4net.ThreadContext.Properties["Data"] = data;


            //Message to log cannot be empty, hence using dummy message, dummy message is not getting logged
            logger.Info("messageDummy");
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

        private static void SetUpOfflineLogger()
        {
            RemoveAppender("TrackUsageOfflineAppender");

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\trkrusg\iap_trackusage.log";
            RollingFileAppender appen = CreateRollingFileAppender("TrackUsageOfflineAppender", filePath,
                "TrackUsageOffline", "Timestamp,UserAlias,MachineName,MachineIP,HostName,ApplicationVersion,Is64Bit,Module,FunctionName,TransactionID,Data",
                "%newline%property{Timestamp},%property{UserAlias},%property{MachineName},%property{MachineIP},%property{HostName},%property{ApplicationVersion},%property{Is64Bit},%property{Module},%property{FunctionName},%property{TransactionID},%property{Data}");
            appen.ActivateOptions();
            hierarchy.Root.AddAppender(appen);

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            hierarchy.Root.AddAppender(memory);

            hierarchy.Root.Level = Level.All;
            hierarchy.Configured = true;
        }

        private static void ShutdownOfflineLogger()
        {
            ILog offlineLogger = LogManager.GetLogger("TrackUsageOffline");
            if (offlineLogger != null)
                offlineLogger.Logger.Repository.Shutdown();
        }

        public static void SynchronizeOfflineTrackUsageLogsWithDB()
        {
            var t = new Thread(() => SynchronizeOfflineTrackUsageLogsWithDB2());
            t.Start();
        }

        private static void SynchronizeOfflineTrackUsageLogsWithDB2()
        {
            //Check if server is reachable
            Boolean serverReachable = CheckServerReachability();
            if (serverReachable)
            {
                //checking if any existing log files in folder
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\trkrusg\";
                // string appDirectory = @"D:\ATR-FEA\Code\WorkflowExecutionManager\TestCustomTraceListener\bin\Debug\trkrusg";

                if (Directory.Exists(appDirectory))
                {
                    //Pick all log files for synchronization
                    string[] files = Directory.GetFiles(appDirectory);

                    //Read each file
                    for (int i = 0; i < files.Length; i++)
                    {
                        string filePath = files[i];

                        string[] allLines = File.ReadAllLines(filePath);
                        {

                            var query = from line in allLines
                                        let data = line.Split(',')
                                        select new
                                        {
                                            Timestamp = data[0],
                                            UserAlias = data[1],
                                            MachineName = data[2],
                                            MachineIP = data[3],
                                            HostName = data[4],
                                            ApplicationVersion = data[5],
                                            Is64Bit = data[6],
                                            Module = data[7],
                                            FunctionName = data[8],
                                            TransactionID = data[9],
                                            Data = data[10]
                                        };

                            //Insert each log in DB except Header
                            Boolean isHeader = true;
                            int j = 0;
                            foreach (var dat in query)
                            {
                                if (isHeader)
                                {
                                    isHeader = false;
                                    j += 1;
                                    continue;
                                }

                                if (trackUsageOnlineLogger == null)
                                    trackUsageOnlineLogger = LogManager.GetLogger("TrackUsageOnline");

                                TrackUsage(trackUsageOnlineLogger, dat.Module, dat.FunctionName, dat.UserAlias, dat.MachineName, dat.MachineIP, dat.ApplicationVersion,
                                    dat.TransactionID, dat.Data, dat.HostName, dat.Timestamp, dat.Is64Bit);
                                j += 1;
                            }

                        }

                        //Delete log file
                        try
                        {
                            File.Delete(filePath);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    //Completed reading all files, close logger
                    ShutdownOfflineLogger();
                }
            }
        }
    }

    public class HeaderOnceAppender : RollingFileAppender
    {
        protected override void WriteHeader()
        {
            if (LockingModel.AcquireLock().Length == 0)
            {
                base.WriteHeader();
            }
        }
    }

    public enum LogCategory
    {
        General = 1,
        Performance,
        Statistics,
        MessageArchive,
        ScriptTracking,
        WFTracking,
        Trace
    }

    public enum LogPriority
    {
        P2 = 2,
        P3 = 3,
        P5 = 5,
        P6 = 6,
        P10 = 10,
        P11 = 11,
        P15 = 15,
        P20 = 20,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class Log4NetTraceListener : CustomTraceListener
    {
        static ILog logger = null;
        private LogEntry CustomData { get; set; }
        private TraceEventType EventType { get; set; }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data is LogEntry)
            {
                this.CustomData = data as LogEntry;
                this.EventType = eventType;
                this.WriteLine(this.Formatter.Format(data as LogEntry));
            }
        }

        public override void Write(string message)
        {
            //TODO
            //Debug.Write(message);
        }

        public override void WriteLine(string message)
        {
            LogEntry local = this.CustomData;
            TraceUsingLog4Net(local);
        }

        private void TraceUsingLog4Net(LogEntry local)
        {
            log4net.Config.XmlConfigurator.Configure();

            log4net.ThreadContext.Properties["pid"] = Process.GetCurrentProcess().Id;
            log4net.ThreadContext.Properties["pname"] = Process.GetCurrentProcess().ProcessName;
            log4net.ThreadContext.Properties["eventId"] = local.EventId;
            log4net.ThreadContext.Properties["severity"] = local.Severity;
            log4net.ThreadContext.Properties["threadName"] = Thread.CurrentThread.Name;
            log4net.ThreadContext.Properties["title"] = "";
            log4net.ThreadContext.Properties["formattedMessage"] = null;
            log4net.ThreadContext.Properties["Timestamp"] = DateTime.UtcNow;
            log4net.ThreadContext.Properties["Priority"] = local.Priority;

            if (EventType == TraceEventType.Error)
            {
                if (logger == null)
                {
                    logger = LogManager.GetLogger(Log4NetConstants.ExceptionsLogger);
                }
            }
            else if (EventType == TraceEventType.Start || EventType == TraceEventType.Stop)
            {
                if (logger == null)
                {
                    logger = LogManager.GetLogger(Log4NetConstants.PerformanceLogger);
                }
            }

            logger.Info(local.Message);
        }
    }
}
