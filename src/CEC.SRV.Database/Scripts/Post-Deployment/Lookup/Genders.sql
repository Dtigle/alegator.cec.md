SET IDENTITY_INSERT Lookup.Genders ON

DECLARE @Lookup_Genders TABLE
(
	[genderId] BIGINT NOT NULL,
	[name] [nvarchar](255) NULL,
	[description] nvarchar(255) NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_Genders ([genderId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1, N'>', N'Ne cunoscut', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'M', N'Masculin', SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'F', N'Feminin', SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.Genders AS target
USING
	(
		SELECT [genderId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_Genders
	) AS source
ON source.[genderId] = target.[genderId]
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
		INSERT ([genderId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[genderId], source.[name], source.[description], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById])
WHEN NOT MATCHED BY source
	THEN DELETE;

SET IDENTITY_INSERT Lookup.Genders OFF