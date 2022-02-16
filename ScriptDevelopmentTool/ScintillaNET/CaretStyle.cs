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
    ///     The style of visual indicator that the caret displayes.
    /// </summary>
    public enum CaretStyle
    {
        /// <summary>
        ///     The caret is not displayed
        /// </summary>
        Invisible = 0,

        /// <summary>
        ///     A vertical line is displayed
        /// </summary>
        Line = 1,

        /// <summary>
        ///     A horizontal block is displayed that may cover the character.
        /// </summary>
        Block = 2
    }
}
