create database InternetMarket
go

use InternetMarket
go


CREATE TABLE [dbo].[Roles](
	[RoleName] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[Users](
	[Login] [varchar](30) NOT NULL,
	[Password] [varchar](30) NOT NULL,
	[Role] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([Role])
REFERENCES [dbo].[Roles] ([RoleName])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Users] ADD  DEFAULT ('Customer') FOR [Role]
GO

CREATE TABLE [dbo].[Status](
	[StatusName] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StatusName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[Status] [varchar](30) NOT NULL,
	[Location] [varchar](30) NOT NULL,
	[UserLogin] [varchar](30) NOT NULL,
	[Date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [Date]
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([Status])
REFERENCES [dbo].[Status] ([StatusName])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserLogin])
REFERENCES [dbo].[Users] ([Login])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

CREATE TABLE [dbo].[Categories](
	[CategoryName] [varchar](30) NOT NULL,
 CONSTRAINT [PK_Categories_CategoryName] PRIMARY KEY CLUSTERED 
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[Products](
	[ProductName] [varchar](30) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[Category] [varchar](30) NOT NULL,
	[Rate] [real] NOT NULL,
	[NumberOfVotes] [int] NOT NULL,
	[Price] [real] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [Rate]
GO

ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [NumberOfVotes]
GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([Category])
REFERENCES [dbo].[Categories] ([CategoryName])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
-- Cart
CREATE TABLE [dbo].[UserProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL primary key,
	[UserLogin] [varchar](30) NOT NULL,
	[ProductName] [varchar](30) NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserProducts]  WITH CHECK ADD FOREIGN KEY([ProductName])
REFERENCES [dbo].[Products] ([ProductName])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserProducts]  WITH CHECK ADD FOREIGN KEY([UserLogin])
REFERENCES [dbo].[Users] ([Login])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

CREATE TABLE [dbo].[OrderProducts](
	[Id] [int] IDENTITY(1,1) NOT NULL primary key,
	[OrderId] [int] NOT NULL,
	[ProductName] [varchar](30) NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OrderProducts]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OrderProducts]  WITH CHECK ADD FOREIGN KEY([ProductName])
REFERENCES [dbo].[Products] ([ProductName])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

CREATE TABLE [dbo].[ProductVotes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductName] [varchar](30) NOT NULL,
	[UserLogin] [varchar](30) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ProductVotes]  WITH CHECK ADD FOREIGN KEY([ProductName])
REFERENCES [dbo].[Products] ([ProductName])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProductVotes]  WITH CHECK ADD FOREIGN KEY([UserLogin])
REFERENCES [dbo].[Users] ([Login])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

CREATE PROCEDURE [dbo].[UpdateProduct]
	@ProductName varchar(30),
	@Description varchar(100),
	@Category varchar(30),
	@Price real,
	@OldProductName varchar(30)
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Update Products
	Set ProductName = @ProductName, Description = @Description,
		Category = @Category, Price = @Price
	Where ProductName = @OldProductName
END

GO

