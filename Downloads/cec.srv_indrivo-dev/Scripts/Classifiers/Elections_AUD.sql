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
ALTER TABLE Audit.Elections_AUD ADD
	nameRo nvarchar(255) NULL,
	nameRu nvarchar(255) NULL,
	description nvarchar(1000) NULL,
	status tinyint NOT NULL CONSTRAINT DF_Elections_AUD_status DEFAULT 1,
	statusDate datetimeoffset(7) NOT NULL CONSTRAINT DF_Elections_AUD_statusDate DEFAULT (sysdatetimeoffset()),
	statusReason nvarchar(255) NULL,
	reportsPath nvarchar(255) NULL
GO
ALTER TABLE Audit.Elections_AUD
	DROP COLUMN electionDate, saiseId, acceptAbroadDeclaration, comments
GO
ALTER TABLE Audit.Elections_AUD SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
