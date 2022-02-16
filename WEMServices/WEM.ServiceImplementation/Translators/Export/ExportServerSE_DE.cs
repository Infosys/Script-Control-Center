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
using SE = Infosys.WEM.Export.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.Service.Implementation.Translators.Export
{
    public class ExportServerSE_DE
    {
        public static DE.ExportServerDetail ServerSEtoDE(SE.ExportServerDetails serverSE)
        {
            DE.ExportServerDetail ServerDE = null;
            if (serverSE != null)
            {
                ServerDE = new DE.ExportServerDetail();

                if (serverSE.Id != 0)
                {
                    ServerDE.id = serverSE.Id;
                }

                ServerDE.DNSServer = serverSE.DNSServer;
                if (serverSE.TargetSystemId > 0)
                    ServerDE.TargetSystemId = serverSE.TargetSystemId;
                ServerDE.CasServer = serverSE.CasServer;
                ServerDE.CreatedBy = Utility.GetLoggedInUser();
                ServerDE.CreatedOn = DateTime.UtcNow;

            }
            return ServerDE;
        }


        public static List<SE.ExportServerDetails> ServerDEtoSE(List<DE.ExportServerDetail> serverDE)
        {
            List<SE.ExportServerDetails> ServerPE = null;
            if (serverDE != null)
            {
                ServerPE = new List<SE.ExportServerDetails>();

                serverDE.ForEach(de =>
                {
                    ServerPE.Add(ServerDEtoSE(de));
                });

            }
            return ServerPE;       
        }

        public static SE.ExportServerDetails ServerDEtoSE(DE.ExportServerDetail serverDE)
        {
            SE.ExportServerDetails ServerPE = null;
            if (serverDE != null)
            {
                ServerPE = new SE.ExportServerDetails();

                ServerPE.Id = serverDE.id;

                ServerPE.DNSServer = serverDE.DNSServer;
                ServerPE.TargetSystemId = serverDE.TargetSystemId;
                ServerPE.CasServer = serverDE.CasServer;
                ServerPE.CreatedBy = Utility.GetLoggedInUser();
                ServerPE.CreatedOn = DateTime.UtcNow;

            }
            return ServerPE;
        }


        public static DE.ExportServerDetail ExportServerSEtoDE(SE.ExportConfigurationMasterDetails serverSE)
        {
            DE.ExportServerDetail ServerDE = null;
            if (serverSE != null)
            {
                ServerDE = new DE.ExportServerDetail();               
                ServerDE.TargetSystemId = serverSE.AutomationServerTypeId;
                ServerDE.DNSServer = serverSE.AutomationServerIPAddress;
                ServerDE.CasServer = serverSE.CasServerIPAddress;
                ServerDE.CreatedBy = Utility.GetLoggedInUser();
                ServerDE.CreatedOn = DateTime.UtcNow;

            }
            return ServerDE;
        }
    }
}
