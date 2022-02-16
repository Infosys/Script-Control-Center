/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.DirectoryServices;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Configuration;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.SecurityAccess.Contracts.Message;
using Infosys.WEM.SecurityAccess.Contracts;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Security.Principal;
using Infosys.IAP.Infrastructure.Common.HttpModule;
using Infosys.IAP.Infrastructure.Common.HttpModule.Entity;
//using Dataccess = Infosys.PPTWare.Resource.DataAccess.SQL;
//using Dataentity = Infosys.PPTWare.Resource.Entities.SQL;

namespace Infosys.IAP.Infrastructure.ApplicationCore.HttpModule
{
    public class Document : IHttpModule
    {
        private const string APPLICATION_TYPE = "lif_document_handler_as_blob";
        private string responseString = "File successfully uploaded.";
        private const string companyPK = "PPTWare";
        private string jsAnalyticTemplate = "<script> var imgTag = document.createElement('img');imgTag.src = \"{0}/Content/tracker/slview.gif?tmp=\"+ new Date().toString();document.body.appendChild(imgTag); </script>";
        //Sid: Set default behavior to false
        //This was done so that our module does not override standard IIS response behaviors
        private bool isSuccess = false;

        private string deploymentUrl = "";

        private byte[] DataStream = default(byte[]);

        #region IHttpModule Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
        public String ModuleName
        {
            get { return "DocHttpModule"; }
        }
        public void Init(HttpApplication context)
        {
            context.AuthorizeRequest += AuthorizeRequest;
            context.PostAuthorizeRequest += new EventHandler(context_PostAuthorizeRequest);
            context.EndRequest += new EventHandler(context_EndRequest);

        }
        void context_EndRequest(object sender, EventArgs e)
        {
            LogHandler.LogDebug("Store context_EndRequest Entered", LogHandler.Layer.Infrastructure);
            HttpContext context = ((HttpApplication)sender).Context;

            //Sid: Fixed the bug (BUG6765) of IE8 unable to download the presentation file
            //Did not include check if the request if of type 201. All HTTPRequest were responded with 201
            // As a result the browser was unable to understand that this was a fetch request and 
            //was expecting the documents to be uploaded
            if (context.Request.RequestType == "POST" && isSuccess)
            {
                //Sid: Moved response string as incorrect message was being send out in case of exception or other 
                //requests
                context.Response.StatusDescription = responseString;
                context.Response.StatusCode = 201;
                //context.Response.Status = "OK";
            }
            //Sid: Fixed the bug (BUG6765) of IE8 unable to download the presentation file.
            //The following lines had to be commented so that our module does not
            //override standard IIS response behaviors
            //else
            //{
            //    context.Response.StatusCode = 500;
            //}
            else if (context.Request.RequestType == "GET" && isSuccess)
            {
                //get the slide url part
                string[] urlParts = context.Request.Url.AbsoluteUri.Split('/');
                if (urlParts.Length > 0)
                {
                    string fileName = urlParts[urlParts.Length - 1];
                    System.Text.RegularExpressions.Regex slideurlEx = new System.Text.RegularExpressions.Regex(@"slide\d{4}.htm");
                    System.Text.RegularExpressions.Regex ppturlEx = new System.Text.RegularExpressions.Regex(@"pptware_[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}.htm");

                    string containerName = urlParts[urlParts.Length - 2];
                    string documentsVDFromRoot = urlParts[urlParts.Length - 3];

                    if (ConfigurationManager.ConnectionStrings["ContentStoreEntities"] != null)
                    {
                        ContentDetails contentDetails = new ContentDetails()
                        {
                            FilePath = fileName,
                            CreatedBy = context.User.Identity.Name,
                            containerName = containerName,
                            documentsVDFromRoot = documentsVDFromRoot
                        };

                        var ContentDetails = FileRepository.GetFileDetails(contentDetails);
                        if (ContentDetails != null)
                            if (ContentDetails.FileContent != null)
                                DataStream = ContentDetails.FileContent;

                        if (DataStream != null)
                        {
                            context.Response.OutputStream.Write(DataStream, 0, DataStream.Length);
                            context.Response.StatusDescription = "File successfully downloaded";
                            context.Response.StatusCode = 200;
                        }
                    }

                    //if (slideurlEx.IsMatch(fileName) || ppturlEx.IsMatch(fileName))
                    //{
                    //    LogHandler.LogDebug("PPTWare Document handler HTTP module- injecting script for analytics", LogHandler.Layer.Infrastructure);
                    //    if (string.IsNullOrEmpty(deploymentUrl))
                    //    {
                    //        //then get the url for the company
                    //        string CompanyId = ConfigurationManager.AppSettings["DefaultCompanyId"];
                    //        Dataccess.DataSources.CompaniesDS companyDs = new Dataccess.DataSources.CompaniesDS();
                    //        Dataentity.Companies company = companyDs.GetOne(new Dataentity.Companies() { PartitionKey = companyPK, RowKey = CompanyId });
                    //        if (company != null)
                    //            deploymentUrl = company.DeploymentBaseUrl;
                    //    }
                    //    jsAnalyticTemplate = string.Format(jsAnalyticTemplate, deploymentUrl);
                    //    context.Response.Write(jsAnalyticTemplate);
                    //}
                }
            }
            // it seems the httpmodule instance is cached in the apppool. Hence the authentication event was not getting fired on each request
            //hence resetting success flag so that the authentication event is re-fired. Refer:http://somenewkid.blogspot.in/2006/03/trouble-with-modules.html
            this.isSuccess = false;
            LogHandler.LogDebug("Store context_EndRequest Exited", LogHandler.Layer.Infrastructure);

        }
        void context_PostAuthorizeRequest(object sender, EventArgs e)
        {
            //Sid: Removed eventlog entries. To be update using EL Log calls
            LogHandler.LogDebug("Store handler Enter postAuthorize request", LogHandler.Layer.Infrastructure);

            WEMValidationException validateException = new WEMValidationException(ErrorMessages.InvalidCharacter_Validation.ToString());
            List<ValidationError> validateErrs = new List<ValidationError>();

            HttpContext context = ((HttpApplication)sender).Context;
            bool auth = context.User.Identity.IsAuthenticated;
            string usr = context.User.Identity.Name;
            if (context.Request.RequestType == "POST")
            {                 
                if (context.Request.Headers.AllKeys.Contains("application_type") && DecodeBase64String(context.Request.Headers["application_type"]) == APPLICATION_TYPE)
                {

                    //Sid: Removed eventlog entries. To be update using EL Log calls
                    LogHandler.LogDebug("Store handler HTTP module called for upload by LIF IISDoucment upload client",
                        LogHandler.Layer.Infrastructure);

                    try
                    {   
                        string containerName = DecodeBase64String(context.Request.Headers["container_name"]);
                        string fileName = DecodeBase64String(context.Request.Headers["file_name"]);

                        if (ValidationUtility.InvalidCharValidatorForFile(Path.GetFileNameWithoutExtension(fileName)))
                        {
                            ValidationError validationErr = new ValidationError();
                            validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                            validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, Path.GetFileNameWithoutExtension(fileName), "Workflow.fileName");
                            validateErrs.Add(validationErr);
                            validateException.Data.Add("ValidationErrors", validateErrs);
                            throw validateException;
                        }

                        string documentsVDFromRoot = DecodeBase64String(context.Request.Headers["documents_VD_from_Root"]);
                        //upload the file to the so-formed virtual directory
                        Stream inFileStream = context.Request.InputStream;

                        if (ConfigurationManager.ConnectionStrings["ContentStoreEntities"] != null)
                        {
                            LogHandler.LogDebug("Store postauthorizerequest enter db store handling for storage", LogHandler.Layer.Infrastructure);
                            System.IO.MemoryStream outStream = new System.IO.MemoryStream();
                            byte[] _buffer = new byte[1024];
                            int _bytesRead = 0;
                            while ((_bytesRead = inFileStream.Read(_buffer, 0, _buffer.Length)) != 0)
                                outStream.Write(_buffer, 0, _bytesRead);

                            int version = (documentsVDFromRoot.StartsWith("iapscriptstore")) ?
                                                            int.Parse(DecodeBase64String(context.Request.Headers["script_ver#"])) :
                                                                    int.Parse(DecodeBase64String(context.Request.Headers["workflow_ver#"]));

                            ContentDetails contentDetails = new ContentDetails()
                            {
                                CompanyId = int.Parse(DecodeBase64String(context.Request.Headers["company_id"])),
                                FilePath = fileName,
                                Version = version,
                                CreatedBy = context.User.Identity.Name,
                                FileContent = outStream.ToArray(),
                                containerName = containerName,
                                documentsVDFromRoot = documentsVDFromRoot
                            };

                            if(!FileRepository.UploadFileDetails(contentDetails))
                                throw new HttpException(500, "Internal Server Error");

                            LogHandler.LogDebug("Store postauthorizerequest  db store handling for storage. File Uploaded!", LogHandler.Layer.Infrastructure);
                        }
                        else
                        {

                        
                        string siteId = context.Request.ServerVariables["INSTANCE_ID"];
                        String strPath = string.Format(@"IIS://localhost/W3SVC/{0}/Root/{1}",siteId,documentsVDFromRoot); //Documents;
                        LogHandler.LogDebug("Store postauthorizerequest enter file store handling for storage." + strPath , LogHandler.Layer.Infrastructure);
                        DirectoryEntry documentsVD = new DirectoryEntry(strPath);

                        //get the physical path
                        string documentsRootPath = (string)documentsVD.Properties["Path"][0];

                        //check if the physical path for the new VD already exists
                        //and create the physical path if doesnot exist
                        string newVDPath = documentsRootPath + @"\" + containerName;
                        if (!System.IO.Directory.Exists(newVDPath))
                        {
                            System.IO.Directory.CreateDirectory(newVDPath);
                        }

                        //create the virtual directory if alreday doesnot exist
                        DirectoryEntries childVDs = documentsVD.Children;
                        //IEnumerable<DirectoryEntry> matchingVDs = childVDs.Cast<DirectoryEntry>().Where(v => v.Name == containerName);
                        IEnumerable<DirectoryEntry> matchingVDs = childVDs.Cast<DirectoryEntry>().Where(v => v.Name.ToLower() == containerName.ToLower());
                        if (matchingVDs == null || (matchingVDs != null && matchingVDs.Count() <= 0))
                        {
                            DirectoryEntry myDirectoryEntry = childVDs.Add(containerName, documentsVD.SchemaClassName);
                            myDirectoryEntry.Properties["Path"][0] = newVDPath;
                            myDirectoryEntry.Properties["AppFriendlyName"][0] = containerName;
                            myDirectoryEntry.CommitChanges();
                        }

                        //check if the file name has folder structure, then create the folder structure and then stream the file
                        if (fileName.Contains('/'))
                        {
                            string[] filePathParts = fileName.Split('/');
                            string folderStructure = fileName.Substring(0, fileName.IndexOf(filePathParts[filePathParts.Length - 1])).Replace('/', '\\');
                            //create the complete path
                            folderStructure = newVDPath + "\\" + folderStructure;

                            //check for value invalidcharacters
                            

                            if (ValidationUtility.InvalidCharValidatorForFolderPath(folderStructure))
                            {
                                ValidationError validationErr = new ValidationError();
                                validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                                validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, folderStructure, "Workflow.folderStructure");
                                validateErrs.Add(validationErr);
                                validateException.Data.Add("ValidationErrors", validateErrs);
                                throw validateException;
                            }

                            if (!System.IO.Directory.Exists(folderStructure))
                            {
                                System.IO.Directory.CreateDirectory(folderStructure);
                            }

                            newVDPath = folderStructure;
                            fileName = filePathParts[filePathParts.Length - 1];

                            if (ValidationUtility.InvalidCharValidatorForFile(Path.GetFileNameWithoutExtension(fileName)))
                            {
                                ValidationError validationErr = new ValidationError();
                                validationErr.Code = Errors.ErrorCodes.InvalidCharacter_Validation.ToString();
                                validationErr.Description = string.Format(ErrorMessages.InvalidCharacter_Validation, fileName, "Workflow.fileName");
                                validateErrs.Add(validationErr);
                                validateException.Data.Add("ValidationErrors", validateErrs);
                                throw validateException;
                            }

                        }

                            FileStream outFileStream = null;

                        //appending as the program doesnot allow to overwrite files but create new version . this makes it easy to find if the file is being tampered
                            if (!File.Exists(newVDPath + @"\" + fileName))
                            outFileStream = new FileStream(newVDPath + @"\" + fileName, FileMode.Create, FileAccess.Write);
                        else
                                outFileStream = new FileStream(newVDPath + @"\" + fileName, FileMode.Append, FileAccess.Write);

                        //check if the header having the block size is provided
                        //default is 1 KB i.e. 1024 bytes
                        int blockSize = 1;
                        if (context.Request.Headers["block_size"] != null && int.Parse(DecodeBase64String(context.Request.Headers["block_size"])) > 0)
                            blockSize = int.Parse(DecodeBase64String(context.Request.Headers["block_size"]));
                        int blockSizeInBytes = blockSize * 1024;
                        byte[] buffer = new byte[blockSizeInBytes];
                        int bytesRead = 0;
                        while ((bytesRead = inFileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            outFileStream.Write(buffer, 0, bytesRead);
                        }
                        outFileStream.Close();
                        LogHandler.LogDebug("Store postauthorizerequest file store handling for storage. File Uploaded!", LogHandler.Layer.Infrastructure);
                        }
                        inFileStream.Close();
                        //end the request
                        isSuccess = true;
                        LogHandler.LogDebug("Store postauthorizerequest completed successfully!", LogHandler.Layer.Infrastructure);
                        ((HttpApplication)sender).CompleteRequest();
                    }
                    catch (Exception ex)
                    {
                        responseString = ex.Message;
                        isSuccess = false;
                        if (ex.InnerException != null)
                        {
                            responseString = responseString + ". Inner Error Message- " + ex.InnerException.Message;
                        }

                        //EventLog eventLog = new EventLog();
                        // eventLog = new EventLog();
                        //eventLog.Source = "Document Upload Http Module";
                        //eventLog.WriteEntry(responseString, EventLogEntryType.Error);

                        //Sid: Removed eventlog entries. To be update using EL Log calls
                        LogHandler.LogError("PPTWare Document handler failed with message: {0} ",
                            LogHandler.Layer.Infrastructure, responseString);
                    }
                }
            }
            else if (context.Request.RequestType == "GET")
            {
                isSuccess = true;    
            }
        }
        string DecodeBase64String(string EncodeString) 
        {
            string prefix = "=?utf-8?B?";
            string fulldelimit = prefix + "?=";
            string base64EncodedString = EncodeString.Substring((prefix.Length), (EncodeString.Length - fulldelimit.Length));
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedString);
            return System.Text.Encoding.Unicode.GetString(base64EncodedBytes);
        }
        private string EncodeStringToBase64(string key)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.Unicode.GetBytes(key);
            string value = System.Convert.ToBase64String(toEncodeAsBytes);
            value = "=?utf-8?B?" + value + "?=";
            return value;
        }
        private void AuthorizeRequest(object sender, EventArgs e)
        {
            var currentUser = HttpContext.Current.User;
            LogHandler.LogDebug("Store postauthorizerequest enter AuthorizeRequest.", LogHandler.Layer.Infrastructure);
            if (currentUser.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserName = currentUser.Identity.Name;
                    if (currentUser.Identity.Name.Contains("\\"))
                        currentUserName = currentUserName.Split(new string[] { "\\" }, StringSplitOptions.None).GetValue(1).ToString();

                    if (HttpContext.Current.Request.Headers["company_id"] != null)
                    {
                        int keyIndex=0;
                        string secureK = DateTime.Now.Ticks.ToString();
                        IsSuperAdminResMsg isSA = ServiceChannel.IsSuperAdmin(Infosys.WEM.Infrastructure.SecurityCore.SecureData.Secure(currentUserName, ApplicationConstants.SECURE_PASSCODE),
                            DecodeBase64String(HttpContext.Current.Request.Headers["company_id"]));
                        if (!isSA.IsSuperAdmin)
                        {
                            GetAllUsersResMsg allUsers = ServiceChannel.GetUsers(Infosys.WEM.Infrastructure.SecurityCore.SecureData.Secure(currentUserName, ApplicationConstants.SECURE_PASSCODE),
                            DecodeBase64String(HttpContext.Current.Request.Headers["company_id"]));

                        if (allUsers == null)
                            throw new HttpException(401, "Unauthorized access");
                    }
                    }
                    LogHandler.LogDebug("Store postauthorizerequest enter AuthorizeRequest. User Authenticated and Authorized!", LogHandler.Layer.Infrastructure);

                }
                catch (Exception ex)
                {   
                    string responseString = ex.Message;
                    isSuccess = false;
                    if (ex.InnerException != null)
                        responseString = responseString + ". Inner Error Message- " + ex.InnerException.Message;
                    
                    LogHandler.LogError("PPTWare Document handler failed with message: {0} ",
                        LogHandler.Layer.Infrastructure, responseString);

                    throw;
                }               
            }
            else
                throw new HttpException(401, "Unauthorized access");
        }
        public ISecurityAccess ServiceChannel
        {
            get
            {
                string deploymentBaseUrl = Convert.ToString(ConfigurationManager.AppSettings["ServiceBaseUrl"]);
                string _serviceUrl = string.Format("{0}/iapwemservices/WEMSecurityAccessService.svc",deploymentBaseUrl);
                Uri serviceAddress = new Uri(_serviceUrl);
                WebHttpBinding serviceBinding = new WebHttpBinding();
                if (serviceAddress.Scheme.Equals(Uri.UriSchemeHttps))
                    serviceBinding.Security.Mode = WebHttpSecurityMode.Transport;
                else
                    serviceBinding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                serviceBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                WebChannelFactory<ISecurityAccess> securityAccessChannel = new WebChannelFactory<ISecurityAccess>(serviceBinding, serviceAddress);
                return securityAccessChannel.CreateChannel();
            }
        }
        #endregion
        
    }
}
