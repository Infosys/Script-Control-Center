/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IMSWorkBench.Infrastructure.Library.CodeGenerator
{
    public class Generate : Common
    {
        public List<Activity> Activities { get; set; }

    }

    public class Activity : Common
    {
        public List<Task> Tasks { get; set; }

        public string Application { get; set; }
    }

    public class Task : Common
    {
        public string TaskName { get; set; }
        public string TaskInfo { get; set; }

    }

    public class Common
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ParentId { get; set; }

    }

    public enum EventTypes
    {
        MouseRightClick,
        MouseLeftClick,
        MouseMiddleClick,
        MouseDoubleClick,
        KeyboardKeyPress,
        Wait
    }

    public class Translate
    {
        static bool issplKey;
        static string splKey;
        static bool alt;
        static List<string> alreadyAdded =
            new List<string>();
        static string browser;

        enum AppType
        {
            Win32,
            WebApplication,
            DirectUI
        }

        static Translate()
        {
            browser = ConfigurationManager.AppSettings["Browser"];
        }

        public static Generate ToCodegenerator(Infosys.ATR.UIAutomation.Entities.UseCase uc)
        {
            Generate cg = new Generate();
            cg.Name = uc.Name;
            cg.Id = uc.Id;
            cg.Activities = ToActivities(uc.Activities);
            return cg;
        }

        private static List<Activity> ToActivities(List<Infosys.ATR.UIAutomation.Entities.Activity> activities)
        {
            List<Activity> codeGenActivities = new List<Activity>();

            activities.ForEach(a =>
            {
                if (!String.IsNullOrEmpty(a.Id))
                {
                    Activity codeGenActivity = new Activity();
                    codeGenActivity.Description = a.Description;
                    codeGenActivity.Id = a.Id;
                    codeGenActivity.Name = a.Name;
                    codeGenActivity.ParentId = a.ParentId;
                    codeGenActivity.Application = GetApplication(a);
                    codeGenActivity.Tasks = ToTasks(a.Tasks, a);
                    codeGenActivities.Add(codeGenActivity);
                    if(!String.IsNullOrEmpty(codeGenActivity.Application) &&
                        codeGenActivity.Tasks.Count > 0)
                        alreadyAdded.Add(a.Id);
                }
            });

            return codeGenActivities;
        }

        private static string GetApplication(Infosys.ATR.UIAutomation.Entities.Activity a)
        {
            StringBuilder sb = new StringBuilder();

            var appType = a.TargetApplication.ApplicationType;
            if (!String.IsNullOrEmpty(appType))
            {
                if (appType == AppType.WebApplication.ToString())
                {
                    //if parent id is present, it is a different link in the same web site
                    //so, do not script App.Open
                    if (!IsPresent(a.ParentId))
                    {
                        var hyperlink = a.TargetApplication.ApplicationExe;

                        if (!String.IsNullOrEmpty(hyperlink))
                        {
                            //if internet explorer then the link to be opened is obtained from different element
                            if (hyperlink.Contains("iexplore.exe"))
                            {
                                hyperlink = GetApplicationName(a);
                            }
                            sb.Append(String.Format("App.open('{0} \"{1}\"')", browser, hyperlink));
                            sb.Append(Environment.NewLine);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private static List<Task> ToTasks(List<Infosys.ATR.UIAutomation.Entities.Task> tasks,
            Infosys.ATR.UIAutomation.Entities.Activity a)
        {
            List<Task> codeGenTasks = new List<Task>();

            tasks.ForEach(t =>
            {
                if (!String.IsNullOrEmpty(t.Name) &&
                    GetApplicationName(a) != "iexplore")
                {
                    Task codeGenTask = new Task();
                    codeGenTask.Description = t.Description;
                    codeGenTask.TaskName = t.Name;
                    codeGenTask.TaskInfo = GetTaskInfo(t);
                    codeGenTask.ParentId = t.ParentId;
                    codeGenTasks.Add(codeGenTask);
                }
            });

            return codeGenTasks;
        }

        private static bool IsPresent(string parentId)
        {
            return alreadyAdded.Contains(parentId) ?
                true : false;
        }

        private static string GetApplicationName(Infosys.ATR.UIAutomation.Entities.Activity activity)
        {
            var t = activity.TargetApplication.TargetApplicationAttributes;
            var applicationName = t.First().Value;
            return applicationName;
        }

        private static string GetTaskInfo(Infosys.ATR.UIAutomation.Entities.Task t)
        {
            StringBuilder sb = new StringBuilder();

            switch (t.Name)
            {
                case "MouseLeftClick":
                    sb.Append(String.Format("(\"{0}.jpg\")", t.Id));
                    break;
                case "MouseDoubleClick":
                    sb.Append(String.Format("(\"{0}.jpg\")", t.Id));
                    break;
                case "MouseRightClick":
                    sb.Append(String.Format("(\"{0}.jpg\")", t.Id));
                    break;
                case "Wait":
                    var interval =
                       t.TargetControlAttributes.Where(ta => ta.Name == "Interval").First().Value;
                    sb.Append(String.Format("({0})", interval));
                    break;
                case "GroupedKeys":
                    var text =
                        t.TargetControlAttributes.Where(ta => ta.Name == "KeyData").First().Value;
                    if (!issplKey)
                    {
                        sb.Append(String.Format("type(\"{0}\")", text.ToLower()));
                    }
                    else
                    {
                        var temp = text.Substring(0, 1);
                        sb.Append(String.Format("type(\"{0}\")", temp.ToLower()));
                        sb.Append(Environment.NewLine);
                        sb.Append(String.Format("keyUp({0})", splKey));
                        sb.Append(Environment.NewLine);
                        sb.Append(String.Format("type(\"{0}\")", text.Substring(1, text.Length - 1)));
                        issplKey = false;
                    }
                    break;
                case "CollectiveKey":
                    var spl =
                        t.TargetControlAttributes.Where(ta => ta.Name == "KeyData").First().Value;
                    if (spl.Contains("Shift"))
                    {
                        splKey = "Key.SHIFT";
                        sb.Append(String.Format("keyDown({0})", "Key.SHIFT"));
                        issplKey = true;
                    }
                    if (spl.Contains("Control"))
                    {
                        splKey = "Key.CTRL";
                        sb.Append(String.Format("keyDown({0})", "Key.CTRL"));
                        issplKey = true;
                    }
                    break;
                case "Space":
                    return " ";
                case "Tab":
                    if (alt)
                    {
                        sb.Append(String.Format("type({0})", "\"\\t\", KEY_ALT"));
                        alt = false;
                    }
                    else
                    {
                        sb.Append(String.Format("keyDown({0})", "Key.TAB"));
                    }
                    break;
                case "LMenu":
                    alt = true;
                    break;
                case "Return":
                    return "Key.ENTER";
            }
            return sb.ToString();
        }
    }
}
