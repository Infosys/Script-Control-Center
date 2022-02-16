/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.UIAutomation.SEE;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Infosys.ATR.DataAnalysis
{
    public static class DataExtraction
    {
        //private variables/methods
        static string usecaseLocation = "";
        static string delimiter = "->";
        static List<Activity> organizedAct = new List<Activity>();

        static List<ExtractedData> GetAll(List<string> usecaseFilenames)
        {
            List<ExtractedData> data = new List<ExtractedData>();
            System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(usecaseLocation);
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            if (usecaseFilenames.Count > 0)
                files = dirSource.GetFiles().ToList().Where(file => usecaseFilenames.Contains(file.Name)).ToList();
            else
                files = dirSource.GetFiles().ToList();
            files.ForEach(file =>
            {
                UseCase usecase = SerializeAndDeserialize.Deserialize(System.IO.File.ReadAllText(file.FullName), typeof(UseCase)) as UseCase;
                if (usecase != null)
                {
                    ExtractedData usecaseData = new ExtractedData();
                    usecaseData.OtherInfo = new List<AllExtractData>();
                    AllExtractData allData1 = new AllExtractData() { 
                        UseCaseId = usecase.Id,
                        UseCaseName = usecase.Name,
                        UseCaseCreatedBy = usecase.CreatedBy,
                        UseCaseCreatedOn = (usecase.CreatedOn != null) ? string.Format("{0}", usecase.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss.fff")) : usecase.CreatedOn.ToString(),
                        MachineName = usecase.MachineName,
                        MachineIP = ReplaceComma(usecase.MachineIP),
                        OS= usecase.OS,
                        OSVersion = usecase.OSVersion,
                        Domain = usecase.Domain,
                        MachineType = usecase.MachineType,
                        ScreenResolution = usecase.ScreenResolution
                    };
                    usecase.Activities.ForEach(act =>
                    {
                        if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                        {
                            AllExtractData allData2 = allData1.DeepCopy();
                            allData2.ActivityId = act.Id;
                            allData2.ActivityName = act.Name;
                            allData2.ActivityCreatedOn = (act.CreatedOn != null) ? string.Format("{0}", act.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss.fff")) : act.CreatedOn.ToString();
                            allData2.ActivityParentId = act.ParentId;
                            allData2.ApplicationExe = act.TargetApplication.ApplicationExe;
                            allData2.ApplicationType = act.TargetApplication.ApplicationType;
                            allData2.ApplicationName = ReplaceComma(act.TargetApplication.TargetApplicationAttributes[0].Value);
                            allData2.ApplicationFileName = act.TargetApplication.TargetApplicationAttributes[1].Value;
                            allData2.ApplicationModuleName = act.TargetApplication.TargetApplicationAttributes[2].Value;
                            act.Tasks.ForEach(task =>
                            {
                                AllExtractData allData3 = allData2.DeepCopy();
                                allData3.TaskId = task.Id;
                                allData3.TaskName = task.Name;
                                allData3.TaskDescription = ReplaceComma(task.Description);
                                allData3.TaskCreatedOn = (task.CreatedOn != null) ? string.Format("{0}", task.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss.fff")) : task.CreatedOn.ToString();
                                allData3.TaskEvent = task.Event.ToString();
                                allData3.TaskTriggeredPattern = ReplaceComma(task.TriggeredPattern);
                                allData3.TaskControlId = task.ControlId;
                                allData3.TaskControlName = ReplaceComma(task.ControlName);
                                allData3.TaskControlType = task.ControlType;
                                allData3.TaskControlXPath = task.XPath;
                                allData3.TaskApplicationTreePath = task.ApplictionTreePath;
                                allData3.TaskWindowTitle = ReplaceComma(task.WindowTitle);
                                allData3.TaskGroupId = task.GroupScriptId;
                                allData3.TaskOrder = task.Order;
                                allData3.TaskSourceIndex = task.SourceIndex;
                                allData3.TaskKeyCodeAttribute = ReplaceComma(GetValuePart( task.TargetControlAttributes.Where(tg => tg.Name == "KeyCode").FirstOrDefault()));
                                allData3.TaskKeyDataAttribute = ReplaceComma(GetValuePart(task.TargetControlAttributes.Where(tg => tg.Name == "KeyData").FirstOrDefault()));
                                allData3.TaskKeyValueAttribute = ReplaceComma(GetValuePart(task.TargetControlAttributes.Where(tg => tg.Name == "KeyValue").FirstOrDefault()));
                                allData3.TaskControlAutomationIdAttribute = GetValuePart(task.TargetControlAttributes.Where(tg => tg.Name == "AutomationId").FirstOrDefault());
                                allData3.TaskControlLocalizedControlTypeAttribute = GetValuePart(task.TargetControlAttributes.Where(tg => tg.Name == "LocalizedControlType").FirstOrDefault());
                                allData3.TaskControlClassNameAttribute = GetValuePart(task.TargetControlAttributes.Where(tg => tg.Name == "ClassName").FirstOrDefault());
                                allData3.ScreenId = task.ScreenId;
                                usecaseData.OtherInfo.Add(allData3);
                            });
                            //usecaseData.OtherInfo.Add(allData2);
                        }
                    });
                    //usecaseData.OtherInfo.Add(allData1);
                    data.Add(usecaseData);
                }
            });
            return data;
        }

        static string GetValuePart(NameValueAtribute att)
        {
            string value = "";
            if (att != null)
                value = att.Value;
            return value;
        }

        static AllExtractData DeepCopy(this AllExtractData source)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Position = 0;
                return (AllExtractData)formatter.Deserialize(stream);
            }
        }

        static List<ExtractedData> GetApplicationScreenPath(List<string> usecaseFilenames)
        {
            List<ExtractedData> data = new List<ExtractedData>();
            System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(usecaseLocation);
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            if (usecaseFilenames.Count > 0)
                files = dirSource.GetFiles().ToList().Where(file => usecaseFilenames.Contains(file.Name)).ToList();
            else
                files = dirSource.GetFiles().ToList();
            files.ForEach(file =>
            {
                UseCase usecase = SerializeAndDeserialize.Deserialize(System.IO.File.ReadAllText(file.FullName), typeof(UseCase)) as UseCase;
                int activityOrder = 0;
                organizedAct = new List<Activity>();
                if (usecase != null)
                {
                    GetForeMostParents(usecase).ForEach(parent => OrganizeActivities(parent.Id, usecase));
                    organizedAct.ForEach(act =>
                    {
                        activityOrder++;
                        //if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                        //{
                            string appName = act.TargetApplication.TargetApplicationAttributes[0].Value;
                            string eventOrder = "";
                            List<ScreenControlMapping> mappings = new List<ScreenControlMapping>();
                            act.Tasks.ForEach(task =>
                            {
                                bool mappingFound = false;
                                mappings.ForEach(map =>
                                {
                                    if (map.ScreenTitle == task.WindowTitle)
                                    {
                                        if (task.Event == EventTypes.KeyboardKeyPress)
                                            map.ControlsOnScreen.Add("T-" + task.TargetControlAttributes[1].Value.ToLower().Replace(",", "%2C"));
                                        else
                                        {
                                            string id = string.IsNullOrEmpty(task.ControlId) ? task.ControlName : task.ControlId;
                                            if (string.IsNullOrEmpty(id))
                                                id = task.XPath;
                                            map.ControlsOnScreen.Add("C-" + id);
                                        }
                                        mappingFound = true;
                                    }
                                });
                                if (!mappingFound)
                                {
                                    ScreenControlMapping map = new ScreenControlMapping() { ScreenTitle = task.WindowTitle };
                                    map.ControlsOnScreen = new List<string>();
                                    if (task.Event == EventTypes.KeyboardKeyPress)
                                        map.ControlsOnScreen.Add("T-" + task.TargetControlAttributes[1].Value.ToLower().Replace(",", "%2C"));
                                    else
                                    {
                                        string id = string.IsNullOrEmpty(task.ControlId) ? task.ControlName : task.ControlId;
                                        if (string.IsNullOrEmpty(id))
                                            id = task.XPath;
                                        map.ControlsOnScreen.Add("C-" + id);
                                    }
                                    mappings.Add(map);
                                }
                            });
                            mappings.ForEach(map =>
                            {
                                //construct the identfier
                                if (eventOrder == "")
                                    eventOrder += ReplaceComma(map.ScreenTitle);
                                else
                                    eventOrder += delimiter + ReplaceComma(map.ScreenTitle);
                            });
                            eventOrder = appName + ":" + eventOrder;
                            data.Add(new ExtractedData()
                            {
                                Identifier = eventOrder,
                                IdentifierIncidentCount = 1,
                                UseCaseId = usecase.Id,
                                ScreenPathId = Guid.NewGuid().ToString(),
                                ScreenPathSequence = activityOrder
                            });
                        //}
                    });
                }
            });
            return data;
        }

        static List<ExtractedData> GetApplications(List<string> usecaseFilenames)
        {
            List<ExtractedData> data = new List<ExtractedData>();
            System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(usecaseLocation);
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            if (usecaseFilenames.Count > 0)
                files = dirSource.GetFiles().ToList().Where(file => usecaseFilenames.Contains(file.Name)).ToList();
            else
                files = dirSource.GetFiles().ToList();
            files.ForEach(file =>
            {
                UseCase usecase = SerializeAndDeserialize.Deserialize(System.IO.File.ReadAllText(file.FullName), typeof(UseCase)) as UseCase;
                if (usecase != null)
                {
                    usecase.Activities.ForEach(act =>
                    {
                        if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                        {
                            data.Add(new ExtractedData()
                            {
                                Identifier = act.TargetApplication.TargetApplicationAttributes[0].Value,
                                IdentifierIncidentCount = 1
                            });
                        }
                    });
                }
            });
            return data;
        }

        static List<ExtractedData> GetApplicationEvents(List<string> usecaseFilenames)
        {
            #region old code
            //List<ExtractedData> data = new List<ExtractedData>();
            //System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(usecaseLocation);
            //List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            //if (usecaseFilenames.Count > 0)
            //    files = dirSource.GetFiles().ToList().Where(file => usecaseFilenames.Contains(file.Name)).ToList();
            //else
            //    files = dirSource.GetFiles().ToList();
            //files.ForEach(file =>
            //{
            //    UseCase usecase = SerializeAndDeserialize.Deserialize(System.IO.File.ReadAllText(file.FullName), typeof(UseCase)) as UseCase;
            //    if (usecase != null)
            //    {
            //        usecase.Activities.ForEach(act =>
            //        {
            //            if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
            //            {
            //                string appName = act.TargetApplication.TargetApplicationAttributes[0].Value;
            //                string eventOrder = appName;
            //                act.Tasks.ForEach(task =>
            //                {
            //                    if(task.Event== EventTypes.KeyboardKeyPress)
            //                        eventOrder += delimiter + task.Event.ToString() + "(T-" + task.TargetControlAttributes[1].Value.Replace(",", "%2C") + ")";
            //                    else
            //                        eventOrder += delimiter + task.Event.ToString() + "(C-" + task.ControlId + ")";
            //                });
            //                data.Add(new ExtractedData()
            //                {
            //                    Identifier = eventOrder,
            //                    IdentifierIncidentCount = 1
            //                });
            //            }
            //        });
            //    }
            //});
            //return data;
            #endregion
            List<ExtractedData> data = new List<ExtractedData>();
            System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(usecaseLocation);
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            if (usecaseFilenames.Count > 0)
                files = dirSource.GetFiles().ToList().Where(file => usecaseFilenames.Contains(file.Name)).ToList();
            else
                files = dirSource.GetFiles().ToList();
            files.ForEach(file =>
            {
                UseCase usecase = SerializeAndDeserialize.Deserialize(System.IO.File.ReadAllText(file.FullName), typeof(UseCase)) as UseCase;
                if (usecase != null)
                {
                    usecase.Activities.ForEach(act =>
                    {
                        if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                        {
                            string appName = act.TargetApplication.TargetApplicationAttributes[0].Value;
                            string eventOrder = appName;
                            List<ScreenControlMapping> mappings = new List<ScreenControlMapping>();
                            act.Tasks.ForEach(task =>
                            {
                                //    //if (task.Event == EventTypes.KeyboardKeyPress)
                                //    //    eventOrder += delimiter + task.Event.ToString() + "(T-" + task.TargetControlAttributes[1].Value.Replace(",", "%2C") + ")";
                                //    //else
                                //    //    eventOrder += delimiter + task.Event.ToString() + "(C-" + task.ControlId + ")";
                                bool mappingFound = false;
                                mappings.ForEach(map =>
                                {
                                    if (map.ScreenTitle == task.WindowTitle)
                                    {
                                        if (task.Event == EventTypes.KeyboardKeyPress)
                                        { 
                                            //currently excluding the key pressed in the analysis data
                                            //map.ControlsOnScreen.Add("T-" + task.TargetControlAttributes[1].Value.ToLower().Replace(",", "%2C")); 
                                        }
                                        else
                                        {
                                            string id = string.IsNullOrEmpty(task.ControlId) ? task.ControlName : task.ControlId;
                                            if (string.IsNullOrEmpty(id))
                                                id = task.ApplictionTreePath;
                                            if (string.IsNullOrEmpty(id))
                                                id = task.XPath;
                                            map.ControlsOnScreen.Add("C-" + id);
                                        }
                                        mappingFound = true;
                                    }
                                });
                                if (!mappingFound)
                                {
                                    ScreenControlMapping map = new ScreenControlMapping() { ScreenTitle = task.WindowTitle };
                                    map.ControlsOnScreen = new List<string>();
                                    if (task.Event == EventTypes.KeyboardKeyPress)
                                        map.ControlsOnScreen.Add("T-" + task.TargetControlAttributes[1].Value.ToLower().Replace(",", "%2C"));
                                    else
                                    {
                                        string id = string.IsNullOrEmpty(task.ControlId) ? task.ControlName : task.ControlId;
                                        if (string.IsNullOrEmpty(id))
                                            id = task.XPath;
                                        map.ControlsOnScreen.Add("C-" + id);
                                    }
                                    mappings.Add(map);
                                }
                            });
                            mappings.ForEach(map =>
                            {
                                //construct the identfier
                                eventOrder += ":" + ReplaceComma(map.ScreenTitle);
                                map.ControlsOnScreen.ForEach(ctl =>
                                {
                                    eventOrder += delimiter + ctl;
                                });
                            });
                            data.Add(new ExtractedData()
                            {
                                Identifier = eventOrder,
                                IdentifierIncidentCount = 1
                            });
                        }
                    });
                }
            });
            return data;
        }

        static List<ExtractedData> GetApplicationRelations(List<string> usecaseFilenames)
        {
            List<ExtractedData> data = new List<ExtractedData>();
            System.IO.DirectoryInfo dirSource = new System.IO.DirectoryInfo(usecaseLocation);
            List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
            if (usecaseFilenames.Count > 0)
                files = dirSource.GetFiles().ToList().Where(file => usecaseFilenames.Contains(file.Name)).ToList();
            else
                files = dirSource.GetFiles().ToList();
            files.ForEach(file =>
            {
                organizedAct = new List<Activity>();
                UseCase usecase = SerializeAndDeserialize.Deserialize(System.IO.File.ReadAllText(file.FullName), typeof(UseCase)) as UseCase;
                GetForeMostParents(usecase).ForEach(parent => OrganizeActivities(parent.Id, usecase));
                //usecase.Activities = organizedAct;
                string activityOrder = "";
                string lasActivity = "";
                foreach (var act in organizedAct)
                {
                    if (lasActivity == act.TargetApplication.TargetApplicationAttributes[0].Value)
                        continue;
                    else
                        lasActivity = act.TargetApplication.TargetApplicationAttributes[0].Value;
                    if (activityOrder == "")
                        activityOrder = act.TargetApplication.TargetApplicationAttributes[0].Value;
                    else
                        activityOrder += delimiter + act.TargetApplication.TargetApplicationAttributes[0].Value;
                }

                //organizedAct.ForEach(act => {
                //    if (lasActivity == act.TargetApplication.TargetApplicationAttributes[0].Value)
                //        continue;
                //    if(activityOrder=="")
                //        activityOrder = act.TargetApplication.TargetApplicationAttributes[0].Value;
                //    else
                //        activityOrder += delimiter + act.TargetApplication.TargetApplicationAttributes[0].Value;
                //});
                data.Add(new ExtractedData()
                {
                    Identifier = activityOrder,
                    IdentifierIncidentCount = 1,
                    UseCaseId = usecase.Id
                });
            });
            return data;
        }

        static List<Activity> GetForeMostParents(UseCase useCaseTobeEdited)
        {
            List<Activity> foreMostParents = new List<Activity>();
            foreach (Activity act in useCaseTobeEdited.Activities)
            {
                if (!string.IsNullOrEmpty(act.TargetApplication.ApplicationExe))
                {
                    if (string.IsNullOrEmpty(act.ParentId))
                    {
                        foreMostParents.Add(act);
                    }
                    else
                    {
                        Activity tempParentAct = useCaseTobeEdited.Activities.Where(act2 => act2.Id == act.ParentId).FirstOrDefault();
                        if (tempParentAct == null)
                        {
                            foreMostParents.Add(act);
                        }
                    }
                }
            }
            organizedAct.AddRange(foreMostParents);
            return foreMostParents;
        }

        static void OrganizeActivities(string foremostActivityId, UseCase useCaseTobeEdited)
        {
            List<Activity> validActs = useCaseTobeEdited.Activities.Where(act => !string.IsNullOrEmpty(act.TargetApplication.ApplicationExe)).ToList();
            for (int i = 0; i < validActs.Count; i++)
            {
                if (foremostActivityId == "")
                    break;
                Activity nextParent = useCaseTobeEdited.Activities.Where(act => act.ParentId == foremostActivityId).FirstOrDefault();
                if (nextParent != null)
                {
                    foremostActivityId = nextParent.Id;
                    organizedAct.Add(nextParent);
                }
                else
                    foremostActivityId = "";
            }
        }

        private static string ReplaceComma(string input)
        {
            if (!string.IsNullOrEmpty(input))
                input = input.Replace(",", "%2C");
            return input;
        }

        //public variables/interfaces
        /// <summary>
        /// The interface to extract the intended data from the script files generated by the script engine
        /// </summary>
        /// <param name="extractionType">the type of data extraction needed</param>
        /// <param name="usecaseFilenames">the list of file names to be considered, 
        /// if no file names are provided, then all the files at the pre-configured location will be considered</param>
        /// <returns></returns>
        public static List<ExtractedData> Extract(DataExtractionType extractionType, List<string> usecaseFilenames)
        {
            List<ExtractedData> data = new List<ExtractedData>();
            //if no file names are provided, then all the files at the pre-configured location will be considered
            if (usecaseFilenames == null)
                usecaseFilenames = new List<string>();

            if (usecaseLocation == "")
                usecaseLocation = ConfigurationManager.AppSettings["UsecaseLocation"];
            if (usecaseLocation != null && System.IO.Directory.Exists(usecaseLocation))
            {
                switch (extractionType)
                {
                    case DataExtractionType.ControlPath:
                        data = GetApplicationEvents(usecaseFilenames);
                        break;
                    case DataExtractionType.ApplicationPath:
                        data = GetApplicationRelations(usecaseFilenames);
                        break;
                    case DataExtractionType.ApplicationsUsage:
                        data = GetApplications(usecaseFilenames);
                        break;
                    case DataExtractionType.ScreenPath:
                        data = GetApplicationScreenPath(usecaseFilenames);
                        break;
                    case DataExtractionType.All:
                        data = GetAll(usecaseFilenames);
                        break;
                }
            }
            return data;
        }
    }

    public enum DataExtractionType
    {
        ApplicationsUsage,
        ControlPath,
        ApplicationPath,
        ScreenPath,
        All
    }

    public class ExtractedData
    {
        public string Identifier { get; set; }
        public int IdentifierIncidentCount { get; set; }
        public string UseCaseId { get; set; }
        public string ScreenPathId { get; set; }
        public int ScreenPathSequence { get; set; }
        public List<AllExtractData> OtherInfo { get; set; }
    }

    public class ScreenControlMapping
    {
        public string ScreenTitle { get; set; }
        public List<string> ControlsOnScreen { get; set; }
    }

    [Serializable]
    public class AllExtractData
    {
        public string UseCaseName { get; set; }
        public string UseCaseId { get; set; }
        public string UseCaseCreatedBy { get; set; }
        public string UseCaseCreatedOn { get; set; }
        public string MachineName { get; set; }
        public string MachineIP { get; set; }
        public string MachineType { get; set; }
        public string OS { get; set; }
        public string OSVersion { get; set; }
        public string Domain { get; set; }
        public string ScreenResolution { get; set; }

        public string ActivityName { get; set; }
        public string ActivityId { get; set; }
        public string ActivityParentId { get; set; }
        public string ActivityCreatedOn { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationType { get; set; }
        public string ApplicationExe { get; set; }
        public string ApplicationFileName { get; set; }
        public string ApplicationModuleName { get; set; }
        public string ScreenId { get; set; }
        public string TaskName { get; set; }
        public string TaskId { get; set; }
        public string TaskDescription { get; set; }
        public string TaskCreatedOn { get; set; }
        public string TaskEvent { get; set; }
        public string TaskTriggeredPattern { get; set; }
        public string TaskWindowTitle { get; set; }
        public string TaskGroupId { get; set; }
        public string TaskControlId { get; set; }
        public string TaskControlName { get; set; }
        public string TaskControlXPath { get; set; }
        public string TaskApplicationTreePath { get; set; }
        public string TaskControlType { get; set; }
        public string TaskControlAutomationIdAttribute { get; set; }
        public string TaskControlLocalizedControlTypeAttribute { get; set; }
        public string TaskControlClassNameAttribute { get; set; }
        public string TaskKeyCodeAttribute { get; set; }
        public string TaskKeyDataAttribute { get; set; }
        public string TaskKeyValueAttribute { get; set; }
        public int TaskOrder { get; set; }
        public int TaskSourceIndex { get; set; }         
    }
}
