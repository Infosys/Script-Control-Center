using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Infosys.ATR.WFDesigner.Services;
using Infosys.ATR.WFDesigner.Entities;
using Infosys.ATR.WFDesigner.Constants;
using Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.Service.Contracts.Message;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;


namespace Infosys.ATR.WFDesigner.Views
{
    public class PublishPresenter : Presenter<IPublish>
    {
        [EventPublication(EventTopicNames.OpenWFFromRepository, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<WorkflowPE>> OpenWFFromRepository;



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
            var sp = this.WorkItem.SmartParts.Get<Publish>("publish");
            this.WorkItem.SmartParts.Remove(sp);
        }

        internal void GetCategories()
        {
            int companyId = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Company"]);
            var response = WFService.GetAllCategoriesByCompany(companyId);
            List<Entities.Category> categories = null;
            categories = Translators.CategoryPE_SE.CategoryListSEtoPE(response.Categories.ToList());
            this.View.Categories = categories;
        }

        //internal GetCompanyResMsg GetCompany(int companyId)
        //{
        //   return  WFService.GetCompany(companyId);

        //}

        internal WorkflowPE Update(WorkflowPE request)
        {
            PublishReqMsg _request = new PublishReqMsg
            {
                CategoryID = request.CategoryID,
               // SubCategoryID=request.SubCategoryID,
              //  CreatedBy = request.CreatedBy,
                Description = request.Description,
                Name = request.Name,
                WorkflowID = request.WorkflowID,
                WorkflowURI = request.WorkflowURI,
                WorkflowVer =request.WorkflowVersion,                
                IncrementVersion = request.IncrementVersion
            };
            return WFService.Update(_request);
        }

        internal WorkflowPE Publish(WorkflowPE request)
        {
            PublishReqMsg _request = new PublishReqMsg {               
                CategoryID = request.CategoryID,      
              //  SubCategoryID=request.SubCategoryID,
                //CreatedBy = request.CreatedBy,
                Description = request.Description,
                Name = request.Name,
                WorkflowID = request.WorkflowID,
                WorkflowURI = request.WorkflowURI,
                IncrementVersion = request.IncrementVersion
            };
            return WFService.Publish(_request);
        }

        internal void AddProperty(WorkflowPE  data)
        {

            OpenWFFromRepository(this, new EventArgs<WorkflowPE>(data));
        }

    }
}
