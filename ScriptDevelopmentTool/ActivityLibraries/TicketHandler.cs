/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Infrastructure.Common;
using InfrastructureClientLibrary;
using System;
using System.Activities;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.ComponentModel;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using IAP.Infrastructure.Services.Contracts;

namespace Infosys.WEM.AutomationActivity.Libraries.TicketHandler
{
    public class ReadTickets : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Path of the csv file containing Ticket dump")]
        public InArgument<string> FilePath { get; set; }

        [RequiredArgument]
        [Description("Column index of Ticket Number in csv")]
        public InArgument<int> TicketNumberColumnIndex { get; set; }

        [RequiredArgument]
        [Description("Column index of Ticket reason in csv")]
        public InArgument<int> TicketReasonColumnIndex { get; set; }

        [RequiredArgument]
        [Description("Column index of Ticket state in csv")]
        public InArgument<int> TicketStateIdColumnIndex { get; set; }

        [RequiredArgument]
        [Description("Column index of Ticket description in csv")]
        public InArgument<int> TicketDescriptionColumnIndex { get; set; }

        [Description("Column index of Ticket Last action in csv")]
        public InArgument<int> TicketLastActionColumnIndex { get; set; }

        [RequiredArgument]
        [Description("Column index of Ticket state updated date in csv")]
        public InArgument<int> TicketStateUpdatedDateColumnIndex { get; set; }

        [RequiredArgument]
        [Description("Column index of Ticket priority in csv")]
        public InArgument<int> TicketPriorityColumnIndex { get; set; }

        [Description("String representing low priority")]
        public InArgument<string> LowPriority { get; set; }

        [Description("String representing medium priority")]
        public InArgument<string> MediumPriority { get; set; }

        [Description("String representing high priority")]
        public InArgument<string> HighPriority { get; set; }

        [RequiredArgument]
        [Description("Tickets read from the csv file")]
        public OutArgument<List<Ticket>> Tickets { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                   context.ActivityInstanceId, ActivityEvents.READTICKETS, ActivityControls.TICKETHANDLER))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId,
                    ActivityEvents.READTICKETS, ActivityControls.TICKETHANDLER);

                //in param 
                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, "AddTicket", Designer.ApplicationConstants.PARAMDIRECTION_IN, FilePath);

                Dictionary<string, int> mapperColumns = new Dictionary<string, int>();
                mapperColumns.Add("TicketNumber", context.GetValue(TicketNumberColumnIndex));
                mapperColumns.Add("Reason", context.GetValue(TicketReasonColumnIndex));
                mapperColumns.Add("StateId", context.GetValue(TicketStateIdColumnIndex));
                mapperColumns.Add("Description", context.GetValue(TicketDescriptionColumnIndex));
                mapperColumns.Add("LastAction", context.GetValue(TicketLastActionColumnIndex));
                mapperColumns.Add("StatusUpdatedDate", context.GetValue(TicketStateUpdatedDateColumnIndex));

                mapperColumns.Add("Priority", context.GetValue(TicketPriorityColumnIndex));

                Dictionary<int, string> mapperPriority = new Dictionary<int, string>();
                if (!string.IsNullOrEmpty(context.GetValue(LowPriority)))
                    mapperPriority.Add(3, context.GetValue(LowPriority));

                if (!string.IsNullOrEmpty(context.GetValue(MediumPriority)))
                    mapperPriority.Add(2, context.GetValue(MediumPriority));

                if (!string.IsNullOrEmpty(context.GetValue(HighPriority)))
                    mapperPriority.Add(1, context.GetValue(HighPriority));

                List<Ticket> results = CsvHelper.ReadCsv(context.GetValue(FilePath), mapperColumns, mapperPriority);
                Tickets.Set(context, results);
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new ReadTickets
            {
                DisplayName = "Read Tickets",
            };
        }
    }

    public class AddTicket : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Services URL")]
        public InArgument<string> ServicesURL { get; set; }

        [RequiredArgument]
        [Description("Ticket object to be inserted")]
        public InArgument<Ticket> Ticket { get; set; }

        [Description("Result, True is ticket is added successfully, false otherwise")]
        public OutArgument<bool> Status { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                   context.ActivityInstanceId, ActivityEvents.ADDTICKET, ActivityControls.TICKETHANDLER))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId,
                    ActivityEvents.ADDTICKET, ActivityControls.TICKETHANDLER);

                //in param 
                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, "AddTicket", Designer.ApplicationConstants.PARAMDIRECTION_IN, Ticket);

                string ServiceBaseUrl = context.GetValue(ServicesURL);
                string ServiceURL = "/iapinfrastructureservices/TicketHandler.svc";

                Response<bool> results = TicketHandlerClient.AddTicket(ServiceBaseUrl + ServiceURL, context.GetValue(Ticket));
                Status.Set(context, results.Results);
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new AddTicket
            {
                DisplayName = "Add Ticket",
            };
        }
    }

    public class UpdateTicket : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Services URL")]
        public InArgument<string> ServicesURL { get; set; }

        [RequiredArgument]
        [Description("Ticket to be updated")]
        public InArgument<Ticket> Ticket { get; set; }

        [Description("Result, True is ticket is updated successfully, false otherwise")]
        public OutArgument<bool> Status { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                   context.ActivityInstanceId, ActivityEvents.PAUSE_TICKET_PROCESSING, ActivityControls.TICKETHANDLER))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId,
                    ActivityEvents.PAUSE_TICKET_PROCESSING, ActivityControls.TICKETHANDLER);


                string ServiceBaseUrl = context.GetValue(ServicesURL);
                string ServiceURL = "/iapinfrastructureservices/TicketHandler.svc";

                Ticket tc = context.GetValue(Ticket);

                Response<bool> results = TicketHandlerClient.UpdateTicket(ServiceBaseUrl + ServiceURL, tc);
                Status.Set(context, results.Results);

                if (results.Results)
                {
                    LocalDataStoreSlot localData = System.Threading.Thread.GetNamedDataSlot("iapworkflowstate");
                    if (tc.StateId == (int)TicketState.ClientPending_RequestInformation || tc.StateId == (int)TicketState.ClientPending_WaitingForApproval || tc.StateId == (int)TicketState.ThirdPartyPending)
                    {
                        //Set the workflow state to the TLS store
                        System.Threading.Thread.FreeNamedDataSlot("iapworkflowstate");
                        localData = System.Threading.Thread.AllocateNamedDataSlot("iapworkflowstate");
                    }
                    System.Threading.Thread.SetData(localData, tc.StateId);
                }
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new UpdateTicket
            {
                DisplayName = "Update Ticket",
            };
        }
    }

    public class FetchNextTicket : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Services URL")]
        public InArgument<string> ServicesURL { get; set; }

        [RequiredArgument]
        [Description("Ticket object if next available ticket exists in Database, null otherwise")]
        public OutArgument<Ticket> Ticket { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                   context.ActivityInstanceId, ActivityEvents.PAUSE_TICKET_PROCESSING, ActivityControls.TICKETHANDLER))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId,
                    ActivityEvents.PAUSE_TICKET_PROCESSING, ActivityControls.TICKETHANDLER);

                //in param 
                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, "PauseTicketProcessing", Designer.ApplicationConstants.PARAMDIRECTION_IN, Ticket);

                string ServiceBaseUrl = context.GetValue(ServicesURL);
                string ServiceURL = "/iapinfrastructureservices/TicketHandler.svc";

                Response<Ticket> results = TicketHandlerClient.GetNextUnAssignedTicketByStates(ServiceBaseUrl + ServiceURL, new int[] { 1, 6 });
                Ticket.Set(context, results.Results);
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new FetchNextTicket
            {
                DisplayName = "Fetch next Ticket",
            };
        }
    }

    public class FetchTicketById : NativeActivity, IActivityTemplateFactory
    {
        [RequiredArgument]
        [Description("Services URL")]
        public InArgument<string> ServicesURL { get; set; }

        [RequiredArgument]
        [Description("Unique Ticket Id")]
        public InArgument<string> TicketId { get; set; }

        [RequiredArgument]
        [Description("Ticket object if next available ticket exists in Database, null otherwise")]
        public OutArgument<Ticket> Ticket { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            //check if stop requested, if show then throw exception
            if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

            using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                   context.ActivityInstanceId, ActivityEvents.PAUSE_TICKET_PROCESSING, ActivityControls.TICKETHANDLER))
            {
                LogHandler.LogInfo(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.ActivityInstanceId,
                    ActivityEvents.PAUSE_TICKET_PROCESSING, ActivityControls.TICKETHANDLER);

                //in param 
                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                            context.ActivityInstanceId, "GetTicketById", Designer.ApplicationConstants.PARAMDIRECTION_IN, Ticket);

                string ServiceBaseUrl = context.GetValue(ServicesURL);
                string ServiceURL = "/iapinfrastructureservices/TicketHandler.svc";

                Response<Ticket> results = TicketHandlerClient.GetTicketById(ServiceBaseUrl + ServiceURL, context.GetValue(TicketId));
                Ticket.Set(context, results.Results);
            }
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new FetchTicketById
            {
                DisplayName = "Fetch Ticket by Id",
            };
        }
    }
}
