/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.ServiceModel.Web;
using System.Configuration;
using System.Net;
using System.IO;
using Infosys.WEM.SecureHandler;
using Infosys.ATR.WFDesigner.Entities;
using Infosys.WEM.Service.Contracts;
using Infosys.WEM.Service.Contracts.Message;
using WEMClient = Infosys.WEM.Client;
using CommonContracts = Infosys.WEM.Service.Common.Contracts.Message;
using Infosys.WEM.Node.Service.Contracts.Message;

namespace Infosys.ATR.WFDesigner.Services
{


    internal class WFService
    {


        WFService()
        {

        }


        internal static CommonContracts.GetCompanyResMsg GetCompanyDetails(int companyid)
        {

            CommonContracts.GetCompanyResMsg responseObj = null;

            WEMClient.CommonRepository commonRepoClient = new WEMClient.CommonRepository();
            responseObj = commonRepoClient.ServiceChannel.GetCompanyDetails(companyid.ToString());
            return responseObj;
        }

        internal static Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg GetAllCategoriesByCompany(int companyid)
        {
            try
            {
                Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response = null;
                WEMClient.CommonRepository commonRepoClient = new WEMClient.CommonRepository();
                response = commonRepoClient.ServiceChannel.GetAllCategoriesByCompany(companyid.ToString(), Constants.Application.ModuleID);
                return response;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        internal static Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg GetAllCategoriesWithData(int companyid)
        {
            try
            {
                Infosys.WEM.Service.Common.Contracts.Message.GetAllCategoriesResMsg response = null;
                WEMClient.CommonRepository commonRepoClient = new WEMClient.CommonRepository();
                response = commonRepoClient.ServiceChannel.GetAllCategoriesWithData(companyid.ToString(), Constants.Application.ModuleID);
                return response;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        internal static WorkflowPE Publish(PublishReqMsg request)
        {
            //using (WebChannelFactory<IWorkflowAutomation> post = new WebChannelFactory<IWorkflowAutomation>(new Uri(_url)))
            {
                WEMClient.WorkflowAutomation wf = new WEMClient.WorkflowAutomation();
                //  System.Net.ServicePointManager.Expect100Continue = false;
                request.CompanyId = ConfigurationManager.AppSettings["Company"];

                    var response1 = wf.ServiceChannel.Publish(request);

                    if (response1 != null)
                    {
                        if (response1.ServiceFaults != null)
                        { 
                            var faults = response1.ServiceFaults;
                            WEM.Infrastructure.Common.WEMException ex =new WEM.Infrastructure.Common.WEMException();
                            ex.Data.Add("ServiceFaults",faults);
                            throw ex;
                        }

                        var response = response1.WorkflowDetails;

                        //IWorkflowAutomation wfService = post.CreateChannel();
                        //System.Net.ServicePointManager.Expect100Continue = false;
                        //var response = wfService.Publish(request).WorkflowDetails;

                        WorkflowPE data = new WorkflowPE
                        {
                            CategoryID = response.CategoryID,
                            //  SubCategoryID = response.SubCategoryID,
                            PartitionKey = response.CategoryID.ToString("00000"),
                            ClientId = response.ClientId,
                            ClientVer = response.ClientVer,
                           // CreatedBy = response.CreatedBy,
                            Description = response.Description,
                            FileSize = response.FileSize,
                            ImageURI = response.ImageURI,
                            IsActive = true,
                            Name = response.Name,
                            SrcIPAddr = response.SrcIPAddr,
                            SrcMachineName = response.SrcMachineName,
                            WorkflowID = response.WorkflowID,
                            WorkflowURI = response.WorkflowURI,
                            UsesUIAutomation = response.UsesUIAutomation,
                            WorkflowVersion = response.WorkflowVersion
                        };

                        return data;
                    }
                    else
                        throw new Exception("There is an error adding the Workflow. \nPlease verify if the details provided are correct and name is not same as any existing Workflow.");

            }


        }

        internal static WorkflowPE Update(PublishReqMsg request)
        {
            // using (WebChannelFactory<IWorkflowAutomation> post = new WebChannelFactory<IWorkflowAutomation>(new Uri(_url)))
            {
                request.CompanyId = ConfigurationManager.AppSettings["Company"];
                WEMClient.WorkflowAutomation wf = new WEMClient.WorkflowAutomation();
                //      System.Net.ServicePointManager.Expect100Continue = false;
                var response = wf.ServiceChannel.UpdateWorkflow(request).WorkflowDetails;

                //IWorkflowAutomation wfService = post.CreateChannel();
                //System.Net.ServicePointManager.Expect100Continue = false;
                //var response = wfService.UpdateWorkflow(request).WorkflowDetails;

                WorkflowPE data = new WorkflowPE
                {
                    CategoryID = response.CategoryID,
                    ClientId = response.ClientId,
                    ClientVer = response.ClientVer,
                  //  CreatedBy = response.CreatedBy,
                    Description = response.Description,
                    FileSize = response.FileSize,
                    ImageURI = response.ImageURI,
                    IsActive = true,
                    Name = response.Name,
                    SrcIPAddr = response.SrcIPAddr,
                    SrcMachineName = response.SrcMachineName,
                    WorkflowID = response.WorkflowID,
                    WorkflowURI = response.WorkflowURI,
                    UsesUIAutomation = response.UsesUIAutomation,
                    WorkflowVersion = response.WorkflowVersion
                };
                return data;
            }

        }

        internal static GetAllActiveWorkflowsByCategoryResMsg GetWorkflowByCategory(int catId)//, int subCatId)
        {
            GetAllActiveWorkflowsByCategoryResMsg ret = null;

            WEMClient.WorkflowAutomation wf = new WEMClient.WorkflowAutomation();
            ret = wf.ServiceChannel.GetAllActiveWorkflowsByCategory(catId.ToString());

            return ret;
        }

        internal static GetWorkflowDetailsResMsg GetWorkFlowDetails(int catId, Guid workFlowId, int workflowVer)
        {
            WEMClient.WorkflowAutomation wf = new WEMClient.WorkflowAutomation();
            return wf.ServiceChannel.GetWorkflowDetails(catId, workFlowId, workflowVer, "", "");
        }


        internal static bool Delete(WorkflowPE pe)
        {
            WEMClient.WorkflowAutomation wf = new WEMClient.WorkflowAutomation();
            var result = wf.ServiceChannel.DeleteWorkflow(new DeleteWorkflowReqMsg
            {
                Workflow = new WEM.Service.Contracts.Data.WorkflowMaster
                {

                    CategoryID = pe.CategoryID,
                    WorkflowVersion = pe.WorkflowVersion,
                    WorkflowID = pe.WorkflowID

                }
            });
            return result.Status;


        }

        internal static Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg GetAllClustersByCategory(string categoryId)
        {
            Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg response = null;
            WEMClient.CommonRepository commonRepoClient = new WEMClient.CommonRepository();
            response = commonRepoClient.ServiceChannel.GetAllClustersByCategory(categoryId.ToString());
            return response;
        }

        internal static string DownloadXAML(string uri)
        {
            string content = null;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            CredentialCache cc = new CredentialCache();
            cc.Add(
                new Uri(Infosys.WEM.Client.CommonServices.Instance.StorageBaseURL),
                "NTLM",
                CredentialCache.DefaultNetworkCredentials);
            req.Credentials = cc;

            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            if (uri.Contains(".iapw"))
            {

                byte[] b = null;
                Stream stream = response.GetResponseStream();
                using (MemoryStream ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        byte[] buffer = new byte[1024];
                        count = stream.Read(buffer, 0, 1024);
                        ms.Write(buffer, 0, count);
                    } while (stream.CanRead && count > 0);
                    b = ms.ToArray();
                }

                stream = new MemoryStream(b);
                stream = Infosys.ATR.Packaging.Operations.ExtractFile(stream, "\\main.xaml");
                var bytes = default(byte[]);

                if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                {                   
                    using (var memstream = new MemoryStream())
                    {
                        var buffer = new byte[1024];
                        var bytesRead = default(int);
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            memstream.Write(buffer, 0, bytesRead);
                        bytes = memstream.ToArray();
                    }

                    if (System.Text.Encoding.Unicode.GetString(bytes).Contains(Infosys.WEM.SecureHandler.SecurePayload.keyText))
                    {
                        byte[] byteContent = SecurePayload.UnSecureBytes(bytes);
                        Stream ScriptContent = new System.IO.MemoryStream(byteContent);
                        content = (new StreamReader(ScriptContent)).ReadToEnd();
                    }
                    else
                        content = (new StreamReader(new System.IO.MemoryStream(bytes)).ReadToEnd());
                }
                else
                    content = (new StreamReader(stream).ReadToEnd());

            }
            else
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    if (Infosys.WEM.Client.CommonServices.Instance.EnableSecureTransactions)
                    {
                        var bytes = default(byte[]);
                        using (var memstream = new MemoryStream())
                        {
                            var buffer = new byte[1024];
                            var bytesRead = default(int);
                            while ((bytesRead = sr.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                memstream.Write(buffer, 0, bytesRead);
                            bytes = memstream.ToArray();
                        }

                        if (System.Text.Encoding.Unicode.GetString(bytes).Contains("aWFw"))
                        {
                            byte[] byteContent = Infosys.WEM.SecureHandler.SecurePayload.UnSecureBytes(bytes);
                            Stream ScriptContent = new System.IO.MemoryStream(byteContent);
                            content = (new StreamReader(ScriptContent)).ReadToEnd();
                        }
                        else
                            content = (new StreamReader(new System.IO.MemoryStream(bytes)).ReadToEnd());
                    }
                    else
                        content = sr.ReadToEnd();
                }

                //return sr.ReadToEnd();
            }
            return content;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        internal static GetTransactionsResMsg GetTransactions(TransactionFilter transFilter) 
        {
            //todo
            //call service to retrieve transaction view information 
            //return data contract Transaction  

            WEMClient.TransactionRepository transactions = new WEMClient.TransactionRepository();
            GetTransactionsReqMsg req = new GetTransactionsReqMsg();
            WEM.Node.Service.Contracts.Data.TransactionFilter wemFilter = new WEM.Node.Service.Contracts.Data.TransactionFilter();
            wemFilter.CategoryId = transFilter.CategoryId;
            wemFilter.CompanyId = transFilter.CompanyId;
            wemFilter.StartDate = transFilter.StartDate;
            wemFilter.EndDate = transFilter.EndDate;
            req.FilterCriteria = wemFilter; 
            return transactions.ServiceChannel.GetTransactions(req);                   
        }

        internal static LogTransactionResMsg UpdateTransaction(Infosys.WEM.Node.Service.Contracts.Data.Transaction trans, StateType stateToUpdate) 
        {
            WEMClient.TransactionRepository transactions = new WEMClient.TransactionRepository();
            var logTransactionResMsg = new LogTransactionReqMsg();
            logTransactionResMsg.Request = trans;
            logTransactionResMsg.Request.CurrentState = (Infosys.WEM.Node.Service.Contracts.Data.StateType)stateToUpdate;
            return transactions.ServiceChannel.LogTransaction(logTransactionResMsg);
        }
    }
}
