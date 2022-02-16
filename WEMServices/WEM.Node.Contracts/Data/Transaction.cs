/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Node.Service.Contracts.Data
{
    [DataContract]
    public class Transaction
    {
        [DataMember(IsRequired = true)]
        public string ModuleId { get; set; }
        /// <summary>
        /// This is mandatory while adding the entries but optional while fetching
        /// </summary>
        [DataMember]
        public int CategoryId { get; set; } //in case this is not provided then details for all the category to which the requestor has access are returned. 
        /// <summary>
        /// Not needed during adding entries. To be used while fetching the transaction details as these are to be grouped in the client side and displayed under the category by name.
        /// </summary>
        [DataMember]
        public string CategoryName { get; set; } // needed because the same is needed in the client to group the transactions
        [DataMember]
        public string InstanceId { get; set; }
        [DataMember]
        public StateType CurrentState { get; set; }
        [DataMember]
        public string Executor { get; set; } //for iap node = <machine name>, for script utility= SC_<machine name>, for workflow = UI_<machine name and for workbench = WB_<machine name>
        [DataMember]
        public string MachineName { get; set; }
        //these below values are determined from http context
        //[DataMember]
        //public string InitiatedBy { get; set; } //the alias of the user who initially started the transaction. this is provided for the first time and it remains unchanged
        //[DataMember]
        //public string TriggeredBy { get; set; } //the alias of the user who resumed the paused/long running transaction
        [DataMember]
        public string CreatedBy { get; set; } //the identity under which the concerned initiating executor is running. this is same as the initiated by
        //[DataMember]
        //public string LastModifiedBy { get; set; } //the identify under which the concerned executor is running. this keeps updated as and when the state is changed
        [DataMember]
        public string IPAddress { get; set; } // the ip address of the executor
        [DataMember]
        public string OSDetails { get; set; } // the OS details of the machine running the executor. it also has the information whether OS is 64 or 32 bit
        [DataMember]
        public string ReferenceKey { get; set; } //the optional information to link the transaction with the business use case e.g. ticket id, etc.
        [DataMember]
        public string ModuleVersion { get; set; } // the version of the workflow/script to be executed or resumed
        [DataMember]
        public DateTime CreatedOn { get; set; } //the date of creation of the transaction
        [DataMember]
        public string FileType { get; set; }//xaml, bat, ps1, etc
        [DataMember]
        public ModuleType Module { get; set; }
        [DataMember(IsRequired = true)]
        public int CompanyId { get; set; }
        [DataMember]
        public string WorkflowPersistedStateId { get; set; }//for workflow, if state persisted then the id of the state persisted
        [DataMember]
        public string WorkflowActivityBookmark { get; set; }//for workflow, if state persisted then the bookmark of the activity whihc paused the workflow
        [DataMember]
        public string ModuleName { get; set; } // the name of workflow/ script 
        [DataMember]
        public string TransactionMetadata { get; set; }
        [DataMember]
        public string Description { get; set; }
    }

    [DataContract]
    public enum StateType
    {
        [EnumMember]
        InProgress = 1, //shud be alsways the first one so that it is by default when a transaction instance is logged.
        [EnumMember]
        Failed = 2,
        [EnumMember]
        Paused = 3,
        [EnumMember]
        Aborted = 4,
        [EnumMember]
        Resumed = 5,
        [EnumMember]
        Completed = 6
    }

    [DataContract]
    public class TransactionFilter
    {
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember(IsRequired = true)]
        public int CompanyId { get; set; }
        //requestor details to be fetched from http context
        //[DataMember]
        //public string Requestor { get; set; }
        [DataMember]
        public DateTime? StartDate { get; set; }
        [DataMember]
        public DateTime? EndDate { get; set; }
    }

    [DataContract]
    public enum ModuleType
    {
        [EnumMember]
        Workflow = 1,
        [EnumMember]
        Script = 2
    }
}
