using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.SmartParts;

namespace Infosys.ATR.WFDesigner.Views
{
    [SmartPart]
    public partial class Publish
    {
        /// <summary>
        /// Sets the presenter. The dependency injection system will automatically
        /// create a new presenter for you.
        /// </summary>
        [CreateNew]
        public PublishPresenter Presenter
        {
            set
            {
                _presenter = value;
                _presenter.View = this;
            }
        }
    }
}
