/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.UIAutomation.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RazorEngine.Templating;
using RazorEngine;

namespace CodeGenerationEngine.Model
{
    public class Utilities
    {
        internal static string Serialize<T>(T obj)
        {
            using (UTF8Writer stream = new UTF8Writer())
            {
                XmlSerializer serialize = new XmlSerializer(typeof(T));
                serialize.Serialize(stream, obj);
                return stream.ToString();
            }
        }
        internal static T Deserialize<T>(string s)
        {
            T t = default(T);
            XmlSerializer xml = new XmlSerializer(typeof(T));
            t = (T)xml.Deserialize(new StringReader(s));
            return t;
        }
        public static void Write(string path, string s)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(s);
            }
        }
        public static void GenerateCode<T>(string templateFilePath, string codeFilePath, T obj)
        {
            var config = new RazorEngine.Configuration.TemplateServiceConfiguration();
            config.DisableTempFileLocking = true;
            config.CachingProvider = new DefaultCachingProvider(t => { });
            var service = RazorEngineService.Create(config);
            Engine.Razor = service;

            using (var writer = new StreamWriter(codeFilePath))
            {
                writer.Write(Engine.Razor.RunCompile((new StreamReader(templateFilePath).ReadToEnd()), typeof(T).Name, typeof(T), obj));//new approach
                //writer.Write(RazorEngine.Razor.Parse<T>(new StreamReader(templateFilePath).ReadToEnd(),obj)); // old approach
            }
        }
    }
    internal class UTF8Writer : StringWriter
    {
        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }


}
