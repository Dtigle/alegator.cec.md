

CREATE PROCEDURE [dbo].[MoveDataFromTemp]
AS

	/* Clean tables */
	DELETE FROM [dbo].[PollingStation];
	DELETE FROM [dbo].[AssignedCircumscription];
	DELETE FROM [dbo].[Region];
	DELETE FROM [dbo].[RegionType];
	DELETE FROM [dbo].[AssignedVoter];
	DELETE FROM [dbo].[Voter];
	DELETE FROM [dbo].[PoliticalPartyStatusOverride];
	DELETE FROM [dbo].[ElectionResult];
	DELETE FROM [dbo].[BallotPaper];
	DELETE FROM [dbo].[ElectionCompetitorMember];
	DELETE FROM [dbo].[ElectionCompetitor];
	DELETE FROM [dbo].[AssignedPollingStation];
	DELETE FROM [dbo].[AssignedRole];
	DELETE FROM [dbo].[SystemUser];
	DELETE FROM [dbo].[ElectionRound];
	DELETE FROM [dbo].[Election];
	DELETE FROM [dbo].[ReportParamValues];	
	DELETE FROM [dbo].[ReportParams];
	DELETE FROM [dbo].[ElectionType];
	DELETE FROM [dbo].[AssignedPermission];
	DELETE FROM [dbo].[Role];
	DELETE FROM [dbo].[Permission];
	DELETE FROM [dbo].[ElectionDay];
	DELETE FROM [dbo].[AgeCategories];
	DELETE FROM [dbo].[CircumscriptionRegion];
	
	/* RegionType */
	SET IDENTITY_INSERT [dbo].[RegionType] ON;	
	INSERT INTO [dbo].[RegionType] ([RegionTypeId],[Name],[Description],[Rank],[EditUserId],[EditDate],[Version])
	(
		SELECT [RegionTypeId],[Name],[Description],[Rank],[EditUserId],[EditDate],[Version]
		FROM [schematmp].[RegionType] 
	);
	SET IDENTITY_INSERT [dbo].[RegionType] OFF;
	
	/* Region */
	SET IDENTITY_INSERT [dbo].[Region] ON;	
	INSERT INTO [dbo].[Region] ([RegionId], [Name],	[NameRu], [Description], [ParentId], [RegionTypeId], [RegistryId], [StatisticCode],	[StatisticIdentifier], [HasStreets], [GeoLatitude], [GeoLongitude], [EditUserId], [EditDate], [Version])
	(
		SELECT 	[RegionId], [Name],	[NameRu], [Description], [ParentId], [RegionTypeId], [RegistryId], [StatisticCode],	[StatisticIdentifier], [HasStreets], [GeoLatitude], [GeoLongitude], [EditUserId], [EditDate], [Version]
		FROM [schematmp].[Region] 
	);
	SET IDENTITY_INSERT [dbo].[Region] OFF;
	
	/* ElectionDay */
	SET IDENTITY_INSERT [dbo].[ElectionDay] ON;	
	INSERT INTO [dbo].[ElectionDay] ([ElectionDayId],[ElectionDayDate],[DeployDbDate],[Name],[Description]) 
	(
		SELECT [ElectionDayId],[ElectionDayDate],[DeployDbDate],[Name],[Description]
		FROM [schematmp].[ElectionDay] 
	);
	SET IDENTITY_INSERT [dbo].[ElectionDay] OFF;	

	/* Permission */
	SET IDENTITY_INSERT [dbo].[Permission] ON;	
	INSERT INTO [dbo].[Permission] ([PermissionId], [Name], [EditUserId], [EditDate], [Version]) 
	(
		SELECT [PermissionId], [Name], [EditUserId], [EditDate], [Version] 
		FROM [schematmp].[Permission] 
	);
	SET IDENTITY_INSERT [dbo].[Permission] OFF;	

	/* Role */
	SET IDENTITY_INSERT [dbo].[Role] ON;	
	INSERT INTO [dbo].[Role] ([RoleId], [Name], [Level], [EditUserId], [EditDate], [Version]) 
	(
		SELECT [RoleId], [Name], [Level], [EditUserId], [EditDate], [Version]
		FROM [schematmp].[Role] 
	);
	SET IDENTITY_INSERT [dbo].[Role] OFF;	

	/* Assigned Permission */
	SET IDENTITY_INSERT [dbo].[AssignedPermission] ON;	
	INSERT INTO [dbo].[AssignedPermission] ([AssignedPermissionId], [RoleId], [PermissionId], [EditUserId], [EditDate], [Version]) 
	(
		SELECT [AssignedPermissionId], [RoleId], [PermissionId], [EditUserId], [EditDate], [Version]
		FROM [schematmp].[AssignedPermission] 
	);
	SET IDENTITY_INSERT [dbo].[AssignedPermission] OFF;	

	/* Polling station */
	SET IDENTITY_INSERT [dbo].[PollingStation] ON;	
	INSERT INTO [dbo].[PollingStation] ([PollingStationId], [Type], [Number], [SubNumber], [OldName], [NameRo], [NameRu], [Address], [RegionId], [StreetId], [StreetNumber], [StreetSubNumber], 
		[EditUserId], [EditDate], [Version], [LocationLatitude], [LocationLongitude], [ExcludeInLocalElections]) 
	(
		SELECT [PollingStationId], [Type], [Number], [SubNumber], [OldName], [NameRo], [NameRu], [Address], [RegionId], [StreetId], [StreetNumber], [StreetSubNumber], 
			[EditUserId], [EditDate], [Version], [LocationLatitude], [LocationLongitude], [ExcludeInLocalElections]
		FROM [schematmp].[PollingStation] 
	);
	SET IDENTITY_INSERT [dbo].[PollingStation] OFF;

	/* System User */
	SET IDENTITY_INSERT [dbo].[SystemUser] ON;	
	INSERT INTO [dbo].[SystemUser] ([SystemUserId], [UserName], [Password], [Email], [Level], [Comments], [Idnp], [FirstName], [Surname], [MiddleName], [DateOfBirth], [Gender], 
		[PasswordQuestion], [PasswordAnswer], [IsApproved], [IsOnLine], [IsLockedOut], [CreationDate], [LastActivityDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedAttemptStart],
		[FailedAnswerStart], [FailedAttemptCount], [FailedAnswerCount], [LastLoginDate], [LastUpdateDate], [Language], [MobileNumber], [ContactName], [ContactMobileNumber], [StreetAddress], 
		[ElectionId], [RegionId], [PollingStationId],[CircumscriptionId], [EditUserId], [EditDate], [Version], [IsDeleted]) 
	(
		SELECT [SystemUserId], [UserName], [Password], [Email], [Level], [Comments], [Idnp], [FirstName], [Surname], [MiddleName], [DateOfBirth], [Gender], 
			[PasswordQuestion], [PasswordAnswer], [IsApproved], [IsOnLine], [IsLockedOut], [CreationDate], [LastActivityDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedAttemptStart],
			[FailedAnswerStart], [FailedAttemptCount], [FailedAnswerCount], [LastLoginDate], [LastUpdateDate], [Language], [MobileNumber], [ContactName], [ContactMobileNumber], [StreetAddress], 
			[ElectionId], [RegionId], [PollingStationId],[CircumscriptionId] , [EditUserId], [EditDate], [Version], [IsDeleted]
		FROM [schematmp].[SystemUser] 
	);
	SET IDENTITY_INSERT [dbo].[SystemUser] OFF;

	/* Assigned Role */
	/*SET IDENTITY_INSERT [dbo].[AssignedRole] ON;	*/
	INSERT INTO [dbo].[AssignedRole] ([RoleId], [SystemUserId], [EditUserId], [EditDate], [Version]) 
	(
		SELECT [RoleId], [SystemUserId], [EditUserId], [EditDate], [Version]
		FROM [schematmp].[AssignedRole] 
	);
	/*SET IDENTITY_INSERT [dbo].[AssignedRole] OFF;*/

	/* Election type */
	INSERT INTO [dbo].[ElectionType] ([ElectionTypeId], [Code],[TypeName],[Description],[ElectionArea],[ElectionCompetitorType],[ElectionRoundsNo],[AcceptResidenceDoc],[AcceptVotingCert],[AcceptAbroadDeclaration])
	(
		SELECT [ElectionTypeId], [Code],[TypeName],[Description],[ElectionArea],[ElectionCompetitorType],[ElectionRoundsNo],[AcceptResidenceDoc],[AcceptVotingCert],[AcceptAbroadDeclaration]
		FROM [schematmp].[ElectionType] 
	);

	/* Election */
	SET IDENTITY_INSERT [dbo].[Election] ON;	
	INSERT INTO [dbo].[Election] ([ElectionId], [Type], [Status], [DateOfElection], [Comments], [EditUserId], [EditDate], [Version], [ReportsPath], [BuletinDateOfElectionRo], [BuletinDateOfElectionRu])
	(
		SELECT [ElectionId], [Type], [Status], [DateOfElection], [Comments], [EditUserId], [EditDate], [Version], [ReportsPath], [BuletinDateOfElectionRo], [BuletinDateOfElectionRu]
		FROM [schematmp].[Election] 
	);
	SET IDENTITY_INSERT [dbo].[Election] OFF;
	
		/* ElectionRound */
	SET IDENTITY_INSERT [dbo].[ElectionRound] ON;	
	INSERT INTO [dbo].[ElectionRound] ([ElectionRoundId],[ElectionId],[Number],[NameRo],[NameRu],[Description],[ElectionDate],[CampaignStartDate],[CampaignEndDate],[Status],[EditUserId],[EditDate],[Version])
	(
		SELECT [ElectionRoundId],[ElectionId],[Number],[NameRo],[NameRu],[Description],[ElectionDate],[CampaignStartDate],[CampaignEndDate],[Status],[EditUserId],[EditDate],[Version]
		FROM [schematmp].[ElectionRound] 
	);
	SET IDENTITY_INSERT [dbo].[ElectionRound] OFF;
	
	/* AssignedCircumscription */
	SET IDENTITY_INSERT [dbo].[AssignedCircumscription] ON;	
	INSERT INTO [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId],[ElectionRoundId],[CircumscriptionId],[RegionId], [Number],[NameRo], [isFromUtan], [EditUserId],[EditDate],[Version])
	(
		SELECT [AssignedCircumscriptionId],[ElectionRoundId],[CircumscriptionId],[RegionId],[Number],[NameRo],[isFromUtan], [EditUserId],[EditDate],[Version]
		FROM [schematmp].[AssignedCircumscription] 
	);
	SET IDENTITY_INSERT [dbo].[AssignedCircumscription] OFF;
	
	/* AssignedPollingStation */
	SET IDENTITY_INSERT [dbo].[AssignedPollingStation] ON;	
	INSERT INTO [dbo].[AssignedPollingStation] ([AssignedPollingStationId], [ElectionRoundId], [AssignedCircumscriptionId], [PollingStationId], [Type], [Status], [IsOpen], [OpeningVoters], [EstimatedNumberOfVoters], 
		[NumberOfRoBallotPapers], [NumberOfRuBallotPapers], [ImplementsEVR], [EditUserId], [EditDate], [Version], [isOpeningEnabled], [isTurnoutEnabled], [isElectionResultEnabled],[numberPerElection],
		[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu])
	(
		SELECT [AssignedPollingStationId], [ElectionRoundId], [AssignedCircumscriptionId], [PollingStationId], [Type], [Status], [IsOpen], [OpeningVoters], [EstimatedNumberOfVoters], 
			[NumberOfRoBallotPapers], [NumberOfRuBallotPapers], [ImplementsEVR], [EditUserId], [EditDate], [Version], [isOpeningEnabled], [isTurnoutEnabled], [isElectionResultEnabled],[numberPerElection],
			[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu]
		FROM [schematmp].[AssignedPollingStation] 
	);
	SET IDENTITY_INSERT [dbo].[AssignedPollingStation] OFF;
	
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
		SELECT [ElectionResultId], [ElectionRoundId], [BallotOrder], [BallotCount], [Comments], [DateOfEntry], [Status], [ElectionCompetitorId], [ElectionCompetitorMemberId],
			[BallotPaperId], [EditUserId], [EditDate], [Version]
		FROM [schematmp].[ElectionResult] 
	);
	SET IDENTITY_INSERT [dbo].[ElectionResult] OFF;
	
	
	/* AuditEventTypes */
	SET IDENTITY_INSERT [dbo].[AuditEventTypes] ON;	
	INSERT INTO [dbo].[AuditEventTypes] ([auditEventTypeId],[code],[auditStrategy],[name],[description],[EditUserId],[EditDate],[Version])
	(
		SELECT [auditEventTypeId],[code],[auditStrategy],[name],[description],[EditUserId],[EditDate],[Version]
		FROM [schematmp].[AuditEventTypes] 
	);
	SET IDENTITY_INSERT [dbo].[AuditEventTypes] OFF;

	/* ReportParams */
	SET IDENTITY_INSERT [dbo].[ReportParams] ON;	
	INSERT INTO [dbo].[ReportParams] ([ReportParamId],[Code],[Description])
	(
		SELECT [ReportParamId],[Code],[Description]
		FROM [schematmp].[ReportParams] 
	);
	SET IDENTITY_INSERT [dbo].[ReportParams] OFF;

	/* ReportParams */
	SET IDENTITY_INSERT [dbo].[ReportParamValues] ON;	
	INSERT INTO [dbo].[ReportParamValues] ([ReportParamValueId],[ReportParamId],[ElectionTypeId],[Value])
	(
		SELECT [ReportParamValueId],[ReportParamId],[ElectionTypeId],[Value]
		FROM [schematmp].[ReportParamValues] 
	);
	SET IDENTITY_INSERT [dbo].[ReportParamValues] OFF;

	/* AgeCategories */
	SET IDENTITY_INSERT [dbo].[AgeCategories] ON;	
	INSERT INTO [dbo].[AgeCategories] ([AgeCategoryId],[From],[To],[Name])
	(
		SELECT [AgeCategoryId],[From],[To],[Name]
		FROM [schematmp].[AgeCategories]
	);
	SET IDENTITY_INSERT [dbo].[AgeCategories] OFF;

	/* CircumscriptionRegion */
	SET IDENTITY_INSERT [dbo].[CircumscriptionRegion] ON;	
	INSERT INTO [dbo].[CircumscriptionRegion] ([CircumscriptionRegionId],[AssignedCircumscriptionId],[ElectionRoundId],[RegionId])
	(
		SELECT [CircumscriptionRegionId],[AssignedCircumscriptionId],[ElectionRoundId],[RegionId]
		FROM [schematmp].[CircumscriptionRegion]
	);
	SET IDENTITY_INSERT [dbo].[CircumscriptionRegion] OFF;


RETURN 0

