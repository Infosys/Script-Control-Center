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
using System.Globalization;

namespace Infosys.WEM.Resource.DataAccess
{
    public class ScheduledRequestActivitiesDS : IDataAccess.IEntity<ScheduledRequestActivities>
    {
        public DataEntity dbCon;
        
        public ScheduledRequestActivities GetOne(ScheduledRequestActivities Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ScheduledRequestActivities> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequestActivities> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequestActivities> GetAllActivitiesForScheduledRequest(ScheduledRequestActivities entity)
        {
            List<ScheduledRequestActivities> activities = null;
            using (dbCon = new DataEntity())
            {
                activities = dbCon.ScheduledRequestActivities.Where(act => act.RowKey.ToLower().Contains(entity.ScheduledRequestId.ToLower())).ToList();
            }
            return activities;
        }

        public List<ScheduledRequestActivities> GetAllByPartitionKeyAndRowKeyRange(string partitionKey, string fromRowKey, string toRowKey)
        {
            List<ScheduledRequestActivities> activities = null;
            if (string.IsNullOrEmpty(partitionKey)
                   || string.IsNullOrEmpty(fromRowKey)
                   || string.IsNullOrEmpty(toRowKey))
                return null;
            using (dbCon = new DataEntity())
            {
                var activitiesRange = (from tempActivities in dbCon.ScheduledRequestActivities
                                      where tempActivities.PartitionKey.ToLower() == partitionKey.ToLower() && //i.e. only the parent level transactions and those part of iteration i.e. those without parent id in the PK
                                      tempActivities.RowKey.ToLower().CompareTo(fromRowKey.ToLower()) >= 0 &&
                                      tempActivities.RowKey.ToLower().CompareTo(toRowKey.ToLower()) <= 0
                                      select tempActivities);
                activities = activitiesRange.ToList();
            }
            return activities;
        }

        public List<string> GetAllChildrenIds(ScheduledRequestActivities entity)
        {
            List<string> ids = new List<string>();
            using (dbCon = new DataEntity())
            {
                var childactivities = dbCon.ScheduledRequestActivities.Where(act => act.PartitionKey.ToLower().Contains(entity.ScheduledRequestId.ToLower())).ToList();
                if (childactivities != null && childactivities.Count > 0)
                {
                    childactivities.ForEach(chd => {
                        ids.Add(chd.ScheduledRequestId);
                    });
                    if (ids.Count > 0)
                        ids = ids.Distinct().ToList();
                }
            }
            return ids;
        }

        public ScheduledRequestActivities Insert(ScheduledRequestActivities entity)
        {
            using (dbCon = new DataEntity())
            {
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                //form the partion and row keys i.e. aftre createdon date is assigned as it is used to form these keys
                entity.PartitionKey = ScheduledRequestActivitiesDS_Ext.FormPartitionKey(entity);
                entity.RowKey = ScheduledRequestActivitiesDS_Ext.FormRowKey(entity);
                dbCon.ScheduledRequestActivities.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public ScheduledRequestActivities Update(ScheduledRequestActivities entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ScheduledRequestActivities entity)
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequestActivities> InsertBatch(IList<ScheduledRequestActivities> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequestActivities> UpdateBatch(IList<ScheduledRequestActivities> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ScheduledRequestActivities> GetAll(ScheduledRequestActivities Entity)
        {
            throw new NotImplementedException();
        }
    }

    public class ScheduledRequestActivitiesDS_Ext
    {
        public static string FormPartitionKey(ScheduledRequestActivities Entity)
        {
            DateTime currentDateTime = Entity.CreatedOn;
            string pk;
            if(!string.IsNullOrEmpty(Entity.ParentScheduledRequestId))
                pk = string.Format("{0}_{1}_{2:D2}_{3}_{4}", Entity.CompanyId.Value.ToString("000"), 9999 - currentDateTime.Year, 13 - currentDateTime.Month, 7 - GetWeekOfMonth(currentDateTime), Entity.ParentScheduledRequestId);
            else
                pk = string.Format("{0}_{1}_{2:D2}_{3}", Entity.CompanyId.Value.ToString("000"), 9999 - currentDateTime.Year, 13 - currentDateTime.Month, 7 - GetWeekOfMonth(currentDateTime));
            return pk;
        }

        public static string FormRowKey(ScheduledRequestActivities Entity)
        {
            DateTime currentDateTime = Entity.CreatedOn;
            return string.Format("{0}_{1}_{2}", string.Format("{0:D19}", DateTime.MaxValue.Ticks - currentDateTime.Ticks), Entity.Status.ToString("00"), Entity.ScheduledRequestId);
        }

        private static int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);
            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }
    }
}
