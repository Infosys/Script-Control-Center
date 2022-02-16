/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Activities;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using System.Activities.Presentation;
using InfrastructureClientLibrary;
using System.Net;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using log4net.Repository.Hierarchy;
using log4net.Appender;
using log4net.Layout;
using log4net;
using log4net.Core;
using Infosys.WEM.SecureHandler;
using IAP.Infrastructure.Services.Contracts;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;

namespace Infosys.WEM.AutomationActivity.Libraries.Utilities
{
    public class InitializeLogger : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("1 for Database, 2 for Text File")]
        public InArgument<int> LogRepositoryTarget { get; set; }

        [Description("For LogStore as DB specify service URL, else leave blank")]
        public InArgument<string> ServerURL { get; set; }

        [Description("For LogStore as Text File specify folder path for logging errors, else leave blank")]
        public InArgument<string> LogErrorsFolderPath { get; set; }

        [Description("For LogStore as Text File specify folder path for logging audits, else leave blank")]
        public InArgument<string> LogAuditsFolderPath { get; set; }

        [Description("For LogStore as Text File specify folder path for logging messages, else leave blank")]
        public InArgument<string> LogMessagesFolderPath { get; set; }

        [RequiredArgument]
        [Description("Object containing Log settings")]
        public OutArgument<LogStore> LogStore { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES);

                    if (context.GetValue(LogRepositoryTarget) == 2)
                    {
                        if (context.GetValue(LogErrorsFolderPath) == null && context.GetValue(LogAuditsFolderPath) == null && context.GetValue(LogMessagesFolderPath) == null)
                        {
                            //throw exception as this is mandatory input parameter
                            throw new System.Exception("Provide paths for LogMessagesFolderPath, LogErrorsFolderPath, LogAuditsFolderPath");
                        }
                    }

                    if (context.GetValue(LogRepositoryTarget) == 1)
                    {
                        if (context.GetValue(ServerURL) == null || context.GetValue(ServerURL) == string.Empty)
                        {
                            //throw exception as this is mandatory input parameter
                            throw new System.Exception("Provide server name");
                        }
                    }

                    LogStore store = new LogStore();

                    store.LogRepositoryTarget = Convert.ToInt32(context.GetValue(LogRepositoryTarget));
                    store.ServerURL = context.GetValue(ServerURL);
                    store.LogErrorsFolderPath = context.GetValue(LogErrorsFolderPath);
                    store.LogAuditsFolderPath = context.GetValue(LogAuditsFolderPath);
                    store.LogMessagesFolderPath = context.GetValue(LogMessagesFolderPath);

                    LogStore.Set(context, store);

                    if (store.LogRepositoryTarget == 2)
                    {
                        // config is dynamic or static check
                        string config = System.Configuration.ConfigurationManager.AppSettings["Log4NetStaticConfig"];
                        if (config == null)
                        {
                            LogHandler.SetupLog4Net(context.GetValue(LogErrorsFolderPath), context.GetValue(LogAuditsFolderPath), context.GetValue(LogMessagesFolderPath));
                        }
                        else
                        {
                            log4net.Config.XmlConfigurator.Configure();
                        }
                    }

                    //in param 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "LogStore", Designer.ApplicationConstants.PARAMDIRECTION_IN, LogRepositoryTarget);
                    //in param 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "LogRepositoryTarget", Designer.ApplicationConstants.PARAMDIRECTION_IN, LogRepositoryTarget);
                    //in param 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "LogErrorsFolderPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, LogErrorsFolderPath);
                    //in param 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "LogAuditsFolderPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, LogAuditsFolderPath);
                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "LogMessagesFolderPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, LogMessagesFolderPath);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new InitializeLogger
            {
                DisplayName = "Initialize Log Store",
            };
        }
    }

    public class LogError : NativeActivity, IActivityTemplateFactory
    {
        private string ticketNumber = string.Empty;
        private int notificationType = 0;

        public InArgument<string> TicketNumber { get; set; }
        [RequiredArgument]
        [Description("Unique Script Id of the Workflow")]
        public InArgument<string> ScriptId { get; set; }
        [RequiredArgument]
        public InArgument<string> Message { get; set; }
        [RequiredArgument]
        public InArgument<string> ErrorDetails { get; set; }
        [RequiredArgument]
        [Description("Set this to True, if email notification should be sent")]
        public InArgument<bool> NotifyAdmin { get; set; }
        [Description("Seperate multiple recipients by comma")]
        public InArgument<string> Recipients { get; set; }
        [RequiredArgument]
        [Description("Object containing Log store settings")]
        public InArgument<LogStore> LogStore { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES);


                    //in param                 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "TicketNumber", Designer.ApplicationConstants.PARAMDIRECTION_IN, TicketNumber);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ParentScriptId", Designer.ApplicationConstants.PARAMDIRECTION_IN, ScriptId);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Message", Designer.ApplicationConstants.PARAMDIRECTION_IN, Message);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ErrorDetails", Designer.ApplicationConstants.PARAMDIRECTION_IN, ErrorDetails);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Recipients", Designer.ApplicationConstants.PARAMDIRECTION_IN, Recipients);

                    ticketNumber = context.GetValue(TicketNumber);

                    //logic
                    LoggerClient client = new LoggerClient();
                    Log log = new Log();
                    log.TicketNumber = context.GetValue(TicketNumber);
                    log.CreatedBy = Environment.UserName;
                    log.MachineName = Environment.MachineName;
                    log.ScriptId = context.GetValue(ScriptId);
                    if (ErrorDetails != null)
                        log.ErrorDetails = context.GetValue(ErrorDetails);
                    log.ErrorTypeID = Convert.ToInt32(ErrorType.Critical);

                    log.Message = context.GetValue(Message);
                    if (context.GetValue(NotifyAdmin) == true)
                    {
                        log.NotificationType = NotificationType.Error;
                        log.Recipients = context.GetValue(Recipients);
                    }
                    log.MachineIP = Dns.GetHostByName(log.MachineName).AddressList[0].ToString();

                    LogStore store = context.GetValue(LogStore);


                    int loggingListener = store.LogRepositoryTarget;
                    if (loggingListener == 1)
                    {
                        string ServiceBaseUrl = store.ServerURL;
                        string ServiceURL = "/iapinfrastructureservices/Logger.svc";
                        client.LogError(ServiceBaseUrl + ServiceURL, log);
                    }
                    else if (loggingListener == 2)
                    {
                        StringBuilder msg = new StringBuilder();
                        msg.Append(context.GetValue(TicketNumber) + ",");
                        msg.Append(DateTime.Now.ToString() + ",");
                        msg.Append(context.GetValue(ScriptId) + ",");
                        msg.Append(log.Message + ",");
                        msg.Append(context.GetValue(ErrorDetails) + ",");
                        msg.Append(log.ErrorTypeID.ToString() + ",");
                        msg.Append(Environment.UserName + ",");
                        msg.Append(log.MachineName + ",");
                        msg.Append(log.MachineIP + ",");

                        LogHandler.WriteActivitiesError(msg.ToString());
                    }
                    else
                    {
                        //throw exception as this is mandatory input parameter
                        throw new System.Exception("Initialize LogStore using InitializeLogStore activity");
                    }
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new LogError
            {
                DisplayName = "Log Error",
            };
        }
    }

    public class LogAudit : NativeActivity, IActivityTemplateFactory
    {
        public InArgument<string> TicketNumber { get; set; }
        [Description("Refer to the API user guide for the list of ticket state codes.")]
        public InArgument<int> TicketStateId { get; set; }
        [RequiredArgument]
        [Description("Unique Script Id of the Workflow")]
        public InArgument<string> ScriptId { get; set; }
        [RequiredArgument]
        public InArgument<string> Message { get; set; }
        [Description("Data that should be audited")]
        public InArgument<List<LogData>> LogData { get; set; }
        [RequiredArgument]
        [Description("Object containing Log store settings")]
        public InArgument<LogStore> LogStore { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.LOGAUDIT, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId, ActivityEvents.LOGAUDIT, ActivityControls.UTILITIES);

                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "TicketNumber", Designer.ApplicationConstants.PARAMDIRECTION_IN, TicketNumber);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ParentScriptId", Designer.ApplicationConstants.PARAMDIRECTION_IN, ScriptId);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Message", Designer.ApplicationConstants.PARAMDIRECTION_IN, Message);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "LogData", Designer.ApplicationConstants.PARAMDIRECTION_IN, LogData);

                    //logic
                    LoggerClient client = new LoggerClient();
                    Log log = new Log();
                    log.TicketNumber = context.GetValue(TicketNumber);
                    log.CreatedBy = Environment.UserName;
                    log.MachineName = Environment.MachineName;
                    log.ScriptId = context.GetValue(ScriptId);
                    log.StateId = 1;
                    log.Data = context.GetValue(LogData);
                    log.Message = context.GetValue(Message);
                    log.MachineIP = Dns.GetHostByName(log.MachineName).AddressList[0].ToString();

                    LogStore store = context.GetValue(LogStore);

                    int loggingListener = store.LogRepositoryTarget;

                    if (loggingListener == 1)
                    {
                        string ServiceBaseUrl = store.ServerURL;
                        string ServiceURL = "/iapinfrastructureservices/Logger.svc";
                        client.LogAudit(ServiceBaseUrl + ServiceURL, log);
                    }
                    else if (loggingListener == 2)
                    {
                        StringBuilder msg = new StringBuilder();

                        msg.Append(context.GetValue(TicketNumber) + ",");
                        msg.Append(context.GetValue(TicketStateId) + ",");
                        msg.Append(context.GetValue(ScriptId) + ",");
                        msg.Append(log.Message + ";");

                        if (context.GetValue(LogData) != null)
                            foreach (LogData logData in context.GetValue(LogData))
                            {
                                msg.Append(logData.Key + ": " + logData.Value + ";");
                            }

                        msg.Append("," + Environment.UserName + ",");
                        msg.Append(DateTime.Now.ToString() + ",");
                        msg.Append(log.MachineName + ",");
                        msg.Append(log.MachineIP + ",");

                        LogHandler.WriteActivitiesAudit(msg.ToString());
                    }
                    else
                    {
                        //throw exception as this is mandatory input parameter
                        throw new System.Exception("Initialize LogStore using InitializeLogStore activity");
                    }
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new LogAudit
            {
                DisplayName = "Log Audit",
            };
        }
    }

    public class Notify : NativeActivity, IActivityTemplateFactory
    {
        public InArgument<string> TicketNumber { get; set; }
        [RequiredArgument]
        [Description("Script Id of the Workflow")]
        public InArgument<string> ScriptId { get; set; }
        [RequiredArgument]
        public InArgument<string> Message { get; set; }
        [RequiredArgument]
        [Description("Seperate multiple recipients by comma")]
        public InArgument<string> Recipients { get; set; }
        [Description("Specify file complete path")]
        public InArgument<List<string>> Attachments { get; set; }
        [Description("Refer to the API user guide for the list of ticket state codes.")]
        public InArgument<int> TicketStateId { get; set; }
        [RequiredArgument]
        [Description("Login=1, Information=4, Custom=5")]
        public InArgument<int> NotificationType { get; set; }
        [Description("Set this only if sending a custom notification")]
        public InArgument<string> CustomNotificationSubject { get; set; }
        [Description("Set this only if sending a custom notification")]
        public InArgument<string> CustomNotificationBody { get; set; }
        [RequiredArgument]
        [Description("Object containing Log store settings")]
        public InArgument<LogStore> LogStore { get; set; }
        [Description("Result")]
        public OutArgument<bool> Status { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.NOTIFY, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId, ActivityEvents.NOTIFY, ActivityControls.UTILITIES);



                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "TicketNumber", Designer.ApplicationConstants.PARAMDIRECTION_IN, TicketNumber);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "ParentScriptId", Designer.ApplicationConstants.PARAMDIRECTION_IN, ScriptId);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Message", Designer.ApplicationConstants.PARAMDIRECTION_IN, Message);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "StateID", Designer.ApplicationConstants.PARAMDIRECTION_IN, TicketStateId);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "notificationType", Designer.ApplicationConstants.PARAMDIRECTION_IN, NotificationType);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "Recipients", Designer.ApplicationConstants.PARAMDIRECTION_IN, Recipients);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "Attachments", Designer.ApplicationConstants.PARAMDIRECTION_IN, Attachments);

                    LogStore store = context.GetValue(LogStore);

                    //logic
                    LoggerClient client = new LoggerClient();
                    Log log = new Log();
                    log.TicketNumber = context.GetValue(TicketNumber);
                    log.CreatedBy = Environment.UserName;
                    log.MachineName = Environment.MachineName;
                    log.ScriptId = context.GetValue(ScriptId);
                    log.StateId = context.GetValue(TicketStateId);
                    log.NotificationType = (NotificationType)context.GetValue(NotificationType);
                    log.Message = context.GetValue(Message);
                    log.MachineIP = Dns.GetHostByName(log.MachineName).AddressList[0].ToString();

                    log.Recipients = context.GetValue(Recipients);
                    log.CustomNotificationBody = context.GetValue(CustomNotificationBody);
                    log.CustomNotificationSubject = context.GetValue(CustomNotificationSubject);
                    log.AttachmentFileNames = context.GetValue(Attachments);

                    if (store.LogRepositoryTarget != 1)
                    {
                        throw new System.Exception("Set Log store as Database in InitializeLogger activity");
                    }

                    string ServiceBaseUrl = store.ServerURL;
                    string ServiceURL = "/iapinfrastructureservices/Logger.svc";


                    bool result = client.Notify(ServiceBaseUrl + ServiceURL, log);
                    Status.Set(context, result);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new Notify
            {
                DisplayName = "Notify",
            };
        }
    }

    public class SendEmail : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("SMTP server name or IP address")]
        public InArgument<string> Server { get; set; }       
        [Description("SMTP server port")]
        public InArgument<int> Port { get; set; }
        [RequiredArgument]
        public InArgument<string> Subject { get; set; }
        [RequiredArgument]
        [Description("This uses plain text/HTML format")]
        public InArgument<string> Body { get; set; }
        [Description("Specify file complete path")]
        public InArgument<List<string>> Attachments { get; set; }
        [RequiredArgument]
        public InArgument<string> FromEmailId { get; set; }
        [Description("Password for the FromEmailId")]
        public InArgument<string> EncryptedPassword { get; set; }
        [RequiredArgument]
        [Description("Seperate multiple recipients by comma")]
        public InArgument<string> Recipients { get; set; }
        [Description("Time-out value in milliseconds, The default value is 100,000 (100 seconds)")]
        public InArgument<int> Timeout { get; set; }
        [Description("Specify if SSL is used to access the specified SMTP mail server, default is False")]
        public InArgument<bool> EnableSsl { get; set; }
        [Description("Result")]
        public OutArgument<bool> Status { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.NOTIFY, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId, ActivityEvents.NOTIFY, ActivityControls.UTILITIES);



                    //in param
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Subject", Designer.ApplicationConstants.PARAMDIRECTION_IN, Subject);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Body", Designer.ApplicationConstants.PARAMDIRECTION_IN, Body);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Server", Designer.ApplicationConstants.PARAMDIRECTION_IN, Server);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Port", Designer.ApplicationConstants.PARAMDIRECTION_IN, Port);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "From", Designer.ApplicationConstants.PARAMDIRECTION_IN, FromEmailId);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "Recipients", Designer.ApplicationConstants.PARAMDIRECTION_IN, Recipients);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                              context.ActivityInstanceId, "Attachments", Designer.ApplicationConstants.PARAMDIRECTION_IN, Attachments);


                    //logic
                    InfrastructureClientLibrary.SendMail client = new InfrastructureClientLibrary.SendMail();
                    int port = context.GetValue(Port) == null ? 0 : context.GetValue(Port);
                    bool result = client.Send(context.GetValue(Subject), context.GetValue(Body), context.GetValue(Attachments),
                        context.GetValue(FromEmailId), context.GetValue(Recipients), context.GetValue(Server), port, context.GetValue(EncryptedPassword),
                        context.GetValue(Timeout), context.GetValue(EnableSsl));

                    Status.Set(context, result);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new SendEmail
            {
                DisplayName = "SendEMail",
            };
        }
    }

    public class WriteToFile : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Message to be logged")]
        public InArgument<string> Message { get; set; }
        [RequiredArgument]
        [Description("Object containing Log store settings")]
        public InArgument<LogStore> LogStore { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES);

                    //in param 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Message", Designer.ApplicationConstants.PARAMDIRECTION_IN, Message);
                    //in param 
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Message", Designer.ApplicationConstants.PARAMDIRECTION_IN, Message);

                    //logic
                    LogStore store = context.GetValue(LogStore);

                    int loggingListener = store.LogRepositoryTarget;

                    if (loggingListener != 2)
                    {
                        //throw exception as this is mandatory input parameter
                        throw new System.Exception("Set LogRepositoryTarget to Text file using InitializeLogger activity");
                    }
                    else
                    {
                        LogHandler.WriteActivitiesInfo(context.GetValue(Message) + "," + DateTime.Now.ToString());
                    }
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new WriteToFile
            {
                DisplayName = "Write to File",
            };
        }
    }

    public class ShutdownLogger : NativeActivity, IActivityTemplateFactory
    {
        //    [RequiredArgument]
        //    [Description("Object containing Log store settings")]
        //    public InArgument<LogStore> LogStore { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            LogHandler.DisposeLog4Net();
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new ShutdownLogger
            {
                DisplayName = "Shutdown Log store",
            };
        }
    }

    public sealed class Encrypt : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Enter Text to be Encrypted")]
        public InArgument<string> Text { get; set; }
        [Description("Encrypted Text")]
        public OutArgument<string> SecuredString { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.ENCRYPT, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.ENCRYPT, ActivityControls.UTILITIES);

                    SecuredString.Set(context, SecurePayload.Secure(context.GetValue(Text), "IAP2GO_SEC!URE"));

                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new Encrypt
            {
                DisplayName = "Secure String",
            };
        }
    }

    public sealed class Decrypt : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Enter Text to be Decrypted")]
        public InArgument<string> Text { get; set; }
        [Description("Decrypted Text")]
        public OutArgument<string> UnSecuredString { get; set; }
        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.DECRYPT, ActivityControls.UTILITIES))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.DECRYPT, ActivityControls.UTILITIES);
                    string s = SecurePayload.UnSecure(context.GetValue(Text), "IAP2GO_SEC!URE");
                    UnSecuredString.Set(context, s);

                }


                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new Decrypt
            {
                DisplayName = "UnSecure String",
            };
        }
    }

    public class LogStore
    {
        public int LogRepositoryTarget { get; set; }
        public string ServerURL { get; set; }
        public string LogErrorsFolderPath { get; set; }
        public string LogMessagesFolderPath { get; set; }
        public string LogAuditsFolderPath { get; set; }
    }

    public class DoInteractiveCheck : NativeActivity, IActivityTemplateFactory
    {
        [Description("Message to be displayed in the Confirmation window")]
        public InArgument<string> Message { get; set; }

        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        private AutomationFacade automationObject;

        protected override void Execute(NativeActivityContext context)
        {
            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.INTERACTIVECHECK, ActivityControls.UTILITIES))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES);

                //in param 
                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, "Message", Designer.ApplicationConstants.PARAMDIRECTION_IN, Message);
                automationObject = context.GetValue(this.AutomationObject);
                automationObject.DoInteractiveCheck(context.GetValue(this.Message));
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new DoInteractiveCheck
            {
                DisplayName = "InteractiveCheck",
            };
        }
    }
}
