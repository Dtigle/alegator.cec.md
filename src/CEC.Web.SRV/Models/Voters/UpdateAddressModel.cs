using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
	public class UpdateAddressModel
    {
		public long Id { get; set; }

		[Display(Name = "Personal_Data", ResourceType = typeof(MUI))]
		public PersonModel PersonInfo { get; set; }

		[Display(Name = "AddressOfResidence", ResourceType = typeof(MUI))]
		public PersonAddressModel BaseAddressInfo { get; set; }

		[Display(Name = "DeclaredAddress", ResourceType = typeof(MUI))]
		public PersonAddressModel DeclaredStayAddressInfo { get; set; }
    }
}