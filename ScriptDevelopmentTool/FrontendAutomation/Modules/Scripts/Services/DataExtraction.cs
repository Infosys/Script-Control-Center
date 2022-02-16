using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Infosys.ATR.UIAutomation.Entities;
using Infosys.ATR.UIAutomation.SEE;
using System.Configuration;

namespace Infosys.ATR.DataAnalysis
{
    public static class DataExtraction
    {
        //private variables/methods
        static string usecaseLocation = "";
        static string delimiter = "->";
        static List<Activity> organizedAct = new List<Activity>();

        static List<ExtractedData> GetApplicationScreenPath (List<string> usecaseFilenames)
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
                            string appName = act.TargetApplication.TargetApplicationAttributes[0].Value;
                            string eventOrder ="";
                            List<ScreenControlMapping> mappings = new List<ScreenControlMapping>();
                            act.Tasks.ForEach(task =>
                            {
                            //    //if (task.Event == EventTypes.KeyboardKeyPress)
                            //    //    eventOrder += delimiter + task.Event.ToString() + "(T-" + task.TargetControlAttributes[1].Value.Replace(",", "%2C") + ")";
                            //    //else
                            //    //    eventOrder += delimiter + task.Event.ToString() + "(C-" + task.ControlId + ")";
                                bool mappingFound = false;
                                mappings.ForEach(map => {
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
                            mappings.ForEach(map => {
                                //construct the identfier
                                if(eventOrder=="")
                                    eventOrder += ReplaceComma(map.ScreenTitle);
                                else
                                    eventOrder += delimiter + ReplaceComma(map.ScreenTitle);                                
                            });
                            eventOrder = appName + ":" + eventOrder;
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
                    IdentifierIncidentCount = 1
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
                    case DataExtractionType.ApplicationEvents:
                        data = GetApplicationEvents(usecaseFilenames);
                        break;
                    case DataExtractionType.ApplicationRelations:
                        data = GetApplicationRelations(usecaseFilenames);
                        break;
                    case DataExtractionType.Applications:
                        data = GetApplications(usecaseFilenames);
                        break;
                    case DataExtractionType.ScreenPath:
                        data = GetApplicationScreenPath(usecaseFilenames);
                        break;
                }
            }
            return data;
        }
    }

    public enum DataExtractionType
    {
        Applications,
        ApplicationEvents,
        ApplicationRelations,
        ScreenPath,
        All
    }

    public class ExtractedData
    {
        public string Identifier { get; set; }
        public int IdentifierIncidentCount { get; set; }
    }

    public class ScreenControlMapping
    {
        public string ScreenTitle { get; set; }
        public List<string> ControlsOnScreen { get; set; }
    }
}
