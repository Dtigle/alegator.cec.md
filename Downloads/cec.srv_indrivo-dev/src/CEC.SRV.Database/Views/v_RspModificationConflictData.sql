CREATE VIEW Importer.[v_RspModificationConflictData]
	AS 
	SELECT        imp.rspModificationDataId, reg.rspRegistrationDataId, imp.idnp, imp.lastName, imp.firstName, imp.secondName, imp.birthdate, imp.sexCode, imp.dead, imp.citizenRm, imp.doctypecode, imp.series, imp.number, imp.issuedate, 
                         imp.expirationdate, imp.validity, reg.regTypeCode, reg.region, reg.locality, reg.administrativecode, reg.streetName, reg.streetcode, reg.houseNr, reg.houseSuffix, reg.apNr, reg.apSuffix, reg.dateOfExpiration, 
                         reg.dateOfRegistration, imp.statusConflictCode, imp.acceptConflictCode, imp.rejectConflictCode, imp.source, imp.[status], imp.statusMessage, imp.created, imp.statusDate, imp.comments, rwl.regionId, reg.isInConflict
FROM            Importer.[RspModificationDatas] AS imp LEFT JOIN
                         Importer.RspRegistrationDatas AS reg ON reg.rspModificationDataId = imp.rspModificationDataId LEFT JOIN
                             (SELECT        rwl.statisticIdentifier, max(rwl. LEVEL) lvl
                               FROM            Lookup.RegionWithLevel rwl
                               GROUP BY rwl.statisticIdentifier) maxlevel ON reg.administrativecode = maxlevel.statisticIdentifier LEFT OUTER JOIN
                         Lookup.RegionWithLevel AS rwl ON maxlevel.statisticIdentifier = rwl.statisticIdentifier AND maxlevel.lvl = rwl.[level]
WHERE        imp.statusConflictCode <> 0 AND statusConflictCode <> (isnull(imp.acceptConflictCode, 0) + isnull(imp.rejectConflictCode, 0)) AND (reg.isInConflict = 1 OR
                         imp.statusConflictCode <> 0)
