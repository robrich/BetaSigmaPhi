CREATE TABLE [dbo].[NodeType] (
    [NodeTypeId]   INT           NOT NULL,
    [NodeTypeName] NVARCHAR (30) NOT NULL,
    CONSTRAINT [PK_NodeType] PRIMARY KEY CLUSTERED ([NodeTypeId] ASC)
);

