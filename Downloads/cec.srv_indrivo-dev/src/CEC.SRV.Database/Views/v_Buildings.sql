CREATE VIEW [SRV].[v_Buildings]
	AS 
SELECT        SRV.Addresses.addressId, SRV.v_Streets.regionId, SRV.Addresses.streetId, SRV.Addresses.pollingStationId, SRV.v_Streets.RegionName, 
                         SRV.v_Streets.fullName AS StreetName, SRV.Addresses.houseNumber, SRV.Addresses.suffix, SRV.fn_BuildingNumberConcat(SRV.Addresses.houseNumber, 
                         SRV.Addresses.suffix) AS buildingNumber, SRV.fn_GetFullAddress(SRV.v_Streets.fullName, SRV.Addresses.houseNumber, SRV.Addresses.suffix) 
                         AS fullBuildingAddress
FROM            SRV.Addresses INNER JOIN
                         SRV.v_Streets ON SRV.Addresses.streetId = SRV.v_Streets.streetId
WHERE        (SRV.Addresses.deleted IS NULL)

