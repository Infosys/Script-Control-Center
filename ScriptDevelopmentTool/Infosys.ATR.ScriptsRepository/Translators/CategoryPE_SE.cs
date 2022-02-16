/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SE = Infosys.WEM.Service.Common.Contracts.Data;
using PE = Infosys.ATR.ScriptRepository.Models;

namespace Infosys.ATR.ScriptRepository.Translators
{
    public class CategoryPE_SE
    {
        public static List<PE.Category> CategoryListSEtoPE(List<SE.Category> catSEs)
        {

            List<PE.Category> catPEs = null;
            if (catSEs != null && catSEs.Count() > 0)
            {
                catPEs = new List<PE.Category>();
                catSEs.ForEach(se =>
                {
                    catPEs.Add(CategorySEtoPE(se));
                });
            }
            return catPEs;
        }

        public static PE.Category CategorySEtoPE(SE.Category catSE)
        {
            PE.Category catPE = null;
            if (catSE != null)
            {
                catPE = new PE.Category();
               // catPE.CreatedBy = catSE.CreatedBy;
                catPE.Description = catSE.Description;
                catPE.Id = catSE.CategoryId.ToString();
               // catPE.ModifiedBy = catSE.ModifiedBy;
                catPE.Name = catSE.Name;
                catPE.ParentId = catSE.ParentId.Value;
                catPE.CompanyId = catSE.CompanyId;
            }
            return catPE;
        }
        //public static PE.Category CategorySEtoPE(SE.Category catSE)
        //{
        //    PE.Category catPE = null;
        //    if (catSE != null)
        //    {
        //        catPE = new PE.Category();
        //        catPE.CreatedBy = catSE.CreatedBy;
        //        catPE.Description = catSE.Description;
        //        catPE.Id = catSE.CategoryId.ToString();
        //        catPE.ModifiedBy = catSE.ModifiedBy;
        //        catPE.Name = catSE.Name;
        //    }
        //    return catPE;
        //}

        public static SE.Category CategoryPEtoSE(PE.Category catPE)
        {
            SE.Category catSE = null;
            if (catPE != null)
            {
                catSE = new SE.Category();
              //  catSE.CreatedBy = catPE.CreatedBy;
                catSE.Description = catPE.Description;
                catSE.CategoryId = int.Parse(catPE.Id);
               // catSE.ModifiedBy = catPE.ModifiedBy;
                catSE.Name = catPE.Name;
            }
            return catSE;
        }

        //public static SE.Category SubCategoryPEtoSE(PE.SubCategory subcatPE, string parentCategoryId)
        //{
        //    SE.Category catSE = null;
        //    if (subcatPE != null)
        //    {
        //        catSE = new SE.Category();
        //        catSE.CreatedBy = subcatPE.CreatedBy;
        //        catSE.Description = subcatPE.Description;
        //        catSE.CategoryId = int.Parse(subcatPE.Id);
        //        catSE.ModifiedBy = subcatPE.ModifiedBy;
        //        catSE.Name = subcatPE.Name;
        //        catSE.ParentCategoryId = int.Parse(parentCategoryId);
        //    }
        //    return catSE;
        //}
    }
}
