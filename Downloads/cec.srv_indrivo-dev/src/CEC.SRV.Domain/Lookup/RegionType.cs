namespace CEC.SRV.Domain.Lookup
{
    public class RegionType: Lookup
    {
        public static long CountryTypeId = 1;

        public static long NoResidenceRegionId = -1;

        public virtual byte Rank { get; set; }
    }
}
