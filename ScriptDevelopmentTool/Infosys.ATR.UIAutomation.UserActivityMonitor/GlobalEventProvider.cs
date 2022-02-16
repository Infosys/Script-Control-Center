using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Infosys.ATR.UIAutomation.UserActivityMonitor
{
    /// <summary>
    /// This component monitors all mouse activities globally (also outside of the application) 
    /// and provides appropriate events.
    /// </summary>
    public class IapGlobalEventProvider : Component
    {
        /// <summary>
        /// This component raises events. The value is always true.
        /// </summary>
        protected override bool CanRaiseEvents
        {
            get
            {
                return true;
            }
        }

        //################################################################
        #region Mouse events

        private event MouseEventHandler m_IapMouseMove;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        public event MouseEventHandler IapMouseMove
        {
            add
            {
                if (m_IapMouseMove == null)
                {
                    IapHookManager.IapMouseMove += IapHookManager_MouseMove;
                }
                m_IapMouseMove += value;
            }

            remove
            {
                m_IapMouseMove -= value;
                if (m_IapMouseMove == null)
                {
                    IapHookManager.IapMouseMove -= IapHookManager_MouseMove;
                }
            }
        }

        void IapHookManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_IapMouseMove != null)
            {
                m_IapMouseMove.Invoke(this, e);
            }
        }

        private event MouseEventHandler m_IapMouseClick;
        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        public event MouseEventHandler IapMouseClick
        {
            add
            {
                if (m_IapMouseClick == null)
                {
                    IapHookManager.IapMouseClick += IapHookManager_MouseClick;
                }
                m_IapMouseClick += value;
            }

            remove
            {
                m_IapMouseClick -= value;
                if (m_IapMouseClick == null)
                {
                    IapHookManager.IapMouseClick -= IapHookManager_MouseClick;
                }
            }
        }

        void IapHookManager_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_IapMouseClick != null)
            {
                m_IapMouseClick.Invoke(this, e);
            }
        }

        private event MouseEventHandler m_IapMouseDown;

        /// <summary>
        /// Occurs when the mouse a mouse button is pressed. 
        /// </summary>
        public event MouseEventHandler IapMouseDown
        {
            add
            {
                if (m_IapMouseDown == null)
                {
                    IapHookManager.IapMouseDown += HookManager_MouseDown;
                }
                m_IapMouseDown += value;
            }

            remove
            {
                m_IapMouseDown -= value;
                if (m_IapMouseDown == null)
                {
                    IapHookManager.IapMouseDown -= HookManager_MouseDown;
                }
            }
        }

        void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_IapMouseDown != null)
            {
                m_IapMouseDown.Invoke(this, e);
            }
        }


        private event MouseEventHandler m_IapMouseUp;

        /// <summary>
        /// Occurs when a mouse button is released. 
        /// </summary>
        public event MouseEventHandler IapMouseUp
        {
            add
            {
                if (m_IapMouseUp == null)
                {
                    IapHookManager.IapMouseUp += IapHookManager_MouseUp;
                }
                m_IapMouseUp += value;
            }

            remove
            {
                m_IapMouseUp -= value;
                if (m_IapMouseUp == null)
                {
                    IapHookManager.IapMouseUp -= IapHookManager_MouseUp;
                }
            }
        }

        void IapHookManager_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_IapMouseUp != null)
            {
                m_IapMouseUp.Invoke(this, e);
            }
        }

        private event MouseEventHandler m_IapMouseDoubleClick;

        /// <summary>
        /// Occurs when a double clicked was performed by the mouse. 
        /// </summary>
        public event MouseEventHandler IapMouseDoubleClick
        {
            add
            {
                if (m_IapMouseDoubleClick == null)
                {
                    IapHookManager.IapMouseDoubleClick += IapHookManager_MouseDoubleClick;
                }
                m_IapMouseDoubleClick += value;
            }

            remove
            {
                m_IapMouseDoubleClick -= value;
                if (m_IapMouseDoubleClick == null)
                {
                    IapHookManager.IapMouseDoubleClick -= IapHookManager_MouseDoubleClick;
                }
            }
        }

        void IapHookManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (m_IapMouseDoubleClick != null)
            {
                m_IapMouseDoubleClick.Invoke(this, e);
            }
        }


        private event EventHandler<IapMouseEventExtArgs> m_IapMouseMoveExt;

        /// <summary>
        /// Occurs when the mouse pointer is moved. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse movement in other applications.
        /// </remarks>
        public event EventHandler<IapMouseEventExtArgs> IapMouseMoveExt
        {
            add
            {
                if (m_IapMouseMoveExt == null)
                {
                    IapHookManager.IapMouseMoveExt += IapHookManager_MouseMoveExt;
                }
                m_IapMouseMoveExt += value;
            }

            remove
            {
                m_IapMouseMoveExt -= value;
                if (m_IapMouseMoveExt == null)
                {
                    IapHookManager.IapMouseMoveExt -= IapHookManager_MouseMoveExt;
                }
            }
        }

        void IapHookManager_MouseMoveExt(object sender, IapMouseEventExtArgs e)
        {
            if (m_IapMouseMoveExt != null)
            {
                m_IapMouseMoveExt.Invoke(this, e);
            }
        }

        private event EventHandler<IapMouseEventExtArgs> m_IapMouseClickExt;

        /// <summary>
        /// Occurs when a click was performed by the mouse. 
        /// </summary>
        /// <remarks>
        /// This event provides extended arguments of type <see cref="MouseEventArgs"/> enabling you to 
        /// supress further processing of mouse click in other applications.
        /// </remarks>
        public event EventHandler<IapMouseEventExtArgs> IapMouseClickExt
        {
            add
            {
                if (m_IapMouseClickExt == null)
                {
                    IapHookManager.IapMouseClickExt += IapHookManager_MouseClickExt;
                }
                m_IapMouseClickExt += value;
            }

            remove
            {
                m_IapMouseClickExt -= value;
                if (m_IapMouseClickExt == null)
                {
                    IapHookManager.IapMouseClickExt -= IapHookManager_MouseClickExt;
                }
            }
        }

        void IapHookManager_MouseClickExt(object sender, IapMouseEventExtArgs e)
        {
            if (m_IapMouseClickExt != null)
            {
                m_IapMouseClickExt.Invoke(this, e);
            }
        }


        #endregion

        //################################################################
        #region Keyboard events

        private event KeyPressEventHandler m_IapKeyPress;

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <remarks>
        /// Key events occur in the following order: 
        /// <list type="number">
        /// <item>KeyDown</item>
        /// <item>KeyPress</item>
        /// <item>KeyUp</item>
        /// </list>
        ///The KeyPress event is not raised by noncharacter keys; however, the noncharacter keys do raise the KeyDown and KeyUp events. 
        ///Use the KeyChar property to sample keystrokes at run time and to consume or modify a subset of common keystrokes. 
        ///To handle keyboard events only in your application and not enable other applications to receive keyboard events, 
        /// set the KeyPressEventArgs.Handled property in your form's KeyPress event-handling method to <b>true</b>. 
        /// </remarks>
        public event KeyPressEventHandler IapKeyPress
        {
            add
            {
                if (m_IapKeyPress==null)
                {
                    IapHookManager.IapKeyPress +=IapHookManager_KeyPress;
                }
                m_IapKeyPress += value;
            }
            remove
            {
                m_IapKeyPress -= value;
                if (m_IapKeyPress == null)
                {
                    IapHookManager.IapKeyPress -= IapHookManager_KeyPress;
                }
            }
        }

        void IapHookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (m_IapKeyPress != null)
            {
                m_IapKeyPress.Invoke(this, e);
            }
        }

        private event KeyEventHandler m_IapKeyUp;

        /// <summary>
        /// Occurs when a key is released. 
        /// </summary>
        public event KeyEventHandler IapKeyUp
        {
            add
            {
                if (m_IapKeyUp == null)
                {
                    IapHookManager.IapKeyUp += IapHookManager_KeyUp;
                }
                m_IapKeyUp += value;
            }
            remove
            {
                m_IapKeyUp -= value;
                if (m_IapKeyUp == null)
                {
                    IapHookManager.IapKeyUp -= IapHookManager_KeyUp;
                }
            }
        }

        private void IapHookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (m_IapKeyUp != null)
            {
                m_IapKeyUp.Invoke(this, e);
            }
        }

        private event KeyEventHandler m_IapKeyDown;

        /// <summary>
        /// Occurs when a key is preseed. 
        /// </summary>
        public event KeyEventHandler IapKeyDown
        {
            add
            {
                if (m_IapKeyDown == null)
                {
                    IapHookManager.IapKeyDown += IapHookManager_KeyDown;
                }
                m_IapKeyDown += value;
            }
            remove
            {
                m_IapKeyDown -= value;
                if (m_IapKeyDown == null)
                {
                    IapHookManager.IapKeyDown -= IapHookManager_KeyDown;
                }
            }
        }

        private void IapHookManager_KeyDown(object sender, KeyEventArgs e)
        {
            m_IapKeyDown.Invoke(this, e);
        }

        #endregion

        
    }
}
