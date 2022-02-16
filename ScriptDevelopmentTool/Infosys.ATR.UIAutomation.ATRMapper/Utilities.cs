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

namespace Infosys.ATR.UIAutomation.ATRMapper
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
