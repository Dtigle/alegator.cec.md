CREATE PROCEDURE [dbo].[CEC_TotalBallots]
	@ElectionId int = 0
AS
BEGIN

	SET NOCOUNT ON;

	SELECT	count(*) totalBallotPapers
			, sum(case when bp.[Status] = 1 then 1 else 0 end) totalProcessedBallotPapers
	from BallotPaper bp
	inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
	where bp.ElectionId = @ElectionId

END
GO
