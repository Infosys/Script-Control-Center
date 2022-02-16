using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.WEM.ScriptExecutionLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
    [TestClass]
    public class ScriptExecutionManagerTest
    {
        [TestMethod()]
        [TestCategory("ScriptExecution")]
        public void testScriptExecutionManager()
        {
            var args = new string[] { "2", "7", "http://localhost/iapwemservices/WEMScriptService.svc", "", "", "", "", "", "" };
            ScriptIndentifier scriptIden = new ScriptIndentifier();
            scriptIden.ScriptId = int.Parse(args[1]);
            scriptIden.SubCategoryId = int.Parse(args[0]);
            scriptIden.WEMScriptServiceUrl = args[2];
            Parameter param = new Parameter();
            scriptIden.Parameters = new List<Parameter>();
            scriptIden.Parameters.Add(new Parameter() { ParameterName = "param1", ParameterValue = "value3", allowedValues="value1,value2"});
            scriptIden.Parameters.Add(new Parameter() { ParameterName = "param2", ParameterValue = "value2", allowedValues = "value1,value2" });

            List<ExecutionResult> results = ScriptExecutionManager.Execute(scriptIden);
        }
        
        [TestMethod()]
        [TestCategory("ScriptExecutionManager")]
        public void testParameter_ValidAllowedValues()
        {
            bool result = ScriptExecutionManager.CheckAllowedParamValues("value1", "value2,value1");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        [TestCategory("ScriptExecutionManager")]
        public void testParameter_InValidAllowedValues()
        {
            bool result = ScriptExecutionManager.CheckAllowedParamValues("value3", "value2,value1");
            Assert.IsFalse(result);
        }


        [TestMethod()]
        [TestCategory("ScriptExecutionManager")]
        public void testParameter_NoAllowedValues()
        {
            bool result = ScriptExecutionManager.CheckAllowedParamValues("value3", null);
            Assert.IsTrue(result);
        }
        
    }
}
