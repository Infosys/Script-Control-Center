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

using Infosys.ATR.WFDesigner.Entities;

namespace Infosys.ATR.WFDesigner.Views
{
    public interface ITransaction
    {
        List<TransactionPE> Transactions { get; set; }
        List<String> Categories { get; set; }
        List<String> Period { get; set; }
        List<String> State { get; set; }
        List<String> Modules { get; set; }
        List<User> Users {get;set;}
        List<Artifact> Artifacts { get; set; }
        List<Node> Nodes { get; set; }
        List<WEM.Node.Service.Contracts.Data.Transaction> WEMTransactions { get; set; }       
    }
}
