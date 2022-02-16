/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.PropertyEditing;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using System.Text.RegularExpressions;
using System.Activities;
using Infosys.WEM.WorkflowExecutionLibrary.Entity;
using Infosys.WEM.WorkflowExecutionLibrary;
using System.Activities.Presentation;
using Infosys.WEM.AutomationActivity.Designers;
using Infosys.WEM.Service.Common.Contracts.Message;
using WEMClient = Infosys.WEM.Client;
using System.Windows.Markup;
using Infosys.WEM.SecureHandler;
namespace Infosys.WEM.AutomationActivity.Libraries.Utilities
{
    [Designer(typeof(Designer.ScheduleConfiguration))]
    public class ScheduleConfiguration : NativeActivity, IActivityTemplateFactory
    {
        DateTime _scheduleStartDateTime = DateTime.Now;
        ModuleType _moduleType = ModuleType.Workflow;
        SchedulePatterns _schedulePattern = SchedulePatterns.ScheduleNow;
        ScheduleStopCriteria _scheduleStopCriteria = ScheduleStopCriteria.EndDate;
        int _priority = 1000;
        int _scheduleOcurrences = 0;
        List<string> _clusterNames = new List<string>();

        [Description("Schedule Now or set Date Time for schedule")]
        public SchedulePatterns SchedulePattern
        {
            get { return _schedulePattern; }
            set { _schedulePattern = value; }
        }
        public ModuleType ModuleType
        {
            get { return _moduleType; }
            set { _moduleType = value; }
        }
        public DateTime ScheduleStartDateTime
        {
            get
            {
                return _scheduleStartDateTime;
            }
            set
            {
                _scheduleStartDateTime = value;
            }
        }
        public ScheduleStopCriteria ScheduleStopCriteria
        {
            get { return _scheduleStopCriteria; }
            set { _scheduleStopCriteria = value; }
        }
        public int ScheduleOcurrences
        {
            get { return _scheduleOcurrences; }
            set { _scheduleOcurrences = value; }
        }
        public DateTime? ScheduleEndDate { get; set; }

        public int SchedulePriority { get; set; }
        public List<string> RegisteredClusters { get; set; }
        public List<string> RegisteredServerNames { get; set; }
        public int CategoryId { get; set; }
        public List<string> RemoteServerNames { get; set; }
        public List<string> SelectedClusters { get; set; }

        [Browsable(false)]       
        public int RemoteExecutionMode { get; set; }

        [RequiredArgument]
        //[DependsOn("RemoteExecutionMode")]
        [OverloadGroup("ScheudleOncluster")]
        public InArgument<string> MetadataRepositoryURI { get; set; }
        [RequiredArgument]
        //[DependsOn("RemoteExecutionMode")]
        [OverloadGroup("ScheudleOncluster")]
        public InArgument<string> CategoryName { get; set; }


        [RequiredArgument]
        [DependsOn("RemoteExecutionMode")]
        [OverloadGroup("ScheudleOnIAPNode")]
        public InArgument<string> DomainName { get; set; }

        [RequiredArgument]
        [DependsOn("RemoteExecutionMode")]
        [Description("Name of ScheduleConfigurationSpec object")]
        public OutArgument<ScheduleConfigurationSpec> ScheduleConfigurationSpec { get; set; }

        public ScheduleConfiguration()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_RESTSERVICES_CALL_BEGIN, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.LOGERROR, ActivityControls.UTILITIES))
                {
                    ScheduleConfigurationSpec options = new ScheduleConfigurationSpec();
                    options.RemoteExecutionMode = RemoteExecutionMode;
                    options.ModuleType = ModuleType;
                    options.SchedulePattern = SchedulePattern;
                    options.SchedulePriority = SchedulePriority;

                    if (options.SchedulePattern == SchedulePatterns.ScheduleNow)
                    {
                        options.ScheduleStartDateTime = DateTime.UtcNow;
                        options.ScheduleStopCriteria = Designer.ScheduleStopCriteria.NoEndDate;
                        options.ScheduleOcurrences = 1;
                        ValidateDates(DateTime.Now, null, Designer.ScheduleStopCriteria.NoEndDate);
                    }
                    else
                    {
                        options.ScheduleStartDateTime = ScheduleStartDateTime.ToUniversalTime();
                        options.ScheduleOcurrences = ScheduleOcurrences;
                        options.ScheduleEndDate = (Convert.ToDateTime(ScheduleEndDate)).ToUniversalTime();
                        options.ScheduleStopCriteria = ScheduleStopCriteria;
                        ValidateDates(ScheduleStartDateTime, ScheduleEndDate, ScheduleStopCriteria);
                    }

                    if (options.RemoteExecutionMode == (int)RemoteExecMode.ScheduleOnIAPnode)
                    {
                        if (RemoteServerNames == null)
                        {
                            throw new System.Exception("IAP Node(s) has not been selected");
                        }
                        else
                            options.RemoteServerNames = RemoteServerNames;
                        //ValidateDates(options);
                    }

                    if (options.RemoteExecutionMode == (int)RemoteExecMode.ScheduleOnCluster)
                    {
                        if (SelectedClusters == null)
                        {
                            throw new System.Exception("Cluster(s) has not been selected");
                        }
                        options.CategoryId = CategoryId;
                        options.ScheduleClusterName = SelectedClusters;
                        bool iscategoryValid = IsCategoryValid(CategoryId, context.GetValue(MetadataRepositoryURI));
                        if (iscategoryValid)
                        {
                            //ValidateDates(options);

                            if (IsClusterValid(options.CategoryId, options.ScheduleClusterName))
                            {
                                options.ScheduleStartDateTime = ScheduleStartDateTime.ToUniversalTime();
                                if (ScheduleEndDate != null)
                                    options.ScheduleEndDate = Convert.ToDateTime(ScheduleEndDate).ToUniversalTime();
                                ScheduleConfigurationSpec.Set(context, options);
                            }
                            else
                            {
                                throw new Exception("Category cluster association is invalid. Please select again.");
                            }
                        }
                        else
                        {
                            throw new Exception("Category is invalid. Please select again.");
                        }
                    }
                    else
                    {
                        options.ScheduleStartDateTime = ScheduleStartDateTime.ToUniversalTime();
                        if (ScheduleEndDate != null)
                            options.ScheduleEndDate = Convert.ToDateTime(ScheduleEndDate).ToUniversalTime();
                        ScheduleConfigurationSpec.Set(context, options);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_RESTSERVICES_CALL_END, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }
        }

        private static void ValidateDates(DateTime startDate, DateTime? endDate, Designer.ScheduleStopCriteria scheduleStopCriteria)
        {
            if (startDate.Date.Subtract(DateTime.Now.Date) < new TimeSpan(0, 0, 0, 0))
                throw new System.Exception("Start date should not be a past date");
            if (scheduleStopCriteria == Designer.ScheduleStopCriteria.EndDate)
            {
                if ((Convert.ToDateTime(endDate)).Subtract(startDate).Days < 0)
                {
                    throw new System.Exception("Start date should not be greater than the end date");
                }
            }
        }

        private Boolean IsCategoryValid(int catId, string serverName)
        {
            Boolean isValid = false;
            var name = GetAlias();
            string companyId = System.Configuration.ConfigurationManager.AppSettings["Company"];
            Infosys.WEM.SecurityAccess.Contracts.Message.GetAllUsersResMsg response = GetUsers(name, companyId);
            List<Infosys.WEM.SecurityAccess.Contracts.Data.User> users = response.Users.FindAll(x => x.CategoryId == catId);
            if (users == null)
                isValid = true;
            foreach (Infosys.WEM.SecurityAccess.Contracts.Data.User u in users)
            {
                if (u.CategoryId == catId)
                    isValid = true;
            }
            return isValid;
        }

        internal static Infosys.WEM.SecurityAccess.Contracts.Message.GetAllUsersResMsg GetUsers(string alias, string companyId)
        {
            Infosys.WEM.SecurityAccess.Contracts.Message.GetAllUsersResMsg responseObj = null;
            Infosys.WEM.Client.SecurityAccess access = new Infosys.WEM.Client.SecurityAccess();
            responseObj = access.ServiceChannel.GetUsers(alias, companyId);
            return responseObj;
        }

        /// <summary>
        /// This method is used to get user alias.
        /// </summary>
        /// <returns>user alias</returns>
        private string GetAlias()
        {
            //var alias = Environment.UserName;
            var alias = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1]; 
            //var alias = System.Threading.Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
            alias = SecurePayload.Secure(alias, "IAP2GO_SEC!URE");
            return alias;
        }

        private Boolean IsClusterValid(int categoryId, List<string> clusters)
        {
            Boolean isValid = false;
            var response = GetAllClustersByCategory(categoryId.ToString());
            List<string> clusterNames = new List<string>();
            if (response.Nodes != null && response.Nodes.Count > 0)
            {
                List<SemanticNode> names = response.Nodes;
                foreach (SemanticNode n in names)
                {
                    List<string> s = clusters.FindAll(x => x.ToLower() == n.Id.ToLower());
                    if (s != null)
                        if (s.Count == 1)
                        {
                            isValid = true;
                        }
                }
            }
            return isValid;
        }

        private static Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg GetAllClustersByCategory(string categoryId)
        {
            Infosys.WEM.Service.Common.Contracts.Message.GetAllClustersByCategoryResMsg response = null;
            WEMClient.CommonRepository commonRepoClient = new WEMClient.CommonRepository();
            response = commonRepoClient.ServiceChannel.GetAllClustersByCategory(categoryId.ToString());
            return response;
        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new ScheduleConfiguration
            {
                DisplayName = "ScheduleConfiguration",
                //RemoteExecutionMode = 2,
            };
        }
    }
}

