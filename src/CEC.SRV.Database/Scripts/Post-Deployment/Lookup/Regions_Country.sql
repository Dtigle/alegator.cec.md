SET IDENTITY_INSERT Lookup.Regions ON

DECLARE @Lookup_Regions TABLE
(
	[regionId] BIGINT NOT NULL,
	[name] [nvarchar](255) NULL,
	[description] [nvarchar](255) NULL, 
	[parentId] BIGINT NULL,
	[regionTypeId] BIGINT NOT NULL,
	[saiseId] BIGINT NULL,
	[registruId] BIGINT NULL,
	[hasStreets] BIT NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL,
	[publicAdministrationId] BIGINT NULL,
	[circumscription] int NULL
)

INSERT INTO @Lookup_Regions ([regionId], [name], [description], [parentId], [regionTypeId],[hasStreets], [saiseId], [registruId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById], [publicAdministrationId], [circumscription])
VALUES
	(1,		N'Moldova',			null, null, 1, 0, null, null, SYSDATETIMEOFFSET(), null, null, 1, null, null, null, null)
	

MERGE Lookup.Regions AS target
USING
	(
		SELECT [regionId], [name], [description], [parentId], [regionTypeId], [hasStreets], [saiseId], [registruId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById], [publicAdministrationId], [circumscription]
		FROM @Lookup_Regions
	) AS source
ON source.[regionId] = target.[regionId]
WHEN MATCHED AND (source.[name] <> target.[name]
OR source.[description] <> target.[description]
OR source.[parentId] <> target.[parentId]
OR source.[regionTypeId] <> target.[regionTypeId]
OR source.[saiseId] <> target.[saiseId]
OR source.[registruId] <> target.[registruId]
OR source.[hasStreets] <> target.[hasStreets]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById]
OR source.[publicAdministrationId] <> target.[publicAdministrationId]
OR source.[circumscription] <> target.[circumscription])
	THEN 
		UPDATE SET 
			target.[name] = source.[name],
			target.[description] = source.[description],
			target.[parentId] = source.[parentId],
			target.[regionTypeId] = source.[regionTypeId],
			target.[saiseId] = source.[saiseId],
			target.[registruId] = source.[registruId],
			target.[hasStreets] = source.[hasStreets],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById],
			target.[publicAdministrationId] = source.[publicAdministrationId],
			target.[circumscription] = source.[circumscription]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([regionId], [name], [description], [parentId], [regionTypeId], [hasStreets], [saiseId], [registruId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById], [publicAdministrationId], [circumscription])
		VALUES (source.[regionId], source.[name], source.[description], source.[parentId], source.[regionTypeId], source.[hasStreets], source.[saiseId], source.[registruId], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById], source.[publicAdministrationId], source.[circumscription]);

SET IDENTITY_INSERT Lookup.Regions OFF