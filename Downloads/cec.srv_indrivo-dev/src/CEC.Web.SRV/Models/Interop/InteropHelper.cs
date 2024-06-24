using System;
using System.Collections.Generic;
using System.Linq;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Interop;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.Interop
{
    public class InteropHelper
    {

        public static Dictionary<string, string> GetTransactionProcessingTypes()
        {
            var result = new Dictionary<string, string>();

            result.Add("", MUI.PleaseSelect);

            var enumValues = Enum.GetValues(typeof(TransactionProcessingTypes)).Cast<TransactionProcessingTypes>();
            foreach (var enumValue in enumValues)
            {
                result.Add(enumValue.GetFilterValue().ToString(), enumValue.GetEnumDescription());
            }

            return result;
        }
        public static Dictionary<string, string> GetTransactionStatuses()
        {
            var result = new Dictionary<string, string>();

            result.Add("", MUI.PleaseSelect);
            
            var enumValues = Enum.GetValues(typeof(TransactionStatus)).Cast<TransactionStatus>();
            foreach (var enumValue in enumValues)
            {
                result.Add(enumValue.GetFilterValue().ToString(), enumValue.GetEnumDescription());
            }

            return result;
        }

    }
}