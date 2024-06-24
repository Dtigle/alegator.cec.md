SET IDENTITY_INSERT Lookup.RegionTypes ON

DECLARE @Lookup_RegionTypes TABLE
(
	[regionTypeId] BIGINT NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[rank] [tinyint] NOT NULL,
	[description] [nvarchar](255) NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL
)

INSERT INTO @Lookup_RegionTypes ([regionTypeId], [name], [rank], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1, N'Republica',	0,	N'Republica',	SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2, N'r-n',			2,	N'raion',		SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3, N'UTA',			1,	N'UTA',			SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(4, N'mun.',		2,	N'municipiu',	SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(5, N'sector',		5,	N'sector',		SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(6, N'or.',			3,	N'oraș',		SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(7, N'orășel',		3,	N'orășel',		SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(8, N'com.',		4,  N'comună',		SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(9, N's.',			5,	N'sat',			SYSDATETIMEOFFSET(), null, null, 1, null, null)

MERGE Lookup.RegionTypes AS target
USING
	(
		SELECT [regionTypeId], [name], [rank], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @Lookup_RegionTypes
	) AS source
ON source.[regionTypeId] = target.[regionTypeId]
WHEN MATCHED AND (source.[name] <> target.[name]
OR source.[rank] <> target.[rank]
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
			target.[rank] = source.[rank],
			target.[description] = source.[description],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([regionTypeId], [name], [rank], [description], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[regionTypeId], source.[name], source.[rank], source.[description], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);

SET IDENTITY_INSERT Lookup.RegionTypes OFF