CREATE PROCEDURE [dbo].[CEC_AssignedPollingStation] 
    @ElectionId int
AS
BEGIN
	SET NOCOUNT ON;
	
	select  aps.AssignedPollingStationId
			,aps.ElectionId
			,aps.OpeningVoters
			,ps.PollingStationId
			,d.number
			,ps.number
			,ps.LocationLatitude
			,ps.LocationLongitude
			,(select count(*) from [dbo].[AssignedVoter] where ElectionId = aps.ElectionId and PollingStationId = aps.PollingStationId and [Status] >= 5000 and [Status] < 9000) as VotersReceivedBallots
			,(select count(*) from [dbo].[AssignedVoter] where ElectionId = aps.ElectionId and PollingStationId = aps.PollingStationId and [Status] in (5001,5002)) as VotersInSupplimentaryList
			,d.NameRo as Circumscription
			,v.NameRo as Locality
	from [dbo].[AssignedPollingStation] aps
	inner join [dbo].[PollingStation] ps on aps.PollingStationId = ps.PollingStationId
	inner join [dbo].Village v on ps.VillageId = v.VillageId
	inner join [dbo].[District] d on v.DistrictId = d.DistrictId
	where [ElectionId] = @ElectionId

	order by d.number, ps.[Number]
	
	 
END