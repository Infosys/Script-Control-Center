/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Infosys.WEM.Service.Contracts.Data;

using Infosys.ATR.WFDesigner.Entities;
using System.Windows.Forms;

namespace Infosys.ATR.WFDesigner.Views
{
    public interface ISettings
    {
        List<Category> Categories { get; set; }
        Tuple<TreeNode, Category> Catdetails { get; set; }
        void ShowCatDetails();
    }
}
