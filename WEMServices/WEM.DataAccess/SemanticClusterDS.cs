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
    public class SemanticClusterDS : IDataAccess.IEntity<SemanticCluster>
    {
        public DataEntity dbCon;
        #region IEntity<SemanticCluster> Members

        public SemanticCluster GetOne(SemanticCluster entity)
        {
            SemanticCluster entityDb = null;
            using (dbCon = new DataEntity())
            {
                entityDb = dbCon.SemanticCluster.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
            }
            return entityDb;
        }

        public IQueryable<SemanticCluster> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<SemanticCluster> GetAll()
        {
            List<SemanticCluster> entities = null;
            using (dbCon = new DataEntity())
            {
                entities = dbCon.SemanticCluster.ToList();
                if (entities == null)
                    entities = new List<SemanticCluster>();
            }
            return entities;
        }

        public SemanticCluster Insert(SemanticCluster entity)
        {
            //first get the count of current rows
            List<SemanticCluster> clusters = GetAll().ToList();
            if (clusters != null && clusters.Count > 0)
                entity.RowKey = entity.Id = SemanticClusterDSExt.GenerateRowKey(clusters.Count);
            else
                entity.RowKey = entity.Id = SemanticClusterDSExt.GenerateRowKey(0); ;
            using (dbCon = new DataEntity())
            {
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                dbCon.SemanticCluster.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public SemanticCluster Update(SemanticCluster entity)
        {
            SemanticCluster entityInDb = GetOne(entity);
            if (entityInDb != null)
            {
                using (dbCon = new DataEntity())
                {
                    if (entity.LastModifiedOn == null || entity.LastModifiedOn == DateTime.MinValue)
                    {
                        entity.LastModifiedOn = DateTime.UtcNow;
                    }

                    entity.CreatedBy = entityInDb.CreatedBy;

                    dbCon.SemanticCluster.Attach(entity);
                    var e = dbCon.Entry(entity);                   
                    e.Property(e1 => e1.ClusterName).IsModified = true;
                    e.Property(e1 => e1.Description).IsModified = true;
                    e.Property(e1 => e1.Priority).IsModified = true;
                    e.Property(e1 => e1.LastModifiedBy).IsModified = true;
                    e.Property(e1 => e1.LastModifiedOn).IsModified = true;
                    e.Property(e1 => e1.IsDeleted).IsModified = true;
                    dbCon.SaveChanges();
                }
            }

            return entityInDb;
        }

        public SemanticCluster GetOneOnRowKey(SemanticCluster entity)
        {
            SemanticCluster entityDb = null;
            using (dbCon = new DataEntity())
            {
                entityDb = dbCon.SemanticCluster.Where(s => s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
            }
            return entityDb;
        }

        public bool Delete(SemanticCluster entity)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticCluster> InsertBatch(IList<SemanticCluster> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticCluster> UpdateBatch(IList<SemanticCluster> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticCluster> GetAll(SemanticCluster Entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class SemanticClusterDSExt
    {
        public static string GenerateRowKey(int currentClusterCount)
        {
            string rowkey = "SC_" + (currentClusterCount + 1).ToString("0000000000"); //e.g. SC_0000000001
            return rowkey;
        }
    }
}
