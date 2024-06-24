DECLARE @Access_UserRoles TABLE
(
	[roleId] [nvarchar](255) NOT NULL,
	[userId] [nvarchar](255) NOT NULL
)

INSERT INTO @Access_UserRoles ([roleId], [userId])
VALUES
	(1, 2)
	

MERGE Access.UserRoles AS target
USING
	(
		SELECT [roleId], [userId]
		FROM @Access_UserRoles
	) AS source
ON source.[roleId] = target.[roleId]
AND source.[userId] = target.[userId]

WHEN NOT MATCHED BY target
	THEN
		INSERT ([roleId], [userId])
		VALUES (source.[roleId], source.[userId]);