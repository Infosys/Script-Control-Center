/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Infosys.ATR.ScriptRepository.Views;
using Infosys.ATR.ScriptRepository.Constants;

using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

using IMSWorkBench.Infrastructure.Library.UI;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Diagnostics;
using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.LicenseValidationClient;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.ScriptRepository
{
    internal class Save
    {
        internal static Dictionary<PythonIDE, bool> Tabs;
        internal static Dictionary<ScriptDesigner, bool> ScriptTabs;
        static Save()
        {
            Tabs = new Dictionary<PythonIDE, bool>();
            ScriptTabs = new Dictionary<ScriptDesigner, bool>();
        }
    }

    public enum Mode
    {
        New,
        Edit
    }

    public class ScriptRepoWorkItem : WorkItemController
    {
        Views.PythonIDE pyIde;
        Views.ScriptDesigner sdIde;
        MainRepositoryView _main = null;
        Dictionary<string, Models.Script> _ides = new Dictionary<string, Models.Script>();

        [EventPublication(Constants.EventTopicNames.LoadGeneric, PublicationScope.Global)]
        public event EventHandler LoadThis;

        [EventPublication(EventTopicNames.TabHoverSet, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> TabHoverSet;

        [EventPublication(EventTopicNames.ScriptSave, PublicationScope.Global)]
        public event EventHandler<EventArgs<string>> ScriptSaved;
        [EventPublication(EventTopicNames.DeActivatePublish, PublicationScope.Global)]
        public event EventHandler DeActivatePublish;

        [EventPublication(EventTopicNames.ActivatePublish, PublicationScope.Global)]
        public event EventHandler ActivatePublish;

        [EventPublication(Constants.EventTopicNames.ShowRun, PublicationScope.Global)]
        public event EventHandler<EventArgs<bool>> ShowRun;

        private Models.Script _script;
        private static int _scriptCount;



        public override void Run()
        {

        }


        internal void runScriptBtn_Click(object sender, EventArgs e)
        {
            _main.CheckScriptParamters(_main.selectedScriptIndex);
        }

        internal void deleteScript_Click(object sender, EventArgs e)
        {
            if (_main != null)
            {
                _main.Delete(_main.selectedScriptIndex);
            }
        }

        internal void newScriptBtn_Click(object sender, EventArgs e)
        {
            string t = "Script - " + _scriptCount++.ToString();
            if (System.Configuration.ConfigurationManager.AppSettings["Mode"] == "Online")
            {
                //_main.CreateNewScript();

                // var t = "Script - " + _scriptCount++.ToString();
                var ide = AddScriptView(t, t, true, Mode.New);
                this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Activate(ide);
            }
            else
            {
                //var t = "Script - " + _scriptCount++.ToString();
                //var ide = AddPythonView(t, t, true);
                var ide = AddScriptView(t, t, true, Mode.New);
                this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Activate(ide);
            }

            Logger.Log("Script", "New Script",
               "New Script " + t);
        }



        internal void closeTab_Click(object sender, EventArgs e)
        {
            var current = GetCurrentTab();

            if (current.GetType() == typeof(Views.PythonIDE))
            {
                var py = current as PythonIDE;
                CloseView(py);
            }
            else if (current.GetType() == typeof(Views.ScriptDesigner))
            {
                var sd = current as ScriptDesigner;
                CloseViewScriptDesigner(sd);
            }
            else if (current.GetType() == typeof(Views.MainRepositoryView))
            {
                this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Close(current);
            }
        }

        private bool CloseView(PythonIDE py)
        {
            if (Save.Tabs[py] == false)
            {
                var result = MessageBox.Show("Do you want to save changes to Script - " + py.ucName + " before exiting?", "IAP", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    saveLocally_Click(this, new EventArgs());
                    Close(py);
                }
                else if (result == DialogResult.No)
                {
                    Close(py);
                }
                else
                {
                    return false;
                }
            }
            else
                Close(py);
            return true;
        }

        private void Close(PythonIDE current)
        {
            current.Close();
            this.WorkItem.SmartParts.Remove(current);
            Save.Tabs.Remove(current as PythonIDE);
        }

        private bool CloseViewScriptDesigner(ScriptDesigner sd)
        {
            if (Save.ScriptTabs[sd] == false)
            {
                var result = MessageBox.Show("Do you want to save changes to Script - " + sd.ucName + " before exiting?", "IAP", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    saveLocally_Click(this, new EventArgs());
                    CloseScriptDesigner(sd);
                }
                else if (result == DialogResult.No)
                {
                    CloseScriptDesigner(sd);
                }
                else
                {
                    return false;
                }
            }
            else
                CloseScriptDesigner(sd);
            return true;
        }

        private void CloseScriptDesigner(ScriptDesigner current)
        {
            current.Close();
            this.WorkItem.SmartParts.Remove(current);
            Save.ScriptTabs.Remove(current as ScriptDesigner);
        }

        internal void publish_Click(object sender, EventArgs e)
        {
            var obj = GetCurrentTab();

            if (obj.GetType().Name == "ScriptDesigner")
            {
                var ide = obj as Views.ScriptDesigner;
                if (ide.OpMode == Mode.New)
                    ide.New();
                else
                    ide.Edit();
            }
            else if (obj.GetType() != typeof(Infosys.ATR.ScriptRepository.Views.MainRepositoryView))
            {
                var ide = obj as Views.PythonIDE;
                string content = ide.RichText;
                string title = ide.ucName;
                byte[] bytes;

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ms))
                    {
                        sw.Write(content);
                        sw.Flush();
                        ms.Position = 0;
                        bytes = ms.ToArray();
                    }

                }
                if (_ides.ContainsKey(title) && _ides[title] != null)
                {
                    _main.Publish(bytes, _ides[title]);
                }
                else
                {
                    MessageBox.Show("Metdata information not found for the script to publish", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        internal void saveLocally_Click(object sender, EventArgs e)
        {
            var obj = GetCurrentTab();

            try
            {
                var mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
                if (obj.GetType() == typeof(Infosys.ATR.ScriptRepository.Views.ScriptDesigner) )//&& mode == "Online")
                {
                    var ide = obj as Views.ScriptDesigner;
                    var title = ide.ucName;
                    Models.Script s = null;
                    //if (_ides[title] != null)
                    //{
                    //    s = _ides[title];
                    //}
                    //else
                    //{
                    //    throw new Exception("Metadata information not found");
                    //}

                    if (ide.RichText.Length > 0)
                    {
                        SaveFileDialog save = new SaveFileDialog();
                        save.FileName = title;
                        save.Filter = "Script File|*.bat;*.vbs;*.iap;*.py;*.js;*.ps1";
                        save.OverwritePrompt = false;
                        var result = save.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(save.FileName))
                            {
                                var safeFileName = System.IO.Path.GetFileNameWithoutExtension(save.FileName);
                                if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(safeFileName))
                                {
                                    MessageBox.Show("Please provide the name without Special Characters", "Invalid Script name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;

                                }
                                sw.Write(ide.RichText);
                            }

                            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(save.FileName + ".meta"))
                            //{
                            //    System.Xml.Serialization.XmlSerializer xml = new System.Xml.Serialization.XmlSerializer(typeof(Models.Script));
                            //    xml.Serialize(sw, s);
                            //}

                            Save.ScriptTabs[ide] = true;
                            ide.ucName = Path.GetFileNameWithoutExtension(save.FileName);                          
                            ScriptSaved(this, new EventArgs<string>(save.FileName));
                        }
                    }
                    else
                    {
                        MessageBox.Show("No data to save", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                //else
                //{
                //    SaveFileDialog save = new SaveFileDialog();
                //    save.Filter = "Script File|*.bat;*.vbs;*.iap;*.py;*.js;*.ps1";
                //    save.FileName = pyIde.ucName;
                //    var result = save.ShowDialog();

                //    if (result == DialogResult.OK)
                //    {
                //        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(save.FileName))
                //        {
                //            var safeFileName = System.IO.Path.GetFileNameWithoutExtension(save.FileName);
                //            if (WEM.Infrastructure.Common.ValidationUtility.InvalidCharacterValidator(safeFileName))
                //            {
                //                MessageBox.Show("Please provide the name without Special Characters", "Invalid Script name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //                return;

                //            }
                //            sw.Write(pyIde.RichText);
                //        }
                //        Save.Tabs[pyIde] = true;
                //        pyIde.ucName = Path.GetFileNameWithoutExtension(save.FileName);
                //        ScriptSaved(this, new EventArgs<string>(save.FileName));
                //    }

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        internal void Open(string file)
        {
            var mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];

            var title = Path.GetFileNameWithoutExtension(file);
            string header = "";
            if (title.Length > 10)
            {
                header = title.Substring(0, 10);
            }
            else
                header = title;

            header = "Script - " + header;

            string content;
            using (StreamReader sr = new StreamReader(file))
            {
                content = sr.ReadToEnd();
            }
            //if (mode == "Online")
            //{
                var ide = AddScriptView(header, title, true, Mode.New);
                ide.scriptName = title;
                var xtn = Path.GetExtension(file);
                if (xtn.Contains('.'))
                    xtn = xtn.Remove(0, 1);
                ide.scriptExtention = xtn;
                ide.scriptPath = file;
                if (String.IsNullOrEmpty(ide.RichText))
                {
                    if(!xtn.ToLower().Equals("iap"))
                        ide.RichText = content;
                }

                this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Activate(ide);

                Save.ScriptTabs[ide] = true;
            //}
            //else
            //{
            //    var ide = AddPythonView(header, title, true);              
                
            //    if (String.IsNullOrEmpty(ide.RichText))
            //    {
            //        ide.RichText = content;
            //    }

            //    this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Activate(ide);

            //    Save.Tabs[ide] = true;
            //}

            Logger.Log("Script", "Openscript",
                "Edit script " + file + " - locally");
        }

        internal void OpenFromPackage(PackageMeta mData)    
        {
            var mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
            var title = mData.Content.Name;
            string header = "";
            if (title.Length > 10)
            {
                header = title.Substring(0, 10);
            }
            else
                header = title;

            header = "Script - " + header;

            string content;
            using (StreamReader sr = new StreamReader(mData.FileStream))
            {
                content = sr.ReadToEnd();
            }
            
            var ide = AddScriptView(header, title, true, Mode.New);
            ide.scriptName = title;
            var xtn = mData.Content.ContentType;
            if (xtn.Contains('.'))
                xtn = xtn.Remove(0, 1);
            ide.scriptExtention = xtn;
            ide.scriptPath = mData.PackagePath;///Path.Combine(metaData.PackageExtractLoc,metaData.Content.Name,metaData.Content.Name+"."+metaData.Content.ContentType);
            if (String.IsNullOrEmpty(ide.RichText))
            {
                if (!(xtn.ToLower().Equals("iap") || xtn.ToLower().Equals("iapd")))
                    ide.RichText = content;
                else if (mData.Content.isGeneratedScript)
                    ide.RichText = content;
            }

            this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Activate(ide);
            Save.ScriptTabs[ide] = true;           

            Logger.Log("Script", "Openscript",
                "Edit script " + ide.scriptPath + " - locally");
        }

        internal void PublishForPackage(PackageMeta mData)   
        {
            var obj = GetCurrentTab();

            if (obj.GetType().Name == "ScriptDesigner")
            {
                var ide = obj as Views.ScriptDesigner;
                ide.Script = Translators.ScriptPE_SE.ScriptOEtoPE(mData.Content);
                if (mData.Content.isGeneratedScript)
                    ide.IsGeneratedScript = true;

                ide.IsIapPackage = true;
                if (ide.OpMode == Mode.New)
                    ide.New();
                else
                    ide.Edit();
            }
            else if (obj.GetType() != typeof(Infosys.ATR.ScriptRepository.Views.MainRepositoryView))
            {
                var ide = obj as Views.PythonIDE;
                string content = ide.RichText;
                string title = ide.ucName;
                byte[] bytes;

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(ms))
                    {
                        sw.Write(content);
                        sw.Flush();
                        ms.Position = 0;
                        bytes = ms.ToArray();
                    }
                }
                if (_ides.ContainsKey(title) && _ides[title] != null)
                    _main.Publish(bytes, _ides[title]);
                else
                    MessageBox.Show("Metdata information not found for the script to publish", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void editScript_Click(ModuleController moduleController, EventArgs e)
        {
            OpenFileDialog edit = new OpenFileDialog();
            edit.Filter = "All files (*.*)|*.*";
            var result = edit.ShowDialog();
            if (result == DialogResult.OK)
            {
                Open(edit.FileName);

            }
        }

        private object GetCurrentTab()
        {
            return this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].ActiveSmartPart;
        }



        internal void runLocally_Click(object sender, EventArgs e)
        {
            var current = GetCurrentTab();

            if (current.GetType() == typeof(Views.MainRepositoryView))
            {
                _main.CheckScriptParamters(_main.selectedScriptIndex);
            }
            else
            {
                var ide = current as Views.PythonIDE;
                var title = ide.ucName;
                var s = _ides[title];
                if (s != null)
                {
                    if (_main != null)
                        _main.RunScript_1(s, _main.selectedScriptIndex);
                    else
                    {
                        _main = this.WorkItem.SmartParts.AddNew<ScriptRepository.Views.MainRepositoryView>();
                        _main.RunScript_1(s, _main.selectedScriptIndex);
                    }
                }
                else
                {
                    MessageBox.Show("Metdata information not found for the script to run", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        internal void newScript_Click(object sender, EventArgs e)
        {
            var mode = System.Configuration.ConfigurationManager.AppSettings["Mode"];
            if (mode == "Online")
                _main.CreateNewScript();
            //else
            //{
            //    EditScript(
            //}
        }


        internal void exit_Click(object sender, EventArgs e)
        {
            closeTab_Click(sender, e);
        }


        internal void AddViews()
        {
            string title = "Script Repository";
            _main = this.WorkItem.SmartParts.AddNew<ScriptRepository.Views.MainRepositoryView>(title);
            _main.Dock = DockStyle.Fill;
            //WindowSmartPartInfo sp = new WindowSmartPartInfo();
            //sp.Title = title;
            //this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(_main, sp);
            //_main.LoadCategory();
            //LoadThis(this, new EventArgs());
        }

        [EventSubscription(Constants.EventTopicNames.EditScript, ThreadOption.UserInterface)]
        public void EditScript(object sender, EventArgs<Object[]> e)
        {
            var script = e.Data[1] as Models.Script;
            this._script = script;
            string header = "";
            if (script.Name.Length > 10)
            {
                header = script.Name.Substring(0, 10);
            }
            else
                header = script.Name;

            header = "Script - " + header;

            //var ide = AddPythonView(header,script.Name,true);
            var ide = AddScriptView(header, script.Name, true, Mode.Edit);
            ide.Script = script;
            if (string.IsNullOrEmpty(ide.RichText))
            {
                ide.RichText = e.Data[0] as string;

                if (!_ides.ContainsKey(script.Name))
                    _ides.Add(script.Name, _script);
            }

            ide.Edit();
            
            this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Activate(ide);

            // Save.Tabs[pyIde] = true;
            Save.ScriptTabs[ide] = true;

            ShowRun(this, new EventArgs<bool>(true));

            Logger.Log("Script", "EditScript",
                "Edit script "+script.Name +" from repository" );
        }

        [EventSubscription(EventTopicNames.CurrentTabSelected, ThreadOption.UserInterface)]
        public void DisablePublish(object sender, EventArgs<string> e)
        {
            ScriptDesigner sd = null;

            foreach (KeyValuePair<ScriptDesigner, bool> kvp in Save.ScriptTabs)
            {
                if (kvp.Key.ucName == e.Data)
                {
                    sd = kvp.Key;
                    break;
                }
            }



            if (sd != null)
            {
                if (sd.OpMode == Mode.Edit)
                    DeActivatePublish(this, new EventArgs());
                else if (sd.OpMode == Mode.New)
                    ActivatePublish(this, new EventArgs());

                if (_ides.ContainsKey(sd.Title))
                {
                    var ide = _ides[sd.Title];
                    if (ide != null && ide.OpenedFromRepository == true)
                        ShowRun(this, new EventArgs<bool>(true));
                }
            }
        }

        private Views.PythonIDE AddPythonView(string header, string title, bool saved)
        {
            pyIde = this.WorkItem.SmartParts.Get(title) as Views.PythonIDE;

            if (pyIde == null)
            {
                pyIde = new Views.PythonIDE();
                pyIde.Dock = DockStyle.Fill;
                pyIde.ucName = title;
                WindowSmartPartInfo smartPart = new WindowSmartPartInfo();
                smartPart.Title = header;
                this.WorkItem.SmartParts.Add(pyIde, title);
                var sp = this.WorkItem.SmartParts.Get(title);
                this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(sp, smartPart);
                Save.Tabs.Add(pyIde, saved);
                return pyIde;
            }
            else
                return pyIde;
        }

        private Views.ScriptDesigner AddScriptView(string header, string title, bool saved, Mode mode)
        {
            sdIde = this.WorkItem.SmartParts.Get(title) as Views.ScriptDesigner;

            if (sdIde == null)
            {
                sdIde = new Views.ScriptDesigner();
                sdIde.Dock = DockStyle.Fill;
                sdIde.ucName = title;

                WindowSmartPartInfo smartPart = new WindowSmartPartInfo();
                smartPart.Title = header;
                this.WorkItem.SmartParts.Add(sdIde, title);
                sdIde.ucName = sdIde.Title = title;
                sdIde.OpMode = mode;
                sdIde.ShowEditor();
                var sp = this.WorkItem.SmartParts.Get(title);
                this.WorkItem.RootWorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(sp, smartPart);
                Save.ScriptTabs.Add(sdIde, saved);
                return sdIde;
            }
            else
                return sdIde;
        }

        [EventSubscription(EventTopicNames.TabHover, ThreadOption.UserInterface)]
        public void TabHover_Handler(object sender, EventArgs e)
        {
            var ide = GetCurrentTab();

            if (ide.GetType() == typeof(Views.PythonIDE))
            {
                var p = ide as Views.PythonIDE;

                if (TabHoverSet != null)
                    TabHoverSet(this, new EventArgs<string>(p.ucName));
            }
            else if (ide.GetType() == typeof(Views.ScriptDesigner))
            {
                var p = ide as Views.ScriptDesigner;

                if (TabHoverSet != null)
                    TabHoverSet(this, new EventArgs<string>(p.ucName));
            }
        }


        internal void ScriptRepoView()
        {
            if (_main != null)
            {
                WindowSmartPartInfo sp = new WindowSmartPartInfo();
                sp.Title = "Script Repository";
                _main.LoadCategory();
                if (_main.categories != null)
                    this.WorkItem.Workspaces[Constants.WorkspaceNames.TabWorkSpace].Show(_main, sp);

                // Logger.Log("Script", "ScriptExplorerView", "Script Explorer View Launched");
                ShowRun(this, new EventArgs<bool>(_main.enableControls));
            }      
        }

        // [EventSubscription(Constants.EventTopicNames.SaveAndClose, ThreadOption.UserInterface)]
        public void SaveAndClose_handler(object sender, EventArgs<FormClosingEventArgs> e)
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["Mode"] == "Online")
            //{
                List<Tuple<ScriptDesigner, bool>> savedScriptTabs = new List<Tuple<ScriptDesigner, bool>>();

                foreach (KeyValuePair<ScriptDesigner, bool> kvp in Save.ScriptTabs)
                {
                    savedScriptTabs.Add(new Tuple<ScriptDesigner, bool>(kvp.Key, kvp.Value));
                }

                savedScriptTabs.ForEach(s =>
                {

                    if (s.Item2 == false)
                    {
                        var close1 = CloseViewScriptDesigner(s.Item1);
                        if (!close1)
                            e.Data.Cancel = true;

                    }
                });
            //}
            //else
            //{
            //    List<Tuple<PythonIDE, bool>> savedTabs = new List<Tuple<PythonIDE, bool>>();

            //    foreach (KeyValuePair<PythonIDE, bool> kvp in Save.Tabs)
            //    {
            //        savedTabs.Add(new Tuple<PythonIDE, bool>(kvp.Key, kvp.Value));
            //    }

            //    savedTabs.ForEach(s =>
            //    {

            //        if (s.Item2 == false)
            //        {
            //            var close = CloseView(s.Item1);
            //            if (!close)
            //                e.Data.Cancel = true;

            //        }
            //    });
            //}


        }
    }
}
