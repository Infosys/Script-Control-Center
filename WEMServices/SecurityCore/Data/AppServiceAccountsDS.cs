/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
//using Microsoft.WindowsAzure.ServiceRuntime;
using Infosys.WEM.Infrastructure.SecurityCore.Data.Entity;
//using Infosys.WEM.Resource.IDataAccess.SQLServer.DE;

namespace Infosys.WEM.Infrastructure.SecurityCore.Data
{
    public partial class AppServiceAccountsDS : DALBase, IAppServiceAccountsDS
    {
        public AppServiceAccountsDS()
        {
            this.DatabaseName = ConfigurationParameters.IAPWEMCORE;
            //SetSQLRetryPolicy(RoleEnvironment.GetConfigurationSettingValue(ConfigurationParameters.PPTWARECORE_SQLAZURERETRY));
            

        }

        public AppServiceAccountsDS(string databaseName, System.Int32 retryCount, System.Int64 retrySpanSeconds)
            : base(databaseName)
        {
            SetSQLRetryPolicy(retryCount, retrySpanSeconds);

        }


        public AppServiceAccounts GetOne(System.String partitionKey, System.String rowKey)
        {
            AppServiceAccounts appServiceAccounts = null;
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            DbCommand command = CommandGetOne(partitionKey, rowKey, database);
            IDataReader dataReader = null;
            int noOfTimesRetryHappened = 0;
            try
            {
                RetrySQLCommand.RetryN(
                () =>
                {
                    dataReader = database.ExecuteReader(command);
                },
                this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);
                if (dataReader != null)
                {
                    using (SqlDataReader sqlDataReader = dataReader as SqlDataReader)
                    {
                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                appServiceAccounts = ConstructAppServiceAccounts(sqlDataReader);
                            }
                        }
                    }
                }

            }
            finally
            {
                if (dataReader != null)
                    dataReader.Dispose();
                command.Dispose();

            }

            return appServiceAccounts;

        }

        public List<AppServiceAccounts> GetAll()
        {
            List<AppServiceAccounts> list = new List<AppServiceAccounts>();
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            DbCommand command = CommandGetAll(database);
            IDataReader dataReader = null;
            int noOfTimesRetryHappened = 0;
            try
            {
                RetrySQLCommand.RetryN(
                () =>
                {
                    dataReader = database.ExecuteReader(command);
                },
                this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);

                if (dataReader!=null)
                {
                    using (SqlDataReader sqlDataReader = dataReader as SqlDataReader)
                    {
                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                AppServiceAccounts appServiceAccounts = ConstructAppServiceAccounts(sqlDataReader);
                                list.Add(appServiceAccounts);
                            }
                        }
                    }
                }

            }
            finally
            {
                if (dataReader!=null)
                    dataReader.Dispose();
                command.Dispose();

            }

            return list;

        }

        public AppServiceAccounts Insert(AppServiceAccounts appServiceAccounts)
        {
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            DbCommand command = CommandInsert(appServiceAccounts, database);
            int noOfTimesRetryHappened = 0;
            try
            {
                RetrySQLCommand.RetryN(
                    () =>
                    {
                        if (database.ExecuteNonQuery(command) > 0)
                        {
                            // Read Output Parameters here
                        }
                    },
                    this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);

            }
            finally
            {
                command.Dispose();
            }
            return appServiceAccounts;
        }
        public List<AppServiceAccounts> BatchInsert(List<AppServiceAccounts> list)
        {
            List<AppServiceAccounts> returnList = new List<AppServiceAccounts>();
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            int noOfTimesRetryHappened = 0;
            DbTransaction transaction = null; RetrySQLCommand.RetryN(
             () =>
             {
                 DbConnection connection = database.CreateConnection();
                 try
                 {
                     connection.Open();
                     transaction = connection.BeginTransaction();
                     try
                     {
                         foreach (AppServiceAccounts appServiceAccounts in list)
                         {
                             DbCommand command = CommandInsert(appServiceAccounts, database);
                             if (database.ExecuteNonQuery(command) > 0)
                             {
                                 // Read Output Parameters here
                                 returnList.Add(appServiceAccounts);

                             }
                         }
                         transaction.Commit();
                     }
                     catch
                     {
                         transaction.Rollback();

                     }
                 }
                 finally
                 {
                     transaction.Dispose();
                     connection.Close();
                     connection.Dispose();

                 }
             },
             this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);
            return returnList;
        }

        public AppServiceAccounts Update(AppServiceAccounts appServiceAccounts)
        {
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            DbCommand command = CommandUpdate(appServiceAccounts, database);
            int noOfTimesRetryHappened = 0;
            try
            {
                RetrySQLCommand.RetryN(
                    () =>
                    {
                        if (database.ExecuteNonQuery(command) > 0)
                        {
                            // Read Output Parameters here
                        }
                        else
                        {
                            Exception concurrencyEx = new Exception("Update failed due to concurrency issue. Row was modified by a different user. Please refresh your data.");
                            throw concurrencyEx;
                        }
                    },
                    this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);

            }
            finally
            {
                command.Dispose();
            }
            return appServiceAccounts;
        }
        public List<AppServiceAccounts> BatchUpdate(List<AppServiceAccounts> list)
        {
            List<AppServiceAccounts> returnList = new List<AppServiceAccounts>();
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            int noOfTimesRetryHappened = 0;
            DbTransaction transaction = null; RetrySQLCommand.RetryN(
             () =>
             {
                 DbConnection connection = database.CreateConnection();
                 try
                 {
                     connection.Open();
                     transaction = connection.BeginTransaction();
                     try
                     {
                         foreach (AppServiceAccounts appServiceAccounts in list)
                         {
                             DbCommand command = CommandUpdate(appServiceAccounts, database);
                             if (database.ExecuteNonQuery(command) > 0)
                             {
                                 // Read Output Parameters here
                                 returnList.Add(appServiceAccounts);

                             }
                         }
                         transaction.Commit();
                     }
                     catch
                     {
                         transaction.Rollback();

                     }
                 }
                 finally
                 {
                     transaction.Dispose();
                     connection.Close();
                     connection.Dispose();

                 }
             },
             this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);
            return returnList;
        }

        public bool Delete(System.String partitionKey, System.String rowKey)
        {
            bool status = false;
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            DbCommand command = CommandDelete(partitionKey, rowKey, database);
            int noOfTimesRetryHappened = 0;
            try
            {
                RetrySQLCommand.RetryN(
                    () =>
                    {
                        if (database.ExecuteNonQuery(command) > 0)
                        {
                            status = true;
                        }
                    },
                    this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);
            }
            finally
            {
                command.Dispose();
            }
            return status;
        }
        public bool BatchDelete(List<AppServiceAccounts> list)
        {
            bool status = false; int noOfTimesRetryHappened = 0;
            DbTransaction transaction = null;
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            RetrySQLCommand.RetryN(
            () =>
            {
                DbConnection connection = database.CreateConnection();
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (AppServiceAccounts appServiceAccounts in list)
                        {
                            DbCommand command = CommandDelete(appServiceAccounts.PartitionKey, appServiceAccounts.RowKey, database);
                            if (database.ExecuteNonQuery(command) > 0)
                            {
                                status = true;
                            }

                        }
                        transaction.Commit();
                        status = true;

                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;

                    }
                }
                finally
                {
                    transaction.Dispose();
                    connection.Close();
                    connection.Dispose();

                }
            },
            this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);
            return status;
        }

        public List<AppServiceAccounts> GetMany(System.String partitionKey)
        {
            List<AppServiceAccounts> list = new List<AppServiceAccounts>();
            Database database = DatabaseFactory.CreateDatabase(DatabaseName);
            DbCommand command = CommandGetMany(partitionKey, database);
            IDataReader dataReader = null;
            int noOfTimesRetryHappened = 0;
            try
            {
                RetrySQLCommand.RetryN(
                () =>
                {
                    dataReader = database.ExecuteReader(command);
                },
                this.SQLAzureRetryCount, this.SQLAzureRetrySpan, ref noOfTimesRetryHappened);

                if (dataReader != null)
                {
                    using (SqlDataReader sqlDataReader = dataReader as SqlDataReader)
                    {
                        if (sqlDataReader.HasRows)
                        {
                            while (sqlDataReader.Read())
                            {
                                AppServiceAccounts appServiceAccounts = ConstructAppServiceAccounts(sqlDataReader);
                                list.Add(appServiceAccounts);
                            }
                        }
                    }
                }

            }
            finally
            {
                if (dataReader != null)
                    dataReader.Dispose();
                command.Dispose();

            }

            return list;

        }


        private DbCommand CommandGetOne(System.String partitionKey, System.String rowKey, Database database)
        {
            DbCommand command = database.GetStoredProcCommand(AppServiceAccountsDSConstants.GetOne_SP);
            if (partitionKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, partitionKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, System.DBNull.Value);
            }
            if (rowKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, rowKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, System.DBNull.Value);
            }

            return command;

        }


        private DbCommand CommandGetAll(Database database)
        {
            DbCommand command = database.GetStoredProcCommand(AppServiceAccountsDSConstants.GetAll_SP);

            return command;

        }


        private DbCommand CommandInsert(AppServiceAccounts appServiceAccounts, Database database)
        {
            DbCommand command = database.GetStoredProcCommand(AppServiceAccountsDSConstants.Insert_SP);
            database.AddInParameter(command, AppServiceAccountsDSConstants.cipherKeyIndex_Param, DbType.Int32, appServiceAccounts.CipherKeyIndex);
            database.AddInParameter(command, AppServiceAccountsDSConstants.companyId_Param, DbType.Guid, appServiceAccounts.CompanyId);
            database.AddInParameter(command, AppServiceAccountsDSConstants.createdOn_Param, DbType.DateTime, appServiceAccounts.CreatedOn);
            if (appServiceAccounts.Description != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.description_Param, DbType.String, appServiceAccounts.Description);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.description_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.Domain != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.domain_Param, DbType.String, appServiceAccounts.Domain);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.domain_Param, DbType.String, System.DBNull.Value);
            }
            database.AddInParameter(command, AppServiceAccountsDSConstants.isActive_Param, DbType.Boolean, appServiceAccounts.IsActive);
            database.AddInParameter(command, AppServiceAccountsDSConstants.lastModifiedOn_Param, DbType.DateTime, appServiceAccounts.LastModifiedOn);
            if (appServiceAccounts.PartitionKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, appServiceAccounts.PartitionKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.RowKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, appServiceAccounts.RowKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.ServiceId != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.serviceId_Param, DbType.String, appServiceAccounts.ServiceId);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.serviceId_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.ServicePassword != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.servicePassword_Param, DbType.String, appServiceAccounts.ServicePassword);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.servicePassword_Param, DbType.String, System.DBNull.Value);
            }

            return command;

        }


        private DbCommand CommandUpdate(AppServiceAccounts appServiceAccounts, Database database)
        {
            DbCommand command = database.GetStoredProcCommand(AppServiceAccountsDSConstants.Update_SP);
            database.AddInParameter(command, AppServiceAccountsDSConstants.cipherKeyIndex_Param, DbType.Int32, appServiceAccounts.CipherKeyIndex);
            database.AddInParameter(command, AppServiceAccountsDSConstants.companyId_Param, DbType.Guid, appServiceAccounts.CompanyId);
            database.AddInParameter(command, AppServiceAccountsDSConstants.createdOn_Param, DbType.DateTime, appServiceAccounts.CreatedOn);
            if (appServiceAccounts.Description != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.description_Param, DbType.String, appServiceAccounts.Description);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.description_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.Domain != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.domain_Param, DbType.String, appServiceAccounts.Domain);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.domain_Param, DbType.String, System.DBNull.Value);
            }
            database.AddInParameter(command, AppServiceAccountsDSConstants.isActive_Param, DbType.Boolean, appServiceAccounts.IsActive);
            database.AddInParameter(command, AppServiceAccountsDSConstants.lastModifiedOn_Param, DbType.DateTime, appServiceAccounts.LastModifiedOn);
            if (appServiceAccounts.PartitionKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, appServiceAccounts.PartitionKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.RowKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, appServiceAccounts.RowKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.ServiceId != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.serviceId_Param, DbType.String, appServiceAccounts.ServiceId);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.serviceId_Param, DbType.String, System.DBNull.Value);
            }
            if (appServiceAccounts.ServicePassword != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.servicePassword_Param, DbType.String, appServiceAccounts.ServicePassword);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.servicePassword_Param, DbType.String, System.DBNull.Value);
            }

            return command;

        }


        private DbCommand CommandDelete(System.String partitionKey, System.String rowKey, Database database)
        {
            DbCommand command = database.GetStoredProcCommand(AppServiceAccountsDSConstants.Delete_SP);
            if (partitionKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, partitionKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, System.DBNull.Value);
            }
            if (rowKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, rowKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.rowKey_Param, DbType.String, System.DBNull.Value);
            }

            return command;

        }


        private DbCommand CommandGetMany(System.String partitionKey, Database database)
        {
            DbCommand command = database.GetStoredProcCommand(AppServiceAccountsDSConstants.GetMany_SP);
            if (partitionKey != null)
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, partitionKey);
            }
            else
            {
                database.AddInParameter(command, AppServiceAccountsDSConstants.partitionKey_Param, DbType.String, System.DBNull.Value);
            }

            return command;

        }


        private AppServiceAccounts ConstructAppServiceAccounts(IDataReader reader)
        {
            AppServiceAccounts appServiceAccounts = new AppServiceAccounts();
            int index = 0;

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.CipherKeyIndex_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.CipherKeyIndex = reader.GetInt32(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.CompanyId_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.CompanyId = reader.GetGuid(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.CreatedOn_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.CreatedOn = reader.GetDateTime(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.Description_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.Description = reader.GetString(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.Domain_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.Domain = reader.GetString(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.IsActive_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.IsActive = reader.GetBoolean(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.LastModifiedOn_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.LastModifiedOn = reader.GetDateTime(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.PartitionKey_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.PartitionKey = reader.GetString(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.RowKey_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.RowKey = reader.GetString(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.ServiceId_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.ServiceId = reader.GetString(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.ServicePassword_Ret);
            if (!reader.IsDBNull(index))
            {
                appServiceAccounts.ServicePassword = reader.GetString(index);
            }

            index = reader.GetOrdinal(AppServiceAccountsDSConstants.Timestamp_Ret);
            //if (!reader.IsDBNull(index))
            //{
            //    appServiceAccounts.Timestamp = reader.GetDateTime(index);
            //}

            return appServiceAccounts;
        }

        internal partial class AppServiceAccountsDSConstants
        {
            public const string GetOne_SP = "dbo.GetAppServiceAccountsByPartitionkeyAndRowkey";
            public const string partitionKey_Param = "partitionKey";
            public const string rowKey_Param = "rowKey";
            public const string CipherKeyIndex_Ret = "CipherKeyIndex";
            public const string CompanyId_Ret = "CompanyId";
            public const string CreatedOn_Ret = "CreatedOn";
            public const string Description_Ret = "Description";
            public const string Domain_Ret = "Domain";
            public const string IsActive_Ret = "IsActive";
            public const string LastModifiedOn_Ret = "LastModifiedOn";
            public const string PartitionKey_Ret = "PartitionKey";
            public const string RowKey_Ret = "RowKey";
            public const string ServiceId_Ret = "ServiceId";
            public const string ServicePassword_Ret = "ServicePassword";
            public const string Timestamp_Ret = "Timestamp";
            public const string GetAll_SP = "dbo.GetAllFromAppServiceAccounts";
            public const string Insert_SP = "dbo.InsertAppServiceAccounts";
            public const string cipherKeyIndex_Param = "cipherKeyIndex";
            public const string companyId_Param = "companyId";
            public const string createdOn_Param = "createdOn";
            public const string description_Param = "description";
            public const string domain_Param = "domain";
            public const string isActive_Param = "isActive";
            public const string lastModifiedOn_Param = "lastModifiedOn";
            public const string serviceId_Param = "serviceId";
            public const string servicePassword_Param = "servicePassword";
            public const string Update_SP = "dbo.UpdateAppServiceAccounts";
            public const string Delete_SP = "dbo.DeleteAppServiceAccounts";
            public const string GetMany_SP = "dbo.GetAppServiceAccountsByPartitionKey";

        }
    }
}
