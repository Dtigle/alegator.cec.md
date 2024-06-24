
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
