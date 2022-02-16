/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;
using System.IO;

namespace Infosys.ATR.UIAutomation.SEE
{
    public static class SerializeAndDeserialize
    {
        public static string Serialize(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            Utf8StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, obj);
            return writer.ToString();
        }
        
        public static object Deserialize(string xmlObj, Type type)
        {
            StringReader stringReader = new StringReader(xmlObj);
            XmlSerializer serializer = new XmlSerializer(type);
            return serializer.Deserialize(stringReader);
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
    }
}
