namespace SAISE.Domain
{
    public class AssignedPollingStation : SaiseEntity
    {
        public virtual SaiseElectionRound ElectionRound { get; set; }

        public virtual SaisePollingStation PollingStation { get; set; }

        public virtual int Type { get; set; }

        public virtual long Status { get; set; }

        public virtual bool IsOpen { get; set; }
    }
}