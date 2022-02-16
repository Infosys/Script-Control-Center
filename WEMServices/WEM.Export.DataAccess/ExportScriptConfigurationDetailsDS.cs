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
    public class ExportScriptConfigurationDetailsDS : IDataAccess.IEntity<Infosys.WEM.Resource.Entity.ExportScriptConfigurationDetail>
    {
        public DataEntity dbCon;
        public bool Delete(ExportScriptConfigurationDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportScriptConfigurationDetail> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<ExportScriptConfigurationDetail> scriptConfig = (from s in dbCon.ExportScriptConfigurationDetails
                                    where s.IsDeleted == false
                                    select s).ToList<ExportScriptConfigurationDetail>();
                return scriptConfig;
            }
        }

        public IList<ExportScriptConfigurationDetail> GetAll(ExportScriptConfigurationDetail Entity)
        {
            using (dbCon = new DataEntity())
            {
                List<ExportScriptConfigurationDetail> scripts = (from s in dbCon.ExportScriptConfigurationDetails
                                    where s.ExportConfigurationId==Entity.ExportConfigurationId
                                    select s).ToList<ExportScriptConfigurationDetail>();
                return scripts;
            }
        }

        public IQueryable<ExportScriptConfigurationDetail> GetAny()
        {
            return dbCon.ExportScriptConfigurationDetails;
        }

        public ExportScriptConfigurationDetail GetOne(ExportScriptConfigurationDetail Entity)
        {
            throw new NotImplementedException();
        }

        public ExportScriptConfigurationDetail Insert(ExportScriptConfigurationDetail entity)
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
                dbCon.ExportScriptConfigurationDetails.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public IList<ExportScriptConfigurationDetail> InsertBatch(IList<ExportScriptConfigurationDetail> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (ExportScriptConfigurationDetail entity in entities)
                {
                    var configMaster = this.GetAny();
                    int? max = configMaster.Max(c => (int?)c.id);
                    int idSeed = this.GetAny().Count();
                    if (max.GetValueOrDefault() > idSeed)
                        idSeed = max.GetValueOrDefault() + 1;
                    else
                        idSeed++;
                    entity.id = idSeed;

                    if (entity.CreatedOn == null)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.ExportScriptConfigurationDetails.Add(entity);
                    dbCon.SaveChanges();
                }

                
            }


            return entities;
        }

        public ExportScriptConfigurationDetail Update(ExportScriptConfigurationDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportScriptConfigurationDetail> UpdateBatch(IList<ExportScriptConfigurationDetail> entities)
        {
            throw new NotImplementedException();
        }
    }
}
