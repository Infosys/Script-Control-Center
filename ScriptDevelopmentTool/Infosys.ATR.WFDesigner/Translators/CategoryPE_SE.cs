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
using PE = Infosys.ATR.WFDesigner.Entities;

namespace Infosys.ATR.WFDesigner.Translators
{
    public class CategoryPE_SE
    {
        public static List< PE.Category> CategoryListSEtoPE(List<SE.Category> catSEs)
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
            //    catPE.CreatedBy = catSE.CreatedBy;
                catPE.Description = catSE.Description;
                catPE.CategoryId = catSE.CategoryId;
            //    catPE.ModifiedBy = catSE.ModifiedBy;
                catPE.Name = catSE.Name;
                catPE.ParentId = catSE.ParentId.Value;
                catPE.CompanyId = catSE.CompanyId;
            }
            return catPE;
        }
    }
}
