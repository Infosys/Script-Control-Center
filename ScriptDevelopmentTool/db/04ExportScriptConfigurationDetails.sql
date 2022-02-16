USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ExportScriptConfigurationDetails]    Script Date: 12/11/2017 10:44:39 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExportScriptConfigurationDetails](
	[id] [int] NOT NULL,
	[ExportConfigurationId] [int] NOT NULL,
	[SourceCategoryId] [int] NOT NULL,
	[SourceScriptPath] [nvarchar](max) NOT NULL,
	[TargetCategoryId] [int] NOT NULL,
	[TargetScriptPath] [nvarchar](max) NOT NULL,
	[SourceScriptId] [int] NOT NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](200) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ExportScriptConfigurationDetails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[ExportScriptConfigurationDetails] ADD  CONSTRAINT [DF_ExportScriptConfigurationDetails_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


