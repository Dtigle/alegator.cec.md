CREATE PROCEDURE [dbo].[GetDataTransferStages]
(
    @serverName	        VARCHAR(500),

	/* execution parameters */
	@execStatus    		INT           OUTPUT,
	@execMsg       		VARCHAR(5000) OUTPUT
)
AS

DECLARE
    @electionDayId		BIGINT,
	@success       INT = 0,
	@sqlError      INT = -2,
	@businessError INT = -1,
	@EDayRepositoryDbName	VARCHAR(50),
	@finalRemoteName	VARCHAR(500),
	@finalRemoteNameDbo	VARCHAR(500),
	@sql NVARCHAR(MAX);

DECLARE @retval int = 0,
            @sysservername sysname;

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    SELECT  @sysservername = CONVERT(sysname, @serverName);
    EXEC @retval = sys.sp_testlinkedserver @sysservername;
END TRY
BEGIN CATCH
    IF EXISTS (select * from sys.servers where name = @serverName)
	BEGIN
    EXEC master.dbo.sp_dropserver @servername, 'droplogins';
	END
END CATCH;  



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

    /* return if parameters are not ok */
	IF (@execStatus = @sqlError OR @execStatus = @businessError)
	BEGIN
		GOTO Custom_Exception_Fail
	END;

    IF NOT EXISTS (select * from sys.servers where name = @serverName)
    BEGIN
		SET @execStatus = @businessError;
		SET @execMsg = 'Linked server-ul asociat bazei de date de Raportare nu a putut fi determinat!';
	    GOTO Custom_Exception_Fail
    END;
    

	SET @finalRemoteName = '[' + @serverName + '].[' + @EDayRepositoryDbName + '].[schematmp]';

	SET @finalRemoteNameDbo = '[' + @serverName + '].[' + @EDayRepositoryDbName + '].[dbo]';

	SET @electionDayId = (SELECT TOP 1 ElectionDayId FROM ElectionDay);


	SET @sql = N'	
	SELECT 
1 as Id,
CAST(''Regiuni'' AS nvarchar(100)) as TableName,
CAST((SELECT COUNT(*)
FROM ' + @finalRemoteName + '.[Region]
WITH (NOLOCK))as decimal) as Processed,
CAST((SELECT COUNT([RegionId])
FROM [dbo].[Region]
WITH (NOLOCK)) as decimal)  as Total
UNION
SELECT
2 as Id,
CAST('+N'N''Lista alegători'''+' AS nvarchar(100)) as TableName,
CAST((SELECT COUNT(*)
FROM ' + @finalRemoteName + '.[Voter]
WITH (NOLOCK))as decimal) as Processed,
CAST((SELECT COUNT([VoterId])
FROM [dbo].[Voter]
WITH (NOLOCK)) as decimal)  as Total
UNION
SELECT 
3 as Id,
CAST('+N'N''Lista alegători asignați la secțiile de votare'''+' AS nvarchar(100)) as TableName,
CAST((SELECT COUNT(*)
FROM 
' + @finalRemoteName + '.[AssignedVoter] as av
WITH (NOLOCK)
WHERE av.[ElectionDayId] = ' + CAST(@electionDayId AS VARCHAR(10)) + '
)as decimal) as Processed,
CAST((SELECT COUNT([VoterId])
FROM [dbo].[AssignedVoter]
WITH (NOLOCK)
) as decimal)  as Total	';
	print 'sql: ' + COALESCE(@sql, '-');
	EXECUTE(@sql);


	SET @execStatus = @success;
	SET @execMsg = 'Procesul de transfer este in progres';

	RETURN 0;

	RETURN 0;

	Custom_Exception_Fail:
	IF XACT_STATE() <> 0
	BEGIN
		SET @execStatus = @sqlError;
		SET @execMsg = 'procedure GetDataTransferStages: ' + COALESCE(ERROR_PROCEDURE(), '-100')
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
	SET @execMsg = 'procedure GetDataTransferStages: ' + COALESCE(ERROR_PROCEDURE(), '-100')
				+ '; number: ' + CAST(COALESCE(ERROR_NUMBER(), -100) AS VARCHAR(7))
				+ '; line: ' + CAST(COALESCE(ERROR_LINE(), -100) AS VARCHAR(7))
				+ '; state: ' + CAST(COALESCE(ERROR_STATE(), -100) AS VARCHAR(7))
				+ '; severity: ' + CAST(COALESCE(ERROR_SEVERITY(), -100) AS VARCHAR(7))
				+ '; message: ' + COALESCE(ERROR_MESSAGE(), '-');

	RETURN;
END CATCH


/****** Object:  StoredProcedure [dbo].[infElectionResult]    Script Date: 05.12.2018 13:44:11 ******/
SET ANSI_NULLS ON
