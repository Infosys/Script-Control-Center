/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TestProject1
{

    [TestClass()]
    public class ATRMapperTest
    {
        [TestMethod()]
        public void DemoTest()
        {
            Utilities.Write(@"D:\Output\20150414134144_20042014_11.atr",
            Utilities.Serialize(clsATRMapper.ConvertToATR(@"D:\20150414134144.atrwb", 4)));
        }
    }
    public class clsATRMapper
    {
        static string _baseImageDir = ConfigurationManager.AppSettings["baseImageDir"];
        static string _ProjectMode = "Win32";


        public static Infosys.ATR.UIAutomation.Entities.AutomationConfig ConvertToATR(string filepath, int AppClustIdentifyer = 1)
        {
            Infosys.ATR.UIAutomation.Entities.UseCase useCase = null;
            Infosys.ATR.UIAutomation.Entities.AutomationConfig autoConfig = null;

            using (StreamReader s = new StreamReader(filepath))
            {
                useCase = Utilities.Deserialize<Infosys.ATR.UIAutomation.Entities.UseCase>(s.ReadToEnd());
            }

            if (useCase != null)
            {
                List<Infosys.ATR.UIAutomation.Entities.AppConfig> lstAppConfig = new List<Infosys.ATR.UIAutomation.Entities.AppConfig>();
                List<Infosys.ATR.UIAutomation.Entities.EntityConfig> listEntityconfig;
                List<Infosys.ATR.UIAutomation.Entities.ScreenConfig> lstScreenConfig;

                foreach (var activity in useCase.Activities)
                {
                    #region local variables

                    listEntityconfig = new List<Infosys.ATR.UIAutomation.Entities.EntityConfig>();
                    lstScreenConfig = new List<Infosys.ATR.UIAutomation.Entities.ScreenConfig>();

                    string _ApplicationLocationPath = string.Empty,
                           _WebBrowser = string.Empty,
                           _WebBrowserVersion = string.Empty,
                           _StringToCampare = string.Empty,
                           _ApplicationName = string.Empty,
                           _ScreenName = "Default_Screen";

                    #endregion
                    _ApplicationLocationPath = activity.TargetApplication.TargetApplicationAttributes.SingleOrDefault(a => a.Name.Equals("ApplicationName")).Value.ToString();
                    #region Manupulation To set name of Aplication,Screen,etc.
                    if (activity.TargetApplication.ApplicationType.Equals("WebApplication"))
                    {
                        if (_ApplicationLocationPath.Split(new char[] { '/' }).Length > (AppClustIdentifyer + 2))
                        {
                            int index = _ApplicationLocationPath.IndexOf(_ApplicationLocationPath.Split(new char[] { '/' })[AppClustIdentifyer + 1]);
                            _ApplicationName = _ApplicationLocationPath.Substring(0, index).Replace("://", "_")
                                                                                           .Replace("/", "_")
                                                                                           .Replace(".", "_")
                                                + "_" + _ApplicationLocationPath.Split(new char[] { '/' })[AppClustIdentifyer + 1];
                        }
                        else
                            _ApplicationName = _ApplicationLocationPath.Replace("://", "_")
                                                                       .Replace("/", "_")
                                                                       .Replace(".", "_");

                        if (_ApplicationLocationPath.Split(new char[] { '/' }).Length > (AppClustIdentifyer + 2))
                        {
                            if (AppClustIdentifyer.Equals(1))
                            {
                                int index = _ApplicationLocationPath.IndexOf(_ApplicationLocationPath.Split(new char[] { '/' })[AppClustIdentifyer + 1]);
                                _StringToCampare = _ApplicationLocationPath.Substring(0, index) + _ApplicationLocationPath.Split(new char[] { '/' })[AppClustIdentifyer + 1];
                            }
                            else
                            {
                                int index = _ApplicationLocationPath.IndexOf(_ApplicationLocationPath.Split(new char[] { '/' })[AppClustIdentifyer + 2]);
                                if (index >= 0)
                                    _StringToCampare = _ApplicationLocationPath.Substring(0, index);
                            }
                        }

                        if (string.IsNullOrEmpty(_StringToCampare))
                            _StringToCampare = _ApplicationLocationPath;

                        _ScreenName = _ApplicationLocationPath.Split(new char[] { '/' }).Length > AppClustIdentifyer + 2 ? _ApplicationLocationPath.Split(new char[] { '/' })[AppClustIdentifyer + 2].Split(new char[] { '.' })[0] : "";

                        int i = AppClustIdentifyer + 2;
                        while (string.IsNullOrEmpty(_ScreenName))
                        {
                            i--;
                            _ScreenName = _ApplicationLocationPath.Split(new char[] { '/' }).Length > i ? _ApplicationLocationPath.Split(new char[] { '/' })[i] : "";
                        }

                        if (string.IsNullOrEmpty(_ApplicationName))
                            _ApplicationName = _ScreenName;

                        _WebBrowser = activity.TargetApplication.TargetApplicationAttributes.SingleOrDefault(a => a.Name.Equals("ModuleName")).Value.ToString();

                        switch (_WebBrowser.ToLower())
                        {
                            case "iexplore.exe":
                                _WebBrowser = "Internet Explorer";
                                break;
                            case "chrome.exe":
                                _WebBrowser = "Chrome";
                                break;
                            case "firefox.exe":
                                _WebBrowser = "Firefox";
                                break;
                            default:
                                _WebBrowser = "Internet Explorer";
                                break;
                        }
                    }
                    else if (activity.TargetApplication.ApplicationType.Equals("Win32"))
                    {
                        _ApplicationName = _ApplicationLocationPath.Replace(".exe", "");
                        _ScreenName = string.Format("{0}_Screen", _ApplicationName);
                        _StringToCampare = _ApplicationName;
                    }
                    #endregion
                    #region Adding Controls
                    foreach (var Task in activity.Tasks)
                    {
                        if (!Task.Event.ToString().ToLower().Equals("keyboardkeypress"))
                        {
                            if (activity.TargetApplication.ApplicationType.Equals("Win32") && Task.WindowTitle != "")
                                _ScreenName = Task.WindowTitle;

                            #region Add Entity
                            //listEntityconfig.Add(new EntityConfig()
                            //{
                            //    EntityName = Task.ControlName,
                            //    Parent = "Screen",
                            //    EntityControlConfig = new ControlConfig()
                            //{
                            //    ControlName = Task.ControlName,
                            //    AutomationId = Task.ControlId,
                            //    ControlClass = Task.ControlType.Replace("ControlType.", ""),
                            //    ControlPath = Task.ApplictionTreePath
                            //},
                            //    EntityImageConfig = new ImageConfig()
                            //    {
                            //        StateImageConfig = new List<StateImageConfig>() 
                            //    { 
                            //        new StateImageConfig()      
                            //        {   
                            //            State="",
                            //            CenterImageName= new CenterImageSearchConfig(){ ImageName=string.Format("{0}.jpg",Task.Id)}
                            //        }
                            //    }
                            //    }
                            //});
                            #endregion

                            Infosys.ATR.UIAutomation.Entities.EntityConfig entity = new Infosys.ATR.UIAutomation.Entities.EntityConfig()
                            {
                                EntityName = Task.ControlName,
                                Parent = "Screen",
                                EntityControlConfig = new Infosys.ATR.UIAutomation.Entities.ControlConfig()
                                {
                                    ControlName = Task.ControlName,
                                    AutomationId = Task.ControlId,
                                    ControlClass = Task.ControlType.Replace("ControlType.", ""),
                                    ControlPath = Task.ApplictionTreePath
                                },
                                EntityImageConfig = new Infosys.ATR.UIAutomation.Entities.ImageConfig()
                                {
                                    StateImageConfig = new List<Infosys.ATR.UIAutomation.Entities.StateImageConfig>() 
                                { 
                                    new Infosys.ATR.UIAutomation.Entities.StateImageConfig()      
                                    {   
                                        State="",
                                        CenterImageName= new Infosys.ATR.UIAutomation.Entities.CenterImageSearchConfig(){ ImageName=string.Format("{0}.jpg",Task.Id)}
                                    }
                                }
                                }
                            };

                            Infosys.ATR.UIAutomation.Entities.ScreenConfig screen = lstScreenConfig.FindAll(x => x.ScreenName.ToUpper().Equals(_ScreenName.ToUpper().Replace(".", "_"))).FirstOrDefault();
                            if (screen != null)
                            {
                                screen.EntityConfigs.Add(entity);
                            }
                            else
                            {
                                lstScreenConfig.Add(new Infosys.ATR.UIAutomation.Entities.ScreenConfig()
                                {
                                    ScreenName = _ScreenName.Replace(".", "_"),
                                    EntityConfigs = new List<Infosys.ATR.UIAutomation.Entities.EntityConfig>() { entity },
                                    ScreenImageConfig = new Infosys.ATR.UIAutomation.Entities.ImageConfig() { },
                                    ScreenControlConfig = new Infosys.ATR.UIAutomation.Entities.ControlConfig() { ControlName = "" }
                                });
                            }
                        }
                    }
                    #endregion
                    #region Adding Screen
                    //lstScreenConfig.Add(new ScreenConfig()
                    //{
                    //    ScreenName = _ScreenName.Replace(".", "_"),
                    //    EntityConfigs = listEntityconfig,
                    //    ScreenImageConfig = new ImageConfig() { },
                    //    ScreenControlConfig = new ControlConfig() { ControlName = "" }
                    //});
                    #endregion
                    #region Checking for existing application object OR Adding new one item to the list
                    Infosys.ATR.UIAutomation.Entities.AppConfig AppInPool = lstAppConfig.Where(x => x.AppControlConfig.ApplicationLocationPath.StartsWith(_StringToCampare.Replace("https", "http"), StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                    if (AppInPool != null)
                    {
                        AppInPool.ScreenConfigs.AddRange(lstScreenConfig);
                        AppInPool.AppImageConfig = new Infosys.ATR.UIAutomation.Entities.ImageConfig() { };
                    }
                    else
                    {
                        Infosys.ATR.UIAutomation.Entities.AppConfig app = new Infosys.ATR.UIAutomation.Entities.AppConfig()
                        {
                            AppName = _ApplicationName,
                            BaseImageDir = _baseImageDir,
                            AppControlConfig = new Infosys.ATR.UIAutomation.Entities.ControlConfig()
                            {
                                ApplicationType = activity.TargetApplication.ApplicationType.Equals("WebApplication") ? "Web" : "WinDesktop",
                                ApplicationLocationPath = _ApplicationLocationPath,
                                WebBrowser = _WebBrowser,
                                WebBrowserVersion = _WebBrowserVersion
                            },
                            ScreenConfigs = lstScreenConfig,
                            AppImageConfig = new Infosys.ATR.UIAutomation.Entities.ImageConfig() { }
                        };
                        lstAppConfig.Add(app);
                    }
                    #endregion
                }
                autoConfig = new Infosys.ATR.UIAutomation.Entities.AutomationConfig() { AppConfigs = lstAppConfig, ProjectMode = _ProjectMode };
            }
            return autoConfig;
        }
    }

    public class Utilities
    {

        public static string Serialize<T>(T obj)
        {
            using (UTF8Writer stream = new UTF8Writer())
            {
                XmlSerializer serialize = new XmlSerializer(typeof(Infosys.ATR.UIAutomation.Entities.AutomationConfig));
                serialize.Serialize(stream, obj);
                return stream.ToString();
            }
        }

        public static T Deserialize<T>(string s)
        {
            T t = default(T);
            XmlSerializer xml = new XmlSerializer(typeof(Infosys.ATR.UIAutomation.Entities.UseCase));
            t = (T)xml.Deserialize(new StringReader(s));
            return t;
        }

        public static void Write(string path, string s)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(s);
            }
        }
    }

    internal class UTF8Writer : StringWriter
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
