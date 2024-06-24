using System;

namespace CEC.Web.SRV.Infrastructure.Grids
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchDataAttribute : Attribute
    {
        public Type Type { get; set; }

        public string DbName { get; set; }
    }
}