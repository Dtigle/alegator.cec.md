CREATE VIEW [Lookup].[v_RegionWithoutSaiseId]
	AS 
select distinct r.* from SRV.PersonAddresses pa
 join SRV.Addresses a on pa.addressId=a.addressId
 join Lookup.Streets s on s.streetId = a.streetId
 join Lookup.v_HierarchicalRegions r on s.regionId = r.RegionId
 where pa.isEligible=1 and pa.deleted is null and r.saiseId is null
