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

namespace Infosys.WEM.Resource.Export.DataAccess
{   
    public class ExportSourceTargetMappingDS : IDataAccess.IEntity<Infosys.WEM.Resource.Entity.ExportSourceTargetMapping>
    {
        public DataEntity dbCon;
        public bool Delete(ExportSourceTargetMapping entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportSourceTargetMapping> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<ExportSourceTargetMapping> GetAll(ExportSourceTargetMapping Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ExportSourceTargetMapping> GetAny()
        {
            return dbCon.ExportSourceTargetMappings;
        }

        public ExportSourceTargetMapping GetOne(ExportSourceTargetMapping Entity)
        {
            ExportSourceTargetMapping entity = null;

            using (dbCon = new DataEntity())
            {
                //    if (Entity.SourceScriptId > 0)
                //        entity = dbCon.ExportSourceTargetMappings.Where(e => e.TargetInstanceId == Entity.TargetInstanceId && e.TargetScriptCategoryId==Entity.TargetScriptCategoryId && e.SourceScriptId==Entity.SourceScriptId).FirstOrDefault();
                //    else
                //        entity = dbCon.ExportSourceTargetMappings.Where(e => e.TargetInstanceId == Entity.TargetInstanceId && e.TargetScriptCategoryId == Entity.TargetScriptCategoryId).FirstOrDefault();
                //}
                if (Entity.SourceScriptId > 0)
                    entity = dbCon.ExportSourceTargetMappings.Where(e => e.TargetInstanceId == Entity.TargetInstanceId && e.TargetScriptCategoryId == Entity.TargetScriptCategoryId && e.SourceScriptCategoryId==Entity.SourceScriptCategoryId && e.SourceScriptId == Entity.SourceScriptId).FirstOrDefault();
                else
                    entity = dbCon.ExportSourceTargetMappings.Where(e => e.TargetInstanceId == Entity.TargetInstanceId && e.TargetScriptCategoryId == Entity.TargetScriptCategoryId && e.SourceScriptCategoryId == Entity.SourceScriptCategoryId).FirstOrDefault();

            }
            return entity;
        }

        public ExportSourceTargetMapping Insert(ExportSourceTargetMapping entity)
        {
            using (dbCon = new DataEntity())
            {
                var targetMapping = this.GetAny();
                int? max = targetMapping.Max(c => (int?)c.id);
                int idSeed = this.GetAny().Count();
                if (max.GetValueOrDefault() > idSeed)
                    idSeed = max.GetValueOrDefault() + 1;
                else
                    idSeed++;
                entity.id = idSeed;
                dbCon.ExportSourceTargetMappings.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public IList<ExportSourceTargetMapping> InsertBatch(IList<ExportSourceTargetMapping> entities)
        {
            throw new NotImplementedException();
        }

        public ExportSourceTargetMapping Update(ExportSourceTargetMapping entity)
        {
            using (dbCon = new DataEntity())
            {
                ExportSourceTargetMapping entityItem = dbCon.ExportSourceTargetMappings.Single(c => c.id == entity.id);

                dbCon.ExportSourceTargetMappings.Attach(entityItem);
                if (entity.SourceScriptVersion > 0)
                    entityItem.SourceScriptVersion = entity.SourceScriptVersion;

                if (entity.TargetScriptVersion > 0)
                    entityItem.TargetScriptVersion = entity.TargetScriptVersion;
                if ( !string.IsNullOrEmpty( entity.ModifiedBy))
                    entityItem.ModifiedBy = entity.ModifiedBy;
                if (entity.ModifiedOn!=null)
                    entityItem.ModifiedOn = entity.ModifiedOn;

                EntityExtension<ExportSourceTargetMapping>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<ExportSourceTargetMapping> UpdateBatch(IList<ExportSourceTargetMapping> entities)
        {
            throw new NotImplementedException();
        }
    }
}
