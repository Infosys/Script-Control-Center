/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Configuration;

using Infosys.ATR.Admin.Constants;
using Infosys.ATR.Admin.Services;
using Infosys.ATR.Admin.Entities;

using Infosys.WEM.Node.Service.Contracts;
using Infosys.WEM.Node.Service.Contracts.Data;
using Infosys.WEM.Node.Service.Contracts.Message;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.WinForms;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.CompositeUI.EventBroker;

namespace Infosys.ATR.Admin.Views
{
    public class SemanticClusterPresenter : Presenter<ISemanticCluster>
    {
        [EventPublication(EventTopicNames.UpdateSematicTree, PublicationScope.Global)]
        public event EventHandler<EventArgs<SemanticGroup>> UpdateSematicTree;

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        public override void OnCloseView()
        {
            base.CloseView();
        }

        [EventSubscription(EventTopicNames.ShowSemanticCluster, ThreadOption.UserInterface)]
        public void ShowSemanticCluster(object sender, EventArgs<SemanticGroup> g)
        {
            this.View.Cluster = g.Data;
            this.View.Show();
        }

        internal void GetDomains()
        {
            var domains = ConfigurationManager.AppSettings["Domains"];
            this.View.Domains = domains.Split(',').ToList();
        }

        internal void GetActiveNodes(string domain)
        {
            var response = WFService.GetNodes(domain).Nodes;
            if (response != null && response.Count > 0)
            {
                var companyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]);

                if (response != null)
                {
                    BindingList<Nodes> nodes1 = new BindingList<Nodes>();

                    response.ForEach(a =>
                    {

                        if (a != null)
                        {

                            Nodes n1 = new Nodes
                            {
                                Name = a.HostMachineName,
                                CompanyId = a.CompanyId.ToString(),
                                Domain = a.HostMachineDomain,
                                DotNetVersion = a.DotNetVersion,
                                ExecutionEngineSupported = a.ExecutionEngineSupported,
                                HttpPort = a.HttpPort,
                                Is64Bit = a.Is64Bit,
                                OSVersion = a.OSVersion,
                                State = a.State.GetTypeCode().ToString(),
                                TcpPort = a.TcpPort,
                                WorkflowServiceVersion = a.WorkflowServiceVersion
                            };

                            nodes1.Add(n1);
                        }

                    });


                    this.View.Nodes = nodes1;

                }
            }


        }

        internal string AddCluster(SemanticGroup semanticGroup)
        {
            var response = WFService.AddSemanticCluster(new AddSemanticClusterReqMsg
             {
                 Request = new WEM.Node.Service.Contracts.Data.SemanticCluster
                 {
                     CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                     Description = semanticGroup.Description,                     
                     Id = semanticGroup.Id,
                     IsDeleted = false,
                     Name = semanticGroup.Name,
                     Priority = 1,
                   //  CreatedBy = semanticGroup.CreatedBy
                 }
             });

            return response;

        }

        internal void UpdateSemanticTree_Handler(SemanticGroup sg)
        {
            UpdateSematicTree(this, new EventArgs<SemanticGroup>(sg));
        }

        internal void AddClusterNode(string response, BindingList<Nodes> Selected)
        {
            foreach (Nodes n in Selected)
            {
                AddclusterNode(response, n);
            }
        }

        internal bool UpdateCluster(SemanticGroup sg,bool enabled)
        {
            var response = WFService.UpdateSemanticCluster(new UpdateSemanticClusterReqMsg
            {
                ClusterId = sg.Id,
                ClusterNewName = sg.NewName,
                ClusterName = sg.Name,
                IsEnabled = enabled,
                CompanyId = Convert.ToInt32(ConfigurationManager.AppSettings["Company"]),
                Description = sg.Description,
                Priority = sg.Priority,
                LastModifiedBy = System.Threading.Thread.CurrentPrincipal.Identity.Name
            });

            return response.IsSuccess;
        }

        private void AddclusterNode(string response, Nodes n)
        {
            var result = WFService.AddSemanticNodeCluster(new AddSemanticNodeClusterReqMsg
               {
                   Request = new SemanticNodeCluster
                   {
                       ClusterId = response,
                     //  CreatedBy = System.Threading.Thread.CurrentPrincipal.Identity.Name,
                       IapNodeId = n.Name,
                       IsDeleted = false,
                       Domain = n.Domain
                   }
               });
        }

       

        internal void UpdateClusterNode(string response, BindingList<Nodes> Selected,string semanticclusterId)
        {

            var nodesinCluster = WFService.GetAllNodesByCluster(semanticclusterId);

            foreach (Nodes n in Selected)
            {                
                var result = WFService.UpdateSemanticNodeCluster(new UpdateSemanticNodeClusterReqMsg
                    {
                        ClusterId = response,
                        IAPNodeId = n.Name,
                        IsEnabled = n.State == "Active" ? true:false,
                        //LastModifiedBy = System.Threading.Thread.CurrentPrincipal.Identity.Name,
                        Domain = n.Domain
                });                
            }
        }
      
    }
}
