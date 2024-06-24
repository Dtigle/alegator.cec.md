
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

/****** Object:  StoredProcedure [dbo].[GetCandidatResult]    Script Date: 05.12.2018 13:42:46 ******/
SET ANSI_NULLS ON
