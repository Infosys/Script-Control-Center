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

using SE = Infosys.ATR.ExportUtility.Models.ECR;
using PE = Infosys.ATR.ExportUtility.Models;


namespace Infosys.ATR.ExportUtility.Translators
{
    public class ECRCategorySE_PE
    {
        internal static List<PE.NIACategory> catList = new List<PE.NIACategory>();
        public static List<PE.NIACategory> CategoryListSEtoPE(List<SE.Category> catSEs)
        {
            List<PE.NIACategory> catPEs = null;
            if (catSEs != null && catSEs.Count() > 0)
            {
                catPEs = new List<PE.NIACategory>();
                catSEs.ForEach(se =>
                {
                    catPEs.Add(CategorySEtoPE(se));
                });
            }
            return catPEs;
        }

        private static PE.NIACategory CategorySEtoPE(SE.Category catSE)
        {
            PE.NIACategory catPE = null;
            if (catSE != null)
            {
                catPE = new PE.NIACategory();
                catPE.Children = new List<PE.NIACategory>();
                catPE.Id = catSE.Id;
                catPE.Name = catSE.Name;
                //catPE.Children = catSE.Children;
                if (catSE.Children != null)
                {
                    catSE.Children.ForEach(se =>
                    {
                        catPE.Children.Add(CategorySEtoPE(se));
                    });
                }
                //if (catSE.Children !=null && catSE.Children.Count > 0)
                //{
                //    var cats = CategorySEtoPE(catSE.Children);
                //    if (cats != null)
                //        catPE.Children.Add(cats);
                //}
            }
            return catPE;
        }

        private static PE.NIACategory CategorySEtoPE(List<SE.Category> catSE)
        {
            PE.NIACategory catPE = null;
            if (catSE != null)
            {
                foreach (SE.Category cat in catSE)
                {
                    catPE = new PE.NIACategory();
                    catPE.Children = new List<PE.NIACategory>();
                    catPE.Id = cat.Id;
                    catPE.Name = cat.Name;
                    if (cat.Children!=null && cat.Children.Count >0)
                        catPE.Children.Add(CategorySEtoPE(cat.Children));
                }
            }
            return catPE;
        }

        public static List<PE.NIACategory> CategoryListSEtoPEWithParentId(List<SE.Category> catSEs)
        {
            //List<PE.NIACategory> catPEs = null;
            if (catSEs != null && catSEs.Count() > 0)
            {
                //catPEs = new List<PE.NIACategory>();
                catSEs.ForEach(se =>
                {
                   CategorySEtoPEWithParentId(se);
                });
            }
            return catList;
        }

        private static void CategorySEtoPEWithParentId(SE.Category catSE, int parentId=0)
        {
            PE.NIACategory catPE = null;
            if (catSE != null)
            {
                catPE = new PE.NIACategory();
                catPE.Children = new List<PE.NIACategory>();
                catPE.Id = catSE.Id;
                catPE.Name = catSE.Name;
                catPE.ParentId = parentId;
                catList.Add(catPE);
                //catPE.Children = catSE.Children;
                if (catSE.Children != null)
                {
                    catSE.Children.ForEach(se =>
                    {
                        CategorySEtoPEWithParentId(se, int.Parse( catSE.Id));
                    });
                }
                //if (catSE.Children !=null && catSE.Children.Count > 0)
                //{
                //    var cats = CategorySEtoPE(catSE.Children);
                //    if (cats != null)
                //        catPE.Children.Add(cats);
                //}
            }
            //return catPE;
        }
    }
}
