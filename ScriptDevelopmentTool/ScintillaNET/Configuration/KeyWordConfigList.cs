/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
#region Using Directives

using System;
using System.Collections.ObjectModel;

#endregion Using Directives


namespace ScintillaNET.Configuration
{
    public class KeyWordConfigList : KeyedCollection<int, KeyWordConfig>
    {
        #region Methods

        protected override int GetKeyForItem(KeyWordConfig item)
        {
            return item.List;
        }

        #endregion Methods
    }
}
