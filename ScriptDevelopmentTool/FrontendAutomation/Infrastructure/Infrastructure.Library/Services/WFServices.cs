/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;

namespace IMSWorkBench.Infrastructure.Library.Services
{
    public class WFServices
    {
        private static WFServices instance;

        List<string> namespaces = null;
        Assembly assembly = null;

        public List<Tuple<string, List<Type>>> activities = null;

        const string AutomationActivityAssemblyName = "Infosys.WEM.AutomationActivity.Libraries";
        const string NativeActivity = "NativeActivity";

        private WFServices()
        {
            activities = new List<Tuple<string, List<Type>>>();
        }

        public static WFServices Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WFServices();
                }
                return instance;
            }
        }

        public void InitialiseToolbox()
        {
            GetAllNamespaces();

            ///********* IFEA WF activity ************///
            foreach (string value in namespaces)
            {
                string activityType = GetActivityType(value);
                {
                    var classList = assembly.GetTypes().ToList().Where(t => t.Namespace == value).OrderBy(column => column.Name).ToList();
                    if (classList != null)
                    {
                        Tuple<String, List<Type>> t = new Tuple<string, List<Type>>(activityType, classList);
                        activities.Add(t);
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to get all the namespaces containing classes that derive from NativeActivity class
        /// </summary>
        private void GetAllNamespaces()
        {
            namespaces = new List<string>();
            LoadAutomationActivityAssembly();
            Type nativeActivityType = GetNativeActivityType();
            List<Type> listOfTypes = FindDerivedTypes(assembly, nativeActivityType);
            string topItem = "";

            foreach (Type type in listOfTypes)
            {
                if (type.Namespace.Equals(AutomationActivityAssemblyName))
                    topItem = type.Namespace;
                else
                    namespaces.Add(type.Namespace);
            }

            // Add IFEA at top
            if (!String.IsNullOrEmpty(topItem))
                namespaces.Insert(0, topItem);
        }

        /// <summary>
        /// This method load WEM.AutomationActivity.Libraries assembly
        /// </summary>
        private void LoadAutomationActivityAssembly()
        {
            string automationActivityAssemblyName = System.Configuration.ConfigurationManager.AppSettings["AutomationActivityAssemblyName"];
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                if (dll.Contains(automationActivityAssemblyName))
                {
                    assembly = Assembly.LoadFile(dll);
                    break;
                }
        }

        /// <summary>
        /// This method loads System.Activities assembly that contains NativeActvity class
        /// </summary>
        /// <returns>NativeActivity Type</returns>
        private Type GetNativeActivityType()
        {
            string nativeActivityAssemblyPath = System.Configuration.ConfigurationManager.AppSettings["NativeActivityAssemblyPath"];
            Assembly nativeAssembly = Assembly.LoadFrom(nativeActivityAssemblyPath);
            Type type = nativeAssembly.GetTypes().Where(t => t.Name.Equals(NativeActivity)).SingleOrDefault();
            return type;
        }

        /// <summary>
        /// This method gets all the classes that inherit from NativeActivity
        /// </summary>
        /// <param name="assembly">Assembly containing NativeActivity class</param>
        /// <param name="baseType">NativeActivity Type</param>
        /// <returns>Lisy of Types</returns>
        private List<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t)).ToList();
        }

        /// <summary>
        /// This method is used to get activity type and accordingly concatenates the type with IFEA
        /// </summary>
        /// <param name="value">Activity Name</param>
        /// <returns>Name concatenated with IFEA</returns>
        private string GetActivityType(string value)
        {
            string activityType = "";
            string activityAutomationCategory = System.Configuration.ConfigurationManager.AppSettings["AutomationActivityCategory"];
            if (value.Equals(AutomationActivityAssemblyName))
                activityType = activityAutomationCategory;
            else
            {
                int index = value.LastIndexOf(".");
                if (index > 0)
                    activityType = activityAutomationCategory + "-" + value.Substring(index + 1);
            }
            return activityType;
        }

        /// <summary>
        /// Loads assembly into the current app domain
        /// </summary>
        /// <param name="path"></param>
        public void LoadAssembly(string path)
        {

            Assembly assembly = Assembly.LoadFrom(path);
        }


        public void LoadAssembly()
        {
            DirectoryInfo dir;
            var path = ConfigurationManager.AppSettings["LoadAssembly"];
            if (!String.IsNullOrEmpty(path))
            {
                //LoadAssembly(path);
                if (Directory.Exists(path))
                {
                    dir = new DirectoryInfo(path);
                    FileInfo[] fInfo = dir.GetFiles("*.dll");
                    foreach (FileInfo fis in fInfo)
                    {
                        path = fis.DirectoryName + "\\" + fis.Name;
                        LoadAssembly(path);

                    }
                }
            }

        }
    }
}
