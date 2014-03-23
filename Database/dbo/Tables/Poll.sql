CREATE TABLE [dbo].[Poll] (
    [PollId]                INT        IDENTITY (1, 1) NOT NULL,
    [FrequencyId]           INT        NOT NULL,
    [VoteCountPerFrequency] INT        CONSTRAINT [DF_Poll_VoteCountPerFrequency] DEFAULT ((1)) NOT NULL,
    [StartDate]             DATETIME   NOT NULL,
    [EndDate]               DATETIME   NOT NULL,
    [CategoryId]            INT        NOT NULL,
    [RowVersion]            ROWVERSION NOT NULL,
    [IsActive]              BIT        NOT NULL,
    [CreatedDate]           DATETIME   NOT NULL,
    [ModifiedDate]          DATETIME   NOT NULL,
    CONSTRAINT [PK_Poll] PRIMARY KEY CLUSTERED ([PollId] ASC),
    CONSTRAINT [FK_Poll_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([CategoryId]),
    CONSTRAINT [FK_Poll_Frequency] FOREIGN KEY ([FrequencyId]) REFERENCES [dbo].[Frequency] ([FrequencyId])
);

