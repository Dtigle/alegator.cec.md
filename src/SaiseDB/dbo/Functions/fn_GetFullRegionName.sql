
CREATE FUNCTION [dbo].[fn_GetFullRegionName] 
(
	@RegionId BIGINT
)
RETURNS nvarchar(1000)
AS
BEGIN
DECLARE @Result nvarchar(1000);
;WITH cte AS (
 SELECT
   r.name
 , rt.name as typeName
 , r.regionId
 , r.regionTypeId
 , r.parentId
 , 1 RegionLevel
 FROM 
 Region r,
 RegionType rt 
 WHERE 
 r.RegionId = @RegionId
 AND rt.regionTypeId = r.regionTypeId
 UNION ALL
 SELECT
   c.name
 , rt.name as typeName
 , c.regionId 
 , c.regionTypeId
 , c.parentId
 , p.RegionLevel + 1
 FROM Region c
 INNER JOIN RegionType rt  ON rt.regionTypeId = c.regionTypeId
 JOIN cte p ON p.ParentId = c.RegionId and c.RegionId <> 1
 )
SELECT @Result = STUFF((SELECT CONCAT( ' / ' , cte.typeName, ' ', cte.Name) FROM cte ORDER BY cte.RegionLevel desc FOR XML PATH ('')), 1, 3, '');

RETURN @Result

END
