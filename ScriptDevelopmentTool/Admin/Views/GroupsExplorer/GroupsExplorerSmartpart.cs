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
    public partial class GroupsExplorer
    {
        /// <summary>
        /// Sets the presenter. The dependency injection system will automatically
        /// create a new presenter for you.
        /// </summary>
        [CreateNew]
        public GroupsExplorerPresenter Presenter
        {
            set
            {
                _presenter = value;
                _presenter.View = this;
            }
        }
    }
}
