
--This database structure is created for testing purposes only

USE [master]
GO

CREATE DATABASE [SQLCCTool]
 
GO
USE [SQLCCTool]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SCCT_SessionLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[key] [varchar](50) NOT NULL,
	[value] [varchar](500) NOT NULL,
	[recordDate] [datetime] NOT NULL,
	[expirationDate] [datetime] NULL,
	[active] [bit] NOT NULL,
	[rowUpdateDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Surname] [varchar](100) NOT NULL,
	[UserName] [varchar](100) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Password] [varchar](300) NOT NULL,
	[Active] [bit] NOT NULL,
	[RowUpdateDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ValueTest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Total] [decimal](18, 6) NOT NULL,
	[Photo] [varbinary](max) NULL,
	[GuidId] [uniqueidentifier] NOT NULL,
	[Active] [bit] NOT NULL,
	[RowUpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK__ValueTes__3214EC07BAAD76F5] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[SCCT_SessionLog] ADD  DEFAULT (getdate()) FOR [recordDate]
GO
USE [master]
GO
ALTER DATABASE [SQLCCTool] SET  READ_WRITE 
GO


USE [master]
GO

CREATE DATABASE [Copier]
 
GO
