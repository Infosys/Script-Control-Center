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
using System.Windows;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base
{
    public abstract class ListBase : Control
    {
        Condition _condition;
        IntPtr _webListCtrlHndl, _appWinHandle, _screenWinHandle;
        bool _multipleSelect = false;

        //private AutomationElement controlFound;

        //protected AutomationElement ControlReference;
        ControlReferenceBase _Control_Reference = new ControlReferenceBase();
        protected ControlReferenceBase Control_Reference
        {
            get {
                _Control_Reference.JavaControlReference = JavaControlElementFound;
                _Control_Reference.WinControlReference = WinControlElementFound;
                return _Control_Reference; 
            }
            set { _Control_Reference = value; }
        }

        protected IntPtr WebListCtrlWinHandle
        {
            get { return _webListCtrlHndl; }
            set { _webListCtrlHndl = value; }
        }


        protected Condition ControlCondition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        public ListBase(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName,
            string applicationTreePath, string applicationType, string fullControlQualifier)
            : base(appWinHandle, screenWinHandle)
        {
            this.AutomationId = automationId;
            this.AutomationName = automationName;
            this.ControlPath = applicationTreePath;
            this.ApplicationType = applicationType;
            this.FullControlQualifier = fullControlQualifier;

            //below items to be used in case of list item selection
            _appWinHandle = appWinHandle;
            _screenWinHandle = screenWinHandle;
        }

        /// <summary>
        /// Interface to select an item under the controls like list or combobox, etc. To be used only for application
        /// of type- windows or web. Can't be used for application like java, for such application type, 
        /// in the atr provide the control information (entityControlConfig) with application tree path
        /// for the item under the list or combobox and call the click method.
        /// </summary>
        /// <param name="matchingKey">the display string of the item to be selected</param>
        public void SelectSingleItem(string matchingKey)
        {
            //TODO: Rahul 
            ListItem listItem = null;
            if (WebListCtrlWinHandle != null && WebListCtrlWinHandle != IntPtr.Zero)
            {
                listItem = new ListItem(WebListCtrlWinHandle, IntPtr.Zero, "", matchingKey, "", this.ApplicationType, this.FullControlQualifier);                
            }
            else
            {
                listItem = new ListItem(_appWinHandle, _screenWinHandle, "", matchingKey, "", this.ApplicationType, this.FullControlQualifier);
            }

            if (listItem != null)
            {
                if (listItem.Control_Reference.WinControlReference != null)
                {
                    object pattern;
                    if (listItem.Control_Reference.WinControlReference.TryGetCurrentPattern(SelectionItemPattern.Pattern, out pattern))
                    {
                        try
                        {
                            SelectionItemPattern select = pattern as SelectionItemPattern;
                            if (listItem.Control_Reference.WinControlReference.Current.IsEnabled)
                            {
                                if (_multipleSelect)
                                    select.AddToSelection();
                                else
                                    select.Select();
                            }
                            //System.Windows.Forms.MessageBox.Show("selected");
                        }
                        catch (InvalidOperationException)
                        {
                            //some times it is noticed that though the select works but aftre that an exception
                            //is raised with message- element is not enabled. 
                            //though we have put for check if the element is enabled then only do select
                        }
                    }
                    else if (listItem.Control_Reference.WinControlReference.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
                    {
                        InvokePattern invoke = pattern as InvokePattern;
                        invoke.Invoke();
                    }
                    else
                    {
                        if (listItem.Control_Reference.WinControlReference.Current.BoundingRectangle != Rect.Empty)
                        {
                            Core.Utilities.PlaceMouseCursor(listItem.Control_Reference.WinControlReference.Current.BoundingRectangle.X, listItem.Control_Reference.WinControlReference.Current.BoundingRectangle.Y);
                            Core.Utilities.DoMouseClick();
                        }
                    }
                }
                else if (Control_Reference.JavaControlReference != null)
                {
                    //for java application, the item under a list or under a combobox, cant be selected
                    //using this interface/approach.
                    //for java application, in the atr provide the control information (entityControlConfig) with application tree path
                    //for the item under the list or combobox and call the click method.
                }
            }
        }

        /// <summary>
        /// Interface to select more than one items from the list. Call this interface only for the controls
        /// which support multiple select.
        /// </summary>
        /// <param name="matchingKeys">the text of the items to be selected</param>
        public void SelectMultipleItems(string[] matchingKeys)
        {
            //TODO: Rahul 
            _multipleSelect = true;
            foreach (string key in matchingKeys)
                SelectSingleItem(key);
            _multipleSelect = false; //so that any explicit call to the SelectSingleItem does not behave like multiple select
        }

        /// <summary>
        /// Interface to select all items from the list. Call this interface only for the controls
        /// which support multiple select.
        /// </summary>
        public void SelectAllItems()
        {
            //TODO: Rahul 
            SelectMultipleItems(ReadAllItems());
        }

        public string ReadSingleItem(int index)
        {
            return readListItems(new int[] { index })[0];
        }
        public string[] ReadMultipleItems(int[] index)
        {
            return readListItems(index);
        }
        public string[] ReadAllItems()
        {
            return readListItems(new int[]{0});
        }

        private string[] readListItems(int[] index)
        {
            string[] itemValue = null;
            if (null != index)
            {
                IntPtr winHndl;
                if ((WebListCtrlWinHandle != null) && (WebListCtrlWinHandle != IntPtr.Zero) ) 
                {
                    //Cases where the WebListCtrlWinHandle has a non-zero pointer. This case will occure, when the control being access is a
                    // web combobox control.
                    winHndl = WebListCtrlWinHandle;
                }
                else
                {
                    winHndl = new IntPtr(Control_Reference.WinControlReference.Current.NativeWindowHandle);
                }
                object selectionPattern;
                if (Control_Reference.WinControlReference.TryGetCurrentPattern(
                            SelectionPattern.Pattern, out selectionPattern))
                {

                    PropertyCondition listItemControl = new PropertyCondition(
                    AutomationElement.ControlTypeProperty,
                    ControlType.ListItem);
                    AutomationElementCollection aec = AutomationElement.FromHandle(winHndl).FindAll(TreeScope.Subtree, listItemControl);

                    if (null != aec)
                    {
                        if ((index.Length == 1) && (index[0] == 0))
                        {
                            itemValue = new string[aec.Count];
                            for (int i = 0; i < aec.Count; i++)
                            {
                                itemValue[i] = aec[i].Current.Name;
                            }

                        }
                        else if ((index.Length == 1) && (index[0] != 0))
                        {
                            itemValue = new string[1];
                            itemValue[0] = aec[index[0]].Current.Name;
                        }
                        else if (index.Length > 1)
                        {
                            itemValue = new string[index.Length];
                            for (int i = 0; i < index.Length; i++)
                            {
                                itemValue[i] = aec[index[i]].Current.Name;
                            }

                        }
                    }

                }
            } 
            return itemValue;

        }

        //public void KeyPress(string keys)
        //{
        //    System.Threading.Thread.Sleep(200); //this wait is needed for the parent list/combox to get renderred properly
        //    Core.Utilities.KeyPress(keys);            
        //}

    }

    public enum ExpandCollapseState
    {
        Collapsed,
        Expanded,
        LeafNode,
        PartiallyExpanded,
        UnInitialized
    }
}
