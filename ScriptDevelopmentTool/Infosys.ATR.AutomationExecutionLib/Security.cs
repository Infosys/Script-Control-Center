using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEMClient =Infosys.WEM.Client;
using Infosys.WEM.SecureHandler;

namespace Infosys.ATR.AutomationExecutionLib
{
    public class Security
    {
        public static bool CheckAccess(string companyid, string categoryId)
        {
            var alias = System.Security.Principal.WindowsIdentity.GetCurrent().Name; 
            alias = alias.Split(new string[] { "\\" }, StringSplitOptions.None)[1];

            var saClient = new WEMClient.SecurityAccess();
            alias = SecurePayload.Secure(alias, "IAP2GO_SEC!URE");

            var superAdmin = saClient.ServiceChannel.IsSuperAdmin(alias, companyid);

            if (superAdmin.IsSuperAdmin)
                return true;

            if (!string.IsNullOrEmpty(categoryId))
            {
                var user = saClient.ServiceChannel.GetUsers(alias, companyid);
                var u = user.Users.FirstOrDefault(u1 => u1.CategoryId == Convert.ToInt32(categoryId));
                //if user is a manager or analyst then allow to run the application
                //2 manager
                //3 analyst
                if (u != null)
                {
                    if (u.Role == 2 || u.Role == 3)
                        return true;
                }
            }

            return false;
        }
    }
}
