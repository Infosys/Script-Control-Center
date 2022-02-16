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
    ///     Type of border to print for a Page Information section
    /// </summary>
    public enum PageInformationBorder
    {
        /// <summary>
        ///     No border
        /// </summary>
        None,

        /// <summary>
        ///     Border along the top
        /// </summary>
        Top,

        /// <summary>
        ///     Border along the bottom
        /// </summary>
        Bottom,

        /// <summary>
        ///     A full border around the page information section
        /// </summary>
        Box
    }
}
