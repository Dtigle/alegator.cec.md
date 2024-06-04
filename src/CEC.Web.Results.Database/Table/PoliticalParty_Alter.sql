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
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#CF0000' WHERE [PoliticalPartyId] = 5;
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#14283D' WHERE [PoliticalPartyId] = 10;
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#00BFF3' WHERE [PoliticalPartyId] = 3;
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#28AE4C' WHERE [PoliticalPartyId] = 9;
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#EC1224' WHERE [PoliticalPartyId] = 65;
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#806C2C' WHERE [PoliticalPartyId] = 29;
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#4B1013' WHERE [PoliticalPartyId] = 2;
UPDATE [dbo].[POLITICALPARTY] SET [COLOR] = '#EC1224' WHERE [PoliticalPartyId] = 60;