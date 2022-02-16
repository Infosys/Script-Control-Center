/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Windows.Forms;

namespace Infosys.ATR.UIAutomation.UserActivityMonitor {

    /// <summary>
    /// This class monitors all global events
    /// </summary>
    public static partial class GlobalEventHandler
    {
        //################################################################
        #region Mouse events

        private static event MouseEventHandler MouseMoveEvent;

        /// <summary>
        /// Mouse pointer moved
        /// </summary>
        public static event MouseEventHandler MouseMovedEvent
        {
            add
            {
                SubscribedToGlobalMouseEvents();
                MouseMoveEvent += value;
            }

            remove
            {
                MouseMoveEvent -= value;
                UnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgsHandler> MouseMoveExtEvent;

        /// <summary>
        /// Mouse pointer moved along with extended mouse event args
        /// </remarks>
        public static event EventHandler<MouseEventExtArgsHandler> MouseMovedExtEvent
        {
            add
            {
                SubscribedToGlobalMouseEvents();
                MouseMoveExtEvent += value;
            }

            remove
            {

                MouseMoveExtEvent -= value;
                UnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseClickEvent;

        /// <summary>
        /// Mouse click
        /// </summary>
        public static event MouseEventHandler MouseClickedEvent
        {
            add
            {
                SubscribedToGlobalMouseEvents();
                MouseClickEvent += value;
            }
            remove
            {
                MouseClickEvent -= value;
                UnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgsHandler> MouseClickExtEvent;

        /// <summary>
        /// Mouse click + extended mouse event args
        /// </remarks>
        public static event EventHandler<MouseEventExtArgsHandler> MouseClickedExtEvent
        {
            add
            {
                SubscribedToGlobalMouseEvents();
                MouseClickExtEvent += value;
            }
            remove
            {
                MouseClickExtEvent -= value;
                UnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseDownEvent;

        /// <summary>
        /// Mouse btn pressed
        /// </summary>
        public static event MouseEventHandler  MouseDownEvents
        {
            add 
            { 
                SubscribedToGlobalMouseEvents();
                MouseDownEvent += value;
            }
            remove
            {
                MouseDownEvent -= value;
                UnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseUpEvent;

        /// <summary>
        /// Mouse btn released
        /// </summary>
        public static event MouseEventHandler MouseUpEvents
        {
            add
            {
                SubscribedToGlobalMouseEvents();
                MouseUpEvent += value;
            }
            remove
            {
                MouseUpEvent -= value;
                UnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler MouseWheelEvent;

        /// <summary>
        /// Mouse wheel moved
        /// </summary>
        public static event MouseEventHandler MouseWheelEvents
        {
            add
            {
                SubscribedToGlobalMouseEvents();
                MouseWheelEvent += value;
            }
            remove
            {
                MouseWheelEvent -= value;
                UnsubscribeFromGlobalMouseEvents();
            }
        }


        private static event MouseEventHandler MouseDoubleClickEvent;

       
        /// <summary>
        /// Mouse dbl click
        /// </summary>
        public static event MouseEventHandler MouseDoubleClickedEvent
        {
            add
            {
                SubscribedToGlobalMouseEvents();
                if (MouseDoubleClickEvent == null)
                {
                    //We create a timer to monitor interval between two clicks
                    doubleClickTimer = new Timer
                    {
                        //This interval will be set to the value we retrive from windows. This is a windows setting from contro planel.
                        Interval = GetDoubleClickTime(),
                        //We do not start timer yet. It will be start when the click occures.
                        Enabled = false
                    };
                    //We define the callback function for the timer
                    doubleClickTimer.Tick += GlobalEvent_DoubleClickTimeElapsed;
                    //We start to monitor mouse up event.
                    MouseUpEvents += GlobalEvent_OnMouseUp;
                }
                MouseDoubleClickEvent += value;
            }
            remove
            {
                if (MouseDoubleClickEvent != null)
                {
                    MouseDoubleClickEvent -= value;
                    if (MouseDoubleClickEvent == null)
                    {
                        //Stop monitoring mouse up
                        MouseUpEvents -= GlobalEvent_OnMouseUp;
                        //Dispose the timer
                        doubleClickTimer.Tick -= GlobalEvent_DoubleClickTimeElapsed;
                        doubleClickTimer = null;
                    }
                }
                UnsubscribeFromGlobalMouseEvents();
            }
        }

        //This field remembers mouse button pressed because in addition to the short interval it must be also the same button.
        private static MouseButtons prevClickedButton;
        //The timer to monitor time interval between two clicks.
        private static Timer doubleClickTimer;

        private static void GlobalEvent_DoubleClickTimeElapsed(object sender, EventArgs e)
        {
            //Timer is alapsed and no second click ocuured
            doubleClickTimer.Enabled = false;
            prevClickedButton = MouseButtons.None;
        }

        /// <summary>
        /// </summary>
        private static void GlobalEvent_OnMouseUp(object sender, MouseEventArgs e)
        {
            //This should not heppen
            if (e.Clicks < 1) { return;}
            //If the secon click heppened on the same button
            if (e.Button.Equals(prevClickedButton))
            {
                if (MouseDoubleClickEvent!=null)
                {
                    //Fire double click
                    MouseDoubleClickEvent.Invoke(null, e);
                }
                //Stop timer
                doubleClickTimer.Enabled = false;
                prevClickedButton = MouseButtons.None;
            }
            else
            {
                //If it was the firts click start the timer
                doubleClickTimer.Enabled = true;
                prevClickedButton = e.Button;
            }
        }
        #endregion

        //################################################################
        #region Keyboard events

        private static event KeyPressEventHandler KeyPressEvent;

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
       
        public static event KeyPressEventHandler KeyPressEvents
        {
            add
            {
                SubscribedToGlobalKeyboardEvents();
                KeyPressEvent += value;
            }
            remove
            {
                KeyPressEvent -= value;
                UnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static event KeyEventHandler KeyUpEvent;

        /// <summary>
        /// Occurs when a key is released. 
        /// </summary>
        public static event KeyEventHandler KeyUpEvents
        {
            add
            {
                SubscribedToGlobalKeyboardEvents();
                KeyUpEvent += value;
            }
            remove
            {
                KeyUpEvent -= value;
                UnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static event KeyEventHandler KeyDownEvent;

        /// <summary>
        /// Occurs when a key is preseed. 
        /// </summary>
        public static event KeyEventHandler KeyDownEvents
        {
            add
            {
                SubscribedToGlobalKeyboardEvents();
                KeyDownEvent += value;
            }
            remove
            {
                KeyDownEvent -= value;
                UnsubscribeFromGlobalKeyboardEvents();
            }
        }


        #endregion
    }
}
