/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

using System.Windows.Automation;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Infosys.WEM.Infrastructure.Common;
using System.Diagnostics;
using Infosys.JavaAccessBridge;
using Microsoft.Win32;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Core
{
    public class Utilities
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string strClassName, string strWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll")]
        static extern int SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 SendInput(int numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        private const string stopFile = "stop.iap";

        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;
        //private const int KEYEVENTF_KEYUP = 2;
        //private const int KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYDOWN = 0x0;
        private const int VK_LCONTROL = 0xA3;

        private const Keys KEYEVENT_SHIFT = Keys.LShiftKey;
        private const Keys KEYEVENT_CONTROL = Keys.ControlKey;
        private const Keys KEYEVENT_WINDOWS = Keys.RWin;
        private const Keys KEYEVENT_ALT = Keys.LMenu;
        private const Keys KEYEVENT_CAPITAL = Keys.Capital;
        private const Keys KEYEVENT_ENTER = Keys.Enter;
        private const Keys KEYEVENT_TAB = Keys.Tab;
        private const Keys KEYEVENT_BACKSPACE = Keys.Back;
        private const Keys KEYEVENT_DEL = Keys.Delete;
        private const Keys KEYEVENT_SPACE = Keys.Space;

        private const string className = "Utilities";

        private static System.Collections.Hashtable cacheControlPathElements = new Hashtable();
        private static System.Collections.Hashtable cacheFocusedElements = new Hashtable();

        public static double ScaleStep { get; set; }
        public static int MaxScaleSteps { get; set; }
        public static byte[] TemplateMatchMapScreen { get { return templateMatchMapScreen; } }

        private static byte[] templateMatchMapScreen;
        public static int TemplateMatchMapBorderThickness { get; set; }
        public static ImageBgr TemplateMatchMapBorderColor { get; set; }

        const bool MULTIPLE_SCALE = true;
        const int DEFAULT_TIMEOUT = 10;
        const int DEFAULT_TIMEOUT_PERINSTANCE = 2;
        const int DEFAULT_CONFIDENCE = 80;
        const int THREAD_SLEEP_DURATION = 100; //in ms

        const string ieWebBrowser = "internet explorer";
        const string firefoxWebBrowser = "firefox";
        const string chromeWebBrowser = "chrome";

        const int INPUT_MOUSE = 0;
        const int INPUT_KEYBOARD = 1;
        const int INPUT_HARDWARE = 2;
        //const uint KEYEVENTF_EXTENDEDKEY1 = 0x0001;
        //const uint KEYEVENTF_KEYUP1 = 0x0002;
        //const uint KEYEVENTF_UNICODE1 = 0x0004;
        // const uint KEYEVENTF_SCANCODE = 8;

        struct INPUT
        {
            public INPUTType type;
            public INPUTUnion Event;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUTUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public KEYEVENTF dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        enum INPUTType : uint
        {
            INPUT_KEYBOARD = 1
        }

        [Flags]
        enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern short VkKeyScan(char ch);
        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();
        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        private const int KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 2;
        private const int KEYEVENTF_SCANCODE = 8;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string path);

        public static Stream IapwPackage = null;

        /// <summary>
        /// This method is used to load dlls based on if the application is running on 32 or 64 bit platform,
        /// </summary>
        public static void SetDLLsPath()
        {
            if (Assembly.GetEntryAssembly() != null) //needed when running thru the test project, in such case this is null
            {
                string path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                path = System.IO.Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x86");
                bool pathFound = SetDllDirectory(path);
                if (!pathFound)
                    throw new System.ComponentModel.Win32Exception();
            }
        }

        // Constructor
        static Utilities()
        {
            SetDLLsPath();
        }

        public static IntPtr GetWindowHandle(string strClassName, string strWindowName)
        {
            if (string.IsNullOrEmpty(strClassName))
                return FindWindow(null, strWindowName);
            else
                return FindWindow(strClassName, null); //While calling user32.dll exposed FindWindow- when passing class name shud not pass window name- e.g. for web page context menu it has to be FindNow(“#32768”,null). Otherwise not windows handle is returned
        }

        public static void DoMouseClick()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        public static void DoMouseRightClick()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }

        public static void PlaceMouseCursor(double x, double y)
        {
            System.Windows.Point clickablePoint = new System.Windows.Point();
            clickablePoint.X = x;
            clickablePoint.Y = y;

            System.Windows.Forms.Cursor.Position =
             new System.Drawing.Point((int)clickablePoint.X, (int)clickablePoint.Y);

        }

        public static int GetProcessId(IntPtr winHandle)
        {
            uint pid = 0;
            GetWindowThreadProcessId(winHandle, out pid);
            return (int)pid;
        }

        public static AutomationElement FilterAutomationElement(AutomationElementCollection elements, Control ctl, IntPtr appWinHandle, string fullControlQualifier)
        {
            AutomationElement elementFound = null;

            string fullControlPath = FormControlQualifierPath(ctl.ControlPath, fullControlQualifier);

            bool automationElementFound = false;
            if (elements != null)
            {
                //first check if any two elements i.e. more than elements has same id and name.
                //if so then skip the below for loop

                if (!HasDuplicateElement(elements, ctl))
                {
                    foreach (AutomationElement element in elements)
                    {
                        //first check for automation id/automation name then application tree path
                        if (string.IsNullOrEmpty(element.Current.AutomationId) && string.IsNullOrEmpty(element.Current.Name))
                            continue;
                        if (element.Current.AutomationId == ctl.AutomationId && element.Current.Name == ctl.AutomationName)
                        {
                            elementFound = element;
                            automationElementFound = true;
                            break;
                        }
                    }
                }
            }
            if (automationElementFound == false)
            {

                //Check cached instance, if available return from the cached path
                if (cacheControlPathElements.ContainsKey(fullControlPath) && cacheControlPathElements[fullControlPath] != null)
                {
                    elementFound = (AutomationElement)cacheControlPathElements[fullControlPath];
                }
                else
                {
                    elementFound = MatchForApplicationTreePath(ctl.ControlPath, appWinHandle, fullControlQualifier, ctl);
                }
            }

            //if (elementFound == null)
            //{
            //    AutomationElement parent = AutomationElement.FromHandle(appWinHandle);
            //    parent.SetFocus();
            //    elementFound = FindElementViaFocusTracking(parent, ctl);

            //}




            return elementFound;
        }

        private static bool HasDuplicateElement(AutomationElementCollection elements, Control ctl)
        {
            bool hasDuplicate = false;
            int duplicateCount = 0;
            foreach (AutomationElement element in elements)
            {
                if (element.Current.AutomationId == ctl.AutomationId && element.Current.Name == ctl.AutomationName)
                    duplicateCount++;
                if (duplicateCount == 2)
                {
                    hasDuplicate = true;
                    break;
                }
            }
            return hasDuplicate;
        }

        private static string FormControlQualifierControlElement(string automationId, string automationName, string controlType, string fullControlQualifier)
        {
            string qualifierRep = FormControlQualifierPath("", fullControlQualifier);
            qualifierRep = qualifierRep + automationId + "." + automationName + "." + controlType;
            return qualifierRep;
        }
        private static string FormControlQualifierPath(string ctlTreePath, string fullControlQualifier)
        {
            string[] controlNameSegments = fullControlQualifier.Split('.');
            string fullControlPath = "";
            if (controlNameSegments.Length > 2) // eg: app.screen.control , app.screen.control1.control2 assuming the control hierarchy should always be a child of the parent control
            {
                fullControlPath = String.Join(".", controlNameSegments, 0, 2);
            }
            else if (controlNameSegments.Length == 2)//eg: app.screen or app.control
            {
                fullControlPath = controlNameSegments[0];
            }

            fullControlPath = fullControlPath + "." + ctlTreePath;
            return fullControlPath;
        }

        public static AutomationElement MatchForApplicationTreePath(string appTreePath, IntPtr appWinHandle, string fullControlQualifier, Control controlObj)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Infrastructure, Guid.Empty, className, Logging.Constants.MATCHFORAPPLICATIONTREEPATH))
            {
                //e.g. of appTreePath - /0[0]/1[3]/2[7]/3[2]
                AutomationElement elementFound = null;
                if (string.IsNullOrEmpty(appTreePath))
                    return elementFound;
                //get the automation element corresponding to the main application
                AutomationElement appWindow = AutomationElement.FromHandle(appWinHandle);
                AutomationElement tempElement = appWindow;
                AutomationElement prevElement = tempElement;
                bool elementMatchFound = false;
                //check if parent instance is available in cache
                string parentControlPath = appTreePath.Substring(0, appTreePath.LastIndexOf('/'));
                string parentControlQualifier = FormControlQualifierPath(parentControlPath, fullControlQualifier);
                //string parentControlQualifier = fullControlQualifier + parentControlPath;
                if (cacheControlPathElements.ContainsKey(parentControlQualifier) && cacheControlPathElements[parentControlQualifier] != null)
                {
                    elementMatchFound = true;
                    tempElement = (AutomationElement)cacheControlPathElements[parentControlQualifier];

                    if (tempElement == null || tempElement.Current.BoundingRectangle.IsEmpty)
                    {
                        cacheControlPathElements.Remove(parentControlQualifier);
                        tempElement = null;
                        elementMatchFound = false;
                    }
                }

                if (elementMatchFound)
                {
                    string childControlPath = appTreePath.Replace(parentControlPath, "");
                    string[] stsSegment = childControlPath.Substring(0, childControlPath.Length - 1).Split('[');
                    int index = int.Parse(stsSegment[1]);
                    AutomationElementCollection children = tempElement.FindAll(TreeScope.Children, Condition.TrueCondition);
                    if (index < children.Count)
                        tempElement = children[index];

                    else
                    {
                        //if the app path is not traversed yet for any reason
                        //e.g. in case of iframe, it dosent expose its child
                        //then we need to follow a different approach to drill down further
                        //e.g. scan pixel by pixel in the parent bounding rectangle using FromPoint api
                        //and try to identify the element
                        //  tempElement = GetAutomationElementFromWithinBoundary(tempElement, childControlPath);

                        //if (tempElement.Current.IsKeyboardFocusable)
                        //{
                        //    tempElement.SetFocus();
                        //}
                        //tempElement = FindElementViaFocusTracking(tempElement, controlObj);
                        tempElement = null;

                    }
                    parentControlQualifier += "/" + childControlPath.Replace("/", "");
                    if (tempElement != null)
                    {
                        //Cache the tree path and automation element instance in memory
                        cacheControlPathElements[parentControlQualifier] = tempElement;

                    }
                }
                else
                {
                    string traversedAppPath = "";
                    if (appWindow != null)
                    {
                        string[] pathParts = appTreePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                        if (pathParts.Length > 0)
                        {
                            traversedAppPath = "/0[0]";
                            for (int i = 1; i < pathParts.Length; i++)
                            {
                                traversedAppPath += "/" + pathParts[i];
                                // string fullControlQualifierPath = fullControlQualifier + "." + traversedAppPath;
                                string fullControlQualifierPath = parentControlQualifier.Substring(0, parentControlQualifier.LastIndexOf('.'))
                                                                    + "." + traversedAppPath;
                                elementMatchFound = false;
                                if (cacheControlPathElements.ContainsKey(fullControlQualifierPath))
                                {
                                    tempElement = (AutomationElement)cacheControlPathElements[fullControlQualifierPath];
                                    elementMatchFound = true;
                                    if (tempElement == null || tempElement.Current.BoundingRectangle.IsEmpty)
                                    {
                                        cacheControlPathElements.Remove(fullControlQualifierPath);
                                        tempElement = null;
                                        elementMatchFound = false;
                                    }

                                }

                                if (!elementMatchFound)
                                {

                                    //starting from second path part i.e. i=1 because the appWindow corresponds to the the first path part i.e. 0[0]
                                    if (prevElement != null)
                                    {

                                        string[] sts = pathParts[i].Substring(0, pathParts[i].Length - 1).Split('[');
                                        int index = int.Parse(sts[1]);

                                        AutomationElementCollection children = prevElement.FindAll(TreeScope.Children, Condition.TrueCondition);
                                        if (index < children.Count)
                                        {
                                            tempElement = children[index];
                                            //if (tempElement.Current.IsKeyboardFocusable)
                                            //{
                                            //try
                                            //{
                                            //    tempElement.SetFocus();
                                            //}catch(Exception){}
                                            // }
                                        }
                                        else
                                        {
                                            //if the app path is not traversed yet for any reason
                                            //e.g. in case of iframe, it dosent expose its child
                                            //then we need to follow a different approach to drill down further
                                            //e.g. scan pixel by pixel in the parent bounding rectangle using FromPoint api
                                            //and try to identify the element
                                            //tempElement = GetAutomationElementFromWithinBoundary(prevElement, 
                                            //    appTreePath.Replace(traversedAppPath, ""));

                                            //tempElement = FindElementViaFocusTracking(prevElement, controlObj);
                                            ////tempElement = null;
                                            //if (tempElement != null)
                                            //    break;
                                            tempElement = null;
                                            break;
                                        }

                                        //Cache the tree path and automation element instance in memory                              
                                        if (tempElement != null && controlObj.DoEntityCaching)
                                        {
                                            cacheControlPathElements.Add(fullControlQualifierPath, tempElement);
                                        }

                                    }

                                }

                                prevElement = tempElement;
                            }

                        }
                    }
                }
                elementFound = tempElement;
                return elementFound;
            }
        }
        public static void ClearCache()
        {
            if (cacheControlPathElements != null && cacheControlPathElements.Count > 0)
                cacheControlPathElements.Clear();
            if (cacheFocusedElements != null && cacheFocusedElements.Count > 0)
                cacheFocusedElements.Clear();
        }

        /// <summary>
        /// To clear the control details for whihc the control tree path is passed from the cache. 
        /// This is to handle the case when the part of application UI is changed but the control tree path of the UI element doesnt change.
        /// </summary>
        /// <param name="controlTreePath">the control tree path of the control which needs to be removed from cache as it has changed in the application in concern</param>
        public static void ClearCache(string controlTreePath)
        {
            if (cacheControlPathElements != null && cacheControlPathElements.Count > 0 && cacheControlPathElements.ContainsKey(controlTreePath))
                cacheControlPathElements.Remove(controlTreePath);
            if (cacheFocusedElements != null && cacheFocusedElements.Count > 0 && cacheFocusedElements.ContainsKey(controlTreePath))
                cacheFocusedElements.Remove(controlTreePath);
        }

        private static AutomationElement GetAutomationElementFromWithinBoundary(AutomationElement parent, string appPathTobeTraversed)
        {

            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Infrastructure, Guid.Empty,
                className, Logging.Constants.GETAUTOMATIONELEMENTFROMWITHINBOUNDARY))
            {
                AutomationElement child = null;
                if (parent != null)
                {
                    //get the bounding rectangle
                    System.Windows.Rect region = parent.Current.BoundingRectangle;
                    region = (System.Windows.Rect)parent.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                    //scan bounding region with x and y offset = 5 pixcels
                    int offset = 10;
                    double x = region.X + offset, y = region.Y + offset;
                    double maxX = region.X + region.Width, maxY = region.Y + region.Height;
                    while (y <= maxY && child == null)
                    {
                        child = ScanXaxix(x, maxX, y, offset, parent, appPathTobeTraversed);
                        y += offset;
                    }
                }
                return child;
            }


        }

        private static AutomationElement childFocusTracked = null;
        private static AutomationElement currentChildElementTracked = null;
        private static System.Windows.Rect firstControl = System.Windows.Rect.Empty;
        private static Control controlToTrack = null;
        private static string tabDirectionForward = "{TAB}";
        private static string tabDirectionBackward = "+{TAB}";
        private static bool firstChanceDone = false;
        private static int maxLoopCount = 50;//temp to be updated based on timeout condition or a better approach
        private static int currentCount = 0;

        //private static Graphics drawingObj = null;
        private static System.Threading.AutoResetEvent arEvent = new System.Threading.AutoResetEvent(false);
        public static AutomationElement FindElementViaFocusTracking(AutomationElement parent, Control ctl)
        {

            childFocusTracked = null;
            currentChildElementTracked = null;
            controlToTrack = ctl;
            //drawingObj = Graphics.FromHwnd((IntPtr)ctl.AppWindowHandle);
            SubscribeToFocusChange();
            string qualifier = "";


            if (ctl.AutomationId == "" && ctl.AutomationName == "")
                return null;
            parent = null;
            do
            {
                //Start: Caching logic
                //qualifier = FormControlQualifierControlElement(
                //    ctl.AutomationId,
                //    ctl.AutomationName,
                //    ctl.GetControlType,
                //    ctl.FullControlQualifier);
                //if (cacheFocusedElements.ContainsKey(qualifier))
                //    childFocusTracked = (AutomationElement) cacheFocusedElements[qualifier];
                //else
                //{
                //    System.Windows.Forms.SendKeys.SendWait("+{TAB}");
                //    arEvent.WaitOne();
                //    if (currentChildElementTracked != null && (currentChildElementTracked.Current.AutomationId != "" && currentChildElementTracked.Current.Name != ""))
                //    {
                //        qualifier = FormControlQualifierControlElement(
                //                currentChildElementTracked.Current.AutomationId,
                //                currentChildElementTracked.Current.Name,
                //                currentChildElementTracked.Current.ControlType.ProgrammaticName,
                //                ctl.FullControlQualifier);
                //        if (!cacheFocusedElements.ContainsKey(qualifier))
                //            cacheFocusedElements.Add(qualifier, currentChildElementTracked);
                //    }

                //}
                //End: Caching logic
                if (!firstChanceDone)
                {
                    System.Windows.Forms.SendKeys.SendWait(tabDirectionBackward);
                    firstChanceDone = true;

                }
                else
                {
                    System.Windows.Forms.SendKeys.SendWait(tabDirectionForward);
                }

                arEvent.WaitOne(700);

                if (parent != null)
                {
                    if (parent.Current.BoundingRectangle == currentChildElementTracked.Current.BoundingRectangle)
                    {
                        break;
                    }
                }
                else
                {
                    parent = currentChildElementTracked;
                }
                //if (currentChildElementTracked!=null && parent.Current.BoundingRectangle == currentChildElementTracked.Current.BoundingRectangle) 
                //    break; //the tab tracking has returned from where it started

                if (currentChildElementTracked != null && firstControl == currentChildElementTracked.Current.BoundingRectangle)
                    break;
                else if (currentCount >= maxLoopCount)
                    break;
                currentCount++;


            } while (childFocusTracked == null);

            UnsubscribeFocusChange();
            firstChanceDone = false;
            return childFocusTracked;
        }
        private static AutomationFocusChangedEventHandler focusHandler = null;

        /// <summary> 
        /// Create an event handler and register it. 
        /// </summary> 
        private static void SubscribeToFocusChange()
        {
            focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }

        /// <summary> 
        /// Handle the event. 
        /// </summary> 
        /// <param name="src">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            // The arguments tell you which elements have lost and received focus.
            // Make sure the element still exists. Elements such as tooltips 
            // can disappear before the event is processed.
            AutomationElement sourceElement;
            try
            {
                //System.Threading.Thread.Sleep(100);
                sourceElement = src as AutomationElement;


                currentChildElementTracked = sourceElement;
                if (firstControl.IsEmpty)
                    firstControl = sourceElement.Current.BoundingRectangle;



                //Pen pen = new Pen(Color.Red,10);
                //Rectangle rect = new Rectangle((int)sourceElement.Current.BoundingRectangle.X,
                //    (int)sourceElement.Current.BoundingRectangle.Y, (int)sourceElement.Current.BoundingRectangle.Width,
                //    (int)sourceElement.Current.BoundingRectangle.Height);

                //drawingObj.DrawRectangle(pen, rect);

                //LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.BoundingRectangle.X", sourceElement.Current.BoundingRectangle.X.ToString());
                //LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.BoundingRectangle.Y",  sourceElement.Current.BoundingRectangle.Y.ToString());
                //LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.BoundingRectangle.Width", sourceElement.Current.BoundingRectangle.Width.ToString());
                //LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.BoundingRectangle.Height", sourceElement.Current.BoundingRectangle.Height.ToString());


            }
            catch (ElementNotAvailableException e1)
            {
                LogHandler.LogError(e1.InnerException.Message + ":" + e1.Message, LogHandler.Layer.Business);
                return;
            }
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "controlToTrack.AutomationId", controlToTrack.AutomationId);
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.AutomationId", sourceElement.Current.AutomationId);
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "controlToTrack.AutomationName", controlToTrack.AutomationName);
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.Name", sourceElement.Current.Name);
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.ControlType", sourceElement.Current.ControlType.ProgrammaticName);
            LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "sourceElement.Current.IsKeyboardFocusable", sourceElement.Current.IsKeyboardFocusable.ToString());

            if ((controlToTrack.AutomationId == sourceElement.Current.AutomationId) &&
                (controlToTrack.AutomationName == sourceElement.Current.Name))
            {
                childFocusTracked = sourceElement;

                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "childFocusTracked.Current.AutomationId", childFocusTracked.Current.AutomationId);
                LogHandler.LogInfo(Logging.InformationMessages.RUNTIMEWRAPPER_VARIABLE_VALUE, LogHandler.Layer.Business, "childFocusTracked.Current.Name", childFocusTracked.Current.Name);

            }

            arEvent.Set();
        }

        /// <summary> 
        /// Cancel subscription to the event. 
        /// </summary> 
        private static void UnsubscribeFocusChange()
        {
            if (focusHandler != null)
            {
                Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
            }
        }
        static Hashtable elementGraph = new Hashtable();
        private static AutomationElement ScanXaxix(double x, double maxX, double y, int offset, AutomationElement parent, string appPathTobeTraversed)
        {
            using (LogHandler.TraceOperations(Logging.InformationMessages.RUNTIMEWRAPPER_ENTER, LogHandler.Layer.Infrastructure, Guid.Empty,
                    className, Logging.Constants.SCANXAXIX))
            {
                AutomationElement child = null;
                //commenting the 'if' part because it may be possible that two elements may be at the same level but under different iframe like container controls

                //maintain the hastable with key as the app tree path and value as the bounding rectangle
                //check if the element is alreday one of those checked
                //if (elementGraph.ContainsKey(appPathTobeTraversed))
                //{
                //    System.Windows.Rect region = (System.Windows.Rect)elementGraph[appPathTobeTraversed];
                //    child = AutomationElement.FromPoint(new System.Windows.Point(region.X, region.Y));
                //}
                //else
                //{
                while (x <= maxX && child == null)
                {
                    AutomationElement tempchild = AutomationElement.FromPoint(new System.Windows.Point(x, y));
                    if (tempchild != null)
                    {
                        string tempApppath = UpdateStartingLevel(appPathTobeTraversed, GetAppPathUntilParent(tempchild, parent));
                        if (!string.IsNullOrEmpty(tempApppath))
                        {
                            //elementGraph.Add(tempApppath, tempchild.Current.BoundingRectangle); //commented because the 'if' part using elementGraph is commented
                            if (tempApppath == appPathTobeTraversed)
                                child = tempchild;
                        }
                    }
                    x += offset;
                }
                //}
                return child;
            }
        }

        private static string UpdateStartingLevel(string sourcePath, string targetPath)
        {
            //sourcePath e.g. /3[2]/4[6]/5[1]
            //targetPath e.g. /0[1]/1[6]/2[2]
            if (!string.IsNullOrEmpty(sourcePath) && !string.IsNullOrEmpty(targetPath))
            {
                string[] sourcePathParts = sourcePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (sourcePathParts.Length > 0)
                {
                    string sourcePathPart = sourcePathParts[0]; //e.g. 3[2]
                    int level = int.Parse(sourcePathPart.Split('[')[0]); //e.g. 3

                    //update targetPath starting from "level"
                    string[] targetPathParts = targetPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> peerIndexes = new List<string>();
                    foreach (string st in targetPathParts)
                    {
                        string[] sts = st.Substring(0, st.Length - 1).Split('[');
                        peerIndexes.Add(sts[1]);
                    }
                    peerIndexes.Reverse();
                    targetPath = "";
                    for (int i = 0; i < peerIndexes.Count; i++)
                    {
                        targetPath += "/" + level + "[" + peerIndexes[i] + "]";
                        level++;
                    }
                }
            }
            return targetPath;
        }

        private static string GetAppPathUntilParent(AutomationElement child, AutomationElement foremostParent, string curAppTree = "")
        {
            //get the parent
            AutomationElement parent = TreeWalker.ControlViewWalker.GetParent(child);
            if (parent != null)
            {
                //get all the children to understand current passed element's index
                AutomationElementCollection children = parent.FindAll(TreeScope.Children, Condition.TrueCondition);
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] == child)
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
                if (parent != foremostParent)
                    curAppTree = GetAppPathUntilParent(parent, foremostParent, curAppTree);
            }
            else
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

        public static Infosys.JavaAccessBridge.EX_JABHelper.AccessibleTreeItem MatchForJavaApplicationTreePath(string appTreePath, IntPtr appWinHandle)
        {
            Infosys.JavaAccessBridge.EX_JABHelper.AccessibleTreeItem javaElement = null;
            if (string.IsNullOrEmpty(appTreePath))
                return javaElement;
            if (IntPtr.Zero != appWinHandle)
            {
                Int32 vmid = 0;
                Infosys.JavaAccessBridge.EX_JABHelper.AccessibleTreeItem javaApp = Infosys.JavaAccessBridge.EX_JABHelper.GetComponentTree(appWinHandle, out vmid);
                Infosys.JavaAccessBridge.EX_JABHelper.AccessibleTreeItem tempElement = javaApp;
                if (javaApp != null)
                {
                    string[] pathParts = appTreePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (pathParts.Length > 0)
                    {
                        for (int i = 1; i < pathParts.Length; i++)
                        {
                            //starting from second path part i.e. i=1 because the appWindow corresponds to the the first path part i.e. 0[0]
                            if (tempElement != null)
                            {
                                string[] sts = pathParts[i].Substring(0, pathParts[i].Length - 1).Split('[');
                                int index = int.Parse(sts[1]);
                                List<Infosys.JavaAccessBridge.EX_JABHelper.AccessibleTreeItem> children = tempElement.children;
                                if (index < children.Count)
                                    tempElement = children[index];
                                else
                                    tempElement = null;
                            }
                        }
                        javaElement = tempElement;
                    }
                }
            }
            return javaElement;
        }

        public static void HoverMouseTo(int x, int y)
        {
            SetCursorPos(x, y);
        }

        //public static void SendText(string Text)
        //{
        //    //SendKeys.SendWait(keys);
        //    SendText(Text);
        //}

        #region old key press wioth modifier

        //public static void KeyPress(string text, int modifier)
        //{
        //    Keys key;
        //    if (KeyModifiers.ContainsKey(modifier))
        //    {
        //        //if(modifier == KeyModifier.CTRL)
        //        //    keybd_event(VK_LCONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //        //else if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
        //        //    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);

        //        //passing 0 instead of KEYEVENTF_EXTENDEDKEY as KEYEVENTF_EXTENDEDKEY was having issue with CTRL
        //        //issue was- ctrl was getting pressed but not released

        //        if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
        //            keybd_event((byte)key, 0, 0, 0);
        //    }
        //    if (!string.IsNullOrEmpty(text))
        //    {
        //        foreach (char c in text.ToUpper())
        //        {
        //            //special char is kept differently then alpha-nuemeric so that modifiers will affect the alpha-nuemeric key press
        //            if (char.IsLetter(c))
        //            {
        //                if (Enum.TryParse(c.ToString(), out key))
        //                {
        //                    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //                    keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        //                    System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
        //                }
        //            }
        //            else if (char.IsDigit(c))
        //            {
        //                if (Enum.TryParse("D" + c.ToString(), out key))
        //                {
        //                    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //                    keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        //                    System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
        //                }
        //            }
        //            else
        //            {
        //                SendKeys.SendWait(c.ToString());
        //            }
        //        }
        //    }
        //    if (KeyModifiers.ContainsKey(modifier))
        //    {
        //        //if(modifier == KeyModifier.CTRL)
        //        //    keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);
        //        //else if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
        //        //    keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        //        if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
        //            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);

        //    }
        //    //keybd_event((byte)Keys.RControlKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //    //keybd_event((byte)Keys.RControlKey, 0, KEYEVENTF_KEYUP, 0);
        //}

        #endregion

        #region old key press with modifier
        //public static void KeyPress(string text, params int[] modifiers)
        //{
        //    Keys key;
        //    foreach (int modifier in modifiers)
        //    {
        //        if (KeyModifiers.ContainsKey(modifier))
        //        {
        //            //if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
        //            //    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //            //passing 0 instead of KEYEVENTF_EXTENDEDKEY as KEYEVENTF_EXTENDEDKEY was having issue with CTRL
        //            //issue was- ctrl was getting pressed but not released

        //            if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
        //                keybd_event((byte)key, 0, 0, 0);
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(text))
        //    {
        //        foreach (char c in text.ToUpper())
        //        {
        //            //special char is kep differently then alpha-nuemeric so that modifiers will affect the alpha-nuemeric key press
        //            if (char.IsLetter(c))
        //            {
        //                if (Enum.TryParse(c.ToString(), out key))
        //                {
        //                    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //                    keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        //                    System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
        //                }
        //            }
        //            else if (char.IsDigit(c))
        //            {
        //                if (Enum.TryParse("D" + c.ToString(), out key))
        //                {
        //                    keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //                    keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        //                    System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
        //                }
        //            }
        //            else
        //            {
        //                SendKeys.SendWait(c.ToString());
        //            }
        //        }
        //    }
        //    for (int i = modifiers.Length - 1; i >= 0; i--)
        //    {
        //        if (KeyModifiers.ContainsKey(modifiers[i]))
        //        {
        //            if (Enum.TryParse(KeyModifiers[modifiers[i]].ToString(), out key))
        //                keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        //        }
        //    }
        //    //foreach (int modifier in modifiers)
        //    //{
        //    //    if (KeyModifiers.ContainsKey(modifier))
        //    //    {
        //    //        if (Enum.TryParse(KeyModifiers[modifier].ToString(), out key))
        //    //            keybd_event((byte)key, 0, KEYEVENTF_KEYUP, 0);
        //    //    }
        //    //}
        //}

        #endregion
        /// <summary>
        /// This method is used to pass text along with modifiers. Text is of only one character e.g. CTRL and "V"
        /// </summary>
        /// <param name="text">Text to be passed</param>
        /// <param name="modifiers">Array of Modifier Values in int-Refer to the API user guide for the list of valid key codes</param>
        //public static void KeyPress(string text, params int[] modifiers)
        //{
        //    List<int> key = new List<int>();
        //    Keys kbKey;

        //    //// Add all the modifiers to array list
        //    if (modifiers != null && modifiers.Length > 0)
        //    {
        //        for (int i = 0; i < modifiers.Length; i++)
        //        {
        //            key.Add(modifiers[i]);
        //        }

        //        //then convert the text to upper case, as text with modifiers shud be always in upper case
        //        if (!string.IsNullOrEmpty(text))
        //            text = text.ToUpper();
        //    }

        //    // If user passed specialy key as text, add the same to special key array list
        //    if (!string.IsNullOrEmpty(text))
        //    {
        //        if (Enum.TryParse(text, out kbKey))
        //            key.Add((int)kbKey);
        //        else
        //            SendText(text);
        //    }
        //    INPUT[] inputs = new INPUT[(key.Count * 2) + 4]; //1

        //    for (int i = 0; i < key.Count; i++)//1
        //    {
        //        switch (key[i])
        //        {
        //            case (int)Keys.LWin:
        //            case (int)Keys.RWin:
        //            case (int)Keys.PageUp:
        //            case (int)Keys.PageDown:
        //            case (int)Keys.Up:
        //            case (int)Keys.Down:
        //            case (int)Keys.Insert:
        //            case (int)Keys.Delete:
        //            case (int)Keys.Home:
        //            case (int)Keys.End:
        //            case (int)Keys.Left:
        //            case (int)Keys.Right:
        //            case (int)Keys.NumLock:
        //            case (int)Keys.Pause:
        //            case (int)Keys.PrintScreen:
        //                {
        //                    short svkey = (short)key[i];
        //                    short skey = VkKeyScan((char)key[i]);
        //                    inputs[i].type = INPUTType.INPUT_KEYBOARD;
        //                    inputs[i].Event.ki.dwFlags = KEYEVENTF.EXTENDEDKEY;
        //                    inputs[i].Event.ki.wVk = (ushort)svkey;
        //                    inputs[i].Event.ki.wScan = (ushort)skey;
        //                    break;
        //                }
        //            default:
        //                {
        //                    uint skey = MapVirtualKey((uint)key[i], (uint)0x0);
        //                    inputs[i].type = INPUTType.INPUT_KEYBOARD;
        //                    inputs[i].Event.ki.dwFlags = KEYEVENTF.SCANCODE;
        //                    inputs[i].Event.ki.wScan = (ushort)skey;
        //                    break;
        //                }

        //        }
        //    }


        //    inputs[key.Count].type = INPUTType.INPUT_KEYBOARD;
        //    inputs[key.Count].Event.ki.dwFlags = KEYEVENTF.UNICODE;
        //    inputs[key.Count].Event.ki.dwFlags |= KEYEVENTF.KEYUP;

        //    uint ctrlkey = MapVirtualKey((uint)Keys.ControlKey, (uint)0x0);
        //    inputs[key.Count + 3].type = INPUTType.INPUT_KEYBOARD;
        //    inputs[key.Count + 3].Event.ki.dwFlags = KEYEVENTF.SCANCODE;
        //    inputs[key.Count + 3].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
        //    inputs[key.Count + 3].Event.ki.wScan = (ushort)ctrlkey;
        //    uint shftkey = MapVirtualKey((uint)Keys.ShiftKey, (uint)0x0);
        //    inputs[key.Count + 1].type = INPUTType.INPUT_KEYBOARD;
        //    inputs[key.Count + 1].Event.ki.dwFlags = KEYEVENTF.SCANCODE;
        //    inputs[key.Count + 1].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
        //    inputs[key.Count + 1].Event.ki.wScan = (ushort)shftkey;
        //    uint altkey = MapVirtualKey((uint)Keys.Menu, (uint)0x0);
        //    inputs[key.Count + 2].type = INPUTType.INPUT_KEYBOARD;
        //    inputs[key.Count + 2].Event.ki.dwFlags = KEYEVENTF.SCANCODE;
        //    inputs[key.Count + 2].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
        //    inputs[key.Count + 2].Event.ki.wScan = (ushort)altkey;

        //    for (int i = 0; i < key.Count; i++)//1
        //    {
        //        switch (key[i])
        //        {
        //            case (int)Keys.LWin:
        //            case (int)Keys.RWin:
        //            case (int)Keys.PageUp:
        //            case (int)Keys.PageDown:
        //            case (int)Keys.Up:
        //            case (int)Keys.Down:
        //            case (int)Keys.Insert:
        //            case (int)Keys.Delete:
        //            case (int)Keys.Home:
        //            case (int)Keys.End:
        //            case (int)Keys.Left:
        //            case (int)Keys.Right:
        //            case (int)Keys.NumLock:
        //            case (int)Keys.Pause:
        //            case (int)Keys.PrintScreen:
        //                {
        //                    short svkey = (short)key[i];
        //                    short skey = VkKeyScan((char)key[i]);
        //                    inputs[key.Count + 4 + i].type = INPUTType.INPUT_KEYBOARD;
        //                    inputs[key.Count + 4 + i].Event.ki.dwFlags = KEYEVENTF.EXTENDEDKEY;
        //                    inputs[key.Count + 4 + i].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
        //                    inputs[key.Count + 4 + i].Event.ki.wVk = (ushort)svkey;
        //                    inputs[key.Count + 4 + i].Event.ki.wScan = (ushort)skey;
        //                    break;
        //                }
        //            default:
        //                {
        //                    break;
        //                }
        //        }
        //    }

        //    foreach (var stroke in inputs)
        //    {
        //        //uint cSuccess = SendInput(inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        //        uint cSuccess = SendInput(1, new INPUT[]{stroke} , Marshal.SizeOf(typeof(INPUT)));

        //        //if (cSuccess != inputs.Length)
        //        //{
        //        //    throw new Win32Exception();
        //        //}
        //        System.Threading.Thread.Sleep(1000);
        //    }

        //}
        public static void KeyPress(string text, params int[] modifiers)
        {
            List<int> key = new List<int>();
            Keys kbKey;

            //// Add all the modifiers to array list
            if (modifiers != null && modifiers.Length > 0)
            {
                for (int i = 0; i < modifiers.Length; i++)
                {
                    key.Add(modifiers[i]);
                }

                //then convert the text to upper case, as text with modifiers shud be always in upper case

            }

            // If user passed specialy key as text, add the same to special key array list
            if (!string.IsNullOrEmpty(text))
            {

                text = text.ToUpper();

                if (Enum.TryParse(text, out kbKey))
                {
                    key.Add((int)kbKey);
                }
                else
                {
                    throw new Exception("Please use Send Text Activity to Send the text");
                }

            }
            INPUT[] inputs = new INPUT[(key.Count * 2) + 4]; //1

            for (int i = 0; i < key.Count; i++)//1
            {
                switch (key[i])
                {
                    case (int)Keys.LWin:
                    case (int)Keys.RWin:
                    case (int)Keys.PageUp:
                    case (int)Keys.PageDown:
                    case (int)Keys.Up:
                    case (int)Keys.Down:
                    case (int)Keys.Insert:
                    case (int)Keys.Delete:
                    case (int)Keys.Home:
                    case (int)Keys.End:
                    case (int)Keys.Left:
                    case (int)Keys.Right:
                    case (int)Keys.NumLock:
                    case (int)Keys.Pause:
                    case (int)Keys.PrintScreen:
                        {
                            short svkey = (short)key[i];
                            short skey = VkKeyScan((char)key[i]);
                            inputs[i].type = INPUTType.INPUT_KEYBOARD;
                            inputs[i].Event.ki.dwFlags = KEYEVENTF.EXTENDEDKEY;
                            inputs[i].Event.ki.wVk = (ushort)svkey;
                            inputs[i].Event.ki.wScan = (ushort)skey;
                            break;
                        }

                    default:
                        {
                            short svkey = (short)key[i];
                            uint skey = MapVirtualKey((uint)key[i], (uint)0x0);
                            inputs[i].type = INPUTType.INPUT_KEYBOARD;
                            inputs[i].Event.ki.dwFlags = KEYEVENTF.SCANCODE;
                            inputs[i].Event.ki.wScan = (ushort)skey;
                            inputs[i].Event.ki.wVk = (ushort)svkey;
                            break;
                        }

                }
            }



            uint ctrlkey = MapVirtualKey((uint)Keys.ControlKey, (uint)0x0);
            inputs[key.Count + 3].type = INPUTType.INPUT_KEYBOARD;

            inputs[key.Count + 3].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
            inputs[key.Count + 3].Event.ki.wScan = (ushort)ctrlkey;
            uint shftkey = MapVirtualKey((uint)Keys.ShiftKey, (uint)0x0);
            inputs[key.Count + 1].type = INPUTType.INPUT_KEYBOARD;

            inputs[key.Count + 1].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
            inputs[key.Count + 1].Event.ki.wScan = (ushort)shftkey;
            uint altkey = MapVirtualKey((uint)Keys.Menu, (uint)0x0);
            inputs[key.Count + 2].type = INPUTType.INPUT_KEYBOARD;

            inputs[key.Count + 2].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
            inputs[key.Count + 2].Event.ki.wScan = (ushort)altkey;

            for (int i = 0; i < key.Count; i++)//1
            {
                switch (key[i])
                {
                    case (int)Keys.LWin:
                    case (int)Keys.RWin:
                    case (int)Keys.PageUp:
                    case (int)Keys.PageDown:
                    case (int)Keys.Up:
                    case (int)Keys.Down:
                    case (int)Keys.Insert:
                    case (int)Keys.Delete:
                    case (int)Keys.Home:
                    case (int)Keys.End:
                    case (int)Keys.Left:
                    case (int)Keys.Right:
                    case (int)Keys.NumLock:
                    case (int)Keys.Pause:
                    case (int)Keys.PrintScreen:
                        {
                            short svkey = (short)key[i];
                            short skey = VkKeyScan((char)key[i]);
                            inputs[key.Count + 4 + i].type = INPUTType.INPUT_KEYBOARD;
                            inputs[key.Count + 4 + i].Event.ki.dwFlags = KEYEVENTF.EXTENDEDKEY;
                            inputs[key.Count + 4 + i].Event.ki.dwFlags |= KEYEVENTF.KEYUP;
                            inputs[key.Count + 4 + i].Event.ki.wVk = (ushort)svkey;
                            inputs[key.Count + 4 + i].Event.ki.wScan = (ushort)skey;
                            break;
                        }
                    default:
                        {
                            short svkey = (short)key[i];
                            uint skey = MapVirtualKey((uint)key[i], (uint)0x0);
                            inputs[key.Count + 4 + i].type = INPUTType.INPUT_KEYBOARD;
                            //  inputs[key.Count + 4 + i].Event.ki.dwFlags = KEYEVENTF.SCANCODE;
                            inputs[key.Count + 4 + i].Event.ki.dwFlags = KEYEVENTF.KEYUP;
                            inputs[key.Count + 4 + i].Event.ki.wScan = (ushort)skey;
                            inputs[key.Count + 4 + i].Event.ki.wVk = (ushort)svkey;
                            break;
                        }
                }
            }


            uint cSuccess = SendInput(inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));


        }


        public static void KeyDown(int modifier)
        {
            Keys vKey;
            if (Enum.TryParse(modifier.ToString(), out vKey))
            {
                switch ((int)vKey)
                {
                    case (int)Keys.LWin:
                    case (int)Keys.RWin:
                    case (int)Keys.PageUp:
                    case (int)Keys.PageDown:
                    case (int)Keys.Up:
                    case (int)Keys.Down:
                    case (int)Keys.Insert:
                    case (int)Keys.Delete:
                    case (int)Keys.Home:
                    case (int)Keys.End:
                    case (int)Keys.Left:
                    case (int)Keys.Right:
                    case (int)Keys.NumLock:
                    case (int)Keys.Pause:
                    case (int)Keys.PrintScreen:
                        {
                            uint skey = MapVirtualKey((uint)vKey, (uint)0x0);
                            keybd_event((byte)vKey, (byte)skey, KEYEVENTF_EXTENDEDKEY, 0);
                            break;
                        }
                    default:
                        {
                            uint skey = MapVirtualKey((uint)vKey, (uint)0x0);
                            keybd_event(0, (byte)skey, KEYEVENTF_SCANCODE, 0);
                            break;
                        }
                }
            }

        }

        public static void KeyUp(int modifier)
        {
            Keys vKey;
            if (Enum.TryParse(modifier.ToString(), out vKey))
            {
                switch ((int)vKey)
                {
                    case (int)Keys.LWin:
                    case (int)Keys.RWin:
                    case (int)Keys.PageUp:
                    case (int)Keys.PageDown:
                    case (int)Keys.Up:
                    case (int)Keys.Down:
                    case (int)Keys.Insert:
                    case (int)Keys.Delete:
                    case (int)Keys.Home:
                    case (int)Keys.End:
                    case (int)Keys.Left:
                    case (int)Keys.Right:
                    case (int)Keys.NumLock:
                    case (int)Keys.Pause:
                    case (int)Keys.PrintScreen:
                        {
                            uint skey = MapVirtualKey((uint)vKey, (uint)0x0);
                            keybd_event((byte)vKey, (byte)skey, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
                            break;
                        }
                    default:
                        {
                            uint skey = MapVirtualKey((uint)vKey, (uint)0x0);
                            keybd_event(0, (byte)skey, KEYEVENTF_SCANCODE | KEYEVENTF_KEYUP, 0);
                            break;
                        }
                }
            }
        }

        public static void SendText(string text)
        {
            List<INPUT> kbInput = new List<INPUT>();

            foreach (char c in text)
            {
                // Send a key down followed by key up.
                foreach (bool keyUp in new bool[] { false, true })
                {
                    INPUT input = new INPUT
                    {
                        type = INPUTType.INPUT_KEYBOARD,
                        Event = new INPUTUnion
                        {
                            // This will contain keyboard event information
                            ki = new KEYBDINPUT
                            {
                                wVk = 0,
                                wScan = c,
                                dwFlags = KEYEVENTF.UNICODE | (keyUp ? KEYEVENTF.KEYUP : 0),
                                dwExtraInfo = GetMessageExtraInfo(),
                            }
                        }
                    };

                    kbInput.Add(input);
                }
            }

            // Call SendInputWindows API to send input
            SendInput((int)kbInput.Count, kbInput.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        public static ControlImageReference GetBoundingRectangle(ControlImageReference imageRef, bool ifImageThenOriginalScale = false, bool ifImageThenWaitIdenfForTemp = false, bool useTrueColorTemplateMatching = false, object searchRegion = null, string atrFolder = "", Stream sourceImageToMatch = null)
        {
            System.Windows.Rect rect = System.Windows.Rect.Empty;
            imageRef.CurrentBoundingRectangle = rect;
            bool requestedWaitforeverFlag = ifImageThenWaitIdenfForTemp;
            //identify the bounding rectangle using the image references
            //e.g. using emgucv
            if (imageRef != null && imageRef.SupportedStates != null && imageRef.SupportedStates.Count > 0)
            {
                //if there are more than one state, then wait for ever is disabled
                //otherwise in case the first tried start is not found, it will not search for the second state.
                if (imageRef.SupportedStates.Count > 1)
                    ifImageThenWaitIdenfForTemp = requestedWaitforeverFlag = false;

                do
                {
                    foreach (var item in imageRef.SupportedStates)
                    {
                        //if the image path is not complete, then it could be relative, conbine it with the folder containing the atr file
                        if (!string.IsNullOrEmpty(item.ImagePath) && item.ImagePath.StartsWith("$"))
                            item.ImagePath = item.ImagePath.Replace("$", atrFolder);

                        if (ifImageThenWaitIdenfForTemp)
                            rect = WaitForElement(item.ImagePath, ifImageThenOriginalScale, useTrueColorTemplateMatching, sourceImageToMatch);
                        else
                        {

                            if (useTrueColorTemplateMatching)
                                rect = FindElementInTrueColor(item.ImagePath, DEFAULT_TIMEOUT, DEFAULT_CONFIDENCE, !ifImageThenOriginalScale, searchRegion, sourceImageToMatch);
                            else
                                rect = FindElement(item.ImagePath, DEFAULT_TIMEOUT, DEFAULT_CONFIDENCE, !ifImageThenOriginalScale, searchRegion, sourceImageToMatch);
                        }
                        if (rect != System.Windows.Rect.Empty)
                        {
                            imageRef.CurrentState = item.State;
                            imageRef.CurrentBoundingRectangle = rect;
                            break;
                        }
                    }
                }
                while (requestedWaitforeverFlag && imageRef.CurrentBoundingRectangle == System.Windows.Rect.Empty);
            }

            return imageRef; //make sure to return empty rectangle in case the bounding rectangle cant be identified
        }

        /// <summary>
        /// The method tries to identify the provided template in the screen shot to be taken.
        /// And then accordingly returns the rectangle.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <param name="multipleScaleMatching">The flag to dictate if the multiple scale template matching is to be followed, default is true</param>
        /// <param name="searchRegion">Optional parameter- the region inside which the template is to be searched</param>
        /// <param name="sourceImageToMatch">Optional parameter- Path of the source image in which the template element is to be recognized. Used if the image recognition is to be run in background mode. 
        /// If no value is set then a screen shot of the current user screen will be taken and used to identify the template element </param>
        /// <returns>The Rectangle object if match is found</returns>
        public static System.Windows.Rect FindElement(string filename, int timeout = DEFAULT_TIMEOUT, double confidence = 80, bool multipleScaleMatching = true, object searchRegion = null, Stream sourceImageToMatch = null)
        {
            //check if stop requested, if show then throw exception
            if (Core.Utilities.IsStopRequested())
                throw new Core.IAPExceptions.StopRequested();


            System.Windows.Rect elementRectTemp = System.Windows.Rect.Empty;
            System.Windows.Rect searchRect = System.Windows.Rect.Empty;
            DateTime startTime = DateTime.Now;
            try
            {
                if (timeout <= 0)
                    timeout = DEFAULT_TIMEOUT; //i.e. assigning the default time period to say 10 sec
                if (System.IO.File.Exists(filename) || (IapwPackage != null && IapwPackage.Length > 0))
                {
                    Image<Gray, byte> template = null;
                    if (IapwPackage == null)
                        template = new Image<Gray, byte>(filename);
                    else
                    {
                        //get the image stream from the iapw stream
                        Stream templateStream = Infosys.ATR.Packaging.Operations.ExtractFile(IapwPackage, filename);
                        template = new Image<Gray, byte>(new Bitmap(templateStream));
                        Infosys.ATR.Packaging.Operations.ClosePackage();
                    }
                    bool backgroundProcessing = false;
                    while ((System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000 && elementRectTemp == System.Windows.Rect.Empty)
                    {
                        if (searchRegion != null && (System.Windows.Rect)searchRegion != System.Windows.Rect.Empty)
                        {
                            searchRect = (System.Windows.Rect)searchRegion;
                        }
                        Image<Gray, byte> source = null;
                        if (sourceImageToMatch != null)
                        {
                            source = new Image<Gray, byte>(new Bitmap(sourceImageToMatch));
                            backgroundProcessing = true;
                        }
                        else
                        {
                            source = GetGrayScreenShot(searchRect);
                        }
                        if (multipleScaleMatching)
                        {
                            //scale up/down the template
                            int direction = 1;
                            for (int i = 0; i <= MaxScaleSteps; i++)
                            {
                                //scale up and verify for confidence
                                double scale = 1 + direction * i * ScaleStep;
                                Image<Gray, byte> templateTemp = ResizeTemplate(template, scale);
                                if (templateTemp != null)
                                {
                                    elementRectTemp = FindRectangle(source, templateTemp, confidence);
                                    if (elementRectTemp != System.Windows.Rect.Empty)
                                    {
                                        break;
                                    }
                                }

                                //then scale down and verify for confidence
                                scale = 1 + (-direction) * i * ScaleStep;
                                templateTemp = ResizeTemplate(template, scale);
                                if (templateTemp != null)
                                {
                                    elementRectTemp = FindRectangle(source, templateTemp, confidence);
                                    if (elementRectTemp != System.Windows.Rect.Empty)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            elementRectTemp = FindRectangle(source, template, confidence);
                            if (elementRectTemp != System.Windows.Rect.Empty)
                            {
                                break;
                            }
                        }
                        System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                    }
                }
                else
                    throw new System.IO.FileNotFoundException("Image Template " + filename + " not found");
                if (elementRectTemp == System.Windows.Rect.Empty)
                    throw new Exception("Could not match the template provided in this trial. Probably in the subsequent trial, the template would be matched.");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                //log exception
                string exMessage = ex.Message;
                string innerExMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                LogHandler.LogError(string.Format(Logging.ErrorMessages.EXCEPTION, "FindElement", exMessage, innerExMessage), LogHandler.Layer.Business);
                throw ex;
            }
            catch (Exception ex)
            {
                //log exception
                string exMessage = ex.Message;
                string innerExMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                LogHandler.LogError(string.Format(Logging.ErrorMessages.EXCEPTION, "FindElement", exMessage, innerExMessage), LogHandler.Layer.Business);
                //dont throw any other kind of exception, just empty rectangle will be sent.
            }

            //offset the found rectangle by the search region if provided
            if (searchRegion != null && searchRect != System.Windows.Rect.Empty)
            {
                elementRectTemp.X += searchRect.X;
                elementRectTemp.Y += searchRect.Y;
            }
            return elementRectTemp;
        }

        /// <summary>
        /// In true color, the method tries to identify the provided template in the screen shot to be taken.
        /// And then accordingly returns the rectangle.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <param name="multipleScaleMatching">The flag to dictate if the multiple scale template matching is to be followed, default is true</param>
        /// <param name="searchRegion">Optional parameter- the region inside which the template is to be searched</param>
        /// <param name="sourceImageToMatch">Optional parameter- Path of the source image in which the template element is to be recognized. Used if the image recognition is to be run in background mode. 
        /// If no value is set then a screen shot of the current user screen will be taken and used to identify the template element </param>
        /// <returns>The Rectangle object if match is found</returns>
        public static System.Windows.Rect FindElementInTrueColor(string filename, int timeout = DEFAULT_TIMEOUT, double confidence = 80, bool multipleScaleMatching = true, object searchRegion = null, Stream sourceImageToMatch = null)
        {
            //check if stop requested, if show then throw exception
            if (Core.Utilities.IsStopRequested())
                throw new Core.IAPExceptions.StopRequested();

            System.Windows.Rect elementRectTemp = System.Windows.Rect.Empty;
            System.Windows.Rect searchRect = System.Windows.Rect.Empty;
            DateTime startTime = DateTime.Now;
            try
            {
                if (timeout <= 0)
                    timeout = DEFAULT_TIMEOUT; //i.e. assigning the default time period to say 10 sec
                if (System.IO.File.Exists(filename) || (IapwPackage != null && IapwPackage.Length > 0))
                {
                    Image<Bgr, byte> template = null;
                    if (IapwPackage == null)
                        template = new Image<Bgr, byte>(filename);
                    else
                    {
                        //get the image stream from the iapw stream
                        Stream templateStream = Infosys.ATR.Packaging.Operations.ExtractFile(IapwPackage, filename);
                        template = new Image<Bgr, byte>(new Bitmap(templateStream));
                        Infosys.ATR.Packaging.Operations.ClosePackage();
                    }
                    bool backgroundProcessing = false;
                    while ((System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000 && elementRectTemp == System.Windows.Rect.Empty)
                    {
                        if (searchRegion != null && (System.Windows.Rect)searchRegion != System.Windows.Rect.Empty)
                        {
                            searchRect = (System.Windows.Rect)searchRegion;
                        }
                        Image<Bgr, byte> source = null;
                        if (sourceImageToMatch != null)
                        {
                            source = new Image<Bgr, byte>(new Bitmap(sourceImageToMatch));
                            backgroundProcessing = true;
                        }
                        else
                        {
                            source = GetTrueColorScreenShot(searchRect);
                        }
                        if (multipleScaleMatching)
                        {
                            //scale up/down the template
                            int direction = 1;
                            for (int i = 0; i <= MaxScaleSteps; i++)
                            {
                                //scale up and verify for confidence
                                double scale = 1 + direction * i * ScaleStep;
                                Image<Bgr, byte> templateTemp = ResizeTemplateInTrueColor(template, scale);
                                if (templateTemp != null)
                                {
                                    elementRectTemp = FindRectangleInTrueColor(source, templateTemp, confidence);
                                    if (elementRectTemp != System.Windows.Rect.Empty)
                                    {
                                        break;
                                    }
                                }

                                //then scale down and verify for confidence
                                scale = 1 + (-direction) * i * ScaleStep;
                                templateTemp = ResizeTemplateInTrueColor(template, scale);
                                if (templateTemp != null)
                                {
                                    elementRectTemp = FindRectangleInTrueColor(source, templateTemp, confidence);
                                    if (elementRectTemp != System.Windows.Rect.Empty)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            elementRectTemp = FindRectangleInTrueColor(source, template, confidence);
                            if (elementRectTemp != System.Windows.Rect.Empty)
                            {
                                break;
                            }
                        }
                        System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                    }
                }
                else
                    throw new System.IO.FileNotFoundException("Image Template " + filename + " not found");

                if (elementRectTemp == System.Windows.Rect.Empty)
                    throw new Exception("Could not match the template provided in this trial. Probably in the subsequent trial, the template would be matched.");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                //log exception
                string exMessage = ex.Message;
                string innerExMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                LogHandler.LogError(string.Format(Logging.ErrorMessages.EXCEPTION, "FindElementInTrueColor", exMessage, innerExMessage), LogHandler.Layer.Business);
                throw ex;
            }
            catch (Exception ex)
            {
                //log exception
                string exMessage = ex.Message;
                string innerExMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                LogHandler.LogError(string.Format(Logging.ErrorMessages.EXCEPTION, "FindElementInTrueColor", exMessage, innerExMessage), LogHandler.Layer.Business);
                //dont throw any other kind of exception so that empty rectangle will be returned.
            }

            //offset the found rectangle by the search region if provided
            if (searchRegion != null && searchRect != System.Windows.Rect.Empty)
            {
                elementRectTemp.X += searchRect.X;
                elementRectTemp.Y += searchRect.Y;
            }
            return elementRectTemp;
        }

        public static System.Windows.Rect WaitForElement(string filename, bool templateMachingInOriginalScale = false, bool useTrueColorTemplateMatching = false, object searchRegion = null, Stream sourceImageToMatch = null)
        {
            System.Windows.Rect elementRectTemp = System.Windows.Rect.Empty;
            while (elementRectTemp == System.Windows.Rect.Empty)
            {
                try
                {
                    if (useTrueColorTemplateMatching)
                        elementRectTemp = FindElementInTrueColor(filename, DEFAULT_TIMEOUT, DEFAULT_CONFIDENCE, !templateMachingInOriginalScale, searchRegion, sourceImageToMatch);
                    else
                        elementRectTemp = FindElement(filename, DEFAULT_TIMEOUT, DEFAULT_CONFIDENCE, !templateMachingInOriginalScale, searchRegion, sourceImageToMatch);
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    throw ex;
                }
                catch (Core.IAPExceptions.StopRequested ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    //just kill the exception and keep trying
                }
                System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
            }
            return elementRectTemp;
        }

        /// <summary>
        /// Tries to find all the occurance of the template image in the screen shot to be taken.
        /// And then accordingly returns the rectangle collection.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds. If needed to wait for ever, set a -ve value e.g. -1</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <param name="multipleScaleMatching">The flag to dictate if the multiple scale template matching is to be followed, default is true</param>
        /// <param name="sourceImageToMatch">Optional parameter- Path of the source image in which the template element is to be recognized. Used if the image recognition is to be run in background mode. 
        /// If no value is set then a screen shot of the current user screen will be taken and used to identify the template element </param>
        /// <param name="enableTemplateMatchMap">Enable the template matches mapping on source images or screen shot image which is being scanned</param>
        /// <returns>List of Rectangle objects for the matches found</returns>
        public static List<System.Windows.Rect> FindAllInstances(string filename, int timeout = DEFAULT_TIMEOUT_PERINSTANCE, double confidence = 80, bool multipleScaleMatching = true, Stream sourceImageToMatch = null,bool enableTemplateMatchMap= false)
        {
            List<System.Windows.Rect> rects = new List<System.Windows.Rect>();
            List<Point[]> boxes = new List<Point[]>();
            int currentCount = 0;
            bool forever = false;

            System.Windows.Rect[] elementRectTemp;
            DateTime startTime = DateTime.Now;
            Image<Bgr, byte> sourceMatches = null; 
            try
            {
                if (timeout == 0)
                    timeout = DEFAULT_TIMEOUT_PERINSTANCE; //i.e. assigning the default time period to say 10 sec
                if (timeout < 0)
                    forever = true;
                if (System.IO.File.Exists(filename))
                {
                    Image<Gray, byte> template = new Image<Gray, byte>(filename);
                    //template = template.ThresholdBinary(new Gray(160), new Gray(255));
                    //test purpose only Start
                    //template.Save(@"d:\images\template_" + System.DateTime.Now.Ticks + ".jpg");
                    //test purpose only End
                    //System.Windows.Rect elementRectTemp = System.Windows.Rect.Empty;// code to make black and white
                    bool backgroundProcessing = false;
                    //Sid: 25-Dec-2017 - Modified check forever loop and exiting when one control is found
                    while ((forever && currentCount == 0) || (System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000 || (rects.Count - currentCount > 0))
                    {
                        currentCount = rects.Count;
                        if (currentCount > 0)
                            forever = false;
                        Image<Gray, byte> source = null;
                        if (sourceImageToMatch != null)
                        {
                            source = new Image<Gray, byte>(new Bitmap(sourceImageToMatch));
                              
                            backgroundProcessing = true;
                            if (enableTemplateMatchMap)
                            {
                                if (templateMatchMapScreen != null)
                                {

                                    sourceMatches = new Image<Bgr, byte>(new Bitmap(new MemoryStream(templateMatchMapScreen)));
                                }
                                else
                                    sourceMatches = new Image<Bgr, byte>(new Bitmap(sourceImageToMatch)); 
                            }
                        }
                        else
                        {
                            source = GetGrayScreenShot();
                            if (enableTemplateMatchMap)
                            {
                                if (templateMatchMapScreen != null)
                                {

                                    sourceMatches = new Image<Bgr, byte>(new Bitmap(new MemoryStream(templateMatchMapScreen)));
                                }
                                else
                                    sourceMatches = GetTrueColorScreenShot();
                            }
                        }
                        //source = source.ThresholdBinary(new Gray(140), new Gray(255));// code to make black and white
                        //test purpose only Start
                        //source.Save(@"d:\images\screenshot_" + System.DateTime.Now.Ticks + ".jpg");
                        //test purpose only End

                        //then on the source hide all the instances found so far
                        if (boxes != null && boxes.Count > 0)
                        {
                            boxes.ForEach(b =>
                            {
                                source.FillConvexPoly(b, new Gray(0));
                            });
                            boxes.ForEach(b =>
                            {

                                sourceMatches.DrawPolyline(b, true,
                                    new Bgr(TemplateMatchMapBorderColor.Blue, TemplateMatchMapBorderColor.Green, TemplateMatchMapBorderColor.Red),
                                    TemplateMatchMapBorderThickness);
                               
                            });
                            
                        }
                        if (multipleScaleMatching)
                        {
                            //scale up/down the template
                            int direction = 1;
                            for (int i = 0; i <= MaxScaleSteps; i++)
                            {
                                //scale up and verify for confidence
                                double scale = 1 + direction * i * ScaleStep;
                                Image<Gray, byte> templateTemp = ResizeTemplate(template, scale);

                                if (templateTemp != null)
                                {
                                    //test purpose only Start
                                    //templateTemp.Save(@"d:\images\templateTempUp_" + System.DateTime.Now.Ticks + ".jpg");
                                    //test purpose only End
                                    elementRectTemp = FindRectangles(source, templateTemp, confidence);
                                    if (elementRectTemp!=null && elementRectTemp.Count() > 0)
                                    {
                                        int countBoxes = 0;
                                        for (int icount = 0; icount < elementRectTemp.Count(); icount++)
                                        {
                                            if (elementRectTemp[icount].Height > 0 && elementRectTemp[icount].Width > 0)
                                            {
                                                rects.Add(elementRectTemp[icount]);
                                                boxes.Add(RectToBox(elementRectTemp[icount]));
                                                countBoxes++;
                                            }
                                        }

                                        if (countBoxes>0) 
                                            break; //break if any template matches have been found at the given resolution
                                    }
                                }

                                //then scale down and verify for confidence
                                scale = 1 + (-direction) * i * ScaleStep;
                                templateTemp = ResizeTemplate(template, scale);

                                if (templateTemp != null)
                                {
                                    //test purpose only Start
                                    //templateTemp.Save(@"d:\images\templateTempdown_" + System.DateTime.Now.Ticks + ".jpg");
                                    //test purpose only End
                                    elementRectTemp = FindRectangles(source, templateTemp, confidence);
                                    if (elementRectTemp != null && elementRectTemp.Count() > 0)
                                    {
                                        int countBoxes = 0;
                                        for (int icount = 0; icount < elementRectTemp.Count(); icount++)
                                        {
                                            if (elementRectTemp[icount].Height > 0 && elementRectTemp[icount].Width > 0)
                                            {
                                                rects.Add(elementRectTemp[icount]);
                                                boxes.Add(RectToBox(elementRectTemp[icount]));
                                                countBoxes++;
                                            }
                                        }

                                        if (countBoxes > 0)
                                            break; //break if any template matches have been found at the given resolution
                                    }
                                }
                            }
                        }
                        else
                        {
                            elementRectTemp = FindRectangles(source, template, confidence);
                            if (elementRectTemp.Count() > 0)
                            {
                                int countBoxes = 0;
                                for (int icount = 0; icount < elementRectTemp.Count(); icount++)
                                {
                                    if (elementRectTemp[icount].Height > 0 && elementRectTemp[icount].Width > 0)
                                    {
                                        rects.Add(elementRectTemp[icount]);
                                        boxes.Add(RectToBox(elementRectTemp[icount]));
                                        countBoxes++;
                                    }
                                }

                                if (countBoxes > 0)
                                    break; //break if any template matches have been found at the given resolution
                            }
                        }
                        //Sid: 21-Dec-2017: Added to handle issue of image recognition attempt going in endless loop inspite of the forever property being 
                        //set to false and timeout exceeding
                        //Sid:  25-Dec-2017: Commented as the logic was exiting for checking other controls in the state and going after foirst control in 
                        //particular state is found
                        //if (!forever && (System.DateTime.Now - startTime).TotalMilliseconds > timeout * 1000)
                        //  break;
                        System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                    }
                }
                if (rects.Count == 0)
                    throw new Exception("Could not match the template provided in this trial. Probably in the subsequent trial, the template would be matched.");
            }
            catch (Exception ex)
            {
                //log exception
                string exMessage = ex.Message;
                string innerExMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                LogHandler.LogError(string.Format(Logging.ErrorMessages.EXCEPTION, "FindElement", exMessage, innerExMessage), LogHandler.Layer.Business);
            }
            if (enableTemplateMatchMap)
            {
                if(sourceMatches!=null)
                    templateMatchMapScreen = sourceMatches.ToJpegData();
            }
            return rects;
        }

        /// <summary>
        /// In true color tries to find all the occurance of the template image in the screen shot to be taken.
        /// And then accordingly returns the rectangle collection.
        /// </summary>
        /// <param name="filename">The file name depicting the template image file</param>
        /// <param name="timeout">The period within which if the element is not identified, it throws a exception. Unit- Seconds. If needed to wait for ever, set a -ve value e.g. -1</param>
        /// <param name="confidence">The minimum confidence depicint the threshold of the image match. Value should be in the range - 1-100</param>
        /// <param name="multipleScaleMatching">The flag to dictate if the multiple scale template matching is to be followed, default is true</param>
        /// <param name="sourceImageToMatch">Optional parameter- Path of the source image in which the template element is to be recognized. Used if the image recognition is to be run in background mode. 
        /// If no value is set then a screen shot of the current user screen will be taken and used to identify the template element </param>
        /// <param name="enableTemplateMatchMap">Enable the template matches mapping on source images or screen shot image which is being scanned</param>
        /// <returns>List of Rectangle objects for the matches found</returns>
        public static List<System.Windows.Rect> FindAllInstancesInTrueColor(string filename, int timeout = DEFAULT_TIMEOUT_PERINSTANCE, double confidence = 80, bool multipleScaleMatching = true, Stream sourceImageToMatch = null, bool enableTemplateMatchMap=false)
        {
            List<System.Windows.Rect> rects = new List<System.Windows.Rect>();
            List<Point[]> boxes = new List<Point[]>();
            int currentCount = 0;
            bool forever = false;
            
            System.Windows.Rect[] elementRectTemp;
            Image<Bgr, byte> sourceMatches = null; 
            DateTime startTime = DateTime.Now;
            try
            {

                if (timeout <= 0)
                    timeout = DEFAULT_TIMEOUT_PERINSTANCE; //i.e. assigning the default time period to say 10 sec
                if (timeout < 0)
                    forever = true;
                if (System.IO.File.Exists(filename))
                {
                    Image<Bgr, byte> template = new Image<Bgr, byte>(filename);
                    //test purpose only Start
                    //template.Save(@"d:\images\templateclr_" + System.DateTime.Now.Ticks + ".jpg");
                    //test purpose only End
                    //System.Windows.Rect elementRectTemp = System.Windows.Rect.Empty;
                    bool backgroundProcessing = false;
                    //while ((System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000)
                    //while (forever || (System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000 || (rects.Count - currentCount > 0) || currentCount == 0)
                    //Sid: 25-Dec-2017 - Modified check forever loop and exiting when one control is found
                    while ((forever && currentCount == 0) || (System.DateTime.Now - startTime).TotalMilliseconds <= timeout * 1000 || (rects.Count - currentCount > 0))
                    {
                        currentCount = rects.Count;
                        if (currentCount > 0)
                            forever = false;

                        Image<Bgr, byte> source = null;
                        if (sourceImageToMatch != null)
                        {
                            source = new Image<Bgr, byte>(new Bitmap(sourceImageToMatch));
                            backgroundProcessing = true;
                        }
                        else
                        {
                            source = GetTrueColorScreenShot();

                        }
                        if (enableTemplateMatchMap)
                        {
                            if (templateMatchMapScreen != null)
                            {

                                sourceMatches = new Image<Bgr, byte>(new Bitmap(new MemoryStream(templateMatchMapScreen)));
                            }
                            else
                                sourceMatches = new Image<Bgr, byte>(source.ToBitmap());
                                
                        }
                        //test purpose only Start
                        //source.Save(@"d:\images\screenshotclr_" + System.DateTime.Now.Ticks + ".jpg");
                        //test purpose only End
                        //then on the source hide all the instances found so far
                        if (boxes != null && boxes.Count > 0)
                        {
                            boxes.ForEach(b =>
                            {
                                source.FillConvexPoly(b, new Bgr(Color.Black));
                            });
                            boxes.ForEach(b =>
                            {
                                sourceMatches.DrawPolyline(b, true,
                                    new Bgr(TemplateMatchMapBorderColor.Blue, TemplateMatchMapBorderColor.Green, TemplateMatchMapBorderColor.Red),
                                    TemplateMatchMapBorderThickness);

                            });
                        }
                        if (multipleScaleMatching)
                        {
                            //scale up/down the template
                            int direction = 1;
                            for (int i = 0; i <= MaxScaleSteps; i++)
                            {
                                //scale up and verify for confidence
                                double scale = 1 + direction * i * ScaleStep;
                                Image<Bgr, byte> templateTemp = ResizeTemplateInTrueColor(template, scale);
                                if (templateTemp != null)
                                {
                                    //test purpose only Start
                                    //templateTemp.Save(@"d:\images\templateTempUp_" + System.DateTime.Now.Ticks + ".jpg");
                                    //test purpose only End
                                    elementRectTemp = FindRectanglesInTrueColor(source, templateTemp, confidence);
                                    if (elementRectTemp.Count() > 0)
                                    {
                                        int countBoxes = 0;
                                        for (int icount = 0; icount < elementRectTemp.Count(); icount++)
                                        {
                                            if (elementRectTemp[icount].Height > 0 && elementRectTemp[icount].Width > 0)
                                            {
                                                rects.Add(elementRectTemp[icount]);
                                                boxes.Add(RectToBox(elementRectTemp[icount]));
                                                countBoxes++;
                                            }
                                        }

                                        if (countBoxes > 0)
                                            break; //break if any template matches have been found at the given resolution
                                    }
                                }

                                //then scale down and verify for confidence
                                scale = 1 + (-direction) * i * ScaleStep;
                                templateTemp = ResizeTemplateInTrueColor(template, scale);
                                if (templateTemp != null)
                                {
                                    //test purpose only Start
                                    //templateTemp.Save(@"d:\images\templateTempDown_" + System.DateTime.Now.Ticks + ".jpg");
                                    //test purpose only End
                                    elementRectTemp = FindRectanglesInTrueColor(source, templateTemp, confidence);
                                    if (elementRectTemp.Count() > 0)
                                    {
                                        int countBoxes = 0;
                                        for (int icount = 0; icount < elementRectTemp.Count(); icount++)
                                        {
                                            if (elementRectTemp[icount].Height > 0 && elementRectTemp[icount].Width > 0)
                                            {
                                                rects.Add(elementRectTemp[icount]);
                                                boxes.Add(RectToBox(elementRectTemp[icount]));
                                                countBoxes++;
                                            }
                                        }

                                        if (countBoxes > 0)
                                            break; //break if any template matches have been found at the given resolution
                                    }
                                }
                            }
                        }
                        else
                        {
                            elementRectTemp = FindRectanglesInTrueColor(source, template, confidence);
                            if (elementRectTemp.Count() > 0)
                            {
                                int countBoxes = 0;
                                for (int icount = 0; icount < elementRectTemp.Count(); icount++)
                                {
                                    if (elementRectTemp[icount].Height > 0 && elementRectTemp[icount].Width > 0)
                                    {
                                        rects.Add(elementRectTemp[icount]);
                                        boxes.Add(RectToBox(elementRectTemp[icount]));
                                        countBoxes++;
                                    }
                                }

                                if (countBoxes > 0)
                                    break; //break if any template matches have been found at the given resolution
                            }
                        }
                        //Sid: 21-Dec-2017: Added to handle issue of image recognition attempt going in endless loop inspite of the forever property being 
                        //set to false and timeout exceeding
                        //Sid:  25-Dec-2017: Commented as the logic was exiting for checking other controls in the state and going after foirst control in 
                        //particular state is found
                        //if (!forever && (System.DateTime.Now - startTime).TotalMilliseconds > timeout * 1000)
                        //  break;
                        System.Threading.Thread.Sleep(THREAD_SLEEP_DURATION);
                    }
                }
                if (rects.Count == 0)
                    throw new Exception("Could not match the template provided in this trial. Probably in the subsequent trial, the template would be matched.");
            }
            catch (Exception ex)
            {
                //log exception
                string exMessage = ex.Message;
                string innerExMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                LogHandler.LogError(string.Format(Logging.ErrorMessages.EXCEPTION, "FindElement", exMessage, innerExMessage), LogHandler.Layer.Business);
            }
            if (enableTemplateMatchMap)
            {
                if (sourceMatches != null)
                    templateMatchMapScreen = sourceMatches.ToJpegData();
            }
            return rects;
        }

        public static Image<Gray, byte> GetGrayScreenShot()
        {
            Image<Gray, byte> screen = null;
            //get the screen height and width
            int height = SystemInformation.PrimaryMonitorSize.Height;
            int width = SystemInformation.PrimaryMonitorSize.Width;
            Bitmap snap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(snap))
                g.CopyFromScreen(new Point(0, 0), Point.Empty, SystemInformation.PrimaryMonitorSize);
            screen = new Image<Gray, byte>(snap);
            return screen;
        }

        public static void SaveImageGray(Image<Gray, byte> image, string filePathToSave)        {
            
            image.Save(filePathToSave);

        }
        public static void SaveImageTrueColor(Image<Bgr, byte> image, string filePathToSave)
        {
            image.Save(filePathToSave);
        }

        public static Image<Gray, byte> GetGrayScreenShot(System.Windows.Rect rect)
        {
            if (rect == System.Windows.Rect.Empty || rect == null)
                return GetGrayScreenShot();
            Image<Gray, byte> screen = null;
            //get the screen height and width
            int height = (int)rect.Height;
            int width = (int)rect.Width;
            Bitmap snap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(snap))
                g.CopyFromScreen(new Point((int)rect.X, (int)rect.Y), Point.Empty, new Size(width, height));
            screen = new Image<Gray, byte>(snap);
            return screen;
        }

        private static Image<Bgr, byte> GetTrueColorScreenShot()
        {
            Image<Bgr, byte> screen = null;
            //get the screen height and width
            int height = SystemInformation.PrimaryMonitorSize.Height;
            int width = SystemInformation.PrimaryMonitorSize.Width;
            Bitmap snap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(snap))
                g.CopyFromScreen(new Point(0, 0), Point.Empty, SystemInformation.PrimaryMonitorSize);
            screen = new Image<Bgr, byte>(snap);
            return screen;
        }

        private static Image<Bgr, byte> GetTrueColorScreenShot(System.Windows.Rect rect)
        {
            if (rect == System.Windows.Rect.Empty || rect == null)
                return GetTrueColorScreenShot();
            Image<Bgr, byte> screen = null;
            //get the screen height and width
            int height = (int)rect.Height;
            int width = (int)rect.Width;
            Bitmap snap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(snap))
                g.CopyFromScreen(new Point((int)rect.X, (int)rect.Y), Point.Empty, new Size(width, height));
            screen = new Image<Bgr, byte>(snap);
            return screen;
        }

        private static Image<Gray, byte> ResizeTemplate(Image<Gray, byte> template, double scale)
        {
            Image<Gray, byte> resizedTemplate = null;
            try
            {
                if (scale == 0)
                    //scale = 1.0;
                    return null;
                else if (scale < 0)
                {
                    scale = 1 / (scale); // removed the - ve assignment of scale. as the scale here was negative, 
                                         //which was resulting the operation to a high upscale
                }
                if (scale < 0.5) //to restrict the down scaling to a threshold of 50% of the original size
                    return null;

                if (template.Width * template.Height < 250)
                    return template;

                resizedTemplate = template.Resize(scale, Inter.Lanczos4);

                if (resizedTemplate.Width * resizedTemplate.Height < 250)
                    return null;
            }
            catch
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
            }
            return resizedTemplate;
        }

        private static Image<Bgr, byte> ResizeTemplateInTrueColor(Image<Bgr, byte> template, double scale)
        {
            Image<Bgr, byte> resizedTemplate = null;
            try
            {
                if (scale == 0)
                    return null;
                else if (scale < 0)
                {
                    scale = 1 / (scale); // removed the - ve assignment of scale. as the scale here was negative, 
                                        //which was resulting the operation to a high upscale
                }
                if (scale < 0.5) //to restrict the down scaling to a threshold of 50% of the original size
                    return null;
                resizedTemplate = template.Resize(scale, Inter.Lanczos4);
            }
            catch
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
            }
            return resizedTemplate;
        }

        private static System.Windows.Rect FindRectangle(Image<Gray, byte> source, Image<Gray, byte> template, double confidence)
        {
            System.Windows.Rect rect = System.Windows.Rect.Empty;
            try
            {
                confidence = confidence / 100; //i.e. from 1-100 form to 0.01-1 form
                using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
                {
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out  maxValues, out  minLocations, out  maxLocations);
                         
                    if ((maxValues[0] <= 1.0) && (maxValues[0] >= confidence))
                    {
                        rect = new System.Windows.Rect(new System.Windows.Point(maxLocations[0].X, maxLocations[0].Y), new System.Windows.Size(template.Size.Width, template.Size.Height));
                    }
                }
            }
            catch(Exception ex)
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
                Exception ex1 = ex;
            }
            return rect;
        }

        private static System.Windows.Rect[] FindRectangles(Image<Gray, byte> source, Image<Gray, byte> template, double confidence)
        {
            System.Windows.Rect[] rectangles = null;
            try
            {
                confidence = confidence / 100; //i.e. from 1-100 form to 0.01-1 form
                source.Save(@"d:\images\1.jpg");
                template.Save(@"d:\images\2.jpg");
                using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
                {
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out  maxValues, out  minLocations, out  maxLocations);

                    rectangles = new System.Windows.Rect[maxValues.Count()];
                    for (int iCount = 0; iCount < maxValues.Count(); iCount++)
                    {

                        if ((maxValues[iCount] <= 1.0) && (maxValues[iCount] >= confidence))
                        {
                            System.Windows.Rect rect = new System.Windows.Rect(new System.Windows.Point(maxLocations[iCount].X, maxLocations[iCount].Y), new System.Windows.Size(template.Size.Width, template.Size.Height));
                            rectangles.SetValue(rect, iCount);
                        }
                    }

                }
            }            
            catch (Exception ex)
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
                Exception ex1 = ex;
            }
            return rectangles;
        }
        private static System.Windows.Rect FindRectangleInTrueColor(Image<Bgr, byte> source, Image<Bgr, byte> template, double confidence)
        {
            System.Windows.Rect rect = System.Windows.Rect.Empty;
            try
            {
                confidence = confidence / 100; //i.e. from 1-100 form to 0.01-1 form
                using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
                {
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out  maxValues, out  minLocations, out  maxLocations);
                    if ((maxValues[0] <= 1.0) && (maxValues[0] >= confidence))
                    {
                        rect = new System.Windows.Rect(new System.Windows.Point(maxLocations[0].X, maxLocations[0].Y), new System.Windows.Size(template.Size.Width, template.Size.Height));
                    }
                }
            }
            catch
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
            }
            return rect;
        }
        private static System.Windows.Rect[] FindRectanglesInTrueColor(Image<Bgr, byte> source, Image<Bgr, byte> template, double confidence)
        {

            System.Windows.Rect[] rectangles = null;
            try
            {
                confidence = confidence / 100; //i.e. from 1-100 form to 0.01-1 form
                using (Image<Gray, float> result = source.MatchTemplate(template, TemplateMatchingType.CcoeffNormed))
                {
                    double[] minValues, maxValues;
                    Point[] minLocations, maxLocations;
                    result.MinMax(out minValues, out  maxValues, out  minLocations, out  maxLocations);
                    rectangles = new System.Windows.Rect[maxValues.Count()];
                    for (int iCount = 0; iCount < maxValues.Count(); iCount++)
                    {

                        if ((maxValues[iCount] <= 1.0) && (maxValues[iCount] >= confidence))
                        {
                            System.Windows.Rect rect = new System.Windows.Rect(new System.Windows.Point(maxLocations[iCount].X, maxLocations[iCount].Y), new System.Windows.Size(template.Size.Width, template.Size.Height));
                            rectangles.SetValue(rect, iCount);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //exception cud be for many reasons, like the region of interest is not found, etc
                //but we need to keep trying untill the timeout happens
                Exception ex1 = ex;
            }
            return rectangles;
        }

        private static Point[] RectToBox(System.Windows.Rect rectangle)
        {
            Point[] box = null;
            if (rectangle != System.Windows.Rect.Empty)
            {
                Point topLeft = new Point((int)rectangle.X, (int)rectangle.Y);
                Point bottomRight = new Point(topLeft.X + (int)rectangle.Width, topLeft.Y + (int)rectangle.Height);
                Point topRight = new Point(bottomRight.X, topLeft.Y);
                Point bottomLeft = new Point(topLeft.X, bottomRight.Y);
                box = new Point[] { topRight, topLeft, bottomLeft, bottomRight };
            }
            return box;
        }

        public static int LaunchApplication(string appPath, string appType, string webBrowser = "", bool showWaitBox = true, string appArgument = "")
        {
            Core.Utilities.WriteLog("launching application- " + appPath);
            int processId = 0;
            List<string> allowedBrowsers = new List<string>() { ieWebBrowser, firefoxWebBrowser, chromeWebBrowser };

            //check if the appPath corresponds to website url, then open the website in IE
            //else run the exe 
            if (!string.IsNullOrEmpty(appPath))
            {
                ProcessStartInfo processStart = new ProcessStartInfo();
                processStart.CreateNoWindow = false;
                //processStart.UseShellExecute = false;
                processStart.WindowStyle = ProcessWindowStyle.Maximized;

                //if (appPath.ToLower().StartsWith("http"))
                if (appType.ToLower() == "web")
                {
                    if ((!String.IsNullOrEmpty(webBrowser)) && allowedBrowsers.Contains(webBrowser.ToLower()))
                    {
                        webBrowser = webBrowser.ToLower();
                    }
                    switch (webBrowser)
                    {
                        case ieWebBrowser:
                            processStart.FileName = "iexplore.exe";
                            if (StartIEInPrivateMode())
                            {
                                //processStart.Arguments = "-new -private " + appPath;
                                processStart.Arguments = "-new " + appPath;
                            }
                            else
                            {
                                processStart.Arguments = "-new " + appPath;
                            }
                            break;
                        case firefoxWebBrowser:
                            processStart.FileName = "firefox.exe";
                            if (StartFirefoxInPrivateMode())
                                processStart.Arguments = "-new -window " + appPath;
                            else
                                processStart.Arguments = "-new " + appPath;
                            break;
                        case chromeWebBrowser:
                            processStart.FileName = "chrome.exe";
                            if (StartChromeInPrivateMode())
                                //processStart.Arguments = "-new -incognito " + appPath;
                                processStart.Arguments = "-new " + appPath;
                            else
                                processStart.Arguments = "-new " + appPath;
                            break;
                        default:
                            processStart.FileName = GetDefaultBrowserPath();
                            string argumentParams = "";
                            if (processStart.FileName.ToLower().Contains("iexplore"))
                            {
                                if (StartIEInPrivateMode())
                                {
                                    //argumentParams = "-new -private ";
                                    argumentParams = "-new ";
                                }
                                else
                                {
                                    argumentParams = "-new ";
                                }
                            }
                            else if (processStart.FileName.ToLower().Contains("firefox"))
                            {
                                if (StartFirefoxInPrivateMode())
                                    //argumentParams = "-new -private-window ";
                                    argumentParams = "-new -window ";
                                else
                                    argumentParams = "-new ";
                            }
                            else if (processStart.FileName.ToLower().Contains("chrome"))
                            {
                                if (StartChromeInPrivateMode())
                                    //argumentParams = "-new -incognito ";
                                    argumentParams = "-new ";
                                else
                                    argumentParams = "-new ";
                            }

                            processStart.Arguments = argumentParams + appPath;

                            break;
                    }
                    processStart.Arguments = processStart.Arguments + " " + appArgument.Trim();
                }
                else if (appPath.ToLower().Contains("java"))
                {
                    processStart.FileName = "java.exe";
                    processStart.Arguments = appPath.Replace("java.exe", "") + " " + appArgument.Trim();

                    //start Java Access Bridge
                    JABHelper.Windows_run();
                }
                //else if (appPath.Contains("#*#"))
                //{
                //    string[] appPathParts = appPath.Split(new string[] { "#*#" }, StringSplitOptions.RemoveEmptyEntries);
                //    if (appPathParts.Length == 2)
                //    {
                //        processStart.FileName = appPathParts[0];
                //        processStart.Arguments = "\"" + appPathParts[1] + "\"";
                //    }
                //}
                else
                {
                    //processStart.FileName = appPath;
                    string[] appPathParts = GetExecutableAndArguement(appPath);

                    if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(System.IO.Path.GetFileNameWithoutExtension(appPathParts[0])))
                    {
                        throw new Exception("Please provide the file name without Special Characters");
                    }
                    processStart.FileName = appPathParts[0];
                    if (appPathParts.Length == 2)
                        processStart.Arguments = appPathParts[1];
                    if (!string.IsNullOrEmpty(appArgument))
                        processStart.Arguments = appArgument;
                }
                using (Process process = Process.Start(processStart))
                {
                    //process.WaitForInputIdle(); //throws error for java application
                    processId = process.Id;
                    //wait for 4-5 seconds as for application like website, we need to give some time to render
                    //before invoking actions on the controls in the website
                    string waitbox = System.Configuration.ConfigurationManager.AppSettings["ShowWaitBox"];
                    if (!string.IsNullOrEmpty(waitbox))
                        bool.TryParse(waitbox, out showWaitBox);
                    if (showWaitBox)
                    {
                        Views.AppStarting wait = new Views.AppStarting();
                        wait.ShowDialog();
                        //the below to show 'wait' dialog in the debug more.
                        //it is knwo issue in vs 2012 that the dialog doesnt work using the above two lines in the debug mode
                        //wait.Visible = true;
                        //wait.Focus();
                        //System.Windows.Forms.Application.Run(wait);
                    }
                    else
                        System.Threading.Thread.Sleep(2000);
                }
            }
            else
            {
                //log error
            }
            Core.Utilities.WriteLog("application process id- " + processId.ToString());
            return processId;
        }

        private static string[] GetExecutableAndArguement(string completeAppPath)
        {
            string delimiter = ".exe";
            string[] parts = completeAppPath.ToLower().Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
            parts[0] = parts[0] + delimiter;
            if (parts.Length == 2)
            {
                parts[1] = parts[1].Trim();
                if (!parts[1].StartsWith("\""))
                {
                    parts[1] = "\"" + parts[1];
                }
                if (!parts[1].EndsWith("\""))
                {
                    parts[1] = parts[1] + "\"";
                }
            }
            return parts;
        }

        /// <summary>
        /// This method is used to get default browser configured on end user's machine
        /// </summary>
        /// <returns>Path of Default Browser</returns>
        public static string GetDefaultBrowserPath()
        {
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";

            RegistryKey userChoiceKey = null;
            string browserPath = "";

            try
            {
                //Read default browser path from userChoiceLKey
                userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    if (browserKey == null)
                    {
                        browserKey =
                        Registry.CurrentUser.OpenSubKey(
                        urlAssociation, false);
                    }
                    var path = (browserKey.GetValue(null) as string).ToLower().Replace("\"", "");
                    browserKey.Close();
                    browserPath = CleanifyBrowserPath(browserPath);
                    return path;
                }
                else
                {
                    // user defined browser choice was found
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();

                    // now look up the path of the executable
                    string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var browserKey = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                    browserPath = (browserKey.GetValue(null) as string).ToLower().Replace("\"", "");
                    browserKey.Close();
                    browserPath = CleanifyBrowserPath(browserPath);

                    return browserPath;
                }
            }
            catch
            {
                //just kill the exception and keep trying
                //exception cud be for many reasons, like the region of interest is not found, etc
                return "";
            }
        }
        /// <summary>
        /// This method is used to remove characters after ".exe" in the browser path
        /// </summary>
        /// <param name="browserPath">Path of the browser</param>
        /// <returns>Browser Path</returns>
        private static string CleanifyBrowserPath(string browserPath)
        {
            string result = string.Empty;

            if (!String.IsNullOrEmpty(browserPath))
            {
                int position = browserPath.IndexOf(".exe");
                if (position > 0)
                    result = browserPath.Substring(0, position);
            }
            return result;
        }
        /// <summary>
        /// This function checkes the version of the IE and returns true if version is > 7.
        /// </summary>
        /// <returns>true/false</returns>
        private static Boolean StartIEInPrivateMode()
        {
            Boolean result = false;
            string key = @"Software\Microsoft\Internet Explorer";
            RegistryKey browserKey = Registry.LocalMachine.OpenSubKey(key, false);
            if (browserKey != null)
            {
                var version = browserKey.GetValue("svcVersion");
                if (version != null)
                {
                    string value = version.ToString();

                    if (GetBrowserVersion(value) > 3)
                        result = true;
                }
            }
            return result;
        }
        /// <summary>
        ///  This function checkes the version of the Firefox and returns true if version is > 3.
        /// </summary>
        /// <returns></returns>
        private static Boolean StartFirefoxInPrivateMode()
        {
            Boolean result = false;
            string key = @"SOFTWARE\Mozilla\Mozilla Firefox";
            RegistryKey browserKey = Registry.LocalMachine.OpenSubKey(key, false);
            if (browserKey != null)
            {
                var version = browserKey.GetValue("CurrentVersion");
                if (version != null)
                {
                    string value = version.ToString();

                    if (GetBrowserVersion(value) > 3)
                        result = true;
                }
            }
            return result;
        }

        /// <summary>
        ///  This function checkes the version of the Chrome and returns true if version is > 1.
        /// </summary>
        /// <returns></returns>
        private static Boolean StartChromeInPrivateMode()
        {
            Boolean result = false;
            string key = @"Software\Google\Chrome";
            RegistryKey browserKey = Registry.LocalMachine.OpenSubKey(key, false);
            if (browserKey != null)
            {
                var version = browserKey.GetValue("CurrentVersion");
                if (version != null)
                {
                    string value = version.ToString();

                    if (GetBrowserVersion(value) > 1)
                        result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// This method is used to extract the browser version
        /// </summary>
        /// <param name="ver">version info</param>
        /// <returns>browser version</returns>
        private static int GetBrowserVersion(string ver)
        {
            int version = 0;
            int position = ver.IndexOf(".");
            if (position > 0)
            {
                ver = ver.Substring(0, position);
                Int32.TryParse(ver, out version);
            }

            return version;
        }

        public static void DoMouseDown()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
        }

        public static void DoMouseUp()
        {
            //Call the imported function with the cursor's current position
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        public static byte[] StreamToByteArray(Stream input)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static bool IsStopRequested()
        {
            if (File.Exists(stopFile))
            {
                File.Delete(stopFile);
                return true;
            }
            else
                return false;
        }

        public static bool IsWindowsHandleActive(IntPtr handle)
        {
            bool isActive = false;
            isActive = IsWindow(handle);
            return isActive;
        }

        public static void WriteLog(string message)
        {
            //this is just for some debug need, will removed once the actual issue is fixed
            //string logfile = "d://iaplog.txt";
            //message = message + Environment.NewLine;
            //File.AppendAllText(logfile, message);
            LogHandler.LogDebug(message, LogHandler.Layer.Infrastructure);
        }
        public struct ImageBgr
        {
            public ImageBgr(double blue, double green, double red)
            {
                Blue = blue;
                Green = green;
                Red = red;
            }

            public double Blue, Green, Red;
            

        }
    }

    // class to contain key modifiers
    public class KeyModifier
    {
        //public const int SHIFT = (int)Keys.ShiftKey;
        //public const int CTRL = (int)Keys.ControlKey;
        //public const int WINDOWS = (int)Keys.LWin;
        //public const int ALT = (int)Keys.Menu;
        //public const int CAPITAL = (int)Keys.Capital;
        //public const int ENTER = (int)Keys.Enter;
        //public const int TAB = (int)Keys.Tab;
        //public const int BACKSPACE = (int)Keys.Back;
        //public const int DEL = (int)Keys.Delete;
        //public const int SPACE = (int)Keys.Space;

        //     No key pressed.
        public const int None = 0;
        //
        // Summary:
        //     The left mouse button.
        public const int LButton = 1;
        //
        // Summary:
        //     The right mouse button.
        public const int RButton = 2;
        //
        // Summary:
        //     The CANCEL key.
        public const int Cancel = 3;
        //
        // Summary:
        //     The middle mouse button (three-button mouse).
        public const int MButton = 4;
        //
        // Summary:
        //     The first x mouse button (five-button mouse).
        public const int XButton1 = 5;
        //
        // Summary:
        //     The second x mouse button (five-button mouse).
        public const int XButton2 = 6;
        //
        // Summary:
        //     The BACKSPACE key.
        public const int Back = 8;
        //
        // Summary:
        //     The TAB key.
        public const int Tab = 9;
        //
        // Summary:
        //     The LINEFEED key.
        public const int LineFeed = 10;
        //
        // Summary:
        //     The CLEAR key.
        public const int Clear = 12;
        //
        // Summary:
        //     The ENTER key.
        public const int Enter = 13;
        //
        // Summary:
        //     The RETURN key.
        public const int Return = 13;
        //
        // Summary:
        //     The SHIFT key.
        public const int ShiftKey = 16;
        //
        // Summary:
        //     The CTRL key.
        public const int ControlKey = 17;
        //
        // Summary:
        //     The ALT key.
        public const int Menu = 18;
        //
        // Summary:
        //     The PAUSE key.
        public const int Pause = 19;
        //
        // Summary:
        //     The CAPS LOCK key.
        public const int CapsLock = 20;
        //
        // Summary:
        //     The CAPS LOCK key.
        public const int Capital = 20;
        //

        // Summary:
        //     The ESC key.
        public const int Escape = 27;

        // Summary:
        //     The SPACEBAR key.
        public const int Space = 32;
        //
        // Summary:
        //     The PAGE UP key.
        public const int Prior = 33;
        //
        // Summary:
        //     The PAGE UP key.
        public const int PageUp = 33;
        //
        // Summary:
        //     The PAGE DOWN key.
        public const int Next = 34;
        //
        // Summary:
        //     The PAGE DOWN key.
        public const int PageDown = 34;
        //
        // Summary:
        //     The END key.
        public const int End = 35;
        //
        // Summary:
        //     The HOME key.
        public const int Home = 36;
        //
        // Summary:
        //     The LEFT ARROW key.
        public const int Left = 37;
        //
        // Summary:
        //     The UP ARROW key.
        public const int Up = 38;
        //
        // Summary:
        //     The RIGHT ARROW key.
        public const int Right = 39;
        //
        // Summary:
        //     The DOWN ARROW key.
        public const int Down = 40;
        //
        // Summary:
        //     The SELECT key.
        public const int Select = 41;
        //
        // Summary:
        //     The PRINT key.
        public const int Print = 42;
        //
        // Summary:
        //     The EXECUTE key.
        public const int Execute = 43;
        //
        // Summary:
        //     The PRINT SCREEN key.
        public const int PrintScreen = 44;
        //
        // Summary:
        //     The PRINT SCREEN key.
        public const int Snapshot = 44;
        //
        // Summary:
        //     The INS key.
        public const int Insert = 45;
        //
        // Summary:
        //     The DEL key.
        public const int Delete = 46;
        //
        // Summary:
        //     The HELP key.
        public const int Help = 47;

        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        public const int LWin = 91;
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        public const int RWin = 92;
        //
        // Summary:
        //     The application key (Microsoft Natural Keyboard).
        public const int Apps = 93;
        //
        // Summary:
        //     The computer sleep key.
        public const int Sleep = 95;
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        public const int NumPad0 = 96;
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        public const int NumPad1 = 97;
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        public const int NumPad2 = 98;
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        public const int NumPad3 = 99;
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        public const int NumPad4 = 100;
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        public const int NumPad5 = 101;
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        public const int NumPad6 = 102;
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        public const int NumPad7 = 103;
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        public const int NumPad8 = 104;
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        public const int NumPad9 = 105;

        // Summary:
        //     The F1 key.
        public const int F1 = 112;
        //
        // Summary:
        //     The F2 key.
        public const int F2 = 113;
        //
        // Summary:
        //     The F3 key.
        public const int F3 = 114;
        //
        // Summary:
        //     The F4 key.
        public const int F4 = 115;
        //
        // Summary:
        //     The F5 key.
        public const int F5 = 116;
        //
        // Summary:
        //     The F6 key.
        public const int F6 = 117;
        //
        // Summary:
        //     The F7 key.
        public const int F7 = 118;
        //
        // Summary:
        //     The F8 key.
        public const int F8 = 119;
        //
        // Summary:
        //     The F9 key.
        public const int F9 = 120;
        //
        // Summary:
        //     The F10 key.
        public const int F10 = 121;
        //
        // Summary:
        //     The F11 key.
        public const int F11 = 122;
        //
        // Summary:
        //     The F12 key.
        public const int F12 = 123;
        //
        // Summary:
        //     The F13 key.
        public const int F13 = 124;
        //
        // Summary:
        //     The F14 key.
        public const int F14 = 125;
        //
        // Summary:
        //     The F15 key.
        public const int F15 = 126;
        //
        // Summary:
        //     The F16 key.
        public const int F16 = 127;
        //
        // Summary:
        //     The F17 key.
        public const int F17 = 128;
        //
        // Summary:
        //     The F18 key.
        public const int F18 = 129;
        //
        // Summary:
        //     The F19 key.
        public const int F19 = 130;
        //
        // Summary:
        //     The F20 key.
        public const int F20 = 131;
        //
        // Summary:
        //     The F21 key.
        public const int F21 = 132;
        //
        // Summary:
        //     The F22 key.
        public const int F22 = 133;
        //
        // Summary:
        //     The F23 key.
        public const int F23 = 134;
        //
        // Summary:
        //     The F24 key.
        public const int F24 = 135;
        //
        // Summary:
        //     The NUM LOCK key.
        public const int NumLock = 144;
        //
        // Summary:
        //     The SCROLL LOCK key.
        public const int Scroll = 145;
        //
        // Summary:
        //     The left SHIFT key.
        public const int LShiftKey = 160;
        //
        // Summary:
        //     The right SHIFT key.
        public const int RShiftKey = 161;
        //
        // Summary:
        //     The left CTRL key.
        public const int LControlKey = 162;
        //
        // Summary:
        //     The right CTRL key.
        public const int RControlKey = 163;
        //
        // Summary:
        //     The left ALT key.
        public const int LMenu = 164;
        //
        // Summary:
        //     The right ALT key.
        public const int RMenu = 165;
        //
        // Summary:
        //     The browser back key (Windows 2000 or later).
        public const int BrowserBack = 166;
        //
        // Summary:
        //     The browser forward key (Windows 2000 or later).
        public const int BrowserForward = 167;
        //
        // Summary:
        //     The browser refresh key (Windows 2000 or later).
        public const int BrowserRefresh = 168;
        //
        // Summary:
        //     The browser stop key (Windows 2000 or later).
        public const int BrowserStop = 169;
        //
        // Summary:
        //     The browser search key (Windows 2000 or later).
        public const int BrowserSearch = 170;
        //
        // Summary:
        //     The browser favorites key (Windows 2000 or later).
        public const int BrowserFavorites = 171;
        //
        // Summary:
        //     The browser home key (Windows 2000 or later).
        public const int BrowserHome = 172;
        //
        // Summary:
        //     The volume mute key (Windows 2000 or later).
        public const int VolumeMute = 173;
        //
        // Summary:
        //     The volume down key (Windows 2000 or later).
        public const int VolumeDown = 174;
        //
        // Summary:
        //     The volume up key (Windows 2000 or later).
        public const int VolumeUp = 175;
        //
        // Summary:
        //     The media next track key (Windows 2000 or later).
        public const int MediaNextTrack = 176;
        //
        // Summary:
        //     The media previous track key (Windows 2000 or later).
        public const int MediaPreviousTrack = 177;
        //
        // Summary:
        //     The media Stop key (Windows 2000 or later).
        public const int MediaStop = 178;
        //
        // Summary:
        //     The media play pause key (Windows 2000 or later).
        public const int MediaPlayPause = 179;
        //
        // Summary:
        //     The launch mail key (Windows 2000 or later).
        public const int LaunchMail = 180;
        //
        // Summary:
        //     The select media key (Windows 2000 or later).
        public const int SelectMedia = 181;
        //
        // Summary:
        //     The start application one key (Windows 2000 or later).
        public const int LaunchApplication1 = 182;
        //
        // Summary:
        //     The start application two key (Windows 2000 or later).
        public const int LaunchApplication2 = 183;
        //
        // Summary:
        //     The OEM 1 key.
        public const int Oem1 = 186;
        //
        // Summary:
        //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
        public const int OemSemicolon = 186;
        //
        // Summary:
        //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
        public const int Oemplus = 187;
        //
        // Summary:
        //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
        public const int Oemcomma = 188;
        //
        // Summary:
        //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
        public const int OemMinus = 189;
        //
        // Summary:
        //     The OEM period key on any country/region keyboard (Windows 2000 or later).
        public const int OemPeriod = 190;
        //
        // Summary:
        //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
        public const int OemQuestion = 191;
        //
        // Summary:
        //     The OEM 2 key.
        public const int Oem2 = 191;
        //
        // Summary:
        //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
        public const int Oemtilde = 192;
        //
        // Summary:
        //     The OEM 3 key.
        public const int Oem3 = 192;
        //
        // Summary:
        //     The OEM 4 key.
        public const int Oem4 = 219;
        //
        // Summary:
        //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
        public const int OemOpenBrackets = 219;
        //
        // Summary:
        //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
        public const int OemPipe = 220;
        //
        // Summary:
        //     The OEM 5 key.
        public const int Oem5 = 220;
        //
        // Summary:
        //     The OEM 6 key.
        public const int Oem6 = 221;
        //
        // Summary:
        //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
        public const int OemCloseBrackets = 221;
        //
        // Summary:
        //     The OEM 7 key.
        public const int Oem7 = 222;
        //
        // Summary:
        //     The OEM singled/double quote key on a US standard keyboard (Windows 2000
        //     or later).
        public const int OemQuotes = 222;
        //
        // Summary:
        //     The OEM 8 key.
        public const int Oem8 = 223;
        //
        // Summary:
        //     The OEM 102 key.
        public const int Oem102 = 226;
        //
        // Summary:
        //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows
        //     2000 or later).
        public const int OemBackslash = 226;
        //
        // Summary:
        //     The PROCESS KEY key.
        public const int ProcessKey = 229;
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key
        //     value is the low word of a 32-bit virtual-key value used for non-keyboard
        //     input methods.
        public const int Packet = 231;
        //
        // Summary:
        //     The ATTN key.
        public const int Attn = 246;
        //
        // Summary:
        //     The CRSEL key.
        public const int Crsel = 247;
        //
        // Summary:
        //     The EXSEL key.
        public const int Exsel = 248;
        //
        // Summary:
        //     The ERASE EOF key.
        public const int EraseEof = 249;
        //
        // Summary:
        //     The PLAY key.
        public const int Play = 250;
        //
        // Summary:
        //     The ZOOM key.
        public const int Zoom = 251;
        //
        // Summary:
        //     A constant reserved for future use.
        public const int NoName = 252;
        //
        // Summary:
        //     The PA1 key.
        public const int Pa1 = 253;
        //
        // Summary:
        //     The CLEAR key.
        public const int OemClear = 254;
        //

    }




    public enum DragDestinationType
    {
        AbsolutePosition,
        RelativePosition
    }
}

