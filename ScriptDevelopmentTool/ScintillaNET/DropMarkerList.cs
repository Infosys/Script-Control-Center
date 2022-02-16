/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using System.Collections.ObjectModel;

#endregion Using Directives


namespace ScintillaNET
{
    /// <summary>
    ///     Data structure used to store DropMarkers in the AllDocumentDropMarkers property.
    /// </summary>
    public class DropMarkerList : KeyedCollection<Guid, DropMarker>
    {
        #region Methods

        protected override Guid GetKeyForItem(DropMarker item)
        {
            return item.Key;
        }

        #endregion Methods
    }
}
