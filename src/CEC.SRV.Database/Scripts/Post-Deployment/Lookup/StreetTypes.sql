SET IDENTITY_INSERT Lookup.StreetTypes ON

DECLARE @Lookup_StreetTypes TABLE
(
	[streetTypeId] BIGINT NOT NULL,
	[name] [nvarchar](255) NULL,
	[description] nvarchar(255) NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_StreetTypes ([streetTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1, N'>',			N'Necunoscut',			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'bd.',			N'bulevard',			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'șos.',		N'șosea',				SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(4, N'str.',		N'stradă',				SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(5, N'str-la',		N'stradelă',			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(6, N'str-la înf.',	N'stradelă înfundată',	SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.StreetTypes AS target
USING
	(
		SELECT [streetTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_StreetTypes
	) AS source
ON source.[streetTypeId] = target.[streetTypeId]
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
		INSERT ([streetTypeId], [name], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[streetTypeId], source.[name], source.[description], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);

SET IDENTITY_INSERT Lookup.StreetTypes OFF