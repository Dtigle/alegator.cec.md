CREATE VIEW SRV.[v_AddressWithPollingStations]
	AS 
SELECT        a.addressId, s.regionId, a.streetId, a.pollingStationId, s.RegionName, 
                         s.fullName AS StreetName, a.houseNumber, a.suffix, 
						 SRV.fn_BuildingNumberConcat(a.houseNumber, a.suffix) AS buildingNumber, 
						ISNULL(rt.name, '') + ' ' + ISNULL(r.name, '') + ' ' + 
						case when LEFT(s.streetName, 1) = '$' then '>' else ISNULL(s.streetName, '') end + ' ' + 
						ISNULL(s.streetType, '') + ', ' + 
						ISNULL(SRV.fn_BuildingNumberConcat(a.houseNumber, a.suffix), '') + ' -> ' + 
						case when ps.pollingStationId is null 
						then '[N/A]' 
						else
							ISNULL(CONVERT(nvarchar(16), SRV.fn_GetCircumscription(r.regionId)), 'nd') + '/' + 
							ISNULL(ps.number, '') 
						end AS fullAddress
FROM            
		SRV.Addresses a INNER JOIN
        SRV.v_Streets s ON a.streetId = s.streetId
		LEFT OUTER JOIN SRV.PollingStations AS ps ON a.pollingStationId = ps.pollingStationId 
		INNER JOIN Lookup.Regions AS r ON r.regionId = s.regionId 
		INNER JOIN Lookup.RegionTypes AS rt ON r.regionTypeId = rt.regionTypeId 
WHERE        (a.deleted IS NULL)
