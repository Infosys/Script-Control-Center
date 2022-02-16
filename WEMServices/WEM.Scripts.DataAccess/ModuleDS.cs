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
using IDataAccess = Infosys.WEM.Resource.IDataAccess;

namespace Infosys.WEM.Scripts.Resource.DataAccess
{
    public class ModuleDS : IDataAccess.IEntity<Module>
    {
        public DataEntity dbCon;
        #region IEntity<Module> Members

        public Module GetOne(Module entity)
        {
            Module _Module;
            using (dbCon = new DataEntity())
            {
                _Module = dbCon.Module.Where(ent => ent.ModuleID == entity.ModuleID).FirstOrDefault();
                return _Module;
            }
        }

        public IQueryable<Module> GetAny()
        {
            return dbCon.Module;
        }

        public IList<Module> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<Module> modules =
                  (from Moduletable in dbCon.Module
                   select Moduletable).ToList<Module>();
                return modules;
            }
        }

        public Module Insert(Module entity)
        {
            using (dbCon = new DataEntity())
            {
                var categories = this.GetAny();
                int idSeed = categories.Max(c => c.ModuleID);
                //int idSeed = this.GetAny().Count();
                idSeed++;
                entity.ModuleID = idSeed;
                dbCon.Module.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public Module Update(Module entity)
        {
            using (dbCon = new DataEntity())
            {
                Module entityItem = dbCon.Module.Single(ent => ent.ModuleID == entity.ModuleID);

                dbCon.Module.Attach(entityItem);
                EntityExtension<Module>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public bool Delete(Module entity)
        {
            throw new NotImplementedException();
        }

        public IList<Module> InsertBatch(IList<Module> entities)
        {
            using (dbCon = new DataEntity())
            {
                int idSeed = this.GetAny().Count();
                foreach (Module entity in entities)
                {
                    idSeed++;
                    entity.ModuleID = idSeed;
                    dbCon.Module.Add(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }

        public IList<Module> UpdateBatch(IList<Module> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (Module entity in entities)
                {
                    Module entityItem = dbCon.Module.Single(c => c.ModuleID == entity.ModuleID);
                    dbCon.Module.Attach(entityItem);
                }
                dbCon.SaveChanges();
            }

            return entities;
        }

        public IList<Module> GetAll(Module Entity)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ModuleDSExt
    {
        public DataEntity dbCon;

        /// <summary>
        /// This method retrieves list of modules from Module table.
        /// </summary>
        /// <returns>List of modules</returns>
        public IList<Infosys.WEM.Resource.Entity.Module> GetAllModules()
        {
            using (dbCon = new DataEntity())
            {
                List<Infosys.WEM.Resource.Entity.Module> modules =
                  (from moduletable in dbCon.Module
                   select moduletable).ToList<Infosys.WEM.Resource.Entity.Module>();
                return modules;
            }
        }

    }
}
