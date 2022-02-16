/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IMSWorkBench.Infrastructure.Interface.Constants;

namespace Infosys.ATR.Admin.Constants
{
    public class EventTopicNames : IMSWorkBench.Infrastructure.Interface.Constants.EventTopicNames
    {
        public const string ShowGroupDetails = "ShowGroupDetials";
        public const string ShowSemanticCluster = "ShowSemanticCluster";
        public const string ShowGroupExplorerDetails = "ShowGroupExplorerDetials";
        public const string UpdateSematicTree ="UpdateSematicTree";
        public const string RefreshExplorerCategories = "RefreshExplorerCategories";
        public const string AddGroupUsers = "AddGroupUsers";
        public const string RefreshGroupUsers = "RefreshGroupUsers";
        public const string RefreshUsers = "RefreshUsers";
      
    }
}
