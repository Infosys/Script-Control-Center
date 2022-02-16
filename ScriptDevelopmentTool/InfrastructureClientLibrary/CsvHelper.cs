/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using IAP.Infrastructure.Services.Contracts;

namespace InfrastructureClientLibrary
{
    public static class CsvHelper
    {
        private static Dictionary<int, string> MapperPriority { get; set; }

        public static List<Ticket> ReadCsv(string filePath, Dictionary<string, int> mapperColumn, Dictionary<int, string> mapperPriority)
        {
            //extractor_priority_handling=High,0,Medium,1,Low,2
            int iTicketNumber = Convert.ToInt32(mapperColumn["TicketNumber"]);
            int iReason = Convert.ToInt32(mapperColumn["Reason"]);
            int iStateId = Convert.ToInt32(mapperColumn["StateId"]);
            int iDescription = Convert.ToInt32(mapperColumn["Description"]);
            int iLastAction = Convert.ToInt32(mapperColumn["LastAction"]);
            int iStatusUpdatedDate = Convert.ToInt32(mapperColumn["StatusUpdatedDate"]);
            int iPriority = Convert.ToInt32(mapperColumn["Priority"]);


            List<Ticket> tckts = new List<Ticket>();
            Ticket tckt = null;
            string[] allLines = File.ReadAllLines(filePath);
            {

                var query = from line in allLines
                            let data = line.Split(',')
                            select new
                            {
                                TicketNumber = data[iTicketNumber],
                                Reason = data[iReason],
                                StateId = data[iStateId],
                                Priority = data[iPriority],
                                Remarks = data[iDescription],
                                StatusUpdatedDate = data[iStatusUpdatedDate],
                                LastAction = data[iLastAction]
                            };

                Boolean isHeader = true;
                int j = 0;
                foreach (var dat in query)
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        j += 1;
                        continue;
                    }
                    tckt = new Ticket();
                    tckt.CreatedBy = Environment.UserName;
                    tckt.CreatedFromMachineIP = GetIP();
                    tckt.CreatedFromMachineName = Environment.MachineName;
                    if (!string.IsNullOrEmpty(dat.LastAction))
                        tckt.LastAction = Convert.ToInt32(dat.LastAction);

                    tckt.StateId = Convert.ToInt32(dat.StateId);

                    if (Convert.ToInt32(TicketState.New) == tckt.StateId)
                    {
                        tckt.LastModifiedFromMachineName = null;
                        tckt.LastModifiedBy = null;
                        tckt.LastModifiedFromMachineIP = null;
                    }
                    else
                    {
                        tckt.LastModifiedFromMachineName = Environment.MachineName;
                        tckt.LastModifiedBy = Environment.UserName;
                        tckt.LastModifiedFromMachineIP = GetIP();
                    }

                    if (MapperPriority != null)
                    {
                        if (MapperPriority.Count() > 0)
                            tckt.Priority = GetPriority(dat.Priority);
                    }
                    tckt.Reason = dat.Reason;
                    tckt.Remarks = dat.Remarks;

                    tckt.StatusUpdatedDate = Convert.ToDateTime(dat.StatusUpdatedDate);
                    tckt.TicketNumber = dat.TicketNumber;
                    tckts.Add(tckt);
                }
            }
            return tckts;
        }

        private static int GetPriority(string p)
        {
            var priority = MapperPriority.FirstOrDefault(x => x.Value.ToLower() == p.ToLower()).Key;
            return Convert.ToInt32(priority);
        }

        public static bool UpdateCsv(string filePath, string state, string ticketNumber)
        {
            bool isSucess = false;
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                var split = line.Split(',');
                if (split[0].Contains(ticketNumber))
                {
                    split[2] = state;
                    line = string.Join(",", split);
                    lines[i] = line;
                }
            }
            File.WriteAllLines(filePath, lines);
            return isSucess;
        }

        private static string GetIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
