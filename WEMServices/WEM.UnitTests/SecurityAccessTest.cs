/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Svc = Infosys.WEM.Service.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Infosys.WEM.SecurityAccess.Contracts.Message;
//using Infosys.WEM.Infrastructure.SecureHandler;

namespace Infosys.WEM.UnitTests
{


    /// <summary>
    ///This is a test class for SecurityAccessTest and is intended
    ///to contain all SecurityAccessTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SecurityAccessTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod()]
        public void AddGroupTest()
        {
            Svc.SecurityAccess target = new Svc.SecurityAccess(); // TODO: Initialize to an appropriate value
            AddGroupReqMsg value = new AddGroupReqMsg();
            value.Group = new global::Infosys.WEM.SecurityAccess.Contracts.Data.Group();
            value.Group.CompanyID = 1;
            value.Group.Description = "Reader";
            value.Group.IsActive = true;
            value.Group.Name = "Readers";
            value.Group.CreatedBy = "shuchi";

            //AddGroupResMsg expected = null; // TODO: Initialize to an appropriate value
            AddGroupResMsg actual = target.AddGroup(value);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void AddRoleTest()
        {
            Svc.SecurityAccess target = new Svc.SecurityAccess();
            AddRoleReqMsg value = new AddRoleReqMsg();
            value.Role = new global::Infosys.WEM.SecurityAccess.Contracts.Data.Role();
            value.Role.CreatedBy = "TestUser";
            value.Role.Description = "Script Executors";
            value.Role.IsActive = true;
            value.Role.Name = "ScriptExecutors";
            value.Role.CompanyId = 1;
            AddRoleResMsg expected = target.AddRole(value);
        }

        [TestMethod()]
        public void DeleteGroupTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            DeleteGroupReqMsg value = new DeleteGroupReqMsg();
            value.CompanyId = 1;
            value.GroupId = 2;
            DeleteGroupResMsg actual = target.DeleteGroup(value);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void DeleteRoleTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            DeleteRoleReqMsg value = new DeleteRoleReqMsg();
            value.RoleId = 2;
            value.CompanyId = 1;
            DeleteRoleResMsg actual = target.DeleteRole(value);
        }

        [TestMethod()]
        public void GetAllGroupsTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            //int groupId = 0;
            GetAllGroupResMsg actual;
            actual = target.GetAllGroups("1");
        }

        [TestMethod()]
        public void GetAllRolesTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();

            GetAllRoleResMsg actual = target.GetAllRoles("1");
        }

        [TestMethod()]
        public void UpdateGroupTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            UpdateGroupReqMsg value = new UpdateGroupReqMsg();
            value.Group = new global::Infosys.WEM.SecurityAccess.Contracts.Data.Group();
            value.Group.CompanyID = 1;
            value.Group.Description = "Has access only to View scripts";
            value.Group.IsActive = true;
            value.Group.Name = "Reader";
            value.Group.CreatedBy = "shuchi";
            value.Group.GroupId = 2;

            UpdateGroupResMsg actual = target.UpdateGroup(value);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void UpdateRoleTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            UpdateRoleReqMsg value = new UpdateRoleReqMsg();
            value.Role = new global::Infosys.WEM.SecurityAccess.Contracts.Data.Role();
            value.Role.CreatedBy = "TestUser";
            value.Role.Description = "Script Executors with Admin priviledge";
            value.Role.IsActive = true;
            value.Role.Name = "ScriptExecutors";
            value.Role.CompanyId = 1;
            value.Role.RoleId = 2;
            UpdateRoleResMsg actual = target.UpdateRole(value);
        }

        [TestMethod()]
        public void AddUserTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            AddUserReqMsg value = new AddUserReqMsg();
            value.User = new global::Infosys.WEM.SecurityAccess.Contracts.Data.User();
            value.User.CreatedBy = "TestUser";
            value.User.Alias = "Harry";
            value.User.IsActive = true;
            value.User.DisplayName = "Harryto";
            value.User.CompanyId = 1;
            value.User.CategoryId = 1;
            value.User.Role = 2;
            AddUserResMsg expected = target.AddUser(value);
        }

        [TestMethod()]
        public void DeleteUserTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            DeleteUserReqMsg value = new DeleteUserReqMsg();
            value.User = new SecurityAccess.Contracts.Data.User();
            value.User.CompanyId = 1;
            value.User.UserId = 3;
            DeleteUserResMsg actual = target.DeleteUser(value);
        }

        [TestMethod()]
        public void GetAllUsersTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();

            GetAllUsersResMsg actual = target.GetAllUsers("1");
        }

        [TestMethod()]
        public void UpdateUserTest()
        {
             Svc.SecurityAccess target = new Svc.SecurityAccess();
            UpdateUserReqMsg value = new UpdateUserReqMsg();
            value.User = new global::Infosys.WEM.SecurityAccess.Contracts.Data.User();
            value.User.CreatedBy = "TestUser";
            value.User.Alias = "Harry";
            value.User.IsActive = true;
            value.User.DisplayName = "Harryto";
            value.User.CompanyId = 1;
            value.User.CategoryId = 1;
            value.User.Role = 1;
            value.User.UserId = 3;
            UpdateUserResMsg actual = target.UpdateUser(value);
        }

        [TestMethod()]
        public void SecureHandlerTest()
        {
            //string strSecure=SecurePayload.Secure("Test", "IAP2GO_SEC!URE");
            //string plainText = SecurePayload.UnSecure(strSecure, "IAP2GO_SEC!URE");
        }

    }
}
