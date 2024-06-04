CREATE VIEW [dbo].[v_UserActivity]
	AS select 
	su.ElectionId,
	su.SystemUserId,
	su.UserName as Utilizator,
	d.Number as Circumscriptie,
	ps.Number as Nr_SV,
	d.NameRo as Raion,
	v.NameRo as Nume_Localitate,
	ProcesatAlegatori = (select count(*) from AssignedVoter av	
			where av.EditUserId = su.SystemUserId and av.ElectionId = su.ElectionId 
					and av.PollingStationId = su.PollingStationId)
	,UltimaEditare = (select max(av.EditDate) from AssignedVoter av	
						where av.EditUserId = su.SystemUserId and av.ElectionId = su.ElectionId and av.PollingStationId = su.PollingStationId)
from SystemUser su
inner join AssignedPollingStation aps on su.PollingStationId = aps.PollingStationId and su.ElectionId=aps.ElectionId
inner join PollingStation ps on aps.PollingStationId = ps.PollingStationId
inner join Village v on ps.VillageId = v.VillageId
inner join District d on v.DistrictId = d.DistrictId

GO
