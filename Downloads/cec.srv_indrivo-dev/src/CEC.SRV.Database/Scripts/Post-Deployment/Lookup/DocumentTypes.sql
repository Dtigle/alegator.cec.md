SET IDENTITY_INSERT Lookup.DocumentTypes ON

DECLARE @Lookup_DocumentTypes TABLE
(
	documentTypeId BIGINT NOT NULL,
    isPrimary BIT null,
    name NVARCHAR(255) null,
    [description] NVARCHAR(255) null,
    created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null
)

INSERT INTO @Lookup_DocumentTypes ([documentTypeId], [name], [description], [isPrimary], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(0, N'NoDocument', N'Fără document de identitate', 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(1, N'Buletin', N'Buletin de identitate', 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'Pașaport', N'Pașaport național', 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'F-9', N'Forma Nr. 9', 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(4, N'Sovietic', N'Pașaport tip sovietic', 0, SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.DocumentTypes AS target
USING
	(
		SELECT [documentTypeId], [isPrimary], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_DocumentTypes
	) AS source
ON source.[documentTypeId] = target.[documentTypeId]
WHEN MATCHED AND (source.[isPrimary] <> target.[isPrimary]
OR source.[name] <> target.[name]
OR source.[description] <> target.[description]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[isPrimary] = source.[isPrimary],
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
		INSERT ([documentTypeId], [isPrimary], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[documentTypeId], source.[isPrimary], source.[name], source.[description], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById])
WHEN NOT MATCHED BY source
	THEN DELETE;

SET IDENTITY_INSERT Lookup.DocumentTypes OFF