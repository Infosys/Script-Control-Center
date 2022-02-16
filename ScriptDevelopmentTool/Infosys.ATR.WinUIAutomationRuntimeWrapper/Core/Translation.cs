/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.UIAutomation.Entities;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Automation;

namespace Infosys.ATR.WinUIAutomationRuntimeWrapper.Core
{
    public class Translation
    {
        static string baseImageDir = "";

        public static Dictionary<string, Application> PopulateApplications(string xmlString, bool LaunchApps = false, bool showAppStartWaitBox = true, string firstAppToStart = "")
        {
            Core.Utilities.WriteLog("populating applications");
            Dictionary<string, Application> applications = new Dictionary<string, Application>();
            //showWaitBox = showAppStartWaitBox;
            AutomationConfig autoConfig = Deserialize(xmlString, typeof(AutomationConfig)) as AutomationConfig;
            Application app = null;
            if (autoConfig != null)
            {
                //put a check if the firstAppToStart has a valid value or not
                if (!string.IsNullOrEmpty(firstAppToStart))
                {
                    if (!autoConfig.AppConfigs.Any(a => a.AppName == firstAppToStart))
                        throw new Exception(string.Format("Application- {0} provided is invalid", firstAppToStart));
                }

                bool FocusSetToFirstApp = false;
                foreach (AppConfig appconfig in autoConfig.AppConfigs)
                {
                    if (LaunchApps || (!string.IsNullOrEmpty(firstAppToStart) && firstAppToStart == appconfig.AppName))
                    {
                        baseImageDir = appconfig.BaseImageDir;
                        string appType = appconfig.AppControlConfig.ApplicationType;
                        int processId = Core.Utilities.LaunchApplication(appconfig.AppControlConfig.ApplicationLocationPath, appType, appconfig.AppControlConfig.WebBrowser, showAppStartWaitBox);
                        try
                        {
                            app = new Application(processId);
                        }
                        catch (Exception ex)
                        {
                            Core.Utilities.WriteLog("instantiating application with control name");
                            app = new Application(appconfig.AppControlConfig.ControlName, true, appconfig.AppControlConfig.ControlClass);
                        }

                        if (!FocusSetToFirstApp && app.WindowsHandle != IntPtr.Zero)
                        {

                            AutomationElement parent = AutomationElement.FromHandle(app.WindowsHandle);
                            if (parent != null && parent.Current.IsKeyboardFocusable)
                                parent.SetFocus();
                        }
                    }                    
                    else
                        app = new Application(appconfig.AppControlConfig.ControlName, false, appconfig.AppControlConfig.ControlClass);

                    if (app != null)
                    {
                        //populate application
                        baseImageDir = appconfig.BaseImageDir;
                        app.Name = appconfig.AppName;
                        app.AppType = appconfig.AppControlConfig.ApplicationType;
                        app.AppLocationPath = appconfig.AppControlConfig.ApplicationLocationPath;
                        app.UIFwk = appconfig.AppControlConfig.UIFwk;
                        //populate application screens and child controls (if any for screen with no name)
                        ScreensAndControlsUnderScreen data = PopulateApplicationScreens(appconfig, app.WindowsHandle, appconfig.AppControlConfig.ApplicationType, app.Name);
                        app.Screens = data.Screens;
                        app.Controls = data.ControlsUndderScreens;

                        app.WebBrowser = appconfig.AppControlConfig.WebBrowser;
                        app.WebBrowserVersion = appconfig.AppControlConfig.WebBrowserVersion;
                        app.ShowAppStartWaitBox = showAppStartWaitBox;
                    }
                    else
                        Core.Utilities.WriteLog(" failed to instantiate application as it is null");

                    applications.Add(appconfig.AppName, app); //check if the appname is right to be used as key
                }
            }
            Core.Utilities.WriteLog("applications populated");
            return applications;
        }

        //private static Dictionary<string, Screen> PopulateApplicationScreens(AppConfig appConfig, IntPtr appWindowHandle)
        private static ScreensAndControlsUnderScreen PopulateApplicationScreens(AppConfig appConfig, IntPtr appWindowHandle, string applicationType, string appName)
        {
            Core.Utilities.WriteLog("populating screens");
            Dictionary<string, Screen> screens = new Dictionary<string, Screen>();
            Dictionary<string, Control> controls = new Dictionary<string, Control>(); //to hold the controls under the control with no name
            Screen screen = null;
            foreach (ScreenConfig screenConfig in appConfig.ScreenConfigs)
            {
                string appScreenQualifier = "";
                if (!string.IsNullOrEmpty(screenConfig.ScreenName))
                {
                    appScreenQualifier = appName + "." + screenConfig.ScreenName;
                    screen = new Screen(screenConfig.ScreenControlConfig.ControlName, appWindowHandle, screenConfig.ScreenControlConfig.ControlClass);
                    screen.Name = screenConfig.ScreenName;
                    //populate controls in the screen
                    screen.Controls = PopulateControls(screenConfig, appWindowHandle, screen.WindowsHandle, applicationType, appScreenQualifier);
                    //add the screen to screens
                    screens.Add(screenConfig.ScreenName, screen);//check if the screen name is right to be used as key
                }
                else
                {
                    appScreenQualifier = appName;
                    //add the controls under the scrren with missing name to the 'controls' to be returned to the application object
                    controls.AddRange(PopulateControls(screenConfig, appWindowHandle, IntPtr.Zero, applicationType, appScreenQualifier));
                }
            }
            Core.Utilities.WriteLog("populated screens");
            return new ScreensAndControlsUnderScreen() { Screens = screens, ControlsUndderScreens = controls };
        }

        private static Dictionary<string, Control> PopulateControls(object objConfig, IntPtr appWinHandle, IntPtr screenHandle, string applicationType, string ControlQualifier)
        {
            Core.Utilities.WriteLog("populating controls");
            Dictionary<string, Control> controls = new Dictionary<string, Control>();
            //Dictionary<string, Control> controlsUnderControl = new Dictionary<string, Control>(); //to hold the controls under the control with no name
            Control ctl = null;
            string fullControlQualifier = "";
            if (objConfig.GetType().Equals(typeof(ScreenConfig)))
            {
                ScreenConfig screenConfig = objConfig as ScreenConfig;
                Core.Utilities.WriteLog("controls populating under screen-" + screenConfig.ScreenName);
                foreach (EntityConfig entityConfig in screenConfig.EntityConfigs)
                {

                    if (!string.IsNullOrEmpty(entityConfig.EntityName))
                    {
                        Core.Utilities.WriteLog("control being added- " + entityConfig.EntityName);
                        fullControlQualifier = ControlQualifier + "." + entityConfig.EntityName;
                        //ctl = new Control(appWinHandle, screenHandle);
                        ctl = GetTypedControl(entityConfig.EntityControlConfig.ControlClass, appWinHandle, screenHandle, entityConfig.EntityControlConfig.AutomationId, entityConfig.EntityControlConfig.ControlName,
                            entityConfig.EntityControlConfig.ControlPath, applicationType, fullControlQualifier);
                        if (ctl == null)
                            continue;
                        //populate control-ctl
                        if (entityConfig.EntityControlConfig.ControlClass == null || entityConfig.EntityControlConfig.ControlClass.ToLower() == "na" || entityConfig.EntityControlConfig.ControlClass.ToLower() == "none")
                            ctl.DiscoveryMode = ElementDiscovertyMode.Image;
                        else
                        ctl.DiscoveryMode = GetDiscoveryMode(entityConfig);
                        Core.Utilities.WriteLog("got discovery mode");
                        ctl.ImageReference = PopulateEntityImage(entityConfig.EntityImageConfig);
                        //ctl.AutomationName = entityConfig.EntityControlConfig.ControlName;
                        //ctl.AutomationId = entityConfig.EntityControlConfig.AutomationId;
                        ctl.Name = entityConfig.EntityName;
                        //ctl.ControlPath = entityConfig.EntityControlConfig.ControlPath;
                        //ctl.ControlTypeName = entityConfig.EntityControlConfig.ControlClass; //confirm this
                        //ctl.ApplicationType = entityConfig.EntityControlConfig.ApplicationType;
                        if (entityConfig.EntityChildConfig != null && entityConfig.EntityChildConfig.Count > 0)
                        {
                            //populate child controls
                            ctl.Controls = PopulateControls(entityConfig, appWinHandle, screenHandle, applicationType, fullControlQualifier);
                        }

                        //add the ctl to controls
                        controls.Add(ctl.Name, ctl);//check if the control name is right to be used as key
                        Core.Utilities.WriteLog("control added- " + ctl.Name);
                    }
                    else
                    {
                        controls.AddRange(PopulateControls(entityConfig, appWinHandle, screenHandle, applicationType, fullControlQualifier));
                    }
                }
            }
            else if (objConfig.GetType().Equals(typeof(EntityConfig)))
            {
                EntityConfig entityConfig = objConfig as EntityConfig;
                Core.Utilities.WriteLog("controls populating under control-" + entityConfig.EntityName);
                foreach (EntityConfig entitychildConfig in entityConfig.EntityChildConfig)
                {
                    if (!string.IsNullOrEmpty(entitychildConfig.EntityName))
                    {
                        fullControlQualifier = ControlQualifier + "." + entitychildConfig.EntityName;
                        //ctl = new Control(appWinHandle, screenHandle);
                        ctl = GetTypedControl(entitychildConfig.EntityControlConfig.ControlClass, appWinHandle, screenHandle, entitychildConfig.EntityControlConfig.AutomationId, entitychildConfig.EntityControlConfig.ControlName,
                            entitychildConfig.EntityControlConfig.ControlPath, applicationType, fullControlQualifier);
                        if (ctl == null)
                            continue;
                        //populate control-ctl
                        if (entityConfig.EntityControlConfig.ControlClass == null || entityConfig.EntityControlConfig.ControlClass.ToLower() == "na" || entityConfig.EntityControlConfig.ControlClass.ToLower() == "none")
                            ctl.DiscoveryMode = ElementDiscovertyMode.Image;
                        else
                        ctl.DiscoveryMode = GetDiscoveryMode(entitychildConfig);
                        ctl.ImageReference = PopulateEntityImage(entitychildConfig.EntityImageConfig);
                        //ctl.AutomationName = entitychildConfig.EntityControlConfig.ControlName;
                        //ctl.AutomationId = entitychildConfig.EntityControlConfig.AutomationId;
                        ctl.Name = entitychildConfig.EntityName;
                        //ctl.ControlPath = entitychildConfig.EntityControlConfig.ControlPath;
                        //ctl.ControlTypeName = entityConfig.EntityControlConfig.ControlClass; //confirm this
                        //ctl.ApplicationType = entitychildConfig.EntityControlConfig.ApplicationType;
                        if (entitychildConfig.EntityChildConfig != null && entityConfig.EntityChildConfig.Count > 0)
                        {
                            //populate child controls
                            ctl.Controls = PopulateControls(entitychildConfig, appWinHandle, screenHandle, applicationType, fullControlQualifier);
                        }
                        //add the ctl to controls
                        controls.Add(ctl.Name, ctl);//check if the control name is right to be used as key
                    }
                    else
                        controls.AddRange(PopulateControls(entitychildConfig, appWinHandle, screenHandle, applicationType, fullControlQualifier));
                }
            }
            Core.Utilities.WriteLog("controls populated");
            return controls;
        }

        private static ControlImageReference PopulateEntityImage(ImageConfig entityImageConfig)
        {
            //currently there is NO support for multiple images which depicts a single control i.e. given top, bottom, left and right image and construct the ultimate region
            ControlImageReference imageRef = new ControlImageReference();
            if (entityImageConfig != null)
            {
                //map the data from image config to image reference;
                if (entityImageConfig != null && entityImageConfig.StateImageConfig != null && entityImageConfig.StateImageConfig.Count > 0)
                {
                    imageRef.SupportedStates = new List<ControlStateReference>();
                    entityImageConfig.StateImageConfig.ForEach(stateConfig =>
                    {
                        if (stateConfig.CenterImageName != null)
                        {
                            imageRef.SupportedStates.Add(new ControlStateReference() { State = stateConfig.State, ImagePath = baseImageDir + "\\" + stateConfig.CenterImageName.ImageName });
                        }
                        //take which ever image information is evailable
                        else if (stateConfig.AboveImageName != null)
                        {
                            imageRef.SupportedStates.Add(new ControlStateReference() { State = stateConfig.State, ImagePath = baseImageDir + "\\" + stateConfig.AboveImageName.ImageName });
                        }
                        else if (stateConfig.LeftImageName != null)
                        {
                            imageRef.SupportedStates.Add(new ControlStateReference() { State = stateConfig.State, ImagePath = baseImageDir + "\\" + stateConfig.LeftImageName.ImageName });
                        }
                        else if (stateConfig.BelowImageName != null)
                        {
                            imageRef.SupportedStates.Add(new ControlStateReference() { State = stateConfig.State, ImagePath = baseImageDir + "\\" + stateConfig.BelowImageName.ImageName });
                        }
                        else if (stateConfig.RightImageName != null)
                        {
                            imageRef.SupportedStates.Add(new ControlStateReference() { State = stateConfig.State, ImagePath = baseImageDir + "\\" + stateConfig.RightImageName.ImageName });
                        }
                        else if (stateConfig.ValidationImageName != null)
                        {
                            imageRef.SupportedStates.Add(new ControlStateReference() { State = stateConfig.State, ImagePath = baseImageDir + "\\" + stateConfig.ValidationImageName.ImageName });
                        }
                    });
                }
            }
            return imageRef;
        }

        private static object Deserialize(string xmlObj, Type type)
        {
            StringReader stringReader = new StringReader(xmlObj);
            XmlSerializer serializer = new XmlSerializer(type);
            return serializer.Deserialize(stringReader);
        }

        public static string XMLDocAsString(System.Xml.XmlDocument xmlDoc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xmlDoc.WriteTo(tx);
            string strXmlText = sw.ToString();
            return strXmlText;
        }

        private static ElementDiscovertyMode GetDiscoveryMode(EntityConfig entityConfig)
        {
            ElementDiscovertyMode mode = ElementDiscovertyMode.None;
            if (entityConfig.EntityControlConfig != null && entityConfig.EntityImageConfig != null)
            {
                if ((!string.IsNullOrEmpty(entityConfig.EntityControlConfig.ControlPath) ||
                    !string.IsNullOrEmpty(entityConfig.EntityControlConfig.AutomationId) ||
                    !string.IsNullOrEmpty(entityConfig.EntityControlConfig.ControlName) ||
                    !string.IsNullOrEmpty(entityConfig.EntityControlConfig.ControlClass))
                    && (entityConfig.EntityImageConfig.StateImageConfig != null && entityConfig.EntityImageConfig.StateImageConfig.Count > 0))
                {
                    mode = ElementDiscovertyMode.APIAndImage;
                }
            }

            if (entityConfig.EntityControlConfig != null && mode == ElementDiscovertyMode.None)
            {
                if (!string.IsNullOrEmpty(entityConfig.EntityControlConfig.ControlPath) ||
                    !string.IsNullOrEmpty(entityConfig.EntityControlConfig.AutomationId) ||
                    !string.IsNullOrEmpty(entityConfig.EntityControlConfig.ControlName) ||
                    !string.IsNullOrEmpty(entityConfig.EntityControlConfig.ControlClass))
                    mode = ElementDiscovertyMode.API;
            }

            if (entityConfig.EntityImageConfig != null && mode == ElementDiscovertyMode.None)
            {
                if (entityConfig.EntityImageConfig.StateImageConfig != null && entityConfig.EntityImageConfig.StateImageConfig.Count > 0)
                    mode = ElementDiscovertyMode.Image;
            }
            return mode;
        }

        private static Control GetTypedControl(string className, IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            Core.Utilities.WriteLog("getting typed control- " + className);
            Core.Utilities.WriteLog("app handle- " + appWinHandle);
            Core.Utilities.WriteLog("screen handle- " + screenHandle);
            Core.Utilities.WriteLog("automationId- " + automationId);
            Core.Utilities.WriteLog("automationName- " + automationName);
            Core.Utilities.WriteLog("applicationTreePath- " + applicationTreePath);
            Core.Utilities.WriteLog("applicationType- " + applicationType);
            Core.Utilities.WriteLog("fullControlQualifier- " + fullControlQualifier);

            if (automationId == null)
                automationId = "";
            if (automationName == null)
                automationName = "";
            Control ctl = null;
            switch (className.ToLower())
            {
                case "button":
                    ctl = GetButton(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "text":
                case "textbox":
                    ctl = GetTextBox(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "checkbox":
                    ctl = GetCheckBox(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "combobox":
                    ctl = GetComboBox(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "custom":
                    ctl = GetCustom(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "document":
                    ctl = GetDocument(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "edit":
                    ctl = GetEdit(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "hyperlink":
                    ctl = GetHyperLink(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "image":
                    ctl = GetImage(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "list":
                    ctl = GetList(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "radiobutton":
                    ctl = GetRadioButton(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "menu":
                case "menuitem":
                    ctl = GetMenu(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "listitem":
                    ctl = GetListItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "tree":
                    ctl = GetTree(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "treeitem":
                    ctl = GetTreeItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "tab":
                    ctl = GetTab(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "tabitem":
                    ctl = GetTabItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "dataitem":
                    ctl = GetDataItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "datagrid":
                    ctl = GetDataGrid(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "table":
                    ctl = GetTable(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                case "tablecell":
                    ctl = GetTableCell(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
                
                //add cases for other controls as needed
                default:
                    ctl = new Control(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
                    break;
            }

            return ctl;
        }

        private static Controls.TableCell GetTableCell(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.TableCell(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Table GetTable(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Table(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.DataGrid GetDataGrid(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.DataGrid(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.DataItem GetDataItem(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.DataItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.TabItem GetTabItem(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.TabItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Tab GetTab(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Tab(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.TreeItem GetTreeItem(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.TreeItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }
        private static Controls.Tree GetTree(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Tree(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.ListItem GetListItem(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.ListItem(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.List GetList(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.List(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Image GetImage(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Image(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.HyperLink GetHyperLink(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.HyperLink(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Edit GetEdit(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Edit(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Document GetDocument(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Document(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Custom GetCustom(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Custom(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.ComboBox GetComboBox(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.ComboBox(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.CheckBox GetCheckBox(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.CheckBox(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Button GetButton(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Button(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.TextBox GetTextBox(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.TextBox(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.RadioButton GetRadioButton(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.RadioButton(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }

        private static Controls.Menu GetMenu(IntPtr appWinHandle, IntPtr screenHandle, string automationId, string automationName, string applicationTreePath, string applicationType, string fullControlQualifier)
        {
            return new Controls.Menu(appWinHandle, screenHandle, automationId, automationName, applicationTreePath, applicationType, fullControlQualifier);
        }
    }

    static class CollectionAddRangeExtension
    {
        public static void AddRange<T>(this ICollection<T> targetCollection, IEnumerable<T> sourceCollection)
        {
            if (targetCollection == null)
                throw new ArgumentNullException("targetCollection");
            if (sourceCollection == null)
                throw new ArgumentNullException("sourceCollection");
            foreach (var element in sourceCollection)
            {
                try
                {
                    if (!targetCollection.Contains(element))
                        targetCollection.Add(element);

                }
                catch (ArgumentException) //an element with the same key already exists in the Dictionary<TKey, TValue>
                {
                    //assumption- the target collection and source collectio wont be having same keys for collection like dictionary
                    //if so, then ignore duplicate one(s)
                }
            }
        }
    }

    class ScreensAndControlsUnderScreen
    {
        /// <summary>
        /// This collection holds the screens with name
        /// </summary>
        public Dictionary<string, Screen> Screens { get; set; }
        /// <summary>
        /// This collection holds the controls under the screen whose name is missing
        /// </summary>
        public Dictionary<string, Control> ControlsUndderScreens { get; set; }
    }
}
