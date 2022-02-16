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
    public class IndicatorConfigList : KeyedCollection<int, IndicatorConfig>
    {
        #region Fields

        private bool? _inherit;

        #endregion Fields


        #region Methods

        protected override int GetKeyForItem(IndicatorConfig item)
        {
            return item.Index;
        }

        #endregion Methods


        #region Properties

        public bool? Inherit
        {
            get
            {
                return _inherit;
            }
            set
            {
                _inherit = value;
            }
        }

        #endregion Properties
    }
}
