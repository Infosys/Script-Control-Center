/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Runtime.Serialization;
using Infosys.WEM.Export.Service.Contracts.Data;

namespace Infosys.WEM.Export.Service.Contracts.Message
{
    [DataContract]
    public class UpdateExportSourceTargetMappingReqMsg
    {
        [DataMember]
        public ExportSourceTargetMapping ExportSourceTargetMapping { get; set; }
    }
}
