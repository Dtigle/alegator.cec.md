CREATE PROCEDURE [dbo].[CEC_TotalVotes]
	@ElectionId int = 0
AS
BEGIN

	SET NOCOUNT ON;

	select sum(bp.BallotsValidVotes) totalValidVotes,
			sum (bp.BallotsSpoiled) totalNonValidVotes,
			sum(bp.BallotsCasted) totalVotes 

	from BallotPaper bp 			
	inner join AssignedPollingStation aps on aps.PollingStationId = bp.PollingStationId
	where bp.ElectionId = aps.ElectionId 
	and bp.ElectionId = @ElectionId

END
GO
