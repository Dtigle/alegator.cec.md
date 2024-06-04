CREATE VIEW [SRV].[v_LinkedRegions]
	AS 
select lr.linkedRegionId, 
		lrr.regionId, 
		FullyQualifiedName = (select hr.FullyQualifiedName from Lookup.v_HierarchicalRegions as hr 
				where lrr.regionId = hr.RegionId ) 
from SRV.LinkedRegions as lr
inner join SRV.LinkedRegions_Region as lrr on lrr.linkedRegionId = lr.linkedRegionId