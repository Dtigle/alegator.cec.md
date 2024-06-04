CREATE PROCEDURE [dbo].[CEC_CountOfVotersInSupplimentaryList] 
    @ElectionId int,
	@PollingStationId bigint
AS
BEGIN
	SET NOCOUNT ON;
	
	select count(*)
	from [dbo].[AssignedVoter] 
	where ElectionId = @ElectionId
	  and PollingStationId = @PollingStationId
	  and [Status] in (5001, 5002, 5020)	
	 
END