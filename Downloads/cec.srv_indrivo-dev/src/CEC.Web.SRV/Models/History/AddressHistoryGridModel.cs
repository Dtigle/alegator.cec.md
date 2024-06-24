using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.History
{
    public class AddressHistoryGridModel : HistoryGridRow
    {
        [Display(Name = "StreetName", ResourceType = typeof(MUI))]
        public string StreetName { get; set; }

        [Display(Name = "HouseNo", ResourceType = typeof(MUI))]
        public long? HouseNumber { get; set; }

        [Display(Name = "HouseSufix", ResourceType = typeof(MUI))]
        public string Suffix { get; set; }

		[Display(Name = "NumberPollingStation", ResourceType = typeof(MUI))]
		public string PollingStation { get; set; }
        
    }
}