GO

-- SELECT Alegator Polling Stations which does not match veilare from SRV
SELECT * 
FROM SAISE.dbo.PollingStation lp
LEFT JOIN Lookup.Regions r on lp.VillageId = r.saiseId
WHERE r.regionId is null


-- INSERT Polling stations FROM Alegator to SRV

INSERT INTO SRV.PollingStations(number, subNumber, saiseId, location, geolatitude, geolongitude, regionId, created, createdById)
SELECT lp.Number, lp.SubNumber, lp.PollingStationId, lp.NameRo, lp.LocationLatitude, lp.LocationLongitude, r.regionId, SYSDATETIMEOFFSET(), 1 

FROM SAISE.dbo.PollingStation lp
INNER JOIN Lookup.Regions r on lp.VillageId = r.saiseId
