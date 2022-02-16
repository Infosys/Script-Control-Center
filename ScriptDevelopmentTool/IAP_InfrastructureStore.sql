


USE [IAP_InfrastructureStore]
GO

CREATE TABLE [dbo].[AlertType](
	[ID] [int] NOT NULL,
	[Type] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Alerts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[ErrorType](
	[ID] [int] NOT NULL,
	[Type] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ErrorType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


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

CREATE TABLE [dbo].[TicketActions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Action] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Ticket_Actions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[TicketStates](
	[Id] [int] NOT NULL,
	[StateName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Ticket_SubStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbo].[Ticket](
	[TicketNumber] [varchar](50) NOT NULL,
	[Reason] [varchar](50) NOT NULL,
	[StateId] [int] NOT NULL,
	[Remarks] [varchar](100) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedFromMachineIP] [varchar](50) NOT NULL,
	[CreatedFromMachineName] [varchar](50) NOT NULL,
	[LastModifiedOn] [datetime] NULL,
	[LastModifiedBy] [varchar](50) NULL,
	[LastModifiedFromMachineIP] [varchar](50) NULL,
	[LastModifiedFromMachineName] [varchar](50) NULL,
	[LastAction] [int] NULL,
	[StatusUpdatedDate] [datetime] NULL,
	[Data] [varchar](max) NULL,
	[Priority] [int] NOT NULL,
 CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED 
(
	[TicketNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

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

INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (1, N'LoginRequired')
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (2, N'TicketError')
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (3, N'HandlerError')
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (4, N'Information')
INSERT [dbo].[AlertType] ([ID], [Type]) VALUES (5, N'Custom')


INSERT [dbo].[ErrorType] ([ID], [Type]) VALUES (1, N'Critical')
INSERT [dbo].[ErrorType] ([ID], [Type]) VALUES (2, N'Warning')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (1, N'New')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (2, N'InProgress')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (3, N'ClientPending-MissingInformation')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (4, N'ClientPending-Resolved')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (5, N'WaitingForApproval')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (6, N'Approved')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (7, N'3rdPartyPending')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (8, N'Completed')
INSERT [dbo].[TicketStates] ([Id], [StateName]) VALUES (9, N'Failed')
ALTER TABLE [dbo].[Modules] ADD  CONSTRAINT [DF_Modules_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Scripts] ADD  CONSTRAINT [DF_Script_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Ticket] ADD  DEFAULT ((2)) FOR [Priority]
GO
ALTER TABLE [dbo].[Alerts]  WITH CHECK ADD  CONSTRAINT [FK_Alerts2_Alerts2] FOREIGN KEY([AlertTypeID])
REFERENCES [dbo].[AlertType] ([ID])
GO
ALTER TABLE [dbo].[Alerts] CHECK CONSTRAINT [FK_Alerts2_Alerts2]
GO
ALTER TABLE [dbo].[AuditTracker]  WITH CHECK ADD  CONSTRAINT [FK_AuditTracker_TicketStates] FOREIGN KEY([StateId])
REFERENCES [dbo].[TicketStates] ([Id])
GO
ALTER TABLE [dbo].[AuditTracker] CHECK CONSTRAINT [FK_AuditTracker_TicketStates]
GO
ALTER TABLE [dbo].[ErrorLog]  WITH CHECK ADD  CONSTRAINT [FK_ErrorLog_ErrorType] FOREIGN KEY([ErrorTypeID])
REFERENCES [dbo].[ErrorType] ([ID])
GO
ALTER TABLE [dbo].[ErrorLog] CHECK CONSTRAINT [FK_ErrorLog_ErrorType]
GO
ALTER TABLE [dbo].[Modules]  WITH CHECK ADD  CONSTRAINT [FK_Modules_Scripts] FOREIGN KEY([ScriptId])
REFERENCES [dbo].[Scripts] ([Id])
GO
ALTER TABLE [dbo].[Modules] CHECK CONSTRAINT [FK_Modules_Scripts]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_TicketActions] FOREIGN KEY([LastAction])
REFERENCES [dbo].[TicketActions] ([Id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_TicketActions]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Tickets_SubStatus] FOREIGN KEY([StateId])
REFERENCES [dbo].[TicketStates] ([Id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Tickets_SubStatus]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Ticket ID in the ticketing tool' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ticket', @level2type=N'COLUMN',@level2name=N'TicketNumber'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'This is ticket fields in ticket. It decides whether request is for partner invitation or new/existing user access
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ticket', @level2type=N'COLUMN',@level2name=N'Reason'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'State of ticket, this is not part of ticketing tool' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ticket', @level2type=N'COLUMN',@level2name=N'StateId'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Any remarks about ticket' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ticket', @level2type=N'COLUMN',@level2name=N'Remarks'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'datetime on which ticket is added in DB' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ticket', @level2type=N'COLUMN',@level2name=N'CreatedOn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'datetime of ticket state update in DB' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Ticket', @level2type=N'COLUMN',@level2name=N'LastModifiedOn'
GO