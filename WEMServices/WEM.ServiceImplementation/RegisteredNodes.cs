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
using System.ServiceModel.Activation;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Node.Service.Contracts;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Resource.DataAccess;


namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class Nodes_ServiceBase : INodes
    {
        #region INodes Members

        public virtual RegisterResMsg Register(RegisterReqMsg value)
        {
            return null;
        }

        public virtual UnRegisterResMsg UnRegister(UnRegisterReqMsg value)
        {
            return null;
        }

        public virtual GetRegisteredNodesResMsg GetRegisteredNodes(string domain, string nodeType, string companyId)
        {
            return null;
        }

        #endregion
    }

    public partial class RegisteredNodes : Nodes_ServiceBase
    {
        public override RegisterResMsg Register(RegisterReqMsg request)
        {
            RegisterResMsg response = null;
            if (request != null && request.Node != null)
            {
                response = new RegisterResMsg();
                try
                {
                    RegisteredNodesDS nodeDs = new RegisteredNodesDS();
                    request.Node.State = Infosys.WEM.Node.Service.Contracts.Data.NodeState.Active;
                    Resource.Entity.RegisterredNodes node = Translators.RegisterredNodes.RegisteredNodes_SE_DE.RegisteredNodeSEtoDE(request.Node);
                    nodeDs.Insert(node);

                    //also make an entry in the ActiveRegisteredNodes
                    ActiveRegisteredNodesDS anodeDs = new ActiveRegisteredNodesDS();
                    Resource.Entity.ActiveRegisteredNodes anode = Translators.RegisterredNodes.ActiveRegisteredNodes_SE_DE.ActiveRegisteredNodesSEtoDe(request.Node);
                    anodeDs.Insert(anode);

                    response.IsSuccess = true;
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            return response;
        }

        public override UnRegisterResMsg UnRegister(UnRegisterReqMsg request)
        {
            UnRegisterResMsg response = null;
            if (request != null)
            {
                response = new UnRegisterResMsg();
                try
                {
                    Infosys.WEM.Node.Service.Contracts.Data.RegisteredNode nodeSe = new Node.Service.Contracts.Data.RegisteredNode();
                    nodeSe.HostMachineDomain = request.Domain;
                    nodeSe.HostMachineName = request.MachineName;
                    nodeSe.State = Node.Service.Contracts.Data.NodeState.Active; //InActive is not assigned here yet as the corresponding entry needs to be first fetched which has pk as Active.
                    //it will be set to InActive in NodeDs

                    RegisteredNodesDS nodeDs = new RegisteredNodesDS();
                    Resource.Entity.RegisterredNodes nodeDe = Translators.RegisterredNodes.RegisteredNodes_SE_DE.RegisteredNodeSEtoDE(nodeSe);
                    bool nodeResult = nodeDs.Delete(nodeDe);

                    //also remove the entry in the ActiveRegisteredNodes
                    ActiveRegisteredNodesDS anodeDs = new ActiveRegisteredNodesDS();
                    Resource.Entity.ActiveRegisteredNodes anode = Translators.RegisterredNodes.ActiveRegisteredNodes_SE_DE.ActiveRegisteredNodesSEtoDe(nodeSe);
                    bool anodeResult = anodeDs.Delete(anode);

                    response.IsSuccess = nodeResult && anodeResult;
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            return response;
        }

        public override GetRegisteredNodesResMsg GetRegisteredNodes(string domain, string nodeType, string companyId="0")
        {
            GetRegisteredNodesResMsg response = new GetRegisteredNodesResMsg();
            //response.Nodes = new List<Infosys.WEM.Node.Service.Contracts.Data.RegisteredNode>();
            try
            {
                RegisteredNodesDS nodeDs = new RegisteredNodesDS();
                Infosys.WEM.Node.Service.Contracts.Data.RegisteredNode nodeSe = new Node.Service.Contracts.Data.RegisteredNode();
                nodeSe.HostMachineDomain = domain;

                Resource.Entity.RegisterredNodes nodeDe = Translators.RegisterredNodes.RegisteredNodes_SE_DE.RegisteredNodeSEtoDE(nodeSe);
                response.Nodes = Translators.RegisterredNodes.RegisteredNodes_SE_DE.RegisterredNodeDEListToSEList(nodeDs.GetAll(nodeDe).ToList());
                
                //check for the nodeType
                //currently valid node types are- 1- Workflow, 2 – Script
                Infosys.WEM.Node.Service.Contracts.Data.NodeType typeOfNode;
                if (!string.IsNullOrEmpty(nodeType) && nodeType != "0" && Enum.TryParse<Infosys.WEM.Node.Service.Contracts.Data.NodeType>(nodeType, out typeOfNode))
                {
                    switch (typeOfNode)
                    {
                        case Node.Service.Contracts.Data.NodeType.Script:
                            //then return only those having nodename as SC_<machine_name>
                            response.Nodes = response.Nodes.Where(n => n.HostMachineName.ToLower().StartsWith("sc_")).ToList();
                            //then remove the SC_ in the machine name
                            //if (response.Nodes != null && response.Nodes.Count > 0)
                            //{
                            //    for (int i = 0; i < response.Nodes.Count; i++)
                            //    {
                            //        response.Nodes[i].HostMachineName = response.Nodes[i].HostMachineName.ToLower().Replace("sc_", "");
                            //    }
                            //}
                            break;
                        case Node.Service.Contracts.Data.NodeType.Workflow:
                            //then return only those having nodename as UI_<machine_name>
                            response.Nodes = response.Nodes.Where(n => n.HostMachineName.ToLower().StartsWith("ui_")).ToList();
                            //then remove the UI_ in the machine name
                            //if (response.Nodes != null && response.Nodes.Count > 0)
                            //{
                            //    for (int i = 0; i < response.Nodes.Count; i++)
                            //    {
                            //        response.Nodes[i].HostMachineName = response.Nodes[i].HostMachineName.ToLower().Replace("ui_", "");
                            //    }
                            //}
                            break;
                        case Node.Service.Contracts.Data.NodeType.WorkflowAndScript:
                            response.Nodes = response.Nodes.Where(n => (n.HostMachineName.ToLower().StartsWith("ui_") || n.HostMachineName.ToLower().StartsWith("sc_"))).ToList();
                            break;
                        case Node.Service.Contracts.Data.NodeType.All:
                            //i.e. return all types of  iap nodes i.e. those hosted on windows service, UI_ as well as SC_
                            //by default response.Nodes has all the nodes, hence no additional action needed here
                            break;
                    }
                }
                else
                {
                    //then return only those iap nodes registered thru the windows service
                    response.Nodes = response.Nodes.Where(n => !n.HostMachineName.ToLower().StartsWith("sc_") && !n.HostMachineName.ToLower().StartsWith("ui_")).ToList();                    
                }

                //filter on the basis of companyid if provided. valid comany id is >0
                int cmpId=0;
                if (!string.IsNullOrEmpty(companyId) && int.TryParse(companyId,out cmpId) && cmpId > 0 && response.Nodes!=null)
                {
                    response.Nodes = response.Nodes.Where(n => n.CompanyId == cmpId).ToList();
                }
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }
    }
}
