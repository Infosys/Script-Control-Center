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
// The ISolutionProfileParser interface is used along with the XmlStreamDependentModuleEnumerator
// to provide parsing services of a profile catalog xml. It returns an array of IModuleInfo
// 
//  
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using Microsoft.Practices.CompositeUI.Configuration;

namespace IMSWorkBench.Infrastructure.Library.Services
{
    public interface ISolutionProfileParser
    {
        IModuleInfo[] Parse(string xml);
    }
}
