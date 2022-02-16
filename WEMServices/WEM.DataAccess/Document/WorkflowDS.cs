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

namespace Infosys.WEM.Resource.DataAccess.Document
{

    /// <summary>
    /// Presentation files will be uploaded to and downloaded from the conifgured document source
    /// </summary>
    public class WorkflowDS : IDocument<DocumentEntity.Workflow>
    {
        string SuccessMessage = "Document successfully uploaded.";
        int statusCode = 0;
        private System.IO.Stream _presentation = null;
        private string response = "";
        private string fileName = "";
        /// <summary>
        /// Upload the Workflow to the document source configured
        /// </summary>
        /// <param name="workflowRequest">Workflow object containing the document with additional metadata
        /// </param>
        /// <returns>Status of operation</returns>
        public DocumentEntity.Workflow Upload(DocumentEntity.Workflow workflowRequest)
        {
            using (LogHandler.TraceOperations("Document.PresentationDS:Upload",
               LogHandler.Layer.Resource,System.Guid.Empty, null))
            {
                AdapterManager adapterManager = new AdapterManager();

                string msgResponse = "";

                NameValueCollection dictionary = new NameValueCollection();

                // Parse CompanyBaseURL and return only the DNS host name
                // For IIS the IISAdapter will construct the complete URL
                Uri uri = new Uri(workflowRequest.StorageBaseURL);
                dictionary.Add("UriScheme", uri.Scheme);
                dictionary.Add("RootDNS", uri.DnsSafeHost);
                dictionary.Add("Port", uri.Port.ToString());

                dictionary.Add("container_name", workflowRequest.WorkflowId);
                dictionary.Add("file_name", workflowRequest.FileName);

                // pass other parameters
                dictionary.Add("workflow_id", workflowRequest.WorkflowId);
                dictionary.Add("workflow_ver#", workflowRequest.WorkflowVer.ToString());
                dictionary.Add("company_id", workflowRequest.CompanyId.ToString());


                if (!string.IsNullOrWhiteSpace(workflowRequest.UploadedBy))
                    dictionary.Add("uploaded_by", workflowRequest.UploadedBy);


                //Post message
                LogHandler.LogDebug(
                    "Document.WorkflowDS: Document {0} message to be posted. The document is for workflow with id {1} and Ver {2} for company with Id {3}",
                    LogHandler.Layer.Resource,
                    workflowRequest.FileName,
                    workflowRequest.WorkflowId,
                    workflowRequest.WorkflowVer,
                    workflowRequest.CompanyId);

                msgResponse = adapterManager.Execute(workflowRequest.File,
                    ApplicationConstants.DOCUMENTSTORE_KEY, dictionary);

                workflowRequest.StatusMessage = msgResponse;

                if (workflowRequest.StatusMessage == SuccessMessage)
                {
                    workflowRequest.StatusCode = 0;
                    LogHandler.LogDebug(
                        "Document.PresentationDS: Document {0} message successfully posted. The document is for workflow with id {1} and Ver {2} for company with Id {2}",
                        LogHandler.Layer.Resource,
                        workflowRequest.FileName,
                        workflowRequest.WorkflowId,
                        workflowRequest.WorkflowVer,
                        workflowRequest.CompanyId);
                }
                else
                {
                    //Indicating of some error occured during upload
                    workflowRequest.StatusCode = -1;
                    LogHandler.LogError(
                        "Document.PresentationDS: Document {0} message failed to be uploaded. The document is for workflow with id {1} and Ver {2} for company with Id {3}. Failure Message- {4}",
                        LogHandler.Layer.Resource,
                        workflowRequest.FileName,
                        workflowRequest.WorkflowId,
                        workflowRequest.WorkflowVer,
                        workflowRequest.CompanyId,
                        workflowRequest.StatusMessage);
                }

                return workflowRequest;
            }
        }


        /// <summary>
        /// Download the Workflow from the document source configured
        /// </summary>
        /// <param name="workflowRequest">Workflow object containing the request details to download a
        /// presentation file from source configured</param>
        /// <returns>Infosys.PPTWare.Resource.Entities.Document.Presentation Workflow object containing the
        /// document with additional metadata</returns>
        public DocumentEntity.Workflow Download(DocumentEntity.Workflow workflowRequest)
        {
            using (LogHandler.TraceOperations("Document.WorkflowDS:Download",
                LogHandler.Layer.Resource,System.Guid.Empty, null))
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
                Uri uri = new Uri(workflowRequest.StorageBaseURL);
                dictionary.Add("UriScheme", uri.Scheme);
                dictionary.Add("RootDNS", uri.DnsSafeHost);
                dictionary.Add("Port", uri.Port.ToString());

                dictionary.Add("container_name", workflowRequest.WorkflowId);
                dictionary.Add("file_name", workflowRequest.FileName);

                // pass other parameters
                // mandatory parameters accepted by repository service
                dictionary.Add("workflow_id", workflowRequest.WorkflowId);
                dictionary.Add("workflow_ver#", workflowRequest.WorkflowVer.ToString());
                dictionary.Add("company_id", workflowRequest.CompanyId.ToString());

                LogHandler.LogDebug("Download Presentation by using httpAdapter for workflowid_version# with id {0}_{1}",
                    LogHandler.Layer.Resource, workflowRequest.WorkflowId,workflowRequest.WorkflowVer);

                adapterManager.Receive(ApplicationConstants.DOCUMENTSTORE_KEY, dictionary);

                // wait till response is received
                arEvent.WaitOne();

                workflowRequest.File = _presentation;
                workflowRequest.StatusMessage = response;
                workflowRequest.StatusCode = statusCode;
                workflowRequest.FileName = fileName;

                if (workflowRequest.StatusCode == 0)
                {
                    LogHandler.LogDebug(
                        "Document.PresentationDS: Document {0} message successfully downloaded. The document is for workflowid_version#  {1}_{2} for company with Id {3}",
                        LogHandler.Layer.Resource,
                        workflowRequest.FileName,
                        workflowRequest.WorkflowId,
                         workflowRequest.WorkflowVer,
                        workflowRequest.CompanyId);
                }
                else
                {
                    //Indicating of some error occured during upload
                    workflowRequest.StatusCode = statusCode;
                    LogHandler.LogDebug(
                    "Document.PresentationDS: Document {0} message failed to be downloaded. The document is for workflowid_versionid  {1}_{2} for company with Id {3}",
                        LogHandler.Layer.Resource,
                        workflowRequest.FileName,
                        workflowRequest.WorkflowId,
                        workflowRequest.WorkflowVer,
                        workflowRequest.CompanyId);
                    LogHandler.LogError(
                        "Document.PresentationDS: Document {0} message failed to be downloaded. The document is for workflowid_versionid  {1}_{2} for company with Id {3}. Status code = {4}, Status Message = {5}",
                        LogHandler.Layer.Resource,
                        workflowRequest.FileName,
                        workflowRequest.WorkflowId,
                        workflowRequest.WorkflowVer,
                        workflowRequest.CompanyId,
                        statusCode,
                        workflowRequest.StatusMessage);
                }
                return workflowRequest;
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

                _presentation = outStream;

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
