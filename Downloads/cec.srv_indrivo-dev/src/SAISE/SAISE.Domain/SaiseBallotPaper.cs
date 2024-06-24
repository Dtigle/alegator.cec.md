namespace SAISE.Domain
{
    public class SaiseBallotPaper : SaiseEntity
    {
        public virtual SaiseElection Election { get; set; }

        public virtual SaisePollingStation PollingStation { get; set; }

        public virtual int Status { get; set; }
    }
}