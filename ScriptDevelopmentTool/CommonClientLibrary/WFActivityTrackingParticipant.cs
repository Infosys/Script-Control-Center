/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.IAP.CommonClientLibrary
{
    public class WFActivityTrackingParticipant : System.Activities.Tracking.TrackingParticipant 
    {
        public event EventHandler<TrackingEventArgs> TrackingRecordReceived;
        public Dictionary<string, Activity> ActivityIdToWorkflowElementMap { get; set; }
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            OnTrackingRecordReceived(record, timeout);
        }

        //On Tracing Record Received call the TrackingRecordReceived with the record received information from the TrackingParticipant. 
        //We also do not worry about Expressions' tracking data
        protected void OnTrackingRecordReceived(TrackingRecord record, TimeSpan timeout)
        {
            if (TrackingRecordReceived != null)
            {
                WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
                if (workflowInstanceRecord != null)
                {
                    Console.WriteLine(String.Format(CultureInfo.InvariantCulture,
                        "\n{0} Workflow InstanceID: {1} \nWorkflow instance state: {2}",
                        DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.ffffff"), record.InstanceId, workflowInstanceRecord.State));
                }

                ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
                if ((activityStateRecord != null) && (!activityStateRecord.Activity.TypeName.Contains("System.Activities.Expressions")))
                {
                    if (ActivityIdToWorkflowElementMap != null)
                        if (!ActivityIdToWorkflowElementMap.ContainsKey(activityStateRecord.Activity.Id))
                            return;


                    TrackingRecordReceived(this, new TrackingEventArgs(
                                                         record,
                                                         timeout,
                                                         (ActivityIdToWorkflowElementMap != null) ? ActivityIdToWorkflowElementMap[activityStateRecord.Activity.Id] : null,
                                                         activityStateRecord
                                                         ));

                }
                else
                {
                    TrackingRecordReceived(this, new TrackingEventArgs(record, timeout, null));
                }
            }
        }
        
        //protected override void Track(TrackingRecord record, TimeSpan timeout)
        //{
        //    WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
        //    if (workflowInstanceRecord != null)
        //    {
        //        Console.WriteLine(String.Format(CultureInfo.InvariantCulture,
        //            "\n{0} Workflow InstanceID: {1} \nWorkflow instance state: {2}",
        //            DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.ffffff"),record.InstanceId, workflowInstanceRecord.State));
        //    }

        //    ActivityStateRecord activityStateRecord = record as ActivityStateRecord;
        //    if (activityStateRecord != null)
        //    {
        //        IDictionary<String, object> variables = activityStateRecord.Variables;
        //        StringBuilder vars = new StringBuilder();

        //        if (variables.Count > 0)
        //        {
        //            vars.AppendLine("\n\tVariables:");
        //            foreach (KeyValuePair<string, object> variable in variables)
        //            {
        //                vars.AppendLine(String.Format("\t\tName: {0} Value: {1}", variable.Key, variable.Value));
        //            }
        //        }

        //        Console.WriteLine(String.Format(CultureInfo.InvariantCulture, "\n{0} Activity {1} {2} {3}",
        //            DateTime.Now.ToString("dd MMM yyyy HH:mm:ss.ffffff"),activityStateRecord.Activity.Name, activityStateRecord.State,
        //            ((variables.Count > 0) ? vars.ToString() : String.Empty)));

        //        if(activityStateRecord.State.Equals("Closed",StringComparison.InvariantCultureIgnoreCase))
        //            Console.WriteLine("===============================================================");
        //    }

        //    CustomTrackingRecord customTrackingRecord = record as CustomTrackingRecord;

        //    if ((customTrackingRecord != null) && (customTrackingRecord.Data.Count > 0))
        //    {
        //        ///to set up the custom tracking logic here 
        //    }
        //    Console.WriteLine();
        //}
    }


    //Custom Tracking EventArgs
    public class TrackingEventArgs : EventArgs
    {
        public TrackingRecord Record { get; set; }
        public TimeSpan Timeout { get; set; }
        public Activity Activity { get; set; }
        public ActivityStateRecord ActivityState { get; set; }

        public TrackingEventArgs(TrackingRecord trackingRecord, TimeSpan timeout, Activity activity)
        {
            this.Record = trackingRecord;
            this.Timeout = timeout;
            this.Activity = activity;
        }

        public TrackingEventArgs(TrackingRecord trackingRecord, TimeSpan timeout, Activity activity, ActivityStateRecord activityState)
        {
            this.Record = trackingRecord;
            this.Timeout = timeout;
            this.Activity = activity;
            this.ActivityState = activityState;
        }
    }
}
