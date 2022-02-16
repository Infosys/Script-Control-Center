/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyIntegratorService;
using Infosys.WEM.Resource.IDataAccess;
using DocumentEntity = Infosys.WEM.Resource.Entity.Document;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.Scripts.Resource.DataAccess.Document
{
    public class ScriptRepositoryDS
    {
        string SuccessMessage = "Document successfully uploaded.";
        int statusCode = 0;
        private System.IO.Stream _script = null;
        private string response = "";
        private string fileName = "";
        /// <summary>
        /// Upload the Script to the document source configured
        /// </summary>
        /// <param name="scriptRepoRequest">Script object containing the document with additional metadata
        /// </param>
        /// <returns>Status of operation</returns>
        public DocumentEntity.Script Upload(DocumentEntity.Script scriptRepoRequest)
        {
            using (LogHandler.TraceOperations("Document.ScriptRepositoryDS:Upload",
               LogHandler.Layer.Resource,System.Guid.Empty, null))
            {
                AdapterManager adapterManager = new AdapterManager();

                string msgResponse = "";

                NameValueCollection dictionary = new NameValueCollection();

                // Parse CompanyBaseURL and return only the DNS host name
                // For IIS the IISAdapter will construct the complete URL
                Uri uri = new Uri(scriptRepoRequest.StorageBaseURL);
                dictionary.Add("UriScheme", uri.Scheme);
                dictionary.Add("RootDNS", uri.DnsSafeHost);
                dictionary.Add("Port", uri.Port.ToString());
                dictionary.Add("UriScheme", uri.Scheme);
                dictionary.Add("container_name", scriptRepoRequest.ScriptContainer);
                dictionary.Add("file_name", scriptRepoRequest.FileName);

                // pass other parameters
                dictionary.Add("script_ver#", scriptRepoRequest.ScriptVer.ToString());
                dictionary.Add("company_id", scriptRepoRequest.CompanyId);


                if (!string.IsNullOrWhiteSpace(scriptRepoRequest.UploadedBy))
                    dictionary.Add("uploaded_by", scriptRepoRequest.UploadedBy);


                //Post message
                LogHandler.LogDebug(
                    "Document.ScriptRepositoryDS: Document {0} message to be posted. The document is for script with container {1} and Ver {2} for company with Id {3}",
                    LogHandler.Layer.Resource,
                    scriptRepoRequest.FileName,
                    scriptRepoRequest.ScriptContainer,
                    scriptRepoRequest.ScriptVer,
                    scriptRepoRequest.CompanyId);

                msgResponse = adapterManager.Execute(scriptRepoRequest.File,
                    ApplicationConstants.SCRIPT_STORE_KEY, dictionary);

                scriptRepoRequest.StatusMessage = msgResponse;

                if (scriptRepoRequest.StatusMessage == SuccessMessage)
                {
                    scriptRepoRequest.StatusCode = 0;
                    LogHandler.LogDebug(
                        "Document.ScriptRepositoryDS: Document {0} successfully posted. The document is for script with container {1} and Ver {2} for company with Id {2}",
                        LogHandler.Layer.Resource,
                        scriptRepoRequest.FileName,
                        scriptRepoRequest.ScriptContainer,
                        scriptRepoRequest.ScriptVer,
                        scriptRepoRequest.CompanyId);
                }
                else
                {
                    //Indicating of some error occured during upload
                    scriptRepoRequest.StatusCode = -1;
                    LogHandler.LogError(
                        "Document.ScriptRepositoryDS: Document {0} failed to be uploaded. The document is for script with container {1} and Ver {2} for company with Id {3}. Failure Message- {4}",
                        LogHandler.Layer.Resource,
                        scriptRepoRequest.FileName,
                        scriptRepoRequest.ScriptContainer,
                        scriptRepoRequest.ScriptVer,
                        scriptRepoRequest.CompanyId,
                        scriptRepoRequest.StatusMessage);
                }
                //scriptRepoRequest.ScriptUrl = scriptRepoRequest.StorageBaseURL + "/iapscriptstore/" + scriptRepoRequest.ScriptContainer + "/" + scriptRepoRequest.FileName;
                scriptRepoRequest.ScriptUrl = "/iapscriptstore/" + scriptRepoRequest.ScriptContainer + "/" + scriptRepoRequest.FileName;
                return scriptRepoRequest;
            }
        }


        /// <summary>
        /// Download the Script file from the document source configured
        /// </summary>
        /// <param name="scriptRepoRequest">An object containing the request details to download a
        /// script file from source configured</param>
        /// <returns>An object containing the document with additional metadata</returns>
        public DocumentEntity.Script Download(DocumentEntity.Script scriptRepoRequest)
        {
            using (LogHandler.TraceOperations("Document.ScriptRepositoryDS:Download",
                LogHandler.Layer.Resource, System.Guid.Empty,null))
            {
                AutoResetEvent arEvent = new AutoResetEvent(false);

                AdapterManager adapterManager = new AdapterManager();

                // initialize adapter manager - entityName is the region name in LISetting.Config file
                adapterManager.ResponseReceived +=
                    new AdapterManager.AdapterReceiveHandler((ea) => adapterManager_ResponseReceived(ea, arEvent));

                NameValueCollection dictionary = new NameValueCollection();

                //Parse CompanyBaseURL and return only the DNS name
                //For IIS the IISAdapter will construct the complete URL
                //dictionary.Add("RootDNS", new Uri(presentationRequest.CompanyBaseURL).DnsSafeHost);
                Uri uri = new Uri(scriptRepoRequest.StorageBaseURL);
                dictionary.Add("UriScheme", uri.Scheme);
                dictionary.Add("RootDNS", uri.DnsSafeHost);
                dictionary.Add("Port", uri.Port.ToString());

                dictionary.Add("container_name", scriptRepoRequest.ScriptContainer);
                dictionary.Add("file_name", scriptRepoRequest.FileName);

                // pass other parameters
                // mandatory parameters accepted by repository service
                dictionary.Add("script_ver#", scriptRepoRequest.ScriptVer.ToString());
                dictionary.Add("company_id", scriptRepoRequest.CompanyId);

                LogHandler.LogDebug("Download Presentation by using httpAdapter for script file with name {0}",
                    LogHandler.Layer.Resource, scriptRepoRequest.FileName);

                adapterManager.Receive(ApplicationConstants.SCRIPT_STORE_KEY, dictionary);

                // wait till response is received
                arEvent.WaitOne();

                scriptRepoRequest.File = _script; //_script wil have the content of the file, this is populated in the event- adapterManager_ResponseReceived
                scriptRepoRequest.StatusMessage = response;
                scriptRepoRequest.StatusCode = statusCode;
                scriptRepoRequest.FileName = fileName;

                if (scriptRepoRequest.StatusCode == 0)
                {
                    LogHandler.LogDebug(
                        "Document.ScriptRepositoryDS: Document {0} successfully downloaded. The document is for script with container {1} .version {2} and company with Id {3}",
                        LogHandler.Layer.Resource,
                        scriptRepoRequest.FileName,
                        scriptRepoRequest.ScriptContainer,
                        scriptRepoRequest.ScriptVer,
                        scriptRepoRequest.CompanyId);
                }
                else
                {
                    //Indicating of some error occured during upload
                    scriptRepoRequest.StatusCode = statusCode;
                    LogHandler.LogError(
                        "Document.ScriptRepositoryDS: Document {0} failed to be downloaded. The document is for script with container {1}, version {2} and company with Id {3}. Status code = {4}, Status Message = {5}",
                        LogHandler.Layer.Resource,
                        scriptRepoRequest.FileName,
                        scriptRepoRequest.ScriptContainer,
                        scriptRepoRequest.ScriptVer,
                        scriptRepoRequest.CompanyId,
                        statusCode,
                        scriptRepoRequest.StatusMessage);
                }
                return scriptRepoRequest;
            }
        }
        
        void adapterManager_ResponseReceived(ReceiveEventArgs eventArgs, AutoResetEvent arEvent)
        {
            statusCode = (int)eventArgs.ResponseDetails["StatusCode"];
            LogHandler.LogDebug(
                "Download File (adapterManager_ResponseReceived) by using httpAdapter called. Status code = {0}",
                    LogHandler.Layer.Resource, statusCode);
            if (statusCode == 0)
            {
                LogHandler.LogDebug(
                 "Download File (adapterManager_ResponseReceived) by using httpAdapter called. Success status code",
                    LogHandler.Layer.Resource, statusCode);
                fileName = eventArgs.ResponseDetails["FileName"] as string;

                System.IO.Stream presentation = eventArgs.ResponseDetails["DataStream"] as System.IO.Stream;

                System.IO.MemoryStream outStream = new System.IO.MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = presentation.Read(buffer, 0, buffer.Length)) != 0)
                {
                    outStream.Write(buffer, 0, bytesRead);
                }

                presentation.Close();

                _script = outStream;

                response = eventArgs.ResponseDetails["Response"] as string;

                if (string.IsNullOrWhiteSpace(response) || string.IsNullOrWhiteSpace(fileName))
                {
                    LogHandler.LogWarning("Invalid response received from the server", LogHandler.Layer.Resource);
                    statusCode = -1;
                    response = "Invalid response received from the server";
                }
            }
            else
            {
                LogHandler.LogDebug(
                    "Download File (adapterManager_ResponseReceived) by using httpAdapter called. Failed status code",
                     LogHandler.Layer.Resource, statusCode);
                response = eventArgs.ResponseDetails["Response"] as string;
            }

            LogHandler.LogDebug(
                "Download File (adapterManager_ResponseReceived) by using httpAdapter call completed",
                LogHandler.Layer.Resource);

            // signal that response has been received
            arEvent.Set();
        }
    }
}
