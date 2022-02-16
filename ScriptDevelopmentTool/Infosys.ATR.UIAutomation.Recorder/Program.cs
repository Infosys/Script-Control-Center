/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using Infosys.LicenseValidationClient;

namespace Infosys.ATR.UIAutomation.Recorder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {            
            string _noLicense = "";
            try
            {
                //ValidationResult result = Validator.Validate();
                //if ((result.IsSuccess && result.FeaturesAllowed != null && result.FeaturesAllowed.Count > 0))
                //{
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
                //}
                //else
                //{
                //    _noLicense = "You do not have license to run this Application. Contact Administrator";
                //    MessageBox.Show(_noLicense, "Recorder",MessageBoxButtons.OK,MessageBoxIcon.Error);                   
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Recorder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
