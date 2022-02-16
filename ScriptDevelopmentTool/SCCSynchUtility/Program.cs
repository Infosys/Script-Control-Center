/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEMClient = Infosys.WEM.Client;
using PE = Infosys.ATR.ScriptRepository.Models;
using System.IO;
using System.IO.Compression;
using Infosys.WEM.SecureHandler;
using static Infosys.ATR.ScriptRepository.Views.ScriptDetails;
using System.Xml.Serialization;
using Infosys.WEM.Client;
using System.Reflection;

namespace SCCSynchUtility
{
    class Program
    {
        static WEMClient.CommonRepository commonRepositoryClient = new WEMClient.CommonRepository();
        static WEMClient.ScriptRepository scriptRepository = new WEMClient.ScriptRepository();
        static WEMClient.AutomationTracker automationTracker = new WEMClient.AutomationTracker();
        static List<PE.Category> categories = new List<PE.Category>();

        private static object lockObject = new object();
        static string logFilePath = "";
        static string sccServiceUrl = null, localpath = null;
        static string companyId = null;
        static string categoryID = null;
        static string zipFilePath = null;
        public static string FileContent { get; set; }

        public static event ReadScript ReadScript_Handler;
        static void Main(string[] args)
        {
            try
            {
                //compressFolder("D:\\test\\SCCScripts");
                //ExtractZipFile("test");
                logFilePath = ConfigurationManager.AppSettings["LogFilePath"].ToString();
                string strMode = null;
                do
                {
                    Console.WriteLine("\nSelect mode from below options:");
                    Console.WriteLine("1. Export");
                    Console.WriteLine("2. Import");
                    Console.WriteLine("3. Exit\n");
                    strMode = Console.ReadLine();
                    if (strMode == "1")
                    {
                        ExportScripts();
                    }
                    else if (strMode == "2")
                    {
                        ImportScripts();
                    }
                    else if(strMode!="1" || strMode!="2" || strMode!="3")
                    {
                        Console.WriteLine("Please select valid mode");                        
                    }

                } while (strMode != "3");

            }
            catch (Exception ex)
            {
                //throw ex;
                Console.WriteLine("Exception Occured in SCCsyncUtility... Error Message:" + ex.Message);
            }
        }
               

        private static void ReadInputs()
        {
            try
            {                
                Console.WriteLine("\nEnter CompanyId");
                companyId = Console.ReadLine();
                Console.WriteLine("\nEnter CategoryId");
                categoryID =Console.ReadLine();
                Console.WriteLine("\nEnter SCCServiceUrl");
                sccServiceUrl = Console.ReadLine();
                Console.WriteLine("\nEnter localpath to export");
                zipFilePath = Console.ReadLine();
                if (Path.GetExtension(zipFilePath) != ".zip")
                {
                    Console.WriteLine("\n Enter valid filepath example:c:\\archive\\scc124-jan.zip");
                    zipFilePath = Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        private static void ReadImportInputs()
        {
            try
            {
                Console.WriteLine("\nEnter CompanyId");
                companyId = Console.ReadLine();
                //Console.WriteLine("\nEnter CategoryId");
                //categoryID = Console.ReadLine();
                Console.WriteLine("\nEnter SCCServiceUrl");
                sccServiceUrl = Console.ReadLine();
                Console.WriteLine("\nEnter localpath to import");
                localpath = Console.ReadLine();
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void ImportScripts()
        {
            try
            {                
                ReadImportInputs();
                if (!string.IsNullOrEmpty(localpath))
                {
                    ExtractZipFile(localpath);
                    Console.WriteLine(string.Format("\nLoading categories "));
                   
                    //GetAllCategories(companyId, sccServiceUrl);
                    //Console.WriteLine(string.Format("\nTotal categories fetched :{0}", categories.Count));

                    string[] directories = Directory.GetDirectories(Path.GetDirectoryName(localpath), "*",SearchOption.AllDirectories);
                    
                    foreach (string s in directories)
                    {
                         
                        //folders.Add(s.Remove(0, Path.GetDirectoryName(localpath).Length));
                        string[] filePaths = Directory.GetFiles(s).Where(name => !name.EndsWith(".zip") && !name.EndsWith(".xml")).ToArray();
                        foreach(string  file in filePaths)
                        {                           
                            GetAllCategories(companyId, sccServiceUrl);
                            string temppath = file.Remove(0, Path.GetDirectoryName(localpath).Length);
                            if (!string.IsNullOrEmpty(temppath))
                            {
                                string[] path = temppath.Split(new[] { "\\" }, StringSplitOptions.None).Where(c => !string.IsNullOrEmpty(c)).ToArray();
                                
                                if (path.Length > 0)
                                {
                                    
                                    string parentID = null;
                                    bool breakflag = false;
                                    for (int i = 0; i < path.Length; i++)
                                    {

                                        if (!string.IsNullOrEmpty(path[i]) && string.IsNullOrEmpty(Path.GetExtension(path[i])) && !breakflag)
                                        {
                                            if (categories == null)
                                            {
                                                int addedID = AddCategory(path[i], parentID, sccServiceUrl);
                                                if (addedID > 0)
                                                    parentID = Convert.ToString(addedID);
                                                Console.WriteLine("Category: " + path[i] + " added successfully");
                                            }
                                            else
                                            {
                                                var category = parentID == null ? categories.Where(c => c.Name.Equals(path[i], StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() :
                                                          categories.Where(c => c.Name.Equals(path[i], StringComparison.InvariantCultureIgnoreCase) && c.ParentId == Convert.ToInt32(parentID)).FirstOrDefault(); ;
                                                if (category == null && i == 0)
                                                {
                                                    Console.WriteLine(string.Format("category:{0} not exists ", path[i]));
                                                    breakflag = true;
                                                }
                                                else if (category == null && i > 0)
                                                {
                                                    int addedID = AddCategory(path[i], parentID, sccServiceUrl);
                                                    if (addedID > 0)
                                                        parentID = Convert.ToString(addedID);
                                                    Console.WriteLine("Category: " + path[i] + " added successfully" );
                                                }
                                                else if (category != null)
                                                    parentID = category.Id;
                                            }
                                        }

                                        else if (!string.IsNullOrEmpty(path[i]) && !string.IsNullOrEmpty(Path.GetExtension(path[i])) && !breakflag)
                                        {
                                            //AddScript(path[i], categoryID);
                                            UploadScript(null, null, path[i], "File", parentID, file);
                                        }
                                    }
                                }
                               
                            }
                            
                        }
                    }                   
                }
                else
                {
                    Console.WriteLine("\nLocalpath value not provided");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error Occured while uploading script");
            }
            finally
            {
                //string scriptPath = Path.GetDirectoryName(localpath) + "\\Script Central";
                //bool isExists = Directory.Exists(scriptPath);
                //if (isExists)
                //    Directory.Delete(scriptPath);
                Console.ReadLine();
            }
        }

        
        private static void UploadScript(byte[] p, string xtn, string scriptName,string tasktype,string catId,string filepath)
        {

            PE.Script _scriptObj = new PE.Script();
            XmlSerializer xs = new XmlSerializer(typeof(PE.Script));
            using (var sr = new StreamReader(Path.GetDirectoryName(filepath) + "\\" + Path.GetFileNameWithoutExtension(filepath) + ".xml"))
            {
                _scriptObj = (PE.Script)xs.Deserialize(sr);
            }

            Infosys.WEM.Scripts.Service.Contracts.Message.AddScriptReqMsg req = new Infosys.WEM.Scripts.Service.Contracts.Message.AddScriptReqMsg();
            req.Script = new Infosys.WEM.Scripts.Service.Contracts.Data.Script();
            req.Script.BelongsToOrg = "1";
            //req.Script.ScriptId = int.Parse(_script.Id);
            req.Script.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            req.Script.Description = _scriptObj.Description;
            req.Script.Name = _scriptObj.Name;
            req.Script.IfeaScriptName = _scriptObj.IfeaScriptName;
            req.Script.UsesUIAutomation = false;
            req.Script.Tags = null;


            foreach (var param in _scriptObj.Parameters)
            {
                if (param.IsSecret)
                    param.DefaultValue = SecurePayload.Secure(param.DefaultValue, "IAP2GO_SEC!URE");
            }

            req.Script.Parameters = Infosys.ATR.ScriptRepository.Translators.ScriptParameterPE_SE.ScriptParameterListPEtoSE(_scriptObj.Parameters);

            byte[] filecontent = null;
            

            if (tasktype.ToString() == "File")
            {
                req.Script.ScriptType = Path.GetExtension(scriptName).Replace(".", "");
            }

            req.Script.CategoryId = Convert.ToInt32(catId);
            
            req.Script.TaskCmd = _scriptObj.TaskCommand;
            req.Script.TaskType = _scriptObj.TaskType;
            req.Script.WorkingDir =null;
            req.Script.RunAsAdmin = _scriptObj.RunAsAdmin;
            req.Script.ScriptURL = _scriptObj.ScriptLocation;

            /*commonRepositoryClient.ServiceUrl = sccServiceUrl + "/" + WEMClient.Services.WEMCommonService + ".svc";
            req.Script.StorageBaseUrl =commonRepositoryClient.ServiceChannel.GetCompanyDetails(companyId.ToString()).Company.StorageBaseUrl;
            

            string stoarageBaseUrl = CommonServices.Instance.StorageBaseURL;
            string _scriptUrl = stoarageBaseUrl + _scriptObj.ScriptLocation;
            //req.Script.ScriptURL = GetRelativeURL(_scriptUrl);*/

            if (System.IO.File.Exists(filepath))
            {
                if (Infosys.WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(Path.GetFileNameWithoutExtension(filepath)))
                {
                    //throw new Exception("Please provide the file name without Special Characters");
                    Console.WriteLine("Please provide the file name without Special Characters", "Invalid file name...");
                    return;
                }
                filecontent = System.IO.File.ReadAllBytes(filepath); //to read the entire package

            }

            if (filecontent != null)
            {
                if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                    filecontent = SecurePayload.SecureBytes(filecontent);

                req.Script.ScriptContent = filecontent;
            }
                  

            scriptRepository.ServiceUrl = sccServiceUrl + "/" + WEMClient.Services.WEMScriptService + ".svc";
            var scriptClient = scriptRepository.ServiceChannel;

            var response = scriptClient.AddScript(req);
            if (response.ServiceFaults != null)
            {
                var faults = response.ServiceFaults;
                Infosys.WEM.Infrastructure.Common.WEMException ex = new Infosys.WEM.Infrastructure.Common.WEMException();
                ex.Data.Add("ServiceFaults", faults);
                throw ex;
            }
            if (response.IsSuccess)
            {
               
                Console.WriteLine(string.Format("The new script- {0} is added.", scriptName), "Done");
            }
            else
                Console.WriteLine("There is an error adding the Script. \nPlease verify if the details provided are correct and name is not same as any existing Script.", "Error");

        }

        private static string GetRelativeURL(string url)
        {
            string relativeURL = url;
            int pos = url.IndexOf("iapscriptstore");
            if (pos > 0)
            {
                relativeURL = url.Substring(pos - 1);
            }

            return relativeURL;
        }

        private static int AddCategory(string name, string parentID, string sccURL)
        {
            
            try
            {
                Console.WriteLine(string.Format("Adding category:{0} and parentID:{1}", name, parentID));
                //LogEntry("Entering AddCategory Method");     
                
                //Load updated categories list -- START 
                commonRepositoryClient.ServiceUrl = sccURL + "/" + WEMClient.Services.WEMCommonService + ".svc";
                var commonChannel = commonRepositoryClient.ServiceChannel;

                Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response =
                           commonChannel.GetAllCategoriesByCompany(companyId, Infosys.ATR.ExportUtility.Constants.Application.ModuleID);

                var updatedCategories = Infosys.ATR.ScriptRepository.Translators.CategoryTreePE_SE.CategoryListSEtoPE(response.Categories);
                //Load updated categories list -- END 

                int newCategoryId = updatedCategories != null?Convert.ToInt32(updatedCategories.OrderByDescending(c=>Convert.ToInt32(c.Id)).FirstOrDefault().Id)+1 :1;
                
                Infosys.WEM.Service.Common.Contracts.Message.AddCategoryReqMsg request = new Infosys.WEM.Service.Common.Contracts.Message.AddCategoryReqMsg();
                List < Infosys.WEM.Service.Common.Contracts.Data.Category> categoriesList = new List<Infosys.WEM.Service.Common.Contracts.Data.Category>();
                categoriesList.Add(new Infosys.WEM.Service.Common.Contracts.Data.Category() {
                    ParentCategoryId=Convert.ToInt32(parentID),
                    Name=name,
                    CompanyId=Convert.ToInt32(companyId),
                    Description=name,
                    CategoryId= newCategoryId,
                    ModuleID =Convert.ToInt32(Infosys.ATR.ExportUtility.Constants.Application.ModuleID),
                    ParentId= Convert.ToInt32(parentID)
                });
                request.Categories = categoriesList;

                Infosys.WEM.Service.Common.Contracts.Message.AddCategoryResMsg response1 =
                           commonChannel.AddCategory(request);

                bool isSuccess = response!=null? response1.IsSuccess:false;
                return newCategoryId;
                //LogEntry("Exiting AddCategory Method");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static List<PE.Category> GetChildCategories(string id,List<PE.Category> masterCategories)
        {
            try
            {
                return masterCategories.Where(c => c.ParentId == Convert.ToInt32(id)).ToList();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static string GetScriptContent(PE.Script script)
        {
            LogEntry("Download script Content for script :" + script.Name);
            string content = "";
            string scriptUrl = WEMClient.CommonServices.Instance.StorageBaseURL + script.SourceUrl;
            try
            {
                Infosys.ATR.RepositoryAccess.Entity.ScriptDoc scriptDoc = new Infosys.ATR.RepositoryAccess.Entity.ScriptDoc();

                Infosys.WEM.Scripts.Service.Contracts.Data.Script scriptdata = new Infosys.WEM.Scripts.Service.Contracts.Data.Script();

                scriptdata.CategoryId = int.Parse(script.CategoryId);
                scriptdata.ScriptId = int.Parse(script.Id);
                scriptdata.ScriptURL = script.ScriptLocation;
                scriptdata.Name = script.Name;
                int companyid = int.Parse(ConfigurationManager.AppSettings["Company"]);
                scriptdata.BelongsToOrg = companyid.ToString("00000");
                scriptdata.StorageBaseUrl = WEMClient.CommonServices.Instance.StorageBaseURL;

                scriptdata.ScriptFileVersion = int.Parse(script.Version);

                Infosys.WEM.ScriptExecutionLibrary.ScriptIndentifier identifier = new Infosys.WEM.ScriptExecutionLibrary.ScriptIndentifier();
                identifier.SubCategoryId = int.Parse(script.CategoryId);
                identifier.ScriptId = int.Parse(script.Id);

                scriptDoc = Infosys.WEM.ScriptExecutionLibrary.Translator.Script_DocumentEntity.ScriptToDocument(scriptdata, identifier);
                Infosys.ATR.RepositoryAccess.FileRepository.ScriptRepositoryDS scriptDocDs = new Infosys.ATR.RepositoryAccess.FileRepository.ScriptRepositoryDS();

                scriptDoc = scriptDocDs.Download(scriptDoc);

                if (scriptDoc.File != null)
                {
                    //scriptDoc.File.Seek(0, SeekOrigin.Begin);
                    //content = StreamToString(scriptDoc.File);

                    string targetPath = script.WorkingDir;
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }
                   
                    //LogEntry(string.Format("Downloading script:{0} of category:{1}", scriptdata.Name + "." + script.ScriptType, scriptdata.CategoryId));
                    string filePath = targetPath + "\\" + scriptdata.Name + "." + script.ScriptType;
                    using (FileStream destination = File.Create(filePath))
                    {
                        scriptDoc.File.Seek(0, SeekOrigin.Begin);
                        scriptDoc.File.CopyTo(destination);
                    }

                    string XMLFilePath = targetPath + "\\" + scriptdata.Name + ".xml";
                    XmlSerializer xs = new XmlSerializer(typeof(PE.Script));

                    TextWriter txtWriter = new StreamWriter(XMLFilePath);

                    xs.Serialize(txtWriter, script);

                    txtWriter.Close();
                }
                else
                {
                    LogEntry("Script cannot be downloaded");
                    throw new Exception("Script cannot be downloaded");
                }

            }
            catch (Exception ex)
            {
                string err = "There is an error downloading the Script.";
                err = err + "\nMore Infomation- " + ex.Message;
                if (ex.InnerException != null)
                    err = err + ". \nInner Exception- " + ex.InnerException.Message;
                LogEntry("Exception in GetScriptContent() Method:" + err);
                throw new Exception(err);
            }
            LogEntry("Script Content downloaded successfully");
            return content;
        }

        private static string StreamToString(Stream fileContent)
        {
            StreamReader reader = new StreamReader(fileContent);
            string fileString = reader.ReadToEnd();
            return fileString;
        }

        public static void compressFolder(string sourcePath)
        {
            try
            {
                string startPath = sourcePath;
                //string destPath = sourcePath + "D:\\archive1";
                string destPath =Path.GetDirectoryName(zipFilePath);
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string zipPath = zipFilePath;
                ZipFile.CreateFromDirectory(startPath, zipPath);
                
            }
            catch (Exception ex)
            {
                
            }
        }

        public static void ExtractZipFile(string filePath)
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(filePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        Console.WriteLine(entry.FullName);
                       // entry.ExtractToFile(Path.Combine(Path.GetDirectoryName(filePath), entry.FullName));
                    }
                }
                ZipFile.ExtractToDirectory(filePath, Path.GetDirectoryName(filePath));



            }
            catch (Exception ex)
            {
               
            }
        }
        private static void ExportScripts()
        {
            try
            {                
                ReadInputs();
                localpath = Convert.ToString(ConfigurationManager.AppSettings["LocalFilePath"]);
                Console.WriteLine(string.Format("\nLoading categories for categoryId:{0}", categoryID));
              
                
                LoadCategory(companyId, categoryID, sccServiceUrl);
                
                if (categories != null && categories.Count>0)
                {
                    categories = categories.OrderBy(c => c.Id).ToList();
                    Console.WriteLine(string.Format("\n{0} categories fetched for categoryId:{1}", categories.Count, categoryID));

                    scriptRepository.ServiceUrl = sccServiceUrl + "/" + WEMClient.Services.WEMScriptService + ".svc";
                    var scriptClient = scriptRepository.ServiceChannel;

                    foreach (PE.Category category in categories.OrderBy(c=>c.Id).ToList())
                    {
                        string targetFilePath = null;
                        PE.Category validationCategory = null;
                        do
                        {

                            var tmpCategory = validationCategory != null ? categories.Where(c => c.Id == Convert.ToString(validationCategory.ParentId)).FirstOrDefault()
                               : categories.Where(c => c.Id == Convert.ToString(category.ParentId)).FirstOrDefault();
                            validationCategory = tmpCategory != null ? tmpCategory : category;
                            targetFilePath = tmpCategory != null ? targetFilePath + "\\" + tmpCategory.Name : localpath;
                        } while (validationCategory.ParentId != Convert.ToInt32(categoryID));
                        if (!string.Equals(targetFilePath, localpath, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string[] path = targetFilePath.Split(new[] { "\\" }, StringSplitOptions.None).Where(c => c != "" || c != null).ToArray();
                            string tempPath = null;

                            if (path.Length > 0)
                            {
                                for (int i = path.Length - 1; i > 0; i--)
                                {
                                    tempPath = tempPath + "\\" + path[i];
                                }
                            }
                            targetFilePath = tempPath;
                        }
                        else
                            targetFilePath = null;

                        string ParentdestinationPath = targetFilePath != null ? localpath + "\\" + targetFilePath + "\\" + category.Name : localpath + "\\" + category.Name; ;
                        Infosys.WEM.Scripts.Service.Contracts.Message.GetAllScriptDetailsResMsg response = scriptClient.GetAllScriptDetails(category.Id);
                        List<PE.Script> scripts = Infosys.ATR.ScriptRepository.Translators.ScriptPE_SE.ScriptListSEtoPE(response.Scripts.ToList());
                        if (scripts != null)
                        {
                            foreach (PE.Script script in scripts)
                            {
                                var sourceScriptDetails = scriptClient.GetScriptDetails(script.Id.ToString(), script.CategoryId.ToString(),script.Version);
                                if (sourceScriptDetails.ScriptDetails != null)
                                {
                                    var sourceScript = Infosys.ATR.ScriptRepository.Translators.ScriptPE_SE.ScriptSEtoPE(sourceScriptDetails.ScriptDetails);
                                    sourceScript.WorkingDir = ParentdestinationPath;
                                    Console.WriteLine(string.Format("Downloading script:{0} of category:{1}", sourceScript.Name + "." + script.ScriptType, category.Name));
                                    string fileContent = GetScriptContent(sourceScript);
                                    Console.WriteLine(string.Format("Download completed for script:{0} of category:{1}", sourceScript.Name + "." + script.ScriptType, category.Name));
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("Script details not found  for script:{0} of category:{1}", script.Id , script.CategoryId));
                                }
                            }
                        }
                    }
                    Console.WriteLine(string.Format("Compressing scripts to Zip"));
                    compressFolder(localpath);
                }
                else
                {
                    Console.WriteLine(string.Format("No data found for categoryId:{0}",categoryID));
                }
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                Console.ReadLine();
            }
        }

        private static void GetAllCategories(string companyId,string sccURL)
        {
            try
            {
                //LogEntry("Entering LoadCategory Method");               
                commonRepositoryClient.ServiceUrl = sccURL + "/" + WEMClient.Services.WEMCommonService + ".svc";
                var commonChannel = commonRepositoryClient.ServiceChannel;

                Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response =
                           commonChannel.GetAllCategoriesByCompany(companyId, Infosys.ATR.ExportUtility.Constants.Application.ModuleID);

                categories = Infosys.ATR.ScriptRepository.Translators.CategoryTreePE_SE.CategoryListSEtoPE(response.Categories);
                //LogEntry("Exiting LoadCategory Method");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LoadCategory(string companyId,string categoryId,string sccURL)
        {
            try
            {
                //LogEntry("Entering LoadCategory Method");               
                commonRepositoryClient.ServiceUrl = sccURL + "/" + WEMClient.Services.WEMCommonService+".svc";                
                var commonChannel = commonRepositoryClient.ServiceChannel;                

                Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response =
                           commonChannel.GetAllCategoriesByCompanyCategoryId(companyId, Infosys.ATR.ExportUtility.Constants.Application.ModuleID, categoryId);

                categories = Infosys.ATR.ScriptRepository.Translators.CategoryTreePE_SE.CategoryListSEtoPE(response.Categories);
                //LogEntry("Exiting LoadCategory Method");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void LogEntry(string message)
        {
            DateTime dt = DateTime.Now;
            String frmdt = dt.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
            lock (lockObject)
            {
                System.IO.File.AppendAllText(Directory.GetCurrentDirectory() +DateTime.Now.ToString("dd-MMM-yyyy")+".txt", message + ", " + frmdt + Environment.NewLine);
            }
        }
    }
}
