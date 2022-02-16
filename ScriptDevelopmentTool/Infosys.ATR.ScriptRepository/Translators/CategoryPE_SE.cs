using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using PE = Infosys.ATR.ScriptRepository.Models;

namespace Infosys.ATR.ScriptRepository.Translators
{
    public class CategoryPE_SE
    {
        public static PE.Category CategorySEtoPE(SE.Category catSE)
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
            }
            return catPE;
        }

        public static SE.Category CategoryPEtoSE(PE.Category catPE)
        {
            SE.Category catSE = null;
            if (catPE != null)
            {
                catSE = new SE.Category();
                catSE.CreatedBy = catPE.CreatedBy;
                catSE.Description = catPE.Description;
                catSE.CategoryId = int.Parse(catPE.Id);
                catSE.ModifiedBy = catPE.ModifiedBy;
                catSE.Name = catPE.Name;
            }
            return catSE;
        }

        public static SE.Category SubCategoryPEtoSE(PE.SubCategory subcatPE, string parentCategoryId)
        {
            SE.Category catSE = null;
            if (subcatPE != null)
            {
                catSE = new SE.Category();
                catSE.CreatedBy = subcatPE.CreatedBy;
                catSE.Description = subcatPE.Description;
                catSE.CategoryId = int.Parse(subcatPE.Id);
                catSE.ModifiedBy = subcatPE.ModifiedBy;
                catSE.Name = subcatPE.Name;
                catSE.ParentCategoryId = int.Parse(parentCategoryId);
            }
            return catSE;
        }
    }
}
