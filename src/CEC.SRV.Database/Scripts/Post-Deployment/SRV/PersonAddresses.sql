SET IDENTITY_INSERT SRV.PersonAddresses ON

DECLARE @SRV_PersonAddresses TABLE
(
	[personAddressId] BIGINT NOT NULL,
    [apNumber] INT null,
	[apSuffix] nvarchar(10) null,
    [isEligible] BIT null,
    [personId] BIGINT not null,
    [addressId] BIGINT not null,
    [personAddressTypeId] BIGINT not null,
    [created] DATETIMEOFFSET null,
    [modified] DATETIMEOFFSET null,
    [deleted] DATETIMEOFFSET null,
    [createdById] NVARCHAR(255) null,
    [modifiedById] NVARCHAR(255) null,
    [deletedById] NVARCHAR(255) null
)

INSERT INTO @SRV_PersonAddresses ([personAddressId], [apNumber], [apSuffix], [isEligible], [personId], [addressId], [personAddressTypeId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	(1,  22, null, 1,  1, 1, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(2,  12, null, 1,  2, 2, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(3,  45, null, 1,  3, 3, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(4,   3, null, 1,  4, 4, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(5,  56, null, 1,  5, 5, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(6,  13, null, 1,  6, 6, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(7,  40, null, 1,  7, 6, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(8,  40, null, 1,  8, 6, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(9,  40, null, 1,  9, 6, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	(10, 40, null, 1, 10, 6, 1, SYSDATETIMEOFFSET(), null, null, 1, null, null)
	

MERGE SRV.PersonAddresses AS target
USING
	(
		SELECT [personAddressId], [apNumber], [apSuffix], [isEligible], [personId], [addressId], [personAddressTypeId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_PersonAddresses
	) AS source
ON source.[personAddressId] = target.[personAddressId]
WHEN MATCHED AND (source.[apNumber] <> target.[apNumber]
OR source.[apSuffix] <> target.[apSuffix]
OR source.[isEligible] <> target.[isEligible]
OR source.[personId] <> target.[personId]
OR source.[addressId] <> target.[addressId]
OR source.[personAddressTypeId] <> target.[personAddressTypeId]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[apNumber] = source.[apNumber],
			target.[apSuffix] = source.[apSuffix],
			target.[isEligible] = source.[isEligible],
			target.[personId] = source.[personId],
			target.[addressId] = source.[addressId],
			target.[personAddressTypeId] = source.[personAddressTypeId],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([personAddressId], [apNumber], [apSuffix], [isEligible], [personId], [addressId], [personAddressTypeId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[personAddressId], source.[apNumber], source.[apSuffix], source.[isEligible], source.[personId], source.[addressId], source.[personAddressTypeId], source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);

SET IDENTITY_INSERT SRV.PersonAddresses OFF