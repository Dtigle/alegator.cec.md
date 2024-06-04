CREATE PROCEDURE [dbo].[CEC_GeneralPartidResults]
	@ElectionId int = 0
AS
BEGIN

	SET NOCOUNT ON;
	
	SELECT er.[PoliticalPartyId] ppId,
		   (select pp.Code from PoliticalParty pp where pp.PoliticalPartyId = er.[PoliticalPartyId]) ppName, 
		   (select pp.Color from PoliticalParty pp where pp.PoliticalPartyId = er.[PoliticalPartyId]) ppColor, 
		   sum(er.[BallotCount]) ppTotal,
		   (select pp.PartyType from PoliticalParty pp where pp.PoliticalPartyId = er.[PoliticalPartyId]) ppPartyType
		   ,er.BallotOrder
	  FROM [dbo].[ElectionResult] er, [dbo].[BallotPaper] bp
	 WHERE er.BallotPaperId = bp.BallotPaperId
	   AND bp.ElectionId = @ElectionId
	GROUP BY er.[PoliticalPartyId], er.BallotOrder
	
END

GO

