CREATE FUNCTION [SRV].[fn_GetCircumscription]
(
	@regionId bigint
)
RETURNS INT 
WITH SCHEMABINDING
AS 
BEGIN
	DECLARE @circumscription int;
	
	WITH CIRCUMSCRIPTIONS( regionId, circumscription) AS
	(
		SELECT r.regionId, r.circumscription FROM Lookup.Regions r
		WHERE r.regionId = @regionId

		UNION ALL
    
		SELECT emp.parentId, emp.circumscription 
		FROM Lookup.Regions AS emp 
		INNER JOIN CIRCUMSCRIPTIONS AS el ON el.regionId = emp.regionId
		WHERE (emp.parentId IS NOT NULL)
	
	)

	SELECT TOP 1 @circumscription = circumscription FROM CIRCUMSCRIPTIONS where circumscription is not null

	RETURN @circumscription
END
GO