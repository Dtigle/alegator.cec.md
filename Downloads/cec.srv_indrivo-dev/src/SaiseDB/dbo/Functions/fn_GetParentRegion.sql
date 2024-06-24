CREATE FUNCTION [dbo].[fn_GetParentRegion]
(
	@regionId bigint
)
RETURNS bigint
AS
BEGIN
	DECLARE @Result bigint;

	With CTE as
	(
	SELECT RegionId, ParentId, RegionTypeId FROM [dbo].[Region] WITH(NOLOCK) WHERE RegionId = @regionId
	UNION ALL
	SELECT a.RegionId, a.ParentId, a.RegionTypeId 
	FROM [dbo].[Region] AS a
	INNER JOIN cte b 
	ON b.ParentId = a.RegionId 
	AND a.RegionId<>b.RegionId
	AND a.RegionTypeId > 1
	)
	SELECT TOP 1 @Result = RegionId FROM cte ORDER BY RegionTypeId ASC

	RETURN @Result
END
