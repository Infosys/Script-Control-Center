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
    public class ExportConfigurationMasterSE_DE
    {
        public static DE.ExportConfigurationMaster ExportConfigurationMasterSEtoDE(SE.ExportConfigurationMaster serverSE)
        {
            DE.ExportConfigurationMaster serverDE = null;
            if (serverSE != null)
            {
                serverDE = new DE.ExportConfigurationMaster();
                if (serverSE.id > 0)
                    serverDE.id = serverSE.id;
                serverDE.TargetServerId = serverSE.TargetServerId;
                serverDE.TargetSystemUserId = serverSE.TargetSystemUserId;
                serverDE.TargetSystemPassword = serverSE.TargetSystemPassword;
                serverDE.ExportStatus = serverSE.ExportStatus;
                serverDE.TargetServerId = serverSE.TargetServerId;
                serverDE.CreatedBy = Utility.GetLoggedInUser();
                serverDE.CreatedOn = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(serverSE.ModifiedBy))
                    serverDE.ModifiedBy = serverSE.ModifiedBy;
                if (serverSE.ModifiedOn != null)
                    serverDE.ModifiedOn = serverSE.ModifiedOn;
                if (serverSE.CompletedOn != null)
                    serverDE.CompletedOn = serverSE.CompletedOn;
                serverDE.IsDeleted = false;
                if (!string.IsNullOrEmpty(serverSE.ScriptRepositoryBaseServerAddress))
                    serverDE.ScriptRepositoryBaseServerAddress = serverSE.ScriptRepositoryBaseServerAddress;
            }
            return serverDE;
        }

        public static DE.ExportConfigurationMaster PastExportConfigurationMasterSEtoDE(SE.ExportConfigurationMaster serverSE)
        {
            DE.ExportConfigurationMaster serverDE = null;
            if (serverSE != null)
            {
                serverDE = new DE.ExportConfigurationMaster();
                if (serverSE.id > 0)
                    serverDE.id = serverSE.id;
                serverDE.TargetServerId = serverSE.TargetServerId;
                serverDE.TargetSystemUserId = serverSE.TargetSystemUserId;
                serverDE.TargetSystemPassword = serverSE.TargetSystemPassword;
                serverDE.ExportStatus = serverSE.ExportStatus;
                serverDE.TargetServerId = serverSE.TargetServerId;
                serverDE.CreatedBy = serverSE.CreatedBy;
                if (!string.IsNullOrEmpty(serverSE.ModifiedBy))
                    serverDE.ModifiedBy = serverSE.ModifiedBy;
                if (serverSE.ModifiedOn != null)
                    serverDE.ModifiedOn = serverSE.ModifiedOn;
                if (serverSE.CompletedOn != null)
                    serverDE.CompletedOn = serverSE.CompletedOn;
                if (serverSE.CreatedOn != null)
                    serverDE.CreatedOn = serverSE.CreatedOn;
                serverDE.IsDeleted = false;
                if (!string.IsNullOrEmpty(serverSE.ScriptRepositoryBaseServerAddress))
                    serverDE.ScriptRepositoryBaseServerAddress = serverSE.ScriptRepositoryBaseServerAddress;
            }
            return serverDE;
        }


        public static SE.ExportConfigurationMaster ExportConfigurationMasterDEtoSE(DE.ExportConfigurationMaster serverDE)
        {
            SE.ExportConfigurationMaster ServerSE = null;
            if (serverDE != null)
            {
                ServerSE = new SE.ExportConfigurationMaster();
                ServerSE.id = serverDE.id;
                ServerSE.TargetServerId = serverDE.TargetServerId;
                ServerSE.TargetSystemUserId = serverDE.TargetSystemUserId;
                ServerSE.TargetSystemPassword = serverDE.TargetSystemPassword;
                ServerSE.ExportStatus = serverDE.ExportStatus;
                ServerSE.TargetServerId = serverDE.TargetServerId;
                if (serverDE.CompletedOn.HasValue)
                    ServerSE.CompletedOn = serverDE.CompletedOn.Value;
                ServerSE.CreatedBy = serverDE.CreatedBy;
                ServerSE.CreatedOn = serverDE.CreatedOn;
                ServerSE.IsDeleted = false;
            }
            return ServerSE;
        }

        public static List<SE.ExportConfigurationMaster> ExportConfigurationMasterDEtoSEList(List<DE.ExportConfigurationMaster> exportDE)
        {
            List<SE.ExportConfigurationMaster> exportSE = null;
            if (exportDE != null)
            {
                exportSE = new List<SE.ExportConfigurationMaster>();
                exportDE.ForEach(de =>
                {
                    exportSE.Add(ExportConfigurationMasterDEtoSE(de));
                });
            }
            return exportSE;
        }

        public static SE.PastExportConfigurationMasterDetails PastExportConfigurationMasterDEtoSE(DE.ExportConfigurationMaster serverDE)
        {
            SE.PastExportConfigurationMasterDetails ServerSE = null;
            if (serverDE != null)
            {
                ServerSE = new SE.PastExportConfigurationMasterDetails();
                ServerSE.masterExportId = serverDE.id;
                //ServerSE.id = serverDE.id;
                //ServerSE.TargetServerId = serverDE.TargetServerId;
                //ServerSE.TargetSystemUserId = serverDE.TargetSystemUserId;
                //ServerSE.TargetSystemPassword = serverDE.TargetSystemPassword;
                ServerSE.ExportStatus = serverDE.ExportStatus;
                //ServerSE.TargetServerId = serverDE.TargetServerId;
                if (serverDE.CompletedOn.HasValue)
                    ServerSE.CompletedOn = serverDE.CompletedOn.Value;
                ServerSE.CreatedBy = serverDE.CreatedBy;

                ServerSE.CreatedOn = serverDE.CreatedOn;
                if (serverDE.ModifiedOn.HasValue)
                {
                    if (serverDE.ModifiedOn.Value.ToString().Contains("1/1/0001"))
                        ServerSE.ModifiedOn = DateTime.Parse("01/01/2001");
                    else
                        ServerSE.ModifiedOn = serverDE.ModifiedOn.Value;
                }
                //ServerSE.IsDeleted = false;
            }
            return ServerSE;
        }

        public static List<SE.PastExportConfigurationMasterDetails> PastExportConfigurationMasterDEtoSEList(List<DE.ExportConfigurationMaster> exportDE)
        {
            List<SE.PastExportConfigurationMasterDetails> exportSE = null;
            if (exportDE != null)
            {
                exportSE = new List<SE.PastExportConfigurationMasterDetails>();
                exportDE.ForEach(de =>
                {
                    exportSE.Add(PastExportConfigurationMasterDEtoSE(de));
                });
            }
            return exportSE;
        }

        //public static DE.ExportScriptConfigurationDetail ExportScriptConfigurationSEtoDE(SE.ExportScriptConfigurationDetails serverSE)
        //{
        //    DE.ExportScriptConfigurationDetail ServerDE = null;
        //    if (serverSE != null)
        //    {
        //        ServerDE = new DE.ExportScriptConfigurationDetail();

        //        if (serverSE.id != 0)
        //        {
        //            ServerDE.id = serverSE.id;
        //        }

        //        ServerDE.ExportConfigurationId = serverSE.ExportConfigurationId;
        //        ServerDE.SourceCategoryId = serverSE.SourceCategoryId;
        //        ServerDE.SourceScriptPath = serverSE.SourceScriptPath;
        //        ServerDE.TargetCategoryId = serverSE.TargetCategoryId;
        //        ServerDE.TargetScriptPath = serverSE.TargetScriptPath;
        //        ServerDE.SourceScriptId = serverSE.SourceScriptId;
        //        ServerDE.CreatedBy = serverSE.CreatedBy;
        //        ServerDE.CreatedOn = DateTime.UtcNow;
        //        ServerDE.IsDeleted = false;
        //    }
        //    return ServerDE;
        //}

        //public static List<DE.ExportScriptConfigurationDetail> ExportScriptConfigurationSEtoDEList(List<SE.ExportScriptConfigurationDetails> exportSE)
        //{
        //    List<DE.ExportScriptConfigurationDetail> exportDE = null;
        //    if (exportSE != null)
        //    {
        //        exportDE = new List<DE.ExportScriptConfigurationDetail>();
        //        exportSE.ForEach(de =>
        //        {
        //            exportDE.Add(ExportScriptConfigurationSEtoDE(de));
        //        });
        //    }
        //    return exportDE;
        //}


        //public static List<SE.ExportScriptConfigurationDetails> ExportScriptConfigurationDEtoSEList(List<DE.ExportScriptConfigurationDetail> exportDE)
        //{
        //    List<SE.ExportScriptConfigurationDetails> exportSE = null;
        //    if (exportDE != null)
        //    {
        //        exportSE = new List<SE.ExportScriptConfigurationDetails>();
        //        exportDE.ForEach(de =>
        //        {
        //            exportSE.Add(ExportScriptConfigurationDEtoSE(de));
        //        });
        //    }
        //    return exportSE;
        //}

        //public static SE.ExportScriptConfigurationDetails ExportScriptConfigurationDEtoSE(DE.ExportScriptConfigurationDetail serverDE)
        //{
        //    SE.ExportScriptConfigurationDetails ServerSE = null;
        //    if (serverDE != null)
        //    {
        //        ServerSE = new SE.ExportScriptConfigurationDetails();

        //        if (serverDE.id != 0)
        //        {
        //            ServerSE.id = serverDE.id;
        //        }

        //        ServerSE.ExportConfigurationId = serverDE.ExportConfigurationId;
        //        ServerSE.SourceCategoryId = serverDE.SourceCategoryId;
        //        ServerSE.SourceScriptPath = serverDE.SourceScriptPath;
        //        ServerSE.TargetCategoryId = serverDE.TargetCategoryId;
        //        ServerSE.TargetScriptPath = serverDE.TargetScriptPath;
        //        ServerSE.SourceScriptId = serverDE.SourceScriptId;
        //        ServerSE.CreatedBy = serverDE.CreatedBy;
        //        ServerSE.CreatedOn = serverDE.CreatedOn;
        //        ServerSE.IsDeleted = serverDE.IsDeleted;
        //    }
        //    return ServerSE;
        //}




        public static DE.ExportConfigurationMaster ExportMasterConfigurationDetailsSEtoDE(int exportId)
        {
            DE.ExportConfigurationMaster ServerDE = new DE.ExportConfigurationMaster();
            ServerDE.id = exportId;
            ServerDE.IsDeleted = false;
            ServerDE.ModifiedBy = Utility.GetLoggedInUser();
            ServerDE.ModifiedOn = DateTime.UtcNow;
            return ServerDE;
        }

        public static DE.ExportConfigurationMaster ExportMasterConfigurationDetailsSEtoDE(int exportId, int exportStatus)
        {
            DE.ExportConfigurationMaster ServerDE = new DE.ExportConfigurationMaster();
            ServerDE.id = exportId;
            ServerDE.ExportStatus = exportStatus;
            ServerDE.ModifiedBy = Utility.GetLoggedInUser();
            ServerDE.ModifiedOn = DateTime.UtcNow;
            return ServerDE;
        }


        public static DE.ExportConfigurationMaster ExportMasterConfigurationDetailsSEtoDE(SE.ExportConfigurationMasterDetails serverSE, int targetServerId)
        {
            DE.ExportConfigurationMaster ServerDE = null;
            if (serverSE != null)
            {
                ServerDE = new DE.ExportConfigurationMaster();
                //ServerDE.TargetServerId = targetServerId;

                //if (serverSE.id != 0)
                //{
                //    ServerDE.id = serverSE.id;
                //}

                ServerDE.TargetServerId = targetServerId;// serverSE.AutomationServerTypeId;
                ServerDE.TargetSystemUserId = serverSE.TargetSystemUserName;
                
                    ServerDE.TargetSystemPassword = serverSE.TargetSystemPassword;
                ServerDE.ExportStatus = Convert.ToInt32(ExportStatus.Submitted);
                // ServerDE.TargetServerId = serverSE.TargetServerId;  // TO DO
                ServerDE.CreatedBy = Utility.GetLoggedInUser();
                ServerDE.CreatedOn = DateTime.UtcNow;
                //if (!string.IsNullOrEmpty(serverSE.ModifiedBy))
                //    ServerDE.ModifiedBy = serverSE.ModifiedBy;
                //if (serverSE.ModifiedOn != null)
                //    ServerDE.ModifiedOn = serverSE.ModifiedOn;
                //if (serverSE.CompletedOn != null)
                //    ServerDE.CompletedOn = serverSE.CompletedOn;
                ServerDE.IsDeleted = true;
            }
            return ServerDE;
        }

        public static List<DE.ExportScriptConfigurationDetail> ExportScriptConfigurationSEtoDEList(SE.ExportConfigurationMasterDetails exportSE, int exportConfigId)
        {
            List<DE.ExportScriptConfigurationDetail> exportDE = null;
            if (exportSE.ExportConfigurationDetails != null)
            {
                exportDE = new List<DE.ExportScriptConfigurationDetail>();
                exportSE.ExportConfigurationDetails.ForEach(de =>
                {
                    exportDE.Add(ExportScriptConfigurationSEtoDE(de,exportConfigId));
                });
            }
            return exportDE;
        }

        public static DE.ExportScriptConfigurationDetail ExportScriptConfigurationSEtoDE(SE.ExportConfigurationScriptDetails serverSE, int exportConfigId)
        {
            DE.ExportScriptConfigurationDetail ServerDE = null;
            if (serverSE != null)
            {
                ServerDE = new DE.ExportScriptConfigurationDetail();

                //if (serverSE.id != 0)
                //{
                //    ServerDE.id = serverSE.id;
                //}

                ServerDE.ExportConfigurationId = exportConfigId;
                ServerDE.SourceCategoryId = serverSE.SourceCategoryId;
                if (!string.IsNullOrEmpty(serverSE.SourceScriptPath))
                    ServerDE.SourceScriptPath = serverSE.SourceScriptPath;
                else
                    ServerDE.SourceScriptPath = "$";
                ServerDE.TargetCategoryId = serverSE.TargetCategoryId;
                if (!string.IsNullOrEmpty(serverSE.TargetScriptPath))
                    ServerDE.TargetScriptPath = serverSE.TargetScriptPath;
                else
                    ServerDE.TargetScriptPath = "$";
                ServerDE.SourceScriptId = serverSE.SourceScriptId;
                ServerDE.CreatedBy = Utility.GetLoggedInUser();
                ServerDE.CreatedOn = DateTime.UtcNow;
                ServerDE.IsDeleted = false;
            }
            return ServerDE;
        }

        public enum ExportStatus
        {
            Submitted = 0,
            InProgress = 1,
            Success = 2,
            Failed = 3,
            Resubmit = 4
        }

    }
}
