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

using Infosys.WEM.Node.Service.Contracts.Data;

namespace Infosys.WEM.Node.Service.Contracts.Message
{
    [DataContract]
    public class GetTransactionsResMsg
    {
        [DataMember]
        public List<Transaction> Transactions { get; set; }
        [DataMember]
        public string AdditionalInfo { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
    }
}
