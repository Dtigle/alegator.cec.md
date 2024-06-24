CREATE PROCEDURE [dbo].[CEC_ElectionDayById] 
	@ElectionId int = 0
AS
BEGIN

	SET NOCOUNT ON;

	SELECT  DateOfElection
      FROM  Election
     WHERE  ElectionId=@ElectionId
   
END

GO

