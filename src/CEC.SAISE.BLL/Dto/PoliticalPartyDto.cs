using System;
using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto
{
	public class PoliticalPartyDto
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public string NameRo { get; set; }
		public string NameRu { get; set; }
		public string Status { get; set; }
		public long CandidateCount { get; set; }
		public long BallotOrder { get; set; }
		public DateTime DateOfRegistration { get; set; }
		public bool IsIndependent { get; set; }
		
		public CandidateDto CandidateData { get; set; }
	}

	public class CandidateDto	
	{
		public long Id { get; set; }
		public string NameRo { get; set; }
		public string LastNameRo { get; set; }
		public string NameRu { get; set; }
		public string LastNameRu { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Occupation { get; set; }
		public string OccupationRu { get; set; }
		public string Workplace { get; set; }
		public string WorkplaceRu { get; set; }
		public long Idnp { get; set; }
		public long CandidateOrder { get; set; }
		public CandidateStatus Status { get; set; }
	}

	public class PoliticalPartyUpdateDto
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public string NameRo { get; set; }
		public string NameRu { get; set; }
		public DateTime DateOfRegistration { get; set; }
		public long Status { get; set; }
		public long ElectionId { get; set; }
		public long VillageId { get; set; }
	}
}