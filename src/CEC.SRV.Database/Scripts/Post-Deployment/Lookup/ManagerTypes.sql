SET IDENTITY_INSERT Lookup.ManagerTypes ON

DECLARE @Lookup_ManagerTypes TABLE
(
	[managerTypeId] BIGINT NOT NULL,
	[name] [nvarchar](255) NULL,
	[description] nvarchar(255) NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_ManagerTypes ([managerTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1, N'Primar',					N'Primar',				SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'Primar-interimar',		N'Primar-interimar',	SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'Viceprimar',				N'Viceprimar',			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(4, N'Pretor',					N'Pretor',				SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(5, N'Pretor-interimar',		N'Pretor-interimar',	SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(6, N'Vicepretor',				N'Vicepretor',			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(7, N'Secretar',				N'Secretar',			SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.ManagerTypes AS target
USING
	(
		SELECT [managerTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_ManagerTypes
	) AS source
ON source.[managerTypeId] = target.[managerTypeId]
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
		INSERT ([managerTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[managerTypeId], source.[name], source.[description], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById])
WHEN NOT MATCHED BY source
	THEN DELETE;

SET IDENTITY_INSERT Lookup.ManagerTypes OFF