using System.ComponentModel;

namespace CEC.SRV.Domain.Lookup
{
    public enum ElectionStatus
    {
        [Description("Nou")]
        New = 1,

        [Description("Finalizat")]
        Finished = 2
    }
}
