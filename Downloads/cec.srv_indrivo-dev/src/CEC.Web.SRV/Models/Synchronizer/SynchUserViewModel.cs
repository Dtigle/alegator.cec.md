using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Models.Synchronizer
{
	public class SynchUserViewModel
	{
		public string Id { get; set; }
		public string RoleId { get; set; }
		public string Username { get; set; }
		public string OldUsername { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Notes { get; set; }
		public string Phone { get; set; }
		public string Details { get; set; }
		public bool LoginActiveDirectory { get; set; }
		public bool LoginLocalDatabase { get; set; }
		public bool ChangePassword { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
		public long? PasswordPolicyId { get; set; }
		public bool LoginMPass { get; set; }
		public string Idnp { get; set; }
		public int Status { get; set; }
		public int Gender { get; set; }
		public DateTime? Created { get; set; }
		public DateTime? BirthDate { get; set; }
		public IEnumerable<long> Regions { get; set; }

	}
}