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
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyIntegratorService;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.SecureHandler;

namespace Infosys.ATR.RepositoryAccess.FileRepository
{
    public class ScriptRepositoryDS
    {
        int statusCode = 0;
        private System.IO.Stream _script = null;
        private string response = "";
        private string fileName = "";

        /// <summary>
        /// Download the Script file from the document source configured
        /// </summary>
        /// <param name="scriptRepoRequest">An object containing the request details to download a
        /// script file from source configured</param>
        /// <returns>An object containing the document with additional metadata</returns>
        public Entity.ScriptDoc Download(Entity.ScriptDoc scriptRepoRequest)
        {            
            using (LogHandler.TraceOperations("FileRepository.ScriptRepositoryDS:Download",
                LogHandler.Layer.Resource,System.Guid.Empty, null))
            {
                try
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

                    scriptRepoRequest.File = _script; //_script will have the content of the file, this is populated in the event- adapterManager_ResponseReceived
                    scriptRepoRequest.StatusMessage = response;
                    scriptRepoRequest.StatusCode = statusCode;
                    scriptRepoRequest.FileName = fileName;

                    if (scriptRepoRequest.StatusCode == 0)
                    {
                        scriptRepoRequest.IsDownloadSuccessful = true;
                        LogHandler.LogDebug(
                            "FileRepository.ScriptRepositoryDS: Document {0} successfully downloaded. The document is for script with container {1} .version {2} and company with Id {3}",
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
                            "FileRepository.ScriptRepositoryDS: Document {0} failed to be downloaded. The document is for script with container {1}, version {2} and company with Id {3}. Status code = {4}, Status Message = {5}",
                            LogHandler.Layer.Resource,
                            scriptRepoRequest.FileName,
                            scriptRepoRequest.ScriptContainer,
                            scriptRepoRequest.ScriptVer,
                            scriptRepoRequest.CompanyId,
                            statusCode,
                            scriptRepoRequest.StatusMessage);
                        throw new Exception("Download Failed! " + scriptRepoRequest.StatusMessage);
                    }                    
                }
                catch (Exception ex)
                {
                    scriptRepoRequest.IsDownloadSuccessful = false;
                    string error = ex.Message;
                    if (ex.InnerException != null)
                        error += ". Inner Exception- " + ex.InnerException.Message;
                    scriptRepoRequest.AnyError = error;
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
                if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                {
                    if (Encoding.Unicode.GetString(outStream.ToArray()).Contains(SecurePayload.keyText))
                    {
                        byte[] outStreamArray = SecurePayload.UnSecureBytes(outStream.ToArray());
                        _script = new System.IO.MemoryStream(outStreamArray);
                    }
                    else
                        _script = outStream;
                }
                else
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


        /// <summary>
        /// Input the secure Workflow file stream from the document source configured
        /// </summary>
        /// <param name="SecureStream">An object containing the Secure workflow file Stream 
        /// from source configured</param>
        /// <returns>An object containing the Unsecure/decrypted stream </returns>
        public System.IO.Stream GetUnSecureStream(System.IO.Stream SecureStream)
        {
            try
            {
                System.IO.MemoryStream outStream = new System.IO.MemoryStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;

                while ((bytesRead = SecureStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    outStream.Write(buffer, 0, bytesRead);
                }

                if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                {
                    if (Encoding.Unicode.GetString(outStream.ToArray()).Contains(SecurePayload.keyText))
                    {
                        byte[] outStreamArray = SecurePayload.UnSecureBytes(outStream.ToArray());
                        _script = new System.IO.MemoryStream(outStreamArray);
                    }
                    else
                        _script = outStream;
                }
                else
                    _script = outStream;
            }
            catch
            {
                _script = null;
            }
            return _script;
        }
    }
}
