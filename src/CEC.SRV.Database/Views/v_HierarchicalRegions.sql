CREATE VIEW [Lookup].[v_HierarchicalRegions]
	AS 
with RegionHierarchy(RegionName, RegionId, Level, FullyQualifiedName, description, parentId, regionTypeId, saiseId, registruId, hasStreets, created, modified, deleted, createdById, modifiedById, deletedById)
as
(
	select cast(rt.name +N' ' + r.name as nvarchar(max)) as RegionName, 
			r.regionId, 
			0, 
			cast(' / '+rt.name +N' ' + r.name as nvarchar(max)),
			r.description, r.parentId, r.regionTypeId, r.saiseId, r.statisticIdentifier as registruId, 
			r.hasStreets, r.created, r.modified, r.deleted, r.createdById, r.modifiedById, r.deletedById 
	from Lookup.Regions r
	inner join Lookup.RegionTypes rt on r.regionTypeId = rt.regionTypeId
	where r.parentId is null

	union all

	select cast(rt.name +N' ' + r.name as nvarchar(max)) as RegionName, 
			r.regionId, 
			rh.Level+1, 
			rh.FullyQualifiedName + ' / ' + rt.name +N' ' + r.name,
			r.description, r.parentId, r.regionTypeId, r.saiseId, r.statisticIdentifier as registruId, 
			r.hasStreets, r.created, r.modified, r.deleted, r.createdById, r.modifiedById, r.deletedById  
	from Lookup.Regions r
	inner join Lookup.RegionTypes rt on r.regionTypeId = rt.regionTypeId
	inner join RegionHierarchy rh on r.parentId = rh.RegionId
)

select Space(Level*4) + rh.RegionName as HierarchyView, 
		rh.FullyQualifiedName,
		rh.RegionId,
		rh.Level,
		rh.description, rh.parentId, rh.regionTypeId, rh.saiseId, rh.registruId, 
		rh.hasStreets, rh.created, rh.modified, rh.deleted, rh.createdById, rh.modifiedById, rh.deletedById 
  from RegionHierarchy rh

