/*
	Was added new column in Rsp Modification table. To fill new column with its name
*/
update Importer.RspRegistrationDatas
	set streetName = s.docprint
from Importer.RspRegistrationDatas as r
	join RSP.StreetTypeCodes as s on r.streetcode=s.rspStreetTypeCodeId