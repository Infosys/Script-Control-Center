/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.Solutions.CodeGeneration.Framework;
using Infosys.ATR.CodeGeneration.Filler;
using System.Web.Script.Serialization;
using Infosys.ATR.UIAutomation.Entities;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Serialization;


namespace Infosys.ATR.CodeGeneration
{


    public class CodeGenerator
    {
        static Application ctrl = null;

        public static string Generate(Infosys.ATR.UIAutomation.Entities.Script script)
        {
            BaseFiller baseFiller = new BaseFiller(script);
            Template template = Template.FromFile(@"Templates\BaseTemplate.txt");
            baseFiller.ContentTemplate = template;
            return baseFiller.GenerateContent();
        }

        //public static string GenerateJsonConfig(Desktop desktop)
        //{
        //    GenerateConfig();
        //    if (ctrl == null)
        //    {
        //        ctrl = GetControls(desktop);
        //    }
        //    return new JavaScriptSerializer().Serialize(ctrl);
        //}

        public static string GenerateConfig(Desktop desktop)
        {
        //{
        //    Application ctrl = GetControls(desktop);
        //    XmlSerializer xml = new XmlSerializer(typeof(Application));
        //    StringWriter sw = null;

        //    using (sw = new StringWriter())
        //    {
        //        xml.Serialize(sw, ctrl);
        //    }

        //    return sw.ToString();

            return null;

        }


        //public static void GenerateConfig()
        //{
        //  //  AutomationConfig appConfig = PopulateAppConfig();

         
        //    using (UTF8Writer writer = new UTF8Writer())
        //    {
        //        XmlSerializer xml = new XmlSerializer(typeof(AutomationConfig));
        //        xml.Serialize(writer, appConfig);
        //        System.IO.File.WriteAllText(@"app.config", writer.ToString());
        //    }
           
        //    var s = Serialize<AutomationConfig>(appConfig);
        //    System.IO.File.WriteAllText(@"app.json", s);
        //}

        //private static AutomationConfig PopulateAppConfig()
        //{
        //    AutomationConfig config = new AutomationConfig();
        //    config.Name = "Sample";
        //    AppConfig appConfig = new AppConfig();           
        //    config.AppConfig = appConfig;            
        //    appConfig.AppName = "SampleApp";
        //    appConfig.AppImageConfig = new ImageConfig();
        //    appConfig.AppImageConfig.StateImageConfig = new List<StateImageConfig>();
        //    appConfig.AppImageConfig.StateImageConfig.Add(new StateImageConfig
        //    {
        //        State = "DEFAULT",
        //        ValidationImageName = "appValidationImageName.png"
        //    });
        //    appConfig.AppControlConfig = new ControlConfig();
        //    appConfig.AppControlConfig.AutomationId = "SampleAppAutomationID-100";
        //    appConfig.AppControlConfig.ControlName = "SampleAppWindowControl";
        //    appConfig.AppControlConfig.ControlClass = "Window";
        //    appConfig.ScreenConfig = new ScreenConfig();
        //    appConfig.ScreenConfig.ScreenName = "SampleScreen1";
        //    appConfig.ScreenConfig.ScreenImageConfig = new ImageConfig();
        //    appConfig.ScreenConfig.ScreenImageConfig.StateImageConfig = new List<StateImageConfig>();
        //    appConfig.ScreenConfig.ScreenImageConfig.StateImageConfig.Add(new StateImageConfig { State = "DAFAULT", ValidationImageName = "screenValidationImageName.png" });
        //    appConfig.ScreenConfig.ScreenControlConfig = new ControlConfig();
        //    appConfig.ScreenConfig.ScreenControlConfig.AutomationId = "SampleAppScreenAutomationID-100";
        //    appConfig.ScreenConfig.ScreenControlConfig.ControlName = "SampleAppWindowControl";
        //    appConfig.ScreenConfig.ScreenControlConfig.ControlClass = "Window";
        //    appConfig.ScreenConfig.EntityConfig = new EntityConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityName = "SampleEntity";
        //    appConfig.ScreenConfig.EntityConfig.EntityImageConfig = new ImageConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityImageConfig.StateImageConfig = new List<StateImageConfig>();
        //    appConfig.ScreenConfig.EntityConfig.EntityImageConfig.StateImageConfig.Add(new StateImageConfig
        //    {
        //        State = "DEFAULT",
        //        ValidationImageName = "entityValidationImageName.png"
        //    });
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig = new ControlConfig();
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig.AutomationId = "SampleEntityAutomationID-100";
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig.ControlName = "SampleEntityWindowPaneControl";
        //    appConfig.ScreenConfig.EntityConfig.EntityControlConfig.ControlClass = "WindowPane";
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig =new List<EntityChildConfig>();
        //    EntityChildConfig child = new EntityChildConfig();
        //    child.EntityName = "SampleChildEntity";
        //    child.EntityControlConfig = new ControlConfig();
        //    child.EntityControlConfig.AutomationId = "SampleEntityAutomationID-100";
        //    child.EntityControlConfig.ControlName = "SampleChildControl";
        //    child.EntityControlConfig.ControlClass = "Button";
        //    appConfig.ScreenConfig.EntityConfig.EntityChildConfig.Add(child);
        //    return config;
        //}

        public static string Serialize<T>(T obj)
        {
            //var name = obj.GetType().Name;
            AutomationConfig automationConfig = null;
            dynamic configObj = new
            { 
                automationConfig = obj
            };

            //JavaScriptSerializer serialize = new JavaScriptSerializer();
            //return serialize.Serialize(configObj);

            return Newtonsoft.Json.JsonConvert.SerializeObject(configObj);

            //DataContractJsonSerializer serializer =
            //    new DataContractJsonSerializer(typeof(T), typeof(T).Name);
            //MemoryStream ms = new MemoryStream();
            //XmlDictionaryWriter w = JsonReaderWriterFactory.CreateJsonWriter(ms);
            //w.WriteStartElement("root");
            //w.WriteAttributeString("type", "object");
            //serializer.WriteObject(w, obj);
            //w.WriteEndElement();
            //w.Flush();
            //string retVal = Encoding.Default.GetString(ms.ToArray());
            //ms.Dispose();
            //return retVal;
        }


        //private static Application GetControls(Desktop _desktop)
        //{
        //    Application ctrl = new Application();

        //    AutomationConfig config = new AutomationConfig();

        //    _desktop.Applications.ForEach(
        //        a =>
        //        {
        //            config.AppConfig = new AppConfig();

        //            config.AppConfig.AppName = a.DisplayText;
        //            config.AppConfig.AppImageConfig = new ImageConfig();
        //            config.AppConfig.AppImageConfig.StateImageConfig = new List<StateImageConfig>();
        //            config.AppConfig.AppImageConfig.StateImageConfig.Add(new StateImageConfig {State="CENTER",
        //            ValidationImageName=GetDirectoryName(a.Images.Center)});
        //            config.AppConfig.AppImageConfig.StateImageConfig.Add(new StateImageConfig
        //            {State = "RIGHT",ValidationImageName = GetDirectoryName(a.Images.Right)});
        //            config.AppConfig.AppImageConfig.StateImageConfig.Add(new StateImageConfig
        //            {State = "LEFT",ValidationImageName = GetDirectoryName(a.Images.Left)});
        //            config.AppConfig.AppImageConfig.StateImageConfig.Add(new StateImageConfig
        //            {State = "UP",ValidationImageName = GetDirectoryName(a.Images.Up)});
        //            config.AppConfig.AppImageConfig.StateImageConfig.Add(new StateImageConfig
        //            {State = "DOWN",ValidationImageName = GetDirectoryName(a.Images.Down)});
        //            config.AppConfig.AppControlConfig = new ControlConfig();
        //            var winCtrl = a.ElementAttributes as WindowControl;
        //            config.AppConfig.AppControlConfig.AutomationId = winCtrl.AutomationId;
        //            config.AppConfig.AppControlConfig.ControlClass = winCtrl.ClassName;
        //            config.AppConfig.AppControlConfig.ControlName = winCtrl.Name;
        //            config.AppConfig.ScreenConfig = new ScreenConfig();
        //            config.AppConfig.ScreenConfig.EntityConfig = new EntityConfig();
                    

        //            ctrl.Control = new Control();
        //            ctrl.Control.Images = new Image();
        //            ctrl.Control.Images.Center = GetDirectoryName(a.Images.Center);
        //            ctrl.Control.Images.Down = GetDirectoryName(a.Images.Down);
        //            ctrl.Control.Images.Left = GetDirectoryName(a.Images.Left);
        //            ctrl.Control.Images.Right = GetDirectoryName(a.Images.Right);
        //            ctrl.Control.Images.Top = GetDirectoryName(a.Images.Up);
        //            ctrl.Control.Images.Validate = GetDirectoryName(a.Images.Validation);
        //            ctrl.Control.Win32 = new Win32();
        //            ctrl.Control.Win32.Name = a.DisplayText;
        //            ctrl.Control.Win32.Id = (a.ElementAttributes as WindowControl).AutomationId;
        //            ctrl.Control.Children = new List<Control>();
        //            GetChildren(a.Children, ctrl);

        //        });

           

        //    return ctrl;
        //}

        private static string GetDirectoryName(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                return Path.GetDirectoryName(path);
            }
            return "";
        }

        private static void GetChildren(List<UIElement> children, Application ctrl)
        {

            children.ForEach(
                c =>
                {
                    Control c1 = new Control();
                    c1.Images = new Image();
                    c1.Images.Center = GetDirectoryName(c.Images.Center);
                    c1.Images.Down = GetDirectoryName(c.Images.Down);
                    c1.Images.Left = GetDirectoryName(c.Images.Left);
                    c1.Images.Right = GetDirectoryName(c.Images.Right);
                    c1.Images.Top = GetDirectoryName(c.Images.Up);
                    c1.Images.Validate = GetDirectoryName(c.Images.Validation);
                    c1.Win32 = new Win32();
                    c1.Win32.Name = c.DisplayText;
                    c1.Win32.Id = (c.ElementAttributes as WindowControl).AutomationId;
                    ctrl.Control.Children.Add(c1);
                    c1.Children = new List<Control>();
                    c.Children.ForEach(
                        cl =>
                        {
                            Control ctrl1 = new Control();
                            ctrl1.Images = new Image();
                            ctrl1.Images.Center = GetDirectoryName(cl.Images.Center);
                            ctrl1.Images.Down = GetDirectoryName(cl.Images.Down);
                            ctrl1.Images.Left = GetDirectoryName(cl.Images.Left);
                            ctrl1.Images.Right = GetDirectoryName(cl.Images.Right);
                            ctrl1.Images.Top = GetDirectoryName(cl.Images.Up);
                            ctrl1.Images.Validate = GetDirectoryName(cl.Images.Validation);
                            ctrl1.Win32 = new Win32();
                            ctrl1.Win32.Name = cl.DisplayText;
                            ctrl1.Win32.Id = (cl.ElementAttributes as WindowControl).AutomationId;
                            c1.Children.Add(ctrl1);

                        }
                    );

                });


        }

    }

    public class UTF8Writer : StringWriter
    {

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
