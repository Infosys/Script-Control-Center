/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;

using Entity = Infosys.ATR.Editor.Entities;
using Infosys.ATR.UIAutomation.Entities;

namespace Infosys.ATR.Editor.Services
{
    public class Utilities
    {
        public static int SequenceNumber { get; set; }
        public static int SelectedSequenceNumber { get; set; }
        static Dictionary<Editor.Views.Editor, bool> _editors;

        public static Dictionary<Editor.Views.Editor, bool> Editors
        {
            get
            {
                if (_editors == null)
                    _editors = new Dictionary<Views.Editor, bool>();
                return _editors;
            }
            set
            {
                _editors = value;
            }
        }

        private static Dictionary<string, bool> _savedTabs;
        public static Dictionary<string, bool> SavedTabs
        {
            get
            {
                if (_savedTabs == null)
                    _savedTabs = new Dictionary<string, bool>();
                return _savedTabs;
            }
            set { _savedTabs = value; }
        }

        private static List<string> _openedTabs;
        public static List<string> OpenedTabs
        {
            get
            {
                if (_openedTabs == null)
                    _openedTabs = new List<string>();
                return _openedTabs;
            }
            set { _openedTabs = value; }
        }

        public static string CurrentTab { get; set; }

        internal static string Serialize<T>(T obj)
        {
            using (UTF8Writer stream = new UTF8Writer())
            {
                XmlSerializer serialize = new XmlSerializer(typeof(AutomationConfig));
                serialize.Serialize(stream, obj);
                return stream.ToString();

            }
        }

        internal static T Deserialize<T>(string s)
        {
            T t = default(T);
            XmlSerializer xml = new XmlSerializer(typeof(AutomationConfig));
            t = (T)xml.Deserialize(new StringReader(s));
            return t;
        }


        internal static void Write(string path, string s)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(s);
            }
        }

        //internal static void RunBat(string fileName)
        //{
        //    ProcessStartInfo psi = new ProcessStartInfo(@"bat\python.bat");
        //    psi.Arguments = fileName;
        //    psi.CreateNoWindow = true;
        //    Process p = new Process();
        //    p.StartInfo = psi;
        //    p.Start();
        //}

        internal static string RunPython(string ifeaPath, string fileName)
        {
            ProcessStartInfo psi = new ProcessStartInfo(ifeaPath);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            Process p = Process.Start(psi);
            psi.Arguments = string.Format("{0}{1}", "-r ", Path.GetDirectoryName(fileName));
            psi.WorkingDirectory = Path.GetDirectoryName(ifeaPath);
            p = Process.Start(psi);
            p.WaitForExit();
            var output = p.StandardOutput.ReadToEnd();


            if (!String.IsNullOrEmpty(output))
                return output;

            if (p.StandardError != null)
            {
                var error = p.StandardError.ReadToEnd();
                if (!String.IsNullOrEmpty(error))
                {
                    throw new Exception(error);
                }
            }

            return "";
        }


        internal static Views.Editor GetKey()
        {
            foreach (KeyValuePair<Editor.Views.Editor, bool> kvp in Utilities.Editors)
            {
                if (kvp.Key._ucName == Utilities.CurrentTab)
                {
                    return kvp.Key;
                }
            }
            return null;
        }

    }
    internal class UTF8Writer : StringWriter
    {

        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }


}
