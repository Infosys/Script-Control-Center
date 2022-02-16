/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Text;
using System.Drawing;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.WinForms;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.CompositeUI.EventBroker;

using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Interface.Services;
using IMSWorkBench.Infrastructure.Shell.Constants;
using IMSWorkBench.Infrastructure.Library;
using IMSWorkBench.Infrastructure.Library.Services;
using System.Windows.Automation;

using Infosys.ATR.DevelopmentStudio;


namespace IMSWorkBench.Infrastructure.Shell
{

    /// <summary>
    /// Main application entry point class.
    /// Note that the class derives from CAB supplied base class FormShellApplication, and the 
    /// main form will be ShellForm, also created by default by this solution template
    /// </summary>
    class ShellApplication : SmartClientApplication<WorkItem, ShellForm>
    {
        internal static uc_ControlExplorer _explorer;
        bool _closed;
        /// <summary>
        /// Application entry point.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if (DEBUG)
            OnLoad();
#else
            OnLoad();
#endif

        }

        private static void OnLoad()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomainUnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);

            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                new ShellApplication().Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception as Exception);
        }




        /// <summary>
        /// Sets the extension site registration after the shell has been created.
        /// </summary>
        protected override void AfterShellCreated()
        {
            try
            {
                base.AfterShellCreated();
                RootWorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.MainToolbar, this.Shell.MainToolStrip);
                RootWorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.MainMenu, this.Shell.MainToolBar);
                RootWorkItem.UIExtensionSites.RegisterSite(UIExtensionSiteNames.FileMenu, this.Shell.FileMenu);

                //add a workspace for modal dialogs
                WindowWorkspace modalWorkspace = new WindowWorkspace(this.Shell);
                RootWorkItem.Workspaces.Add(modalWorkspace, WorkspaceNames.ModalWindows);

                //load control explorer
                _explorer = this.RootWorkItem.SmartParts.AddNew<uc_ControlExplorer>("Explorer");
                _explorer.OnSelect += new uc_ControlExplorer.Select(_explorer_OnSelect);
                _explorer.SaveImageHandler += new uc_ControlExplorer.SaveImage(_explorer_SaveImageHandler);
                WindowSmartPartInfo sp = new WindowSmartPartInfo();
                sp.Title = "Explorer";
                this.RootWorkItem.Workspaces[Constants.WorkspaceNames.DeckWorkSpace].Show(_explorer, sp);
            }
            catch(Exception e)
            {
                HandleException(e);
            }

        }


        void _explorer_SaveImageHandler(System.Drawing.Bitmap image)
        {
            this.Shell.SaveImageHandler(image);
        }

        //event handler to display automation properties of selected control from control explorer 
        void _explorer_OnSelect(string[] automationElements)
        {
            this.Shell.AutomationChild = automationElements[0];
            this.Shell.AutomationChildSubData = automationElements[1];
            this.Shell.AutomationParent = automationElements[2];
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }


        private static void HandleException(Exception ex)
        {
            if (ex == null)
                return;

            var exConfig = ConfigurationManager.GetSection("exceptionHandling");
            if(exConfig != null)
                ExceptionPolicy.HandleException(ex, "Default Policy");

            var message = ex.InnerException != null ? (!String.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message:ex.Message) : ex.Message;

            MessageBox.Show(message, "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (ex.GetType() == typeof(GenericException))
            {
                if (((GenericException)ex).ExitApplication)
                {
                    Terminate();
                }
            }
            else if (ex.GetType() == typeof(Exception))
            {
                //Terminate();
                
            }
        }

        private static void Terminate()
        {
            MessageBox.Show("Application Terminating", "IAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }


    }
}
