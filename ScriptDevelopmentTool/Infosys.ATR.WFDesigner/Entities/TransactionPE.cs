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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Infosys.ATR.WFDesigner.Entities
{
    public class TransactionPE
    {
        [ReadOnly(true)]
        public DateTime InitiatedOn { get; set; }
        [ReadOnly(true)]
        //name of script/WF
        public string Name { get; set; }
        [ReadOnly(true)]
        //script/wf part of what category
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }
        [ReadOnly(true)]
        //type - Script/WF
        public string Module { get; set; }
        [ReadOnly(true)]
        public string Type { get; set; }
        [ReadOnly(true)]
        public string User { get; set; }
        [ReadOnly(true)]
        public string State { get; set; }
        [ReadOnly(true)]
        //machine where it is running
        public string Node { get; set; }
        [ReadOnly(true)]
         [DisplayName("Running Days")]
        public TimeSpan RunningDays { get; set; }
        [ReadOnly(true)]
        [Browsable(true)] //this ensures property is not seen when bound to a control
        public string TransactionId { get; set; }
        [ReadOnly(true)]
        [DisplayName("Transaction Metadata")]
        public string Metadata { get; set; }
        [ReadOnly(true)]
        public string Description { get; set; }
        [ReadOnly(true)]
        [DisplayName("Reference Key")]
        public string ReferenceKey { get; set; }

         [ReadOnly(true)]
        [DisplayName("Activity Bookmark")]
        public string WorkflowActivityBookmark { get; set; } 
    }
    public class User
    {
        public bool Check { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
    public class Artifact
    {
        public bool Check { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    public class Node
    {
        public bool Check { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
    public enum StateType
    {

        InProgress = 1, //shud be alsways the first one so that it is by default when a transaction instance is logged.

        Failed = 2,

        Paused = 3,

        Aborted = 4,

        Completed = 5
    }
    public enum Period
    {
       
        ALL = 1,
        
        Today = 2,
       
        Last_5_Days = 3,

        Last_30_Days = 4,

        Last_6_Month = 5,

        Last_1_Year = 6
    }
    public class TransactionFilter
    {

        public int CategoryId { get; set; }

        public int CompanyId { get; set; }

        //public string Requestor { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
    public enum ModuleType
    {
        Workflow = 1,

        Script = 2
    }
    public static class ExtensionMethods
    {
        public static string EnumValue(this Period e)
        {
            switch (e)
            {
                case Period.ALL:
                    return "ALL";
                case Period.Today:
                    return "Today";
                case Period.Last_1_Year:
                    return "Last 1 Year";
                case Period.Last_30_Days:
                    return "Last 30 Days";
                case Period.Last_5_Days:
                    return "Last 5 Days";
                case Period.Last_6_Month:
                    return "Last 6 Month";
            }
            return "";
        }
    }
       
}
