using System.ComponentModel;

namespace CEC.SRV.Domain
{
    public enum BuildingTypes
    {
		[Description("Blocuri")]
		Undefined = 0,
		[Description("Locativ")]
        Apartment = 1,
        [Description("Administrativ")]
        Administrative = 2
    }
}