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

namespace Infosys.WEM.Scripts.Resource.DataAccess
{
    public class ScriptParamDS : IDataAccess.IEntity<ScriptParams>
    {
        public DataEntity dbCon;
        #region IEntity<ScriptParams> Members

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of ScriptParams entity</param>
        /// <returns>object of ScriptParams entity</returns>
        public ScriptParams GetOne(ScriptParams entity)
        {
            ScriptParams _ScriptParam;
            using (dbCon = new DataEntity())
            {
                _ScriptParam = dbCon.ScriptParams.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _ScriptParam;
            }
        }

        public IQueryable<ScriptParams> GetAny()
        {
            return dbCon.ScriptParams;
        }

        public IList<ScriptParams> GetAll()
        {
            throw new NotImplementedException();
        }

        public ScriptParams Insert(ScriptParams entity)
        {
            throw new NotImplementedException();
        }

        public ScriptParams Update(ScriptParams entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ScriptParams entity)
        {
            throw new NotImplementedException();
        }

        public IList<ScriptParams> InsertBatch(IList<ScriptParams> entities)
        {
            using (dbCon = new DataEntity())
            {
                int idSeed = this.GetAny().Select(x => x.ParamId).Distinct().Count();
                foreach (ScriptParams entity in entities)
                {
                    idSeed++;
                    entity.RowKey = CategoryKeysExtension.GenerateRowKey(idSeed);
                    entity.ParamId = idSeed;
                    if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.ScriptParams.Add(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }

        public IList<ScriptParams> UpdateBatch(IList<ScriptParams> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (ScriptParams entity in entities)
                {

                    ScriptParams entityItem = dbCon.ScriptParams.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                    dbCon.ScriptParams.Attach(entityItem);

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
                    EntityExtension<ScriptParams>.ApplyOnlyChanges(entityItem, entity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on partitionkey 
        /// </summary>
        /// <param name="entity">ScriptParams entity having the partitonkey value</param>
        /// <returns>List of ScriptParams</returns>
        public IList<ScriptParams> GetAll(ScriptParams entity)
        {
            using (dbCon = new DataEntity())
            {
                List<ScriptParams> scriptParams = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                    scriptParams = dbCon.ScriptParams.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && !ent.IsDeleted).ToList<ScriptParams>();
                }
                return scriptParams;
            }
        }

        #endregion
    }
}
