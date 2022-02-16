/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.Editor.Constants;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Infosys.ATR.Editor.Views
{
    public class SettingsPresenter : Presenter<ISettings>
    {

        [EventPublication(EventTopicNames.SetIFEAPath, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<string>> SetIFEAPath;

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

        internal void Set(string p)
        {
            if (SetIFEAPath != null)
                SetIFEAPath(this, new EventArgs<string>(p));
        }
    }
}
