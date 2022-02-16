/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Windows.Forms;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface.Services;

namespace IMSWorkBench.Infrastructure.Interface
{
    /// <summary>
    /// Base class for a WorkItem controller.
    /// </summary>
    public abstract class WorkItemController : IWorkItemController
    {
        private WorkItem _workItem;

        /// <summary>
        /// Gets or sets the work item.
        /// </summary>
        /// <value>The work item.</value>
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return _workItem; }
            set { _workItem = value; }
        }

        public IActionCatalogService ActionCatalogService
        {
            get { return _workItem.Services.Get<IActionCatalogService>(); }
        }

        public virtual void Run()
        {
        }

        /// <summary>
        /// Creates and shows a smart part on the specified workspace.
        /// </summary>
        /// <typeparam name="TView">The type of the smart part to create and show.</typeparam>
        /// <param name="workspaceName">The name of the workspace in which to show the smart part.</param>
        /// <returns>The new smart part instance.</returns>
        protected virtual TView ShowViewInWorkspace<TView>(string workspaceName)
        {
            TView view = WorkItem.SmartParts.AddNew<TView>();
            WorkItem.Workspaces[workspaceName].Show(view);
            return view;
        }

        /// <summary>
        /// Shows a specific smart part in the workspace. If a smart part with the specified id
        /// is not found in the <see cref="WorkItem.SmartParts"/> collection, a new instance
        /// will be created; otherwise, the existing instance will be re used.
        /// </summary>
        /// <typeparam name="TView">The type of the smart part to show.</typeparam>
        /// <param name="viewId">The id of the smart part in the <see cref="WorkItem.SmartParts"/> collection.</param>
        /// <param name="workspaceName">The name of the workspace in which to show the smart part.</param>
        /// <returns>The smart part instance.</returns>
        protected virtual TView ShowViewInWorkspace<TView>(string viewId, string workspaceName)
        {
            TView view = default(TView);
            if (WorkItem.SmartParts.Contains(viewId))
            {
                view = WorkItem.SmartParts.Get<TView>(viewId);
            }
            else
            {
                view = WorkItem.SmartParts.AddNew<TView>(viewId);
            }

            WorkItem.Workspaces[workspaceName].Show(view);

            return view;
        }
    }
}
