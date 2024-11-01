//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance Package
//
// This file was generated by the "Add View" recipe.
//
// A presenter calls methods of a view to update the information that the view displays. 
// The view exposes its methods through an interface definition, and the presenter contains
// a reference to the view interface. This allows you to test the presenter with different 
// implementations of a view (for example, a mock view).
//
// 
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using System;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI.EventBroker;
using IMSWorkBench.Scripts.Constants;

namespace IMSWorkBench.Scripts
{
    public partial class PlaybackPresenter : Presenter<IPlayback>
    {
        [EventPublication(EventTopicNames.Script, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs> Script;
    
        [EventPublication(EventTopicNames.Run, PublicationScope.Global)]
        public event EventHandler<EventArgs> Run;

        [EventPublication(EventTopicNames.Edit, PublicationScope.Global)]
        public event EventHandler<EditEventArgs> Edit;

        [EventPublication(EventTopicNames.Execute, PublicationScope.Global)]
        public event EventHandler<EditEventArgs> Execute;

        [EventPublication(EventTopicNames.Open, PublicationScope.Global)]
        public event EventHandler<EventArgs> Open;
        /// <summary>
        /// This method is a placeholder that will be called by the view when it has been loaded.
        /// </summary>
        public override void OnViewReady()
        {
            base.OnViewReady();
        }
        
        /// <summary>
        /// Close the view
        /// </summary>
        public override void OnCloseView()
        {
            base.CloseView();
        }

        internal virtual void OnRun(EventArgs e)
        {
            if (Run != null)
            {
                Run(this, e);
            }            
        }


        internal void OnEdit(EditEventArgs e)
        {
            if (Edit != null)
            {
                Edit(this, e);
            }
        }

        internal virtual void OnScript(EditEventArgs e)
        {
            if (Script != null)
            {
                Script(this, e);
            }
        }

        internal virtual void OnExecute(EditEventArgs e)
        {
            if (Execute != null)
            {
                Execute(this, e);
            }
        }

        internal virtual void OnOpen(EventArgs e)
        {
            if (Open != null)
            {
                Open(this, e);
            }
        }
    }
}

