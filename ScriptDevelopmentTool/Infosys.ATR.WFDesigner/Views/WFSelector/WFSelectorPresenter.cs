/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.WFDesigner.Services;
using Infosys.ATR.WFDesigner.Entities;
using Infosys.ATR.WFDesigner.Constants;
using Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.Service.Contracts.Message;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Library.Services;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Windows.Forms;
using Infosys.WEM.Client;

namespace Infosys.ATR.WFDesigner.Views
{
    public class WFSelectorPresenter : Presenter<IWFSelector>
    {

        [EventPublication(EventTopicNames.OpenWFFromRepository, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<WorkflowPE>> OpenWFFromRepository;

        [EventPublication(EventTopicNames.ShowWFDetails, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<WorkflowPE>> ShowWFDetailsEvent;

        [EventPublication(EventTopicNames.ShowCatDetails, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<Tuple<TreeNode, Category>>> ShowCatDetailsEvent;

        [EventPublication(EventTopicNames.ActivateMenu, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<bool>> ActivateMenu;

        [EventPublication(EventTopicNames.EnableDeleteMenu, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<bool>> EnableDeleteMenu;
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

        internal void GetCategories(int companyId, bool refresh)
        {
            List<Entities.Category> categories = null;
            if (refresh || WFCache.CategoryDetails == null)
            {
               var response = WFService.GetAllCategoriesByCompany(companyId);
                categories = Translators.CategoryPE_SE.CategoryListSEtoPE(response.Categories.ToList());
            }
            else
            {
                categories = WFCache.CategoryDetails;
            }
            this.View.Categories = categories;
        }

        internal void GetCategoriesWithData(int companyId, bool refresh)
        {
            List<Entities.Category> categories = null;
            if (refresh || WFCache.CategoryDetailsWithData == null)
            {
                var response = WFService.GetAllCategoriesWithData(companyId);
                categories = Translators.CategoryPE_SE.CategoryListSEtoPE(response.Categories.ToList());
            }
            else
            {
                categories = WFCache.CategoryDetails;
            }
            this.View.Categories = categories;
        }

        internal void ShowCatDetails_Handler(Tuple<TreeNode, Category> catDetails)
        {
            ShowCatDetailsEvent(this, new EventArgs<Tuple<TreeNode, Category>>(catDetails));
        }

        //internal string GetCompany()
        //{
        //    int companyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);
        //    return WFService.GetCompanyDetails(companyId).Company.StorageBaseUrl;

        //}
        internal void GetWorkflowByCategory(int catId)//, int subCatId)
        {
            List<WorkflowPE> workflows = null;
            //var result = WFService.GetWorkflowByCategory(catId, subCatId);
            var result = WFService.GetWorkflowByCategory(catId);
            if (result != null)
            {
                workflows = new List<WorkflowPE>();
                // string storageBaseUrl = GetCompany();
                string storageBaseUrl = CommonServices.Instance.StorageBaseURL;
                result.CategoryWorkflowMapping.ToList().ForEach(wf =>
                {
                    WorkflowPE wfPE = new WorkflowPE();
                    wfPE.CategoryID = wf.CategoryID;
                    //wfPE.SubCategoryID = wf.SubCategoryID;
                    wfPE.Description = wf.Description;
                    wfPE.Name = wf.Name;
                    wfPE.WorkflowID = wf.WorkflowID;
                    wfPE.WorkflowURI = storageBaseUrl + wf.WorkflowURI;
                    wfPE.WorkflowVersion = wf.WorkflowVersion;
                    wfPE.CreatedBy = wf.CreatedBy;
                    wfPE.PublishedOn = Convert.ToDateTime(wf.PublishedOn);
                    wfPE.LastModifiedBy = wf.LastModifiedBy;
                    wfPE.LastModifiedOn = Convert.ToString(wf.LastModifiedOn);
                    wfPE.UsesUIAutomation = wf.UsesUIAutomation;
                    wfPE.IslongRunningWorkflow = wf.IslongRunningWorkflow;
                    wfPE.IdleStateTimeout = wf.IdleStateTimeout;
                    wfPE.Tags = wf.Tags;
                    wfPE.LicenseType = wf.LicenseType;
                    wfPE.SourceUrl = wf.SourceUrl;
                    wfPE.Parameters = Translators.WorkflowParameterPE_SE.WorkflowParameterListSEtoPE(wf.Parameters);
                    workflows.Add(wfPE);
                });
            }
            this.View.Workflows = workflows;
        }

        internal void OpenWFFromRepository_handler(WorkflowPE data)
        {
            try
            {
                OpenWFFromRepository(this, new EventArgs<WorkflowPE>(data));
            }
            catch (Exception ex)
            {
                throw new GenericException("Unable to open the selected Workflow from repository", true, false);
            }
        }

        internal void ShowWFDetails(WorkflowPE pe)
        {
            ShowWFDetailsEvent(this, new EventArgs<WorkflowPE>(pe));
        }
        internal void Delete(WorkflowPE pe)
        {
            WFService.Delete(pe);
        }

        internal void ActivateMenuHandler()
        {
            ActivateMenu(this, new EventArgs<bool>(true));
        }

        internal void DeActivateMenuHandler()
        {
            ActivateMenu(this, new EventArgs<bool>(false));
        }

        internal void EnableDeleteMenu_Handler(bool enable)
        {

            ((ToolStripMenuItem)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainMenu].ToList()[0]).DropDownItems[2].Enabled =

            ((ToolStripButton)this.WorkItem.RootWorkItem.UIExtensionSites[Constants.UIExtensionSiteNames.MainToolbar].ToList()[7]).Enabled =
            enable;

            //EnableDeleteMenu(this, new EventArgs<bool>(enable));
        }

        [EventSubscription(EventTopicNames.DeleteWF, ThreadOption.UserInterface)]
        public void DeleteWF(object sender, EventArgs e)
        {
            this.View.DeleteWF();
        }

        [EventSubscription(EventTopicNames.RunWFSelector, ThreadOption.UserInterface)]
        public void RunWFSelector(object sender, EventArgs e)  
        { 
            this.View.RunWFSelector();
        }
    }
}
