USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ScheduledRequest]    Script Date: 3/23/2015 6:03:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ScheduledRequest](
	[PartitionKey] [nvarchar](20) NOT NULL,
	[RowKey] [nvarchar](50) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ModifiedOn] [datetime2](7) NULL,
	[Id] [nvarchar](50) NOT NULL,
	[MachineName] [nvarchar](20) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[RequestVersion] [int] NULL,
	[InputParameters] [nvarchar](max) NULL,
	[OutputParameters] [nvarchar](max) NULL,
	[ExecuteOn] [datetime2](7) NULL,
	[Message] [nvarchar](max) NULL,
	[State] [int] NULL,
	[RequestType] [int] NOT NULL,
	[RequestId] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ScheduledRequest] PRIMARY KEY CLUSTERED 
(
	[PartitionKey] ASC,
	[RowKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[ScheduledRequest] ADD  CONSTRAINT [DF_ScheduledRequest_Timestamp]  DEFAULT (sysutcdatetime()) FOR [Timestamp]
GO

ALTER TABLE [dbo].[ScheduledRequest] ADD  CONSTRAINT [DF_ScheduledRequest_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'same as machine name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'PartitionKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'same as Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'RowKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'should be guid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'Id'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'e.g. workflow version' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'RequestVersion'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'json data' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'InputParameters'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'json data' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'OutputParameters'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'to be used to schedule the execution later' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'ExecuteOn'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'status Message in case of success and failure' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'Message'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1- New, 2- initiated, 3- in progress, 4- completed, 5- failed, 6- resubmit' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'State'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1- Workflow, 2 – Script' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'RequestType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'workflow id or script id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ScheduledRequest', @level2type=N'COLUMN',@level2name=N'RequestId'
GO


