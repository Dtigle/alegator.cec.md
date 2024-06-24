using CEC.SAISE.Domain;

namespace CEC.SAISE.BLL.Dto.Concurents
{
	public class CandidateConflictDto
	{
		public ElectionRound ElectionRound { get; set; }
		public ElectionCompetitor PoliticalParty { get; set; }
		public AssignedCircumscription AssignedCircumscription { get; set; }
	}
}