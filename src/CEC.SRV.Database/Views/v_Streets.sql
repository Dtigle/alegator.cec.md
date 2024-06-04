CREATE VIEW [SRV].[v_Streets]
	AS 
SELECT        Lookup.Streets.regionId, Lookup.RegionTypes.name + ' ' + Lookup.Regions.name AS RegionName, Lookup.Streets.streetId, 
                         Lookup.Streets.name AS streetName, Lookup.StreetTypes.name AS streetType, 
						 case when LEFT(Lookup.Streets.name, 1) = '$' then N'< str.' else Lookup.Streets.name + ' ' + Lookup.StreetTypes.name end AS fullName
FROM            Lookup.RegionTypes INNER JOIN
                         Lookup.Regions ON Lookup.RegionTypes.regionTypeId = Lookup.Regions.regionTypeId INNER JOIN
                         Lookup.Streets INNER JOIN
                         Lookup.StreetTypes ON Lookup.Streets.streetTypeId = Lookup.StreetTypes.streetTypeId ON Lookup.Regions.regionId = Lookup.Streets.regionId
WHERE        (Lookup.Streets.deleted IS NULL)
