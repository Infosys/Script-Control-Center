/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.ObjectBuilder;
using IMSWorkBench.Infrastructure.Interface;

namespace Infosys.ATR.Admin.Views
{
    [SmartPart]
    public partial class Explorer
    {
        /// <summary>
        /// Sets the presenter. The dependency injection system will automatically
        /// create a new presenter for you.
        /// </summary>
        [CreateNew]
        public ExplorerPresenter Presenter
        {
            set
            {
                _presenter = value;
                _presenter.View = this;
            }
        }
    }
}
