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
using Infosys.WEM.SecureHandler;

namespace Infosys.WEM.AutomationActivity.Designers
{
    public class Utilies
    {
        public string GetAlias()
        {
            var alias = System.Threading.Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
            alias = SecurePayload.Secure(alias, "IAP2GO_SEC!URE");
            return alias;

        }
    }
}
