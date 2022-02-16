/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.WFDesigner.Entities;
using Infosys.WEM.Service.Contracts.Data;

namespace Infosys.ATR.WFDesigner.Views
{
    public interface IWFSelector
    {
        List<Entities.Category> Categories { get; set; }

        List<WorkflowPE> Workflows { get; set; }

        void DeleteWF();

        void RunWFSelector(); 
    }
}
