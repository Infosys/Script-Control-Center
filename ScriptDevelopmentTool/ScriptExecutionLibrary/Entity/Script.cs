/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    [Serializable]
    public class Script : ICloneable
    {
        public string ScriptName { get; set; }
        public string ExecutionDir { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string TaskCmd { get; set; }
        public string TaskType { get; set; }
        public bool RunAsAdmin { get; set; }
        public string IfeaScriptName { get; set; }
        public string MethodName { get; set; }

        #region ICloneable Members

        public object Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (this.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    return formatter.Deserialize(stream);
                }
                return null;
            }
        }

        #endregion
    }

    [Serializable]
    public class Parameter
    {
        public string DataType { get; set; }
        public bool IsPaired { get; set; }
        public bool IsSecret { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string allowedValues { get; set; }
    }

    public class ExecutionResult
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public string ComputerName { get; set; }
        public string InputCommand { get; set; }

        public Guid TransactionId { get; set; }
        public List<OutParameter> Output { get; set; }
        public List<string> ScheduledRequestIds { get; set; }
        public string Status { get; set; }
        public string SourceTransactionId { get; set; }
        public string LogData { get; set; }
    }

    public class OutParameter
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
    }

    public class ClientInitializationResult
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ScriptIndentifier
    {
        public int ScriptId { get; set; }
        public int CompanyId { get; set; }
        /// <summary>
        /// Needed as sub category id the partion key in the Script table
        /// </summary>
        public int SubCategoryId { get; set; }
        public string ResponseNotificationCallbackURL { get; set; }
        public string WEMScriptServiceUrl { get; set; }
        public string ScriptName { get; set; }
        /// <summary>
        /// Optional list of parameters to be used to override the default parameter list, if any
        /// </summary>
        public List<Parameter> Parameters { get; set; }

        //Property to hold user name for running script on remote machine under different account
        public string UserName { get; set; }

        //Property to hold password for running script on remote machine under different account
        public SecureString Password { get; set; }

        // Remote server names separated by comma
        public string RemoteServerNames { get; set; }

        //property to hold value for RemoteExecutionHost Enum
        public RemoteExecutionHost RemoteExecutionMode;
        public string WorkingDir { get; set; }

        public Guid TransactionId { get; set; }
        public string LinuxKeyPath { get; set; }
        public ExecutionModeType ExecutionMode { get; set; }
        public ScheduledPatternType ScheduledPattern { get; set; }
        public DateTime ScheduleStartDateTime { get; set; }
        public DateTime ScheduleEndDateTime { get; set; }
        public ScheduleStopCriteriaType ScheduleStopCriteria { get; set; }
        public int ScheduleOccurences { get; set; }
        public int SchedulePriority { get; set; }
        public List<string> ScheduleOnClusters { get; set; }
        public IapNodeTransportType IapNodeTransport { get; set; }
        public int IapNodeHttpPort { get; set; }
        public int IapNodeNetTcpPort { get; set; }
        public string Domain { get; set; }
        public bool UsesUIAutomation { get; set; }
        /// <summary>
        /// This is an optional property to link to any business usecase like ticket id, etc
        /// </summary>
        public string ReferenceKey { get; set; }
        public string Path { get; set; }

        public string Operation { get; set; }


        public enum RemoteExecutionHost
        {
            PS = 1,
            IAPNodes = 2,
            Linux = 3
        };
    }

    public enum IapNodeTransportType
    {
        Http = 1,
        Nettcp = 2
    }

    public enum ScheduleStopCriteriaType
    {
        NoEndDate = 1,
        OccurenceCount = 2,
        EndDate = 3
    }

    public enum ScheduledPatternType
    {
        ScheduleNow = 1,
        ScheduleWithRecurrence = 2
    }

    public enum ExecutionModeType
    {
        Local = 1,
        RunOnIAPNode = 2,
        ScheduledOnIAPNode = 3,
        ScheduledOnIAPCluster = 4,
        PowerShell = 5,
        Linux = 6,
        Delegate = 7
    }

    public enum Status
    {
        initial,
        queued,
        success,
        failed
    }

}
