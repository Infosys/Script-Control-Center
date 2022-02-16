/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.ATR.UIAutomation.ScreenCapture.Interface
{
    public interface IMyScreen
    {
         void SubscribeScreenCapture(string folderPath);
         void UnSubscribeScreenCapture();

         event EventHandler<ScreenEventArgs> StartCapture;
         event EventHandler<ScreenEventArgs> EndCapture;
         

         string CapturedImagePath
         {
             get;
             set;
         }

         string ImageName
         {
             get;
             set;
         }

    }
}
