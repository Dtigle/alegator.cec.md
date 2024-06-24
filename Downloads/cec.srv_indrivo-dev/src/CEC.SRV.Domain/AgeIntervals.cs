
using System.ComponentModel;

namespace CEC.SRV.Domain
{
    public enum AgeIntervals
    {
        [Description("mai mic de 18 ani")]
        AgeLessThen18 = 0,
        [Description("18-40 ani")]
        AgeBetween18And40 = 1,
        [Description("41-65 ani")]
        AgeBetween41And65 = 2,
        [Description("66-75 ani")]
        AgeBetween66And75 = 3,
        [Description("76-90 ani")]
        AgeBetween76And90 = 4,
        [Description("mai mare de 90 ani")]
        AgeGreaterThen90 = 5,

    }
    
}