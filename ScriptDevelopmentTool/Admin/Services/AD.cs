/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using Infosys.ATR.Admin.Entities;
using System.Windows.Forms;

namespace Infosys.ATR.Admin.Services
{
    public class AD
    {
        public static Users GetUserDetails(string alias)
        {
            string connString = ConfigurationManager.AppSettings["ADConnectionString"];
            Users users = null;
            string domainUsers = "domain users";

            if (!alias.ToLower().Equals(domainUsers))
            {
                if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForUserAlias(alias))
                    throw new Exception("Alias should be alphanumeric with allowed characters underscore and dot only");
            }
                //MessageBox.Show("Alias should be alphanumeric with allowed characters underscore and dot only", "Invalid alias ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return users;

            DirectorySearcher user = new DirectorySearcher(connString);
            user.Filter = String.Format("(sAMAccountName={0})", alias);
            var result = user.FindOne();            

            if (result != null)
            {       
                users = new Users();
                if (alias.ToLower().Equals(domainUsers))
                {
                    users.DisplayName = domainUsers;
                }
                else
                {
                    if (result.Path.Contains("Group"))
                        users.IsDL = true;
                    DirectoryEntry userDir = result.GetDirectoryEntry();
                    users.DisplayName = userDir.Properties["displayName"].Value.ToString();
                }
            }
            return users;
        }
    }
}
