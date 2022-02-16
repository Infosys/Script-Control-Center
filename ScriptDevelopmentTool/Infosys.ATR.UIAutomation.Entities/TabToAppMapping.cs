/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Infosys.ATR.UIAutomation.Entities
{
    public class TabToAppMapping
    {
        public IntPtr TabItemHandle { get; set; }
        public IntPtr TabHandle { get; set; }
        public IntPtr AppHandle { get; set; }
        public string AppPointer { get; set; }
        public int AppProcessId { get; set; }
        public Rectangle BoundingRectangle { get; set; }
    }
}
