/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.WEM.Infrastructure.Common;


namespace Infosys.ATR.WinUIAutomationRuntimeWrapper
{

    public class Screen
    {
        IntPtr _winHandle;
        IntPtr _appWindowHandle;
        int _winHandleId;
        private string _screenName;
        private string _automationControlName;
        private string _automationClassName;
        Dictionary<string, Control> _controls;

        private string className = "Screen";

        public Screen()
        {
        }

        public Screen(string screenNameInput, IntPtr appWindowHandle, string className="")
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.SCREEN))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "screenNameInput", Logging.Constants.PARAMDIRECTION_IN, screenNameInput);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWindowHandle", Logging.Constants.PARAMDIRECTION_IN, appWindowHandle.ToString());

                _automationControlName = screenNameInput;
                _automationClassName = className;
                _appWindowHandle = appWindowHandle;
                _winHandle = GetWindowsHandle();
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.SCREEN);
        }

        public Screen(int winHandleId, IntPtr appWindowHandle)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.SCREEN))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "winHandleId", Logging.Constants.PARAMDIRECTION_IN, winHandleId.ToString());
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWindowHandle", Logging.Constants.PARAMDIRECTION_IN, appWindowHandle.ToString());

                _winHandleId = winHandleId;
                _appWindowHandle = appWindowHandle;
                _winHandle = GetWindowsHandle();
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.SCREEN);
        }
        public string Name
        {
            get { return _screenName; }
            set { _screenName = value; }
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

        public IntPtr WindowsHandle
        {
            get { return _winHandle; }
        }

        public Dictionary<string, Control> Controls
        {
            get { return _controls; }
            set { _controls = value; }
        }

        public void RefreshScreenHandle(IntPtr appWindowHandle)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.REFRESHSCREENHANDLE))
            {
                //in param
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_PARAMETERS, LogHandler.Layer.Business, "appWindowHandle", Logging.Constants.PARAMDIRECTION_IN, appWindowHandle.ToString());

                _appWindowHandle = appWindowHandle;
                _winHandle = GetWindowsHandle();

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "_controls", _controls.Count.ToString());

                if (_controls.Count > 0)
                {
                    foreach (KeyValuePair<string, Control> ctrl in _controls)
                    {
                        //Set the reference of the parent automation element
                        _controls[ctrl.Key].RefreshControlHandle(_appWindowHandle, _winHandle);
                    }
                }
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.REFRESHSCREENHANDLE);
        }


        public void RefreshScreenHandle()
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, Guid.Empty, className, Logging.Constants.REFRESHSCREENHANDLE))
            {
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Business, className, Logging.Constants.REFRESHSCREENHANDLE);
                GetWindowsHandle();
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_EXIT, LogHandler.Layer.Business, className, Logging.Constants.REFRESHSCREENHANDLE);
        }

        public IntPtr GetWindowsHandle()
        {
            if (!string.IsNullOrEmpty(_automationControlName) && !string.IsNullOrEmpty(_automationClassName))
            {
                _winHandle = Core.Utilities.GetWindowHandle(_automationClassName, _automationControlName);
            }
            else if (!string.IsNullOrEmpty(_automationControlName))
            {
                _winHandle = Core.Utilities.GetWindowHandle(null, _automationControlName);
            }
            else if (!string.IsNullOrEmpty(_automationClassName))
            {
                _winHandle = Core.Utilities.GetWindowHandle(_automationClassName, null);
            }

            if (_winHandleId > 0)
            {
                _winHandle = new IntPtr(Convert.ToInt32(_winHandleId));
            }

            return _winHandle;
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
    }
}
