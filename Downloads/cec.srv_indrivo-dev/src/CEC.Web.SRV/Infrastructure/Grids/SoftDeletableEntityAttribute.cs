using System;

namespace CEC.Web.SRV.Infrastructure.Grids
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SoftDeletableEntityAttribute : Attribute
    {
        public Type Type { get; set; }
    }
}