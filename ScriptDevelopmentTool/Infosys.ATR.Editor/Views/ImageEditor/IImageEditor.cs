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

using Infosys.ATR.Editor.Entities;

namespace Infosys.ATR.Editor.Views
{
    public interface IImageEditor
    {
        string ImagePath { get; set; }
        string BaseDir { get; set; }
        string Name { get; set; }
        BindingList<String> States { get; set; }
        void ShowImage();
        void ClearAll();
        void UpdateImageProperties(object[] param);


        void UpdateApplicationProperties(ApplicationProperties baseProperties);
    }
}
