CREATE TABLE [dbo].[Vote] (
    [VoteId]        INT        IDENTITY (1, 1) NOT NULL,
    [PollId]        INT        NOT NULL,
    [VoteDate]      DATETIME   NOT NULL,
    [VoterUserId]   INT        NOT NULL,
    [ElectedUserId] INT        NOT NULL,
    [CategoryId]    INT        NOT NULL,
    [RowVersion]    ROWVERSION NOT NULL,
    [IsActive]      BIT        NOT NULL,
    [CreatedDate]   DATETIME   NOT NULL,
    [ModifiedDate]  DATETIME   NOT NULL,
    CONSTRAINT [PK_Vote] PRIMARY KEY CLUSTERED ([VoteId] ASC),
    CONSTRAINT [FK_Vote_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([CategoryId]),
    CONSTRAINT [FK_Vote_Poll] FOREIGN KEY ([PollId]) REFERENCES [dbo].[Poll] ([PollId]),
    CONSTRAINT [FK_Vote_User] FOREIGN KEY ([ElectedUserId]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_Vote_User1] FOREIGN KEY ([VoterUserId]) REFERENCES [dbo].[User] ([UserId])
);

