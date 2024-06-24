CREATE VIEW [SRV].[v_PollingStationStatistics]
	AS 
select 
		t.pollingStationId, 
		r.regionId,
		rt.name + ' ' + r.name as RegionName, 
		convert(varchar(5), srv.fn_GetCircumscription(r.regionId))+'/'+ ps1.number as PollingStation, 
t.votersCount from
(
select 
	ps.pollingStationId,
	votersCount = (select count(*) from srv.v_Voters v where v.pollingStationId = ps.pollingStationId)
from srv.PollingStations ps
group by  ps.pollingStationId) t
inner join srv.PollingStations ps1 on t.pollingStationId = ps1.pollingStationId
inner join Lookup.Regions r on ps1.regionId = r.regionId
inner join Lookup.RegionTypes rt on r.regionTypeId = rt.regionTypeId
where ps1.deleted is null

