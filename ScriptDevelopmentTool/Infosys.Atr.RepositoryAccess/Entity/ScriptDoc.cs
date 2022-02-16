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

namespace Infosys.ATR.RepositoryAccess.Entity
{
    public class ScriptDoc
    {
        /// <summary>
        /// Script file stream data
        /// </summary>
        public Stream File { get; set; }
        /// <summary>
        /// Script container name as "S_[company id]_[script id]"
        /// </summary>
        public string ScriptContainer { get; set; }
        /// <summary>
        /// Script Ver number
        /// </summary>
        public int ScriptVer { get; set; }
        /// <summary>
        /// StorageBaseURL
        /// </summary>
        public string StorageBaseURL { get; set; }
        /// <summary>
        /// Filename as "[script name]_[version].ext" ; ext- as applicable e.g.- vbs, bat, etc
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// CompanyId
        /// </summary>
        public string CompanyId { get; set; }
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
        /// <summary>
        /// Final http url
        /// </summary>
        public string ScriptUrl { get; set; }
        /// <summary>
        /// Flag to tell if the downloading succeeded
        /// </summary>
        public bool IsDownloadSuccessful { get; set; }
        /// <summary>
        /// The error may occur while downloading
        /// </summary>
        public string AnyError { get; set; }
    }
}
