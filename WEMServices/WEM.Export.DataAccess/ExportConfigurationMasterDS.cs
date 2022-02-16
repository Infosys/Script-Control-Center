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
    public class ExportConfigurationMasterDS : IDataAccess.IEntity<Infosys.WEM.Resource.Entity.ExportConfigurationMaster>
    {
        public DataEntity dbCon;
        public bool Delete(ExportConfigurationMaster entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportConfigurationMaster> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<ExportConfigurationMaster> masterConfig = (from s in dbCon.ExportConfigurationMasters
                                                                where s.ExportStatus == 0 || s.ExportStatus == 4
                                                                select s).ToList<ExportConfigurationMaster>();
                return masterConfig;
            }
        }

        public IList<ExportConfigurationMaster> GetAll(ExportConfigurationMaster Entity)
        {
            List<ExportConfigurationMaster> entity = null;

            using (dbCon = new DataEntity())
            {
                if (string.IsNullOrEmpty(Entity.CreatedBy))
                {
                    if (Entity.ExportStatus > 5)
                        entity = dbCon.ExportConfigurationMasters.Where(e => e.CreatedOn >= Entity.CreatedOn).ToList();
                    else
                        entity = dbCon.ExportConfigurationMasters.Where(e => e.CreatedOn >= Entity.CreatedOn && e.ExportStatus == Entity.ExportStatus).ToList();
                }
                else
                {
                    if (Entity.ExportStatus > 5)
                        entity = dbCon.ExportConfigurationMasters.Where(e =>  e.CreatedOn >= Entity.CreatedOn && e.CreatedBy.Equals(Entity.CreatedBy)).ToList();
                    else
                        entity = dbCon.ExportConfigurationMasters.Where(e => e.CreatedOn >= Entity.CreatedOn && e.CreatedBy.Equals(Entity.CreatedBy) && e.ExportStatus == Entity.ExportStatus).ToList();
                }
            }
            return entity;
        }      

        public IQueryable<ExportConfigurationMaster> GetAny()
        {
            return dbCon.ExportConfigurationMasters;
        }

        public ExportConfigurationMaster GetOne(ExportConfigurationMaster Entity)
        {
            ExportConfigurationMaster entity = null;

            using (dbCon = new DataEntity())
            {
                if (Entity == null)
                {
                    entity = (from s in dbCon.ExportConfigurationMasters
                              where s.ExportStatus == 0 || s.ExportStatus == 4
                              select s).OrderByDescending(s => s.CreatedOn).FirstOrDefault();
                }
                else
                    entity = dbCon.ExportConfigurationMasters.Where(e => e.TargetServerId == Entity.TargetServerId && (e.ExportStatus == 0 || e.ExportStatus == 4)).FirstOrDefault();
                // entity = dbCon.ExportConfigurationMasters.Where(e => e.TargetServerId == Entity.TargetServerId && e.CreatedBy.Contains(Entity.CreatedBy)).FirstOrDefault();
            }
            return entity;
        }

        public ExportConfigurationMaster Insert(ExportConfigurationMaster entity)
        {
            using (dbCon = new DataEntity())
            {
                var configMaster = this.GetAny();
                int? max = configMaster.Max(c => (int?)c.id);
                int idSeed = this.GetAny().Count();
                if (max.GetValueOrDefault() > idSeed)
                    idSeed = max.GetValueOrDefault() + 1;
                else
                    idSeed++;
                entity.id = idSeed;
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                dbCon.ExportConfigurationMasters.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public IList<ExportConfigurationMaster> InsertBatch(IList<ExportConfigurationMaster> entities)
        {
            throw new NotImplementedException();
        }

        public ExportConfigurationMaster Update(ExportConfigurationMaster entity)
        {
            using (dbCon = new DataEntity())
            {
                ExportConfigurationMaster entityItem = dbCon.ExportConfigurationMasters.Single(c => c.id == entity.id);

                dbCon.ExportConfigurationMasters.Attach(entityItem);
                entityItem.ExportStatus = entity.ExportStatus;

                if (entity.CompletedOn != null)
                    entityItem.CompletedOn = entity.CompletedOn;

                if (entity.IsDeleted != null)
                    entityItem.IsDeleted = entity.IsDeleted;

                entityItem.ModifiedBy = entity.ModifiedBy;
                entityItem.ModifiedOn = entity.ModifiedOn;

                EntityExtension<ExportConfigurationMaster>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<ExportConfigurationMaster> UpdateBatch(IList<ExportConfigurationMaster> entities)
        {
            throw new NotImplementedException();
        }
    }
}
