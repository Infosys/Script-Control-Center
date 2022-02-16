/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;

namespace IMSWorkBench.Infrastructure.Library.Services
{
    public enum ShowWindowCommands
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,
        /// <summary>
        /// Activates and displays a window. If the window is minimized or 
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window 
        /// for the first time.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,
        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?
        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>       
        ShowMaximized = 3,
        /// <summary>
        /// Displays a window in its most recent size and position. This value 
        /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except 
        /// the window is not activated.
        /// </summary>
        ShowNoActivate = 4,
        /// <summary>
        /// Activates the window and displays it in its current size and position. 
        /// </summary>
        Show = 5,
        /// <summary>
        /// Minimizes the specified window and activates the next top-level 
        /// window in the Z order.
        /// </summary>
        Minimize = 6,
        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the 
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,
        /// <summary>
        /// Displays the window in its current size and position. This value is 
        /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the 
        /// window is not activated.
        /// </summary>
        ShowNA = 8,
        /// <summary>
        /// Activates and displays the window. If the window is minimized or 
        /// maximized, the system restores it to its original size and position. 
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,
        /// <summary>
        /// Sets the show state based on the SW_* value specified in the 
        /// STARTUPINFO structure passed to the CreateProcess function by the 
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,
        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread 
        /// that owns the window is not responding. This flag should only be 
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }

    public class Win32
    {
        private const int GWL_STYLE = (-16);
        private const long WS_BORDER = 0x800000L;
        private const UInt32 WS_MAXIMIZE = 0x01000000;
        private const UInt32 WS_CHILD = 0x40000000;
        private const UInt32 WS_POPUP = 0x80000000;
        private static IntPtr processHwnd = IntPtr.Zero;
        private const int WM_COMMAND = 0x111;
        private const int MIN_ALL = 419;
        private const int MIN_ALL_UNDO = 416;
        public static AutomationFocusChangedEventHandler UIFocuseventHandler;
        //public static AutomationPropertyChangedEventHandler UIAPropertyeventHandler;
        static IntPtr sikuliHandle;

        public delegate void ProcessFocusEventHandler(IntPtr handle,ShowWindowCommands windowState);
        public static event ProcessFocusEventHandler Sikuli;
      //  public static event ProcessFocusEventHandler Java;

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        public static extern UInt32 GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongA", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 SetWindowLong(IntPtr hwnd, int nIndex, UInt32 dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        static Win32()
        {
            //UIFocuseventHandler = new AutomationFocusChangedEventHandler(UIFocusPropertyChanged);
            //UIAPropertyeventHandler = new AutomationPropertyChangedEventHandler(UIControlPropertyChanged);
            //Automate();
        }

        public static void Automate()
        {
            Automation.AddAutomationFocusChangedEventHandler(UIFocuseventHandler);
            //AutomationProperty[] objParam = new AutomationProperty[] { AutomationElementIdentifiers.NameProperty,
            //    SelectionItemPattern.IsSelectedProperty , ValuePattern.ValueProperty
            //};
            //Automation.AddAutomationPropertyChangedEventHandler(AutomationElement.RootElement, TreeScope.Descendants, UIControlPropertyChanged,
            //     objParam);
        }

        public static void Maximize(IntPtr handle)
        {
            ShowWindow(handle, ShowWindowCommands.Maximize);
            SetForegroundWindow(handle);
        }


        private static void UIControlPropertyChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            AutomationElement obj = ((AutomationElement)sender);

            if (obj != null)
            {
                AutomationElement.AutomationElementInformation objInfo = obj.Current;
                try
                {
                    if (objInfo.ControlType != null)
                    {
                        ControlType ct = objInfo.ControlType;
                        string text = objInfo.Name;
                        Debug.Write(text);
                        if (text.Contains("Workbench"))
                        {
                            //  System.Windows.Forms.MessageBox.Show(text);
                            //IntPtr handle = Process.GetProcessById(obj.Current.ProcessId).MainWindowHandle;
                            SetFocus(sikuliHandle);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private static void UIFocusPropertyChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            AutomationElement obj = ((AutomationElement)sender);

            if (obj != null)
            {
                AutomationElement.AutomationElementInformation objInfo = obj.Current;
                try
                {
                    ControlType ct = objInfo.ControlType;
                    string text = obj.Current.Name;
                 //   if (text.Contains("Sikuli IDE"))
                    if (text.Contains("iFEA IDE"))
                    {
                        int processId = Process.GetProcessById(obj.Current.ProcessId).Id;
                        IntPtr sikuliHandler = Process.GetProcessById(obj.Current.ProcessId).MainWindowHandle;
                        sikuliHandle = sikuliHandler;
                        if(obj.Current.LocalizedControlType == "pane")
                            Sikuli(sikuliHandler,ShowWindowCommands.Minimize);
                        else if(obj.Current.LocalizedControlType == "window")
                            Sikuli(sikuliHandler,ShowWindowCommands.Maximize);
                    }
                    //else if (text.Contains("java"))
                    //{
                    //    int processId = Process.GetProcessById(obj.Current.ProcessId).Id;
                    //    IntPtr javaHandler = Process.GetProcessById(obj.Current.ProcessId).MainWindowHandle;
                    //    Java(javaHandler);
                    //}
                    else if (text.Contains("Workbench"))
                    {
                        //  System.Windows.Forms.MessageBox.Show(text);
                        IntPtr handle = Process.GetProcessById(obj.Current.ProcessId).MainWindowHandle;
                        SetFocus(sikuliHandle);
                    }

                }
                catch
                {
                    //do nothing
                }
            }
        }

        public static void MinimizeAll()
        {

            var processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                ShowWindow(p.MainWindowHandle, ShowWindowCommands.ShowMinimized);
            }

        }

        public static bool ModifyWindowStyles(IntPtr appHandle)
        {

            //SetWindowLong(appHandle, GWL_STYLE, (GetWindowLong(appHandle, GWL_STYLE) & ~WS_POPUP) | WS_MAXIMIZE | WS_CHILD);
            SetWindowLong(appHandle, GWL_STYLE, (GetWindowLong(appHandle, GWL_STYLE) & ~WS_POPUP) | WS_CHILD);

            // Get the current window style
            //UInt32 currentStyle = GetWindowLong(appHandle, GWL_STYLE);

            //// Modify the styles specified on the current style
            //// Remove window border and show the window in a maximized state
            //UInt32 newStyle =  (currentStyle & ~WS_POPUP) | WS_MAXIMIZE | WS_CHILD;
            //if (currentStyle == newStyle)
            //{
            //    //MessageBox.Show("Styles are the same! No changes made.", "No Action Taken");
            //    return false;
            //}

            //// Set the new window style-
            //UInt32 res = SetWindowLong(appHandle, GWL_STYLE, newStyle);

            return true;

        }


        public static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "NO OWNER";
        }

        public static void Start(string process, string argument)
        {
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(process);
            psi.Arguments = argument;
         //   psi.WorkingDirectory = workingDirectory;
            psi.UseShellExecute = false;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();
            string msg = "";
            if (p.StandardError != null)
            {
                msg = p.StandardError.ReadToEnd();
                if (!String.IsNullOrEmpty(msg))
                    System.Windows.Forms.MessageBox.Show(msg, "Frontend Automation", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            if (p.StandardOutput != null)
            {
                msg = p.StandardOutput.ReadToEnd();
                if (!String.IsNullOrEmpty(msg))
                    System.Windows.Forms.MessageBox.Show(msg, "Frontend Automation", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
        }

        public static IntPtr GetHandle(string process, string argument, out int pid)
        {

            Process p = null;
            processHwnd = IntPtr.Zero;

            if (process.ToLower().Contains("iexplore"))
            {
                p = Process.Start(process, "-nomerge " + argument);

            }
            else
            {
                //p = Process.Start(process, argument);
                p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo(process);
                psi.Arguments = argument;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardOutput = true;
                p.StartInfo = psi;
                p.Start();
                // p.WaitForExit();
                Thread.Sleep(3000);
            }

            while (!p.HasExited && processHwnd == IntPtr.Zero)
            {
                processHwnd = p.MainWindowHandle;
            }
            pid = p.Id;
            return processHwnd;

        }

        public static IntPtr Dockit(IntPtr pHandle, string process, string argument, out int pid)
        {

            int id = 0;
            IntPtr cHandle = GetHandle(process, argument, out id);
            Win32.SetParent(processHwnd, pHandle);
            Win32.SetFocus(processHwnd);
            pid = id;
            return cHandle;
        }
    }
}
