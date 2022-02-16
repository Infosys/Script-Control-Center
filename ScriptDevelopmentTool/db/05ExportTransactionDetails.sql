USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ExportTransactionDetails]    Script Date: 12/11/2017 10:45:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExportTransactionDetails](
	[id] [int] NOT NULL,
	[ExportScriptConfigurationId] [int] NOT NULL,
	[SourceCategoryId] [int] NOT NULL,
	[SourceScriptId] [int] NOT NULL,
	[SourceScriptVersion] [int] NOT NULL,
	[SourceScriptPath] [nvarchar](500) NULL,
	[TargetCategoryId] [int] NOT NULL,
	[TargetScriptId] [int] NOT NULL,
	[TargetScriptVersion] [int] NOT NULL,
	[TargetScriptName] [nvarchar](300) NOT NULL,
	[TargetScriptPath] [nvarchar](500) NULL,
	[Status] [smallint] NOT NULL,
	[ExistReasonCode] [smallint] NULL,
	[Details] [nvarchar](max) NULL,
	[Action] [int] NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](200) NULL,
	[ModifiedOn] [datetime] NULL,
	[isActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_ExportTransactionDetails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[ExportTransactionDetails] ADD  CONSTRAINT [DF_ExportTransactionDetails_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


