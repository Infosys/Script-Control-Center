/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     Provides data for the CharAdded event
    /// </summary>
    public class CharAddedEventArgs : EventArgs
    {
        #region Fields

        private char _ch;

        #endregion Fields


        #region Properties

        /// <summary>
        ///     Returns the character that was added
        /// </summary>
        public char Ch
        {
            get
            {
                return _ch;
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the CharAddedEventArgs class.
        /// </summary>
        /// <param name="ch">The character that was added</param>
        public CharAddedEventArgs(char ch)
        {
            _ch = ch;
        }

        #endregion Constructors
    }
}
