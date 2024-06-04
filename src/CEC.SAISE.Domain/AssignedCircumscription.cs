namespace CEC.SAISE.Domain
{
    public class AssignedCircumscription : SaiseBaseEntity
    {
        public virtual string Number { get; set; }

        public virtual string NameRo { get; set; }
        
        public virtual long CircumscriptionId { get; set; }

        public virtual bool IsFromUtan { get; set; }

        public virtual ElectionRound ElectionRound { get; set; }

        public virtual Region Region { get; set; }


        public virtual string GetFullName()
        {
            return string.Format("{0,3:D3} - {1}", this.Number, this.NameRo);
        }
    }
}
