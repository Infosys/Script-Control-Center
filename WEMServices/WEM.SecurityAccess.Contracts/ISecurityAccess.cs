/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Infosys.WEM.SecurityAccess.Contracts.Message;
using System.ServiceModel.Activation;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Infrastructure.Common.Validators;

using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Integration.WCF;

namespace Infosys.WEM.SecurityAccess.Contracts
{
    [ServiceContract]
    [ValidationBehavior]    
    public interface ISecurityAccess
    {
        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
       AddGroupResMsg AddGroup(AddGroupReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteGroupResMsg DeleteGroup(DeleteGroupReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateGroupResMsg UpdateGroup(UpdateGroupReqMsg value);

        [WebGet(UriTemplate = "/GetAllGroups/{companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllGroupResMsg GetAllGroups(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddRoleResMsg AddRole(AddRoleReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteRoleResMsg DeleteRole(DeleteRoleReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateRoleResMsg UpdateRole(UpdateRoleReqMsg value);


        [WebGet(UriTemplate = "/GetAllRoles/{companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllRoleResMsg GetAllRoles(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        AddGroupAccessResMsg AddGroupAccess(AddGroupAccessReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteGroupAccessResMsg DeleteGroupAccess(DeleteGroupAccessReqMsg value);

        [WebGet(UriTemplate = "/GetAllGroupAccess/")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllGroupAccessResMsg GetAllGroupAccess();

        [WebInvoke]
        [OperationContract]        
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]  
        AddUserResMsg AddUser(AddUserReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]        
        AddUserResMsg AddUserToAGroup(AddUserReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteUserResMsg DeleteUser(DeleteUserReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        DeleteUserResMsg DeleteUserInAGroup(DeleteUserReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateUserResMsg UpdateUser(UpdateUserReqMsg value);

        [WebInvoke]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        UpdateUserResMsg UpdateUserCategory(UpdateUserCategoryReqMsg value);

        [WebGet(UriTemplate = "/GetAllUsers/{companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllUsersResMsg GetAllUsers(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebGet(UriTemplate = "/GetAnyUser?categoryId={categoryId}&companyId={companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllUsersResMsg GetAnyUser(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string categoryId,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);


        [WebGet(UriTemplate = "/GetAllUsersInAGroup?groupId={groupId}&companyId={companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllUsersResMsg GetAllUsersInAGroup(
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string groupId,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebGet(UriTemplate = "/GetUsers?alias={alias}&companyId={companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetAllUsersResMsg GetUsers(
             [ADValidator(MessageTemplate = "User not found in AD")]
            string alias,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebGet(UriTemplate = "/IsSuperAdmin?alias={alias}&companyId={companyId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        IsSuperAdminResMsg IsSuperAdmin(
            [ADValidator(MessageTemplate="User not found in AD")]
            string alias,
             [SpecialCharacterValidator(MessageTemplate = "Special characters not allowed")]
            string companyId);

        [WebGet(UriTemplate = "/GetRoleAssignment?alias={alias}&cId={companyId}&catId={categoryId}")]
        [OperationContract]
        [FaultContract(typeof(ServiceFaultError))]
        [FaultContract(typeof(ValidationFault))]
        GetRoleAssignmentResMsg GetRoleAssignment(
            [ADValidator(MessageTemplate = "User not found in AD")]
            string alias,
            int companyId,
            int categoryId);


       
    }
}
