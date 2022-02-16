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

namespace Infosys.IAP.CommonClientLibrary.Models
{
    public class ContentMeta
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ModuleType ModuleType { get; set; }
        [DisplayName("Content Type")]
        public string ContentType { get; set; } 
        public string ModuleLocation { get; set; } 
        public string ArgumentString { get; set; }
        public string TaskCommand { get; set; }
        public string TaskType { get; set; }
        public string WorkingDir { get; set; }
        public string IfeaScriptName { get; set; }
        public string CallMethod { get; set; }
        public bool RunAsAdmin { get; set; }
        public bool UsesUIAutomation { get; set; }
        public string SourceUrl { get; set; }
        public string Tags { get; set; }
        public string LicenseType { get; set; }
        public List<ContentParameter> Parameters { get; set; }
        public Guid WorkflowID { get; set; }
        public string WorkflowURI { get; set; }
        public int Version { get; set; }
        public bool isGeneratedScript { get; set; }
    }
    public class ContentParameter
    {
        public string Name { get; set; }
        
        [DisplayName("Allowed Values")]
        public string AllowedValues { get; set; }
        [DisplayName("Is Mandatory")]
        public bool IsMandatory { get; set; }
        [DisplayName("IO Type")]
        public ParameterIOTypes IOType { get; set; }
        public string DefaultValue { get; set; }
        public bool IsSecret { get; set; }
        public string ScriptId { get; set; }
        public bool IsUnnamed { get; set; }
        public string DataType { get; set; }
        public bool IsReferenceKey { get; set; }      

    }    
    public enum ParameterIOTypes
    {
        In = 0,
        Out = 1,
        InAndOut = 2
    }
    public enum ModuleType
    { 
        Workflow,
        Script
    }
}
