using System;
using System.ComponentModel;

namespace CEC.SRV.Domain.Interop
{
    public enum TransactionProcessingTypes
    {
        [Description("Automat")]
        Automatic = 0,

        [Description("Manual")]
        Manual = 1
    }
}