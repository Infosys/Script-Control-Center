
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/29/2016 18:30:21
-- Generated from EDMX file: D:\TFS_Build\ATR-FEA\Code\WorkflowExecutionManager\WEM.DataEntity\WorkflowExecutionStore.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [IAP_WorkflowExecutionStore];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Account]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Account];
GO
IF OBJECT_ID(N'[dbo].[ActiveRegisteredNodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActiveRegisteredNodes];
GO
IF OBJECT_ID(N'[dbo].[Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Category];
GO
IF OBJECT_ID(N'[dbo].[CategoryWorkflowMap]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CategoryWorkflowMap];
GO
IF OBJECT_ID(N'[dbo].[Companies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Companies];
GO
IF OBJECT_ID(N'[dbo].[Group]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Group];
GO
IF OBJECT_ID(N'[dbo].[GroupAccess]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupAccess];
GO
IF OBJECT_ID(N'[dbo].[Module]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Module];
GO
IF OBJECT_ID(N'[dbo].[ObjectModelMaster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ObjectModelMaster];
GO
IF OBJECT_ID(N'[dbo].[Organization]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Organization];
GO
IF OBJECT_ID(N'[dbo].[RegisterredNodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegisterredNodes];
GO
IF OBJECT_ID(N'[dbo].[Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Role];
GO
IF OBJECT_ID(N'[dbo].[ScheduledRequest]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ScheduledRequest];
GO
IF OBJECT_ID(N'[dbo].[ScheduledRequestActivities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ScheduledRequestActivities];
GO
IF OBJECT_ID(N'[dbo].[Script]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Script];
GO
IF OBJECT_ID(N'[dbo].[ScriptInstance]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ScriptInstance];
GO
IF OBJECT_ID(N'[dbo].[ScriptParams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ScriptParams];
GO
IF OBJECT_ID(N'[dbo].[SemanticCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SemanticCategory];
GO
IF OBJECT_ID(N'[dbo].[SemanticCluster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SemanticCluster];
GO
IF OBJECT_ID(N'[dbo].[SemanticNodeCluster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SemanticNodeCluster];
GO
IF OBJECT_ID(N'[dbo].[SuperAdmin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SuperAdmin];
GO
IF OBJECT_ID(N'[dbo].[Track]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Track];
GO
IF OBJECT_ID(N'[dbo].[TransactionInstance]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TransactionInstance];
GO
IF OBJECT_ID(N'[dbo].[Type]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Type];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO
IF OBJECT_ID(N'[dbo].[WorkflowCategoryMaster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkflowCategoryMaster];
GO
IF OBJECT_ID(N'[dbo].[WorkflowMaster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkflowMaster];
GO
IF OBJECT_ID(N'[dbo].[WorkflowParams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkflowParams];
GO
IF OBJECT_ID(N'[dbo].[WorkflowTrackingDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkflowTrackingDetails];
GO
IF OBJECT_ID(N'[dbo].[WorkflowTrackingMaster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkflowTrackingMaster];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Account'
CREATE TABLE [dbo].[Account] (
    [PartitonKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [AccountId] int  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [Description] nvarchar(100)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'CategoryWorkflowMap'
CREATE TABLE [dbo].[CategoryWorkflowMap] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(50)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CategoryId] int  NOT NULL,
    [WorkflowId] uniqueidentifier  NOT NULL,
    [ActiveWorkflowVersion] int  NOT NULL,
    [WorkflowName] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'ObjectModelMaster'
CREATE TABLE [dbo].[ObjectModelMaster] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CategoryId] int  NOT NULL,
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [ObjectModelVer] int  NOT NULL,
    [ObjectModelUri] nvarchar(max)  NOT NULL,
    [PublishedOn] datetime  NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [SourceIPAddress] nvarchar(50)  NULL,
    [Client] nvarchar(100)  NULL,
    [ClientVersion] nvarchar(10)  NULL,
    [SourceMachineName] nvarchar(100)  NULL,
    [FileSizeKB] int  NULL,
    [IsActive] bit  NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [LastModifiedOn] datetime  NULL
);
GO

-- Creating table 'Organization'
CREATE TABLE [dbo].[Organization] (
    [PartitionKey] nvarchar(10)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [OrgId] int  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [Description] nvarchar(100)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'ScriptInstance'
CREATE TABLE [dbo].[ScriptInstance] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [InstanceId] nvarchar(50)  NOT NULL,
    [RunAsUser] nvarchar(50)  NOT NULL,
    [ArgString] varchar(max)  NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NOT NULL,
    [VerboseLog] varchar(max)  NULL,
    [ExecutionStatus] nvarchar(50)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL
);
GO

-- Creating table 'Track'
CREATE TABLE [dbo].[Track] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [TrackId] int  NOT NULL,
    [Description] nvarchar(100)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'WorkflowCategoryMaster'
CREATE TABLE [dbo].[WorkflowCategoryMaster] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Id] int  NOT NULL,
    [CompanyId] int  NOT NULL,
    [Name] nvarchar(50)  NULL,
    [Description] varchar(max)  NULL,
    [Type] int  NOT NULL,
    [IsActive] bit  NULL,
    [CreatedOn] datetime  NULL,
    [LastModifiedOn] datetime  NULL,
    [ParentId] int  NULL
);
GO

-- Creating table 'WorkflowTrackingDetails'
CREATE TABLE [dbo].[WorkflowTrackingDetails] (
    [PartitionKey] nvarchar(50)  NOT NULL,
    [RowKey] nvarchar(10)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [WorkflowInstanceId] uniqueidentifier  NULL,
    [ActivityStepCount] decimal(8,4)  NULL,
    [ActivityId] nvarchar(max)  NULL,
    [ActivityDisplayName] nvarchar(max)  NULL,
    [ActivityParameters] nvarchar(max)  NULL,
    [ActivityState] nvarchar(50)  NULL,
    [StartedOn] datetime  NULL,
    [EndedOn] datetime  NULL,
    [StatusDescription] nvarchar(max)  NULL,
    [LastModifiedOn] datetime  NULL
);
GO

-- Creating table 'WorkflowTrackingMaster'
CREATE TABLE [dbo].[WorkflowTrackingMaster] (
    [PartitionKey] nvarchar(50)  NOT NULL,
    [RowKey] nvarchar(50)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [WorkflowId] uniqueidentifier  NOT NULL,
    [WorkflowVer] int  NOT NULL,
    [InstanceId] uniqueidentifier  NOT NULL,
    [State] nvarchar(20)  NULL,
    [StatusDescription] nvarchar(max)  NULL,
    [RequestorId] nvarchar(50)  NOT NULL,
    [RequestorSourceIP] nvarchar(50)  NULL,
    [StartedOn] datetime  NULL,
    [RunAtIPAddress] nvarchar(50)  NULL,
    [RunAtMachineName] nvarchar(100)  NULL,
    [CompletedOn] datetime  NULL,
    [LastModifiedOn] datetime  NULL
);
GO

-- Creating table 'GroupAccess'
CREATE TABLE [dbo].[GroupAccess] (
    [PartitionKey] nchar(10)  NOT NULL,
    [RowKey] nchar(10)  NOT NULL,
    [GroupId] int  NOT NULL,
    [ParentId] int  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [LastModifiedOn] datetime  NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [IsActive] bit  NOT NULL
);
GO

-- Creating table 'Type'
CREATE TABLE [dbo].[Type] (
    [PartitionKey] nchar(5)  NOT NULL,
    [RowKey] nchar(10)  NOT NULL,
    [Id] int  NOT NULL,
    [Name] nchar(50)  NOT NULL,
    [Description] nvarchar(100)  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [LastModifiedBy] int  NULL,
    [LastModifiedOn] datetime  NULL,
    [IsActive] bit  NULL,
    [Timestamp] datetime  NOT NULL
);
GO

-- Creating table 'Role'
CREATE TABLE [dbo].[Role] (
    [PartitionKey] nvarchar(50)  NOT NULL,
    [RowKey] nchar(10)  NOT NULL,
    [Id] int  NOT NULL,
    [Name] nchar(25)  NOT NULL,
    [Description] nvarchar(250)  NULL,
    [CreatedOn] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [LastModified] datetime  NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [Timestamp] datetime  NOT NULL,
    [IsActive] bit  NULL,
    [CompanyId] int  NOT NULL
);
GO

-- Creating table 'SuperAdmin'
CREATE TABLE [dbo].[SuperAdmin] (
    [PartitionKey] nchar(10)  NOT NULL,
    [RowKey] nvarchar(105)  NOT NULL,
    [Id] int  NOT NULL,
    [Alias] nvarchar(105)  NOT NULL,
    [CompanyId] int  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CreatedBy] nvarchar(105)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(105)  NULL,
    [ModifiedOn] datetime  NULL
);
GO

-- Creating table 'Group'
CREATE TABLE [dbo].[Group] (
    [PartitionKey] nchar(5)  NOT NULL,
    [RowKey] nchar(10)  NOT NULL,
    [Timestamp] datetime  NULL,
    [Id] int  NOT NULL,
    [Name] nchar(50)  NOT NULL,
    [Description] nchar(100)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [LastModifiedOn] datetime  NULL,
    [IsActive] bit  NULL,
    [CompanyId] int  NOT NULL,
    [ParentId] int  NULL
);
GO

-- Creating table 'User'
CREATE TABLE [dbo].[User] (
    [PartitionKey] nvarchar(105)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CompanyId] int  NOT NULL,
    [Alias] nvarchar(100)  NOT NULL,
    [Role] int  NOT NULL,
    [CategoryId] int  NULL,
    [CreatedOn] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [LastModifiedOn] datetime  NULL,
    [IsActive] bit  NULL,
    [Id] int  NOT NULL,
    [DisplayName] nvarchar(100)  NULL,
    [IsDL] bit  NULL,
    [GroupId] int  NULL,
    [IsActiveGroup] bit  NULL
);
GO

-- Creating table 'RegisterredNodes'
CREATE TABLE [dbo].[RegisterredNodes] (
    [PartitionKey] nvarchar(50)  NOT NULL,
    [RowKey] nvarchar(50)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [HttpPort] int  NULL,
    [TcpPort] int  NULL,
    [RegisteredOn] datetime  NOT NULL,
    [UnRegisteredOn] datetime  NULL,
    [MachineName] nvarchar(20)  NOT NULL,
    [DomainName] nvarchar(30)  NOT NULL,
    [State] nvarchar(10)  NOT NULL,
    [OSVersion] nvarchar(200)  NULL,
    [Is64Bit] bit  NOT NULL,
    [WorkflowServiceVersion] nvarchar(20)  NULL,
    [DotNetVersion] nvarchar(50)  NULL,
    [ExecutionEngineSupported] int  NOT NULL,
    [CreatedOn] datetime  NULL,
    [LastModifiedOn] datetime  NULL,
    [CompanyId] int  NULL
);
GO

-- Creating table 'ActiveRegisteredNodes'
CREATE TABLE [dbo].[ActiveRegisteredNodes] (
    [PartitionKey] nvarchar(50)  NOT NULL,
    [RowKey] nvarchar(50)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [MachineName] nvarchar(50)  NOT NULL,
    [DomainName] nvarchar(50)  NOT NULL,
    [CompanyId] int  NULL
);
GO

-- Creating table 'WorkflowParams'
CREATE TABLE [dbo].[WorkflowParams] (
    [PartitionKey] nvarchar(50)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [DefaultValue] varchar(max)  NULL,
    [IsMandatory] bit  NOT NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL,
    [AllowedValues] varchar(max)  NULL,
    [IsSecret] bit  NOT NULL,
    [ParamType] nvarchar(10)  NOT NULL,
    [ParamId] int  NOT NULL,
    [IsReferenceKey] bit  NOT NULL
);
GO

-- Creating table 'Module'
CREATE TABLE [dbo].[Module] (
    [ModuleID] int  NOT NULL,
    [ModuleName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Category'
CREATE TABLE [dbo].[Category] (
    [PartitionKey] nvarchar(10)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CategoryId] int  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Description] varchar(max)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL,
    [ParentId] int  NULL,
    [CompanyId] int  NULL,
    [ModuleID] int  NULL
);
GO

-- Creating table 'SemanticCluster'
CREATE TABLE [dbo].[SemanticCluster] (
    [PartitionKey] nvarchar(20)  NOT NULL,
    [RowKey] nvarchar(20)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [LastModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL,
    [ClusterName] nvarchar(20)  NOT NULL,
    [Id] nvarchar(15)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Priority] int  NULL,
    [CompanyId] int  NULL
);
GO

-- Creating table 'Companies'
CREATE TABLE [dbo].[Companies] (
    [PartitionKey] nvarchar(10)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CompanyId] int  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [CompanyURL] nvarchar(max)  NULL,
    [LogoURL] nvarchar(max)  NULL,
    [NormalizedName] nvarchar(50)  NOT NULL,
    [StorageBaseUrl] nvarchar(max)  NOT NULL,
    [DeploymentBaseUrl] nvarchar(max)  NULL,
    [Region] nvarchar(15)  NULL,
    [CreatedOn] datetime  NOT NULL,
    [LastModifiedOn] datetime  NULL,
    [RemoteShareUrl] nvarchar(max)  NULL,
    [EnableSecureTransactions] bit  NOT NULL
);
GO

-- Creating table 'ScheduledRequest'
CREATE TABLE [dbo].[ScheduledRequest] (
    [PartitionKey] nvarchar(20)  NOT NULL,
    [RowKey] nvarchar(50)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [Id] nvarchar(50)  NOT NULL,
    [MachineName] nvarchar(20)  NOT NULL,
    [CategoryId] int  NOT NULL,
    [RequestVersion] int  NULL,
    [InputParameters] nvarchar(max)  NULL,
    [OutputParameters] nvarchar(max)  NULL,
    [ExecuteOn] datetime  NULL,
    [Message] nvarchar(max)  NULL,
    [State] int  NULL,
    [RequestType] int  NOT NULL,
    [RequestId] nvarchar(50)  NOT NULL,
    [Priority] int  NOT NULL,
    [AssignedTo] nvarchar(20)  NULL,
    [Executor] nvarchar(20)  NULL,
    [ParentId] nvarchar(50)  NULL,
    [StopType] int  NULL,
    [IterationSetRoot] nvarchar(50)  NULL,
    [IsIterationSetRoot] bit  NULL,
    [CompanyId] int  NULL
);
GO

-- Creating table 'ScheduledRequestActivities'
CREATE TABLE [dbo].[ScheduledRequestActivities] (
    [PartitionKey] nvarchar(100)  NOT NULL,
    [RowKey] nvarchar(100)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Status] int  NOT NULL,
    [ParentScheduledRequestId] nvarchar(50)  NULL,
    [ScheduledRequestId] nvarchar(50)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [IterationSetRoot] nvarchar(50)  NULL,
    [CompanyId] int  NULL
);
GO

-- Creating table 'SemanticCategory'
CREATE TABLE [dbo].[SemanticCategory] (
    [PartitionKey] nchar(10)  NOT NULL,
    [RowKey] nchar(20)  NOT NULL,
    [CategoryId] int  NOT NULL,
    [SemanticClusterId] nvarchar(15)  NOT NULL,
    [SemanticClusterName] nvarchar(20)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] nvarchar(50)  NOT NULL,
    [LastModifiedBy] nvarchar(50)  NULL,
    [LastModifiedOn] datetime  NULL
);
GO

-- Creating table 'ScriptParams'
CREATE TABLE [dbo].[ScriptParams] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(5)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [ParamId] int  NOT NULL,
    [Name] nvarchar(200)  NULL,
    [DefaultValue] varchar(max)  NULL,
    [AllowedValues] varchar(max)  NULL,
    [IsMandatory] bit  NOT NULL,
    [ParamType] nvarchar(50)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL,
    [IsSecret] bit  NOT NULL,
    [IsUnnamed] bit  NOT NULL,
    [DataType] nvarchar(6)  NULL,
    [IsReferenceKey] bit  NOT NULL
);
GO

-- Creating table 'SemanticNodeCluster'
CREATE TABLE [dbo].[SemanticNodeCluster] (
    [PartitionKey] nvarchar(20)  NOT NULL,
    [RowKey] nvarchar(100)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [LastModifiedOn] datetime  NULL,
    [IsDeleted] bit  NOT NULL,
    [ClusterId] nvarchar(20)  NOT NULL,
    [IapNodeId] nvarchar(20)  NOT NULL
);
GO

-- Creating table 'TransactionInstance'
CREATE TABLE [dbo].[TransactionInstance] (
    [PartitionKey] nvarchar(100)  NOT NULL,
    [RowKey] nvarchar(100)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CategoryId] int  NOT NULL,
    [WorkflowId] nvarchar(100)  NULL,
    [ScriptId] nvarchar(100)  NULL,
    [InstanceId] nvarchar(100)  NOT NULL,
    [CurrentState] int  NOT NULL,
    [Executor] nvarchar(50)  NOT NULL,
    [MachineName] nvarchar(50)  NOT NULL,
    [InitiatedBy] nvarchar(100)  NOT NULL,
    [TriggeredBy] nvarchar(100)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [IPAddress] nvarchar(50)  NULL,
    [OSDetails] nvarchar(max)  NULL,
    [ReferenceKey] nvarchar(100)  NULL,
    [WorkflowVersion] nvarchar(10)  NULL,
    [LastModifiedOn] datetime  NULL,
    [CreatedOn] datetime  NOT NULL,
    [FileType] nvarchar(10)  NOT NULL,
    [ScriptVersion] nvarchar(10)  NULL,
    [CategoryName] nvarchar(100)  NULL,
    [WorkflowPersistedStateId] nvarchar(100)  NULL,
    [ModuleName] nvarchar(100)  NULL,
    [TransactionMetadata] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [BookMark] nvarchar(100)  NULL
);
GO

-- Creating table 'WorkflowMaster'
CREATE TABLE [dbo].[WorkflowMaster] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(50)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CategoryId] int  NOT NULL,
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [WorkflowVer] int  NOT NULL,
    [WorkflowURI] nvarchar(max)  NOT NULL,
    [ImageURI] nvarchar(max)  NULL,
    [PublishedOn] datetime  NULL,
    [CreatedBy] nvarchar(100)  NULL,
    [SourceIPAddress] nvarchar(50)  NULL,
    [Client] nvarchar(100)  NULL,
    [ClientVersion] nvarchar(10)  NULL,
    [SourceMachineName] nvarchar(100)  NULL,
    [FileSizeKB] int  NULL,
    [IsActive] bit  NULL,
    [LastModifiedBy] nvarchar(100)  NULL,
    [LastModifiedOn] datetime  NULL,
    [UsesUIAutomation] bit  NULL,
    [IslongRunningWorkflow] bit  NOT NULL,
    [IdleStateTimeout] int  NOT NULL,
    [Tags] varchar(100)  NULL,
    [LicenseType] varchar(100)  NULL,
    [SourceUrl] varchar(300)  NULL
);
GO

-- Creating table 'Script'
CREATE TABLE [dbo].[Script] (
    [PartitionKey] nvarchar(5)  NOT NULL,
    [RowKey] nvarchar(10)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [ScriptId] int  NOT NULL,
    [Name] nvarchar(200)  NOT NULL,
    [Description] varchar(max)  NULL,
    [ScriptType] nvarchar(50)  NULL,
    [TaskType] nvarchar(10)  NULL,
    [TaskCmd] varchar(max)  NULL,
    [ScriptURL] nvarchar(500)  NULL,
    [ArgString] varchar(max)  NULL,
    [WorkingDir] nvarchar(200)  NULL,
    [BelongsToOrg] nvarchar(5)  NULL,
    [BelongsToAccount] nvarchar(5)  NULL,
    [BelongsToTrack] nvarchar(5)  NULL,
    [CreatedBy] nvarchar(100)  NOT NULL,
    [CreatedOn] datetime  NOT NULL,
    [ModifiedBy] nvarchar(100)  NULL,
    [ModifiedOn] datetime  NULL,
    [Version] int  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [RunAsAdmin] bit  NULL,
    [CategoryId] int  NOT NULL,
    [UsesUIAutomation] bit  NULL,
    [IfeaScriptName] varchar(100)  NULL,
    [CallMethod] varchar(100)  NULL,
    [Tags] nvarchar(500)  NULL,
    [LicenseType] varchar(100)  NULL,
    [SourceUrl] nvarchar(500)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [PartitonKey], [RowKey] in table 'Account'
ALTER TABLE [dbo].[Account]
ADD CONSTRAINT [PK_Account]
    PRIMARY KEY CLUSTERED ([PartitonKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'CategoryWorkflowMap'
ALTER TABLE [dbo].[CategoryWorkflowMap]
ADD CONSTRAINT [PK_CategoryWorkflowMap]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'ObjectModelMaster'
ALTER TABLE [dbo].[ObjectModelMaster]
ADD CONSTRAINT [PK_ObjectModelMaster]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Organization'
ALTER TABLE [dbo].[Organization]
ADD CONSTRAINT [PK_Organization]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'ScriptInstance'
ALTER TABLE [dbo].[ScriptInstance]
ADD CONSTRAINT [PK_ScriptInstance]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Track'
ALTER TABLE [dbo].[Track]
ADD CONSTRAINT [PK_Track]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'WorkflowCategoryMaster'
ALTER TABLE [dbo].[WorkflowCategoryMaster]
ADD CONSTRAINT [PK_WorkflowCategoryMaster]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'WorkflowTrackingDetails'
ALTER TABLE [dbo].[WorkflowTrackingDetails]
ADD CONSTRAINT [PK_WorkflowTrackingDetails]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'WorkflowTrackingMaster'
ALTER TABLE [dbo].[WorkflowTrackingMaster]
ADD CONSTRAINT [PK_WorkflowTrackingMaster]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'GroupAccess'
ALTER TABLE [dbo].[GroupAccess]
ADD CONSTRAINT [PK_GroupAccess]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Type'
ALTER TABLE [dbo].[Type]
ADD CONSTRAINT [PK_Type]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Role'
ALTER TABLE [dbo].[Role]
ADD CONSTRAINT [PK_Role]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'SuperAdmin'
ALTER TABLE [dbo].[SuperAdmin]
ADD CONSTRAINT [PK_SuperAdmin]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Group'
ALTER TABLE [dbo].[Group]
ADD CONSTRAINT [PK_Group]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'RegisterredNodes'
ALTER TABLE [dbo].[RegisterredNodes]
ADD CONSTRAINT [PK_RegisterredNodes]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'ActiveRegisteredNodes'
ALTER TABLE [dbo].[ActiveRegisteredNodes]
ADD CONSTRAINT [PK_ActiveRegisteredNodes]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'WorkflowParams'
ALTER TABLE [dbo].[WorkflowParams]
ADD CONSTRAINT [PK_WorkflowParams]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [ModuleID] in table 'Module'
ALTER TABLE [dbo].[Module]
ADD CONSTRAINT [PK_Module]
    PRIMARY KEY CLUSTERED ([ModuleID] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Category'
ALTER TABLE [dbo].[Category]
ADD CONSTRAINT [PK_Category]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'SemanticCluster'
ALTER TABLE [dbo].[SemanticCluster]
ADD CONSTRAINT [PK_SemanticCluster]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Companies'
ALTER TABLE [dbo].[Companies]
ADD CONSTRAINT [PK_Companies]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'ScheduledRequest'
ALTER TABLE [dbo].[ScheduledRequest]
ADD CONSTRAINT [PK_ScheduledRequest]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'ScheduledRequestActivities'
ALTER TABLE [dbo].[ScheduledRequestActivities]
ADD CONSTRAINT [PK_ScheduledRequestActivities]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'SemanticCategory'
ALTER TABLE [dbo].[SemanticCategory]
ADD CONSTRAINT [PK_SemanticCategory]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'ScriptParams'
ALTER TABLE [dbo].[ScriptParams]
ADD CONSTRAINT [PK_ScriptParams]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'SemanticNodeCluster'
ALTER TABLE [dbo].[SemanticNodeCluster]
ADD CONSTRAINT [PK_SemanticNodeCluster]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'TransactionInstance'
ALTER TABLE [dbo].[TransactionInstance]
ADD CONSTRAINT [PK_TransactionInstance]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'WorkflowMaster'
ALTER TABLE [dbo].[WorkflowMaster]
ADD CONSTRAINT [PK_WorkflowMaster]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- Creating primary key on [PartitionKey], [RowKey] in table 'Script'
ALTER TABLE [dbo].[Script]
ADD CONSTRAINT [PK_Script]
    PRIMARY KEY CLUSTERED ([PartitionKey], [RowKey] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------