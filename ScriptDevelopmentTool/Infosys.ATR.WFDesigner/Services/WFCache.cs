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
using Infosys.ATR.WFDesigner.Entities;

namespace Infosys.ATR.WFDesigner.Services
{
    public class WFCache
    {
        WFCache()
        {
        }

        public static List<Category> CategoryDetails { get; set; }

        public static List<Category> CategoryDetailsWithData { get; set; }
    }
}
