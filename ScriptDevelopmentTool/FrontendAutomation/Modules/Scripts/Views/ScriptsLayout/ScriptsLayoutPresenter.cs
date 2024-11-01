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
    public partial class ScriptsLayoutPresenter : Presenter<IScriptsLayout>
    {
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

        [EventSubscription(EventTopicNames.FormClosing, ThreadOption.UserInterface)]
        public void OnFormClosing(object sender, EventArgs eventArgs)
        {//TODO: Add your code here

        }

    }
}

