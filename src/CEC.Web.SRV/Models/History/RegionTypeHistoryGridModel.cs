using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Resources;

namespace CEC.Web.SRV.Models.History
{
    public class RegionTypeHistoryGridModel : LookupHistoryGridModel
    {
        [Display(Name = "RegionTypesLookup_Rank", ResourceType = typeof(MUI))]
        public int Rank { get; set; }
    }
}