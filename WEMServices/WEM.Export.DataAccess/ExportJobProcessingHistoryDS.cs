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
    public class ExportJobProcessingHistoryDS : IDataAccess.IEntity<Infosys.WEM.Resource.Entity.ExportJobProcessingHistory>
    {
        public DataEntity dbCon;
        public bool Delete(ExportJobProcessingHistory entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportJobProcessingHistory> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<ExportJobProcessingHistory> GetAll(ExportJobProcessingHistory Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ExportJobProcessingHistory> GetAny()
        {
            return dbCon.ExportJobProcessingHistories;
        }

        public ExportJobProcessingHistory GetOne(ExportJobProcessingHistory Entity)
        {
            ExportJobProcessingHistory entity = null;

            using (dbCon = new DataEntity())
            {
                entity = dbCon.ExportJobProcessingHistories.Where(e => e.JobId == Entity.JobId).FirstOrDefault();
            }
            return entity;
        }

        public ExportJobProcessingHistory Insert(ExportJobProcessingHistory entity)
        {
            using (dbCon = new DataEntity())
            {
                var jobHistory = this.GetAny();
                int? max = jobHistory.Max(c => (int?)c.JobId);
                int idSeed = this.GetAny().Count();
                if (max.GetValueOrDefault() > idSeed)
                    idSeed = max.GetValueOrDefault() + 1;
                else
                    idSeed++;
                entity.JobId = idSeed;
                dbCon.ExportJobProcessingHistories.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public IList<ExportJobProcessingHistory> InsertBatch(IList<ExportJobProcessingHistory> entities)
        {
            throw new NotImplementedException();
        }

        public ExportJobProcessingHistory Update(ExportJobProcessingHistory entity)
        {
            ExportJobProcessingHistory entityItem = GetOne(entity);
            using (dbCon = new DataEntity())
            {
                dbCon.ExportJobProcessingHistories.Attach(entityItem);
                if (entity.CompletedOn!=null)
                    entityItem.CompletedOn = entity.CompletedOn;
                EntityExtension<ExportJobProcessingHistory>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<ExportJobProcessingHistory> UpdateBatch(IList<ExportJobProcessingHistory> entities)
        {
            throw new NotImplementedException();
        }
    }
}
