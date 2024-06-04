using System;

namespace CEC.Web.SRV.Models.Voters
{
    public class ModalDataModel
    {
        public Type ModelType { get; set; }
        public string PersonId { get; set; }
        public long? RegionId { get; set; }
        public long? LocalityId { get; set; }
        public long? AddressId { get; set; }
        public long? StreetId { get; set; }
        public int? HouseNumber { get; set; }
        public int? ApNumber { get; set; }
        public string ApSuffix { get; set; }
        public object AdditionalData { get; set; }
    }
}