CREATE VIEW [SRV].[v_Voters]
	AS 
select 
		ps.pollingStationId, ps.number as PollingStationNumber,
		p.personId, p.idnp, p.firstName, p.surname, p.middleName, p.dateOfBirth, 
		g.name as Gender,
		vb.RegionName, vb.StreetName, vb.houseNumber, vb.suffix as houseSuffix, vb.fullBuildingAddress as EligibleAddress,
		pa.apNumber, pa.apSuffix, SRV.fn_BuildingNumberConcat(pa.apNumber, pa.apSuffix) as fullApNumber,
		p.doc_seria as DocSeria, p.doc_number as DocNumber, 
		pst.name as [Status]
from SRV.People p
inner join Lookup.Genders g on p.genderId = g.genderId
inner join SRV.PersonAddresses pa on p.personId = pa.personId

--inner join Lookup.DocumentTypes dt on p.doc_typeId = dt.documentTypeId
inner join SRV.PersonStatuses stat on p.personId = stat.personId
inner join Lookup.PersonStatusTypes pst on stat.statusTypeId = pst.personStatusTypeId
inner join SRV.v_Buildings vb on pa.addressId = vb.addressId
inner join SRV.PollingStations ps on vb.pollingStationId = ps.pollingStationId
where 
	pa.isEligible = 1
	--and dt.isPrimary = 1
	and pst.isExcludable = 0
	and stat.isCurrent = 1
	and p.deleted is null and pa.deleted is null and ps.deleted is null
