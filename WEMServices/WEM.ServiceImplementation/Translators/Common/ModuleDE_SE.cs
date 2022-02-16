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
using SE = Infosys.WEM.Service.Common.Contracts.Data;
using DE = Infosys.WEM.Resource.Entity;

namespace Infosys.WEM.Service.Implementation.Translators.Common
{
    public class ModuleDE_SE
    {
        public static List<SE.Module> ModuleDEListtoSEList(List<DE.Module> moduleDEList)
        {
            List<SE.Module> moduleSEList = null;
            if (moduleDEList != null)
            {
                moduleSEList = new List<SE.Module>();
                moduleDEList.ForEach(de =>
                {
                    moduleSEList.Add(ModuleDEtoSE(de));
                });
            }
            return moduleSEList;
        }

        public static SE.Module ModuleDEtoSE(DE.Module moduleDE)
        {
            SE.Module moduleSE = null;
            if (moduleDE != null)
            {
                moduleSE = new SE.Module();              
                moduleSE.ModuleId = moduleDE.ModuleID;
                moduleSE.ModuleName = moduleDE.ModuleName;
            }
            return moduleSE;
        }
    }
}
