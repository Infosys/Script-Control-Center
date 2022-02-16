/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization.Json;
using System.Configuration;
using Infosys.WEM.Client;
using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Message;
using Infosys.WEM.Scripts.Service.Contracts;
//using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Scripts.Service.Contracts.Data;
using Infosys.WEM.Service.Common.Contracts;
using Infosys.WEM.Service.Common.Contracts.Message;
using Infosys.WEM.Node.Service.Contracts.Message;

namespace Infosys.ATR.Admin.Services
{


    internal class WFService
    {
        WFService()
        {

        }

        internal static GetAllGroupResMsg GetallGroups(string companyId)
        {
            GetAllGroupResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.GetAllGroups(companyId);
            return responseObj;
        }

        internal static GetAllUsersResMsg GetAnyUsers(string categoryId, string companyId)
        {
            GetAllUsersResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.GetAnyUser(categoryId, companyId);
            return responseObj;
        }

        internal static GetAllUsersResMsg GetAllUsersInaGroup(string groupId, string companyId)
        {
            GetAllUsersResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.GetAllUsersInAGroup(groupId, companyId);
            return responseObj;
        }


        internal static GetAllUsersResMsg GetAllUsers(string companyId)
        {
            GetAllUsersResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.GetAllUsers(companyId);
            return responseObj;
        }

        internal static GetAllRoleResMsg GetAllRoles(string companyId)
        {
            GetAllRoleResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.GetAllRoles(companyId);
            return responseObj;
        }

        internal static GetAllClustersResMsg GetAllClusters(string companyId)
        {
            GetAllClustersResMsg responseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            responseObj = cluster.ServiceChannel.GetAllClusters(companyId);
            return responseObj;
        }

        internal static UpdateUserResMsg UpdateUser(UpdateUserReqMsg updateUserReqMsg)
        {
            UpdateUserResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.UpdateUser(updateUserReqMsg);
            return responseObj;
        }

        internal static UpdateUserResMsg UpdateUserCategory(UpdateUserCategoryReqMsg updateUserReqMsg)
        {
            UpdateUserResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.UpdateUserCategory(updateUserReqMsg);
            return responseObj;
        }

        internal static AddUserResMsg AddUser(AddUserReqMsg addUserReqMsg)
        {
            AddUserResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.AddUser(addUserReqMsg);
            return responseObj;
        }

        internal static AddUserResMsg AddUserToAGroup(AddUserReqMsg addUserReqMsg)
        {
            AddUserResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.AddUserToAGroup(addUserReqMsg);
            return responseObj;
        }

        internal static DeleteUserResMsg DeleteUser(DeleteUserReqMsg user)
        {
            DeleteUserResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.DeleteUser(user);
            return responseObj;
        }

        internal static DeleteUserResMsg DeleteUserInAGroup(DeleteUserReqMsg user)
        {
            DeleteUserResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.DeleteUserInAGroup(user);
            return responseObj;
        }


        internal static AddGroupResMsg AddGroups(AddGroupReqMsg addGroupReqMsg)
        {
            AddGroupResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.AddGroup(addGroupReqMsg);
            return responseObj;
        }

        internal static AddCategoryResMsg AddCategory(AddCategoryReqMsg addCategoryReqMsg)
        {
            AddCategoryResMsg responseObj = null;        
            CommonRepository common = new CommonRepository();
            responseObj = common.ServiceChannel.AddCategory(addCategoryReqMsg);
            return responseObj;
        }

        internal static UpdateCategoryResMsg UpdateCategory(UpdateCategoryReqMsg updateCategoryReqMsg)
        {
            UpdateCategoryResMsg responseObj = null;
            CommonRepository common = new CommonRepository();
            responseObj = common.ServiceChannel.UpdateCategory(updateCategoryReqMsg);
            return responseObj;
        }

        internal static UpdateGroupResMsg UpdateGroups(UpdateGroupReqMsg updateGroupReqMsg)
        {
            UpdateGroupResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.UpdateGroup(updateGroupReqMsg);
            return responseObj;
        }

        internal static DeleteGroupResMsg DeleteGroup(DeleteGroupReqMsg deleteGroupReqMsg)
        {
            DeleteGroupResMsg responseObj = null;
            SecurityAccess access = new SecurityAccess();
            responseObj = access.ServiceChannel.DeleteGroup(deleteGroupReqMsg);
            return responseObj;
        }


        internal static GetAllNodesByClusterResMsg GetAllNodesByCluster(string clusterId)
        {
            GetAllNodesByClusterResMsg responseObj = null;
            SemanticCluster semanticCluser = new SemanticCluster();
            responseObj = semanticCluser.ServiceChannel.GetAllNodesByCluster(clusterId);
            return responseObj;
        }

        internal static GetRegisteredNodesResMsg GetNodes(string domain)
        {
            GetRegisteredNodesResMsg responseObj = null;

            RegisteredNodes nodes = new RegisteredNodes();
            string strcompany = System.Configuration.ConfigurationManager.AppSettings["Company"];
            if (string.IsNullOrEmpty(strcompany))
                strcompany = "0";
 
            responseObj = nodes.ServiceChannel.GetRegisteredNodes(domain,"3", strcompany);
            return responseObj;
        }

        internal static string AddSemanticCluster(AddSemanticClusterReqMsg sc)
        {
            string responseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            responseObj = cluster.ServiceChannel.AddSemanticCluster(sc);
            return responseObj;
        }

        internal static UpdateSemanticClusterResMsg UpdateSemanticCluster(UpdateSemanticClusterReqMsg sc)
        {
            UpdateSemanticClusterResMsg responseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            responseObj = cluster.ServiceChannel.UpdateSemanticCluster(sc);
            return responseObj;
        }

        internal static AddSemanticNodeClusterResMsg AddSemanticNodeCluster(AddSemanticNodeClusterReqMsg snc)
        {
            AddSemanticNodeClusterResMsg responseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            responseObj = cluster.ServiceChannel.AddSemanticNodeCluster(snc);
            return responseObj;
        }

        internal static UpdateSemanticNodeClusterResMsg UpdateSemanticNodeCluster(UpdateSemanticNodeClusterReqMsg snc)
        {
            UpdateSemanticNodeClusterResMsg responseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            responseObj = cluster.ServiceChannel.UpdateSemanticNodeCluster(snc);
            return responseObj;
        }

        internal static AddSemanticCategoryResMsg AddSemanticCategory(AddSemanticCategoryReqMsg addSemanticCategoryReqMsg)
        {
            AddSemanticCategoryResMsg resonseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            resonseObj = cluster.ServiceChannel.AddSemanticCategory(addSemanticCategoryReqMsg);
            return resonseObj;
        }

        internal static UpdateSemanticCategoryResMsg UpdateSemanticCategory(UpdateSemanticCategoryReqMsg updateSemanticCategoryReqMsg)
        {
            UpdateSemanticCategoryResMsg resonseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            resonseObj = cluster.ServiceChannel.UpdateSemanticCategory(updateSemanticCategoryReqMsg);
            return resonseObj;
        }

        internal static DeleteSemanticCategoryResMsg DeleteSemanticCategory(DeleteSemanticCategoryReqMsg updateSemanticCategoryReqMsg)
        {
            DeleteSemanticCategoryResMsg resonseObj = null;
            SemanticCluster cluster = new SemanticCluster();
            resonseObj = cluster.ServiceChannel.DeleteSemanticCategory(updateSemanticCategoryReqMsg);
            return resonseObj;
        }

        internal static GetAllClustersByCategoryResMsg GetAllClustersByCategory(int categoryId)
        {
            GetAllClustersByCategoryResMsg resonseObj = null;
            CommonRepository common = new CommonRepository();
            resonseObj = common.ServiceChannel.GetAllClustersByCategory(categoryId.ToString());
            return resonseObj;
        }
    }
}
