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
    public class SemanticCategoryDS : IDataAccess.IEntity<SemanticCategory>
    {
        private DataEntity dbCon;

        public SemanticCategory GetOne(SemanticCategory Entity)
        {
            Entity.PartitionKey =
                SemanticCategoryKeysExtension.GeneratePartitionKey(Entity.CategoryId);

            using (dbCon = new DataEntity())
            {
                return dbCon.SemanticCategory.FirstOrDefault(sc => sc.PartitionKey == Entity.PartitionKey
                    && sc.RowKey == Entity.RowKey);
            }
        }

        public IQueryable<SemanticCategory> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<SemanticCategory> GetAll()
        {
            using(dbCon = new DataEntity())
                return dbCon.SemanticCategory.ToList();
        }

        public SemanticCategory Insert(SemanticCategory entity)
        {
            entity.PartitionKey = SemanticCategoryKeysExtension.GeneratePartitionKey(entity.CategoryId);

            using (dbCon = new DataEntity())
            {
                entity.CreatedDate = DateTime.UtcNow;
                dbCon.SemanticCategory.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public SemanticCategory Update(SemanticCategory entity)
        {
            entity.PartitionKey = SemanticCategoryKeysExtension.GeneratePartitionKey(entity.CategoryId);

            var semanticCategory = GetOne(entity);

            if (semanticCategory != null)
            {
                using (dbCon = new DataEntity())
                {
                    entity.LastModifiedOn = DateTime.UtcNow;
                    entity.CreatedBy = semanticCategory.CreatedBy;
                    entity.CreatedDate = semanticCategory.CreatedDate;
                    dbCon.SemanticCategory.Attach(entity);
                    var e = dbCon.Entry(entity);
                    e.Property(p => p.IsActive).IsModified = true;
                    e.Property(p => p.SemanticClusterName).IsModified = true;
                    e.Property(p => p.SemanticClusterId).IsModified = true;
                    e.Property(p => p.LastModifiedBy).IsModified = true;
                    e.Property(p => p.LastModifiedOn).IsModified = true;
                    dbCon.SaveChanges();
                }
            }
            return semanticCategory;
        }

        public bool Delete(SemanticCategory entity)
        {
            using (dbCon = new DataEntity())
            {
                var sc = dbCon.SemanticCategory.FirstOrDefault(n => n.PartitionKey == entity.PartitionKey && n.RowKey == entity.RowKey);
                
                sc.LastModifiedOn = DateTime.UtcNow;
                sc.LastModifiedBy = entity.CreatedBy;
                sc.IsActive = false;
                entity.CreatedBy = sc.CreatedBy;
                dbCon.SemanticCategory.Attach(entity);
                var e = dbCon.Entry(entity);
                e.Property(p => p.IsActive).IsModified = true;
                e.Property(p => p.LastModifiedBy).IsModified = true;
                e.Property(p => p.LastModifiedOn).IsModified = true;
                dbCon.SaveChanges();

                return true;

            }
        }

        public IList<SemanticCategory> InsertBatch(IList<SemanticCategory> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticCategory> UpdateBatch(IList<SemanticCategory> entities)
        {
            throw new NotImplementedException();
        }

        public IList<SemanticCategory> GetAll(SemanticCategory Entity)
        {
            var parKey = SemanticCategoryKeysExtension.GeneratePartitionKey(Entity.CategoryId);

            using (dbCon = new DataEntity())
            {
                return dbCon.SemanticCategory.Where(sc => sc.PartitionKey == parKey).ToList();
            }
        }
    }

    public static class SemanticCategoryKeysExtension
    {
        public static string GeneratePartitionKey(int categoryId)
        {
            return categoryId.ToString("00000");
        }
    }
}
