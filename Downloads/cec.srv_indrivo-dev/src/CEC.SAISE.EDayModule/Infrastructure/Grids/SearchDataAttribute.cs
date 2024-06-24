using System;

namespace CEC.SAISE.EDayModule.Infrastructure.Grids
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchDataAttribute : Attribute
    {
        public Type Type { get; set; }

        public string DbName { get; set; }
    }
}