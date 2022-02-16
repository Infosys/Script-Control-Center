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
using System.Windows.Forms;
using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.UIAutomation.Entities;

namespace Infosys.ATR.Editor.Views
{
    public interface IEditor
    {
        Entity.Area Area { get; set; }
        String State { get; set; }
        String BaseDir { get; }
        void SaveImage(System.Drawing.Image image);
        void BuildTree(TreeNode node);
        void Save(string autoConfig, string editorUcName);
        void UpdateName(string name);
        Entity.Root _root { get; set; }
        string ObjectModel { get; set; }
        Entity.ProjectMode Mode { get; set; }

        void UpdateControlProperties(string p);
        void UpdateAppProperties(string[] p);
    }
}
