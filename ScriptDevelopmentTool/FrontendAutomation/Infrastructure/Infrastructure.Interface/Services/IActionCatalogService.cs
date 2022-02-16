/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance Package
//
// The IActionCatalogService defines the ability to conditionally execute code based upon 
// aspects of a program that can change at run time 
//
//  
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using Microsoft.Practices.CompositeUI;

namespace IMSWorkBench.Infrastructure.Interface.Services
{
    public delegate void ActionDelegate(object caller, object target);

    public interface IActionCatalogService
    {
        bool CanExecute(string action, WorkItem context, object caller, object target);
        bool CanExecute(string action);
        void Execute(string action, WorkItem context, object caller, object target);

        void RegisterSpecificCondition(string action, IActionCondition actionCondition);
        void RegisterGeneralCondition(IActionCondition actionCondition);
        void RemoveSpecificCondition(string action, IActionCondition actionCondition);
        void RemoveGeneralCondition(IActionCondition actionCondition);

        void RemoveActionImplementation(string action);
        void RegisterActionImplementation(string action, ActionDelegate actionDelegate);
    }
}
