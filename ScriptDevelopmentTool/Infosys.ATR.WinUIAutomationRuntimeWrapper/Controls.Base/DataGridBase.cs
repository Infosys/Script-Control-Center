/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;

using Infosys.JavaAccessBridge;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base
{
    public class DataGridBase: Control
    {
        Condition _condition;
        ControlReferenceBase _Control_Reference = new ControlReferenceBase();
        protected ControlReferenceBase Control_Reference
        {
            get
            {
                _Control_Reference.JavaControlReference = JavaControlElementFound;
                _Control_Reference.WinControlReference = WinControlElementFound;
                return _Control_Reference;
            }
            set { _Control_Reference = value; }
        }

        protected Condition ControlCondition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public DataGridBase(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName,
            string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle)
        {
            this.AutomationId = automationId;
            this.AutomationName = automationName;
            this.ControlPath = applicationTreePath;
            this.ApplicationType = applicationType;
            this.FullControlQualifier = fullControlQualifier;
        }

        public List<DataItem> GetSelectedItems()
        {
            List<DataItem> items = new List<DataItem>();
            if (this.Control_Reference != null)
            {
                PropertyCondition dataItemControl = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem);
                AutomationElementCollection ctls = this.Control_Reference.WinControlReference.FindAll(TreeScope.Subtree, dataItemControl);
                if (ctls != null && ctls.Count > 0)
                {
                    foreach (AutomationElement ctl in ctls)
                    {
                        object pattern;
                        if (ctl.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                        {
                            SelectionItemPattern selectionItemPattern = pattern as SelectionItemPattern;
                            if (selectionItemPattern.Current.IsSelected)
                            {
                                items.Add(new DataItem(new IntPtr(this.AppWindowHandle), new IntPtr(this.ScreenWindowHandle), ctl.Current.AutomationId, ctl.Current.Name, "", this.ApplicationType, ""));
                                break;
                            }
                        }
                    }
                }
            }
            return items;
        }

        public void SelectItems(List<string> names)
        {
            if (this.Control_Reference != null)
            {
                PropertyCondition typeCond = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem);
                foreach (string name in names)
                {
                    PropertyCondition nameCond = new PropertyCondition(AutomationElement.NameProperty, name);
                    AndCondition andCond = new AndCondition(typeCond, nameCond);
                    AutomationElementCollection ctls = this.Control_Reference.WinControlReference.FindAll(TreeScope.Subtree, andCond);
                    if (ctls != null && ctls.Count > 0)
                    {
                        object pattern;
                        if (ctls[0].TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                        {
                            SelectionItemPattern selectionItemPattern = pattern as SelectionItemPattern;
                            selectionItemPattern.Select();
                        }
                    }
                }
            }
        }

        public void SelectAllItems()
        {
            if (this.Control_Reference != null)
            {
                PropertyCondition typeCond = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem);
                AutomationElementCollection ctls = this.Control_Reference.WinControlReference.FindAll(TreeScope.Subtree, typeCond);
                if (ctls != null && ctls.Count > 0)
                {
                    object pattern;
                    if (ctls[0].TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                    {
                        SelectionItemPattern selectionItemPattern = pattern as SelectionItemPattern;
                        selectionItemPattern.Select();
                    }
                }
            }
        }
    }
}
