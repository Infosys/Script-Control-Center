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
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Threading.Tasks;

namespace Infosys.WEM.ScriptExecutionLibrary
{
    public class ExecuteSSH : ExecuteBase
    {
        ConnectionInfo connectionInfo;
       
        List<ExecutionResult> consolidatedOutput = new List<ExecutionResult>();

        public ExecuteSSH()
        {
            string executeAt = "local";
        }
        public string scriptToBeExecuted = string.Empty;
        public string fileContent;

        public ExecuteSSH(ConnectionInfo connectionInfo)
        {
            this.connectionInfo = connectionInfo;
        }

        /// <summary>
        /// Starts the ssh invocation of commands to remote server. 
        /// </summary>
        public override List<ExecutionResult> Start()
        {
            ExecutionResult output = null;
            List<string> remoteServers = ScriptIden.RemoteServerNames.Split(new char[] { ',' }).ToList();
            Parallel.ForEach(remoteServers, s => ExecuteRemoteCommandSSH(ScriptName, /*arr,*/ null, s, FilePath, ScriptIden));
            return consolidatedOutput;
        }

        /// <summary>
        /// Constructs the parameters to be sent via ssh to the remote server.
        /// </summary>
        private string getParameters()
        {
            return "";
        }

        public ExecutionResult ExecuteRemoteCommandSSH(
                string scriptToBeExecuted,
                string parameterString,
                string serverName,
                string filePath,
                ScriptIndentifier scriptIden
            )
        {
            ExecutionResult output1 = new ExecutionResult();
            StringBuilder result = new StringBuilder();
            string dirpath = String.Empty;
            if (!String.IsNullOrEmpty(scriptIden.WorkingDir))
            {
                dirpath = scriptIden.WorkingDir;
            }
            using (Runspace runSpace = RunspaceFactory.CreateRunspace())
            {
                String insecurePassword = null;
                if (scriptIden.Password != null)
                {
                    insecurePassword = SecureStringToString(scriptIden.Password);
                }
                connectionInfo = getConnectionInfo(serverName, scriptIden.UserName, insecurePassword, scriptIden.LinuxKeyPath);
                try
                {
                    bool IsServerExist = true;
                    if (!IsCommand)
                    {
                        try
                        {
                            using (var sftp = new SftpClient(connectionInfo))
                            {
                                sftp.Connect();
                                if (!string.IsNullOrEmpty(FileContent))
                                {
                                    byte[] byteArray = Encoding.UTF8.GetBytes(FileContent);
                                    MemoryStream stream = new MemoryStream(byteArray);
                                    using (var fileStream = stream)
                                    {
                                        sftp.BufferSize = 4 * 1024;
                                        if (!String.IsNullOrEmpty(dirpath))
                                            sftp.ChangeDirectory(dirpath);
                                        sftp.UploadFile(fileStream, Path.GetFileName(FilePath), null);
                                    }
                                    sftp.Disconnect();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            IsServerExist = false;
                            output1.ErrorMessage = ex.Message.ToString();
                            output1.IsSuccess = false;
                            output1.ComputerName = serverName;

                        }
                        if (IsServerExist)
                        {
                            if (String.IsNullOrEmpty(dirpath))
                                dirpath = @"./";

                            if (scriptIden.Parameters != null && scriptIden.Parameters.Count > 0)
                            {

                                executeSSHCommand("sh " + dirpath + "/" + scriptToBeExecuted + ParameterString, serverName);
                            }
                            else
                            {
                                executeSSHCommand("sh " + dirpath + "/" + scriptToBeExecuted, serverName);
                            }
                        }

                    }
                    else
                    {
                        if (IsServerExist)
                        {
                            if (scriptIden.Parameters != null && scriptIden.Parameters.Count > 0)
                            {
                           
                                executeSSHCommand(scriptToBeExecuted + ParameterString, serverName);
                            }
                            else
                            {
                                executeSSHCommand(scriptToBeExecuted, serverName);
                            }
                        }
                    }
                    }
                
                catch (Exception ex)
                {
                    output1.ErrorMessage = ex.Message.ToString();
                    output1.IsSuccess = false;
                    output1.ComputerName = serverName;
                }
                if(!string.IsNullOrEmpty(output1.ComputerName))
                {
                    lock(consolidatedOutput)
                    consolidatedOutput.Add(new ExecutionResult() { ComputerName = output1.ComputerName, IsSuccess = false, ErrorMessage = output1.ErrorMessage });
                }
            
                return output1;
            }
        }
        public string executeSSHCommand(string command,string server)
        {
            ExecutionResult output = new ExecutionResult();
            string commandResult = "";
            string filename = "";
            using (var client = new SshClient(connectionInfo))
            {
                try
                {
                    client.Connect();
                    var cmd = client.CreateCommand(command);
                    commandResult = cmd.Execute();
                    if (!string.IsNullOrEmpty(cmd.Error))
                    {
                        output.ErrorMessage = cmd.Error + cmd.Result;
                        Console.WriteLine(output.ErrorMessage);
                        output.IsSuccess = false;
                        output.ComputerName = server;
                    }
                    else
                    {
                        if (commandResult.Length > 0 && string.IsNullOrEmpty(output.ErrorMessage))
                        {
                            commandResult = commandResult.Substring(0, commandResult.Length - 1);
                        }
                        else
                        {
                            commandResult = string.Empty;
                        }
                        output.IsSuccess = true;
                        output.SuccessMessage = commandResult;
                        Console.WriteLine(output.SuccessMessage);
                        output.TransactionId = ScriptIden.TransactionId;
                        output.ComputerName = server;
                    }
                    client.Disconnect();
                    if (!IsCommand)
                    {
                        client.Connect();
                        if (command.Contains("sh "))
                            filename = command.Substring(3);
                        else
                            filename = command;
                        var cmdToDelete = client.CreateCommand("rm -f " + filename);
                        cmdToDelete.Execute();
                        client.Disconnect();
                    }
                }
                catch(Exception ex)
                {
                    output.ErrorMessage = ex.Message.ToString();
                    output.IsSuccess = false;
                    output.ComputerName = server;
                }
                lock (consolidatedOutput)
                    consolidatedOutput.Add(new ExecutionResult() { ComputerName = output.ComputerName, SuccessMessage = output.SuccessMessage, IsSuccess = output.IsSuccess, ErrorMessage = output.ErrorMessage });
                output = null;
            }
            return commandResult;
        }

        String SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
        private ConnectionInfo getConnectionInfo(string host, string username, String password,string LinuxKey)
        {
            if (!string.IsNullOrEmpty(LinuxKey))
             {

                 var keyFile = new PrivateKeyFile(LinuxKey); 
                 var keyFiles = new[] { keyFile };
                 var connectionInfo = new PrivateKeyConnectionInfo(host, username, keyFiles);
                return connectionInfo;
             }
             else
             {
               var connectionInfo = new PasswordConnectionInfo(host, username, password);
                var encoding = new Renci.SshNet.Common.ASCIIEncoding();
            connectionInfo.PasswordExpired += delegate(object sender, AuthenticationPasswordChangeEventArgs e)
            {
                e.NewPassword = encoding.GetBytes(password + "1");
            };
            return connectionInfo;
        }
        }

    }
}
