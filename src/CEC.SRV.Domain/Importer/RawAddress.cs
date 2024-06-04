namespace CEC.SRV.Domain.Importer
{
    public class RawAddress 
    {
        public virtual long? ExternalRegionId { get; set; }

        public virtual string RegionName { get; set; }

        public virtual long? ExternalStreetId { get; set; }

        public virtual string StreetName { get; set; }

        public virtual string HomeNr { get; set; }

        public virtual string HomeSuffix { get; set; }
    }
}