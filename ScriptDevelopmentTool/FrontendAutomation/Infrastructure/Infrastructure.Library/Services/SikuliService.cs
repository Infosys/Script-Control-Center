/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Configuration;
using Infosys.ATR.UIAutomation.Entities;
using IMSWorkBench.Infrastructure.Library.CodeGenerator;
using Infosys.Solutions.CodeGeneration.Framework;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace IMSWorkBench.Infrastructure.Library.Services
{
    enum AppType
    {
        Win32,
        WebApplication,
        DirectUI
    }
    public class Sikuli
    {
        static string SIKULI_HOME = @ConfigurationManager.AppSettings["Sikulihome"];
        static string IMAGES_HOME = @ConfigurationManager.AppSettings["TaskImageLocation"];
        static string PROJECT_HOME = @ConfigurationManager.AppSettings["ProjectHome"];
        static string SIKULI_ARGS = "-jar {0} -r {1}";
        static string SIKULI_IDE_ARGS = " -Xms64M -Xmx512M -Dfile.encoding=UTF-8 -Dsikuli.FromCommandLine -jar \"{0}\" -l \"{1}\"";
        static string useCaseName = "";
        static string fileName = "";
        static string useCaseId = "";
        static IntPtr parent;
        static List<String> alreadyAdded = new List<string>();

        static Sikuli()
        {
            Win32.Sikuli += new Win32.ProcessFocusEventHandler(Win32_Sikuli);
        }

        /// <summary>
        /// Create sikuli projects for orphaned atrwbs
        /// </summary>
        public static void Refresh()
        {
            var sikuliproject = ConfigurationManager.AppSettings["ProjectHome"];
            var usecase = ConfigurationManager.AppSettings["UsecaseLocation"];
            IEnumerable<String> usecases = Directory.GetFiles(usecase).Select(f => Path.GetFileNameWithoutExtension(f));
            IEnumerable<String> projects = Directory.GetDirectories(sikuliproject).Select(d => Path.GetFileNameWithoutExtension(d));
            IEnumerable<String> genScripts = usecases.Except(projects);
            string filePath="";
            foreach (string file in genScripts)
            {
                try
                {
                    filePath = Path.Combine(usecase, file + ".atrwb");
                    Script(filePath);
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Script not generated for " + filePath);
                }
            }
        }

        public static void Launch(IntPtr pHandle, string file)
        {
            string filePath = Path.Combine(PROJECT_HOME, file + ".sikuli");
            parent = pHandle;
            Win32.Start("java",
                String.Format(SIKULI_IDE_ARGS, ConfigurationManager.AppSettings["SikuliIDE"], filePath));
            //Win32.Start("cmd","ide.bat",ConfigurationManager.AppSettings["Ifea"]);

        }

        static void Win32_Sikuli(IntPtr handle, ShowWindowCommands windowState)
        {
            Win32.SetParent(handle, parent);
            Win32.ShowWindow(handle, windowState);
            Win32.SetFocus(handle);
        }


        public static void ScriptAndExecute(string filePath)
        {
            Script(filePath);
            Execute(filePath);
        }

        public static void Run(string filePath)
        {
            Execute(filePath);
        }

        private static bool IsPresent(string parentId)
        {
            return alreadyAdded.Contains(parentId) ?
                true : false;
        }

        public static void Script(String filePath)
        {
            fileName = Path.GetFileNameWithoutExtension(filePath);
            UseCase uc = Deserialize<UseCase>(filePath);
           
            var script = Script(uc);
            if (script.Length > 0)
                Save(script);
            else
            {
                throw new Exception("Script not generated");
            }
        }

        private static string Script(UseCase uc)
        {         
            Generate cg = Translate.ToCodegenerator(uc);
            StringBuilder sb = new StringBuilder();

            useCaseName = cg.Name;
            useCaseId = cg.Id;

            for (int i = 0; i < cg.Activities.Count; i++)
            {
                Filler s = new Filler(cg.Activities[i]);
                Template template = Template.FromFile(@"Templates\SikuliTemplate.txt");
                s.ContentTemplate = template;
                var output = s.GenerateContent();
                sb.Append(output);
            }
            
            //remove unwated spaces
            char[] lineconstants = { '\r','\t','\n' };
            string[] spaces = sb.ToString().Split(new char[] {'\r','\n' });          
            sb = sb.Clear();
            for (int i = 0;i< spaces.Length; i++)
            {
                var e = spaces[i].Trim(lineconstants);
                if (!string.IsNullOrEmpty(e))
                {
                    sb.Append(e);
                    sb.Append(Environment.NewLine);
                }
            }            
            return sb.ToString();
        }

        //private static string Script(UseCase uc)
        //{
        //    useCaseName = uc.Name;
        //    useCaseId = uc.Id;
        //    var browser = ConfigurationManager.AppSettings["Browser"];
        //    bool alt = false;

        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    uc.Activities.ForEach(
        //       a =>
        //       {
        //           //do not generate script if no activity id is present
        //           //script App.Open only for web applications
        //           //for win32, scripting is done by clicking the win32 application icon
        //           if (!String.IsNullOrEmpty(a.Id))
        //           {
        //               var appType = a.TargetApplication.ApplicationType;
        //               if (!String.IsNullOrEmpty(appType))
        //               {
        //                   if (appType == AppType.WebApplication.ToString())
        //                   {
        //                       //if parent id is present, it is a different link in the same web site
        //                       //so, do not script App.Open
        //                       if (!IsPresent(a.ParentId))
        //                       {
        //                           var hyperlink = a.TargetApplication.ApplicationExe;

        //                           if (!String.IsNullOrEmpty(hyperlink))
        //                           {
        //                               //if internet explorer then the link to be opened is obtained from different element
        //                               if (hyperlink.Contains("iexplore.exe"))
        //                               {
        //                                   hyperlink = GetApplicationName(a);
        //                               }
        //                               sb.Append(String.Format("App.open('{0} \"{1}\"')", browser, hyperlink));
        //                               sb.Append(Environment.NewLine);                                      
        //                           }
        //                       }
        //                   }
        //               }

        //               //ignore "iexplore" kind of win32 app which gets generated for internet explorer scenario
        //               if (GetApplicationName(a) != "iexplore")
        //               {
        //                   var tasks = a.Tasks.Where(task => !String.IsNullOrEmpty(task.Name));
        //                   bool issplKey = false;
        //                   string splKey = "";
        //                   foreach (Task t in tasks)
        //                   {
        //                       var attr = t.Name;
        //                       switch (attr)
        //                       {
        //                           case "MouseLeftClick":
        //                               sb.Append(String.Format("click(\"{0}.jpg\")", t.Id));
        //                               break;
        //                           case "MouseDoubleClick":
        //                               sb.Append(String.Format("doubleClick(\"{0}.jpg\")", t.Id));
        //                               break;
        //                           case "MouseRightClick":
        //                               sb.Append(String.Format("rightClick(\"{0}.jpg\")", t.Id));
        //                               break;
        //                           case "Wait":
        //                               var interval =
        //                                  t.TargetControlAttributes.Where(ta => ta.Name == "Interval").First().Value;
        //                               sb.Append(String.Format("sleep({0})", interval));
        //                               break;
        //                           case "GroupedKeys":
        //                               var text =
        //                                   t.TargetControlAttributes.Where(ta => ta.Name == "KeyData").First().Value;
        //                               if (!issplKey)
        //                               {
        //                                   sb.Append(String.Format("type(\"{0}\")", text.ToLower()));
        //                               }
        //                               else
        //                               {
        //                                   var temp = text.Substring(0, 1);
        //                                   sb.Append(String.Format("type(\"{0}\")", temp.ToLower()));
        //                                   sb.Append(Environment.NewLine);
        //                                   sb.Append(String.Format("keyUp({0})", splKey));
        //                                   sb.Append(Environment.NewLine);
        //                                   sb.Append(String.Format("type(\"{0}\")", text.Substring(1, text.Length - 1)));
        //                                   issplKey = false;
        //                               }
        //                               break;
        //                           case "CollectiveKey":
        //                               var spl =
        //                                   t.TargetControlAttributes.Where(ta => ta.Name == "KeyData").First().Value;
        //                               if (spl.Contains("Shift"))
        //                               {
        //                                   splKey = "Key.SHIFT";
        //                                   sb.Append(String.Format("keyDown({0})", "Key.SHIFT"));
        //                                   issplKey = true;
        //                               }
        //                               if (spl.Contains("Control"))
        //                               {
        //                                   splKey = "Key.CTRL";
        //                                   sb.Append(String.Format("keyDown({0})", "Key.CTRL"));
        //                                   issplKey = true;
        //                               }
        //                               break;
        //                           case "Space":
        //                               sb.Append(String.Format("type(\"{0}\")", " "));
        //                               break;
        //                           case "Tab":
        //                               if (alt)
        //                               {
        //                                   sb.Append(String.Format("type({0})", "\"\\t\", KEY_ALT"));
        //                                   alt = false;
        //                               }
        //                               else
        //                               {
        //                                   sb.Append(String.Format("keyDown({0})", "Key.TAB"));
        //                               }
        //                               break;
        //                           case "LMenu":
        //                               alt = true;
        //                               break;
        //                           case "Return":
        //                               sb.Append(String.Format("keyDown({0})", "Key.ENTER"));
        //                               break;
        //                       }
        //                       sb.Append(Environment.NewLine);
        //                   }
        //                   alreadyAdded.Add(a.Id);
        //               }

        //           }
        //       }
        //   );
        //    return sb.ToString();
        //}


        private static T Deserialize<T>(string filePath)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(filePath);

            using (StringReader usecase = new StringReader(xmlDoc.OuterXml))
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                return (T)xml.Deserialize(usecase);
            }
        }

        //public static void Script(string filePath)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();

        //    try
        //    {
        //        fileName = Path.GetFileNameWithoutExtension(filePath);
        //        XDocument xDoc = XDocument.Load(filePath);
        //        var useCase = xDoc.Elements("UseCase");
        //        useCaseName = useCase.Attributes("Name").First().Value;
        //        useCaseId = useCase.Attributes("Id").First().Value;
        //        var activities = useCase.Elements("Activity");
        //        var browser = ConfigurationManager.AppSettings["Browser"];
        //        bool alt = false;
        //        for (int i = 0; i < activities.Count(); i++)
        //        {
        //            var activity = activities.ElementAt(i);
        //            string activityId;
        //            if (activity.Attribute("Id") != null)
        //                activityId = activity.Attribute("Id").Value;
        //            else
        //                continue;
        //            string parentId = "";
        //            if (activity.Attribute("ParentId") != null)
        //            {
        //                parentId = activity.Attribute("ParentId").Value;
        //            }

        //            string applicationType = string.Empty;

        //            if (activity.Element("TargetApplication").Attribute("ApplicationType") != null)
        //            {
        //                applicationType = activity.Element("TargetApplication").Attribute("ApplicationType").Value;

        //                if (!String.IsNullOrEmpty(applicationType))
        //                {
        //                    if (applicationType == AppType.WebApplication.ToString())
        //                    {
        //                        if (!IsPresent(parentId))
        //                        {
        //                            var exe = activity.Element("TargetApplication").Attribute("ApplicationExe");
        //                            if (exe != null)
        //                            {
        //                                var targetapplication = exe.Value;

        //                                if (targetapplication.Contains("iexplore.exe"))
        //                                {
        //                                   // var t = activity.Element("TargetApplication").Elements("TargetApplicationAttributes");
        //                                    targetapplication = GetApplicationName(activity);
        //                                }
        //                                sb.Append(String.Format("App.open('{0} \"{1}\"')", browser, targetapplication));
        //                                sb.Append(Environment.NewLine);
        //                                //alreadyAdded.Add(targetapplication);
        //                                alreadyAdded.Add(activityId);
        //                            }
        //                        }
        //                    }


        //                    if (GetApplicationName(activity) != "iexplore")
        //                    {

        //                        var tasks = activity.Elements("Task").Where(t => t.Attribute("Name").Value != "None");
        //                        bool issplKey = false;
        //                        string splKey = "";
        //                        for (int j = 0; j < tasks.Count(); j++)
        //                        {
        //                            var t = tasks.ElementAt(j);
        //                            var attr = t.Attribute("Name").Value;
        //                            switch (attr)
        //                            {
        //                                case "MouseLeftClick":
        //                                    sb.Append(String.Format("click(\"{0}.jpg\")", t.Attribute("Id").Value));
        //                                    break;
        //                                case "MouseDoubleClick":
        //                                    sb.Append(String.Format("doubleClick(\"{0}.jpg\")", t.Attribute("Id").Value));
        //                                    break;
        //                                case "MouseRightClick":
        //                                    sb.Append(String.Format("rightClick(\"{0}.jpg\")", t.Attribute("Id").Value));
        //                                    break;
        //                                case "Wait":
        //                                    var interval =
        //                                        t.Elements("TargetControlAttributes").Where(a => a.Attribute("Name").Value == "Interval").First().Attribute("Value").Value;
        //                                    sb.Append(String.Format("sleep({0})", interval));
        //                                    break;
        //                                case "GroupedKeys":
        //                                    var text =
        //                                        t.Elements("TargetControlAttributes").Where(a => a.Attribute("Name").Value == "KeyData").First().Attribute("Value").Value;
        //                                    //if (text == "F11")
        //                                    //{
        //                                    //    sb.Append(String.Format("type(\"{0}\")", "Key.F11"));
        //                                    //}
        //                                    if (!issplKey)
        //                                    {
        //                                        sb.Append(String.Format("type(\"{0}\")", text.ToLower()));
        //                                    }
        //                                    else
        //                                    {
        //                                        var temp = text.Substring(0, 1);
        //                                        sb.Append(String.Format("type(\"{0}\")", temp.ToLower()));
        //                                        sb.Append(Environment.NewLine);
        //                                        sb.Append(String.Format("keyUp({0})", splKey));
        //                                        sb.Append(Environment.NewLine);
        //                                        sb.Append(String.Format("type(\"{0}\")", text.Substring(1, text.Length - 1)));
        //                                        issplKey = false;
        //                                    }
        //                                    break;
        //                                case "CollectiveKey":
        //                                    var spl =
        //                                        t.Elements("TargetControlAttributes").Where(a => a.Attribute("Name").Value == "KeyData").First().Attribute("Value").Value;
        //                                    if (spl.Contains("Shift"))
        //                                    {
        //                                        splKey = "Key.SHIFT";
        //                                        sb.Append(String.Format("keyDown({0})", "Key.SHIFT"));
        //                                        issplKey = true;
        //                                    }
        //                                    if (spl.Contains("Control"))
        //                                    {
        //                                        splKey = "Key.CTRL";
        //                                        sb.Append(String.Format("keyDown({0})", "Key.CTRL"));
        //                                        issplKey = true;
        //                                    }
        //                                    break;
        //                                case "Space":
        //                                    sb.Append(String.Format("type(\"{0}\")", " "));
        //                                    break;
        //                                case "Tab":
        //                                    if (alt)
        //                                    {
        //                                        sb.Append(String.Format("type({0})", "\"\\t\", KEY_ALT"));
        //                                        alt = false;
        //                                    }
        //                                    else
        //                                    {
        //                                        sb.Append(String.Format("keyDown({0})", "Key.TAB"));
        //                                    }
        //                                    break;
        //                                case "LMenu":
        //                                    alt = true;
        //                                    break;
        //                                case "Return":
        //                                    sb.Append(String.Format("keyDown({0})", "Key.ENTER"));
        //                                    break;
        //                            }
        //                            sb.Append(Environment.NewLine);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    var script = sb.ToString();
        //    if (script.Length > 0)
        //        Save(sb.ToString());
        //    else
        //        throw new Exception("Script not genearated");

        //}

        //private static string GetApplicationName(XElement activity)
        //{
        //    var t = activity.Element("TargetApplication").Elements("TargetApplicationAttributes");
        //    var applicationName = t.First().Attribute("Value").Value;
        //    return applicationName;
        //}

        private static string GetApplicationName(Infosys.ATR.UIAutomation.Entities.Activity activity)
        {
            var t = activity.TargetApplication.TargetApplicationAttributes;
            var applicationName = t.First().Value;
            return applicationName;
        }

        //private static XElement GetApplicationType(IEnumerable<XElement> activities)
        //{
        //    for (int i = 0; i < activities.Count(); i++)
        //    {
        //        var targetApplication = activities.Elements("TargetApplication").First();
        //        if (targetApplication.Attribute("ApplicationType") != null)
        //        {
        //            return targetApplication;
        //        }
        //    }
        //    return null;
        //}

        private static void Save(string content)
        {
            if (Infosys.WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFile(fileName))
            {
                throw new Exception("Please provide the file name without Special Characters");
                //MessageBox.Show("Please provide the file name without Special Characters", "Invalid file name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }

            if (!Directory.Exists(PROJECT_HOME))
                Directory.CreateDirectory(PROJECT_HOME);
            string sikuliDir = PROJECT_HOME + fileName + ".sikuli";

            if (Infosys.WEM.Infrastructure.Common.ValidationUtility.InvalidCharValidatorForFolderPath(sikuliDir))
            {
                throw new Exception("Please provide the directory name without Special Characters");
                //MessageBox.Show("Please provide the directory name without Special Characters", "Invalid directory name...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }

            DirectoryInfo newDir = Directory.CreateDirectory(sikuliDir);
            System.IO.File.WriteAllText(newDir.FullName + "\\" + fileName + ".py", content);
            string images = Path.Combine(IMAGES_HOME, useCaseId);
            foreach (var file in Directory.GetFiles(images))
            {
                System.IO.File.Copy(file, newDir.FullName + "\\" + Path.GetFileName(file), true);
            }
        }

        public static void Execute(string path)
        {
            Win32.MinimizeAll();
            fileName = Path.GetFileNameWithoutExtension(path);
            string agent = ConfigurationManager.AppSettings["AgentBat"];
            string arguments = String.Format(SIKULI_ARGS, SIKULI_HOME + "sikuli-script.jar", PROJECT_HOME + fileName + ".sikuli");
            Win32.Start("java", arguments);
        }
    }
}
