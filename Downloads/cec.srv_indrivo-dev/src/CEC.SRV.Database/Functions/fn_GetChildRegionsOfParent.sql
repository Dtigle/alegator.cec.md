CREATE FUNCTION [SRV].[fn_GetChildRegionsOfParent]
(
	@regionId bigint
)
RETURNS TABLE AS RETURN
(
	WITH Regions AS 
	(
		SELECT r.regionId FROM Lookup.Regions r
		WHERE r.regionId = @regionId

		UNION ALL
    
		SELECT emp.regionId
		FROM Lookup.Regions AS emp 
		INNER JOIN Regions AS el ON emp.parentId = el.regionId
		WHERE (emp.parentId IS NOT NULL)
	)
	
	SELECT * FROM Regions
)
