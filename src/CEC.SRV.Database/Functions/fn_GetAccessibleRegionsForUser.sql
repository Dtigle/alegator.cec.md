CREATE FUNCTION [SRV].[fn_GetAccessibleRegionsForUser]
(
	@userId nvarchar(255)
)
RETURNS TABLE AS RETURN
(
	WITH Regions AS 
	(
		SELECT r.regionId FROM Lookup.Regions r
		INNER JOIN [SRV].[SRVIdentityUsersRegions] t ON r.regionId = t.regionId
		WHERE t.identityUserId = @userId

		UNION ALL
    
		SELECT emp.regionId
		FROM Lookup.Regions AS emp 
		INNER JOIN Regions AS el ON emp.parentId = el.regionId
		WHERE (emp.parentId IS NOT NULL)
	)
	
	SELECT * FROM Regions

)
