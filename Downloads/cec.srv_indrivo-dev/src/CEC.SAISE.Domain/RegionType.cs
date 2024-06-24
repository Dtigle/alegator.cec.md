namespace CEC.SAISE.Domain
{
    public class RegionType : SaiseBaseEntity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int Rank { get; set; }
    }
}
