using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.ATR.UIAutomation.SEE
{
    public delegate void PauseEventHandler(bool e);
    public class Events
    {        
        public static event PauseEventHandler Pause;
    }


}
