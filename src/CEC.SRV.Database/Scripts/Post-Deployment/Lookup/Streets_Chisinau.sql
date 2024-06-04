SET IDENTITY_INSERT Lookup.Streets ON

DECLARE @Lookup_Streets TABLE
(
	[streetId] BIGINT NOT NULL,
	[name] NVARCHAR(255) null,
	[description] NVARCHAR(255) NULL, 
	[streetTypeId] BIGINT NOT NULL,
	[regionId] BIGINT NOT NULL,
	[ropId] BIGINT NULL,
	[saiseId] BIGINT NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_Streets ([streetId], [name], [description], [streetTypeId], [regionId], [saiseId], [ropId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1,		N'Ștefan cel Mare și Sfânt',		null, 2, 42, null, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2,		N'Alecsandri V.',					null, 4, 42, null, null, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3,		N'Pușkin A.',						null, 4, 42, null, null, SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.Streets AS target
USING
	(
		SELECT [streetId], [name], [description], [streetTypeId], [regionId], [saiseId], [ropId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_Streets
	) AS source
ON source.[streetId] = target.[streetId]
WHEN MATCHED AND (source.[name] <> target.[name]
OR source.[description] <> target.[description]
OR source.[streetTypeId] <> target.[streetTypeId]
OR source.[regionId] <> target.[regionId]
OR source.[saiseId] <> target.[saiseId]
OR source.[ropId] <> target.[ropId]
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
			target.[streetTypeId] = source.[streetTypeId],
			target.[regionId] = source.[regionId],
			target.[saiseId] = source.[saiseId],
			target.[ropId] = source.[ropId],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([streetId], [name], [description], [streetTypeId], [regionId], [saiseId], [ropId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[streetId], source.[name], source.[description], source.[streetTypeId], source.[regionId], source.[saiseId], source.[ropId], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);

SET IDENTITY_INSERT Lookup.Streets OFF