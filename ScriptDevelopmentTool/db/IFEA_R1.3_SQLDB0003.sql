CREATE DATABASE [IAP_InfrastructureStore]

Use [IAP_InfrastructureStore]

CREATE TABLE [dbo].[TicketStates](
	[Id] [int] NOT NULL,
	[StateName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Ticket_SubStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[AlertType](
	[ID] [int] NOT NULL,
	[Type] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Alerts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ErrorType](
	[ID] [int] NOT NULL,
	[Type] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ErrorType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[AuditTracker](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TicketNumber] [varchar](50) NULL,
	[StateId] [int] NULL,
	[ScriptId] [varchar](50) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[Data] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING ON
ALTER TABLE [dbo].[AuditTracker] ADD [CreatedBy] [varchar](50) NOT NULL
ALTER TABLE [dbo].[AuditTracker] ADD [TimeStamp] [datetime] NOT NULL
SET ANSI_PADDING OFF
ALTER TABLE [dbo].[AuditTracker] ADD [CreatedFromMachineName] [varchar](50) NOT NULL
ALTER TABLE [dbo].[AuditTracker] ADD [CreatedFromMachineIP] [varchar](50) NOT NULL
 CONSTRAINT [PK_AuditTracker] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ErrorLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TicketNumber] [varchar](50) NULL,
	[TimeStamp] [datetime] NOT NULL,
	[ErrorDetails] [varchar](max) NOT NULL,
	[CreatedFromMachineName] [varchar](50) NOT NULL,
	[ErrorTypeID] [int] NOT NULL,
	[ScriptId] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedFromMachineIP] [varchar](50) NOT NULL,
	[Message] [varchar](max) NULL,
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


ALTER TABLE [dbo].[ErrorLog]  WITH CHECK ADD  CONSTRAINT [FK_ErrorLog_ErrorType] FOREIGN KEY([ErrorTypeID])
REFERENCES [dbo].[ErrorType] ([ID])
GO

ALTER TABLE [dbo].[ErrorLog] CHECK CONSTRAINT [FK_ErrorLog_ErrorType]


CREATE TABLE [dbo].[Scripts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScriptIdentifier] [varchar](max) NOT NULL,
	[Version] [int] NOT NULL,
	[ReleasedOn] [datetime] NOT NULL,
	[Name] [varchar](250) NULL,
	[BusinessArea] [varchar](50) NULL,
	[SubBusinessArea] [varchar](50) NULL,
	[Active] [bit] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModifiedBy] [varchar](50) NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_Script] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Scripts] ADD  CONSTRAINT [DF_Script_Active]  DEFAULT ((1)) FOR [Active]
GO

CREATE TABLE [dbo].[Modules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TicketReason] [varchar](max) NOT NULL,
	[ScriptId] [int] NOT NULL,
	[FunctionName] [varchar](250) NULL,
	[BusinessArea] [varchar](50) NULL,
	[SubBusinessArea] [varchar](50) NULL,
	[Remarks] [varchar](max) NULL,
	[Active] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_Modules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF_Modules_Active]  DEFAULT ((1)) FOR [Active]
GO

ALTER TABLE [dbo].[Modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_Scripts] FOREIGN KEY([ScriptId])
REFERENCES [dbo].[Scripts] ([Id])
GO

ALTER TABLE [dbo].[Modules] CHECK CONSTRAINT [FK_Modules_Scripts]
GO

CREATE TABLE [dbo].[Alerts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TicketNumber] [varchar](50) NULL,
	[AlertTypeID] [int] NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[Recepient] [varchar](max) NOT NULL,
	[IsNotfied] [bit] NOT NULL,
	[ScriptId] [varchar](50) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[CreatedFromMachineName] [varchar](50) NOT NULL,
	[CreatedFromMachineIP] [varchar](50) NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Alerts2] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Alerts]  WITH CHECK ADD  CONSTRAINT [FK_Alerts2_Alerts2] FOREIGN KEY([AlertTypeID])
REFERENCES [dbo].[AlertType] ([ID])
GO

ALTER TABLE [dbo].[Alerts] CHECK CONSTRAINT [FK_Alerts2_Alerts2]
GO

USE [IAP_InfrastructureStore]
GO
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (1, N'LoginRequired')
GO
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (2, N'TicketError')
GO
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (3, N'HandlerError')
GO
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (4, N'Information')
GO
INSERT [dbo].[ErrorType] ([ID], [Type]) VALUES (1, N'Critical')
GO
INSERT [dbo].[ErrorType] ([ID], [Type]) VALUES (2, N'Warning')
GO
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (1, N'InProgress')
GO
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (2, N'ClientPending-MissingInformation')
GO
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (3, N'ClientPending-Resolved')
GO
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (4, N'3rdPartyPending')
GO
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (5, N'Completed')
GO
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (6, N'Failed')
GO
