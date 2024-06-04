using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Address
{
	public class EditGeolocationModel
    {
        public long Id { get; set; }
		
		[Required]
		public double Geolongitude { get; set; }

		[Required]
		public double Geolatitude { get; set; }

		[Display(Name = "GeoAddress", ResourceType = typeof(MUI))]
		public string Address { get; set; }
    }
}