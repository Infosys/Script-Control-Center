using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace Infosys.ATR.ScriptRepository.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public List<SubCategory> SubCategories { get; set; }
    }

    public class SubCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public List<Script> Scripts { get; set; }
    }

    public class CategorySubCategorySubset
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Script
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Script Type")]
        public string ScriptType { get; set; }
        public string ScriptLocation { get; set; }
        [DisplayName("Modified By")]
        public string ModifiedBy { get; set; }
        public string ArgumentString { get; set; }
        public string TaskCommand { get; set; }
        public string TaskType { get; set; }
        public string WorkingDir { get; set; }
        public string SubCategory { get; set; }
        public List<ScriptParameter> Parameters { get; set; }
    }

    public class ScriptSubSet
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Script Type")]
        public string ScriptType { get; set; }
        //[DisplayName("Modified By")]
        //public string ModifiedBy { get; set; }
        //public string Location { get; set; }
    }

    public class ScriptParameter
    {
        public string Name { get; set; }
        //public string Type { get; set; }
        [DisplayName("Allowed Values")]
        public string AllowedValues { get; set; }
        [DisplayName("Is Mandatory")]
        public bool IsMandatory { get; set; }
        [DisplayName("IO Type")]
        public ParameterIOTypes IOType { get; set; }
        public string DefaultValue { get; set; }
        public string ParamId { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsSecret { get; set; }
        public string ScriptId { get; set; }
        public bool IsPaired { get; set; }
    }

    public class ScriptParameterSubSet
    {
        public string Name { get; set; }
        [DisplayName("Is Mandatory")]
        public string IsMandatory { get; set; }
        //[DisplayName("Allowed Values")]
        //public string AllowedValues { get; set; }
        [DisplayName("Direction")]
        public ParameterIOTypes IOType { get; set; }
    }

    public enum ParameterIOTypes
    {
        In = 0,
        Out = 1,
        InAndOut = 2
    }

    public enum EntityProcessingTypes
    {
        Add, Update, Delete
    }
}
