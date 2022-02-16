using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infosys.WEM.ScriptExecutionLibrary;
using System.Security;
using TestProject1.Props;

namespace TestProject1
{
    [TestClass]
    public class RemoteSSHScriptTest
    {
        [TestMethod()]
        [TestCategory("SSHClient")]
        public void testRemoteScriptExecution()
        {
            
            ScriptIndentifier scriptId = new ScriptIndentifier();
            scriptId.UserName = Resources.USERNAME;
            string password = Resources.PASSWORD;
            char[] pass = password.ToCharArray();
            var s = new SecureString();
            for (int count=0; count < pass.Length; count++ )
                s.AppendChar(pass[count]);
            //s.AppendChar('i');s.AppendChar('m');s.AppendChar('n');s.AppendChar('a');s.AppendChar('@');
            //s.AppendChar('1');s.AppendChar('2');s.AppendChar('3');
            scriptId.Password = s;

            ExecuteSSH execSSH = new ExecuteSSH();
            execSSH.ExecuteRemoteCommandSSH("listfiles.sh", null, Resources.HOST, "C:\\ATR\\TestProject1\\data", scriptId);

            //TODO:Write Asset to check the return result is what is expected.
        }

        [TestMethod()]
        [TestCategory("SSHClient")]
        
        public void testRemoteScriptExecution_InvalidPath()
        {

        }

        [TestMethod()]
        [TestCategory("SSHClient")]

        public void testRemoteScriptExecution_WrongScript()
        {

        }


    }
}
