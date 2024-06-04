CREATE PROCEDURE [dbo].[infElectionResult]
AS

DECLARE @NationalElectionRound BIGINT;
SET @NationalElectionRound = (SELECT TOP 1 AC.ElectionRoundId FROM [dbo].[AssignedCircumscription] as AC WITH(NOLOCK) WHERE AC.RegionId = 1);

SELECT 
a.[Type],
a.[KeyId],
a.[ElectionRoundId],
a.[ElectionCompetitorId],
a.[ElectionCompetitorMemberId],
a.[Color],
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
T1.AssignedCircumscriptionId as [Number],
CAST(ISNULL(T1.Number,0) AS BIGINT) as [KeyId],
T1.ElectionRoundId,
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
T4.Color,
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
WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK) WHERE er.ElectionId = e.ElectionId AND (e.Type = 21 OR e.Type = 27))
GROUP BY T1.AssignedCircumscriptionId, T1.Number, T1.ElectionRoundId, T3.ElectionCompetitorId, T4.Color, T3.ElectionCompetitorMemberId
UNION
SELECT 
2 as [Type],
T1.StatisticCode as [Number],
T1.RegionId as [KeyId],
@NationalElectionRound,
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
T4.Color,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[Region] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = @NationalElectionRound AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.ParentRegionId = T1.RegionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE
T1.RegionTypeId between 2 and 4
and T1.RegionId<> -1 
GROUP BY T1.RegionId, T1.StatisticCode,  T3.ElectionCompetitorId, T4.Color, T3.ElectionCompetitorMemberId
UNION
SELECT 
3 as [Type],
T1.StatisticCode as [Number],
T1.RegionId as [KeyId],
@NationalElectionRound as [ElectionRoundId],
T3.ElectionCompetitorId,
T3.ElectionCompetitorMemberId,
T4.Color,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2  THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[Region] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = @NationalElectionRound AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE
T1.RegionTypeId = 5 
and T1.ParentId = 2
GROUP BY T1.RegionId, T1.StatisticCode,  T3.ElectionCompetitorId, T4.Color, T3.ElectionCompetitorMemberId
union
SELECT
4,
100000 as [Number],
4000 as [KeyId],
@NationalElectionRound as [ElectionRoundId],
sub.[ElectionCompetitorId],
sub.[ElectionCompetitorMemberId],
sub.Color,
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
T4.Color,
SUM(CASE WHEN T2.Status = 0 OR T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalots,
SUM(CASE WHEN T2.Status = 1 OR T2.Status = 2 THEN 1 ELSE 0 END) TotalBalotsProcessed,
SUM(T2.BallotsValidVotes) as BallotsValidVotes,
SUM(T3.BallotCount) as BallotCount
FROM 
[dbo].[Region] T1 WITH(NOLOCK)
LEFT OUTER JOIN  [dbo].[BallotPaper] T2 WITH(NOLOCK) ON  T2.ElectionRoundId = @NationalElectionRound AND T2.PollingStationId IN (SELECT aps.PollingStationId FROM [dbo].[AssignedPollingStation] as aps WITH(NOLOCK) WHERE aps.RegionId = T1.RegionId)
LEFT OUTER JOIN  [dbo].[ElectionResult] T3 WITH(NOLOCK) ON T2.BallotPaperId = T3.BallotPaperId
LEFT OUTER JOIN  [dbo].[ElectionCompetitor] as T4 WITH(NOLOCK) ON T3.ElectionCompetitorId = T4.ElectionCompetitorId
LEFT OUTER JOIN  [dbo].[ElectionCompetitorMember] as T5 WITH(NOLOCK) ON T3.ElectionCompetitorMemberId = T5.ElectionCompetitorMemberId AND T4.ElectionCompetitorId = T3.ElectionCompetitorId
WHERE
T1.RegionTypeId <> 5 
and (T1.ParentId = 2 OR T1.RegionId IN (SELECT rt.RegionId FROM [dbo].[Region] rt WITH(NOLOCK) WHERE rt.ParentId IN (SELECT rtt.RegionId FROM [dbo].[Region] rtt WITH(NOLOCK) WHERE rtt.ParentId = 2) ) )
AND T3.ElectionCompetitorId IS NOT NULL
GROUP BY T1.RegionId, T3.ElectionCompetitorId, T4.Color, T3.ElectionCompetitorMemberId
) sub
GROUP BY sub.[ElectionCompetitorId], sub.[Color], sub.[ElectionCompetitorMemberId]
) a
ORDER BY [Type], [Number], [ElectionRoundId], [KeyId]
