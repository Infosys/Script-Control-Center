/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.OCRWrapper;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using Infosys.ATR.WinUIAutomationRuntimeWrapper.Core;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
//using System.Windows;

namespace OCRTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TestJavaBasedApproach();
                //Infosys.ATR.TessaractWrapper.TessaractWrapper obj = new Infosys.ATR.TessaractWrapper.TessaractWrapper();
                //bool a = obj.CheckSaveOCRImages();
                //GetGrayScreenShot(100, 200, 50, 200);
                //ControlReadTextAreaTest();
            }
            catch(System.Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
            //FacadeReadTextAreaTest();
        }

        private static void TestJavaBasedApproach()
        {
            AutomationFacade automationFacade = new AutomationFacade(@"D:\ATR-FEA\Code\ATR\OCRTest\Cmd\javaOCR.atr", true, false, "", true, false, true);
            string canonicalPath = "JAVA_IFEA.Tool Bar.Ctrl"; //provide the canonical path for the element to be identified
            Control actual;
            actual = automationFacade.FindControl(canonicalPath);

            string data = actual.ReadTextArea(0, 0, 0, 0, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            Console.WriteLine("data: ");
            Console.WriteLine(data);
            Console.ReadKey();
        }

        private static void FacadeReadTextAreaTest()
        {
            AutomationFacade automationFacade = new AutomationFacade(@"D:\ATR-FEA\Code\ATR\OCRTest\Cmd\Cmd.atr", false, false, "", true, false, true);
            automationFacade.Sleep(5);
            string text = automationFacade.ReadTextArea(0, 60,25, 330 , "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");

            Console.WriteLine("text: ");
            Console.WriteLine(text);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void ControlReadTextAreaTest()
        {
            AutomationFacade automationFacade = new AutomationFacade(@"D:\ATR-FEA\Code\ATR\OCRTest\Cmd\Cmd.atr", true, false, "", true, false, true);

            string canonicalPath = "Cmd.Title.Title";
            Control ctrl = automationFacade.FindControl(canonicalPath);
            //ctrl.RightClick();

            //SetFontAndBg(automationFacade);


            //read ipadress
            automationFacade.Sleep(2);
            ctrl.KeyPress("ipconfig", KeyModifier.Enter);
            automationFacade.Sleep(3);
            canonicalPath = "Cmd.Title.IPv4Address";
            ctrl = automationFacade.FindControl(canonicalPath);
            System.Windows.Rect rect = ctrl.ImageBoundingRectangle;
            //string ipAddress = ctrl.ReadTextArea(430, 0, 0, 170, TextType.IPAddress);
            string ipAddress = ctrl.ReadTextArea(430, 0, 0, 170, "1234567890.");
            Console.WriteLine("ipAddress: ");
            Console.WriteLine(ipAddress);

            //////read domain
            ////automationFacade.Sleep(2);
            ////ctrl.KeyPress("ipconfig", KeyModifier.Enter);
            ////automationFacade.Sleep(3);
            ////canonicalPath = "Cmd.Title.domain";
            ////ctrl = automationFacade.FindControl(canonicalPath);
            ////rect = ctrl.ImageBoundingRectangle;
            //////string domain = ctrl.ReadTextArea(460, rect.Height * 2.6, 0, 180, "abcdefghijklmnopqrstuvwxyz.");
            ////string domain = ctrl.ReadTextArea(457, 50, 18, 177, "abcdefghijklmnopqrstuvwxyz.");
            ////Console.WriteLine("domain: ");
            ////Console.WriteLine(domain);

            //////Read mac address
            ////automationFacade.Sleep(3);
            ////ctrl.KeyPress("getmac", KeyModifier.Enter);
            ////automationFacade.Sleep(3);
            ////canonicalPath = "Cmd.Title.PhysicalAddress";
            ////ctrl = automationFacade.FindControl(canonicalPath);
            ////rect = ctrl.ImageBoundingRectangle;
            ////string macAddress = ctrl.ReadTextArea(0, 30, 0, 210, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-");
            ////Console.WriteLine("macAddress: ");
            ////Console.WriteLine(macAddress);


            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void SetFontAndBg(AutomationFacade automationFacade)
        {
            string canonicalPath = "Cmd.Title.Properties";
            Control  ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();

            canonicalPath = "Cmd.Title.ScreenBg";
            ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();
            canonicalPath = "Cmd.Title.White";
            ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();

            canonicalPath = "Cmd.Title.ScreenText";
            ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();
            canonicalPath = "Cmd.Title.Black";
            ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();

            canonicalPath = "Cmd.Title.FontTab";
            ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();
            canonicalPath = "Cmd.Title.Font12by16";
            ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();

            canonicalPath = "Cmd.Title.Ok";
            ctrl = automationFacade.FindControl(canonicalPath);
            ctrl.Click();            
        }

        public static Bitmap GetGrayScreenShot(double X, double Y, double height, double width)
        {
            Bitmap snap = new Bitmap((int)width, (int)height);

            using (var g = Graphics.FromImage(snap))
            {
                g.CopyFromScreen(new Point((int)X, (int)Y), Point.Empty, new Size((int)width, (int)height));
            }

            //Should the image be saved in some directory
            if (ConfigurationManager.AppSettings["SaveOCRImages"] != null)
            {
                if (ConfigurationManager.AppSettings["SaveOCRImages"].ToString().ToLower() == "true")
                {
                    string directory = Directory.GetCurrentDirectory();

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    string path = directory + @"\TestImages\custom_" + X.ToString() + "_" + Y.ToString() + "_" + height.ToString() + ".jpg";
                    //Save image in the path
                    snap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }

            return snap;
        }
    }
}
