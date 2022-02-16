using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Infosys.ATR.WFDesigner.Entities;
using Infosys.WEM.Service.Contracts.Data;

namespace Infosys.ATR.WFDesigner.Views
{
    public interface IPublish
    {
        List<Entities.Category> Categories { get; set; }
    }
}
