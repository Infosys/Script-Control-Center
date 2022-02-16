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
using Infosys.WEM.Resource.Entity;
using Infosys.WEM.Infrastructure.Common;
using IDataAccess = Infosys.WEM.Resource.IDataAccess;

namespace Infosys.WEM.Resource.DataAccess
{
    public class ScheduledRequestDS : IDataAccess.IEntity<ScheduledRequest>
    {
        public DataEntity dbCon;
        #region IEntity<ScheduledRequest> Members

        public ScheduledRequest GetOne(ScheduledRequest entity)
        {
            ScheduledRequest req = null;
            using (dbCon = new DataEntity())
            {
                req = dbCon.ScheduledRequest.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
            }
            return req;
        }

        public IQueryable<ScheduledRequest> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequest> GetAll()
        {
            throw new NotImplementedException();
        }

        public ScheduledRequest Insert(ScheduledRequest entity)
        {
            using (dbCon = new DataEntity())
            {
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }

                //entity.Priority to be used in future to set the order or execution

                dbCon.ScheduledRequest.Add(entity);
                dbCon.SaveChanges();
            }

            //get the entry just added, to get the scheduled request id
            entity = GetOne(entity);
            if (entity != null)
            {
                //add an entry to the scheduled request activities
                ScheduledRequestActivities activity = new ScheduledRequestActivities()
                {
                    CompanyId = entity.CompanyId,
                    CreatedOn = entity.CreatedOn,
                    IterationSetRoot = entity.IterationSetRoot,
                    ParentScheduledRequestId = entity.ParentId,
                    ScheduledRequestId = entity.Id,
                    Status = entity.State.Value
                };

                activity.PartitionKey = ScheduledRequestActivitiesDS_Ext.FormPartitionKey(activity);
                activity.RowKey = ScheduledRequestActivitiesDS_Ext.FormRowKey(activity);

                var actDe = new ScheduledRequestActivitiesDS().Insert(activity);
            }

            return entity;
        }

        public ScheduledRequest Update(ScheduledRequest entity)
        {
            ScheduledRequest entityInDb = GetOne(entity);
            if (entityInDb != null)
            {
                bool proceed = true;
                if (!string.IsNullOrEmpty(entityInDb.Executor) && !string.IsNullOrEmpty(entity.Executor) && entityInDb.Executor.ToLower() != entity.Executor.ToLower())
                {
                    proceed = false;
                }

                if (proceed)
                {
                    using (dbCon = new DataEntity())
                    {
                        dbCon.ScheduledRequest.Attach(entityInDb);
                        if (entity.ModifiedOn == null || entity.ModifiedOn == DateTime.MinValue)
                        {
                            entity.ModifiedOn = DateTime.UtcNow;
                        }

                        EntityExtension<ScheduledRequest>.ApplyOnlyChanges(entityInDb, entity);
                        dbCon.SaveChanges();
                    }

                    //add an entry to the scheduled request activities
                    ScheduledRequestActivities activity = new ScheduledRequestActivities()
                    {
                        CompanyId = entityInDb.CompanyId,
                        CreatedOn = DateTime.UtcNow,
                        IterationSetRoot = entityInDb.IterationSetRoot,
                        ParentScheduledRequestId = entityInDb.ParentId,
                        ScheduledRequestId = entityInDb.Id,
                        Status = entityInDb.State.Value
                    };

                    activity.PartitionKey = ScheduledRequestActivitiesDS_Ext.FormPartitionKey(activity);
                    activity.RowKey = ScheduledRequestActivitiesDS_Ext.FormRowKey(activity);

                    var actDe = new ScheduledRequestActivitiesDS().Insert(activity);
                }
            }

            return entityInDb;
        }

        public bool Delete(ScheduledRequest entity)
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequest> InsertBatch(IList<ScheduledRequest> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequest> UpdateBatch(IList<ScheduledRequest> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequest> GetAll(ScheduledRequest Entity)
        {
            using (dbCon = new DataEntity())
            {
                List<ScheduledRequest> reqs = null;
                if (Entity != null && !string.IsNullOrEmpty(Entity.PartitionKey))
                {
                    reqs = dbCon.ScheduledRequest.Where(node => node.PartitionKey.ToLower() == Entity.PartitionKey.ToLower()).ToList();
                }
                return reqs;
            }
        }

        public IList<ScheduledRequest> GetAllNewAndResubmit(List<ScheduledRequest> entities)
        {
            //get all those which are
            //1. at state new or resubmitted
            using (dbCon = new DataEntity())
            {
                List<ScheduledRequest> reqs = null;
                if (entities != null && entities.Count > 0)
                {
                    reqs = new List<ScheduledRequest>();
                    entities.ForEach(e =>
                    {                        
                        if (e != null && !string.IsNullOrEmpty(e.PartitionKey))
                        {
                            var tempreqs = dbCon.ScheduledRequest.Where(req => req.PartitionKey.ToLower() == e.PartitionKey.ToLower() && (req.State == 1 || req.State ==6)).ToList();
                            if (tempreqs != null && tempreqs.Count > 0)
                            {
                                reqs.AddRange(tempreqs);
                            }
                        }
                        
                    });
                }
                return reqs;
            }
        }

        public List<ScheduledRequest> GetLongInitiated(ScheduledRequest entity)
        {
            //get all those which are long initiated i.e.
            //1. state = 2 
            //2. and last modified time is one hour or more hours prior to current time
            using (dbCon = new DataEntity())
            {
                DateTime tempDate = DateTime.UtcNow.AddHours(-1);
                List<ScheduledRequest> reqs = dbCon.ScheduledRequest.Where(req => req.CategoryId == entity.CategoryId && req.State == 2 && req.ModifiedOn != null && tempDate >= req.ModifiedOn.Value).ToList();
                return reqs;
            }
        }

        #endregion
    }
}
