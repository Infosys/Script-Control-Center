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


namespace Infosys.WEM.Service.Contracts.Message
{
    [DataContract]
    public class PublishObjectModelReqMsg
    {
        [DataMember(IsRequired = true)]
        public Guid ObjectModelId { get; set; }

        [DataMember(Name = "CategoryID", IsRequired = true)]
        public int CategoryID { get; set; }

        [DataMember(IsRequired = true)]
        public int ObjectModelVer { get; set; }

        [DataMember(Name = "ObjectModelName", IsRequired = true)]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember(IsRequired = true)]
        public string ObjectModelURI { get; set; }

        [DataMember(IsRequired = true)]
        public string ImageURI { get; set; }

        [DataMember]
        public string ClientId { get; set; }

        [DataMember]
        public string ClientVer { get; set; }

        [DataMember]
        public string SrcMachineName { get; set; }

        [DataMember]
        public string SrcIPAddr { get; set; }

        [DataMember(IsRequired = true)]
        public string CreatedBy { get; set; }

        [DataMember(IsRequired = true)]
        public int FileSizeInKb { get; set; }

    }
}
