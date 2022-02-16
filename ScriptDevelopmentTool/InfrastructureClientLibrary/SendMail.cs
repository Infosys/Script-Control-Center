/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace InfrastructureClientLibrary
{
    public class SendMail
    {
        public bool Send(string subject, string body, List<string> filePaths, string From, string recipients, string hostIP, int hostPort,
            string password, int timeOut, bool enableSSL)
        {
            bool result = true;
            MailMessage msg = new MailMessage(From, recipients);
            msg.Subject = subject;

            Attachment attachment = null;
            if (filePaths != null)
            {
                foreach (string path in filePaths)
                {
                    attachment = new Attachment(path);
                    msg.Attachments.Add(attachment);
                }
            }


            msg.IsBodyHtml = true;
            msg.Body = body;
            SmtpClient smtp = null;

            if (hostPort == 0)
            {
                smtp = new System.Net.Mail.SmtpClient(hostIP);
            }
            else
            {
                smtp = new System.Net.Mail.SmtpClient(hostIP, hostPort);
            }

            smtp.EnableSsl = enableSSL;
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            if (password == null)
            {
                smtp.Credentials = new NetworkCredential(From, Infosys.WEM.SecureHandler.SecurePayload.UnSecure("", "IAP2GO_SEC!URE"));
            }
            else
            {
                smtp.Credentials = new NetworkCredential(From, Infosys.WEM.SecureHandler.SecurePayload.UnSecure(password, "IAP2GO_SEC!URE"));
            }
            smtp.Timeout = timeOut;

            smtp.Send(msg);

            return result;
        }
    }
}
