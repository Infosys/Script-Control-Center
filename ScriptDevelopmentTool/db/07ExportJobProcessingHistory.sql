USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ExportJobProcessingHistory]    Script Date: 12/11/2017 10:45:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExportJobProcessingHistory](
	[JobId] [int] NOT NULL,
	[ExportConfigurationId] [int] NOT NULL,
	[StartedOn] [datetime] NOT NULL,
	[CompletedOn] [datetime] NULL,
	[ProcessingSystemIp] [nvarchar](50) NOT NULL,
	[ProcessingSystemName] [nvarchar](200) NOT NULL,
	[ProcessedBy] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_ExportJobProcessingHistory] PRIMARY KEY CLUSTERED 
(
	[JobId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


