SET IDENTITY_INSERT [RSP].[SexCodes] ON

MERGE INTO [RSP].[SexCodes] AS Target
USING (VALUES
(1 ,N'Masculin' , N'Мужской'),
(2 ,N'Feminin' , N'Женский'),
(3 ,N'Neidentificat' , N'Неопознанный')
) as Source (sexCodeId, [name], [namerus])
ON (Target.sexCodeId = Source.sexCodeId)
WHEN MATCHED AND (
	NULLIF(Source.[name], Target.[name]) IS NOT NULL OR NULLIF(Target.[name], Source.[name]) IS NOT NULL OR 
	NULLIF(Source.[namerus], Target.[namerus]) IS NOT NULL OR NULLIF(Target.[namerus], Source.[namerus]) IS NOT NULL  ) 
	THEN
 UPDATE SET
 [name] = Source.[name],
[namerus] = Source.[namerus]
WHEN NOT MATCHED BY TARGET THEN
 INSERT(sexCodeId,[name],[namerus])
 VALUES(Source.sexCodeId,Source.[name],Source.[namerus]);

GO
DECLARE @mergeError int
 , @mergeCount int
SELECT @mergeError = @@ERROR, @mergeCount = @@ROWCOUNT
IF @mergeError != 0
 BEGIN
 PRINT 'ERROR OCCURRED IN MERGE FOR [RSP].[sexCode]. Rows affected: ' + CAST(@mergeCount AS VARCHAR(100)); -- SQL should always return zero rows affected
 END
ELSE
 BEGIN
 PRINT '[RSP].[sexCode] rows affected by MERGE: ' + CAST(@mergeCount AS VARCHAR(100));
 END
GO

SET IDENTITY_INSERT [RSP].[SexCodes] OFF