BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE Lookup.CircumscriptionLists SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
BEGIN TRANSACTION
GO
EXECUTE sp_rename N'Lookup.ElectionTypes.acceptStayDeclaration', N'Tmp_acceptResidenceDoc', 'COLUMN' 
GO
EXECUTE sp_rename N'Lookup.ElectionTypes.Tmp_acceptResidenceDoc', N'acceptResidenceDoc', 'COLUMN' 
GO
ALTER TABLE Lookup.ElectionTypes ADD
	acceptVotingCert bit NOT NULL CONSTRAINT DF_ElectionTypes_acceptVotingCert DEFAULT 0,
	acceptAbroadDeclaration bit NOT NULL CONSTRAINT DF_ElectionTypes_acceptAbroadDeclaration DEFAULT 0,
	circumscriptionListId bigint NULL,
	electionArea tinyint NULL,
	electionCompetitorType tinyint NULL,
	electionRoundsNo tinyint NULL,
	code int NULL
GO
ALTER TABLE Lookup.ElectionTypes ADD CONSTRAINT
	FK_ElectionTypes_CircumscriptionLists FOREIGN KEY
	(
	circumscriptionListId
	) REFERENCES Lookup.CircumscriptionLists
	(
	circumscriptionListId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Lookup.ElectionTypes SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
