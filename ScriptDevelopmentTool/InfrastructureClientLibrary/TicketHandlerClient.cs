/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using IAP.Infrastructure.Services.Contracts;
using InfrastructureClientLibrary;
using System;
using System.Net;
using System.Web.Script.Serialization;

namespace InfrastructureClientLibrary
{
    public static class TicketHandlerClient
    {

        public static Response<bool> AddTicket(string resourceUrl, Ticket ticket)
        {
            if (string.IsNullOrEmpty(ticket.CreatedBy))
            {
                ticket.CreatedBy = Environment.UserName;
            }
            if (string.IsNullOrEmpty(ticket.CreatedFromMachineIP))
            {
                ticket.CreatedFromMachineIP = GetIP();
            }
            if (string.IsNullOrEmpty(ticket.CreatedFromMachineName))
            {
                ticket.CreatedFromMachineName = Environment.MachineName;
            }

            TicketHandlerService service = new TicketHandlerService(resourceUrl);
            var data = service.ServiceChannel.AddTicket(ticket);
            return data;
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

        public static Response<Ticket> GetTicketById(string resourceUrl, string ticketId)
        {
            Response<Ticket> data = null;

            TicketHandlerService service = new TicketHandlerService(resourceUrl);
            data = service.ServiceChannel.GetTicketById(ticketId);
            return data;
        }

        public static Response<bool> UpdateTicket(string resourceUrl, Ticket ticket)
        {
            TicketHandlerService service = new TicketHandlerService(resourceUrl);
            return service.ServiceChannel.UpdateTicket(ticket);
        }

        public static Response<Module> GetModule(string resourceUrl, string ticketReason)
        {
            TicketHandlerService service = new TicketHandlerService(resourceUrl);
            return service.ServiceChannel.GetModule(ticketReason); 
        }

        public static Response<Ticket> GetNextUnAssignedTicket(string resourceUrl)
        {
            TicketHandlerService service = new TicketHandlerService(resourceUrl);
            return service.ServiceChannel.GetNextUnAssignedTicket(); 
        }

        public static Response<Ticket> GetNextUnAssignedTicketByStates(string resourceUrl, int[] states)
        {
            TicketHandlerService service = new TicketHandlerService(resourceUrl);
            return service.ServiceChannel.GetNextUnAssignedTicketByStates(states); 
        }
    }
}
