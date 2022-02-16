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

using Infosys.ATR.ModuleLoader.Constants;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;


namespace Infosys.ATR.ModuleLoader.Views
{
    public class ModuleSelectorPresenter : Presenter<IModuleSelector>
    {
        [EventPublication(EventTopicNames.TerminateApp,PublicationScope.Global)]
        public event EventHandler<EventArgs> TerminateApp;

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

        public void TerminateApp_Handler()
        {
            TerminateApp(this, new EventArgs());
        }
    }
}
