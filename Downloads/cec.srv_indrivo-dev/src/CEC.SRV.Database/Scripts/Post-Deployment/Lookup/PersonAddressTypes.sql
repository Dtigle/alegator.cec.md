SET IDENTITY_INSERT Lookup.PersonAddressTypes ON

DECLARE @Lookup_PersonAddressTypes TABLE
(
	[personAddressTypeId] BIGINT NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](100) NOT NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_PersonAddressTypes ([personAddressTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(0, N'Fără domiciliu', N'Fără domiciliu', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(1, N'Domiciliu', N'Viză de reședință', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'Viză temporară', N'Viză temporară', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'Declarație', N'Declarație de ședere', SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.PersonAddressTypes AS target
USING
	(
		SELECT [personAddressTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_PersonAddressTypes
	) AS source
ON source.[personAddressTypeId] = target.[personAddressTypeId]
WHEN MATCHED AND (source.[name] <> target.[name]
OR source.[description] <> target.[description]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[name] = source.[name],
			target.[description] = source.[description],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([personAddressTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[personAddressTypeId], source.[name], source.[description], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById])
WHEN NOT MATCHED BY source
	THEN DELETE;

SET IDENTITY_INSERT Lookup.PersonAddressTypes OFF