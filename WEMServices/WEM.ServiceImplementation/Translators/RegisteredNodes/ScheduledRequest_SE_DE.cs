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
using Infosys.WEM.Infrastructure.Common;

using SE = Infosys.WEM.Node.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Node.Service.Contracts.Data;

namespace Infosys.WEM.Service.Implementation.Translators.RegisteredNodes
{
    public class ScheduledRequest_SE_DE
    {
        public static DE.ScheduledRequest ScheduledRequestSeToDe(SE.ScheduledRequest schReqSE)
        {
            DE.ScheduledRequest schReqDE = null;
            if (schReqSE != null)
            {
                schReqDE = new DE.ScheduledRequest();
                schReqDE.PartitionKey = schReqSE.AssignedTo;
                if (string.IsNullOrEmpty(schReqSE.Id)) //i.e. for new request
                    schReqDE.RowKey = Guid.NewGuid().ToString();
                else
                    schReqDE.RowKey = schReqSE.Id;
                schReqDE.Id = schReqDE.RowKey;
                schReqDE.CategoryId = schReqSE.CategoryId;
                schReqDE.CreatedBy = schReqSE.Requestor;
                schReqDE.CreatedOn = DateTime.UtcNow;
                //execute on has to have a valid value as in case of indefinite case, subsequent scheduled request will be incremented by one day
                if (schReqSE.StopType == StopTypes.Indefinite && schReqSE.ExecuteOn == null)
                    schReqDE.ExecuteOn = DateTime.UtcNow;
                else
                    schReqDE.ExecuteOn = schReqSE.ExecuteOn;
                schReqDE.InputParameters = schReqSE.InputParameters;
                schReqDE.OutputParameters = schReqSE.OutputParameters;
                schReqDE.MachineName = schReqDE.AssignedTo = schReqSE.AssignedTo;
                schReqDE.ModifiedBy = Utility.GetLoggedInUser();
                schReqDE.ModifiedOn = DateTime.UtcNow;
                schReqDE.RequestId = schReqSE.RequestId;
                schReqDE.RequestType = (int)schReqSE.RequestType;
                schReqDE.RequestVersion = schReqSE.RequestVersion;
                schReqDE.State = (int)schReqSE.State;
                schReqDE.Message = schReqSE.Message;
                schReqDE.Priority = schReqSE.Priority;
                schReqDE.Executor = schReqSE.Executor;
                schReqDE.ParentId = schReqSE.ParentId;
                schReqDE.StopType = (int)schReqSE.StopType;
                schReqDE.CompanyId = schReqSE.CompanyId; //only while tranlating to de this to be mapped as this is not to be returned to the client
                schReqDE.IterationSetRoot = schReqSE.IterationSetRoot; //only while tranlating to de this to be mapped as this is not to be returned to the client
                schReqDE.IsIterationSetRoot = schReqSE.IsIterationSetRoot; //only while tranlating to de this to be mapped as this is not to be returned to the client
            }

            return schReqDE;
        }

        public static SE.ScheduledRequest ScheduledRequestDeToSe(DE.ScheduledRequest schReqDE)
        {
            SE.ScheduledRequest schReqSE = null;
            if (schReqDE != null)
            {
                schReqSE = new SE.ScheduledRequest();
                schReqSE.CategoryId = schReqDE.CategoryId;
                schReqSE.Requestor = schReqDE.CreatedBy;
                schReqSE.CreatedOn = schReqDE.CreatedOn;
                if (schReqDE.ExecuteOn != null)
                    schReqSE.ExecuteOn = schReqDE.ExecuteOn.Value;
                schReqSE.Id = schReqDE.Id;
                schReqSE.InputParameters = schReqDE.InputParameters;
                schReqSE.AssignedTo = schReqDE.AssignedTo;
                schReqSE.Message = schReqDE.Message;
                schReqSE.ModifiedBy = schReqDE.ModifiedBy;
                if (schReqDE.ModifiedOn != null)
                    schReqSE.ModifiedOn = schReqDE.ModifiedOn.Value;
                schReqSE.OutputParameters = schReqDE.OutputParameters;
                schReqSE.RequestId = schReqDE.RequestId;
                schReqSE.RequestType = (RequestTypeEnum)schReqDE.RequestType;
                if (schReqDE.RequestVersion != null)
                    schReqSE.RequestVersion = (int)schReqDE.RequestVersion;
                schReqSE.State = (RequestExecutionStatus)schReqDE.State;
                schReqSE.Priority = schReqDE.Priority;
                schReqSE.Executor = schReqDE.Executor;
                schReqSE.ParentId = schReqDE.ParentId;
                if (schReqDE.StopType != null)
                    schReqSE.StopType = (StopTypes)schReqDE.StopType;
            }
            return schReqSE;
        }

        public static List<SE.ScheduledRequest> ScheduledRequestListDeToSe(List<DE.ScheduledRequest> schReqDEs)
        {
            List<SE.ScheduledRequest> schReqSEs = null;
            if (schReqDEs != null && schReqDEs.Count > 0)
            {
                schReqSEs = new List<SE.ScheduledRequest>();
                schReqDEs.ForEach(de =>
                {
                    schReqSEs.Add(ScheduledRequestDeToSe(de));
                });
            }
            return schReqSEs;
        }
    }
}
