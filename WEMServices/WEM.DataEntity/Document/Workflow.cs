/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.WEM.Resource.Entity.Document
{
    public class Workflow
    {
        /// <summary>
        /// Workflow/Image file stream data
        /// </summary>
        public Stream File { get; set; }
        /// <summary>
        /// Is a WorkflowId (GUID)
        /// </summary>
        public string WorkflowId { get; set; }
        /// <summary>
        /// WorkflowVer number
        /// </summary>
        public int WorkflowVer { get; set; }
        /// <summary>
        /// StorageBaseURL
        /// </summary>
        public string StorageBaseURL { get; set; }
        /// <summary>
        /// Filename
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// CompanyId
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// Uploaded by
        /// </summary>
        public string UploadedBy { get; set; }
        /// <summary>
        /// Status Message
        /// </summary>
        public string StatusMessage { get; set; }
        /// <summary>
        /// Status Code
        /// </summary>
        public int StatusCode { get; set; }


    }
}
