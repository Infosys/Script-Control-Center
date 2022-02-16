/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace Infosys.ATR.WFDesigner.Entities
{
    //public class Category
    //{
    //    private List<Category> _sub = new List<Category>();
    //    public int Key { get; set; }
    //    public string Value { get; set; }
    //    public List<Category> SubCategory
    //    { get { return _sub; } set { _sub = value; } }
    //}

    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public int ParentId { get; set; }
        public int CompanyId { get; set; }
    }

    //public class SubCategory
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string CreatedBy { get; set; }
    //    public string ModifiedBy { get; set; }
    //    public string ParentId { get; set; }
    //    public List<WorkflowPE> WFPE { get; set; }
    //}

    public class CategorySE
    {
        public int ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
    }

    public class CategoryPE
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; }
        public int CompanyId { get; set; }
        //public List<CategoryPE> SubCategory { get; set; }
    }

    public class WorkflowPE
    {
        [Browsable(false)]
        public Guid WorkflowID { get; set; }
        [Browsable(false)]
        public int CategoryID { get; set; }
        //[Browsable(false)]
        //public int SubCategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [ReadOnly(true)]
        public int WorkflowVersion { get; set; }
        [ReadOnly(true)]
        public string WorkflowURI { get; set; }
        [Browsable(false)]
        public string ImageURI { get; set; }
        [Browsable(false)]
        public DateTime PublishedOn { get; set; }
        [Browsable(false)]
        public string CreatedBy { get; set; }
        [Browsable(false)]
        public string SrcIPAddr { get; set; }
        [Browsable(false)]
        public string ClientId { get; set; }
        [Browsable(false)]
        public string ClientVer { get; set; }
        [Browsable(false)]
        public string SrcMachineName { get; set; }
        [Browsable(false)]
        public int FileSize { get; set; }
        [Browsable(false)]
        public bool IsActive { get; set; }
        [Browsable(false)]
        public string LastModifiedBy { get; set; }
        [Browsable(false)]
        public string LastModifiedOn { get; set; }
        [Browsable(false)]  
        public string FileName { get; set; }
        [Browsable(false)]  
        public int CompanyId { get; set; }
        [Browsable(false)]
        public string PartitionKey { get; set; }
        [Browsable(false)]
        public bool IncrementVersion { get; set; }
        [Browsable(false)]
        public bool UsesUIAutomation { get; set; }
        [Browsable(false)]
        public byte[] WFContent { get; set; }
        [Browsable(false)]
        public bool Modified { get; set; }
        public List<WorkflowParameterPE> Parameters { get; set; }
        [Browsable(false)]
        public bool IslongRunningWorkflow { get; set; }
        [Browsable(false)]
        public int IdleStateTimeout { get; set; }        
        [Browsable(false)]
        public string SourceUrl { get; set; }
        [Browsable(false)]
        public string Tags { get; set; }
        [Browsable(false)]
        public string LicenseType { get; set; }
        
    }
}

