/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/


namespace IMSWorkBench.Infrastructure.Interface
{
    /// <summary>
    /// Controller used by <see cref="ControlledWorkItem{TController}"/>.
    /// </summary>
    public interface IWorkItemController
    {
        /// <summary>
        /// Called when the controller is ready to run.
        /// </summary>
        void Run();
    }
}
