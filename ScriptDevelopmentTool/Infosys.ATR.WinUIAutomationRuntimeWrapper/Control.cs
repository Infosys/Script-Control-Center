/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;

using Infosys.JavaAccessBridge;
using Infosys.WEM.Infrastructure.Common;
using Infosys.ATR.OCRWrapper;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper
{
    public class Control
    {

        private AutomationElement _HandleElement;
        private AutomationElementCollection _controlElementCollectionFound;
        private AutomationElement _controlElementFound;
        private EX_JABHelper.AccessibleTreeItem _javacontrolElementFound;
        private IntPtr _appWinHandle;
        private IntPtr _screenWinHandle;
        private AutomationElement _parentControl;
        private Condition _condition;
        private Dictionary<string, Control> _controls;
        private int[] _runtimeId;
        private string _automationControlName;

        private bool _highlightElement = false; //to be used to highlight the matched region in case of image based recognition
        private Views.Highlighter highlighter = null; //-do-

        private string className = "Control";

        //events for client developers
        #region Event- PropertyHasChanged
        public class PropertyHasChangedArgs : EventArgs
        {
            public Control Control { get; set; }
            public string ChangedProperty { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }
        public delegate void PropertyHasChangedEventHandler(PropertyHasChangedArgs e);
        public event PropertyHasChangedEventHandler PropertyHasChanged;
        #endregion

        #region Event- StructureHasChanged
        public class StructureHasChangedArgs : EventArgs
        {
            public Control Control { get; set; }
            public string StructureChangeType { get; set; }
        }
        public delegate void StructureHasChangedEventHandler(StructureHasChangedArgs e);
        public event StructureHasChangedEventHandler StructureHasChanged;
        #endregion

        protected AutomationElement WinControlElementFound
        {
            get
            {
                return _controlElementFound;
            }
        }

        protected EX_JABHelper.AccessibleTreeItem JavaControlElementFound
        {
            get
            {
                return _javacontrolElementFound;
            }
        }

        public Control(IntPtr appWinHandle, IntPtr screenWinHandle)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.CONTROL))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWinHandle", Logging.Constants.PARAMDIRECTION_IN, appWinHandle.ToString());

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "screenWinHandle", Logging.Constants.PARAMDIRECTION_IN, screenWinHandle.ToString());

                _appWinHandle = appWinHandle;
                _screenWinHandle = screenWinHandle;
                IntPtr winHandleToUse;
                winHandleToUse = (_screenWinHandle != IntPtr.Zero) ? _screenWinHandle : _appWinHandle;
                if (winHandleToUse != IntPtr.Zero && Core.Utilities.IsWindowsHandleActive(winHandleToUse))
                    _HandleElement = AutomationElement.FromHandle(winHandleToUse);

            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.CONTROL);
        }

        private AutomationElement MainHandleElement
        {
            get { return _HandleElement; }
        }

        protected AutomationElementCollection FindControls(Condition condition)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROLS))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "condition", Logging.Constants.PARAMDIRECTION_IN, condition.ToString());

                _condition = condition;
                if (null == ParentControl)
                {
                    _controlElementCollectionFound = MainHandleElement.FindAll(TreeScope.Subtree, condition);
                }
                else
                {
                    _controlElementCollectionFound = _parentControl.FindAll(TreeScope.Subtree, condition);
                }
            }

            //out param      
            if (_controlElementCollectionFound != null)
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "_controlElementCollectionFound", Logging.Constants.PARAMDIRECTION_OUT, _controlElementCollectionFound.Count.ToString());

            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROLS);
            return _controlElementCollectionFound;
        }

        protected void InitializeControlCondition(Condition condition)
        {
            _condition = condition;
        }

        /// <summary>
        /// To refresh a control when the content of the control is changed but the control tree path is same- call the new interface- RefreshControl instead of RefreshControlHandle.
        /// </summary>
        public void RefreshControl()
        {
            Core.Utilities.ClearCache(this.ControlPath);
            RefreshControlHandle(new IntPtr(this.AppWindowHandle), new IntPtr(this.ScreenWindowHandle));
        }

        protected AutomationElement FindControl(Condition condition)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROL))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "condition", Logging.Constants.PARAMDIRECTION_IN, condition.ToString());

                _condition = condition;
                //check if both the name and id properties are blank, 
                //then directly take the 'control tree path' approach
                if (condition != null && (condition.GetType() == typeof(System.Windows.Automation.AndCondition)))
                {
                    Condition controlTypeCondition = null;
                    Condition[] conditions = ((System.Windows.Automation.AndCondition)(condition)).GetConditions();
                    if (conditions != null && conditions.Length == 3)
                    {
                        string name = "", id = "";
                        foreach (Condition cond in conditions)
                        {
                            switch (((System.Windows.Automation.PropertyCondition)cond).Property.ProgrammaticName)
                            {
                                case "AutomationElementIdentifiers.NameProperty":
                                    name = ((System.Windows.Automation.PropertyCondition)cond).Value as string;
                                    break;
                                case "AutomationElementIdentifiers.AutomationIdProperty":
                                    id = ((System.Windows.Automation.PropertyCondition)cond).Value as string;
                                    break;
                                case "AutomationElementIdentifiers.ControlTypeProperty":
                                    controlTypeCondition = cond;
                                    break;
                            }
                        }
                        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(this.ControlPath))
                        {
                            _controlElementFound = Core.Utilities.FilterAutomationElement(_controlElementCollectionFound, this, _appWinHandle, this.FullControlQualifier);
                            if (_controlElementFound != null)
                                _runtimeId = _controlElementFound.GetRuntimeId();

                            if (HasChildren)
                            {
                                foreach (KeyValuePair<string, Control> ctrl in Controls)
                                {
                                    //Set the reference of the parent automation element
                                    Controls[ctrl.Key].ParentControl = _controlElementFound;
                                }
                            }
                            return _controlElementFound;
                        }
                        //to handle condition where user may need to find control based on control type eg.. find all text controls only
                        if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(id) && string.IsNullOrEmpty(this.ControlPath) && controlTypeCondition != null)
                        {
                            condition = controlTypeCondition;
                    }
                }
                }

                if (null == ParentControl)
                {
                    if (MainHandleElement == null)
                        return null;
                    _controlElementCollectionFound = MainHandleElement.FindAll(TreeScope.Subtree, condition);
                }
                else
                {
                    if (_parentControl == null)
                        return null;
                    _controlElementCollectionFound = _parentControl.FindAll(TreeScope.Subtree, condition);
                }

                if (_controlElementCollectionFound == null)
                    return null;

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "_controlElementCollectionFound", _controlElementCollectionFound.Count.ToString());
                AutomationElement tempControl = null;
                switch (_controlElementCollectionFound.Count)
                {
                    case 0:
                        _controlElementFound = null;

                        tempControl = AutomationElement.FocusedElement;

                        if ((tempControl.Current.AutomationId == this.AutomationId) &&
                            (tempControl.Current.Name == this.AutomationName))
                        {
                            _controlElementFound = tempControl;
                        }
                        else
                        {
                            if (ApplicationType.ToLower() == "web")
                                _controlElementFound = Core.Utilities.MatchForApplicationTreePath(this.ControlPath, _appWinHandle, this.FullControlQualifier, this);

                        }

                        if (_controlElementFound == null && DoTabTracking)
                            _controlElementFound = Core.Utilities.FindElementViaFocusTracking(tempControl, this);
                        if (_controlElementFound != null)
                            _runtimeId = _controlElementFound.GetRuntimeId();
                        break;
                    case 1:
                        _controlElementFound = _controlElementCollectionFound[0];
                        if (_controlElementFound != null)
                            _runtimeId = _controlElementFound.GetRuntimeId();
                        break;
                    default:
                        if (!GetAllMatchingControls) //if only one control is intended then go inside
                        {
                            //TODO:Rahul _controlElementFound= Rahul Find the specific control to be mapped (read controltreepath and read the right control runtimeid) if there are either muliple or no
                            //automationelement object returned.
                            //Load _runtimeId property

                            //if app tree parth is blank, then return the first item in the list
                            if (string.IsNullOrEmpty(this.ControlPath))
                                _controlElementFound = _controlElementCollectionFound[0];
                            else
                                _controlElementFound = Core.Utilities.FilterAutomationElement(_controlElementCollectionFound, this, _appWinHandle, this.FullControlQualifier);

                            if (_controlElementFound != null)
                                _runtimeId = _controlElementFound.GetRuntimeId();
                        }
                        else
                        {
                            _controlElementFound = _controlElementCollectionFound[0]; // this is just to make sure the 'is control available' returns true when FindControls is called code control collection
                        }
                        break;
                }
                if (HasChildren) //Bug: The children mapping logic is incorrect if the control returns a collection of automation elements. The following logic then maps the child to only parent and not different child for different matches
                {

                    foreach (KeyValuePair<string, Control> ctrl in Controls)
                    {
                        //Set the reference of the parent automation element
                        Controls[ctrl.Key].ParentControl = _controlElementFound;
                    }

                }
            }
            //out param      
            if (_controlElementFound != null)
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "_controlElementFound", Logging.Constants.PARAMDIRECTION_OUT, _controlElementFound.ToString());

            // Log entry for method exit
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROL);

            return _controlElementFound;
        }

        protected EX_JABHelper.AccessibleTreeItem FindControl(string appTreePath, bool isJavaApp = true)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROL))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appTreePath", Logging.Constants.PARAMDIRECTION_IN, appTreePath);

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "isJavaApp", Logging.Constants.PARAMDIRECTION_IN, isJavaApp.ToString());

                if (isJavaApp)
                {
                    _javacontrolElementFound = Core.Utilities.MatchForJavaApplicationTreePath(appTreePath, _appWinHandle);
                }
            }
            //if (HasChildren)
            //{
            //    foreach (KeyValuePair<string, Control> ctrl in Controls)
            //    {
            //        //Set the reference of the parent automation element
            //        Controls[ctrl.Key].ParentControl = _controlElementFound;
            //    }
            //}

            //out param    
            if (_javacontrolElementFound != null)
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "_javacontrolElementFound", Logging.Constants.PARAMDIRECTION_OUT, _javacontrolElementFound.ToString());

            //Log entry for method exit
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROL);

            return _javacontrolElementFound;
        }

        public void AddControl(string key, Control control)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.ADDCONTROL))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "key", Logging.Constants.PARAMDIRECTION_IN, key);

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "control", Logging.Constants.PARAMDIRECTION_IN, control.ToString());

                control.ParentControl = _controlElementFound;
                _controls.Add(key, control);
            }

            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.ADDCONTROL);
        }

        //Controls can be at multiple level depths in the hierarchy
        public Dictionary<string, Control> Controls
        {
            get { return _controls; }
            set { _controls = value; }
        }

        public bool HasChildren
        {
            get
            {
                return (_controls != null && _controls.Count > 0) ? true : false;
            }
        }

        protected AutomationElement ParentControl
        {
            get { return _parentControl; }
            set { _parentControl = value; }
        }

        public string AutomationId { get; set; }

        public string FullControlQualifier { get; set; }//Appname.Screenname.controlname

        public string Name { get; set; }

        public string Value
        {
            get
            {
                string val = "";
                object valPattern;
                if (_controlElementFound != null)
                {
                    //check if supports value pattern then return value if text pattern is supported then retrun text
                    if (_controlElementFound.TryGetCurrentPattern(ValuePattern.Pattern, out valPattern))
                    {
                        ValuePattern vp = (ValuePattern)valPattern;
                        val = vp.Current.Value;
                    }
                    else if (_controlElementFound.TryGetCurrentPattern(TextPattern.Pattern, out valPattern))
                    {
                        if (_controlElementFound.Current.IsKeyboardFocusable)
                            _controlElementFound.SetFocus();
                        System.Windows.Automation.Text.TextPatternRange rng = ((TextPattern)valPattern).DocumentRange;
                        val = rng.GetText(-1);
                    }
                }
                else if (_javacontrolElementFound != null)
                {
                    val = _javacontrolElementFound.textValue;
                }
                return val;
            }
        }

        public int AppWindowHandle { get { return _appWinHandle.ToInt32(); } }

        public int ScreenWindowHandle { get { return _screenWinHandle.ToInt32(); } }

        public string GetControlType
        {
            get
            {
                return ControlType.LookupById(
                    (int)((System.Windows.Automation.PropertyCondition)((System.Windows.Automation.AndCondition)_condition).GetConditions()[0]).Value).ProgrammaticName;
            }
        }

        public string AutomationName
        {
            get { return _automationControlName; }
            set { _automationControlName = value; }

        }

        public string ApplicationType { get; set; }

        public string ControlTypeName
        {
            get { return _controlElementFound.Current.ControlType.ProgrammaticName; }
        }

        public bool IsControlAvailable()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.ISCONTROLAVAILABLE))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.ISCONTROLAVAILABLE);

                bool isAvailable = true;
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "ApplicationType", this.ApplicationType.ToLower());

                switch (this.ApplicationType.ToLower())
                {
                    case "java":
                        if (_javacontrolElementFound == null)
                            isAvailable = false;
                        else
                        {
                            try
                            {
                                isAvailable = _javacontrolElementFound.accessibleInterfaces;
                            }
                            catch
                            {
                                isAvailable = false;
                            }
                        }
                        break;
                    default:
                        if (_controlElementFound == null)
                            isAvailable = false;
                        else
                        {
                            try
                            {
                                int[] checkElementId = _controlElementFound.GetRuntimeId();
                                isAvailable = true;
                            }
                            catch (ElementNotAvailableException)
                            {
                                isAvailable = false;
                            }
                        }
                        break;
                }

                //out param          
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "isAvailable", Logging.Constants.PARAMDIRECTION_OUT, isAvailable.ToString());

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.ISCONTROLAVAILABLE);
                return isAvailable;
            }
        }

        public string ControlPath { get; set; }


        //Pass ControlName and return list of controls matching the name
        //TODO:Rahul 
        public Control FindControl(string name)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.FINDCONTROL))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "name", Logging.Constants.PARAMDIRECTION_IN, name);

                Control ctlFound = null;
                if (!string.IsNullOrEmpty(name))
                {
                    foreach (Control ctl in Controls.Values)
                    {
                        if (!string.IsNullOrEmpty(ctl.Name) && ctl.Name.ToLower() == name.ToLower()) //is the control name fine to be used
                        {
                            ctlFound = ctl;
                            ctlFound.ParentControl = this.ControlElementFound; // fix added as when the parent was a collection of controls then the wrong parent was being associated with the children
                            break;
                        }
                    }
                }
                //out param
                if (ctlFound != null)
                    LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "ctlFound", Logging.Constants.PARAMDIRECTION_OUT, ctlFound.ToString());

                // Log entry for method exit
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.FINDCONTROL);

                return ctlFound;
            }
        }

        public void Click()
        {
            //Button,HyperLink - IINVOKE
            //Checkbox - ToggleProvider
            //COMBOBOX - IExpandCollapseProvider
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.CLICK))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.CLICK);

                if (null != _controlElementFound)
                {
                    LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "_controlElementFound", _controlElementFound.ToString());

                    object invokePattern;
                    object togglePattern;
                    object expandCollapsePattern;
                    if (_controlElementFound.TryGetCurrentPattern(
                        InvokePattern.Pattern, out invokePattern))
                    {
                        //Button,HyperLink
                        ((InvokePattern)invokePattern).Invoke();

                    }
                    else if (_controlElementFound.TryGetCurrentPattern(
                        TogglePattern.Pattern, out togglePattern))
                    {
                        //CheckBox
                        TogglePattern togPattern = (TogglePattern)togglePattern;
                        (togPattern).Toggle();

                    }
                    else if (_controlElementFound.TryGetCurrentPattern(
                            ExpandCollapsePattern.Pattern, out expandCollapsePattern))
                    {
                        //Menu,ComboBox
                        ExpandCollapsePattern excolPattern = (ExpandCollapsePattern)expandCollapsePattern;
                        if (excolPattern.Current.ExpandCollapseState == ExpandCollapseState.Expanded)
                        {
                            excolPattern.Collapse();
                        }
                        else if (excolPattern.Current.ExpandCollapseState == ExpandCollapseState.Collapsed ||
                            excolPattern.Current.ExpandCollapseState == ExpandCollapseState.PartiallyExpanded)
                        {

                            excolPattern.Expand();
                        }

                    }
                    else
                    {
                        //If all fails identify the position of the control using BoundingRectangle and Invoke a Mouse click action
                        if (_controlElementFound.Current.BoundingRectangle != Rect.Empty)
                        {
                            //Core.Utilities.PlaceMouseCursor(_controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width / 2, _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height / 2);
                            double x = _controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width / 2;
                            double y = _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height / 2;
                            Core.Utilities.PlaceMouseCursor(x, y);
                            Core.Utilities.DoMouseClick();
                        }
                    }

                }
                else if (_javacontrolElementFound != null)
                {
                    if (_javacontrolElementFound.Bounds != null)
                    {
                        Core.Utilities.PlaceMouseCursor((double)_javacontrolElementFound.Bounds.X, (double)_javacontrolElementFound.Bounds.Y);
                        Core.Utilities.DoMouseClick();
                    }
                }
                else if (ImageBoundingRectangle != null) //for image based control identification
                {
                    if (highlighter != null)
                    {
                        highlighter.Close();
                        highlighter = null;
                    }
                    //position the mouse at the center of the rectangle
                    Core.Utilities.PlaceMouseCursor(ImageBoundingRectangle.X + ImageBoundingRectangle.Width / 2, ImageBoundingRectangle.Y + ImageBoundingRectangle.Height / 2);
                    Core.Utilities.DoMouseClick();
                }

            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.CLICK);
        }

        /// <summary>
        /// To click at a point which is at a position displaced by the offset provided from the control in concern
        /// </summary>
        /// <param name="x">+ve or -ve x displacement</param>
        /// <param name="y">+ve or -ve y displacement</param>
        public void ClickWithOffset(int x, int y)
        {
            double xaxis = 0, yaxis = 0;
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.CLICK))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.CLICK);

                if (null != _controlElementFound)
                {
                    if (_controlElementFound.Current.BoundingRectangle != Rect.Empty)
                    {
                        if (x < 0)
                            xaxis = _controlElementFound.Current.BoundingRectangle.X + x;
                        else if (x > 0)
                            xaxis = _controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width + x;
                        else
                            xaxis = _controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width / 2;

                        if (y < 0)
                            yaxis = _controlElementFound.Current.BoundingRectangle.Y + y;
                        else if (y > 0)
                            yaxis = _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height + y;
                        else
                            yaxis = _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height / 2;

                        Core.Utilities.PlaceMouseCursor(xaxis, yaxis);
                        Core.Utilities.DoMouseClick();
                    }
                }
                else if (_javacontrolElementFound != null)
                {
                    if (_javacontrolElementFound.Bounds != null)
                    {
                        if (x < 0)
                            xaxis = _javacontrolElementFound.Bounds.X + x;
                        else if (x > 0)
                            xaxis = _javacontrolElementFound.Bounds.X + _javacontrolElementFound.Bounds.Width + x;
                        else
                            xaxis = _javacontrolElementFound.Bounds.X + _javacontrolElementFound.Bounds.Width / 2;

                        if (y < 0)
                            yaxis = _javacontrolElementFound.Bounds.Y + y;
                        else if (y > 0)
                            yaxis = _javacontrolElementFound.Bounds.Y + _javacontrolElementFound.Bounds.Height + y;
                        else
                            yaxis = _javacontrolElementFound.Bounds.Y + _javacontrolElementFound.Bounds.Height / 2;

                        Core.Utilities.PlaceMouseCursor(xaxis, yaxis);
                        Core.Utilities.DoMouseClick();
                    }
                }
                else if (ImageBoundingRectangle != null) //for image based control identification
                {
                    if (highlighter != null)
                    {
                        highlighter.Close();
                        highlighter = null;
                    }
                    //position the mouse at the intended point based on offset
                    if (x < 0)
                        xaxis = ImageBoundingRectangle.X + x;
                    else if (x > 0)
                        xaxis = ImageBoundingRectangle.X + ImageBoundingRectangle.Width + x;
                    else
                        xaxis = ImageBoundingRectangle.X + ImageBoundingRectangle.Width / 2;

                    if (y < 0)
                        yaxis = ImageBoundingRectangle.Y + y;
                    else if (y > 0)
                        yaxis = ImageBoundingRectangle.Y + ImageBoundingRectangle.Height + y;
                    else
                        yaxis = ImageBoundingRectangle.Y + ImageBoundingRectangle.Height / 2;

                    Core.Utilities.PlaceMouseCursor(xaxis, yaxis);
                    Core.Utilities.DoMouseClick();
                }

            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.CLICK);
        }

        public void RightClick()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.RIGHTCLICK))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.RIGHTCLICK);

                if (_controlElementFound != null)
                {
                    if (_controlElementFound.Current.BoundingRectangle != Rect.Empty)
                    {
                        //position the mouse at the center of the rectangle
                        Core.Utilities.PlaceMouseCursor(_controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width / 2, _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height / 2);
                        Core.Utilities.DoMouseRightClick();
                    }
                }
                else if (_javacontrolElementFound != null)
                {
                    if (_javacontrolElementFound.Bounds != null)
                    {
                        //position the mouse at the center of the rectangle
                        Core.Utilities.PlaceMouseCursor((double)(_javacontrolElementFound.Bounds.X + _javacontrolElementFound.Bounds.Width / 2), (double)(_javacontrolElementFound.Bounds.Y + _javacontrolElementFound.Bounds.Height / 2));
                        Core.Utilities.DoMouseRightClick();
                    }
                }
                else if (ImageBoundingRectangle != null) //for image based control identification
                {
                    if (highlighter != null)
                    {
                        highlighter.Close();
                        highlighter = null;
                    }
                    //position the mouse at the center of the rectangle
                    Core.Utilities.PlaceMouseCursor(ImageBoundingRectangle.X + ImageBoundingRectangle.Width / 2, ImageBoundingRectangle.Y + ImageBoundingRectangle.Height / 2);
                    Core.Utilities.DoMouseRightClick();
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.RIGHTCLICK);
        }

        public void DoubleClick()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.DOUBLECLICK))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.DOUBLECLICK);

                //TODO:Rahul
                if (_controlElementFound != null)
                {
                    LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "_controlElementFound", _controlElementFound.ToString());

                    //there is no direct way to invoke double click,
                    //so to invoke double click the approach is do two clicks after a timespan of 150 ms
                    object pattern;
                    if (_controlElementFound.TryGetCurrentPattern(InvokePattern.Pattern, out pattern))
                    {
                        InvokePattern invokePattern = pattern as InvokePattern;
                        invokePattern.Invoke();
                        System.Threading.Thread.Sleep(150);
                        invokePattern.Invoke();
                    }

                    //other approach could be using user32 mouse_event api, 
                    //but there also we need raise twise mouse left click after a time gap, e.g. 150 ms
                    else
                    {
                        //if fails then identify the position of the control using BoundingRectangle and Invoke a Mouse click action
                        if (_controlElementFound.Current.BoundingRectangle != Rect.Empty)
                        {
                            Core.Utilities.PlaceMouseCursor(_controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width / 2, _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height / 2);
                            Core.Utilities.DoMouseClick();
                            System.Threading.Thread.Sleep(150);
                            Core.Utilities.DoMouseClick();
                        }
                    }
                }
                else if (_javacontrolElementFound != null)
                {
                    if (_javacontrolElementFound.Bounds != null)
                    {
                        Core.Utilities.PlaceMouseCursor((double)_javacontrolElementFound.Bounds.X, (double)_javacontrolElementFound.Bounds.Y);
                        Core.Utilities.DoMouseClick();
                        System.Threading.Thread.Sleep(150);
                        Core.Utilities.DoMouseClick();
                    }
                }
                else if (ImageBoundingRectangle != null) //for image based control identification
                {
                    if (highlighter != null)
                    {
                        highlighter.Close();
                        highlighter = null;
                    }
                    //position the mouse at the center of the rectangle
                    Core.Utilities.PlaceMouseCursor(ImageBoundingRectangle.X + ImageBoundingRectangle.Width / 2, ImageBoundingRectangle.Y + ImageBoundingRectangle.Height / 2);
                    Core.Utilities.DoMouseClick();
                    System.Threading.Thread.Sleep(150);
                    Core.Utilities.DoMouseClick();
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.DOUBLECLICK);
        }

        public void Hover()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.HOVER))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.HOVER);

                //TODO:Rahul
                if (ImageBoundingRectangle != null & ImageBoundingRectangle != Rect.Empty & ImageBoundingRectangle != new Rect(0, 0, 0, 0)) //for image based control identification
                {

                    if (highlighter != null)
                    {
                        highlighter.Close();
                        highlighter = null;
                    }
                    //position the mouse at the center of the rectangle
                    Core.Utilities.PlaceMouseCursor(ImageBoundingRectangle.X + ImageBoundingRectangle.Width / 2, ImageBoundingRectangle.Y + ImageBoundingRectangle.Height / 2);

                }
                else if (_controlElementFound != null)
                {
                    ////get the clickable point on the control
                    //System.Windows.Point ctlPt;
                    //if (_controlElementFound.TryGetClickablePoint(out ctlPt))
                    //{
                    //    if (ctlPt != null)
                    //    {
                    //        //Core.Utilities.HoverMouseTo((int)ctlPt.X, (int)ctlPt.Y);
                    //        Core.Utilities.PlaceMouseCursor(ctlPt.X, ctlPt.Y);
                    //    }
                    //}
                    //else
                    //{
                    //    Core.Utilities.PlaceMouseCursor(_controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width / 2, _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height / 2);
                    //}
                    double x = _controlElementFound.Current.BoundingRectangle.X + _controlElementFound.Current.BoundingRectangle.Width / 2;
                    double y = _controlElementFound.Current.BoundingRectangle.Y + _controlElementFound.Current.BoundingRectangle.Height / 2;
                    Core.Utilities.PlaceMouseCursor(x, y);
                }
                else if (_javacontrolElementFound != null)
                {
                    if (_javacontrolElementFound.Bounds != null)
                    {
                        Core.Utilities.PlaceMouseCursor((double)_javacontrolElementFound.Bounds.X, (double)_javacontrolElementFound.Bounds.Y);
                    }
                }
                //else if (ImageBoundingRectangle != null) //for image based control identification
                //{
                //    if (highlighter != null)
                //    {
                //        highlighter.Close();
                //        highlighter = null;
                //    }
                //    //position the mouse at the center of the rectangle
                //    Core.Utilities.PlaceMouseCursor(ImageBoundingRectangle.X + ImageBoundingRectangle.Width / 2, ImageBoundingRectangle.Y + ImageBoundingRectangle.Height / 2);
                //}
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.HOVER);
        }

        public void KeyPress(string keys)
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.KEYPRESS))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "keys", Logging.Constants.PARAMDIRECTION_IN, keys);

                System.Threading.Thread.Sleep(200); //this wait is needed for scenarios with the parent list/combox to get renderred properly
                Core.Utilities.KeyPress(keys);
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.KEYPRESS);
        }

        public void SendText(string text)
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.KEYPRESS))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "keys", Logging.Constants.PARAMDIRECTION_IN, text);

                System.Threading.Thread.Sleep(200); //this wait is needed for scenarios with the parent list/combox to get renderred properly
                Core.Utilities.SendText(text);
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.KEYPRESS);
        }

        /// <summary>
        /// Imitate the key board button press for the provided text and modifier (e.g. ctrl, shift, etc)
        /// </summary>
        /// <param name="text">The alpha nuemeric keys to be pressed</param>
        /// <param name="modifiers">modifier: 0 for shift, 1 for ctrl, 2 for windows, 3 for alt, 4 for caps, 5 for enter, 6 for tab, 7 for back space, 8 for del, 9 for space. NB- in case of CAPS/ENTER, etc first call with text= blank or null and modifiers= 4 or 5, then again call with the required text as neeeded and modifier = -1.
        /// Alternatively use enums- KeyModifier.SHIFT/CTRL/WINDOWS/ALT/CAPITAL/ENTER/TAB/BACKSPACE/DEL/SPACE</param>
        //public void KeyPress(string text, int modifiers)
        //{
        //    if (highlighter != null)
        //    {
        //        highlighter.Close();
        //        highlighter = null;
        //    }
        //    Core.Utilities.KeyPress(modifiers: modifiers, text: text);
        //}

        /// <summary>
        /// Imitate the key board button press for the provided text and modifier (e.g. ctrl, shift, etc)
        /// </summary>
        /// <param name="text">The alpha nuemeric keys to be pressed</param>
        /// <param name="modifiers">Refer to the API user guide for the list of valid key codes</param>     
        public void KeyPress(string text, params int[] modifiers)
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }

            //if (modifiers != null)
            Core.Utilities.KeyPress(modifiers: modifiers, text: text);
            //else
            //    Core.Utilities.KeyPress(text);
        }
        public void SendText(string text, params int[] modifiers)
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Core.Utilities.SendText(text: text);
        }
        public void KeyDown(int modifier)
        {
            Core.Utilities.KeyDown(modifier);
        }
        public void KeyUp(int modifier)
        {
            Core.Utilities.KeyUp(modifier);
        }
        /// <summary>
        /// Raises Mouse left key down. To use this, first position the mouse at the intended location.
        /// </summary>
        public virtual void MouseDown()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Core.Utilities.DoMouseDown();
        }

        /// <summary>
        /// Raises Mouse left key up. To use this, first position the mouse at the intended location.
        /// </summary>
        public virtual void MouseUp()
        {
            if (highlighter != null)
            {
                highlighter.Close();
                highlighter = null;
            }
            Core.Utilities.DoMouseUp();
        }

        //Method to update handle and also search for the automation control again
        public void RefreshControlHandle(IntPtr appWinHandle, IntPtr screenWinHandle)
        {
            if (this.DiscoveryMode == ElementDiscovertyMode.Image)
                return;
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.REFRESHCONTROLHANDLE))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWinHandle", Logging.Constants.PARAMDIRECTION_IN, appWinHandle.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "screenWinHandle", Logging.Constants.PARAMDIRECTION_IN, screenWinHandle.ToString());

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "ApplicationType", ApplicationType.ToLower());

                switch (ApplicationType.ToLower())
                {
                    case "java":
                        //Int32 vmid = 0;
                        //EX_JABHelper.AccessibleTreeItem javaNode = EX_JABHelper.GetComponentTree(appWinHandle, out vmid);
                        FindControl(this.ControlPath, true);
                        break;
                    default:
                        IntPtr winHandleToUse;
                        _appWinHandle = appWinHandle;
                        _screenWinHandle = screenWinHandle;
                        winHandleToUse = ((_screenWinHandle != IntPtr.Zero) && (_screenWinHandle != null)) ? _screenWinHandle : _appWinHandle;
                        if (winHandleToUse != IntPtr.Zero)
                            _HandleElement = AutomationElement.FromHandle(winHandleToUse);
                        FindControl(_condition);
                        break;
                }
                //if (HasChildren)
                //{
                //    foreach (KeyValuePair<string, Control> ctrl in Controls)
                //    {
                //        //Set the reference of the parent automation element
                //        Controls[ctrl.Key].RefreshControlHandle(_appWinHandle, _screenWinHandle);
                //    }
                //}
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.REFRESHCONTROLHANDLE);
        }

        /// <summary>
        /// Bounding rectangle to be populated during Control population
        /// </summary>
        public Rect ImageBoundingRectangle
        {
            get;
            set;
        }

        public ControlImageReference ImageReference
        {
            get;
            set;
        }

        public ElementDiscovertyMode DiscoveryMode { get; set; }

        /// <summary>
        /// To tell if the region is to be highlighted. By default the region is not highlighted
        /// </summary>
        public void Highlight()
        {
            if (ImageBoundingRectangle == null || ImageBoundingRectangle == Rect.Empty || ImageBoundingRectangle == new Rect(0, 0, 0, 0))
            {
                //this may the case of the UI automation based findcontrol
                if (WinControlElementFound != null)
                    ImageBoundingRectangle = WinControlElementFound.Current.BoundingRectangle;
                else if (JavaControlElementFound != null)
                {
                    ImageBoundingRectangle = new Rect(JavaControlElementFound.x, JavaControlElementFound.y, JavaControlElementFound.width, JavaControlElementFound.height);
                }

                //if yet undefined rectangle, then return
                if (ImageBoundingRectangle == null || ImageBoundingRectangle == Rect.Empty || ImageBoundingRectangle == new Rect(0, 0, 0, 0))
                    return;
            }
            _highlightElement = true;
            //open a tranparent form with minimum or no border and then highlight the region/rectangle
            //and then on each event like click, type, etc, close this form
            highlighter = new Views.Highlighter(ImageBoundingRectangle);
            highlighter.ShowDialog(new System.Windows.Forms.Form());
        }

        /// <summary>
        /// Interface to specify custom region/rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetRegion(int x, int y, int width, int height)
        {
            this.ImageBoundingRectangle = new Rect((double)x, (double)y, (double)width, (double)height);
        }

        /// <summary>
        /// Interface to offset/displace control region/rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void OffsetRegion(int x, int y)
        {
            if (ImageBoundingRectangle == null || ImageBoundingRectangle == Rect.Empty || ImageBoundingRectangle == new Rect(0, 0, 0, 0))
            {
                //this may the case of the UI automation based findcontrol
                if (WinControlElementFound != null)
                    ImageBoundingRectangle = WinControlElementFound.Current.BoundingRectangle;
                else if (JavaControlElementFound != null)
                {
                    ImageBoundingRectangle = new Rect(JavaControlElementFound.x, JavaControlElementFound.y, JavaControlElementFound.width, JavaControlElementFound.height);
                }

                //if yet undefined rectangle, then return
                if (ImageBoundingRectangle == null || ImageBoundingRectangle == Rect.Empty || ImageBoundingRectangle == new Rect(0, 0, 0, 0))
                    return;
            }
            this.ImageBoundingRectangle = new Rect(this.ImageBoundingRectangle.X + (double)x, this.ImageBoundingRectangle.Y + (double)y, this.ImageBoundingRectangle.Width, this.ImageBoundingRectangle.Height);
        }

        public void UpdateCondition(string automationId, string automationName, bool isAndCondition = true)
        {
            this.ImageBoundingRectangle = Rect.Empty;
            if (automationId == null)
                automationId = "";
            if (automationName == null)
                automationName = "";
            PropertyCondition typeCond = null;
            PropertyCondition automationNameProp = new PropertyCondition(AutomationElement.NameProperty, automationName);
            PropertyCondition automationIdProp = new PropertyCondition(AutomationElement.AutomationIdProperty, automationId);
            if (_condition != null)
            {
                Condition[] conditions;
                if (_condition.GetType() == typeof(System.Windows.Automation.AndCondition))
                    conditions = ((System.Windows.Automation.AndCondition)(_condition)).GetConditions();
                else
                    conditions = ((System.Windows.Automation.OrCondition)(_condition)).GetConditions();
                if (conditions != null && conditions.Length == 3)
                {
                    for (int i = 0; i < conditions.Count(); i++)
                    {
                        switch (((System.Windows.Automation.PropertyCondition)conditions[i]).Property.ProgrammaticName)
                        {
                            case "AutomationElementIdentifiers.ControlTypeProperty":
                                typeCond = conditions[i] as PropertyCondition;
                                break;
                        }
                    }
                }
            }
            if (typeCond != null)
            {
                if (isAndCondition)
                _condition = new AndCondition(typeCond, automationNameProp, automationIdProp);
                else
                    _condition = new OrCondition(typeCond, automationNameProp, automationIdProp);
            }
        }
        bool _DoTabTracking = true;
        public bool DoTabTracking
        {
            get { return _DoTabTracking; }
            set { _DoTabTracking = value; }
        }

        bool _DoEntityCaching = true;
        public bool DoEntityCaching
        {
            get { return _DoEntityCaching; }
            set { _DoEntityCaching = value; }
        }

        bool _GetAllMatchingControls = false; // the property to be use in case of FinControls i.e. collection of controls matching the provided canonical path
        public bool GetAllMatchingControls
        {
            get { return _GetAllMatchingControls; }
            set { _GetAllMatchingControls = value; }
        }

        public AutomationElementCollection ControlElementCollectionFound
        {
            get { return _controlElementCollectionFound; }
        }

        public AutomationElement ControlElementFound
        {
            get { return _controlElementFound; }
            set { _controlElementFound = value; }
        }

        public void SubscribeToPropertyChangeEvent(PropertyHasChangedEventHandler propertyChangedCallBack)
        {
            if (_controlElementFound != null)
            {
                PropertyHasChanged += propertyChangedCallBack;
                AutomationPropertyChangedEventHandler propertyHandler = new AutomationPropertyChangedEventHandler(PropertyChanged_EventHandler);
                Automation.AddAutomationPropertyChangedEventHandler(_controlElementFound, TreeScope.Children, propertyHandler, AutomationElement.AutomationIdProperty, AutomationElement.NameProperty);
            }
        }

        public void DesubscribeToPropertyChangeEvent(PropertyHasChangedEventHandler propertyChangedCallBack)
        {
            if (_controlElementFound != null)
            {
                PropertyHasChanged -= propertyChangedCallBack;
                AutomationPropertyChangedEventHandler propertyHandler = new AutomationPropertyChangedEventHandler(PropertyChanged_EventHandler);
                Automation.RemoveAutomationPropertyChangedEventHandler(_controlElementFound, propertyHandler);
            }
        }

        private void PropertyChanged_EventHandler(object sender, AutomationPropertyChangedEventArgs e)
        {
            //from here raise the event to be used by the client code
            if (PropertyHasChanged != null)
            {
                Control ctl = new Control(IntPtr.Zero, IntPtr.Zero);
                ctl.ControlElementFound = sender as AutomationElement;
                if (ctl.ControlElementFound != null)
                {
                    ctl.AutomationId = ctl.ControlElementFound.Current.AutomationId;
                    ctl.AutomationName = ctl.ControlElementFound.Current.Name;
                    //ctl.ControlTypeName = ctl.ControlElementFound.Current.ControlType.ProgrammaticName; /// not needed at the property ControlTypeName looks into the under lining automation element
                }
                PropertyHasChanged(new PropertyHasChangedArgs() { Control = ctl, ChangedProperty = e.Property.ProgrammaticName, OldValue = e.OldValue, NewValue = e.NewValue });
            }
        }

        public void SubscribeToStructureChangeEvent(StructureHasChangedEventHandler structureChangedCallBack)
        {
            if (_controlElementFound != null)
            {
                StructureHasChanged += structureChangedCallBack;
                StructureChangedEventHandler structureHandler = new StructureChangedEventHandler(StructureChanged_EventHandler);
                Automation.AddStructureChangedEventHandler(_controlElementFound, TreeScope.Children, structureHandler);
            }
        }

        public void DesubscribeToStructureChangeEvent(StructureHasChangedEventHandler structureChangedCallBack)
        {
            if (_controlElementFound != null)
            {
                StructureHasChanged -= structureChangedCallBack;
                StructureChangedEventHandler structureHandler = new StructureChangedEventHandler(StructureChanged_EventHandler);
                Automation.RemoveStructureChangedEventHandler(_controlElementFound, structureHandler);
            }
        }

        private void StructureChanged_EventHandler(object sender, StructureChangedEventArgs e)
        {
            //from here raise the event to be used by the client code
            if (StructureHasChanged != null)
            {
                Control ctl = new Control(IntPtr.Zero, IntPtr.Zero);
                ctl.ControlElementFound = sender as AutomationElement;
                if (ctl.ControlElementFound != null)
                {
                    ctl.AutomationId = ctl.ControlElementFound.Current.AutomationId;
                    ctl.AutomationName = ctl.ControlElementFound.Current.Name;
                    //ctl.ControlTypeName = ctl.ControlElementFound.Current.ControlType.ProgrammaticName; /// not needed at the property ControlTypeName looks into the under lining automation element
                }
                StructureHasChanged(new StructureHasChangedArgs() { Control = ctl, StructureChangeType = e.StructureChangeType.ToString() });
            }
        }

        public Control(IntPtr appWinHandle, IntPtr screenWinHandle, string automationId, string automationName,
            string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            this.AutomationId = automationId;
            this.AutomationName = automationName;
            this.ControlPath = applicationTreePath;
            this.ApplicationType = applicationType;
            this.FullControlQualifier = fullControlQualifier;
            _appWinHandle = appWinHandle;
            _screenWinHandle = screenWinHandle;

            switch (this.ApplicationType.ToLower())
            {
                case "java":
                    //Control_Reference.JavaControlReference = FindControl(this.ControlPath, true);
                    break;
                default:
                    _condition = PrepareCondition();
                    break;
            }
        }

        private Condition PrepareCondition()
        {
            PropertyCondition automationNameProp = new PropertyCondition(
            AutomationElement.NameProperty,
            this.AutomationName);
            PropertyCondition automationIdProp = new PropertyCondition(
             AutomationElement.AutomationIdProperty,
            this.AutomationId);
            AndCondition controlCondition = new AndCondition(automationNameProp, automationIdProp);
            return controlCondition;
        }

        /// <summary>
        /// Reads text from an image. Image is captured using the input parameters provided
        /// </summary>
        /// <param name="x">Offset from X coordinate on the screen</param>
        /// <param name="y">Offset from Y coordinate on the screen</param>
        /// <param name="height">height of the image to be captured</param>
        /// <param name="width">width of the image to be captured</param>
        /// <param name="filter">filter string consisting of characters that could possibly occur in the image. It can be alphanumeric or especial characters too</param>
        /// <param name="imageResizeCoeff">Coefficient used to resize the original image. A positive coeff will result increasing image size and negative coeff will result in decreasing the image size</param>
        /// <returns>Text in the image captured using input parameters</returns>
        public string ReadTextArea(double offsetX, double offsetY, double height, double width, string filter = "",float imageResizeCoeff=1)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.RIGHTCLICK))
            {
                string text = null;

                double absoluteX;
                double absoluteY;
                double absoluteHeight;
                double absoluteWidth;
                CalculateImageAbsoluteCooridnates(offsetX, offsetY, height, width, out absoluteX, out absoluteY, out absoluteHeight, out absoluteWidth);

                text = TextRecognitionManager.ReadTextArea(absoluteX, absoluteY, absoluteHeight, absoluteWidth, filter,imageResizeCoeff);
                return text;
            }
        }

        private void CalculateImageAbsoluteCooridnates(double offsetX, double offsetY, double height, double width, out double absoluteX, out double absoluteY, out double absoluteHeight, out double absoluteWidth)
        {
            absoluteX = 0;
            absoluteY = 0;
            absoluteHeight = 0;
            absoluteWidth = 0;

            Rect rect = new Rect();

            //If using Image based automation              
            if (ImageBoundingRectangle != null)
            {
                rect = ImageBoundingRectangle;
            }
            //Using win32 based automation
            else if (_controlElementFound != null)
            {
                rect = WinControlElementFound.Current.BoundingRectangle;
            }
            //If using java based automation  
            else if (JavaControlElementFound != null)
            {
                rect = new Rect(JavaControlElementFound.x, JavaControlElementFound.y, JavaControlElementFound.width, JavaControlElementFound.height);
            }

            //Caluclate absolute coordinates
            if (offsetX == 0 && offsetY == 0)
            {
                absoluteX = rect.X;
                absoluteY = rect.Y;
            }
            else if (offsetX == 0 && offsetY > 0)
            {
                absoluteX = rect.X;
                absoluteY = rect.Y + offsetY;
            }
            else if (offsetX > 0 && offsetY == 0)
            {
                absoluteX = rect.X + offsetX;
                absoluteY = rect.Y;
            }
            else
            {
                absoluteX = offsetX + rect.X;
                absoluteY = offsetY + rect.Y;
            }

            if (height == 0 && width == 0)
            {
                absoluteHeight = rect.Height;
                absoluteWidth = rect.Width;
            }
            else if (height > 0 && width == 0)
            {
                absoluteHeight = height;
                absoluteWidth = rect.Width;
            }
            else if (height == 0 && width > 0)
            {
                absoluteHeight = rect.Height;
                absoluteWidth = width;
            }
            else
            {
                absoluteWidth = width;
                absoluteHeight = height;
            }
        }

        /// <summary>
        /// Reads text from an image. Image is captured using the input parameters provided
        /// </summary>
        /// <param name="x">Offset from X coordinate on the screen</param>
        /// <param name="y">Offset from Y coordinate on the screen</param>
        /// <param name="height">height of the image to be captured</param>
        /// <param name="width">width of the image to be captured</param>
        /// <param name="filter">Enum which specifies what type of text is being read</param>
        /// <param name="imageResizeCoeff">Coefficient used to resize the original image. A positive coeff will result increasing image size and negative coeff will result in decreasing the image size</param>
        /// <returns>Text in the image captured using input parameters</returns>
        public string ReadTextArea(double offsetX, double offsetY, double height, double width, TextType filter,float imageResizeCoeff=1)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.RIGHTCLICK))
            {
                string text = null;

                double absoluteX = 0;
                double absoluteY = 0;
                double absoluteHeight = 0;
                double absoluteWidth = 0;

                CalculateImageAbsoluteCooridnates(offsetX, offsetY, height, width, out absoluteX, out absoluteY, out absoluteHeight, out absoluteWidth);

                text = TextRecognitionManager.ReadTextArea(absoluteX, absoluteY, absoluteHeight, absoluteWidth, filter,imageResizeCoeff);
                return text;
            }
        }

        /// <summary>
        ///  Applicable for win32 based approach. Gets the collection of child controls (if any) of all types under the current control
        /// </summary>
        /// <returns>collection of child controls</returns>
        public List<Control> GetChildren()
        {
            List<Control> children = null;
            if (this.ControlElementFound != null)
            {
                var ctrls = this.ControlElementFound.FindAll(TreeScope.Children, Condition.TrueCondition);
                if (ctrls != null && ctrls.Count > 0)
                {
                    children = new List<Control>();
                    for (int i = 0; i < ctrls.Count; i++)
                    {
                        children.Add(new Control(IntPtr.Zero, IntPtr.Zero) { ControlElementFound = ctrls[i] });
                    }
                }
            }
            return children;
        }

        /// <summary>
        ///  Applicable for win32 based approach. Gets the collection of child controls (if any) of the asked type under the current control. 
        /// For getting child control of type table row and table cell, refer to the interfaces exposed by table and table row repectively.
        /// </summary>
        /// <param name="type">the type of child control</param>
        /// <returns>collection of child controls</returns>
        public List<Control> GetChildren(IAPControlType type)
        {
            List<Control> children = null;
            if (this.ControlElementFound != null)
            {
                //prepare the control type condition
                Condition condition = Condition.TrueCondition;
                switch (type)
                {
                    case IAPControlType.Button:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Button);
                        break;
                    case IAPControlType.CheckBox:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.CheckBox);
                        break;
                    case IAPControlType.ComboBox:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.ComboBox);
                        break;
                    case IAPControlType.Custom:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Custom);
                        break;
                    case IAPControlType.DataGrid:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.DataGrid);
                        break;
                    case IAPControlType.DataItem:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.DataItem);
                        break;
                    case IAPControlType.Document:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Document);
                        break;
                    case IAPControlType.Edit:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Edit);
                        break;
                    case IAPControlType.HyperLink:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Hyperlink);
                        break;
                    case IAPControlType.Image:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Image);
                        break;
                    case IAPControlType.List:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.List);
                        break;
                    case IAPControlType.ListItem:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.ListItem);
                        break;
                    case IAPControlType.Menu:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Menu);
                        break;
                    case IAPControlType.RadioButton:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.RadioButton);
                        break;
                    case IAPControlType.Tab:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Tab);
                        break;
                    case IAPControlType.TabItem:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.TabItem);
                        break;
                    case IAPControlType.Table:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Table);
                        break;
                    case IAPControlType.TextBox:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Text);
                        break;
                    case IAPControlType.Tree:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.Tree);
                        break;
                    case IAPControlType.TreeItem:
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, System.Windows.Automation.ControlType.TreeItem);
                        break;
                    //case IAPControlType.TableCell:
                    //use the get rows interfaces exposed by table type of control
                    //case IAPControlType.TableRow:
                    //use the get cells kind of interfaces exposed by the table row type of control
                }

                var ctrls = this.ControlElementFound.FindAll(TreeScope.Children, condition);
                if (ctrls != null && ctrls.Count > 0)
                {
                    children = new List<Control>();
                    for (int i = 0; i < ctrls.Count; i++)
                    {
                        children.Add(new Control(IntPtr.Zero, IntPtr.Zero) { ControlElementFound = ctrls[i] });
                    }
                }
            }
            return children;
        }
    }

    public class ControlReferenceBase
    {
        public AutomationElement WinControlReference;
        public EX_JABHelper.AccessibleTreeItem JavaControlReference;

        //public ControlReferenceBase(AutomationElement winControl, EX_JABHelper.AccessibleTreeItem javaControl)
        //{
        //    WinControlReference = winControl;
        //    JavaControlReference = javaControl;
        //}

        public ControlReferenceBase() { }
    }

    public class ControlImageReference
    {
        public List<ControlStateReference> SupportedStates { get; set; }
        public string CurrentState { get; set; }
        public System.Windows.Rect CurrentBoundingRectangle { get; set; }
    }

    public class ControlStateReference
    {
        public string State { get; set; }
        public string ImagePath { get; set; }
    }

    public enum ElementDiscovertyMode
    {
        Image, //should always be the first member otherwsie image based approcah may fail
        API,
        APIAndImage,
        None
    }

    public enum IAPControlType
    {
        Button,
        CheckBox,
        ComboBox,
        Custom,
        DataGrid,
        DataItem,
        Document,
        Edit,
        HyperLink,
        Image,
        List,
        ListItem,
        Menu,
        RadioButton,
        Tab,
        TabItem,
        Table,
        TableCell,
        TableRow,
        TextBox,
        Tree,
        TreeItem
    }
}
