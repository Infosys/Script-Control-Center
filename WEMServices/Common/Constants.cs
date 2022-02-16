/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
namespace Infosys.WEM.Infrastructure.Common
{

    /// <summary>
    ///This should be used only for internal application usage purposes so as to avoid hardcoding values 
    ///directly inline with code such that any change in value can be controlled from one location. 
    ///Also no user displayed text messages have to be placed here they need to be placed in the Resource (Resx) files
    /// </summary>
    public struct ApplicationConstants
    {

        public const string APP_NAME = "WEM";
        public const string DOCUMENTSTORE_KEY = "WorkflowRepository";
        public const string SCRIPT_STORE_KEY = "ScriptRepository";
        public const string TWOFIELD_KEY_FORMAT = "{0}_{1}";
        public const string SERVICE_EXCEPTIONHANDLING_POLICY = "WEM.SERVICE";
        public const string SCRIPT_REPO_SERVICEINTERFACE = "/iapwemservices/WEMScriptService.svc";
        public const string COMMON_SERVICEINTERFACE = "/iapwemservices/WEMCommonService.svc";
        public const string SECURE_PASSCODE = "IAP2GO_SEC!URE";
        public const string ROLE_MANAGER = "Manager";
        public const string ROLE_ANALYST = "Analyst";
        public const string ROLE_AGENT = "Agent";
        public const string ROLE_GUEST = "Guest";
        public const string DOMAIN_USERS = "domain users";
    }

    public struct SplitConstants
    {
        public const char COMMA_DELIMITER = ',';
        public const char DOT_DELIMITER = '.';
        public const char SEMICOLON_DELIMITER = ';';
        public const char COLON_DELIMITER = ':';
        public const string JOIN_DELIMITER = ",";
        public const string JOIN_SEMICOLON = ";";
    }

    public class Log4NetConstants
    {
        public const string ErrorsLogger = "ErrorsLogger";
        public const string AuditsLogger = "AuditsLogger";
        public const string MessagesLogger = "MessagesLogger";
        public const string ErrorsRollingFile = "ErrorsRollingFile";
        public const string AuditsRollingFile = "AuditsRollingFile";
        public const string MessagesRollingFile = "MessagesRollingFile";
        public const string ErrorsHeader = "TicketNumber,StateId,ScriptId,Message,LogData,CreatedBy,TimeStamp,CreatedFromMachineName,CreatedFromMachineIP";
        public const string AuditsHeader = "TicketNumber,StateId,ScriptId,Message,LogData,CreatedBy,TimeStamp,CreatedFromMachineName,CreatedFromMachineIP";
        public const string MessagesHeader = "Message,TimeStamp";

        public const string GeneralLogger = "GeneralLogger";
        public const string DatabaseTraceListener = "Database Trace Listener";
        public const string PerformanceLogger = "PerformanceLogger";
        public const string ExceptionsLogger = "ExceptionsLogger";
        public const string StatisticsLogger = "StatisticsLogger";
        public const string MessageLogger = "MessageLogger";
        public const string ScriptTrackingLogger = "ScriptTrackingLogger";
        public const string TraceLogger = "TraceLogger";
        public const string WFTracking = "WFTrackingLogger";
    }

    public struct ECRServiceConstants
    {
        public const string CasServerUriFormat = "https://{0}/cas/v1/tickets";
        public const string ECRServerUriFormat = "http://{0}/iap-controller/iap-client";
        public const string BrosweCategoryUriFormat = "http://{0}/iap-controller/spring/category/browse";
        public const string AddECRScriptCategory = "http://{0}/iap-controller/spring/category/add";
        public const string GetScriptByCategoryUrl = "http://{0}/iap-controller/spring/script/fetch/ScriptsBycategory";
    }
}
