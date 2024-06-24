
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class ResolveAddressConflictModel
    {
        public long RspId { get; set; }
        public long RegionId { get; set; }
        public string RspAddress { get; set; }

        [Display(Name = "AllAddressPerUser", ResourceType = typeof(MUI))]
        [UIHint("Checkbox")]
        public bool AllAddressPerUser { get; set; }

        [Display(Name = "ApplyToAllIdenticalAddresses", ResourceType = typeof(MUI))]
        [UIHint("Checkbox")]
        public bool ApplyToAllIdenticalAddresses { get; set; }
    }
}