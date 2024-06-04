using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEC.SRV.Domain;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Infrastructure
{
    public class SearchableColumnsHelper
    {
        public static Dictionary<string, string> GetBooleanSelect()
        {
            return new Dictionary<string, string>
            {
                {"", MUI.PleaseSelect},    
                {bool.TrueString, MUI.Yes},
                {bool.FalseString, MUI.No}
            };
        }

        public static Dictionary<string, string> GetPollingStationTypes()
        {
            var result = new Dictionary<string, string>();

            //if (insertPrompt)
            {
                result.Add("", MUI.PleaseSelect);
            }

            var enumValues = Enum.GetValues(typeof(PollingStationTypes)).Cast<PollingStationTypes>();
            foreach (var enumValue in enumValues)
            {
                result.Add(enumValue.GetFilterValue().ToString(), enumValue.GetEnumDescription());
            }

            return result;
        }
    }
}