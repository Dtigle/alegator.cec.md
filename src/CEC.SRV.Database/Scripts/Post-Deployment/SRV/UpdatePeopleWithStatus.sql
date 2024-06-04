/*
UPDATE SRV.People 
SET SRV.People.[statusId] = SRV.PersonStatuses.personStatusId 
FROM SRV.People
inner join SRV.PersonStatuses on SRV.PersonStatuses.personId = SRV.People.personId
*/
