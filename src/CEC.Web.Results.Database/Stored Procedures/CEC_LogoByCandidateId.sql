CREATE PROCEDURE [dbo].[CEC_LogoByCandidateId]
	@CandidateId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select pp.Logo
	  from PoliticalParty pp 
	 where pp.PoliticalPartyId = @CandidateId;

END
GO