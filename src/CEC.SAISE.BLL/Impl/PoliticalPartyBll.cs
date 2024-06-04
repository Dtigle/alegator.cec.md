using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CEC.SAISE.BLL.Dto;
using CEC.SAISE.BLL.Helpers;
using CEC.SAISE.Domain;
using NHibernate.Mapping.ByCode.Impl;

namespace CEC.SAISE.BLL.Impl
{
	public class PoliticalPartyBll : IPoliticalPartyBll
	{
		private readonly ISaiseRepository _repository;

		public PoliticalPartyBll(ISaiseRepository repository)
		{
			_repository = repository;
		}

		public IList<PoliticalPartyDto> Get(long electionRoundId, long regionId)
		{
			var parties = _repository.Query<ElectionResult>()
				.Where(x => x.BallotPaper.ElectionRound.Id == electionRoundId)
				.Where(x => x.BallotPaper.PollingStation.Region.Id == regionId)
				.OrderBy(x => x.PoliticalParty.BallotOrder).Select(x => x.PoliticalParty).Distinct().ToList();

			//var data = parties.Select(x => new PoliticalPartyDto
			//{
			//	Id = x.Id,
			//	Code = x.Code,
			//	NameRo = x.NameRo,
			//	NameRu = x.NameRu,
			//	Status = x.Status.ToString(),
			//	DateOfRegistration = x.DateOfRegistration,
			//	BallotOrder = GetBallorOrder(x.Id, electionId, villageId),
			//	CandidateCount = GetCandidateCount(x.Id, electionId, villageId),
			//	IsIndependent = x.IsIndependent,
			//	AdditionInformation = (x.IsIndependent) ? GetAAdditionInformation(x.Id, electionId, villageId) : string.Empty,
			//	AdditionInformationRu = (x.IsIndependent) ? GetAAdditionInformationRu(x.Id, electionId, villageId) : string.Empty,
			//}).ToList().Distinct().OrderBy(x => x.BallotOrder).ToList();
			
			//return data;

			return parties.Select(x => MapPoliticalParty(x, electionRoundId, regionId, GetBallorOrder, GetCandidateCount))
				.ToList().Distinct().OrderBy(x => x.BallotOrder).ToList();
		}

		public IList<PoliticalPartyDto> Get(long electionRoundId)
		{
			var parties = _repository.Query<ElectionResult>()
				.Where(x => x.BallotPaper.ElectionRound.Id == electionRoundId)
				.OrderBy(x => x.PoliticalParty.BallotOrder).Select(x => x.PoliticalParty).Distinct().ToList();

			//var partiesProcessed = parties
			//	.Select(x => new PoliticalPartyDto()
			//	{
			//		Id = x.Id,
			//		Code = x.Code,
			//		NameRo = x.NameRo,
			//		NameRu = x.NameRu,
			//		Status = x.Status.ToString(),
			//		DateOfRegistration = x.DateOfRegistration,
			//		BallotOrder = GetBallorOrder(x.Id, electionId, -1),
			//		CandidateCount = GetCandidateCount(x.Id, electionId, -1),
			//		IsIndependent = x.IsIndependent,
			//		AdditionInformation = (x.IsIndependent) ? GetAAdditionInformation(x.Id, electionId, -1) : string.Empty,
			//		AdditionInformationRu = (x.IsIndependent) ? GetAAdditionInformationRu(x.Id, electionId, -1) : string.Empty,
			//	})
			//	.OrderBy(x => x.BallotOrder).ToList();

			return parties.Select(x => MapPoliticalParty(x, electionRoundId, -1, GetBallorOrder, GetCandidateCount))
				.OrderBy(x =>x.BallotOrder).ToList();

		}

		public List<ElectionCompetitorMember> GetCandidates(long partyId, long regionId, long electionRoundId)
		{
		    var p = _repository.Query<PollingStation>().FirstOrDefault(x => x.Region.Id == regionId);
		    var ap = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation == p)
		        .AssignedCircumscription;

            return _repository.Query<ElectionCompetitorMember>()
				.Where(x => x.ElectionRound.Id == electionRoundId)
				.Where(x => x.ElectionCompetitor.Id == partyId)
				.Where(x => x.AssignedCircumscription == ap)
				.OrderBy(x => x.CompetitorMemberOrder)
				.ThenBy(x => x.Id)
				.ToList();
		}

		public void UpdatePartyStatus(long partyId, PoliticalPartyStatus statusId)
		{
			var party = _repository.Get<ElectionCompetitor>(partyId);
			party.Status = statusId;
			_repository.SaveOrUpdate(party);
		}

		public void UpdateCandidateStatus(long candidateId, CandidateStatus statusId)
		{
			var candidate = _repository.Get<ElectionCompetitorMember>(candidateId);
			candidate.Status = statusId;
			_repository.SaveOrUpdate(candidate);
		}

		public IList<PoliticalPartyDto> GetAll(long electionRoundId, long regionId)
		{
		    var p = _repository.Query<PollingStation>().FirstOrDefault(x => x.Region.Id == regionId);
		    var ap = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation == p)
		        .AssignedCircumscription;

            var parties = _repository.Query<ElectionCompetitor>()
					.Where(x => x.ElectionRound.Id == electionRoundId)
					.Where(x => x.AssignedCircumscription  == ap )
					.Where(x => x.Id > 0)
					.OrderByDescending(x => x.PartyOrder)
					.ThenBy(x => x.Code)
					.Select(x => x)
					.Distinct().ToList();

			return parties.Select(x => new PoliticalPartyDto()
					{
						Id = x.Id,
						Code = x.Code,
						NameRo = x.NameRo,
						NameRu = x.NameRu,
						DateOfRegistration = x.DateOfRegistration,
						Status = x.Status.ToString(),
						BallotOrder = x.BallotOrder,
						CandidateCount = GetCandidateCount(x.Id, electionRoundId, regionId),
						IsIndependent = x.IsIndependent,
						CandidateData = (x.IsIndependent) ? GetAdditionInformation(x.Id, electionRoundId, regionId) : null
					}).ToList();

			//return parties.Select(x => MapPoliticalParty(x, electionId, villageId, (partyId, election, village) => x.BallotOrder, GetCandidateCount)).ToList();
		}

		public long GetCandidateCount(long partyId, long electionRoundId, long regionId)
		{
		    var p = _repository.Query<PollingStation>().FirstOrDefault(x => x.Region.Id == regionId);
		    var ap = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation == p)
		        .AssignedCircumscription;

            var candidates = _repository.Query<ElectionCompetitorMember>()
				.Where(x => x.ElectionRound.Id == electionRoundId && 
							x.AssignedCircumscription == ap &&
							x.ElectionCompetitor.Id == partyId)
				.Select(x => x)
				.Count();
			return candidates;
		}

		public IList<PoliticalPartyDto> GetAll(long electionId)
		{
			var parties = _repository.Query<ElectionCompetitor>()
				.Where(x => x.Id > 0)
				.OrderByDescending(x => x.PartyOrder)
				.ThenByDescending(x => x.Code)
				.Distinct().ToList();

			return parties.Select(x => new PoliticalPartyDto()
				{
					Id = x.Id,
					Code = x.Code,
					NameRo = x.NameRo,
					NameRu = x.NameRu,
					DateOfRegistration = x.DateOfRegistration,
					Status = x.Status.ToString(),
					BallotOrder = x.BallotOrder,
					CandidateCount = 0,
					IsIndependent = x.IsIndependent,
					//AdditionInformation = (x.IsIndependent) ? GetAAdditionInformation(x.Id, electionId, -1) : string.Empty,
					//AdditionInformationRu = (x.IsIndependent) ? GetAAdditionInformationRu(x.Id, electionId, -1) : string.Empty,
					CandidateData = (x.IsIndependent) ? GetAdditionInformation(x.Id, electionId, -1) : null
				}).ToList();
			//return parties.Select(x => MapPoliticalParty(x, electionId, -1, 
			//	(partyId, election, village) => x.BallotOrder,
			//	(partyId, election, village) => 0 ))
			//	.ToList();
		}

		public PoliticalPartyDto SaveUpdateParty(PoliticalPartyUpdateDto model)
		{
			if (model == null) throw new ArgumentNullException("model");

			var userId = SecurityHelper.GetLoggedUserId();
			var user = _repository.GetAsync<SystemUser>(userId).Result;

			ElectionCompetitor politicalParty = null;
			var newCreatedParty = model.Id == 0;
			politicalParty = newCreatedParty ? new ElectionCompetitor() : _repository.Get<ElectionCompetitor>(model.Id);

			politicalParty.Code = model.Code;
			politicalParty.NameRo = model.NameRo;
			politicalParty.NameRu = model.NameRu;
			politicalParty.Status = (PoliticalPartyStatus)model.Status;
			politicalParty.DateOfRegistration = model.DateOfRegistration;
			politicalParty.IsIndependent = false;
			politicalParty.EditDate = DateTime.Now;
			politicalParty.EditUser = user;
			

			_repository.SaveOrUpdate(politicalParty);

			if (newCreatedParty)
			{
				//var defaultElectionRound = _repository.Get<Election>(-1);
				//var defaultRegion = _repository.Get<Region>(-1);
				//var partyRelation = new ElectionCompetitor
				//					{
				//						ElectionCompetitor = politicalParty,
				//						Election = defaultElection,
				//						Village = defaultVillage
				//					};
				_repository.SaveOrUpdate(politicalParty);
			}

			return MapPoliticalParty(politicalParty, model.ElectionId, model.VillageId, (partyId, electionId, villageId) => politicalParty.BallotOrder, (partyId, electionId, villageId) => 0);

		}

		private long GetBallorOrder(long partyId, long electionRoundId, long regionId)
		{
			return _repository.Query<ElectionResult>()
				.Where(x => x.BallotPaper.ElectionRound.Id == electionRoundId)
				.Where(x => x.PoliticalParty.Id == partyId)
				.Where(x => x.BallotPaper.PollingStation.Region.Id == regionId)
				.Select(x => x.BallotOrder).ToList().FirstOrDefault();
		}

		private CandidateDto GetAdditionInformation(long partyId, long electionRoundId, long regionId)
		{
		    var p = _repository.Query<PollingStation>().FirstOrDefault(x => x.Region.Id == regionId);
		    var ap = _repository.Query<AssignedPollingStation>().FirstOrDefault(x => x.PollingStation == p)
		        .AssignedCircumscription;

            var candidate = _repository.Query<ElectionCompetitorMember>()
				.Where(x => x.ElectionRound.Id == electionRoundId)
				.Where(x => x.AssignedCircumscription == ap)
				.Where(x => x.ElectionCompetitor.Id == partyId)
				.Select(x => x).FirstOrDefault();

			if (candidate != null)
			{
				return MapCandidateToCandidateDto(candidate);
			}

			return null;
		}

		private CandidateDto MapCandidateToCandidateDto(ElectionCompetitorMember candidate)
		{
			return new CandidateDto()
			       {
				       DateOfBirth = candidate.DateOfBirth,
					   Occupation = candidate.Occupation,
					   OccupationRu = candidate.OccupationRu,
					   Workplace = candidate.Workplace,
					   WorkplaceRu = candidate.WorkplaceRu,
					   Id = candidate.Id,
					   Idnp = candidate.Idnp,
					   Status = candidate.Status,
					   NameRo = candidate.NameRo,
					   LastNameRo = candidate.LastNameRo,
					   NameRu = candidate.NameRu,
					   LastNameRu = candidate.LastNameRu,
			       };
		}

		private PoliticalPartyDto MapPoliticalParty(ElectionCompetitor party, long electionRoundId, long regionId, Func<long, long, long, long> ballotOrderFunc, Func<long, long, long, long> candidateCountFunc)
		{
			return new PoliticalPartyDto()
			       {
				       Id = party.Id,
					   Code = party.Code,
					   NameRo = party.NameRo,
					   NameRu = party.NameRu,
					   DateOfRegistration = party.DateOfRegistration,
					   Status = party.Status.ToString(),
					   BallotOrder = ballotOrderFunc(party.Id, electionRoundId, regionId),
					   CandidateCount = candidateCountFunc(party.Id, electionRoundId, regionId),
					   IsIndependent = party.IsIndependent,
					   CandidateData = (party.IsIndependent) ? GetAdditionInformation(party.Id, electionRoundId, regionId) : null
			       };
		}
		
		
	}
}