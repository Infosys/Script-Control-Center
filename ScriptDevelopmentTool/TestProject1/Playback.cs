/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Infosys.ATR.UIAutomation.SEE;
using Infosys.ATR.UIAutomation.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{

    public class ParentApp
    {
        public string Id { get; set; }
        public string App { get; set; }
        public IntPtr wHandle { get; set; }
        public int ProcessId { get; set; }
    }

    [TestClass]
    public class Playback
    {
        PlayActions actions = null;

        [TestMethod]
        public void Play()
        {


            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(@"C:\Usecase\20150318182657.atrwb");
            UseCase UseCaseCaptured = Infosys.ATR.UIAutomation.SEE.SerializeAndDeserialize.Deserialize(xmlDoc.OuterXml, typeof(UseCase)) as UseCase;
            var apps = UseCaseCaptured.Activities.Where(a1 => a1.TargetApplication != null && !String.IsNullOrEmpty(a1.TargetApplication.ApplicationExe)).ToList();

            var parentapps = apps.Where(a => String.IsNullOrEmpty(a.ParentId));

            List<ParentApp> parents = new List<ParentApp>();

            parentapps.ToList().ForEach(p =>
            {
                ParentApp pa = new ParentApp { App = p.TargetApplication.TargetApplicationAttributes[2].Value, Id = p.Id };
                parents.Add(pa);
            });

            actions = new PlayActions(UseCaseCaptured);
            actions.ReadyToExecute += actions_ReadyToExecute;
            actions.InitiatePlayBack();


            apps.ForEach(a =>
            {
                var p = a.TargetApplication.ApplicationExe;
               // if (!p.Equals(@"C:\Windows\Explorer.EXE"))//temp fix
                {
                    if (String.IsNullOrEmpty(a.ParentId))
                    {

                        ProcessStartInfo psi = new ProcessStartInfo(p);
                        Process process = Process.Start(psi);
                        System.Threading.Thread.Sleep(100);

                        var parent = parents.FirstOrDefault(pa => pa.Id == a.Id);
                        parent.wHandle = process.Handle;
                        parent.ProcessId = process.Id;
                        System.Threading.Thread.Sleep(1000);
                        actions.ExecuteActionsOn(p, process.Handle, process.Id);
                    }
                    else
                    {
                        var p1 = parents.FirstOrDefault(p2 => p2.Id == a.ParentId);
                        parents.Add(new ParentApp { App = p1.App, Id = a.Id, ProcessId = p1.ProcessId, wHandle = p1.wHandle });

                        if (p1 != null)
                        {
                            actions.ExecuteActionsOn(p, p1.wHandle, p1.ProcessId);
                        }


                    }
                }
            });

        }

        void actions_ReadyToExecute(PlayActions.ReadyToExecuteEventArgs e)
        {

        }

    }
}
