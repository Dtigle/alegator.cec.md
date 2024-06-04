using System;
using System.ComponentModel.DataAnnotations;
using CEC.SRV.Domain;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
	public class PersonInfo
	{
		public long Id { get; set; }

		[Display(Name = "Voters_Name", ResourceType = typeof(MUI))]
		public string FullName { get; set; }

		[Display(Name = "Voters_PrimaryDocument", ResourceType = typeof(MUI))]
		public string PrimaryDocument { get; set; }

		[Display(Name = "Voters_Idnp", ResourceType = typeof(MUI))]
		public string Idnp { get; set; }

		[Display(Name = "Voters_Address", ResourceType = typeof(MUI))]
		public string Address { get; set; }
    }
}