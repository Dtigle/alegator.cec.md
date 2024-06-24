USE [master];
ALTER DATABASE model SET RECOVERY SIMPLE;
USE [master]
GO
IF  EXISTS (SELECT name FROM sys.databases WHERE name = '$(dbname)')
BEGIN
	ALTER DATABASE [$(dbname)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [$(dbname)]
END
GO

CREATE DATABASE [$(dbname)]
GO
ALTER DATABASE [$(dbname)] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [$(dbname)].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [$(dbname)] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [$(dbname)] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [$(dbname)] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [$(dbname)] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [$(dbname)] SET ARITHABORT OFF 
GO
ALTER DATABASE [$(dbname)] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [$(dbname)] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [$(dbname)] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [$(dbname)] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [$(dbname)] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [$(dbname)] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [$(dbname)] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [$(dbname)] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [$(dbname)] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [$(dbname)] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [$(dbname)] SET  DISABLE_BROKER 
GO
ALTER DATABASE [$(dbname)] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [$(dbname)] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [$(dbname)] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [$(dbname)] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [$(dbname)] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [$(dbname)] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [$(dbname)] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [$(dbname)] SET RECOVERY FULL 
GO
ALTER DATABASE [$(dbname)] SET  MULTI_USER 
GO
ALTER DATABASE [$(dbname)] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [$(dbname)] SET DB_CHAINING OFF 
GO
ALTER DATABASE [$(dbname)] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [$(dbname)] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [$(dbname)]
GO
/****** Object:  Schema [Audit]    Script Date: 23.10.2018 16:24:24 ******/
CREATE SCHEMA [Audit]
GO
/****** Object:  Schema [schematmp]    Script Date: 23.10.2018 16:24:24 ******/
CREATE SCHEMA [schematmp]
GO


/****** Object:  StoredProcedure [dbo].[ElectionResultsByParams]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ElectionResultsByParams]
(
	@ElectionRoundId BIGINT,
	@CircumscriptionId BIGINT,
	@PollingStationId BIGINT
)
AS
SELECT 
ps.PollingStationId,
ISNULL(aps.NumberPerElection, (SELECT TOP 1 apst.NumberPerElection FROM  [dbo].[AssignedPollingStation] as apst WHERE apst.PollingStationId = ps.PollingStationId AND NumberPerElection IS NOT NULL)) as Number,
ps.NameRo as PollingStation,
dbo.fn_GetFullRegionName(ps.RegionId) as RegionName,
ec.NameRo as Competitor,
Concat(ecm.LastNameRo,' ', ecm.NameRo) as CompetitorMember,
SUM(bp.RegisteredVoters) as RegisteredVoters,
SUM(bp.BallotsIssued) as BallotsIssued,
SUM(bp.Supplementary) as SupplementaryVotes,
SUM(bp.BallotsCasted) as BallotsCasted,
SUM(bp.DifferenceIssuedCasted) as DifferenceIssuedCasted,
SUM(bp.BallotsSpoiled) as BallotsSpoiled,
SUM(bp.BallotsValidVotes) as BallotsValidVotes,
SUM(bp.BallotsReceived) as BallotsReceived,
SUM(bp.BallotsUnusedSpoiled) as BallotsUnusedSpoiled,
CAST(SUM(er.BallotCount) AS BIGINT) as BallotCount
FROM 
[dbo].[ElectionResult] as er
INNER JOIN [dbo].[BallotPaper] as bp ON er.BallotPaperId = bp.BallotPaperId AND bp.ElectionRoundId = er.ElectionRoundId
INNER JOIN [dbo].[PollingStation] as ps ON ps.PollingStationId = bp.PollingStationId
INNER JOIN [dbo].[AssignedPollingStation] as aps ON aps.PollingStationId = ps.PollingStationId AND aps.ElectionRoundId = er.ElectionRoundId
INNER JOIN [dbo].[ElectionCompetitor] as ec ON ec.ElectionCompetitorId = er.ElectionCompetitorId AND ec.ElectionRoundId = er.ElectionRoundId
LEFT OUTER JOIN [dbo].[ElectionCompetitorMember] as ecm ON ecm.ElectionCompetitorMemberId = er.ElectionCompetitorMemberId AND ecm.ElectionRoundId = er.ElectionRoundId
WHERE 
er.ElectionRoundId = @ElectionRoundId
AND (aps.AssignedCircumscriptionId = @CircumscriptionId OR @CircumscriptionId = 0) 
AND (ps.PollingStationId = @PollingStationId OR @PollingStationId = 0)
GROUP BY ps.PollingStationId, aps.NumberPerElection, dbo.fn_GetFullRegionName(ps.RegionId), ps.NameRo, ec.NameRo, ecm.LastNameRo, ecm.NameRo
ORDER BY Number ASC

GO
/****** Object:  StoredProcedure [dbo].[GetBallotPaper]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetBallotPaper]
(
    @ElectionRoundId	  bigint,
	@PollingStationId bigint,
	@AssignedCircumscriptionId bigint
)
AS
declare 
@electionType bigint = (select top(1) type from Election  where ElectionId = (select top(1) ElectionId from ElectionRound where ElectionRoundId =@ElectionRoundId))
 
 declare @Verification nvarchar(2) 
 set @Verification = (select top(1) value  from [dbo].[ReportParamValues] where reportParamId =3 and electionTypeId =@electionType)

 declare @regionNameCirc nvarchar(200)

  IF (@Verification ='da')
 BEGIN
set @regionNameCirc =(select top(1) name from [dbo].Region where RegionId = (select top(1) RegionId from AssignedCircumscription where AssignedCircumscriptionId=@AssignedCircumscriptionId))

 END

 SELECT top(1)
ec.Code, bp.RegisteredVoters,er.ElectionResultId ,
er.ElectionRoundId ,er.ElectionCompetitorId,er.ElectionCompetitorMemberId,
er.Status , er.BallotPaperId ,  elcm.AssignedCircumscriptionId , 
bp.BallotsCasted , bp.BallotsIssued , bp.BallotsReceived, bp.BallotsUnused, 
bp.BallotsSpoiled ,bp.BallotsUnusedSpoiled , bp.BallotsValidVotes ,bp.Supplementary, bp.DifferenceIssuedCasted,
bp.DateOfEntry ,

( select top(1) value  from [dbo].ReportParamValues where reportParamId =1 and electionTypeId =@electionType) as Param1,

( select top(1) value  from [dbo].ReportParamValues where reportParamId =2 and electionTypeId =@electionType) as Param2,

(@regionNameCirc) as Param3,

( select top(1) value  from [dbo].[ReportParamValues] where reportParamId =4 and electionTypeId =@electionType) as Param4,

( select top(1) value  from [dbo].[ReportParamValues] where reportParamId =5 and electionTypeId =@electionType) as Param5,

(select top(1) numberPerElection from [dbo].AssignedPollingStation where PollingStationId = @PollingStationId and numberPerElection is not null) as NumberPerElection,
(Select top(1) ElectionDate from [dbo].ElectionRound where ElectionRoundId = @ElectionRoundId),
(select top(1) name from [dbo].Region where RegionId = (select top(1) RegionId from PollingStation where PollingStationId=@PollingStationId)) as RegionName
 
  FROM [dbo].[ElectionResult] er
  join [dbo].ElectionCompetitorMember elcm on er.ElectionCompetitorMemberId=elcm.ElectionCompetitorMemberId
  join [dbo].ElectionCompetitor ec on er.ElectionCompetitorId = ec.ElectionCompetitorId
  join [dbo].BallotPaper bp on bp.BallotPaperId=er.BallotPaperId 
   where er.ElectionRoundId = @ElectionRoundId and bp.PollingStationId=@PollingStationId  

GO
/****** Object:  StoredProcedure [dbo].[GetCandidatResult]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetCandidatResult]
(
    @ElectionRoundId	  bigint,
	@PollingStationId bigint,
	@AssignedCircumscriptionId bigint
	
)
AS
declare 
@electionType bigint = (select top(1) type from Election  where ElectionId = (select top(1) ElectionId from ElectionRound where ElectionRoundId =@ElectionRoundId))
 
 SELECT Concat('g',ROW_NUMBER() OVER(PARTITION BY er.BallotPaperId ORDER BY ec.BallotOrder ASC)) AS Position  ,
CONCAT( elcm.NameRo , ' ' , elcm.LastNameRo) as name,
 er.BallotCount
  FROM [dbo].[ElectionResult] er
  join [dbo].ElectionCompetitorMember elcm on er.ElectionCompetitorMemberId=elcm.ElectionCompetitorMemberId
  join [dbo].ElectionCompetitor ec on er.ElectionCompetitorId = ec.ElectionCompetitorId
  join [dbo].BallotPaper bp on bp.BallotPaperId=er.BallotPaperId 
   where er.ElectionRoundId = @ElectionRoundId and bp.PollingStationId=@PollingStationId and ec.AssignedCircumscriptionId= @AssignedCircumscriptionId

GO
/****** Object:  StoredProcedure [dbo].[GetDataTransferStages]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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


/****** Object:  StoredProcedure [dbo].[GetElectionRoundResult]    Script Date: 3/14/2019 3:19:20 PM ******/
SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[GetElectionRoundResult]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[GetElectionRoundResult]
(
    @ElectionRoundId	  bigint
)
AS
SELECT 
ROW_NUMBER() OVER (ORDER BY T1.AssignedCircumscriptionId)  as Id,
T1.AssignedCircumscriptionId,
APS.PollingStationId,
T3.ElectionCompetitorId,
T4.IsIndependent,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[AssignedCircumscription] T1
INNER JOIN [dbo].[AssignedPollingStation] APS ON APS.AssignedCircumscriptionId = T1.AssignedCircumscriptionId
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 ON  T2.ElectionRoundId = T1.ElectionRoundId AND T2.PollingStationId = APS.PollingStationId
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE
T1.ElectionRoundId = @ElectionRoundId
GROUP BY T1.AssignedCircumscriptionId,  APS.PollingStationId, T3.ElectionCompetitorId, T4.IsIndependent, T4.Color, T3.ElectionCompetitorMemberId


GO
/****** Object:  StoredProcedure [dbo].[infElectionResult]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[infElectionResult]
(
    @ElectionCode int
)
AS

DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WITH(NOLOCK) WHERE et.Code = @ElectionCode);

DECLARE @ElectionRound BIGINT;
SET @ElectionRound =  (SELECT TOP 1 AC.ElectionRoundId FROM [dbo].[AssignedCircumscription] as AC WITH(NOLOCK))

DECLARE @NationalElectionRound BIGINT;
SET @NationalElectionRound =  (SELECT TOP 1 AC.ElectionRoundId FROM [dbo].[AssignedCircumscription] as AC WITH(NOLOCK))
--SET @NationalElectionRound =  (SELECT TOP 1 AC.ElectionRoundId FROM [dbo].[AssignedCircumscription] as AC WITH(NOLOCK) WHERE AC.RegionId = 1
--AND AC.ElectionRoundId  IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))

--);

SELECT DISTINCT 
a.[Type],
a.[KeyId],
a.[ElectionRoundId],
a.[ElectionCompetitorId],
a.[ElectionCompetitorMemberId],
a.[TotalBalots],
a.[TotalBalotsProcessed],
a.[BallotsValidVotes],
a.[BallotCount],
ISNULL((SELECT TOP 1 ET.Code FROM [dbo].[Election] E WITH(NOLOCK), [dbo].[ElectionRound] ER WITH(NOLOCK), [dbo].[ElectionType] ET WITH(NOLOCK)
WHERE ER.ElectionRoundId = a.[ElectionRoundId] AND ER.ElectionId = E.ElectionId AND E.[Type] = ET.ElectionTypeId AND a.[ElectionRoundId] <> 0 ),0) as ElectionType
FROM 
(
SELECT 
1 as [Type],
1 as [Number],
1 as [KeyId],
T1.ElectionRoundId,
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = T1.ElectionRoundId AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))

GROUP BY T1.ElectionRoundId, T3.ElectionCompetitorId,T3.ElectionCompetitorMemberId
union
SELECT 
1 as [Type],
T1.AssignedCircumscriptionId as [Number],
CAST(ISNULL(T1.RegionId,0) AS BIGINT) as [KeyId],
T1.ElectionRoundId,
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = T1.ElectionRoundId AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))
GROUP BY T1.AssignedCircumscriptionId, T1.RegionId, T1.ElectionRoundId, T3.ElectionCompetitorId,T3.ElectionCompetitorMemberId
--UNION
--SELECT 
--2 as [Type],
--T1.StatisticCode as [Number],
--T1.RegionId as [KeyId],
--@NationalElectionRound as ElectionRoundId,
--T3.ElectionCompetitorId,
--T3.ElectionCompetitorMemberId,
--SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
--SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
--SUM(T2.BallotsValidVotes) as BallotsValidVotes,
--SUM(T3.BallotCount) as BallotCount
--FROM 
--[dbo].[Region] T1 WITH(NOLOCK)
--INNER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  
--T2.ElectionRoundId = @NationalElectionRound AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.ParentRegionId = T1.RegionId)
--AND T2.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))
--LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
--LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
--LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
--WHERE
--T1.RegionTypeId between 2 and 4
--and T1.RegionId<> -1 
--and T1.RegionId IN (3 ,14721)
--GROUP BY T1.RegionId, T1.StatisticCode,  T3.ElectionCompetitorId, T3.ElectionCompetitorMemberId
UNION
SELECT 
3 as [Type],
T1.StatisticCode as [Number],
T1.RegionId as [KeyId],
@ElectionRound  as [ElectionRoundId],
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2  THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[Region] T1 WITH(NOLOCK)
INNER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  
T2.ElectionRoundId = @ElectionRound 
AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId)
AND T2.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE
T1.RegionTypeId = 5 
and T1.ParentId = 2
and T1.RegionId <> 14722
GROUP BY T1.RegionId, T1.StatisticCode,  T3.ElectionCompetitorId, T3.ElectionCompetitorMemberId
union
SELECT
4,
100000 as [Number],
4000 as [KeyId],
@ElectionRound  as [ElectionRoundId],
sub.[ElectionCompetitorId],
sub.[ElectionCompetitorMemberId],
SUM(sub.TotalBalots) TotalBalots,
SUM(sub.TotalBalotsProcessed) TotalBalotsProcessed,
SUM(sub.BallotsValidVotes) as BallotsValidVotes,
SUM(sub.BallotCount) as BallotCount
FROM
(
SELECT 
T1.RegionId as [KeyId],
@NationalElectionRound as [ElectionRoundId],
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[Region] T1 WITH(NOLOCK)
INNER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  
T2.ElectionRoundId = @ElectionRound  
AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId)
AND T2.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE
T1.RegionTypeId <> 5 
and (T1.ParentId = 2 OR T1.RegionId IN (SELECT rt.RegionId FROM [dbo].[Region] rt WITH(NOLOCK) WHERE rt.ParentId IN (SELECT rtt.RegionId FROM [dbo].[Region] rtt WITH(NOLOCK) WHERE rtt.ParentId = 2) ) )
AND T3.ElectionCompetitorId IS NOT NULL
GROUP BY T1.RegionId, T3.ElectionCompetitorId, T3.ElectionCompetitorMemberId
) sub
GROUP BY sub.[ElectionCompetitorId],  sub.[ElectionCompetitorMemberId]
) a 
ORDER BY a.[Type], a.[KeyId]
GO
/****** Object:  StoredProcedure [dbo].[infLocalElectionResult]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[infLocalElectionResult]
(
    @ElectionCode int
)
AS

DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = @ElectionCode);

SELECT 
a.[Type],
a.[KeyId],
a.[ElectionRoundId],
a.[ElectionCompetitorId],
a.[ElectionCompetitorMemberId],
a.[TotalBalots],
a.[TotalBalotsProcessed],
a.[BallotsValidVotes],
a.[BallotCount],
a.[PoliticalPartyId]
FROM
(
SELECT 
1 as [Type],
T1.AssignedCircumscriptionId as [KeyId],
ISNULL(T1.Number,0) as [Number],
T1.ElectionRoundId,
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount,
T4.PoliticalPartyId
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = T1.ElectionRoundId AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND e.Type = @ElectionTypeId)
GROUP BY T1.AssignedCircumscriptionId, T1.Number, T1.ElectionRoundId, T3.ElectionCompetitorId,T4.PoliticalPartyId, T3.ElectionCompetitorMemberId
) a
ORDER BY [Type], [Number], [ElectionRoundId], [KeyId]
GO
/****** Object:  StoredProcedure [dbo].[infLocalElectionResultCheckFinalized]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[infLocalElectionResultCheckFinalized]
AS
DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code IN  (10,11));

DECLARE @ElectionTypeGagauzId BIGINT;
SET @ElectionTypeGagauzId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = 3);

SELECT 
T1.RegionId as [KeyId],
CASE WHEN SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) = SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END)  THEN 1 ELSE 0 END AS IsFinalized
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND e.Type IN (SELECT et.ElectionTypeId FROM ElectionType et WHERE et.Code IN  (10,11)))
GROUP BY T1.AssignedCircumscriptionId,T1.RegionId
UNION
SELECT
37 as KeyId,
CASE WHEN SUM(TotalBalots) = SUM(TotalBalotsProcessed)  THEN 1 ELSE 0 END AS IsFinalized
FROM
(SELECT 
T1.RegionId as [KeyId],
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
WHERE 
T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND e.Type = @ElectionTypeGagauzId)
AND T1.RegionId IN (SELECT r.RegionId FROM Region as r WHERE r.ParentId = 37)
GROUP BY T1.AssignedCircumscriptionId,T1.RegionId) as a
GO

/****** Object:  StoredProcedure [dbo].[infLocalElectionResultCheckFinalized2]    Script Date: 29.10.2019 10:27:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[infLocalElectionResultCheckFinalized2]
AS
SELECT 
T1.RegionId as [KeyId],
CASE WHEN SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) = SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END)  THEN 1 ELSE 0 END AS IsFinalized
FROM 
[dbo].[RegionType] T4 WITH(NOLOCK),
[dbo].[Region] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.ParentRegionId = T1.RegionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
WHERE
T1.RegionTypeId between 2 and 4
and T4.RegionTypeId = T1.RegionTypeId
and T1.RegionId<> -1 
and T1.RegionId<> 14721 
GROUP BY T1.RegionId
ORDER BY T1.RegionId
GO


/****** Object:  StoredProcedure [dbo].[infLocalElectionResultDetailed]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[infLocalElectionResultDetailed]
(  
    @ElectionCode int
)
AS

DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = @ElectionCode);

SELECT
1 as [Type],
a.[KeyId],
a.[ParentRegionId],
a.[ChildRegionId],
a.ElectionRoundId,
a.ElectionCompetitorId,
a.ElectionCompetitorMemberId,
SUM(TotalBalots) as TotalBalots,
SUM(TotalBalotsProcessed) TotalBalotsProcessed,
SUM(BallotsValidVotes) as BallotsValidVotes,
SUM(BallotCount) as BallotCount,
a.PoliticalPartyId
FROM
(

SELECT 
1 as [Type],
T1.AssignedCircumscriptionId as [KeyId],
T1.RegionId as [ParentRegionId],
T1.Number,
ISNULL((SELECT TOP 1 r.ParentId FROM Region r, Region r2 WHERE r.ParentId = r2.RegionId and r.RegionId = T0.RegionId AND r2.RegionTypeId = 8),T0.RegionId)  as [ChildRegionId],
T1.ElectionRoundId,
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount,
T4.PoliticalPartyId
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
INNER JOIN [dbo].[AssignedPollingStation] T0 ON T0.AssignedCircumscriptionId = T1.AssignedCircumscriptionId
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = T1.ElectionRoundId AND T2.PollingStationId = T0.PollingStationId
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE 
T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND e.Type = @ElectionTypeId)
GROUP BY T1.AssignedCircumscriptionId, T1.RegionId, T0.RegionId, T1.Number, T1.ElectionRoundId, T3.ElectionCompetitorId, T4.PoliticalPartyId, T3.ElectionCompetitorMemberId
) as a
GROUP BY [KeyId], [ParentRegionId], [ChildRegionId], Number, ElectionRoundId, ElectionCompetitorId, PoliticalPartyId, ElectionCompetitorMemberId
ORDER BY [Type], [ElectionRoundId], [KeyId], [ParentRegionId], [ChildRegionId]

GO
/****** Object:  StoredProcedure [dbo].[infLocalElectionResultMun]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[infLocalElectionResultMun]
(
  
    @ElectionCode int
)
AS

DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = @ElectionCode);


SELECT 
1 as [Type],
T1.AssignedCircumscriptionId as [KeyId],
T1.RegionId as [ParentRegionId],
T0.RegionId as [ChildRegionId],
T1.ElectionRoundId,
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount,
T4.PoliticalPartyId
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
INNER JOIN [dbo].[AssignedPollingStation] T0 ON T0.AssignedCircumscriptionId = T1.AssignedCircumscriptionId
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = T1.ElectionRoundId AND T2.PollingStationId = T0.PollingStationId
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE 
T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND e.Type = @ElectionTypeId)
AND T0.RegionId IN
(
SELECT 
T1.RegionId
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T1.RegionTypeId = 5 
and T1.RegionId <> 14722
and T4.RegionTypeId = T1.RegionTypeId
and T1.ParentId = 2
union
SELECT 
  T1.RegionId
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T1.RegionTypeId <> 5 
and T4.RegionTypeId = T1.RegionTypeId
and (T1.ParentId = 2 OR T1.RegionId IN (SELECT rt.RegionId FROM [dbo].[Region] rt WITH(NOLOCK) WHERE rt.ParentId IN (SELECT rtt.RegionId FROM [dbo].[Region] rtt WITH(NOLOCK) WHERE rtt.ParentId = 2) ) )
union
SELECT 
T1.RegionId
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T4.RegionTypeId = T1.RegionTypeId
and T1.RegionId = 3
)
GROUP BY T1.AssignedCircumscriptionId, T1.RegionId, T0.RegionId, T1.Number, T1.ElectionRoundId, T3.ElectionCompetitorId, T4.PoliticalPartyId,T3.ElectionCompetitorMemberId
ORDER BY [Type], [Number], [ElectionRoundId], [KeyId], T1.RegionId, T0.RegionId
GO
/****** Object:  StoredProcedure [dbo].[infLocalElectionVoterTurnout]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[infLocalElectionVoterTurnout]
(
    @ElectionCode int
)
AS

DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = @ElectionCode);


SELECT 
a.[Type],
a.[KeyId],
a.[ElectionRoundId],
a.[Name],
a.[TotalVoters],
(a.M+a.F) as [TotalVoted],
a.[M],
a.[F],
a.[C1],
a.[C2],
a.[C3],
a.[C4],
a.[C5]
FROM
(
SELECT 
  1 as [Type],
  T1.AssignedCircumscriptionId as [KeyId],
  T1.ElectionRoundId,
  --T1.Number as [Number],
  T1.NameRo as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId),0)+
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)),0)
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'TotalVoters',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'M',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'F',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C1',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C2',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C3',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C4',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C5'
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK)  WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))
GROUP BY T1.AssignedCircumscriptionId, T1.Number, T1.NameRo, T1.ElectionRoundId
) a
UNION
SELECT 
1 'Type',
37 'KeyId',
b.[ElectionRoundId],
'UTA GĂGĂUZIA' 'Name',
SUM(b.[TotalVoters]) 'TotalVoters',
SUM((b.M+b.F)) as [TotalVoted],
SUM(b.[M]) 'M',
SUM(b.[F]) 'F',
SUM(b.[C1]) 'C1',
SUM(b.[C2]) 'C2',
SUM(b.[C3]) 'C3',
SUM(b.[C4]) 'C4',
SUM(b.[C5]) 'C5'
FROM
(
SELECT 
  1 as [Type],
  T1.AssignedCircumscriptionId as [KeyId],
  T1.ElectionRoundId,
  --T1.Number as [Number],
  T1.NameRo as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId),0)+
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)),0)
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'TotalVoters',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'M',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'F',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C1',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C2',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C3',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C4',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C5'
FROM 
[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
WHERE 
T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK)  WHERE er.ElectionId = e.ElectionId AND (e.Type = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = 3)))
AND T1.RegionId IN (SELECT r.RegionId FROM Region as r WHERE r.ParentId = 37)
AND @ElectionCode = 10
GROUP BY T1.AssignedCircumscriptionId, T1.Number, T1.NameRo, T1.ElectionRoundId
) b
GROUP BY b.[ElectionRoundId]
ORDER BY [ElectionRoundId], [KeyId]
GO

/****** Object:  StoredProcedure [dbo].[infLocalElectionVoterTurnout2]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[infLocalElectionVoterTurnout2]
(
    @ElectionCode int
)
AS

DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = @ElectionCode);

DECLARE @NationalElectionRound BIGINT;
SET @NationalElectionRound = (SELECT TOP 1 er.ElectionRoundId FROM [dbo].[ElectionRound] er, [dbo].[Election] as e WITH(NOLOCK) WHERE e.Type = @ElectionTypeId AND er.ElectionId = e.ElectionId);

SELECT 
a.[Type],
a.[KeyId],
a.[ElectionRoundId],
a.[Name],
a.[TotalVoters],
(a.M+a.F) as [TotalVoted],
a.[M],
a.[F],
a.[C1],
a.[C2],
a.[C3],
a.[C4],
a.[C5],
ISNULL((SELECT TOP 1 ET.Code FROM [dbo].[Election] E WITH(NOLOCK), [dbo].[ElectionRound] ER WITH(NOLOCK), [dbo].[ElectionType] ET WITH(NOLOCK)
WHERE ER.ElectionRoundId = a.[ElectionRoundId] AND ER.ElectionId = E.ElectionId AND E.[Type] = ET.ElectionTypeId AND a.[ElectionRoundId] <> 0 ),0) as ElectionType
FROM
(
SELECT 
  2 as [Type],
  T1.StatisticCode as [Number],
  CAST(0 AS BIGINT) as [ElectionRoundId],
  T1.RegionId as [KeyId],
  T1.RegionTypeId as [RegionTypeId],
  CONCAT(T4.Name, ' ',T1.Name) as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.ParentRegionId = T1.RegionId and aps.ElectionRoundId = @NationalElectionRound),0)+
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.ParentRegionId =  T1.RegionId),0)  
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.ParentRegionId = T1.RegionId)) 'TotalVoters',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.ParentRegionId =  T1.RegionId) 'M',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.ParentRegionId =  T1.RegionId) 'F',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1  AND T2.ParentRegionId =  T1.RegionId) 'C1',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2  AND T2.ParentRegionId =  T1.RegionId) 'C2',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3  AND T2.ParentRegionId =  T1.RegionId) 'C3',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4  AND T2.ParentRegionId =  T1.RegionId) 'C4',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5  AND T2.ParentRegionId =  T1.RegionId) 'C5'
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T1.RegionTypeId between 2 and 3
and T4.RegionTypeId = T1.RegionTypeId
and T1.RegionId<> -1 
and T1.RegionId<> 14721 
) a
ORDER BY [Type], [RegionTypeId], [Number], [ElectionRoundId], [KeyId]
GO


/****** Object:  StoredProcedure [dbo].[infLocalElectionVoterTurnoutMun]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[infLocalElectionVoterTurnoutMun]
AS

DECLARE @NationalElectionRound BIGINT;
SET @NationalElectionRound = (SELECT MAX(AC.ElectionRoundId) FROM [dbo].[AssignedCircumscription] as AC WITH(NOLOCK) WHERE AC.RegionId = 2);

SELECT 
a.[Type],
a.[KeyId],
CAST(a.[ElectionRoundId] as BIGINT) [ElectionRoundId],
a.[Name],
SUM(a.[TotalVoters]) as [TotalVoters],
SUM((a.M+a.F)) as [TotalVoted],
SUM(a.[M]) as [M],
SUM(a.[F]) as [F],
SUM(a.[C1]) as [C1],
SUM(a.[C2]) as [C2],
SUM(a.[C3]) as [C3],
SUM(a.[C4]) as [C4],
SUM(a.[C5]) as [C5]
FROM (
SELECT 
  3 as [Type],
  T1.StatisticCode as [Number],
  0 as [ElectionRoundId],
  CASE  
     WHEN T1.ParentId=3 THEN T1.ParentId  
     ELSE T1.RegionId
  END as [KeyId],  
  CASE  
     WHEN T1.ParentId=3 THEN (SELECT TOP 1 CONCAT(brt.Name, ' ',br.Name)  FROM [dbo].[Region] br, [dbo].[RegionType] brt WHERE br.RegionTypeId = brt.RegionTypeId and br.RegionId =  T1.ParentId)
     ELSE CONCAT(T4.Name, ' ',T1.Name)
  END as 'Name',  
  --T1.RegionId  ,
  --CONCAT(T4.Name, ' ',T1.Name) as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId and (aps.ElectionRoundId = @NationalElectionRound OR T1.ParentId = 3)),0)+ 
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.RegionId =  T1.RegionId),0)   
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.RegionId = T1.RegionId)) 'TotalVoters',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.RegionId =  T1.RegionId) 'M',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.RegionId =  T1.RegionId) 'F',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1  AND T2.RegionId =  T1.RegionId) 'C1',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2  AND T2.RegionId =  T1.RegionId) 'C2',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3  AND T2.RegionId =  T1.RegionId) 'C3',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4  AND T2.RegionId =  T1.RegionId) 'C4',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5  AND T2.RegionId =  T1.RegionId) 'C5'
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
(T1.RegionTypeId = 5  OR T1.ParentId = 3)
and T1.RegionId <> 14722
and T4.RegionTypeId = T1.RegionTypeId
and (T1.ParentId = 2  OR T1.ParentId = 3)
union
SELECT
sub.[Type],
100000 as [Number],
0 as [ElectionRoundId],
4000 as [KeyId],
'Suburbii' as 'Name',
SUM(sub.[TotalVoters]) 'TotalVoters',
SUM(sub.[M]) 'M',
SUM(sub.[F]) 'F',
SUM(sub.[C1]) 'C1',
SUM(sub.[C2]) 'C2',
SUM(sub.[C3]) 'C3',
SUM(sub.[C4]) 'C4',
SUM(sub.[C5]) 'C5'
FROM
(
SELECT 
  4 as [Type],
  0 as [ElectionRoundId],
  T1.RegionId as [KeyId],
  CONCAT(T4.Name, ' ',T1.Name) as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId and aps.ElectionRoundId = @NationalElectionRound),0)+
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.RegionId =  T1.RegionId),0) 
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.RegionId = T1.RegionId)) 'TotalVoters',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.RegionId =  T1.RegionId) 'M',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.RegionId =  T1.RegionId) 'F',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1  AND T2.RegionId =  T1.RegionId) 'C1',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2  AND T2.RegionId =  T1.RegionId) 'C2',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3  AND T2.RegionId =  T1.RegionId) 'C3',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4  AND T2.RegionId =  T1.RegionId) 'C4',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5  AND T2.RegionId =  T1.RegionId) 'C5'
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T1.RegionTypeId <> 5 
and T4.RegionTypeId = T1.RegionTypeId
and (T1.ParentId = 2 OR T1.RegionId IN (SELECT rt.RegionId FROM [dbo].[Region] rt WITH(NOLOCK) WHERE rt.ParentId IN (SELECT rtt.RegionId FROM [dbo].[Region] rtt WITH(NOLOCK) WHERE rtt.ParentId = 2) ) )
) sub
GROUP BY sub.[Type]
union
SELECT 
  3 as [Type],
  T1.StatisticCode as [Number],
  0 as [ElectionRoundId],
  T1.RegionId as [KeyId],
  CONCAT(T4.Name, ' ',T1.Name) as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId and aps.ElectionRoundId = @NationalElectionRound),0)+ 
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.RegionId =  T1.RegionId),0)   
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.RegionId = T1.RegionId)) 'TotalVoters',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.RegionId =  T1.RegionId) 'M',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.RegionId =  T1.RegionId) 'F',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1  AND T2.RegionId =  T1.RegionId) 'C1',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2  AND T2.RegionId =  T1.RegionId) 'C2',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3  AND T2.RegionId =  T1.RegionId) 'C3',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4  AND T2.RegionId =  T1.RegionId) 'C4',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5  AND T2.RegionId =  T1.RegionId) 'C5'
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T4.RegionTypeId = T1.RegionTypeId
and T1.RegionId = 3
) a
GROUP BY  [Type], [ElectionRoundId], [Name], [KeyId]
ORDER BY [Type], [ElectionRoundId], [KeyId]
GO
/****** Object:  StoredProcedure [dbo].[infVoterTurnout]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[infVoterTurnout]
(
    @ElectionCode int
)
AS

DECLARE @ElectionTypeId BIGINT;
SET @ElectionTypeId = (SELECT TOP 1 et.ElectionTypeId FROM ElectionType et WHERE et.Code = @ElectionCode);

DECLARE @NationalElectionRound BIGINT;
SET @NationalElectionRound = (SELECT TOP 1 AC.ElectionRoundId FROM [dbo].[AssignedCircumscription] as AC WITH(NOLOCK));

SELECT 
a.[Type],
a.[KeyId],
@NationalElectionRound as [ElectionRoundId],
a.[Name],
a.[TotalVoters],
(a.M+a.F) as [TotalVoted],
a.[M],
a.[F],
a.[C1],
a.[C2],
a.[C3],
a.[C4],
a.[C5],
ISNULL((SELECT TOP 1 ET.Code FROM [dbo].[Election] E WITH(NOLOCK), [dbo].[ElectionRound] ER WITH(NOLOCK), [dbo].[ElectionType] ET WITH(NOLOCK)
WHERE ER.ElectionRoundId = a.[ElectionRoundId] AND ER.ElectionId = E.ElectionId AND E.[Type] = ET.ElectionTypeId AND a.[ElectionRoundId] <> 0 ),2) as ElectionType
FROM
(
--(SELECT 
--  1 as [Type],
--  T1.AssignedCircumscriptionId as [Number],
--  T1.ElectionRoundId,
--  CAST(ISNULL(Number,0) AS BIGINT) as [KeyId],
--  T1.NameRo as 'Name',
--  CAST(
--  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId),0)+
--  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)),0)
--  AS INT) 'TotalVoters',
--  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'TotalVoters',
--  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'M',
--  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'F',
--  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C1',
--  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C2',
--  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C3',
--  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C4',
--  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5 AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.AssignedCircumscriptionId = T1.AssignedCircumscriptionId)) 'C5'
--FROM 
--[dbo].[AssignedCircumscription] T1 WITH(NOLOCK)
--WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK)  WHERE er.ElectionId = e.ElectionId AND (e.Type = @ElectionTypeId))
--GROUP BY T1.AssignedCircumscriptionId, T1.Number, T1.NameRo, T1.ElectionRoundId
--union
SELECT 
  2 as [Type],
  T1.StatisticCode as [Number],
  0 as [ElectionRoundId],
  T1.RegionId as [KeyId],
  CONCAT(T4.Name, ' ',T1.Name) as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.ParentRegionId = T1.RegionId and aps.ElectionRoundId = @NationalElectionRound and aps.RegionId <> 14722),0)+
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND (T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722)),0)  
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.ParentRegionId = T1.RegionId)) 'TotalVoters',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722) 'M',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722) 'F',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1  AND T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722) 'C1',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2  AND T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722) 'C2',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3  AND T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722) 'C3',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4  AND T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722) 'C4',
  (SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5  AND T2.ParentRegionId =  T1.RegionId and T2.RegionId <> 14722) 'C5'
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T1.RegionTypeId between 2 and 4
and T4.RegionTypeId = T1.RegionTypeId
and T1.RegionId<> -1 
and exists(select 1 from Region R3 WHERE (R3.RegionTypeId = 4 AND R3.RegionId = T1.RegionId AND R3.RegionId < 38) OR T1.RegionTypeId <> 4)
union
SELECT 
  3 as [Type],
  T1.StatisticCode as [Number],
  0 as [ElectionRoundId],
  T1.RegionId as [KeyId],
  CONCAT(T4.Name, ' ',T1.Name) as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId and aps.ElectionRoundId = @NationalElectionRound),0)+ 
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.RegionId =  T1.RegionId),0)   
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.RegionId = T1.RegionId)) 'TotalVoters',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.RegionId =  T1.RegionId) 'M',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.RegionId =  T1.RegionId) 'F',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1  AND T2.RegionId =  T1.RegionId) 'C1',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2  AND T2.RegionId =  T1.RegionId) 'C2',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3  AND T2.RegionId =  T1.RegionId) 'C3',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4  AND T2.RegionId =  T1.RegionId) 'C4',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5  AND T2.RegionId =  T1.RegionId) 'C5'
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T1.RegionTypeId = 5 
and T4.RegionTypeId = T1.RegionTypeId
and T1.ParentId = 2
union
SELECT
sub.[Type],
100000 as [Number],
0 as [ElectionRoundId],
4000 as [KeyId],
'Suburbii' as 'Name',
SUM(sub.[TotalVoters]) 'TotalVoters',
SUM(sub.[M]) 'M',
SUM(sub.[F]) 'F',
SUM(sub.[C1]) 'C1',
SUM(sub.[C2]) 'C2',
SUM(sub.[C3]) 'C3',
SUM(sub.[C4]) 'C4',
SUM(sub.[C5]) 'C5'
FROM
(
SELECT 
  4 as [Type],
  0 as [ElectionRoundId],
  T1.RegionId as [KeyId],
  CONCAT(T4.Name, ' ',T1.Name) as 'Name',
  CAST(
  ISNULL((SELECT SUM(aps.OpeningVoters) FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId and aps.ElectionRoundId = @NationalElectionRound),0)+
  ISNULL((SELECT COUNT(T2.AssignedVoterStatisticId) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AssignedVoterStatus between 5001 and 5002 AND T2.RegionId =  T1.RegionId),0) 
  AS INT) 'TotalVoters',
  --(SELECT COUNT(T3.AssignedVoterId) FROM [dbo].[AssignedVoter] AS T3 WHERE T3.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WHERE aps.RegionId = T1.RegionId)) 'TotalVoters',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 1 AND T2.RegionId =  T1.RegionId) 'M',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.Gender = 2 AND T2.RegionId =  T1.RegionId) 'F',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 1  AND T2.RegionId =  T1.RegionId) 'C1',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 2  AND T2.RegionId =  T1.RegionId) 'C2',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 3  AND T2.RegionId =  T1.RegionId) 'C3',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 4  AND T2.RegionId =  T1.RegionId) 'C4',
  (SELECT COUNT((T2.AssignedVoterStatisticId)) FROM [dbo].[AssignedVoterStatistics] AS T2 WITH(NOLOCK) WHERE T2.AgeCategoryId = 5  AND T2.RegionId =  T1.RegionId) 'C5'
FROM 
[dbo].[Region] T1 WITH(NOLOCK),
[dbo].[RegionType] T4 WITH(NOLOCK)
WHERE
T1.RegionTypeId <> 5 
and T4.RegionTypeId = T1.RegionTypeId
and (T1.ParentId = 2 OR T1.RegionId IN (SELECT rt.RegionId FROM [dbo].[Region] rt WITH(NOLOCK) WHERE rt.ParentId IN (SELECT rtt.RegionId FROM [dbo].[Region] rtt WITH(NOLOCK) WHERE rtt.ParentId = 2) ) )
) sub
GROUP BY sub.[Type]
) a
ORDER BY [Type] , [KeyId]
GO

/****** Object:  StoredProcedure [dbo].[MoveDataFromTemp]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[MoveDataFromTemp]
AS

	/* Clean tables */
	--DELETE FROM [dbo].[ReportParameterValues];
	--DELETE FROM [dbo].[Documents];
	--DELETE FROM [dbo].[ReportParameters];	
	--DELETE FROM [dbo].[Templates];
	--DELETE FROM [dbo].[TemplateNames];
	--DELETE FROM [dbo].[TemplateTypes];
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
	
	/* ElectionDuration */
	INSERT INTO [dbo].[ElectionDuration] ([ElectionDurationId], [Name])
	(
	SELECT [ElectionDurationId], [Name]
	FROM [schematmp].[ElectionDuration]
	);
	
	/* AssignedPollingStation */ --updated by Alexandru Gîscă
	SET IDENTITY_INSERT [dbo].[AssignedPollingStation] ON;	
	INSERT INTO [dbo].[AssignedPollingStation] ([AssignedPollingStationId], [ElectionRoundId], [AssignedCircumscriptionId], [PollingStationId], [Type], [Status], [IsOpen], [OpeningVoters], [EstimatedNumberOfVoters], 
		[NumberOfRoBallotPapers], [NumberOfRuBallotPapers], [ImplementsEVR], [EditUserId], [EditDate], [Version], [isOpeningEnabled], [isTurnoutEnabled], [isElectionResultEnabled],[numberPerElection],
		[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu], [ElectionDurationId])
	(
		SELECT [AssignedPollingStationId], [ElectionRoundId], [AssignedCircumscriptionId], [PollingStationId], [Type], [Status], [IsOpen], [OpeningVoters], [EstimatedNumberOfVoters], 
			[NumberOfRoBallotPapers], [NumberOfRuBallotPapers], [ImplementsEVR], [EditUserId], [EditDate], [Version], [isOpeningEnabled], [isTurnoutEnabled], [isElectionResultEnabled],[numberPerElection],
			[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu], 1
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

	-- here comes Copy from schematmp to  DocumentTemplate Tables
	/*TemplateTypes*/
	INSERT INTO [dbo].[TemplateTypes] ([TemplateTypeId], [Title], [EditUserId], [EditDate], [Version])
	(
	SELECT [TemplateTypeId], [Title], [EditUserId], [EditDate], [Version]
	FROM [schematmp].[TemplateTypes]
	);

	/* TemplateNames */
	INSERT INTO [dbo].[TemplateNames] ([TemplateNameId], [TemplateTypeId], Title, EditUserId, EditDate, [Version])
	(
	SELECT [TemplateNameId], [TemplateTypeId], Title, EditUserId, EditDate, [Version]
	FROM [schematmp].[TemplateNames]
	);

	/* Templates */
	INSERT INTO [dbo].[Templates] (TemplateId, Content, UploadDate, TemplateNameId, EditUserId, EditDate, [Version], [ParentId])
	(
	SELECT TemplateId, Content, UploadDate, TemplateNameId, EditUserId, EditDate, [Version], [ParentId]
	FROM [schematmp].[Templates]
	);

	/* ReportParameters */
	INSERT INTO [dbo].[ReportParameters] (ReportParameterId, ParameterName, TemplateId, EditUserId, EditDate, IsLookup, [Version], ParameterCode)
	(
	SELECT ReportParameterId, ParameterName, TemplateId, EditUserId, EditDate, IsLookup, [Version], ParameterCode
	FROM [schematmp].[ReportParameters]
	);
	----------

RETURN 0


GO
/****** Object:  StoredProcedure [dbo].[MoveDataToRepository]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


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
	  ,[NumberOfRuBallotPapers],[ImplementsEVR],[isOpeningEnabled],[isTurnoutEnabled],[isElectionResultEnabled],[NumberPerElection],[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu], [ElectionDurationId])';
	SET @sql = @sql + '(SELECT [AssignedPollingStationId],' + CAST(@electionDayId AS VARCHAR(10)) + ',[ElectionRoundId]
	  ,[PollingStationId],[AssignedCircumscriptionId],[Type],[Status],[IsOpen],[OpeningVoters],[EstimatedNumberOfVoters],[NumberOfRoBallotPapers]
	  ,[NumberOfRuBallotPapers],[ImplementsEVR],[isOpeningEnabled],[isTurnoutEnabled],[isElectionResultEnabled],[NumberPerElection],[RegionId],[ParentRegionId],[CirculationRo],[CirculationRu], 1';
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

GO

/****** Object:  StoredProcedure [dbo].[UpdateDataFromTemp]    Script Date: 09.10.2019 09:48:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


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
		WHERE [AssignedCircumscriptionId] IN (SELECT ac.AssignedCircumscriptionId FROM AssignedCircumscription ac)
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
		WHERE [AssignedCircumscriptionId] IN (SELECT ac.AssignedCircumscriptionId FROM AssignedCircumscription ac)
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
		AND ER.ElectionCompetitorId IN (SELECT ec.ElectionCompetitorId FROM [ElectionCompetitor] ec)
		AND ER.ElectionCompetitorMemberId IN (SELECT ecm.ElectionCompetitorMemberId FROM [ElectionCompetitorMember] ecm)
	);
	SET IDENTITY_INSERT [dbo].[ElectionResult] OFF;

RETURN 0
GO

/****** Object:  StoredProcedure [dbo].[GetVotersList]    Script Date: 10/26/2023 10:32:15 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetVotersList]
(
    @ElectionRoundId	  bigint,
	@PollingStationId bigint,
	@AssignedCircumscriptionId bigint,
	@ElectionId bigint,
	@VoterStatus bigint
)
AS
SELECT 
	ROW_NUMBER() OVER(ORDER BY AV.[AssignedVoterId]) AS RowNumber
    , AV.[AssignedVoterId]
    , AV.[RegionId]
	, ISNULL('str. ' + V.StreetName, '') 
		+ ISNULL(' ' + CONVERT(VARCHAR, V.StreetNumber), '') 		
		+ ISNULL('/' + CONVERT(VARCHAR, V.StreetSubNumber), '') 
		+ ISNULL(' ap. ' + CONVERT(VARCHAR, V.BlockNumber), '')
		+ ISNULL(CONVERT(VARCHAR, V.BlockSubNumber), '') 
		+ ', ' + R.PersonAddress as 'PersonFullAdress'
    , PS1.NameRo AS 'RequestingPollingStationName'
    , PS1.Number AS 'RequestingPollingStationNumber'
	, PS2.Number AS 'PollingStationNumber'
    , AV.[PollingStationId]
	, AV.[RequestingPollingStationId]
    , PS2.NameRo AS 'PollingStationName' 
	, PS2.[Address] AS 'PSAdress'
	, R1.PollingStationAddress AS 'PollingStationAddress'
    , AV.[VoterId]
    , V.Idnp
    , V.LastNameRo +' '+V.NameRo +' '+ V.PatronymicRo AS 'VoterFullName'
    , V.DateOfBirth
    , V.PlaceOfResidence
    , V.DocumentNumber
    , AVS.[Signature]
    , AV.[Category]
    , AV.[Status]
    , AV.[Comment]
    , AV.[ElectionListNr]
    , AV.[EditUserId]
    , SU.UserName
    , AV.[EditDate]
    , AV.[Version]
	,E.[Type]
	,ET.[TypeName]

FROM 
    [dbo].[AssignedVoter] AS AV
LEFT JOIN 
    (
    SELECT 
        r1.RegionId,
        ISNULL(rt1.[Name] + ' ' + r1.[Name], '') + ', ' +
        ISNULL(CASE WHEN r2.RegionId = 1 THEN null ELSE rt2.[Name] + ' ' + r2.[Name] END, '') + ', ' +
        ISNULL(CASE WHEN r3.RegionId = 1 THEN null ELSE rt3.[Name] + ' ' + r3.[Name] END, '') AS PersonAddress

    FROM 
        [dbo].[Region] r1
    LEFT JOIN 
        [dbo].[Region] r2 ON r1.ParentId = r2.RegionId
    LEFT JOIN 
        [dbo].[Region] r3 ON r2.ParentId = r3.RegionId
    LEFT JOIN 
        [dbo].[RegionType] rt1 ON r1.RegionTypeId = rt1.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt2 ON r2.RegionTypeId = rt2.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt3 ON r3.RegionTypeId = rt3.RegionTypeId
    ) AS R ON R.RegionId = AV.RegionId
LEFT JOIN 
    [dbo].[PollingStation] AS PS1 ON PS1.PollingStationId = AV.RequestingPollingStationId
LEFT JOIN 
    [dbo].[PollingStation] AS PS2 ON PS2.PollingStationId = AV.PollingStationId
LEFT JOIN 
    [dbo].[Voter] AS V ON V.VoterId = AV.VoterId
LEFT JOIN 
    [dbo].[SystemUser] AS SU ON SU.SystemUserId = AV.EditUserId
LEFT JOIN 
    [dbo].[AssignedVoterStatistics] AS AVS ON AVS.AssignedVoterId = AV.AssignedVoterId
LEFT JOIN 
    [dbo].[AssignedPollingStation] AS APS ON APS.PollingStationId = AV.[PollingStationId]
LEFT JOIN 
	[dbo].[ElectionRound] AS ER ON ER.ElectionRoundId = APS.ElectionRoundId
LEFT JOIN [dbo].[Election] as E on E.ElectionId = ER.ElectionId
LEFT JOIN [dbo].[ElectionType] as ET on ET.ElectionTypeId = E.[Type]
LEFT JOIN 
    (
    SELECT 
        r1.RegionId,
        ISNULL(rt1.[Name] + ' ' + r1.[Name], '') + ', ' +
        ISNULL(CASE WHEN r2.RegionId = 1 THEN null ELSE rt2.[Name] + ' ' + r2.[Name] END, '') --+ ', ' +
        --ISNULL(CASE WHEN r3.RegionId = 1 THEN null ELSE rt3.[Name] + ' ' + r3.[Name] END, '') 
		AS PollingStationAddress

    FROM 
        [dbo].[Region] r1
    LEFT JOIN 
        [dbo].[Region] r2 ON r1.ParentId = r2.RegionId
    LEFT JOIN 
        [dbo].[Region] r3 ON r2.ParentId = r3.RegionId
    LEFT JOIN 
        [dbo].[RegionType] rt1 ON r1.RegionTypeId = rt1.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt2 ON r2.RegionTypeId = rt2.RegionTypeId
    LEFT JOIN 
        [dbo].[RegionType] rt3 ON r3.RegionTypeId = rt3.RegionTypeId
    ) AS R1 ON R1.RegionId = PS2.RegionId
WHERE E.ElectionId = @ElectionId
AND APS.ElectionRoundId = @ElectionRoundId
AND APS.AssignedCircumscriptionId = @AssignedCircumscriptionId
AND AV.[PollingStationId] = @PollingStationId
AND AV.[Status] = @VoterStatus;

RETURN 0
GO

CREATE FUNCTION [dbo].[fn_GetFullRegionName] 
(
	@RegionId BIGINT
)
RETURNS nvarchar(1000)
AS
BEGIN
DECLARE @Result nvarchar(1000);
;WITH cte AS (
 SELECT
   r.name
 , rt.name as typeName
 , r.regionId
 , r.regionTypeId
 , r.parentId
 , 1 RegionLevel
 FROM 
 Region r,
 RegionType rt 
 WHERE 
 r.RegionId = @RegionId
 AND rt.regionTypeId = r.regionTypeId
 UNION ALL
 SELECT
   c.name
 , rt.name as typeName
 , c.regionId 
 , c.regionTypeId
 , c.parentId
 , p.RegionLevel + 1
 FROM Region c
 INNER JOIN RegionType rt  ON rt.regionTypeId = c.regionTypeId
 JOIN cte p ON p.ParentId = c.RegionId and c.RegionId <> 1
 )
SELECT @Result = STUFF((SELECT CONCAT( ' / ' , cte.typeName, ' ', cte.Name) FROM cte ORDER BY cte.RegionLevel desc FOR XML PATH ('')), 1, 3, '');

RETURN @Result

END

GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetParentRegion]    Script Date: 3/14/2019 3:19:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_GetParentRegion]
(
	@regionId bigint
)
RETURNS bigint
AS
BEGIN
	DECLARE @Result bigint;

	With CTE as
	(
	SELECT RegionId, ParentId, RegionTypeId FROM [dbo].[Region] WITH(NOLOCK) WHERE RegionId = @regionId
	UNION ALL
	SELECT a.RegionId, a.ParentId, a.RegionTypeId 
	FROM [dbo].[Region] AS a
	INNER JOIN cte b 
	ON b.ParentId = a.RegionId 
	AND a.RegionId<>b.RegionId
	AND a.RegionTypeId > 1
	)
	SELECT TOP 1 @Result = RegionId FROM cte ORDER BY RegionTypeId ASC

	RETURN @Result
END

GO
/****** Object:  Table [Audit].[BallotPaper_AUD]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Audit].[BallotPaper_AUD](
	[BallotPaperId] [bigint] NOT NULL,
	[REV] [int] NOT NULL,
	[REVTYPE] [tinyint] NOT NULL,
	[EntryLevel] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[RegisteredVoters] [bigint] NOT NULL,
	[Supplementary] [bigint] NOT NULL,
	[BallotsIssued] [bigint] NOT NULL,
	[BallotsCasted] [bigint] NOT NULL,
	[DifferenceIssuedCasted] [bigint] NOT NULL,
	[BallotsValidVotes] [bigint] NOT NULL,
	[BallotsReceived] [bigint] NOT NULL,
	[BallotsUnusedSpoiled] [bigint] NOT NULL,
	[BallotsSpoiled] [bigint] NOT NULL,
	[BallotsUnused] [bigint] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[DateOfEntry] [date] NOT NULL,
	[VotingPointId] [bigint] NULL,
	[PollingStationId] [bigint] NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[IsResultsConfirmed] [bit] NOT NULL,
	[ConfirmationUserId] [bigint] NULL,
	[ConfirmationDate] [datetime] NULL,
	[Version] [int] NULL,
 CONSTRAINT [PK_Constituency_1] PRIMARY KEY CLUSTERED 
(
	[BallotPaperId] ASC,
	[REV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Audit].[ElectionResult_AUD]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Audit].[ElectionResult_AUD](
	[ElectionResultId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[REV] [int] NOT NULL,
	[REVTYPE] [tinyint] NOT NULL,
	[BallotOrder] [int] NULL,
	[BallotCount] [bigint] NULL,
	[Comments] [nvarchar](max) NULL,
	[DateOfEntry] [datetime] NULL,
	[Status] [int] NULL,
	[ElectionCompetitorId] [bigint] NULL,
	[ElectionCompetitorMemberId] [bigint] NULL,
	[BallotPaperId] [bigint] NULL,
	[EditUserId] [bigint] NULL,
	[EditDate] [datetime] NULL,
	[Version] [int] NULL,
 CONSTRAINT [PK_ElectionResult_AUD] PRIMARY KEY NONCLUSTERED 
(
	[ElectionResultId] ASC,
	[REV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [Audit].[REVINFO]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Audit].[REVINFO](
	[REV] [int] IDENTITY(1,1) NOT NULL,
	[REVTSTMP] [datetime] NULL,
 CONSTRAINT [PK_REVINFO] PRIMARY KEY CLUSTERED 
(
	[REV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Audit].[VoterCertificat_AUD]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Audit].[VoterCertificat_AUD](
	[VoterCertificatId] [bigint] NOT NULL,
	[REV] [int] NOT NULL,
	[REVTYPE] [tinyint] NOT NULL,
	[AssignedVoterId] [bigint] NULL,
	[ReleaseDate] [datetime] NULL,
	[CertificatNr] [nvarchar](255) NULL,
	[PollingStationId] [bigint] NULL,
	[EditUserID] [bigint] NULL,
	[EditDate] [datetime] NULL,
	[Version] [int] NULL,
	[Deleted] [datetime] NULL,
 CONSTRAINT [PK_VoterCertificat_AUD] PRIMARY KEY CLUSTERED 
(
	[VoterCertificatId] ASC,
	[REV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AgeCategories]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AgeCategories](
	[AgeCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[From] [smallint] NOT NULL,
	[To] [smallint] NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_AgeCategories] PRIMARY KEY CLUSTERED 
(
	[AgeCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_AgeCategories] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Alerts]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Alerts](
	[AlertId] [bigint] IDENTITY(1,1) NOT NULL,
	[VoterId] [bigint] NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Patronymic] [nvarchar](100) NULL,
	[Idnp] [bigint] NULL,
	[DateOfBirth] [datetime] NULL,
	[Adress] [nvarchar](max) NULL,
	[DocumentNumber] [nvarchar](50) NULL,
	[PollingStationId] [bigint] NOT NULL,
	[PollingStationAdress] [nvarchar](100) NULL,
	[DateRegistration] [datetimeoffset](7) NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Alerts] PRIMARY KEY CLUSTERED 
(
	[AlertId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssignedCircumscription]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedCircumscription](
	[AssignedCircumscriptionId] [bigint] IDENTITY(1,1) NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[CircumscriptionId] [bigint] NOT NULL,
	[RegionId] [bigint] NOT NULL,
	[Number] [nvarchar](32) NOT NULL,
	[NameRo] [nvarchar](255) NOT NULL,
	[isFromUtan] [bit] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedCircumscription] PRIMARY KEY CLUSTERED 
(
	[AssignedCircumscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssignedPermission]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedPermission](
	[AssignedPermissionId] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[PermissionId] [bigint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedPermissions] PRIMARY KEY CLUSTERED 
(
	[AssignedPermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ElectionDuration]    Script Date: 15.09.2023 14:27:03 ******/
CREATE TABLE [dbo].[ElectionDuration](
	[ElectionDurationId] [int] not null,
	[Name]		[nvarchar](255) not null,
	 CONSTRAINT [PK_ElectionDuration] PRIMARY KEY NONCLUSTERED 
(
	[ElectionDurationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[AssignedPollingStation]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedPollingStation](
	[AssignedPollingStationId] [bigint] IDENTITY(1,1) NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[AssignedCircumscriptionId] [bigint] NOT NULL,
	[PollingStationId] [bigint] NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [bigint] NOT NULL,
	[IsOpen] [bit] NOT NULL,
	[OpeningVoters] [bigint] NOT NULL,
	[EstimatedNumberOfVoters] [int] NOT NULL,
	[NumberOfRoBallotPapers] [int] NOT NULL,
	[NumberOfRuBallotPapers] [int] NOT NULL,
	[ImplementsEVR] [bit] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[isOpeningEnabled] [bit] NOT NULL,
	[isTurnoutEnabled] [bit] NOT NULL,
	[isElectionResultEnabled] [bit] NOT NULL,
	[numberPerElection] [char](10) NULL,
	[RegionId] [bigint] NULL,
	[ParentRegionId] [bigint] NULL,
	[CirculationRo] [int] NULL,
	[CirculationRu] [int] NULL,
	-- Added by Alexandru Gisca, Script Date: 15.09.2023 14:27:05  --
	[ElectionStartTime] [DATETIME],
    [ElectionEndTime] [DATETIME],
    [TimeDifferenceMoldova] [INT],   
    [HoursExtended] [INT],     
    [IsSuspended] [BIT] NOT NULL DEFAULT 0,
    [IsCapturingSignature] [BIT] NOT NULL DEFAULT 0,
    [ElectionDurationId] [INT] NULL

 CONSTRAINT [PK_AssignedPollingStation] PRIMARY KEY NONCLUSTERED 
(
	[AssignedPollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[AssignedRole]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedRole](
	[AssignedRoleId] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[SystemUserId] [bigint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedRoles] PRIMARY KEY CLUSTERED 
(
	[AssignedRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssignedVoter]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedVoter](
	[AssignedVoterId] [bigint] IDENTITY(1,1) NOT NULL,
	[RegionId] [bigint] NOT NULL,
	[RequestingPollingStationId] [bigint] NOT NULL,
	[PollingStationId] [bigint] NOT NULL,
	[VoterId] [bigint] NOT NULL,
	[Category] [bigint] NOT NULL,
	[Status] [bigint] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[ElectionListNr] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedVoter] PRIMARY KEY CLUSTERED 
(
	[AssignedVoterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssignedVoterStatistics]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssignedVoterStatistics](
	[AssignedVoterStatisticId] [bigint] IDENTITY(1,1) NOT NULL,
	[AssignedVoterId] [bigint] NOT NULL,
	[AssignedVoterStatus] [int] NOT NULL,
	[Gender] [int] NOT NULL,
	[AgeCategoryId] [int] NOT NULL,
	[PollingStationId] [bigint] NOT NULL,
	[RegionId] [bigint] NOT NULL,
	[ParentRegionId] [bigint] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[Signature] [varbinary](MAX) NULL,
 CONSTRAINT [PK_AssignedVoterStatistics] PRIMARY KEY CLUSTERED 
(
	[AssignedVoterStatisticId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditEvents]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditEvents](
	[auditEventId] [bigint] IDENTITY(1,1) NOT NULL,
	[auditEventTypeId] [bigint] NOT NULL,
	[level] [tinyint] NOT NULL,
	[generatedAt] [datetime] NOT NULL,
	[message] [nvarchar](1000) NULL,
	[userId] [nvarchar](128) NULL,
	[userMachineIp] [nvarchar](50) NULL,
	[EditUserID] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AuditEvents] PRIMARY KEY CLUSTERED 
(
	[auditEventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditEventTypes]    Script Date: 07.08.2019 14:27:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditEventTypes](
	[auditEventTypeId] [bigint] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[auditStrategy] [smallint] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](500) NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AuditEventTypes] PRIMARY KEY CLUSTERED 
(
	[auditEventTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BallotPaper]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BallotPaper](
	[BallotPaperId] [bigint] IDENTITY(1,1) NOT NULL,
	[PollingStationId] [bigint] NULL,
	[ElectionRoundId] [bigint] NULL,
	[EntryLevel] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[RegisteredVoters] [bigint] NOT NULL,
	[Supplementary] [bigint] NOT NULL,
	[BallotsIssued] [bigint] NOT NULL,
	[BallotsCasted] [bigint] NOT NULL,
	[DifferenceIssuedCasted] [bigint] NOT NULL,
	[BallotsValidVotes] [bigint] NOT NULL,
	[BallotsReceived] [bigint] NOT NULL,
	[BallotsUnusedSpoiled] [bigint] NOT NULL,
	[BallotsSpoiled] [bigint] NOT NULL,
	[BallotsUnused] [bigint] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[DateOfEntry] [date] NOT NULL,
	[VotingPointId] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[IsResultsConfirmed] [bit] NOT NULL,
	[ConfirmationUserId] [bigint] NULL,
	[ConfirmationDate] [datetime] NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Constituency_1] PRIMARY KEY CLUSTERED 
(
	[BallotPaperId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CircumscriptionRegion]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CircumscriptionRegion](
	[CircumscriptionRegionId] [bigint] IDENTITY(1,1) NOT NULL,
	[AssignedCircumscriptionId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[RegionId] [bigint] NOT NULL,
 CONSTRAINT [PK_CircumscriptionRegion] PRIMARY KEY CLUSTERED 
(
	[CircumscriptionRegionId] ASC,
	[AssignedCircumscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Election]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Election](
	[ElectionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [bigint] NOT NULL,
	[Status] [int] NOT NULL,
	[DateOfElection] [datetime] NOT NULL,
	[Comments] [nvarchar](max) NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[ReportsPath] [varchar](max) NOT NULL,
	[BuletinDateOfElectionRo] [nvarchar](20) NULL,
	[BuletinDateOfElectionRu] [nvarchar](20) NULL,
 CONSTRAINT [PK_Election] PRIMARY KEY CLUSTERED 
(
	[ElectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ElectionCompetitor]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectionCompetitor](
	[ElectionCompetitorId] [bigint] IDENTITY(1,1) NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[AssignedCircumscriptionId] [bigint] NULL,
	[PoliticalPartyId] [bigint] NULL,
	[Code] [nvarchar](100) NOT NULL,
	[NameRo] [nvarchar](max) NOT NULL,
	[NameRu] [nvarchar](max) NOT NULL,
	[ColorLogo] [varbinary](max) NULL,
	[DateOfRegistration] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[IsIndependent] [bit] NOT NULL,
	[BallotOrder] [int] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[PartyOrder] [int] NULL,
	[DisplayFromNameRo] [nvarchar](max) NULL,
	[DisplayFromNameRu] [nvarchar](max) NULL,
	[RegistryNumber] [int] NULL,
	[BlackWhiteLogo] [varbinary](max) NULL,
	[PartyType] [int] NOT NULL,
	[BallotPaperNameRo] [nvarchar](max) NULL,
	[BallotPaperNameRu] [nvarchar](max) NULL,
	[BallotPapperCustomCssRo] [nchar](128) NULL,
	[BallotPapperCustomCssRu] [nchar](128) NULL,
	[Color] [varchar](6) NULL,
 CONSTRAINT [PK_ElectionCompetitor] PRIMARY KEY CLUSTERED 
(
	[ElectionCompetitorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ElectionCompetitorMember]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectionCompetitorMember](
	[ElectionCompetitorMemberId] [bigint] IDENTITY(1,1) NOT NULL,
	[AssignedCircumscriptionId] [bigint] NULL,
	[ElectionRoundId] [bigint] NULL,
	[LastNameRo] [nvarchar](100) NOT NULL,
	[LastNameRu] [nvarchar](100) NULL,
	[NameRo] [nvarchar](100) NOT NULL,
	[NameRu] [nvarchar](100) NULL,
	[PatronymicRo] [nvarchar](100) NULL,
	[PatronymicRu] [nvarchar](100) NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[PlaceOfBirth] [nvarchar](100) NOT NULL,
	[Gender] [int] NOT NULL,
	[Occupation] [nvarchar](100) NULL,
	[OccupationRu] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[DesignationRu] [nvarchar](100) NULL,
	[Workplace] [nvarchar](200) NULL,
	[WorkplaceRu] [nvarchar](200) NULL,
	[Idnp] [bigint] NOT NULL,
	[ElectionCompetitorId] [bigint] NULL,
	[DateOfRegistration] [date] NULL,
	[ColorLogo] [varbinary](max) NULL,
	[BlackWhiteLogo] [varbinary](max) NULL,
	[Picture] [varbinary](max) NULL,
	[Status] [int] NOT NULL,
	[CompetitorMemberOrder] [int] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ElectionCompetitorMember] PRIMARY KEY NONCLUSTERED 
(
	[ElectionCompetitorMemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ElectionDay]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectionDay](
	[ElectionDayId] [bigint] IDENTITY(1,1) NOT NULL,
	[ElectionDayDate] [datetimeoffset](7) NOT NULL,
	[DeployDbDate] [datetimeoffset](7) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[StartDateToReportDb] [datetime] NULL,
	[EndDateToReportDb] [datetime] NULL,
 CONSTRAINT [PK_ElectionDays] PRIMARY KEY CLUSTERED 
(
	[ElectionDayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ElectionResult]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectionResult](
	[ElectionResultId] [bigint] IDENTITY(1,1) NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[BallotOrder] [int] NOT NULL,
	[BallotCount] [bigint] NOT NULL,
	[Comments] [nvarchar](max) NOT NULL,
	[DateOfEntry] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[ElectionCompetitorId] [bigint] NOT NULL,
	[ElectionCompetitorMemberId] [bigint] NULL,
	[BallotPaperId] [bigint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ElectionResult] PRIMARY KEY NONCLUSTERED 
(
	[ElectionResultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ElectionRound]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectionRound](
	[ElectionRoundId] [bigint] IDENTITY(1,1) NOT NULL,
	[ElectionId] [bigint] NOT NULL,
	[Number] [tinyint] NOT NULL,
	[NameRo] [nvarchar](255) NOT NULL,
	[NameRu] [nvarchar](255) NULL,
	[Description] [nvarchar](1000) NULL,
	[ElectionDate] [date] NOT NULL,
	[CampaignStartDate] [date] NULL,
	[CampaignEndDate] [date] NULL,
	[Status] [tinyint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ElectionRounds] PRIMARY KEY CLUSTERED 
(
	[ElectionRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ElectionRounds] UNIQUE NONCLUSTERED 
(
	[ElectionId] ASC,
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ElectionType]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ElectionType](
	[ElectionTypeId] [bigint] NOT NULL,
	[Code] [int] NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[ElectionArea] [tinyint] NULL,
	[ElectionCompetitorType] [tinyint] NULL,
	[ElectionRoundsNo] [tinyint] NULL,
	[AcceptResidenceDoc] [bit] NULL,
	[AcceptVotingCert] [bit] NULL,
	[AcceptAbroadDeclaration] [bit] NULL,
 CONSTRAINT [PK_ElectionType] PRIMARY KEY CLUSTERED 
(
	[ElectionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permission]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permission](
	[PermissionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Permission] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PoliticalPartyStatusOverride]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PoliticalPartyStatusOverride](
	[PoliticalPartyStatusOverrideId] [bigint] IDENTITY(1,1) NOT NULL,
	[PoliticalPartyStatus] [int] NOT NULL,
	[ElectionCompetitorId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[AssignedCircumscriptionId] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_PoliticalPartyStatusOverride] PRIMARY KEY CLUSTERED 
(
	[PoliticalPartyStatusOverrideId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PollingStation]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PollingStation](
	[PollingStationId] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[SubNumber] [nvarchar](50) NULL,
	[OldName] [nvarchar](max) NULL,
	[NameRo] [nvarchar](max) NULL,
	[NameRu] [nvarchar](max) NULL,
	[Address] [nvarchar](500) NULL,
	[RegionId] [bigint] NOT NULL,
	[StreetId] [bigint] NULL,
	[StreetNumber] [int] NULL,
	[StreetSubNumber] [nvarchar](50) NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[LocationLatitude] [float] NULL,
	[LocationLongitude] [float] NULL,
	[ExcludeInLocalElections] [bit] NOT NULL,
 CONSTRAINT [PK_PollingUnit] PRIMARY KEY CLUSTERED 
(
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Region]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Region](
	[RegionId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[NameRu] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
	[ParentId] [bigint] NULL,
	[RegionTypeId] [bigint] NOT NULL,
	[RegistryId] [bigint] NULL,
	[StatisticCode] [bigint] NULL,
	[StatisticIdentifier] [bigint] NULL,
	[HasStreets] [bit] NOT NULL,
	[GeoLatitude] [float] NULL,
	[GeoLongitude] [float] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RegionType]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegionType](
	[RegionTypeId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Rank] [tinyint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_RegionType] PRIMARY KEY CLUSTERED 
(
	[RegionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ReportParams]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportParams](
	[ReportParamId] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_ReportParams] PRIMARY KEY CLUSTERED 
(
	[ReportParamId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UK_ReportParams_code] UNIQUE NONCLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReportParamValues]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportParamValues](
	[ReportParamValueId] [bigint] IDENTITY(1,1) NOT NULL,
	[ReportParamId] [bigint] NOT NULL,
	[ElectionTypeId] [bigint] NOT NULL,
	[Value] [nvarchar](512) NULL,
 CONSTRAINT [PK_ReportParamValues] PRIMARY KEY CLUSTERED 
(
	[ReportParamValueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Role]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Level] [int] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SystemUser]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemUser](
	[SystemUserId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Level] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[Idnp] [bigint] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[Surname] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[Gender] [int] NOT NULL,
	[PasswordQuestion] [nvarchar](100) NULL,
	[PasswordAnswer] [nvarchar](100) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsOnLine] [bit] NOT NULL,
	[IsLockedOut] [bit] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastActivityDate] [datetime] NOT NULL,
	[LastPasswordChangedDate] [datetime] NOT NULL,
	[LastLockoutDate] [datetime] NOT NULL,
	[FailedAttemptStart] [datetime] NOT NULL,
	[FailedAnswerStart] [datetime] NOT NULL,
	[FailedAttemptCount] [int] NOT NULL,
	[FailedAnswerCount] [int] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
	[Language] [nvarchar](100) NULL,
	[MobileNumber] [nvarchar](20) NULL,
	[ContactName] [nvarchar](100) NULL,
	[ContactMobileNumber] [nvarchar](20) NULL,
	[StreetAddress] [nvarchar](100) NULL,
	[ElectionId] [bigint] NULL,
	[RegionId] [bigint] NULL,
	[PollingStationId] [bigint] NULL,
	[CircumscriptionId] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SystemUser_1] PRIMARY KEY CLUSTERED 
(
	[SystemUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Voter]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Voter](
	[VoterId] [bigint] IDENTITY(1,1) NOT NULL,
	[NameRo] [nvarchar](100) NOT NULL,
	[LastNameRo] [nvarchar](100) NOT NULL,
	[PatronymicRo] [nvarchar](100) NULL,
	[LastNameRu] [nvarchar](100) NULL,
	[NameRu] [nvarchar](100) NULL,
	[PatronymicRu] [nvarchar](100) NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[PlaceOfBirth] [nvarchar](max) NULL,
	[PlaceOfResidence] [nvarchar](max) NULL,
	[Gender] [int] NOT NULL,
	[DateOfRegistration] [datetime] NOT NULL,
	[Idnp] [bigint] NOT NULL,
	[DocumentNumber] [nvarchar](50) NOT NULL,
	[DateOfIssue] [datetime] NULL,
	[DateOfExpiry] [datetime] NULL,
	[Status] [bigint] NOT NULL,
	[BatchId] [bigint] NULL,
	[StreetId] [bigint] NULL,
	[RegionId] [bigint] NOT NULL,
	[StreetName] [nvarchar](max) NULL,
	[StreetNumber] [bigint] NULL,
	[StreetSubNumber] [nvarchar](50) NULL,
	[BlockNumber] [bigint] NULL,
	[BlockSubNumber] [nvarchar](50) NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[ElectionListNr] [bigint] NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[VoterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoterCertificat]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoterCertificat](
	[VoterCertificatId] [bigint] IDENTITY(1,1) NOT NULL,
	[AssignedVoterId] [bigint] NOT NULL,
	[ReleaseDate] [datetime] NOT NULL,
	[CertificatNr] [nvarchar](255) NOT NULL,
	[PollingStationId] [bigint] NOT NULL,
	[EditUserID] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[Deleted] [datetime] NULL,
 CONSTRAINT [PK_VoterCertificat] PRIMARY KEY CLUSTERED 
(
	[VoterCertificatId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[TemplateTypes]   Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemplateTypes](
    [TemplateTypeId] [bigint] NOT NULL,
    [Title] [nvarchar](255) NOT NULL,
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_TemplateTypes] PRIMARY KEY CLUSTERED 
    (
        [TemplateTypeId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId])
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[TemplateNames]   Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemplateNames](
    [TemplateNameId] [bigint] NOT NULL,
    [Title] [nvarchar](255) NOT NULL,
    [Description] [nvarchar](MAX),
    [TemplateTypeId] [bigint],
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_TemplateNames] PRIMARY KEY CLUSTERED 
    (
        [TemplateNameId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId]),
    FOREIGN KEY ([TemplateTypeId]) REFERENCES [TemplateTypes]([TemplateTypeId])
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Templates]    Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Templates](
    [TemplateId] [bigint] NOT NULL,
	[TemplateNameId] [bigint],
	[ParentId] [bigint] NULL,
	[ElectionTypeId] [bigint] NULL,
    [Content] [nvarchar](MAX) NOT NULL,
    [UploadDate] [datetime] NOT NULL,
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED 
    (
        [TemplateId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    FOREIGN KEY ([TemplateNameId]) REFERENCES [TemplateNames]([TemplateNameId]),
    FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId]),
	FOREIGN KEY ([ElectionTypeId]) REFERENCES [ElectionType]([ElectionTypeId]),
	FOREIGN KEY ([ParentId]) REFERENCES [Templates]([TemplateId])
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ReportParameters]    Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportParameters](
    [ReportParameterId] [bigint] NOT NULL,
    [ParameterName] [nvarchar](255) NOT NULL,
    [ParameterDescription] [nvarchar](MAX),
    [ParameterCode] [nvarchar](255),
    [IsLookup] [bit] NOT NULL,
    [TemplateId] [bigint],
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_ReportParameters] PRIMARY KEY CLUSTERED 
    (
        [ReportParameterId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    FOREIGN KEY ([TemplateId]) REFERENCES [Templates]([TemplateId]),
    FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId])
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Documents]   Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
    [DocumentId] [bigint] IDENTITY(1,1) NOT NULL,
    [PollingStationId] [bigint] NULL, 
    [ElectionRoundId] [bigint] NULL, 
	[AssignedCircumscriptionId] [bigint] NULL,
	[BallotPaperId] [bigint] NULL,
    [StatusId] [int], 
    [EntryLevelId] [int],
    [IsResultsConfirmed] [bit] NOT NULL DEFAULT 0,
    [ConfirmationUserId] [bigint],
    [ConfirmationDate] [datetime],
    [DocumentName] [nvarchar](255),
    [DocumentPath] [nvarchar](255),
    [FileSize] [nvarchar](255),
    [FileLength] [int],
    [FileContent] [varbinary](MAX),
    [FileExtension] [nvarchar](50),
    [TemplateId] [bigint],
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED 
    (
        [DocumentId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    FOREIGN KEY ([TemplateId]) REFERENCES [Templates]([TemplateId]),
    FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId]),
	FOREIGN KEY ([PollingStationId]) REFERENCES [PollingStation]([PollingStationId]),
	FOREIGN KEY ([ElectionRoundId]) REFERENCES [ElectionRound]([ElectionRoundId]),
	FOREIGN KEY ([BallotPaperId]) REFERENCES [BallotPaper]([BallotPaperId]),
	FOREIGN KEY ([AssignedCircumscriptionId]) REFERENCES [AssignedCircumscription]([AssignedCircumscriptionId])
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ReportParameterValues]    Script Date: 22.08.2023 15:40:51 ******/
CREATE TABLE [dbo].[ReportParameterValues](
    [ReportParameterValueId] [bigint] IDENTITY(1,1) NOT NULL,
    [ValueContent] [nvarchar](MAX),
    [DocumentId] [bigint],
    [ReportParameterId] [bigint],
	[ElectionCompetitorName] [NVARCHAR](255) NULL,
	[ElectionCompetitorMemberName] [NVARCHAR](255) NULL,
	[BallotCount] [bigint] NULL,
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_ReportParameterValues] PRIMARY KEY CLUSTERED 
    (
        [ReportParameterValueId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    FOREIGN KEY ([DocumentId]) REFERENCES [Documents]([DocumentId]),
    FOREIGN KEY ([ReportParameterId]) REFERENCES [ReportParameters]([ReportParameterId]),
    FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId])
) ON [PRIMARY]
GO


/****** Object:  Table [schematmp].[AgeCategories]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[AgeCategories](
	[AgeCategoryId] [int] NOT NULL,
	[From] [smallint] NOT NULL,
	[To] [smallint] NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_AgeCategories] PRIMARY KEY CLUSTERED 
(
	[AgeCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[AssignedCircumscription]    Script Date: 07.08.2019 14:27:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[AssignedCircumscription](
	[AssignedCircumscriptionId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[CircumscriptionId] [bigint] NOT NULL,
	[RegionId] [bigint] NOT NULL,
	[Number] [nvarchar](32) NOT NULL,
	[NameRo] [nvarchar](255) NOT NULL,
	[isFromUtan] [bit] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedCircumscription] PRIMARY KEY CLUSTERED 
(
	[AssignedCircumscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[AssignedPermission]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[AssignedPermission](
	[AssignedPermissionId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[PermissionId] [bigint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedPermissions] PRIMARY KEY CLUSTERED 
(
	[AssignedPermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[AssignedPollingStation]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[AssignedPollingStation](
	[AssignedPollingStationId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[AssignedCircumscriptionId] [bigint] NOT NULL,
	[PollingStationId] [bigint] NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [bigint] NOT NULL,
	[IsOpen] [bit] NOT NULL,
	[OpeningVoters] [bigint] NOT NULL,
	[EstimatedNumberOfVoters] [int] NOT NULL,
	[NumberOfRoBallotPapers] [int] NOT NULL,
	[NumberOfRuBallotPapers] [int] NOT NULL,
	[ImplementsEVR] [bit] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[isOpeningEnabled] [bit] NOT NULL,
	[isTurnoutEnabled] [bit] NOT NULL,
	[isElectionResultEnabled] [bit] NOT NULL,
	[numberPerElection] [char](10) NULL,
	[RegionId] [bigint] NULL,
	[ParentRegionId] [bigint] NULL,
	[CirculationRo] [int] NULL,
	[CirculationRu] [int] NULL,
	-- Added by Alexandru Gisca, Script Date: 15.09.2023 14:27:05  --
	[ElectionStartTime] [DATETIME],
    [ElectionEndTime] [DATETIME],
    [TimeDifferenceMoldova] [INT],   
    [HoursExtended] [INT],     
    [IsSuspended] [BIT] NOT NULL DEFAULT 0,
    [IsCapturingSignature] [BIT] NOT NULL DEFAULT 0,
    [ElectionDurationId] [INT] NULL

 CONSTRAINT [PK_AssignedPollingStation] PRIMARY KEY NONCLUSTERED 
(
	[AssignedPollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [schematmp].[ElectionDuration]    Script Date: 15.09.2023 14:27:03 ******/
CREATE TABLE [schematmp].[ElectionDuration](
	[ElectionDurationId] [int] not null,
	[Name]		[nvarchar](255) not null,
	 CONSTRAINT [PK_ElectionDuration] PRIMARY KEY NONCLUSTERED 
(
	[ElectionDurationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[AssignedRole]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[AssignedRole](
	[AssignedRoleId] [bigint] IDENTITY(1,1) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[SystemUserId] [bigint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedRoles] PRIMARY KEY CLUSTERED 
(
	[AssignedRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[AssignedVoter]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[AssignedVoter](
	[AssignedVoterId] [bigint] NOT NULL,
	[RegionId] [bigint] NOT NULL,
	[RequestingPollingStationId] [bigint] NOT NULL,
	[PollingStationId] [bigint] NOT NULL,
	[VoterId] [bigint] NOT NULL,
	[Category] [bigint] NOT NULL,
	[Status] [bigint] NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[ElectionListNr] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AssignedVoter] PRIMARY KEY CLUSTERED 
(
	[AssignedVoterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[AuditEventTypes]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[AuditEventTypes](
	[auditEventTypeId] [bigint] NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[auditStrategy] [smallint] NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[description] [nvarchar](500) NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_AuditEventTypes] PRIMARY KEY CLUSTERED 
(
	[auditEventTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[BallotPaper]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[BallotPaper](
	[BallotPaperId] [bigint] NOT NULL,
	[PollingStationId] [bigint] NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[EntryLevel] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[RegisteredVoters] [bigint] NOT NULL,
	[Supplementary] [bigint] NOT NULL,
	[BallotsIssued] [bigint] NOT NULL,
	[BallotsCasted] [bigint] NOT NULL,
	[DifferenceIssuedCasted] [bigint] NOT NULL,
	[BallotsValidVotes] [bigint] NOT NULL,
	[BallotsReceived] [bigint] NOT NULL,
	[BallotsUnusedSpoiled] [bigint] NOT NULL,
	[BallotsSpoiled] [bigint] NOT NULL,
	[BallotsUnused] [bigint] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[DateOfEntry] [date] NOT NULL,
	[VotingPointId] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[IsResultsConfirmed] [bit] NOT NULL,
	[ConfirmationUserId] [bigint] NULL,
	[ConfirmationDate] [datetime] NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Constituency_1] PRIMARY KEY CLUSTERED 
(
	[BallotPaperId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[CircumscriptionRegion]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[CircumscriptionRegion](
	[CircumscriptionRegionId] [bigint] NOT NULL,
	[AssignedCircumscriptionId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[RegionId] [bigint] NOT NULL,
 CONSTRAINT [PK_CircumscriptionRegion] PRIMARY KEY CLUSTERED 
(
	[CircumscriptionRegionId] ASC,
	[AssignedCircumscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[Election]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[Election](
	[ElectionId] [bigint] NOT NULL,
	[Type] [bigint] NOT NULL,
	[Status] [int] NOT NULL,
	[DateOfElection] [datetime] NOT NULL,
	[Comments] [nvarchar](max) NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[ReportsPath] [varchar](max) NOT NULL,
	[BuletinDateOfElectionRo] [nvarchar](20) NULL,
	[BuletinDateOfElectionRu] [nvarchar](20) NULL,
 CONSTRAINT [PK_Election] PRIMARY KEY CLUSTERED 
(
	[ElectionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[ElectionCompetitor]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ElectionCompetitor](
	[ElectionCompetitorId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[PoliticalPartyId] [bigint] NULL,
	[AssignedCircumscriptionId] [bigint] NULL,
	[Code] [nvarchar](100) NOT NULL,
	[NameRo] [nvarchar](max) NOT NULL,
	[NameRu] [nvarchar](max) NOT NULL,
	[ColorLogo] [varbinary](max) NULL,
	[DateOfRegistration] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[IsIndependent] [bit] NOT NULL,
	[BallotOrder] [int] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[PartyOrder] [int] NULL,
	[DisplayFromNameRo] [nvarchar](max) NULL,
	[DisplayFromNameRu] [nvarchar](max) NULL,
	[RegistryNumber] [int] NULL,
	[BlackWhiteLogo] [varbinary](max) NULL,
	[PartyType] [int] NOT NULL,
	[BallotPaperNameRo] [nvarchar](max) NULL,
	[BallotPaperNameRu] [nvarchar](max) NULL,
	[BallotPapperCustomCssRo] [nchar](128) NULL,
	[BallotPapperCustomCssRu] [nchar](128) NULL,
	[Color] [varchar](6) NULL,
 CONSTRAINT [PK_ElectionCompetitor] PRIMARY KEY CLUSTERED 
(
	[ElectionCompetitorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[ElectionCompetitorMember]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ElectionCompetitorMember](
	[ElectionCompetitorMemberId] [bigint] NOT NULL,
	[AssignedCircumscriptionId] [bigint] NULL,
	[ElectionRoundId] [bigint] NULL,
	[LastNameRo] [nvarchar](100) NOT NULL,
	[LastNameRu] [nvarchar](100) NULL,
	[NameRo] [nvarchar](100) NOT NULL,
	[NameRu] [nvarchar](100) NULL,
	[PatronymicRo] [nvarchar](100) NULL,
	[PatronymicRu] [nvarchar](100) NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[PlaceOfBirth] [nvarchar](100) NOT NULL,
	[Gender] [int] NOT NULL,
	[Occupation] [nvarchar](100) NULL,
	[OccupationRu] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[DesignationRu] [nvarchar](100) NULL,
	[Workplace] [nvarchar](200) NULL,
	[WorkplaceRu] [nvarchar](200) NULL,
	[Idnp] [bigint] NOT NULL,
	[ElectionCompetitorId] [bigint] NULL,
	[DateOfRegistration] [date] NULL,
	[CompetitorMemberOrder] [int] NOT NULL,
	[ColorLogo] [varbinary](max) NULL,
	[BlackWhiteLogo] [varbinary](max) NULL,
	[Picture] [varbinary](max) NULL,
	[Status] [int] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ElectionCompetitorMember] PRIMARY KEY NONCLUSTERED 
(
	[ElectionCompetitorMemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[ElectionDay]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ElectionDay](
	[ElectionDayId] [bigint] NOT NULL,
	[ElectionDayDate] [datetimeoffset](7) NOT NULL,
	[DeployDbDate] [datetimeoffset](7) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](500) NULL,
 CONSTRAINT [PK_ElectionDays] PRIMARY KEY CLUSTERED 
(
	[ElectionDayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[ElectionResult]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ElectionResult](
	[ElectionResultId] [bigint] IDENTITY(1,1) NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[BallotOrder] [int] NOT NULL,
	[BallotCount] [bigint] NOT NULL,
	[Comments] [nvarchar](max) NOT NULL,
	[DateOfEntry] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
	[ElectionCompetitorId] [bigint] NOT NULL,
	[ElectionCompetitorMemberId] [bigint] NULL,
	[BallotPaperId] [bigint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ElectionResult] PRIMARY KEY NONCLUSTERED 
(
	[ElectionResultId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[ElectionRound]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ElectionRound](
	[ElectionRoundId] [bigint] NOT NULL,
	[ElectionId] [bigint] NOT NULL,
	[Number] [tinyint] NOT NULL,
	[NameRo] [nvarchar](255) NOT NULL,
	[NameRu] [nvarchar](255) NULL,
	[Description] [nvarchar](1000) NULL,
	[ElectionDate] [date] NOT NULL,
	[CampaignStartDate] [date] NULL,
	[CampaignEndDate] [date] NULL,
	[Status] [tinyint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ElectionRounds] PRIMARY KEY CLUSTERED 
(
	[ElectionRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[ElectionType]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ElectionType](
	[ElectionTypeId] [bigint] NOT NULL,
	[Code] [int] NULL,
	[TypeName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
	[ElectionArea] [tinyint] NULL,
	[ElectionCompetitorType] [tinyint] NULL,
	[ElectionRoundsNo] [tinyint] NULL,
	[AcceptResidenceDoc] [bit] NULL,
	[AcceptVotingCert] [bit] NULL,
	[AcceptAbroadDeclaration] [bit] NULL,
 CONSTRAINT [PK_ElectionType] PRIMARY KEY CLUSTERED 
(
	[ElectionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[Permission]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[Permission](
	[PermissionId] [bigint] NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY],
 CONSTRAINT [IX_Permission] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[PoliticalPartyStatusOverride]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[PoliticalPartyStatusOverride](
	[PoliticalPartyStatusOverrideId] [bigint] NOT NULL,
	[PoliticalPartyStatus] [int] NOT NULL,
	[ElectionCompetitorId] [bigint] NOT NULL,
	[ElectionRoundId] [bigint] NOT NULL,
	[AssignedCircumscriptionId] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_PoliticalPartyStatusOverride] PRIMARY KEY CLUSTERED 
(
	[PoliticalPartyStatusOverrideId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[PollingStation]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[PollingStation](
	[PollingStationId] [bigint] NOT NULL,
	[Type] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[SubNumber] [nvarchar](50) NULL,
	[OldName] [nvarchar](max) NULL,
	[NameRo] [nvarchar](max) NULL,
	[NameRu] [nvarchar](max) NULL,
	[Address] [nvarchar](500) NULL,
	[RegionId] [bigint] NULL,
	[StreetId] [bigint] NULL,
	[StreetNumber] [int] NULL,
	[StreetSubNumber] [nvarchar](50) NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[LocationLatitude] [float] NULL,
	[LocationLongitude] [float] NULL,
	[ExcludeInLocalElections] [bit] NOT NULL,
 CONSTRAINT [PK_PollingUnit] PRIMARY KEY CLUSTERED 
(
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[Region]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[Region](
	[RegionId] [bigint] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[NameRu] [nvarchar](100) NULL,
	[Description] [nvarchar](500) NULL,
	[ParentId] [bigint] NULL,
	[RegionTypeId] [bigint] NOT NULL,
	[RegistryId] [bigint] NULL,
	[StatisticCode] [bigint] NULL,
	[StatisticIdentifier] [bigint] NULL,
	[HasStreets] [bit] NOT NULL,
	[GeoLatitude] [float] NULL,
	[GeoLongitude] [float] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[RegionType]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[RegionType](
	[RegionTypeId] [bigint] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Rank] [tinyint] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_RegionTypes] PRIMARY KEY CLUSTERED 
(
	[RegionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [schematmp].[ReportParams]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ReportParams](
	[ReportParamId] [bigint] NOT NULL,
	[Code] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_ReportParam] PRIMARY KEY CLUSTERED 
(
	[ReportParamId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[ReportParamValues]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ReportParamValues](
	[ReportParamValueId] [bigint] NOT NULL,
	[ReportParamId] [bigint] NOT NULL,
	[ElectionTypeId] [bigint] NOT NULL,
	[Value] [nvarchar](512) NULL,
 CONSTRAINT [PK_ReportParamValue] PRIMARY KEY CLUSTERED 
(
	[ReportParamValueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [schematmp].[Role]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[Role](
	[RoleId] [bigint] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Level] [int] NOT NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[SystemUser]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[SystemUser](
	[SystemUserId] [bigint] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Level] [int] NOT NULL,
	[Comments] [nvarchar](max) NULL,
	[Idnp] [bigint] NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[Surname] [nvarchar](100) NOT NULL,
	[MiddleName] [nvarchar](100) NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[Gender] [int] NOT NULL,
	[PasswordQuestion] [nvarchar](100) NULL,
	[PasswordAnswer] [nvarchar](100) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsOnLine] [bit] NOT NULL,
	[IsLockedOut] [bit] NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastActivityDate] [datetime] NOT NULL,
	[LastPasswordChangedDate] [datetime] NOT NULL,
	[LastLockoutDate] [datetime] NOT NULL,
	[FailedAttemptStart] [datetime] NOT NULL,
	[FailedAnswerStart] [datetime] NOT NULL,
	[FailedAttemptCount] [int] NOT NULL,
	[FailedAnswerCount] [int] NOT NULL,
	[LastLoginDate] [datetime] NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
	[Language] [nvarchar](100) NULL,
	[MobileNumber] [nvarchar](20) NULL,
	[ContactName] [nvarchar](100) NULL,
	[ContactMobileNumber] [nvarchar](20) NULL,
	[StreetAddress] [nvarchar](100) NULL,
	[ElectionId] [bigint] NULL,
	[RegionId] [bigint] NULL,
	[PollingStationId] [bigint] NULL,
	[CircumscriptionId] [bigint] NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_SystemUser_1] PRIMARY KEY CLUSTERED 
(
	[SystemUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [schematmp].[Voter]    Script Date: 07.08.2019 14:27:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[Voter](
	[VoterId] [bigint] NOT NULL,
	[NameRo] [nvarchar](100) NOT NULL,
	[LastNameRo] [nvarchar](100) NOT NULL,
	[PatronymicRo] [nvarchar](100) NULL,
	[LastNameRu] [nvarchar](100) NULL,
	[NameRu] [nvarchar](100) NULL,
	[PatronymicRu] [nvarchar](100) NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[PlaceOfBirth] [nvarchar](max) NULL,
	[PlaceOfResidence] [nvarchar](max) NULL,
	[Gender] [int] NOT NULL,
	[DateOfRegistration] [datetime] NOT NULL,
	[Idnp] [bigint] NOT NULL,
	[DocumentNumber] [nvarchar](50) NOT NULL,
	[DateOfIssue] [datetime] NULL,
	[DateOfExpiry] [datetime] NULL,
	[Status] [bigint] NOT NULL,
	[BatchId] [bigint] NULL,
	[StreetId] [bigint] NULL,
	[RegionId] [bigint] NOT NULL,
	[StreetName] [nvarchar](max) NULL,
	[StreetNumber] [bigint] NULL,
	[StreetSubNumber] [nvarchar](50) NULL,
	[BlockNumber] [bigint] NULL,
	[BlockSubNumber] [nvarchar](50) NULL,
	[EditUserId] [bigint] NOT NULL,
	[EditDate] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
	[ElectionListNr] [bigint] NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[VoterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [schematmp].[TemplateNames]   Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[TemplateNames](
    [TemplateNameId] [bigint] NOT NULL,
    [Title] [nvarchar](255) NOT NULL,
    [Description] [nvarchar](MAX),
    [TemplateTypeId] [bigint],
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_TemplateNames] PRIMARY KEY CLUSTERED 
    (
        [TemplateNameId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    --FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId]),
    --FOREIGN KEY ([TemplateTypeId]) REFERENCES [TemplateTypes]([TemplateTypeId])
) ON [PRIMARY]
GO

/****** Object:  Table [schematmp].[Templates]    Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[Templates](
    [TemplateId] [bigint] NOT NULL,
	[TemplateNameId] [bigint],
	[ParentId] [bigint] NULL,
	[ElectionTypeId] [bigint] NULL,
    [Content] [nvarchar](MAX) NOT NULL,
    [UploadDate] [datetime] NOT NULL,
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_Templates] PRIMARY KEY CLUSTERED 
    (
        [TemplateId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 --   FOREIGN KEY ([TemplateNameId]) REFERENCES [TemplateNames]([TemplateNameId]),
 --   FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId]),
	--FOREIGN KEY ([ElectionTypeId]) REFERENCES [ElectionType]([ElectionTypeId]),
	--FOREIGN KEY ([ParentId]) REFERENCES [Templates]([TemplateId])
) ON [PRIMARY]
GO

/****** Object:  Table [schematmp].[ReportParameters]    Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[ReportParameters](
    [ReportParameterId] [bigint] NOT NULL,
    [ParameterName] [nvarchar](255) NOT NULL,
    [ParameterDescription] [nvarchar](MAX),
    [ParameterCode] [nvarchar](255),
    [IsLookup] [bit] NOT NULL,
    [TemplateId] [bigint],
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_ReportParameters] PRIMARY KEY CLUSTERED 
    (
        [ReportParameterId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    --FOREIGN KEY ([TemplateId]) REFERENCES [Templates]([TemplateId]),
    --FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId])
) ON [PRIMARY]
GO

/****** Object:  Table [schematmp].[TemplateTypes]   Script Date: 22.08.2023 15:40:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [schematmp].[TemplateTypes](
    [TemplateTypeId] [bigint] NOT NULL,
    [Title] [nvarchar](255) NOT NULL,
    [EditUserId] [bigint],
    [EditDate] [datetime],
    [Version] [int],
    CONSTRAINT [PK_TemplateTypes] PRIMARY KEY CLUSTERED 
    (
        [TemplateTypeId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    --FOREIGN KEY ([EditUserId]) REFERENCES [SystemUser]([SystemUserId])
) ON [PRIMARY]
GO


/****** Object:  View [dbo].[AuditVoter]    Script Date: 3/14/2019 3:19:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AuditVoter]
AS
SELECT        TOP (100) PERCENT dbo.SystemUser.UserName, dbo.AuditEvents.auditEventId, dbo.AuditEvents.auditEventTypeId, dbo.AuditEvents.[level], 
                         dbo.AuditEvents.generatedAt, dbo.AuditEvents.message, dbo.AuditEvents.userId, dbo.AuditEvents.userMachineIp, dbo.AuditEvents.EditUserID, 
                         dbo.AuditEvents.EditDate, dbo.AuditEvents.Version, dbo.SystemUser.PollingStationId, dbo.SystemUser.CircumscriptionId
FROM            dbo.AuditEvents INNER JOIN
                         dbo.SystemUser ON dbo.AuditEvents.EditUserID = dbo.SystemUser.SystemUserId
WHERE        (dbo.AuditEvents.message IN
                             (SELECT        message
                               FROM            (SELECT        message, COUNT(*) AS a
                                                         FROM            dbo.AuditEvents AS AuditEvents_1
                                                         WHERE        (auditEventTypeId = 67)
                                                         GROUP BY message) AS b
                               WHERE        (a > 1)))
ORDER BY dbo.AuditEvents.message, dbo.AuditEvents.EditDate

GO
/****** Object:  View [dbo].[CandidatiUninominalToti]    Script Date: 3/14/2019 3:19:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CandidatiUninominalToti]
AS
SELECT        dbo.AssignedCircumscription.Number, dbo.AssignedCircumscription.NameRo AS Expr2, dbo.ElectionCompetitor.Code, dbo.ElectionCompetitor.NameRo AS Expr1, 
                         dbo.ElectionCompetitorMember.NameRo, dbo.ElectionCompetitorMember.LastNameRo, dbo.ElectionCompetitorMember.NameRu, 
                         dbo.ElectionCompetitorMember.LastNameRu, dbo.ElectionCompetitorMember.DateOfBirth, dbo.ElectionCompetitorMember.Idnp, 
                         dbo.ElectionCompetitorMember.Gender
FROM            dbo.ElectionCompetitor INNER JOIN
                         dbo.ElectionCompetitorMember ON dbo.ElectionCompetitor.ElectionCompetitorId = dbo.ElectionCompetitorMember.ElectionCompetitorId INNER JOIN
                         dbo.AssignedCircumscription ON dbo.ElectionCompetitor.AssignedCircumscriptionId = dbo.AssignedCircumscription.AssignedCircumscriptionId
WHERE        (dbo.ElectionCompetitorMember.ElectionRoundId = 10044)

GO
/****** Object:  View [dbo].[Rezultate]    Script Date: 3/14/2019 3:19:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Rezultate]
AS
SELECT        TOP (100) PERCENT CASE WHEN A.C <= (A.A + A.B) AND A.C >= A. D AND A. D = (A.F + A.H) AND A.E = (A.C - A. D) AND A.F = (A. D - A.H) AND A.I = (A.C + A.J) AND 
                         A.J = (A.I - A.C) THEN '' ELSE 'eroare' END AS EroareFormula, CASE WHEN A.I <> A.TIRAJ THEN 'incorect' ELSE '' END AS EroareTiraj, CircNr, SectieNr, ScrutinIndex, 
                         SCRUTIN, BallotPaperId, RECEPTIONAT, TIRAJ, A, B, C, D, E, F, H, I, J, G1, G2, G3, G4, G5, G6, G7, G8, G9, G10, G11, G12, G13, G14, G15
FROM            (SELECT        AC2.Number AS CircNr, APS2.numberPerElection AS SectieNr, 1 AS ScrutinIndex, 'Proportional' AS SCRUTIN, BP.BallotPaperId, 
                                                    CASE WHEN BP.Status > 0 THEN 'Da' ELSE 'Nu' END AS RECEPTIONAT, APS2.NumberOfRoBallotPapers + APS2.NumberOfRuBallotPapers AS TIRAJ, 
                                                    BP.RegisteredVoters AS A, BP.Supplementary AS B, BP.BallotsIssued AS C, BP.BallotsCasted AS D, BP.DifferenceIssuedCasted AS E, 
                                                    BP.BallotsSpoiled AS F, BP.BallotsValidVotes AS H, BP.BallotsReceived AS I, BP.BallotsUnusedSpoiled AS J,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 1)) AS G1,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 2)) AS G2,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 3)) AS G3,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 4)) AS G4,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 5)) AS G5,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 6)) AS G6,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 7)) AS G7,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 8)) AS G8,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 9)) AS G9,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 10)) AS G10,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 11)) AS G11,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 12)) AS G12,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 13)) AS G13,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 14)) AS G14,
                                                        (SELECT        ER.BallotCount
                                                          FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                    dbo.ElectionCompetitor AS EC ON EC.ElectionCompetitorId = ER.ElectionCompetitorId
                                                          WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 15)) AS G15
                          FROM            dbo.AssignedCircumscription AS AC INNER JOIN
                                                    dbo.AssignedPollingStation AS APS ON APS.AssignedCircumscriptionId = AC.AssignedCircumscriptionId INNER JOIN
                                                    dbo.BallotPaper AS BP ON BP.ElectionRoundId = AC.ElectionRoundId AND BP.PollingStationId = APS.PollingStationId INNER JOIN
                                                    dbo.PollingStation AS PS ON PS.PollingStationId = APS.PollingStationId INNER JOIN
                                                    dbo.AssignedPollingStation AS APS2 ON APS2.ElectionRoundId = 10044 AND APS2.PollingStationId = APS.PollingStationId INNER JOIN
                                                    dbo.AssignedCircumscription AS AC2 ON AC2.AssignedCircumscriptionId = APS2.AssignedCircumscriptionId
                          WHERE        (AC.ElectionRoundId = 10048)
                          UNION ALL
                          SELECT        AC.Number AS CircNr, APS.numberPerElection AS SectieNr, 2 AS ScrutinIndex, 'Uninominal' AS SCRUTIN, BP.BallotPaperId, 
                                                   CASE WHEN BP.Status > 0 THEN 'Da' ELSE 'Nu' END AS RECEPTIONAT, APS.NumberOfRoBallotPapers + APS.NumberOfRuBallotPapers AS TIRAJ, 
                                                   BP.RegisteredVoters AS A, BP.Supplementary AS B, BP.BallotsIssued AS C, BP.BallotsCasted AS D, BP.DifferenceIssuedCasted AS E, 
                                                   BP.BallotsSpoiled AS F, BP.BallotsValidVotes AS H, BP.BallotsReceived AS I, BP.BallotsUnusedSpoiled AS J,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 1)) AS G1,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 2)) AS G2,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 3)) AS G3,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 4)) AS G4,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 5)) AS G5,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 6)) AS G6,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 7)) AS G7,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 8)) AS G8,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 9)) AS G9,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 10)) AS G10,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 11)) AS G11,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 12)) AS G12,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 13)) AS G13, NULL AS G14, NULL AS G15
                          FROM            dbo.AssignedCircumscription AS AC INNER JOIN
                                                   dbo.AssignedPollingStation AS APS ON APS.AssignedCircumscriptionId = AC.AssignedCircumscriptionId INNER JOIN
                                                   dbo.BallotPaper AS BP ON BP.ElectionRoundId = AC.ElectionRoundId AND BP.PollingStationId = APS.PollingStationId INNER JOIN
                                                   dbo.PollingStation AS PS ON PS.PollingStationId = APS.PollingStationId
                          WHERE        (AC.ElectionRoundId = 10044)
                          UNION ALL
                          SELECT        AC2.Number AS CircNr, APS2.numberPerElection AS SectieNr, 3 AS ScrutinIndex, 'REF-101' AS SCRUTIN, BP.BallotPaperId, 
                                                   CASE WHEN BP.Status > 0 THEN 'Da' ELSE 'Nu' END AS RECEPTIONAT, APS2.NumberOfRoBallotPapers + APS2.NumberOfRuBallotPapers AS TIRAJ, 
                                                   BP.RegisteredVoters AS A, BP.Supplementary AS B, BP.BallotsIssued AS C, BP.BallotsCasted AS D, BP.DifferenceIssuedCasted AS E, 
                                                   BP.BallotsSpoiled AS F, BP.BallotsValidVotes AS H, BP.BallotsReceived AS I, BP.BallotsUnusedSpoiled AS J,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 1)) AS G1,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 2)) AS G2, NULL AS G3, NULL AS G4, NULL AS G5, NULL AS G6, NULL 
                                                   AS G7, NULL AS G8, NULL AS G9, NULL AS G10, NULL AS G11, NULL AS G12, NULL AS G13, NULL AS G14, NULL AS G15
                          FROM            dbo.AssignedCircumscription AS AC INNER JOIN
                                                   dbo.AssignedPollingStation AS APS ON APS.AssignedCircumscriptionId = AC.AssignedCircumscriptionId INNER JOIN
                                                   dbo.BallotPaper AS BP ON BP.ElectionRoundId = AC.ElectionRoundId AND BP.PollingStationId = APS.PollingStationId INNER JOIN
                                                   dbo.PollingStation AS PS ON PS.PollingStationId = APS.PollingStationId INNER JOIN
                                                   dbo.AssignedPollingStation AS APS2 ON APS2.ElectionRoundId = 10044 AND APS2.PollingStationId = APS.PollingStationId INNER JOIN
                                                   dbo.AssignedCircumscription AS AC2 ON AC2.AssignedCircumscriptionId = APS2.AssignedCircumscriptionId
                          WHERE        (AC.ElectionRoundId = 10045)
                          UNION ALL
                          SELECT        AC2.Number AS CircNr, APS2.numberPerElection AS SectieNr, 4 AS ScrutinIndex, 'REF-Revoc' AS SCRUTIN, BP.BallotPaperId, 
                                                   CASE WHEN BP.Status > 0 THEN 'Da' ELSE 'Nu' END AS RECEPTIONAT, APS2.NumberOfRoBallotPapers + APS2.NumberOfRuBallotPapers AS TIRAJ, 
                                                   BP.RegisteredVoters AS A, BP.Supplementary AS B, BP.BallotsIssued AS C, BP.BallotsCasted AS D, BP.DifferenceIssuedCasted AS E, 
                                                   BP.BallotsSpoiled AS F, BP.BallotsValidVotes AS H, BP.BallotsReceived AS I, BP.BallotsUnusedSpoiled AS J,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 1)) AS G1,
                                                       (SELECT        ER.BallotCount
                                                         FROM            dbo.ElectionResult AS ER INNER JOIN
                                                                                   dbo.ElectionCompetitorMember AS ECM ON ECM.ElectionCompetitorMemberId = ER.ElectionCompetitorMemberId
                                                         WHERE        (ER.BallotPaperId = BP.BallotPaperId) AND (ER.BallotOrder = 2)) AS G2, NULL AS G3, NULL AS G4, NULL AS G5, NULL AS G6, NULL 
                                                   AS G7, NULL AS G8, NULL AS G9, NULL AS G10, NULL AS G11, NULL AS G12, NULL AS G13, NULL AS G14, NULL AS G15
                          FROM            dbo.AssignedCircumscription AS AC INNER JOIN
                                                   dbo.AssignedPollingStation AS APS ON APS.AssignedCircumscriptionId = AC.AssignedCircumscriptionId INNER JOIN
                                                   dbo.BallotPaper AS BP ON BP.ElectionRoundId = AC.ElectionRoundId AND BP.PollingStationId = APS.PollingStationId INNER JOIN
                                                   dbo.PollingStation AS PS ON PS.PollingStationId = APS.PollingStationId INNER JOIN
                                                   dbo.AssignedPollingStation AS APS2 ON APS2.ElectionRoundId = 10044 AND APS2.PollingStationId = APS.PollingStationId INNER JOIN
                                                   dbo.AssignedCircumscription AS AC2 ON AC2.AssignedCircumscriptionId = APS2.AssignedCircumscriptionId
                          WHERE        (AC.ElectionRoundId = 10046)) AS A
ORDER BY CAST(CircNr AS INTEGER), CAST(SectieNr AS INTEGER), ScrutinIndex

GO
/****** Object:  View [dbo].[VoterInCantemir]    Script Date: 3/14/2019 3:19:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[VoterInCantemir]
AS
SELECT        TOP (1000) VoterId, NameRo, LastNameRo, PatronymicRo, LastNameRu, NameRu, PatronymicRu, DateOfBirth, PlaceOfBirth, PlaceOfResidence, Gender, 
                         DateOfRegistration, Idnp, DocumentNumber, DateOfIssue, DateOfExpiry, Status, BatchId, StreetId, RegionId, StreetName, StreetNumber, StreetSubNumber, 
                         BlockNumber, BlockSubNumber, EditUserId, EditDate, Version, ElectionListNr
FROM            dbo.Voter
WHERE        (VoterId IN ('25252', '28565', '47632', '52811', '53693', '54170', '73414', '73807', '82760', '82720', '86178', '93496', '127610', '163345', '229416', '230862', '256204', 
                         '257556', '268206', '287050', '305134', '311521', '311088', '320738', '324531', '324542', '325297', '326013', '335667', '343479', '347359', '347986', '357547', 
                         '368887', '369779', '380776', '391308', '395680', '406774', '413833', '423873', '424379', '441096', '447045', '447942', '448704', '451254', '452675', '454438', 
                         '457682', '464306', '464508', '468213', '468579', '469337', '479682', '486118', '507916', '510849', '521257', '533023', '533079', '541512', '562715', '564094', 
                         '566603', '567644', '567300', '571030', '571072', '572544', '572981', '572715', '573558', '573509', '577894', '583084', '583347', '584087', '588017', '590365', 
                         '599661', '600809', '605554', '606909', '684255', '684440', '684213', '684663', '684968', '685943', '686010', '686730', '686966', '692413', '692331', '693508', 
                         '693941', '741285', '741074', '741026', '742012', '743657', '743900', '743368', '743550', '743587', '744786', '744741', '745230', '745888', '745312', '746360', 
                         '746473', '747017', '747868', '747708', '748835', '748795', '749136', '749560', '749780', '785000', '834718', '834753', '859866', '862724', '863271', '863087', 
                         '863119', '864427', '864455', '865332', '865339', '885015', '902358', '902930', '903232', '903146', '905208', '905010', '906148', '906393', '907239', '908882', 
                         '980183', '981811', '981165', '981917', '982324', '984426', '984490', '984603', '985789', '985798', '985090', '985366', '986175', '986841', '1015652', '1015020', 
                         '1015470', '1016459', '1018189', '1018084', '1018314', '1020780', '1021615', '1021585', '1021571', '1021573', '1080748', '1110153', '1111764', '1116154', 
                         '1116989', '1116956', '1116667', '1116811', '1117189', '1117271', '1117700', '1117046', '1117384', '1117622', '1117514', '1117398', '1117895', '1118174', 
                         '1118890', '1119777', '1119531', '1119356', '1119805', '1119966', '1120497', '1120284', '1121377', '1147458', '1166558', '1166248', '1167613', '1167205', 
                         '1167710', '1167067', '1168699', '1168747', '1168556', '1169443', '1169901', '1170514', '1171356', '1172308', '1232355', '1255518', '1255614', '1255708', 
                         '1256649', '1257331', '1257610', '1258134', '1259038', '1260843', '1262132', '1263350', '1263817', '1263545', '1264812', '1264104', '1265620', '1296311', 
                         '1308245', '1308317', '1308271', '1308001', '1309212', '1309468', '1309445', '1309338', '1310852', '1310377', '1310999', '1311033', '1312418', '1314789', 
                         '1348135', '1361861', '1362679', '1362256', '1362965', '1363634', '1363991', '1363340', '1363839', '1364039', '1365415', '1365294', '1365915', '1366957', 
                         '1375241', '1379302', '1387033', '1387781', '1388043', '1389557', '1389857', '1389682', '1390378', '1390692', '1418460', '1419250', '1419519', '1432768', 
                         '1432992', '1433572', '1435123', '1435828', '1435429', '1437695', '1452379', '1452111', '1453335', '1453604', '1463576', '1463299', '1463241', '1463242', 
                         '1463820', '1472894', '1472444', '1478875', '1486573', '1488744', '1490130', '1490720', '1497576', '1499768', '1502150', '1507835', '1507076', '1676122', 
                         '1712064', '1790118', '1926180', '1935488', '1968655', '2025842', '2025652', '2095060', '2144499', '2144256', '2144176', '2274961', '2286989', '2291838', 
                         '2386932', '2459382', '2469587', '2528626', '2538169', '2656225', '2665181', '2678151', '2692220', '2757717', '2812039', '2893112', '2894581', '2894680', 
                         '2895271', '2895808', '2895405', '2895533', '2896498', '2896508', '2896056', '2897009', '3127365', '3156433', '3165807', '3182026', '3183007', '3240655', 
                         '3246979', '3251101', '3257163', '3258500', '3287942', '3303625', '3305944', '3316945', '3319791', '3328145', '3329063', '3334181', '3342466', '3346192', 
                         '3353536', '3358836', '3372095', '3382093', '3383438', '3384268', '3388926', '3391427', '3392996', '3396479', '3401397', '3415698', '3416865', '3418433', 
                         '3432222', '3435797', '3454511', '3455633', '3343381', '862732', '3468412', '1523644', '3187155', '3355107', '1115310', '1261965', '3167662', '984791', '3167608', 
                         '3068645', '3169199', '52897', '303531', '1329984', '3073835', '160677', '531499', '2143418', '3080012', '431186', '3169734', '465068', '1364664', '3397261', 
                         '1365538', '3178830', '3378679', '1365886', '1361426', '980291', '861416', '599598', '1453964', '3180617', '3328512', '1433304', '1115740', '1364517'))

GO

/****** Object:  Index [IX_AgeCategories_From]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AgeCategories_From] ON [dbo].[AgeCategories]
(
	[From] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AgeCategories_To]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AgeCategories_To] ON [dbo].[AgeCategories]
(
	[To] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedCircumscription_ElectionRound]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedCircumscription_ElectionRound] ON [dbo].[AssignedCircumscription]
(
	[ElectionRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedCircumscription_Region]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedCircumscription_Region] ON [dbo].[AssignedCircumscription]
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedPollingStationCircumscription]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationCircumscription] ON [dbo].[AssignedPollingStation]
(
	[AssignedCircumscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedPollingStationElectionRound]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationElectionRound] ON [dbo].[AssignedPollingStation]
(
	[ElectionRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedPollingStationParentRegion]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationParentRegion] ON [dbo].[AssignedPollingStation]
(
	[ParentRegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedPollingStationPollingStation]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationPollingStation] ON [dbo].[AssignedPollingStation]
(
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedPollingStationRegion]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedPollingStationRegion] ON [dbo].[AssignedPollingStation]
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoter_VoterId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoter_VoterId] ON [dbo].[AssignedVoter]
(
	[VoterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterPollingStation]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterPollingStation] ON [dbo].[AssignedVoter]
(
	[PollingStationId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatistics_AgeCategory]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_AgeCategory] ON [dbo].[AssignedVoterStatistics]
(
	[AgeCategoryId] ASC,
	[RegionId] ASC,
	[ParentRegionId] ASC,
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatistics_CreationDate]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_CreationDate] ON [dbo].[AssignedVoterStatistics]
(
	[CreationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatistics_Gender]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_Gender] ON [dbo].[AssignedVoterStatistics]
(
	[Gender] ASC,
	[RegionId] ASC,
	[ParentRegionId] ASC,
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatistics_ParentRegionId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_ParentRegionId] ON [dbo].[AssignedVoterStatistics]
(
	[ParentRegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatistics_PollingStation]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_PollingStation] ON [dbo].[AssignedVoterStatistics]
(
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatistics_RegionId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_RegionId] ON [dbo].[AssignedVoterStatistics]
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatistics_Status]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatistics_Status] ON [dbo].[AssignedVoterStatistics]
(
	[AssignedVoterStatus] ASC,
	[PollingStationId] ASC,
	[RegionId] ASC,
	[ParentRegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatisticsPSAgeCategory]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsPSAgeCategory] ON [dbo].[AssignedVoterStatistics]
(
	[AgeCategoryId] ASC,
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatisticsPSGender]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsPSGender] ON [dbo].[AssignedVoterStatistics]
(
	[Gender] ASC,
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatisticsRegAgeCategory]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsRegAgeCategory] ON [dbo].[AssignedVoterStatistics]
(
	[AgeCategoryId] ASC,
	[RegionId] ASC,
	[ParentRegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_AssignedVoterStatisticsRegGender]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AssignedVoterStatisticsRegGender] ON [dbo].[AssignedVoterStatistics]
(
	[Gender] ASC,
	[RegionId] ASC,
	[ParentRegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AuditEventTypes_Code]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_AuditEventTypes_Code] ON [dbo].[AuditEventTypes]
(
	[code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_BallotPaper_PollingStationId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_BallotPaper_PollingStationId] ON [dbo].[BallotPaper]
(
	[PollingStationId] ASC,
	[ElectionRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ElectionCompetitor_CircumscriptionId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_ElectionCompetitor_CircumscriptionId] ON [dbo].[ElectionCompetitor]
(
	[AssignedCircumscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ElectionCompetitorMember_CircumscriptionId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_ElectionCompetitorMember_CircumscriptionId] ON [dbo].[ElectionCompetitorMember]
(
	[AssignedCircumscriptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ElectionCompetitorMember_CompetitorId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_ElectionCompetitorMember_CompetitorId] ON [dbo].[ElectionCompetitorMember]
(
	[ElectionCompetitorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ElectionResult_BallotPaperId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_ElectionResult_BallotPaperId] ON [dbo].[ElectionResult]
(
	[BallotPaperId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_PollingStationRegion]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_PollingStationRegion] ON [dbo].[PollingStation]
(
	[RegionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Region_RegionTypeId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_Region_RegionTypeId] ON [dbo].[Region]
(
	[RegionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_RegionParent]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_RegionParent] ON [dbo].[Region]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Voter]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_Voter] ON [dbo].[Voter]
(
	[DateOfBirth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Voter_idnp]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_Voter_idnp] ON [dbo].[Voter]
(
	[Idnp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_VoterCertificat_AssignedVoterId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_VoterCertificat_AssignedVoterId] ON [dbo].[VoterCertificat]
(
	[AssignedVoterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_VoterCertificat_PollingStationId]    Script Date: 07.08.2019 14:27:06 ******/
CREATE NONCLUSTERED INDEX [IX_VoterCertificat_PollingStationId] ON [dbo].[VoterCertificat]
(
	[PollingStationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssignedCircumscription] ADD  CONSTRAINT [DF_AssignedCircumscription_isFromUtan]  DEFAULT ((0)) FOR [isFromUtan]
GO
ALTER TABLE [dbo].[AssignedCircumscription] ADD  CONSTRAINT [DF_AssignedCircumscription_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[AssignedCircumscription] ADD  CONSTRAINT [DF_AssignedCircumscription_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[AssignedCircumscription] ADD  CONSTRAINT [DF_AssignedCircumscription_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[AssignedPermission] ADD  CONSTRAINT [DF_AssignedPermission_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[AssignedPermission] ADD  CONSTRAINT [DF_AssignedPermission_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[AssignedPermission] ADD  CONSTRAINT [DF_AssignedPermissions_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_Type]  DEFAULT ((1)) FOR [Type]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_IsOpen]  DEFAULT ((0)) FOR [IsOpen]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_OpeningVoters]  DEFAULT ((0)) FOR [OpeningVoters]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_EstimatedNumberOfVoters]  DEFAULT ((0)) FOR [EstimatedNumberOfVoters]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_NumberOfRoBallotPapers]  DEFAULT ((0)) FOR [NumberOfRoBallotPapers]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_NumberOfRuBallotPapers]  DEFAULT ((0)) FOR [NumberOfRuBallotPapers]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_ImplementsEVR]  DEFAULT ((0)) FOR [ImplementsEVR]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF_AssignedPollingStation_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF__AssignedP__isOpe__7E37BEF6]  DEFAULT ((0)) FOR [isOpeningEnabled]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF__AssignedP__isTur__7F2BE32F]  DEFAULT ((0)) FOR [isTurnoutEnabled]
GO
ALTER TABLE [dbo].[AssignedPollingStation] ADD  CONSTRAINT [DF__AssignedP__isEle__00200768]  DEFAULT ((0)) FOR [isElectionResultEnabled]
GO
ALTER TABLE [dbo].[AssignedRole] ADD  CONSTRAINT [DF_AssignedRole_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[AssignedRole] ADD  CONSTRAINT [DF_AssignedRole_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[AssignedRole] ADD  CONSTRAINT [DF_AssignedRoles_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_RegionId]  DEFAULT ((-1)) FOR [RegionId]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_RequestingPollingStationId]  DEFAULT ((-1)) FOR [RequestingPollingStationId]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_PollingStationId]  DEFAULT ((-1)) FOR [PollingStationId]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_VoterId]  DEFAULT ((-1)) FOR [VoterId]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_Type]  DEFAULT ((1)) FOR [Category]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[AssignedVoter] ADD  CONSTRAINT [DF_AssignedVoter_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[AssignedVoterStatistics] ADD  CONSTRAINT [DF_AssignedVoterStatistics_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_EntryLevel]  DEFAULT ((-1)) FOR [EntryLevel]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_Type_1]  DEFAULT ((1)) FOR [Type]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_RegisteredVoters]  DEFAULT ((0)) FOR [RegisteredVoters]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_Supplementary]  DEFAULT ((0)) FOR [Supplementary]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_BallotsIssued]  DEFAULT ((0)) FOR [BallotsIssued]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_BallotsCasted]  DEFAULT ((0)) FOR [BallotsCasted]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_DifferenceIssuedCasted]  DEFAULT ((0)) FOR [DifferenceIssuedCasted]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_BallotsValidVotes]  DEFAULT ((0)) FOR [BallotsValidVotes]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_BallotsReceived1]  DEFAULT ((0)) FOR [BallotsReceived]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_BallotsUnusedSpoiled]  DEFAULT ((0)) FOR [BallotsUnusedSpoiled]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_BallotsSpoiled]  DEFAULT ((0)) FOR [BallotsSpoiled]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_BallotsUnused]  DEFAULT ((0)) FOR [BallotsUnused]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_Description]  DEFAULT ('No Description') FOR [Description]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_Comments]  DEFAULT ('No Comments') FOR [Comments]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_DateOfEntry]  DEFAULT (sysdatetime()) FOR [DateOfEntry]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_IsResultsConfirmed]  DEFAULT ((0)) FOR [IsResultsConfirmed]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_BallotPaper_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[BallotPaper] ADD  CONSTRAINT [DF_Constituency_Version_1]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_Type]  DEFAULT ((0)) FOR [Type]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_DateOfElection]  DEFAULT (sysdatetime()) FOR [DateOfElection]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_Comments]  DEFAULT ('TBD') FOR [Comments]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[Election] ADD  CONSTRAINT [DF_Election_ReportsPath]  DEFAULT ('') FOR [ReportsPath]
GO
ALTER TABLE [dbo].[ElectionCompetitor] ADD  CONSTRAINT [DF_ElectionCompetitor_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[ElectionCompetitor] ADD  CONSTRAINT [DF_ElectionCompetitor_IsIndependent]  DEFAULT ((0)) FOR [IsIndependent]
GO
ALTER TABLE [dbo].[ElectionCompetitor] ADD  CONSTRAINT [DF_ElectionCompetitor_BallotOrder]  DEFAULT ((0)) FOR [BallotOrder]
GO
ALTER TABLE [dbo].[ElectionCompetitor] ADD  CONSTRAINT [DF_ElectionCompetitor_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[ElectionCompetitor] ADD  CONSTRAINT [DF_ElectionCompetitor_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[ElectionCompetitor] ADD  CONSTRAINT [DF_ElectionCompetitor_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[ElectionCompetitor] ADD  CONSTRAINT [DF_Political_Party_PartyType]  DEFAULT ((0)) FOR [PartyType]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] ADD  CONSTRAINT [DF_ElectionCompetitorMember_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] ADD  CONSTRAINT [DF_ElectionCompetitorMember_CompetitorMemberOrder]  DEFAULT ((1)) FOR [CompetitorMemberOrder]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] ADD  CONSTRAINT [DF_ElectionCompetitorMember_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] ADD  CONSTRAINT [DF_ElectionCompetitorMember_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] ADD  CONSTRAINT [DF_ElectionCompetitorMember_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_BallotOrder]  DEFAULT ((-1)) FOR [BallotOrder]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_BallotCount]  DEFAULT ((0)) FOR [BallotCount]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_Comments]  DEFAULT ('No Comment') FOR [Comments]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_ElectionCompetitorMemberId]  DEFAULT ((-1)) FOR [ElectionCompetitorMemberId]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[ElectionResult] ADD  CONSTRAINT [DF_ElectionResult_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[ElectionRound] ADD  CONSTRAINT [DF_ElectionRound_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[ElectionRound] ADD  CONSTRAINT [DF_ElectionRound_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[ElectionRound] ADD  CONSTRAINT [DF_ElectionRound_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [DF_Permission_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [DF_Permission_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[Permission] ADD  CONSTRAINT [DF_Permission_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride] ADD  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride] ADD  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[PollingStation] ADD  CONSTRAINT [DF_PollingStation_Type]  DEFAULT ((1)) FOR [Type]
GO
ALTER TABLE [dbo].[PollingStation] ADD  CONSTRAINT [DF_PollingStation_Number]  DEFAULT ((0)) FOR [Number]
GO
ALTER TABLE [dbo].[PollingStation] ADD  CONSTRAINT [DF_PollingStation_RegionId]  DEFAULT ((0)) FOR [RegionId]
GO
ALTER TABLE [dbo].[PollingStation] ADD  CONSTRAINT [DF_PollingStation_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[PollingStation] ADD  CONSTRAINT [DF_PollingStation_EditDate_2]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[PollingStation] ADD  CONSTRAINT [DF_PollingStation_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[PollingStation] ADD  CONSTRAINT [DF_PollingStation_ExcludeInLocalElection]  DEFAULT ((0)) FOR [ExcludeInLocalElections]
GO
ALTER TABLE [dbo].[Region] ADD  CONSTRAINT [DF_Region_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[Region] ADD  CONSTRAINT [DF_Region_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[Region] ADD  CONSTRAINT [DF_Region_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[RegionType] ADD  CONSTRAINT [DF_RegionType_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[RegionType] ADD  CONSTRAINT [DF_RegionType_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[RegionType] ADD  CONSTRAINT [DF_RegionType_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_Level]  DEFAULT ((1)) FOR [Level]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_Level]  DEFAULT ((99999)) FOR [Level]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_IsApproved]  DEFAULT ((0)) FOR [IsApproved]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_IsOnLine]  DEFAULT ((0)) FOR [IsOnLine]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_IsLockedOut]  DEFAULT ((0)) FOR [IsLockedOut]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_FailedAttemptCount]  DEFAULT ((0)) FOR [FailedAttemptCount]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_FailedAnswerCount]  DEFAULT ((0)) FOR [FailedAnswerCount]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_ElectionId]  DEFAULT ((-2)) FOR [ElectionId]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_RegionId]  DEFAULT ((-2)) FOR [RegionId]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_PollingStationId]  DEFAULT ((-2)) FOR [PollingStationId]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_CircumscriptionId]  DEFAULT ((-2)) FOR [CircumscriptionId]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [dbo].[SystemUser] ADD  CONSTRAINT [DF_SystemUser_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Voter] ADD  CONSTRAINT [DF_Voter_Status]  DEFAULT ((0)) FOR [Status]
GO
ALTER TABLE [dbo].[Voter] ADD  CONSTRAINT [DF_Voter_RegionId]  DEFAULT ((-1)) FOR [RegionId]
GO
ALTER TABLE [dbo].[Voter] ADD  CONSTRAINT [DF_Voter_EditUserId]  DEFAULT ((1)) FOR [EditUserId]
GO
ALTER TABLE [dbo].[Voter] ADD  CONSTRAINT [DF_Voter_EditDate]  DEFAULT (sysdatetime()) FOR [EditDate]
GO
ALTER TABLE [dbo].[Voter] ADD  CONSTRAINT [DF_Person_Version]  DEFAULT ((1)) FOR [Version]
GO
ALTER TABLE [schematmp].[AssignedCircumscription] ADD  CONSTRAINT [DF_AssignedCircumscription_isFromUtan_1]  DEFAULT ((0)) FOR [isFromUtan]
GO
ALTER TABLE [Audit].[BallotPaper_AUD]  WITH CHECK ADD  CONSTRAINT [FK_BallotPaper_AUD_REVINFO] FOREIGN KEY([REV])
REFERENCES [Audit].[REVINFO] ([REV])
GO
ALTER TABLE [Audit].[BallotPaper_AUD] CHECK CONSTRAINT [FK_BallotPaper_AUD_REVINFO]
GO
ALTER TABLE [Audit].[ElectionResult_AUD]  WITH CHECK ADD  CONSTRAINT [FK_ElectionResult_AUD_REVINFO] FOREIGN KEY([REV])
REFERENCES [Audit].[REVINFO] ([REV])
GO
ALTER TABLE [Audit].[ElectionResult_AUD] CHECK CONSTRAINT [FK_ElectionResult_AUD_REVINFO]
GO
ALTER TABLE [Audit].[VoterCertificat_AUD]  WITH CHECK ADD  CONSTRAINT [FK6271F260AAE62361] FOREIGN KEY([REV])
REFERENCES [Audit].[REVINFO] ([REV])
GO
ALTER TABLE [Audit].[VoterCertificat_AUD] CHECK CONSTRAINT [FK6271F260AAE62361]
GO
ALTER TABLE [dbo].[Alerts]  WITH CHECK ADD  CONSTRAINT [FK_Alerts_PollingStation] FOREIGN KEY([PollingStationId])
REFERENCES [dbo].[PollingStation] ([PollingStationId])
GO
ALTER TABLE [dbo].[Alerts] CHECK CONSTRAINT [FK_Alerts_PollingStation]
GO
ALTER TABLE [dbo].[Alerts]  WITH CHECK ADD  CONSTRAINT [FK_Alerts_Voter] FOREIGN KEY([VoterId])
REFERENCES [dbo].[Voter] ([VoterId])
GO
ALTER TABLE [dbo].[Alerts] CHECK CONSTRAINT [FK_Alerts_Voter]
GO
ALTER TABLE [dbo].[AssignedCircumscription]  WITH CHECK ADD  CONSTRAINT [FK_AssignedCircumscription_ElectionRound_ElectionRoundId] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[AssignedCircumscription] CHECK CONSTRAINT [FK_AssignedCircumscription_ElectionRound_ElectionRoundId]
GO
ALTER TABLE [dbo].[AssignedCircumscription]  WITH CHECK ADD  CONSTRAINT [FK_AssignedCircumscription_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[AssignedCircumscription] CHECK CONSTRAINT [FK_AssignedCircumscription_Region]
GO
ALTER TABLE [dbo].[AssignedPermission]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPermissions_Permission] FOREIGN KEY([PermissionId])
REFERENCES [dbo].[Permission] ([PermissionId])
GO
ALTER TABLE [dbo].[AssignedPermission] CHECK CONSTRAINT [FK_AssignedPermissions_Permission]
GO
ALTER TABLE [dbo].[AssignedPermission]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPermissions_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[AssignedPermission] CHECK CONSTRAINT [FK_AssignedPermissions_Role]
GO
ALTER TABLE [dbo].[AssignedPollingStation]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPollingStation_AssignedCircumscription] FOREIGN KEY([AssignedCircumscriptionId])
REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId])
GO
ALTER TABLE [dbo].[AssignedPollingStation] CHECK CONSTRAINT [FK_AssignedPollingStation_AssignedCircumscription]
GO
ALTER TABLE [dbo].[AssignedPollingStation]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPollingStation_ElectionRound] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[AssignedPollingStation] CHECK CONSTRAINT [FK_AssignedPollingStation_ElectionRound]
GO
ALTER TABLE [dbo].[AssignedPollingStation]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPollingStation_PollingStation] FOREIGN KEY([PollingStationId])
REFERENCES [dbo].[PollingStation] ([PollingStationId])
GO
ALTER TABLE [dbo].[AssignedPollingStation] CHECK CONSTRAINT [FK_AssignedPollingStation_PollingStation]
GO
ALTER TABLE [dbo].[AssignedPollingStation]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPollingStation_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[AssignedPollingStation] CHECK CONSTRAINT [FK_AssignedPollingStation_Region]
GO
ALTER TABLE [dbo].[AssignedPollingStation]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPollingStation_RegionParent] FOREIGN KEY([ParentRegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[AssignedPollingStation] CHECK CONSTRAINT [FK_AssignedPollingStation_RegionParent]
GO
--ALTER TABLE [dbo].[AssignedPollingStation] CHECK CONSTRAINT [FK_AssignedPollingStation_ElectionDuration]
--GO
ALTER TABLE [dbo].[AssignedPollingStation]  WITH CHECK ADD  CONSTRAINT [FK_AssignedPollingStation_ElectionDuration] FOREIGN KEY([ElectionDurationId])
REFERENCES [dbo].[ElectionDuration] ([ElectionDurationId])
GO
ALTER TABLE [dbo].[AssignedRole]  WITH CHECK ADD  CONSTRAINT [FK_AssignedRoles_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[AssignedRole] CHECK CONSTRAINT [FK_AssignedRoles_Role]
GO
ALTER TABLE [dbo].[AssignedRole]  WITH CHECK ADD  CONSTRAINT [FK_AssignedRoles_SystemUser1] FOREIGN KEY([SystemUserId])
REFERENCES [dbo].[SystemUser] ([SystemUserId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[AssignedRole] CHECK CONSTRAINT [FK_AssignedRoles_SystemUser1]
GO
ALTER TABLE [dbo].[AssignedVoter]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoter_PollingStation] FOREIGN KEY([PollingStationId])
REFERENCES [dbo].[PollingStation] ([PollingStationId])
GO
ALTER TABLE [dbo].[AssignedVoter] CHECK CONSTRAINT [FK_AssignedVoter_PollingStation]
GO
ALTER TABLE [dbo].[AssignedVoter]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoter_PollingStation1] FOREIGN KEY([RequestingPollingStationId])
REFERENCES [dbo].[PollingStation] ([PollingStationId])
GO
ALTER TABLE [dbo].[AssignedVoter] CHECK CONSTRAINT [FK_AssignedVoter_PollingStation1]
GO
ALTER TABLE [dbo].[AssignedVoter]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoter_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[AssignedVoter] CHECK CONSTRAINT [FK_AssignedVoter_Region]
GO
ALTER TABLE [dbo].[AssignedVoter]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoter_Voter] FOREIGN KEY([VoterId])
REFERENCES [dbo].[Voter] ([VoterId])
GO
ALTER TABLE [dbo].[AssignedVoter] CHECK CONSTRAINT [FK_AssignedVoter_Voter]
GO
ALTER TABLE [dbo].[AssignedVoterStatistics]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoterStatistics_AgeCategories] FOREIGN KEY([AgeCategoryId])
REFERENCES [dbo].[AgeCategories] ([AgeCategoryId])
GO
ALTER TABLE [dbo].[AssignedVoterStatistics] CHECK CONSTRAINT [FK_AssignedVoterStatistics_AgeCategories]
GO
ALTER TABLE [dbo].[AssignedVoterStatistics]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoterStatistics_AssignedVoter] FOREIGN KEY([AssignedVoterId])
REFERENCES [dbo].[AssignedVoter] ([AssignedVoterId])
GO
ALTER TABLE [dbo].[AssignedVoterStatistics] CHECK CONSTRAINT [FK_AssignedVoterStatistics_AssignedVoter]
GO
ALTER TABLE [dbo].[AssignedVoterStatistics]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoterStatistics_PollingStation] FOREIGN KEY([PollingStationId])
REFERENCES [dbo].[PollingStation] ([PollingStationId])
GO
ALTER TABLE [dbo].[AssignedVoterStatistics] CHECK CONSTRAINT [FK_AssignedVoterStatistics_PollingStation]
GO
ALTER TABLE [dbo].[AssignedVoterStatistics]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoterStatistics_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[AssignedVoterStatistics] CHECK CONSTRAINT [FK_AssignedVoterStatistics_Region]
GO
ALTER TABLE [dbo].[AssignedVoterStatistics]  WITH CHECK ADD  CONSTRAINT [FK_AssignedVoterStatistics_RegionParent] FOREIGN KEY([ParentRegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[AssignedVoterStatistics] CHECK CONSTRAINT [FK_AssignedVoterStatistics_RegionParent]
GO
ALTER TABLE [dbo].[AuditEvents]  WITH CHECK ADD  CONSTRAINT [FK_AuditEvents_AuditEventTypes] FOREIGN KEY([auditEventTypeId])
REFERENCES [dbo].[AuditEventTypes] ([auditEventTypeId])
GO
ALTER TABLE [dbo].[AuditEvents] CHECK CONSTRAINT [FK_AuditEvents_AuditEventTypes]
GO
ALTER TABLE [dbo].[BallotPaper]  WITH CHECK ADD  CONSTRAINT [FK_BallotPaper_ElectionRound] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[BallotPaper] CHECK CONSTRAINT [FK_BallotPaper_ElectionRound]
GO
ALTER TABLE [dbo].[BallotPaper]  WITH CHECK ADD  CONSTRAINT [FK_BallotPaper_PollingStation] FOREIGN KEY([PollingStationId])
REFERENCES [dbo].[PollingStation] ([PollingStationId])
GO
ALTER TABLE [dbo].[BallotPaper] CHECK CONSTRAINT [FK_BallotPaper_PollingStation]
GO
ALTER TABLE [dbo].[CircumscriptionRegion]  WITH CHECK ADD  CONSTRAINT [FK_CircumscriptionRegion_AssignedCircumscription] FOREIGN KEY([AssignedCircumscriptionId])
REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId])
GO
ALTER TABLE [dbo].[CircumscriptionRegion] CHECK CONSTRAINT [FK_CircumscriptionRegion_AssignedCircumscription]
GO
ALTER TABLE [dbo].[CircumscriptionRegion]  WITH CHECK ADD  CONSTRAINT [FK_CircumscriptionRegion_ElectionRoundId] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[CircumscriptionRegion] CHECK CONSTRAINT [FK_CircumscriptionRegion_ElectionRoundId]
GO
ALTER TABLE [dbo].[CircumscriptionRegion]  WITH CHECK ADD  CONSTRAINT [FK_CircumscriptionRegion_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[CircumscriptionRegion] CHECK CONSTRAINT [FK_CircumscriptionRegion_Region]
GO
ALTER TABLE [dbo].[Election]  WITH CHECK ADD  CONSTRAINT [FK_Election_ElectionType] FOREIGN KEY([Type])
REFERENCES [dbo].[ElectionType] ([ElectionTypeId])
GO
ALTER TABLE [dbo].[Election] CHECK CONSTRAINT [FK_Election_ElectionType]
GO
ALTER TABLE [dbo].[ElectionCompetitor]  WITH CHECK ADD  CONSTRAINT [FK_ElectionCompetitor_AssignedCircumscription_AssignedCircumscriptionId] FOREIGN KEY([AssignedCircumscriptionId])
REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId])
GO
ALTER TABLE [dbo].[ElectionCompetitor] CHECK CONSTRAINT [FK_ElectionCompetitor_AssignedCircumscription_AssignedCircumscriptionId]
GO
ALTER TABLE [dbo].[ElectionCompetitor]  WITH CHECK ADD  CONSTRAINT [FK_ElectionCompetitor_ElectionRound_ElectionRoundId] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[ElectionCompetitor] CHECK CONSTRAINT [FK_ElectionCompetitor_ElectionRound_ElectionRoundId]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember]  WITH CHECK ADD  CONSTRAINT [FK_ElectionCompetitorMember_AssignedCircumscription] FOREIGN KEY([AssignedCircumscriptionId])
REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId])
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] CHECK CONSTRAINT [FK_ElectionCompetitorMember_AssignedCircumscription]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember]  WITH CHECK ADD  CONSTRAINT [FK_ElectionCompetitorMember_ElectionCompetitor] FOREIGN KEY([ElectionCompetitorId])
REFERENCES [dbo].[ElectionCompetitor] ([ElectionCompetitorId])
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] CHECK CONSTRAINT [FK_ElectionCompetitorMember_ElectionCompetitor]
GO
ALTER TABLE [dbo].[ElectionCompetitorMember]  WITH CHECK ADD  CONSTRAINT [FK_ElectionCompetitorMember_ElectionRound] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[ElectionCompetitorMember] CHECK CONSTRAINT [FK_ElectionCompetitorMember_ElectionRound]
GO
ALTER TABLE [dbo].[ElectionResult]  WITH CHECK ADD  CONSTRAINT [FK_ElectionResult_BallotPaper] FOREIGN KEY([BallotPaperId])
REFERENCES [dbo].[BallotPaper] ([BallotPaperId])
GO
ALTER TABLE [dbo].[ElectionResult] CHECK CONSTRAINT [FK_ElectionResult_BallotPaper]
GO
ALTER TABLE [dbo].[ElectionResult]  WITH CHECK ADD  CONSTRAINT [FK_ElectionResult_ElectionCompetitor] FOREIGN KEY([ElectionCompetitorId])
REFERENCES [dbo].[ElectionCompetitor] ([ElectionCompetitorId])
GO
ALTER TABLE [dbo].[ElectionResult] CHECK CONSTRAINT [FK_ElectionResult_ElectionCompetitor]
GO
ALTER TABLE [dbo].[ElectionResult]  WITH CHECK ADD  CONSTRAINT [FK_ElectionResult_ElectionCompetitorMember] FOREIGN KEY([ElectionCompetitorMemberId])
REFERENCES [dbo].[ElectionCompetitorMember] ([ElectionCompetitorMemberId])
GO
ALTER TABLE [dbo].[ElectionResult] CHECK CONSTRAINT [FK_ElectionResult_ElectionCompetitorMember]
GO
ALTER TABLE [dbo].[ElectionResult]  WITH CHECK ADD  CONSTRAINT [FK_ElectionResult_ElectionRound_ElectionRoundId] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[ElectionResult] CHECK CONSTRAINT [FK_ElectionResult_ElectionRound_ElectionRoundId]
GO
ALTER TABLE [dbo].[ElectionRound]  WITH CHECK ADD  CONSTRAINT [FK_ElectionRound_Election_ElectionId] FOREIGN KEY([ElectionId])
REFERENCES [dbo].[Election] ([ElectionId])
GO
ALTER TABLE [dbo].[ElectionRound] CHECK CONSTRAINT [FK_ElectionRound_Election_ElectionId]
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride]  WITH CHECK ADD  CONSTRAINT [FK_PoliticalPartyStatusOverride_AssignedCircumscription_AssignedCircumscriptionId] FOREIGN KEY([AssignedCircumscriptionId])
REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId])
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride] CHECK CONSTRAINT [FK_PoliticalPartyStatusOverride_AssignedCircumscription_AssignedCircumscriptionId]
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride]  WITH CHECK ADD  CONSTRAINT [FK_PoliticalPartyStatusOverride_ElectionCompetitor_StatusOverrides] FOREIGN KEY([ElectionCompetitorId])
REFERENCES [dbo].[ElectionCompetitor] ([ElectionCompetitorId])
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride] CHECK CONSTRAINT [FK_PoliticalPartyStatusOverride_ElectionCompetitor_StatusOverrides]
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride]  WITH CHECK ADD  CONSTRAINT [FK_PoliticalPartyStatusOverride_ElectionRound_ElectionRoundId] FOREIGN KEY([ElectionRoundId])
REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride] CHECK CONSTRAINT [FK_PoliticalPartyStatusOverride_ElectionRound_ElectionRoundId]
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride]  WITH CHECK ADD  CONSTRAINT [FK_PoliticalPartyStatusOverride_SystemUsers_editUserId] FOREIGN KEY([EditUserId])
REFERENCES [dbo].[SystemUser] ([SystemUserId])
GO
ALTER TABLE [dbo].[PoliticalPartyStatusOverride] CHECK CONSTRAINT [FK_PoliticalPartyStatusOverride_SystemUsers_editUserId]
GO
ALTER TABLE [dbo].[PollingStation]  WITH NOCHECK ADD  CONSTRAINT [FK_PollingStation_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[PollingStation] CHECK CONSTRAINT [FK_PollingStation_Region]
GO
ALTER TABLE [dbo].[Region]  WITH CHECK ADD  CONSTRAINT [FK_Region_Parent] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[Region] CHECK CONSTRAINT [FK_Region_Parent]
GO
ALTER TABLE [dbo].[Region]  WITH CHECK ADD  CONSTRAINT [FK_Region_RegionType_RegionTypeId] FOREIGN KEY([RegionTypeId])
REFERENCES [dbo].[RegionType] ([RegionTypeId])
GO
ALTER TABLE [dbo].[Region] CHECK CONSTRAINT [FK_Region_RegionType_RegionTypeId]
GO
--Document management section 07/09/2023
ALTER TABLE [dbo].[TemplateTypes] WITH CHECK ADD CONSTRAINT [FK_TemplateTypes_SystemUser] FOREIGN KEY ([EditUserId])
REFERENCES [dbo].[SystemUser]([SystemUserId])
GO
ALTER TABLE [dbo].[TemplateNames] WITH CHECK ADD CONSTRAINT [FK_TemplateNames_SystemUser] FOREIGN KEY ([EditUserId])
REFERENCES [dbo].[SystemUser]([SystemUserId])
GO
ALTER TABLE [dbo].[TemplateNames] WITH CHECK ADD CONSTRAINT [FK_TemplateNames_TemplateType] FOREIGN KEY ([TemplateTypeId])
REFERENCES [dbo].[TemplateTypes]([TemplateTypeId])
GO
ALTER TABLE [dbo].[Templates] WITH CHECK ADD CONSTRAINT [FK_Templates_SystemUser] FOREIGN KEY ([EditUserId])
REFERENCES [dbo].[SystemUser]([SystemUserId])
GO
ALTER TABLE [dbo].[Templates] WITH CHECK ADD CONSTRAINT [FK_Templates_TemplateName] FOREIGN KEY ([TemplateNameId]) 
REFERENCES [dbo].[TemplateNames]([TemplateNameId])
GO
ALTER TABLE [dbo].[Templates] WITH CHECK ADD CONSTRAINT [FK_Templates_ElectionType] FOREIGN KEY ([ElectionTypeId]) 
REFERENCES [dbo].[ElectionType]([ElectionTypeId])
GO
ALTER TABLE [dbo].[Templates] WITH CHECK ADD CONSTRAINT [FK_Templates_Templates] FOREIGN KEY ([ParentId]) 
REFERENCES [dbo].[Templates]([TemplateId])
GO
ALTER TABLE [dbo].[ReportParameters] WITH CHECK ADD CONSTRAINT [FK_ReportParameters_SystemUser] FOREIGN KEY ([EditUserId])
REFERENCES [dbo].[SystemUser]([SystemUserId])
GO
ALTER TABLE [dbo].[ReportParameters] WITH CHECK ADD CONSTRAINT [FK_ReportParameters_Template] FOREIGN KEY ([TemplateId]) 
REFERENCES [dbo].[Templates]([TemplateId])
GO
ALTER TABLE [dbo].[Documents] WITH CHECK ADD CONSTRAINT [FK_Documents_SystemUser] FOREIGN KEY ([EditUserId])
REFERENCES [dbo].[SystemUser]([SystemUserId])
GO
ALTER TABLE [dbo].[Documents] WITH CHECK ADD CONSTRAINT [FK_Documents_Template] FOREIGN KEY ([TemplateId]) 
REFERENCES [dbo].[Templates]([TemplateId])
GO
ALTER TABLE [dbo].[Documents] WITH CHECK ADD CONSTRAINT [FK_Documents_PollingStation] FOREIGN KEY ([PollingStationId]) 
REFERENCES [dbo].[PollingStation]([PollingStationId])
GO
ALTER TABLE [dbo].[Documents] WITH CHECK ADD CONSTRAINT [FK_Documents_ElectionRound] FOREIGN KEY ([ElectionRoundId]) 
REFERENCES [dbo].[ElectionRound]([ElectionRoundId])
GO
ALTER TABLE [dbo].[Documents] WITH CHECK ADD CONSTRAINT [FK_Documents_AssignedCircumscription] FOREIGN KEY ([AssignedCircumscriptionId]) 
REFERENCES [dbo].[AssignedCircumscription]([AssignedCircumscriptionId])
GO
--Excluded 09/07/2023
--ALTER TABLE [dbo].[ReportParamValues]  WITH CHECK ADD  CONSTRAINT [FK_ReportParamValues_ElectionType] FOREIGN KEY([ElectionTypeId])
--REFERENCES [dbo].[ElectionType] ([ElectionTypeId])
--GO
--ALTER TABLE [dbo].[ReportParamValues] CHECK CONSTRAINT [FK_ReportParamValues_ElectionType]
--GO
--ALTER TABLE [dbo].[ReportParamValues]  WITH CHECK ADD  CONSTRAINT [FK_ReportParamValues_ReportParams] FOREIGN KEY([ReportParamId])
--REFERENCES [dbo].[ReportParams] ([ReportParamId])
--GO
--ALTER TABLE [dbo].[ReportParamValues] CHECK CONSTRAINT [FK_ReportParamValues_ReportParams]
--GO
ALTER TABLE [dbo].[Voter]  WITH CHECK ADD  CONSTRAINT [FK_Voter_Region] FOREIGN KEY([RegionId])
REFERENCES [dbo].[Region] ([RegionId])
GO
ALTER TABLE [dbo].[Voter] CHECK CONSTRAINT [FK_Voter_Region]
GO
ALTER TABLE [dbo].[VoterCertificat]  WITH CHECK ADD  CONSTRAINT [FK_VoterCertificat_AssignedVoter] FOREIGN KEY([AssignedVoterId])
REFERENCES [dbo].[AssignedVoter] ([AssignedVoterId])
GO
ALTER TABLE [dbo].[VoterCertificat] CHECK CONSTRAINT [FK_VoterCertificat_AssignedVoter]
GO
ALTER TABLE [dbo].[VoterCertificat]  WITH CHECK ADD  CONSTRAINT [FK_VoterCertificat_PollingStation] FOREIGN KEY([PollingStationId])
REFERENCES [dbo].[PollingStation] ([PollingStationId])
GO
ALTER TABLE [dbo].[VoterCertificat] CHECK CONSTRAINT [FK_VoterCertificat_PollingStation]
GO
ALTER TABLE [dbo].[VoterCertificat]  WITH CHECK ADD  CONSTRAINT [FK_VoterCertificat_VoterCertificat] FOREIGN KEY([VoterCertificatId])
REFERENCES [dbo].[VoterCertificat] ([VoterCertificatId])
GO
ALTER TABLE [dbo].[VoterCertificat] CHECK CONSTRAINT [FK_VoterCertificat_VoterCertificat]
GO


/****** Object:  Trigger [dbo].[tr_AssignedVoter_Inserted]    Script Date: 07.08.2019 14:27:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_AssignedVoter_Inserted]
   ON [dbo].[AssignedVoter]
   AFTER INSERT
AS BEGIN
    SET NOCOUNT ON;   
	INSERT INTO AssignedVoterStatistics
        ([AssignedVoterId],[AssignedVoterStatus],[Gender],[AgeCategoryId],[PollingStationId],[RegionId],[ParentRegionId])
    SELECT
        I.AssignedVoterId, 
		I.Status,
		V.Gender,
		(SELECT TOP 1 [AgeCategoryId] FROM [dbo].[AgeCategories] WITH(NOLOCK) WHERE ((0 + Convert(Char(8),(SELECT TOP 1 ElectionDayDate FROM [dbo].[ElectionDay] WITH(NOLOCK)),112) - Convert(Char(8),V.[DateOfBirth],112))/10000) between [From] AND ISNULL([To],150)),
		I.PollingStationId,
		(SELECT TOP 1 ps.RegionId FROM [dbo].[PollingStation] as ps WITH(NOLOCK) WHERE ps.PollingStationId = I.PollingStationId),
		[dbo].[fn_GetParentRegion]((SELECT TOP 1 ps.RegionId FROM [dbo].[PollingStation] as ps WITH(NOLOCK) WHERE ps.PollingStationId = I.PollingStationId))
    FROM 
	Inserted I WITH (NOLOCK)
	INNER JOIN  [dbo].[Voter] as V WITH(NOLOCK) ON I.VoterId = V.VoterId
	WHERE
	I.Status >=5000 and I.Status <=5004
END

GO
ALTER TABLE [dbo].[AssignedVoter] ENABLE TRIGGER [tr_AssignedVoter_Inserted]
GO
/****** Object:  Trigger [dbo].[tr_AssignedVoter_Modified]    Script Date: 07.08.2019 14:27:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[tr_AssignedVoter_Modified]
   ON [dbo].[AssignedVoter]
   AFTER UPDATE
AS BEGIN
    SET NOCOUNT ON;   
	INSERT INTO AssignedVoterStatistics
        ([AssignedVoterId],[AssignedVoterStatus],[Gender],[AgeCategoryId],[PollingStationId],[RegionId],[ParentRegionId])
    SELECT
        I.AssignedVoterId, 
		I.Status,
		V.Gender,
		(SELECT TOP 1 [AgeCategoryId] FROM [dbo].[AgeCategories] WITH(NOLOCK) WHERE ((0 + Convert(Char(8),(SELECT TOP 1 ElectionDayDate FROM [dbo].[ElectionDay] WITH(NOLOCK)),112) - Convert(Char(8),V.[DateOfBirth],112))/10000) between [From] AND ISNULL([To],150)),
		I.PollingStationId,
		(SELECT TOP 1 ps.RegionId FROM [dbo].[PollingStation] as ps WITH(NOLOCK) WHERE ps.PollingStationId = I.PollingStationId),
		[dbo].[fn_GetParentRegion]((SELECT TOP 1 ps.RegionId FROM [dbo].[PollingStation] as ps WITH(NOLOCK) WHERE ps.PollingStationId = I.PollingStationId))
    FROM 
	Inserted I WITH (NOLOCK)
	INNER JOIN  [dbo].[Voter] as V WITH(NOLOCK) ON I.VoterId = V.VoterId
	WHERE
	I.Status >=5000 and I.Status <=5004
END

GO

ALTER TABLE [dbo].[AssignedVoter] ENABLE TRIGGER [tr_AssignedVoter_Modified]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Varianta specifica a denumirii pentru Buletin de Vot. Se accepta si HTML taguri.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ElectionCompetitor', @level2type=N'COLUMN',@level2name=N'BallotPaperNameRo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Varianta specifica a denumirii pentru Buletin de Vot. Se accepta si HTML taguri.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ElectionCompetitor', @level2type=N'COLUMN',@level2name=N'BallotPaperNameRu'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Clasa speciala de stiluri CSS atribuita denumiii partidului in Buletinul de Vot.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ElectionCompetitor', @level2type=N'COLUMN',@level2name=N'BallotPapperCustomCssRo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Clasa speciala de stiluri CSS atribuita denumiii partidului in Buletinul de Vot.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ElectionCompetitor', @level2type=N'COLUMN',@level2name=N'BallotPapperCustomCssRu'
GO

ALTER DATABASE [$(dbname)] SET  READ_WRITE 
GO
