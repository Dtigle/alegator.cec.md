
using CEC.SRV.BLL.Dto;

namespace CEC.Web.Api.Models
{
	public class StreetRequest : Select2Request
	{
		public long RegionId { get; set; }
	}
}
