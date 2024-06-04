namespace CEC.SAISE.Domain
{
    public class AssignedRole : SaiseBaseEntity
    {
        public virtual Role Role { get; set; }

        public virtual SystemUser SystemUser { get; set; }
    }
}