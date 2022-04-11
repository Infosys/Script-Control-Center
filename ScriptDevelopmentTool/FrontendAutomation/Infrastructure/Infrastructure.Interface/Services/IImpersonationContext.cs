/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance Package
//
// This file was generated by this guidance package as part of the solution template
//
// The IImpersonationContext interface is used along with the IImpersonationService
// in order to mantain the state and revert the impersonation process
//
//  
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using System;

namespace IMSWorkBench.Infrastructure.Interface.Services
{
    public interface IImpersonationContext : IDisposable
    {
        object State { get; set; }
        void Undo();
    }
}