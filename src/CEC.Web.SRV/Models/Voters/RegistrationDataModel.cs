using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
	public class RegistrationDataModel
    {
		[Display(Name = "RspInfo_ExpirationDateAddress", ResourceType = typeof(MUI))]
		public string ExpirationDateAddress { get; set; }

		[Display(Name = "RspInfo_RegDate", ResourceType = typeof(MUI))]
		public string RegDate { get; set; }

		public int? RegTypeCode { get; set; }

		[Display(Name = "RspInfo_RegType", ResourceType = typeof(MUI))]
		public string RegType { get; set; }

		[Display(Name = "RspInfo_Street", ResourceType = typeof(MUI))]
		public string Street { get; set; }

		[Display(Name = "RspInfo_StreetCode", ResourceType = typeof(MUI))]
		public int? StreetCode { get; set; }

		[Display(Name = "RspInfo_StreetAddress", ResourceType = typeof(MUI))]
		public string StreetAddress { get; set; }

		[Display(Name = "RspInfo_House", ResourceType = typeof(MUI))]
		public string House { get; set; }

		[Display(Name = "RspInfo_Block", ResourceType = typeof(MUI))]
		public string Block { get; set; }

		[Display(Name = "RspInfo_Flat", ResourceType = typeof(MUI))]
		public string Flat { get; set; }

		[Display(Name = "RspInfo_Locality", ResourceType = typeof(MUI))]
		public string Locality { get; set; }

		[Display(Name = "RspInfo_Region", ResourceType = typeof(MUI))]
		public string Region { get; set; }

		[Display(Name = "RspInfo_AdministrativCode", ResourceType = typeof(MUI))]
		public string AdministrativCode { get; set; }
    }
}