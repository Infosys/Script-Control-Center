

/****** Object:  Table [dbo].[WorkflowParams]    Script Date: 1/14/2015 4:46:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WorkflowParams](
	[PartitionKey] [nvarchar](50) NOT NULL,
	[RowKey] [nvarchar](5) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[ParamId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[DefaultValue] [text] NULL,
	[IsMandatory] [bit] NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[ModifiedOn] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[AllowedValues] [text] NULL,
	[IsSecret] [bit] NOT NULL,
	[ParamType] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_WorkflowArguements] PRIMARY KEY CLUSTERED 
(
	[PartitionKey] ASC,
	[RowKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[WorkflowParams] ADD  CONSTRAINT [DF_WorkflowArguements_Timestamp]  DEFAULT (sysutcdatetime()) FOR [Timestamp]
GO

ALTER TABLE [dbo].[WorkflowParams] ADD  CONSTRAINT [DF_WorkflowArguements_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO

ALTER TABLE [dbo].[WorkflowParams] ADD  CONSTRAINT [DF_WorkflowArguements_IsSecret]  DEFAULT ((0)) FOR [IsSecret]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'holds the  Master work flow Id (guid)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WorkflowParams', @level2type=N'COLUMN',@level2name=N'PartitionKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'holds the Argument Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WorkflowParams', @level2type=N'COLUMN',@level2name=N'RowKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'will hold values like In, Out and InAndOut' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WorkflowParams', @level2type=N'COLUMN',@level2name=N'ParamType'
GO


