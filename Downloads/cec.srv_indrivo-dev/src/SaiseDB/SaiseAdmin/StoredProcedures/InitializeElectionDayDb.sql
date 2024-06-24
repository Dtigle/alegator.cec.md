USE [SAISE.Admin2]
GO
/****** Object:  StoredProcedure [SAISE].[InitializeElectionDayDb]    Script Date: 12/8/2023 5:24:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER   PROCEDURE [SAISE].[InitializeElectionDayDb]
(
    /* create DB params */
    @electionDate			DATE,
	
	/* execution parameters */
	@execStatus    			INT           OUTPUT,
	@execMsg       			VARCHAR(5000) OUTPUT
)
AS

declare
 @is_test int=0;

--set @is_test=0; /* 0-prod; 1-test; */

if(@is_test=1) begin
	set @electionDate=isnull(@electionDate,'2023-03-26'); 
	--set @execStatus=0; 
	--set @execMsg='';

	insert into [dbo].[sql_log]([sql_params],[sql_query])
	values('set @electionDate='+quotename(isnull(@electionDate,''))	+'; set @execStatus='+QUOTENAME(isnull(@execStatus,''))+'; set @execMsg='+QUOTENAME(isnull(@execMsg,''))+';','[SAISE].[InitializeElectionDayDb]');
end

DECLARE
	@success		INT = 0,
	@sqlError		INT = -2,
	@businessError	INT = -1;

DECLARE
	@sql               	NVARCHAR(MAX),
	@electionDbName		VARCHAR(50),
	@electionDayId		BIGINT,
	@electionDayAppUid	INT,
	@editUserId			VARCHAR(10),
	@electionDayAppCode VARCHAR(100),
	@appPropDistrictKey	VARCHAR(100),
	@appPropDistrictIds	VARCHAR(100),
	@appPropVillageKey	VARCHAR(100),
	@appPropVillageIds	VARCHAR(100),
	@linkedServerName	VARCHAR(1000),
	@finalRemoteName	VARCHAR(255),
	@finalRemoteNameDbo	VARCHAR(255),
	@electionCount		INT,
	@finalElectionId	BIGINT,
	@varMaxSaiseRoleId	BIGINT,
	@varUserId			VARCHAR(255),
	@varNewUserId		BIGINT,
	@varRegionId		BIGINT,
	@varElectionId		BIGINT,
	@varElectionCircumscriptionId	BIGINT,
	@varPollingStationId	BIGINT,
	@varBallotPaperId	BIGINT,
	@varElectionRoundId	BIGINT,
	@varElectionName	NVARCHAR(255),
	@varElectionName2	NVARCHAR(255),
	@varRoleName		NVARCHAR(255),
	@varPermissisonName	NVARCHAR(255),
	@electionDateString	VARCHAR(255),
	@electionGroupName	NVARCHAR(255),
	@varUserName		NVARCHAR(255),
	@varPassword		NVARCHAR(255),
	@varEmail			NVARCHAR(255),
	@varFirstName		NVARCHAR(255),
	@varLastName		NVARCHAR(255),
	@varIdnp			VARCHAR(255),
	@varUserCreated		DATETIMEOFFSET,
	@varPhone			VARCHAR(255),
	@varElectionCompetitorCiName	NVARCHAR(500),
	@varElectionCompetitorType	CHAR(1),
	@varCFirstNameRo	NVARCHAR(255),
	@varCLastNameRo		NVARCHAR(255),
	@varCiId			BIGINT,
	@electionCompetitorId BIGINT,
	@electionCompetitorType TINYINT,
	@varCPosition		INT;

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
	SET @electionDayAppCode = 'EDay';
	SET @editUserId = '-1';
	SET @appPropDistrictKey = 'DistrictTypeId';
	SET @appPropVillageKey = 'VilageTypeId';

	/* return if technical parameters are not ok */
	IF (@execStatus = @sqlError) BEGIN
		GOTO Custom_Exception_Fail
	END;

	/* validate param */

	IF (@electionDate IS NULL)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Ziua votului nu a fost specificata!';
	    GOTO Custom_Exception_Fail
	END;

	SELECT @electionCount = COUNT([electionRoundId])
	FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = @electionDate;

	IF (@electionCount = 0)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Nu exista niciun scrutin in ziua ' + CAST(@electionDate AS VARCHAR(MAX)) + '!';
	    GOTO Custom_Exception_Fail
	END;

	SET @electionDbName = 'SAISE.ElectionDay' + convert(nvarchar(MAX), @electionDate, 112);
	
	/*IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @electionDbName)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Baza de date ZIUA_VOTULUI nu a fost generata pentru ziua ' + CAST(@electionDate AS VARCHAR(MAX)) + '!';
	    GOTO Custom_Exception_Fail
	END */

    /* return if parameters are not ok */
	IF (@execStatus = @sqlError OR @execStatus = @businessError)
	BEGIN
		GOTO Custom_Exception_Fail
	END;

	/* Insert initial data */

	SET @electionDateString = FORMAT(@electionDate, 'dd.MM.yyyy');

	SELECT @electionDayAppUid = applicationId from [SAISE].[Applications] where code = @electionDayAppCode;

	IF (@electionDayAppUid IS NULL)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Aplicatia ZIUA_VOTULUI nu este configurata in SAISE Admin!';
	    GOTO Custom_Exception_Fail
	END;

	SELECT @linkedServerName = [linkedServerName], @electionDayId = [id] from [SAISE].[ElectionDayDeploy] where CAST([electionDate] AS DATE) = @electionDate;

	IF (@linkedServerName IS NULL)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Linked server-ul asociat bazei de date ZIUA_VOTULUI nu a putut fi determinat!';
	    GOTO Custom_Exception_Fail
	END;

	SELECT @appPropDistrictIds = [value] FROM [SAISE].[ApplicationProperties] WHERE [key] = @appPropDistrictKey;
	SELECT @appPropVillageIds = [value] FROM [SAISE].[ApplicationProperties] WHERE [key] = @appPropVillageKey;

	IF (@appPropDistrictIds IS NULL)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Tipurile de regiuni asociate districtelor nu au fost configurate!';
	    GOTO Custom_Exception_Fail
	END;

	IF (@appPropVillageIds IS NULL)
	BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Tipurile de regiuni asociate localitatilor (village) nu au fost configurate!';
	    GOTO Custom_Exception_Fail
	END;

	IF (@electionCount > 1)
	BEGIN
		SET @finalElectionId = 999999;
	END
	
	IF (@electionCount = 1)
	BEGIN
		SELECT @finalElectionId = [electionId]
		FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = @electionDate;
	END;

	SET @finalRemoteName = '[' + @linkedServerName + '].[' + @electionDbName + '].[schematmp]';

	if(@is_test=0)begin
		/* Delete existing data */
		SET @sql = '';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ElectionResult];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[BallotPaper];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[AssignedVoter];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[AssignedPollingStation];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ElectionCompetitorMember];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[PoliticalPartyStatusOverride];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ElectionCompetitor];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[Election];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ElectionRound];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[AssignedCircumscription];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ElectionType];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[PollingStation];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[Voter];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[AssignedRole];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[AssignedPermission];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[Role];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[Permission];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ElectionDay];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[Region];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[RegionType];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[AuditEventTypes];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ReportParamValues];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ReportParams];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[SystemUser];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[AgeCategories];';
		SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[CircumscriptionRegion];';
		-- DOcument Management Tables
		--SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[Documets];';
		--SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ReportParameterValues];';
		--SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[ReportParameters];';
		--SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[Templates];';
		--SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[TemplateNames];';
		--SET @sql = @sql + 'DELETE FROM ' + @finalRemoteName + '.[TemplateTypes];';

		EXECUTE(@sql);
		print 'sql: ' + COALESCE(@sql, '-');

		/* ElectionDay */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionDay] ([ElectionDayId],[ElectionDayDate],[DeployDbDate],[Name],[Description]) ';
		SET @sql = @sql + '(SELECT [id],[electionDate],[deployDbDate],[eDayName],[dbName]'
		SET @sql = @sql + ' FROM [SAISE].[ElectionDayDeploy] '
		SET @sql = @sql + ' WHERE [id] = ' + CAST(@electionDayId as varchar(10))
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Age Categories */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AgeCategories] ([AgeCategoryId],[From],[To],[Name]) ';
		SET @sql = @sql + '(SELECT [ageCategoryId],[from],[to],[name]';
		SET @sql = @sql + ' FROM [Lookup].[AgeCategories] ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* AuditEventsTypes */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AuditEventTypes] ([auditEventTypeId],[code],[auditStrategy],[name],[description],[EditUserId],[EditDate],[Version]) ';
		SET @sql = @sql + '(SELECT  [auditEventTypeId],[code],[auditStrategy],[name],[description], ' + @editUserId + ', GETDATE(), 1 ';
		SET @sql = @sql + ' FROM [SAISE].[AuditEventTypes] ';
		SET @sql = @sql + ' WHERE [applicationId] = ' + CAST(@electionDayAppUid AS VARCHAR(10));
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* System user */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[SystemUser] ([SystemUserId], [UserName], [Password], [Email], [Level], [Comments], [Idnp], [FirstName], [Surname], [MiddleName], [DateOfBirth], [Gender], ';
		SET @sql = @sql + '[PasswordQuestion], [PasswordAnswer], [IsApproved], [IsOnLine], [IsLockedOut], [CreationDate], [LastActivityDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedAttemptStart], ';
		SET @sql = @sql + '[FailedAnswerStart], [FailedAttemptCount], [FailedAnswerCount], [LastLoginDate], [LastUpdateDate], [Language], [MobileNumber], [ContactName], [ContactMobileNumber], [StreetAddress], ';
		SET @sql = @sql + '[ElectionId], [RegionId], [PollingStationId], [EditUserId], [EditDate], [Version], [IsDeleted]) ';
		SET @sql = @sql + ' VALUES (' + @editUserId + ', N''Saise.Admin'', N''-'', N''-'', ';
		SET @sql = @sql + ' 0, null, ''-'', N''Technical user'', N''-'', null, ''1900.01.01'', 0, ';
		SET @sql = @sql + '	NULL, NULL, 1, 0, 0, GETDATE(), GETDATE(), GETDATE(), GETDATE(), GETDATE(), ';
		SET @sql = @sql + '	GETDATE(), 0, 0, GETDATE(), GETDATE(), ''Romanian'', NULL, NULL, ''-'', NULL, ';
		SET @sql = @sql + ' null, null, -1, ' + @editUserId + ', GETDATE(), 1, 0 ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);
	
		/* Default Pollingstation  */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[PollingStation] ([PollingStationId], [Type], [Number], [SubNumber], [OldName], [NameRo], [NameRu], [RegionId], [StreetId], [StreetNumber], [StreetSubNumber], ';
		SET @sql = @sql + '[EditUserId], [EditDate], [Version], [LocationLatitude], [LocationLongitude], [ExcludeInLocalElections]) ';
		SET @sql = @sql + ' VALUES (-1,0, 999, ';
		SET @sql = @sql + ' null, null,N''Sectie de votare fictivă'', N''Sectie de votare fictivă'', -1, null,  null,  null, ';
		SET @sql = @sql + '' + @editUserId + ', GETDATE(), 1,null, null, 0 ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Permission */
		SET @sql = '' ;
		--SET @sql = @sql + 'IF NOT EXISTS ( SELECT * FROM '+@finalRemoteName+'.[Permission] WHERE [Name] IN (SELECT [code] FROM [Access].[Transactions] WHERE [applicationId] = ' + CAST(@electionDayAppUid AS VARCHAR(10))+' ) ) ';
		SET @sql = @sql + ' BEGIN ';
		SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[Permission] ([PermissionId], [Name], [EditUserId], [EditDate], [Version]) ';
		SET @sql = @sql + '(SELECT [transactionId], [code], ' + @editUserId + ', GETDATE(), 1 ';
		SET @sql = @sql + ' FROM [Access].[Transactions] ';
		SET @sql = @sql + ' WHERE [applicationId] = ' + CAST(@electionDayAppUid AS VARCHAR(10));
		SET @sql = @sql + ');';
		SET @sql = @sql + 'END ';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Role */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[Role] ([RoleId], [Name], [Level], [EditUserId], [EditDate], [Version]) ';
		SET @sql = @sql + '(SELECT [roleId], [name], 1, ' + @editUserId + ', GETDATE(), 1 '
		SET @sql = @sql + ' FROM [Access].[Roles] '
		SET @sql = @sql + ' WHERE [applicationId] = ' + CAST(@electionDayAppUid AS VARCHAR(10))
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		SELECT @varMaxSaiseRoleId = MAX(RoleId)
		FROM [Access].[Roles]
		WHERE [applicationId] = @electionDayAppUid;

		/* Assigned Permission */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedPermission] ([AssignedPermissionId], [RoleId], [PermissionId], [EditUserId], [EditDate], [Version]) ';
		SET @sql = @sql + '(SELECT TRA.[roleTransactionId], TRA.[roleId], TRA.[transactionId], ' + @editUserId + ', GETDATE(), 1 ';
		SET @sql = @sql + ' FROM [Access].[RoleTransactions] TRA ';
		SET @sql = @sql + ' LEFT JOIN [Access].[Transactions] T ON T.[transactionId] = TRA.[transactionId] ';
		SET @sql = @sql + ' WHERE T.[applicationId] = ' + CAST(@electionDayAppUid AS VARCHAR(10));
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Specific roles and permissisons */ 


		/* ElectionRound */
		SET @sql = @sql + '';
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionRound] ([ElectionRoundId],[ElectionId],[Number],[NameRo],[NameRu],[Description],[ElectionDate],[CampaignStartDate],[CampaignEndDate],[Status], [EditUserId], [EditDate], [Version]) ';
		SET @sql = @sql + '(SELECT [electionRoundId],[electionId],[number],[nameRo],[nameRu],[description],[electionDate],[campaignStartDate],[campaignEndDate],[status], ' + @editUserId + ', GETDATE(), 1';
		SET @sql = @sql + '	FROM [SAISE].[ElectionRounds] where CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + '''); ';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* AssignedCircumscription*/
		SET @sql = @sql + '';
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedCircumscription] ([AssignedCircumscriptionId],[ElectionRoundId],[CircumscriptionId],[RegionId],[Number],[NameRo],[isFromUtan],[EditUserId],[EditDate],[Version]) ';
		SET @sql = @sql + '(SELECT ec.[electionCircumscriptionId],ec.[electionRoundId],ec.[circumscriptionId],ec.[regionId],ec.[number],ec.[nameRo], cir.[isFromUtan] , ' + @editUserId + ', GETDATE(), 1';
		SET @sql = @sql + '	FROM [SAISE].[ElectionCircumscriptions] ec';
		SET @sql = @sql + '	INNER JOIN [Lookup].[Circumscriptions] cir ON cir.[circumscriptionId] = ec.[circumscriptionId]';
		SET @sql = @sql + '	WHERE ec.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = ''' +  CAST(@electionDate AS VARCHAR(MAX)) + ''' ));';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);


		/* CircumscriptionRegion */
		SET @sql = @sql + '';
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[CircumscriptionRegion] ([CircumscriptionRegionId],[AssignedCircumscriptionId],[ElectionRoundId],[RegionId]) ';
		SET @sql = @sql + '(SELECT cr.[circumscriptionRegionId],ec.[electionCircumscriptionId],ec.[electionRoundId],cr.[regionId]';
		SET @sql = @sql + '	FROM [SAISE].[ElectionCircumscriptions] ec';
		SET @sql = @sql + '	INNER JOIN [Lookup].[CircumscriptionRegions] cr ON cr.[circumscriptionId] = ec.[circumscriptionId]';
		SET @sql = @sql + '	WHERE ec.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = ''' +  CAST(@electionDate AS VARCHAR(MAX)) + ''' ));';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

					--Insert Data Into Document Management Tables
			/* TemplateTypes */	
			SET @sql = '';
			SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[TemplateTypes] ([TemplateTypeId], [Title], [EditUserId], [EditDate], [Version]) ';
			SET @sql = @sql + 'SELECT [TemplateTypeId], [Title], [EditUserId], [EditDate], [Version] FROM [SAISE].[TemplateTypes];';
		
			PRINT 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			/* TemplateNames */
			SET @sql = '';
			SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[TemplateNames] ([TemplateNameId], [TemplateTypeId], Title, EditUserId, EditDate, [Version]) ';
			SET @sql = @sql + 'SELECT [TemplateNameId], [TemplateTypeId], Title, EditUserId, EditDate, [Version] FROM [SAISE].[TemplateNames];';
		
			PRINT 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			/* Templates */
			SET @sql = '';
			SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[Templates] (TemplateId, Content, UploadDate, TemplateNameId, EditUserId, EditDate, [Version], [ParentId]) ';
			SET @sql = @sql + 'SELECT TemplateId, Content, UploadDate, TemplateNameId, EditUserId, EditDate, [Version], [ParentId] FROM [SAISE].[Templates];';
		
			PRINT 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			/* ReportParameters */
			SET @sql = '';
			SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[ReportParameters] (ReportParameterId, ParameterName, TemplateId, EditUserId, EditDate, IsLookup, [Version], ParameterCode) ';
			SET @sql = @sql + 'SELECT ReportParameterId, ParameterName, TemplateId, EditUserId, EditDate, IsLookup, [Version], ParameterCode FROM [SAISE].[ReportParameters];';
		
			PRINT 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			/* ElectionDuration */
			SET @sql = '';

			SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[ElectionDuration] ([ElectionDurationId],[Name]) ';
			SET @sql = @sql + 'VALUES (1, ''1 zile''), (2, ''2 zile'');';

			PRINT 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			/* TemplateMapping */
			SET @sql = '';

			SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[TemplateMapping] (TemplateNameId, ElectionTypeCode,IsCECE, ElectionRoundCode, OneDay, TwoDayFirstDay, TwoDaySecondDay) ';
			SET @sql = @sql + 'SELECT TemplateNameId, ElectionTypeCode,IsCECE, ElectionRoundCode, OneDay, TwoDayFirstDay, TwoDaySecondDay FROM [SAISE].[TemplateMapping];';

			PRINT 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);



		DECLARE CURSOR_E CURSOR LOCAL FOR
		SELECT E.[electionId], E.[nameRo] 
		FROM [SAISE].[Elections] E
		LEFT JOIN [SAISE].[ElectionRounds] R ON E.[electionId] = R.[electionId]
		WHERE CAST(r.[electionDate] AS DATE) = @electionDate
		GROUP BY E.[electionId], E.[nameRo];

		OPEN CURSOR_E;
		FETCH NEXT FROM CURSOR_E INTO @varElectionId, @varElectionName;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @varElectionName2 = REPLACE(@varElectionName, ' ', '~'); 
			SET @varRoleName = N'RestrictElection~' + @varElectionName2; 
			SET @varPermissisonName = @electionDateString + '~' + @varElectionName2;

			SET @sql = 'IF NOT EXISTS ( SELECT * FROM '+@finalRemoteName+'.[Permission] WHERE [Name] IN (N''' + @varPermissisonName + ''') ) BEGIN ';	
			SET @sql = @sql + 'INSERT INTO ' + @finalRemoteName + '.[Permission] ([PermissionId], [Name], [EditUserId], [EditDate], [Version]) ';
			SET @sql = @sql + ' VALUES (';
			SET @sql = @sql + ' COALESCE((SELECT max([PermissionId]) + 1 FROM ' + @finalRemoteName + '.[Permission]), 1), N''' + @varPermissisonName + ''', ' + @editUserId + ', GETDATE(), 1';
			SET @sql = @sql + ');';
			SET @sql = @sql + ' END ';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[Role] ([RoleId], [Name], [Level], [EditUserId], [EditDate], [Version]) ';
			SET @sql = @sql + ' VALUES (';
			SET @sql = @sql + ' COALESCE((SELECT max([RoleId]) + 1 FROM ' + @finalRemoteName + '.[Role]), 1), N''' + @varRoleName + ''', 1, ' + @editUserId + ', GETDATE(), 1';
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedPermission] ([AssignedPermissionId], [RoleId], [PermissionId], [EditUserId], [EditDate], [Version]) ';
			SET @sql = @sql + ' VALUES (';
			SET @sql = @sql + ' COALESCE((SELECT max([AssignedPermissionId]) + 1 FROM ' + @finalRemoteName + '.[AssignedPermission]), 1), ';
			SET @sql = @sql + ' (SELECT [RoleId] FROM ' + @finalRemoteName + '.[Role] where [Name] = N''' + @varRoleName + '''), ';
			SET @sql = @sql + ' (SELECT [PermissionId] FROM ' + @finalRemoteName + '.[Permission] where [Name] = N''' + @varPermissisonName + '''), ';
			SET @sql = @sql + @editUserId + ', GETDATE(), 1';
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedPermission] ([AssignedPermissionId], [RoleId], [PermissionId], [EditUserId], [EditDate], [Version]) ';
			SET @sql = @sql + '(SELECT ';
			SET @sql = @sql + '		(ROW_NUMBER() OVER (ORDER BY [RoleId]) + (COALESCE((SELECT MAX([AssignedPermissionId]) + 1 FROM ' + @finalRemoteName + '.[AssignedPermission]), 1))), ';
			SET @sql = @sql + '		[RoleId], ';
			SET @sql = @sql + '		(SELECT [PermissionId] FROM ' + @finalRemoteName + '.[Permission] where [Name] = N''' + @varPermissisonName + '''), ';
			SET @sql = @sql + @editUserId + ', GETDATE(), 1';
			SET @sql = @sql + ' FROM ' + @finalRemoteName + '.[Role]';
			SET @sql = @sql + ' WHERE [RoleId] <= ' + CAST(COALESCE(@varMaxSaiseRoleId, -1) AS VARCHAR(MAX)) + ' AND [Name] != ''Administrator'' ';
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			FETCH NEXT FROM CURSOR_E INTO @varElectionId, @varElectionName;
		END;

		CLOSE CURSOR_E;
		DEALLOCATE CURSOR_E;


		--/* District Type - {raion, UTA, municipiu} */
		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[DistrictType] ([DistrictTypeId], [NameRo], [NameRu], [NameDeclinRo], [NameDeclinRu]) ';
		--SET @sql = @sql + 'VALUES (0, N''Necunoscut'', N''Неизвестен'', N''Necunoscut'', N''Неизвестен'');';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[DistrictType] ([DistrictTypeId], [NameRo], [NameRu], [NameDeclinRo], [NameDeclinRu]) ';
		--SET @sql = @sql + '(SELECT [regionTypeId], [name], [name], [description], [description] ';
		--SET @sql = @sql + ' FROM [Lookup].[RegionTypes] ';
		--SET @sql = @sql + ' WHERE [regionTypeId] in (' + @appPropDistrictIds + ') ';
		--SET @sql = @sql + ');';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		/* Region Type */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[RegionType] ([RegionTypeId],[Name],[Description],[Rank],[EditUserId],[EditDate],[Version]) ';
		SET @sql = @sql + '(SELECT [regionTypeId], [name], [description], [rank], ' + @editUserId + ', GETDATE(), 1';
		SET @sql = @sql + ' FROM [Lookup].[RegionTypes] ';
		--SET @sql = @sql + ' WHERE [regionTypeId] in (' + @appPropDistrictIds + ') ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Region */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[Region] ([RegionId], [Name], [NameRu], [Description], [ParentId], [RegionTypeId], [RegistryId], [StatisticCode],	[StatisticIdentifier], [HasStreets], [GeoLatitude], [GeoLongitude], [EditUserId], [EditDate], [Version]) ';
		SET @sql = @sql + '(SELECT [regionId], [name], [nameRu], [description],[parentId],[regionTypeId],[registryId],[statisticCode],[statisticIdentifier],[hasStreets],[geoLatitude],[geoLongitude],' + @editUserId + ', GETDATE(), 1';
		SET @sql = @sql + ' FROM [Lookup].[Regions] R ';
		--SET @sql = @sql + ' WHERE R.[regionTypeId] in (' + @appPropDistrictIds + ') ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* District - {raion, UTA, municipiu} */
		/* todo: de vazut daca exportam toate regiunile, sau doar cele in care exista circumscriptii pentru ziua votului */
		/* todo: de actualizat coloana [Number] */
		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[District] ([DistrictId], [RopUniqueId], [Number], [NameRo], [NameRu], [OldName], [DistrictCouncilSeats], [EditUserId], [EditDate], [Version], [DistrictTypeId]) ';
		--SET @sql = @sql + '(SELECT R.[regionId], COALESCE(R.[registryId], -1), -1, R.[Name], R.[Name], NULL, NULL, ' + @editUserId + ', GETDATE(), 1, R.[regionTypeId] ';
		--SET @sql = @sql + ' FROM [Lookup].[Regions] R ';
		--SET @sql = @sql + ' WHERE R.[regionTypeId] in (' + @appPropDistrictIds + ') ';
		--SET @sql = @sql + ');';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		--SET @sql = 'IF NOT EXISTS (SELECT * FROM ' + @finalRemoteName + '.[District] where DistrictId = -1) BEGIN INSERT INTO ' + @finalRemoteName + '.[District] ([DistrictId], [RopUniqueId], [Number], [NameRo], [NameRu], [OldName], [DistrictCouncilSeats], [EditUserId], [EditDate], [Version], [DistrictTypeId]) ';
		--SET @sql = @sql + 'VALUES (-1, -1, -1, ''TBD'', ''TBD'', ''TBD'', 0, ' + @editUserId + ', GETDATE(), 2, 0); end; ';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		/* Update circumscription number */
		--SET @sql = 'UPDATE D ';
		--SET @sql = @sql + ' SET D.[Number] = COALESCE(EC.[number], -1)';
		--SET @sql = @sql + '	FROM ' + @finalRemoteName + '.[District] D ';
		--SET @sql = @sql + '	LEFT JOIN [SAISE].[ElectionCircumscriptions] EC ON EC.[regionId] = D.[DistrictId] ';
		--SET @sql = @sql + '	WHERE D.[DistrictId] > 0 and EC.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = ''' +  CAST(@electionDate AS VARCHAR(MAX)) + ''' );';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		--/* Village - {sector, oras, orasel, comuna, sat} */
		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[Village] ([VillageId], [RopUniqueId], [Number], [NameRo], [NameRu], [OldName], [Type], [LocalCouncilSeats], [DistrictId], [EditUserId], [EditDate], [Version]) ';
		--SET @sql = @sql + 'VALUES (-1, -1, -1, ''TBD'', ''TBD'', ''TBD'', 0, null, -1, ' + @editUserId + ', GETDATE(), 1);';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[Village] ([VillageId], [RopUniqueId], [Number], [NameRo], [NameRu], [OldName], [Type], [LocalCouncilSeats], [DistrictId], [EditUserId], [EditDate], [Version]) ';
		--SET @sql = @sql + '(SELECT R.[regionId], coalesce(R.[registryId], -1), -1, R.[Name], R.[Name], NULL, R.[regionTypeId], 0, (CASE WHEN P.regionTypeId IN (' + @appPropDistrictIds + ') THEN R.[parentId] ELSE P.[parentId] END ), ' + @editUserId + ', GETDATE(), 1 ';
		--SET @sql = @sql + ' FROM [Lookup].[Regions] R ';
		--SET @sql = @sql + ' LEFT JOIN [Lookup].[Regions] P ON P.regionId = R.parentId ';
		--SET @sql = @sql + ' WHERE R.[regionTypeId] in (' + @appPropVillageIds + ') ';
		--SET @sql = @sql + ');';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		/* Update circumscription number */
		--SET @sql = 'UPDATE V ';
		--SET @sql = @sql + ' SET V.[Number] = D.[Number]';
		--SET @sql = @sql + '	FROM ' + @finalRemoteName + '.[Village] V ';
		--SET @sql = @sql + '	LEFT JOIN ' + @finalRemoteName + '.[District] D ON D.[DistrictId] = V.[DistrictId] ';
		--SET @sql = @sql + '	WHERE D.[DistrictId] IS NOT NULL;';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		--SET @sql = 'UPDATE V ';
		--SET @sql = @sql + ' SET V.[Number] = COALESCE(EC.[number], -1)';
		--SET @sql = @sql + '	FROM ' + @finalRemoteName + '.[Village] V ';
		--SET @sql = @sql + '	LEFT JOIN [SAISE].[ElectionCircumscriptions] EC ON EC.[regionId] = V.[VillageId] ';
		--SET @sql = @sql + '	WHERE V.[VillageId] > 0 AND V.[Number] = -1 AND EC.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = ''' +  CAST(@electionDate AS VARCHAR(MAX)) + ''' );';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		/* Polling station */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[PollingStation] ([PollingStationId], [Type], [Number], [SubNumber], [OldName], [NameRo], [NameRu], [Address], [RegionId], [StreetId], [StreetNumber], [StreetSubNumber], ';
		SET @sql = @sql + '[EditUserId], [EditDate], [Version], [LocationLatitude], [LocationLongitude], [ExcludeInLocalElections]) ';
		SET @sql = @sql + '(SELECT distinct PS.[pollingStationId], PS.[type], PS.[number], NULL, NULL, PS.[nameRo], PS.[nameRu], PS.[address], ';
		SET @sql = @sql + '		PS.[regionId] AS RegionId, NULL, NULL, NULL, ' + @editUserId + ', ';
		SET @sql = @sql + '		GETDATE(), 1, PS.[geoLatitude], PS.[geoLongitude], 0 ';
		SET @sql = @sql + '	FROM [SAISE].[ElectionPollingStations] EPS ';
		SET @sql = @sql + '	LEFT JOIN [SAISE].[PollingStations] PS ON PS.pollingStationId = EPS.pollingStationId ';
		SET @sql = @sql + '	LEFT JOIN [Lookup].[Regions] R ON R.[regionId] = PS.[regionId] ';
		--Comentat DE GIP --
		--SET @sql = @sql + '	WHERE EPS.[electionRoundId] IN (SELECT electionRoundId FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''' )';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Election type */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionType] ([ElectionTypeId],[Code],[TypeName],[Description],[ElectionArea],[ElectionCompetitorType],[ElectionRoundsNo],[AcceptResidenceDoc],[AcceptVotingCert],[AcceptAbroadDeclaration]) ';
		SET @sql = @sql + '(SELECT [electionTypeId],[code],[name],[description],[electionArea],[electionCompetitorType],[electionRoundsNo],[acceptResidenceDoc],[acceptVotingCert],[acceptAbroadDeclaration] ';
		SET @sql = @sql + ' FROM [Lookup].[ElectionTypes] ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		--/* ReportParams */
		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ReportParams] ([reportParamId],[code],[description]) ';
		--SET @sql = @sql + '(SELECT [reportParamId],[code],[description] ';
		--SET @sql = @sql + ' FROM [SAISE].[ReportParams] ';
		--SET @sql = @sql + ');';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		--/* ReportParamValues */
		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ReportParamValues] ([reportParamValueId],[reportParamId],[electionTypeId],[value]) ';
		--SET @sql = @sql + '(SELECT [reportParamValueId],[reportParamId],[electionTypeId],[value] ';
		--SET @sql = @sql + ' FROM [SAISE].[ReportParamValues] ';
		--SET @sql = @sql + ');';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		/* Election */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[Election] ([ElectionId], [Type], [Status], [DateOfElection], [Comments], [EditUserId], [EditDate], [Version], [ReportsPath], [BuletinDateOfElectionRo], [BuletinDateOfElectionRu]) ';
		SET @sql = @sql + '(SELECT E.[electionId], E.[ElectionTypeId], 0, CAST(''' + CAST(@electionDate AS VARCHAR(MAX)) + ''' AS DATE), E.[nameRo], ' + @editUserId + ', GETDATE(), 1, coalesce(E.[reportsPath], ''-''), NULL, NULL ';
		SET @sql = @sql + '	FROM [SAISE].[Elections] E ';
		SET @sql = @sql + '	WHERE E.[electionId] IN (SELECT [electionId] FROM [SAISE].[ElectionRounds] where CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''') ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Custom election group */
		--IF (@electionCount > 1)
		--BEGIN
		--	SET @electionGroupName = 'Grupul de scrutinuri aferent zilei votului ' + @electionDateString;

		--	SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionType] ([ElectionTypeId], [TypeName], [DescriptionRo], [DescriptionRu]) ';
		--	SET @sql = @sql + ' VALUES (9999999, N''Prezenta la vot'', N''Prezenta la vot'', ''Prezenta la vot''); ';
		--	print 'sql: ' + COALESCE(@sql, '-');
		--	EXECUTE(@sql);

		--	SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[Election] ([ElectionId], [Type], [Status], [DateOfElection], [Comments], [EditUserId], [EditDate], [Version], [ReportsPath], [BuletinDateOfElectionRo], [BuletinDateOfElectionRu]) ';
		--	SET @sql = @sql + ' VALUES (' + CAST(@finalElectionId AS VARCHAR(MAX)) + ', 9999999, 0, CAST(''' + CAST(@electionDate AS VARCHAR(MAX)) + ''' AS DATE), N''' + @electionGroupName + ''', ';
		--	SET @sql = @sql + @editUserId + ', GETDATE(), 1, ''-'', NULL, NULL';
		--	SET @sql = @sql + ');';
		--	print 'sql: ' + COALESCE(@sql, '-');
		--	EXECUTE(@sql);		
		--END;

		/* System User */
		/* TODO: de completat valori pentru coloanele FirstName, Surname */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[SystemUser] ([SystemUserId], [UserName], [Password], [Email], [Level], [Comments], [Idnp], [FirstName], [Surname], [MiddleName], [DateOfBirth], [Gender], ';
		SET @sql = @sql + '[PasswordQuestion], [PasswordAnswer], [IsApproved], [IsOnLine], [IsLockedOut], [CreationDate], [LastActivityDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedAttemptStart], ';
		SET @sql = @sql + '[FailedAnswerStart], [FailedAttemptCount], [FailedAnswerCount], [LastLoginDate], [LastUpdateDate], [Language], [MobileNumber], [ContactName], [ContactMobileNumber], [StreetAddress], ';
		SET @sql = @sql + '[ElectionId], [RegionId], [PollingStationId],[CircumscriptionId], [EditUserId], [EditDate], [Version], [IsDeleted]) ';
		SET @sql = @sql + '(SELECT [electionUserId], [username], [passwordHash], ''-'', 0, NULL, 0, ''-'', ''-'', NULL, ''1900.01.01'', 0, ';
		SET @sql = @sql + '		NULL, NULL, 1, 0, 0, GETDATE(), GETDATE(), GETDATE(), GETDATE(), GETDATE(), ';
		SET @sql = @sql + '		GETDATE(), 0, 0, GETDATE(), GETDATE(), ''Romanian'', NULL, NULL, NULL, NULL, ';
		SET @sql = @sql + '		' + CAST(@finalElectionId AS VARCHAR(MAX)) + ', NULL, [pollingStationId], [circumscriptionId], ' + @editUserId + ', GETDATE(), 1, 0 ';
		SET @sql = @sql + '	FROM [SAISE].[ElectionUsers] ';
		SET @sql = @sql + '	WHERE CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''' ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Assigned Role */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedRole] ([RoleId], [SystemUserId], [EditUserId], [EditDate], [Version]) ';
		SET @sql = @sql + '(SELECT [roleId], [electionUserId], ' + @editUserId + ', GETDATE(), 1 ';
		SET @sql = @sql + ' FROM [SAISE].[ElectionUsers] ';
		SET @sql = @sql + ' WHERE CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''' ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* Custom roles */
		DECLARE CURSOR_E CURSOR LOCAL FOR
		SELECT R.[electionRoundId], E.[electionId], E.[nameRo] 
		FROM [SAISE].[Elections] E
		LEFT JOIN [SAISE].[ElectionRounds] R ON E.[electionId] = R.[electionId]
		WHERE CAST(r.[electionDate] AS DATE) = @electionDate;

		OPEN CURSOR_E;
		FETCH NEXT FROM CURSOR_E INTO @varElectionRoundId, @varElectionId, @varElectionName;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @varElectionName2 = REPLACE(@varElectionName, ' ', '~'); 
			SET @varRoleName = 'RestrictElection~' + @varElectionName2; 
			SET @varPermissisonName = @electionDateString + '~' + @varElectionName2;

			/* Asociem rolul @varRoleName tuturor utilizatorilor asociati sectiilor de vot asignate turului de scrutin curent */
			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedRole] ([RoleId], [SystemUserId], [EditUserId], [EditDate], [Version]) ';
			SET @sql = @sql + '(SELECT ';
			SET @sql = @sql + '		(SELECT [RoleId] FROM ' + @finalRemoteName + '.[Role] where [Name] = N''' + @varRoleName + '''), ';
			SET @sql = @sql + '		[electionUserId], ' + @editUserId + ', GETDATE(), 1 ';
			SET @sql = @sql + ' FROM [SAISE].[ElectionUsers] ';
			SET @sql = @sql + ' WHERE CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''' AND [pollingStationId] IN ';
			SET @sql = @sql + '		( SELECT [pollingStationId] from [SAISE].[ElectionPollingStations] where [electionRoundId] = ' + CAST(@varElectionRoundId as VARCHAR(MAX)) + ')';
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			FETCH NEXT FROM CURSOR_E INTO @varElectionRoundId, @varElectionId, @varElectionName;
		END;

		CLOSE CURSOR_E;
		DEALLOCATE CURSOR_E;

		/* Insert nominal users */
		DECLARE CURSOR_U CURSOR LOCAL FOR
		SELECT U.[userId], U.[userName], U.[passwordHash], U.[email], U.[firstName], U.[lastName], U.[idnp], U.[created], U.[phone]   
		FROM [Access].[Users] U
		LEFT JOIN [Access].[UserRoles] UR ON UR.[userId] = U.[userId]
		LEFT JOIN [Access].[Roles] R ON R.[roleId] = UR.[roleId]
		WHERE R.[applicationId] = @electionDayAppUid AND U.[loginLocalDatabase] = 1;

		SELECT @varNewUserId = MAX(electionUserId)
		FROM [SAISE].[ElectionUsers]
		WHERE CAST([electionDate] AS DATE) = @electionDate;

		IF (@varNewUserId IS NULL)
		BEGIN
			SET @varNewUserId = 1;
		END;

		OPEN CURSOR_U;
		FETCH NEXT FROM CURSOR_U INTO @varUserId, @varUserName, @varPassword, @varEmail, @varFirstName, @varLastName, @varIdnp, @varUserCreated, @varPhone;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @varNewUserId = @varNewUserId + 1;
			SET @varRegionId = -1;

			SELECT TOP 1 @varRegionId = C.[regionId]
			FROM [Access].[UserRoles] UR 
			LEFT JOIN [Lookup].[Circumscriptions] C ON C.[circumscriptionId] = UR.[circumscriptionId]
			LEFT JOIN [Access].[Roles] R ON R.[roleId] = UR.[roleId]
			WHERE UR.[userId] = @varUserId AND R.[applicationId] = @electionDayAppUid AND UR.[circumscriptionId] IS NOT NULL; 

			/* System user */
			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[SystemUser] ([SystemUserId], [UserName], [Password], [Email], [Level], [Comments], [Idnp], [FirstName], [Surname], [MiddleName], [DateOfBirth], [Gender], ';
			SET @sql = @sql + '[PasswordQuestion], [PasswordAnswer], [IsApproved], [IsOnLine], [IsLockedOut], [CreationDate], [LastActivityDate], [LastPasswordChangedDate], [LastLockoutDate], [FailedAttemptStart], ';
			SET @sql = @sql + '[FailedAnswerStart], [FailedAttemptCount], [FailedAnswerCount], [LastLoginDate], [LastUpdateDate], [Language], [MobileNumber], [ContactName], [ContactMobileNumber], [StreetAddress], ';
			SET @sql = @sql + '[ElectionId], [RegionId], [PollingStationId], [CircumscriptionId], [EditUserId], [EditDate], [Version], [IsDeleted]) ';
			SET @sql = @sql + ' VALUES (' + CAST(@varNewUserId AS VARCHAR(MAX)) + ', N''' + coalesce(@varUserName, ' ') + ''', N''' + coalesce(@varPassword, ' ') + ''', N''' + coalesce(@varEmail, ' ')  + ''', ';
			SET @sql = @sql + ' 0, null, ''' + coalesce(@varIdnp, ' ') + ''', N''' +coalesce(@varFirstName, ' ') + ''', N''' + coalesce(@varLastName, ' ') + ''', null, ''1900.01.01'', 0, ';
			SET @sql = @sql + '	NULL, NULL, 1, 0, 0, GETDATE(), GETDATE(), GETDATE(), GETDATE(), GETDATE(), ';
			SET @sql = @sql + '	GETDATE(), 0, 0, GETDATE(), GETDATE(), ''Romanian'', NULL, NULL, ''' + coalesce(@varPhone, ' ') + ''', NULL, ';
			SET @sql = @sql + CAST(@finalElectionId AS VARCHAR(MAX)) + ', ' + CAST((COALESCE(@varRegionId, -1)) AS VARCHAR(MAX)) + ', null, null, ' + @editUserId + ', GETDATE(), 1, 0 ';
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			/* Assigned Role */
			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedRole] ([RoleId], [SystemUserId], [EditUserId], [EditDate], [Version]) ';
			SET @sql = @sql + '(SELECT UR.[roleId], ' + CAST(@varNewUserId AS VARCHAR(MAX))  + ' , ' + @editUserId + ', GETDATE(), 1 ';
			SET @sql = @sql + ' FROM [Access].[UserRoles] UR ';
			SET @sql = @sql + ' LEFT JOIN [Access].[Roles] R ON R.[roleId] = UR.[roleId] ';
			SET @sql = @sql + ' WHERE UR.[userId] = ''' + CAST(@varUserId AS VARCHAR(MAX)) + ''' AND R.[applicationId] = ' + CAST(@electionDayAppUid AS VARCHAR(MAX));
			SET @sql = @sql + '		AND UR.[validFrom] <= ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''' AND (UR.[validTo] IS NULL OR UR.[validTo] > ''' +  CAST(@electionDate AS VARCHAR(MAX)) + ''')';
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			FETCH NEXT FROM CURSOR_U INTO @varUserId, @varUserName, @varPassword, @varEmail, @varFirstName, @varLastName, @varIdnp, @varUserCreated, @varPhone;
		END;

		CLOSE CURSOR_U;
		DEALLOCATE CURSOR_U;

		/* Update system users - set RegionId */
		SET @sql = 'UPDATE U ';
		SET @sql = @sql + ' SET U.[RegionId] = PS.[RegionId]';
		SET @sql = @sql + '	FROM ' + @finalRemoteName + '.[SystemUser] U ';
		SET @sql = @sql + '	LEFT JOIN ' + @finalRemoteName + '.[PollingStation] PS ON PS.[PollingStationId] = U.[PollingStationId] ';
		SET @sql = @sql + '	WHERE U.[PollingStationId] IS NOT NULL;';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* AssignedPollingStation */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[AssignedPollingStation] ([AssignedPollingStationId], [ElectionRoundId], [AssignedCircumscriptionId], [PollingStationId], [Type], [Status], [IsOpen], [OpeningVoters], [EstimatedNumberOfVoters], ';
		SET @sql = @sql + '[NumberOfRoBallotPapers], [NumberOfRuBallotPapers], [ImplementsEVR], [EditUserId], [EditDate], [Version], [isOpeningEnabled], [isTurnoutEnabled], [isElectionResultEnabled],[numberPerElection],[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu], [ElectionDurationId]) ';
		SET @sql = @sql + '(SELECT [electionPollingStationId], [electionRoundId], [electionCircumscriptionId], [pollingStationId], [type], 0, 0, 0, 0, ';
		SET @sql = @sql + '		0, 0, 1, ' + @editUserId + ', GETDATE(), 1, 0, 0, 0,[numberPerElection],[regionId],[dbo].[fn_GetParentRegion]([regionId]),[circulationRo],[circulationRu], 1';
		SET @sql = @sql + ' FROM [SAISE].[ElectionPollingStations] EPS ';
		SET @sql = @sql + ' WHERE [electionRoundId] IN (SELECT [SAISE].[ElectionRounds].[electionRoundId] FROM [SAISE].[ElectionRounds] WHERE CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX))  + ''') ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		--/* ElectionCompetitor */
		--SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitor] ([ElectionCompetitorId], [PoliticalPartyId], [ElectionRoundId], [AssignedCircumscriptionId], [Code], [NameRo], [NameRu], [ColorLogo], [DateOfRegistration], [Status], [IsIndependent], [BallotOrder], [EditUserId], [EditDate], [Version], [PartyOrder], ';
		--SET @sql = @sql + '[DisplayFromNameRo], [DisplayFromNameRu], [RegistryNumber], [BlackWhiteLogo], [PartyType], [BallotPaperNameRo], [BallotPaperNameRu], [BallotPapperCustomCssRo], [BallotPapperCustomCssRu])';
		--SET @sql = @sql + '(SELECT EC.[electionCompetitorId], EC.[politicalPartyId], EC.[electionRoundId], EC.[electionCircumscriptionId], PP.[shortName], PP.[nameRo], COALESCE(PP.[NameRu], ''-''), NULL, PP.[registrationDate], 2, 0, EC.[position], ' + @editUserId + ', GETDATE(), 1, -1, ';
		--SET @sql = @sql + '		PP.[nameRo], PP.[NameRu], PP.[registryNumber], NULL, 0, NULL, NULL, NULL, NULL ';
		--SET @sql = @sql + '	FROM [SAISE].[ElectionCompetitors] EC ';
		--SET @sql = @sql + '	INNER JOIN [SAISE].[PoliticalParties] PP ON PP.[politicalPartyId] = EC.[politicalPartyId] ';
		--SET @sql = @sql + '	WHERE EC.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] where CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''') ';
		--SET @sql = @sql + ');';
		--print 'sql: ' + COALESCE(@sql, '-');
		--EXECUTE(@sql);

		--/* ElectionCompetitor*/
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitor] ([ElectionCompetitorId], [PoliticalPartyId], [ElectionRoundId], [AssignedCircumscriptionId], [Code], [NameRo], [NameRu], [ColorLogo], [DateOfRegistration], [Status], [IsIndependent], [BallotOrder], [EditUserId], [EditDate], [Version], [PartyOrder], ';
		SET @sql = @sql + '[DisplayFromNameRo], [DisplayFromNameRu], [RegistryNumber], [BlackWhiteLogo], [PartyType], [BallotPaperNameRo], [BallotPaperNameRu], [BallotPapperCustomCssRo], [BallotPapperCustomCssRu],[Color])';
		SET @sql = @sql + '(SELECT EC.[electionCompetitorId], EC.[politicalPartyId], EC.[electionRoundId], EC.[electionCircumscriptionId], PP.[shortName], PP.[nameRo], COALESCE(PP.[NameRu], ''-''), ISNULL(EC.[colorLogo],(SELECT TOP 1 pp.colorLogo FROM SAISE.PoliticalParties pp WHERE pp.politicalPartyId = EC.politicalPartyId)) , PP.[registrationDate], 2, 0, EC.[position], ' + @editUserId + ', GETDATE(), 1, -1, ';
		SET @sql = @sql + 'PP.[nameRo], PP.[NameRu], PP.[registryNumber], EC.[blackWhiteLogo], ';
		SET @sql = @sql + '	 CASE WHEN (E.electionTypeId=1 OR E.electionTypeId=3) THEN 1 else 0 end, ';
		SET @sql = @sql + '	 NULL, NULL, NULL, NULL, [color]';
		SET @sql = @sql + ' FROM [SAISE].[ElectionCompetitors] EC ';
		SET @sql = @sql + '	INNER JOIN [SAISE].[PoliticalParties] PP ON PP.[politicalPartyId] = EC.[politicalPartyId] ';
		SET @sql = @sql + '	LEFT JOIN [SAISE].[ElectionRounds] ER ON ER.[electionRoundId] = EC.[electionRoundId] ';
		SET @sql = @sql + '	LEFT JOIN [SAISE].[Elections] E ON E.[electionId] = ER.[electionId] ';
   
		SET @sql = @sql + '	WHERE EC.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] where CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''') ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);

		/* ElectionCompetitorMember */
		SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitorMember] ([ElectionCompetitorMemberId], [AssignedCircumscriptionId], [ElectionRoundId], [LastNameRo], [LastNameRu], [NameRo], [NameRu], [PatronymicRo], [PatronymicRu], [DateOfBirth], [PlaceOfBirth], [Gender], [Occupation], ';
		SET @sql = @sql + ' [OccupationRu], [Designation], [DesignationRu], [Workplace], [WorkplaceRu], [Idnp], ';
		SET @sql = @sql + ' [ElectionCompetitorId], [DateOfRegistration], [ColorLogo], [BlackWhiteLogo], [Picture], [Status], [CompetitorMemberOrder], [EditUserId], [EditDate], [Version])';
		SET @sql = @sql + '(SELECT M.[electionCompetitorMemberId], EC.[electionCircumscriptionId], EC.[electionRoundId], M.[lastNameRo], M.[lastNameRu], M.[firstNameRo], M.[firstNameRu], M.[patronymic], M.[patronymic], M.[birthDate], ''-'' , M.[gender], M.[occupationRo], ';
		SET @sql = @sql + '		M.[occupationRu], M.[designationRo], M.[designationRu], M.[workplaceRo], M.[workplaceRu], M.[idnp], ';
		SET @sql = @sql + '		(case when pp.politicalPartyId is not null then M.[electionCompetitorId] else null end) as [electionCompetitorId], ';
		SET @sql = @sql + '		M.[created], M.[colorLogo], M.[blackWhiteLogo], M.[picture], 0, M.[position], ' + @editUserId + ', GETDATE(), 1 ';
		SET @sql = @sql + ' FROM [SAISE].[ElectionCompetitorMembers] M ';
		SET @sql = @sql + ' LEFT JOIN [SAISE].[ElectionCompetitors] EC ON EC.[electionCompetitorId] = M.[electionCompetitorId] ';
		SET @sql = @sql + ' LEFT JOIN [SAISE].[PoliticalParties] PP ON PP.[politicalPartyId] = EC.[politicalPartyId] ';
		SET @sql = @sql + ' WHERE EC.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] where CAST([electionDate] AS DATE) = ''' + CAST(@electionDate AS VARCHAR(MAX)) + ''') AND EC.[politicalPartyId] IS NOT NULL ';
		SET @sql = @sql + ');';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);




		--/* todo: Inseram candidatii independenti */
		DECLARE CURSOR_CI CURSOR LOCAL FOR
		SELECT 
		EC.[electionCompetitorId],
		EC.[electionRoundId],
		EC.[type],
		EC.[nameRo],
		ISNULL(EC.[electionCircumscriptionId], 0),
		EC.[position] 
		FROM [SAISE].[ElectionCompetitors] EC
		WHERE  EC.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] where CAST([electionDate] AS DATE) = CAST(@electionDate AS DATE)) AND EC.[politicalPartyId] IS NULL;
	
		OPEN CURSOR_CI;
		FETCH NEXT FROM CURSOR_CI INTO @electionCompetitorId, @varElectionRoundId, @electionCompetitorType, @varElectionCompetitorCiName, @varElectionCircumscriptionId, @varCPosition;
		WHILE @@FETCH_STATUS = 0
		BEGIN

			SET @varElectionCompetitorCiName = CASE WHEN (@electionCompetitorType = 1) THEN (SELECT TOP 1 CONCAT('Candidat independent ',M.[firstNameRo],' ',M.[lastNameRo]) FROM [SAISE].[ElectionCompetitorMembers] M WHERE M.electionCompetitorId =  @electionCompetitorId) else @varElectionCompetitorCiName  end;

			SET @varElectionCompetitorType = CASE WHEN (@electionCompetitorType = 1) THEN 'i' else 'r' end;

			/* Insert Election Competitor */
			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitor] ([ElectionCompetitorId],  [ElectionRoundId], [AssignedCircumscriptionId], [Code], [NameRo], [NameRu], [ColorLogo], [DateOfRegistration], [Status], [IsIndependent], [BallotOrder], [EditUserId], [EditDate], [Version], [PartyOrder], ';
			SET @sql = @sql + ' [DisplayFromNameRo], [DisplayFromNameRu], [RegistryNumber], [BlackWhiteLogo], [PartyType], [BallotPaperNameRo], [BallotPaperNameRu], [BallotPapperCustomCssRo], [BallotPapperCustomCssRu], [Color])';
			SET @sql = @sql + '(SELECT EC.[electionCompetitorId], EC.[electionRoundId], '+ CASE WHEN (@varElectionCircumscriptionId=0) THEN 'NULL' else cast(@varElectionCircumscriptionId AS VARCHAR(MAX))  end +', ''' + @varElectionCompetitorType +''', N'''+ @varElectionCompetitorCiName +''', N''' + @varElectionCompetitorCiName + ''', EC.[colorLogo], GETDATE(), 2, 1, EC.[position]' + ', ' + @editUserId + ', GETDATE(), 1, -1, ';
			SET @sql = @sql + '	N''' + @varElectionCompetitorCiName + ''', N''' + @varElectionCompetitorCiName + ''', null, EC.[blackWhiteLogo], 0, null, null, null, null, EC.[color] ';
			SET @sql = @sql + ' FROM [SAISE].[ElectionCompetitors] EC ';
			SET @sql = @sql + ' WHERE EC.[electionCompetitorId] = ' + CAST(@electionCompetitorId AS VARCHAR(MAX));
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);


			/* Insert Election Competitor Member */
			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitorMember] ([ElectionCompetitorMemberId], [AssignedCircumscriptionId], [ElectionRoundId],  [LastNameRo], [LastNameRu], [NameRo], [NameRu], [PatronymicRo], [PatronymicRu], [DateOfBirth], [PlaceOfBirth], [Gender], [Occupation], ';
			SET @sql = @sql + ' [OccupationRu], [Designation], [DesignationRu], [Workplace], [WorkplaceRu], [Idnp],';
			SET @sql = @sql + ' [ElectionCompetitorId], [DateOfRegistration], [ColorLogo], [BlackWhiteLogo], [Picture], [Status], [CompetitorMemberOrder], [EditUserId], [EditDate], [Version])';
			SET @sql = @sql + '(SELECT M.[electionCompetitorMemberId], ' + CASE WHEN (@varElectionCircumscriptionId=0) THEN 'NULL' else cast(@varElectionCircumscriptionId AS VARCHAR(MAX))  end + ',' + cast(@varElectionRoundId AS VARCHAR(MAX)) + ', M.[lastNameRo], M.[lastNameRu], M.[firstNameRo], M.[firstNameRu], M.[patronymic], M.[patronymic], M.[birthDate], COALESCE(M.[placeOfBirth], ''-''), M.[gender], M.[occupationRo], ';
			SET @sql = @sql + '		M.[occupationRu], M.[designationRo], M.[designationRu], M.[workplaceRo], M.[workplaceRu], M.[idnp], ';
			SET @sql = @sql + '		' + cast(@electionCompetitorId AS VARCHAR(MAX)) + ', ';
			SET @sql = @sql + '		M.[created], M.[colorLogo], M.[blackWhiteLogo], M.[picture], 0, M.[position], ' + @editUserId + ', GETDATE(), 1 ';
			SET @sql = @sql + ' FROM [SAISE].[ElectionCompetitorMembers] M ';
			SET @sql = @sql + ' WHERE M.[electionCompetitorId] = ' + CAST(@electionCompetitorId AS VARCHAR(MAX));
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);

			FETCH NEXT FROM CURSOR_CI INTO @electionCompetitorId, @varElectionRoundId, @electionCompetitorType, @varElectionCompetitorCiName, @varElectionCircumscriptionId, @varCPosition;
		END;



		CLOSE CURSOR_CI;
		DEALLOCATE CURSOR_CI;

			/* todo: Inseram referendum */
		--DECLARE CURSOR_R CURSOR LOCAL FOR
	 --   SELECT 
		--M.[electionCompetitorMemberId], 
		--M.[electionCompetitorId],
		--EC.[electionRoundId],
		--EC.[electionCircumscriptionId],
		--M.[firstNameRo], 
		--M.[lastNameRo], 
		--EC.[position] 
	 --   FROM [SAISE].[ElectionCompetitorMembers] M 
		--INNER JOIN [SAISE].[ElectionCompetitors] EC ON EC.[electionCompetitorId] = M.[electionCompetitorId]
		--WHERE EC.[electionRoundId] IN (SELECT [electionRoundId] FROM [SAISE].[ElectionRounds] 	where CAST([electionDate] AS DATE) = CAST(@electionDate AS DATE)) AND EC.[politicalPartyId] IS NULL
		--AND EC.type = 3;

		--OPEN CURSOR_R;
	 --   FETCH NEXT FROM CURSOR_R INTO @varCiId, @electionCompetitorId, @varElectionRoundId, @varElectionCircumscriptionId, @varCFirstNameRo, @varCLastNameRo, @varCPosition;
		--WHILE @@FETCH_STATUS = 0
	 --   BEGIN

		--	SET @varElectionCompetitorCiName = 'Referendum';

		--	/* Insert Election Competitor */
		--	SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitor] ([ElectionCompetitorId],  [ElectionRoundId], [AssignedCircumscriptionId], [Code], [NameRo], [NameRu], [ColorLogo], [DateOfRegistration], [Status], [IsIndependent], [BallotOrder], [EditUserId], [EditDate], [Version], [PartyOrder], ';
		--	SET @sql = @sql + ' [DisplayFromNameRo], [DisplayFromNameRu], [RegistryNumber], [BlackWhiteLogo], [PartyType], [BallotPaperNameRo], [BallotPaperNameRu], [BallotPapperCustomCssRo], [BallotPapperCustomCssRu])';
		--	SET @sql = @sql + ' VALUES (' + cast(@electionCompetitorId AS VARCHAR(MAX)) + ',' + cast(@varElectionRoundId AS VARCHAR(MAX)) +','+ cast(@varElectionCircumscriptionId AS VARCHAR(MAX)) +', ''r'', N'''+ @varElectionCompetitorCiName +''', N''' + @varElectionCompetitorCiName + ''', null, GETDATE(), 2, 1, ';
		--	SET @sql = @sql + ' ' + cast(@varCPosition AS VARCHAR(MAX)) + ', ' + @editUserId + ', GETDATE(), 1, -1, ';
		--	SET @sql = @sql + '	N''' + @varElectionCompetitorCiName + ''', N''' + @varElectionCompetitorCiName + ''', null, null, 0, null, null, null, null);';
		--	print 'sql: ' + COALESCE(@sql, '-');
		--	EXECUTE(@sql);

		--	/* Insert Election Competitor Member */
		--	SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionCompetitorMember] ([ElectionCompetitorMemberId], [AssignedCircumscriptionId], [ElectionRoundId],  [LastNameRo], [LastNameRu], [NameRo], [NameRu], [PatronymicRo], [PatronymicRu], [DateOfBirth], [PlaceOfBirth], [Gender], [Occupation], ';
		--	SET @sql = @sql + ' [OccupationRu], [Designation], [DesignationRu], [Workplace], [WorkplaceRu], [Idnp],';
		--	SET @sql = @sql + ' [ElectionCompetitorId], [DateOfRegistration], [ColorLogo], [BlackWhiteLogo], [Status], [CompetitorMemberOrder], [EditUserId], [EditDate], [Version])';
		--	SET @sql = @sql + '(SELECT M.[electionCompetitorMemberId], ' + cast(@varElectionCircumscriptionId AS VARCHAR(MAX)) + ',' + cast(@varElectionRoundId AS VARCHAR(MAX)) + ', M.[lastNameRo], M.[lastNameRu], M.[firstNameRo], M.[firstNameRu], M.[patronymic], M.[patronymic], M.[birthDate], COALESCE(M.[placeOfBirth], ''-''), M.[gender], M.[occupationRo], ';
		--	SET @sql = @sql + '		M.[occupationRu], M.[designationRo], M.[designationRu], M.[workplaceRo], M.[workplaceRu], M.[idnp], ';
		--	SET @sql = @sql + '		' + cast(@electionCompetitorId AS VARCHAR(MAX)) + ', ';
		--	SET @sql = @sql + '		M.[created], NULL, NULL, 0, M.[position], ' + @editUserId + ', GETDATE(), 1 ';
		--	SET @sql = @sql + ' FROM [SAISE].[ElectionCompetitorMembers] M ';
		--	SET @sql = @sql + ' WHERE M.[electionCompetitorMemberId] = ' + CAST(@varCiId AS VARCHAR(MAX));
		--	SET @sql = @sql + ');';
		--	print 'sql: ' + COALESCE(@sql, '-');
		--	EXECUTE(@sql);

		--	FETCH NEXT FROM CURSOR_R INTO  @varCiId, @electionCompetitorId, @varElectionRoundId, @varElectionCircumscriptionId, @varCFirstNameRo, @varCLastNameRo, @varCPosition;
	 --   END;

		--CLOSE CURSOR_R;
	 --   DEALLOCATE CURSOR_R;

		/* BallotPaper, ElectionResult */
		DECLARE CURSOR_E CURSOR LOCAL FOR
		SELECT 
		PS.[pollingStationId], 
		ER.[electionRoundId], 
		ER.[electionId]
		FROM [SAISE].[ElectionPollingStations] PS 
		LEFT JOIN [SAISE].[ElectionRounds] ER ON ER.[electionRoundId] = PS.[electionRoundId]
		WHERE CAST(ER.[electionDate] AS DATE) = @electionDate;

		SET @varBallotPaperId = 0;

		OPEN CURSOR_E;
		FETCH NEXT FROM CURSOR_E INTO @varPollingStationId, @varElectionRoundId, @varElectionId;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @varBallotPaperId = @varBallotPaperId + 1;

			SET @varElectionCompetitorType = (SELECT TOP 1 ET.electionCompetitorType FROM [Lookup].[ElectionTypes] ET, [SAISE].[Elections] E WHERE E.electionTypeId = ET.electionTypeId AND E.electionId = @varElectionId);

			/* Insert ballot paper */
			SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[BallotPaper] ([BallotPaperId], [EntryLevel], [Type], [Status], [RegisteredVoters], [Supplementary], [BallotsIssued], [BallotsCasted], ';
			SET @sql = @sql + ' [DifferenceIssuedCasted], [BallotsValidVotes], [BallotsReceived], [BallotsUnusedSpoiled], [BallotsSpoiled], [BallotsUnused], [Description], [Comments], [DateOfEntry], ';
			SET @sql = @sql + ' [VotingPointId], [PollingStationId], [ElectionRoundId], [EditUserId], [EditDate], [IsResultsConfirmed], [ConfirmationUserId], [ConfirmationDate], [Version]) ';
			SET @sql = @sql + ' VALUES (' + CAST(@varBallotPaperId AS VARCHAR(MAX)) + ' , -1, 0, 0, 0, 0, 0, 0, ';
			SET @sql = @sql + '		0, 0, 0, 0, 0, 0, null, null, GETDATE(),';
			SET @sql = @sql + '		null, ' + cast(@varPollingStationId AS VARCHAR(MAX)) + ', ' + CAST(@varElectionRoundId AS VARCHAR(MAX)) + ', ' + @editUserId + ', GETDATE(), 0, null, null, 1 ';
			SET @sql = @sql + ');';
			print 'sql: ' + COALESCE(@sql, '-');
			EXECUTE(@sql);


			IF(@varElectionCompetitorType = 1 OR @varElectionCompetitorType = 3)
			BEGIN
				/* Insert ElectionResult */  
				SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionResult] ([ElectionRoundId], [BallotOrder], [BallotCount], [Comments], [DateOfEntry], [Status], [ElectionCompetitorId], [ElectionCompetitorMemberId], ';
				SET @sql = @sql + ' [BallotPaperId], [EditUserId], [EditDate], [Version]) ';
				SET @sql = @sql + '(SELECT C.[ElectionRoundId], C.[CompetitorMemberOrder], 0, '''', GETDATE(), 0, C.[ElectionCompetitorId], C.[ElectionCompetitorMemberId], ' ;
				SET @sql = @sql + '		' + CAST(@varBallotPaperId AS VARCHAR(MAX)) + ', ' + @editUserId + ', GETDATE(), 1';
				SET @sql = @sql + ' FROM ' + @finalRemoteName + '.[ElectionCompetitorMember] C ';
				SET @sql = @sql + ' WHERE C.[ElectionRoundId] = ' + CAST(@varElectionRoundId AS VARCHAR(MAX))+' AND EXISTS (SELECT 1 FROM ' + @finalRemoteName + '.[AssignedPollingStation] APS WHERE  APS.PollingStationId = ' + cast(@varPollingStationId AS VARCHAR(20)) + ' AND (C.AssignedCircumscriptionId IS NULL OR C.AssignedCircumscriptionId =  APS.AssignedCircumscriptionId) ) ';
				SET @sql = @sql + ');';
				print 'sql: ' + COALESCE(@sql, '-');
				EXECUTE(@sql);
			END
			ELSE
				BEGIN
				/* Insert ElectionResult */  
				SET @sql = 'INSERT INTO ' + @finalRemoteName + '.[ElectionResult] ([ElectionRoundId], [BallotOrder], [BallotCount], [Comments], [DateOfEntry], [Status], [ElectionCompetitorId], [ElectionCompetitorMemberId], ';
				SET @sql = @sql + ' [BallotPaperId], [EditUserId], [EditDate], [Version]) ';
				SET @sql = @sql + '(SELECT C.[ElectionRoundId], C.[BallotOrder], 0, '''', GETDATE(), 0, C.[ElectionCompetitorId], NULL, ' ;
				SET @sql = @sql + '		' + CAST(@varBallotPaperId AS VARCHAR(MAX)) + ', ' + @editUserId + ', GETDATE(), 1';
				SET @sql = @sql + ' FROM ' + @finalRemoteName + '.[ElectionCompetitor] C ';
				SET @sql = @sql + ' WHERE C.[ElectionRoundId] = ' + CAST(@varElectionRoundId AS VARCHAR(MAX))+' AND EXISTS (SELECT 1 FROM ' + @finalRemoteName + '.[AssignedPollingStation] APS WHERE  APS.PollingStationId = ' + cast(@varPollingStationId AS VARCHAR(20)) + ' AND (C.AssignedCircumscriptionId IS NULL OR C.AssignedCircumscriptionId =  APS.AssignedCircumscriptionId) ) ';
				SET @sql = @sql + ');';
				print 'sql: ' + COALESCE(@sql, '-');
				EXECUTE(@sql);
			END

			FETCH NEXT FROM CURSOR_E INTO @varPollingStationId, @varElectionRoundId, @varElectionId;
		END;

		CLOSE CURSOR_E;
		DEALLOCATE CURSOR_E;

		/* todo: PoliticalPartyStatusOverride */
		/* todo: PV_LOG_SEARCH_IDNP */
		/* todo: PV_LOG_VERIFY_ERRORS */

		/* Voter, AssignedVoter - import from RSA, not from SAISE.Admin */


		/* Import data from schematmp to schema dbo */  
		SET @finalRemoteNameDbo = '[' + @linkedServerName + '].[' + @electionDbName + '].[dbo]';
		SET @sql = 'EXEC ' + @finalRemoteNameDbo + '.[MoveDataFromTemp];';
		print 'sql: ' + COALESCE(@sql, '-');
		EXECUTE(@sql);


	END

	SET @execStatus = @success;
	SET @execMsg = 'Baza de date pentru ziua votului ' + CAST(@electionDate AS VARCHAR(MAX)) + ' a fost initializata cu succes!';

	RETURN 0;

	Custom_Exception_Fail:
	IF XACT_STATE() <> 0
	BEGIN
		SET @execStatus = @sqlError;
		SET @execMsg = 'procedure InitializeElectionDayDb: ' + COALESCE(ERROR_PROCEDURE(), '-100')
					+ '; number: ' + CAST(COALESCE(ERROR_NUMBER(), -100) AS VARCHAR(7))
					+ '; line: ' + CAST(COALESCE(ERROR_LINE(), -100) AS VARCHAR(7))
					+ '; state: ' + CAST(COALESCE(ERROR_STATE(), -100) AS VARCHAR(7))
					+ '; severity: ' + CAST(COALESCE(ERROR_SEVERITY(), -100) AS VARCHAR(7))
					+ '; message: ' + COALESCE(ERROR_MESSAGE(), '-');
		RETURN;
	END;

END TRY
BEGIN CATCH
		SET @execStatus = @sqlError;
		SET @execMsg = 'procedure InitializeElectionDayDb: ' + COALESCE(ERROR_PROCEDURE(), '-100')
					+ '; number: ' + CAST(COALESCE(ERROR_NUMBER(), -100) AS VARCHAR(7))
					+ '; line: ' + CAST(COALESCE(ERROR_LINE(), -100) AS VARCHAR(7))
					+ '; state: ' + CAST(COALESCE(ERROR_STATE(), -100) AS VARCHAR(7))
					+ '; severity: ' + CAST(COALESCE(ERROR_SEVERITY(), -100) AS VARCHAR(7))
					+ '; message: ' + COALESCE(ERROR_MESSAGE(), '-');
END CATCH
print 'exec message:' + @execMsg;