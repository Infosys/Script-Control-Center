/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

namespace Infosys.ATR.WFDesigner.Services
{
    public static class WCFExtensions
    {
        public static void Using<T>(this T client, Action<T> work)
            where T : ICommunicationObject
        {
            try
            {
                work(client);                
                client.Close();
            }
            catch (CommunicationException e)
            {
                client.Abort();
            }
            catch (TimeoutException e)
            {
                client.Abort();
            }
            catch (Exception e)
            {
                client.Abort();
                throw;
            }
        }
    }
}
