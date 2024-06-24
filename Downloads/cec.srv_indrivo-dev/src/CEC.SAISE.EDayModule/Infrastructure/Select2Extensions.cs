using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SRV.BLL.Dto;

namespace CEC.SAISE.EDayModule.Infrastructure
{
    public static class Select2Extensions
    {
        public static IEnumerable<Select2Item> ToSelectSelect2List<T>(
           this IEnumerable<T> source,
           Func<T, long> valueFunc,
           Func<T, string> textFieldFunc)
        {
            return source.Select(x => new Select2Item { id = valueFunc(x), text = textFieldFunc(x) });
        }
    }
}