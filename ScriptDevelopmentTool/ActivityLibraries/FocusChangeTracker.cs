/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using Infosys.ATR.WinUIAutomationRuntimeWrapper.Core;
using System;
using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.AutomationActivity.Libraries
{
    public sealed class FocusChangeTracker : NativeActivity<Control>  
    {
        internal const string BookmarkName = "WaitingForEventToBeFired";
        private readonly Variable<NoPersistHandle> _noPersistHandle = new Variable<NoPersistHandle>();
        private BookmarkCallback _tabBookmarkCallback;

        public InArgument<FocusTracker> FocusObject { get; set; }  

        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        private AutomationFacade automationObject;
        private static FocusTracker focusTracker;  

        public BookmarkCallback TabBookmarkCallback 
        {
            get
            {
                return _tabBookmarkCallback ??
                       (_tabBookmarkCallback = new BookmarkCallback(OnTabCallback));
            }
        }

        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Tell the runtime that we need this extension
            metadata.RequireExtension(typeof(FocusChangeTrackerExtension));
            // Provide a Func<T> to create the extension if it does not already exist
            metadata.AddDefaultExtensionProvider(() => new FocusChangeTrackerExtension());
            metadata.AddArgument(new RuntimeArgument("FocusObject", typeof(FocusTracker), ArgumentDirection.In, true));
            metadata.AddArgument(new RuntimeArgument("AutomationObject", typeof(AutomationFacade), ArgumentDirection.In, true));
            metadata.AddArgument(new RuntimeArgument("Result", typeof(Control), ArgumentDirection.Out, false));
            metadata.AddImplementationVariable(_noPersistHandle);
        }

        protected override void Execute(NativeActivityContext context)
        {
            automationObject = context.GetValue(this.AutomationObject);
            focusTracker = context.GetValue(this.FocusObject);

            // Enter a no persist zone to pin this activity to memory since we are setting up a delegate to receive a callback
            var handle = _noPersistHandle.Get(context);
            handle.Enter(context);

            // Get (which may create) the extension
            var extension = context.GetExtension<FocusChangeTrackerExtension>();

            // Add the callback            
            extension.AddTabCallback(focusTracker);
            //System.Threading.Thread.Sleep(500);
            AutomationFacade.FocusHasChangedEventHandler focushandler = new AutomationFacade.FocusHasChangedEventHandler(target_FocusHasChanged);
            automationObject.SubscribeToFocusChangeEvent(focushandler);
       

            // Set a bookmark - the extension will resume when the Event is fired
            context.CreateBookmark(BookmarkName, TabBookmarkCallback);
        }

        private void target_FocusHasChanged(AutomationFacade.FocusHasChangedArgs e)
        {
            if (e != null)
            {
                focusTracker.FireEvent(e.Control);                
            }

            automationObject.DesubscribeToFocusChangeEvent(target_FocusHasChanged);            
        }

        internal void OnTabCallback(NativeActivityContext context, Bookmark bookmark, Object value) 
        {   
            // Store the result
            Result.Set(context, (Control)value);

            // Exit the no persist zone 
            var handle = _noPersistHandle.Get(context);            
            handle.Exit(context);
        }
    }

    internal class FocusChangeTrackerExtension : IWorkflowInstanceExtension
    {
        private bool _addedCallback;
        private WorkflowInstanceProxy _instance;

        #region IWorkflowInstanceExtension Members

        public IEnumerable<object> GetAdditionalExtensions()
        {
            return null;
        }

        public void SetInstance(WorkflowInstanceProxy instance)
        {
            _instance = instance;
        }

        #endregion

        internal void AddTabCallback(FocusTracker focus) 
        {
            //if (!_addedCallback)
            //{
                _addedCallback = true;
                focus.Fire += OnEventFired;
            //}
        }

        internal void OnEventFired(object sender, TrackerEventArgs args)
        {
            // Event was fired, resume the bookmark
            _instance.BeginResumeBookmark(
                new Bookmark(FocusChangeTracker.BookmarkName),
                args.TControl,
                (asr) => _instance.EndResumeBookmark(asr),
                null);
        }
    }

    public class TrackerEventArgs : EventArgs   
    {
        public Control TControl { get; set; }  
    }
    public class FocusTracker 
    {
        public event EventHandler<TrackerEventArgs> Fire;

        public void FireEvent(Control control)    
        {
            if (Fire != null)
            {
                Fire(this, new TrackerEventArgs { TControl = control });  
            }
        }
    }
}
