CREATE VIEW [SRV].[v_StreetWithCountOfAddresses]
	AS 
select 
	st.streetId ,
	(select count(*) from SRV.Addresses a where a.streetId = st.streetId and a.deleted is null) as HousesCount	
from Lookup.Streets st