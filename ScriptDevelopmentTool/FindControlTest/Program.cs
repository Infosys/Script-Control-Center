/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Infosys.ATR.WinUIAutomationRuntimeWrapper;

namespace FindControlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
           // {
                Console.WriteLine("Going to invoke automation engine...");
                #region winform
                //AutomationFacade automationFacade = new AutomationFacade(@"Calc_multiple_find\Calc_multiple_find.atr", true, true, "", true, false, false);

                //string canonicalPath = "calculator.KeyPanel.two";
                //Control ctrl = automationFacade.FindControl(canonicalPath);

                //if (ctrl != null)
                //    ctrl.Click();

                //automationFacade.Sleep(2);

                //canonicalPath = "calculator.KeyPanel.add";
                //ctrl = automationFacade.FindControl(canonicalPath);

                //if (ctrl != null)
                //    ctrl.Click();

                //automationFacade.Sleep(2);

                //canonicalPath = "calculator.KeyPanel.five";
                //ctrl = automationFacade.FindControl(canonicalPath);

                //if (ctrl != null)
                //    ctrl.Click();

                //automationFacade.Sleep(2);

                //canonicalPath = "calculator.KeyPanel.equal";
                //ctrl = automationFacade.FindControl(canonicalPath);

                //if (ctrl != null)
                //    ctrl.Click();
                #endregion

                #region web
                //AutomationFacade automationFacade = new AutomationFacade(@"sparsh_multiple_Find\sparsh_m.atr", true, true, "", true, false, false);

                //string canonicalPath = "Sparsh.SearchPanel.SearchText";
                //Control ctrl = automationFacade.FindControl(canonicalPath);

                //if (ctrl != null)
                //{
                //    ctrl.Click();
                //    ctrl.KeyPress("infosys");
                //}

                //automationFacade.Sleep(2);

                //canonicalPath = "Sparsh.SearchPanel.SearchButton";
                //ctrl = automationFacade.FindControl(canonicalPath);

                //if (ctrl != null)
                   // ctrl.Click();
            #endregion

            #region web
            AutomationFacade automationFacade = new AutomationFacade(@"buttonslist\FIND.atr", true, true, "", true, false, false);

            List<Control> ctrls = automationFacade.FindControls("Sparsh.SearchPanel.links");

            foreach(Control ctrl in ctrls)
            {

                ctrl.Highlight();
               // Thread.Sleep(1000);
            }
            #endregion

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            Console.WriteLine("End of the automation. Pres ENTER to exit");
            Console.ReadLine();
        }
    }
}
