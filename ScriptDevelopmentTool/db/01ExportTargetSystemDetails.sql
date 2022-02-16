USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ExportTargetSystemDetails]    Script Date: 12/11/2017 10:43:30 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExportTargetSystemDetails](
	[id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[APIType] [int] NOT NULL,
	[Protocol] [int] NOT NULL,
	[DefaultType] [bit] NOT NULL,
 CONSTRAINT [PK_TargetSystemDetails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


