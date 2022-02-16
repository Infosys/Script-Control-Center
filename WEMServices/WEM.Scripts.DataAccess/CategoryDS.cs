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

using System.Reflection;
using Infosys.WEM.Resource.Entity;
using Infosys.WEM.Infrastructure.Common;
using IDataAccess = Infosys.WEM.Resource.IDataAccess;


namespace Infosys.WEM.Scripts.Resource.DataAccess
{
    public class CategoryDS : IDataAccess.IEntity<Category>
    {
        public DataEntity dbCon;
        #region IEntity<Category> Members

        public Category GetOne(Category entity)
        {
            Category _Category;
            using (dbCon = new DataEntity())
            {
                _Category = dbCon.Category.Where(ent => ent.PartitionKey.Equals(entity.PartitionKey) && ent.RowKey.Equals(entity.RowKey)).FirstOrDefault();
                return _Category;
            }
        }

        public IQueryable<Category> GetAny()
        {
            return dbCon.Category;
        }

        public IList<Category> GetAll()
        {
            using (dbCon = new DataEntity())
            {
                List<Category> categories =
                  (from categorytable in dbCon.Category
                   where categorytable.IsDeleted == false
                   select categorytable).ToList<Category>();
                return categories;
            }
        }

        public Category Insert(Category entity)
        {
            using (dbCon = new DataEntity())
            {
                var categories = this.GetAny();
                int? max = categories.Max(c => (int?)c.CategoryId);
                int idSeed = this.GetAny().Count();
                if (max.GetValueOrDefault() > idSeed)
                    idSeed = max.GetValueOrDefault() + 1;
                else
                    idSeed++;
                entity.RowKey = CategoryKeysExtension.GenerateRowKey(idSeed);
                entity.CategoryId = idSeed;
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                    entity.Timestamp = DateTime.UtcNow;
                }
                dbCon.Category.Add(entity);
                dbCon.SaveChanges();
            }
            return entity;
        }

        public Category Update(Category entity)
        {
            using (dbCon = new DataEntity())
            {
                Category entityItem = dbCon.Category.Single(c => c.PartitionKey == entity.PartitionKey &&
                   c.RowKey == entity.RowKey);

                dbCon.Category.Attach(entityItem);

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
                EntityExtension<Category>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public bool Delete(Category entity)
        {
            throw new NotImplementedException();
        }

        public IList<Category> InsertBatch(IList<Category> entities)
        {
            using (dbCon = new DataEntity())
            {
                int idSeed = this.GetAny().Count();
                foreach (Category entity in entities)
                {
                    idSeed++;
                    entity.RowKey = CategoryKeysExtension.GenerateRowKey(idSeed);
                    entity.CategoryId = idSeed;
                    if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    {
                        entity.CreatedOn = DateTime.UtcNow;
                    }
                    dbCon.Category.Add(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }

        public IList<Category> UpdateBatch(IList<Category> entities)
        {
            using (dbCon = new DataEntity())
            {
                foreach (Category entity in entities)
                {
                    entity.PartitionKey = CategoryKeysExtension.GeneratePartitionKey(entity.CategoryId, entity.CompanyId.GetValueOrDefault());
                    entity.RowKey = CategoryKeysExtension.GenerateRowKey(entity.CategoryId);

                    Category entityItem = dbCon.Category.Single(c => c.RowKey == entity.RowKey);

                    dbCon.Category.Attach(entityItem);

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

                    entityItem.ModifiedOn = entity.ModifiedOn;
                    entityItem.IsDeleted = entity.IsDeleted;

                    var ei = dbCon.Entry(entityItem);
                    ei.Property(e => e.IsDeleted).IsModified = true;
                    ei.Property(e => e.ModifiedOn).IsModified = true;

                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }


            return entities;
        }

        public IList<Category> GetAll(Category Entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public static class CategoryKeysExtension
    {
        public static string GenerateRowKey(int index)
        {
            return index.ToString("00000");
        }

        public static string GeneratePartitionKey(int index, int companyId)
        {
            return companyId.ToString() + "_" + index.ToString("00000");
        }
    }

    public class CategoryDSExt
    {
        public DataEntity dbCon;
        List<Category> childCatList = new List<Category>();
        /// <summary>
        /// Extension to return only the root categories and not the sub-categories from the Category table
        /// </summary>
        /// <returns>list of root categories</returns>
        public IList<Category> GetAllCategories()
        {
            using (dbCon = new DataEntity())
            {
                List<Category> categories =
                 dbCon.Category.Where(c => c.IsDeleted == false).ToList();
                return categories;
            }
        }

        public IList<Category> GetAllCategories(string companyId, string moduleId, out Dictionary<int, int> numberOfScripts, string categoryId = "")
        {
            List<Category> categories = null;
            int id = 0;
            using (dbCon = new DataEntity())
            {
                if (string.IsNullOrEmpty(moduleId))
                {
                    categories = dbCon.Category.Where(c => c.IsDeleted == false &&
                                      (c.PartitionKey.Contains(companyId + "_") || c.PartitionKey.Contains("0_"))).ToList();
                 
                    numberOfScripts = dbCon.Script.Join(dbCon.Category, ent => ent.CategoryId, cat => cat.CategoryId, (ent, cat) => new { ent, cat })
                                                  .Where(m => m.ent.IsDeleted == false && (m.cat.IsDeleted == false &&
                                                                (m.cat.PartitionKey.Contains(companyId + "_") || m.cat.PartitionKey.Contains("0_"))))
                                                  .GroupBy(m => m.ent.CategoryId)
                                                  .Select(g => new { Key = g.Key, Count = g.Count() }).Distinct().ToDictionary(item => item.Key, item => item.Count);
                }
                else
                {
                    int.TryParse(moduleId, out id);
                    categories = dbCon.Category.Where(c => c.IsDeleted == false && (c.ModuleID == id || c.ModuleID == 1) &&
                                                          (c.PartitionKey.Contains(companyId + "_") || c.PartitionKey.Contains("0_"))).ToList();

                    numberOfScripts = dbCon.Script.Join(dbCon.Category, ent => ent.CategoryId, cat => cat.CategoryId, (ent, cat) => new { ent, cat })
                                                  .Where(m => m.ent.IsDeleted == false && (m.cat.IsDeleted == false && (m.cat.ModuleID == id || m.cat.ModuleID == 1) &&
                                                                (m.cat.PartitionKey.Contains(companyId + "_") || m.cat.PartitionKey.Contains("0_"))))
                                                  .GroupBy(m => m.ent.CategoryId)
                                                  .Select(g => new { Key = g.Key, Count = g.Count() }).Distinct().ToDictionary(item => item.Key, item => item.Count);
                }

                if (!string.IsNullOrEmpty(categoryId))
                {
                    categories = GetAllChildCategories(categories, int.Parse(categoryId));
                }

                return categories;
            }
        }

        /// <summary>
        /// Extension to return the sub categories under the intended category
        /// </summary>
        /// <param name="categoryId">parent category</param>
        /// <returns>the list of sub categories under the intended parent category</returns>
        public IList<Category> GetAllSubCategories(string categoryId)
        {
            int catid = 0;
            int.TryParse(categoryId, out catid);
            categoryId = catid.ToString("00000");
            using (dbCon = new DataEntity())
            {
                List<Category> categories =
                  (from categorytable in dbCon.Category
                   where categorytable.IsDeleted == false && categorytable.PartitionKey == categoryId
                   select categorytable).ToList<Category>();
                return categories;
            }
        }

        /// <summary>
        /// Extension to check if the name of the category or subcategory is same as any existing one.
        /// No two category or subcategory can have same names.
        /// </summary>
        /// <param name="name">The name of the intended category or subcategory</param>
        /// <returns>true if duplicate</returns>
        public bool IsDuplicate(string name)
        {
            bool isDuplicate = false;
            using (dbCon = new DataEntity())
            {
                List<Category> categories =
                  (from categorytable in dbCon.Category
                   where categorytable.IsDeleted == false
                   select categorytable).ToList<Category>();
                foreach (Category category in categories)
                {
                    if (category.Name.ToLower() == name.ToLower())
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }

            return isDuplicate;
        }

        /// <summary>
        /// Extension to check if the name of any of the categories or subcategories provided is same as any existing one.
        /// No two category or subcategory can have same names.
        /// </summary>
        /// <param name="categories">the list of new categories/subcategories</param>
        /// <returns>true if any duplication</returns>
        public bool IsAnyDuplicate(List<Category> categories)
        {
            bool isDuplicate = false;
            using (dbCon = new DataEntity())
            {
                List<Category> presentCategories =
                  (from categorytable in dbCon.Category
                   where categorytable.IsDeleted == false
                   select categorytable).ToList<Category>();
                foreach (Category category in categories)
                {
                    if (!isDuplicate)
                    {
                        foreach (Category presentcategory in presentCategories)
                        {
                            if (category.Name.ToLower() == presentcategory.Name.ToLower())
                            {
                                isDuplicate = true;
                                break;
                            }
                        }
                    }
                    else
                        break;
                }
            }
            return isDuplicate;
        }

        public bool IsDuplicateCategory(Category category)
        {
            bool isDuplicate = false;
            using (dbCon = new DataEntity())
            {
                List<Category> presentCategories =
                    (from categorytable in dbCon.Category
                     where categorytable.PartitionKey == "1_ROOT" && categorytable.IsDeleted == false
                     select categorytable).ToList();
                foreach (Category presentcategory in presentCategories)
                {
                    if (category.Name.ToLower() == presentcategory.Name.ToLower())
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }
            return isDuplicate;
        }

        public bool IsDuplicateSubCategory(Category subcategory)
        {
            bool isDuplicate = false;
            using (dbCon = new DataEntity())
            {
                List<Category> presentSubcategories =
                    (from categorytable in dbCon.Category
                     where categorytable.PartitionKey == subcategory.PartitionKey && categorytable.IsDeleted == false
                     select categorytable).ToList();
                foreach (Category presentsubcategory in presentSubcategories)
                {
                    if (subcategory.Name.ToLower() == presentsubcategory.Name.ToLower())
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }
            return isDuplicate;
        }

        public List<Category> GetAllChildCategories(List<Category> categories, int categoryId)
        {        
            var resp = categories.Where(c => c.ParentId == categoryId).ToList();
            if (resp != null && resp.Count > 0)
            {
                resp.ForEach(cat =>
                {
                    childCatList.Add(cat);
                    GetAllChildCategories(categories, cat.CategoryId);
                });
            }
            return childCatList;
        }

        //public bool InsertBatch(List<Category> categories)
        //{
        //    bool isDuplicate = false;
        //    foreach (Category category in categories)
        //    {
        //        if (category.PartitionKey == "ROOT")
        //        {
        //            isDuplicate = IsDuplicateCategory(category);
        //            if (isDuplicate)
        //                break;

        //        }
        //    }
        //    return isDuplicate;
        //}
    }
}
