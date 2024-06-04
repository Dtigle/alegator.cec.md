SET IDENTITY_INSERT [RSP].[DocTypeCodes] ON

MERGE INTO [RSP].[DocTypeCodes] AS Target
USING (VALUES
(3 ,N'PA PAŞAPORTUL CETĂŢEANULUI REPUBLICII MOLDOVA' , N'PA ПАСПОРТ ГРАЖДАНИНА РЕСПУБЛИКИ МОЛДОВА'),
(5 ,N'CA BULETIN DE IDENTITATE AL CETĂŢEANULUI RM' , N'CA УДОСТОВЕРЕНИЕ ЛИЧНОСТИ ГРАЖДАНИНА РМ'),
(60 ,N'AA PAŞAPORT DE TIP SOVIETIC (modelul an.1974)' , N'AA  ПАСПОРТ СОВЕТСКОГО ОБРАЗЦА (1974 года)'),
(71 ,N'BP BULETIN DE IDENTITATE PROVIZORIU (cu IDNP)' , N'BP ВРЕМЕННОЕ УДОСТОВЕРЕНИЕ ЛИЧНОСТИ (c IDNP)')
) as Source (docTypeCodeId, [name], [namerus])
ON (Target.docTypeCodeId = Source.docTypeCodeId)
WHEN MATCHED AND (
	NULLIF(Source.[name], Target.[name]) IS NOT NULL OR NULLIF(Target.[name], Source.[name]) IS NOT NULL OR 
	NULLIF(Source.[namerus], Target.[namerus]) IS NOT NULL OR NULLIF(Target.[namerus], Source.[namerus]) IS NOT NULL  ) 
	THEN
 UPDATE SET
 [name] = Source.[name],
[namerus] = Source.[namerus]
WHEN NOT MATCHED BY TARGET THEN
 INSERT(docTypeCodeId,[name],[namerus])
 VALUES(Source.docTypeCodeId,Source.[name],Source.[namerus]);

GO
DECLARE @mergeError int
 , @mergeCount int
SELECT @mergeError = @@ERROR, @mergeCount = @@ROWCOUNT
IF @mergeError != 0
 BEGIN
 PRINT 'ERROR OCCURRED IN MERGE FOR [RSP].[DocTypeCode]. Rows affected: ' + CAST(@mergeCount AS VARCHAR(100)); -- SQL should always return zero rows affected
 END
ELSE
 BEGIN
 PRINT '[RSP].[DocTypeCode] rows affected by MERGE: ' + CAST(@mergeCount AS VARCHAR(100));
 END
GO

SET IDENTITY_INSERT [RSP].[DocTypeCodes] OFF