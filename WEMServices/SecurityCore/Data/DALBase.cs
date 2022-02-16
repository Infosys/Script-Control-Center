/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.WEM.Infrastructure.SecurityCore.Data
{
    public abstract partial class DALBase
    {
        public string DatabaseName
        {
            get;
            set;
        }

        public DALBase(string databaseName)
        {
            this.DatabaseName = databaseName;
        }

        public DALBase()
        {
            this.DatabaseName = string.Empty;
        }

        public void HandleException(System.Data.SqlClient.SqlException ex)
        {
            //Apply DAL specific exception handling policy here.
            //TODO: Add exception handling logic here.
        }

        public void HandleException(System.Data.SqlClient.SqlException ex, out Exception returnException)
        {
            //Apply DAL specific exception handling policy here.
            returnException = new Exception(ex.Message, ex);
        }

        public int SQLAzureRetryCount
        {
            get;
            set;
        }

        public TimeSpan SQLAzureRetrySpan
        {
            get;
            set;
        }

        protected void SetSQLRetryPolicy(string retryConfigurationValue)
        {
            int retrySpanSeconds = 0;
            int retryCount = 0;

            string[] retryValues = retryConfigurationValue.Split(',');

            if (retryValues.Length == 2)
            {
                string[] countValues = retryValues[0].Split('=');
                if (countValues[0].ToLower().Equals("count"))
                {
                    int.TryParse(countValues[1], out retryCount);
                }
                string[] spanValues = retryValues[1].Split('=');
                if (spanValues[0].ToLower().Equals("span"))
                {
                    int.TryParse(spanValues[1], out retrySpanSeconds);
                }
            }
            SQLAzureRetryCount = retryCount;
            SQLAzureRetrySpan = TimeSpan.FromSeconds(retrySpanSeconds);
        }

        protected void SetSQLRetryPolicy(int retryCount, long retrySpanSeconds)
        {
            SQLAzureRetryCount = retryCount;
            SQLAzureRetrySpan = TimeSpan.FromSeconds(retrySpanSeconds);
        }
    }
}
