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
    public class SemanticNodeClusterDS : IDataAccess.IEntity<SemanticNodeCluster>
    {
        public DataEntity dbCon;
        #region IEntity<SemanticNodeCluster> Members

        public SemanticNodeCluster GetOne(SemanticNodeCluster entity)
        {
            SemanticNodeCluster entityDb = null;
            using (dbCon = new DataEntity())
            {
                entityDb = 
                    dbCon.SemanticNodeCluster.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
            }
            return entityDb;
        }

        public IQueryable<SemanticNodeCluster> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<SemanticNodeCluster> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                return dbCon.SemanticNodeCluster.ToList();
            }            
        }

        public SemanticNodeCluster Insert(SemanticNodeCluster entity)
        {
            using (dbCon = new DataEntity())
            {
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                dbCon.SemanticNodeCluster.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public SemanticNodeCluster Update(SemanticNodeCluster entity)
        {
            SemanticNodeCluster entityInDb = GetOne(entity);
            if (entityInDb != null)
            {
                using (dbCon = new DataEntity())
                {
                    dbCon.SemanticNodeCluster.Attach(entityInDb);
                    if (entity.LastModifiedOn == null || entity.LastModifiedOn == DateTime.MinValue)
                    {
                        entity.LastModifiedOn = DateTime.UtcNow;
                    }

                    EntityExtension<SemanticNodeCluster>.ApplyOnlyChanges(entityInDb, entity);
                    dbCon.SaveChanges();
                }
            }

            return entityInDb;
        }

        public bool Delete(SemanticNodeCluster entity)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticNodeCluster> InsertBatch(IList<SemanticNodeCluster> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticNodeCluster> UpdateBatch(IList<SemanticNodeCluster> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticNodeCluster> GetAll(SemanticNodeCluster Entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class SemanticNodeClusterDSExt
    {
        public DataEntity dbCon;

        public List<string> GetAllAssociatedCluster(string iapNode, string domain)
        {
            List<string> clusters = null;
            if (!string.IsNullOrEmpty(iapNode))
            {
                using (dbCon = new DataEntity())
                {
                    clusters = new List<string>();
                    string rk = iapNode.ToLower() + ";" + domain.ToLower();
                    List<SemanticNodeCluster> nodeClusters = dbCon.SemanticNodeCluster.Where(snc => snc.RowKey.ToLower() == rk).ToList();
                    if (nodeClusters != null && nodeClusters.Count > 0)
                    {
                        nodeClusters.ForEach(nc =>
                        {
                            clusters.Add(nc.PartitionKey);
                        });
                    }
                }
            }
            return clusters;
        }
    }
}
