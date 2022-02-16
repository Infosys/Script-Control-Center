using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WEMClient = Infosys.WEM.Client;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Scripts.Service.Contracts.Data;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace Infosys.ATR.ScriptRepository.Views
{
    public partial class ScriptDetails : UserControl
    {
        Models.Script _script;
        bool clearedOnLoad = false;
        string _categoryId, _subCategory, _paramId, _scriptUrl, _downloadScriptname;
        bool _newItem, _newParam;
        List<Models.ScriptParameter> _parameters = new List<Models.ScriptParameter>();
        WEMClient.ScriptRepository client = new WEMClient.ScriptRepository();
        private const string SCRIPTRESULT = "Script Execution Result";

        public class ScriptProcessedArgs : EventArgs
        {
            public string ScriptId { get; set; }
            public string CategoryId { get; set; }
            public string SubCategoryId { get; set; }
            public string Name { get; set; }
        }
        public delegate void ScriptProcessedEventHandler(ScriptProcessedArgs e);
        public event ScriptProcessedEventHandler ScriptProcessed;

        public ScriptDetails(Models.Script script, string categoryId = "", bool newItem = false, string subCategoryId = "")
        {
            InitializeComponent();
            _newItem = newItem;
            _categoryId = categoryId;
            _subCategory = subCategoryId;
            toolTip1.SetToolTip(txtScriptName, "Script name should be same as the file name (without ext) in case of script file of type .iap (ifea)");

            if (script != null)
            {
                _script = script;
                txtScriptName.Text = script.Name;
                txtDesc.Text = script.Description;
                //txtScriptType.Text = script.ScriptType;
                //txtLoc.Text = script.ScriptLocation;
                if (!string.IsNullOrEmpty(script.ScriptLocation))
                {
                    btnDownload.Enabled = true;
                    _downloadScriptname = script.Name + "." + script.ScriptType;
                }
                else
                    btnDownload.Enabled = false;
                _scriptUrl = script.ScriptLocation;
                txtCommand.Text = script.TaskCommand;
                txtArgs.Text = script.ArgumentString;
                txtWorkingDir.Text = script.WorkingDir;
                lblTaskType.Text = script.TaskType;
                cmbTaskType.SelectedItem = script.TaskType;
                UpdateTaskType(script.TaskType);
                _subCategory = script.SubCategory;
                btnRunScript.Enabled = true;
                if (script.Parameters != null && script.Parameters.Count > 0)
                {
                    _parameters = script.Parameters;
                    dgParams.DataSource = GetScriptSubset(script.Parameters);
                    ClearSelection();
                }
                else
                {
                    pnlParameters.Visible = false;
                }
            }
            else
            {
                pnlParameters.Visible = false;
                btnRunScript.Enabled = false;
            }

            if (newItem)
            {
                btnSave.Text = "Add";
                label2.Text = "New Script";
            }

            //fetch all the categories
            FetchCategories(categoryId);
        }

        private void FetchCategories(string categoryId)
        {
            //fetch all the categories
            GetAllCategoriesResMsg catResponse = client.ServiceChannel.GetAllCategories();
            if (catResponse != null && catResponse.Categories != null && catResponse.Categories.Count > 0)
            {
                cmbCategory.DisplayMember = "Name";
                cmbCategory.ValueMember = "CategoryId";
                cmbCategory.DataSource = catResponse.Categories;

                if (!string.IsNullOrEmpty(categoryId))
                {
                    catResponse.Categories.ForEach(c =>
                    {
                        if (c.CategoryId.ToString() == categoryId)
                            cmbCategory.SelectedValue = int.Parse(categoryId);
                    });
                }
            }
        }
        private void btnRunScript_Click(object sender, EventArgs e)
        {
            int executionScriptId = 0;
            int subCatID = 0;
            Infosys.WEM.ScriptExecutionLibrary.ScriptIndentifier scriptIden = new Infosys.WEM.ScriptExecutionLibrary.ScriptIndentifier();
            if (Int32.TryParse(_script.Id, out executionScriptId))
            {
                scriptIden.ScriptId = executionScriptId;
            }
            if (Int32.TryParse(_subCategory, out subCatID))
            {
                scriptIden.SubCategoryId = subCatID;
            }

            if (executionScriptId > 0 && subCatID > 0 && !String.IsNullOrEmpty(_scriptUrl))
            {
                Infosys.WEM.ScriptExecutionLibrary.ExecutionResult resutl = Infosys.WEM.ScriptExecutionLibrary.ScriptExecutionManager.Execute(scriptIden);

                if (resutl.IsSuccess)
                    MessageBox.Show("Script executed successfully. Please refer to below details for output.\n\n" + resutl.SuccessMessage, SCRIPTRESULT);
                else
                    MessageBox.Show("Error in script execution. Refer to following details.\n\n" + resutl.ErrorMessage, SCRIPTRESULT);
            }
            else
            {
                MessageBox.Show("Invalid Script", SCRIPTRESULT);
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidationPassed())
            {
                MessageBox.Show("Please provide the details for all these Script fields- Cateory, Sub Category, Name, Task Type.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            try
            {

                if (_newItem)
                {
                    AddScriptReqMsg req = new AddScriptReqMsg();
                    req.Script = new Script();
                    if (txtArgs.Enabled)
                        req.Script.ArgString = txtArgs.Text.Trim();
                    req.Script.BelongsToOrg = System.Configuration.ConfigurationManager.AppSettings["Company"];
                    req.Script.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    req.Script.Description = txtDesc.Text.Trim();
                    req.Script.Name = txtScriptName.Text.Trim();
                    req.Script.Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_parameters);
                    if (txtScriptFile.Enabled && !string.IsNullOrEmpty(txtScriptFile.Text.Trim()))
                        req.Script.ScriptType = txtScriptFile.Text.Trim().Substring(txtScriptFile.Text.Trim().IndexOf('.') + 1);
                    req.Script.SubCategoryId = (int)cmbSubCategory.SelectedValue;
                    if (txtCommand.Enabled)
                        req.Script.TaskCmd = txtCommand.Text.Trim();
                    req.Script.TaskType = lblTaskType.Text;
                    req.Script.WorkingDir = txtWorkingDir.Text.Trim();

                    if (txtScriptFile.Enabled && txtScriptFile.Text.Trim() != "")
                    {
                        //System.IO.FileStream stream = new System.IO.FileStream(txtScriptFile.Text.Trim(), System.IO.FileMode.Open, System.IO.FileAccess.Read);
                        //req.Script.ScriptContent = stream;

                        //cant use steram with webhttpbinding, refer- http://stackoverflow.com/questions/24527029/unable-to-send-image-file-as-part-datacontract
                        //causes error- Type 'System.IO.FileStream' with data contract name 'FileStream:http://schemas.datacontract.org/2004/07/System.IO' is not expected. Consider using a DataContractResolver or add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to DataContractSerializer.
                        //need to instead use byte[]
                        byte[] filecontent = null;
                        if (cmbTaskType.SelectedItem != null)
                        {
                            switch (cmbTaskType.SelectedItem.ToString())
                            {
                                //the below case is commented because as per the new implementation of the ifea runtime
                                //the ifea scripts are already packaged as single .iap file and ifea runtime will be also expecting the script package as single .iap file
                                //but keeping the switch so that in future for any other types of scripts, if the approach happens to be different then 
                                //we will be able to handle it appropriately

                                //case "ifea":
                                //    filecontent = PackageIfeaFolder(txtScriptFile.Text.Trim());
                                //    req.Script.ScriptContent = filecontent;
                                //    break;
                                default:
                                    if (System.IO.File.Exists(txtScriptFile.Text.Trim()))
                                    {
                                        filecontent = System.IO.File.ReadAllBytes(txtScriptFile.Text.Trim());
                                        req.Script.ScriptContent = filecontent;
                                    }
                                    break;
                            }
                        }
                    }

                    if (client.ServiceChannel.AddScript(req).IsSuccess)
                    {
                        if (ScriptProcessed != null)
                        {
                            ScriptProcessedArgs scriptArgs = new ScriptProcessedArgs();
                            scriptArgs.SubCategoryId = cmbSubCategory.SelectedValue.ToString();
                            scriptArgs.CategoryId = cmbCategory.SelectedValue.ToString();
                            ScriptProcessed(scriptArgs);
                        }
                        MessageBox.Show(string.Format("The new script- {0} is added.", txtScriptName.Text.Trim()), "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("There is an error adding the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    UpdateScriptReqMsg req = new UpdateScriptReqMsg();
                    req.Script = new Script();
                    if (txtArgs.Enabled)
                        req.Script.ArgString = txtArgs.Text.Trim();
                    req.Script.BelongsToOrg = System.Configuration.ConfigurationManager.AppSettings["Company"];
                    req.Script.ScriptId = int.Parse(_script.Id);
                    req.Script.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    req.Script.Description = txtDesc.Text.Trim();
                    req.Script.Name = txtScriptName.Text.Trim();
                    req.Script.ScriptURL = _scriptUrl;
                    req.Script.Parameters = Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_parameters);
                    //assign script id
                    if (req.Script.Parameters != null)
                        foreach (ScriptParam parameter in req.Script.Parameters)
                        {
                            parameter.ScriptId = req.Script.ScriptId;
                            parameter.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        }
                    if (txtScriptFile.Enabled && !string.IsNullOrEmpty(txtScriptFile.Text.Trim()))
                        req.Script.ScriptType = txtScriptFile.Text.Trim().Substring(txtScriptFile.Text.Trim().IndexOf('.') + 1);
                    req.Script.SubCategoryId = (int)cmbSubCategory.SelectedValue;
                    if (txtCommand.Enabled)
                        req.Script.TaskCmd = txtCommand.Text.Trim();
                    req.Script.TaskType = lblTaskType.Text;
                    req.Script.WorkingDir = txtWorkingDir.Text.Trim();

                    if (txtScriptFile.Enabled && txtScriptFile.Text.Trim() != "")
                    {
                        if (System.IO.File.Exists(txtScriptFile.Text.Trim()))
                        {
                            byte[] filecontent = null;
                            switch (cmbTaskType.SelectedItem.ToString())
                            {
                                //the below case is commented because as per the new implementation of the ifea runtime
                                //the ifea scripts are already packaged as single .iap file and ifea runtime will be also expecting the script package as single .iap file
                                //but keeping the switch so that in future for any other types of scripts, if the approach happens to be different then 
                                //we will be able to handle it appropriately

                                //case "ifea":
                                //    filecontent = PackageIfeaFolder(txtScriptFile.Text.Trim());
                                //    req.Script.ScriptContent = filecontent;
                                //    break;
                                default:
                                    if (System.IO.File.Exists(txtScriptFile.Text.Trim()))
                                    {
                                        filecontent = System.IO.File.ReadAllBytes(txtScriptFile.Text.Trim());
                                        req.Script.ScriptContent = filecontent;
                                    }
                                    break;
                            }
                        }

                    }

                    if (client.ServiceChannel.UpdateScript(req).IsSuccess)
                    {
                        if (ScriptProcessed != null)
                        {
                            ScriptProcessedArgs scriptArgs = new ScriptProcessedArgs();
                            scriptArgs.SubCategoryId = cmbSubCategory.SelectedValue.ToString();
                            scriptArgs.CategoryId = cmbCategory.SelectedValue.ToString();
                            scriptArgs.ScriptId = _script.Id;
                            ScriptProcessed(scriptArgs);
                        }
                        MessageBox.Show(string.Format("The script- {0} is updated.", txtScriptName.Text.Trim()), "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        MessageBox.Show("There is an error updating the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string err = "There is an error adding/updating the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.";
                err = err + "\nMore Infomation- " + ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtScriptName.ReadOnly = false;
            if (cmbTaskType.SelectedItem != null)
            {
                switch (cmbTaskType.SelectedItem.ToString().ToLower())
                {
                    case "file":
                        OpenFileDialog browseScript = new OpenFileDialog();
                        browseScript.Filter = "Script Files(*.txt; *.bat; *.cmd; *.vbs; *.iap)|*.txt; *.bat; *.cmd; *.vbs; *.iap";
                        if (browseScript.ShowDialog() == DialogResult.OK)
                        {
                            txtScriptFile.Text = browseScript.FileName;
                            if (System.IO.Path.GetExtension(txtScriptFile.Text) == ".iap")
                            {
                                txtScriptName.Text = browseScript.SafeFileName.Substring(0, browseScript.SafeFileName.IndexOf('.'));
                                txtScriptName.ReadOnly = true;
                            }
                        }
                        break;
                    //the below case is commented because as per the new implementation of the ifea runtime
                    //the ifea scripts are already packaged as single .iap file and ifea runtime will be also expecting the script package as single .iap file
                    //but keeping the switch so that in future for any other types of scripts, if the approach happens to be different then 
                    //we will be able to handle it appropriately

                    //case "ifea":
                    //    FolderBrowserDialog browseIfeaFolder = new FolderBrowserDialog();
                    //    browseIfeaFolder.Description = "Select the ifea folder, e.g. *.ifea :";
                    //    browseIfeaFolder.RootFolder = Environment.SpecialFolder.MyComputer;
                    //    if (browseIfeaFolder.ShowDialog() == DialogResult.OK)
                    //    {
                    //        //verify if the right ifea folder is selected
                    //        if (browseIfeaFolder.SelectedPath.EndsWith(".ifea", true, null))
                    //            txtScriptFile.Text = browseIfeaFolder.SelectedPath;
                    //        else
                    //            MessageBox.Show("Select the right ifea folder, e.g. *.ifea.", "Wrong ifea folder", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    //    }
                    //    break;
                }
            }
        }

        private void btnAddParam_Click(object sender, EventArgs e)
        {
            if (!pnlParameters.Visible)
                pnlParameters.Visible = true;
            pnlParam.Visible = true;
            _newParam = true;
        }

        private List<Models.ScriptParameterSubSet> GetScriptSubset(List<Models.ScriptParameter> parameters)
        {
            List<Models.ScriptParameterSubSet> objs = new List<Models.ScriptParameterSubSet>();
            parameters.ForEach(c =>
            {
                objs.Add(new Models.ScriptParameterSubSet() { Name = c.Name, IsMandatory = c.IsMandatory.ToString(), IOType = c.IOType });
            });
            return objs;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlParam.Visible = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbIOTypes.SelectedIndex < 0 || cmbBool.SelectedIndex < 0)// || string.IsNullOrEmpty(txtParamName.Text.Trim()))
            {
                MessageBox.Show("Please provide the details for all these Parameter fields- Name, Direction, Is Mandatory.", "Data Missing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //once the parameter is added to the grid, close it
            Models.ScriptParameter param1 = new Models.ScriptParameter();
            param1.AllowedValues = txtAllowedValues.Text.Trim();
            if (_newParam)
                param1.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            else
                param1.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            param1.DefaultValue = txtDefaultValue.Text.Trim();
            if (cmbBool.SelectedItem != null)
                param1.IsMandatory = bool.Parse(cmbBool.SelectedItem.ToString());
            if (cmbIsSecret.SelectedItem != null)
                param1.IsSecret = bool.Parse(cmbIsSecret.SelectedItem.ToString());
            if (cmbIsPaired.SelectedItem != null)
                param1.IsPaired = bool.Parse(cmbIsPaired.SelectedItem.ToString());
            param1.Name = txtParamName.Text.Trim();
            param1.IOType = (Models.ParameterIOTypes)Enum.Parse(typeof(Models.ParameterIOTypes), cmbIOTypes.SelectedItem.ToString());

            if (_newParam)
            {
                param1.ParamId = Guid.NewGuid().ToString();
                _parameters.Add(param1);
            }
            else
            {
                param1.ParamId = _paramId;
                _parameters.ForEach(p =>
                {
                    if (p.ParamId == _paramId)
                        _parameters.Remove(p);
                });
                _parameters.Add(param1);
            }
            //chane the datasource of the grid to the new parameter list
            dgParams.DataSource = GetScriptSubset(_parameters);
            pnlParam.Visible = false;
        }

        private void dgParams_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                //i.e. Delete is called
                if (MessageBox.Show("Are you sure you want to delete the parameter?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //delete the row selected
                    _parameters.RemoveAt(e.RowIndex);
                    //chane the datasource of the grid to the new parameter list
                    dgParams.DataSource = GetScriptSubset(_parameters);
                }
            }
            else if (e.RowIndex >= 0)
            {
                pnlParam.Visible = true;
                PopulateParameter(e.RowIndex);
            }
        }

        private void PopulateParameter(int row)
        {
            _newParam = false;
            Models.ScriptParameter parameter = _script.Parameters[row];
            txtParamName.Text = parameter.Name;
            txtAllowedValues.Text = parameter.AllowedValues;
            txtDefaultValue.Text = parameter.DefaultValue;
            if (parameter.IsMandatory)
                cmbBool.SelectedIndex = 0;
            else
                cmbBool.SelectedIndex = 1;

            if (parameter.IsSecret)
                cmbIsSecret.SelectedIndex = 1;
            else
                cmbIsSecret.SelectedIndex = 0;

            if (parameter.IsPaired)
                cmbIsPaired.SelectedIndex = 0;
            else
                cmbIsPaired.SelectedIndex = 1;

            //cmbIOTypes.SelectedText = parameter.IOType.ToString();
            cmbIOTypes.SelectedIndex = (int)parameter.IOType;
            _paramId = parameter.ParamId;
        }

        private void ClearSelection()
        {
            if (dgParams.Rows.Count > 0)
                dgParams.Rows[0].Selected = false;
        }

        private void dgParams_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (!clearedOnLoad)
            {
                ClearSelection();
                clearedOnLoad = true;
            }
        }

        private void UpdateTaskType(string taskType)
        {
            switch (taskType.ToLower())
            {
                case "file":
                    EnableDisableTasksDetails(false);
                    break;
                case "command":
                    EnableDisableTasksDetails(true);
                    break;
            }
        }

        private void EnableDisableTasksDetails(bool isCommand)
        {
            btnBrowse.Enabled = !isCommand;
            txtScriptFile.Enabled = !isCommand;
            btnAddParam.Enabled = !isCommand;
            txtCommand.Enabled = isCommand;
            txtArgs.Enabled = isCommand;
            //txtWorkingDir.Enabled = isCommand;
        }

        //private void lblUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    if (lblUrl.Text != "---")
        //    {
        //        ProcessStartInfo sInfo = new ProcessStartInfo(lblUrl.Text);
        //        Process.Start(sInfo);
        //    }
        //}

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //fetch the subcategories and bind the intended one
            GetAllSubCategoriesResMsg response = client.ServiceChannel.GetAllSubCategories(cmbCategory.SelectedValue.ToString());
            if (response.SubCategories != null && response.SubCategories.Count > 0)
            {
                cmbSubCategory.DataSource = response.SubCategories;
                cmbSubCategory.DisplayMember = "Name";
                cmbSubCategory.ValueMember = "CategoryId";
                if (!string.IsNullOrEmpty(_subCategory))
                {
                    response.SubCategories.ForEach(sc =>
                    {
                        if (sc.CategoryId.ToString() == _subCategory)
                        {
                            cmbSubCategory.SelectedValue = sc.CategoryId;
                        }
                    });
                }
            }
            else
                cmbSubCategory.DataSource = null;
        }

        private void cmbTaskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblTaskType.Text = cmbTaskType.SelectedItem.ToString();
            UpdateTaskType(lblTaskType.Text);
        }

        private bool ValidationPassed()
        {
            bool isPass = true;
            if (cmbTaskType.SelectedIndex < 0 || cmbCategory.SelectedIndex < 0 || cmbSubCategory.SelectedIndex < 0)
            {
                isPass = false;
                return isPass;
            }

            if (string.IsNullOrEmpty(txtScriptName.Text.Trim()))
            {
                isPass = false;
                return isPass;
            }
            return isPass;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadScript();
        }

        private void DownloadScript()
        {
            this.Cursor = Cursors.WaitCursor;
            if (!string.IsNullOrEmpty(_scriptUrl))
            {
                //SaveFileDialog saveScript = new SaveFileDialog();
                FolderBrowserDialog saveScript = new FolderBrowserDialog();
                saveScript.Description = "Select the script download folder:";
                //saveScript.FileName = _downloadScriptname;                
                if (saveScript.ShowDialog() == DialogResult.OK)
                {
                    string downloadLoc = saveScript.SelectedPath + "\\" + _downloadScriptname;
                    WebRequest request = WebRequest.Create(_scriptUrl);
                    CredentialCache credential = new CredentialCache();
                    credential.Add(new Uri(_scriptUrl), "NTLM", CredentialCache.DefaultNetworkCredentials);
                    request.Credentials = credential;
                    try
                    {
                        using (WebResponse response = request.GetResponse())
                        {
                            using (FileStream stream = new FileStream(downloadLoc, FileMode.Create, FileAccess.Write))
                            {
                                byte[] bytes = ReadFully(response.GetResponseStream());
                                stream.Write(bytes, 0, bytes.Length);
                            }
                        }
                        MessageBox.Show("Script file downloaded.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        string err = "There is an error downloading the Script.";
                        err = err + "\nMore Infomation- " + ex.Message;
                        if (ex.InnerException != null)
                            err = err + ". \nInner Exception- " + ex.InnerException.Message;
                        MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
            this.Cursor = Cursors.Default;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        //private byte[] PackageIfeaFolder(string ifeaFolder)
        //{
        //    byte[] package = null;
        //    if (System.IO.Directory.Exists(ifeaFolder))
        //    {
        //        //zip the content in the provided folder
        //        //and provide the compressed file name same as folder name with ext- .ifeapkg
        //        //and then return the byte array of the so-formed compressed file
        //        Infosys.Compress_DeCompress.Compression compClient = new Compress_DeCompress.Compression();
        //        //get the ifea folder name
        //        string[] folderparts = ifeaFolder.Split(new char[] { '\\' });
        //        string packagename = folderparts[folderparts.Length - 1];
        //        compClient.AddFolder(packagename, ifeaFolder);
        //        package = System.IO.File.ReadAllBytes(packagename);
        //    }
        //    return package;
        //}
    }
}
