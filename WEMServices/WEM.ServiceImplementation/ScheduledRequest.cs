/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Activation;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Node.Service.Contracts;
using Infosys.WEM.Scripts.Service.Contracts;
using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Scripts.Service.Contracts.Message;
using Infosys.WEM.Resource.DataAccess;
using Infosys.WEM.Scripts.Resource.DataAccess;
using Infosys.WEM.SecurityAccess.Contracts.Data;
using System.Globalization;
using System.ServiceModel;

namespace Infosys.WEM.Service.Implementation
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class ScheduledRequest_ServiceBase : IScheduleRequest
    {
        #region IScheduleRequest Members

        public virtual AddScheduledRequestResMsg AddScheduledRequest(AddScheduledRequestReqMsg value)
        {
            return null;
        }

        public virtual GetExecutionStatusResMsg GetExecutionStatus(string requestId, string iapNodeName)
        {
            return null;
        }

        public virtual GetNextRequestResMsg GetNextRequest(string domain, string iapNodeName, string requestType)
        {
            return null;
        }

        public virtual bool UpdateRequestExecutionStatus(UpdateRequestExecutionStatusReqMsg value)
        {
            return false;
        }

        public virtual GetScheduledRequestActivitiesResMsg GetScheduledRequestActivities(string scheduledRequestId)
        {
            return null;
        }

        public virtual GetNScheduledRequestActivitiesResMsg GetTodaysScheduledRequestActivities(string companyId)
        {
            return null;
        }

        public virtual GetNScheduledRequestActivitiesResMsg GetScheduledRequestActivitiesFor(string companyId, string lastNdays)
        {
            return null;
        }

        public virtual GetNScheduledRequestActivitiesResMsg GetScheduledRequestActivitiesBetween(string companyId, string fromDateTicks, string toDateTicks)
        {
            return null;
        }

        public virtual GetLongInitiatedRequestsResMsg GetLongInitiatedRequests(string companyId, string categoryId, string requestor)
        {
            return null;
        }


        public GetNextRequestResMsg GetNextRequest(string iapNodeName, string requestType)
        {
            return null;
        }
        #endregion
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class ScheduledRequest : ScheduledRequest_ServiceBase
    {
        public override AddScheduledRequestResMsg AddScheduledRequest(AddScheduledRequestReqMsg value)
        {
            string id = null;
            AddScheduledRequestResMsg response = new AddScheduledRequestResMsg();
            List<string> ids = null;
            if (value != null && value.Request != null && !string.IsNullOrEmpty(value.Request.AssignedTo))
            {
                if (Security.Access.Check(value.Request.CategoryId.ToString()))
                {
                    try
                    {
                        ids = new List<string>();

                        //check if end date future to start date
                        if (value.Request.EndDate != null && value.Request.ExecuteOn != null)
                        {
                            if (value.Request.EndDate < value.Request.ExecuteOn)
                            {
                                ids.Add("End date should be future to the start date i.e. Execute-On date");
                                response.ScheduledRequestIds = ids;
                                response.IsSuccess = false;
                                //return ids;
                                return response;
                            }
                        }

                        //check if Requestor has access to the category in which this workflow/ script is associated.
                        //then check for workflow or script id and category id, if there is any valid entry

                        if (!CheckRequestorAccess(value.Request.Requestor, value.Request.CompanyId, value.Request.CategoryId))
                        {
                            ids.Add(string.Format("Requestor doesnt have the role of Manager or Analyst for category with id -{0}", value.Request.CategoryId));
                            response.ScheduledRequestIds = ids;
                            response.IsSuccess = false;
                            //return ids;
                            return response;
                        }

                        //Future -  Check if the group associated with the user has permission to send a scheduled transaction to run in that group based on category group association

                        switch (value.Request.RequestType)
                        {
                            case Node.Service.Contracts.Data.RequestTypeEnum.Script:
                                string scriptid = value.Request.RequestId;
                                ScriptDS scriptDs = new ScriptDS();
                                Infosys.WEM.Scripts.Service.Contracts.Data.Script scriptSE = new Infosys.WEM.Scripts.Service.Contracts.Data.Script() { ScriptId = int.Parse(scriptid), CategoryId = value.Request.CategoryId, ScriptFileVersion = value.Request.RequestVersion };
                                var script = Translators.Scripts.ScriptSE_DE.ScriptDEtoSE(scriptDs.GetOne(Translators.Scripts.ScriptSE_DE.ScriptSEtoDE(scriptSE)));
                                if (script == null)
                                {
                                    ids.Add("Invalid Script and Category Id combination provided");
                                    response.ScheduledRequestIds = ids;
                                    response.IsSuccess = false;
                                    //return ids;
                                    return response;
                                }
                                break;
                            case Node.Service.Contracts.Data.RequestTypeEnum.Workflow:
                                Guid workflowId = new Guid(value.Request.RequestId);
                                var workflow = new WorkflowAutomation().GetWorkflowDetails(value.Request.CategoryId, workflowId, value.Request.RequestVersion, null, null);
                                if (workflow == null)
                                {
                                    ids.Add("Invalid Workflow and Category Id combination provided");
                                    response.ScheduledRequestIds = ids;
                                    response.IsSuccess = false;
                                    //return ids;
                                    return response;
                                }
                                break;
                            default:
                                ids.Add("Invalid Request Type");
                                response.ScheduledRequestIds = ids;
                                response.IsSuccess = false;
                                //return ids;
                                return response;
                        }
                        //not need to assign the execution status as by default it is "new"
                        if (value.Request.EndDate != null && value.Request.EndDate != DateTime.MinValue && value.Request.ExecuteOn != null && value.Request.ExecuteOn != DateTime.MinValue)
                        {
                            //int iteration = (int)(value.Request.EndDate.Value - value.Request.ExecuteOn.Value).TotalDays;
                            //if (iteration > 0)
                            //    value.Request.Iterations = iteration;

                            //date difference include the extremes
                            //date.substract is excatly same as d2-d1 as both returns timespan object with samne value
                            //main logic is with the math.ceiling
                            var days = value.Request.EndDate.Value.Subtract(value.Request.ExecuteOn.Value).TotalDays;
                            if (value.Request.EndDate.Value.Date > value.Request.ExecuteOn.Value.Date)
                                value.Request.Iterations = (int)Math.Ceiling(days) + 1;
                            else
                                value.Request.Iterations = (int)Math.Ceiling(days);
                        }
                        //check the end date and then accordingly populate iteration

                        ScheduledRequestDS schReqDs = new ScheduledRequestDS();
                        if (value.Request.Iterations > 1 && value.Request.ExecuteOn != null && value.Request.StopType == Node.Service.Contracts.Data.StopTypes.Limited)
                        {
                            DateTime startExecuteOn = value.Request.ExecuteOn.Value;
                            for (int i = 0; i < value.Request.Iterations; i++)
                            {
                                value.Request.ExecuteOn = startExecuteOn.AddDays(i);
                                //assign the iteration set root related info
                                if (ids.Count == 0)
                                    value.Request.IsIterationSetRoot = true;
                                else
                                {
                                    value.Request.IsIterationSetRoot = false;
                                    value.Request.IterationSetRoot = ids[0];
                                }
                                Resource.Entity.ScheduledRequest schReqDe = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestSeToDe(value.Request);
                                Resource.Entity.ScheduledRequest enterredDe = schReqDs.Insert(schReqDe);
                                if (enterredDe != null)
                                    id = enterredDe.Id;
                                else
                                    id = "Error";
                                ids.Add(id);
                            }
                        }
                        else
                        {
                            Resource.Entity.ScheduledRequest schReqDe = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestSeToDe(value.Request);
                            Resource.Entity.ScheduledRequest enterredDe = schReqDs.Insert(schReqDe);
                            if (enterredDe != null)
                                id = enterredDe.Id;
                            else
                                id = "Error";
                            ids.Add(id);
                        }

                    }


                    catch (Exception wemScriptException)
                    {
                        Exception ex = new Exception();
                        bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                        if (rethrow)
                        {
                            throw ex;
                        }
                    }
                }
                else
                    throw new Exception("You do not have access to schedule a request in this category");
            }

            response.ScheduledRequestIds = ids;
            response.IsSuccess = true;
            //return ids;
            return response;

        }

        public override GetLongInitiatedRequestsResMsg GetLongInitiatedRequests(string companyId, string categoryId, string requestor)
        {
            GetLongInitiatedRequestsResMsg response = null;
            try
            {
                response = new GetLongInitiatedRequestsResMsg();
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(categoryId))
                {
                    //check if the requestor has access to the category as super admin or manager or analyst
                    if (CheckRequestorAccess(requestor, int.Parse(companyId), int.Parse(categoryId)))
                    {
                        ScheduledRequestDS schReqDs = new ScheduledRequestDS();
                        response.Requests = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestListDeToSe(schReqDs.GetLongInitiated(Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestSeToDe(new Node.Service.Contracts.Data.ScheduledRequest() { CategoryId = int.Parse(categoryId) })));
                    }
                    else
                    {
                        response.AdditionalInfo = string.Format("Requestor doesnt have the role of Manager or Analyst for category with id -{0}", categoryId);
                        response.IsSuccess = false;
                    }

                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.AdditionalInfo = "Invalid Company Id or Category Id";
                }
            }
            catch (Exception wemScriptException)
            {
                response.AdditionalInfo = wemScriptException.Message;
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetExecutionStatusResMsg GetExecutionStatus(string requestId, string nodeName)
        {
            GetExecutionStatusResMsg response = null;
            if (!string.IsNullOrEmpty(requestId) && !string.IsNullOrEmpty(nodeName))
            {
                response = new GetExecutionStatusResMsg();
                try
                {
                    ScheduledRequestDS schReqDs = new ScheduledRequestDS();
                    Node.Service.Contracts.Data.ScheduledRequest request = new Node.Service.Contracts.Data.ScheduledRequest();
                    request.Id = requestId;
                    request.AssignedTo = nodeName;
                    Resource.Entity.ScheduledRequest schReqDe = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestSeToDe(request);
                    Resource.Entity.ScheduledRequest enterredDe = schReqDs.GetOne(schReqDe);
                    if (enterredDe != null)
                    {
                        response.OutputParameters = enterredDe.OutputParameters;
                        response.Message = enterredDe.Message;
                        if (enterredDe.State != null)
                            response.ExecutionStatus = (Node.Service.Contracts.Data.RequestExecutionStatus)enterredDe.State;
                    }
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            return response;
        }

        public override bool UpdateRequestExecutionStatus(UpdateRequestExecutionStatusReqMsg value)
        {
            //TBD- additional check for the keys. 
            //check for the executor for the concerned request in db is same as this iapnode from where this update request is coming- is in the Update datacess method
            //set the executor to null if status is resubmit
            bool response = false;
            if (value != null && !string.IsNullOrEmpty(value.IAPNode) && !string.IsNullOrEmpty(value.AssignedTo))
            {
                try
                {
                    ScheduledRequestDS schReqDs = new ScheduledRequestDS();
                    Node.Service.Contracts.Data.ScheduledRequest request = new Node.Service.Contracts.Data.ScheduledRequest();
                    request.Id = value.Id;
                    request.AssignedTo = value.AssignedTo;
                    request.State = value.ExecutionStatus;
                    if (request.State == Node.Service.Contracts.Data.RequestExecutionStatus.ReSubmit)
                        request.Executor = null;
                    else
                        request.Executor = value.IAPNode;
                    request.Message = value.Message;
                    //request.ModifiedBy = value.ModifiedBy;
                    request.OutputParameters = value.OutputParameters;
                    Resource.Entity.ScheduledRequest schReqDe = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestSeToDe(request);
                    Resource.Entity.ScheduledRequest enterredDe = schReqDs.Update(schReqDe);
                    if (enterredDe != null)
                        response = true;

                    //if the status is user action completed, add a new entry for scheduled request
                    if (value.ExecutionStatus == Node.Service.Contracts.Data.RequestExecutionStatus.UserActionCompleted)
                    {
                        //first reset the id and assign parent id
                        //enterredDe.ParentId = enterredDe.Id;
                        var tempSe = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestDeToSe(enterredDe);
                        tempSe.ParentId = tempSe.Id;
                        tempSe.Id = null;
                        AddScheduledRequest(new AddScheduledRequestReqMsg() { Request = tempSe });
                    }
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            return response;
        }

        public override GetNextRequestResMsg GetNextRequest(string domain, string iapNodeName, string requestType)
        {
            GetNextRequestResMsg response = null;
            ScheduledRequestDS schReqDs = new ScheduledRequestDS();
            if (!string.IsNullOrEmpty(iapNodeName))
            {
                response = new GetNextRequestResMsg();
                try
                {
                    //valid request types are- 1- Workflow, 2 – Script
                    Node.Service.Contracts.Data.RequestTypeEnum reqType;

                    //get all the groups of which this iapnode is a member
                    SemanticNodeClusterDSExt clusterDs = new SemanticNodeClusterDSExt();
                    List<string> processorIds = clusterDs.GetAllAssociatedCluster(iapNodeName, domain);
                    if (processorIds == null)
                        processorIds = new List<string>();
                    //add the iapnode name to the list of processors
                    processorIds.Add(iapNodeName);

                    //now get all the scheduled requests for these processor list

                    //first get all the requests in db for the provided iap node and cluster ids
                    //if there is more than 1 requests, then sort it based on priority
                    //if there is more than one with highest priority, then sort it based on created-on date

                    List<Resource.Entity.ScheduledRequest> schReqDes = new List<Resource.Entity.ScheduledRequest>();

                    foreach (string processor in processorIds)
                    {
                        Node.Service.Contracts.Data.ScheduledRequest request = new Node.Service.Contracts.Data.ScheduledRequest();
                        request.AssignedTo = processor;
                        Resource.Entity.ScheduledRequest schReqDe = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestSeToDe(request);
                        schReqDes.Add(schReqDe);
                    }

                    List<Node.Service.Contracts.Data.ScheduledRequest> requests = Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestListDeToSe(schReqDs.GetAllNewAndResubmit(schReqDes).ToList());
                    if (requests != null && requests.Count > 0)
                    {
                        //now filter if any request type is provided and not equal to 0
                        if (requestType != "0" && Enum.TryParse<Node.Service.Contracts.Data.RequestTypeEnum>(requestType, out reqType))
                        {
                            requests = requests.Where(req => req.RequestType == reqType).ToList();
                        }


                        //get all those for which executor is null
                        var newrequests = requests.Where(req => string.IsNullOrEmpty(req.Executor)).ToList();

                        requests = new List<Node.Service.Contracts.Data.ScheduledRequest>();
                        if (newrequests != null && newrequests.Count > 0)
                            requests.AddRange(newrequests);

                        if (requests != null && requests.Count > 0)
                        {
                            if (requests.Count == 1)
                                response.Request = requests[0];
                            else if (requests.Count > 1)
                            {
                                requests = requests.OrderBy(req => req.CreatedOn).ToList(); //first order by created and then priority
                                requests = requests.OrderBy(req => req.Priority).ToList();
                                requests = requests.OrderBy(req => req.ExecuteOn).ToList();

                                response.Request = requests[0];
                            }

                            //now check ExecuteOn for the received schedulerequest.
                            if (response.Request.ExecuteOn != null && response.Request.ExecuteOn > DateTime.UtcNow)
                                response.Request = null; //i.e. dont return anything.
                            //if execute on is null then it will be returned now and to executed asap
                        }
                    }
                    if (response != null && response.Request != null)
                    {
                        //then update the Executor same as iapNodeName for the to be returned ScheduledRequest
                        response.Request.State = Node.Service.Contracts.Data.RequestExecutionStatus.Initiated;
                        response.Request.Executor = iapNodeName;
                        schReqDs.Update(Translators.RegisteredNodes.ScheduledRequest_SE_DE.ScheduledRequestSeToDe(response.Request));

                        //if scheduled request is to be executed indefinitely then add a new scheduled request with execute on incremented by one day
                        if (response.Request.StopType == Node.Service.Contracts.Data.StopTypes.Indefinite)
                        {
                            var tempReq = new Node.Service.Contracts.Data.ScheduledRequest();
                            tempReq.AssignedTo = response.Request.AssignedTo;
                            tempReq.CategoryId = response.Request.CategoryId;
                            tempReq.CompanyId = response.Request.CompanyId;
                            tempReq.CreatedOn = response.Request.CreatedOn;
                            tempReq.ExecuteOn = response.Request.ExecuteOn.Value.AddDays(1);
                            //tempReq.Executor dont assign for new one
                            tempReq.Id = null;
                            if (response.Request.ParentId == null) // to keep same parent id
                                tempReq.ParentId = response.Request.Id;
                            tempReq.ParentId = response.Request.ParentId;
                            tempReq.InputParameters = response.Request.InputParameters;
                            //tempReq.Iterations not needed for indefnite execution
                            tempReq.Message = response.Request.Message;
                            //tempReq.OutputParameters not assigned as it is new one
                            tempReq.Priority = response.Request.Priority;
                            tempReq.RequestId = response.Request.RequestId;
                            tempReq.Requestor = response.Request.Requestor;
                            tempReq.RequestType = response.Request.RequestType;
                            tempReq.RequestVersion = response.Request.RequestVersion;
                            tempReq.State = Node.Service.Contracts.Data.RequestExecutionStatus.New;
                            tempReq.StopType = response.Request.StopType;
                            AddScheduledRequest(new AddScheduledRequestReqMsg() { Request = tempReq });
                        }
                    }
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// To check if the requestor has access to the category
        /// </summary>
        /// <param name="requestor"></param>
        /// <param name="companyId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private bool CheckRequestorAccess(string requestor, int companyId, int categoryId)
        {
            bool hasAccess = false;

            //first check if the user is super admin, then allow
            //else check if the user is analyst or manager           

            if (requestor.Contains("\\"))
                requestor = requestor.Split(new string[] { "\\" }, StringSplitOptions.None)[1];

            requestor = Infosys.WEM.Infrastructure.SecurityCore.SecureData.Secure(requestor, ApplicationConstants.SECURE_PASSCODE);

            var superadmin = new SecurityAccess().IsSuperAdmin(requestor, companyId.ToString());
            if (superadmin != null && superadmin.IsSuperAdmin)
            {
                hasAccess = true;
            }
            else
            {
                requestor = Infosys.WEM.Infrastructure.SecurityCore.SecureData.UnSecure(requestor, ApplicationConstants.SECURE_PASSCODE);
                //the below code taken from SecurityAccess -> GetAnyUser
                UserDS userDS = new UserDS();
                var users = userDS.GetAnyUser(new Resource.Entity.User
                {
                    CategoryId = categoryId,
                    CompanyId = companyId
                }).Select(u => new User
                {
                    Alias = u.Alias,
                    CompanyId = u.CompanyId,
                   // CreatedBy = u.CreatedBy,
                    DisplayName = u.DisplayName,
                    CategoryId = u.CategoryId.GetValueOrDefault(),
                    IsActive = u.IsActive.GetValueOrDefault(),
                    //LastModifiedBy = u.LastModifiedBy,
                    Role = u.Role,
                    UserId = u.Id,
                    IsDL = u.IsDL.GetValueOrDefault(),
                    GroupId = u.GroupId

                }).ToList();
                //till here code taken from SecurityAccess -> GetAnyUser

                if (users != null)
                {                                      
                    var user = users.FirstOrDefault(u => u.Alias.ToLower() == requestor.ToLower());                    
                    if (user != null && (user.Role ==2 || user.Role == 3 || user.Role == 5))
                    {
                        hasAccess = true;                        
                    }
                }
            }
            return hasAccess;
        }

        public override GetScheduledRequestActivitiesResMsg GetScheduledRequestActivities(string scheduledRequestId)
        {
            GetScheduledRequestActivitiesResMsg response = null;
            if (!string.IsNullOrEmpty(scheduledRequestId))
            {
                try
                {
                    response = new GetScheduledRequestActivitiesResMsg();
                    response.ActivitDetails = new Node.Service.Contracts.Data.ScheduledRequestDetails();
                    response.ActivitDetails.ScheduledRequestId = scheduledRequestId;

                    ScheduledRequestActivitiesDS activitiesDs = new ScheduledRequestActivitiesDS();
                    Node.Service.Contracts.Data.ScheduledRequestActivity activity = new Node.Service.Contracts.Data.ScheduledRequestActivity() { ScheduledRequestId = scheduledRequestId };
                    var activityDe = Translators.RegisteredNodes.ScheduledRequestActivities_SE_DE.ScheduledRequestActivitiesSEtoDE(activity);
                    response.ActivitDetails.Activities = Translators.RegisteredNodes.ScheduledRequestActivities_SE_DE.ScheduledRequestActivitiesListDetoSE(activitiesDs.GetAllActivitiesForScheduledRequest(activityDe).ToList());

                    //check if any child schduled request activities are there for this scheduled request.
                    response.ActivitDetails.ChildScheduledRequestIds = activitiesDs.GetAllChildrenIds(activityDe);
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            return response;
        }

        public override GetNScheduledRequestActivitiesResMsg GetTodaysScheduledRequestActivities(string companyId)
        {
            GetNScheduledRequestActivitiesResMsg response = null;
            try
            {
                DateTime currentDateTime = DateTime.UtcNow;
                response = GetAllScheduledRequestActivitiesFor(companyId, currentDateTime, currentDateTime.Date);
            }
            catch (Exception wemScriptException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                if (rethrow)
                {
                    throw ex;
                }
            }
            return response;
        }

        public override GetNScheduledRequestActivitiesResMsg GetScheduledRequestActivitiesFor(string companyId, string lastNdays)
        {
            GetNScheduledRequestActivitiesResMsg response = null;
            int iLastNdays;
            if (int.TryParse(lastNdays, out iLastNdays) && iLastNdays > 0)
            {
                if (iLastNdays == 0)
                    response = GetTodaysScheduledRequestActivities(companyId);
                else
                {
                    try
                    {
                        DateTime fromdate = DateTime.UtcNow;
                        DateTime todate = fromdate.AddDays(-iLastNdays);
                        response = GetAllScheduledRequestActivitiesFor(companyId, fromdate, todate);
                    }
                    catch (Exception wemScriptException)
                    {
                        Exception ex = new Exception();
                        bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                        if (rethrow)
                        {
                            throw ex;
                        }
                    }
                }
            }
            return response;
        }

        public override GetNScheduledRequestActivitiesResMsg GetScheduledRequestActivitiesBetween(string companyId, string fromDateTicks, string toDateTicks)
        {
            GetNScheduledRequestActivitiesResMsg response = null;
            long lfromdateticks, ltodateticks;
            if (long.TryParse(fromDateTicks, out lfromdateticks) && long.TryParse(toDateTicks, out ltodateticks))
            {
                try
                {
                    DateTime fromdate = new DateTime(lfromdateticks);
                    DateTime todate = new DateTime(ltodateticks);
                    response = GetAllScheduledRequestActivitiesFor(companyId, fromdate, todate);
                }
                catch (Exception wemScriptException)
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(wemScriptException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                    if (rethrow)
                    {
                        throw ex;
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// The generic method to fetch all the scheduled request activities for the pasked company id and date range.
        /// This doesnot include the children scheduled requests, as these are returned only by GetScheduledRequestActivities
        /// </summary>
        /// <param name="companyId">the comapny id</param>
        /// <param name="fromDate">the from date</param>
        /// <param name="toDate">the to date. todate is previous to fromdate</param>
        /// <returns></returns>
        private GetNScheduledRequestActivitiesResMsg GetAllScheduledRequestActivitiesFor(string companyId, DateTime fromDate, DateTime toDate)
        {
            GetNScheduledRequestActivitiesResMsg response = new GetNScheduledRequestActivitiesResMsg();
            if (!string.IsNullOrEmpty(companyId))
            {
                response.ActivityDetails = new List<Node.Service.Contracts.Data.ScheduledRequestDetails>();

                DateTime currentDateTime = DateTime.UtcNow;
                //<CompanyId>_<9999-YYYY>_<13-Month>_<7-Week>
                string PK = FormPK(currentDateTime, companyId);
                string fromRK = FormRK(fromDate);
                //for today the date has to be taken from 00:00 hrs
                string toRK = FormRK(toDate);
                //from RK < to RK
                ScheduledRequestActivitiesDS activitiesDs = new ScheduledRequestActivitiesDS();
                var todaysActivities = Translators.RegisteredNodes.ScheduledRequestActivities_SE_DE.ScheduledRequestActivitiesListDetoSE(activitiesDs.GetAllByPartitionKeyAndRowKeyRange(PK, fromRK, toRK));
                if (todaysActivities != null && todaysActivities.Count > 0)
                {
                    //group the activities for ScheduledRequestId
                    var groupedActivity = todaysActivities.GroupBy(act => act.ScheduledRequestId);
                    foreach (var group in groupedActivity)
                    {
                        response.ActivityDetails.Add(new Node.Service.Contracts.Data.ScheduledRequestDetails() { ScheduledRequestId = group.Key, Activities = group.ToList() }); //children schedule request ids are not assigned
                    }
                }
            }
            return response;
        }

        /// <summary>
        /// Given the date, gets the week of the month in whihc the date falls
        /// </summary>
        /// <param name="date">the date</param>
        /// <returns>the week in which the date given date falls</returns>
        private int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        /// <summary>
        /// Forms the partion key pattern to be used to fetch scheduled request activities
        /// </summary>
        /// <param name="currentDateTime"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        private string FormPK(DateTime currentDateTime, string companyId)
        {
            return string.Format("{0}_{1}_{2:D2}_{3}", companyId.PadLeft(3, '0'), 9999 - currentDateTime.Year, 13 - currentDateTime.Month, 7 - GetWeekOfMonth(currentDateTime));
        }

        /// <summary>
        /// Forms the row key pattern to be used to fetch scheduled request activities
        /// </summary>
        /// <param name="currentDateTime"></param>
        /// <returns></returns>
        public string FormRK(DateTime currentDateTime)
        {
            string nineteenDigitTicks = string.Format("{0:D19}", DateTime.MaxValue.Ticks - currentDateTime.Ticks);
            return nineteenDigitTicks;
        }
    }
}
