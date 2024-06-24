using System.ComponentModel;

namespace CEC.SAISE.Domain
{
    public enum VillageType
    {
        [Description("Necunoscut")]
        Unknown = 0,

        [Description("Republica")]
        Republica = 1,

        [Description("r-n")]
        Region = 2,

        [Description("UTA")]
        UTA = 3,

        [Description("mun.")]
        MUN = 4,

        [Description("sector")]
        Sect = 5,

        [Description("or.")]
        Sity = 6,

        [Description("orășel")]
        SmallSity = 7,

        [Description("com.")]
        COM = 8,

        [Description("com.")]
        Village = 9,


    }
}