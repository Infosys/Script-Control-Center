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
using System.Windows.Forms;
using Infosys.ATR.Admin.Entities;

namespace Infosys.ATR.Admin.Views
{
    public interface IExplorer
    {
        List<Groups> Groups { get; set; }
        List<SemanticGroup> Clusters { get; set; }
        BindingList<Users> Users { get; set; }
        void RefreshCategory(string node);
        TreeNode SelectedNode { get; set; }
        void GetAllNodes(Groups g);
        void DisplayMessage(string p);
        void RefreshSemanticTree();
        void RefreshUsers();
    }

}
