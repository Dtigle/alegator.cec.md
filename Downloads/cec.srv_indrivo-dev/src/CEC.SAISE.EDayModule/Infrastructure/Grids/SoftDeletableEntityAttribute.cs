using System;

namespace CEC.SAISE.EDayModule.Infrastructure.Grids
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SoftDeletableEntityAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}