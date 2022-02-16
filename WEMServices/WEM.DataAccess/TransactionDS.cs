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
using System.Globalization;


namespace Infosys.WEM.Resource.DataAccess
{
    public class TransactionDS : IDataAccess.IEntity<TransactionInstance>
    {
        public DataEntity dbCon;

        public TransactionInstance GetOne(TransactionInstance entity)
        {
            //Block 3_1
            DateTime processStartedTime3_1 = DateTime.Now;
            TransactionInstance req = null;
            using (dbCon = new DataEntity())
            {
                req = dbCon.TransactionInstance.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
            }
            LogHandler.LogError(string.Format("Time taken by Transaction:Block 3_1 (GetOne) : {0}", DateTime.Now.Subtract(processStartedTime3_1).TotalSeconds), LogHandler.Layer.Business, null);

            return req;
        }

        public IQueryable<TransactionInstance> GetAny()
        {
            throw new NotImplementedException();
        }

        public IList<TransactionInstance> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<TransactionInstance> GetAllMatching(TransactionInstance entity, DateTime? startdate, DateTime? enddate)
        {
            List<TransactionInstance> transactions = new List<TransactionInstance>();
            using (dbCon = new DataEntity())
            {
                if (startdate == null || enddate == null)
                    transactions = dbCon.TransactionInstance.Where(trans => trans.CategoryId == entity.CategoryId).ToList();
                else
                    transactions = dbCon.TransactionInstance.Where(trans => trans.CategoryId == entity.CategoryId && trans.CreatedOn >= startdate.Value && trans.CreatedOn <= enddate.Value).ToList();
                
                if (transactions != null)
                {
                    transactions.ForEach(ent =>
                    {                        
                        if (!string.IsNullOrEmpty(ent.WorkflowId))
                        {
                            var version=int.Parse(ent.WorkflowVersion);
                            var module = dbCon.WorkflowMaster.Where(workflow => workflow.Id == Guid.Parse(ent.WorkflowId) && workflow.CategoryId.Equals(ent.CategoryId) && workflow.WorkflowVer == version).ToList().FirstOrDefault();
                            if (module != null)
                                ent.ModuleName = module.Name;
                            else
                                transactions.Remove(ent);
                        }
                        else if (!string.IsNullOrEmpty(ent.ScriptId))
                        {
                            var scriptId = int.Parse(ent.ScriptId);
                            var version = int.Parse(ent.ScriptVersion);
                            var module = dbCon.Script.Where(script => script.ScriptId == scriptId && script.CategoryId.Equals(ent.CategoryId) && script.Version == version).ToList().FirstOrDefault();
                            if (module!=null)
                                ent.ModuleName = module.Name;
                            else
                                transactions.Remove(ent);
                        }
                    });
                }
            }            
            return transactions;
        }

        public TransactionInstance Insert(TransactionInstance entity)
        {
            //Block 3
            DateTime processStartedTime3 = DateTime.Now;

           
            //first check if it is an existing instance,
            //if so then update it otherwise add a new one
            TransactionInstance entityInDb = GetOne(entity);

            using (dbCon = new DataEntity())
            {
                //TransactionInstance entityInDb = dbCon.TransactionInstance.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
                //TransactionInstance entityInDb= dbCon.TransactionInstance.Where(s => s.PartitionKey.ToLower().Equals(entity.PartitionKey.ToLower()) && s.RowKey.ToLower().Equals(entity.RowKey.ToLower())).FirstOrDefault();
                if (entityInDb == null)
                {
                    if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.TransactionInstance.Add(entity);
                    dbCon.SaveChanges();
                }
                else
                {
                    dbCon.TransactionInstance.Attach(entityInDb);
                    if (entity.LastModifiedOn == null || entity.LastModifiedOn == DateTime.MinValue)
                    {
                        entity.LastModifiedOn = DateTime.UtcNow;
                    }
                    EntityExtension<TransactionInstance>.ApplyOnlyChanges(entityInDb, entity);
                    dbCon.SaveChanges();
                }

            }

            LogHandler.LogError(string.Format("Time taken by Transaction:Block 3 (Insert) : {0}", DateTime.Now.Subtract(processStartedTime3).TotalSeconds), LogHandler.Layer.Business, null);

            return entity;
        }

        public TransactionInstance Update(TransactionInstance entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(TransactionInstance entity)
        {
            throw new NotImplementedException();
        }

        public IList<TransactionInstance> InsertBatch(IList<TransactionInstance> entities)
        {
            throw new NotImplementedException();
        }

        public IList<TransactionInstance> UpdateBatch(IList<TransactionInstance> entities)
        {
            throw new NotImplementedException();
        }

        public IList<TransactionInstance> GetAll(TransactionInstance Entity)
        {
            throw new NotImplementedException();
        }       
    }
}
