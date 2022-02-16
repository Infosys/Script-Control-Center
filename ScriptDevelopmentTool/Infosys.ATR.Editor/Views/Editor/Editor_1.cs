using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.UIAutomation.Entities;
using IMSWorkBench.Infrastructure.Interface.Services;
using System.Text.RegularExpressions;
using Infosys.ATR.Editor.Services;

namespace Infosys.ATR.Editor.Views
{
    partial class Editor : IEditor
    {
        string _baseDir;

        #region -- IEditor Members --

        public void SaveImage(System.Drawing.Image image)
        {
            if (Directory.Exists(_baseDir))
            {
                SaveFileDialog saveImage = new SaveFileDialog();
                saveImage.Filter = "Image files (*.jpg)|*.jpg";
                saveImage.InitialDirectory = _baseDir;
                var saved = saveImage.ShowDialog();

                if (saved == DialogResult.OK)
                {
                    image.Save(saveImage.FileName);
                    image.Dispose();
                    this._presenter.Save(Path.GetFileName(saveImage.FileName));
                    UpdateImageState(Path.GetFileName(saveImage.FileName));
                }
                if (String.IsNullOrEmpty(_baseDir) && !String.IsNullOrEmpty(saveImage.FileName))
                    _baseDir = Path.GetDirectoryName(saveImage.FileName);
            }
            else
            {
                MessageBox.Show(_baseDir + " does not exist. Verify whether base directory is configured correctly", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void BuildTree(TreeNode node)
        {
            panel3.Visible = true;
            this.treeView1.Nodes.Clear();
            this.treeView1.Nodes.Add(node);
            this.treeView1.SelectedNode = node.Nodes[0];
        }

        public void Save(string xml, string editorUcName)
        {
            if (_root.Application.Count > 0)
            {
                SaveFileDialog objectModel = new SaveFileDialog();

                objectModel.Filter = "ATR files (*.atr)|*.atr";
                objectModel.InitialDirectory = _baseDir;
                objectModel.FileName = editorUcName.Replace(Constants.Application.ObjectModel, String.Empty);
                var saved = objectModel.ShowDialog();

                if (saved == DialogResult.OK)
                {
                    if (_baseDir.Equals(Path.GetDirectoryName(objectModel.FileName),StringComparison.InvariantCultureIgnoreCase))
                    {
                        var t = _baseDir.Replace("\\",@"\");

                        var autoConfig =
                            Utilities.Deserialize<AutomationConfig>(xml);

                        autoConfig.AppConfigs.ForEach(a => 
                            {
                                if (a.BaseImageDir == t)
                                {
                                    a.BaseImageDir = "$";
                                }
                            });



                        xml = Utilities.Serialize<AutomationConfig>(autoConfig);  

                    }
                    this._presenter.Write(objectModel.FileName, xml);
                    Utilities.SavedTabs[editorUcName] = true;
                    var key = Utilities.GetKey();                    
                    if (key != null)
                        Utilities.Editors[key] = true;
                    this.ObjectModel = Path.GetFileNameWithoutExtension(objectModel.FileName);
                    this._presenter.OMSaveHandler(this.ObjectModel);
                    //    this._presenter.Write(Path.Combine(_baseDir,_objectModelfileName), xml);
                    //     this._baseDir = Path.GetDirectoryName(objectModel.FileName);
                    //GenerateScript(_root);
                }
            }
            else
                throw new GenericException("There are no application(s) to save", true, false);

        }

        public void UpdateName(string name)
        {
            UpdateTreeNodeText(name);

            var ctrl = this.treeView1.SelectedNode.Tag as Entities.BaseProperties;
            ctrl.Name = name;


        }

        public void UpdateAppProperties(string[] p)
        {
            UpdateTreeNodeText(p[0]);
            var ctrl = this.treeView1.SelectedNode.Tag as Entities.ApplicationProperties;
            if (ctrl != null)
            {
                ctrl.Name = p[0];
                ctrl.ApplicationType = p[1];
                ctrl.ApplicationLocationPath = p[2];
                ctrl.WebBrowser = p[3];
                ctrl.WebBrowserVersion = p[4];
            }
        }

        public void UpdateControlProperties(string p1)
        {
            if (Mode == Entity.ProjectMode.Win32)
            {
                if (this.treeView1.SelectedNode != null)
                {
                    var ctrl = this.treeView1.SelectedNode.Tag as Entities.ControlProperties;
                    if (ctrl != null)
                    {
                        string[] p = Regex.Split(p1, "\r\n");

                        ctrl.Name = p[0];
                        ctrl.AutomationId = p[1];
                        string ctrlType = p[2].Replace("ControlType.", String.Empty);
                        ctrl.Type = ctrlType;
                        ctrl.ControlPath = p[4];
                        ctrl.ControlName = p[0];
                    }
                    this.propertyGrid1.Refresh();
                }
            }
        }

        public Entity.Area Area { get; set; }
        public String State { get; set; }
        public String BaseDir { get { return _baseDir; } set { _baseDir = value; } }
        public String ObjectModel { get { return _objectModelfileName; } set { _objectModelfileName = value; } }
        public Entity.Root _root { get; set; }
        public Entities.ProjectMode Mode { get { return _mode; } set { _mode = value; } }
        #endregion



    }
}
