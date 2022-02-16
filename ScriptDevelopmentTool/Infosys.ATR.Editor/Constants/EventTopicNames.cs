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

namespace Infosys.ATR.Editor.Constants
{
    public class EventTopicNames : IMSWorkBench.Infrastructure.Interface.Constants.EventTopicNames
    {
        public const string Capture = "Capture";
        public const string SaveImage = "SaveImage";
        public const string Clear = "Clear";
        public const string UpdateImage = "UpdateImage";
        public const string NodeAdded = "NodeAdded";
        public const string UpdateBaseDir = "UpdateBaseDir";
        public const string BaseDir = "BaseDir";
        public const string UpdateName = "UpdateName";
        public const string SetIFEAPath = "SetIFEAPath";
        public const string UpdateAppProperties = "UpdateAppProperties";
        public const string UpdateControlProperties = "UpdateControlProperties";
        public const string UpdateApplication = "UpdateApplication";
        public const string CopyControl = "CopyControl";
        public const string TerminateApp = "TerminateApp";

    }
}
