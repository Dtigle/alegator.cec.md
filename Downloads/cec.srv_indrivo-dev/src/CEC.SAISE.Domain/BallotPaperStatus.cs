using System.ComponentModel;

namespace CEC.SAISE.Domain
{
    public enum BallotPaperStatus
    {
        [Description("Nou")]
        New = 0,

        [Description("Așteptarea aprobării")]
        WaitingForApproval = 1,

        [Description("Aprobat")]
        Approved = 2,

        [Description("Informație lipsă")]
        MissingInformation = 3,

        [Description("Respins")]
        Rejected = 4,

        [Description("Inactiv")]
        Inactive = 5,
    }
}