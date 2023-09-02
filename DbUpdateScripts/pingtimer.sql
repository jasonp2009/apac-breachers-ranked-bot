USE [ApacBreachersDb-Dev]
GO

/****** Object:  Table [dbo].[TimedPings]    Script Date: 2/09/2023 8:26:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TimedPings](
	[RoleId] [decimal](20, 0) IDENTITY(1,1) NOT NULL,
	[TimeOutMins] [int] NOT NULL,
	[IsTimedOut] [bit] NOT NULL,
	[NextPingUtc] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_TimedPings] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


