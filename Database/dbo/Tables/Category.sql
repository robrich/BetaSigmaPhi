CREATE TABLE [dbo].[Category] (
    [CategoryId]   INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [RowVersion]   ROWVERSION     NOT NULL,
    [IsActive]     BIT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC)
);

