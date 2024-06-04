CREATE PROCEDURE [dbo].[p_ElectionResultPerEDVS_DS_Parliamentary]
	@ElectionId						bigint,
	@DistrictId						bigint,
	@VillageId						bigint,
	@PollingStationId				bigint,
	@Mode							int = 1
	
AS
BEGIN
	/* ==========================================================================================
	INPUT PARAMETERS:
		@ElectionId
		@DistrictId
		@VillageId
		@PollingStationId
		
		@DataSource - [1|2]
			1 - Operative data source
			2 - Fixed data source (Not implemented)
			
		@Mode - [1|2]
			1 - Standard ouput (Election results for partyes per EDVS)
			2 - Mandate allocation output (Mandates allocation for candidates per partyes and per EDV)
			
	OUTPUT PARAMETERS: None
	
	OUTPUT COLUMNS:
		[ResultStatus] = 
			0 - Unknown			(for Consilieri & Primar)
			1 - Looser			(for Consilieri & Primar)
			2 - VotingRound II  (for Primar only)
			3 - Winner			(for Primar only)

		[DrawStatus] = 
			0 - Unknown
			1 - Winner			(Last mandate winner or winner to voting round 2)


	PERSONAL ERRORS:
	0 = No errors
	700000    -- Unintercepted error. Check your code. @ErrorException = %d
	700001    -- Invalid argument: %s. 
	700003    -- Cannot continue work. %s.

	USAGE EXAMPLE:

	begin tran
	declare @return_status int

	exec @return_status = dbo.pDK_ElectionResultPerEDVS_DS_Local
			 
	select [@return_status] = @return_status
	if @return_status != 0
	begin
	  rollback
	end
	else 
	  commit tran
  
	============================================================================================ */

	set nocount on

	-- get election description for report caption
	declare @E_DescriptionRo nvarchar(200)
	declare @E_ElectionId bigint 
	declare @E_DateOfElection datetime 
	declare @E_ElectionType int 
	declare @E_TypeName nvarchar(200)
	select 
		@E_ElectionId = E.E_ElectionId,
		@E_DescriptionRo = E.E_DescriptionRo,
		@E_DateOfElection = E.E_DateOfElection,
		@E_ElectionType = E.E_ElectionType,
		@E_TypeName = E.E_TypeName
	from vDK_Election as E where E.E_ElectionId = @ElectionId

	--select @E_DescriptionRo = et.DescriptionRo
	--from ElectionType et 
	--inner join Election e on et.ElectionTypeId = e.Type
	--where e.ElectionId = @ElectionId

	declare @results table(
		RowNumber bigint 
		,CountCompetitors_PerEDVS int
		,SelectionType int 
		,CountBP_Total_PerEDVS bigint 
		,CountBP_Filled_PerEDVS bigint 
		,Sum_Ballots_RegisteredVoters_Total_PerEDVS bigint 
		,Sum_Ballots_Supplementary_Total_PerEDVS bigint 
		,Ratio_CountBP_Processed float 
		,Sum_Ballots_BallotsIssued_Total_PerEDVS bigint 
		,Sum_Ballots_BallotsCasted_Total_PerEDVS bigint 
		,Sum_Ballots_RatioParticipation_Total_PerEDVS float 
		,ER_PoliticalPartyId bigint 
		,PP_PoliticalPartyId bigint 
		,Sum_Ballots_DifferenceIssuedCasted_Total_PerEDVS bigint 
		,ER_Sum_BallotCount bigint 
		,Sum_Ballots_BallotsSpoiled_Total_PerEDVS bigint 
		,Ratio_BallotCount float 
		,Ratio_BallotCountDisp float 
		,Sum_Ballots_ValidVotes_Total_PerEDVS bigint 
		,Sum_Ballots_ValidVotes_TotalER_PerEDVS bigint
		,ST_MandateColumnIsActual bit
		,Sum_Ballots_BallotsReceived_Total_PerEDVS bigint
		,ST_PP_MandateCount bigint
		,ST_ResultStatus tinyint
		,ST_DrawExists bit
		,Sum_Ballots_BallotsUnusedSpoiled_Total_PerEDVS bigint
		,ST_IsDrawed bit
		,C_CandidateId bigint
		,C_LastNameRo nvarchar(200)
		,C_LastNameRu nvarchar(200)
		,C_NameRo nvarchar(200)
		,C_NameRu nvarchar(200)
		,ST_DrawStatus tinyint
		,ST_DrawId bigint
		,ST_DrawDescription nvarchar(200)
		,PP_Code nvarchar(50)
		,PP_NameRo nvarchar(200)
		,PP_NameRu nvarchar(200)
		,PP_DisplayFromNameRo nvarchar(200)
		,PP_DisplayFromNameRu nvarchar(200)
		,MNumberCount int
		,PP_IsIndependent bit not null
		,E_ElectionId bigint
		,E_ElectionType int
		,E_TypeName nvarchar(200)
		,E_DateOfElection datetime
		,E_DescriptionRo nvarchar(200)
		,PP_RepDisp_NameRO nvarchar(200)
		,PP_RepDisp_NameRU nvarchar(200)
		,C_RepDisp_NameRO nvarchar(200)
		,C_RepDisp_NameRU nvarchar(200)
		,D_DistrictId bigint
		,D_Nr int
		,D_NamePropRo nvarchar(200)
		,D_NamePropRu nvarchar(200)
		,D_MapName nvarchar(200)
		,D_DistrictCouncilSeats tinyint
		,V_VillageId bigint
		,V_Nr int
		,V_Type int
		,V_TypeDisplayRo nvarchar(200)
		,V_TypeDisplayRU nvarchar(200)
		,V_NamePropRo nvarchar(200)
		,V_NamePropRu nvarchar(200)
		,V_LocalCouncilSeats tinyint
		,PS_PollingStationId bigint
		,PS_Type int
		,PS_Nr int
		,PS_NameRo nvarchar(200)
		,PS_NameRU nvarchar(200)
		,ER_BallotOrder int
	);

	declare @CountBP_Total_PerEDVS bigint
	declare @CountBP_Filled_PerEDVS bigint
	declare @Ratio_CountBP_Processed float
	declare @Sum_Ballots_RegisteredVoters_Total_PerEDVS bigint
	declare @Sum_Ballots_Supplementary_Total_PerEDVS bigint
	declare @Sum_Ballots_BallotsIssued_Total_PerEDVS bigint
	declare @Sum_Ballots_BallotsCasted_Total_PerEDVS bigint
	declare @Sum_Ballots_RatioParticipation_Total_PerEDVS float
	declare @Sum_Ballots_DifferenceIssuedCasted_Total_PerEDVS bigint
	declare @Sum_Ballots_BallotsSpoiled_Total_PerEDVS bigint
	declare @Sum_Ballots_ValidVotes_Total_PerEDVS bigint
	declare @Sum_Ballots_BallotsReceived_Total_PerEDVS bigint
	declare @Sum_Ballots_BallotsUnusedSpoiled_Total_PerEDVS bigint
	-- Определяем ОБЩЕЕ количество протоколов по EDVS
		select 
			@CountBP_Total_PerEDVS = count(BP.BallotPaperId),
			@CountBP_Filled_PerEDVS = sum(case when bp.Status = 1 then 1 else 0 end),
			@Ratio_CountBP_Processed = case when @CountBP_Total_PerEDVS <> 0 then cast(@CountBP_Filled_PerEDVS as float) / @CountBP_Total_PerEDVS else 0 end,
			@Sum_Ballots_RegisteredVoters_Total_PerEDVS = sum(bp.RegisteredVoters),
			@Sum_Ballots_Supplementary_Total_PerEDVS = sum(bp.Supplementary),
			@Sum_Ballots_BallotsIssued_Total_PerEDVS = SUM(BP.BallotsIssued),
			@Sum_Ballots_BallotsCasted_Total_PerEDVS = SUM(BP.BallotsCasted),
			@Sum_Ballots_RatioParticipation_Total_PerEDVS = 
				case when (@Sum_Ballots_RegisteredVoters_Total_PerEDVS + @Sum_Ballots_Supplementary_Total_PerEDVS) <> 0 then
					cast(@Sum_Ballots_BallotsIssued_Total_PerEDVS as float)/(@Sum_Ballots_RegisteredVoters_Total_PerEDVS + @Sum_Ballots_Supplementary_Total_PerEDVS)
				else 0
				end,
			
			
			@Sum_Ballots_DifferenceIssuedCasted_Total_PerEDVS = SUM(BP.DifferenceIssuedCasted),
			@Sum_Ballots_BallotsSpoiled_Total_PerEDVS = SUM(BP.BallotsSpoiled),
			
			-- Определяем ОБЩЕЕ количество валидных ГОЛОСОВ в ОБРАБОТАННЫХ протоколах по EDVS
			@Sum_Ballots_ValidVotes_Total_PerEDVS = SUM(BP.BallotsValidVotes),
			@Sum_Ballots_BallotsReceived_Total_PerEDVS = SUM(BP.BallotsReceived),
			@Sum_Ballots_BallotsUnusedSpoiled_Total_PerEDVS = SUM(BP.BallotsUnusedSpoiled)
		from BallotPaper bp
		where bp.BallotPaperId in (select ballotPaperId from fn_GetBallotPapersNonLocal(@electionId, @districtId, @villageId, @pollingStationId))

		declare @D_DistrictId bigint
		declare @D_Nr int 
		declare @D_NamePropRo nvarchar(200)
		declare @D_NamePropRu nvarchar(200)
		declare @D_MapName nvarchar(200)
		select 
			@D_DistrictId = d.D_DistrictId,
			@D_Nr = d.D_Nr,
			@D_NamePropRo = d.D_NameRo,
			@D_NamePropRu = d.D_NameRu,
			@D_MapName = d.D_MapName
		from vDK_District d where d.D_DistrictId = @DistrictId

		declare @V_VillageId bigint 
		declare @V_Type int 
		declare @V_Nr int 
		declare @V_NamePropRo nvarchar(200) 
		declare @V_NamePropRu nvarchar(200)
		select 
			@V_VillageId = V.V_VillageId,
			@V_Type = V.V_Type,
			@V_Nr = V.V_Nr,
			@V_NamePropRo = V.V_NamePropRo,
			@V_NamePropRu = V.V_NamePropRu
			from vDK_Village as V where V.V_VillageId = @VillageId

		declare @V_TypeDisplayRo nvarchar(200)
		declare @V_TypeDisplayRu nvarchar(200)
		set @V_TypeDisplayRo = dbo.fDK_GetVillageDisplayType(@V_Type, 1)
		set @V_TypeDisplayRu = dbo.fDK_GetVillageDisplayType(@V_Type, 2)

		declare @PS_PollingStationId bigint 
		declare @PS_Type int 
		declare @PS_Nr int 
		declare @PS_NameRo nvarchar(200) 
		declare @PS_NameRu nvarchar(200)
		select 
			@PS_PollingStationId = PS.PS_PollingStationId,
			@PS_Type = PS.PS_Type,
			@PS_Nr = PS.PS_Nr,
			@PS_NameRo = PS.PS_NameRo,
			@PS_NameRu = PS.PS_NameRu
		from vDK_PollingStation as PS where PS.PS_PollingStationId = @PollingStationId

		declare @ST_MandateColumnIsActual bit
		set @ST_MandateColumnIsActual = case when @DistrictId  is null then 1 else 0 end

	insert into @results 
			(RowNumber  
			,CountCompetitors_PerEDVS 
			,SelectionType  
			,CountBP_Total_PerEDVS  
			,CountBP_Filled_PerEDVS  
			,Sum_Ballots_RegisteredVoters_Total_PerEDVS  
			,Sum_Ballots_Supplementary_Total_PerEDVS  
			,Ratio_CountBP_Processed  
			,Sum_Ballots_BallotsIssued_Total_PerEDVS  
			,Sum_Ballots_BallotsCasted_Total_PerEDVS  
			,Sum_Ballots_RatioParticipation_Total_PerEDVS  
			,ER_PoliticalPartyId  
			,PP_PoliticalPartyId  
			,Sum_Ballots_DifferenceIssuedCasted_Total_PerEDVS  
			,ER_Sum_BallotCount  
			,Sum_Ballots_BallotsSpoiled_Total_PerEDVS  
			,Ratio_BallotCount  
			,Ratio_BallotCountDisp  
			,Sum_Ballots_ValidVotes_Total_PerEDVS  
			,Sum_Ballots_ValidVotes_TotalER_PerEDVS 
			,ST_MandateColumnIsActual 
			,Sum_Ballots_BallotsReceived_Total_PerEDVS 
			,ST_PP_MandateCount 
			,ST_ResultStatus 
			,ST_DrawExists 
			,Sum_Ballots_BallotsUnusedSpoiled_Total_PerEDVS 
			,ST_IsDrawed 
			,C_CandidateId 
			,C_LastNameRo 
			,C_LastNameRu 
			,C_NameRo 
			,C_NameRu 
			,ST_DrawStatus 
			,ST_DrawId 
			,ST_DrawDescription 
			,PP_Code
			,PP_NameRo 
			,PP_NameRu 
			,PP_DisplayFromNameRo 
			,PP_DisplayFromNameRu 
			,MNumberCount 
			,PP_IsIndependent  
			,E_ElectionId 
			,E_ElectionType 
			,E_TypeName 
			,E_DateOfElection 
			,E_DescriptionRo 
			,PP_RepDisp_NameRO 
			,PP_RepDisp_NameRU 
			,C_RepDisp_NameRO 
			,C_RepDisp_NameRU 
			,D_DistrictId 
			,D_Nr 
			,D_NamePropRo 
			,D_NamePropRu 
			,D_MapName 
			,D_DistrictCouncilSeats 
			,V_VillageId 
			,V_Nr 
			,V_Type 
			,V_TypeDisplayRo 
			,V_TypeDisplayRU 
			,V_NamePropRo 
			,V_NamePropRu 
			,V_LocalCouncilSeats 
			,PS_PollingStationId 
			,PS_Type 
			,PS_Nr 
			,PS_NameRo 
			,PS_NameRU 
			,ER_BallotOrder)
	select	x.BallotOrder, 
			-1, --CountCompetitors_PerEDVS 
			-1, --SelectionType  
			@CountBP_Total_PerEDVS, --CountBP_Total_PerEDVS  
			@CountBP_Filled_PerEDVS, --CountBP_Filled_PerEDVS  
			@Sum_Ballots_RegisteredVoters_Total_PerEDVS, --Sum_Ballots_RegisteredVoters_Total_PerEDVS  
			@Sum_Ballots_Supplementary_Total_PerEDVS, --Sum_Ballots_Supplementary_Total_PerEDVS  
			@Ratio_CountBP_Processed, --Ratio_CountBP_Processed  
			@Sum_Ballots_BallotsIssued_Total_PerEDVS, --Sum_Ballots_BallotsIssued_Total_PerEDVS  
			@Sum_Ballots_BallotsCasted_Total_PerEDVS, --Sum_Ballots_BallotsCasted_Total_PerEDVS  
			@Sum_Ballots_RatioParticipation_Total_PerEDVS, --Sum_Ballots_RatioParticipation_Total_PerEDVS  
			x.PoliticalPartyId, --ER_PoliticalPartyId  
			x.PoliticalPartyId, --PP_PoliticalPartyId  
			@Sum_Ballots_DifferenceIssuedCasted_Total_PerEDVS, --Sum_Ballots_DifferenceIssuedCasted_Total_PerEDVS  
			x.BallotCountTotal, --ER_Sum_BallotCount  
			@Sum_Ballots_BallotsSpoiled_Total_PerEDVS, --Sum_Ballots_BallotsSpoiled_Total_PerEDVS  
			-1, --Ratio_BallotCount  
			case when x.BallotsValidVotes > 0 then cast(x.BallotCountTotal as float) / x.BallotsValidVotes else 0 end, --Ratio_BallotCountDisp  
			@Sum_Ballots_ValidVotes_Total_PerEDVS, --Sum_Ballots_ValidVotes_Total_PerEDVS  
			-1, --Sum_Ballots_ValidVotes_TotalER_PerEDVS 
			@ST_MandateColumnIsActual, --ST_MandateColumnIsActual 
			@Sum_Ballots_BallotsReceived_Total_PerEDVS, --Sum_Ballots_BallotsReceived_Total_PerEDVS 
			0, --ST_PP_MandateCount 
			0, --ST_ResultStatus 
			0, --ST_DrawExists 
			@Sum_Ballots_BallotsUnusedSpoiled_Total_PerEDVS, --Sum_Ballots_BallotsUnusedSpoiled_Total_PerEDVS 
			0, --ST_IsDrawed 
			-1, --C_CandidateId 
			-1, --C_LastNameRo 
			-1, --C_LastNameRu 
			-1, --C_NameRo 
			-1, --C_NameRu 
			0, --ST_DrawStatus 
			-1, --ST_DrawId 
			-1, --ST_DrawDescription 
			-1, --PP_Code
			-1, --PP_NameRo 
			-1, --PP_NameRu 
			-1, --PP_DisplayFromNameRo 
			-1, --PP_DisplayFromNameRu 
			101, --MNumberCount 
			x.IsIndependent, --PP_IsIndependent  
			@E_ElectionId, --E_ElectionId 
			@E_ElectionType, --E_ElectionType 
			@E_TypeName, --E_TypeName 
			@E_DateOfElection, --E_DateOfElection 
			@E_DescriptionRo, --E_DescriptionRo 
			x.NameRo, --PP_RepDisp_NameRO 
			x.NameRo, --PP_RepDisp_NameRU 
			'', --C_RepDisp_NameRO 
			'', --C_RepDisp_NameRU 
			@D_DistrictId, --D_DistrictId 
			@D_Nr, --D_Nr 
			@D_NamePropRo, --D_NamePropRo 
			@D_NamePropRu, --D_NamePropRu 
			@D_MapName, --D_MapName 
			0, --D_DistrictCouncilSeats 
			@V_VillageId, --V_VillageId 
			@V_Nr, --V_Nr 
			@V_Type, --V_Type 
			@V_TypeDisplayRo, --V_TypeDisplayRo 
			@V_TypeDisplayRU, --V_TypeDisplayRU 
			@V_NamePropRo, --V_NamePropRo 
			@V_NamePropRu, --V_NamePropRu 
			0, --V_LocalCouncilSeats 
			@PS_PollingStationId, --PS_PollingStationId 
			@PS_Type, --PS_Type 
			@PS_Nr, --PS_Nr 
			@PS_NameRo, --PS_NameRo 
			@PS_NameRU, --PS_NameRU 
			x.BallotOrder --ER_BallotOrder
	from(
	select
		pp.PoliticalPartyId, pp.IsIndependent, pp.Status, pp.Code, pp.NameRo, pp.BallotOrder, 
		sum(pp.BallotCountTotal) as BallotCountTotal,
		sum(pp.BallotsValidVotes) as BallotsValidVotes
	from 
	(
		select pp.PoliticalPartyId, pp.IsIndependent, pp.Status, pp.Code, pp.NameRo, er.BallotOrder, 
				bp.BallotsValidVotes, er.BallotCount as BallotCountTotal
		from ElectionResult er
		inner join BallotPaper bp on er.BallotPaperId = bp.BallotPaperId
		inner join PoliticalParty pp on er.PoliticalPartyId = pp.PoliticalPartyId
		left outer join Candidate c on er.CandidateId = c.CandidateId
		where bp.ElectionId = @electionId and er.BallotPaperId in (select ballotPaperId from fn_GetBallotPapersNonLocal(@electionId, @districtId, @villageId, @pollingStationId))
	) pp
	group by pp.PoliticalPartyId, pp.IsIndependent, pp.Status, pp.Code, pp.NameRo, pp.BallotOrder
	) x
	order by x.BallotOrder

	if @Mode = 2
	BEGIN
		/*
		 Create mandates
		*/
		 declare @totalMandates bigint = 101
		 declare @totalMandatesRes bigint = 101
		 declare @individualThreshold bigint = 2
		 declare @politicalPartyThreshold bigint = 4
		 declare @numberOfIndependentPassed bigint = 0
		 declare @numberVotesOfIndependentPassed bigint = 0
		 declare @totalNumberOfVotes bigint = 0
		 declare @electoralCoefficient float = 0.0
		 declare @totalValidVotesRes bigint = 0;

		declare @row bigint

		DECLARE db_cursor CURSOR FOR  
		SELECT RowNumber
		FROM @results
		 where PP_IsIndependent = 1 and Ratio_BallotCountDisp >= 0.02  

		OPEN db_cursor  
		FETCH NEXT FROM db_cursor INTO @row  

		WHILE @@FETCH_STATUS = 0  
		BEGIN  
				--// Article 87 (1)
			   set @numberOfIndependentPassed = @numberOfIndependentPassed + 1
			   update @results set ST_PP_MandateCount = 1 where RowNumber =@row
			   --// Article 87 (2)
			   set @totalMandatesRes = @totalMandatesRes - 1

			   --// Article 87 (3)
			   update @results set Sum_Ballots_ValidVotes_Total_PerEDVS -= Ratio_BallotCount where RowNumber = @row

			   set @numberVotesOfIndependentPassed = @numberVotesOfIndependentPassed + (select Ratio_BallotCount from @results where RowNumber = @row)
			   --print '@numberVotesOfIndependentPassed ' + cast(@numberVotesOfIndependentPassed as nvarchar(200))
			   FETCH NEXT FROM db_cursor INTO @row  
		END  
		CLOSE db_cursor  
		DEALLOCATE db_cursor



		--// Article 87 (3)
		--// Electoral Coefficient
		set @electoralCoefficient = (cast(@totalNumberOfVotes as float) - cast(@numberVotesOfIndependentPassed as float))/ (@totalMandates - @numberOfIndependentPassed)
		--print '@electoralCoefficient ' + cast(@electoralCoefficient as nvarchar(200))



		DECLARE db_cursor CURSOR FOR  
		SELECT RowNumber
		FROM @results
		 where PP_IsIndependent = 0 and Ratio_BallotCountDisp >= 0.04    

		OPEN db_cursor  
		FETCH NEXT FROM db_cursor INTO @row  

		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			   --// Article 87 (4)
			   --// FO: Rounding needed	
			   declare @Ratio_BallotCountDisp bigint
			   select @Ratio_BallotCountDisp = cast(Ratio_BallotCountDisp as float)/ @electoralCoefficient from @results where RowNumber = @row
			   update @results set ST_PP_MandateCount = cast(Ratio_BallotCountDisp as float)/ @electoralCoefficient where RowNumber = @row
			   set @totalMandatesRes = @totalMandatesRes - @Ratio_BallotCountDisp
			   --print '@totalMandatesRes ' + cast(@totalMandatesRes as nvarchar(200))

			   FETCH NEXT FROM db_cursor INTO @row  
		END  
		CLOSE db_cursor  
		DEALLOCATE db_cursor 

		while @totalMandatesRes > 0
		begin
			DECLARE db_cursor CURSOR FOR  
			SELECT RowNumber
			FROM @results
			 where PP_IsIndependent = 0 and Ratio_BallotCountDisp >= 0.04 order by ST_PP_MandateCount desc  

			OPEN db_cursor  
			FETCH NEXT FROM db_cursor INTO @row  

			WHILE (@@FETCH_STATUS = 0 and @totalMandatesRes > 0)
			BEGIN  	   
				   update @results set ST_PP_MandateCount = ST_PP_MandateCount + 1 where RowNumber = @row
				   set @totalMandatesRes = @totalMandatesRes - 1
				   --print '@totalMandatesRes ' + cast(@totalMandatesRes as nvarchar(200))

				   FETCH NEXT FROM db_cursor INTO @row  
			END  
			CLOSE db_cursor  
			DEALLOCATE db_cursor 
			if @totalMandatesRes < 1 begin break end
		end
	END
	select * from @results
	--select RowNumber, ST_PP_MandateCount, Ratio_BallotCountDisp, Ratio_BallotCount, cast(Ratio_BallotCountDisp as float)/ @electoralCoefficient from @results where ST_PP_MandateCount > -1
	--select * from @results where PP_IsIndependent = 1 and Ratio_BallotCountDisp >= 0.02
	--select * from @results where PP_IsIndependent = 0 and Ratio_BallotCountDisp >= 0.04
	
END -- CREATE PROCEDURE
