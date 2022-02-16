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

namespace Infosys.ATR.Editor.Views.Create
{
    public partial class FileCreatePresenter : Presenter<IFileCreate> 
    {
        [EventPublication(EventTopicNames.BaseDir, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<string[]>> BaseDir;

        [EventPublication(Constants.EventTopicNames.ToggleDeckPanel, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ToggleDeckPanel;

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

        internal void Update(string[] param)
        {
            if (BaseDir != null)
            {
                BaseDir(this,new EventArgs<string[]>(param));
            }
        }

        internal void ToggleDeckPanel_handler()
        {
            ToggleDeckPanel(this, new EventArgs<bool>(true));
        }
    }
}
