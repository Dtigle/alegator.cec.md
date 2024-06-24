-- =============================================
-- Author:		Amdaris SRL
-- Create date: 
-- Description:	
-- =============================================
create FUNCTION [dbo].[fn_GetBallotPapersNonLocal] 
(
	-- Add the parameters for the function here
	@electionId bigint, 
	@districtId bigint = null,
	@villageId bigint = null,
	@pollingStationid bigint = null
)
RETURNS 
@Table_Var TABLE 
(
	-- Add the column definitions for the TABLE variable here
	ballotPaperId bigint 
)
AS
BEGIN
	declare @electionType bigint

	select @electionType = e.[Type] from Election e
	inner join ElectionType et on e.[Type] = et.ElectionTypeId
	where e.ElectionId = @electionId

	if @electionType & 2 = 2 -- checking for local election. if so, throw error.
	begin
		--'use non local elections with this function'
		return
	end

	-- Fill the table variable with the rows for your result set
	if @districtId is null
	begin
		insert into @Table_Var (ballotPaperId)
		select bp.BallotPaperId from BallotPaper bp 
		inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
		where bp.ElectionId=@electionId
	end
	else
	begin
		if @villageId is null
		begin
			insert into @Table_Var (ballotPaperId)
			select bp.BallotPaperId from BallotPaper bp 
			inner join PollingStation ps on bp.PollingStationId = ps.PollingStationId
			inner join AssignedPollingStation aps on ps.PollingStationId = aps.PollingStationId and bp.ElectionId = aps.ElectionId
			inner join Village v on ps.VillageId = v.VillageId
			where bp.ElectionId=@electionId and v.DistrictId = @districtId
		end
		else
		begin
			if @pollingStationId is null
			begin
				insert into @Table_Var (ballotPaperId)
				select bp.BallotPaperId from BallotPaper bp
				inner join PollingStation ps on bp.PollingStationId = ps.PollingStationId
				inner join AssignedPollingStation aps on ps.PollingStationId = aps.PollingStationId  and bp.ElectionId = aps.ElectionId
				where bp.ElectionId=@electionId and ps.VillageId = @villageId
			end
			else
			begin
				insert into @Table_Var (ballotPaperId)
				select bp.BallotPaperId from BallotPaper bp 
				inner join AssignedPollingStation aps on bp.PollingStationId = aps.PollingStationId  and bp.ElectionId = aps.ElectionId
				where bp.ElectionId=@electionId and bp.PollingStationId = @pollingStationId
			end
		end
	end

	RETURN 
END

GO


