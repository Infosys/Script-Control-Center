/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using Infosys.ATR.WinUIAutomationRuntimeWrapper.Core;
using System.Xml;
using System.Windows;
using System.Diagnostics;


namespace TestProject1
{
    [TestClass()]
    public class AutomationFacadeTest
    {
        #region main


        bool allApplicationsLaunched = false; // to be used to test all the events like click, doubl click, read text, etc
        AutomationFacade automationfacade = null;
        /// <summary>
        ///A test for AutomationFacade Constructor for provided xml file path and without launching applications
        ///</summary>
        [TestMethod()]
        public void AutomationFacadeConstructorTest_xmlPath_noLaunchApp()
        {
            bool LaunchApps = false;
            int appCount = 1; //provide the expected application count in the xml provided
            AutomationFacade target = LoadXML(LaunchApps);
            Assert.IsTrue(appCount == target.Applications.Count);
        }        

        /// <summary>
        ///A test for AutomationFacade Constructor for provided xml doc and without launching applications
        ///</summary>
        [TestMethod()]
        public void AutomationFacadeConstructorTest_xmlDoc()
        {
            XmlDocument automationConfigXML = new XmlDocument();
            string xMLPath = AppDomain.CurrentDomain.BaseDirectory + @"\xmls\calc.atr"; //provide the path of the xml
            if (!System.IO.File.Exists(xMLPath))
            {
                Assert.Fail("XML path- " + xMLPath + " can't be reached.");
                return;
            }
            automationConfigXML.Load(xMLPath);
            bool LaunchApps = false;
            int appCount = 2; //provide the expected application count in the xml provided
            AutomationFacade target = new AutomationFacade(automationConfigXML, LaunchApps);
            Assert.IsTrue(appCount == target.Applications.Count);
        }

        /// <summary>
        ///A test for AutomationFacade Constructor for provided xml file path and also launching applications
        ///</summary>
        [TestMethod()]
        public void AutomationFacadeConstructorTest_xmlPath_LaunchApp()
        {
            bool LaunchApps = true;
            int appCount = 1; //provide the expected application count in the xml provided
            AutomationFacade target = LoadXML(LaunchApps);
            
            if (appCount == target.Applications.Count)
            {
                //look for process id
                bool pass = true;
                foreach (Application app in target.Applications.Values)
                {
                    if (app.ProcessId == 0)
                    {
                        Assert.Inconclusive("The Process Id for application- " + app.Name + " is zero.");
                        pass = false;
                    }
                }
                if (pass)
                {
                    allApplicationsLaunched = true;
                    automationfacade = target;
                    Assert.IsTrue(true) ; //All the applications started successfully
                }
                else
                    Assert.Fail("one or more applications failed to start.");
            }
            else
                Assert.Fail("Application count is not as expected.");
        }

        /// <summary>
        ///A test for FindControl for the provided canonical path
        ///</summary>
        [TestMethod()]
        public void FindControlTest()
        {
            //assumption -> ideally need not to get the target again, instead use the one created by earlier test method
            //otherwise to many instances of the same application would be launched
            //confirm the above assumption...

            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            string canonicalPath = "Calculator.Screen 1.One"; //provide the canonical path of the control to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            Assert.IsNotNull(actual, "Control found is null, probably it is currently not available");
        }

        /// <summary>
        /// A test for right click mouse event
        /// </summary>
        [TestMethod()]
        public void RightClickTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            target.HighlightElement = true;
            string canonicalPath = "Calculator.Screen 1.LCD"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.RightClick();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        [TestMethod()]
        public void Excel_Test()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"xls\ExcelPoc.atr");
            target.HighlightElement = true;
           //wait for the manul open of the xls file
            string canonicalPath = "ExcelPoc.Sheet1.CellA1"; 
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.DataItem cell1 = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.DataItem;
                string text = cell1.ReceiveText();
                Assert.IsFalse(string.IsNullOrEmpty(text));
                cell1.KeyPress("new text");
            }
        }

        [TestMethod()]
        public void Excel_namebox_Test()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"xls\ExcelPoc.atr");
            target.HighlightElement = true;
            //wait for the manul open of the xls file
            string canonicalPath = "ExcelPoc.Sheet1.namebox";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit cell1 = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                cell1.Click();
                System.Threading.Thread.Sleep(2000);
                string text = cell1.ReceiveText();
                //Assert.IsFalse(string.IsNullOrEmpty(text));
                System.Windows.Forms.MessageBox.Show(text);
                cell1.SendText("c1");
                System.Threading.Thread.Sleep(2000);
                actual.KeyPress("", KeyModifier.Enter);

            }
        }

        [TestMethod()]
        public void Excel_Runtime_Test()
        {
            bool LaunchApps = false;
            AutomationFacade target = LoadXML(LaunchApps, @"xls\ExcelPoc.atr", "ExcelPoc");
            //AutomationFacade target = LoadXML(LaunchApps, @"xls\ExcelPoc.atr");
            target.HighlightElement = true;
            //Application app = target.FindApplication("ExcelPoc");
            //app.StartApp();
            //wait for the manul open of the xls file
            string canonicalPath = "ExcelPoc.Sheet1.CellA1";
            Control actual;
            actual = target.FindControl(canonicalPath,null,"B1");
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.DataItem cell1 = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.DataItem;
                string text = cell1.ReceiveText();
                Assert.IsFalse(string.IsNullOrEmpty(text));
            }
        }

        [TestMethod()]
        public void Excel_Runtime2_Test()
        {
            bool LaunchApps = false;
            AutomationFacade target = LoadXML(LaunchApps, @"xls\ExcelPoc.atr");
            target.HighlightElement = true;
            Application app = target.FindApplication("ExcelPoc");
            app.StartApp(@"d:\Emp_parameters.xlsx");
            Assert.IsTrue(true);
            string canonicalPath = "ExcelPoc.Sheet1.CellA1";
            Control actual;
            actual = target.FindControl(canonicalPath, null, "B1");
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.DataItem cell1 = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.DataItem;
                string text = cell1.ReceiveText();
                Assert.IsFalse(string.IsNullOrEmpty(text));
            }
        }

        [TestMethod()]
        public void Excel_Test2()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"xls\excel.atr");
            target.HighlightElement = true;
            Application app = target.FindApplication("Excel App");
            app.KeyPress("wq", KeyModifier.Menu);
            Control actual;
            string canonicalPath = "Excel App.Zoom.25";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("25% Control not found.");

            canonicalPath = "Excel App.Zoom.OK";
            
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("OK Control not found.");
        }

        /// <summary>
        /// A test for right click mouse event using image
        /// </summary>
        [TestMethod()]
        public void RightClickTest_image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "cal_image.atr");
            target.HighlightElement = true;
            target.UseTrueColorTemplateMatching = true;
            string canonicalPath = "calculator.Screen1.lcd"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.RightClick();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for right click mouse event with offset using image
        /// </summary>
        [TestMethod()]
        public void RightClickTestWithOffset_image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "cal_image.atr");
            target.HighlightElement = true;
            string canonicalPath = "calculator.Screen1.lcd"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.OffsetRegion(-100, -50);
                actual.RightClick();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for right click mouse event with offset using image
        /// </summary>
        [TestMethod()]
        public void ClickTestWithOffset_image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "cal_image.atr");
            target.HighlightElement = true;
            string canonicalPath = "calculator.Screen1.lcd"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.ClickWithOffset(-100, -200);
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for click event
        /// </summary>
        [TestMethod()]
        public void ClickTest_ondemand_appstart()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(false);
            target.HighlightElement = true;
            string canonicalPath = "Calculator.Screen 1.Four"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Application app = target.FindApplication("Calculator");
            app.StartApp();
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");

            //Application app = target.FindApplication("Calculator");
            //app.KeyPress("2", KeyModifier.ALT);
        }        

        [TestMethod()]
        public void ClickTest_Java_App()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "java-app.atr");
            string canonicalPath = "Personalized Recommendation Engine.Shopping List.New List"; 
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for click event with no screen name
        /// </summary>
        [TestMethod()]
        public void ClickTest_NoScreenName()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "calc3.atr");
            string canonicalPath = "Calculator.One"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
                                                     //note, screen name is missing
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for double click event
        /// </summary>
        [TestMethod()]
        public void DoubleClickTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            string canonicalPath = "Calculator.Screen 1.Four"; //provide the canonical path of the control to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.DoubleClick();
                Assert.IsTrue(true);//"Double Click fired on control- " + actual.Name)
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for reading text from any text area
        /// </summary>
        [TestMethod()]
        public void ReadTextTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            string canonicalPath = "Calculator.Screen 1.LCD"; //provide the canonical path of the LCD for calc
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                string text = textbox.ReceiveText();//read from the LCD
                Assert.IsTrue(!string.IsNullOrEmpty(text));
            }
            else
                Assert.Fail("Control not found.");
        }

        [TestMethod()]
        public void ReadTextTest_Java_App()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "java-app2.atr");
            string canonicalPath = "SwingSet2.Internal Frame Generator.Text Box 1"; //provide the canonical path of the LCD for calc
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                string text = textbox.ReceiveText();//read from the LCD
                Assert.IsTrue(!string.IsNullOrEmpty(text));
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for sending text to any text area
        /// </summary>
        [TestMethod()]
        public void SendTextTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            string canonicalPath = "Calculator.Screen 1.LCD"; //provide the canonical path of the LCD for calc
            string textTobeSent = "3";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);//send text to the LCD
                Assert.IsTrue(result,"Failesd to send text, probably the target text area is not editable or focusable.");
            }
            else
                Assert.Fail("Control not found.");
        }

        [TestMethod()]
        public void SendTextTest_Java_App()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps,"java-app2.atr");
            string canonicalPath = "SwingSet2.Internal Frame Generator.Text Box 1"; //provide the canonical path of the LCD for calc
            string textTobeSent = "Hello....";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
                Assert.IsTrue(result, "Failesd to send text, probably the target text area is not editable or focusable.");
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for hovering over any control
        /// </summary>
        [TestMethod()]
        public void HoverTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            string canonicalPath = "Calculator.Screen 1.One"; //provide the canonical path for the element to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Hover();
                Assert.IsTrue(true);// "Hovered over the control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for expanding any control like drop down, tree node, etc.
        /// before testing make sure the application opens up in a mode where the expected controls are visible.
        /// e.g. for calc from view menu selct 'unit conversion' to show the dropdowns on startup
        /// </summary>
        [TestMethod()]
        public void ExpandComboboxTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps,"calc2.atr");
            string canonicalPath = "Calculator.Screen 1.Control 1"; //provide the canonical path for the element to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
                if (combobox != null)
                {
                    combobox.Expand();
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("ComboBox control not found.");                
            }
            else
                Assert.Fail("Control not found.");
        }

        [TestMethod()]
        public void ExpandComboboxTest_Java_App()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "java-app2.atr");
            string canonicalPath = "SwingSet2.Tool Bar.Toggle Button 1"; //provide the canonical path for the element to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button toolbarButton = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                if (toolbarButton != null)
                {
                    toolbarButton.Click();
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("Tool bar Button control not found.");
            }
            else
                Assert.Fail("Control not found.");
            System.Threading.Thread.Sleep(200);

            canonicalPath = "SwingSet2.Tool Bar.Toggle Button 1.Presets"; //provide the canonical path for the element to be identified            
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
                if (combobox != null)
                {
                    combobox.Expand();
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("Combobox control not found.");
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for expanding any control like drop down, tree node, etc.
        /// before testing make sure the application opens up in a mode where the expected controls are visible.
        /// e.g. for calc from view menu selct 'unit conversion' to show the dropdowns on startup
        /// </summary>
        [TestMethod()]
        public void ComboboxItemSelectTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "calc2.atr");
            string canonicalPath = "Calculator.Screen 1.Control 1"; //provide the canonical path for the element to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
                if (combobox != null)
                {
                    combobox.Click();
                    combobox.KeyPress("Po");
                    //combobox.Expand();
                    //combobox.SelectSingleItem("Power");
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("ComboBox control not found.");
            }
            else
                Assert.Fail("Control not found.");
        }

        [TestMethod()]
        public void ComboboxItemSelectTest_Java_App()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "java-app2.atr");
            string canonicalPath = "SwingSet2.Tool Bar.Toggle Button 1"; //provide the canonical path for the element to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button toolbarButton = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                if (toolbarButton != null)
                {
                    toolbarButton.Click();
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("Tool bar Button control not found.");
            }
            else
                Assert.Fail("Control not found.");
            System.Threading.Thread.Sleep(200);

            canonicalPath = "SwingSet2.Tool Bar.Toggle Button 1.Presets"; //provide the canonical path for the element to be identified            
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
                if (combobox != null)
                {
                    combobox.Expand();
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("Combobox control not found.");
            }
            else
                Assert.Fail("Control not found.");
            System.Threading.Thread.Sleep(200);

            canonicalPath = "SwingSet2.Tool Bar.Toggle Button 1.Presets.Lara, Larry, Lisa"; 
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ListItem listItem = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ListItem;
                if (listItem != null)
                {
                    listItem.Click();
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("List Item control not found.");
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for selecting any control like radio button.
        /// before testing make sure the application opens up in a mode where the expected controls are visible.
        /// e.g. for calc from view menu selct 'scientific' to show the radio buttons on startup
        /// </summary>
        [TestMethod()]
        public void RadioButtonSelectTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "calc2.atr");
            string canonicalPath = "Calculator.Screen 1.Control 2"; //provide the canonical path for the element to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.RadioButton radioBtn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.RadioButton;
                if (radioBtn != null)
                {
                    if (!radioBtn.IsSelected)
                    {
                        radioBtn.Select();
                        Assert.IsTrue(true);
                    }
                    else
                        Assert.Inconclusive("Radio button is already selected");
                }
                else
                    Assert.Fail("Radio button control not found.");
            }
            else
                Assert.Fail("Control not found.");
        }

        [TestMethod()]
        public void RadioButtonSelectTest_Java_App()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "java-app.atr");
            string canonicalPath = "Personalized Recommendation Engine.Recommendation.Top 10"; 
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.RadioButton radioBtn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.RadioButton;
                if (radioBtn != null)
                {
                    if (!radioBtn.IsSelected)
                    {
                        radioBtn.Select();
                        Assert.IsTrue(true);
                    }
                    else
                        Assert.Inconclusive("Radio button is already selected");
                }
                else
                    Assert.Fail("Radio button control not found.");
            }
            else
                Assert.Fail("Control not found.");
        }

        //[TestMethod()] - tested as part of the MenuItemClickTest
        public AutomationFacade ExpandMenuTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "calc2.atr");
            string canonicalPath = "Calculator.Screen 1.Control 3"; //provide the canonical path for the element to be identified
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Menu menu = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Menu;
                if (menu != null)
                {                   
                    if (!(menu.State == Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Base.ExpandCollapseState.Expanded))
                    {
                        menu.Expand();
                        Assert.IsTrue(true);
                    }
                    else
                        Assert.Inconclusive("Menu is already expanded");
                }
                else
                    Assert.Fail("Menu control not found.");
            }
            else
                Assert.Fail("Control not found.");
            return target;
        }

        [TestMethod()]
        public void MenuItemClickTest()
        {
            //bool LaunchApps = true;
            //AutomationFacade target = LoadXML(LaunchApps, "calc2.atr");
            string canonicalPath = "Calculator.Screen 1.Control 3.Control 4"; //provide the canonical path for the element to be identified
            AutomationFacade target = ExpandMenuTest();
            //good practice- add some pause as below between each automated activities
            //otherwise subsequent element may not be available for automation
            System.Threading.Thread.Sleep(200);
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Menu menu = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Menu;
                if (menu != null)
                {
                    //menu.PropertyHasChanged += actual_PropertyHasChanged;
                    //menu.StructureHasChanged += actual_StructureHasChanged;
                    menu.Click();
                    Assert.IsTrue(true);                    
                }
                else
                    Assert.Fail("Menu control not found.");
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test to automate the Service Now login page.
        /// N.B.- make sure no other IE instances are running, if any close all instances
        /// </summary>
        [TestMethod()]
        public void ServiceNow_LoginPage_Waitforever_Test()
        {
            bool LaunchApps = false;
            AutomationFacade target = LoadXML(LaunchApps, "ServiceNow.atr");
            
            target.WaitForever = true;
            target.ShowApplicationStartingWaitBox = false;
            Application app = target.FindApplication("Service Now");
            app.StartApp();
            //pass the value to the text fields
            string canonicalPath = "Service Now.Login.FirstName";
            string textTobeSent = "Rahul";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("FirstName Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.Last Name";
            textTobeSent = "Bandopadhyaya";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Last Name Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.Business Email";
            textTobeSent = "XXXXX";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Business Email Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.Company";
            textTobeSent = "Infosys";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Company Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.Title";
            textTobeSent = "Software Eng.";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Title Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.Business Phone";
            textTobeSent = "123456789";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Business Phone Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.State or Province";
            //textTobeSent = "ON";
            //actual = target.FindControl(canonicalPath);
            //if (actual != null)
            //{
            //    Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
            //    if (combobox != null)
            //    {
            //        combobox.Click();
            //        combobox.KeyPress(textTobeSent);
            //        Assert.IsTrue(true);
            //    }
            //    else
            //        Assert.Fail("State or Province ComboBox control not found.");
            //}
            //else
            //    Assert.Fail("State or Province Control not found.");

            //using the expand approach
            textTobeSent = "ON - Ontario";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
                if (combobox != null)
                {
                    combobox.Expand();
                    //the sleep is added as in case of web application, to get the hande of the list under the combobox, some delay was happening
                    //and as result, the select operation - SelectSingleItem, was not working
                    System.Threading.Thread.Sleep(200);
                    combobox.SelectSingleItem(textTobeSent);
                    //combobox.Collapse();
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("State or Province ComboBox control not found.");
            }
            else
                Assert.Fail("State or Province Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.Country";
            textTobeSent = "CA";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
                if (combobox != null)
                {
                    combobox.Click();
                    combobox.KeyPress(textTobeSent);
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("Country ComboBox control not found.");
            }
            else
                Assert.Fail("Country Control not found.");
            System.Threading.Thread.Sleep(500);

            canonicalPath = "Service Now.Login.ITSM Solution";
            textTobeSent = "VM";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox combobox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.ComboBox;
                if (combobox != null)
                {
                    combobox.Click();
                    combobox.KeyPress(textTobeSent);
                    Assert.IsTrue(true);
                }
                else
                    Assert.Fail("ITSM Solution ComboBox control not found.");
            }
            else
                Assert.Fail("ITSM Solution Control not found.");
            System.Threading.Thread.Sleep(1000);

            canonicalPath = "Service Now.Login.Demo";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);
            }
            else
                Assert.Fail("Demo Control not found.");
        }

        [TestMethod]
        public void SimultaneousATRs_Test()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "ServiceNow.atr");

            System.Threading.Thread.Sleep(5000);
            AutomationFacade target1 = LoadXML(LaunchApps, "ServiceNow.atr");
        }

        [TestMethod]
        public void HPServiceManager_LoginPage_Test()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "GAPDemo_287.atr");

            //pass the value to the text fields
            string canonicalPath = "GAP - ServiceManager.Login.Login";
            string textTobeSent = "RahulBandopadhyaya";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Edit;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Login Control not found.");
        }

        [TestMethod]
        public void HPServiceManager_LoginPage_Test_Image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"hp\HPServiceManager_Image.atr");
            target.HighlightElement = true;

            //pass the value to the login id fields
            string canonicalPath = "ServiceManager.Login Page.Login";
            string textTobeSent = "si2y2i8";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Login ID Control not found.");

            System.Threading.Thread.Sleep(200);

            //pass the value to the password fields
            canonicalPath = "ServiceManager.Login Page.Password";
            textTobeSent = "Gap1234";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Password Control not found.");

            System.Threading.Thread.Sleep(200);

            //click login button
            canonicalPath = "ServiceManager.Login Page.LoginButton";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Login button Control not found.");

            System.Threading.Thread.Sleep(200);
        }

        [TestMethod]
        public void Google_Search_Image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"google\google_search_image.atr");

            Application app = target.FindApplication("google search");
            //app.KeyPress("t",1); //press ctrl+t : new tab
            //app.KeyPress("d", 3); //press alt+d : to go to the address bar
            //app.KeyPress()
            //app.KeyPress("r", KeyModifier.META);
            System.Threading.Thread.Sleep(2000);
            string canonicalPath = "google search.search pane.logo";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Image img = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Image;
                img.Click();
                //actual.KeyPress("r", KeyModifier.META);
            }
            else
                Assert.Fail("Logo not found.");

            System.Threading.Thread.Sleep(2000);

            canonicalPath = "google search.search pane.search field";
            string textTobeSent = "infosys ltd.";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Search field not found.");

            app.KeyPress("", 5); //press 'enter'

            System.Threading.Thread.Sleep(2000);
        }

        [TestMethod]
        public void HPServiceManager_Complete_Test_Image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"service_manager_image\service_manager_image.atr");
            target.HighlightElement = true;

            string canonicalPath = "Home Page.Login Pane.Login Id";
            string textTobeSent = "si2y2i8";
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Login ID Control not found.");

            System.Threading.Thread.Sleep(200);
            /*********************************/

            canonicalPath = "Home Page.Login Pane.Password";
            textTobeSent = "Gap1234";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Password Control not found.");

            System.Threading.Thread.Sleep(200);
            /*********************************/

            canonicalPath = "Home Page.Login Pane.Login Button";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Login button Control not found.");

            System.Threading.Thread.Sleep(20000);
            /*********************************/

            canonicalPath = "Home Page.Navigator Page.Menu";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.DoubleClick();
            }
            else
                Assert.Fail("Navigator Page.Menu Control not found.");

            System.Threading.Thread.Sleep(5000);
            /*********************************/

            canonicalPath = "Home Page.Navigator Page.Menu.Incident Management";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.DoubleClick();
            }
            else
                Assert.Fail("Navigator Page.Menu.Incident Management Control not found.");

            System.Threading.Thread.Sleep(5000);
            /*********************************/

            canonicalPath = "Home Page.Navigator Page.Menu.Incident Management.Search";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Navigator Page.Menu.Incident Management.Search Control not found.");

            System.Threading.Thread.Sleep(10000);
            /*********************************/

            canonicalPath = "Home Page.Search Fields.Assignment group";
            textTobeSent = "asi-retail-dc wms";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Home Page.Search Fields.Assignment group Control not found.");

            System.Threading.Thread.Sleep(200);
            /*********************************/

            canonicalPath = "Home Page.Search Fields.Opened After";
            textTobeSent = "09/01/14 00:00:00";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Home Page.Search Fields.Assignment group Control not found.");

            System.Threading.Thread.Sleep(200);
            /*********************************/

            canonicalPath = "Home Page.Search Fields.Opened before";
            textTobeSent = "09/15/14 02:32:13";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox textbox = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TextBox;
                bool result = textbox.SendText(textTobeSent);
            }
            else
                Assert.Fail("Home Page.Search Fields.Opened before Control not found.");

            System.Threading.Thread.Sleep(200);
            /*********************************/

            canonicalPath = "Home Page.Search Fields.Search";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Home Page.Search Fields.Search Control not found.");

            System.Threading.Thread.Sleep(10000);
            /*********************************/

            canonicalPath = "Home Page.Search Fields.List";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Home Page.Search Fields.List Control not found.");

            System.Threading.Thread.Sleep(200);
            /*********************************/

            canonicalPath = "Home Page.Search Fields.Export";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Home Page.Search Fields.Export Control not found.");

            System.Threading.Thread.Sleep(5000);
            /*********************************/

            canonicalPath = "Home Page.Search Fields.ok";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Home Page.Search Fields.ok Control not found.");

            System.Threading.Thread.Sleep(500);
            /*********************************/

            canonicalPath = "Home Page.Save file.Save";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Home Page.Save file.Save Control not found.");

            System.Threading.Thread.Sleep(10000);
            /*********************************/

            canonicalPath = "Home Page.Save file.Open Folder";
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button btn = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Button;
                btn.Click();
            }
            else
                Assert.Fail("Home Page.Save file.Open Folder Control not found.");

            System.Threading.Thread.Sleep(200);
            /*********************************/
        }

        private AutomationFacade LoadXML(bool LaunchApps, string fileName="", string appName="")
        {
            string xMLPath = "";
            if(!string.IsNullOrEmpty(fileName))
                xMLPath = AppDomain.CurrentDomain.BaseDirectory + @"\xmls\" + fileName; //provide the path of the xml
            else
                xMLPath = AppDomain.CurrentDomain.BaseDirectory + @"\xmls\calc.atr"; //default file path
            if (!System.IO.File.Exists(xMLPath))
            {
                Assert.Fail("XML path- " + xMLPath + " can't be reached.");
                return null;
            }
            AutomationFacade target = new AutomationFacade(xMLPath, LaunchApps, true,  appName);
            //AutomationFacade target = new AutomationFacade(xMLPath, LaunchApps);
            if (target == null || target.Applications == null)
            {
                Assert.Fail("Load XML failed, either Auomation Facade or Applications is null");
                return null;
            }
            return target;
        }

        

        /// <summary>
        /// A test for click event
        /// </summary>
        [TestMethod()]
        public void ClickTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            target.HighlightElement = true;
            string canonicalPath = "Calculator.Screen 1.Four"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");

            //Application app = target.FindApplication("Calculator");
            //app.KeyPress("2", KeyModifier.ALT);
        }

        /// <summary>
        /// A test for click event
        /// </summary>
        [TestMethod()]
        public void ClickTest_Waitforever()
        {
            bool LaunchApps = false;
            AutomationFacade target = LoadXML(LaunchApps);
            target.HighlightElement = true;
            target.WaitForever = true;
            target.ShowApplicationStartingWaitBox = false;
            Application app = target.FindApplication("Calculator");
            app.StartApp();
            //target.Sleep(10);
            string canonicalPath = "Calculator.Screen 1.Four"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");

            //Application app = target.FindApplication("Calculator");
            //app.KeyPress("2", KeyModifier.ALT);
        }

        /// <summary>
        /// A test for click event using image
        /// </summary>
        [TestMethod()]
        public void ClickTest_Image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, "cal_image.atr");
            target.HighlightElement = true;
            string canonicalPath = "calculator.Screen1.one"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for click event using image
        /// </summary>
        [TestMethod()]
        public void ClickTest_Color_Image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"colorbaseddetection\ColorBased.atr");
            target.HighlightElement = true;
            target.UseTrueColorTemplateMatching = true;
            string canonicalPath = "ColorStub.Panel.Red"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.DoubleClick();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// A test for FindControls
        /// </summary>
        [TestMethod()]
        public void FindControls_Test()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps);
            target.HighlightElement = true;
            string canonicalPath = "Calculator.Screen 1.Buttons"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            List<Control> ctrls = target.FindControls(canonicalPath);
            System.Windows.Forms.MessageBox.Show(ctrls.Count.ToString());

            //change the ui to show more buttons and then call find controls again
            ctrls = target.FindControls(canonicalPath);
            System.Windows.Forms.MessageBox.Show(ctrls.Count.ToString());
        }

        /// <summary>
        /// A test for FindControls using image
        /// </summary>
        [TestMethod()]
        public void FindControls_Test_Image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"colorbaseddetection\ColorBased.atr");
            target.HighlightElement = true;
            target.UseTrueColorTemplateMatching = true;
            target.WaitForever = true;
            //string canonicalPath = "ColorStub.Panel.White"; //will return 1
            string canonicalPath = "ColorStub.Panel.Blue"; //will return 3 as blue and red looks similar in gray scale
            List<Control> ctrls = target.FindControls(canonicalPath);
            System.Windows.Forms.MessageBox.Show(ctrls.Count.ToString());

            //check then again call find controls
            ctrls = target.FindControls(canonicalPath);
            System.Windows.Forms.MessageBox.Show(ctrls.Count.ToString());
        }

        /// <summary>
        /// A test to verify drag interface
        /// </summary>
        [TestMethod()]
        public void Excel_CellDrag_Test()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"xls\ExcelPoc.atr");
            target.HighlightElement = true;
            //wait for the manul open of the xls file
            string startcanonicalPath = "ExcelPoc.Sheet1.CellA1";
            string endcanonicalPath = "ExcelPoc.Sheet1.CellD1";
            target.Drag(startcanonicalPath, endcanonicalPath);
        }

        /// <summary>
        /// A test to verify drag interface
        /// </summary>
        [TestMethod()]
        public void Excel_CellDrag_Test2()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"xls\ExcelPoc.atr");
            target.HighlightElement = true;
            //wait for the manul open of the xls file
            string startcanonicalPath = "ExcelPoc.Sheet1.CellA1";
            target.Drag(startcanonicalPath, 200,0, DragDestinationType.RelativePosition);
        }

        /// <summary>
        /// A test for click event
        /// </summary>
        [TestMethod()]
        public void Event_Test()
        {
            bool LaunchApps = false;
            AutomationFacade target = LoadXML(LaunchApps);
            //target.FocusHasChanged += target_FocusHasChanged;
            AutomationFacade.FocusHasChangedEventHandler focushandler = new AutomationFacade.FocusHasChangedEventHandler(target_FocusHasChanged);
            target.SubscribeToFocusChangeEvent(focushandler);

            Application app = target.FindApplication("Calculator");
            target.HighlightElement = true;
            string canonicalPath = "Calculator.Screen 1.Four"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            //app.PropertyHasChanged += app_PropertyHasChanged;
            //app.StructureHasChanged += app_StructureHasChanged; 
            app.StartApp();
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                //actual.PropertyHasChanged += actual_PropertyHasChanged;
                //actual.StructureHasChanged += actual_StructureHasChanged;
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");

            //Application app = target.FindApplication("Calculator");
            //app.KeyPress("2", KeyModifier.ALT);
            int counter = 5;
            while (counter != 0) { counter--; System.Threading.Thread.Sleep(1000); }
            target.DesubscribeToFocusChangeEvent(target_FocusHasChanged);
        }

        /// <summary>
        /// A test for click event using image
        /// </summary>
        [TestMethod()]
        public void RestrictedSearch_Test_Color_Image()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"RestrictedSearch\restrict.atr");
            target.HighlightElement = true;
            target.UseTrueColorTemplateMatching = true;
            string canonicalPath = "ColorApp.Screen.Region1.blue"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                actual.Click();
                Assert.IsTrue(true);//"Click fired on control- " + actual.Name
            }
            else
                Assert.Fail("Control not found.");
        }

        /// <summary>
        /// The test depends on the html - @"POCTable\download.html. So deploy is on IIS and accordingly change the application path in the atr.
        /// </summary>
        [TestMethod()]
        public void Table_Test()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"POCTable\POCTable.atr");
            target.HighlightElement = true;
            target.UseTrueColorTemplateMatching = true;
            string canonicalPath = "Table.Screen 1.Table1"; //provide the canonical path of the control to be identified e.g. button 1 on calculator
            Control actual;
            actual = target.FindControl(canonicalPath);
            if (actual != null)
            {
                Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Table table = actual as Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.Table;
                table.CellsPerRow = 2;
                List<Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls.TableRow> rows = table.GetAllRows();
                List<int> indexes = rows[0].GetCellIndexesWithtext("Cell 1a");
            }
            else
                Assert.Fail("Control not found.");
        }

        [TestMethod]
        public void ApplicationOverwrite_Test()
        {
            bool LaunchApps = true;
            //in the GetWindowsHandle wait, untill the timeout (default 3 secs) to get handle reached and hence no windows handle will be assigned
            AutomationFacade target = LoadXML(LaunchApps, "cal_image.atr");
            target.HighlightElement = true;
            Application app = target.FindApplication("calculator", "Calculator");
        }

        [TestMethod]
        public void MSDoc_ReadRectangle()
        {
            bool LaunchApps = false;
            AutomationFacade target = LoadXML(LaunchApps, @"ms_doc\MSDOC.atr");
            target.ShowApplicationStartingWaitBox = false;
            Application app = target.FindApplication("MsWord");
            app.StartApp();

            while(!app.IsAvailable)
                app = target.FindApplication("MsWord");

            target.HighlightElement = true;
            target.WaitForever = true;
            string canonicalpath = "MsWord.EditPane.Column_6_2.Data_6_2_2";
            //Control cell = target.FindControl(canonicalpath);

            List<Control> ctrls = target.FindControls(canonicalpath);
            System.Windows.Forms.MessageBox.Show("total controls: " + ctrls.Count);
            target.Sleep(2);
            //canonicalpath = "MsWord.EditPane.Column_6_2.Data_6_2_2";
            //cell = target.FindControl(canonicalpath);

            //ctrls = target.FindControls(canonicalpath);

            //cell.DoubleClick();
            //target.Sleep(2);
            //cell.KeyPress("new text");

            //canonicalpath = "MsWord.EditPane.Column_6_2";
            //cell = target.FindControl(canonicalpath);
        }

        [TestMethod()]
        public void FindControl_Test_Image_waitforever()
        {
            bool LaunchApps = true;
            AutomationFacade target = LoadXML(LaunchApps, @"colorbaseddetection\ColorBased.atr");
            target.HighlightElement = true;
            target.UseTrueColorTemplateMatching = true;
            target.WaitForever = true;
            target.MultipleScaleTemplateMatching = false;
            string canonicalPath = "ColorStub.Panel.White"; //will return 1
            Control ctrl = target.FindControl(canonicalPath);
            if (ctrl == null)
                System.Windows.Forms.MessageBox.Show("not found");
            else
                System.Windows.Forms.MessageBox.Show("found");
        }

        void target_FocusHasChanged(AutomationFacade.FocusHasChangedArgs e)
        {           
            System.Windows.Forms.MessageBox.Show("automation facade focus changed");
        }

        void app_StructureHasChanged(Application.StructureHasChangedArgs e)
        {
            System.Windows.Forms.MessageBox.Show("app structure changed");
        }

        void app_PropertyHasChanged(Application.PropertyHasChangedArgs e)
        {
            System.Windows.Forms.MessageBox.Show("app property changed");
        }

        void actual_StructureHasChanged(Control.StructureHasChangedArgs e)
        {
            System.Windows.Forms.MessageBox.Show("structure changed");
        }

        void actual_PropertyHasChanged(Control.PropertyHasChangedArgs e)
        {
            System.Windows.Forms.MessageBox.Show("property changed");
        }

        #endregion

        [TestMethod()]
        public void DemoClickTest()
        {
            bool LaunchApps = true;
            AutomationFacade target = new AutomationFacade(@"D:\IMS\KT_related\KTCalc.atr", true, true, "", true);//provide the constructor parameters
            string canonicalPath = "Calculator.keypad.four";
            Control actual;
            actual = target.FindControl(canonicalPath);           
            actual.Click();

            canonicalPath = "Calculator.keypad.multiple";
            actual = target.FindControl(canonicalPath);
            actual.Click();

            canonicalPath = "Calculator.keypad.six";
            actual = target.FindControl(canonicalPath);
            actual.Click();

            canonicalPath = "Calculator.keypad.equal";
            actual = target.FindControl(canonicalPath);
            actual.Click();
           
        }

        [TestMethod()]
        public void DemoMouseEventTest() 
        {
            bool LaunchApps = true;
            AutomationFacade automationFacade = new AutomationFacade(@"D:\Test\AutomationTestExample\AutomationTestExample\Sparsh\Sparsh_i.atr", true, true, "", true);
            automationFacade.HighlightElement = true;
            System.Threading.Thread.Sleep(2000);
            string canonicalPath = "sparsh_i.Search_panel.Harmony"  ;
            Control ctrlRef;
            ctrlRef = automationFacade.FindControl(canonicalPath);
            Rect ctrlRectangle = ctrlRef.ImageBoundingRectangle;
            ctrlRef.OffsetRegion(Convert.ToInt32(ctrlRectangle.X), Convert.ToInt32(ctrlRectangle.Y));
            if (ctrlRef != null)
            {
                ctrlRef.Hover();
                Debug.WriteLine("before MouseDown");
                ctrlRef.MouseDown();
                automationFacade.Sleep(5);
                Debug.WriteLine("after MouseDown");
            }
        }      

       

    }
}
