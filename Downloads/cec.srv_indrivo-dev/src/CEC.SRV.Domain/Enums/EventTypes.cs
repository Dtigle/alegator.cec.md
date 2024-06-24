using System.ComponentModel;

namespace CEC.SRV.Domain
{
    public enum EventTypes
    {
		[Description("Creare")]
        New = 1,
        [Description("Modificare")]
        Update = 2,
        [Description("Ștergere")]
        Delete = 3
    }
}