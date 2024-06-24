using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.Address
{
    public class AddressApartmentGridModel : AddressGridModel
    {
		[Display(Name = "NumberPollingStation", ResourceType = typeof(MUI))]
		[JqGridColumnSortable(true)]
		[JqGridColumnSearchable(true)]
        [JqGridColumnEditable(false)]
		[SearchData(DbName = "PollingStation.Number", Type = typeof(string))]
		[Required]
		public string PollingStation { get; set; }
        
    }
}