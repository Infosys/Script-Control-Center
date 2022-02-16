/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.IO;
using System.Reflection;

namespace Infosys.ATR.OCRWrapper
{
    public static class TextRecognitionManager
    {
        static string defaultOCRWrapper = "Infosys.ATR.TessaractWrapper.dll";
        static ITextRecognition ocr = null;

        public static string ReadTextArea(double x, double y, double height, double width, string filter = "", float imageResizeCoeff=1)
        {
            string detectedText = null;
            if (ocr == null)
            {
                //load ocr
                ocr = GetOCREngine();
            }

            if (ocr != null)
                detectedText = ocr.ReadTextArea(x, y, height, width, filter, imageResizeCoeff);

            return detectedText;
        }

        public static string ReadTextArea(double x, double y, double height, double width, TextType filter, float imageResizeCoeff)
        {
            string detectedText = null;
            string alphanumericFilter = string.Empty;
            if (ocr == null)
            {
                //load ocr
                ocr = GetOCREngine();
            }

            if (ocr != null)
                alphanumericFilter = GetFilterFromEnum(filter);
                detectedText = ocr.ReadTextArea(x, y, height, width, alphanumericFilter,imageResizeCoeff);

            return detectedText;
        }

        private static string GetFilterFromEnum(TextType filter)
        {
            string alphanumericFilter = "";
            switch (filter)
            {
                case TextType.DHCP:
                    alphanumericFilter = "YesNo";
                    break;
                case TextType.DiskSpace:
                    alphanumericFilter = "GgTMKBb1234567890";
                    break;
                case TextType.Domain:
                    alphanumericFilter = "abcdefghijklmnopqrstuvwxyz.";
                    break;
                case TextType.IPAddress:
                    alphanumericFilter = "1234567890.";
                    break;
                case TextType.MacAddress:
                    alphanumericFilter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
                    break;
            }
            return alphanumericFilter;
        }

        public static ITextRecognition GetOCREngine()
        {
            ITextRecognition ocreng = null;
            string directory = Directory.GetCurrentDirectory();
            string ocrdll = "";
            // below code till //end added by Suchi to stop using appsetting 
            // CAUTION: in the OCRWarpper folder there has to be only one dll i.e. the wrapper itself.
            //              rest all supporting dlls (if any should be in another child folder but not on the parent OCRWrapper folder directly
            //              otherwise the below logic may fail to assign the right dll to the variable- ocrdll)
            string[] filePaths = Directory.GetFiles(directory + @"\OCRWrapper", "*.dll");
            if (filePaths.Count() > 0)
            {
                ocrdll = filePaths[0];
            }
            //end
            
            if (string.IsNullOrEmpty(ocrdll))
                ocrdll = defaultOCRWrapper;
            if (File.Exists(ocrdll))
            {
                var dll = Assembly.LoadFile(ocrdll);
                Type[] types = dll.GetTypes();
                foreach (Type t in types)
                {
                    if (t.GetInterfaces().Contains(typeof(ITextRecognition)))
                    {
                        ocreng = Activator.CreateInstance(t) as ITextRecognition;
                        break;
                    }
                }
            }
            else
                throw new Exception("Either OCR dll is not configured or it is missing.");
            return ocreng;
        }
    }
}
