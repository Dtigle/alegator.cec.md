CREATE FUNCTION [SRV].[fn_GetParentRegionsOfChild]
(
	@regionId bigint
)
RETURNS TABLE AS RETURN
(
	WITH Regions AS 
	(
	    SELECT r.regionId AS parentId FROM Lookup.Regions r
		WHERE r.regionId = @regionId

		UNION ALL
    
		SELECT emp.parentId 
		FROM Lookup.Regions AS emp 
		INNER JOIN Regions AS el ON el.parentId = emp.regionId
		WHERE (emp.parentId IS NOT NULL)
	)
	
	SELECT * FROM Regions
)
