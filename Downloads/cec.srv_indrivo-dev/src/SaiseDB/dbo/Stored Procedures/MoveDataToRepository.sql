
CREATE PROCEDURE [dbo].[MoveDataToRepository]
(
    @serverName	        VARCHAR(500),
    @serverIpAddress	VARCHAR(20),
    @localUsername      VARCHAR(20),
    @remoteUsername     VARCHAR(20),
    @remotePassword     VARCHAR(20),

	/* execution parameters */
	@execStatus    		INT           OUTPUT,
	@execMsg       		VARCHAR(5000) OUTPUT
)
AS

DECLARE
	@success       INT = 0,
	@sqlError      INT = -2,
	@businessError INT = -1,
	@electionDayId	BIGINT,
	@EDayRepositoryDbName	VARCHAR(50),
	@finalRemoteName	VARCHAR(500),
	@finalRemoteNameDbo	VARCHAR(500),
	@sql NVARCHAR(MAX),
	@retval int = 0,
    @sysservername sysname;

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
	/* return if technical parameters are not ok */
	IF (@execStatus = @sqlError) BEGIN
		GOTO Custom_Exception_Fail
	END;

	SET @EDayRepositoryDbName  = 'SAISE.ReportingServer';

	/* validate param */

	IF (@serverName IS NULL OR LEN(@serverName) = 0)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Numele serverului nu a fost specificat!';
	    GOTO Custom_Exception_Fail
	END;

	IF (@serverIpAddress IS NULL OR LEN(@serverIpAddress) = 0)
	BEGIN
	    SET @execStatus = @businessError;
		SET @execMsg = 'Adresa IP nu a fost specificata!';
	    GOTO Custom_Exception_Fail
	END;

	IF (@localUsername IS NULL OR LEN(@localUsername) = 0)
	BEGIN
	    SET @execStatus = @businessError;
		SET @execMsg = 'Userul de admin local nu a fost specificat!';
	    GOTO Custom_Exception_Fail
	END;

	IF (@remoteUsername IS NULL OR LEN(@remoteUsername) = 0)
	BEGIN
	    SET @execStatus = @businessError;
		SET @execMsg = 'Userul de admin remote nu a fost specificat!';
	    GOTO Custom_Exception_Fail
	END;

	IF (@remotePassword IS NULL OR LEN(@remotePassword) = 0)
	BEGIN
	    SET @execStatus = @businessError;
		SET @execMsg = 'Parola userului de admin remote nu a fost specificata!';
	    GOTO Custom_Exception_Fail
	END;

    /* return if parameters are not ok */
	IF (@execStatus = @sqlError OR @execStatus = @businessError)
	BEGIN
		GOTO Custom_Exception_Fail
	END;

    IF EXISTS (select * from sys.servers where name = @serverName)
    BEGIN
        /* Delete existing linked server */
        EXEC master.dbo.sp_dropserver @serverName, 'droplogins';
    END;
    
	EXEC master.dbo.sp_addlinkedserver @server = @serverName, @srvproduct=N'sql_server', @provider=N'SQLNCLI11', @datasrc=@serverIpAddress

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'collation compatible', @optvalue=N'false'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'data access', @optvalue=N'true'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'dist', @optvalue=N'false'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'pub', @optvalue=N'false'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'rpc', @optvalue=N'true'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'rpc out', @optvalue=N'true'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'sub', @optvalue=N'false'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'connect timeout', @optvalue=N'0'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'collation name', @optvalue=null

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'lazy schema validation', @optvalue=N'false'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'query timeout', @optvalue=N'0'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'use remote collation', @optvalue=N'true'

	EXEC master.dbo.sp_serveroption @server=@serverName, @optname=N'remote proc transaction promotion', @optvalue=N'true'

	EXEC master.dbo.sp_addlinkedsrvlogin @rmtsrvname = @serverName, @locallogin = NULL , @useself = N'False', @rmtuser = @remoteUsername, @rmtpassword = @remotePassword

	BEGIN TRY
    SELECT  @sysservername = CONVERT(sysname, @serverName);
    EXEC @retval = sys.sp_testlinkedserver @sysservername;
	END TRY
	BEGIN CATCH
		IF EXISTS (select * from sys.servers where name = @serverName)
		BEGIN
		EXEC master.dbo.sp_dropserver @servername, 'droplogins';
		SET @execStatus = @sqlError;
		SET @execMsg = 'Eroare de conectare la serverul bazei de date de raportare!';
		RETURN;
		END
	END CATCH;  

    IF EXISTS (select * from sys.servers where name = @serverName)
    BEGIN

	SET @finalRemoteName = '[' + @serverName + '].[' + @EDayRepositoryDbName + '].[schematmp]';

	SET @finalRemoteNameDbo = '[' + @serverName + '].[' + @EDayRepositoryDbName + '].[dbo]';

	SET @electionDayId = (SELECT TOP 1 ElectionDay.ElectionDayId FROM ElectionDay);

	UPDATE ElectionDay
	SET 
	StartDateToReportDb = GETDATE(),
	EndDateToReportDb = NULL
	WHERE ElectionDayId = @electionDayId;


	SET @finalRemoteNameDbo = '[' + @serverName  + '].[' + @EDayRepositoryDbName + '].[dbo]';
	SET @sql = 'EXEC ' + @finalRemoteNameDbo + '.[ClearTemp];';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);
	


	/* AssignedCircumscription */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[AssignedCircumscription] ([AssignedCircumscriptionId],[ElectionRoundId],[CircumscriptionId],[RegionId],[Number],[NameRo],[isFromUtan]) ';
	SET @sql = @sql + '(SELECT [AssignedCircumscriptionId],[ElectionRoundId],[CircumscriptionId],[RegionId],[Number],[NameRo],[isFromUtan]';
	SET @sql = @sql + ' FROM [dbo].[AssignedCircumscription]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* AssignedPollingStation */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[AssignedPollingStation] ([AssignedPollingStationId],[ElectionDayId],[ElectionRoundId]
	  ,[PollingStationId],[AssignedCircumscriptionId],[Type],[Status],[IsOpen],[OpeningVoters],[EstimatedNumberOfVoters],[NumberOfRoBallotPapers]
	  ,[NumberOfRuBallotPapers],[ImplementsEVR],[isOpeningEnabled],[isTurnoutEnabled],[isElectionResultEnabled],[NumberPerElection],[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu])';
	SET @sql = @sql + '(SELECT [AssignedPollingStationId],' + CAST(@electionDayId AS VARCHAR(10)) + ',[ElectionRoundId]
	  ,[PollingStationId],[AssignedCircumscriptionId],[Type],[Status],[IsOpen],[OpeningVoters],[EstimatedNumberOfVoters],[NumberOfRoBallotPapers]
	  ,[NumberOfRuBallotPapers],[ImplementsEVR],[isOpeningEnabled],[isTurnoutEnabled],[isElectionResultEnabled],[NumberPerElection],[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu]';
	SET @sql = @sql + ' FROM [dbo].[AssignedPollingStation]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* AssignedVoter */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[AssignedVoter] ([AssignedVoterId],[ElectionDayId],[RequestingPollingStationId],[PollingStationId],[VoterId],[Idnp],[Status],[ElectionListNr]) ';
	SET @sql = @sql + '(SELECT [AssignedVoterId],' + CAST(@electionDayId AS VARCHAR(10)) + ',[RequestingPollingStationId],[PollingStationId],[VoterId]
	  ,(SELECT TOP 1 [Idnp] FROM [dbo].[Voter] WHERE [dbo].[Voter].[VoterId]= [dbo].[AssignedVoter].[VoterId]),[Status],[ElectionListNr]';
	SET @sql = @sql + ' FROM [dbo].[AssignedVoter]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* BallotPaper */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[BallotPaper] ([BallotPaperId],[ElectionDayId],[PollingStationId],[ElectionRoundId],
	[EntryLevel],[RegisteredVoters],[Supplementary],[BallotsIssued],[BallotsCasted],[DifferenceIssuedCasted],
	[BallotsValidVotes],[BallotsReceived],[BallotsUnusedSpoiled],[BallotsSpoiled],[BallotsUnused],[Description],[Comments],
	[DateOfEntry],[VotingPointId],[IsResultsConfirmed],[ConfirmationUserId],[ConfirmationDate]) ';
	SET @sql = @sql + '(SELECT [BallotPaperId],' + CAST(@electionDayId AS VARCHAR(10)) + ',[PollingStationId],[ElectionRoundId],
	[EntryLevel],[RegisteredVoters],[Supplementary],[BallotsIssued],[BallotsCasted],[DifferenceIssuedCasted],
	[BallotsValidVotes],[BallotsReceived],[BallotsUnusedSpoiled],[BallotsSpoiled],[BallotsUnused],[Description],[Comments],
	[DateOfEntry],[VotingPointId],[IsResultsConfirmed],[ConfirmationUserId],[ConfirmationDate]';
	SET @sql = @sql + ' FROM [dbo].[BallotPaper]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* ElectionCompetitor */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitor] ([ElectionCompetitorId],[PoliticalPartyId],[ElectionRoundId]
	  ,[AssignedCircumscriptionId],[Code],[NameRo],[NameRu],[DateOfRegistration],[Status],[IsIndependent],[BallotOrder],[PartyOrder]
	  ,[DisplayFromNameRo],[DisplayFromNameRu],[RegistryNumber],[PartyType],[BallotPaperNameRo]
	  ,[BallotPaperNameRu],[BallotPapperCustomCssRo],[BallotPapperCustomCssRu],[ColorLogo],[BlackWhiteLogo],[Color])';
	SET @sql = @sql + '(SELECT [ElectionCompetitorId],[PoliticalPartyId],[ElectionRoundId]
	  ,[AssignedCircumscriptionId],[Code],[NameRo],[NameRu],[DateOfRegistration],[Status],[IsIndependent],[BallotOrder],[PartyOrder]
	  ,[DisplayFromNameRo],[DisplayFromNameRu],[RegistryNumber],[PartyType],[BallotPaperNameRo]
	  ,[BallotPaperNameRu],[BallotPapperCustomCssRo],[BallotPapperCustomCssRu],[ColorLogo],[BlackWhiteLogo],[Color]';
	SET @sql = @sql + ' FROM [dbo].[ElectionCompetitor]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* ElectionCompetitorMember */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitorMember] ([ElectionCompetitorMemberId],[AssignedCircumscriptionId],[ElectionRoundId]
	  ,[LastNameRo],[LastNameRu],[NameRo],[NameRu],[PatronymicRo],[PatronymicRu],[DateOfBirth],[PlaceOfBirth],[Gender],[Occupation],[OccupationRu],[Designation]
      ,[DesignationRu],[Workplace],[WorkplaceRu],[Idnp],[ElectionCompetitorId],[DateOfRegistration],[Status],[CompetitorMemberOrder],[ColorLogo],[BlackWhiteLogo],[Picture]) ';
	SET @sql = @sql + '(SELECT [ElectionCompetitorMemberId],[AssignedCircumscriptionId],[ElectionRoundId]
	  ,[LastNameRo],[LastNameRu],[NameRo],[NameRu],[PatronymicRo],[PatronymicRu],[DateOfBirth],[PlaceOfBirth],[Gender],[Occupation],[OccupationRu],[Designation]
      ,[DesignationRu],[Workplace],[WorkplaceRu],[Idnp],[ElectionCompetitorId],[DateOfRegistration],[Status],[CompetitorMemberOrder],[ColorLogo],[BlackWhiteLogo],[Picture]';
	SET @sql = @sql + ' FROM [dbo].[ElectionCompetitorMember]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* Election */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[Election] ([ElectionId],[Type],[Status],[DateOfElection]
	  ,[Comments],[ReportsPath],[BuletinDateOfElectionRo],[BuletinDateOfElectionRu]) ';
	SET @sql = @sql + '(SELECT [ElectionId],[Type],[Status],[DateOfElection],[Comments]
      ,[ReportsPath],[BuletinDateOfElectionRo],[BuletinDateOfElectionRu]';
	SET @sql = @sql + ' FROM [dbo].[Election]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* ElectionDay */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[ElectionDay] ([ElectionDayId],[ElectionDayDate],[Name],[Description])';
	SET @sql = @sql + '(SELECT [ElectionDayId],[ElectionDayDate],[Name],[Description]';
	SET @sql = @sql + ' FROM [dbo].[ElectionDay]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* ElectionResult */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[ElectionResult] ([ElectionResultId],[ElectionDayId],[ElectionRoundId]
	  ,[ElectionCompetitorId],[ElectionCompetitorMemberId],[BallotPaperId],[BallotOrder],[BallotCount],[Comments],[DateOfEntry])';
	SET @sql = @sql + '(SELECT [ElectionResultId],' + CAST(@electionDayId AS VARCHAR(10)) + ',[ElectionRoundId]
	  ,[ElectionCompetitorId],[ElectionCompetitorMemberId],[BallotPaperId],[BallotOrder],[BallotCount],[Comments],[DateOfEntry]';
	SET @sql = @sql + ' FROM [dbo].[ElectionResult]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* ElectionRound */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[ElectionRound] ([ElectionRoundId],[ElectionDayId],[ElectionId],[Number]
	  ,[NameRo],[NameRu],[Description],[ElectionDate],[CampaignStartDate],[CampaignEndDate],[Status])';
	SET @sql = @sql + '(SELECT [ElectionRoundId],' + CAST(@electionDayId AS VARCHAR(10)) + ',[ElectionId],[Number]
	  ,[NameRo],[NameRu],[Description],[ElectionDate],[CampaignStartDate],[CampaignEndDate],[Status]';
	SET @sql = @sql + ' FROM [dbo].[ElectionRound]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* ElectionType */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[ElectionType] ([ElectionTypeId],[Code],[TypeName],[Description],[ElectionArea],[ElectionCompetitorType],[ElectionRoundsNo],[AcceptResidenceDoc],[AcceptVotingCert],[AcceptAbroadDeclaration])';
	SET @sql = @sql + '(SELECT [ElectionTypeId],[Code],[TypeName],[Description],[ElectionArea],[ElectionCompetitorType],[ElectionRoundsNo],[AcceptResidenceDoc],[AcceptVotingCert],[AcceptAbroadDeclaration]';
	SET @sql = @sql + ' FROM [dbo].[ElectionType]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* PoliticalPartyStatusOverride */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[PoliticalPartyStatusOverride] ([PoliticalPartyStatusOverrideId],[ElectionDayId]
	  ,[ElectionRoundId],[ElectionCompetitorId],[AssignedCircumscriptionId],[PoliticalPartyStatus])';
	SET @sql = @sql + '(SELECT [PoliticalPartyStatusOverrideId],' + CAST(@electionDayId AS VARCHAR(10)) + '
	  ,[ElectionRoundId],[ElectionCompetitorId],[AssignedCircumscriptionId],[PoliticalPartyStatus]';
	SET @sql = @sql + ' FROM [dbo].[PoliticalPartyStatusOverride]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* PollingStation */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[PollingStation] ([PollingStationId],[ElectionDayId]
	  ,[RegionId],[Type],[Number],[SubNumber],[OldName],[NameRo],[NameRu],[Address],[StreetId],[StreetNumber]
	  ,[StreetSubNumber],[LocationLatitude],[LocationLongitude],[ExcludeInLocalElections])';
	SET @sql = @sql + '(SELECT [PollingStationId],' + CAST(@electionDayId AS VARCHAR(10)) + '
	  ,[RegionId],[Type],[Number],[SubNumber],[OldName],[NameRo],[NameRu],[Address],[StreetId],[StreetNumber]
	  ,[StreetSubNumber],[LocationLatitude],[LocationLongitude],[ExcludeInLocalElections]';
	SET @sql = @sql + ' FROM [dbo].[PollingStation]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* Region */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[Region] ([RegionId],[ElectionDayId],[Name],[NameRu],[Description]
	  ,[ParentId],[RegionTypeId],[RegistryId],[StatisticCode],[StatisticIdentifier],[HasStreets],[GeoLatitude],[GeoLongitude])';
	SET @sql = @sql + '(		
		SELECT r.[RegionId], ' + CAST(@electionDayId AS VARCHAR(10)) + ', r.[Name],	r.[NameRu], r.[Description], r.[ParentId], r.[RegionTypeId], r.[RegistryId], r.[StatisticCode],	r.[StatisticIdentifier], r.[HasStreets], r.[GeoLatitude], r.[GeoLongitude] FROM [dbo].[Region] as r		
		--EXCEPT
		--SELECT ls.[RegionId], ' + CAST(@electionDayId AS VARCHAR(10)) + ' , ls.[Name],	ls.[NameRu], ls.[Description], ls.[ParentId], ls.[RegionTypeId], ls.[RegistryId], ls.[StatisticCode],	ls.[StatisticIdentifier], ls.[HasStreets], ls.[GeoLatitude], ls.[GeoLongitude] FROM '+@finalRemoteNameDbo+'.[Region] as ls					
	);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* RegionType */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[RegionType] ([RegionTypeId],[Name],[Description],[Rank])';
	SET @sql = @sql + '(SELECT [RegionTypeId],[Name],[Description],[Rank]';
	SET @sql = @sql + ' FROM [dbo].[RegionType]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* Voter */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[Voter] ([ElectionDayId],[NameRo],[LastNameRo],[PatronymicRo],[LastNameRu]
	  ,[NameRu],[PatronymicRu],[DateOfBirth],[PlaceOfBirth],[PlaceOfResidence],[Gender],[DateOfRegistration],[Idnp],[DocumentNumber],[DateOfIssue]
	  ,[DateOfExpiry],[Status],[BatchId],[StreetId],[RegionId],[StreetName],[StreetNumber],[StreetSubNumber],[BlockNumber],[BlockSubNumber])';
	SET @sql = @sql + '(
	SELECT ' + CAST(@electionDayId AS VARCHAR(10)) + ', v.[NameRo], v.[LastNameRo],v.[PatronymicRo],v.[LastNameRu]
		,v.[NameRu],v.[PatronymicRu],v.[DateOfBirth],v.[PlaceOfBirth],v.[PlaceOfResidence],v.[Gender],v.[DateOfRegistration],v.[Idnp],v.[DocumentNumber],v.[DateOfIssue]
		,v.[DateOfExpiry],v.[Status],v.[BatchId],v.[StreetId],v.[RegionId],v.[StreetName],v.[StreetNumber],v.[StreetSubNumber],v.[BlockNumber],v.[BlockSubNumber] FROM [dbo].[Voter] v	
	--EXCEPT
	--SELECT ' + CAST(@electionDayId AS VARCHAR(10)) + ', ls.[NameRo],ls.[LastNameRo],ls.[PatronymicRo],ls.[LastNameRu]
	--	,ls.[NameRu],ls.[PatronymicRu],ls.[DateOfBirth],ls.[PlaceOfBirth],ls.[PlaceOfResidence],ls.[Gender],ls.[DateOfRegistration],ls.[Idnp],ls.[DocumentNumber],ls.[DateOfIssue]
	--	,ls.[DateOfExpiry],ls.[Status],ls.[BatchId],ls.[StreetId],ls.[RegionId],ls.[StreetName],ls.[StreetNumber],ls.[StreetSubNumber],ls.[BlockNumber],ls.[BlockSubNumber] FROM '+@finalRemoteNameDbo+'.[Voter] as ls
	);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	/* VoterCertificat */
	SET @sql = '
	INSERT INTO ' + @finalRemoteName + '.[VoterCertificat] ([VoterCertificatId],[ElectionDayId],[PollingStationId],[AssignedVoterId],[ReleaseDate],[CertificatNr])';
	SET @sql = @sql + '(SELECT [VoterCertificatId],' + CAST(@electionDayId AS VARCHAR(10)) + ',[PollingStationId],[AssignedVoterId],[ReleaseDate],[CertificatNr]';
	SET @sql = @sql + ' FROM [dbo].[VoterCertificat]);';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	UPDATE ElectionDay
	SET EndDateToReportDb = GETDATE()
	WHERE ElectionDayId = @electionDayId;

	/* Import data from schematmp to schema dbo */  
	SET @finalRemoteNameDbo = '[' + @serverName  + '].[' + @EDayRepositoryDbName + '].[dbo]';
	SET @sql = 'EXEC ' + @finalRemoteNameDbo + '.[MoveDataFromTemp] @electionDayId = ' + CAST(@electionDayId AS VARCHAR(10)) + ';';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);

	SET @execStatus = @success;
	SET @execMsg = 'Baza de date pentru ziua votului a fost migrata cu succes in baza de date de Raportare!';

	RETURN 0;

	Custom_Exception_Fail:
	IF XACT_STATE() <> 0
	BEGIN
		SET @execStatus = @sqlError;
		SET @execMsg = 'procedure MoveDataToRepository: ' + COALESCE(ERROR_PROCEDURE(), '-100')
					+ '; number: ' + CAST(COALESCE(ERROR_NUMBER(), -100) AS VARCHAR(7))
					+ '; line: ' + CAST(COALESCE(ERROR_LINE(), -100) AS VARCHAR(7))
					+ '; state: ' + CAST(COALESCE(ERROR_STATE(), -100) AS VARCHAR(7))
					+ '; severity: ' + CAST(COALESCE(ERROR_SEVERITY(), -100) AS VARCHAR(7))
					+ '; message: ' + COALESCE(ERROR_MESSAGE(), '-');
		RETURN;
	END;
	END;
END TRY
BEGIN CATCH
	SET @execStatus = @sqlError;
	SET @execMsg = 'procedure MoveDataToRepository: ' + COALESCE(ERROR_PROCEDURE(), '-100')
				+ '; number: ' + CAST(COALESCE(ERROR_NUMBER(), -100) AS VARCHAR(7))
				+ '; line: ' + CAST(COALESCE(ERROR_LINE(), -100) AS VARCHAR(7))
				+ '; state: ' + CAST(COALESCE(ERROR_STATE(), -100) AS VARCHAR(7))
				+ '; severity: ' + CAST(COALESCE(ERROR_SEVERITY(), -100) AS VARCHAR(7))
				+ '; message: ' + COALESCE(ERROR_MESSAGE(), '-');

	RETURN;
END CATCH


/****** Object:  StoredProcedure [dbo].[UpdateDataFromTemp]    Script Date: 05.12.2018 13:48:17 ******/
SET ANSI_NULLS ON
