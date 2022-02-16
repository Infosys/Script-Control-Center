using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyIntegratorService;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.WorkflowExecutionLibrary.FileRepository
{
    public class WorkflowRepositoryDS
    {
        int statusCode = 0;
        private System.IO.Stream _workflow = null;
        private string response = "";
        private string fileName = "";

        /// <summary>
        /// Download the Workflow file from the document source configured
        /// </summary>
        /// <param name="workflowRepoRequest">An object containing the request details to download a
        /// workflow file from source configured</param>
        /// <returns>An object containing the document with additional metadata</returns>
        public Entity.WorkflowDoc Download(Entity.WorkflowDoc workflowRepoRequest)
        {
            using (LogHandler.TraceOperations("FileRepository.WorkflowRepositoryDS:Download",
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
                    Uri uri = new Uri(workflowRepoRequest.StorageBaseURL);
                    dictionary.Add("RootDNS", uri.DnsSafeHost);
                    dictionary.Add("Port", uri.Port.ToString());

                    dictionary.Add("container_name", workflowRepoRequest.WorkflowContainer);
                    dictionary.Add("file_name", workflowRepoRequest.FileName);

                    // pass other parameters
                    // mandatory parameters accepted by repository service
                    dictionary.Add("workflow_ver#", workflowRepoRequest.WorkflowVer.ToString());
                    dictionary.Add("company_id", workflowRepoRequest.CompanyId);

                    LogHandler.LogDebug("Download Presentation by using httpAdapter for workflow file with name {0}",
                        LogHandler.Layer.Resource, workflowRepoRequest.FileName);

                    adapterManager.Receive(ApplicationConstants.DOCUMENTSTORE_KEY, dictionary);

                    // wait till response is received
                    arEvent.WaitOne();

                    workflowRepoRequest.File = _workflow; //_script will have the content of the file, this is populated in the event- adapterManager_ResponseReceived
                    workflowRepoRequest.StatusMessage = response;
                    workflowRepoRequest.StatusCode = statusCode;
                    workflowRepoRequest.FileName = fileName;

                    if (workflowRepoRequest.StatusCode == 0)
                    {
                        workflowRepoRequest.IsDownloadSuccessful = true;
                        LogHandler.LogDebug(
                            "FileRepository.WorkflowRepositoryDS: Document {0} successfully downloaded. The document is for workflow with container {1} .version {2} and company with Id {3}",
                            LogHandler.Layer.Resource,
                            workflowRepoRequest.FileName,
                            workflowRepoRequest.WorkflowContainer,
                            workflowRepoRequest.WorkflowVer,
                            workflowRepoRequest.CompanyId);
                    }
                    else
                    {
                        //Indicating of some error occured during upload
                        workflowRepoRequest.StatusCode = statusCode;
                        LogHandler.LogError(
                            "FileRepository.WorkflowRepositoryDS: Document {0} failed to be downloaded. The document is for workflow with container {1}, version {2} and company with Id {3}. Status code = {4}, Status Message = {5}",
                            LogHandler.Layer.Resource,
                            workflowRepoRequest.FileName,
                            workflowRepoRequest.WorkflowContainer,
                            workflowRepoRequest.WorkflowVer,
                            workflowRepoRequest.CompanyId,
                            statusCode,
                            workflowRepoRequest.StatusMessage);
                        throw new Exception("Download Failed! " + workflowRepoRequest.StatusMessage);
                    }                    
                }
                catch (Exception ex)
                {
                    workflowRepoRequest.IsDownloadSuccessful = false;
                    string error = ex.Message;
                    if (ex.InnerException != null)
                        error += ". Inner Exception- " + ex.InnerException.Message;
                    workflowRepoRequest.AnyError = error;
                }
                return workflowRepoRequest;
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
                    if (Encoding.Unicode.GetString(outStream.ToArray()).Contains(SecureHandler.SecurePayload.keyText))
                    {
                        byte[] outStreamArray = SecureHandler.SecurePayload.UnSecureBytes(outStream.ToArray());
                        _workflow = new System.IO.MemoryStream(outStreamArray);
                    }
                    else
                        _workflow = outStream;
                }
                else
                    _workflow = outStream;

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
