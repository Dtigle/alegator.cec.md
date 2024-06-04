
using System;
using System.ComponentModel.DataAnnotations;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Voters
{
	public class RspInfoModel
    {
		[Display(Name = "RspInfo_Idnp", ResourceType = typeof(MUI))]
		public string Idnp { get; set; }

		[Display(Name = "RspInfo_BirthDate", ResourceType = typeof(MUI))]
		public string BirthDate { get; set; }

		[Display(Name = "RspInfo_CitizenRm", ResourceType = typeof(MUI))]
		public string CitizenRm { get; set; }

		[Display(Name = "RspInfo_Dead", ResourceType = typeof(MUI))]
		public string Dead { get; set; }

		[Display(Name = "RspInfo_FirstName", ResourceType = typeof(MUI))]
		public string FirstName { get; set; }

		[Display(Name = "RspInfo_DocType", ResourceType = typeof(MUI))]
		public string DocType { get; set; }

		[Display(Name = "RspInfo_ExpirationDate", ResourceType = typeof(MUI))]
		public string ExpirationDate { get; set; }

		[Display(Name = "RspInfo_IssueDate", ResourceType = typeof(MUI))]
		public string IssueDate { get; set; }

		[Display(Name = "RspInfo_IssueLocation", ResourceType = typeof(MUI))]
		public string IssueLocation { get; set; }

		[Display(Name = "RspInfo_Number", ResourceType = typeof(MUI))]
		public string Number { get; set; }

		[Display(Name = "RspInfo_Series", ResourceType = typeof(MUI))]
		public string Series { get; set; }

		[Display(Name = "RspInfo_Validity", ResourceType = typeof(MUI))]
		public string Validity { get; set; }

		[Display(Name = "RspInfo_LastName", ResourceType = typeof(MUI))]
		public string LastName { get; set; }

		public RegistrationDataModel[] Registration { get; set; }

		[Display(Name = "RspInfo_SecondName", ResourceType = typeof(MUI))]
		public string SecondName{ get; set; }

		[Display(Name = "RspInfo_Sex", ResourceType = typeof(MUI))]
		public string Sex { get; set; }
    }
}