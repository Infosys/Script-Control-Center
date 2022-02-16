/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using IAP.Infrastructure.Services.Contracts;

namespace InfrastructureClientLibrary
{
    public class LoggerClient
    {
        public void LogAudit(string resourceUrl, Log log)
        {
            LoggerService service = new LoggerService(resourceUrl);
            service.ServiceChannel.LogAudit(log); 
        }

        public void LogError(string resourceUrl, Log log)
        {
            LoggerService service = new LoggerService(resourceUrl);
            service.ServiceChannel.LogError(log); 
        }

        public bool Notify(string resourceUrl, Log log)
        {
            if (log.AttachmentFileNames != null)
            {
                List<string> fileNames = new List<string>();
                log.AttachmentFileStreams = new System.Collections.Generic.List<byte[]>();
                foreach (string p in log.AttachmentFileNames)
                {
                    string contentType = MimeMapping.GetMimeMapping(p);

                    log.AttachmentFileStreams.Add(System.IO.File.ReadAllBytes(p));
                    //Keep only file names
                    fileNames.Add(Path.GetFileName(p));
                }
                log.AttachmentFileNames = fileNames;

                ValidateFilesToBeAttached(fileNames);
            }

            LoggerService service = new LoggerService(resourceUrl);
            Response<bool> response = service.ServiceChannel.Notify(log);
            if (response != null)
                return response.Results;
            else return false;
        }

        private static void ValidateFilesToBeAttached(List<string> fileNames)
        {
            bool isValid = false;

            foreach (string name in fileNames)
            {
                if (name.Length > 255)
                    throw new System.Exception("Invalid file name");

                Regex regex = new Regex(@"[a-zA-Z0-9]{1,200}\.[a-zA-Z0-9]{1,10}");
                Match match = regex.Match(name);
                if (!match.Success)
                    throw new System.Exception("Invalid file type");

                string[] dat = name.Split('.');
                if (dat.Length > 2)
                {
                    throw new System.Exception("Invalid file type");
                }

                string[] whitelist = { ".docx", ".doc", ".xlsx", ".xls", ".png", ".jpg", ".bmp", ".jpeg", ".gif", ".tif", ".gif", ".msg", ".txt", ".ppt", ".pdf" };
                foreach (var ext in whitelist)
                {
                    if (name.Contains(ext))
                        isValid = true;
                }

                if (isValid)
                {
                    string[] blacklist = { ";", ":", ">", "<", "/", "\\", "..", "+", "-", "*", "&", "@", "!", "%", "^", "#", "=", "|", ",", "?", "~", "$" };
                    //string[] blacklist = { "." };
                    foreach (var ch in blacklist)
                    {
                        if (name.Contains(ch))
                            isValid = false;
                    }
                }

                if (!isValid)
                    throw new System.Exception("Invalid file type");
            }
        }
    }
}
