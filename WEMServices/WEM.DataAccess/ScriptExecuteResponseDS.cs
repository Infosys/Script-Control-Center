/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Resource.DataAccess
{
    public class ScriptExecuteResponseDS : IDataAccess.IEntity<ScriptExecuteResponse>
    {
        public DataEntity dbCon;

        #region IEntity<ScriptExecuteResponse> Members

        public IList<ScriptExecuteResponse> GetAll()
        {
            using (dbCon = new DataEntity())
                return dbCon.ScriptExecuteResponse.ToList();
        }        

        public ScriptExecuteResponse GetOne(ScriptExecuteResponse Entity)
        {
            using (dbCon = new DataEntity())
            {
                return dbCon.ScriptExecuteResponse.FirstOrDefault(sc => sc.transactionId == Entity.transactionId);
            }
        }

        public ScriptExecuteResponse Insert(ScriptExecuteResponse entity)
        {
            using (dbCon = new DataEntity())
            {
                if (entity.createddate == null || entity.createddate == DateTime.MinValue)
                {
                    entity.createddate = DateTime.UtcNow;
                }

                //entity.Priority to be used in future to set the order or execution

                dbCon.ScriptExecuteResponse.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public List<ScriptExecuteResponse> InsertBatch(List<ScriptExecuteResponse> entities)
        {
            using (dbCon = new DataEntity())
            {                
                foreach (ScriptExecuteResponse entity in entities)
                {                                        
                    if (entity.createddate == null || entity.createddate == DateTime.MinValue)
                    {
                        entity.createddate = DateTime.UtcNow;
                    }
                    dbCon.ScriptExecuteResponse.Add(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }

        public ScriptExecuteResponse Update(ScriptExecuteResponse entity)
        {
            using (dbCon = new DataEntity())
            {
                ScriptExecuteResponse entityItem=new ScriptExecuteResponse();
                if (string.IsNullOrEmpty(entity.computername))
                {
                     entityItem = dbCon.ScriptExecuteResponse.Single(c => c.transactionId == entity.transactionId);
                }
                else
                    entityItem = dbCon.ScriptExecuteResponse.Single(c => c.transactionId == entity.transactionId&&c.computername==entity.computername);

                dbCon.ScriptExecuteResponse.Attach(entityItem);

                if (entity.modifieddate == null || entity.modifieddate == DateTime.MinValue)
                {
                    entity.modifieddate = DateTime.UtcNow;
                }
                entity.scriptexecuteresponseid = entityItem.scriptexecuteresponseid;
                //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                EntityExtension<ScriptExecuteResponse>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public List<ScriptExecuteResponse> UpdateBatch(List<ScriptExecuteResponse> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (ScriptExecuteResponse entity in entities)
                {
                    ScriptExecuteResponse entityItem = dbCon.ScriptExecuteResponse.Single(c => c.transactionId == entity.transactionId &&
                   c.computername == entity.computername);

                    dbCon.ScriptExecuteResponse.Attach(entityItem);

                    if (entity.modifieddate == null || entity.modifieddate == DateTime.MinValue)
                    {
                        entity.modifieddate = DateTime.UtcNow;
                    }                    
                    EntityExtension<ScriptExecuteResponse>.ApplyOnlyChanges(entityItem, entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }

        public IList<ScriptExecuteResponse> GetAll(ScriptExecuteResponse Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ScriptExecuteResponse> GetAny()
        {
            throw new NotImplementedException();
        }
        public bool Delete(ScriptExecuteResponse entity)
        {
            throw new NotImplementedException();
        }        

        public IList<ScriptExecuteResponse> UpdateBatch(IList<ScriptExecuteResponse> entities)
        {
            throw new NotImplementedException();
        }

        public IList<ScriptExecuteResponse> InsertBatch(IList<ScriptExecuteResponse> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ScriptExecuteResponseDSExt
    {
        public DataEntity dbCon;

        public ScriptExecuteResponse GetOne(ScriptExecuteResponse Entity)
        {
           
            using (dbCon = new DataEntity())
            {
               
               return dbCon.ScriptExecuteResponse.FirstOrDefault(sc => sc.transactionId == Entity.transactionId && sc.computername == Entity.computername);
               
            }

           
        }

      
    }
}
