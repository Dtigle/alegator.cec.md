using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.History
{
    public class LookupHistoryGridModel : HistoryGridRow
    {
        [Display(Name = "Lookups_Name", ResourceType = typeof(MUI))]
        public string Name { get; set; }

        [Display(Name = "Lookups_Description", ResourceType = typeof(MUI))]
        public string Description { get; set; }
    }
}