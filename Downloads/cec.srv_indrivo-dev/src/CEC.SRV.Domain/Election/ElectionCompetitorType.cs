using System.ComponentModel;

namespace CEC.SRV.Domain.Lookup
{
    public enum ElectionCompetitorType
    {
        [Description("Candidaţi")]
        Candidate = 1,

        [Description("Partide politice")]
        PoliticalParty = 2,

        [Description("Întrebări referendum")]
        Question = 3
    }
}
