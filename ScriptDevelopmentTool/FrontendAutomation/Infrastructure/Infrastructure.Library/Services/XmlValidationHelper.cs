/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
//----------------------------------------------------------------------------------------
// patterns & practices - Smart Client Software Factory - Guidance Package
//
// This file was generated by this guidance package as part of the solution template
//
// The XmlValidationHelper class is a helper used internally to work with xml serialization and validation
// 
//  
//
//
// Latest version of this Guidance Package: http://go.microsoft.com/fwlink/?LinkId=62182
//----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;

namespace IMSWorkBench.Infrastructure.Library
{
    internal static class XmlValidationHelper
    {
        public static TRootElement DeserializeXml<TRootElement>(string xml, string xsdResourceName, string schemaUri)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TRootElement));

            using (XmlReader reader = GetValidatingReader(xml, xsdResourceName, schemaUri))
                return (TRootElement)serializer.Deserialize(reader);
        }

        public static XmlReader GetValidatingReader(string xml, string xsdResourceName, string schemaUri)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("{1}.{0}", xsdResourceName, typeof(XmlValidationHelper).Namespace));
            XmlTextReader schemaReader = new XmlTextReader(stream);
            stream.Dispose();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(schemaUri, schemaReader);
            StringReader xmlStringReader = new StringReader(xml);
            XmlReader catalogReader = XmlReader.Create(xmlStringReader, settings);

            return catalogReader;
        }
    }
}
