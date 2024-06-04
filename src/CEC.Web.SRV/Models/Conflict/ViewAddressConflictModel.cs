
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Conflict
{
    public class ViewAddressConflictModel : StatusMessageConflictModel
    {
        [Display(Name = "Conflict_Address", ResourceType = typeof(MUI))]
        public string Address { get; set; }
    }
}