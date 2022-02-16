/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

using Infosys.ATR.WFDesigner.Constants;
using Infosys.ATR.WFDesigner.Services;
using Infosys.WEM.Service.Contracts;
using Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.Service.Contracts.Message;
using Infosys.ATR.WFDesigner.Entities;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Infosys.ATR.WFDesigner.Views
{
    public class SettingsPresenter : Presenter<ISettings>
    {
        

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
       

        //internal void GetCategories()
        //{
        //    int companyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);
        //    var response = WFService.GetWFCategories(companyId);
        //}

        [EventSubscription(Constants.EventTopicNames.ShowCatDetails,ThreadOption.UserInterface)]
        public void ShowCatDetails(object sender,EventArgs<Tuple<TreeNode,Category>> e)
        {
            this.View.Catdetails = e.Data;
            this.View.ShowCatDetails();
        }


        internal void ShowSuccessMessage(string s)
        {
            MessageBox.Show(String.Format("{0} sucessfully added", s), "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
