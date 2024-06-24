using System;

namespace CEC.SAISE.EDayModule.Infrastructure.Grids
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FilterValueAttribute : Attribute
    {
        public object Value { get; set; }
    }
}