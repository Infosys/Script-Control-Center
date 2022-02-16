/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.UIAutomation.Entities;

namespace Infosys.ATR.Editor.Services
{
    internal partial class Translate
    {
        static string _baseImageDir;

        internal static AutomationConfig ToAutomationConfig(Entity.Root _root)
        {
            AutomationConfig autoConfig = new AutomationConfig();
            var baseImageDir = _root.BaseDirectory;
            autoConfig.AppConfigs = new List<AppConfig>();
            autoConfig.ProjectMode = _root.Mode.ToString();

            foreach (Entity.Application a in _root.Application)
            {
                AppConfig appConfig = new AppConfig();
                appConfig.AppName = a.Properties.Name;
                appConfig.BaseImageDir = baseImageDir;                

                appConfig.AppControlConfig = new ControlConfig();
                appConfig.AppControlConfig.AutomationId = a.Properties.AutomationId;
                appConfig.AppControlConfig.ControlClass = a.Properties.Type;
                appConfig.AppControlConfig.ControlName = a.Properties.ControlName;
                appConfig.AppControlConfig.ApplicationLocationPath = a.Properties.ApplicationLocationPath;
                appConfig.AppControlConfig.ApplicationType = a.Properties.ApplicationType.ToString();
                appConfig.AppControlConfig.UIFwk = a.Properties.UIFramework;
                appConfig.AppControlConfig.WebBrowser = a.Properties.WebBrowser;
                appConfig.AppControlConfig.WebBrowserVersion = a.Properties.WebBrowserVersion;

                appConfig.AppImageConfig = new ImageConfig();
                appConfig.AppImageConfig.StateImageConfig = new List<StateImageConfig>();
                appConfig.AppImageConfig.StateImageConfig.AddRange(GetStateImageConfig(a.Properties.State));

                appConfig.ScreenConfigs = new List<ScreenConfig>();
                a.Screens.ForEach(s => appConfig.ScreenConfigs.Add(GetScreenConfig(s)));

                autoConfig.AppConfigs.Add(appConfig);
            }

            return autoConfig;
        }

        private static List<StateImageConfig> GetStateImageConfig(List<Entity.State> states)
        {
            List<StateImageConfig> stateImages = new List<StateImageConfig>();
            string[] property = { "Center", "Left", "Right", "Above", "Below", "Validate" };

            states.ForEach(s =>
            {
                var area = s.Area;
                var properties = s.Area.GetType().GetProperties().Where(p => property.Contains(p.Name)).ToList();
                StateImageConfig stateImage = new StateImageConfig();
                stateImage.State = s.Name;
                properties.ForEach(prop =>
                {
                    var imageProp = prop.GetValue(area, null) as Entity.ImageProperties;

                    if (!string.IsNullOrEmpty(imageProp.Path))
                    {
                        var path = Path.GetFileName(imageProp.Path);
                        switch (prop.Name)
                        {
                            case "Center":
                                stateImage.CenterImageName = new CenterImageSearchConfig();
                                stateImage.CenterImageName.ImageName = path;
                                break;

                            case "Right":
                                stateImage.RightImageName = new RightImageSearchConfig();
                                stateImage.RightImageName.ImageName = path;
                                break;

                            case "Left":
                                stateImage.RightImageName = new RightImageSearchConfig();
                                stateImage.RightImageName.ImageName = path;
                                break;

                            case "Above":
                                stateImage.AboveImageName = new AboveImageSearchConfig();
                                stateImage.AboveImageName.ImageName = path;
                                break;

                            case "Below":
                                stateImage.BelowImageName = new BelowImageSearchConfig();
                                stateImage.BelowImageName.ImageName = path;
                                break;

                            case "Validate":
                                stateImage.ValidationImageName = new ValidationImageSearchConfig();
                                stateImage.ValidationImageName.ImageName = path;
                                break;
                        }

                    }
                });
                stateImages.Add(stateImage);
            });

            return stateImages;
        }

        private static ScreenConfig GetScreenConfig(Entity.Screen screen)
        {
            ScreenConfig screenConfig = new ScreenConfig();
            screenConfig.ScreenName = screen.Properties.Name;

            screenConfig.ScreenControlConfig = new ControlConfig();
            screenConfig.ScreenControlConfig.AutomationId = screen.Properties.AutomationId;
            screenConfig.ScreenControlConfig.ControlClass = screen.Properties.Type;
            screenConfig.ScreenControlConfig.ControlName = screen.Properties.ControlName;

            screenConfig.ScreenImageConfig = new ImageConfig();
            screenConfig.ScreenImageConfig.StateImageConfig = new List<StateImageConfig>();
            screenConfig.ScreenImageConfig.StateImageConfig.AddRange(GetStateImageConfig(screen.Properties.State));

            screenConfig.EntityConfigs = new List<EntityConfig>();

            GetChildEntities(screen.Controls, screenConfig);

            return screenConfig;
        }

        private static void GetChildEntities(List<Entity.Control> ctrls, ScreenConfig s)
        {
            ctrls.ForEach(c =>
            {

                EntityConfig e = new EntityConfig();

                e.EntityName = c.Properties.Name;
                e.Parent = c.Properties.Parent;

                e.EntityControlConfig = new ControlConfig();
                e.EntityControlConfig.AutomationId = c.Properties.AutomationId;
                e.EntityControlConfig.ControlClass = c.Properties.Type;
                e.EntityControlConfig.ControlName = c.Properties.ControlName;
                e.EntityControlConfig.ControlPath = c.Properties.ControlPath; ;

                e.EntityImageConfig = new ImageConfig();
                e.EntityImageConfig.StateImageConfig = new List<StateImageConfig>();
                e.EntityImageConfig.StateImageConfig.AddRange(GetStateImageConfig(c.Properties.State));

                e.EntityChildConfig = new List<EntityConfig>();
                if (c.Properties.Parent == "Screen")
                {
                    s.EntityConfigs.Add(e);
                }
                else
                {
                    var parent = GetParent(s.EntityConfigs, c.Properties.Parent);
                    parent.EntityChildConfig.Add(e);
                }

                GetChildEntities(c.InnerControls, s);
            });
        }

        private static EntityConfig GetParent(List<EntityConfig> entities, string parent)
        {

            foreach (EntityConfig e in entities)
            {
                //if (e.EntityControlConfig.ControlName == parent)
                if (e.EntityName == parent)
                    return e;
                else
                {
                    var ch = GetParent(e.EntityChildConfig, parent);
                    if (ch != null)
                        return ch;
                }
            }

            return null;
        }



        internal static TreeNode ToTreeView(AutomationConfig autoConfig)
        {
            TreeNode _root = new TreeNode();
            _root.Text = "Desktop";
            _root.ImageKey = "desktop";            
            Entities.Root desktop = new Entity.Root();
            desktop.BaseDirectory =_baseImageDir = autoConfig.AppConfigs[0].BaseImageDir;
            desktop.Mode = (Entity.ProjectMode)Enum.Parse(typeof(Entity.ProjectMode), autoConfig.ProjectMode);
            _root.Tag = desktop;
            autoConfig.AppConfigs.ForEach(
                a =>
                {
                    TreeNode appNode = new TreeNode();
                    appNode.Text = "Application - " + a.AppName;
                    appNode.ImageKey = appNode.SelectedImageKey = "application";
                    var ctrl = GetApplicationControlProperties(a.AppControlConfig, a.AppName);
                    ctrl.State = GetImageProperties(a.AppImageConfig);
                    GetScreenConfig(a.ScreenConfigs, appNode);
                    appNode.Tag = ctrl;
                    _root.Nodes.Add(appNode);
                }
                );
            return _root;
        }

        private static void GetScreenConfig(List<ScreenConfig> screens, TreeNode app)
        {
            screens.ForEach(s =>
            {
                TreeNode screen = new TreeNode();
                screen.Text = "Screen - " + s.ScreenName;
                screen.ImageKey = screen.SelectedImageKey = "screen";
                var ctrl = GetScreenProperties(s.ScreenControlConfig, s.ScreenName);
                ctrl.State = GetImageProperties(s.ScreenImageConfig);
                screen.Tag = ctrl;
                GetEntityConfigRecursively(s.EntityConfigs, screen);
                app.Nodes.Add(screen);
            });
        }
       

        private static void GetEntityConfigRecursively(List<EntityConfig> entities, TreeNode screen)
        {

            entities.ForEach(
                e =>
                {
                    TreeNode entity = new TreeNode();
                    entity.Text = "Control - " + e.EntityName;
                    entity.ImageKey = entity.SelectedImageKey = "control";
                    var ctrl = GetControlProperties(e.EntityControlConfig, e.EntityName);
                    ctrl.State = GetImageProperties(e.EntityImageConfig);
                    entity.Tag = ctrl;

                    if (e.Parent == "Screen")
                    {
                        screen.Nodes.Add(entity);
                    }
                    else
                    {
                        var parent = GetParentNode(screen.Nodes, e.Parent);
                        parent.Nodes.Add(entity);
                    }

                    GetEntityConfigRecursively(e.EntityChildConfig, screen);

                });

        }

        private static TreeNode GetParentNode(TreeNodeCollection nodes,string parent)
        {
            foreach (TreeNode n in nodes)
            {
                var ctrl = n.Tag as Entity.ControlProperties;
               // if (ctrl.ControlName == parent)
                if (ctrl.Name == parent)
                    return n;
                else
                {
                    var node = GetParentNode(n.Nodes,parent);
                    if (node != null)
                        return node;
                }
            }

            return null;
        }

        private static Entities.BaseProperties GetScreenProperties(ControlConfig a, string name)
        {
            Entity.BaseProperties s = new Entity.BaseProperties();
            s.AutomationId = a.AutomationId;
            s.Type = a.ControlClass;
            s.ControlName = a.ControlName;            
            s.Name = name;
            return s;
        }

        private static Entity.ApplicationProperties GetApplicationControlProperties(ControlConfig a, string name)
        {
            Entity.ApplicationProperties ap = new Entity.ApplicationProperties();
            ap.ApplicationLocationPath = a.ApplicationLocationPath;
            ap.ApplicationType = a.ApplicationType;
            ap.AutomationId = a.AutomationId;
            ap.Type = a.ControlClass;
            ap.ControlName = a.ControlName;            
            ap.UIFramework = a.UIFwk;
            ap.Name = name;
            ap.WebBrowser = a.WebBrowser;
            ap.WebBrowserVersion = a.WebBrowserVersion;
            return ap;
        }

        private static Entity.ControlProperties GetControlProperties(ControlConfig a, string name)
        {
            Entity.ControlProperties cp = new Entity.ControlProperties();            
            cp.AutomationId = a.AutomationId;
            cp.Type = a.ControlClass;
            cp.ControlName = a.ControlName;
            cp.Name = name;
            cp.ControlPath = a.ControlPath;            
            return cp;
        }

        private static List<Entity.State> GetImageProperties(ImageConfig i)
        {
            List<Entity.State> states = new List<Entity.State>();
            string[] boundaries = { "CenterImageName", "AboveImageName", "BelowImageName", "RightImageName", "LeftImageName", "ValidationImageName" };

            i.StateImageConfig.ForEach(state =>
            {
                Entity.State s = new Entity.State();
                s.Name = state.State;
                s.Area = new Entity.ImageArea();

                var properties = state.GetType().GetProperties().Where(p => boundaries.Contains(p.Name)).ToList();

                properties.ForEach(prop =>
                {
                    switch (prop.Name)
                    {
                        case "ValidationImageName":
                            if (state.ValidationImageName != null)
                                s.Area.Validate.Path = Path.Combine(_baseImageDir, state.ValidationImageName.ImageName);
                            break;
                        case "CenterImageName":
                            if (state.CenterImageName != null)
                                s.Area.Center.Path = Path.Combine(_baseImageDir, state.CenterImageName.ImageName);
                            break;
                        case "RightImageName":
                            if (state.RightImageName != null)
                                s.Area.Right.Path = Path.Combine(_baseImageDir, state.RightImageName.ImageName);
                            break;
                        case "LeftImageName":
                            if (state.LeftImageName != null)
                                s.Area.Left.Path = Path.Combine(_baseImageDir, state.LeftImageName.ImageName);
                            break;
                        case "AboveImageName":
                            if (state.AboveImageName != null)
                                s.Area.Above.Path = Path.Combine(_baseImageDir, state.AboveImageName.ImageName);
                            break;
                        case "BelowImageName":
                            if (state.BelowImageName != null)
                                s.Area.Below.Path = Path.Combine(_baseImageDir, state.BelowImageName.ImageName);
                            break;
                    }
                });
                states.Add(s);

            });

            return states;
        }
    }
}
