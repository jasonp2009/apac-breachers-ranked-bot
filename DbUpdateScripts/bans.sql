USE [ApacBreachersDb-Live]
GO

/****** Object:  Table [dbo].[UserBans]    Script Date: 27/08/2023 5:16:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserBans](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [decimal](20, 0) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[BannedAtUtc] [datetime2](7) NOT NULL,
	[Duration] [time](7) NOT NULL,
	[ExpiryUtc] [datetime2](7) NOT NULL,
	[Reason] [nvarchar](max) NOT NULL,
	[UnBanOverride] [bit] NOT NULL,
	[UnBanReason] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserBans] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


USE [ApacBreachersDb-Live]
GO

/****** Object:  Table [dbo].[ActiveBansMessages]    Script Date: 27/08/2023 5:16:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ActiveBansMessages](
	[Id] [uniqueidentifier] NOT NULL,
	[ActiveBansMessageId] [decimal](20, 0) NOT NULL,
 CONSTRAINT [PK_ActiveBansMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
