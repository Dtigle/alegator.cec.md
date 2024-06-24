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
EXECUTE sp_rename N'Audit.ElectionTypes_AUD.acceptStayDeclaration', N'Tmp_acceptResidenceDoc_2', 'COLUMN' 
GO
EXECUTE sp_rename N'Audit.ElectionTypes_AUD.Tmp_acceptResidenceDoc_2', N'acceptResidenceDoc', 'COLUMN' 
GO
ALTER TABLE Audit.ElectionTypes_AUD ADD
	acceptVotingCert bit NOT NULL CONSTRAINT DF_ElectionTypes_AUD_acceptVotingCert DEFAULT 0,
	acceptAbroadDeclaration bit NOT NULL CONSTRAINT DF_ElectionTypes_AUD_acceptAbroadDeclaration DEFAULT 0,
	electionArea tinyint NULL,
	electionCompetitorType tinyint NULL,
	electionRoundsNo tinyint NULL,
	code int NULL,
	circumscriptionListId bigint NULL
GO
ALTER TABLE Audit.ElectionTypes_AUD SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
