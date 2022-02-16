/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using System.ComponentModel;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     Provides data for a DropMarkerCollect event
    /// </summary>
    public class DropMarkerCollectEventArgs : CancelEventArgs
    {
        #region Fields

        private DropMarker _dropMarker;

        #endregion Fields


        #region Properties

        /// <summary>
        ///     Returns the DropMarker that was collected
        /// </summary>
        public DropMarker DropMarker
        {
            get
            {
                return _dropMarker;
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the DropMarkerCollectEventArgs class.
        /// </summary>
        public DropMarkerCollectEventArgs(DropMarker dropMarker)
        {
            _dropMarker = dropMarker;
        }

        #endregion Constructors
    } 
}
