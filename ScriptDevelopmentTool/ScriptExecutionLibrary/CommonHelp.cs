/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public class CommonHelp
    {
        public static void WriteLog(string message)
        {
            //this is just for some debug need, will removed once the actual issue is fixed
            //string logfile = "d://iaplog.txt";
            //message = message + Environment.NewLine;
            //File.AppendAllText(logfile, message);
            Infosys.WEM.Infrastructure.Common.LogHandler.LogDebug(message, Infrastructure.Common.LogHandler.Layer.ScriptEngine);
        }
    }
}
