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
using System.Data.Entity.Validation;

namespace Infosys.WEM.Resource.Export.DataAccess
{
    public class ExportTransactionDetailsDS : IDataAccess.IEntity<Infosys.WEM.Resource.Entity.ExportTransactionDetail>
    {
        public DataEntity dbCon;
        public bool Delete(ExportTransactionDetail entity)
        {
            throw new NotImplementedException();
        }

        public IList<ExportTransactionDetail> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<ExportTransactionDetail> transDetails = (from t in dbCon.ExportTransactionDetails
                                                              where t.IsDeleted == false && t.Status == 0
                                                              select t).ToList<ExportTransactionDetail>();
                return transDetails;
            }
        }

        public IList<ExportTransactionDetail> GetAll(ExportTransactionDetail Entity)
        {
            List<ExportTransactionDetail> transDetails = null;
            using (dbCon = new DataEntity())
            {
                if (string.IsNullOrEmpty(Entity.CreatedBy))
                {
                    //transDetails = (from t in dbCon.ExportTransactionDetails
                    //                where t.ExportScriptConfigurationId == Entity.ExportScriptConfigurationId && t.Status==Entity.Status
                    //                select t).ToList<ExportTransactionDetail>();
                    transDetails = (from t in dbCon.ExportTransactionDetails
                                    where t.ExportScriptConfigurationId == Entity.ExportScriptConfigurationId
                                    select t).ToList<ExportTransactionDetail>();
                }
                else
                {
                    //transDetails = (from s in dbCon.ExportTransactionDetails
                    //                where s.ExportScriptConfigurationId == Entity.ExportScriptConfigurationId && s.Status == Entity.Status && s.CreatedBy.Equals(Entity.CreatedBy)
                    //                select s).ToList<ExportTransactionDetail>();
                    transDetails = (from s in dbCon.ExportTransactionDetails
                                    where s.ExportScriptConfigurationId == Entity.ExportScriptConfigurationId && s.CreatedBy.Equals(Entity.CreatedBy)
                                    select s).ToList<ExportTransactionDetail>();
                }
                return transDetails;
            }
        }

        public IQueryable<ExportTransactionDetail> GetAny()
        {
            return dbCon.ExportTransactionDetails;
        }

        public ExportTransactionDetail GetOne(ExportTransactionDetail Entity)
        {
            throw new NotImplementedException();
        }

        public ExportTransactionDetail Insert(ExportTransactionDetail entity)
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
                dbCon.ExportTransactionDetails.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public IList<ExportTransactionDetail> InsertBatch(IList<ExportTransactionDetail> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (ExportTransactionDetail entity in entities)
                {
                    bool isDuplicate = false;
                    var found = (from t in dbCon.ExportTransactionDetails
                                 where t.ExportScriptConfigurationId == entity.ExportScriptConfigurationId &&
                                 t.SourceCategoryId == entity.SourceCategoryId &&
                                 t.SourceScriptId == entity.SourceScriptId
                                 //t.TargetCategoryId == transaction.TargetCategoryId
                                 select t).ToList<ExportTransactionDetail>();
                    if (found != null && found.Count > 0)
                        isDuplicate = true;

                    if (isDuplicate == false)
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
                        dbCon.ExportTransactionDetails.Add(entity);
                        //try
                        //{
                            dbCon.SaveChanges();
                        //}
                       

                    }
                }
            }
            return entities;
        }

        public ExportTransactionDetail Update(ExportTransactionDetail entity)
        {
            using (dbCon = new DataEntity())
            {
                ExportTransactionDetail entityItem = dbCon.ExportTransactionDetails.FirstOrDefault(c => c.id == entity.id);

                dbCon.ExportTransactionDetails.Attach(entityItem);

                entityItem.Status = entity.Status;
                if (entity.Action != null)
                    entityItem.Action = entity.Action;
                if (!string.IsNullOrEmpty(entity.Details))
                    entityItem.Details = entity.Details;
                if (entity.ExistReasonCode != null)
                    entityItem.ExistReasonCode = entity.ExistReasonCode;
                entityItem.ModifiedOn = entity.ModifiedOn;
                entityItem.ModifiedBy = entity.ModifiedBy;

                if (!string.IsNullOrEmpty(entity.TargetScriptName))
                    entityItem.TargetScriptName = entity.TargetScriptName;
                if (!string.IsNullOrEmpty(entity.TargetScriptPath))
                    entityItem.TargetScriptPath = entity.TargetScriptPath;

                EntityExtension<ExportTransactionDetail>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<ExportTransactionDetail> UpdateBatch(IList<ExportTransactionDetail> entities)
        {
            throw new NotImplementedException();
        }

        public bool IsDuplicateTransaction(ExportTransactionDetail transaction)
        {
            bool isDuplicate = false;
            using (dbCon = new DataEntity())
            {
                var found = (from t in dbCon.ExportTransactionDetails
                             where t.ExportScriptConfigurationId == transaction.ExportScriptConfigurationId &&
                             t.SourceCategoryId == transaction.SourceCategoryId &&
                             t.SourceScriptId == transaction.SourceScriptId
                             //t.TargetCategoryId == transaction.TargetCategoryId
                             select t).ToList<ExportTransactionDetail>();
                if (found != null && found.Count > 0)
                    isDuplicate = true;
            }
            return isDuplicate;
        }
    }
}
