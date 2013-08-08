CREATE TABLE [dbo].[ErrorLog] (
    [ErrorLogId]       INT             IDENTITY (1, 1) NOT NULL,
    [Message]          NVARCHAR (MAX)  NULL,
    [ExceptionDetails] NVARCHAR (MAX)  NULL,
    [UserId]           INT             NULL,
    [ClientIp]         NVARCHAR (128)  NULL,
    [HttpMethod]       NVARCHAR (10)   NULL,
    [PageUrl]          NVARCHAR (512)  NULL,
    [ReferrerUrl]      NVARCHAR (512)  NULL,
    [UserAgent]        NVARCHAR (1024) NULL,
    [RowVersion]       ROWVERSION      NOT NULL,
    [IsActive]         BIT             NOT NULL,
    [CreatedDate]      DATETIME        NOT NULL,
    [ModifiedDate]     DATETIME        NOT NULL,
    CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED ([ErrorLogId] ASC),
    CONSTRAINT [FK_ErrorLog_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([UserId])
);

