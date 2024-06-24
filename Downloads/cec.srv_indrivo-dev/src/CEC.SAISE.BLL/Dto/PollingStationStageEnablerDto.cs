using Amdaris.Domain;
using CEC.SAISE.Domain;
using System;

namespace CEC.SAISE.BLL.Dto
{
	public class PollingStationStageEnablerDto : IEntity
	{
		public long Id { get; set; }

		public string Circumscription { get; set; }

		public string Lacality { get; set; }

		public string CircumscriptionNumber { get; set; }
        public string VotersListOrderTypes { get; set; }

        public string PollingStation { get; set; }

		public bool EnableOpening { get; set; }

		public bool EnableTurnout { get; set; }

		public bool EnabelElectionResult { get; set; }

	    public long OpeningVoters { get; set; }

	    public bool PSIsOpen { get; set; }
        public long? BallotPaperId { get; set; }
        public BallotPaperStatus? BallotPaperStatus { get; set; }
        public TimeSpan? ElectionStartTime { get; set; }
        public TimeSpan? ElectionEndTime { get; set; }
        public int TimeDifferenceMoldova { get; set; }
        public int ActivityTimeExtendedSecondDay { get; set; }
        public int ActivityTimeExtendedFirstDay { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCapturingSignature { get; set; }
        public string ElectionDuration { get; set; }


    }
}