
-- =============================================
-- Create date: 04-08-2014
-- Description:	Get voting results per interval
-- =============================================
CREATE PROCEDURE [dbo].[CEC_GeneralOnTimeAddResults] 
    @ElectionId int,
	@StartHour datetime = null, 
	@EndHour datetime = null,
	@interval int = 20
AS
BEGIN
	SET NOCOUNT ON;
	  
	if(@StartHour is null)
	   raiserror('invalid parameter: @StartHour', 18, 0);
	if(@EndHour is null)
	   raiserror('invalid parameter: @EndHour', 18, 0);
	    
	select  a1.hr1 ora, 
			a1.mn1 as minuta,
			COUNT(*) voters
	from
	(SELECT 
		   datepart(hh, [EditDate]) as hr1
		   ,@interval  * (datepart(N, [EditDate]) / @interval)  as mn1      
	 FROM [dbo].[AssignedVoter]
	 where [EditDate] between @starthour and DATEADD(minute, @interval, @endhour)
	   AND [ElectionId] = @ElectionId
	   AND ([Status] = 5001 OR [Status] = 5002 OR [Status] = 5020)) a1   
	 group by a1.hr1, a1.mn1
	 order by ora, minuta;
	 
END
