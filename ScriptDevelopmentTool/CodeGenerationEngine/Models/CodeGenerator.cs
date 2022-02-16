/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.ATR.UIAutomation.ATRMapper;
using Infosys.ATR.UIAutomation.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerationEngine.Model 
{
    public class CodeGenerator :IDisposable
    {

        public class SendExecutionStatusArgs : EventArgs
        {                       
            public string StatusMessage { get; set; }
            public bool IsSuccess { get; set; }           
            public int PercentComplete { get; set; }
        }

        public delegate void SendExecutionStatusEventHandler(SendExecutionStatusArgs e);
        public static event SendExecutionStatusEventHandler SendExecutionStatus;

        public static string outputPath { get; set; }
        public static string atrwbPath { get; set; }
        internal static UseCase useCase { get; set; }
        internal static AutomationConfig atrModel { get; set; }
        internal static Dictionary<string, List<string>> CanonicalPaths { get; set; }
        internal static IList<PyBaseAssembly> BaseAssemblies { get; set; }
        internal static Dictionary<string, IList<PyAddReference>> AddReferences { get; set; }
        public static List<string> ParamNames { get; set; }
        public static string UseCaseName { get; set; }
        internal static string strParameter { get; set; }
 
        //private static CodeGenerator _instance;

        //public static CodeGenerator Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = new CodeGenerator(); 
        //        }
        //        return _instance;
        //    }
        //    set { }
        //}
        private static void LoadCodeGenerator()
        {
            sendExecutionStatus(true, "Initializing Component Started", 5);
            var atr = new clsATRmapper();
            ParamNames = new List<string>();
            useCase = Utilities.Deserialize<UseCase>((new StreamReader(atrwbPath)).ReadToEnd().ToString());
            if (useCase == null)
                return;

            outputPath = string.Format("{0}\\{1}_{2}", ConfigurationManager.AppSettings["CodeGenerationLoc"], useCase.Name, DateTime.Now.ToString("yyyy-MM-dd-hhmmssfff"));
            string atrfile = atr.ConvertToATR(new List<string>() { (new StreamReader(atrwbPath)).ReadToEnd().ToString() }, null, null, ApplicationMode.ImageCapture);
            UseCaseName = useCase.Name;
            if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);
            Utilities.Write(string.Format("{0}\\App.atr", outputPath), atrfile);
            atrModel = Utilities.Deserialize<AutomationConfig>(atrfile);
            CanonicalPaths = CanonicalPath();
            Init_Reference();
            sendExecutionStatus(true, "Initializing Component Completed", 10);
        }
        private static Dictionary<string, List<string>> CanonicalPath()
        {
            Dictionary<string, List<string>> _dctCanonical = new Dictionary<string, List<string>>();
            List<string> values;
            foreach (AppConfig app in atrModel.AppConfigs)
            {
                foreach (var screen in app.ScreenConfigs)
                {
                    foreach (var entity in screen.EntityConfigs)
                    {
                        string img = entity.EntityImageConfig.StateImageConfig.Select(x => x.CenterImageName.ImageName).SingleOrDefault();
                        values = new List<string>()
                        {
                            string.Format("{0}_{1}", screen.ScreenName, entity.EntityName).ToUpper(),
                            string.Format("{0}.{1}.{2}", app.AppName, screen.ScreenName, entity.EntityName),
                            string.Format("{0}",entity.EntityName.ToUpper()),
                            string.Format("{0}",Convert.ToString(entity.EntityControlConfig.ControlClass))
                        };
                        _dctCanonical.Add(Path.GetFileNameWithoutExtension(img), values);
                    }
                }
            }
            return _dctCanonical;
        }       
        private static void Init_Reference() 
        {
            var AddRef = new Dictionary<string, IList<PyAddReference>>();

            AddRef.Add("Resolver",new List<PyAddReference>()
            {
                new PyAddReference(){ Assembly="Infosys.ATR.WinUIAutomationRuntimeWrapper.dll",Imports=new List<ImpNamespace>()
                {
                    new ImpNamespace(){ FromNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper", ImportNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper"},
                    new ImpNamespace(){ FromNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper.Core", ImportNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper.Core"} 
                }},
            });

            AddRef.Add("Controller", new List<PyAddReference>()
            {
                new PyAddReference()
                { 
                    Assembly="Infosys.ATR.WinUIAutomationRuntimeWrapper.dll", 
                    Imports=new List<ImpNamespace>()
                    {
                        new ImpNamespace(){ FromNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper", ImportNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper"},
                        new ImpNamespace(){ FromNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper.Core", ImportNamespace="Infosys.ATR.WinUIAutomationRuntimeWrapper.Core"} 
                    } 
                },
                new PyAddReference(){ Assembly="IAP.Infrastructure.Services.Contracts.dll",Imports=new List<ImpNamespace>(){new ImpNamespace(){ ImportNamespace="IAP.Infrastructure.Services.Contracts", FromNamespace="IAP.Infrastructure.Services.Contracts"}}},
                new PyAddReference(){ Assembly="InfrastructureClientLibrary.dll",Imports=new List<ImpNamespace>(){new ImpNamespace(){ ImportNamespace="InfrastructureClientLibrary", FromNamespace="InfrastructureClientLibrary"}}},
                new PyAddReference(){ Assembly="mscorlib.dll",Imports=new List<ImpNamespace>(){new ImpNamespace(){ FromNamespace="System.Collections.Generic"}}}
            });

            AddRef.Add("Logger", new List<PyAddReference>()
            {                
                new PyAddReference(){ Assembly="IAP.Infrastructure.Services.Contracts.dll",Imports=new List<ImpNamespace>(){new ImpNamespace(){ ImportNamespace="IAP.Infrastructure.Services.Contracts", FromNamespace="IAP.Infrastructure.Services.Contracts"}}},
                new PyAddReference(){ Assembly="InfrastructureClientLibrary.dll",Imports=new List<ImpNamespace>(){new ImpNamespace(){ ImportNamespace="InfrastructureClientLibrary", FromNamespace="InfrastructureClientLibrary"}}},
                new PyAddReference(){ Assembly="mscorlib.dll",Imports=new List<ImpNamespace>(){new ImpNamespace(){ FromNamespace="System.Collections.Generic"}}}
            });

            AddReferences = AddRef;

        }
        private static void CopyImges()
        {
            string sourceFolderPath = ConfigurationManager.AppSettings["TaskImageLocation"];
            string copyToPath = ConfigurationManager.AppSettings["CodeGenerationLoc"];
            foreach (var activity in CodeGenerator.useCase.Activities)
            {
                foreach (var task in activity.Tasks) 
                {
                    string filePath = string.Format("{0}\\{1}\\{2}.jpg", sourceFolderPath, useCase.Id, task.Id);
                    if (File.Exists(filePath))
                    {
                        if (!Directory.Exists(outputPath))
                            Directory.CreateDirectory(outputPath);

                        File.Copy(filePath, string.Format("{0}\\{1}", outputPath, Path.GetFileName(filePath)));
                    }
                }
            }
           
        }
        public static  void GenerateCode()
        {
            LoadCodeGenerator();

            sendExecutionStatus(true, "Generating Iron Python Script Started", 15);
            var iapResolverBase = new Iap_Resolver_Base();
            Utilities.GenerateCode<Iap_Resolver_Base>(@"Template/iap_resolver_base_template.txt", string.Format("{0}/iap_resolver_base.py", outputPath), iapResolverBase);
            sendExecutionStatus(true, "Generating Iron Python Script.", 25);
            var iapResolver = new Iap_Resolver();
            Utilities.GenerateCode<Iap_Resolver>(@"Template/iap_resolver_template.txt", string.Format("{0}/iap_resolver.py", outputPath), iapResolver);
            sendExecutionStatus(true, "Generating Iron Python Script.", 35);
            var iapControllerBase = new Iap_Controller_Base();
            Utilities.GenerateCode<Iap_Controller_Base>(@"Template/iap_controller_base_template.txt", string.Format("{0}/iap_controller_base.py",outputPath), iapControllerBase);
            sendExecutionStatus(true, "Generating Iron Python Script.", 45);
            var iapController = new Iap_Controller();
            Utilities.GenerateCode<Iap_Controller>(@"Template/iap_controller_template.txt", string.Format("{0}/iap_controller.py", outputPath), iapController);
            sendExecutionStatus(true, "Generating Iron Python Script.", 55);
            var iapModel = new Iap_Model();
            Utilities.GenerateCode<Iap_Model>(@"Template/iap_model_template.txt", string.Format("{0}/iap_model.py", outputPath), iapModel);
            sendExecutionStatus(true, "Generating Iron Python Script.", 60);
            var iapConstant = new iap_constants();
            Utilities.GenerateCode<iap_constants>(@"Template/iap_constants_template.txt", string.Format("{0}/iap_constants.py", outputPath), iapConstant);
            sendExecutionStatus(true, "Generating Iron Python Script.", 65);
            var iapLog_ini = new LoggingIni();
            Utilities.GenerateCode<LoggingIni>(@"Template/logging_ini_template.txt", string.Format("{0}/logging.ini", outputPath), iapLog_ini);
            sendExecutionStatus(true, "Generating Iron Python Script.", 70);
            var iapApp_ini = new AppIni();
            Utilities.GenerateCode<AppIni>(@"Template/app_ini_template.txt", string.Format("{0}/App.ini", outputPath), iapApp_ini);
            sendExecutionStatus(true, "Generating Iron Python Script.", 75);
            var iapLogger = new Iap_Logger();
            Utilities.GenerateCode<Iap_Logger>(@"Template/iap_logger_template.txt", string.Format("{0}/iap_logger.py", outputPath), iapLogger);
            sendExecutionStatus(true, "Generating Iron Python Script.", 80);
            var iapUtilities = new Iap_Utilities();
            Utilities.GenerateCode<Iap_Utilities>(@"Template/iap_utilities_template.txt", string.Format("{0}/iap_utilities.py", outputPath), iapUtilities);

            var iapParameters = new Iap_Parameters(strParameter);
            Utilities.GenerateCode<Iap_Parameters>(@"Template/iap_parameter_template.txt", string.Format("{0}/iap_parameters.py", outputPath), iapParameters);

            sendExecutionStatus(true, "Generating Iron Python Script.", 85);
            CopyImges();
            sendExecutionStatus(true, "Copying Images", 100);
        }


        private static void sendExecutionStatus(bool isSuccess, string statusMessage, int Progress)
        {
            SendExecutionStatusArgs e = new SendExecutionStatusArgs();
            e.IsSuccess = isSuccess;           
            e.StatusMessage = statusMessage;            
            e.PercentComplete = Progress;
            if (SendExecutionStatus != null)
                SendExecutionStatus((SendExecutionStatusArgs)e);
        }

        public void Dispose()
        {
           
        }

        //public static void DisposeInstance()
        //{
        //    if (_instance != null)
        //    {
        //        Instance.Dispose();
        //        Instance = null;
        //    }
        //}
    }
}
