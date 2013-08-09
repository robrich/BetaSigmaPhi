/*
POST DEPLOYMENT SCRIPT
*/
-- NodeType data
MERGE INTO NodeType AS Target 
USING (VALUES 
  (1, N'Root'), 
  (2, N'Branch'), 
  (3, N'Leaf')
) 
AS Source (NodeTypeId, NodeTypeName) 
ON Target.NodeTypeId = Source.NodeTypeId 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET NodeTypeName = Source.NodeTypeName 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (NodeTypeId, NodeTypeName) 
VALUES (NodeTypeId, NodeTypeName) 
-- delete rows that are in the target but not the source 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

-- Ensure root Document exists
IF (SELECT COUNT(*) FROM dbo.Document WHERE DocumentId = 1) = 0
BEGIN
	SET IDENTITY_INSERT dbo.Document ON
	INSERT INTO dbo.Document (
		DocumentId, NodeTypeId, ParentDocumentId, NodeIdPath, Depth, Slug,
		Name, PageContent, HeadContent, ScriptContent,
		CreatedDate, ModifiedDate, IsActive
	) VALUES (
		1, 1, 1, '/1', 0, '',
		'CMS', '', NULL, NULL,
		GETDATE(), GETDATE(), 1
	)
	SET IDENTITY_INSERT dbo.Document ON
END;
