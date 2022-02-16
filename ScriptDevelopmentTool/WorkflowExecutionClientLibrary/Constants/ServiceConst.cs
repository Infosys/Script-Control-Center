/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Client.Constants
{
     public class ServiceConst 
    {
//for debug env        
#if DEBUG
                 public const string sUrl = "{0}/{1}.svc";   
        #else
                 public const string sUrl =  "{0}/iapwemservices/{1}.svc";                 
        #endif
    }
}
