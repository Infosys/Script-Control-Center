USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[RegisterredNodes]    Script Date: 1/30/2015 6:36:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RegisterredNodes](
	[PartitionKey] [nvarchar](50) NOT NULL,
	[RowKey] [nvarchar](50) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[RegisteredOn] [datetime2](7) NOT NULL,
	[UnRegisteredOn] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[HttpPort] [int] NULL,
	[TcpPort] [int] NULL,
	[MachineName] [nvarchar](20) NOT NULL,
	[DomainName] [nvarchar](30) NOT NULL,
	[State] [nvarchar](10) NOT NULL,
	[OSVersion] [nvarchar](200) NULL,
	[Is64Bit] [bit] NOT NULL,
	[WorkflowServiceVersion] [nvarchar](20) NULL,
	[DotNetVersion] [nvarchar](50) NULL,
	[ExecutionEngineSupported] [int] NOT NULL,
	[CreatedOn] [datetime2](7) NULL,
	[LastModifiedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_RegisterredNodes] PRIMARY KEY CLUSTERED 
(
	[PartitionKey] ASC,
	[RowKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RegisterredNodes] ADD  CONSTRAINT [DF_RegisterredNodes_Timestamp]  DEFAULT (sysutcdatetime()) FOR [Timestamp]
GO

ALTER TABLE [dbo].[RegisterredNodes] ADD  CONSTRAINT [DF_RegisterredNodes_DomainName]  DEFAULT (N'ad.infosys.com') FOR [DomainName]
GO

ALTER TABLE [dbo].[RegisterredNodes] ADD  CONSTRAINT [DF_RegisterredNodes_Is64Bit]  DEFAULT ((0)) FOR [Is64Bit]
GO

ALTER TABLE [dbo].[RegisterredNodes] ADD  CONSTRAINT [DF_RegisterredNodes_ExecutionEngineSupported]  DEFAULT ((1)) FOR [ExecutionEngineSupported]
GO

ALTER TABLE [dbo].[RegisterredNodes] ADD  CONSTRAINT [DF_RegisterredNodes_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'holds the domain name e.g. ad.infosys.com' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RegisterredNodes', @level2type=N'COLUMN',@level2name=N'PartitionKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'hosting machine name e.g. punhjw166142d' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RegisterredNodes', @level2type=N'COLUMN',@level2name=N'RowKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'hostine machine name e.g. punhjw166142d' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RegisterredNodes', @level2type=N'COLUMN',@level2name=N'MachineName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Active/InActive ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RegisterredNodes', @level2type=N'COLUMN',@level2name=N'State'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'the kind of execution is supported e.g. for work flow -1 (0001), for script 2(0010), for both 3 (0011)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'RegisterredNodes', @level2type=N'COLUMN',@level2name=N'ExecutionEngineSupported'
GO


