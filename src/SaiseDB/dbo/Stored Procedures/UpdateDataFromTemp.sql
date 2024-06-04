
CREATE PROCEDURE [dbo].[UpdateDataFromTemp]
AS
	/* Clean tables */
	TRUNCATE TABLE [dbo].[ElectionResult];
	DELETE FROM [dbo].[BallotPaper];
	DELETE FROM [dbo].[ElectionCompetitorMember];
	DELETE FROM [dbo].[ElectionCompetitor];


    
	/* ElectionCompetitor */
	SET IDENTITY_INSERT [dbo].[ElectionCompetitor] ON;	
	INSERT INTO [dbo].[ElectionCompetitor] ([ElectionCompetitorId], [PoliticalPartyId], [ElectionRoundId], [AssignedCircumscriptionId], [Code], [NameRo], [NameRu], [colorLogo], [DateOfRegistration], [Status], [IsIndependent], [BallotOrder], [EditUserId], [EditDate], [Version], [PartyOrder], 
		[DisplayFromNameRo], [DisplayFromNameRu], [RegistryNumber], [blackWhiteLogo], [PartyType], [BallotPaperNameRo], [BallotPaperNameRu], [BallotPapperCustomCssRo], [BallotPapperCustomCssRu],[Color])
	(
		SELECT [ElectionCompetitorId], [PoliticalPartyId], [ElectionRoundId], [AssignedCircumscriptionId], [Code], [NameRo], [NameRu], [colorLogo], [DateOfRegistration], [Status], [IsIndependent], [BallotOrder], [EditUserId], [EditDate], [Version], [PartyOrder], 
			[DisplayFromNameRo], [DisplayFromNameRu], [RegistryNumber], [blackWhiteLogo], [PartyType], [BallotPaperNameRo], [BallotPaperNameRu], [BallotPapperCustomCssRo], [BallotPapperCustomCssRu], [Color]
		FROM [schematmp].[ElectionCompetitor] 
	);
	SET IDENTITY_INSERT [dbo].[ElectionCompetitor] OFF;

	/* ElectionCompetitorMember */
	SET IDENTITY_INSERT [dbo].[ElectionCompetitorMember] ON;	
	INSERT INTO [dbo].[ElectionCompetitorMember] ([ElectionCompetitorMemberId], [AssignedCircumscriptionId], [ElectionRoundId], [LastNameRo], [LastNameRu], [NameRo], [NameRu], [PatronymicRo], [PatronymicRu], [DateOfBirth], [PlaceOfBirth], [Gender], [Occupation],
		[OccupationRu], [Designation], [DesignationRu], [Workplace], [WorkplaceRu], [Idnp], [ElectionCompetitorId], [DateOfRegistration],[ColorLogo], [BlackWhiteLogo], [Picture], [Status], [CompetitorMemberOrder], [EditUserId], [EditDate], [Version])
	(
		SELECT [ElectionCompetitorMemberId], [AssignedCircumscriptionId], [ElectionRoundId],  [LastNameRo], [LastNameRu], [NameRo], [NameRu], [PatronymicRo], [PatronymicRu], [DateOfBirth], [PlaceOfBirth], [Gender], [Occupation],
			[OccupationRu], [Designation], [DesignationRu], [Workplace], [WorkplaceRu], [Idnp], [ElectionCompetitorId], [DateOfRegistration], [ColorLogo], [BlackWhiteLogo], [Picture], [Status], [CompetitorMemberOrder], [EditUserId], [EditDate], [Version]
		FROM [schematmp].[ElectionCompetitorMember] 
	);
	SET IDENTITY_INSERT [dbo].[ElectionCompetitorMember] OFF;

	/* Ballot paper */
	SET IDENTITY_INSERT [dbo].[BallotPaper] ON;	
	INSERT INTO [dbo].[BallotPaper] ([BallotPaperId], [EntryLevel], [Type], [Status], [RegisteredVoters], [Supplementary], [BallotsIssued], [BallotsCasted],
		[DifferenceIssuedCasted], [BallotsValidVotes], [BallotsReceived], [BallotsUnusedSpoiled], [BallotsSpoiled], [BallotsUnused], [Description], [Comments], [DateOfEntry],
		[VotingPointId], [PollingStationId], [ElectionRoundId], [EditUserId], [EditDate], [IsResultsConfirmed], [ConfirmationUserId], [ConfirmationDate], [Version])
	(
		SELECT [BallotPaperId], [EntryLevel], [Type], [Status], [RegisteredVoters], [Supplementary], [BallotsIssued], [BallotsCasted],
			[DifferenceIssuedCasted], [BallotsValidVotes], [BallotsReceived], [BallotsUnusedSpoiled], [BallotsSpoiled], [BallotsUnused], [Description], [Comments], [DateOfEntry],
			[VotingPointId], [PollingStationId], [ElectionRoundId], [EditUserId], [EditDate], [IsResultsConfirmed], [ConfirmationUserId], [ConfirmationDate], [Version]
		FROM [schematmp].[BallotPaper] 
	);
	SET IDENTITY_INSERT [dbo].[BallotPaper] OFF;


	/* Ballot paper */
	SET IDENTITY_INSERT [dbo].[ElectionResult] ON;	
	INSERT INTO [dbo].[ElectionResult] ([ElectionResultId],[ElectionRoundId], [BallotOrder], [BallotCount], [Comments], [DateOfEntry], [Status], [ElectionCompetitorId], [ElectionCompetitorMemberId],
		[BallotPaperId], [EditUserId], [EditDate], [Version])
	(
		SELECT ER.[ElectionResultId], ER.[ElectionRoundId], ER.[BallotOrder], ER.[BallotCount], ER.[Comments], ER.[DateOfEntry], ER.[Status], ER.[ElectionCompetitorId],ER.[ElectionCompetitorMemberId],
			ER.[BallotPaperId], ER.[EditUserId], ER.[EditDate], ER.[Version]
		FROM 
		[schematmp].[ElectionResult] ER,
		[dbo].[BallotPaper]
		WHERE ER.BallotPaperId = [dbo].[BallotPaper].BallotPaperId
	);
	SET IDENTITY_INSERT [dbo].[ElectionResult] OFF;

RETURN 0
