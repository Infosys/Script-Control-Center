/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using IMSWorkBench.Infrastructure.Library.Services;
using Infosys.ATR.Editor.Entities;
using Infosys.ATR.Editor.Constants;

using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.CompositeUI;
using IMSWorkBench.Infrastructure.Interface;
using Microsoft.Practices.CompositeUI.EventBroker;

using Infosys.ATR.UIAutomation.Recorder.ScreenCapture;
using Infosys.IAP.CommonClientLibrary;

namespace Infosys.ATR.Editor.Views
{
    public partial class ImageEditorPresenter : Presenter<IImageEditor>
    {
        List<Entities.State> imagestates = new List<Entities.State>();

        [EventPublication(EventTopicNames.Capture, PublicationScope.WorkItem)]
        public event EventHandler<Selector.ImageCapturedArguements> Capture;

        [EventPublication(EventTopicNames.UpdateName, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<String>> UpdateName;

        [EventPublication(EventTopicNames.UpdateAppProperties, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<String[]>> UpdateAppProperties;

        [EventPublication(EventTopicNames.UpdateControlProperties, PublicationScope.WorkItem)]
        public event EventHandler<EventArgs<String>> UpdateControlProperties;

        [EventSubscription(EventTopicNames.CopyControl,ThreadOption.UserInterface)]
        public void OnUpdateControlProperties(object sender, EventArgs<string> eventArgs)
        {
            if (UpdateControlProperties != null)
            {
                UpdateControlProperties(sender, eventArgs);
            } 
        }

        Area _area;
        string _name;

        /// <summary>
        /// This method is a placeholder that will be called by the view when it has been loaded.
        /// </summary>
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

        public void CaptureThis(Area area,string name)
        {
            _area = area;
            _name = name;
            Selector _captureArea = new Selector();
            _captureArea.ImageCaptured += new Selector.ImageCapturedEventHandler(captureArea_ImageCaptured);
            _captureArea.Show();
            Win32.SetForegroundWindow(_captureArea.Handle);
            Win32.SetFocus(_captureArea.Handle);
        }

        void captureArea_ImageCaptured(Selector.ImageCapturedArguements e)
        {
            if (Capture != null)
            {
                e.Area = _area.ToString();
                e.State = _name;
                Capture(this, e);
            }
        }

        internal void OnUpdateName(string str)
        {
            if (UpdateName != null)
            {
                UpdateName(this,new EventArgs<String>(str));
            }
        }

        internal void UpdateAppProperties_Handler(string[] properties)
        {
            if (UpdateAppProperties != null)
            {
                UpdateAppProperties(this, new EventArgs<string[]>(properties));
            }
        }

        [EventSubscription(EventTopicNames.SaveImage, ThreadOption.UserInterface)]
        public void OnSaveImage(object sender, EventArgs<String> e)
        {
            this.View.ImagePath = e.Data;
            this.View.ShowImage();

        }

        [EventSubscription(EventTopicNames.Clear, ThreadOption.UserInterface)]
        public void OnClear(object sender, EventArgs e)
        {
            this.View.ClearAll();
        }

        [EventSubscription(EventTopicNames.UpdateImage, ThreadOption.UserInterface)]
        public void OnUpdateImage(object sender, EventArgs<object[]> e)
        {
            this.View.UpdateImageProperties(e.Data);

        }

        [EventSubscription(EventTopicNames.UpdateApplication, ThreadOption.UserInterface)]
        public void OnUpdateImage(object sender, EventArgs<ApplicationProperties> e)
        {
            this.View.UpdateApplicationProperties(e.Data);

        }


        [EventSubscription(EventTopicNames.UpdateBaseDir, ThreadOption.UserInterface)]
        public void OnUpdateImage(object sender, EventArgs<String> e)
        {
            this.View.BaseDir = e.Data;
        }

        [EventSubscription(EventTopicNames.SaveImageIntr, ThreadOption.UserInterface)]
        public void SaveImageHandler(object sender, EventArgs<Bitmap> e)
        {
            if (!String.IsNullOrEmpty(this.View.Name))
            {
                if (!this.View.States.Contains("default"))
                    this.View.States.Add("default");

                Selector.ImageCapturedArguements imageArgs = new Selector.ImageCapturedArguements();                
                imageArgs.Area =   Area.Center.ToString();
                imageArgs.State = _name = "default";
                imageArgs.Image = e.Data;
                captureArea_ImageCaptured(imageArgs);
            }
            else
                throw new Exception("No node to associate image with");
        }   
    }
}
