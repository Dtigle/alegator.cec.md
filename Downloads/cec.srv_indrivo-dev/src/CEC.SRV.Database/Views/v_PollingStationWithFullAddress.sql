CREATE VIEW [SRV].[v_PollingStationWithFullAddress]
	AS 
SELECT pollingStationId,
(SELECT ISNULL(rt.name, '') + ' ' + ISNULL(r.name, '') + ', ' +   case when LEFT(s.name, 1) = '$' then '>' else ISNULL(s.name, '') end + ' ' +  ISNULL(st.name, '')+ ', ' + ISNULL(SRV.fn_BuildingNumberConcat(a.houseNumber, a.suffix), '')
   FROM SRV.Addresses a, Lookup.Regions r, Lookup.RegionTypes rt, Lookup.Streets s, Lookup.StreetTypes st
  WHERE a.addressId = p.pollingStationAddressId 
    AND r.regionId = p.regionId
    AND r.regionTypeId = rt.regionTypeId 
    AND a.streetId = s.streetId 
	AND s.streetTypeId = st.streetTypeId
  )  AS fullAddress,
(SELECT count(*) 
   FROM SRV.Addresses a 
  WHERE p.pollingStationId = a.pollingStationId) AS totalAddress
   FROM SRV.PollingStations p