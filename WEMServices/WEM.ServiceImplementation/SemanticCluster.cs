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
    public abstract class SemanticCluster_ServiceBase : ISemanticCluster
    {
        #region ISemanticCluster Members

        public virtual string AddSemanticCluster(AddSemanticClusterReqMsg value)
        {
            return null;
        }

        public virtual AddSemanticNodeClusterResMsg AddSemanticNodeCluster(AddSemanticNodeClusterReqMsg value)
        {
            return null;
        }

        public virtual UpdateSemanticNodeClusterResMsg UpdateSemanticNodeCluster(UpdateSemanticNodeClusterReqMsg value)
        {
            return null;
        }

        public virtual UpdateSemanticClusterResMsg UpdateSemanticCluster(UpdateSemanticClusterReqMsg value)
        {
            return null;
        }

        public virtual GetAllNodesByClusterResMsg GetAllNodesByCluster(string clusterId)
        {
            return null;
        }

        public virtual GetAllClustersResMsg GetAllClusters(string clusterId)
        {
            return null;
        }


        public virtual AddSemanticCategoryResMsg AddSemanticCategory(AddSemanticCategoryReqMsg value)
        {
            return null;
        }

        public virtual UpdateSemanticCategoryResMsg UpdateSemanticCategory(UpdateSemanticCategoryReqMsg value)
        {
            return null;
        }

        public virtual DeleteSemanticCategoryResMsg DeleteSemanticCategory(DeleteSemanticCategoryReqMsg value)
        {
            return null;
        }

        #endregion
    }

    public class SemanticCluster : SemanticCluster_ServiceBase
    {
        public override string AddSemanticCluster(AddSemanticClusterReqMsg value)
        {
            string clusterId = null;
            if (value != null && value.Request != null && !string.IsNullOrEmpty(value.Request.Name))
            {
                try
                {
                    if (Security.Access.IsSuperAdmin())
                    {
                        SemanticClusterDS clusterDs = new SemanticClusterDS();
                        var de = Translators.RegisteredNodes.SemanticCluster_SE_DE.SemanticClusterSEtoDE(value.Request);
                        var isDuplicate = CheckForDuplicateName(de.ClusterName);
                        if (!isDuplicate)
                        {
                            var clusterDe = clusterDs.Insert(de);
                            if (clusterDe != null)
                                clusterId = clusterDe.Id;
                        }
                    }
                    else
                        throw new Exception("Your role does not have permission to create a semantic cluster");
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
            return clusterId;
        }

        private bool CheckForDuplicateName(string clusterName)
        {
            try
            {
                SemanticClusterDS clusterDs = new SemanticClusterDS();
                var clusters = clusterDs.GetAll().Where(c1 => c1.IsDeleted == false);
                var cluster = clusters.FirstOrDefault(c => c.ClusterName.ToLower() == clusterName.ToLower());
                if (cluster == null) return false;
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                if (rethrow)
                {
                    throw ex;
                }
                return true;
            }
            return true;
        }


        public override AddSemanticNodeClusterResMsg AddSemanticNodeCluster(AddSemanticNodeClusterReqMsg value)
        {
            AddSemanticNodeClusterResMsg response = null;
            if (value != null && value.Request != null)
            {
                try
                {
                    if (Security.Access.IsSuperAdmin())
                    {
                        SemanticNodeClusterDS nodeClusterDs = new SemanticNodeClusterDS();

                        if (NodeExists(value.Request.IapNodeId))
                        {
                            var nodeClusterDe = nodeClusterDs.Insert(Translators.RegisteredNodes.SemanticNodeCluster_SE_DE.SemanticNodeClusterSEtoDE(value.Request));
                            if (nodeClusterDe != null)
                            {
                                response = new AddSemanticNodeClusterResMsg() { ClusterId = nodeClusterDe.ClusterId, IAPNodeId = nodeClusterDe.IapNodeId };
                            }
                        }
                        else
                            throw new Exception("Node does not exist");
                    }
                    else
                        throw new Exception("Your role does not have permission to add a node to a semantic cluster");
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

        private bool NodeExists(string nodeId)
        {
            RegisteredNodesDS nodeDs = new RegisteredNodesDS();
            var nodes = nodeDs.GetAll();
            var node = nodes.FirstOrDefault(n => n.MachineName == nodeId);
            if (node == null) return false;
            return true;
        }

        public override UpdateSemanticNodeClusterResMsg UpdateSemanticNodeCluster(UpdateSemanticNodeClusterReqMsg value)
        {
            UpdateSemanticNodeClusterResMsg response = null;
            if (value != null)
            {
                response = new UpdateSemanticNodeClusterResMsg();
                try
                {
                    if (Security.Access.IsSuperAdmin())
                    {
                        SemanticNodeClusterDS nodeClusterDs = new SemanticNodeClusterDS();

                        if (NodeExists(value.IAPNodeId))
                        {
                            var exists = nodeClusterDs.GetOne(new Resource.Entity.SemanticNodeCluster
                            {
                                PartitionKey = value.ClusterId,
                                RowKey = value.IAPNodeId + ";" + value.Domain
                            });

                            if (exists != null)
                            {
                                var updatedDe = nodeClusterDs.Update(Translators.RegisteredNodes.SemanticNodeCluster_SE_DE.SemanticNodeClusterSEtoDE
                                    (new Node.Service.Contracts.Data.SemanticNodeCluster()
                                    {
                                        ClusterId = value.ClusterId,
                                        IapNodeId = value.IAPNodeId,
                                        IsDeleted = !value.IsEnabled,
                                        Domain = value.Domain
                                    }));

                                if (updatedDe != null)
                                    response.IsSuccess = true;
                            }
                            else
                            {
                                var nodeadded = AddSemanticNodeCluster(new AddSemanticNodeClusterReqMsg
                                {

                                    Request = new Node.Service.Contracts.Data.SemanticNodeCluster
                                    {

                                        ClusterId = value.ClusterId,
                                        IapNodeId = value.IAPNodeId,
                                        IsDeleted = false,
                                        Domain = value.Domain
                                    }

                                });

                                if (nodeadded != null)
                                    response.IsSuccess = true;
                            }
                        }
                        else
                            throw new Exception("Node does not exist");
                    }
                    else
                        throw new Exception("Your role does not have permission to update node on a semantic cluster");

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

        public override AddSemanticCategoryResMsg AddSemanticCategory(AddSemanticCategoryReqMsg value)
        {
            AddSemanticCategoryResMsg response = null;
            if (value != null && value.SemanticCategory != null)
            {
                try
                {
                    if (Security.Access.Role(value.SemanticCategory.CategoryId.ToString()))
                    {
                        SemanticCategoryDS semanticCategory = new SemanticCategoryDS();

                        var semanticCategories = semanticCategory.GetAll();

                        var sc1 = semanticCategories.FirstOrDefault(sc => sc.SemanticClusterId == value.SemanticCategory.ClusterId &&
                            sc.CategoryId == value.SemanticCategory.CategoryId);

                        Infosys.WEM.Resource.Entity.SemanticCategory result = null;

                        if (sc1 == null)
                        {
                            result = semanticCategory.Insert(new Resource.Entity.SemanticCategory
                            {

                                CategoryId = value.SemanticCategory.CategoryId,
                                CreatedBy = Utility.GetLoggedInUser(),
                                IsActive = value.SemanticCategory.IsActive,
                                RowKey = value.SemanticCategory.ClusterId,
                                SemanticClusterId = value.SemanticCategory.ClusterId,
                                SemanticClusterName = value.SemanticCategory.ClusterName

                            });
                        }
                        else
                        {
                            sc1.IsActive = true;
                            result = semanticCategory.Update(sc1);
                        }

                        if (result != null)
                        {
                            response = new AddSemanticCategoryResMsg { IsSuccess = true };
                        }
                    }
                    else
                        throw new Exception("You do not have access to add cluster to this category");
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

        public override UpdateSemanticCategoryResMsg UpdateSemanticCategory(UpdateSemanticCategoryReqMsg value)
        {
            UpdateSemanticCategoryResMsg response = null;
            if (value != null)
            {
                response = new UpdateSemanticCategoryResMsg();
                try
                {
                    if (Security.Access.Role(value.SemanticCategory.CategoryId.ToString()))
                    {
                        SemanticCategoryDS semanticCategoryDS = new SemanticCategoryDS();
                        var updatedDe = semanticCategoryDS.Update(new Resource.Entity.SemanticCategory
                        {

                            CategoryId = value.SemanticCategory.CategoryId,
                            IsActive = value.SemanticCategory.IsActive,
                            SemanticClusterId = value.SemanticCategory.ClusterId,
                            SemanticClusterName = value.SemanticCategory.ClusterName,
                            RowKey = value.SemanticCategory.ClusterId,
                            LastModifiedBy = Utility.GetLoggedInUser()
                        });

                        if (updatedDe != null)
                            response.IsSuccess = true;
                    }
                    else
                        throw new Exception("You do not have access to update cluster in this category");
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

        public override DeleteSemanticCategoryResMsg DeleteSemanticCategory(DeleteSemanticCategoryReqMsg value)
        {
            DeleteSemanticCategoryResMsg response = null;
            if (value != null)
            {
                response = new DeleteSemanticCategoryResMsg();
                try
                {
                    if (Security.Access.Role(value.SemanticCategory.CategoryId.ToString()))
                    {
                        SemanticCategoryDS semanticCategoryDS = new SemanticCategoryDS();
                        var updatedDe = semanticCategoryDS.Delete(new Resource.Entity.SemanticCategory
                        {
                            PartitionKey = value.SemanticCategory.CategoryId.ToString("00000"),
                            CategoryId = value.SemanticCategory.CategoryId,
                            IsActive = value.SemanticCategory.IsActive,
                            RowKey = value.SemanticCategory.ClusterId,
                            LastModifiedBy = Utility.GetLoggedInUser(),
                            SemanticClusterId = value.SemanticCategory.ClusterId,
                            SemanticClusterName = value.SemanticCategory.ClusterName
                        });

                        if (updatedDe != null)
                            response.IsSuccess = true;
                    }
                    else
                    {
                        throw new Exception("You do not have access to this category " + value.SemanticCategory.CategoryId);
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
            }
            return response;
        }

        public override UpdateSemanticClusterResMsg UpdateSemanticCluster(UpdateSemanticClusterReqMsg value)
        {
            UpdateSemanticClusterResMsg response = null;
            if (value != null)
            {
                response = new UpdateSemanticClusterResMsg();
                try
                {                    
                    SemanticClusterDS clusterDs = new SemanticClusterDS();

                    if (Security.Access.IsSuperAdmin())
                    {

                        if (!String.IsNullOrEmpty(value.ClusterNewName) && value.ClusterName != value.ClusterNewName)
                        {
                            if (CheckForDuplicateName(value.ClusterNewName))
                            {
                                response.IsSuccess = false;
                                throw new Exception("Semantic Category name " + value.ClusterNewName + "  is already present.Try giving a different name");
                            }
                        }

                        var updatedDe = clusterDs.Update(

                            Translators.RegisteredNodes.SemanticCluster_SE_DE.SemanticClusterSEtoDE(new Node.Service.Contracts.Data.SemanticCluster()
                        {
                            Id = value.ClusterId,
                            Name = value.ClusterName,
                            IsDeleted = value.IsEnabled,
                            NewName = value.ClusterNewName,
                            //LastModifiedBy = Utility.GetLoggedInUser(),
                            Description = value.Description,
                            Priority = value.Priority,
                            CompanyId = value.CompanyId
                        }
                        ));

                        if (updatedDe != null)
                            response.IsSuccess = true;
                    }
                    else
                        throw new Exception("Your role does not have permission to update this cluster");
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

        public override GetAllNodesByClusterResMsg GetAllNodesByCluster(string clusterId)
        {
            GetAllNodesByClusterResMsg response = null;
            if (!String.IsNullOrEmpty(clusterId))
            {
                response = new GetAllNodesByClusterResMsg();
                try
                {
                    SemanticNodeClusterDS nodeClusterDs = new SemanticNodeClusterDS();
                    var nodes = nodeClusterDs.GetAll().Where(node => node.IsDeleted == false &&
                        node.ClusterId == clusterId);
                    if (nodes != null)
                    {
                        RegisteredNodesDS regNodeDS = new RegisteredNodesDS();

                        response.Nodes = new List<Node.Service.Contracts.Data.RegisteredNode>();

                        var regNodes = regNodeDS.GetAll();

                        nodes.ToList().ForEach(n =>
                        {

                            var regNode = regNodes.FirstOrDefault(n1 => n1.MachineName == n.IapNodeId);

                            Node.Service.Contracts.Data.RegisteredNode dRegNode = new Node.Service.Contracts.Data.RegisteredNode
                            {
                                CompanyId = regNode.CompanyId.GetValueOrDefault(),
                                DotNetVersion = regNode.DotNetVersion,
                                ExecutionEngineSupported = regNode.ExecutionEngineSupported,
                                HostMachineDomain = regNode.DomainName,
                                HostMachineName = regNode.MachineName,
                                HttpPort = regNode.HttpPort,
                                Is64Bit = regNode.Is64Bit,
                                OSVersion = regNode.OSVersion,
                                State = GetState(regNode.IsActive),
                                TcpPort = regNode.TcpPort,
                                WorkflowServiceVersion = regNode.WorkflowServiceVersion
                            };

                            response.Nodes.Add(dRegNode);

                        });
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
            }
            return response;
        }

        private Node.Service.Contracts.Data.NodeState GetState(bool p)
        {
            if (p)
                return Node.Service.Contracts.Data.NodeState.Active;
            return Node.Service.Contracts.Data.NodeState.InActive;
        }

        public override GetAllClustersResMsg GetAllClusters(string companyId)
        {
            GetAllClustersResMsg response = null;
            if (!String.IsNullOrEmpty(companyId))
            {
                response = new GetAllClustersResMsg();
                try
                {
                    SemanticClusterDS nodeClusterDs = new SemanticClusterDS();
                    var nodes = nodeClusterDs.GetAll();
                    if (nodes != null)
                    {
                        response.Nodes = nodes.Where(n => n.CompanyId == Convert.ToInt32(companyId) && n.IsDeleted == false).Select(sc =>
                            new Infosys.WEM.Node.Service.Contracts.Data.SemanticCluster
                            {
                                Id = sc.Id,
                                Name = sc.ClusterName,
                                Priority = sc.Priority.GetValueOrDefault(),
                                Description = sc.Description
                            }).ToList();
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
            }
            return response;
        }
    }
}
