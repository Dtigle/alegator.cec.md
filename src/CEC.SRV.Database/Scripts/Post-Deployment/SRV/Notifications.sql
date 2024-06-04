SET IDENTITY_INSERT SRV.Notifications ON

DECLARE @SRV_Notifications TABLE
(
	[notificationId] BIGINT NULL,
	[messageBody] nvarchar(255) not null,
	[eventId] BIGINT not null,
	[created] DATETIMEOFFSET null,
    [modified] DATETIMEOFFSET null,
    [deleted] DATETIMEOFFSET null,
    [createdById] NVARCHAR(255) null,
    [modifiedById] NVARCHAR(255) null,
    [deletedById] NVARCHAR(255) null
)

INSERT INTO @SRV_Notifications ([notificationId], [messageBody], [eventId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	( 1, N'O nouă stradă a fost creată', 1, SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 2, N'O nouă adresă a fost creată', 2, SYSDATETIMEOFFSET(), null, null, 1, null, null)
	
MERGE SRV.Notifications AS target
USING
	(
		SELECT [notificationId], [messageBody], [eventId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_Notifications
	) AS source
ON source.[notificationId] = target.[notificationId]
WHEN MATCHED AND (source.[messageBody] <> target.[messageBody]
OR source.[eventId] <> target.[eventId]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[messageBody] = source.[messageBody],
			target.[eventId] = source.[eventId],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([notificationId], [messageBody], [eventId], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[notificationId], source.[messageBody], source.[eventId], 
		source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);
SET IDENTITY_INSERT SRV.Notifications OFF