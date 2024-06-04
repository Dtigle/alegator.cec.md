


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
