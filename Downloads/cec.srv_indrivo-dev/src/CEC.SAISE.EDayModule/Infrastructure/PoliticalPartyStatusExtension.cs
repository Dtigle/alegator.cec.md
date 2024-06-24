using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace CEC.SAISE.EDayModule.Infrastructure
{
    public static class PoliticalPartyStatusExtension
    {
        public static KeyValuePair<int, string>[] GetValuesAsArray<T>()
        {
            return
                Enum.GetValues(typeof(T))
                    .Cast<T>()
                    .Select(x => new KeyValuePair<int, string>((int)Convert.ChangeType(x, TypeCode.Int32), GetEnumDescription(x)))
                    .ToArray();

        }

        private static string GetEnumDescription<T>(T value)
        {
            var fi = typeof(T).GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}