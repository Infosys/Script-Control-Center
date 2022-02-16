using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEMClient = Infosys.WEM.Client;
using Infosys.WEM.SecureHandler;
using System.ServiceModel;
using Infosys.WEM.SecurityAccess.Contracts.Message;


namespace Infosys.IAP.CommonClientLibrary
{
    public class Security
    {
        public static bool CheckAccessInRole(string alias, int companyId, int categoryId, string[] roles)
        {
            if (string.IsNullOrEmpty(alias) || companyId == 0 || categoryId == 0 || (roles.Count() == 0))
                throw new Exception("Invalid Parameters passed to check role access for user. Please try again with valid parameters.");;

            if (alias.Contains("\\"))
                alias = alias.Split(new string[] { "\\" }, StringSplitOptions.None)[1];

            
            var saClient = new WEMClient.SecurityAccess();
            alias = SecurePayload.Secure(alias, SecurePayload.PASS);

            var channel = saClient.ServiceChannel;
            GetRoleAssignmentResMsg roleAssignmentDetails = null;
            using (new OperationContextScope((IContextChannel)channel))
            {
                roleAssignmentDetails = channel.GetRoleAssignment(alias, companyId, categoryId);
            }

            if (roleAssignmentDetails.isSA)
                return true;
            if ((roleAssignmentDetails.Role != null) && (roles.Contains(roleAssignmentDetails.Role.Name.Trim(), StringComparer.OrdinalIgnoreCase)))
                return true;
            return false ;

        }
        public static bool CheckAccessInRole(int companyid, int categoryId, string[] roles)
        {
            var alias = System.Security.Principal.WindowsIdentity.GetCurrent().Name;            

            return CheckAccessInRole(alias, companyid, categoryId, roles);

        }


        //    var superAdmin = saClient.ServiceChannel.IsSuperAdmin(alias, companyid);

        //    if (superAdmin.IsSuperAdmin)
        //        return true;

        //    if (!string.IsNullOrEmpty(categoryId))
        //    {
        //        var user = saClient.ServiceChannel.GetUsers(alias, companyid);
        //        var u = user.Users.FirstOrDefault(u1 => u1.CategoryId == Convert.ToInt32(categoryId));
        //        //if user is a manager or analyst then allow to run the application
        //        //2 manager
        //        //3 analyst
        //        if (u != null)
        //        {
        //            if (u.Role == 2 || u.Role == 3)
        //                return true;
        //        }
        //    }

        //    return false;
        //}
    }
}
