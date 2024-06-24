
if not exists(select * from sys.columns 
            where Name = N'Color' and Object_ID = Object_ID(N'PoliticalParty'))
begin

	BEGIN TRANSACTION

	ALTER TABLE dbo.PoliticalParty ADD
		Color nvarchar(10) NULL

	ALTER TABLE dbo.PoliticalParty SET (LOCK_ESCALATION = TABLE)

	COMMIT

end

if not exists(select 8 from sys.columns
			where Name = N'PartyType' and OBJECT_ID = OBJECT_ID(N'PoliticalParty'))
begin

	BEGIN TRANSACTION

	ALTER TABLE dbo.PoliticalParty ADD
		PartyType int NOT NULL default 0

	ALTER TABLE dbo.PoliticalParty SET (LOCK_ESCALATION = TABLE)

	COMMIT

end