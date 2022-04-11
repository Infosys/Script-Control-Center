﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using PE=  Infosys.ATR.ScriptRepository.Models;

namespace Infosys.ATR.ScriptRepository.Translators
{
    public class CategoryTreePE_SE
    {
        public static List<PE.Category> CategoryListSEtoPE(List<SE.CategoryTree> catSEs)
        {
            List<PE.Category> catPEs = null;
            if (catSEs != null && catSEs.Count() > 0)
            {
                catPEs = new List<PE.Category>();
                catSEs.ForEach(se => {
                    catPEs.Add(CategorySEtoPE(se));
                });
            }
            return catPEs;
        }

        public static PE.Category CategorySEtoPE(SE.CategoryTree catSE)
        {
            PE.Category catPE = null;
            if (catSE != null)
            {
                catPE = new PE.Category();
                catPE.CreatedBy = catSE.CreatedBy;
                catPE.Description = catSE.Description;
                catPE.Id = catSE.CategoryId.ToString();
                catPE.ModifiedBy = catSE.ModifiedBy;
                catPE.Name = catSE.Name;
                if(catSE.SubCategories != null && catSE.SubCategories.Count>0)
                {
                    catPE.SubCategories = new List<PE.SubCategory>();
                    catSE.SubCategories.ForEach(se => {
                        catPE.SubCategories.Add(SubCategorySEtoPE(se));
                    });
                }
            }
            return catPE;
        }

        public static PE.SubCategory SubCategorySEtoPE(SE.Category subcatSE)
        {
            PE.SubCategory subcatPE = null;
            if (subcatSE != null)
            {
                subcatPE = new PE.SubCategory();
                subcatPE.CreatedBy = subcatSE.CreatedBy;
                subcatPE.Description = subcatSE.Description;
                subcatPE.Id = subcatSE.CategoryId.ToString();
                subcatPE.ModifiedBy = subcatSE.ModifiedBy;
                subcatPE.Name = subcatSE.Name;
            }
            return subcatPE;
        }
        
    }
}