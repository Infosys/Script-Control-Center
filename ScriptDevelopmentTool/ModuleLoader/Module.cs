/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Reflection;
using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Library.Services;
using Microsoft.Practices.CompositeUI.Services;

namespace Infosys.ATR.ModuleLoader
{
    public class Module : ModuleInit
    {
        private WorkItem _rootWorkItem;

        [InjectionConstructor]
        public Module([ServiceDependency] WorkItem rootWorkItem)
        {
            _rootWorkItem = rootWorkItem;
        }

        public override void Load()
        {
            base.Load();

            ControlledWorkItem<ModuleController> workItem = _rootWorkItem.WorkItems.AddNew<ControlledWorkItem<ModuleController>>();
            workItem.Controller.Run();
        }
    }
}
