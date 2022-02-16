/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.ATR.WFDesigner.Entities
{
    public class WorkflowParameterPE
    {
        public string Name { get; set; }
        public string WorkflowId { get; set; }
        public string DefaultValue { get; set; }
        public string AllowedValues { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsSecret { get; set; }
        public ParameterIOTypes ParamIOType { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; } // to be used to tell if the concerned parameter is to be deleted
        public string Id { get; set; }
        public bool IsNew { get; set; }
        public bool IsReferenceKey { get; set; }
    }

    public class WorkflowParameterSubSet
    {
        public string Name { get; set; }
        [DisplayName("Is Mandatory")]
        public string IsMandatory { get; set; }
        [DisplayName("Direction")]
        public ParameterIOTypes IOType { get; set; }
    }

    public enum ParameterIOTypes
    {
        In = 0,
        Out = 1,
        InAndOut = 2
    }
}
