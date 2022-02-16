USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ExportConfigurationMaster]    Script Date: 12/11/2017 10:42:11 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExportConfigurationMaster](
	[id] [int] NOT NULL,
	[TargetServerId] [int] NOT NULL,
	[TargetSystemUserId] [nvarchar](200) NOT NULL,
	[TargetSystemPassword] [nvarchar](300) NOT NULL,
	[ExportStatus] [int] NOT NULL,
	[CompletedOn] [datetime] NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](200) NULL,
	[ModifiedOn] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[ScriptRepositoryBaseServerAddress] [nvarchar](200) NULL,
 CONSTRAINT [PK_ExportConfigurationMaster] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ExportConfigurationMaster] ADD  CONSTRAINT [DF_ExportConfigurationMaster_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO


