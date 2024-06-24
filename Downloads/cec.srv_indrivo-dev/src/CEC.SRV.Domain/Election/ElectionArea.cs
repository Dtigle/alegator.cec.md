using System.ComponentModel;

namespace CEC.SRV.Domain.Lookup
{
    public enum ElectionArea
    {
        [Description("Alegeri locale")]
        Local = 1,

        [Description("Alegeri generale")]
        General = 2
    }
}
