SET IDENTITY_INSERT SRV.PersonStatuses ON

DECLARE @SRV_PersonStatuses TABLE
(
    personStatusId BIGINT not null,
	personId BIGINT not null,
    statusTypeId BIGINT not null,
    confirmation NVARCHAR(255) not null,
	isCurrent BIT not null,
    [created] DATETIMEOFFSET null,
    [modified] DATETIMEOFFSET null,
    [deleted] DATETIMEOFFSET null,
    [createdById] NVARCHAR(255) null,
    [modifiedById] NVARCHAR(255) null,
    [deletedById] NVARCHAR(255) null
)

INSERT INTO @SRV_PersonStatuses ([personStatusId], [personId], statusTypeId, confirmation, isCurrent, [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	( 1, 1, 1, N'Import', 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 2, 2, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 3, 3, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 4, 4, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 5, 5, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 6, 6, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 7, 7, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 8, 8, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 9, 9, 1, N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 10, 10, 1,N'Import', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null)
	
MERGE SRV.PersonStatuses AS target
USING
	(
		SELECT [personStatusId], [personId], statusTypeId, confirmation, isCurrent, [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_PersonStatuses
	) AS source
ON source.[personStatusId] = target.[personStatusId]
WHEN MATCHED AND (source.[personId] <> target.[personId]
OR source.statusTypeId <> target.statusTypeId
OR source.[confirmation] <> target.[confirmation]
OR source.[isCurrent] <> target.[isCurrent]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[personId] = source.[personId],
			target.statusTypeId = source.statusTypeId,
			target.[confirmation] = source.[confirmation],
			target.[isCurrent] = source.[isCurrent],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([personStatusId], [personId], statusTypeId, confirmation, isCurrent, [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[personStatusId], source.[personId], source.statusTypeId, source.confirmation, source.isCurrent, source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);
SET IDENTITY_INSERT SRV.PersonStatuses OFF