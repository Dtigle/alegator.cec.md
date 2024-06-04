SET IDENTITY_INSERT Lookup.PersonStatusTypes ON

DECLARE @Lookup_PersonStatusTypes TABLE
(
	[personStatusTypeId] BIGINT NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](100) NOT NULL,
	[isExcludable] [bit] NOT NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_PersonStatusTypes ([personStatusTypeId], [name], [description], [isExcludable], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1, N'Alegător', N'Alegător', 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'Decedat', N'Persoană decedată', 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'Hotarârea judecății', N'Hotarârea judecății', 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(4, N'Altă cetățenie', N'Persoana nu deține cetățenia RM și nu are dreptul la vot', 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(5, N'Militar', N'Persoana satisface serviciul militar în termen', 0, SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.PersonStatusTypes AS target
USING
	(
		SELECT [personStatusTypeId], [name], [description], [isExcludable], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_PersonStatusTypes
	) AS source
ON source.[personStatusTypeId] = target.[personStatusTypeId]
WHEN MATCHED AND (source.[name] <> target.[name]
OR source.[description] <> target.[description]
OR source.[isExcludable] <> target.[isExcludable]
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
			target.[isExcludable] = source.[isExcludable],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([personStatusTypeId], [name], [description], [isExcludable], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[personStatusTypeId], source.[name], source.[description], source.[isExcludable], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById])
WHEN NOT MATCHED BY source
	THEN DELETE;

SET IDENTITY_INSERT Lookup.PersonStatusTypes OFF