/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infosys.ATR.UIAutomation.Entities;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TestProject1
{
    [TestClass]
    public class CodeGenerationTest
    {
        //[TestMethod]
        //public void GenerateConfig()
        //{
        //    AppConfig appConfig = PopulateAppConfig();
        //    using (StringWriter sw = new StringWriter())
        //    {
        //        XmlSerializer xml = new XmlSerializer(typeof(AppConfig));
        //        xml.Serialize(sw, appConfig);
        //        using (StreamWriter write = new StreamWriter(@"app.config"))
        //        {
        //            write.WriteLine(sw.ToString());
        //        }
        //    }
        //}

        //private AppConfig PopulateAppConfig()
        //{
        //    AppConfig appConfig = new AppConfig();
        //    appConfig.AppName = "SampleApp";
        //    appConfig.AppImageConfig = new ImageConfig();
        //    appConfig.AppImageConfig.StateImageConfig = new List<StateImageConfig>();
        //    appConfig.AppImageConfig.StateImageConfig.Add(new StateImageConfig {State="DEFAULT",
        //    ValidationImageName="appValidationImageName.png"});
        //    appConfig.AppControlConfig = new ControlConfig();
        //    appConfig.AppControlConfig.AutomationId = "SampleAppAutomationID-100";
        //    appConfig.AppControlConfig.ControlName = "SampleAppWindowControl";
        //    appConfig.AppControlConfig.ControlClass = "Window";
        //    appConfig.ScreenConfig = new ScreenConfig();
        //    appConfig.ScreenConfig.ScreenName = "SampleScreen1";
        //    appConfig.ScreenConfig.ScreenImageConfig = new ImageConfig();
        //    appConfig.ScreenConfig.ScreenImageConfig.StateImageConfig = new List<StateImageConfig>();
        //    appConfig.ScreenConfig.ScreenImageConfig.StateImageConfig.Add(new StateImageConfig {State="DAFAULT",ValidationImageName="screenValidationImageName.png" });
        //    appConfig.ScreenConfig.ScreenControlConfig = new ControlConfig();
        //    appConfig.ScreenConfig.ScreenControlConfig.AutomationId = "SampleAppScreenAutomationID-100";
        //    appConfig.ScreenConfig.ScreenControlConfig.ControlName = "SampleAppWindowControl";
        //    appConfig.ScreenConfig.ScreenControlConfig.ControlClass = "Window";
        //    appConfig.ScreenConfig.EntityConfig = new EntityConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityName = "SampleEntity";
        //    appConfig.ScreenConfig.EntityConfig.EntityImageConfig = new ImageConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityImageConfig.StateImageConfig = new List<StateImageConfig>();
        //    appConfig.ScreenConfig.EntityConfig.EntityImageConfig.StateImageConfig.Add(new StateImageConfig { State="DEFAULT",
        //    ValidationImageName="entityValidationImageName.png"});
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig = new ControlConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig.AutomationId = "SampleEntityAutomationID-100";
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig.ControlName = "SampleEntityWindowPaneControl";
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig.ControlClass = "WindowPane";
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig = new EntityChildConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig.EntityName = "SampleChildEntity";
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig.EntityControlConfig = new ControlConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig.EntityControlConfig.AutomationId = "SampleEntityAutomationID-100";
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig.EntityControlConfig.ControlName = "SampleChildControl";
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig.EntityControlConfig.ControlClass = "Button";
        //    return appConfig;
        //}
    }
}
