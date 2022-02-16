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
using System.Configuration;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Text.RegularExpressions;
using System.DirectoryServices.AccountManagement;

namespace Infosys.WEM.Infrastructure.Common
{
    public class Utility
    {
        public static string GetLoggedInUser()
        {
            //windows authentication
            var name = OperationContext.Current.ServiceSecurityContext.WindowsIdentity.Name;   
            
            //https
           if(String.IsNullOrEmpty(name))
                return OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;

            return name;
        }

        public static string GetLoggedInUser(bool domain)
        {
            if (domain)
            {
                var user = GetLoggedInUser();
                return user.Split(new string[] { "\\" }, StringSplitOptions.None)[1];
            }
            return GetLoggedInUser();
        }

        public static bool CheckValidUser(string alias)
        {
            string connString = ConfigurationManager.AppSettings["ADConnectionString"];


            if (ValidationUtility.InvalidCharValidatorForUserAlias(alias))
                throw new Exception("Alias should be alphanumeric with allowed characters underscore and dot only");

            DirectorySearcher user = new DirectorySearcher(connString);
            user.Filter = String.Format("(sAMAccountName={0})", alias);
            var result = user.FindOne();
            if (result == null) return false;
            return true;
        }


        public static List<string> GetDLs(string alias)   
        {
            List<string> resultDls = new List<string>(); 
            string connString = ConfigurationManager.AppSettings["ADConnectionString"];

            if (ValidationUtility.InvalidCharValidatorForUserAlias(alias))
                throw new Exception("Alias should be alphanumeric with allowed characters underscore and dot only");

            DirectorySearcher user = new DirectorySearcher(connString);
            user.Filter = String.Format("(sAMAccountName={0})", alias);
            var result = user.FindOne();
            
            if (result != null)
            {
                DirectoryEntry dsresult = result.GetDirectoryEntry();
                dsresult.RefreshCache(new string[] { "tokenGroups" });

                //get recursive list of groupmembership of user
                foreach (byte[] resultBytes in dsresult.Properties["tokenGroups"])
                {
                    System.Security.Principal.SecurityIdentifier mySID = new
                            System.Security.Principal.SecurityIdentifier(resultBytes, 0);

                    DirectorySearcher sidSearcher = new DirectorySearcher();
                    sidSearcher = new DirectorySearcher(connString);
                    sidSearcher.Filter = "(objectSid=" + mySID.Value + ")";
                    sidSearcher.PropertiesToLoad.Add("distinguishedName");

                    SearchResult sidResult = sidSearcher.FindOne();

                    if (sidResult != null)
                    {
                        string distinguishedName = (string)sidResult.Properties["distinguishedName"][0];
                        string key = distinguishedName.Split(',')[0].Split('=')[1].Trim();
                        resultDls.Add(key.ToLower());
                    }
                }
            }
            return resultDls;
        }

        public static bool IsUserInGroup(string alias, string group)
        {
            string domain = ConfigurationManager.AppSettings["Domain"];
            PrincipalContext pcContext = new PrincipalContext(ContextType.Domain, domain);
            var user = UserPrincipal.FindByIdentity(pcContext, alias);

            GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(pcContext, group);

            foreach (Principal p in groupPrincipal.Members)
            {
                if (p.Name == "Domain Users")
                {
                    using (var domainContext = new PrincipalContext(ContextType.Domain, domain)) //domain name will also be configured
                    {
                        using (var foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, alias))
                        {
                            if (foundUser != null)
                                return true;
                        }
                    }
                }
                else if ((p.StructuralObjectClass == "group"))
                {
                    var grpPrincipal = GroupPrincipal.FindByIdentity(pcContext, p.Name);

                    if (user.IsMemberOf(groupPrincipal))
                        return true;
                }
                else
                {
                    if (user.IsMemberOf(groupPrincipal))
                        return true;
                }
            }
            return false;
        }
        public static string GetMachineName()
        {
            string machineName = Environment.MachineName;
            return machineName;
        }
    }
}
