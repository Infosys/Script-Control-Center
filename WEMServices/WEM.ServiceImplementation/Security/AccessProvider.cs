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
using System.ServiceModel;
using System.Configuration;

using DE = Infosys.WEM.Resource.Entity;
using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using Infosys.WEM.Resource.DataAccess;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Scripts.Resource.DataAccess;

namespace Infosys.WEM.Service.Implementation.Security
{
    internal class Access
    {
        internal Access()
        {
        }

       

        internal static bool Check(string categoryId)
        {
            var overrideCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideUserCategoryCheck"]);

            if (!overrideCheck)
            {
                string loggedinuser = Utility.GetLoggedInUser(true);

                if (IsSuperAdmin(loggedinuser))
                    return true;

                //if (CheckAccess(loggedinuser, categoryId))
                // return true;
                if (CheckinAllRoles(loggedinuser, categoryId))
                        return true;

                return false;
            }
            else
                return true;
        }

        internal static bool Check(string categoryId, string groupName)
        {


            var overrideCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideUserCategoryCheck"]);

            if (!overrideCheck)
            {
                string loggedinuser = Utility.GetLoggedInUser(true);

                if (IsSuperAdmin(loggedinuser))
                    return true;

                if (CheckAccess(loggedinuser, categoryId, groupName))
                    return true;

                return false;
            }
            else
                return true;
        }

        private static bool CheckAccess(string alias, string categoryId)
        {
            UserDS userDs = new UserDS();
            string group = ConfigurationManager.AppSettings["DL"];
            var users = userDs.GetAll();

            if (users.ToList().Exists(u => u.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase) && u.CategoryId.GetValueOrDefault().Equals(Convert.ToInt32(categoryId))))
                return true;
            else if (users.ToList().Exists(u => u.Alias.Equals(group, StringComparison.InvariantCultureIgnoreCase) && u.CategoryId.GetValueOrDefault().Equals(Convert.ToInt32(categoryId))))
            {
                if (Utility.IsUserInGroup(alias, group))
                    return true;
            }
            return false;
        }


        private static bool CheckAccess(string alias, string categoryId, string groupName)
        {
            UserDS userDs = new UserDS();

            var users = userDs.GetAll();

            if (groupName.Equals(Infosys.WEM.Infrastructure.Common.ApplicationConstants.DOMAIN_USERS))
            {
                string loggedinuser = Utility.GetLoggedInUser(true);
                if (alias.ToLower().Equals(loggedinuser.ToLower()))
                    return true;

                    //string userName= System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    //if (!string.IsNullOrEmpty(userName))
                    //{
                    //    string userNameWithoutDomain= userName.Substring(userName.IndexOf(@"\") + 1);
                    //    if (userNameWithoutDomain.ToLower().Equals(alias.ToLower()) && System.Security.Principal.WindowsIdentity.GetCurrent().IsAuthenticated)
                    //        return true;
                    //}
                    return false;
            }
            else if (alias.Equals(groupName, StringComparison.CurrentCultureIgnoreCase))
            {
                if (users.ToList().Exists(u => u.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase) && u.CategoryId.GetValueOrDefault().Equals(Convert.ToInt32(categoryId))))
                    return true;
            }
            else
            {
                if (users.ToList().Exists(u => u.Alias.Equals(groupName, StringComparison.InvariantCultureIgnoreCase) && u.CategoryId.GetValueOrDefault().Equals(Convert.ToInt32(categoryId))))
                    if (Utility.IsUserInGroup(alias, groupName))
                        return true;
            }
            return false;
        }




        private static bool CheckAccessInDL(string alias, string categoryId)
        {
            UserDS userDs = new UserDS();
            string strDL = ConfigurationManager.AppSettings["DL"];

            if (userDs.IsBelongsToUser(strDL, Convert.ToInt32(categoryId)))
                if (Utility.IsUserInGroup(alias, strDL))
                    return true;

            return false;
        }

        private static bool IsSuperAdmin(string alias)
        {
            SuperAdminDS superAdmin = new SuperAdminDS();
            var superAdmins = superAdmin.GetAll();
            if (superAdmins.ToList().FirstOrDefault(
                s => s.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase)) == null)
                return false;
            return true;
        }

        public static bool Role(string categoryId)
        {
            var overrideCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideUserCategoryCheck"]);

            if (!overrideCheck)
            {
                string loggedinuser = Utility.GetLoggedInUser(true);

                if (IsSuperAdmin(loggedinuser))
                    return true;

                if (CheckAccessForRole(loggedinuser, categoryId))
                    return true;

                return false;
            }
            else
                return true;
            
        }

        public static bool ManagerOrAnalyst(string categoryId)
        {
            var overrideCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideUserCategoryCheck"]);

            if (!overrideCheck)
            {
                string loggedinuser = Utility.GetLoggedInUser(true);

                if (IsSuperAdmin(loggedinuser))
                    return true;

                if (ManagerOrAnalyst(loggedinuser, categoryId))
                    return true;

                return false;
            }
            else
                return true;
        }

        public static int GetRoleId(string alias, int companyId, int categoryId)
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
            //Block39
            //DateTime processStartedTime = DateTime.Now;
            UserDS userDs = new UserDS();
            //Fetch list of all categories the user is associated with in a specific company
            var userCategoryList = userDs.GetAllUsersbyCategory(alias, companyId.ToString(), categoryId);
            //LogHandler.LogDebug(string.Format("Time taken by Block 39 : WEM-GetRoleId()- GetallUsers() :{0}", DateTime.Now.Subtract(processStartedTime).TotalSeconds), LogHandler.Layer.Business, null);
            //Sort the list according to the Role and get the highest role from the userCategoryList
            List<DE.User> SortedList = userCategoryList.OrderBy(o => o.Role).ToList();
            if (SortedList != null) //check if user is associated with any category
            {

                var user = SortedList.FirstOrDefault(u => u.CategoryId.GetValueOrDefault() == categoryId &&
                                                                        u.IsActive == true  );
                if (user != null) //check if category has a parent. 
                {
                    return user.Role; //Return UserRole Id in the category
                }
                else
                {
                    return 0; // User is not associated with any category and has no role designation
                }
            }
            else
            {
                return 0; // User is not been assigned to any category
            }
        }
        private static bool CheckAccessForRole(string alias, string categoryId)
        {
            UserDS userDs = new UserDS();
            var users = userDs.GetAll();

            var userCategories = users.ToList().Where(u => u.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase));

            var user = userCategories.ToList().FirstOrDefault(u => u.CategoryId.GetValueOrDefault() == Convert.ToInt32(categoryId));

            if (user == null) //check if category has a parent. 
            {
                CategoryDS catDS = new CategoryDS();
                var cat = catDS.GetAll().FirstOrDefault(c => c.CategoryId ==Convert.ToInt32(categoryId));
                if (cat == null)
                {
                    return false;
                }
                else
                {
                    var parentId = cat.ParentId.GetValueOrDefault();
                    var userCat = userCategories.ToList().FirstOrDefault(u => u.CategoryId.GetValueOrDefault() == Convert.ToInt32(parentId));
                    if (userCat == null)
                        return false;
                    else if (userCat != null && userCat.Role == 2)
                        return true;
                }                
            }
            else if (user != null && user.Role ==2 )
                return true;

            return false;
        }

        private static bool ManagerOrAnalyst(string alias, string categoryId)
        {
            IList<DE.User> users;
            DE.User user;
            GetUsersbyCategory(alias, categoryId, out users, out user);


            // if user is null, check if user present in any of DLs
            if (user == null)
            {
                var getDLs = users.ToList().Where(u => u.CategoryId.ToString().Equals(categoryId) && u.IsDL == true && u.IsActive == true);

                foreach (Infosys.WEM.Resource.Entity.User dlUser in getDLs.ToList())
                {
                    if (Infosys.WEM.Service.Implementation.Security.Access.Check(categoryId, dlUser.Alias.Trim()))
                        return true;
                }
                return false;
            }
            else if (user != null && (user.Role == 2 || user.Role == 3))
                return true;
            else
                return false;
        }

        private static bool CheckinAllRoles(string alias, string categoryId)
        {
            IList<DE.User> users;
            DE.User user;
            GetUsersbyCategory(alias, categoryId, out users, out user);


            // if user is null, check if user present in any of DLs
            if (user == null)
            {
                var getDLs = users.ToList().Where(u => (u.CategoryId.ToString().Equals(categoryId) && u.IsDL == true && u.IsActive == true) || u.CategoryId.ToString().Equals(categoryId) && u.Alias == Infosys.WEM.Infrastructure.Common.ApplicationConstants.DOMAIN_USERS && u.IsActive == true);

                //List<User> userDets = dbCon.User.Where(u => (u.PartitionKey == entity.PartitionKey && u.IsActive == true) || (u.IsDL == true && u.IsActive == true) || (u.Alias == "domain users" && u.IsActive == true)).ToList();

                foreach (Infosys.WEM.Resource.Entity.User dlUser in getDLs.ToList())
                {
                    if (Infosys.WEM.Service.Implementation.Security.Access.Check(categoryId, dlUser.Alias.Trim()))
                        return true;
                }
                return false;
            }
            else if (user != null && (user.Role == 2 || user.Role == 3|| user.Role == 4 || user.Role == 5))
                return true;
            else
                return false;
        }
        private static void GetUsersbyCategory(string alias, string categoryId, out IList<DE.User> users, out DE.User user)
        {
            UserDS userDs = new UserDS();
            users = userDs.GetAll();
            var userCategories = users.ToList().Where(u => u.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase));

            user = userCategories.ToList().FirstOrDefault(u => u.CategoryId.GetValueOrDefault() == Convert.ToInt32(categoryId));
        }

        internal static bool IsSuperAdmin()
        {
             var overrideCheck = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideUserCategoryCheck"]);

             if (!overrideCheck)
             {
                 string loggedinuser = Utility.GetLoggedInUser(true);

                 if (IsSuperAdmin(loggedinuser))
                     return true;
             }
             return false;
        }
    }
}
