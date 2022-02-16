/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Automation;

using Infosys.ATR.Editor.Services;

namespace Infosys.ATR.Editor.Entities
{

    public class Root
    {
        [Browsable(false)]
        public List<Application> Application { get; set; }

        [Browsable(false)]
        public ProjectMode Mode { get; set; }

        [Description("Set base directory for saving Xmls, images")]
        [System.ComponentModel.Editor(typeof(System.Windows.Forms.Design.FolderNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string BaseDirectory { get; set; }

        public string ObjectModelName { get; set; }
    }

    public class ApplicationBase
    {
        public ApplicationProperties Properties { get; set; }
    }


    public class Application : ApplicationBase
    {
        public List<Screen> Screens { get; set; }
    }

    public class Screen
    {
        public BaseProperties Properties { get; set; }
        public List<Control> Controls { get; set; }
    }

    public class Control
    {
        public ControlProperties Properties { get; set; }
        public List<Control> InnerControls { get; set; }
    }

    public class BaseProperties
    {
        bool save;

        public void SetCurrentTab(object newValue, object oldValue)
        {
            if (oldValue != null)
            {
                var key = Utilities.GetKey();
                if (newValue == oldValue && !save)
                {
                    Utilities.SavedTabs[Utilities.CurrentTab] = true;                    
                    if(key != null)
                        Utilities.Editors[key] = true;

                }
                else
                {
                    Utilities.SavedTabs[Utilities.CurrentTab] = false;                    
                    if (key != null)
                        Utilities.Editors[key] = false;
                    save = true;
                }
            }
        }

      

        string _name;

        [Category("General")]
        [Description("Name of the control")]
        [Required]
        public string Name
        {
            get { return _name; }
            set
            {
                SetCurrentTab(value, _name);
                _name = value.Trim();
            }
        }

        private string _ctrlName = "";

        [Category("Automation")]
        [Description("Win32 Name of the control")]
        // [Required]
        public string ControlName
        {
            get { return _ctrlName; }
            set
            {
                SetCurrentTab(value, _ctrlName);
                _ctrlName = value;
            }
        }

        string _automationId;

        [Category("Automation")]
        [Description("Unique id to identify the control")]
        public string AutomationId
        {
            get { return _automationId; }
            set
            {
                SetCurrentTab(value, _automationId);
                _automationId = value;
            }
        }
        string _type;

        [Category("Automation")]
        [Description("Select the type of the control")]
        [TypeConverter(typeof(StringListConvertor))]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                SetCurrentTab(value, _type);
                _type = value;
            }
        }


        [Category("Image")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Browsable(false)]
        public List<State> State { get; set; }

    }

    public class ApplicationProperties : BaseProperties
    {
        string _appLocalPath;
        string _appType;

        [Category("Automation")]
        [Description("Set application executable path")]
        [System.ComponentModel.Editor(typeof(System.Windows.Forms.Design.FileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        [Required]
        public string ApplicationLocationPath
        {
            get { return _appLocalPath; }
            set
            {
                SetCurrentTab(value, _appLocalPath);
                _appLocalPath = value;
            }
        }


        [Category("Automation")]
        [Description("Select the type of the a")]
        [TypeConverter(typeof(ApplicationPathConvertor))]
        [Required]
        public string ApplicationType
        {
            get { return _appType; }
            set
            {
                SetCurrentTab(value, _appType);
                _appType = value;
            }
        }

        string _UIFrmwk;

        [Category("Automation")]
        [Description("Set framework and version e.g net4.0")]
        public string UIFramework
        {
            get
            {
                return _UIFrmwk;
            }
            set
            {
                SetCurrentTab(value, _UIFrmwk);
                _UIFrmwk = value;
            }
        }

        string _webBrowser;

        [Category("Automation")]
        [Description("Set browser used to capture the controls")]
        [TypeConverter(typeof(WebBrowserConvertor))]
        public string WebBrowser
        {
            get
            {
                return _webBrowser;
            }
            set
            {
                SetCurrentTab(value, _webBrowser);
                _webBrowser = value;
            }
        }

        string _webBrowserVersion;

        [Category("Automation")]
        [Description("Set browser version")]
        public string WebBrowserVersion
        {
            get
            {
                return _webBrowserVersion;
            }
            set
            {
                SetCurrentTab(value, _webBrowserVersion);
                _webBrowserVersion = value;
            }
        }

    }


    public class ControlProperties : BaseProperties
    {
        private string _ctrlPath = "";
        [Category("Control")]
        [Description("Set the hierarchial path of the control")]
        // [Required]
        public string ControlPath
        {
            get { return _ctrlPath; }
            set
            {
                SetCurrentTab(value, _ctrlPath);
                _ctrlPath = value;
            }
        }


        [Browsable(false)]
        public List<ControlProperties> Children { get; set; }

        string _parent;

        [Browsable(false)]
        public string Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                SetCurrentTab(value, _parent);
                _parent = value;
            }
        }

        public ControlProperties()
        {

        }

        public ControlProperties(string name)
        {
            this.Name = name;
        }
    }

    public class State : BaseProperties
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { SetCurrentTab(value, _name); _name = value; }
        }
        public ImageArea Area { get; set; }
    }

    public class ImageProperties : BaseProperties
    {
        string _path = "";

        [Description("Physical path of the image")]
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                //if (_path != null)
                //{
                //    Utilities.SavedTabs[Utilities.CurrentTab] = false;
                //}
                //else if (_path != null && _path.ToString() != String.Empty)
                //{
                //    Utilities.SavedTabs[Utilities.CurrentTab] = false;
                //}
                //else
                //{
                //    Utilities.SavedTabs[Utilities.CurrentTab] = true;
                //}
                SetCurrentTab(value, _path);
                _path = value;
            }
        }

        public ImageProperties()
        { }

        public override string ToString()
        {
            return String.Empty;
        }
    }

    public class ImageArea
    {

        [Category("Image")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Actual image of the control")]
        public ImageProperties Center { get; set; }

        [Category("Image")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Image nearest to the right of the actual image")]
        public ImageProperties Right { get; set; }

        [Category("Image")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Image nearest to the left of the actual image")]
        public ImageProperties Left { get; set; }

        [Category("Image")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Image nearest to the top of the actual image")]
        public ImageProperties Above { get; set; }

        [Category("Image")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Image nearest to the bottom of the actual image")]
        public ImageProperties Below { get; set; }

        [Category("Image")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Description("Image used to validate the actual image")]
        public ImageProperties Validate { get; set; }

        public ImageArea()
        {
            Center = new ImageProperties();
            Above = new ImageProperties();
            Right = new ImageProperties();
            Below = new ImageProperties();
            Left = new ImageProperties();
            Validate = new ImageProperties();
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }

    public class StringListConvertor : TypeConverter
    {
        private static string[] controlTypes;

        public StringListConvertor()
        {
            GetControlValues();
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(controlTypes);

        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private void GetControlValues()
        {
            controlTypes = typeof(System.Windows.Automation.ControlType).GetFields().
                Where(p => p.FieldType.FullName == "System.Windows.Automation.ControlType").Select(s => s.Name).ToArray();
        }
    }

    public class ApplicationPathConvertor : TypeConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(Enum.GetNames(typeof(ApplicationTypes)));
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class WebBrowserConvertor : TypeConverter
    {

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "Internet Explorer", "Firefox", "Chrome" });

        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

    }

    public enum Area
    {
        Center,
        Right,
        Left,
        Above,
        Below,
        Validate
    }

    internal enum ControlType
    {
        Application,
        Screen,
        Entity
    }

    public enum ApplicationTypes
    {
        WinDesktop,
        Java,
        Web
    }

    public enum ProjectMode
    {
        ImageCapture,
        Win32
    }

}
