/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.WFDesigner.Services;
using Infosys.ATR.WFDesigner.Entities;
using Infosys.ATR.WFDesigner.Constants;
using Infosys.WEM.Service.Contracts.Data;
using Infosys.WEM.Service.Contracts.Message;
using IMSWorkBench.Infrastructure.Interface.Services;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using IMSWorkBench.Infrastructure.Library.Services;
using Microsoft.Practices.CompositeUI.EventBroker;
using System.Windows.Forms;
using Infosys.WEM.Client;
using System.Configuration;

namespace Infosys.ATR.WFDesigner.Views
{
    public class TransactionPresenter : Presenter<ITransaction>
    {

        [EventPublication(EventTopicNames.ShowTransaction, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> TransactionStatusUpdate;

        public override void OnViewReady()
        {
            base.OnViewReady();

        }

        /// <summary>
        /// Close the view
        /// </summary>
        public override void OnCloseView()
        {
            base.CloseView();            
        }

        internal void GetTransactions()
        {
            //todo

            //call service to retrieve transaction view information 
            //transalte data contract received from service to transaction presentation entity(TransactionPE)

            TransactionFilter filter = new TransactionFilter() { CompanyId = int.Parse(ConfigurationManager.AppSettings["Company"]) };
            var transactionsResMsg = WFService.GetTransactions(filter);

            if (transactionsResMsg != null)
            {
                this.View.WEMTransactions = transactionsResMsg.Transactions;

                if (transactionsResMsg.IsSuccess)
                {
                    List<TransactionPE> transactions = new List<TransactionPE>();
                    if (transactionsResMsg.Transactions != null)
                    {
                        transactionsResMsg.Transactions.ForEach(trans =>
                        {
                            DateTime createdOn = DateTime.SpecifyKind(trans.CreatedOn, DateTimeKind.Utc).ToLocalTime();
                            TransactionPE tPE = new TransactionPE
                            {
                                CategoryName = trans.CategoryName, //map the category id retrieved from data contracts to its category name
                                InitiatedOn = createdOn,  //DateTime.Now,
                                Name = trans.ModuleName,
                                Module = trans.Module.ToString(),
                                Node = trans.MachineName,
                                RunningDays = TimeSpan.FromDays((DateTime.Now - createdOn).TotalDays), //subtract initiated on from current datetime in number of days metric
                                State = trans.CurrentState.ToString(),
                                Type = trans.FileType, //file extension
                                User = trans.CreatedBy,
                                TransactionId = trans.InstanceId,
                                Description = trans.Description,
                                Metadata = trans.TransactionMetadata,
                                ReferenceKey = trans.ReferenceKey,
                                WorkflowActivityBookmark = trans.WorkflowActivityBookmark
                            };

                            if (!trans.CurrentState.ToString().Equals("Resumed", StringComparison.InvariantCultureIgnoreCase))
                                transactions.Add(tPE);
                        });
                    }

                    this.View.Transactions = transactions;
                }
                else
                {
                    string strError=string.Format("{0}",transactionsResMsg.AdditionalInfo);
                    //System.Windows.Forms.MessageBox.Show(strError, "IAP - Transaction View", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }

        internal void GetCatgeories()
        {
            List<String> categories = new List<string>();
            categories.Add("-- Select Category --");
            if (this.View.Transactions!=null)
            {
                this.View.Transactions.ForEach(t =>
                {
                    if (!categories.Contains(t.CategoryName))
                        categories.Add(t.CategoryName);
                });
            }

            this.View.Categories = categories;
        }

        internal void GetModules()
        {
            List<String> modules = new List<string>();
            modules.Add("-- Select Module --");
            if (this.View.Transactions != null)
            {
                this.View.Transactions.ForEach(t =>
                {
                    if (!modules.Contains(t.Module))
                        modules.Add(t.Module);
                });
            }
            this.View.Modules = modules;
        }

        internal void GetPeriod()
        {
            List<String> period = new List<string>();
            period.Add("-- Select Period --");
            if (this.View.Transactions != null)
            {
                this.View.Transactions.ForEach(t =>
                {
                    period.Add(t.RunningDays.ToString());
                });
            }
            this.View.Period = period;
        }

        internal void GetExecutionState()
        {
            List<String> states = new List<string>();
            if (this.View.Transactions != null)
            {
                this.View.Transactions.ForEach(t =>
                {
                    states.Add(t.State);
                });
            }
            this.View.State = states;
        }

        internal void GetUsers(List<TransactionPE> transactions=null)
        {
            if (transactions == null)
                transactions = this.View.Transactions;

            List<User> users = new List<User>();
            if (transactions != null)
            {
                transactions.ForEach(t =>
                {
                    //User u = new User { Check = false,Name = t.User};
                    if (!users.Exists(x => x.Name.Equals(t.User,StringComparison.InvariantCultureIgnoreCase)))
                        users.Add(new User { Check = false, Name = t.User});
                });
            }
            this.View.Users = users;                
        }

        internal void GetArtifacts(List<TransactionPE> transactions = null)
        {
            if (transactions == null)
                transactions = this.View.Transactions;

            List<Artifact> artifacts = new List<Artifact>();
            if (transactions != null)
            {
                transactions.Select(x=>x.Name).Distinct().ToList().ForEach(t =>
                {
                    //Artifact a = new Artifact { Check = false, Name = t.Name };
                    //if (!artifacts.Exists(x => x.Name.Equals(t,StringComparison.InvariantCultureIgnoreCase)))
                        artifacts.Add(new Artifact { Check = false, Name = t});
                });
            }
            this.View.Artifacts = artifacts;
        }

        internal void GetNodes(List<TransactionPE> transactions = null)
        {
            if (transactions == null)
                transactions = this.View.Transactions;

            List<Node> nodes = new List<Node>();
            if (transactions != null)
            {
                transactions.ForEach(t =>
                {
                    //Node n = new Node { Check = false, Name = t.Node };
                    if (!nodes.Exists(x => x.Name.Equals(t.Node, StringComparison.InvariantCultureIgnoreCase)))
                        nodes.Add(new Node { Check = false, Name = t.Node });
                });
            }
            this.View.Nodes = nodes;
        }

        internal void UpdateStatus(List<TransactionPE> transactions) 
        {
            int total = transactions.Count;
            int? completed = transactions.Count(t => t.State == "Completed");
            int? inprogress = transactions.Count(t => t.State == "InProgress");
            int? paused = transactions.Count(t => t.State == "Paused");
            int? failed = transactions.Count(t => t.State == "Failed");
            int? aborted = transactions.Count(t => t.State == "Aborted");

            var status = String.Format("Transaction Details: Rowsfound {0} | "
                + "Completed {1} | In Progress {2} | Paused {3} | Failed {4} | Aborted {5} ",
                total.ToString(), completed.GetValueOrDefault(), inprogress.GetValueOrDefault(), paused.GetValueOrDefault(),
                failed.GetValueOrDefault(), aborted.GetValueOrDefault());

            TransactionStatusUpdate(this, new EventArgs<String>(status));
                
        }

        internal void RemoveStatus()
        {
            TransactionStatusUpdate(this, new EventArgs<String>(String.Empty));            
        }

        internal bool UpdateTransactionStatus(string transactionId, StateType type)
        {
            bool updateStatus = false;
            string msg = String.Format("Transaction {0} Failed", type.ToString());

            var transaction = this.View.WEMTransactions.FindAll(x => x.InstanceId.Equals(transactionId)).FirstOrDefault();

            if (transaction != null)
            {
                var result = WFService.UpdateTransaction(transaction, type);            
                if (result.IsSuccess)
                {
                    msg = String.Format("Transaction {0} Successfully", type.ToString());
                    updateStatus = true;
                    GetTransactions();
                    UpdateStatus(this.View.Transactions);
                }
            }

            System.Windows.Forms.MessageBox.Show(msg, "IAP", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            return updateStatus;
        }
    }
}
