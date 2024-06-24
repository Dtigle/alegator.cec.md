CREATE VIEW SRV.PersonFullAddress
WITH SCHEMABINDING
AS

SELECT 
	pa.personAddressId, 
	r.regionId, 
	r.hasStreets as RegionHasStreets,
	ps.pollingStationId, 
	pa.personId, 
	pa.isEligible, 
	pa.personAddressTypeId, 
	pa.apNumber, 
	pa.apSuffix, 
	ISNULL(rt.name, '') + ' ' + ISNULL(r.name, '') + ' ' + 
	case when LEFT(s.name, 1) = '$' then '>' else ISNULL(s.name, '') end + ' ' + 
	ISNULL(st.name, '') + ', ' + 
	ISNULL(SRV.fn_BuildingNumberConcat(a.houseNumber, a.suffix), '') + ' -> ' + 
	case when ps.pollingStationId is null 
	then '[N/A]' 
	else
		ISNULL(CONVERT(nvarchar(16), SRV.fn_GetCircumscription(r.regionId)), 'nd') + '/' + 
		ISNULL(ps.number, '') 
	end AS fullAddress,
	pa.dateOfExpiration
FROM SRV.PersonAddresses AS pa 
INNER JOIN SRV.Addresses AS a ON pa.addressId = a.addressId 
INNER JOIN Lookup.Streets AS s ON a.streetId = s.streetId 
LEFT OUTER JOIN SRV.PollingStations AS ps ON a.pollingStationId = ps.pollingStationId 
INNER JOIN Lookup.Regions AS r ON r.regionId = s.regionId 
INNER JOIN Lookup.RegionTypes AS rt ON r.regionTypeId = rt.regionTypeId 
INNER JOIN Lookup.StreetTypes AS st ON s.streetTypeId = st.streetTypeId

GO

--index creation not supported for views non-deterministic invoking UDFs
--CREATE UNIQUE CLUSTERED INDEX IX_PersonFullAddress_PersonAddressId ON SRV.PersonFullAddress(personAddressId)