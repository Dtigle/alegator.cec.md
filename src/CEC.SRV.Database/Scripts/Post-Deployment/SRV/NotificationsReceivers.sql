SET IDENTITY_INSERT SRV.NotificationReceivers ON

DECLARE @SRV_NotificationReceivers TABLE
(
	[notificationReceiverId] BIGINT NULL,
    [notificationId] BIGINT NOT null,
    [identityUserId] NVARCHAR(255) not null,
    [notificationIsRead] BIT not null,
    [created] DATETIMEOFFSET null,
    [modified] DATETIMEOFFSET null,
    [deleted] DATETIMEOFFSET null,
    [createdById] NVARCHAR(255) null,
    [modifiedById] NVARCHAR(255) null,
    [deletedById] NVARCHAR(255) null
)

INSERT INTO @SRV_NotificationReceivers ([notificationReceiverId], [notificationId], [identityUserId], [notificationIsRead], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
VALUES
	( 1, 1, 2, 0 ,SYSDATETIMEOFFSET(), null, null, 1, null, null),
	( 2, 2, 2, 0, SYSDATETIMEOFFSET(), null, null, 1, null, null)
	
MERGE SRV.NotificationReceivers AS target
USING
	(
		SELECT [notificationReceiverId], [notificationId], [identityUserId], [notificationIsRead], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById]
		FROM @SRV_NotificationReceivers
	) AS source
ON source.[notificationReceiverId] = target.[notificationReceiverId]
WHEN MATCHED AND (source.[notificationId] <> target.[notificationId]
OR source.[identityUserId] <> target.[identityUserId]
OR source.[notificationIsRead] <> target.[notificationIsRead]
OR source.[created] <> target.[created]
OR source.[modified] <> target.[modified]
OR source.[deleted] <> target.[deleted]
OR source.[createdById] <> target.[createdById]
OR source.[modifiedById] <> target.[modifiedById]
OR source.[deletedById] <> target.[deletedById])
	THEN 
		UPDATE SET 
			target.[notificationId] = source.[notificationId],
			target.[identityUserId] = source.[identityUserId],
			target.[notificationIsRead] = source.[notificationIsRead],
			target.[created] = source.[created],
			target.[modified] = source.[modified],
			target.[deleted] = source.[deleted],
			target.[createdById] = source.[createdById],
			target.[modifiedById] = source.[modifiedById],
			target.[deletedById] = source.[deletedById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([notificationReceiverId], [notificationId], [identityUserId], [notificationIsRead], [created], [modified], [deleted], [createdById], [modifiedById], [deletedById])
		VALUES (source.[notificationReceiverId], source.[notificationId], source.[identityUserId], source.[notificationIsRead],
		source.[created], source.[modified], source.[deleted], source.[createdById], source.[modifiedById], source.[deletedById]);
SET IDENTITY_INSERT SRV.NotificationReceivers OFF