


IF NOT EXISTS ( SELECT 1 FROM Importer.RspRegistrationDatas)
BEGIN


INSERT INTO Importer.RspRegistrationDatas(dateOfExpiration,	regTypeCode, region, locality, administrativecode, streetcode, houseNr, houseSuffix,
	apNr, apSuffix, dateOfRegistration, rspModificationDataId)

SELECT 
	rm.dateOfExpiration,
	rm.regTypeCode,
	rm.region,
	rm.locality,
	rm.administrativecode,
	rm.streetcode,
	rm.houseNr,
	rm.houseSuffix,
	rm.apNr,
	rm.apSuffix,
	rm.dateOfRegistration,
	rm.rspModificationDataId

FROM Importer.RspModificationDatas rm
LEFT JOIN Importer.RspRegistrationDatas rr ON rm.rspModificationDataId = rr.rspModificationDataId
WHERE rr.rspRegistrationDataId IS NULL


END
