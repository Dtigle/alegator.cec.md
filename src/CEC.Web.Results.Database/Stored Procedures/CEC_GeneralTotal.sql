 
CREATE PROCEDURE [dbo].[CEC_GeneralTotal] 
	@ElectionId int
AS
BEGIN
	SET NOCOUNT ON;
	  
	SELECT COUNT(*) voters
	 FROM  [dbo].[AssignedVoter]
	 where [ElectionId] = @ElectionId
	   
END
GO

