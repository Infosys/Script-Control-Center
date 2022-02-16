/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using InfrastructureClientLibrary;
using System.Web.Script.Serialization;
using IAP.Infrastructure.Services.Contracts;

namespace InfrastructureTest
{
    [TestClass]
    public class InfrastructureClientTest
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void TestLoggerClient_Notify()
        {
            string resource_url = "http://localhost/iapinfrastructureservices/Logger.svc/Notify/";
            LoggerClient target = new LoggerClient();
            Log log = new Log();
            log.TicketNumber = "ABCD1";
            log.CreatedBy = "admin";
            log.MachineName = "localhost";
            log.Message = "Automation started";
            log.NotificationType =(NotificationType.Custom);
            log.ScriptId = "jhdkuwef7656";
            log.Recipients = "XXXXX";
            log.ErrorDetails = "none";
            log.ErrorTypeID = 2;
            log.MachineIP = "127.0.0.1";
            log.StateId = 4;
            log.CustomNotificationSubject = "Prebuild completed successfully";
            log.CustomNotificationBody = "Hi Suchi, <br/><br/>Prebuild data: <br/>RAM is 16 GB <br/>C drive is 80 GB free<br/><br/>Regards, <br/> Suchi";
            log.AttachmentFileNames = new System.Collections.Generic.List<string>() { @"D:\Test\shuchi1.txt", @"D:\Test\shuchi2.txt" };
            bool result = target.Notify(resource_url, log);
        }

        [TestMethod]
        public void TestLoggerClient_LogError()
        {
            string resource_url = "http://localhost/iapinfrastructureservices/Logger.svc/LogError/";

           


            //string resource_url = "http://localhost:58325/Logger.svc/LogError";
            LoggerClient target = new LoggerClient();
            Log log = new Log();
            // log.TicketNumber = "ITSM7878";
            log.CreatedBy = "admin";
            log.MachineName = "127.0.0.1";
            log.ScriptId = "jhdkuwef7656";
            log.ErrorDetails = "none";
            log.ErrorTypeID = 1;
            log.Recipients = "XXXXXX";
            log.Message = "Ticket processing failed with Critical error";
            log.NotificationType = (NotificationType.Error);
            log.MachineIP = "127.0.0.1";
            //log.StateId = 3;
            target.LogError(resource_url, log);
        }

        [TestMethod]
        public void TestLoggerClient_LogAudit()
        {
            string resource_url = "http://localhost/iapinfrastructureservices/Logger.svc/LogAudit/";
            LoggerClient target = new LoggerClient();
            Log log = new Log();
            log.TicketNumber = "ITSM7878";
            log.CreatedBy = "admin";
            log.StateId = 3;

            List<LogData> data = new List<LogData>();
            LogData dat1 = new LogData();
            dat1.Key = "word doc 2";
            dat1.Value = "found";
            LogData dat2 = new LogData();
            dat2.Key = "word doc 2 read";
            dat2.Value = "true";
            data.Add(dat1);
            data.Add(dat2);

            log.Data = data;
            log.MachineName = "127.0.0.1";
            log.MachineIP = "127.0.0.1";
            log.Message = "Access has been given to the user";
            log.ScriptId = "jhdkuwef7656";
            target.LogAudit(resource_url, log);
        }

        [TestMethod]
        public void TestTicketHandlerClient_AddTicket()
        {
      
           
            string resource_url = "http://localhost:58325/TicketHandler.svc";
            

            Ticket ticket = new Ticket();
            ticket.Reason = "Internal user access";
            ticket.TicketNumber = "12IM0851009";
            ticket.StateId = 1;
            ticket.Priority = 1;
            ticket.LastModifiedFromMachineIP = "127.0.0.1";
            ticket.LastModifiedFromMachineName = "127.0.0.1";
            ticket.Remarks = "processing.. ticket";
            ticket.LastModifiedBy = @"admin";
            ticket.StatusUpdatedDate = DateTime.Now;
            ticket.CreatedFromMachineIP = "127.0.0.1";
            ticket.CreatedFromMachineName = "127.0.0.1";
            ticket.CreatedBy = @"admin";

            //ticket.Data1 = @"[{""Name"":""FirstApproval"",""Data"":[{""Key"":""Response"",""Value"":""Approved""}]}]";


            List<TicketData> data = new List<TicketData>();
            TicketData d1 = new TicketData();
            KeyValueCustomType cd1 = new KeyValueCustomType();
            cd1.Key = "Response";
            cd1.Value = "AAA";
            d1.Name = "FirstApproval";
            d1.Data = new List<KeyValueCustomType>();
            d1.Data.Add(cd1);
            data.Add(d1);
            ticket.Data = data;

            TicketHandlerClient.AddTicket(resource_url, ticket);

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.MaxJsonLength = int.MaxValue;
            //string responseData = RestHelper.CallServicePOST(resource_url, serializer.Serialize(ticket));
            string succ = "";
        }

        [TestMethod]
        public void TestTicketHandlerClient_UpdateTicket()
        {
            string resource_url = "http://localhost/IAPInfrastructureServices/TicketHandler.svc/UpdateTicket/";
            Ticket ticket = new Ticket();
            ticket.Reason = "Internal user access";
            ticket.TicketNumber = "IM0851000";
            ticket.StateId = 6;
            ticket.Priority = 1;
            ticket.LastModifiedFromMachineIP = "127.0.0.1";
            ticket.LastModifiedFromMachineName = "127.0.0.1";
            ticket.Remarks = "processing.. ticket";
            ticket.LastModifiedBy = @"admin";
            ticket.StatusUpdatedDate = DateTime.Now;
            List<TicketData> data = new List<TicketData>();
            TicketData d1 = new TicketData();
            KeyValueCustomType cd1 = new KeyValueCustomType();
            cd1.Key = "HardDisk";
            cd1.Value = "16GB";
            d1.Name = "Actual";
            d1.Data = new List<KeyValueCustomType>();
            d1.Data.Add(cd1);
            data.Add(d1);

            ticket.Data = data;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string responseData = CallServicePOST(resource_url, serializer.Serialize(ticket));
        }

        [TestMethod]
        public void TestTicketHandlerClient_GetTicketById()
        {

            string resource_url = "http://localhost/iapinfrastructureservices/TicketHandler.svc/GetTicketById?ticketId=IM0851000";
            Response<Ticket> actual = null;
            //actual = TicketHandlerClient.GetTicketById(resource_url);
            //int a = 10;
            //int b = a + 10;
        }

        [TestMethod()]
        public void GetNextUnAssignedTicketTest()
        {
            string resource_url = "http://localhost/iapinfrastructureservices/TicketHandler.svc/GetNextUnAssignedTicket/";
            Response<Ticket> actual = TicketHandlerClient.GetNextUnAssignedTicket(resource_url);
            int a = 10;
            int b = a + 10;
        }

        [TestMethod()]
        public void GetNextUnAssignedTicketByStateTest()
        {
            string resource_url = "http://localhost/iapinfrastructureservices/TicketHandler.svc/GetNextUnAssignedTicketByState?stateId=2";
            //Response<Ticket> actual = TicketHandlerClient.GetNextUnAssignedTicketByState(resource_url);
        }

        [TestMethod()]
        public void GetNextUnAssignedTicketByStatesTest()
        {
            string resource_url = "http://localhost/iapinfrastructureservices/TicketHandler.svc/GetNextUnAssignedTicketByStates";
            int[] states = { 1 };
            Response<Ticket> actual = TicketHandlerClient.GetNextUnAssignedTicketByStates(resource_url, states);
            Ticket tc = actual.Results;
        }

        [TestMethod]
        public void TestTicketHandlerClient_GetModule()
        {
           
        }

        [TestMethod]
        public void TestCsvRead()
        {
            string path = @"D:\ATR-FEA\Code\ATR\InfrastructureClientLibrary\Tickets.csv";
            Dictionary<string, int> mapper = new Dictionary<string, int>();
            mapper.Add("TicketNumber", 0);
            mapper.Add("Reason", 1);
            mapper.Add("StateId", 2);
            mapper.Add("Description", 4);
            mapper.Add("LastAction", 6);
            mapper.Add("StatusUpdatedDate", 5);
            mapper.Add("Data", 7);
            mapper.Add("Priority", 3);
            //List<Ticket> tickets = CsvHelper.ReadCsv2(path, mapper);
            //int i = tickets.Count;
        }

        [TestMethod]
        public void TestCsvWrite()
        {
            string path = @"D:\ATR-FEA\Code\ATR\InfrastructureClientLibrary\Tickets.csv";
            bool result = CsvHelper.UpdateCsv(path, "6", "IM0851000");
        }

        private static void WebClient(string resource_url, string machineName)
        {
            Ticket ticket = new Ticket();
            ticket.Reason = "internal user access";
            ticket.TicketNumber = "RF3848785bb";
            ticket.StateId = 5;
            ticket.CreatedFromMachineIP = "127.0.0.1";
            ticket.CreatedFromMachineName = machineName;
            ticket.Remarks = "processing.. ticket";
            ticket.CreatedBy = "fareast\v-supaha";
            ticket.LastModifiedFromMachineIP = "127.0.0.1";
            ticket.LastModifiedFromMachineName = machineName;
            ticket.Remarks = "processing.. ticket";
            ticket.LastModifiedBy = "fareast\v-supaha";
            ticket.StatusUpdatedDate = DateTime.Now;
            List<TicketData> data = new List<TicketData>();
            TicketData d1 = new TicketData();
            KeyValueCustomType cd1 = new KeyValueCustomType();
            cd1.Key = "RAM";
            cd1.Value = "16";
            //d1.Data = new List<CustomDictionary>();
            //d1.Data.Add(cd1);
            data.Add(d1);

            //ticket.Data = data;


            //ticket.LastAction = 3;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = int.MaxValue;
            string responseData = CallServicePOST(resource_url, serializer.Serialize(ticket));
            Console.WriteLine(machineName + ":" + responseData);
            //Console.Read();
        }

        private static string CallServicePOST(string url, string parameters)
        {
            var request = WebRequest.Create(url);
            //request.PreAuthenticate = true;
            //request.Proxy = System.Net.WebRequest.GetSystemWebProxy();
            //request.UseDefaultCredentials = true;
            //request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.Method = "POST";
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(parameters);
            request.ContentType = "application/json";
            request.ContentLength = byteData.Length;
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;
                Console.WriteLine("Response status code: " + response.StatusCode);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }
    }
}
