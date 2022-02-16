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

using SE = Infosys.WEM.Node.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;
using Infosys.WEM.Node.Service.Contracts.Data;

namespace Infosys.WEM.Service.Implementation.Translators.RegisteredNodes
{
    public class ScheduledRequestActivities_SE_DE
    {
        public static DE.ScheduledRequestActivities ScheduledRequestActivitiesSEtoDE(SE.ScheduledRequestActivity activitySe)
        {
            DE.ScheduledRequestActivities activityDe= null;
            if (activitySe != null)
            {
                activityDe = new DE.ScheduledRequestActivities();
                activityDe.ScheduledRequestId = activitySe.ScheduledRequestId;
                activityDe.ParentScheduledRequestId = activitySe.ParentScheduledRequestId;
                activityDe.Status = (int)activitySe.Status;
                activityDe.IterationSetRoot = activitySe.IterationSetRoot;
                activityDe.CompanyId = activitySe.CompanyId;
                //map other properties as needed
            }
            return activityDe;
        }

        public static SE.ScheduledRequestActivity ScheduledRequestActivitiesDetoSE(DE.ScheduledRequestActivities activityDe)
        {
            SE.ScheduledRequestActivity activitySe = null;
            if (activityDe != null)
            {
                activitySe = new ScheduledRequestActivity();
                activitySe.ParentScheduledRequestId = activityDe.ParentScheduledRequestId;
                activitySe.ScheduledRequestId = activityDe.ScheduledRequestId;
                activitySe.Status = (RequestExecutionStatus)activityDe.Status;
                activitySe.IterationSetRoot = activityDe.IterationSetRoot;
                activitySe.CompanyId = activityDe.CompanyId.Value;
                //map other properties as needed
            }
            return activitySe;
        }

        public static List<SE.ScheduledRequestActivity> ScheduledRequestActivitiesListDetoSE(List<DE.ScheduledRequestActivities> activitiesDe)
        {
            List<SE.ScheduledRequestActivity> activitiesSe = null;
            if (activitiesDe != null && activitiesDe.Count > 0)
            {
                activitiesSe = new List<ScheduledRequestActivity>();
                activitiesDe.ForEach(de => {
                    activitiesSe.Add(ScheduledRequestActivitiesDetoSE(de));
                });
            }
            return activitiesSe;
        }
    }
}
