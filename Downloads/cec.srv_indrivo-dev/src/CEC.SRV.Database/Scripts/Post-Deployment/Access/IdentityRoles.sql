
DECLARE @Access_IdentityRoles TABLE
(
	[identityRoleId] [nvarchar] (255) NOT NULL,
	[name] [nvarchar](255) NOT NULL
)

INSERT INTO @Access_IdentityRoles ([identityRoleId], [name])
VALUES
	(1, N'Administrator'),
	(2, N'Registrator')

MERGE Access.IdentityRoles AS target
USING
	(
		SELECT [identityRoleId], [name]
		FROM @Access_IdentityRoles
	) AS source
ON source.[identityRoleId] = target.[identityRoleId]
WHEN MATCHED AND (source.[name] <> target.[name])
	THEN 
		UPDATE SET 
			target.[name] = source.[name]
WHEN NOT MATCHED BY target
	THEN
		INSERT ([identityRoleId], [name])
		VALUES (source.[identityRoleId], source.[name])
WHEN NOT MATCHED BY source
	THEN DELETE;
