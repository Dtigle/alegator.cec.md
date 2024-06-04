SET IDENTITY_INSERT SRV.[Events] ON

DECLARE @SRV_Events TABLE
(
	[eventId] BIGINT NULL,
	[eventType] INT not null,
	[entityType] NVARCHAR(255) not null,
	[entityId] BIGINT not null,
	[created] DATETIMEOFFSET null,
    [modified] DATETIMEOFFSET null,
    [deleted] DATETIMEOFFSET null,
    [createdById] NVARCHAR(255) null,
    [modifiedById] NVARCHAR(255) null,
    [deletedById] NVARCHAR(255) null
)

INSERT INTO @SRV_Events ([eventId], [eventType], [entityType], [entityId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	( 1, 1,  N'Street', 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 2, 1, N'Address', 1,  SYSDATETIMEOFFSET(), null, null, 1, null, null)
	
MERGE SRV.[Events] AS target
USING
	(
		SELECT [eventId], [eventType], [entityType], [entityId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_Events
	) AS source
ON source.[eventId] = target.[eventId]
WHEN MATCHED AND (source.[eventType] <> target.[eventType]
OR source.[entityType] <> target.[entityType]
OR source.[entityId] <> target.[entityId]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[eventType] = source.[eventType],
			target.[entityType] = source.[entityType],
			target.[entityId] = source.[entityId],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([eventId], [eventType], [entityType], [entityId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[eventId], source.[eventType], source.[entityType], source.[entityId],
		source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);
SET IDENTITY_INSERT SRV.[Events] OFF