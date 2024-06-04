CREATE FUNCTION [SRV].[fn_GetUsersWithAccessToRegion]
(
	@regionId bigint
)
RETURNS TABLE AS RETURN
(
	WITH ChildRegions AS
	(
		SELECT regionId, parentId FROM Lookup.Regions WHERE regionId = @regionId

		UNION ALL

		SELECT r.regionId, r.parentId FROM Lookup.Regions r
		INNER JOIN ChildRegions cr on cr.parentId = r.regionId
		WHERE r.parentId IS NOT NULL
	)

	SELECT DISTINCT iur.identityUserId FROM ChildRegions r
	INNER JOIN [SRV].[SRVIdentityUsersRegions] iur on r.regionId = iur.regionId

	UNION
	--Get all administrators as well
	SELECT iu.identityUserId FROM Access.IdentityUsers iu
	INNER JOIN Access.UserRoles ur ON iu.identityUserId = ur.userId
	INNER JOIN Access.IdentityRoles ir on ir.identityRoleId = ur.roleId
	WHERE ir.name = 'Administrator'
)
