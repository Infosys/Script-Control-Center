/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.WEM.Infrastructure.SecurityCore.Data
{
    /// <summary>
    /// This class is responsible for executing the RetryLogic on SQLAzure. This class is used across DataAccessLayer
    /// </summary>
    static public class RetrySQLCommand
    {

        // StorageClient API provides the class 'RetryPolicies' to enable retries in azure storage operations. 
        // and we will protect 'SqlAzure' operations using it, as StorageClient API does not 
        //provide one for 'SqlAzure'.
        /// <summary>
        /// Sets the retry parameters
        /// </summary>
        /// <param name="mainAction">Main Action to be executed under retry loop</param>            
        /// <param name="maxRetries">Maximum number of retries count</param>
        /// <param name="intervalBetweenRetries">Interval between subsequent retry, in seconds</param>
        /// <param name="retryCounter">Retry Counter indicates at which attempt the operation was sucessful</param>
        public static void RetryN(Action mainAction, int maxRetries, TimeSpan intervalBetweenRetries,
            ref int retryCounter)
        {
            do
            {
                try
                {
                    // Execute the main action.
                    mainAction();
                    break;
                }
                catch (Exception)
                {
                    if (maxRetries == 0)
                    {
                        throw;
                    }
                    retryCounter++;
                    if (intervalBetweenRetries > TimeSpan.Zero)
                    {
                        System.Threading.Thread.Sleep(intervalBetweenRetries);
                    }
                }
            }
            while (maxRetries-- > 0);
        }


        // StorageClient API provides the class 'RetryPolicies' to enable retries in azure storage operations. 
        // and we will protect 'SqlAzure' operations using it, as StorageClient API does not 
        //provide one for 'SqlAzure'.
        /// <summary>
        /// Sets the retry parameters
        /// </summary>
        /// <param name="mainAction">Main Action to be executed under retry loop</param>
        /// <param name="exceptionAction">Action to be executed in case of exceptios in Main Action</param>
        /// <param name="maxRetries">Maximum number of retries count</param>
        /// <param name="intervalBetweenRetries">Interval between subsequent retry, in seconds</param>
        /// <param name="retryCounter">Retry Counter indicates at which attempt the operation was sucessful</param>
        public static void RetryN(Action mainAction, Action exceptionAction, int maxRetries, TimeSpan intervalBetweenRetries,
            ref int retryCounter)
        {
            do
            {
                try
                {
                    // Execute the main action.
                    mainAction();
                    break;
                }
                catch (Exception)
                {
                    if (maxRetries == 0)
                    {
                        throw;
                    }
                    retryCounter++;
                    if (intervalBetweenRetries > TimeSpan.Zero)
                    {
                        System.Threading.Thread.Sleep(intervalBetweenRetries);
                    }
                    // If an action to execute on occurance of exception had been provided, then execute it.
                    if (exceptionAction != null)
                    {
                        exceptionAction();
                    }

                }
            }
            while (maxRetries-- > 0);
        }
    }
}
