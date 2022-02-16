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
using System.Configuration;

using Infosys.WEM.Node.Service.Contracts.Message;
using Infosys.WEM.Node.Service.Contracts.Data;
using Infosys.ATR.AutomationEngine.Contracts;

namespace Infosys.ATR.IAPNodeChecker
{
    class Program
    {
        //default mode is non polling, so that can be even scheduled thru task scheduler
        //mode=1 => non-polling and mode=2 => polling
        static void Main(string[] args)
        {
            //args = new string[] {"mode:1"};
            //args = new string[] { "help" };
            try
            {
                bool inPolling = false;
                bool helpAsked = false;

                if (args != null && args.Length > 0)
                {
                    if (args[0].ToLower().Contains("mode:"))
                    {
                        string[] modeParts = args[0].ToLower().Split(':');
                        if (modeParts.Length == 2 && modeParts[1] == "2")
                            inPolling = true;
                    }
                    else if (args[0].ToLower().Contains("help"))
                    {
                        helpAsked = true;
                        Console.WriteLine("");
                        Console.WriteLine("IAPNodeChecker usage...");
                        Console.WriteLine("");
                        Console.WriteLine("IAPNodeChecker.exe <polling mode e.g. mode:1 or mode:2>");
                        Console.WriteLine("");
                        Console.WriteLine("This utility can be used in both polling as well as non-polling mode.");
                        Console.WriteLine("Default mode is non polling, so that it can be even scheduled through task scheduler.");
                        Console.WriteLine("For:");
                        Console.WriteLine("1. Non Polling mode-> either no argument or one argument as mode:1");
                        Console.WriteLine("2. Polling mode-> one argument as mode:2");
                        Console.WriteLine("");
                    }
                }

                if (helpAsked)
                    return;
                string domain = ConfigurationManager.AppSettings["Domain"];
                string pollingGap = ConfigurationManager.AppSettings["PollingGapInMilliseconds"];
                int iPollingGap = 60000; //default 1 minute i.e. poll after 1 minute
                int.TryParse(pollingGap, out iPollingGap);
                if (!string.IsNullOrEmpty(domain))
                {                                     
                    switch (inPolling)
                    {
                        case true:
                            while (true)
                            {
                                CheckIAPNode(domain);
                                System.Threading.Thread.Sleep(iPollingGap); 
                            }
                            break; //ignore the warning
                        case false:
                            CheckIAPNode(domain);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Error: Domain is not configured");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n Error: " + ex.Message);
                if(ex.InnerException!=null)
                    Console.WriteLine("\n" + ex.InnerException.Message);
            }
        }

        static void CheckIAPNode(string domain)
        {
            Infosys.WEM.Client.RegisteredNodes nodeclient = new WEM.Client.RegisteredNodes();
            NodeChannel iapNodeClient = null;
            string strcompany = System.Configuration.ConfigurationManager.AppSettings["Company"];
            if (string.IsNullOrEmpty(strcompany))
                strcompany = "0";
            GetRegisteredNodesResMsg activeNodes = nodeclient.ServiceChannel.GetRegisteredNodes(domain, "0", strcompany); // 0- to fetch both the workflow as well as script windows service based active agents
            if (activeNodes != null && activeNodes.Nodes != null && activeNodes.Nodes.Count > 0)
            {
                activeNodes.Nodes.ForEach(n => {
                    try
                    {
                        //if node is not reachable thru the ping, then unregister it.
                        iapNodeClient = new NodeChannel("http://" + n.HostMachineName + "." + n.HostMachineDomain + ":" + n.HttpPort + "/iap/rest");
                        var response = iapNodeClient.ServiceChannel.Ping();
                        bool isNodeUp = false;
                        if (!string.IsNullOrEmpty(response))
                            isNodeUp = true;

                        if (!isNodeUp)
                        {
                            nodeclient.ServiceChannel.UnRegister(new UnRegisterReqMsg() { Domain = n.HostMachineDomain, MachineName = n.HostMachineName });
                        }
                    }
                    catch (System.ServiceModel.EndpointNotFoundException ex)
                    {
                        Console.WriteLine("\n Error: Node: " + n.HostMachineName + " not reachable and hence un-registring."); 
                        nodeclient.ServiceChannel.UnRegister(new UnRegisterReqMsg() { Domain = n.HostMachineDomain, MachineName = n.HostMachineName });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("\n Error: " + ex.Message);
                        if (ex.InnerException != null)
                            Console.WriteLine("\n" + ex.InnerException.Message);
                    }

                });
            }
        }
    }
}
