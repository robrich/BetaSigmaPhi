CREATE TABLE [dbo].[Document] (
    [DocumentId]       INT            IDENTITY (1, 1) NOT NULL,
    [NodeTypeId]       INT            NOT NULL,
    [ParentDocumentId] INT            NOT NULL,
    [NodeIdPath]       NVARCHAR (400) NOT NULL,
    [Depth]            INT            NOT NULL,
    [Slug]             NVARCHAR (80)  NOT NULL,
    [Name]             NVARCHAR (80)  NOT NULL,
    [PageContent]      NVARCHAR (MAX) NULL,
    [HeadContent]      NVARCHAR (MAX) NULL,
    [ScriptContent]    NVARCHAR (MAX) NULL,
    [RowVersion]       ROWVERSION     NOT NULL,
    [IsActive]         BIT            NOT NULL,
    [CreatedDate]      DATETIME       NOT NULL,
    [ModifiedDate]     DATETIME       NOT NULL,
    CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED ([DocumentId] ASC),
    CONSTRAINT [FK_Document_Document] FOREIGN KEY ([ParentDocumentId]) REFERENCES [dbo].[Document] ([DocumentId]),
    CONSTRAINT [FK_Document_NodeType] FOREIGN KEY ([NodeTypeId]) REFERENCES [dbo].[NodeType] ([NodeTypeId])
);

