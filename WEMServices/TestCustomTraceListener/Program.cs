/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.Infrastructure.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestCustomTraceListener
{
    class Program
    {
        static void Main(string[] args)
        {
            
           LogHandler.SynchronizeOfflineTrackUsageLogsWithDB();
        }

        private static void testLog()
        {


            TraceManager traceMgr = new TraceManager(EnterpriseLibraryContainer.Current.GetInstance<LogWriter>());
            Tracer tracer = traceMgr.StartTrace("Performance");

        }
    }
}
