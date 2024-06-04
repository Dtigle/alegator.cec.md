using System.ComponentModel;

namespace CEC.SRV.Domain.Print
{
    public enum PrintStatus
    {
		[Description("In așteptare")]
        Pending = 0,
		[Description("In execuție")]
        InProgress = 1,
		[Description("Procesat")]
        Finished = 2,
		[Description("Anulat")]
        Canceled = 3,
		[Description("Eronat")]
        Failed = 4
    }
}