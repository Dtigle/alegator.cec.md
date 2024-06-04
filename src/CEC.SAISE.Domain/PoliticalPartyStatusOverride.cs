namespace CEC.SAISE.Domain
{
	public class PoliticalPartyStatusOverride : SaiseBaseEntity
	{
		public virtual ElectionCompetitor PoliticalParty { get; set; }

		public virtual PoliticalPartyStatus Status { get; set; }

		public virtual ElectionRound ElectionRound { get; set; }

		public virtual AssignedCircumscription AssignedCircumscription { get; set; }
	}
}