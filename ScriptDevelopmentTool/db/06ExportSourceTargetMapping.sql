USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ExportSourceTargetMapping]    Script Date: 12/11/2017 10:45:25 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExportSourceTargetMapping](
	[id] [int] NOT NULL,
	[SourceInstanceAddr] [nvarchar](200) NOT NULL,
	[SourceScriptCategoryId] [int] NOT NULL,
	[SourceScriptId] [int] NOT NULL,
	[SourceScriptVersion] [int] NOT NULL,
	[TargetInstanceId] [int] NOT NULL,
	[TargetScriptCategoryId] [int] NOT NULL,
	[TargetScriptId] [int] NOT NULL,
	[TargetScriptVersion] [int] NOT NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](200) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ExportSourceTargetMapping] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


