using System.ComponentModel;
using CEC.Web.SRV.Infrastructure.Grids;

namespace CEC.Web.SRV.Models.Account
{
    public enum AccountStatus
    {
		[Description("Activ")]
        [FilterValue(Value = false)]
        Active = 1,
        [Description("Blocat")]
        [FilterValue(Value = true)]
        Blocked
    }
}