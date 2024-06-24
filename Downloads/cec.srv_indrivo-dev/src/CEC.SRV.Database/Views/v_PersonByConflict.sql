CREATE VIEW Importer.[v_PersonByConflict]
	AS 
	SELECT p.*, conf.rspModificationDataId
	FROM Importer.[RspModificationDatas] as conf 
	inner join SRV.People as p ON p.idnp = conf.idnp
