using System.ComponentModel;

namespace CEC.SRV.Domain.Lookup
{
    public enum ElectionRoundStatus
    {
        [Description("Nou")]
        New = 1,

        [Description("Aprobat")]
        Aproved = 2,
        
        [Description("Cu rezultate")]
        WithResults = 3,
        
        [Description("Validat")]
        Validated = 4,
    }
}
