using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CEC.Web.SRV.Infrastructure;
using CEC.Web.SRV.Infrastructure.Grids;
using CEC.Web.SRV.Models.History;
using CEC.Web.SRV.Resources;
using Lib.Web.Mvc.JQuery.JqGrid;
using Lib.Web.Mvc.JQuery.JqGrid.Constants;
using Lib.Web.Mvc.JQuery.JqGrid.DataAnnotations;

namespace CEC.Web.SRV.Models.History
{
    public class RegionHistoryGridModel : LookupHistoryGridModel
	{
        //[Display(Name = "RegionLookup_Parent", ResourceType = typeof(MUI))]
        //public string Parent { get; set; }

        [Display(Name = "RegionLookup_RegionType", ResourceType = typeof(MUI))]
		public string RegionType { get; set; }

        [Display(Name = "RegionLookup_HasStreets", ResourceType = typeof(MUI))]
		public bool HasStreets { get; set; }

        [Display(Name = "SAISE ID")]
		public long? SaiseId { get; set; }

        [Display(Name = "Registru ID")]
		public long? RegistruId { get; set; }

        [Display(Name = "RegionLookup_HasChildren", ResourceType = typeof(MUI))]
		public bool HasChildren { get; set; }

		[Display(Name = "Circumscription_Number", ResourceType = typeof(MUI))]
		public int? Circumscription { get; set; }
	}
}