/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Activities.Presentation.PropertyEditing;

namespace Infosys.WEM.AutomationActivity.Libraries.Editors
{
    public class ListSelectionEditor : PropertyValueEditor
    {
        public ListSelectionEditor()
        {
            this.InlineEditorTemplate = new DataTemplate();
            FrameworkElementFactory stack = new FrameworkElementFactory(typeof(StackPanel));
            FrameworkElementFactory listBox = new FrameworkElementFactory(typeof(ListBox));
            Binding listBoxBinding = new Binding("Value");
            listBoxBinding.Mode = BindingMode.TwoWay;
            listBox.SetValue(System.Windows.Controls.DataGrid.ItemsSourceProperty, listBoxBinding);
            stack.AppendChild(listBox);
            this.InlineEditorTemplate.VisualTree = stack;
        }
    }
}
