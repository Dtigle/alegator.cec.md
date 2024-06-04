CREATE PROCEDURE [dbo].CEC_CountOfVotersReceivedBallots 
    @ElectionId int,
	@PollingStationId bigint
AS
BEGIN
	SET NOCOUNT ON;
	
	select count(*)
	from [dbo].[AssignedVoter] 
	where ElectionId = @ElectionId
	  and PollingStationId = @PollingStationId
	  and [Status] >= 5000 
	  --removing dead, no citizens and other people without voting rights
	  and [Status] not in (9010, 9002, 9004)	
	 
END