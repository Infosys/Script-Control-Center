/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using CodeGenerationEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new string[] { @"D:\POC\telstra\output\recording\recordingfiles\atrs\20160418174802.atrwb" }; 

            if (args.Length != 0)
            {
                CodeGenerator.atrwbPath = args[0].ToString();
                CodeGenerator.SendExecutionStatus += CodeGenerator_SendExecutionStatus;
                CodeGenerator.GenerateCode();
                List<string> lst = CodeGenerator.ParamNames;
                Console.WriteLine("Python Code Generated successfully........!!!");
                Console.WriteLine("Output Folder Location: {0}", CodeGenerator.outputPath);
                Console.Read();
            }
            else
            {
                Console.WriteLine(".atrwb file path missing.");
                Console.WriteLine("Proper Usage is : Test.exe path");
                Console.Read();
            }

        }

        private static void CodeGenerator_SendExecutionStatus(CodeGenerator.SendExecutionStatusArgs e)
        {
            Console.WriteLine(String.Format("{0} {1}", e.StatusMessage, e.PercentComplete.ToString()));
        }
    }
}
