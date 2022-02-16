/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using Infosys.WEM.Infrastructure.SecurityCore;
using Infosys.WEM.Business.Component;
using Infosys.WEM.SecurityAccess.Contracts;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using Infosys.WEM.SecurityAccess.Contracts.Message;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Resource.DataAccess;
using Infosys.WEM.Service.Implementation.Translators;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class SecurityAccess_ServiceBase : ISecurityAccess
    {
        public virtual AddGroupResMsg AddGroup(AddGroupReqMsg value)
        {
            return null;
        }

        public virtual DeleteGroupResMsg DeleteGroup(DeleteGroupReqMsg value)
        {
            return null;
        }

        public virtual UpdateGroupResMsg UpdateGroup(UpdateGroupReqMsg value)
        {
            return null;
        }

        public virtual GetAllGroupResMsg GetAllGroups(string companyId)
        {
            return null;
        }

        public virtual AddRoleResMsg AddRole(AddRoleReqMsg value)
        {
            return null;
        }

        public virtual DeleteRoleResMsg DeleteRole(DeleteRoleReqMsg value)
        {
            return null;
        }

        public virtual UpdateRoleResMsg UpdateRole(UpdateRoleReqMsg value)
        {
            return null;
        }

        public virtual GetAllRoleResMsg GetAllRoles(string companyId)
        {
            return null;
        }

        public virtual AddGroupAccessResMsg AddGroupAccess(AddGroupAccessReqMsg value)
        {
            return null;
        }

        public virtual DeleteGroupAccessResMsg DeleteGroupAccess(DeleteGroupAccessReqMsg value)
        {
            return null;
        }

        public virtual GetAllGroupAccessResMsg GetAllGroupAccess()
        {
            return null;
        }

        public virtual AddUserResMsg AddUser(AddUserReqMsg value)
        {
            return null;
        }

        public virtual DeleteUserResMsg DeleteUser(DeleteUserReqMsg value)
        {
            return null;
        }

        public virtual UpdateUserResMsg UpdateUser(UpdateUserReqMsg value)
        {
            return null;
        }

        public virtual GetAllUsersResMsg GetAllUsers(string companyId)
        {
            return null;
        }


        public virtual GetAllUsersResMsg GetAnyUser(string groupId, string companyId)
        {
            return null;
        }

        public virtual GetAllUsersResMsg GetAllUsersInAGroup(string groupId, string companyId)
        {
            return null;
        }

        public virtual GetAllUsersResMsg GetUsers(string alias, string companyId)
        {
            return null;
        }


        public virtual IsSuperAdminResMsg IsSuperAdmin(string alias, string companyId)
        {
            return null;
        }

        public virtual UpdateUserResMsg UpdateUserCategory(UpdateUserCategoryReqMsg value)
        {
            return null;
        }


        public virtual AddUserResMsg AddUserToAGroup(AddUserReqMsg value)
        {
            return null;
        }

        public virtual DeleteUserResMsg DeleteUserInAGroup(DeleteUserReqMsg value)
        {
            return null;
        }

        public virtual GetRoleAssignmentResMsg GetRoleAssignment(string alias, int companyId, int categoryId)
        {
            return null;
        }
    }

    public class SecurityAccess : SecurityAccess_ServiceBase
    {

        public override AddGroupResMsg AddGroup(AddGroupReqMsg value)
        {
            AddGroupResMsg response = new AddGroupResMsg();
            try
            {
                if (value != null)
                {
                    GroupDS groupDs = new GroupDS();
                    groupDs.Insert(GroupSE_DE.GroupSEToGroupDE(value.Group));
                    response.IsSuccess = true;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override DeleteGroupResMsg DeleteGroup(DeleteGroupReqMsg value)
        {
            DeleteGroupResMsg response = new DeleteGroupResMsg();
            try
            {
                if (value != null)
                {
                    GroupDS groupDs = new GroupDS();
                    Group groupSE = new Group();
                    groupSE.GroupId = value.GroupId;
                    groupSE.CompanyID = value.CompanyId;
                    groupDs.Delete(new Resource.Entity.Group { CompanyId = value.CompanyId, Id = value.GroupId });
                    response.IsSuccess = true;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateGroupResMsg UpdateGroup(UpdateGroupReqMsg value)
        {
            UpdateGroupResMsg response = new UpdateGroupResMsg();
            try
            {
                if (value != null)
                {
                    GroupDS groupDs = new GroupDS();
                    groupDs.Update(GroupSE_DE.GroupSEToGroupDE(value.Group));
                    response.IsSuccess = true;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetAllGroupResMsg GetAllGroups(string companyId)
        {
            GetAllGroupResMsg response = new GetAllGroupResMsg();
            try
            {
                GroupDS groupDS = new GroupDS();

                //Infosys.WEM.Resource.Entity.Group input = new Resource.Entity.Group();
                //input.Id = groupId;                
                response.Groups = Translators.GroupSE_DE.GroupDEListtoSEList(groupDS.GetAll(Convert.ToInt32(companyId)) as List<Infosys.WEM.Resource.Entity.Group>);
                //response.Groups = Translators.GroupSE_DE.GroupDEListtoSEList(groupDS.GetAll().ToList());
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

        public override AddRoleResMsg AddRole(AddRoleReqMsg value)
        {
            AddRoleResMsg response = new AddRoleResMsg();
            try
            {
                if (value != null)
                {
                    RoleDS roleDs = new RoleDS();
                    roleDs.Insert(RoleSE_DE.RoleSEToRoleDE(value.Role));
                    response.IsSuccess = true;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override DeleteRoleResMsg DeleteRole(DeleteRoleReqMsg value)
        {
            DeleteRoleResMsg response = new DeleteRoleResMsg();
            try
            {
                if (value != null)
                {
                    RoleDS roleDS = new RoleDS();
                    Role roleSE = new Role();
                    roleSE.RoleId = value.RoleId;
                    roleSE.CompanyId = value.CompanyId;
                    roleDS.Delete(RoleSE_DE.RoleSEToRoleDE(roleSE));
                    response.IsSuccess = true;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateRoleResMsg UpdateRole(UpdateRoleReqMsg value)
        {
            UpdateRoleResMsg response = new UpdateRoleResMsg();
            try
            {
                if (value != null)
                {
                    RoleDS roleDS = new RoleDS();
                    roleDS.Update(RoleSE_DE.RoleSEToRoleDE(value.Role));
                    response.IsSuccess = true;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetAllRoleResMsg GetAllRoles(string companyId)
        {
            GetAllRoleResMsg response = new GetAllRoleResMsg();
            try
            {
                RoleDS roleDS = new RoleDS();
                //Infosys.WEM.Resource.Entity.Role input = new Resource.Entity.Role();
                //input.Id = RoleId;
                response.Roles = Translators.RoleSE_DE.RoleDEListtoSEList(roleDS.GetAll(Convert.ToInt32(companyId)) as List<Infosys.WEM.Resource.Entity.Role>);
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

        public override AddGroupAccessResMsg AddGroupAccess(AddGroupAccessReqMsg value)
        {
            AddGroupAccessResMsg response = new AddGroupAccessResMsg();
            GroupAccess groupAccess = null;
            try
            {
                if (value != null)
                {
                    GroupAccessDS groupAccessDS = new GroupAccessDS();
                    var result = groupAccessDS.Insert(new Resource.Entity.GroupAccess
                    {
                        IsActive = true,
                        ParentId = value.GroupAccess.ParentId,
                        CreatedBy = Utility.GetLoggedInUser(),// value.GroupAccess.CreatedBy,
                        CreatedOn = DateTime.Now
                    });
                    groupAccess = new GroupAccess
                    {
                        // CreatedBy =Utility.GetLoggedInUser(),// result.CreatedBy,
                        // CreatedOn = DateTime.UtcNow,// result.CreatedOn,
                        GroupId = result.GroupId,
                        IsActive = result.IsActive,
                        // LastModifiedBy = Utility.GetLoggedInUser(),//result.LastModifiedBy,
                        // LastModifiedOn = DateTime.UtcNow,// result.LastModifiedOn,
                        ParentId = result.ParentId,
                    };
                    response.GroupAccess = groupAccess;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetAllGroupAccessResMsg GetAllGroupAccess()
        {
            GetAllGroupAccessResMsg response = new GetAllGroupAccessResMsg();
            try
            {
                GroupAccessDS groupAccessDS = new GroupAccessDS();
                var result = groupAccessDS.GetAll();

                response.GroupAccessList = new List<GroupAccess>();
                response.GroupAccessList = result.Select(
                    s => new GroupAccess
                    {//CreatedBy = s.CreatedBy,
                        //CreatedOn = s.CreatedOn,
                        GroupId = s.GroupId,
                        IsActive = s.IsActive,
                        // LastModifiedBy = s.LastModifiedBy,
                        // LastModifiedOn = s.LastModifiedOn,
                        ParentId = s.ParentId
                    }).ToList();
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

        public override DeleteGroupAccessResMsg DeleteGroupAccess(DeleteGroupAccessReqMsg value)
        {
            DeleteGroupAccessResMsg response = new DeleteGroupAccessResMsg();
            try
            {
                if (value != null)
                {
                    GroupAccessDS groupAccessDS = new GroupAccessDS();
                    groupAccessDS.Delete(new Resource.Entity.GroupAccess { GroupId = value.GroupAccess.GroupId });
                    response.IsSuccess = true;
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override AddUserResMsg AddUser(AddUserReqMsg value)
        {
            AddUserResMsg response = new AddUserResMsg();
            try
            {
                if (value != null)
                {
                    if (Security.Access.Role(value.User.CategoryId.ToString()))
                    {
                        UserDS userDS = new UserDS();
                        value.User.Alias = SecureData.UnSecure(value.User.Alias, ApplicationConstants.SECURE_PASSCODE);
                        userDS.Insert(UserSE_DE.UserSEToUserDE(value.User));
                        response.IsSuccess = true;
                    }
                    else
                        throw new Exception("You do not have permission to add user");
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override AddUserResMsg AddUserToAGroup(AddUserReqMsg value)
        {
            AddUserResMsg response = new AddUserResMsg();
            try
            {
                if (value != null)
                {
                    if (Security.Access.Role(value.User.CategoryId.ToString()))
                    {
                        UserDS userDS = new UserDS();
                        userDS.InsertUserToAGroup(UserSE_DE.UserSEToUserDE(value.User));
                        response.IsSuccess = true;
                    }
                    else
                        throw new Exception("You do not have permission to add user to a group");
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override DeleteUserResMsg DeleteUser(DeleteUserReqMsg value)
        {
            DeleteUserResMsg response = new DeleteUserResMsg();
            try
            {
                if (value != null)
                {
                    if (Security.Access.Role(value.User.CategoryId.ToString()))
                    {
                        UserDS userDS = new UserDS();
                        User userSE = new User();
                        userSE.UserId = value.User.UserId;
                        userSE.CompanyId = value.User.CompanyId;
                        userSE.Alias = SecureData.UnSecure(value.User.Alias, ApplicationConstants.SECURE_PASSCODE);
                        userSE.IsActive = false;
                        userDS.Delete(UserSE_DE.UserSEToUserDE(userSE));
                        response.IsSuccess = true;
                    }
                    else
                        throw new Exception("You do not have permission to delete user");
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override DeleteUserResMsg DeleteUserInAGroup(DeleteUserReqMsg value)
        {
            DeleteUserResMsg response = new DeleteUserResMsg();
            try
            {
                if (value != null)
                {
                    if (Security.Access.Role(value.User.CategoryId.ToString()))
                    {
                        UserDS userDS = new UserDS();
                        User userSE = new User();
                        userSE.UserId = value.User.UserId;
                        userSE.CompanyId = value.User.CompanyId;
                        userSE.Alias = value.User.Alias;
                        userSE.IsActive = false;
                        userSE.IsActiveGroup = false;
                        userDS.DeleteUserInAGroup(UserSE_DE.UserSEToUserDE(userSE));
                        response.IsSuccess = true;
                    }
                    else
                        throw new Exception("You do not have permission to delete user from a group");
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateUserResMsg UpdateUser(UpdateUserReqMsg value)
        {
            UpdateUserResMsg response = new UpdateUserResMsg();
            try
            {
                if (value != null)
                {
                    if (Security.Access.Role(value.User.CategoryId.ToString()))
                    {
                        UserDS userDS = new UserDS();
                        value.User.Alias = SecureData.UnSecure(value.User.Alias, ApplicationConstants.SECURE_PASSCODE);
                        userDS.Update(UserSE_DE.UserSEToUserDE(value.User));
                        response.IsSuccess = true;
                    }
                    else
                        throw new Exception("You do not have permission to update user");
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override UpdateUserResMsg UpdateUserCategory(UpdateUserCategoryReqMsg value)
        {
            UpdateUserResMsg response = new UpdateUserResMsg();
            try
            {
                if (value != null)
                {
                    if (Security.Access.Role(value.CategoryId.ToString()))
                    {
                        ManageUsers user = new ManageUsers();
                        value.Groups.ForEach(g => { user.UpdateCategory(value.CategoryId, g); });
                        response.IsSuccess = true;
                    }
                    else
                        throw new Exception("You do not have access to update user");
                }
            }
            catch (Exception wemSecurityException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemSecurityException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetAllUsersResMsg GetAllUsers(string companyId)
        {
            GetAllUsersResMsg response = new GetAllUsersResMsg();
            try
            {
                UserDS userDS = new UserDS();
                var users = userDS.GetAll(Convert.ToInt32(companyId));
                users = users.Where(u => u.IsActive == true).ToList();
                response.Users = Translators.UserSE_DE.UserDEListtoSEList(users as List<Infosys.WEM.Resource.Entity.User>);
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

        public override GetAllUsersResMsg GetAnyUser(string categoryId, string companyId)
        {
            GetAllUsersResMsg response = new GetAllUsersResMsg();
            try
            {
                if (Security.Access.Check(categoryId))
                {
                    UserDS userDS = new UserDS();
                    response.Users = userDS.GetAnyUser(new Resource.Entity.User
                    {
                        CategoryId = Convert.ToInt32(categoryId),
                        CompanyId = Convert.ToInt32(companyId)
                    }).Select(u => new User
                    {

                        Alias = u.Alias,
                        CompanyId = u.CompanyId,
                        //  CreatedBy = u.CreatedBy,
                        DisplayName = u.DisplayName,
                        CategoryId = u.CategoryId.GetValueOrDefault(),
                        IsActive = u.IsActive.GetValueOrDefault(),
                        //  LastModifiedBy = u.LastModifiedBy,
                        Role = u.Role,
                        UserId = u.Id,
                        IsDL = u.IsDL.GetValueOrDefault(),
                        GroupId = u.GroupId

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
            return response;
        }

        public override GetAllUsersResMsg GetAllUsersInAGroup(string groupId, string companyId)
        {
            GetAllUsersResMsg response = new GetAllUsersResMsg();
            try
            {
                UserDS userDS = new UserDS();
                response.Users = userDS.GetAllUsersInAGroup(new Resource.Entity.User
                {
                    GroupId = Convert.ToInt32(groupId),
                    CompanyId = Convert.ToInt32(companyId)
                }).Select(u => new User
                {

                    Alias = u.Alias,
                    CompanyId = u.CompanyId,
                    //  CreatedBy = u.CreatedBy,
                    DisplayName = u.DisplayName,
                    CategoryId = u.CategoryId.GetValueOrDefault(),
                    IsActive = u.IsActive.GetValueOrDefault(),
                    //  LastModifiedBy = u.LastModifiedBy,
                    Role = u.Role,
                    UserId = u.Id,
                    IsDL = u.IsDL.GetValueOrDefault(),
                    GroupId = u.GroupId

                }).ToList();
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

        public override GetAllUsersResMsg GetUsers(string alias, string companyId)
        {
            GetAllUsersResMsg response = new GetAllUsersResMsg();
            try
            {
                UserDS userDS = new UserDS();

                alias = SecureData.UnSecure(alias, ApplicationConstants.SECURE_PASSCODE);

                var u = userDS.GetAllUsers(alias, companyId);

                u = u.Where(users => users.IsActive == true).ToList();

                if (u != null && u.Count > 0)
                {
                    response.Users = new List<User>();

                    u.ForEach(user =>
                    {

                        response.Users.Add(new User
                        {

                            Alias = user.Alias,
                            CompanyId = user.CompanyId,
                            //  CreatedBy = user.CreatedBy,
                            DisplayName = user.DisplayName,
                            CategoryId = user.CategoryId.GetValueOrDefault(),
                            IsActive = user.IsActive.GetValueOrDefault(),
                            // LastModifiedBy = user.LastModifiedBy,
                            Role = user.Role,
                            UserId = user.Id,
                            GroupId = user.GroupId

                        });

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
            return response;
        }

        public override IsSuperAdminResMsg IsSuperAdmin(string alias, string companyId)
        {
            IsSuperAdminResMsg response = new IsSuperAdminResMsg();
            try
            {
                alias = SecureData.UnSecure(alias, ApplicationConstants.SECURE_PASSCODE);
                response.IsSuperAdmin = new SuperAdminDS().IsSuperAdmin(new Resource.Entity.SuperAdmin { Alias = alias, CompanyId = Convert.ToInt32(companyId) });
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

        public override GetRoleAssignmentResMsg GetRoleAssignment(string alias, int companyId, int categoryId)
        {
            GetRoleAssignmentResMsg response = new GetRoleAssignmentResMsg();
            try
            {
                if (string.IsNullOrEmpty(alias) || companyId==0 || categoryId==0)
                {
                    WEMValidationException validateException = new WEMValidationException(ErrorMessages.Method_Input_Parameters_Invalid);
                    List<ValidationError> validateErrs = new List<ValidationError>();
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = Errors.ErrorCodes.Method_Input_Parameters_Invalid.ToString();
                    validationErr.Description = ErrorMessages.Method_Input_Parameters_Invalid;
                    validateErrs.Add(validationErr);

                    if (validateErrs.Count > 0)
                    {
                        validateException.Data.Add("ValidationErrors", validateErrs);
                        throw validateException;
                    }
                }
                alias = SecureData.UnSecure(alias, ApplicationConstants.SECURE_PASSCODE);
                response.isSA = new SuperAdminDS().IsSuperAdmin(new Resource.Entity.SuperAdmin { Alias = alias, CompanyId = companyId });
                //Get Role Id
                int roleId = Security.Access.GetRoleId(alias,companyId,categoryId);

                //Get Role Details
                RoleDS roleDS = new RoleDS();
                Infosys.WEM.Resource.Entity.Role input = new Resource.Entity.Role();
                input.Id = roleId;
                response.Role = Translators.RoleSE_DE.RoleDEToRoleSE(roleDS.GetOneIfActive(input));

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
