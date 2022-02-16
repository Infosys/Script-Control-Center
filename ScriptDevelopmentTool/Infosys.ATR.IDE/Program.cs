/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Configuration;

namespace Infosys.ATR.DevelopmentStudio
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string executionMode = ConfigurationManager.AppSettings["ExecutionMode"];
            if (!string.IsNullOrEmpty(executionMode))
            {
                switch(executionMode)
                {
                    case "ControlLookup":
                        Application.Run(new ControlExplorer());
                        //Application.Run(new Shell());
                        break;
                    case "Snapper":
                        Application.Run(new Snapper());
                        break;
                    default:
                        Application.Run(new IDE(args));
                        break;
                }
            }
        }
    }
}
