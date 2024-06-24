namespace CEC.SRV.Domain.Lookup
{
    public class DocumentType : Lookup
    {
        public static long NoDocument = 0;
        public static long Buletin = 1;
        public static long Passport = 2;
        public static long Sovietic = 4;
        public static long Provizoriu = 17;
        public static long F_9 = 3;
        public virtual bool IsPrimary { get; set; }

        public static long Parse(int rspDocTypeCode)
        {
            switch (rspDocTypeCode)
            {
                case 0:
                    return NoDocument;
                case 5:
                    return Buletin;
                case 3:
                    return Passport;
                case 71:
                    return F_9;
                case 60:
                    return Sovietic;
                case 17:
                    return Provizoriu;
                default:
                    return -1;
            }
        }
    }
}