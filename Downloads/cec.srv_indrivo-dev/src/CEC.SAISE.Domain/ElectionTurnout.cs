namespace CEC.SAISE.Domain
{
    public class ElectionTurnout : SaiseBaseEntity
    {
        public virtual long ListCount { get; set; }

        public virtual long SupplementaryCount { get; set; }

        public virtual string TimeOfEntry { get; set; }

        public virtual AssignedPollingStation AssignedPollingStation { get; set; }
    }
}