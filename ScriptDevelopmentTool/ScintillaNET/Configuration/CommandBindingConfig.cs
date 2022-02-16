/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET.Configuration
{
    public struct CommandBindingConfig
    {
        #region Fields

        public KeyBinding KeyBinding;
        public bool? ReplaceCurrent;
        public BindableCommand BindableCommand;

        #endregion Fields


        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the CommandBindingConfig structure.
        /// </summary>
        public CommandBindingConfig(KeyBinding keyBinding, bool? replaceCurrent, BindableCommand bindableCommand)
        {
            KeyBinding = keyBinding;
            ReplaceCurrent = replaceCurrent;
            BindableCommand = bindableCommand;
        }

        #endregion Constructors
    }
}
