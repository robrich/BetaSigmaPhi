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

MERGE INTO dbo.Frequency AS Target 
USING (VALUES 
  (1, N'Weekly'), 
  (2, N'Monthly'), 
  (3, N'Quarterly'), 
  (4, N'SemiAnually'), 
  (5, N'Anually')
) 
AS Source (FrequencyId, FrequencyDesc) 
ON Target.FrequencyId = Source.FrequencyId 
-- update matched rows 
WHEN MATCHED THEN 
UPDATE SET FrequencyDesc = Source.FrequencyDesc 
-- insert new rows 
WHEN NOT MATCHED BY TARGET THEN 
INSERT (FrequencyId, FrequencyDesc) 
VALUES (FrequencyId, FrequencyDesc) 
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
		'Beta Sigma Phi', '', NULL, NULL,
		GETDATE(), GETDATE(), 1
	)
	SET IDENTITY_INSERT dbo.Document ON
END;
