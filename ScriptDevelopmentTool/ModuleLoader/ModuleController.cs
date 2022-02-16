/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Configuration;

using Infosys.ATR.ModuleLoader.Services;
using Infosys.ATR.Entities;
using Infosys.WEM.SecureHandler;

using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Library.Services;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;

using Microsoft.Practices.CompositeUI.Services;

using Infosys.ATR.ModuleLoader.Views;

namespace Infosys.ATR.ModuleLoader
{
    public class ModuleController : WorkItemController
    {

        Users _user = null;
        string _companyId;

        public override void Run()
        {
            var mode = ConfigurationManager.AppSettings["Mode"];
            _companyId = ConfigurationManager.AppSettings["Company"];

            if (mode == "Online")
            {
                this.WorkItem.RootWorkItem.State["Mode"] = "Online";
            }
            else
            {
                this.WorkItem.RootWorkItem.State["Mode"] = "Offline";
                TrackUsageShellLaunch();
                return;
            }

            bool overrideSecurity = false;

            try
            {
                overrideSecurity = Convert.ToBoolean(ConfigurationManager.AppSettings["OverrideSecurity"]);
            }
            catch(Exception ex)
            {

            }

            if (!overrideSecurity)
            {
                this.WorkItem.RootWorkItem.State["Security"] = "AllowAuthorised";

                var isSuperAdmin = IsSuperAdmin(GetAlias());

                var isActiveUser = IsActiveUser();

                if (!(isSuperAdmin || isActiveUser))
                {
                    throw new Exception("Oops ! You do not have access to this application. Contact Administrator");
                }                
            }
            else
            {
                this.WorkItem.RootWorkItem.State["Security"] = "AllowAll";
                this.WorkItem.RootWorkItem.Items.Add(false, "IsSuperAdmin");

            }
            //LoadModules();
            //TrackUsageShellLaunch();
        }

        private void TrackUsageShellLaunch()
        {
            Logger.Log("ModuleLoader", "Run", "Shell launched");
            Logger.InitiateOfflineLog();
        }

        private bool IsSuperAdmin(string p)
        {
            var alias = GetAlias();
            alias = SecurePayload.Secure(alias, "IAP2GO_SEC!URE");
            bool isSuperAdmin = WFService.IsSuperAdmin(alias, _companyId).IsSuperAdmin;
            CommonObjects.IsSuperAdmin = isSuperAdmin;
            this.WorkItem.RootWorkItem.Items.Add(isSuperAdmin, "IsSuperAdmin");
            return isSuperAdmin;
        }

        private string GetAlias()
        {
            return System.Threading.Thread.CurrentPrincipal.Identity.Name.Split('\\')[1];
        }

        private bool IsActiveUser()
        {
            var name = GetAlias();
            var response = WFService.GetUsers(name,_companyId);
            if (response.Users != null && response.Users.Count > 0)
            {
                List<Users> users = new List<Users>();
                List<int> categories = new List<int>();
                response.Users.ForEach(u =>
                {
                    Users _user = new Users();
                    _user.Alias = u.Alias;
                    _user.Role = Enum.GetName(typeof(Roles), u.Role);
                    _user.DisplayName = u.DisplayName;
                    _user.CategoryId = u.CategoryId;
                    _user.GroupId = u.GroupId.GetValueOrDefault();
                    _user.Id = u.UserId;
                    users.Add(_user);
                    categories.Add(u.CategoryId);
                });

                this.WorkItem.RootWorkItem.Items.Add(users, "CurrentUser");
                CommonObjects.Users = users;                
                this.WorkItem.RootWorkItem.Items.Add(categories, "CurrentUserCategories");
                return true;
            }
            return false;


        }


        private void LoadModules()
        {
            //ModuleSelector module = new ModuleSelector();
            //module.ShowDialog();

            var module = this.WorkItem.SmartParts.AddNew<ModuleSelector>("ModuleSelector");
            module.ShowDialog();

            var assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (module.selected == "Object Model")
            {
                Load(Path.Combine(assemblyPath, "Infosys.ATR.Editor.dll"));

            }
            else if (module.selected == "WF Designer")
            {
                Load(Path.Combine(assemblyPath, "Infosys.ATR.WFDesigner.dll"));
            }

            else if (module.selected == "Script Repository")
            {
                Load(Path.Combine(assemblyPath, "Infosys.ATR.ScriptsRepository.dll"));
            }

            else if (module.selected == "Admin Module")
            {
                Load(Path.Combine(assemblyPath, "Infosys.ATR.Admin.dll"));
            }
        }

        private void Load(string path)
        {
            var srvc = this.WorkItem.Services.Get<IModuleLoaderService>(true);
            Assembly a1 = Assembly.LoadFile(path);
            srvc.Load(this.WorkItem, new Assembly[] { a1 });
        }

        private void AddServices()
        {

        }

        private void ExtendMenu()
        {



        }


        private void ExtendToolStrip()
        {

        }
    }
}
