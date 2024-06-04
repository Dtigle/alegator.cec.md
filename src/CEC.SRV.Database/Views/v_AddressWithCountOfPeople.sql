CREATE VIEW [SRV].[v_AddressWithCountOfPeople]
	AS 
select a.addressId, 
(select count(*) from SRV.PersonAddresses pa where pa.addressId = a.addressId and pa.deleted is null and pa.isEligible = 1) as PeopleCount	
    from SRV.Addresses a