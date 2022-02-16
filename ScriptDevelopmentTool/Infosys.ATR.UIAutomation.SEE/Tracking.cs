/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHDocVw;
using Infosys.ATR.UIAutomation.UserActivityMonitor;
using Infosys.ATR.UIAutomation.Entities;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Threading;
using System.Diagnostics;
using mshtml;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Drawing.Imaging;
using System.Drawing;

namespace Infosys.ATR.UIAutomation.SEE
{
    public static class Tracking
    {
        public static bool trakingMouseClick = false, trakingMouseDoubleClick = false, trakingKeyboard = false, settingToForegroundDone = false, lastUrlChanged= false;
        static int toolProcessId = 0, lastProcessId, trackTaskOrder = 1, trackKeyCount = 0, maxToBeCaptured = 300;
        static string keysPressed = "", lastWebsiteUrl, startingWebsite = "";
        //static List<string> htmlBrowsers = new List<string>() { "iexplore.exe", "chrome.exe" };
        static List<string> htmlBrowsers = new List<string>() { "chrome.exe" };
        static string lastUrlVisited = "";
        static string groupScriptId, windowTitle;
        static int _height;
        static int _width;
        static string _taskId;
        static string _screenId;

        public static IntPtr currentHandle;
        class PatternAndState
        {
            public string Pattern { get; set; }
            public string State { get; set; }
        }

        //user32 interfaces
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern IntPtr SetForegroundWindowNative(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        //public properties
        public static System.Windows.Forms.WebBrowser Browser;
        public static int Width
        {
            get { return Tracking._width; }
            set { Tracking._width = value; }
        }
        public static int Height
        {
            get { return Tracking._height; }
            set { Tracking._height = value; }
        }
        public static string TaskId
        {
            get { return Tracking._taskId; }
            set { Tracking._taskId = value; }
        }
        public static string ScreenId
        {
            get { return Tracking._screenId; }
            set { Tracking._screenId = value; }
        }

        //event args
        public class BaseTaskArguements : EventArgs
        {
            public bool IsDifferentActivity { get; set; }
            public int ProcessId { get; set; }
            public bool IsHTMLContent { get; set; }
            public string AdditionalInfo { get; set; }
            public string ApplicationName { get; set; }
            public string ApplicationType { get; set; }
            public string ModuleName { get; set; }
            public string FileName { get; set; }
            public Task Task { get; set; }
        }

        public class KeyBoardKeyPresedEventArgs : BaseTaskArguements
        {
            //public bool IsDifferentActivity { get; set; }
            //public int ProcessId { get; set; }
            //public bool IsHTMLContent { get; set; }
            //public string ParentActivity { get; set; }
            //public string ApplicationName { get; set; }
            //public string ModuleName { get; set; }
            //public string FileName { get; set; }
            //public Task Task { get; set; }
        }
        public class KeyBoardFlushCaturedEventArgs : EventArgs
        {
            public bool IsDifferentActivity { get; set; }
            public string ParentActivity { get; set; }
            public Task Task { get; set; }
        }
        public class MouseClickedEventArgs : BaseTaskArguements
        {
            //public bool IsDifferentActivity { get; set; }
            //public int ProcessId { get; set; }
            //public bool IsHTMLContent { get; set; }
            //public string ParentActivity { get; set; }
            //public string ApplicationName { get; set; }
            //public string ModuleName { get; set; }
            //public string FileName { get; set; }
            //public Task Task { get; set; }
        }

        public class WaitEventArgs : BaseTaskArguements
        {

        }

        //events and associated delegates
        public delegate void KeyBoardFlushCaturedEventHandler(KeyBoardFlushCaturedEventArgs e);
        public static event KeyBoardFlushCaturedEventHandler KeyBoardFlushCatured;
        public delegate void KeyBoardKeyPresedEventHandler(KeyBoardKeyPresedEventArgs e);
        public static event KeyBoardKeyPresedEventHandler KeyBoardKeyPresed;
        public delegate void MouseClickedEventHandler(MouseClickedEventArgs e);
        public static event MouseClickedEventHandler MouseClicked;

        public delegate void IsSendKeyHandler(bool e);
        public static event IsSendKeyHandler IsSendKey; 


        public static void RecordMouseClick(bool start = true, int height = 0, int width = 0)
        {
            if (start && !trakingMouseClick)
            {
                trakingMouseClick = true;
                GlobalEventHandler.MouseDownEvents += new MouseEventHandler(HookManager_MouseDown);
                _height = height;
                _width = width;
            }
            else
            {
                trakingMouseClick = false;
                GlobalEventHandler.MouseDownEvents -= new MouseEventHandler(HookManager_MouseDown);
                //the below assignment is needed as once the recordng is paused/stopped, current window may not be on the top
                settingToForegroundDone = false;
            }

            //get the current tool's process id
            if (toolProcessId == 0)
            {
                toolProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
                currentHandle = Process.GetCurrentProcess().MainWindowHandle;
            }
        }

        public static void RecordMouseDoubleClick(bool start = true, int height = 0, int width = 0)
        {
            if (start && !trakingMouseDoubleClick)
            {
                trakingMouseDoubleClick = true;
                GlobalEventHandler.MouseDoubleClickedEvent += new MouseEventHandler(HookManager_MouseDoubleClick);
                _height = height;
                _width = width;
            }
            else
            {
                trakingMouseDoubleClick = false;
                GlobalEventHandler.MouseDoubleClickedEvent -= new MouseEventHandler(HookManager_MouseDoubleClick);
                //the below assignment is needed as once the recordng is paused/stopped, current window may not be on the top
                settingToForegroundDone = false;
            }

            //get the current tool's process id
            if (toolProcessId == 0)
            {
                toolProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
                currentHandle = Process.GetCurrentProcess().MainWindowHandle;
            }
        }

        static void HookManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (trakingMouseDoubleClick)
            {
                groupScriptId = ""; //to be used to group the text field and the associated keys pressed
                AutomationElement element = ElementFromCursor(e.X, e.Y, e.Button, true);
            }
        }

        static void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            //get the element details and raise event
            //return information:
            //1. if the element is from the same process or different than the last instance
            //2. if the underlying process is of browser type e.g. iexplore.exe, chrome.exe, etc
            //if (!isHTMLApp)
            //{
            if (trakingMouseClick)
            {
                groupScriptId = ""; //to be used to group the text field and the associated keys pressed
                AutomationElement element = ElementFromCursor(e.X, e.Y, e.Button);
            }

            //}
            //else if (browser != null)
            //{
            //    UpdateHTMLElementDetail(e.X, e.Y);
            //}

        }

        static AutomationElement ElementFromCursor(int X, int Y, MouseButtons button, bool isDoubleClick = false)
        {
            // Convert mouse position to System.Windows.Point.
            AutomationElement element = null;
            //putting this code under a different thread because of the exception:
            //An outgoing call cannot be made since the application is dispatching an input-synchronous call. (Exception from HRESULT: 0x8001010D (RPC_E_CANTCALLOUT_ININPUTSYNCCALL)).
            //ref.- http://stackoverflow.com/questions/18670437/com-exception-was-unhandled-in-c-sharp, http://go4answers.webhost4life.com/Example/exception-observed-while-getting-58920.aspx
            Thread thread = new Thread(() =>
            {
                //System.Windows.Point point = new System.Windows.Point(X, Y);
                CacheRequest cacheRequest = CreateCacheRequest();                
                // Activate the request.
                cacheRequest.Push();
                // Obtain an element and cache the requested items.
                try
                {
                    element = AutomationElement.FromPoint(new System.Windows.Point(X, Y));
                }
                catch { }

                // At this point, you could call another method that creates a CacheRequest and calls Push/Pop.
                // While that method was retrieving automation elements, the CacheRequest set in this method 
                // would not be active. 

                // Deactivate the request.
                cacheRequest.Pop();

                if (element != null)
                    UpdateElementDetails(element, button, X, Y, isDoubleClick, cacheRequest);
                            
            });
            //changing the apartment state to STA as for certain control like webbrowser (activex) getting error
            //that activex control can be used in a SAT only
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return element;
        }

        private static int GetXoffset(HtmlElement el)
        {
            //get element pos
            int xPos = el.OffsetRectangle.Left;

            //get the parents pos
            HtmlElement tempEl = el.OffsetParent;
            while (tempEl != null)
            {
                xPos += tempEl.OffsetRectangle.Left;
                tempEl = tempEl.OffsetParent;
            }

            return xPos;
        }

        private static int GetYoffset(HtmlElement el)
        {
            //get element pos
            int yPos = el.OffsetRectangle.Top;

            //get the parents pos
            HtmlElement tempEl = el.OffsetParent;
            while (tempEl != null)
            {
                yPos += tempEl.OffsetRectangle.Top;
                tempEl = tempEl.OffsetParent;
            }

            return yPos;
        }

        static string GetWindowTitle(int processID)
        {
            IntPtr handle = IntPtr.Zero;
            //first get the active window
            if (!settingToForegroundDone)
            {
                //this kind of approach is needed as fr the first time the title of the 
                //recording window is getting assigned as it is made te stay on the top by default.
                handle = Process.GetProcessById(processID).MainWindowHandle;
                settingToForegroundDone = true;
            }
            else
                handle = GetForegroundWindow();

            //then get lenght of the window title
            int length = GetWindowTextLength(handle);

            //now get the window title
            StringBuilder sbTitle = new StringBuilder(length + 1);
            GetWindowText(handle, sbTitle, length + 1);

            return sbTitle.ToString();
        }

        static void UpdateHTMLElementDetail(AutomationElement element, MouseButtons button, int X = 0, int Y = 0, bool isDoubleClick = false)
        {
            MouseClickedEventArgs elementClicked = new MouseClickedEventArgs();
            elementClicked.IsHTMLContent = true;
            elementClicked.AdditionalInfo = "If the Element extracted is of Type IFRAME, then the underlying element clicked might not have got extracted if it is fetched from a different domain. Such ations need to be done manually.";

            if (Browser != null && !Browser.IsDisposed)
            {
                if (Browser.InvokeRequired)
                {
                    Browser.BeginInvoke(new Action<AutomationElement, MouseButtons, int, int, bool>(UpdateHTMLElementDetail), new object[] { element, button, X, Y, isDoubleClick });
                    return;
                }

                //keep the record of the starting website, to be used to define parent/child activities
                if (string.IsNullOrEmpty(startingWebsite))
                    startingWebsite = Browser.Url.ToString();
                else
                {
                    //check if the url root dns is changed, if so then accordingly change the 
                    //startingWebsite to the url browser url
                    if (Browser.Url != null && IsURLHostChanged(startingWebsite, Browser.Url.ToString()))
                        startingWebsite = Browser.Url.ToString();
                }

                System.Drawing.Point screenPt = new System.Drawing.Point(X, Y);
                System.Drawing.Point browserPt = Browser.PointToClient(screenPt);
                //create the point expected to be inside the webbrowser bound with respect to the application hosting the web browser
                System.Drawing.Point pt = new Point(browserPt.X + Browser.Bounds.X, browserPt.Y + Browser.Bounds.Y);

                //check if the mouse position inside the boundary of the container browser control
                //removing the check as sometime webbrowser doesnt consider this point inside its bound
                if (Browser.Bounds.Contains(pt))
                {
                    if (Browser.Document == null)
                        return;
                    HtmlElement htmlElement = Browser.Document.GetElementFromPoint(browserPt);
                    if (htmlElement == null)
                        return;
                    //get the domlelement to get some more element identifiers
                    IHTMLElement htmlElement2 = null;

                    //check if the htmlelement is of type iframe
                    string iframeXpath = "";
                    if (htmlElement.TagName.ToLower() == "iframe")
                    {
                        iframeXpath = GetXPath(htmlElement);
                        //get the X and Y with respect to the iframe
                        int tempx = X - GetXoffset(htmlElement) - Browser.PointToScreen(System.Drawing.Point.Empty).X;
                        int tempy = Y - GetYoffset(htmlElement) - Browser.PointToScreen(System.Drawing.Point.Empty).Y;

                        foreach (HtmlWindow frame in Browser.Document.Window.Frames)
                        {
                            try
                            {
                                if (frame.WindowFrameElement == htmlElement)
                                {
                                    HtmlElement tempel = frame.Document.GetElementFromPoint(new System.Drawing.Point(tempx, tempy));
                                    if (tempel != null)
                                        htmlElement = tempel;
                                    break;
                                }
                            }
                            catch (System.UnauthorizedAccessException)
                            {
                                //when the source of the iframe belongs to a different domain
                                //actions on such part has to be done manually
                            }
                        }
                    }

                    htmlElement2 = htmlElement.DomElement as IHTMLElement;

                    //if (htmlElement2 != null)
                    //{
                    if (htmlElement.Document.Url.ToString() != lastWebsiteUrl)
                    {
                        elementClicked.IsDifferentActivity = true;
                        lastWebsiteUrl = htmlElement.Document.Url.ToString();
                    }
                    elementClicked.Task = new Task();
                    if (string.IsNullOrEmpty(groupScriptId))
                    {
                        groupScriptId = System.Guid.NewGuid().ToString();
                        elementClicked.Task.GroupScriptId = groupScriptId;
                    }
                    elementClicked.Task.Order = trackTaskOrder++;
                    elementClicked.Task.ControlOnApplication = ApplicationTypes.WebApplication;
                    elementClicked.ProcessId = element.Cached.ProcessId;
                    elementClicked.Task.ControlId = htmlElement.Id;
                    //elementClicked.Task.ControlId = htmlElement2.id;
                    if (htmlElement2 != null)
                        elementClicked.Task.SourceIndex = htmlElement2.sourceIndex;
                    elementClicked.Task.XPath = iframeXpath + GetXPath(htmlElement);
                    //elementClicked.Task.XPath = GetXPath(htmlElement2);
                    elementClicked.Task.ControlName = htmlElement.Name;
                    //elementClicked.Task.ControlName = htmlElement2.className;
                    try
                    {
                        elementClicked.Task.ControlType = htmlElement.GetAttribute("type");
                        //check if the type attribute is missing, then asign the tag attribute
                        if (string.IsNullOrEmpty(elementClicked.Task.ControlType))
                            elementClicked.Task.ControlType = htmlElement.TagName.ToLower();
                    }
                    catch (System.UnauthorizedAccessException)
                    {
                        //when the source of the iframe belongs to a different domain
                        //"type" cant be fetched
                    }
                    //if (htmlElement2.getAttribute("type").GetType() != typeof(System.DBNull))
                    //    elementClicked.Task.ControlType = htmlElement2.getAttribute("type");

                    string eventDesc = "";
                    switch (button)
                    {
                        case MouseButtons.Left:
                            if (isDoubleClick)
                            {
                                elementClicked.Task.Event = EventTypes.MouseDoubleClick;
                                eventDesc = "Mouse Left Button Double clicked on- ";
                            }
                            else
                            {
                                elementClicked.Task.Event = EventTypes.MouseLeftClick;
                                eventDesc = "Mouse Left Button clicked on- ";
                            }
                            break;
                        case MouseButtons.Right:
                            if (isDoubleClick)
                            {
                                elementClicked.Task.Event = EventTypes.MouseDoubleClick;
                                eventDesc = "Mouse Right Button Double clicked on- ";
                            }
                            else
                            {
                                elementClicked.Task.Event = EventTypes.MouseRightClick;
                                eventDesc = "Mouse Right Button clicked on- ";
                            }
                            break;
                        case MouseButtons.Middle:
                            if (isDoubleClick)
                            {
                                elementClicked.Task.Event = EventTypes.MouseDoubleClick;
                                eventDesc = "Mouse Middle Button Double clicked on- ";
                            }
                            else
                            {
                                elementClicked.Task.Event = EventTypes.MouseMiddleClick;
                                eventDesc = "Mouse Middle Button clicked on- ";
                            }
                            break;
                    }
                    elementClicked.Task.Description = eventDesc + htmlElement.Id;
                    //elementClicked.Task.Id = System.Guid.NewGuid().ToString();
                    elementClicked.Task.Name = elementClicked.Task.Event.ToString();
                    PatternAndState elementStatus = GetHTMLPatterFor(htmlElement);
                    elementClicked.Task.TriggeredPattern = elementStatus.Pattern;
                    elementClicked.Task.CurrentState = elementStatus.State;
                    elementClicked.Task.TargetControlAttributes = new List<NameValueAtribute>();
                    elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "Class", Value = htmlElement.GetAttribute("className") });
                    elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "TabIndex", Value = htmlElement.TabIndex.ToString() });
                    elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "Tag", Value = htmlElement.TagName });
                    elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "OuterHTML", Value = htmlElement.OuterHtml });
                    windowTitle = Browser.DocumentTitle;
                    elementClicked.ApplicationName = windowTitle;
                    elementClicked.Task.WindowTitle = windowTitle;
                    elementClicked.ApplicationType = Entities.ApplicationTypes.WebApplication.ToString();
                    //currently assigning the file name with MS IE assuming at least this browser will be available in the machine being used
                    //elementClicked.FileName = "iexplore.exe " + Browser.Url;
                    elementClicked.FileName = startingWebsite;// Browser.Url.ToString();
                    elementClicked.ModuleName = "Internet Browser";
                    //}
                    if (string.IsNullOrEmpty(_taskId))
                    {
                        elementClicked.Task.Id = System.Guid.NewGuid().ToString();
                        //take a snap shot of the region near the mouse click
                        TakeSnapShot(X, Y, elementClicked.Task.Id);
                    }
                    else
                    {
                        elementClicked.Task.Id = _taskId;
                        _taskId = "";
                    }
                }
            }
            if (MouseClicked != null)
            {
                //elementClicked.Task.Order = trackTaskOrder++;
                MouseClicked(elementClicked);
            }
        }

        static void UpdateElementDetails(AutomationElement element, MouseButtons button, int X = 0, int Y = 0, bool isDoubleClick = false, CacheRequest cacheRequest = null)
        {
            try
            {
                ////if mouse down happended on the recording tool itself, then ignore
                if (toolProcessId == element.Cached.ProcessId)
                {
                    ////check if the mouse down happened on the webbrowser hosting the html page
                    //if (Browser != null)
                    //{
                    //    UpdateHTMLElementDetail(element, button, X, Y, isDoubleClick);
                    //}
                    return;
                }                
                ////check if the content is html type
                string moduleName = GetModuleName(element.Cached.ProcessId);
                //if (htmlBrowsers.Contains(moduleName.ToLower()))
                //{
                //    UpdateHTMLElementDetail(element, button, X, Y);
                //    return;
                //}

                MouseClickedEventArgs elementClicked = new MouseClickedEventArgs();
                elementClicked.FileName = GetFileName(element.Cached.ProcessId);
                elementClicked.ApplicationName = GetProcessName(element.Cached.ProcessId);
                elementClicked.Task = new Task();
                elementClicked.Task.CapturedTime = DateTime.Now;
                #region commented for performance issue
                //try
                //{
                //    elementClicked.Task.ApplictionTreePath = ReverseLevelsinApplicationTree(GetApplicationTree(element, "", cacheRequest));
                //}
                //catch { elementClicked.Task.ApplictionTreePath = ""; }
                #endregion

                if (element.Cached.ProcessId != lastProcessId || lastUrlChanged)
                {
                    elementClicked.IsDifferentActivity = true;
                    lastUrlChanged = false;
                    lastProcessId = element.Cached.ProcessId;
                    settingToForegroundDone = false; // to be used to extract the window title
                }
                //elementClicked.Task = new Task();
                //elementClicked.Task.ApplictionTreePath = ReverseLevelsinApplicationTree(GetApplicationTree(element, ""));
                if (string.IsNullOrEmpty(groupScriptId))
                {
                    groupScriptId = System.Guid.NewGuid().ToString();
                    elementClicked.Task.GroupScriptId = groupScriptId;
                }
                elementClicked.ProcessId = element.Cached.ProcessId;
                elementClicked.Task.ControlId = element.Cached.AutomationId;
                elementClicked.Task.ControlName = element.Cached.Name;
                elementClicked.Task.ControlType = element.Cached.ControlType.ProgrammaticName;
                string eventDesc = "";
                switch (button)
                {
                    case MouseButtons.Left:

                        if (isDoubleClick)
                        {
                            elementClicked.Task.Event = EventTypes.MouseDoubleClick;
                            eventDesc = "Mouse Left Button Double clicked on- ";
                        }
                        else
                        {
                            elementClicked.Task.Event = EventTypes.MouseLeftClick;
                            eventDesc = "Mouse Left Button clicked on- ";
                        }
                        break;
                    case MouseButtons.Right:
                        if (isDoubleClick)
                        {
                            elementClicked.Task.Event = EventTypes.MouseDoubleClick;
                            eventDesc = "Mouse Right Button Double clicked on- ";
                        }
                        else
                        {
                            elementClicked.Task.Event = EventTypes.MouseRightClick;
                            eventDesc = "Mouse Right Button clicked on- ";
                        }
                        break;
                    case MouseButtons.Middle:
                        if (isDoubleClick)
                        {
                            elementClicked.Task.Event = EventTypes.MouseDoubleClick; ;
                            eventDesc = "Mouse Middle Button Double clicked on- ";
                        }
                        else
                        {
                            elementClicked.Task.Event = EventTypes.MouseMiddleClick;
                            eventDesc = "Mouse Middle Button clicked on- ";
                        }
                        break;
                }
                elementClicked.Task.Description = eventDesc + element.Cached.AutomationId;
                //elementClicked.Task.Id = System.Guid.NewGuid().ToString();
                elementClicked.Task.Name = elementClicked.Task.Event.ToString();
                PatternAndState elementStatus = GetPatternFor(element);
                elementClicked.Task.TriggeredPattern = elementStatus.Pattern;
                elementClicked.Task.CurrentState = elementStatus.State;
                elementClicked.Task.AccessKey = element.Cached.AccessKey;
                elementClicked.Task.TargetControlAttributes = new List<NameValueAtribute>();
                elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "AutomationId", Value = element.Cached.AutomationId });
                elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "LocalizedControlType", Value = element.Cached.LocalizedControlType });
                //elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "AccessKey", Value = element.Cached.AccessKey });
                elementClicked.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "ClassName", Value = element.Cached.ClassName });
                //elementClicked.ApplicationName = GetProcessName(element.Current.ProcessId);
                windowTitle = GetWindowTitle(element.Cached.ProcessId);
                elementClicked.Task.WindowTitle = windowTitle;
                elementClicked.ModuleName = moduleName;// GetModuleName(element.Cached.ProcessId);
                if (!string.IsNullOrEmpty(element.Cached.FrameworkId))
                    elementClicked.ApplicationType = element.Cached.FrameworkId;
                else
                    elementClicked.ApplicationType = GetExceptionalApplicationType(elementClicked.ApplicationName);

                if (!string.IsNullOrEmpty(elementClicked.ApplicationType))
                    if (elementClicked.ApplicationType.Equals(Entities.ApplicationTypes.WebApplication.ToString()))
                        GetApplicationPathWithSendkeys(elementClicked.ApplicationName);

                //elementClicked.FileName = GetFileName(element.Cached.ProcessId);
                if (elementClicked.ApplicationType.Equals(Entities.ApplicationTypes.WebApplication.ToString()) && !string.IsNullOrEmpty(lastUrlVisited))
                {
                    elementClicked.ApplicationName = lastUrlVisited;
                }

                //take a snap shot of the region near the mouse click
                if (string.IsNullOrEmpty(_taskId))
                {
                    try
                    {
                        elementClicked.Task.Id = System.Guid.NewGuid().ToString();
                        elementClicked.Task.ScreenId = System.Guid.NewGuid().ToString();
                        TakeSnapShot(X, Y, elementClicked.Task.Id);
                        TakeScreenShot(elementClicked.Task.ScreenId);
                    }
                    catch { }
                }
                else
                {
                    elementClicked.Task.Id = _taskId;
                    _taskId = "";
                    elementClicked.Task.ScreenId = _screenId;
                    _screenId = "";
                }

                if (MouseClicked != null)
                {
                    elementClicked.Task.Order = trackTaskOrder++;
                    MouseClicked(elementClicked);
                }
            }
            catch{}
        }

        static PatternAndState GetHTMLPatterFor(HtmlElement htmlElement)
        {
            PatternAndState obj = new PatternAndState();
            switch (htmlElement.GetAttribute("type").ToLower())
            {
                case "checkbox":
                    //send the opposite state as once clicked the state will get changed
                    obj.State = (!(htmlElement.GetAttribute("checked").ToLower() == "true")).ToString();
                    obj.Pattern = Entities.HTMLPattern.Check.ToString();
                    break;
                case "radio":
                    obj.State = "true";
                    obj.Pattern = Entities.HTMLPattern.Radio.ToString();
                    break;
                case "submit":
                case "button":
                    obj.Pattern = Entities.HTMLPattern.Invoke.ToString();
                    break;
                case "text":
                case "select-one":
                    obj.Pattern = Entities.HTMLPattern.Focus.ToString();
                    break;
                default:
                    obj.Pattern = Entities.HTMLPattern.Invoke.ToString();
                    break;
            }
            return obj;
        }

        static PatternAndState GetHTMLPatterFor(IHTMLElement htmlElement)
        {
            PatternAndState obj = new PatternAndState();
            string elementType = "";
            if (htmlElement.getAttribute("type").GetType() != typeof(System.DBNull))
                elementType = htmlElement.getAttribute("type").ToLower();
            switch (elementType)
            {
                case "checkbox":
                    //send the opposite state as once clicked the state will get changed
                    obj.State = (!(htmlElement.getAttribute("checked").ToLower() == "true")).ToString();
                    obj.Pattern = Entities.HTMLPattern.Check.ToString();
                    break;
                case "radio":
                    obj.State = "true";
                    obj.Pattern = Entities.HTMLPattern.Radio.ToString();
                    break;
                case "submit":
                case "button":
                    obj.Pattern = Entities.HTMLPattern.Invoke.ToString();
                    break;
                case "text":
                    obj.Pattern = Entities.HTMLPattern.Focus.ToString();
                    break;
                default:
                    obj.Pattern = Entities.HTMLPattern.Invoke.ToString();
                    break;
            }
            return obj;
        }

        static PatternAndState GetPatternFor(AutomationElement element)
        {
            PatternAndState obj = new PatternAndState();
            object objPattern;
            switch (element.Cached.ControlType.ProgrammaticName)
            {
                case "ControlType.Button": //for button, dropdown, hyperlink
                case "ControlType.Hyperlink":
                    obj.Pattern = "InvokePatternIdentifiers.Pattern";
                    break;
                case "ControlType.CheckBox":
                    obj.Pattern = "TogglePatternIdentifiers.Pattern";
                    if (element.TryGetCachedPattern(TogglePattern.Pattern, out objPattern))
                    {
                        //send the opposite state as once clicked the state will get changed
                        TogglePattern togglePattern = objPattern as TogglePattern;
                        obj.State = (!(togglePattern.Cached.ToggleState == ToggleState.On)).ToString();
                    }
                    //obj.State = (!(((System.Windows.Automation.TogglePattern)(element.GetCachedPattern(TogglePattern.Pattern))).Cached.ToggleState == ToggleState.On)).ToString();
                    break;
                case "ControlType.RadioButton": //for radio button, dropdown item, tab item
                case "ControlType.ListItem":
                case "ControlType.TabItem":
                    obj.Pattern = "SelectionItemPatternIdentifiers.Pattern";
                    if (element.TryGetCachedPattern(SelectionItemPattern.Pattern, out objPattern))
                    {
                        SelectionItemPattern selectPattern = objPattern as SelectionItemPattern;
                        //obj.State = selectPattern.Cached.IsSelected.ToString();
                        obj.State = "true"; //as on click the itme like radio will be selected whether it is currently selected or not
                    }
                    //obj.State = ((System.Windows.Automation.SelectionItemPattern)(element.GetCachedPattern(SelectionItemPattern.Pattern))).Cached.IsSelected.ToString();
                    break;
                case "ControlType.Document":
                    //no need to pass anything as document type controls are handled differently
                    break;
                case "ControlType.Text":
                case "ControlType.Edit":
                    obj.Pattern = "ValuePatternIdentifiers.Pattern";
                    break;
                case "ControlType.MenuItem":
                    obj.Pattern = "ExpandCollapsePatternIdentifiers.Pattern, InvokePatternIdentifiers.Pattern";
                    break;
                case "ControlType.ComboBox":
                    obj.Pattern = "ExpandCollapsePatternIdentifiers.Pattern, SelectionPatternIdentifiers.Pattern";
                    break;
            }
            return obj;
        }

        static string GetProcessName(int processId)
        {
            return Process.GetProcessById(processId).ProcessName;
        }

        static string GetModuleName(int processId)
        {
            return Process.GetProcessById(processId).MainModule.ModuleName;
        }

        static string GetFileName(int processId)
        {
            return Process.GetProcessById(processId).MainModule.FileName;
        }

        public static void RecordKeyboardKeydown(bool start = true)
        {
            if (start && !trakingKeyboard)
            {
                trakingKeyboard = true;
                GlobalEventHandler.KeyDownEvents += new KeyEventHandler(HookManager_KeyDown);
            }
            else
            {
                trakingKeyboard = false;
                GlobalEventHandler.KeyDownEvents -= new KeyEventHandler(HookManager_KeyDown);
                //the below assignment is needed as once the recordng is paused/stopped, Cached window may not be on the top
                settingToForegroundDone = false;
            }
        }

        public static WaitEventArgs AddWaitEvent()
        {
            WaitEventArgs e = new WaitEventArgs();
            Task t = new Task();
            t.Name = "Wait";
            t.Description = "Wait(5)";
            t.Id = Guid.NewGuid().ToString();
            t.Event = EventTypes.Wait;
            t.ControlOnApplication = ApplicationTypes.None;
            t.Order = trackTaskOrder++;
            t.TargetControlAttributes = new List<NameValueAtribute>();
            t.TargetControlAttributes.Add(new NameValueAtribute { Name = "Interval", Value = "5" });
            t.TargetControlAttributes.Add(new NameValueAtribute { Name = "Unit", Value = "Second(s)" });
            e.Task = t;
            return e;
        }

        static void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (!CurrentWindow() && trakingKeyboard)
            {
                //raise the default event with the key details
                KeyBoardKeyPresedEventArgs arg = new KeyBoardKeyPresedEventArgs();
                arg.Task = new Task() { TriggeredPattern = "KeyPress", ControlName = "Text Area", ControlType = "text area", CreatedOn = DateTime.Now };
                arg.Task.GroupScriptId = groupScriptId;
                arg.Task.WindowTitle = windowTitle;
                arg.Task.Description = e.KeyCode + "- key is pressed from Keyboard";
                if (string.IsNullOrEmpty(_taskId))
                    arg.Task.Id = System.Guid.NewGuid().ToString();
                else
                {
                    arg.Task.Id = _taskId;
                    _taskId = "";
                }
                arg.Task.CapturedTime = DateTime.Now;
                arg.Task.Name = e.KeyCode.ToString();
                arg.Task.Event = EventTypes.KeyboardKeyPress;
                arg.Task.TargetControlAttributes = new List<NameValueAtribute>();
                arg.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "KeyCode", Value = e.KeyCode.ToString() });
                arg.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "KeyData", Value = e.KeyData.ToString() });
                arg.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "KeyValue", Value = e.KeyValue.ToString() });
                arg.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "IsControlKeyPressed", Value = e.Control.ToString() });
                arg.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "IsAltKeyPressed", Value = e.Alt.ToString() });
                arg.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "IsShiftKeyPressed", Value = e.Shift.ToString() });
                arg.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "Modifiers", Value = e.Modifiers.ToString() });

                if (KeyBoardKeyPresed != null)
                {
                    arg.Task.Order = trackTaskOrder++;
                    KeyBoardKeyPresed(arg);
                }

                if (keysPressed == "")
                    keysPressed += e.KeyCode;
                else
                {
                    trackKeyCount++;
                    if (trackKeyCount == maxToBeCaptured - 1)
                    {
                        //allowing one less so that one may close the below shown message box by clicking "enter" from keyboard
                        //raise event with the captured key codes
                        KeyBoardFlushCaturedEventArgs flushArgs = new KeyBoardFlushCaturedEventArgs();
                        flushArgs.Task = new Task();
                        flushArgs.Task.Description = keysPressed + " keys are pressed from Keyboard";
                        flushArgs.Task.Id = System.Guid.NewGuid().ToString();
                        flushArgs.Task.Name = "KeysFlushed";
                        flushArgs.Task.Event = EventTypes.KeyboardKeyPress;
                        flushArgs.Task.TargetControlAttributes = new List<NameValueAtribute>();
                        flushArgs.Task.TargetControlAttributes.Add(new NameValueAtribute() { Name = "KeysPressed", Value = keysPressed });

                        if (KeyBoardFlushCatured != null)
                        {
                            flushArgs.Task.Order = trackTaskOrder++;
                            KeyBoardFlushCatured(flushArgs);
                            trackKeyCount = 0;
                            keysPressed = "";
                        }
                        return;
                    }
                    keysPressed += ", " + e.KeyCode;
                }
            }
        }

        private static bool CurrentWindow()
        {
            if (currentHandle != GetForegroundWindow())
                return false;
            return true;
        }

        public static string FlushKeyboardKeys()
        {
            return keysPressed;
        }

        private static string GetXPath(HtmlElement htmlElement)
        {
            string xpath = "/" + htmlElement.TagName;
            if (htmlElement.Parent != null)
            {
                PositionInfo info = GetPosition(htmlElement);
                if (info.HasSibling)
                    xpath = xpath + "[" + info.Position.ToString() + "]";
                xpath = GetXPath(htmlElement.Parent) + xpath;
            }
            return xpath;
        }

        private static string GetXPath(IHTMLElement htmlElement)
        {
            string xpath = "/" + htmlElement.tagName;
            if (htmlElement.parentElement != null)
            {
                PositionInfo info = GetPosition(htmlElement);
                if (info.HasSibling)
                    xpath = xpath + "[" + info.Position.ToString() + "]";
                xpath = GetXPath(htmlElement.parentElement) + xpath;
            }
            return xpath;
        }

        /// <summary>
        /// Gets the position details of the html provided. Current it returns the position as zero based index.
        /// If needed modify this method to return one based index.
        /// </summary>
        /// <param name="htmlElement"></param>
        /// <returns></returns>
        private static PositionInfo GetPosition(HtmlElement htmlElement)
        {
            int position = 0;
            int htmlElementTracker = 0;
            bool hasSibling = false;
            HtmlElement parent = htmlElement.Parent;
            for (int i = 0; i < parent.Children.Count; i++)
            {
                if (parent.Children[i] != htmlElement && parent.Children[i].TagName == htmlElement.TagName)
                {
                    hasSibling = true;
                    htmlElementTracker++;
                }

                if (parent.Children[i] == htmlElement)
                    position = htmlElementTracker;
            }

            return new PositionInfo() { HasSibling = hasSibling, Position = position };
        }
        private static PositionInfo GetPosition(IHTMLElement htmlElement)
        {
            int position = 0;
            int htmlElementTracker = 0;
            bool hasSibling = false;
            IHTMLElement parent = htmlElement.parentElement;
            foreach (var item in parent.children)
            {
                if (item != htmlElement && item.tagName == htmlElement.tagName)
                {
                    hasSibling = true;
                    htmlElementTracker++;
                }

                if (item == htmlElement)
                    position = htmlElementTracker;
            }
            //for (int i = 0; i < parent.children.Count; i++)
            //{
            //    if (parent.children[i] != htmlElement && parent.children[i].TagName == htmlElement.tagName)
            //    {
            //        hasSibling = true;
            //        htmlElementTracker++;
            //    }

            //    if (parent.children[i] == htmlElement)
            //        position = htmlElementTracker;
            //}

            return new PositionInfo() { HasSibling = hasSibling, Position = position };
        }

        private static void TakeSnapShot(int x, int y, string taskId)
        {
            //get the location, width and height
            string location = ConfigurationManager.AppSettings["TaskImageLocation"];
            string height = _height == 0 ? ConfigurationManager.AppSettings["TaskImageHeight"] : _height.ToString();
            string width = _width == 0 ? ConfigurationManager.AppSettings["TaskImageWidth"] : _width.ToString();
            string capture = ConfigurationManager.AppSettings["CaptureTaskImage"];

        if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(height) && !string.IsNullOrEmpty(width) && !string.IsNullOrEmpty(capture))
            {
                //check if the location exists, if not try creating
                if (!System.IO.Directory.Exists(location))
                    System.IO.Directory.CreateDirectory(location);
                if (bool.Parse(capture.ToLower()))
                {
                    int h = int.Parse(height);
                    int w = int.Parse(width);
                    Bitmap snap = new Bitmap(w, h);
                    //take snap of the region on the either sides of the mouse position
                    using (var g = Graphics.FromImage(snap))
                        g.CopyFromScreen(new Point(x - w / 2, y - h / 2), Point.Empty, new Size(w, h));
                    snap.Save(location + @"\" + taskId + ".jpg", ImageFormat.Jpeg);
                }
            }
        }

        private static void TakeScreenShot(string screenId)
        {
            //get the location, width and height
            string location = ConfigurationManager.AppSettings["TaskImageLocation"];

            Rectangle screen = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(screen.Width, screen.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, screen.Size);
                }
                bitmap.Save(location + @"\" + screenId + ".jpg", ImageFormat.Jpeg);
            }
        }

        private static bool IsURLHostChanged(string lastUrl, string currentUrl)
        {
            var uri = new Uri(lastUrl);
            string lastHost = uri.Host.ToLower();
            uri = new Uri(currentUrl);
            string currentHost = uri.Host.ToLower();
            if (lastHost == currentHost)
                return false;
            else
                return true;
        }

        private static string ReverseLevelsinApplicationTree(string applicationTree)
        {
            if (!string.IsNullOrEmpty(applicationTree))
            {
                //e.g. applicationTree = "/0[6]/1[5]/2[7]/3[0]/4[0]"- level[peer index]
                string[] strs = applicationTree.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> levels = new List<string>();
                List<string> newlevels = new List<string>();
                List<string> peerIndexes = new List<string>();
                foreach (string st in strs)
                {
                    string[] sts = st.Substring(0, st.Length - 1).Split('[');
                    levels.Add(sts[0]);
                    peerIndexes.Add(sts[1]);
                }
                peerIndexes.Reverse();
                applicationTree = "";
                for (int i = 0; i < levels.Count; i++)
                {
                    applicationTree += "/" + levels[i] + "[" + peerIndexes[i] + "]";
                }
            }
            return applicationTree;
        }

        private static string GetApplicationTree(AutomationElement currElement, string curAppTree, CacheRequest cacheRequest)
        {
            //set url is available
            if (currElement.Cached.ClassName == "Internet Explorer_Server")
                lastUrlChanged = AssignUrlVisited(currElement);
            //get the parent
            AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(currElement.GetUpdatedCache(cacheRequest), cacheRequest);
            if (parent != null)// && !string.IsNullOrEmpty(curAppTree))
            {
                //get all the children to understand current passed element's index
                AutomationElementCollection children =parent.FindAll(TreeScope.Children, Condition.TrueCondition);
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] == currElement)
                    {
                        if (string.IsNullOrEmpty(curAppTree))
                            curAppTree = "/0[" + i + "]";
                        else
                        {
                            int level = GetCurrentLevelInAppTree(curAppTree) + 1;
                            curAppTree += "/" + level + "[" + i + "]";
                        }
                        break;
                    }
                }
                if (parent.Cached.LocalizedControlType.ToLower() != "window")
                    curAppTree = GetApplicationTree(parent, curAppTree, cacheRequest);
            }
            if (parent == null || parent.Cached.LocalizedControlType.ToLower() == "window")
            {
                int level = GetCurrentLevelInAppTree(curAppTree) + 1;
                curAppTree += "/" + level + "[0]";
            }

            return curAppTree;
        }

        private static int GetCurrentLevelInAppTree(string appTreePath)
        {
            //e.g. of appTreePath - /0[0]/1[3], control level is 1
            if (!string.IsNullOrEmpty(appTreePath))
            {
                string[] appTreePathParts = appTreePath.Split('/');
                string controlAppTreePathPart = appTreePathParts[appTreePathParts.Length - 1]; //e.g. 1[3]
                int controlLevel = int.Parse(controlAppTreePathPart.Split('[')[0]); //e.g. 1
                return controlLevel;
            }
            else
                return -2; //that is level cant be calculated
        }

        private static string GetExceptionalApplicationType(string processName)
        {
            //this function is meant for the scenario where the automation element could not ge
            //the ui framework details for certain section of the application ui.
            //for instance in case of IE, the automation element could not get the ui framework
            //details for the control in the html body
            string applicationTypes = "";

            switch(processName.ToLower())
            {
                case "iexplore":
                case "chrome":
                    applicationTypes= Entities.ApplicationTypes.WebApplication.ToString();
                    break;
                default:
                    applicationTypes="";
                    break;
            }

            return applicationTypes;
        }

        private static bool AssignUrlVisited(AutomationElement element)
        {
            object objPattern;
            bool urlChanged = false;
            if (element.TryGetCachedPattern(ValuePattern.Pattern, out objPattern))
            {
                ValuePattern valuePattern = objPattern as ValuePattern;
                string url = valuePattern.Cached.Value;
                if (url != lastUrlVisited)
                {
                    urlChanged = true;
                    lastUrlVisited = url;
                }
            }
            return urlChanged;
        }

        static CacheRequest CreateCacheRequest()  
        {
            CacheRequest cacheRequest = new CacheRequest();            
            cacheRequest.AutomationElementMode = AutomationElementMode.None;//specifies that returned elements having NO reference to the underlying UI
            //cacheRequest.AutomationElementMode = AutomationElementMode.Full;// specifies that returned elements having full reference to the underlying UI
            cacheRequest.TreeFilter = Automation.RawViewCondition;
            cacheRequest.Add(AutomationElement.AcceleratorKeyProperty);
            cacheRequest.Add(AutomationElement.AccessKeyProperty);
            cacheRequest.Add(AutomationElement.AutomationIdProperty);
            cacheRequest.Add(AutomationElement.BoundingRectangleProperty);
            cacheRequest.Add(AutomationElement.ClassNameProperty);
            cacheRequest.Add(AutomationElement.ClickablePointProperty);
            cacheRequest.Add(AutomationElement.ControlTypeProperty);
            cacheRequest.Add(AutomationElement.CultureProperty);
            cacheRequest.Add(AutomationElement.FrameworkIdProperty);
            cacheRequest.Add(AutomationElement.HasKeyboardFocusProperty);
            cacheRequest.Add(AutomationElement.HelpTextProperty);
            cacheRequest.Add(AutomationElement.IsContentElementProperty);
            cacheRequest.Add(AutomationElement.IsControlElementProperty);
            cacheRequest.Add(AutomationElement.IsDockPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsEnabledProperty);
            cacheRequest.Add(AutomationElement.IsExpandCollapsePatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsGridItemPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsGridPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsInvokePatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsItemContainerPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsKeyboardFocusableProperty);
            cacheRequest.Add(AutomationElement.IsMultipleViewPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsOffscreenProperty);
            cacheRequest.Add(AutomationElement.IsPasswordProperty);
            cacheRequest.Add(AutomationElement.IsRangeValuePatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsRequiredForFormProperty);
            cacheRequest.Add(AutomationElement.IsScrollItemPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsScrollPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsSelectionItemPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsSelectionPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsSynchronizedInputPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsTableItemPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsTablePatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsTextPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsTogglePatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsTransformPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsValuePatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsVirtualizedItemPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.IsWindowPatternAvailableProperty);
            cacheRequest.Add(AutomationElement.ItemStatusProperty);
            cacheRequest.Add(AutomationElement.ItemTypeProperty);
            cacheRequest.Add(AutomationElement.LabeledByProperty);
            cacheRequest.Add(AutomationElement.LocalizedControlTypeProperty);
            cacheRequest.Add(AutomationElement.NativeWindowHandleProperty);
            cacheRequest.Add(AutomationElement.NameProperty);
            cacheRequest.Add(AutomationElement.OrientationProperty);
            cacheRequest.Add(AutomationElement.ProcessIdProperty);
            cacheRequest.Add(AutomationElement.RuntimeIdProperty);
            cacheRequest.Add(SelectionItemPattern.Pattern);
            cacheRequest.Add(SelectionItemPattern.SelectionContainerProperty);
            //cacheRequest.TreeScope = TreeScope.Children | TreeScope.Element;

            return cacheRequest;        
        }

        private static void GetApplicationPath(string pName)
        {
            string url = string.Empty;
            string searchEngine = ConfigurationManager.AppSettings["WebSearchEngine"];

            Process[] pBrowser = Process.GetProcessesByName(pName);            
            foreach (Process proc in pBrowser)   
            {
                // the chrome process must have a window
                if (proc.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }

                //whandle = Process.GetProcessById(processID).MainWindowHandle;
                //if (whandle == IntPtr.Zero)
                //  return;

                PropertyCondition nameProperty = null;

                switch (pName)
                {
                    case "iexplore":
                        string ObjNameProp = "Address and search using Bing";
                        if (!string.IsNullOrEmpty(searchEngine))
                            ObjNameProp = string.Format("Address and search using {0}", searchEngine);
                        nameProperty = new PropertyCondition(AutomationElement.NameProperty, ObjNameProp, PropertyConditionFlags.IgnoreCase);
                        break;
                    case "chrome":
                        nameProperty = new PropertyCondition(AutomationElement.NameProperty, "Address and search bar", PropertyConditionFlags.IgnoreCase);
                        break;
                    default:
                        nameProperty = new PropertyCondition(AutomationElement.NameProperty, "Address and search using Bing", PropertyConditionFlags.IgnoreCase);
                        break;
                }

                AndCondition andCondition = new AndCondition(nameProperty, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),Condition.TrueCondition);              
                AutomationElement elm;
                AutomationElement elmUrlBar;
                #region Commented code of Caching Approach
                //CacheRequest cacheRequest = new CacheRequest();
                //cacheRequest.AutomationElementMode = AutomationElementMode.Full;
                //cacheRequest.TreeFilter = Automation.ControlViewCondition;
                //cacheRequest.Add(AutomationElement.ControlTypeProperty);
                //cacheRequest.Add(AutomationElement.NameProperty);
                //cacheRequest.Add(SelectionItemPattern.Pattern);
                //cacheRequest.Add(SelectionItemPattern.SelectionContainerProperty);
                //cacheRequest.TreeScope = TreeScope.Descendants | TreeScope.Element;
                //cacheRequest.Push();
                #endregion

                elm = AutomationElement.FromHandle(proc.MainWindowHandle);
                elmUrlBar = elm.FindFirst(TreeScope.Descendants, andCondition);
                //cacheRequest.Pop();
                //TreeWalker tw = new TreeWalker(andCondition);
                //AutomationElement elmUrlBar = tw.GetFirstChild(elm);               

                if (elmUrlBar != null)
                {
                    url = ((ValuePattern)elmUrlBar.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                    if (url != lastUrlVisited)
                    {
                        lastUrlChanged = true;
                        lastUrlVisited = url;
                        break;
                    }
                }                
            }
        }

        private static void GetApplicationPathWithSendkeys(string pName)
        {
            string clsNameProp = "";

            switch (pName.ToLower())
            {
                case "iexplore":
                    clsNameProp = "IEFrame";
                    break;
                case "chrome":
                    clsNameProp = "Chrome_WidgetWin_1";
                    break;
                default:
                    clsNameProp = "None";
                    break;
            }

            IsSendKey(true);

            AutomationElement.RootElement
                .FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, clsNameProp, PropertyConditionFlags.IgnoreCase))// "Chrome_WidgetWin_1"))
                .SetFocus();
            SendKeys.SendWait("^l");
            var elmUrlBar = AutomationElement.FocusedElement;

            IsSendKey(false);

            if (elmUrlBar != null)
            {
               string  url = ((ValuePattern)elmUrlBar.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                if (url != lastUrlVisited)
                {
                    lastUrlChanged = true;
                    lastUrlVisited = url;
                }
            }            
        }

        private static void GetApplicationPathWithShellWindows(string pName)
        {
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
           
            foreach (SHDocVw.InternetExplorer ie in shellWindows)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                if (!filename.Equals("iexplore"))
                    continue;
                
                    HTMLDocument doc = ie.Document as mshtml.HTMLDocument;
                    if (doc.GetType().FullName == "mshtml.HTMLDocumentClass")
                        if (doc != null)
                            if (doc.hasFocus() == true)
                            {
                                if (ie.LocationURL.ToString() != lastUrlVisited)
                                {
                                    lastUrlChanged = true;
                                    lastUrlVisited = ie.LocationURL.ToString();
                                }
                            }
                
            }
        }
    }
}
