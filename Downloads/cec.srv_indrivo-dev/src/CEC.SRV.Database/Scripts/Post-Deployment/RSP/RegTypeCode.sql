SET IDENTITY_INSERT [RSP].[RegTypeCodes] ON

MERGE INTO [RSP].[RegTypeCodes] AS Target
USING (VALUES
(1 ,N'Permanentă' , N'Постоянная'),
(2 ,N'Temporară' , N'Временная')
) as Source (regTypeCodeId, [name], [namerus])
ON (Target.regTypeCodeId = Source.regTypeCodeId)
WHEN MATCHED AND (
	NULLIF(Source.[name], Target.[name]) IS NOT NULL OR NULLIF(Target.[name], Source.[name]) IS NOT NULL OR 
	NULLIF(Source.[namerus], Target.[namerus]) IS NOT NULL OR NULLIF(Target.[namerus], Source.[namerus]) IS NOT NULL  ) 
	THEN
 UPDATE SET
 [name] = Source.[name],
[namerus] = Source.[namerus]
WHEN NOT MATCHED BY TARGET THEN
 INSERT(regTypeCodeId,[name],[namerus])
 VALUES(Source.regTypeCodeId,Source.[name],Source.[namerus]);

GO
DECLARE @mergeError int
 , @mergeCount int
SELECT @mergeError = @@ERROR, @mergeCount = @@ROWCOUNT
IF @mergeError != 0
 BEGIN
 PRINT 'ERROR OCCURRED IN MERGE FOR [RSP].[RegTypeCode]. Rows affected: ' + CAST(@mergeCount AS VARCHAR(100)); -- SQL should always return zero rows affected
 END
ELSE
 BEGIN
 PRINT '[RSP].[RegTypeCode] rows affected by MERGE: ' + CAST(@mergeCount AS VARCHAR(100));
 END
GO

SET IDENTITY_INSERT [RSP].[RegTypeCodes] OFF