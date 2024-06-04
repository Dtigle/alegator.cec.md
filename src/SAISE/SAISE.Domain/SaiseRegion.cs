namespace SAISE.Domain
{
    public class SaiseRegion : SaiseEntity
    {
        public virtual string Name { get; set; }
        public virtual string NameRu { get; set; }
        public virtual string Description { get; set; }
    }
}
