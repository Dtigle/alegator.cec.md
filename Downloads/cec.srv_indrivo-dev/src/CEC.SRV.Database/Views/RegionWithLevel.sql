CREATE VIEW [Lookup].[RegionWithLevel]
AS

WITH RegionWithLevels AS 
(
		SELECT        regionId, name, description, parentId, regionTypeId, saiseId, registruId, statisticIdentifier, hasStreets, created, modified, deleted, createdById, modifiedById, deletedById, 1 AS [level]
        FROM            Lookup.Regions
        WHERE        (parentId IS NULL)
        UNION ALL
        SELECT        emp.regionId, emp.name, emp.description, emp.parentId, emp.regionTypeId, emp.saiseId, emp.registruId,emp.statisticIdentifier, emp.hasStreets, emp.created, emp.modified, emp.deleted, 
                                emp.createdById, emp.modifiedById, emp.deletedById, el.[level] + 1 AS Expr1
        FROM            Lookup.Regions AS emp INNER JOIN
                                RegionWithLevels AS el ON emp.parentId = el.regionId
        WHERE        (emp.parentId IS NOT NULL)
)
    SELECT        regionId, name, description, parentId, regionTypeId, saiseId, registruId, statisticIdentifier, hasStreets, created, modified, deleted, createdById, modifiedById, deletedById, [level]
     FROM            RegionWithLevels AS RegionWithLevels_1
