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

using System.Activities;
using System.Runtime.DurableInstancing;
using System.Activities.DurableInstancing;
using System.Configuration;
using System.Threading;
using Infosys.WEM.AutomationActivity.Designers;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.WorkflowStateManagement
{
    public class Operations
    {
        private Entity.Result _outputs;

        public Entity.Result Invoke(Activity workflow, IDictionary<string, object> wfInParams, bool unloadFromMemory = false, WFActivityTrackingParticipant wfTrackingParticipant = null)
        {
            _outputs = new Entity.Result();
            AutoResetEvent synchingEvent = new AutoResetEvent(false);

            WorkflowApplication wfApp = null;
            if (wfInParams != null && wfInParams.Count > 0)
                wfApp = new WorkflowApplication(workflow, wfInParams);
            else
                wfApp = new WorkflowApplication(workflow);
            wfApp.Completed = (eventArg) =>
            {
                if (eventArg.CompletionState == ActivityInstanceState.Closed)
                {
                    _outputs.Output = eventArg.Outputs as Dictionary<string, object>;
                }
                _currentState = State.Completed;
                synchingEvent.Set();
            };

            wfApp.PersistableIdle = (eventArgs) =>
            {
                _currentState = State.Paused; //paused because it is persistable                
                _outputs.PersistedWorkflowInstanceId = wfApp.Id;
                //check if the pausing acivity's bookmark is available in the TLS
                try
                {
                    LocalDataStoreSlot tlsdata = System.Threading.Thread.GetNamedDataSlot("iapPausingAcivityBookMark");
                    if (tlsdata != null)
                    {
                        _outputs.BookMarkOfPausingActivity = System.Threading.Thread.GetData(tlsdata).ToString();
                    }

                }
                catch
                {
                    //i.e. there isnt any TLS data slot by that name
                }

                synchingEvent.Set();
                if (unloadFromMemory)
                    return PersistableIdleAction.Unload;
                else
                    return PersistableIdleAction.Persist;
            };

            wfApp.Unloaded = (eventArgs) =>
            {
                synchingEvent.Set();
            };

            //wfApp.Persisted = (eventArgs) =>
            //{
            //    synchingEvent.Set();
            //};


            wfApp.InstanceStore = GetInstanceStore();

            if (wfTrackingParticipant != null)
                wfApp.Extensions.Add(wfTrackingParticipant);

            wfApp.Run();

            // waiting for Completed to arrive and signal that
            // the workflow is complete.
            synchingEvent.WaitOne();

            return _outputs;
        }

        public Entity.Result Resume(Guid persistedWorkflowInstanceId, Activity workflow, IDictionary<string, object> wfInParams, PeristenceType persistenceType, string bookMark = "", WFActivityTrackingParticipant wfTrackingParticipant = null)// bool unloadFromMemory = false, bool rePersist = true)
        {
            _outputs = new Entity.Result();
            AutoResetEvent synchingEvent = new AutoResetEvent(false);

            WorkflowApplication wfApp = null;
            //if (wfInParams != null && wfInParams.Count > 0)
            //    //wfApp = new WorkflowApplication(workflow, wfInParams); //while resuming, input parameters are not allowed by the .net workflow foundation
            //    wfApp = new WorkflowApplication(workflow, wfInParams);
            //else
            wfApp = new WorkflowApplication(workflow);
            if (wfTrackingParticipant != null)
                wfApp.Extensions.Add(wfTrackingParticipant);

            wfApp.Completed = (eventArg) =>
            {
                if (eventArg.CompletionState == ActivityInstanceState.Closed)
                {
                    _outputs.Output = eventArg.Outputs as Dictionary<string, object>;
                }
                _currentState = State.Completed;
                synchingEvent.Set();
            };

            wfApp.PersistableIdle = (eventArgs) =>
            {
                _outputs.PersistedWorkflowInstanceId = wfApp.Id;
                _currentState = State.Paused;
                synchingEvent.Set();
                switch (persistenceType)
                {
                    case PeristenceType.PersistAndUnload:
                        return PersistableIdleAction.Unload;
                    case PeristenceType.Persist:
                        return PersistableIdleAction.Persist;
                    default:
                        return PersistableIdleAction.None;

                }
                //if (unloadFromMemory)
                //    return PersistableIdleAction.Unload;
                //else if (rePersist)
                //    return PersistableIdleAction.Persist;
                //else
                //    return PersistableIdleAction.None;
            };

            wfApp.Unloaded = (eventArgs) =>
            {
                synchingEvent.Set();
            };

            wfApp.InstanceStore = GetInstanceStore();
            wfApp.Load(persistedWorkflowInstanceId);
            if ((wfInParams == null || wfInParams.Count == 0) && string.IsNullOrEmpty(bookMark))
                wfApp.Run();
            else
            {
                //resume using book mark
                if (string.IsNullOrEmpty(bookMark))
                    throw new Exception("Book Mark is mandatory for resuming persisted workflow with input parameters.");
                else
                {
                    //map the input
                    List<WorkflowParam> inputArguments = null;
                    if (wfInParams != null && wfInParams.Count > 0)
                    {
                        inputArguments = new List<WorkflowParam>();
                        foreach (var input in wfInParams.Keys)
                        {
                            inputArguments.Add(new WorkflowParam() { ParameterName = input, ParameterValue = wfInParams[input] as string });
                        }
                    }
                    wfApp.ResumeBookmark(bookMark, inputArguments);
                }
            }

            // waiting for Completed to arrive and signal that
            // the workflow is complete.
            synchingEvent.WaitOne();

            //_outputs.PersistedWorkflowInstanceId = wfApp.Id;
            return _outputs;
        }

        private InstanceStore GetInstanceStore()
        {
            InstanceStore store = null;
            string connectionString = ConfigurationManager.AppSettings["WorkflowStateStore"];
            if (!string.IsNullOrEmpty(connectionString))
            {
                store = new SqlWorkflowInstanceStore(connectionString);
            }
            else
                throw new Exception("IAP Workflow state persistence store is not configured");
            return store;
        }

        private State _currentState;

        public State CurrentState
        {
            get { return _currentState; }
        }
        //public State CurrentState { get; set; }
    }

    public enum PeristenceType
    {
        None,
        PersistAndUnload,
        Persist
    }

    public enum State
    {
        Completed,
        Paused
    }
}
