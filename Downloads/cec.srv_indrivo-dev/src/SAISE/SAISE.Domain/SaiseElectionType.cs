namespace SAISE.Domain
{
    public class SaiseElectionType : SaiseEntity
    {
        public const long BaseList = 0;
        public const long Parliamentary = 1;
        public const long Local = 2;
        public const long Local_PrimarLocal = 10;
        public const long Local_ConsilieriLocal = 14;
        public const long Local_ConsilieriRaional = 18;
        public const long Local_ConsilieriMunicipali = 22;
        public const long Local_PrimarGeneral = 26;
        public const long Referendum = 3;


        public virtual string TypeName { get; set; }

        public virtual string DescriptionRo { get; set; }
    }
}