/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Microsoft.Practices.CompositeUI;

namespace IMSWorkBench.Infrastructure.Interface
{
    /// <summary>
    /// Represents a WorkItem that uses a WorkItem controller to perform its business logic.
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    public sealed class ControlledWorkItem<TController> : WorkItem
    {
        private TController _controller;

        /// <summary>
        /// Gets the controller.
        /// </summary>
        public TController Controller
        {
            get { return _controller; }
        }

        /// <summary>
        /// See <see cref="M:Microsoft.Practices.ObjectBuilder.IBuilderAware.OnBuiltUp(System.String)"/> for more information.
        /// </summary>
        public override void OnBuiltUp(string id)
        {
            base.OnBuiltUp(id);

            _controller = Items.AddNew<TController>();
        }
    }
}
