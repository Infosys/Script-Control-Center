/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SE = Infosys.WEM.Scripts.Service.Contracts.Data;
using OE = Infosys.IAP.CommonClientLibrary.Models;
using Infosys.WEM.Infrastructure.Common;

namespace Infosys.WEM.ScriptExecutionLibrary.Translator
{
    public class Script_SE
    {
        public static Script ScriptFromSE(SE.Script scriptSE)
        {
            using (LogHandler.TraceOperations("Script_SE:ScriptFromSE", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                Script scripttobeExecuted = null;
                if (scriptSE != null)
                {
                    scripttobeExecuted = new Script();
                    scripttobeExecuted.ExecutionDir = scriptSE.WorkingDir;
                    scripttobeExecuted.TaskCmd = scriptSE.TaskCmd;
                    scripttobeExecuted.TaskType = scriptSE.TaskType;

                    if (!string.IsNullOrEmpty(scriptSE.ArgString))
                    {
                        // scripttobeExecuted.Parameters.Add(new Parameter() { ParameterValue=scriptSE.Parameters[0].DefaultValue });
                    }
                    else if (scriptSE.Parameters != null && scriptSE.Parameters.Count > 0)
                        scripttobeExecuted.Parameters = ScriptParameter_SE.ScriptParameterListFromSE(scriptSE.Parameters);
                    //else if (!string.IsNullOrEmpty(scriptSE.ArgString))
                    //{
                    //    scripttobeExecuted.Parameters = new List<Parameter>();
                    //    //string[] args = scriptSE.ArgString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    //    Regex rgx = new Regex(@"[^\s""]+|""[^""]*""", RegexOptions.IgnoreCase);
                    //    MatchCollection matches = rgx.Matches(scriptSE.ArgString);
                    //    if (matches.Count > 0)
                    //    {

                    //        //foreach (string arg in args)
                    //        foreach (Match match in matches)
                    //        {
                    //            //add nameless parameter
                    //            // scripttobeExecuted.Parameters.Add(new Parameter() { ParameterValue = arg });
                    //            string value = match.Value;
                    //            if (value.Contains("\""))
                    //                value = value.Replace("\"", "");

                    //            scripttobeExecuted.Parameters.Add(new Parameter() { ParameterValue = value });
                    //        }
                    //    }
                    //}
                    scripttobeExecuted.ScriptName = scriptSE.Name + "." + scriptSE.ScriptType;
                    scripttobeExecuted.IfeaScriptName = scriptSE.IfeaScriptName;
                    scripttobeExecuted.RunAsAdmin = scriptSE.RunAsAdmin;
                    scripttobeExecuted.MethodName = scriptSE.CallMethod;
                }
                return scripttobeExecuted;
            }
        }

        public static Script ScriptFromOE(OE.ContentMeta scriptSE)
        {
            using (LogHandler.TraceOperations("Script_SE:ScriptFromOE", LogHandler.Layer.ScriptEngine, System.Guid.Empty, null))
            {
                Script scripttobeExecuted = null;
                if (scriptSE != null)
                {
                    scripttobeExecuted = new Script();
                    scripttobeExecuted.ExecutionDir = scriptSE.WorkingDir;
                    scripttobeExecuted.TaskCmd = scriptSE.TaskCommand;
                    scripttobeExecuted.TaskType = scriptSE.TaskType;

                    if (!string.IsNullOrEmpty(scriptSE.ArgumentString))
                    {
                        // scripttobeExecuted.Parameters.Add(new Parameter() { ParameterValue=scriptSE.Parameters[0].DefaultValue });
                    }
                    else if (scriptSE.Parameters != null && scriptSE.Parameters.Count > 0)
                        scripttobeExecuted.Parameters = ScriptParameter_SE.ScriptParameterListFromOE(scriptSE.Parameters);

                    scripttobeExecuted.ScriptName = scriptSE.Name + "." + scriptSE.ContentType;
                    scripttobeExecuted.IfeaScriptName = scriptSE.IfeaScriptName;
                    scripttobeExecuted.RunAsAdmin = scriptSE.RunAsAdmin;
                    scripttobeExecuted.MethodName = scriptSE.CallMethod;
                }
                return scripttobeExecuted;
            }
        }
    }
}
