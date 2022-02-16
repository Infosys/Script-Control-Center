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
using Infosys.WEM.Infrastructure.Common;
using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators.Scripts
{
    public class CategorySE_DE
    {
        public static DE.Category CategorySEtoDE(SE.Category categorySE)
        {
            DE.Category categoryDE = null;
            if (categorySE != null)
            {
                categoryDE = new DE.Category();
                //categoryDE.CategoryId = categorySE.CategoryId; //not neded as it is identity column
              //  if (!string.IsNullOrEmpty(categorySE.CreatedBy))
                categoryDE.CreatedBy = Utility.GetLoggedInUser();
               // if (!string.IsNullOrEmpty(categorySE.ModifiedBy))
                categoryDE.ModifiedBy = Utility.GetLoggedInUser();
                categoryDE.Description = categorySE.Description;
                categoryDE.Name = categorySE.Name;
                categoryDE.PartitionKey = categorySE.ParentCategoryId == 0 ? "ROOT" : categorySE.ParentCategoryId.ToString("00000");
                categoryDE.IsDeleted = categorySE.IsDeleted;
                if (categorySE.CategoryId > 0)
                {
                    //assign the row key- to be used during update/delete
                    categoryDE.RowKey = categorySE.CategoryId.ToString("00000"); //e.g. to convert 1 to 00001, 10 to 00010
                }
                categoryDE.ParentId = categorySE.ParentId;
            }
            return categoryDE;
        }

        public static SE.Category CategoryDEtoSE(DE.Category categoryDE)
        {
            SE.Category categorySE = null;
            if (categoryDE != null)
            {
                categorySE = new SE.Category();
                if (categoryDE.PartitionKey.Contains("_"))
                {
                    int index = categoryDE.PartitionKey.IndexOf("_");
                    if (index > 0)
                        categoryDE.PartitionKey = categoryDE.PartitionKey.Substring(index+1);
                }
                categorySE.CategoryId = categoryDE.CategoryId;
                categorySE.Description = categoryDE.Description;
                categorySE.Name = categoryDE.Name;
                categorySE.ParentCategoryId = categoryDE.PartitionKey.ToLower() == "root" ? 0 : int.Parse(categoryDE.PartitionKey);
                categorySE.ParentId = categoryDE.ParentId;
            }

            return categorySE;
        }

        public static List<SE.Category> CategoryDEListtoSEList(List<DE.Category> categoryDEList)
        {
            List<SE.Category> categorySEList = null;
            if (categoryDEList != null)
            {
                categorySEList = new List<SE.Category>();
                categoryDEList.ForEach(de =>
                {
                    categorySEList.Add(CategoryDEtoSE(de));
                });
            }
            return categorySEList;
        }

        public static List<DE.Category> CategorySEListtoDEList(List<SE.Category> categorySEList)
        {
            List<DE.Category> categoryDEList = null;
            if (categorySEList != null)
            {
                categoryDEList = new List<DE.Category>();
                categorySEList.ForEach(se =>
                {
                    categoryDEList.Add(CategorySEtoDE(se));
                });
            }
            return categoryDEList;
        }

        public static List<SE.Category> CategoryDEListtoSETree(List<DE.Category> categoryDEList)
        {
            List<SE.Category> seCat = null;
            if (categoryDEList != null)
            {
                seCat = CategoryDEListtoSEList(categoryDEList);
            }
            return seCat;
        }

        public static List<SE.CategoryTree> CategoryDEListtoSETreeList(List<DE.Category> categoryDEList)
        {
            List<SE.CategoryTree> categoryTreeList = null;
            if (categoryDEList != null)
            {
                List<SE.Category> seCat = CategoryDEListtoSEList(categoryDEList);
                categoryTreeList = new List<SE.CategoryTree>();
                //first accumulate the root categories
                seCat.Where(c => c.ParentCategoryId == 0).ToList().ForEach(ca =>
                {
                    categoryTreeList.Add(CategorySEtoSETree(ca));
                });

                //then map the sub categories to parent categories
                seCat.Where(c => c.ParentCategoryId != 0).ToList().ForEach(subcat =>
                {
                    categoryTreeList.Where(cat => cat.CategoryId == subcat.ParentCategoryId).ToList().ForEach(c => c.SubCategories.Add(subcat));
                });
            }
            return categoryTreeList;
        }

        private static SE.CategoryTree CategorySEtoSETree(SE.Category categorySE)
        {
            SE.CategoryTree cateTree = new SE.CategoryTree();
            cateTree.CategoryId = categorySE.CategoryId;
          //  cateTree.CreatedBy = Utility.GetLoggedInUser();
            cateTree.Description = categorySE.Description;
            cateTree.IsDeleted = categorySE.IsDeleted;
           // cateTree.ModifiedBy = Utility.GetLoggedInUser();
            cateTree.Name = categorySE.Name;
            cateTree.ParentId = categorySE.ParentId.GetValueOrDefault();
            cateTree.SubCategories = new List<SE.Category>();//initializing so that the subsequent addition of subcategory if any will not fail
            return cateTree;
        }
    }
}
