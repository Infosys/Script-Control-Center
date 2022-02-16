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
    ///     Controls color mode fore printing
    /// </summary>
    public enum PrintColorMode
    {
        /// <summary>
        ///     Normal
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     Inverts the colors
        /// </summary>
        InvertLight = 1,

        /// <summary>
        ///     Black Text on white background
        /// </summary>
        BlackOnWhite = 2,

        /// <summary>
        ///     Styled color text on white background
        /// </summary>
        ColorOnWhite = 3,

        /// <summary>
        ///     Styled color text on white background for unstyled background colors
        /// </summary>
        ColorOnWhiteDefaultBackground = 4,
    }
}
