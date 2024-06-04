CREATE VIEW Importer.[v_RspModificationConflictDataAdmin]
	AS SELECT 
	imp.rspModificationDataId, 
	reg.rspRegistrationDataId,
	imp.idnp,
	imp.lastName,
	imp.firstName,
	imp.secondName,
	imp.birthdate,
	imp.sexCode,
	imp.dead,
	imp.citizenRm,
	imp.doctypecode,
	imp.series,
	imp.number,
	imp.issuedate,
	imp.expirationdate,
	imp.validity,
	reg.regTypeCode,
	reg.region,
	reg.locality,
	reg.administrativecode,
	reg.streetName,
	reg.streetcode,
	reg.houseNr,
	reg.houseSuffix,
	reg.apNr,
	reg.apSuffix,
	reg.dateOfExpiration,
	reg.dateOfRegistration,
	imp.statusConflictCode,
	imp.acceptConflictCode,
	imp.rejectConflictCode,
	imp.source,
	imp.[status],
	imp.statusMessage,
	imp.created,
	imp.statusDate,
	imp.comments,  
	rwl.regionId,
	reg.isInConflict
	FROM Importer.[RspModificationDatas] as imp
	left join Importer.RspRegistrationDatas as reg on reg.rspModificationDataId = imp.rspModificationDataId
	left join (select rwl.registruId, max(rwl.level) lvl
		from Lookup.RegionWithLevel rwl
		group by rwl.registruId) maxlevel on reg.administrativecode = maxlevel.registruId
	left outer join Lookup.RegionWithLevel as rwl on maxlevel.registruId = rwl.registruId and maxlevel.lvl = rwl.[level]
	where imp.statusConflictCode <> 0
	and statusConflictCode <> isnull(imp.acceptConflictCode, 0)
	and (reg.isInConflict = 1 or imp.statusConflictCode <> 0) 
