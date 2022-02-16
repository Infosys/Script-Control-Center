/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.WEM.AutomationTracker.Contracts.Message;
using Infosys.WEM.Infrastructure.Common;
using Infosys.WEM.Observer.Contracts.Message;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;

namespace Infosys.WEM.Service.Implementation.Translators.Observer
{
    public class NotificationCallBack
    {

        public async Task<NotificationOutput> NotificationCall(NotificationInput value)
        {
            NotificationOutput output = new NotificationOutput();

            try
            {
                string isBypasscertificate = Convert.ToString(ConfigurationManager.AppSettings["ByPassCertificate"]);
                if (isBypasscertificate.ToUpper() == "YES")
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                }


                using (HttpClient client = new HttpClient())
                {

                    // create data for post request
                    var postData = new Dictionary<string, string>
                    {
                       { "transactionid", value.TransactionId },
                       { "status", value.CurrentState }
                    };

                    // convert it into json
                    var jsonData = JsonConvert.SerializeObject(postData);

                    // create content
                    HttpContent content = new StringContent(jsonData, UTF8Encoding.UTF8, "application/json");

                    //call the post service 
                    HttpResponseMessage response = await client.PostAsync(new Uri(value.NotificationCallBackURL), content).ConfigureAwait(false);


                    if (response.IsSuccessStatusCode)
                    {
                        var resultResponse = response.Content.ReadAsStringAsync().Result;
                        string outputresult = JsonConvert.DeserializeObject(resultResponse.ToString()).ToString();
                        APIResponse apiOutput = JsonConvert.DeserializeObject<APIResponse>(outputresult);
                        output.response = new List<APIResponse>();
                        output.response.Add(new APIResponse { status = apiOutput.status, remarks = apiOutput.remarks });
                        //output.IsSuccess = true;
                        if (apiOutput.status.Equals("success", StringComparison.InvariantCultureIgnoreCase))
                        {
                            output.IsNotified = true;
                       
                        }
                        else
                        {
                            output.IsNotified = false;
                            output.NotificationRemarks = apiOutput.remarks.Length <= 500 ? apiOutput.remarks.ToString() : apiOutput.remarks.Substring(0, 500);
                        }
                        
                    }
                    else
                    {

                        output.IsNotified = false;
                        //output.IsSuccess = true;
                        // add remarks value returned. Limit it to 500 characters 
                        output.NotificationRemarks = response.ToString().Length <= 500 ? response.ToString() : response.ToString().Substring(0, 500);
                    }
                }

            }
            catch (Exception wemNotificationExeception)
            {
                // data to be updated for NotificationDetails
                //output.IsSuccess = false;
                output.IsNotified = false;
               
                if(!string.IsNullOrEmpty(wemNotificationExeception.Message.ToString()))
                    output.NotificationRemarks = wemNotificationExeception.Message.ToString().Length <= 500 ? wemNotificationExeception.Message.ToString() : wemNotificationExeception.Message.ToString().Substring(0, 500); 
                else
                    output.NotificationRemarks = "Some error occurred in calling Notification Call Back Url";
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(wemNotificationExeception, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
            }
            return output;
        }



    }




    public class NotificationInput
    { 
        public string NotificationCallBackURL;
        public string TransactionId;
        public string CurrentState;

        public NotificationInput(string strNotificationURL, string strTransactionId, string strCurrentState)
        {
            NotificationCallBackURL = strNotificationURL;
            TransactionId = strTransactionId;
            CurrentState = strCurrentState;
        }
    }

    public class NotificationOutput
    {
        public bool IsNotified;
        public string NotificationRemarks;
      
        public List<APIResponse> response;
    }

    public class APIResponse
    {
        public string status;
        public string remarks;
    }

    
}
