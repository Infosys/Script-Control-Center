/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Infosys.WEM.Infrastructure.Common;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Reflection;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper
{
    //Add the additional Application object properties
    public class Application
    {

        [DllImport("user32.dll", CharSet=CharSet.Auto,ExactSpelling=true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        private IntPtr _winHandle;
        private int _processId;
        private string _appName;
        private string _automationControlName;
        private string _automationClassName;
        private string _appType;
        private string _appLocationPath;
        private string _uiFwk;
        private Dictionary<string, Screen> _screens;
        Dictionary<string, Control> _controls;
        private bool appLaunched = true;
        private string _webBrowser;
        private string _webBrowserVersion;
        private int _getWindowsHandleTimeOut = 3; //in seconds, the duration for which it would be tried to get the windows handlle based on the process id or application control name

        private string className = "Application";

        //events for client developers
        #region Event- PropertyHasChanged
        public class PropertyHasChangedArgs : EventArgs
        {
            public Control Control { get; set; }
            public string ChangedProperty{ get; set; }
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

        /// <summary>
        /// Application Constructor
        /// </summary>
        /// <param name="applicationName">Name of Application</param>
        /// <param name="launchedApp">Boolean value stating if the application should be launched or not</param>
        /// <param name="applicationClassName">Optional, to set the application class name. e.g. to be used in case of IE context menu</param>
        public Application(string applicationName, bool launchedApp= false, string applicationClassName="")
        {
            Core.Utilities.SetDLLsPath();
            appLaunched = launchedApp;
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.APPLICATION))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "applicationName", Logging.Constants.PARAMDIRECTION_IN, applicationName);

                _automationControlName = applicationName;
                _automationClassName = applicationClassName;
                _winHandle = GetWindowsHandle();               
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.APPLICATION);
        }
        /// <summary>
        /// Application Constructor
        /// </summary>
        /// <param name="pid">Process Id of the application</param>
        public Application(int pid)
        {
            Core.Utilities.WriteLog("instantiating application with process id");
            appLaunched = true;
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.APPLICATION))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "pid", Logging.Constants.PARAMDIRECTION_IN, pid.ToString());

                _processId = pid;
                _winHandle = GetWindowsHandle();                
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.APPLICATION);
        }


        public string Name
        {
            get { return _appName; }
            set { _appName = value; }

        }
        public string AutomationName
        {
            get { return _automationControlName; }
            set { _automationControlName = value; }
        }

        public string AutomationClassName
        {
            get { return _automationClassName; }
            set { _automationClassName = value; }
        }

        public int ProcessId
        {
            get { return _processId; }

        }

        public IntPtr WindowsHandle
        {
            get { return _winHandle; }

        }

        public string AppType
        {
            get { return _appType; }
            set { _appType = value; }

        }

        public string AppLocationPath
        {
            get { return _appLocationPath; }
            set { _appLocationPath = value; }

        }

        public string UIFwk
        {
            get { return _uiFwk; }
            set { _uiFwk = value; }
        }

        public Dictionary<string, Control> Controls
        {
            get { return _controls; }
            set { _controls = value; }
        }

        public Dictionary<string, Screen> Screens
        {
            get { return _screens; }
            set { _screens = value; }
        }

        public string WebBrowser
        {
            get { return _webBrowser; }
            set { _webBrowser = value; }

        }

        public string WebBrowserVersion
        {
            get { return _webBrowserVersion; }
            set { _webBrowserVersion = value; }

        }

        public bool ShowAppStartWaitBox { get; set; }

        /// <summary>
        /// This method is used to set the reference of the parent automation element.
        /// </summary>
        public void RefreshAppHandle()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.REFRESHAPPHANDLE))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.REFRESHAPPHANDLE);

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.GETWINDOWSHANDLE);
                GetWindowsHandle();
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.GETWINDOWSHANDLE);

                if (_screens.Count > 0)
                {
                    foreach (KeyValuePair<string, Screen> scrn in _screens)
                    {
                        //Set the reference of the parent automation element
                        _screens[scrn.Key].RefreshScreenHandle(_winHandle);
                    }
                }               
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.REFRESHAPPHANDLE);
        }
        
        /// <summary>
        /// This method is ised to return Window handle for the current active application.
        /// </summary>
        /// <returns></returns>
        public IntPtr GetWindowsHandle()
        {
            Core.Utilities.WriteLog("getting application handle");
            if (string.IsNullOrEmpty(_automationControlName) && _processId == 0)
            {
                Core.Utilities.WriteLog("application control name is blank and also the process id is zero hence returning zero windows handle");
                return IntPtr.Zero; //error
            }

            //in the below while loop also include the _getWindowsHandleTimeOut
            DateTime startTime = DateTime.Now;
            while (_winHandle == IntPtr.Zero && (System.DateTime.Now - startTime).TotalMilliseconds <= _getWindowsHandleTimeOut * 1000)
            {
                if (!string.IsNullOrEmpty(_automationControlName) && !string.IsNullOrEmpty(_automationClassName))
                {
                    _winHandle = Core.Utilities.GetWindowHandle(_automationClassName, _automationControlName);
                    if(_winHandle!= IntPtr.Zero)
                        _processId = Core.Utilities.GetProcessId(_winHandle);
                }
                else if (!string.IsNullOrEmpty(_automationControlName))
                {
                    _winHandle = Core.Utilities.GetWindowHandle(null, _automationControlName);
                    if (_winHandle != IntPtr.Zero)
                        _processId = Core.Utilities.GetProcessId(_winHandle);
                }
                else if (!string.IsNullOrEmpty(_automationClassName))
                {
                    _winHandle = Core.Utilities.GetWindowHandle(_automationClassName, null);
                    if (_winHandle != IntPtr.Zero)
                        _processId = Core.Utilities.GetProcessId(_winHandle);
                }
                else
                {
                    Process process = Process.GetProcessById(ProcessId);
                    if(process != null)
                    {
                        _winHandle = process.MainWindowHandle;
                        _automationControlName = process.MainWindowTitle;
                    }
                }

            }
            if(_winHandle!= IntPtr.Zero)
                Core.Utilities.WriteLog("application handle is valid");
            else
                Core.Utilities.WriteLog("application handle is invalid");
            return _winHandle;
        }

        public void SubscribeToPropertyChangeEvent(PropertyHasChangedEventHandler propertyChangedCallBack)
        {
            if (_winHandle != IntPtr.Zero && _winHandle != null)
            {
                //get the automation element from the _winHandle and then register for property and structure changed handler
                AutomationElement appAutoElement = AutomationElement.FromHandle(_winHandle);
                if (appAutoElement != null)
                {
                    PropertyHasChanged += propertyChangedCallBack;
                    AutomationPropertyChangedEventHandler propertyHandler = new AutomationPropertyChangedEventHandler(PropertyChanged_EventHandler);
                    Automation.AddAutomationPropertyChangedEventHandler(appAutoElement, TreeScope.Children, propertyHandler, AutomationElement.AutomationIdProperty, AutomationElement.NameProperty);
                }
            }
        }

        public void DesubscribeToPropertyChangeEvent(PropertyHasChangedEventHandler propertyChangedCallBack)
        {
            if (_winHandle != IntPtr.Zero && _winHandle != null)
            {
                //get the automation element from the _winHandle and then register for property and structure changed handler
                AutomationElement appAutoElement = AutomationElement.FromHandle(_winHandle);
                if (appAutoElement != null)
                {
                    PropertyHasChanged -= propertyChangedCallBack;
                    AutomationPropertyChangedEventHandler propertyHandler = new AutomationPropertyChangedEventHandler(PropertyChanged_EventHandler);
                    Automation.RemoveAutomationPropertyChangedEventHandler(appAutoElement, propertyHandler);
                }
            }
        }

        public void SubscribeToStructureChangeEvent(StructureHasChangedEventHandler structureChangedCallBack)
        {
            if (_winHandle != IntPtr.Zero && _winHandle != null)
            {
                //get the automation element from the _winHandle and then register for property and structure changed handler
                AutomationElement appAutoElement = AutomationElement.FromHandle(_winHandle);
                if (appAutoElement != null)
                {
                    StructureHasChanged += structureChangedCallBack;
                    StructureChangedEventHandler structureHandler = new StructureChangedEventHandler(StructureChanged_EventHandler);
                    Automation.AddStructureChangedEventHandler(appAutoElement, TreeScope.Children, structureHandler);
                }
            }
        }

        public void DesubscribeToStructureChangeEvent(StructureHasChangedEventHandler structureChangedCallBack)
        {
            if (_winHandle != IntPtr.Zero && _winHandle != null)
            {
                //get the automation element from the _winHandle and then register for property and structure changed handler
                AutomationElement appAutoElement = AutomationElement.FromHandle(_winHandle);
                if (appAutoElement != null)
                {
                    StructureHasChanged -= structureChangedCallBack;
                    StructureChangedEventHandler structureHandler = new StructureChangedEventHandler(StructureChanged_EventHandler);
                    Automation.RemoveStructureChangedEventHandler(appAutoElement, structureHandler);
                }
            }
        }

        private void PropertyChanged_EventHandler(object sender, AutomationPropertyChangedEventArgs  e)
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
                PropertyHasChanged(new PropertyHasChangedArgs() { Control = ctl, ChangedProperty = e.Property.ProgrammaticName, OldValue= e.OldValue, NewValue = e.NewValue });
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

        /// <summary>
        /// Imitate the key board button press for the provided text and modifier (e.g. ctrl, shift, etc)
        /// </summary>
        /// <param name="text">The alpha nuemeric keys to be pressed</param>
        /// <param name="modifiers">modifier: 0 for shift, 1 for ctrl, 2 for windows, 3 for alt, 4 for caps, 5 for enter, 6 for tab, 7 for back space, 8 for del, 9 for space. NB- in case of CAPS/ENTER, etc first call with text= blank or null and modifiers= 4 or 5, then again call with the required text as neeeded and modifier = -1.
        /// Alternatively use enums- KeyModifier.SHIFT/CTRL/WINDOWS/ALT/CAPITAL/TAB/BACKSPACE/DEL/SPACE</param>
        //public void KeyPress(string text, int modifiers)
        //{
        //    Core.Utilities.KeyPress(modifiers: modifiers, text: text);
        //}

        /// <summary>
        /// Imitate the key board button press for the provided text and modifier (e.g. ctrl, shift, etc)
        /// </summary>
        /// <param name="text">The alpha nuemeric keys to be pressed</param>
        /// <param name="modifiers">Refer to the API user guide for the list of valid key codes</param>     
        public void KeyPress(string text, params int[] modifiers)
        {
            if (modifiers != null)
                Core.Utilities.KeyPress(modifiers: modifiers, text: text);
            else
                Core.Utilities.KeyPress(text);
        }
        public void SendText(string text)
        {
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
        /// This method is used to start the application.
        /// </summary>
        /// <returns>true- if has access to the windows handle of the application started, else false</returns>
        public bool StartApp()
        {
            bool startedSuccwessfully = true;
            _processId = Core.Utilities.LaunchApplication(this.AppLocationPath,AppType, this.WebBrowser, this.ShowAppStartWaitBox);
            appLaunched = true;
            _winHandle = GetWindowsHandle();
            if (!IsAvailable)
                startedSuccwessfully = false;
            return startedSuccwessfully;
        }
        /// <summary>
        /// This method is used to start the application based on appArguement.
        /// </summary>
        /// <param name="appArguement">Argument to be passed</param>
        /// /// <returns>true- if has access to the windows handle of the application started, else false</returns>
        public bool StartApp(string appArguement)
        {
            bool startedSuccwessfully = true;
            _processId = Core.Utilities.LaunchApplication(this.AppLocationPath,this.AppType, this.WebBrowser, this.ShowAppStartWaitBox, appArguement);
            appLaunched = true;
            _winHandle = GetWindowsHandle();
            if (!IsAvailable)
                startedSuccwessfully = false;
            return startedSuccwessfully;
        }
        /// <summary>
        /// This method is used to close the application based on _processId value.
        /// </summary>
        public void CloseApp()
        {
            if (_processId > 0)
            {
                Process process = Process.GetProcessById(_processId);
                process.Kill();
            }
        }
        /// <summary>
        /// This method is used to set focus to the application based on _processId value.
        /// </summary>
        public void SetFocus()
        {
            if (_processId > 0)
            {
                Process process = Process.GetProcessById(_processId);
                SetFocus(new HandleRef(null, process.MainWindowHandle));
            }
        }

        public bool AppLaunched
        {
            set
            {
                appLaunched = value;
            }
        }

        public bool IsAvailable
        {
            get
            {
                if (WindowsHandle == IntPtr.Zero || WindowsHandle == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// In seconds, the duration for which it would be tried to get the windows handlle based on the process id or application control name
        /// </summary>
        public int TimeOut
        {
            set
            {
                if (value > 0)
                    _getWindowsHandleTimeOut = value;
            }
        }
    }
}
