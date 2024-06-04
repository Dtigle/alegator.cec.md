using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Infrastructure.Grids;

namespace CEC.Web.SRV.Infrastructure
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = null;
            if (fi != null)
            {
                attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);
            }
            else
            {
                return string.Empty;
            }
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static object GetFilterValue(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (FilterValueAttribute[])fi.GetCustomAttributes(
                typeof(FilterValueAttribute),
                false);

            return attributes.Length > 0 ? attributes[0].Value : value;
        }

        public static string GetEnumFullName(this Enum value)
        {
            return string.Format("{0}.{1}", value.GetType().Name, value);
        }

        public static Dictionary<string, string> GetValues(Type enumType)
        {
            var result = new Dictionary<string, string>();

            var enumValues = Enum.GetValues(enumType).Cast<Enum>();
            foreach (Enum enumValue in enumValues)
            {
                result.Add(enumValue.GetFilterValue().ToString(), enumValue.GetEnumDescription());
            }

            return result;
        }
    }
}