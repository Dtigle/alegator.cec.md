using System.ComponentModel;

namespace CEC.SRV.Domain.Interop
{
    public enum TransactionStatus
    {
        [Description("Nouă")]
        New = 1,

        [Description("Procesată cu succes")]
        Success = 2,

        [Description("Procesată cu eroare")]
        Error = 3,
    }
}