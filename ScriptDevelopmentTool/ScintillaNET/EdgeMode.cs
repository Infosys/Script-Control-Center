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
    ///     How long lines are visually indicated
    /// </summary>
    public enum EdgeMode
    {
        /// <summary>
        ///     No indication
        /// </summary>
        None = 0,

        /// <summary>
        ///     A vertical line is displayed
        /// </summary>
        Line = 1,

        /// <summary>
        ///     The background color changes
        /// </summary>
        Background = 2,
    }
}
