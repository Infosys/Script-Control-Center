USE [IAP_WorkflowExecutionStore]
GO

/****** Object:  Table [dbo].[ExportServerDetails]    Script Date: 12/11/2017 10:43:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExportServerDetails](
	[id] [int] NOT NULL,
	[DNSServer] [nvarchar](200) NOT NULL,
	[CasServer] [nvarchar](200) NOT NULL,
	[TargetSystemId] [int] NOT NULL,
	[CreatedBy] [nvarchar](200) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](200) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_ServerDetails] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


