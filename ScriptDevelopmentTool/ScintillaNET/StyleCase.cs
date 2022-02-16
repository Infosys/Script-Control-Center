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
    ///     Represents casing styles
    /// </summary>
    public enum StyleCase
    {
        /// <summary>
        ///     Both upper and lower case
        /// </summary>
        Mixed = 0,

        /// <summary>
        ///     Only upper case
        /// </summary>
        Upper = 1,

        /// <summary>
        ///     Only lower case
        /// </summary>
        Lower = 2,
    }
}
