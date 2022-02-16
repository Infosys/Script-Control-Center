/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using IMSWorkBench.Infrastructure.Interface;
using Infosys.ATR.ProcessRecorder.Constants;
using Infosys.ATR.ProcessRecorder.Entities;
using Infosys.ATR.UIAutomation.Entities;
using Microsoft.Practices.CompositeUI.EventBroker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Infosys.ATR.ProcessRecorder.Views
{
    public class RecordingPresenter : Presenter<IRecording>
    {

        [EventPublication(Constants.EventTopicNames.PublishPlaybackScript, PublicationScope.Global)]
        public event EventHandler PublishPlaybackScript;

        [EventPublication(Constants.EventTopicNames.ExecutePlaybackScript, PublicationScope.Global)]
        public event EventHandler ExecutePlaybackScript; 

        [EventPublication(Constants.EventTopicNames.CloseRecordingTab, PublicationScope.Global)]
        public event EventHandler CloseRecordingTab;


        [EventPublication(EventTopicNames.ShowTransaction, PublicationScope.Global)]
        public event EventHandler<EventArgs<String>> RecorderViewStatusUpdate;

        public override void OnViewReady()
        {
            base.OnViewReady();
        }

        /// <summary>
        /// Close the view
        /// </summary>
        public override void OnCloseView()
        {
            base.CloseView();
        }
        internal static T Deserialize<T>(string s)
        {
            T t = default(T);
            XmlSerializer xml = new XmlSerializer(typeof(T));
            t = (T)xml.Deserialize(new StringReader(s));
            return t;
        }
        internal void GetUseCases()
        {
            string folderPath = ConfigurationManager.AppSettings["UsecaseLocation"];
            List<UseCasePE> lstUseCase = new List<UseCasePE>();

            if (Directory.Exists(folderPath))
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                FileInfo[] files = di.GetFiles("*.atrwb");

                files.ToList().ForEach(file =>
                {
                    UseCasePE useCase = new UseCasePE();
                    useCase.FileName = file.Name;
                    useCase.FilePath = file.FullName;
                    var objUseCase = Deserialize<UseCase>(File.ReadAllText(file.FullName));
                    if (objUseCase != null)
                    {   
                        useCase.Id = objUseCase.Id;
                        useCase.Name = objUseCase.Name;
                        useCase.MachineName = objUseCase.MachineName;
                        useCase.MachineType = objUseCase.MachineType;
                        useCase.OS = objUseCase.OS;
                        useCase.OSVersion = objUseCase.OSVersion;
                        useCase.ScreenResolution = objUseCase.ScreenResolution;
                        useCase.InitiatedOn = objUseCase.CreatedOn;
                        useCase.CreatedBy = objUseCase.CreatedBy;
                        useCase.Domain = objUseCase.Domain;
                        useCase.AssociatedTasks = objUseCase.Activities.Sum(x=>x.Tasks.Count());
                        lstUseCase.Add(useCase);
                    }
                });

                this.View.UseCases = lstUseCase;

                //if(lstUseCase==null)
                    //System.Windows.Forms.MessageBox.Show("No Record Avalable", "IAP - Recording View", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            //else
                //System.Windows.Forms.MessageBox.Show("No Record Avalable", "IAP - Recording View", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

         public string UseCasesFilePath { get; set; }
        
         internal void GeneratePlaybackScript()
         {
             PublishPlaybackScript(this, new EventArgs<string>(UseCasesFilePath));
         }
         internal void UpdateStatus(int count)
         {
             var status = string.Format("Recording History Details: Rowsfound {0}", count.ToString());
             RecorderViewStatusUpdate(this, new EventArgs<String>(status));
         }

         internal void RunReplaybackScript()   
         {
             ExecutePlaybackScript(this, new EventArgs<string>(UseCasesFilePath));
             string folder = Path.GetFileNameWithoutExtension(this.UseCasesFilePath);
             if (System.IO.Directory.Exists(folder))
                 DeleteFile(folder); 
         }

         /// <summary>
         /// Delete file from disk even if it is temporary taken by some other program
         /// </summary>
         /// <param name="filePath">Path to file</param>
         public static void DeleteFile(string FolderPath)
         {
             System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(deleteFileProcess), FolderPath);
         }
         private static void deleteFileProcess(object o)
         {
             try
             {
                 if (System.IO.Directory.Exists(o.ToString()))
                     System.IO.Directory.Delete(o.ToString(), true);
             }
             catch
             {
                 System.Threading.Thread.Sleep(5000);
                 deleteFileProcess(o);
                 return;
             }
         }
    }
}
