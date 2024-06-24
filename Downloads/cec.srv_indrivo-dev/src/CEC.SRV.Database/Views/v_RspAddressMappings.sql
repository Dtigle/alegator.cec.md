CREATE VIEW [Importer].[v_RspAddressMappings]
	AS 
	select 
		ma.mappingAddressId,
		aps.regionId as srvRegionId, 
		aps.addressId as srvAddressId, 
		aps.fullAddress as srvFullAddress, 
		ma.rspAdministrativeCode,
		r.regionId as rspRegionId,
		ma.rspStreetCode,
		stc.name as rspStreetName,
		ma.rspHouseNr as rspHouseNumber,
		ma.rspHouseSuf as rspHouseSuffix
	from Importer.MappingAddresses ma
	inner join SRV.v_AddressWithPollingStations aps on ma.srvAddressId = aps.addressId
	left outer join RSP.StreetTypeCodes stc on ma.rspStreetCode = stc.rspStreetTypeCodeId
	left outer join Lookup.Regions r on ma.rspAdministrativeCode = r.statisticIdentifier
