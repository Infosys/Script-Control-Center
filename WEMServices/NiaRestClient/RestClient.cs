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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Nia.Services
{
    public class RestClient
    {
        private String userName;
        private String password;
        private String casServer;
        private String casService;
        private HttpClient client;
        //private RestClient niaRestClient = null;
        public String TGTToken;
        private int ticketCount = 0;

        private RestClient(String userName, String password, String casServerURLPrefix, String appServerURL)
        {
            password = Infosys.WEM.Infrastructure.SecurityCore.SecureData.UnSecure(password, "IAP2GO_SEC!URE");
            this.userName = userName;
            this.password = password;
            this.casServer =casServerURLPrefix;
            this.casService = appServerURL;
            this.client = new HttpClient();
            string token = getTicketGrantingTicket();
            this.TGTToken = token;
        }
        public Boolean getServiceCallStatus(string token)
        {
            String st = getServiceTicket(token);
            Boolean status = getServiceCall(st);
            return status;
        }
        public static RestClient getInstance(String userName, String password, String casServer, String casService)
        {
            return new RestClient(userName, password, casServer, casService);
        }

        public String getTicket()
        {
            String grantingTicket = getTicketGrantingTicket();
            String serviceTicket = getServiceTicket(grantingTicket);
            getServiceCall(serviceTicket);
            return serviceTicket;
        }

        public string getTicketGrantingTicket()
        {
            string result = "";
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                            (
                               delegate { return true; }
                            );

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("username", userName));
                postData.Add(new KeyValuePair<string, string>("password", password));
                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                HttpResponseMessage response = client.PostAsync(new Uri(casServer), content).Result;
                if (response.IsSuccessStatusCode)
                {
                    result = response.Headers.Location.Segments.LastOrDefault();
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public string getServiceTicket(string grantingTicket)
        {
            int retryCount = 1;
            try
            {
                while (((null == grantingTicket)) && (retryCount <= 2))
                {
                    this.TGTToken = getTicketGrantingTicket();
                    grantingTicket = this.TGTToken;
                    retryCount++;

                    if ((null != grantingTicket))
                        break;
                }
                if ((null == grantingTicket))
                {
                    throw new Exception("IIP_4003 - Error in creating TGT token");
                }

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                            (
                               delegate { return true; }
                            );
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("service", casService));

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                HttpResponseMessage response = client.PostAsync(new Uri(casServer + "/" + grantingTicket), content).Result;

                if (response.IsSuccessStatusCode)
                {
                    this.ticketCount = 0;
                    return response.Content.ReadAsStringAsync().Result;
                }
                if (this.ticketCount < 5)
                {
                    this.ticketCount += 1;
                    this.TGTToken = getTicketGrantingTicket();
                    return getServiceTicket(this.TGTToken);
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public Boolean getServiceCall(string serviceTicket)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                            (
                               delegate { return true; }
                            );
                Uri url = new Uri(casService + "?" + "ticket=" + serviceTicket);
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public string getResponseOfGET(string restUrl)
        {
            try
            {
               // String st = getServiceTicket(this.TGTToken);
                //Boolean status = getServiceCall(st);
                //if (status)
                //{
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                                (
                                   delegate { return true; }
                                );
                    Uri url = new Uri(restUrl);
                    HttpResponseMessage response = this.client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
               // }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public string getResponseOfPOST(string restUrl, string requestBody)
        {
            try
            {
                //String st = getServiceTicket(this.TGTToken);
                //Boolean status = getServiceCall(st);
                //if (status)
                //{
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                                (
                                   delegate { return true; }
                                );
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                    HttpResponseMessage response = client.PostAsync(restUrl, new StringContent(requestBody, Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public string getResponseOfPOST(string restUrl, string requestBody, Dictionary<string, string> requestHeaderMap)
        {
            try
            {
                //String st = getServiceTicket(this.TGTToken);
                //Boolean status = getServiceCall(st);
                //if (status)
                //{
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                                (
                                   delegate { return true; }
                                );
                    if ((requestHeaderMap != null) && (requestHeaderMap.Count > 0))
                    {
                        foreach (KeyValuePair<string, string> entry in requestHeaderMap)
                        {
                            //client.DefaultRequestHeaders.Add(entry.Key, entry.Value);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(entry.Value));
                        }
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    }

                    HttpResponseMessage response = client.PostAsync(restUrl, new StringContent(requestBody, Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                //}
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public string AddNiaScript(string url, string fileParamName)
        {
            try
            {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                            (
                               delegate { return true; }
                            );

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.PostAsync(url, new StringContent(fileParamName, Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.ToString());
            }
            return null;
        }
    }
}
