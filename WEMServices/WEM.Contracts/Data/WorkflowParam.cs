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
using System.Runtime.Serialization;

namespace Infosys.WEM.Service.Contracts.Data
{
    [DataContract]
    public class WorkflowParam
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ParamId { get; set; }
        [DataMember]
        public string WorkflowId { get; set; }
        [DataMember]
        public string DefaultValue { get; set; }
        [DataMember]
        public string AllowedValues { get; set; }
        [DataMember]
        public bool IsMandatory { get; set; }
        [DataMember]
        public bool IsSecret { get; set; }
        [DataMember]
        public ParamDirection ParamType { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public bool IsNew { get; set; }
        [DataMember]
        public bool IsReferenceKey { get; set; }
    }
    public enum ParamDirection
    {
        In,
        Out,
        InAndOut
    }
}
