namespace CEC.SRV.Domain.Lookup
{
    public class PersonStatusType: Lookup
    {
        public virtual bool IsExcludable { get; set; }

        public const long Voter = 1;
        public const long Death = 2;
        public const long Judged = 3;
        public const long ForeignCitizen = 4;
        public const long Military = 5;
        public const long StatementAbroad = 6;
        public const long Detainee = 7;
    }
}
