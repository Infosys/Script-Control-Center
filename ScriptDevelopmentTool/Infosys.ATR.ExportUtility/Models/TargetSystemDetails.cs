﻿/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.ATR.ExportUtility.Models
{
    public class TargetSystemDetails
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int APIType { get; set; }
        public int Protocol { get; set; }
        public Boolean DefaultType { get; set; }
    }
}