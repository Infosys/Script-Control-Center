/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Infosys.ATR.UIAutomation.UserActivityMonitor
{
    public static partial class GlobalEventHandler
    {
        /// <summary>
        /// This procedure is an application-defined or library-defined callback 
        /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
        /// to this callback function. CallWndProc is a placeholder for the application-defined 
        /// or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int GlobalEventProc(int nCode, int wParam, IntPtr lParam);

        //##############################################################################
        #region Mouse hook processing

    
        private static GlobalEventProc mouseDelegate;

        /// <summary>
        /// Stores the handle to the mouse hook procedure.
        /// </summary>
        private static int mouseGlobalEventHandle;

        private static int mPrevX;
        private static int mPrevY;

        /// <summary>
        /// A callback function which will be called every Time a mouse activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private static int MouseGlobalEventProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //Marshall the data from callback.
                MouseLLGlobalEventStruct mouseGlobalEventStruct = (MouseLLGlobalEventStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLGlobalEventStruct));

                //detect button clicked
                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                int clickCount = 0;
                bool mouseDown = false;
                bool mouseUp = false;

                switch (wParam)
                {
                    case WMLBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WMLBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WMLBUTTONDBLCLK: 
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case WMRBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WMRBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WMRBUTTONDBLCLK: 
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                    case WMMOUSEWHEEL:
                        //If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
                        //One wheel click is defined as WHEEL_DELTA, which is 120. 
                        //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseGlobalEventStruct.mouseData >> 16) & 0xffff);
                  
                        break;
                }

                //generate event 
                MouseEventExtArgsHandler e = new MouseEventExtArgsHandler(
                                                   button,
                                                   clickCount,
                                                   mouseGlobalEventStruct.point.X,
                                                   mouseGlobalEventStruct.point.Y,
                                                   mouseDelta);

                //Mouse up
                if (MouseUpEvent!=null && mouseUp)
                {
                    MouseUpEvent.Invoke(null, e);
                }

                //Mouse down
                if (MouseDownEvent != null && mouseDown)
                {
                    MouseDownEvent.Invoke(null, e);
                }

                //Mouse click
                if (MouseClickEvent != null && clickCount>0)
                {
                    MouseClickEvent.Invoke(null, e);
                }

               
                if (MouseClickExtEvent != null && clickCount > 0)
                {
                    MouseClickExtEvent.Invoke(null, e);
                }

                //Mouse dbl click
                if (MouseDoubleClickEvent != null && clickCount == 2)
                {
                    MouseDoubleClickEvent.Invoke(null, e);
                }

                //Wheel was moved
                if (MouseWheelEvent!=null && mouseDelta!=0)
                {
                    MouseWheelEvent.Invoke(null, e);
                }

                //Mouse move 
                if ((MouseMoveEvent!=null || MouseMoveExtEvent!=null) && (mPrevX != mouseGlobalEventStruct.point.X || mPrevY != mouseGlobalEventStruct.point.Y))
                {
                    mPrevX = mouseGlobalEventStruct.point.X;
                    mPrevY = mouseGlobalEventStruct.point.Y;
                    if (MouseMoveEvent != null)
                    {
                        MouseMoveEvent.Invoke(null, e);
                    }

                    if (MouseMoveExtEvent != null)
                    {
                        MouseMoveExtEvent.Invoke(null, e);
                    }
                }

                if (e.Handled)
                {
                    return -1;
                }
            }

            //call next hook
            return CallNextHookEx(mouseGlobalEventHandle, nCode, wParam, lParam);
        }
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);
        
        private static void SubscribedToGlobalMouseEvents()
        {
            // install Mouse hook only if it is not installed and must be installed
            if (mouseGlobalEventHandle == 0)
            {
                //See comment of this field. To avoid GC to clean it up.
                mouseDelegate = MouseGlobalEventProc;
              
                var mar = LoadLibrary("user32.dll");
                mouseGlobalEventHandle = SetWindowsHookEx(
                    WHMOUSE_LL,
                    mouseDelegate,
                    mar,
                    0);
                //If SetWindowsHookEx fails.
                if (mouseGlobalEventHandle == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup

                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private static void UnsubscribeFromGlobalMouseEvents()
        {
            //if no subsribers are registered unsubsribe from hook
            if (MouseClickEvent == null &&
                MouseDownEvent == null &&
                MouseMoveEvent == null &&
                MouseUpEvent == null &&
                MouseClickExtEvent == null &&
                MouseMoveExtEvent == null &&
                MouseWheelEvent == null)
            {
                ForceUnsunscribeToGlobalMouseEvents();
            }
        }

        private static void ForceUnsunscribeToGlobalMouseEvents()
        {
            if (mouseGlobalEventHandle != 0)
            {
                //uninstall hook
                int result = UnhookWindowsHookEx(mouseGlobalEventHandle);
                //reset invalid handle
                mouseGlobalEventHandle = 0;
                //Free up for GC
                mouseDelegate = null;
                //if failed and exception must be thrown
                if (result == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }
        
        #endregion

        //##############################################################################
        #region Keyboard event handlers

       
        private static GlobalEventProc keyboardDelegate;

        /// <summary>
        /// Stores the handle to the Keyboard hook procedure.
        /// </summary>
        private static int keyboardGlobalEventHandle;

        /// <summary>
        /// A callback function which will be called every Time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        private static int KeyboardGlobalEventProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;

            if (nCode >= 0)
            {
                //read structure KeyboardHookStruct at lParam
                KeyboardGlobalEventStruct MyKeyboardHookStruct = (KeyboardGlobalEventStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardGlobalEventStruct));
                //raise KeyDown
                if (KeyDownEvent != null && (wParam == WMKEYDOWN || wParam == WMSYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.virtualKeyCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyDownEvent.Invoke(null, e);
                    handled = e.Handled;
                }

                // raise KeyPress
                if (KeyPressEvent != null && wParam == WMKEYDOWN)
                {
                    bool isDownShift = ((GetKeyState(VKSHIFT) & 0x80) == 0x80 ? true : false);
                    bool isDownCapslock = (GetKeyState(VKCAPITAL) != 0 ? true : false);

                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.virtualKeyCode,
                              MyKeyboardHookStruct.scanCode,
                              keyState,
                              inBuffer,
                              MyKeyboardHookStruct.flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                        KeyPressEventArgs e = new KeyPressEventArgs(key);
                        KeyPressEvent.Invoke(null, e);
                        handled = handled || e.Handled;
                    }
                }

                // raise KeyUp
                if (KeyUpEvent != null && (wParam == WMKEYUP || wParam == WMSYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.virtualKeyCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyUpEvent.Invoke(null, e);
                    handled = handled || e.Handled;
                }

            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                return -1;

            //forward to other application
            return CallNextHookEx(keyboardGlobalEventHandle, nCode, wParam, lParam);
        }

        private static void SubscribedToGlobalKeyboardEvents()
        {
            // install Keyboard hook only if it is not installed and must be installed
            if (keyboardGlobalEventHandle == 0)
            {
                //See comment of this field. To avoid GC to clean it up.
                keyboardDelegate = KeyboardGlobalEventProc;
             
                var mar = LoadLibrary("user32.dll");
                keyboardGlobalEventHandle = SetWindowsHookEx(
                    WHKEYBOARD_LL,
                    keyboardDelegate,
                    mar,
                    0);
                //If SetWindowsHookEx fails.
                if (keyboardGlobalEventHandle == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup

                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private static void UnsubscribeFromGlobalKeyboardEvents()
        {
            //if no subsribers are registered unsubsribe from hook
            if (KeyDownEvent == null &&
                KeyUpEvent == null &&
                KeyPressEvent == null)
            {
                ForceUnsunscribeToGlobalKeyboardEvents();
            }
        }

        private static void ForceUnsunscribeToGlobalKeyboardEvents()
        {
            if (keyboardGlobalEventHandle != 0)
            {
                //uninstall hook
                int result = UnhookWindowsHookEx(keyboardGlobalEventHandle);
                //reset invalid handle
                keyboardGlobalEventHandle = 0;
                //Free up for GC
                keyboardDelegate = null;
                //if failed and exception must be thrown
                if (result == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        #endregion

    }
}
