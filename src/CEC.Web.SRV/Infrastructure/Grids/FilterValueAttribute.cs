using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEC.Web.SRV.Infrastructure.Grids
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FilterValueAttribute : Attribute
    {
        public object Value { get; set; }
    }
}