

CREATE PROCEDURE [dbo].[infVoterTurnout]
AS

DECLARE @NationalElectionRound BIGINT;
SET @NationalElectionRound = (SELECT TOP 1 AC.ElectionRoundId FROM [dbo].[AssignedCircumscription] as AC WITH(NOLOCK) WHERE AC.RegionId = 1);

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
(SELECT 
  1 as [Type],
  T1.AssignedCircumscriptionId as [Number],
  T1.ElectionRoundId,
  CAST(ISNULL(Number,0) AS BIGINT) as [KeyId],
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
WHERE T1.ElectionRoundId IN (SELECT er.ElectionRoundId FROM [dbo].[ElectionRound] er WITH(NOLOCK), [dbo].[Election] e WITH(NOLOCK)  WHERE er.ElectionId = e.ElectionId AND (e.Type = 21 OR e.Type = 27))
GROUP BY T1.AssignedCircumscriptionId, T1.Number, T1.NameRo, T1.ElectionRoundId
union
SELECT 
  2 as [Type],
  T1.StatisticCode as [Number],
  0 as [ElectionRoundId],
  T1.RegionId as [KeyId],
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
T1.RegionTypeId between 2 and 4
and T4.RegionTypeId = T1.RegionTypeId
and T1.RegionId<> -1 
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
ORDER BY [Type], [Number], [ElectionRoundId], [KeyId]
