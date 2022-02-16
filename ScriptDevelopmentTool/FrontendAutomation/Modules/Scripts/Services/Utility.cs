using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.DataAnalysis;
using Infosys.ATR.UIAutomation.SEE;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Configuration;

namespace IMSWorkBench.Scripts.Services
{
    public static class Usecase
    {
        private static string _name;
        private static string _ApplicationFilter;
        //Added by Nimna

        public static void Export(List<ExtractedData> usecases, string ApplicationFilter)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            switch (ApplicationFilter.ToLower())
            {
                case "all data":
                    sb = BuildAllData(usecases);
                    FormatAllDataForReport(sb);
                    break;
                case "application path":
                    _ApplicationFilter = ApplicationFilter.Replace(' ', '_');
                    sb.Append(String.Format("{0},{1},{2}{3}",
                        _ApplicationFilter,
                        "Frequency",
                        "UseCaseId",
                        Environment.NewLine));

                    usecases.ForEach(xml =>
                    {
                        sb.Append(String.Format("{0},{1},{2}{3}",
                            xml.Identifier,
                            xml.IdentifierIncidentCount,
                            xml.UseCaseId,
                            Environment.NewLine
                            ));
                    });
                    FormatForReports(sb);
                    break;
                case "screen path":
                    _ApplicationFilter = ApplicationFilter.Replace(' ', '_');
                    sb.Append(String.Format("{0},{1},{2},{3},{4}{5}",
                        _ApplicationFilter,
                        "Frequency",
                        "UseCaseId",
                        "ScreenPathId",
                        "Sequence",
                        Environment.NewLine));

                    usecases.ForEach(xml =>
                    {
                        sb.Append(String.Format("{0},{1},{2},{3},{4}{5}",
                            xml.Identifier,
                            xml.IdentifierIncidentCount,
                            xml.UseCaseId,
                            xml.ScreenPathId,
                            xml.ScreenPathSequence,
                            Environment.NewLine
                            ));
                    });
                    FormatForReports(sb);
                    break;
                default:
                    _ApplicationFilter = ApplicationFilter.Replace(' ', '_');
                    sb.Append(String.Format("{0},{1}{2}",
                        _ApplicationFilter,
                        "Frequency",
                        Environment.NewLine));

                    usecases.ForEach(xml =>
                    {
                        sb.Append(String.Format("{0},{1}{2}",
                            xml.Identifier,
                            xml.IdentifierIncidentCount,
                            Environment.NewLine
                            ));
                    });
                    FormatForReports(sb);
                    break;
            }

        }

        private static StringBuilder BuildAllData(List<ExtractedData> datas)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //first provide the columns
            sb.Append(
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41}{42}",
                "UseCaseName", "UseCaseId", "UseCaseCreatedBy", "UseCaseCreatedOn", "ActivityName", "ActivityId", "ActivityParentId", "ActivityCreatedOn", "ApplicationName", "ApplicationType", "ApplicationExe", "ApplicationFileName", "ApplicationModuleName", "TaskName", "TaskId", "TaskDescription", "TaskCreatedOn", "TaskEvent", "TaskTriggeredPattern", "TaskWindowTitle", "TaskGroupId", "TaskControlId", "TaskControlName", "TaskControlXPath", "TaskControlInAppTreePath", "TaskControlType", "TaskControlAutomationIdAttribute", "TaskControlLocalizedControlTypeAttribute", "TaskControlClassNameAttribute", "TaskKeyCodeAttribute", "TaskKeyDataAttribute", "TaskKeyValueAttribute", "Frequency", "TaskOrder", "TaskSourceIndex", "MachineName", "MachineIP", "OS", "OSVersion", "Domain", "MachineType", "ScreenResolution", Environment.NewLine
                ));
            datas.ForEach(data =>
            {
                data.OtherInfo.ForEach(alldata =>
                {
                    sb.Append(
                string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41}{42}",
                alldata.UseCaseName, alldata.UseCaseId, alldata.UseCaseCreatedBy, alldata.UseCaseCreatedOn, alldata.ActivityName, alldata.ActivityId, alldata.ActivityParentId, alldata.ActivityCreatedOn, alldata.ApplicationName, alldata.ApplicationType, alldata.ApplicationExe, alldata.ApplicationFileName, alldata.ApplicationModuleName, alldata.TaskName, alldata.TaskId, alldata.TaskDescription, alldata.TaskCreatedOn, alldata.TaskEvent, alldata.TaskTriggeredPattern, alldata.TaskWindowTitle, alldata.TaskGroupId, alldata.TaskControlId, alldata.TaskControlName, alldata.TaskControlXPath, alldata.TaskApplicationTreePath, alldata.TaskControlType, alldata.TaskControlAutomationIdAttribute, alldata.TaskControlLocalizedControlTypeAttribute, alldata.TaskControlClassNameAttribute, alldata.TaskKeyCodeAttribute, alldata.TaskKeyDataAttribute, alldata.TaskKeyValueAttribute, 1, alldata.TaskOrder, alldata.TaskSourceIndex, alldata.MachineName, alldata.MachineIP, alldata.OS, alldata.OSVersion, alldata.Domain, alldata.MachineType, alldata.ScreenResolution, Environment.NewLine
                ));
                });
            });
            return sb;
        }

        private static void FormatAllDataForReport(StringBuilder sb)
        {
            string columnnames = "UseCaseName,UseCaseId,UseCaseCreatedBy,UseCaseCreatedOn,ActivityName,ActivityId,ActivityParentId,ActivityCreatedOn,ApplicationName,ApplicationType,ApplicationExe,ApplicationFileName,ApplicationModuleName,TaskName,TaskId,TaskDescription,TaskCreatedOn,TaskEvent,TaskTriggeredPattern,TaskWindowTitle,TaskGroupId,TaskControlId,TaskControlName,TaskControlXPath,TaskControlInAppTreePath,TaskControlType,TaskControlAutomationIdAttribute,TaskControlLocalizedControlTypeAttribute,TaskControlClassNameAttribute,TaskKeyCodeAttribute,TaskKeyDataAttribute,TaskKeyValueAttribute,Frequency,TaskOrder,TaskSourceIndex,MachineName,MachineIP,OS,OSVersion,Domain,MachineType,ScreenResolution";
            string columntypes = "varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),int,int,int,varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100),varchar(100)";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("datasettype", "new");
            nvc.Add("existingdatasetId", "");
            nvc.Add("selecteddataset", "");
            nvc.Add("newdatasetname", "atrclickstream_" + Thread.CurrentPrincipal.Identity.Name.Substring(Thread.CurrentPrincipal.Identity.Name.IndexOf(@"\") + 1) + "_alldata");
            nvc.Add("delimiter", ",");
            nvc.Add("columnnames", "");
            nvc.Add("operation", "overwrite");
            nvc.Add("columnNamesList", columnnames);
            nvc.Add("columnTypesList", columntypes);
            nvc.Add("primarykey", "");
            byte[] upload = Encoding.UTF8.GetBytes(sb.ToString());
            string serviceUrl = ConfigurationManager.AppSettings["DataAnalysisSericeURL"];
            if (!string.IsNullOrEmpty(serviceUrl))
                Upload(serviceUrl, upload, "csvfiles", "application/vnd.ms-excel", nvc);
        }

        private static void FormatForReports(StringBuilder sb)
        {
            if (string.IsNullOrEmpty(_ApplicationFilter))
            {
                _ApplicationFilter = "ALL";
            }

            string columnnames ="", columntypes = "";

            switch(_ApplicationFilter.ToLower())
            {
                case "application_path":
                    columnnames = _ApplicationFilter + ",Frequency,UseCaseId";
                    columntypes = "varchar(1000),int,varchar(50)";
                    break;
                case "screen_path":
                    columnnames = _ApplicationFilter + ",Frequency,UseCaseId,ScreenPathId,Sequence";
                    columntypes = "varchar(1000),int,varchar(50),varchar(50),int";
                    break;
                default:
                    columnnames = _ApplicationFilter + ",Frequency";
                    columntypes = "varchar(1000),int";
                    break;
            }
            //string columnnames = _ApplicationFilter + ",Frequency";
            //string columntypes = "varchar(1000),int";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("datasettype", "new");
            nvc.Add("existingdatasetId", "");
            nvc.Add("selecteddataset", "");
            nvc.Add("newdatasetname", "atrclickstream_" + Thread.CurrentPrincipal.Identity.Name.Substring(Thread.CurrentPrincipal.Identity.Name.IndexOf(@"\") + 1) + "_" + _ApplicationFilter);//+ new Random().Next(100).ToString());
            nvc.Add("delimiter", ",");
            nvc.Add("columnnames", "");
            nvc.Add("operation", "overwrite");
            nvc.Add("columnNamesList", columnnames);
            nvc.Add("columnTypesList", columntypes);
            nvc.Add("primarykey", "");
            byte[] upload = Encoding.UTF8.GetBytes(sb.ToString());
#if DEBUG
            // File.WriteAllText("atrclickstream." + Thread.CurrentPrincipal.Identity.Name + "." + _ApplicationFilter.Replace(" ", "_") + new Random().Next(100).ToString() + ".csv", sb.ToString());
#endif
            string serviceUrl = ConfigurationManager.AppSettings["DataAnalysisSericeURL"];
            if (!string.IsNullOrEmpty(serviceUrl))
                Upload(serviceUrl, upload, "csvfiles", "application/vnd.ms-excel", nvc);
        }

        //End code added by Nimna
        public static void Export(List<UseCase> usecases)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            _name = usecases[0].Name;
            _name = "All";
            sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}{16}",
                "UsecaseName",
                "ApplicationName",
                "CreatedBy",
                "CreatedOn",
                "Event",
                "Sequence",
                "ControlId",
                "ControlName",
                "ControlType",
                "CurrentState",
                "TriggeredPattern",
                "TaskDescription",
                "XPath",
                "WindowTitle",
                "SourceIndex",
                "Frequency",
                Environment.NewLine));

            usecases.ForEach(xml =>
                {

                    xml.Activities.ForEach(a =>
                    {
                        a.Tasks.ForEach(task =>
                        {
                            int slash = a.TargetApplication.ApplicationExe.LastIndexOf("\\");
                            string app = a.TargetApplication.ApplicationExe.Substring(slash + 1, a.TargetApplication.ApplicationExe.Length - slash - 1);
                            sb.Append(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}{16}",
                                xml.Name,
                                app,
                                Thread.CurrentPrincipal.Identity.Name.Substring(Thread.CurrentPrincipal.Identity.Name.IndexOf(@"\") + 1),
                                DateTime.Now,
                                task.Event.ToString(),
                                task.Order,
                                ReplaceComma(task.ControlId),
                                ReplaceComma(task.ControlName),
                                task.ControlType,
                                task.CurrentState,
                                ReplaceComma(task.TriggeredPattern),
                                ReplaceComma(task.Description),
                                task.XPath,
                                ReplaceComma(task.WindowTitle),
                                task.SourceIndex,
                                1,
                                Environment.NewLine
                                ));
                        });

                    });
                });

            Format(sb);
        }

        private static string ReplaceComma(string input)
        {
            if (!string.IsNullOrEmpty(input))
                input = input.Replace(",", "%2C");
            return input;
        }

        private static void Format(StringBuilder sb)
        {
            string columnnames = "UsecaseName,ApplicationName,CreatedBy,CreatedOn,Event,Sequence,ControlId,ControlName,ControlType,CurrentState,TriggeredPattern,TaskDescription,XPath,WindowTitle,SourceIndex,Frequency";
            string columntypes = "varchar(64),varchar(64),varchar(64),varchar(64),varchar(64),varchar(64),varchar(64),varchar(64),varchar(64),varchar(64),varchar(64),varchar(1000),varchar(64),varchar(64),varchar(64),int";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("datasettype", "new");
            nvc.Add("existingdatasetId", "");
            nvc.Add("selecteddataset", "");
            nvc.Add("newdatasetname", "atrclickstream_" + Thread.CurrentPrincipal.Identity.Name.Substring(Thread.CurrentPrincipal.Identity.Name.IndexOf(@"\") + 1) + "_" + _name);
            nvc.Add("delimiter", ",");
            nvc.Add("columnnames", "");
            nvc.Add("operation", "overwrite");
            nvc.Add("columnNamesList", columnnames);
            nvc.Add("columnTypesList", columntypes);
            nvc.Add("primarykey", "");
            byte[] upload = Encoding.UTF8.GetBytes(sb.ToString());
#if DEBUG
            // File.WriteAllText(_name + new Random().Next(100).ToString() + ".csv", sb.ToString());
#endif
            string serviceUrl = ConfigurationManager.AppSettings["DataAnalysisSericeURL"];
            if (!string.IsNullOrEmpty(serviceUrl))
                Upload(serviceUrl, upload, "csvfiles", "application/vnd.ms-excel", nvc);
            //Upload(@"http://blrkecperfcoe5:8080/ATR/rest/save", upload, "csvfiles", "application/vnd.ms-excel", nvc);
        }

        public static void Upload(string uploadUrl, byte[] fileToUpload, string fileType, string contentType, NameValueCollection data)
        {

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uploadUrl);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            using (Stream stream = request.GetRequestStream())
            {

                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in data.Keys)
                {
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, data[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    stream.Write(formitembytes, 0, formitembytes.Length);
                }
                stream.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, fileType, fileToUpload, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                stream.Write(headerbytes, 0, headerbytes.Length);

                stream.Write(fileToUpload, 0, fileToUpload.Length);

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                stream.Write(trailer, 0, trailer.Length);
                stream.Close();
            }

            using (WebResponse response = request.GetResponse())
            {
                Stream stream2 = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
            }
        }
    }
}
