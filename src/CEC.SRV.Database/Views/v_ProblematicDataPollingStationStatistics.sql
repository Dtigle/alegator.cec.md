CREATE VIEW [SRV].[v_ProblematicDataPollingStationStatistics]
	AS 

with RegionHierarchy(RegionId,FullRegionName)
as
(
	select r.regionId, 		
			cast(' / '+rt.name +N' ' + r.name as nvarchar(max))	
	from Lookup.Regions r
	inner join Lookup.RegionTypes rt on r.regionTypeId = rt.regionTypeId		

	where r.parentId is null

	union all

	select	r.regionId,	rh.FullRegionName + ' / ' + rt.name +N' ' + r.name
	from Lookup.Regions r
	inner join Lookup.RegionTypes rt on r.regionTypeId = rt.regionTypeId
	inner join RegionHierarchy rh on r.parentId = rh.RegionId   	
)

select p.pollingStationId ,rh.RegionId, rh.FullRegionName,	convert(varchar(5), srv.fn_GetCircumscription(rh.RegionId))+'/'+ p.number as PollingStation, (select count(*) from SRV.v_Voters v where v.pollingStationId = p.pollingStationId) as VotersCount
  from SRV.PollingStations p
  inner join RegionHierarchy rh on p.regionId = rh.RegionId 
  inner join ( select number, regionId from SRV.PollingStations where deleted is null  group by number, regionId having count(*) > 1 ) a on a.number = p.number and a.regionId = p.regionId
  where p.deleted is null
  

