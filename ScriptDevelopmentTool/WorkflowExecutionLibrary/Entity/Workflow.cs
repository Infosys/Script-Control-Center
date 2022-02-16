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
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.WorkflowExecutionLibrary.Entity
{    
    public class WorkflowIndentifier
    {
        public Guid WorkflowId { get; set; }
        public int CategoryId { get; set; }
        public string WEMWorkflowServiceUrl { get; set; }
        public int WorkflowVersion { get; set; }
        /// <summary>
        /// Optional list of parameters to be used in future, if any
        /// </summary>
        public List<Parameter> Parameters { get; set; }
        public Guid Identifier { get; set; }
        public string path { get; set; }

        //the below properties are to support state management
        //public bool PersistWorkflowState { get; set; } // this property is now available in the workflow metadata
        public string LastWorkflowStateId { get; set; }

        bool _unloadWorkflowFromMemory = true;
        /// <summary>
        /// In case the workflow is to be unloaded from memory after the execution and state persistence. Default value assumed is false in the workflow execution engine
        /// </summary>
        public bool UnloadWorkflowFromMemory
        {
            get { return _unloadWorkflowFromMemory; }
            set { _unloadWorkflowFromMemory = value; }
        }

        /// <summary>
        /// This is an optional property to link to any business usecase like ticket id, etc
        /// </summary>
        public string ReferenceKey { get; set; }

        public string TransactionInstanceId { get; set; } //to be used for transaction management while resuming a paused workflow

        public string TransDescription { get; set; } //to be used for transaction description 
        public string TransactionMetadata { get; set; } // to be used to store the transaction metadata
        public string BookMark { get; set; } //to be used to resume workflow with input parameters
    }

    public class WorkflowIndentifier2
    {
        public Guid WorkflowId { get; set; }
        public int CategoryId { get; set; }
        public string WEMWorkflowServiceUrl { get; set; }
        public int WorkflowVersion { get; set; }
        /// <summary>
        /// Optional list of parameters to be used in future, if any
        /// </summary>
        public List<Parameter> Parameters { get; set; }
        public Guid Identifier { get; set; }
        public ExecutionModeType ExecutionMode { get; set; }
        public ScheduledPatternType ScheduledPattern { get; set; }
        public DateTime ScheduleStartDateTime { get; set; }
        public DateTime ScheduleEndDateTime { get; set; }
        public ScheduleStopCriteriaType ScheduleStopCriteria { get; set; }
        public int ScheduleOccurences { get; set; }
        public int SchedulePriority { get; set; }
        public List<string> ScheduleOnClusters { get; set; }
        public IapNodeTransportType IapNodeTransport { get; set; }
        // Remote server names separated by comma
        public string RemoteServerNames { get; set; }
        public int IapNodeHttpPort { get; set; }
        public int IapNodeNetTcpPort { get; set; }
        public string Domain { get; set; }
        public bool UsesUIAutomation { get; set; }
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
        ScheduledOnIAPCluster = 4
    }

    [Serializable]
    public class Parameter
    {
        public string DataType { get; set; }
        public bool IsPaired { get; set; }
        public bool IsSecret { get; set; }
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public string allowedValues { get; set; }
    }

    public class ExecutionResult
    {
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public List<Parameter> Output { get; set; }
        public List<string> ScheduledRequestIds { get; set; }
        public string MachineName { get; set; }
    }

    public class TransactionMetadata
    {
        public string executionMode { get; set; }
        public List<Parameter> Parameters { get; set; }       
    }
}
