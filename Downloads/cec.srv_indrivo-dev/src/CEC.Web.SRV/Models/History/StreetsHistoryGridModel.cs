using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.History
{
    public class StreetsHistoryGridModel : LookupHistoryGridModel
    {
        [Display(Name = "Lookups_StreetTypeName", ResourceType = typeof(MUI))]
        public string StreetType { get; set; }

    }
}