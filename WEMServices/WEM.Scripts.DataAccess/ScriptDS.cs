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

namespace Infosys.WEM.Scripts.Resource.DataAccess
{

    /// <summary>
    /// /// <summary>
    /// Class definition to define the CRUD operation on Script data entity class
    /// </summary>
    public partial class ScriptDS : IDataAccess.IEntity<Script>
    {
        public DataEntity dbCon;       

        /// <summary>
        /// This method is to get one specific record from table
        /// </summary>
        /// <param name="entity">object of Script</param>
        /// <returns>object of Script</returns>
        public Script GetOne(Script entity)
        {
            Script _Script = null;
            if (entity.Version == 0)
                _Script = ScriptDSExt.GetLatestVersion(entity);
            else
            {
                using (dbCon = new DataEntity())
                {
                    Script tempScript = ScriptDSExt.GenerateRowKey(entity);
                    _Script = dbCon.Script.Where(s => s.PartitionKey.Equals(entity.PartitionKey) && s.RowKey.Equals(tempScript.RowKey)).FirstOrDefault();
                }
            }
            return _Script;
        }
        /// <summary>
        /// This method is to get all records from table
        /// </summary>
        /// <param >none</param>
        /// <returns>List of Script</returns>
        public IList<Script> GetAll()
        {
            using (dbCon = new DataEntity())
            {

                List<Script> scripts =
                  (from scripttable in dbCon.Script
                   where scripttable.IsDeleted == false
                   select scripttable).ToList<Script>();
                return scripts;
            }
        }
        /// <summary>
        /// This method is to insert one record in table
        /// </summary>
        /// <param name="entity">object of Script</param>
        /// <returns>bool</returns>
        public Script Insert(Script entity)
        {
            using (dbCon = new DataEntity())
            {
                //reset modified by, if set during version update
                entity.ModifiedBy = null;

                if (entity.ScriptId == 0)
                {
                    int idSeed = this.GetAny().Select(x => x.ScriptId).Distinct().Count();
                    idSeed++;
                    entity.ScriptId = idSeed;
                }
                entity.Version = ScriptDSExt.GetNextVersionNumber(entity);
                entity = ScriptDSExt.GenerateRowKey(entity);

                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    entity.CreatedOn = DateTime.UtcNow;
                if (entity.Timestamp == null || entity.Timestamp == DateTime.MinValue)
                    entity.Timestamp = DateTime.UtcNow;

                dbCon.Script.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to update one record in table
        /// </summary>
        /// <param name="entity">object of Script</param>
        /// <returns>bool</returns>
        public Script Update(Script entity)
        {
            Script entityItem = GetOne(entity);
            using (dbCon = new DataEntity())
            {
                //Script entityItem = dbCon.Script.Single(c => c.PartitionKey == entity.PartitionKey &&
                //    c.RowKey == entity.RowKey);                

                dbCon.Script.Attach(entityItem);

                DateTime? lastModifiedOn = null;

                lastModifiedOn = entityItem.ModifiedOn;

                if (entity.ModifiedOn == null || entity.ModifiedOn == DateTime.MinValue)
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }
                if (lastModifiedOn == entity.ModifiedOn)
                {
                    entity.ModifiedOn = DateTime.UtcNow;
                }

                //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                EntityExtension<Script>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }
        /// <summary>
        /// This method is to delete one record from table
        /// </summary>
        /// <param name="entity">Object of Script</param>
        /// <returns>bool</returns>
        public bool Delete(Script entity)
        {
            bool result = false;
            using (dbCon = new DataEntity())
            {
                dbCon.Script.Attach(entity);
                dbCon.Script.Remove(entity);
                dbCon.SaveChanges();
            }
            result = true;
            return result;
        }
        /// <summary>
        /// This method is to insert multiple records in table
        /// </summary>
        /// <param name="entities">List of Script entities</param>
        /// <returns>bool</returns>
        public IList<Script> InsertBatch(IList<Script> entities)
        {

            using (dbCon = new DataEntity())
            {
                int idSeed = this.GetAny().Count();
                foreach (Script entity in entities)
                {
                    if (entity.ScriptId == 0)
                    {
                        idSeed++;
                        entity.ScriptId = idSeed;
                    }
                    entity.Version = ScriptDSExt.GetNextVersionNumber(entity);
                    entity.RowKey = ScriptDSExt.GenerateRowKey(entity).RowKey;
                    if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.Script.Add(entity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to update multiple records in table
        /// </summary>
        /// <param name="entities">List of Script entities</param>
        /// <returns>bool</returns>
        public IList<Script> UpdateBatch(IList<Script> entities)
        {

            using (dbCon = new DataEntity())
            {
                foreach (Script entity in entities)
                {
                   // Script entityItem = dbCon.Script.Single(c => c.PartitionKey == entity.PartitionKey &&
                   //c.RowKey == entity.RowKey);
                    Script entityItem = GetOne(entity);

                    dbCon.Script.Attach(entityItem);

                    DateTime? lastModifiedOn = null;

                    lastModifiedOn = entityItem.ModifiedOn;

                    if (entity.ModifiedOn == null || entity.ModifiedOn == DateTime.MinValue)
                    {
                        entity.ModifiedOn = DateTime.UtcNow;
                    }
                    if (lastModifiedOn == entity.ModifiedOn)
                    {
                        entity.ModifiedOn = DateTime.UtcNow;
                    }

                    EntityExtension<Script>.ApplyOnlyChanges(entityItem, entity);
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        /// <summary>
        /// This method is to get an entity
        /// </summary>
        /// <returns>IQueryable<Script></returns>
        public IQueryable<Script> GetAny()
        {
            return dbCon.Script;
        }

        /// <summary>
        /// This method is to get multiple records from table 
        /// based on partitionkey 
        /// </summary>
        /// <param name="entity">Script entity having the partitonkey value</param>
        /// <returns>List of Script</returns>
        public IList<Script> GetAll(Script entity)
        {
            using (dbCon = new DataEntity())
            {
                List<Script> scripts = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                        scripts = dbCon.Script.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && !ent.IsDeleted).ToList<Script>();   

                        scripts.ForEach(script => 
                    {
                        Script _Script = dbCon.Script.Where(ent => ent.PartitionKey.Equals(script.PartitionKey) && ent.ScriptId.Equals(script.ScriptId))
                                                    .OrderBy(ent => ent.CreatedOn).FirstOrDefault<Script>();

                        script.CreatedBy = _Script.CreatedBy;
                        script.CreatedOn = _Script.CreatedOn;                       
                    });                   

                    //scripts = dbCon.Script.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey)).ToList<Script>();
                }
                return scripts;
            }
        }    

    }

    public static class ScriptDSExt
    { 
        public static Script GenerateRowKey(Script entity)
        {
            entity.RowKey = entity.ScriptId.ToString("00000") + "_" + entity.Version;
            return entity;
        }

        public static Script GetLatestVersion(Script entity)
        {
            using (DataEntity dbCon = new DataEntity())
            {
                //Script tempScript = dbCon.Script.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.ScriptId.Equals(entity.ScriptId) && !ent.IsDeleted).FirstOrDefault();
                Script tempScript = dbCon.Script.Where(ent =>  ent.ScriptId.Equals(entity.ScriptId) && !ent.IsDeleted).FirstOrDefault();
                return tempScript;
            }
        }

        public static int GetNextVersionNumber(Script entity)
        {
            int ver = 1;
            using (DataEntity dbCon = new DataEntity())
            {
                //Script tempScript = dbCon.Script.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.ScriptId.Equals(entity.ScriptId) && !ent.IsDeleted).FirstOrDefault();
                //if (tempScript != null)
                //    ver = tempScript.Version + 1;
                string rowKey = entity.ScriptId.ToString("00000") + "_";
                int scriptCount = dbCon.Script.Where(ent => ent.RowKey.Contains(rowKey)).Count();
                ver = scriptCount + 1;

            }
            return ver;
        }

        public static bool IsDuplicate(Script entity)
        {
            bool isDuplicate = false;
            using (DataEntity dbCon = new DataEntity())
            {
                List<Script> scripts = (from scriptable in dbCon.Script
                                       where scriptable.CategoryId==entity.CategoryId
                                       && scriptable.IsDeleted == false
                                       select scriptable).ToList();
                foreach (Script item in scripts)
                {
                    if (item.Name.ToLower() == entity.Name.ToLower())
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }

            return isDuplicate;
        }

        //public static List<string> GenerateParitionKey(List<int> catList)
        //{
        //    List<string> partitionKeyList = new List<string>();
        //    foreach(var categoryId in catList)
        //    {
        //        partitionKeyList.Add(categoryId.ToString("00000"));
        //    }
        //    return partitionKeyList;
        //}
    }

    public class ScriptDSScriptExt
    {
        List<int> childCatList;
        public IList<Script> GetAllScript(Script entity, Boolean IncludeSubCategoryScripts)
        {
            using (DataEntity dbCon = new DataEntity())
            {
                List<Script> scripts = null;
                if ((!string.IsNullOrEmpty(entity.PartitionKey) && string.IsNullOrEmpty(entity.RowKey)))
                {
                    if (IncludeSubCategoryScripts)
                    {
                        childCatList = new List<int>();
                        childCatList.Add(entity.CategoryId);
                        var catList = GetAllChildCategories(entity.CategoryId);
                        //var partitionKeys = ScriptDSExt.GenerateParitionKey(catList);
                        scripts = dbCon.Script.Where(ent => catList.Contains(ent.CategoryId) && !ent.IsDeleted).ToList<Script>();
                    }
                    else
                        scripts = dbCon.Script.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && !ent.IsDeleted).ToList<Script>();

                    scripts.ForEach(script =>
                    {
                        Script _Script = dbCon.Script.Where(ent => ent.PartitionKey.Equals(script.PartitionKey) && ent.ScriptId.Equals(script.ScriptId))
                                                    .OrderBy(ent => ent.CreatedOn).FirstOrDefault<Script>();

                        script.CreatedBy = _Script.CreatedBy;
                        script.CreatedOn = _Script.CreatedOn;
                    });

                    //scripts = dbCon.Script.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey)).ToList<Script>();
                }
                return scripts;
            }
        }

        public List<int> GetAllChildCategories(int categoryId)
        {
            DataEntity dbCon = new DataEntity();
            var resp = dbCon.Category.Where(c => c.ParentId == categoryId && c.IsDeleted == false).ToList();
            if (resp != null && resp.Count > 0)
            {
                resp.ForEach(cat =>
                {
                    childCatList.Add(cat.CategoryId);
                    GetAllChildCategories(cat.CategoryId);
                });
            }
            return childCatList;
        }

    }
}

