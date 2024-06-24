namespace CEC.SAISE.Domain
{
    public class AssignedPermission : SaiseBaseEntity
    {
        public virtual Role Role { get; set; }

        public virtual Permission Permission { get; set; }
    }
}