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
    public class WorkflowParamDS : IDataAccess.IEntity<WorkflowParams>
    {
        public DataEntity dbCon;
        #region IEntity<WorkflowParams> Members

        public WorkflowParams GetOne(WorkflowParams entity)
        {
            WorkflowParams _WorkflowParams;
            using (dbCon = new DataEntity())
            {
                _WorkflowParams = dbCon.WorkflowParams.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _WorkflowParams;
            }
        }

        public IQueryable<WorkflowParams> GetAny()
        {
            return dbCon.WorkflowParams;
        }

        public IList<WorkflowParams> GetAll()
        {
            throw new NotImplementedException();
        }

        public WorkflowParams Insert(WorkflowParams entity)
        {
            throw new NotImplementedException();
        }

        public WorkflowParams Update(WorkflowParams entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(WorkflowParams entity)
        {
            throw new NotImplementedException();
        }

        public IList<WorkflowParams> InsertBatch(IList<WorkflowParams> entities)
        {
            if (entities != null)
            {
                using (dbCon = new DataEntity())
                {
                    int idSeed = GetAny().Select(x => x.ParamId).Distinct().Count();
                    foreach (WorkflowParams entity in entities)
                    {
                        idSeed++;
                        entity.RowKey = WorkflowParamKeysExtension.GenerateRowKey(idSeed);
                        //entity.PartitionKey =  Workflow id
                        entity.ParamId = idSeed;
                        if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                        {
                            entity.CreatedOn = DateTime.UtcNow;
                        }
                        dbCon.WorkflowParams.Add(entity);
                    }
                    dbCon.SaveChanges();
                }
            }
            return entities;
        }

        public IList<WorkflowParams> UpdateBatch(IList<WorkflowParams> entities)
        {
            if (entities != null)
            {
                using (dbCon = new DataEntity())
                {
                    foreach (WorkflowParams entity in entities)
                    {

                        WorkflowParams entityItem = dbCon.WorkflowParams.Single(c => c.PartitionKey == entity.PartitionKey &&
                       c.RowKey == entity.RowKey);

                        dbCon.WorkflowParams.Attach(entityItem);

                        DateTime? lastModifiedOn = null;

                        lastModifiedOn = entityItem.ModifiedOn;

                        if (entity.ModifiedOn == null || entity.ModifiedOn == DateTime.MinValue)
                        {
                            entity.ModifiedOn = DateTime.UtcNow;
                        }
                        if (lastModifiedOn == entity.ModifiedOn)
                        {
                            entity.ModifiedOn = DateTime.UtcNow;
                        }

                        //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                        EntityExtension<WorkflowParams>.ApplyOnlyChanges(entityItem, entity);
                    }
                    dbCon.SaveChanges();
                }
            }
            return entities;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on partitionkey 
        /// </summary>
        /// <param name="entity">WorkflowParams entity having the partitonkey value</param>
        /// <returns>List of WorkflowParams</returns>
        public IList<WorkflowParams> GetAll(WorkflowParams entity)
        {
            using (dbCon = new DataEntity())
            {
                List<WorkflowParams> wfParams = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                    wfParams = dbCon.WorkflowParams.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && !ent.IsDeleted).ToList<WorkflowParams>();
                }
                return wfParams;
            }
        }

        #endregion
    }

    public static class WorkflowParamKeysExtension
    {
        public static string GenerateRowKey(int index)
        {
            return index.ToString("00000");
        }
    }
}
