SET IDENTITY_INSERT Lookup.ElectionTypes ON

DECLARE @Lookup_ElectionTypes TABLE
(
	[electionTypeId] BIGINT NOT NULL,
	[name] [nvarchar](255) NULL,
	[description] nvarchar(255) NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_ElectionTypes ([electionTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1, N'Referendum Local', N'Referendum Local', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'Referendum Constituțional Republican', N'Referendum Constituțional Republican', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'Alegerile Parlamentului Republicii Moldova', N'Alegerile Parlamentului Republicii Moldova', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(4, N'Alegerile Locale în Republicii Moldova', N'Alegerile Locale în Republicii Moldova', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(5, N'Alegerile Locale Generale în Republicii Moldova', N'Alegerile Locale Generale în Republicii Moldova', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(6, N'Alegerile Locale Noi în Republicii Moldova', N'Alegerile Locale Noi în Republicii Moldova', SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.ElectionTypes AS target
USING
	(
		SELECT [electionTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_ElectionTypes
	) AS source
ON source.[electionTypeId] = target.[electionTypeId]
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
		INSERT ([electionTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[electionTypeId], source.[name], source.[description], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById])
WHEN NOT MATCHED BY source
	THEN DELETE;

SET IDENTITY_INSERT Lookup.ElectionTypes OFF