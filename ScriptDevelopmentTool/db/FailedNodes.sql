/****** Object:  Table [dbo].[FailedNodes]    Script Date: 1/16/2015 1:40:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FailedNodes](
	[PartitionKey] [nvarchar](50) NOT NULL,
	[RowKey] [nvarchar](50) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[MachineName] [nvarchar](50) NOT NULL,
	[DomainName] [nvarchar](50) NOT NULL,
	[Cause] [text] NOT NULL,
 CONSTRAINT [PK_FailedNode] PRIMARY KEY CLUSTERED 
(
	[PartitionKey] ASC,
	[RowKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[FailedNodes] ADD  CONSTRAINT [DF_FailedNodes_Timestamp]  DEFAULT (sysutcdatetime()) FOR [Timestamp]
GO

ALTER TABLE [dbo].[FailedNodes] ADD  CONSTRAINT [DF_FailedNodes_CreatedOn]  DEFAULT (sysutcdatetime()) FOR [CreatedOn]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'holds the domain name e.g. ad.infosys.com' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FailedNodes', @level2type=N'COLUMN',@level2name=N'PartitionKey'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'hosting machine name e.g. punhjw166142d' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'FailedNodes', @level2type=N'COLUMN',@level2name=N'RowKey'
GO

