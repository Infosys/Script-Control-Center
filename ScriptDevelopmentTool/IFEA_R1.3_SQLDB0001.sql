USE [IAP_WorkflowExecutionStore]
alter table category add ModuleID int

GO

Update category set ModuleID =1

GO

/****** Object:  Table [dbo].[Module]    Script Date: 3/19/2015 3:38:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Module](
       [ModuleID] [int] NOT NULL,
       [ModuleName] [nvarchar](50) NOT NULL,
CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
(
       [ModuleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

GO

INSERT INTO [dbo].[Module] ([ModuleID],[ModuleName])
     VALUES (1,'All')

INSERT INTO [dbo].[Module] ([ModuleID],[ModuleName])
     VALUES (2,'Script')

INSERT INTO [dbo].[Module] ([ModuleID],[ModuleName])
     VALUES (3,'Workflow')

GO
