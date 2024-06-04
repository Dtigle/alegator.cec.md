SET IDENTITY_INSERT SRV.Addresses ON

DECLARE @SRV_Addresses TABLE
(
	[addressId] BIGINT NOT NULL,
    [houseNumber] BIGINT null,
    [suffix] NVARCHAR(255) null,
	[pollingStationId] BIGINT null,
    [created] DATETIMEOFFSET null,
    [modified] DATETIMEOFFSET null,
    [deleted] DATETIMEOFFSET null,
    [streetId] BIGINT not null,
    [createdById] NVARCHAR(255) null,
    [modifiedById] NVARCHAR(255) null,
    [deletedById] NVARCHAR(255) null,
	[buildingType] INT NOT NULL
)

INSERT INTO @SRV_Addresses ([addressId], [houseNumber], [suffix], [pollingStationId], [streetId], [buildingType], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	( 1, 136,  null, null, 1, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 2,  26,  null, null, 1, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 3,  15,  null, null, 3, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 4,  36,  null, null, 3, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 5, 134,  null, null, 1, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 6,  74,  null, null, 2, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 7,  78,  null, null, 2, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 8,  21,  null, null, 3, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null)
	
MERGE SRV.Addresses AS target
USING
	(
		SELECT [addressId], [houseNumber], [suffix], [pollingStationId], [streetId], [buildingType], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_Addresses
	) AS source
ON source.[addressId] = target.[addressId]
WHEN MATCHED AND (source.[houseNumber] <> target.[houseNumber]
OR source.[suffix] <> target.[suffix]
OR source.[pollingStationId] <> target.[pollingStationId]
OR source.[streetId] <> target.[streetId]
OR source.[buildingType] <> target.[buildingType]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[houseNumber] = source.[houseNumber],
			target.[suffix] = source.[suffix],
			target.[pollingStationId] = source.[pollingStationId],
			target.[streetId] = source.[streetId],
			target.[buildingType] = source.[buildingType],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([addressId], [houseNumber], [suffix], [pollingStationId], [streetId], [buildingType], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[addressId], source.[houseNumber], source.[suffix], source.[pollingStationId], source.[streetId], source.[buildingType], 
		source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);
SET IDENTITY_INSERT SRV.Addresses OFF