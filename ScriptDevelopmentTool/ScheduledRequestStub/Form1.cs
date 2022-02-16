using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infosys.WEM.Client;
using Infosys.WEM.Node;
using Infosys.WEM.Node.Service.Contracts.Message;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ScheduledRequestStub
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddScheduledRequestReqMsg req = new AddScheduledRequestReqMsg();
            req.Request = new Infosys.WEM.Node.Service.Contracts.Data.ScheduledRequest();
            req.Request.CategoryId = 9;
            req.Request.Requestor = @"admin";
            //req.Request.Id //no need to pass as it is applied by the service
            //req.Request.CreatedOn //no need to pass as it is applied by the service
            //req.Request.ExecuteOn // for future enhancements to schedule the task for future execution
            req.Request.InputParameters = "";
            req.Request.AssignedTo = "127.0.0.1";
            //req.Request.Message // not needed for new tasks
            //req.Request.OutputParameters //not needed for new tasks, needed when calling UpdateRequestExecutionStatus
            //req.Request.ModifiedBy // needed when calling UpdateRequestExecutionStatus
            //req.Request.ModifiedOn //no need to pass as it is applied by the service
            req.Request.Priority = 1;
            req.Request.RequestId = "02CADB06-20AE-4841-B3FD-9D1212871AE5"; //for workflow or script id
            req.Request.RequestType = Infosys.WEM.Node.Service.Contracts.Data.RequestTypeEnum.Workflow;
            req.Request.RequestVersion = 1;//workflow version if request type is workflow otherwise not need to assign for script
            req.Request.State = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.New;
            req.Request.StopType = Infosys.WEM.Node.Service.Contracts.Data.StopTypes.Limited;
            req.Request.CompanyId = 1;

            ScheduledRequest client = new ScheduledRequest("http://localhost:58127/WEMScheduledRequest.svc");
            var scheduledReqIds=client.ServiceChannel.AddScheduledRequest(req);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateRequestExecutionStatusReqMsg req = new UpdateRequestExecutionStatusReqMsg();
            req.ExecutionStatus = Infosys.WEM.Node.Service.Contracts.Data.RequestExecutionStatus.Completed;
            req.IAPNode = "127.0.0.1"; // same as the machine handling this request, may be different than the "assigned to"
            req.Id = "e240545f-390e-4a22-bc46-2b5f5424605f";
            req.Message = "successfully completed";
            req.ModifiedBy = @"admin";
            req.OutputParameters = "nothing";
            req.AssignedTo = "127.0.0.1"; //as per the value provided while adding this request

            ScheduledRequest client = new ScheduledRequest("http://localhost:58127/WEMScheduledRequest.svc");
            bool result = client.ServiceChannel.UpdateRequestExecutionStatus(req);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter() { ParameterName = "name1", ParameterValue = "value1" });
            parameters.Add(new Parameter() { ParameterName = "name2", ParameterValue = "value2" });

            string json = JSONSerialize(parameters);

            string str = Environment.UserDomainName + "\\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        private string JSONSerialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer jsonSer = new DataContractJsonSerializer(obj.GetType());
            jsonSer.WriteObject(stream, obj);
            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            string json = sr.ReadToEnd();
            return json;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string path = @"D:\IAP\University of California\rfp-52815\Attachment A.4 (Bidder References).xlsx";
            //string folder = System.IO.Path.GetDirectoryName(path);

            //string completestr = "this is the complete string" + Environment.NewLine + " from here the sub string is to be found" + Environment.NewLine + "is this fine";
            //string partstr = "from here the sub string is to be found";

            //string resultstr = completestr.Substring(completestr.IndexOf(partstr) + partstr.Length);

            //string pythonIntLoc64 = "c:\\64.exe";
            //string pythonIntLoc32 = "c:\\32.exe";

            //string compiler = "{$os=Get-WMIObject win32_operatingsystem " + Environment.NewLine;
            //compiler = compiler + "if ($os.OSArchitecture -eq \"64-bit\" ){" + Environment.NewLine;
            //compiler = compiler + pythonIntLoc64 + "#fileParam#" + Environment.NewLine;
            //compiler = compiler + "  }" + Environment.NewLine;
            //compiler = compiler + "  else" + Environment.NewLine;
            //compiler = compiler + "  {" + Environment.NewLine;
            //compiler = compiler + pythonIntLoc32 + "#fileParam#" + Environment.NewLine;
            //compiler = compiler + "  }" + Environment.NewLine;
            //compiler = compiler + "} ";

            //MessageBox.Show(compiler);

            //give access to the local Users group to modify files under it
            //string appdataFilepath = @"C:\ProgramData\Infosys\tt";
            //SecurityIdentifier users = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            //DirectorySecurity dirSec = System.IO.Directory.GetAccessControl(appdataFilepath);
            //dirSec.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            //System.IO.Directory.SetAccessControl(appdataFilepath, dirSec);

            //string base64 = EncodeToBase64("Infosys1");
            //Infosys.ATR.Packaging.Result result = Infosys.ATR.Packaging.Operations.Package(@"D:\temp\iapds\sample1\sample1.py");
            //Infosys.ATR.Packaging.Result result = Infosys.ATR.Packaging.Operations.Unpackage(@"D:\temp\sample.iapd", @"D:\temp");

            //Stream data = Infosys.ATR.Packaging.Operations.ExtractFile(@"D:\temp\sample1.iapd", @"\supporting\child\2.txt");
            //StreamToString(data);
            //Infosys.ATR.Packaging.Operations.ClosePackage();

            string xMLPath = "http://docserver/docvp/guid/name.iapw/folder1/folder2/some.atr";
            if (IsValidUrlFormat(xMLPath))
            {
                string iapwLoc = xMLPath.Substring(0, xMLPath.IndexOf(".iapw") + 5);
                string atrLoc = xMLPath.Replace(iapwLoc, "").Replace(@"/", "\\"); ;
                string atrDir = System.IO.Path.GetDirectoryName(atrLoc);
            }
            //System.Threading.Thread.FreeNamedDataSlot("iapwurl");
            //LocalDataStoreSlot localData = System.Threading.Thread.AllocateNamedDataSlot("iapwurl");
            //System.Threading.Thread.SetData(localData, "https://msdn.microsoft.com/en-us/library/system.threading.thread.allocatedataslot(v=vs.110).aspx");

            //System.Threading.Thread.FreeNamedDataSlot("iapwurl");
            //localData = System.Threading.Thread.AllocateNamedDataSlot("iapwurl");
            //System.Threading.Thread.SetData(localData, "https://msdn.x");


            //localData = System.Threading.Thread.GetNamedDataSlot("iapwurl");
            //string iapwurl = System.Threading.Thread.GetData(localData).ToString();
        }

        private string EncodeToBase64(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private const long BUFFER_SIZE = 4096;
        private string StreamToString(Stream fileContent)
        {
            long bufferSize = fileContent.Length < BUFFER_SIZE ? fileContent.Length : BUFFER_SIZE;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0; long bytesWritten = 0;
            string content = "";
            while ((bytesRead = fileContent.Read(buffer, 0, buffer.Length)) != 0)
            {
                content += System.Text.Encoding.UTF8.GetString(buffer);
                bytesWritten += bufferSize;
            }
            return content;
        }

        private bool IsValidUrlFormat(string url)
        {
            Uri uriTryResult;
            bool isValid = Uri.TryCreate(url, UriKind.Absolute, out uriTryResult) && (uriTryResult.Scheme == Uri.UriSchemeHttp || uriTryResult.Scheme == Uri.UriSchemeHttps);
            return isValid;
        }
    }

    public class Parameter
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }
}
