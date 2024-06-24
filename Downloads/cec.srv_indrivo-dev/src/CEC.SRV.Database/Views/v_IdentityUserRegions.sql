CREATE VIEW [SRV].[v_IdentityUserRegions]
WITH SCHEMABINDING

AS

WITH Regions(regionId, identityUserId) AS 
	(
		SELECT t.regionId, t.identityUserId FROM Lookup.Regions r
		INNER JOIN [SRV].[SRVIdentityUsersRegions] t ON r.regionId = t.regionId

		UNION ALL
    
		SELECT emp.regionId, el.identityUserId
		FROM Lookup.Regions AS emp 
		INNER JOIN Regions AS el ON emp.parentId = el.regionId
		WHERE (emp.parentId IS NOT NULL)
	)
	
	SELECT regionId, identityUserId FROM Regions


GO