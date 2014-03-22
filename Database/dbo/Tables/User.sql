CREATE TABLE [dbo].[User] (
    [UserId]              INT            IDENTITY (1, 1) NOT NULL,
    [AuthenticationToken] NVARCHAR (32)  NOT NULL,
    [Email]               NVARCHAR (128) NOT NULL,
    [FirstName]           NVARCHAR (64)  NOT NULL,
    [LastName]            NVARCHAR (64)  NULL,
    [Password]            NVARCHAR (256) NOT NULL,
    [Salt]                NVARCHAR (64)  NOT NULL,
    [LoginFailCount]      INT            NOT NULL,
    [LoginFailStartDate]  DATETIME       NULL,
    [IsAdmin]             BIT            NOT NULL,
    [IsElectable]         BIT            CONSTRAINT [DF_User_IsElectable] DEFAULT ((1)) NOT NULL,
    [RowVersion]          ROWVERSION     NOT NULL,
    [IsActive]            BIT            NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL,
    [ModifiedDate]        DATETIME       NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC)
);



