DECLARE @Access_IdentityUsers TABLE
(
	[identityUserId] [nvarchar](255) NOT NULL,
	[userName] [nvarchar](255) NOT NULL,
	[passwordHash] [nvarchar](255) NULL,
	[securityStamp] [nvarchar](255) NULL,
	[displayName] [nvarchar](255) NULL,
	[preferredEmail] [nvarchar](255) NULL,
	[isBuiltIn] BIT NOT NULL,
	[isBlocked] BIT NOT NULL ,
	[created] [datetimeoffset](7) null,
	[createdById] [bigint] null
)

INSERT INTO @Access_IdentityUsers ([identityUserId], [userName], [passwordHash], [securityStamp], [displayName], [preferredEmail],[isBuiltIn], [isBlocked], [created], [createdById])
VALUES
	(1, N'System', null, null, null, null, 1, 1, SYSDATETIMEOFFSET(), 1),
	(2, N'Administrator', N'ACAw6tdsCa1nYgJufn7dwof8NvdyfBAAc73zJjDX+fxPSKQ2o4Nc3/dzQUbQRz9mIw==', N'59dbd888-da41-450c-9b5a-fab00f74ac3c', null, null, 1, 0, SYSDATETIMEOFFSET(), 1)
	

MERGE Access.IdentityUsers AS target
USING
	(
		SELECT [identityUserId], [userName], [passwordHash], [securityStamp]
		FROM @Access_IdentityUsers
	) AS source
ON source.[identityUserId] = target.[identityUserId]
WHEN MATCHED AND (source.[userName] <> target.[userName]
OR source.[passwordHash] <> target.[passwordHash]
OR source.[securityStamp] <> target.[securityStamp])
	THEN 
		UPDATE SET 
			target.[userName] = source.[userName],
			target.[passwordHash] = source.[passwordHash],
			target.[securityStamp] = source.[securityStamp]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([identityUserId], [userName], [passwordHash], [securityStamp])
		VALUES (source.[identityUserId], source.[userName], source.[passwordHash], source.[securityStamp]);

MERGE SRV.SRVIdentityUsers AS target
USING
	(
		SELECT [identityUserId], [isBuiltIn], [isBlocked], [created], [createdById]
		FROM @Access_IdentityUsers
	) AS source
ON source.[identityUserId] = target.[identityUserId]
WHEN MATCHED AND (source.[isBuiltIn] <> target.[isBuiltIn]
OR source.[isBlocked] <> target.[isBlocked])
	THEN 
		UPDATE SET 
			target.[isBuiltIn] = source.[isBuiltIn],
			target.[isBlocked] = source.[isBlocked],
			target.[created] = source.[created],
			target.[createdById] = source.[createdById]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([identityUserId], [isBuiltIn], [isBlocked], [created], [createdById])
		VALUES (source.[identityUserId], source.[isBuiltIn], source.[isBlocked], source.[created], source.[createdById]);