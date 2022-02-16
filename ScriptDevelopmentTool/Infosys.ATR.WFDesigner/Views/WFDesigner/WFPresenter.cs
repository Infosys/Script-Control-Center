/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;
using Infosys.ATR.WFDesigner.Constants;
using Infosys.ATR.WFDesigner.Services;
using Infosys.WEM.Service.Common.Contracts.Message;

namespace Infosys.ATR.WFDesigner.Views
{
    public class WFPresenter :Presenter<IWFDesigner>
    {
        [EventPublication(EventTopicNames.ActivateMenu, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ActivateMenu;

        [EventPublication(EventTopicNames.WFSave, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> WFSaved;

        [EventPublication(EventTopicNames.DisablePublish, PublicationScope.Global)]
        public event EventHandler disablePublish;

        //[EventPublication(EventTopicNames.DeActivateMenu, PublicationScope.Global)]
        //public event EventHandler DeActivateMenu;
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

        internal void ActivateMenuHandler()
        {
            ActivateMenu(this, new EventArgs<bool>(true));
        }

        internal void DeActivateMenuHandler()
        {
            ActivateMenu(this, new EventArgs<bool>(false));
        }

        internal void WFSaveHandler(string wfname)
        {
            WFSaved(this, new EventArgs<string>(wfname));
        }

        internal void DisbalePublish_Handler()
        {
            disablePublish(this, new EventArgs());
        }

        internal GetCompanyResMsg GetCompany(int companyId)
        {
            return WFService.GetCompanyDetails(companyId);

        }        
    }
}
