/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace Infosys.WEM.Scripts.Service.Contracts.Data
{
    [DataContract]
    public class Script
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int ScriptId { get; set; }
        [DataMember]
        public string ScriptType { get; set; }
        [DataMember]
        public string TaskType { get; set; }
        [DataMember]
        public string TaskCmd { get; set; }
        [DataMember]
        public string ScriptURL { get; set; }
        [DataMember]
        public string ArgString { get; set; }
        [DataMember]
        public string WorkingDir { get; set; }
        /// <summary>
        /// Should be the Id of the company/organization
        /// </summary>
        [DataMember]
        public string BelongsToOrg { get; set; }
        [DataMember]
        public string BelongsToAccount { get; set; }
        [DataMember]
        public string BelongsToTrack { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public List<ScriptParam> Parameters { get; set; }
        /// <summary>
        /// In case of script file, assign the content of the file
        /// </summary>
        [DataMember]
        public byte[] ScriptContent { get; set; }
        ///// <summary>
        ///// In case of script file, assign the type of the file i.e. the extension of the file e.g. .bat, .vbs, etc
        ///// </summary>
        //[DataMember]
        //public string ScriptFileType { get; set; }
        [DataMember]
        public int ScriptFileVersion { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public string StorageBaseUrl { get; set; }
        [DataMember]
        public bool RunAsAdmin { get; set; }
        [DataMember]
        public bool UsesUIAutomation { get; set; }
        [DataMember]
        public string IfeaScriptName { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        [DataMember]
        public string CallMethod { get; set; }
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public string LicenseType { get; set; }
        [DataMember]
        public string SourceUrl { get; set; }

        [DataMember]
        public string ExternalReferences { get; set; }
    }
}
