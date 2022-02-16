/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Infosys.ATR.ScriptRepository.Constants;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Infosys.ATR.ScriptRepository.Views
{
    public class ScriptDesignerPresenter : Presenter<IScriptDesigner>
    {
        [EventPublication(EventTopicNames.DeActivatePublish, PublicationScope.Global)]
        public event EventHandler DeActivatePublish;

        [EventPublication(EventTopicNames.ActivatePublish, PublicationScope.Global)]
        public event EventHandler ActivatePublish;

        [EventPublication(Constants.EventTopicNames.ShowOutputView, PublicationScope.Global)]
        public event EventHandler<EventArgs<List<CommonViews.ExecutionResultView>>> ShowOutputView;
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

        //[EventSubscription(EventTopicNames.CurrentTab, ThreadOption.UserInterface)]
        public void DisablePublish(object sender, EventArgs<string> e)
        {
            if (this.View.OpMode == Mode.Edit)
                DeActivatePublish(this, new EventArgs());
            else if (this.View.OpMode == Mode.New)
                ActivatePublish(this, new EventArgs());
        }

        internal void DisablePublish()
        {
            DeActivatePublish(this, new EventArgs());
        }



        internal void ShowOutputView_Handler(List<CommonViews.ExecutionResultView> e)
        {
            ShowOutputView(this, new EventArgs<List<CommonViews.ExecutionResultView>>(e));
        }
    }
}
