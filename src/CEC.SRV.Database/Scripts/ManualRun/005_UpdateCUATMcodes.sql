update Importer.RspRegistrationDatas 
set 
	administrativecode=M.statisticIdentifier
from 
	Importer.RspRegistrationDatas
inner join 
(
	select rspRegistrationDataId, administrativecode, r.statisticIdentifier from Importer.RspRegistrationDatas m
	join 
	(
		select registruId, max(statisticIdentifier) as 'statisticIdentifier' from Lookup.Regions group by registruId
	) as r
	on m.administrativecode = r.registruId
	where administrativecode > 1000000
) M
on 
	Importer.RspRegistrationDatas.rspRegistrationDataId = M.rspRegistrationDataId


update Importer.MappingAddresses 
set 
	rspAdministrativeCode=M.statisticIdentifier
from 
	Importer.MappingAddresses
inner join 
(
	select mappingAddressId, rspAdministrativeCode, r.statisticIdentifier from Importer.MappingAddresses m
	join 
	(
		select registruId, max(statisticIdentifier) as 'statisticIdentifier' from Lookup.Regions group by registruId
	) as r
	on m.rspAdministrativeCode = r.registruId
	where rspAdministrativeCode > 1000000 and r.statisticIdentifier is not null
) M
on 
	Importer.MappingAddresses.mappingAddressId = M.mappingAddressId


update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=9281 where administrativecode=2892758--PETREȘTI LOC.ST.C.F.
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=9639 where administrativecode=2834722--ETULIA LOC.ST.C.F.
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=9637 where administrativecode=2834701--VULCĂNEȘTI LOC.ST.C.F.
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=2341 where administrativecode=2845701--CĂINARI LOC.ST.C.F.
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=2733 where administrativecode=2855720--ZAIM LOC.ST.C.F.


update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=9281 where rspAdministrativeCode=2892758--PETREȘTI LOC.ST.C.F.
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=9639 where rspAdministrativeCode=2834722--ETULIA LOC.ST.C.F.
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=9637 where rspAdministrativeCode=2834701--VULCĂNEȘTI LOC.ST.C.F.
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=2341 where rspAdministrativeCode=2845701--CĂINARI LOC.ST.C.F.
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=2733 where rspAdministrativeCode=2855720--ZAIM LOC.ST.C.F.


-- UTA TRANSNISTRIA
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=1915 where administrativecode=2853735
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=1945 where administrativecode= 2853702
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=5021 where administrativecode= 2838727
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=6928 where administrativecode= 2878761
update [CEC.SRV].Importer.RspRegistrationDatas set administrativecode=6942 where administrativecode= 2878781

update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=1915 where rspAdministrativeCode= 2853735
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=1945 where rspAdministrativeCode= 2853702
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=5021 where rspAdministrativeCode= 2838727
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=6928 where rspAdministrativeCode= 2878761
update [CEC.SRV].Importer.MappingAddresses set rspAdministrativeCode=6942 where rspAdministrativeCode= 2878781


delete from Importer.MappingAddresses 
where srvAddressId in (
 select a.addressId from SRV.Addresses a where a.deleted is not null
)