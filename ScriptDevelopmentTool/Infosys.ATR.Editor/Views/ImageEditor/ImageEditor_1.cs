using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Collections.Specialized;

using Infosys.ATR.Editor.Entities;

namespace Infosys.ATR.Editor.Views.ImageEditor
{
    public partial class ImageEditor : IImageEditor
    {
        #region -- IImageEditor Members --

        public string ImagePath { get; set; }
        public string BaseDir { get; set; }
        public BindingList<String> States { get { return _states; } set { _states = value; } }
        public String Name { get { return _name; } set {_name = value ;} }
        public void ShowImage()
        {
            ShowImage(ImagePath);
            UpdatePath();
        }


        public void ClearAll()
        {
            ClearText();
            if (_states.Count > 0) _states.Clear();
        }

        public void UpdateImageProperties(object[] param)
        {
            _imagestates.Clear();
            _states.Clear();

            var states = param[1] as List<State>;

            string[] area = Enum.GetNames(typeof(Area));

            states.ForEach(s =>
            {

                _states.Add(s.Name);

                var properties = s.Area.GetType().GetProperties().Where(p => area.Contains(p.Name)).ToList();
                Dictionary<Area, string> ips = new Dictionary<Area, string>();

                properties.ForEach(p =>
                {
                    var ip = p.GetValue(s.Area, null) as ImageProperties;
                    ips.Add(((Area)Enum.Parse(typeof(Area), p.Name)), Path.GetFileName(ip.Path));
                });
                 _imagestates.Add(s.Name, ips);

            });

            cmbState.Refresh();
            ClearText();
            if (_states != null && _states.Count > 0)
                UpdateImage(_states[0]);
            var t = (param[0] as string).Split('-');
            if (t.Length == 2)
                txtstateName.Text = _name = t[1];
            else
                txtstateName.Text = _name = "";
        }

        public void UpdateApplicationProperties(ApplicationProperties a)
        {
            txtAppName.Text = a.Name;
            txtAppPath.Text = a.ApplicationLocationPath;
            cmbAppType.Text = a.ApplicationType;
            cmbBrowser.Text = a.WebBrowser;
            txtVersion.Text = a.WebBrowserVersion;

        }

        #endregion


       
    }
}
